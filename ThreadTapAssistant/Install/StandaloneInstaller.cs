using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Win32;

namespace ThreadTapSetup
{
    public class InstallerForm : Form
    {
        private const string ADDIN_GUID = "{E5B26E24-8D29-4A7B-9E33-01D0C8D6C910}";

        private Panel pnlHeader;
        private Label lblTitle;
        private Label lblSubtitle;

        private Label lblPathTitle;
        private TextBox txtPath;
        private Button btnBrowse;

        private Label lblDesc;
        private ProgressBar progressBar;
        private Button btnInstall;
        private Label lblStatus;

        public InstallerForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "SolidWorks 螺纹底孔小径助手 - 安装程序";
            this.Size = new Size(520, 360);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = true;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            this.BackColor = Color.FromArgb(248, 249, 250);

            // 1. 顶部横幅
            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(24, 144, 255),
                Padding = new Padding(20, 15, 20, 15)
            };

            lblTitle = new Label
            {
                Text = "SolidWorks 螺纹底孔小径与尺寸联动助手",
                Font = new Font("Microsoft YaHei UI", 12.5F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 15)
            };
            pnlHeader.Controls.Add(lblTitle);

            lblSubtitle = new Label
            {
                Text = "全球11国标准 · 纯选择防呆 · A~ZC / IT01~IT18 全公差 · 原生集成",
                Font = new Font("Microsoft YaHei UI", 8.5F),
                ForeColor = Color.FromArgb(230, 247, 255),
                AutoSize = true,
                Location = new Point(22, 45)
            };
            pnlHeader.Controls.Add(lblSubtitle);
            this.Controls.Add(pnlHeader);

            // 2. 安装目录设置
            lblPathTitle = new Label
            {
                Text = "安装目标文件夹 (建议保持默认):",
                Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                Location = new Point(25, 105),
                Size = new Size(300, 20)
            };
            this.Controls.Add(lblPathTitle);

