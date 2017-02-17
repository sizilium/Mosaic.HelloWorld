using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading;
using VP.FF.PT.Common.Infrastructure;

namespace MosaicSample.MosaicService
{
    class Program
    {
        static readonly Func<string> DefaultApplicationName = () => Process.GetCurrentProcess().ProcessName.Replace(".vshost", "");
        private static Bootstrapper _bootstrapper;

        #region Console Handler
        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);

        // A delegate type to be used as the handler routine 
        // for SetConsoleCtrlHandler.
        public delegate bool HandlerRoutine(CtrlTypes CtrlType);

        // An enumerated type for the control messages
        // sent to the handler routine.
        public enum CtrlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }

        private static bool ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            _bootstrapper.Stop();
            return true;
        }
        #endregion

        public static void Main(string[] args)
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += UnhandledExceptionHandler;
            Thread.CurrentThread.Name = "Main";

            if (Environment.UserInteractive)
            {
                Console.Title = DefaultApplicationName() + " (SW-Ver. " + LineControlVersion + ")";

                SetConsoleCtrlHandler(ConsoleCtrlCheck, true);

                _bootstrapper = new Bootstrapper();
                _bootstrapper.Run();
                Console.ReadLine();
                _bootstrapper.Stop();
            }
            else
            {
                //Logger.DebugFormat("Running the ServiceHost as a Windows Service");
                var servicesToRun = new ServiceBase[]
                {
                    new ServiceHost()
                };
                ServiceBase.Run(servicesToRun);
            }
        }

        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            _bootstrapper.LogApplicationCrash((Exception)unhandledExceptionEventArgs.ExceptionObject);
        }

        private static string LineControlVersion
        {
            get
            {
                return Versioning.GetVersion(Assembly.GetExecutingAssembly());
            }
        }
    }
}
