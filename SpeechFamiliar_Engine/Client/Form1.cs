using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//using System.Speech.Recognition;
//using SpeechFamiliar.UI;
using SilentOrb.Utility;
using System.Reflection;
using SpeechFamiliar.Scripting;

namespace SpeechFamiliar.Forms
{
    public partial class Form1 : Form
    {
        public Timer timer;
        public Window_Info active_window = new Window_Info();
        public Message_Manager manager;

        public Form1()
        {
            InitializeComponent();
#if DEBUG
            Height += 200;
            Width += 200;
#endif
            Feedback.initialize();
            //Feedback.print_method+= delegate(string text)
            //{
            //    SilentOrb.Threading.Threading.AppendText(text_Main, text);
            //});
            set_icon("sleep.ico");
            
            Feedback.log_folder = Global.configuration.log_path;
            Feedback.print_method += delegate(string text)
            {
                manager.send("set_text", text);
            };

            timer = new Timer();

            //  try
            {

                timer.Tick += new EventHandler(timer_Tick);
                timer.Interval = 250;

                manager = new Message_Manager(this, 50);
                manager.add_router("sendkeys", typeof(SendKeys), "Send");
                manager.add_router("set_title", this, "set_title");
                manager.add_router("set_icon", this, "set_icon");
                manager.add_router("set_text", this, "set_text");
                manager.add_router("close", this, "Close");
                manager.start();
                //    Controller hand  = new Controller();
            }
            //   catch
            {
                //         Close();
            }
            Global.thread_busy = true;
            timer.Start();
            Text = "*Loading...";
            //   SystemEvents.EventsThreadShutdown += new EventHandler(SystemEvents_EventsThreadShutdown);
        }


        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
#if DEBUG
            Global.init();
#else
            SilentOrb.Threading.Threading.start_new_thread(typeof(Global), "init", null);
#endif

        }

        public void set_title(string title)
        {
            this.Text = title;
        }

        public void set_text(string text)
        {
            text_Main.Text += text;
            text_Main.SelectionStart = text_Main.Text.Length - 1;
            text_Main.ScrollToCaret();
        }

        public void set_icon( string icon_name)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            this.Icon = new Icon(assembly.GetManifestResourceStream("SpeechFamiliar." + icon_name));
        }
         
        public void Start()
        {
            timer.Start();
            Feedback.print("Refresh Timer started.\r\n", Feedback.Status.story);
        }

        //protected override void OnFormClosing(FormClosingEventArgs e)
        //{
        //    base.OnFormClosing(e);
        //    UnregisterFromSystemEvents();            
        //}

        private void timer_Tick(Object sender, EventArgs e)
        {
            if (Global.thread_busy)
                return;
            //          Program.debug_string += "(";

            lock (Global.thread_control)
            {
                Window_Info current_window = Windows.GetForegroundWindow();
                if (current_window.title != active_window.title && current_window.handle != (IntPtr)0)
                {
                    active_window = current_window;
                    Automation.window_changed(active_window);
                    text_active_window.Text = active_window.class_name;
                }
            }
        }

        delegate void SetTextCallback(Control control, string text);

        public void SetText(Control control, string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (control.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                control.Invoke(d, new object[] { control, text });
            }
            else
            {
                control.Text = text;
            }
        }

    }


}