using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace ThreadTapSetup
{
    static class UninstallProgram
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

                string windir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
                string regasm = Path.Combine(windir, @"Microsoft.NET\Framework64\v4.0.30319\regasm.exe");

                if (!File.Exists(regasm))
                {
                    MessageBox.Show("未在系统中检测到 64位 .NET Framework 运行环境！", "系统环境错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var psi = new ProcessStartInfo
                {
                    FileName = regasm,
                    Arguments = "/u \"" + dllPath + "\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (var proc = Process.Start(psi))
                {
                    proc.WaitForExit();
                    MessageBox.Show("【螺纹底孔小径助手】已成功从系统中注销卸载！", "卸载完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("卸载过程中发生异常：\n" + ex.Message, "卸载异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
