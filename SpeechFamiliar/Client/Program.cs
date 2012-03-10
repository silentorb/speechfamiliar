using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SilentOrb.Utility;
using Microsoft.Win32;
using SpeechFamiliar.Forms;

namespace SpeechFamiliar
{
    static class Program
    {

        //#else
        //        static public SpeechEngine speech;
        //#endif
        //static public SpeechFamiliar.UI.Controller controller;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                //System.IO.File.WriteAllText("test.txt", "this is a test");
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                Global.initialize();


                SystemEvents.PowerModeChanged += new PowerModeChangedEventHandler(SystemEvents_PowerModeChanged);
                Application.ApplicationExit += new EventHandler(Application_ApplicationExit);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ": " + ex.StackTrace, "Immediate Program Error!");
                return;
            }
            Application.Run(Global.Main_Window);
        }



        //void SystemEvents_EventsThreadShutdown(object sender, EventArgs e)
        //{
        //    UnregisterFromSystemEvents();
        //}

        static void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            if (e.Mode == PowerModes.Resume)
            {
                Global.speech.engine.initialize();
                Global.speech.LoadInitialGrammars();
                Feedback.print("Resumed from standby.\r\n", Feedback.Status.story);
            }
        }

        static void UnregisterFromSystemEvents()
        {
            SystemEvents.PowerModeChanged -= new PowerModeChangedEventHandler(SystemEvents_PowerModeChanged);
            //        SystemEvents.EventsThreadShutdown -= new EventHandler(SystemEvents_EventsThreadShutdown);
        }

        static void Application_ApplicationExit(object sender, EventArgs e)
        {
            UnregisterFromSystemEvents();
        }

    }
}