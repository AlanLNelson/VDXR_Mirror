using System;
using System.Windows;

namespace VDXRMirror
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Initialize application settings
            AppSettings.Load();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Save settings on exit
            AppSettings.Save();
            
            base.OnExit(e);
        }
    }
}