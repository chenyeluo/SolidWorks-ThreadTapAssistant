using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace ThreadTapSetup
{
    static class SetupProgram
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string dllPath = Path.Combine(baseDir, "ThreadTapAssistant.dll");

                // 如果存在更新的编译文件，自动同步到 Release 目录
                string objDll = Path.GetFullPath(Path.Combine(baseDir, @"..\..\obj\Release\ThreadTapAssistant.dll"));
                if (File.Exists(objDll))
                {
                    try
                    {
                        File.Copy(objDll, dllPath, true);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            "检测到 SolidWorks 正在运行并占用了插件文件！\n\n请先保存模型并【关闭退出 SolidWorks】，然后再运行此安装程序更新！\n\n占用提示: " + ex.Message,
                            "请先关闭 SolidWorks",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                        return;
                    }
                }

                if (!File.Exists(dllPath))
                {
                    MessageBox.Show(
                        "未在当前目录下找到 ThreadTapAssistant.dll 文件！\n\n请确保安装程序解压后与 dll 处于同一文件夹。",
                        "文件缺失",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }

                string windir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
                string regasm = Path.Combine(windir, @"Microsoft.NET\Framework64\v4.0.30319\regasm.exe");

                if (!File.Exists(regasm))
                {
                    MessageBox.Show(
                        "未在系统中检测到 64位 .NET Framework 运行环境 (v4.0.30319)！\n路径: " + regasm,
                        "系统环境错误",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }

                var psi = new ProcessStartInfo
                {
                    FileName = regasm,
                    Arguments = "\"" + dllPath + "\" /codebase /tlb",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (var proc = Process.Start(psi))
                {
                    string output = proc.StandardOutput.ReadToEnd();
                    string err = proc.StandardError.ReadToEnd();
                    proc.WaitForExit();

                    if (proc.ExitCode == 0)
                    {
                        MessageBox.Show(
                            "🎉 【螺纹底孔小径助手】已成功安装并注册到 SolidWorks！\n\n" +
                            "使用指南：\n" +
                            "1. 打开 SolidWorks；\n" +
                            "2. 点击菜单栏【工具】 -> 【插件】 (Tools -> Add-Ins)；\n" +
                            "3. 找到【螺纹底孔小径助手】，勾选左侧（活动）与右侧（随SW启动）；\n" +
                            "4. 在 SolidWorks 界面右侧【任务窗格】中点击螺纹图标即可展开使用！",
                            "安装成功",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                    else
                    {
                        MessageBox.Show(
                            "插件注册未完成，返回信息：\n" + output + "\n" + err,
                            "注册提示",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("安装过程中发生异常：\n" + ex.Message, "安装异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
