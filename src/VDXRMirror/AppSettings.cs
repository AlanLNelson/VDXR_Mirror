using System;
using System.IO;
using System.Text.Json;

namespace VDXRMirror
{
    public static class AppSettings
    {
        private static readonly string SettingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "VDXRMirror",
            "settings.json");

        // Default settings
        public static string Resolution { get; set; } = "1080p";
        public static string EyeSelection { get; set; } = "Right";
        public static bool SmoothingEnabled { get; set; } = true;
        public static int SmoothingStrength { get; set; } = 50;
        public static double WindowX { get; set; } = 100;
        public static double WindowY { get; set; } = 100;

        public static void Load()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    var json = File.ReadAllText(SettingsPath);
                    var settings = JsonSerializer.Deserialize<SettingsData>(json);
                    
                    if (settings != null)
                    {
                        Resolution = settings.Resolution;
                        EyeSelection = settings.EyeSelection;
                        SmoothingEnabled = settings.SmoothingEnabled;
                        SmoothingStrength = settings.SmoothingStrength;
                        WindowX = settings.WindowX;
                        WindowY = settings.WindowY;
                    }
                }
            }
            catch (Exception ex)
            {
                // If settings can't be loaded, use defaults
                System.Diagnostics.Debug.WriteLine($"Failed to load settings: {ex.Message}");
            }
        }

        public static void Save()
        {
            try
            {
                var settingsDir = Path.GetDirectoryName(SettingsPath);
                if (!Directory.Exists(settingsDir))
                {
                    Directory.CreateDirectory(settingsDir!);
                }

                var settings = new SettingsData
                {
                    Resolution = Resolution,
                    EyeSelection = EyeSelection,
                    SmoothingEnabled = SmoothingEnabled,
                    SmoothingStrength = SmoothingStrength,
                    WindowX = WindowX,
                    WindowY = WindowY
                };

                var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions 
                { 
                    WriteIndented = true 
                });
                
                File.WriteAllText(SettingsPath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save settings: {ex.Message}");
            }
        }

        private class SettingsData
        {
            public string Resolution { get; set; } = "1080p";
            public string EyeSelection { get; set; } = "Right";
            public bool SmoothingEnabled { get; set; } = true;
            public int SmoothingStrength { get; set; } = 50;
            public double WindowX { get; set; } = 100;
            public double WindowY { get; set; } = 100;
        }
    }
}