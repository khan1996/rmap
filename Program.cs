using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;

namespace rMap
{
    static class Program
    {
        public static System.Globalization.NumberFormatInfo Number = System.Globalization.NumberFormatInfo.InvariantInfo;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Security.Checkin.Init();

            Form f = Security.Checkin.Load(args);

            if (f != null)
                Application.Run(f);
        }

        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.ToString(), "Unhandled Thread Exception");
            // here you can log the exception ...
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show((e.ExceptionObject as Exception).ToString(), "Unhandled UI Exception");
            // here you can log the exception ...
        }

    }
}