            string defaultDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                "SolidWorks_ThreadTapAssistant"
            );

            txtPath = new TextBox
            {
                Text = defaultDir,
                Location = new Point(25, 130),
                Size = new Size(370, 26),
                ReadOnly = false
            };
            this.Controls.Add(txtPath);

            btnBrowse = new Button
            {
                Text = "浏览...",
                Location = new Point(405, 128),
                Size = new Size(80, 28),
                BackColor = Color.White,
                FlatStyle = FlatStyle.System
            };
            btnBrowse.Click += BtnBrowse_Click;
            this.Controls.Add(btnBrowse);

            // 3. 说明与进度
            lblDesc = new Label
            {
                Text = "安装程序将自动解压插件库文件，并向当前计算机系统的 SolidWorks 注册 COM 插件组件。无需手动配置路径或宏脚本。",
                ForeColor = Color.FromArgb(108, 117, 125),
                Location = new Point(25, 170),
                Size = new Size(460, 36)
            };
            this.Controls.Add(lblDesc);

            progressBar = new ProgressBar
            {
                Location = new Point(25, 215),
                Size = new Size(460, 12),
                Visible = false
            };
            this.Controls.Add(progressBar);

            lblStatus = new Label
            {
                Text = "就绪。点击下方按钮开始一键安装。",
                ForeColor = Color.FromArgb(108, 117, 125),
                Location = new Point(25, 235),
                Size = new Size(460, 20)
            };
            this.Controls.Add(lblStatus);

            // 4. 底部安装按钮
            btnInstall = new Button
            {
                Text = "🚀 一键安装到 SolidWorks",
                Font = new Font("Microsoft YaHei UI", 10.5F, FontStyle.Bold),
                BackColor = Color.FromArgb(24, 144, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(140, 265),
                Size = new Size(230, 42),
                Cursor = Cursors.Hand
            };
            btnInstall.FlatAppearance.BorderSize = 0;
            btnInstall.Click += BtnInstall_Click;
            this.Controls.Add(btnInstall);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "请选择插件安装目录";
                fbd.SelectedPath = txtPath.Text;
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    txtPath.Text = fbd.SelectedPath;
                }
            }
        }

        private void BtnInstall_Click(object sender, EventArgs e)
        {
            string targetDir = txtPath.Text.Trim();
            if (string.IsNullOrEmpty(targetDir))
            {
                MessageBox.Show("请指定有效的安装路径！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 检查 SolidWorks 是否正在运行
            var swProcs = Process.GetProcessesByName("SLDWORKS");
            if (swProcs.Length > 0)
            {
                var dr = MessageBox.Show(
                    "检测到 SolidWorks 正在运行！\n\n为确保插件顺利写入并加载，强烈建议您先保存模型并【退出 SolidWorks】。\n\n是否仍要继续安装？",
                    "SolidWorks 运行中提示",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );
                if (dr == DialogResult.No) return;
            }

            btnInstall.Enabled = false;
            btnBrowse.Enabled = false;
            txtPath.Enabled = false;
            progressBar.Visible = true;
            progressBar.Style = ProgressBarStyle.Marquee;

            try
            {
                SetStatus("正在创建安装目录并解压组件文件...");

                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }

                // 从嵌入资源中提取 payload.zip
                ExtractPayload(targetDir);

                SetStatus("正在向系统与 SolidWorks 注册插件组件...");

                // 执行 64位 COM 注册
                string dllPath = Path.Combine(targetDir, "ThreadTapAssistant.dll");
                RegisterComDll(dllPath);

                // 写入 SolidWorks 注册表项
                RegisterSolidWorksKeys(targetDir);

                // 创建独立卸载程序与控制面板入口
                CreateUninstaller(targetDir);

                progressBar.Visible = false;
                SetStatus("安装成功！");

                MessageBox.Show(
                    "🎉 【SolidWorks 螺纹底孔小径与尺寸助手】已成功安装并注册！\n\n" +
                    "使用方法：\n" +
                    "1. 启动 SolidWorks；\n" +
                    "2. 点击顶部菜单栏【工具】 -> 【插件】 (Tools -> Add-Ins)；\n" +
                    "3. 勾选【螺纹底孔小径助手】两边的复选框；\n" +
                    "4. 在 SolidWorks 最右侧的任务窗格中点击螺纹图标即可展开使用！",
                    "安装完成",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                this.Close();
            }
            catch (Exception ex)
            {
                progressBar.Visible = false;
                btnInstall.Enabled = true;
                btnBrowse.Enabled = true;
                txtPath.Enabled = true;
                SetStatus("安装失败: " + ex.Message);
                MessageBox.Show("安装过程中发生错误:\n" + ex.Message, "安装失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExtractPayload(string targetDir)
        {
            var asm = Assembly.GetExecutingAssembly();
            using (var stream = asm.GetManifestResourceStream("Payload.zip"))
            {
                if (stream == null)
                {
                    throw new Exception("未找到内置安装资源文件 Payload.zip！");
                }

                using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
                {
                    foreach (var entry in archive.Entries)
                    {
                        string destinationPath = Path.GetFullPath(Path.Combine(targetDir, entry.FullName));
                        if (entry.FullName.EndsWith("/") || entry.FullName.EndsWith("\\"))
                        {
                            Directory.CreateDirectory(destinationPath);
                            continue;
                        }

                        string parentDir = Path.GetDirectoryName(destinationPath);
                        if (!Directory.Exists(parentDir))
                        {
                            Directory.CreateDirectory(parentDir);
                        }

                        entry.ExtractToFile(destinationPath, true);
                    }
                }
            }
        }

        private void RegisterComDll(string dllPath)
        {
            string windir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            string regasm = Path.Combine(windir, @"Microsoft.NET\Framework64\v4.0.30319\regasm.exe");

            if (!File.Exists(regasm))
            {
                throw new Exception("未在系统中检测到 64位 .NET Framework 运行库 (v4.0.30319)！");
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
                proc.WaitForExit();
                if (proc.ExitCode != 0)
                {
                    string err = proc.StandardError.ReadToEnd();
                    throw new Exception("COM 注册异常: " + err);
                }
            }
        }

        private void RegisterSolidWorksKeys(string targetDir)
        {
            // 写入 HKCU 与 HKLM
            RegisterKeysToRoot(Registry.CurrentUser);
            try { RegisterKeysToRoot(Registry.LocalMachine); } catch { }
        }

        private void RegisterKeysToRoot(RegistryKey root)
        {
            using (var key = root.CreateSubKey($@"Software\SolidWorks\AddIns\{ADDIN_GUID}"))
            {
                if (key != null)
                {
                    key.SetValue(null, 1);
                    key.SetValue("Description", "螺纹底孔小径快速计算与草图尺寸联动助手");
                    key.SetValue("Title", "螺纹底孔小径助手");
                }
            }

            using (var startupKey = root.CreateSubKey($@"Software\SolidWorks\AddInsStartup\{ADDIN_GUID}"))
            {
                if (startupKey != null)
                {
                    startupKey.SetValue(null, 1);
                }
            }
        }

        private void CreateUninstaller(string targetDir)
        {
            try
            {
                // 创建卸载脚本 Uninstall.bat
                string uninstallerBat = Path.Combine(targetDir, "Uninstall.bat");
                string batContent = "@echo off\r\n" +
                    "setlocal\r\n" +
                    $"\"%SystemRoot%\\Microsoft.NET\\Framework64\\v4.0.30319\\regasm.exe\" /u \"{targetDir}\\ThreadTapAssistant.dll\"\r\n" +
                    "reg delete \"HKCU\\Software\\SolidWorks\\AddIns\\" + ADDIN_GUID + "\" /f >nul 2>&1\r\n" +
                    "reg delete \"HKCU\\Software\\SolidWorks\\AddInsStartup\\" + ADDIN_GUID + "\" /f >nul 2>&1\r\n" +
                    "reg delete \"HKLM\\Software\\SolidWorks\\AddIns\\" + ADDIN_GUID + "\" /f >nul 2>&1\r\n" +
                    "reg delete \"HKLM\\Software\\SolidWorks\\AddInsStartup\\" + ADDIN_GUID + "\" /f >nul 2>&1\r\n" +
                    "reg delete \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\ThreadTapAssistant\" /f >nul 2>&1\r\n" +
                    "echo [完成] 螺纹底孔助手已成功注销卸载！\r\n" +
                    "pause\r\n";

                File.WriteAllText(uninstallerBat, batContent, System.Text.Encoding.Default);

                // 注册到 Windows 控制面板应用列表
                using (var unKey = Registry.LocalMachine.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall\ThreadTapAssistant"))
                {
                    if (unKey != null)
                    {
                        unKey.SetValue("DisplayName", "SolidWorks 螺纹底孔小径与尺寸联动助手");
                        unKey.SetValue("UninstallString", $"\"{uninstallerBat}\"");
                        unKey.SetValue("DisplayIcon", Path.Combine(targetDir, "ThreadTapAssistant.dll"));
                        unKey.SetValue("Publisher", "SolidWorks Add-in");
                        unKey.SetValue("DisplayVersion", "1.0.0");
                    }
                }
            }
            catch { }
        }

        private void SetStatus(string msg)
        {
            lblStatus.Text = msg;
            Application.DoEvents();
        }

        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new InstallerForm());
        }
    }
}
