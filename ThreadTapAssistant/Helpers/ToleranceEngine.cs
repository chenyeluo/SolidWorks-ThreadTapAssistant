using System;
using System.Linq;

namespace ThreadTapAssistant.Helpers
{
    public class ToleranceResult
    {
        public string Code { get; set; }           // e.g. "H7", "6H", "G6"
        public string DeviationLetter { get; set; } // e.g. "H"
        public string ITGrade { get; set; }         // e.g. "IT7"
        public int PreferenceLevel { get; set; }    // 1: 绿, 2: 黄, 3: 橙
        public string PreferenceDesc { get; set; }  // "工程首选推荐", "次选", "不建议/迫不得已"
        public double EIMm { get; set; }            // 下偏差 (mm)
        public double ESMm { get; set; }            // 上偏差 (mm)
        public double ITMm { get; set; }            // 公差带大小 (mm)
        public double MinorMin { get; set; }        // 小径/底孔理论下限 (mm)
        public double MinorMax { get; set; }        // 小径/底孔理论上限 (mm)
        public double RecommendedDrill { get; set; } // 推荐底孔钻头 (mm)
    }

    /// <summary>
    /// 全球标准公差计算引擎 (遵循 ISO 286 / GB/T 1800.1 / GB/T 197)
    /// 全面支持 A 到 ZC (28种基本偏差) 以及 IT01 到 IT18 (20种公差等级)
    /// </summary>
    public static class ToleranceEngine
    {
        // 28种内孔/内螺纹基本偏差代号 (大写)
        public static readonly string[] FundamentalDeviations = new string[]
        {
            "A", "B", "C", "CD", "D", "E", "EF", "F", "FG", "G",
            "H", "J", "JS", "K", "M", "N", "P", "R", "S", "T",
            "U", "V", "X", "Y", "Z", "ZA", "ZB", "ZC"
        };

        // 20种国家/国际标准公差等级
        public static readonly string[] ITGrades = new string[]
        {
            "IT01", "IT0", "IT1", "IT2", "IT3", "IT4", "IT5", "IT6",
            "IT7", "IT8", "IT9", "IT10", "IT11", "IT12", "IT13", "IT14",
            "IT15", "IT16", "IT17", "IT18"
        };

        /// <summary>
        /// 计算指定尺寸与偏差公差组合的完整极限数值
        /// </summary>
        public static ToleranceResult CalculateTolerance(
            double nominalMajorMm,
            double pitchMm,
            double defaultMinorMm,
            string deviationLetter,
            string itGrade)
        {
            deviationLetter = (deviationLetter ?? "H").Trim().ToUpper();
            itGrade = (itGrade ?? "IT7").Trim().ToUpper();

            double D = Math.Max(1.0, nominalMajorMm);
            double Dm = GetDiameterGeometricMean(D);

            // 1. 计算 IT 公差值 (单位: mm)
            double itValueMm = CalculateITValueMm(Dm, itGrade);

            // 2. 计算基本偏差下偏差 EI 与上偏差 ES (单位: mm)
            double eiMm = CalculateEIMm(Dm, pitchMm, deviationLetter, itValueMm);
            double esMm = eiMm + itValueMm;

            // 3. 计算理论小径上下限
            double baseMinor = defaultMinorMm > 0 ? defaultMinorMm : (nominalMajorMm - 1.08253 * pitchMm);
            double minorMin = baseMinor + eiMm;
            double minorMax = baseMinor + esMm;

            // 4. 计算车间推荐钻头尺寸
            double drill = CalculateRecommendedDrill(baseMinor, minorMin, minorMax, deviationLetter, itGrade);

            // 5. 判定工程推荐优先级 (1:绿, 2:黄, 3:橙)
            int prefLevel;
            string prefDesc;
            DeterminePreference(deviationLetter, itGrade, out prefLevel, out prefDesc);

            return new ToleranceResult
            {
                Code = deviationLetter + itGrade.Replace("IT", ""),
                DeviationLetter = deviationLetter,
                ITGrade = itGrade,
                PreferenceLevel = prefLevel,
                PreferenceDesc = prefDesc,
                EIMm = eiMm,
                ESMm = esMm,
                ITMm = itValueMm,
                MinorMin = minorMin,
                MinorMax = minorMax,
                RecommendedDrill = drill
            };
        }

