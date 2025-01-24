using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Project_College_App
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            
            try
            {
                // Kill any existing instances
                foreach (var process in Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Application.ExecutablePath)))
                {
                    try
                    {
                        if (process.Id != Process.GetCurrentProcess().Id)
                        {
                            process.Kill();
                            process.WaitForExit(2000); // Increased wait time to 2 seconds
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error closing existing instance: {ex.Message}", "Warning", 
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                // Add a small delay to ensure processes are fully terminated
                System.Threading.Thread.Sleep(500);

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new LoginForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Startup Error: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show($"Thread Exception: {e.Exception.Message}", "Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"Unhandled Exception: {((Exception)e.ExceptionObject).Message}", "Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
