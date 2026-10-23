using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swpublished;

namespace ThreadTapAssistant
{
    [Guid("E5B26E24-8D29-4A7B-9E33-01D0C8D6C910")]
    [ComVisible(true)]
    [DisplayName("螺纹底孔小径助手 (Thread Tap Assistant)")]
    [Description("快速查询机械螺纹标准底孔与理论小径，一键修改SolidWorks草图尺寸")]
    public class SwAddin : ISwAddin
    {
        private ISldWorks _swApp;
        private int _addinCookie;
        private ITaskpaneView _taskpaneView;
        private ThreadTapTaskPaneControl _taskpaneControl;

        #region ISwAddin Implementation

        public bool ConnectToSW(object ThisSW, int cookie)
        {
            try
            {
                _swApp = (ISldWorks)ThisSW;
                _addinCookie = cookie;

                // 注册插件回调信息
                _swApp.SetAddinCallbackInfo2(0, this, cookie);

                // 创建右侧任务窗格面板
                CreateTaskPane();

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ConnectToSW 异常: " + ex.Message);
                return false;
            }
        }

        public bool DisconnectFromSW()
        {
            try
            {
                RemoveTaskPane();

                _taskpaneControl = null;
                _swApp = null;

                // 强制垃圾回收
                GC.Collect();
                GC.WaitForPendingFinalizers();

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("DisconnectFromSW 异常: " + ex.Message);
                return false;
            }
        }

        #endregion

        #region TaskPane Management

        private void CreateTaskPane()
        {
            try
            {
                string assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string iconPath = Path.Combine(assemblyDir, "Resources", "icon20.bmp");

                if (!File.Exists(iconPath))
                {
                    iconPath = ""; // 若无图标，SW 将使用默认插件图标
                }

                // 创建右侧任务窗格视图
                _taskpaneView = _swApp.CreateTaskpaneView2(iconPath, "螺纹底孔助手");

                if (_taskpaneView != null)
                {
                    // 将自定义 UserControl 控件嵌入任务窗格
                    _taskpaneControl = (ThreadTapTaskPaneControl)_taskpaneView.AddControl(
                        ThreadTapTaskPaneControl.PROG_ID,
                        string.Empty
                    );

                    if (_taskpaneControl != null)
                    {
                        _taskpaneControl.Initialize(_swApp);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("CreateTaskPane 异常: " + ex.Message);
            }
        }

        private void RemoveTaskPane()
        {
            try
            {
                if (_taskpaneView != null)
                {
                    _taskpaneView.DeleteView();
                    Marshal.ReleaseComObject(_taskpaneView);
                    _taskpaneView = null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("RemoveTaskPane 异常: " + ex.Message);
            }
        }

        #endregion

        #region COM Registration Functions (RegAsm 自动触发)

        [ComRegisterFunction]
        public static void RegisterFunction(Type t)
        {
            try
            {
                string addinGuid = t.GUID.ToString("B").ToUpper();

                // 优先注册到当前用户 HKCU（无需管理员权限也能被 SolidWorks 识别）
                RegisterAddinKey(Registry.CurrentUser, addinGuid);

                // 同时尝试注册到本机 HKLM（如果是管理员运行安装包）
                try
                {
                    RegisterAddinKey(Registry.LocalMachine, addinGuid);
                }
                catch
                {
                    // 忽略无管理员权限时的 HKLM 写入异常
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("COM 注册异常: " + ex.Message);
            }
        }

        private static void RegisterAddinKey(RegistryKey rootKey, string addinGuid)
        {
            using (var key = rootKey.CreateSubKey($@"Software\SolidWorks\AddIns\{addinGuid}"))
            {
                if (key != null)
                {
                    key.SetValue(null, 1); // 1 = 默认加载启用
                    key.SetValue("Description", "螺纹底孔小径快速计算与草图尺寸联动助手");
                    key.SetValue("Title", "螺纹底孔小径助手");
                }
            }

            using (var startupKey = rootKey.CreateSubKey($@"Software\SolidWorks\AddInsStartup\{addinGuid}"))
            {
                if (startupKey != null)
                {
                    startupKey.SetValue(null, 1); // 随 SolidWorks 启动自动加载
                }
            }
        }

        [ComUnregisterFunction]
        public static void UnregisterFunction(Type t)
        {
            try
            {
                string addinGuid = t.GUID.ToString("B").ToUpper();
                UnregisterAddinKey(Registry.CurrentUser, addinGuid);

                try
                {
                    UnregisterAddinKey(Registry.LocalMachine, addinGuid);
                }
                catch { }
            }
            catch { }
        }

        private static void UnregisterAddinKey(RegistryKey rootKey, string addinGuid)
        {
            try { rootKey.DeleteSubKeyTree($@"Software\SolidWorks\AddIns\{addinGuid}", false); } catch { }
            try { rootKey.DeleteSubKeyTree($@"Software\SolidWorks\AddInsStartup\{addinGuid}", false); } catch { }
        }

        #endregion
    }
}