        private static double GetDiameterGeometricMean(double d)
        {
            if (d <= 3) return Math.Sqrt(1.0 * 3.0);
            if (d <= 6) return Math.Sqrt(3.0 * 6.0);
            if (d <= 10) return Math.Sqrt(6.0 * 10.0);
            if (d <= 18) return Math.Sqrt(10.0 * 18.0);
            if (d <= 30) return Math.Sqrt(18.0 * 30.0);
            if (d <= 50) return Math.Sqrt(30.0 * 50.0);
            if (d <= 80) return Math.Sqrt(50.0 * 80.0);
            if (d <= 120) return Math.Sqrt(80.0 * 120.0);
            if (d <= 180) return Math.Sqrt(120.0 * 180.0);
            if (d <= 250) return Math.Sqrt(180.0 * 250.0);
            if (d <= 315) return Math.Sqrt(250.0 * 315.0);
            if (d <= 400) return Math.Sqrt(315.0 * 400.0);
            return Math.Sqrt(400.0 * 500.0);
        }

        private static double CalculateITValueMm(double Dm, string itGrade)
        {
            // 标准公差因子 i (微米)
            double i = 0.45 * Math.Pow(Dm, 1.0 / 3.0) + 0.001 * Dm;
            double itMicrons;

            switch (itGrade)
            {
                case "IT01": itMicrons = 0.3 + 0.008 * Dm; break;
                case "IT0":  itMicrons = 0.5 + 0.012 * Dm; break;
                case "IT1":  itMicrons = 0.8 + 0.020 * Dm; break;
                case "IT2":  itMicrons = 1.2 + 0.028 * Dm; break;
                case "IT3":  itMicrons = 2.0 + 0.038 * Dm; break;
                case "IT4":  itMicrons = 3.0 + 0.050 * Dm; break;
                case "IT5":  itMicrons = 7.0 * i; break;
                case "IT6":  itMicrons = 10.0 * i; break;
                case "IT7":  itMicrons = 16.0 * i; break;
                case "IT8":  itMicrons = 25.0 * i; break;
                case "IT9":  itMicrons = 40.0 * i; break;
                case "IT10": itMicrons = 64.0 * i; break;
                case "IT11": itMicrons = 100.0 * i; break;
                case "IT12": itMicrons = 160.0 * i; break;
                case "IT13": itMicrons = 250.0 * i; break;
                case "IT14": itMicrons = 400.0 * i; break;
                case "IT15": itMicrons = 640.0 * i; break;
                case "IT16": itMicrons = 1000.0 * i; break;
                case "IT17": itMicrons = 1600.0 * i; break;
                case "IT18": itMicrons = 2500.0 * i; break;
                default:     itMicrons = 16.0 * i; break; // 默认 IT7
            }

            return Math.Round(itMicrons, 2) / 1000.0;
        }

