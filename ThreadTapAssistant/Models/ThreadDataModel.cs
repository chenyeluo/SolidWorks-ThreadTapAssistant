using System;
using System.Collections.Generic;

namespace ThreadTapAssistant.Models
{
    public class ThreadDatabase
    {
        public List<StandardItem> Standards { get; set; } = new List<StandardItem>();
    }

    public class StandardItem
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public List<ThreadTypeItem> ThreadTypes { get; set; } = new List<ThreadTypeItem>();

        public override string ToString() => DisplayName ?? Id;
    }

    public class ThreadTypeItem
    {
        public string Id { get; set; }
        public string Category { get; set; } // "普通螺纹", "管螺纹", "传动螺纹"
        public string DisplayName { get; set; }
        public string Unit { get; set; }
        public List<ThreadSpecItem> Specifications { get; set; } = new List<ThreadSpecItem>();

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Category))
            {
                return $"[{Category}] {DisplayName}";
            }
            return DisplayName ?? Id;
        }
    }

    public class ThreadSpecItem
    {
        public string SpecName { get; set; }
        public string DisplayName { get; set; }
        public string DiameterGroup { get; set; }
        public string PitchType { get; set; } // "粗牙", "细牙", "标准牙"
        public double MajorDiameter { get; set; }
        public double Pitch { get; set; }
        public string PitchDisplay { get; set; }
        public double DefaultTapDrill { get; set; }
        public List<ToleranceItem> Tolerances { get; set; } = new List<ToleranceItem>();

        public override string ToString() => DisplayName ?? SpecName;
    }

    public class ToleranceItem
    {
        public string Grade { get; set; }
        public int PreferenceLevel { get; set; } = 1; // 1: 绿色(推荐/首选), 2: 黄色(次选), 3: 橙色(不建议/特殊)
        public string PreferenceDesc { get; set; } = "首选推荐";
        public double MinorMin { get; set; }
        public double MinorMax { get; set; }
        public double RecommendedTapDrill { get; set; }

        public override string ToString()
        {
            string tag = PreferenceLevel == 1 ? "【首选推荐】" : (PreferenceLevel == 2 ? "【次选】" : "【不建议】");
            return $"{Grade} {tag}";
        }
    }
}
