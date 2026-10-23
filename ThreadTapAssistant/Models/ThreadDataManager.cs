using System;
using System.IO;
using System.Reflection;
using System.Web.Script.Serialization;

namespace ThreadTapAssistant.Models
{
    public static class ThreadDataManager
    {
        private static ThreadDatabase _database;

        public static ThreadDatabase Database
        {
            get
            {
                if (_database == null)
                {
                    LoadDatabase();
                }
                return _database;
            }
        }

        public static void Reload()
        {
            _database = null;
            LoadDatabase();
        }

        private static void LoadDatabase()
        {
            try
            {
                string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string jsonPath = Path.Combine(assemblyFolder, "Data", "ThreadStandards.json");

                if (!File.Exists(jsonPath))
                {
                    // Fallback to current directory or directly alongside DLL
                    jsonPath = Path.Combine(assemblyFolder, "ThreadStandards.json");
                }

                if (File.Exists(jsonPath))
                {
                    string jsonContent = File.ReadAllText(jsonPath, System.Text.Encoding.UTF8);
                    var serializer = new JavaScriptSerializer();
                    _database = serializer.Deserialize<ThreadDatabase>(jsonContent);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading ThreadStandards.json: " + ex.Message);
            }

            if (_database == null || _database.Standards == null || _database.Standards.Count == 0)
            {
                _database = CreateFallbackDatabase();
            }
        }

        private static ThreadDatabase CreateFallbackDatabase()
        {
            var db = new ThreadDatabase();
            var std = new StandardItem { Id = "GB", DisplayName = "GB (中国国家标准)" };
            var type = new ThreadTypeItem { Id = "MetricCoarse", DisplayName = "普通粗牙 (Metric Coarse - 60°)", Unit = "mm" };

            var m6 = new ThreadSpecItem
            {
                SpecName = "M6×1.0",
                MajorDiameter = 6.0,
                Pitch = 1.0,
                PitchDisplay = "1.00 mm",
                DefaultTapDrill = 5.0
            };
            m6.Tolerances.Add(new ToleranceItem { Grade = "6H", MinorMin = 4.917, MinorMax = 5.153, RecommendedTapDrill = 5.0 });
            m6.Tolerances.Add(new ToleranceItem { Grade = "7H", MinorMin = 4.917, MinorMax = 5.217, RecommendedTapDrill = 5.0 });

            type.Specifications.Add(m6);
            std.ThreadTypes.Add(type);
            db.Standards.Add(std);
            return db;
        }
    }
}