        private static double CalculateEIMm(double Dm, double pitchMm, string letter, double itValueMm)
        {
            double eiMicrons = 0;

            switch (letter)
            {
                case "A":  eiMicrons = 265.0 + 1.3 * Dm; break;
                case "B":  eiMicrons = 140.0 + 0.85 * Dm; break;
                case "C":  eiMicrons = 52.0 * Math.Pow(Dm, 0.2); break;
                case "CD": eiMicrons = 32.0 * Math.Pow(Dm, 0.3); break;
                case "D":  eiMicrons = 16.0 * Math.Pow(Dm, 0.44); break;
                case "E":  eiMicrons = 11.0 * Math.Pow(Dm, 0.41); break;
                case "EF": eiMicrons = 8.0 * Math.Pow(Dm, 0.41); break;
                case "F":  eiMicrons = 5.5 * Math.Pow(Dm, 0.41); break;
                case "FG": eiMicrons = 4.0 * Math.Pow(Dm, 0.38); break;
                case "G":  eiMicrons = 2.5 * Math.Pow(Dm, 0.34); break;
                case "H":  eiMicrons = 0.0; break; // 基准孔/内螺纹基准
                case "JS": eiMicrons = -(itValueMm * 1000.0) / 2.0; break;
                case "J":  eiMicrons = -(itValueMm * 1000.0) * 0.25; break;
                case "K":  eiMicrons = -(itValueMm * 1000.0) * 0.5; break;
                case "M":  eiMicrons = -(itValueMm * 1000.0) * 0.75; break;
                case "N":  eiMicrons = -(itValueMm * 1000.0) - 5.0 * Math.Pow(Dm, 0.34); break;
                case "P":  eiMicrons = -(itValueMm * 1000.0) - 12.0 * Math.Pow(Dm, 0.34); break;
                case "R":  eiMicrons = -(itValueMm * 1000.0) - 18.0 * Math.Pow(Dm, 0.34); break;
                case "S":  eiMicrons = -(itValueMm * 1000.0) - 25.0 * Math.Pow(Dm, 0.34); break;
                case "T":  eiMicrons = -(itValueMm * 1000.0) - 35.0 * Math.Pow(Dm, 0.34); break;
                case "U":  eiMicrons = -(itValueMm * 1000.0) - 50.0 * Math.Pow(Dm, 0.34); break;
                case "V":  eiMicrons = -(itValueMm * 1000.0) - 70.0 * Math.Pow(Dm, 0.34); break;
                case "X":  eiMicrons = -(itValueMm * 1000.0) - 100.0 * Math.Pow(Dm, 0.34); break;
                case "Y":  eiMicrons = -(itValueMm * 1000.0) - 140.0 * Math.Pow(Dm, 0.34); break;
                case "Z":  eiMicrons = -(itValueMm * 1000.0) - 190.0 * Math.Pow(Dm, 0.34); break;
                case "ZA": eiMicrons = -(itValueMm * 1000.0) - 250.0 * Math.Pow(Dm, 0.34); break;
                case "ZB": eiMicrons = -(itValueMm * 1000.0) - 330.0 * Math.Pow(Dm, 0.34); break;
                case "ZC": eiMicrons = -(itValueMm * 1000.0) - 430.0 * Math.Pow(Dm, 0.34); break;
                default:   eiMicrons = 0.0; break;
            }

            return Math.Round(eiMicrons, 2) / 1000.0;
        }

        private static double CalculateRecommendedDrill(double baseMinor, double minMinor, double maxMinor, string letter, string itGrade)
        {
            // 对于过渡与过盈配合孔(K~ZC)，底孔通常取接近上限以避免攻丝折断
            if ("K,M,N,P,R,S,T,U,V,X,Y,Z,ZA,ZB,ZC".Split(',').Contains(letter))
            {
                return Math.Round(maxMinor, 2);
            }

            // 对于标准基准孔 H 或常用间隙孔 G
            double mid = (minMinor + maxMinor) / 2.0;
            // 车间通常取标准钻头规格 (保留1位或2位小数常用钻头步长)
            return Math.Round(mid, 2);
        }

        private static void DeterminePreference(string letter, string itGrade, out int level, out string desc)
        {
            // 1. 工程首选推荐 (绿色 🟢)
            if ((letter == "H" && (itGrade == "IT6" || itGrade == "IT7" || itGrade == "IT8")) ||
                (letter == "G" && (itGrade == "IT6" || itGrade == "IT7")))
            {
                level = 1;
                desc = "工程首选推荐";
                return;
            }

            // 2. 次推荐公差 (黄色 🟡)
            if ((letter == "H" && (itGrade == "IT5" || itGrade == "IT9" || itGrade == "IT10")) ||
                (letter == "G" && (itGrade == "IT8" || itGrade == "IT9")) ||
                (letter == "F" && (itGrade == "IT7" || itGrade == "IT8")) ||
                (letter == "E" && (itGrade == "IT7" || itGrade == "IT8")) ||
                (letter == "JS" && (itGrade == "IT6" || itGrade == "IT7" || itGrade == "IT8")) ||
                (letter == "K" && (itGrade == "IT6" || itGrade == "IT7")) ||
                (letter == "M" && (itGrade == "IT6" || itGrade == "IT7")) ||
                (letter == "N" && (itGrade == "IT7" || itGrade == "IT8")))
            {
                level = 2;
                desc = "次选公差";
                return;
            }

            // 3. 不建议使用/迫不得已 (橙红色 🟠)
            level = 3;
            desc = "不建议/迫不得已";
        }
    }
}
