using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using SilentOrb.Utility;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Diagnostics;
using System.Management.Automation.Runspaces;
using SpeechFamiliar.UI;
using System.Text.RegularExpressions;
using SpeechFamiliar.Forms;

namespace SpeechFamiliar.Scripting
{
    public static class Automation
    {
        public delegate void Window_Event(Window_Info info);
        public static event Window_Event Active_Window_Changed;
        internal static Context default_context;
        public static Speech_Engine engine { get; internal set; }
        //static public SpeechFamiliar.UI.Controller controller;

        public static string text_dialog(string title)
        {

            Text_Dialog dialog = new Text_Dialog();
            dialog.Text = title;

            dialog.ShowDialog();//.ShowDialog(Global.Main_Window);

            if (dialog.Result)
                return dialog.text_Input.Text;
            else
                return "";
        }

        public static void add_word_dialog(string grammar, string old_word)
        {
            string[] split = grammar.Split('#');
            Familiar_Document document = Familiar_Document.find_document(split[0]);
            string rule_name = "";

            if (split.Length > 1)
                rule_name = split[1];
            else
                rule_name = "rule_name";

            Element_Word word = null;

            if (old_word != null && old_word != "")
            {
                word = document.get_word(old_word, rule_name);
            }
            else
                word = new Element_Word();

            Generic_Form dialog = new Generic_Form(word, "Add New Word");

            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                if (old_word == null || old_word == "")
                    document.add_word(word, rule_name);
                document.save();
                reload();
            }
            catch (Exception ex)
            {
                Feedback.print(ex);
            }
        }

        //public static string format_grammar_name(string grammar_name)
        //{
        //    string[] split = grammar_name.Split('#');
        //    Familiar_Document document = Familiar_Document.find_document(split[0]);

        //    if (split.Length > 1)
        //    {
        //        return split[1];
        //    }
        //    else
        //        document.add_new_word(word, "rule_name";
        //}

        //public static void add_new_word(Element_Item word, string grammar_name)
        //{
        //    //    string[] split = grammar_name.Split(new char[] { '#' });
        //    //    Familiar_Document document = Familiar_Document.find_document(split[0]);

        //    //    add_new_word(new Element_Item(document, word), grammar_name);
        //    //}

        //    //public static void add_new_word(Element_Base word, string grammar_name)
        //    //{
        //    try
        //    {


        //        document.save();
        //        reload();
        //    }
        //    catch (Exception ex)
        //    {
        //        Feedback.print(ex);
        //    }
        //}

        public static string get_pretext()
        {
            var text = current_context.get_pretext();
            if (text == null)
            {
                if (default_context != null)
                {
                    text = default_context.get_pretext();

                    if (text == null)
                        return null;
                }
            }

            return text;
        }

        public static void sleep()
        {
            Global.speech.standby = true;
            Global.speech.set_title();
        }

        public static void sleep(int amount)
        {
            Thread.Sleep(amount);
        }

        public static void close_speech_familiar()
        {
            //Global.Main_Window.manager.send("close", null);

            SilentOrb.Threading.Threading.cross_thread_control(Global.Main_Window,
                (window) => (window as Form).Close());
            //     Windows.close_window(Global.Main_Window.Handle);
        }

        public static void wake_up()
        {
            Global.speech.standby = false;
        }

        public static object execute(string command_name)
        {
            return execute(command_name, null);
        }

        public static object execute(string command_name, object[] arguments)
        {
            string[] info = command_name.Split(new char[] { '.' });
            if (info.Length > 1)
            {
                if (Global.plugins.ContainsKey(info[0]))
                {
                    Plugin plugin = Global.plugins[info[0]];
                    Type[] types = new Type[arguments.Length];
                    for (int i = 0; i < arguments.Length; i++)
                    {
                        types[i] = arguments[i].GetType();
                    }

                    System.Reflection.MethodInfo method = plugin.GetType().GetMethod(info[1], types);
                    return method.Invoke(plugin, arguments);
                }
            }

            return null;
        }

        public static object execute(string command_name, object argument)
        {
            return execute(command_name, new object[] { argument });
        }

        public static void reload()
        {
            //Global.Main_Window.timer.Stop();
            sleep();
            Global.debug_string = "";
            Global.thread_busy = true;
            Global.debug_string += "b";
            Thread.Sleep(500);
            lock (Global.thread_control)
            {
                Global.debug_string += "1";
                Familiar_Document.reset();
                //     Global.load_plugins();
                engine.reload();

                Feedback.print("Finished Reloading.", Feedback.Status.story);
            }

            Global.thread_busy = false;
            wake_up();
            //Global.Main_Window.timer.Start();
        }

        internal static void window_changed(Window_Info info)
        {
            if (Active_Window_Changed != null)
                Active_Window_Changed.Invoke(info);
        }

        public static void sendkeys(string text)
        {
            try
            {
                Feedback.print("sendkeys " + text + "\r\n", Feedback.Status.debug);
                send_input.sendkeys(text);
            }
            catch (Exception ex)
            {
                Feedback.print(ex);
            }
        }

        public static void print(string text)
        {
            Feedback.print(text, Feedback.Status.info);
        }

        public static Familiar_Document get_library(string library_name)
        {
            if (Global.speech.vocabularies.ContainsKey(library_name))
                return Global.speech.vocabularies[library_name];

            return null;
        }

        public static void hide_speech_familiar()
        {
            Global.Main_Window.WindowState = FormWindowState.Minimized;
        }

        public static void show_speech_familiar()
        {
            Global.Main_Window.WindowState = FormWindowState.Normal;
        }

        public static string system(string scriptText)
        {
            // create Powershell runspace

            Runspace runspace = RunspaceFactory.CreateRunspace();

            // open it

            runspace.Open();

            // create a pipeline and feed it the script text

            Pipeline pipeline = runspace.CreatePipeline();
            pipeline.Commands.AddScript(scriptText);

            // add an extra command to transform the script
            // output objects into nicely formatted strings

            // remove this line to get the actual objects
            // that the script returns. For example, the script

            // "Get-Process" returns a collection
            // of System.Diagnostics.Process instances.

            pipeline.Commands.Add("Out-String");

            // execute the script

            Collection<PSObject> results = pipeline.Invoke();

            // close the runspace

            runspace.Close();

            // convert the script result into a single string

            StringBuilder stringBuilder = new StringBuilder();
            foreach (PSObject obj in results)
            {
                stringBuilder.AppendLine(obj.ToString());
            }

            return stringBuilder.ToString();
        }

        public static void run_program(string command)
        {
            ProcessStartInfo program = new ProcessStartInfo();
            program.FileName = command;
            //program.FileName = program.FileName.Replace("%SystemRoot%", Environment.SystemDirectory);
            System.Diagnostics.Process.Start(program);

        }

        public static Context current_context
        {
            get { return engine.active_profile; }
        }

        public static Window_Info current_window
        {
            get { return Global.Main_Window.active_window; }
        }

        public static Element_Word find_word(string text)
        {
            foreach (var document in current_context.vocabularies.Values)
            {
                Element_Word result = document.find_word(text);
                if (result != null)
                    return result;
            }

            return null;
        }

        //        def find_word text
        //    Automation.current_context.vocabularies.values.each do |document|
        //        #			print document.name.to_s + " #{document.vocabulary.count}\r\n"
        //        result = document.find_word text
        //        return result if result			
        //    end

        //    nil
        //end
    }

}
