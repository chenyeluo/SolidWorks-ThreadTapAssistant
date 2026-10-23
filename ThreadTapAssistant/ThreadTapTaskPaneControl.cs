using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SolidWorks.Interop.sldworks;
using ThreadTapAssistant.Helpers;
using ThreadTapAssistant.Models;

namespace ThreadTapAssistant
{
    [ComVisible(true)]
    [ProgId(PROG_ID)]
    [Guid("7F2A28F0-55DF-45E9-8239-0AC9B0E1B001")]
    public class ThreadTapTaskPaneControl : UserControl
    {
        public const string PROG_ID = "ThreadTapAssistant.TaskpaneView";

        private ISldWorks _swApp;

        // UI 布局容器
        private TableLayoutPanel mainLayout;

        private Label lblHeader;
        private Label lblSubtitle;
        private Label sep1;

        // 四级级联控件
        private Label lblStdTitle;
        private ComboBox cmbStandard;

        private Label lblTypeTitle;
        private ComboBox cmbThreadType;

        private Label lblSpecTitle;
        private ComboBox cmbSpec;

        // 公差等级区域 (支持常用预设 + A~ZC / IT01~IT18 全量计算)
        private Label lblTolTitle;
        private FlowLayoutPanel pnlTolMode;
        private RadioButton rbTolPreset;
        private RadioButton rbTolFull;

        private ComboBox cmbTolerance;

        // 全量 A~ZC × IT01~IT18 面板
        private TableLayoutPanel pnlFullTol;
        private Label lblDevTitle;
        private ComboBox cmbDeviation;
        private Label lblITTitle;
        private ComboBox cmbITGrade;

        // 结果卡片控件
        private Panel pnlResultCard;
        private TableLayoutPanel cardLayout;
        private Label lblCardTitle;
        private Label lblResultDrill;
        private Label lblTolBadge;
        private Label lblMinorRange;
        private Label lblPitchInfo;
        private Label lblMajorInfo;

        // 尺寸模式选择 (旋转直径 vs 半径)
        private GroupBox grpMode;
        private FlowLayoutPanel pnlRadios;
        private RadioButton rbDiameter;
        private RadioButton rbRadius;

        // 数值取值模式
        private Label lblValSource;
        private ComboBox cmbValueSource;

        // 操作按钮与提示
        private Button btnApply;
        private Button btnCopy;
        private Label lblStatus;
        private Button btnReloadData;

        // 当前全量公差计算结果缓存
        private ToleranceResult _currentFullTolResult;

        public ThreadTapTaskPaneControl()
        {
            InitializeComponent();
            LoadDataIntoUI();
        }

        public void Initialize(ISldWorks swApp)
        {
            _swApp = swApp;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.AutoScaleMode = AutoScaleMode.None;
            this.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.AutoScroll = true;
            this.Dock = DockStyle.Fill;

            mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 1,
                Padding = new Padding(10, 10, 10, 15),
                BackColor = Color.Transparent
            };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            // 1. 顶部标题区
            lblHeader = new Label
            {
                Text = "螺纹底孔与尺寸助手",
                Font = new Font("Microsoft YaHei UI", 11.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                AutoSize = true,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 0, 0, 3)
            };
            mainLayout.Controls.Add(lblHeader);

            lblSubtitle = new Label
            {
                Text = "11国标准 · 纯下拉选择 · A~ZC / IT01~IT18 全公差",
                Font = new Font("Microsoft YaHei UI", 8F),
                ForeColor = Color.FromArgb(108, 117, 125),
                AutoSize = true,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 0, 0, 8)
            };
            mainLayout.Controls.Add(lblSubtitle);

            sep1 = new Label
            {
                BorderStyle = BorderStyle.Fixed3D,
                Height = 2,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 0, 0, 10)
            };
            mainLayout.Controls.Add(sep1);

            // 2. ① 螺纹标准
            lblStdTitle = new Label
            {
                Text = "① 螺纹标准 (11国国际标准协议):",
                Font = new Font(this.Font, FontStyle.Bold),
                AutoSize = true,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 0, 0, 3)
            };
            mainLayout.Controls.Add(lblStdTitle);

            cmbStandard = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Dock = DockStyle.Top,
                Height = 26,
                Margin = new Padding(0, 0, 0, 10)
            };
            cmbStandard.SelectedIndexChanged += CmbStandard_SelectedIndexChanged;
            mainLayout.Controls.Add(cmbStandard);

            // 3. ② 螺纹种类与类型
            lblTypeTitle = new Label
            {
                Text = "② 螺纹种类与类型 (普通/管螺纹/传动):",
                Font = new Font(this.Font, FontStyle.Bold),
                AutoSize = true,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 0, 0, 3)
            };
            mainLayout.Controls.Add(lblTypeTitle);

            cmbThreadType = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Dock = DockStyle.Top,
                Height = 26,
                Margin = new Padding(0, 0, 0, 10)
            };
            cmbThreadType.SelectedIndexChanged += CmbThreadType_SelectedIndexChanged;
            mainLayout.Controls.Add(cmbThreadType);

            // 4. ③ 螺纹规格（纯选择）
            lblSpecTitle = new Label
            {
                Text = "③ 螺纹规格与牙距 (点击直接选择):",
                Font = new Font(this.Font, FontStyle.Bold),
                AutoSize = true,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 0, 0, 3)
            };
            mainLayout.Controls.Add(lblSpecTitle);

            cmbSpec = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Dock = DockStyle.Top,
                Height = 26,
                DropDownWidth = 280,
                Margin = new Padding(0, 0, 0, 10)
            };
            cmbSpec.SelectedIndexChanged += CmbSpec_SelectedIndexChanged;
            mainLayout.Controls.Add(cmbSpec);

            // 5. ④ 公差等级选择模式切换
            lblTolTitle = new Label
            {
                Text = "④ 公差等级 (绿=推荐首选, 黄=次选, 橙=不建议):",
                Font = new Font(this.Font, FontStyle.Bold),
                AutoSize = true,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 0, 0, 3)
            };
            mainLayout.Controls.Add(lblTolTitle);

            pnlTolMode = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0, 0, 0, 4)
            };
            rbTolPreset = new RadioButton { Text = "常用螺纹公差", Checked = true, AutoSize = true, Margin = new Padding(0, 0, 12, 0) };
            rbTolFull = new RadioButton { Text = "全量标准公差 (A~ZC × IT01~18)", AutoSize = true, Margin = new Padding(0) };
            pnlTolMode.Controls.Add(rbTolPreset);
            pnlTolMode.Controls.Add(rbTolFull);
            mainLayout.Controls.Add(pnlTolMode);

            // 常用公差下拉框
            cmbTolerance = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Dock = DockStyle.Top,
                Height = 28,
                DrawMode = DrawMode.OwnerDrawFixed,
                ItemHeight = 22,
                Margin = new Padding(0, 0, 0, 10)
            };
            cmbTolerance.DrawItem += CmbTolerance_DrawItem;
            cmbTolerance.SelectedIndexChanged += (s, e) => RefreshCurrentTolerance();
            mainLayout.Controls.Add(cmbTolerance);

            // 全量 A~ZC 与 IT01~IT18 网格面板 (左右两列选择)
            pnlFullTol = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 2,
                Visible = false,
                Margin = new Padding(0, 0, 0, 10)
            };
            pnlFullTol.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            pnlFullTol.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            lblDevTitle = new Label { Text = "偏差代号 (A~ZC):", AutoSize = true, Margin = new Padding(0, 0, 4, 2) };
            lblITTitle = new Label { Text = "公差等级 (IT01~18):", AutoSize = true, Margin = new Padding(4, 0, 0, 2) };
            pnlFullTol.Controls.Add(lblDevTitle, 0, 0);
            pnlFullTol.Controls.Add(lblITTitle, 1, 0);

            cmbDeviation = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Top, Height = 26, Margin = new Padding(0, 0, 4, 0) };
            foreach (var dev in ToleranceEngine.FundamentalDeviations) cmbDeviation.Items.Add(dev);
            cmbDeviation.SelectedItem = "H";
            cmbDeviation.SelectedIndexChanged += (s, e) => RefreshCurrentTolerance();

            cmbITGrade = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Top, Height = 26, Margin = new Padding(4, 0, 0, 0) };
            foreach (var it in ToleranceEngine.ITGrades) cmbITGrade.Items.Add(it);
            cmbITGrade.SelectedItem = "IT7";
            cmbITGrade.SelectedIndexChanged += (s, e) => RefreshCurrentTolerance();

            pnlFullTol.Controls.Add(cmbDeviation, 0, 1);
            pnlFullTol.Controls.Add(cmbITGrade, 1, 1);
            mainLayout.Controls.Add(pnlFullTol);

            rbTolPreset.CheckedChanged += (s, e) => {
                cmbTolerance.Visible = rbTolPreset.Checked;
                pnlFullTol.Visible = rbTolFull.Checked;
                RefreshCurrentTolerance();
            };

            // 6. 结果展示卡片 Panel
            pnlResultCard = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = Color.FromArgb(232, 240, 254),
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(0, 0, 0, 12),
                Padding = new Padding(8)
            };

            cardLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 1,
                BackColor = Color.Transparent
            };
            cardLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            lblCardTitle = new Label
            {
                Text = "推荐攻丝底孔直径 (Tap Drill):",
                Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(26, 115, 232),
                AutoSize = true,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 0, 0, 4)
            };
            cardLayout.Controls.Add(lblCardTitle);

            lblResultDrill = new Label
            {
                Text = "Ф 5.000 mm",
                Font = new Font("Microsoft YaHei UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(217, 48, 37),
                AutoSize = true,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 0, 0, 4)
            };
            cardLayout.Controls.Add(lblResultDrill);

            lblTolBadge = new Label
            {
                Text = "● 6H 【工程首选推荐】",
                Font = new Font("Microsoft YaHei UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(46, 125, 50),
                AutoSize = true,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 0, 0, 4)
            };
            cardLayout.Controls.Add(lblTolBadge);

            lblMinorRange = new Label
            {
                Text = "理论小径: 4.917 ~ 5.153 mm",
                Font = new Font("Microsoft YaHei UI", 8.5F),
                ForeColor = Color.FromArgb(60, 64, 67),
                AutoSize = true,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 0, 0, 3)
            };
            cardLayout.Controls.Add(lblMinorRange);

            lblPitchInfo = new Label
            {
                Text = "螺距/牙距: 1.00 mm",
                Font = new Font("Microsoft YaHei UI", 8.5F),
                ForeColor = Color.FromArgb(60, 64, 67),
                AutoSize = true,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 0, 0, 3)
            };
            cardLayout.Controls.Add(lblPitchInfo);

            lblMajorInfo = new Label
            {
                Text = "公称大径: 6.00 mm",
                Font = new Font("Microsoft YaHei UI", 8.5F),
                ForeColor = Color.FromArgb(60, 64, 67),
                AutoSize = true,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 0, 0, 2)
            };
            cardLayout.Controls.Add(lblMajorInfo);

            pnlResultCard.Controls.Add(cardLayout);
            mainLayout.Controls.Add(pnlResultCard);

            // 7. 写入尺寸模式
            grpMode = new GroupBox
            {
                Text = " 目标尺寸模式 ",
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Margin = new Padding(0, 0, 0, 10),
                Padding = new Padding(10, 24, 10, 8)
            };

            pnlRadios = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0)
            };

            rbDiameter = new RadioButton { Text = "旋转直径/孔径 (Φ)", Checked = true, AutoSize = true, Margin = new Padding(0, 0, 12, 0) };
            rbRadius = new RadioButton { Text = "半径 (r = Φ/2)", AutoSize = true, Margin = new Padding(0) };
            pnlRadios.Controls.Add(rbDiameter);
            pnlRadios.Controls.Add(rbRadius);
            grpMode.Controls.Add(pnlRadios);
            mainLayout.Controls.Add(grpMode);

            // 8. 数值取值模式
            lblValSource = new Label
            {
                Text = "应用数值类型:",
                AutoSize = true,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 0, 0, 3)
            };
            mainLayout.Controls.Add(lblValSource);

            cmbValueSource = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Dock = DockStyle.Top,
                Height = 26,
                Margin = new Padding(0, 0, 0, 12)
            };
            cmbValueSource.Items.Add("推荐标准攻丝底孔 (默认)");
            cmbValueSource.Items.Add("理论小径下限 (Min)");
            cmbValueSource.Items.Add("理论小径中值 (Mid)");
            cmbValueSource.Items.Add("理论小径上限 (Max)");
            cmbValueSource.SelectedIndex = 0;
            mainLayout.Controls.Add(cmbValueSource);

            // 9. 操作按钮
            btnApply = new Button
            {
                Text = "⚡ 一键应用到选定草图尺寸/圆",
                Font = new Font("Microsoft YaHei UI", 9.5F, FontStyle.Bold),
                BackColor = Color.FromArgb(24, 144, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.Top,
                Height = 42,
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 0, 8)
            };
            btnApply.FlatAppearance.BorderSize = 0;
            btnApply.Click += BtnApply_Click;
            mainLayout.Controls.Add(btnApply);

            btnCopy = new Button
            {
                Text = "📋 复制底孔数值到剪贴板",
                Font = new Font("Microsoft YaHei UI", 9F),
                BackColor = Color.FromArgb(240, 240, 240),
                ForeColor = Color.FromArgb(33, 37, 41),
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.Top,
                Height = 32,
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 0, 10)
            };
            btnCopy.Click += BtnCopy_Click;
            mainLayout.Controls.Add(btnCopy);

            // 10. 状态提示栏
            lblStatus = new Label
            {
                Text = "就绪。请在草图中选中要修改的尺寸或草图圆。",
                Font = new Font("Microsoft YaHei UI", 8.5F),
                ForeColor = Color.FromArgb(108, 117, 125),
                AutoSize = true,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 0, 0, 10)
            };
            mainLayout.Controls.Add(lblStatus);

            // 11. 重载数据库按钮
            btnReloadData = new Button
            {
                Text = "🔄 重新载入螺纹数据库 (ThreadStandards.json)",
                Font = new Font("Microsoft YaHei UI", 7.5F),
                ForeColor = Color.Gray,
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.Top,
                Height = 26,
                Margin = new Padding(0, 0, 0, 0)
            };
            btnReloadData.Click += (s, e) => {
                ThreadDataManager.Reload();
                LoadDataIntoUI();
                SetStatus("螺纹数据库已重新载入！", false);
            };
            mainLayout.Controls.Add(btnReloadData);

            this.Controls.Add(mainLayout);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void LoadDataIntoUI()
        {
            var db = ThreadDataManager.Database;
            cmbStandard.Items.Clear();

            if (db?.Standards == null || db.Standards.Count == 0)
            {
                SetStatus("警告：未找到螺纹数据！", true);
                return;
            }

            foreach (var std in db.Standards)
            {
                cmbStandard.Items.Add(std);
            }

            int defaultIdx = 0;
            for (int i = 0; i < cmbStandard.Items.Count; i++)
            {
                var s = cmbStandard.Items[i] as StandardItem;
                if (s != null && s.Id == "GB")
                {
                    defaultIdx = i;
                    break;
                }
            }
            cmbStandard.SelectedIndex = defaultIdx;
        }

        private void CmbStandard_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedStd = cmbStandard.SelectedItem as StandardItem;
            cmbThreadType.Items.Clear();

            if (selectedStd?.ThreadTypes != null)
            {
                foreach (var t in selectedStd.ThreadTypes)
                {
                    cmbThreadType.Items.Add(t);
                }
            }

            if (cmbThreadType.Items.Count > 0)
            {
                cmbThreadType.SelectedIndex = 0;
            }
        }

        private void CmbThreadType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedType = cmbThreadType.SelectedItem as ThreadTypeItem;
            cmbSpec.Items.Clear();

            if (selectedType?.Specifications != null)
            {
                foreach (var spec in selectedType.Specifications)
                {
                    cmbSpec.Items.Add(spec);
                }
            }

            if (cmbSpec.Items.Count > 0)
            {
                int m6Idx = 0;
                for (int i = 0; i < cmbSpec.Items.Count; i++)
                {
                    var sp = cmbSpec.Items[i] as ThreadSpecItem;
                    if (sp != null && (sp.SpecName == "M6" || sp.SpecName.StartsWith("M6 ")))
                    {
                        m6Idx = i;
                        break;
                    }
                }
                cmbSpec.SelectedIndex = m6Idx;
            }
            else
            {
                ClearResultCard();
            }
        }

        private void CmbSpec_SelectedIndexChanged(object sender, EventArgs e)
        {
            var spec = cmbSpec.SelectedItem as ThreadSpecItem;
            cmbTolerance.Items.Clear();

            if (spec?.Tolerances != null && spec.Tolerances.Count > 0)
            {
                var sortedTols = spec.Tolerances.OrderBy(t => t.PreferenceLevel).ToList();
                foreach (var tol in sortedTols)
                {
                    cmbTolerance.Items.Add(tol);
                }
                cmbTolerance.SelectedIndex = 0;
            }

            RefreshCurrentTolerance();
        }

        private void RefreshCurrentTolerance()
        {
            var spec = cmbSpec.SelectedItem as ThreadSpecItem;
            if (spec == null)
            {
                ClearResultCard();
                return;
            }

            if (rbTolPreset.Checked)
            {
                var tol = cmbTolerance.SelectedItem as ToleranceItem;
                UpdateResultCardPreset(spec, tol);
            }
            else
            {
                string dev = cmbDeviation.SelectedItem?.ToString() ?? "H";
                string it = cmbITGrade.SelectedItem?.ToString() ?? "IT7";

                _currentFullTolResult = ToleranceEngine.CalculateTolerance(
                    spec.MajorDiameter,
                    spec.Pitch,
                    spec.DefaultTapDrill,
                    dev,
                    it
                );

                UpdateResultCardFull(spec, _currentFullTolResult);
            }
        }

        private void CmbTolerance_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= cmbTolerance.Items.Count) return;
            var tol = cmbTolerance.Items[e.Index] as ToleranceItem;
            if (tol == null) return;

            e.DrawBackground();

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Color dotColor;
            string levelTag;
            switch (tol.PreferenceLevel)
            {
                case 1:
                    dotColor = Color.FromArgb(46, 125, 50);
                    levelTag = "【工程首选推荐】";
                    break;
                case 2:
                    dotColor = Color.FromArgb(230, 140, 0);
                    levelTag = "【次选公差】";
                    break;
                default:
                    dotColor = Color.FromArgb(216, 67, 21);
                    levelTag = "【不建议/迫不得已】";
                    break;
            }

            int dotSize = 10;
            int dotX = e.Bounds.X + 6;
            int dotY = e.Bounds.Y + (e.Bounds.Height - dotSize) / 2;
            using (var brush = new SolidBrush(dotColor))
            {
                g.FillEllipse(brush, dotX, dotY, dotSize, dotSize);
            }

            string text = $"{tol.Grade}  {levelTag}";
            Color textColor = (e.State & DrawItemState.Selected) == DrawItemState.Selected
                ? SystemColors.HighlightText
                : Color.FromArgb(33, 37, 41);

            using (var textBrush = new SolidBrush(textColor))
            {
                Font fontToUse = (tol.PreferenceLevel == 1) ? new Font(e.Font, FontStyle.Bold) : e.Font;
                g.DrawString(text, fontToUse, textBrush, e.Bounds.X + 22, e.Bounds.Y + (e.Bounds.Height - fontToUse.Height) / 2);
            }

            e.DrawFocusRectangle();
        }

        private void UpdateResultCardPreset(ThreadSpecItem spec, ToleranceItem tol)
        {
            double drill = tol?.RecommendedTapDrill > 0 ? tol.RecommendedTapDrill : spec.DefaultTapDrill;
            lblResultDrill.Text = string.Format("Ф {0:0.###} mm", drill);

            if (tol != null)
            {
                string tag = tol.PreferenceLevel == 1 ? "【工程首选推荐】" : (tol.PreferenceLevel == 2 ? "【次选公差】" : "【不建议使用】");
                Color tagColor = tol.PreferenceLevel == 1
                    ? Color.FromArgb(46, 125, 50)
                    : (tol.PreferenceLevel == 2 ? Color.FromArgb(230, 140, 0) : Color.FromArgb(216, 67, 21));

                lblTolBadge.Text = $"● {tol.Grade} {tag}";
                lblTolBadge.ForeColor = tagColor;

                if (tol.MinorMin > 0 || tol.MinorMax > 0)
                {
                    lblMinorRange.Text = string.Format("理论小径范围: {0:0.###} ~ {1:0.###} mm", tol.MinorMin, tol.MinorMax);
                }
                else
                {
                    lblMinorRange.Text = "理论小径范围: 标准推荐配合";
                }
            }
            else
            {
                lblTolBadge.Text = "● 标准公差配合";
                lblTolBadge.ForeColor = Color.FromArgb(46, 125, 50);
                lblMinorRange.Text = "理论小径: 标准推荐";
            }

            lblPitchInfo.Text = string.Format("螺距/牙距: {0}", spec.PitchDisplay ?? (spec.Pitch + " mm"));
            lblMajorInfo.Text = string.Format("公称大径: {0:0.###} mm", spec.MajorDiameter);
        }

        private void UpdateResultCardFull(ThreadSpecItem spec, ToleranceResult res)
        {
            if (res == null) return;

            lblResultDrill.Text = string.Format("Ф {0:0.###} mm", res.RecommendedDrill);

            Color tagColor = res.PreferenceLevel == 1
                ? Color.FromArgb(46, 125, 50)
                : (res.PreferenceLevel == 2 ? Color.FromArgb(230, 140, 0) : Color.FromArgb(216, 67, 21));

            string signEI = res.EIMm >= 0 ? "+" : "";
            string signES = res.ESMm >= 0 ? "+" : "";
            lblTolBadge.Text = $"● {res.Code} 【{res.PreferenceDesc}】 ({signEI}{res.EIMm * 1000:0.#} / {signES}{res.ESMm * 1000:0.#} μm)";
            lblTolBadge.ForeColor = tagColor;

            lblMinorRange.Text = string.Format("理论小径范围: {0:0.###} ~ {1:0.###} mm (IT={2:0.###}mm)", res.MinorMin, res.MinorMax, res.ITMm);
            lblPitchInfo.Text = string.Format("螺距/牙距: {0}", spec.PitchDisplay ?? (spec.Pitch + " mm"));
            lblMajorInfo.Text = string.Format("公称大径: {0:0.###} mm", spec.MajorDiameter);
        }

        private void ClearResultCard()
        {
            lblResultDrill.Text = "Ф -- mm";
            lblTolBadge.Text = "● --";
            lblMinorRange.Text = "理论小径: --";
            lblPitchInfo.Text = "螺距: --";
            lblMajorInfo.Text = "公称大径: --";
        }

        private double GetTargetValue()
        {
            var spec = cmbSpec.SelectedItem as ThreadSpecItem;
            if (spec == null) return 0;

            if (rbTolPreset.Checked)
            {
                var tol = cmbTolerance.SelectedItem as ToleranceItem;
                double defaultDrill = tol?.RecommendedTapDrill > 0 ? tol.RecommendedTapDrill : spec.DefaultTapDrill;

                switch (cmbValueSource.SelectedIndex)
                {
                    case 1:
                        return tol != null && tol.MinorMin > 0 ? tol.MinorMin : defaultDrill;
                    case 2:
                        return tol != null && tol.MinorMin > 0 && tol.MinorMax > 0 ? (tol.MinorMin + tol.MinorMax) / 2.0 : defaultDrill;
                    case 3:
                        return tol != null && tol.MinorMax > 0 ? tol.MinorMax : defaultDrill;
                    default:
                        return defaultDrill;
                }
            }
            else
            {
                if (_currentFullTolResult == null) return spec.DefaultTapDrill;

                switch (cmbValueSource.SelectedIndex)
                {
                    case 1:
                        return _currentFullTolResult.MinorMin;
                    case 2:
                        return (_currentFullTolResult.MinorMin + _currentFullTolResult.MinorMax) / 2.0;
                    case 3:
                        return _currentFullTolResult.MinorMax;
                    default:
                        return _currentFullTolResult.RecommendedDrill;
                }
            }
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            double targetValueMm = GetTargetValue();
            if (targetValueMm <= 0)
            {
                MessageBox.Show("请先选择有效的螺纹规格！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool isRadius = rbRadius.Checked;
            var result = SwSketchHelper.ApplySizeToSelection(_swApp, targetValueMm, isRadius);

            if (result.Success)
            {
                SetStatus(result.Message, false);
            }
            else
            {
                SetStatus(result.Message, true);
                MessageBox.Show(result.Message, "应用结果提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnCopy_Click(object sender, EventArgs e)
        {
            double targetValueMm = GetTargetValue();
            if (targetValueMm <= 0) return;

            double copyValue = rbRadius.Checked ? (targetValueMm / 2.0) : targetValueMm;
            Clipboard.SetText(copyValue.ToString("0.###"));
            SetStatus(string.Format("已复制数值 {0:0.###} 到剪贴板！", copyValue), false);
        }

        private void SetStatus(string message, bool isError)
        {
            lblStatus.Text = message;
            lblStatus.ForeColor = isError ? Color.FromArgb(217, 48, 37) : Color.FromArgb(40, 167, 69);
        }
    }
}
