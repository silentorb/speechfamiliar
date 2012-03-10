using System;
using System.Collections.Generic;
using System.Text;
using SpeechFamiliar.Scripting;
using SpeechFamiliar;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Threading;
using SilentOrb.Utility;
using System.Xml;
using System.Diagnostics;
using SilentOrb.Reflection; 

namespace SpeechFamiliar.Plugins
{
    [PluginInfo("standard")]
    public class Standard : Plugin
    {
        Dictionary<string, string> special_keys = new Dictionary<string, string>();
        Dictionary<string, Target_Path> programs = new Dictionary<string, Target_Path>();

        //public override string Name
        //{
        //    get { return "Standard"; }
        //}

        //public override void initialize()
        //{
        //    special_keys.Add("control", "^");
        //    special_keys.Add("alt", "%");
        //    special_keys.Add("shift", "+");
        //}

        public void press_key(Sentence result)
        {
            try
            {
                string text = "";

                foreach (Token word in result.parameters["keys"])
                {
                    text += word.text;
                }

                Automation.sendkeys(text);
            }
            catch (Exception exception)
            {
                Feedback.print(exception);
            }
        }

        public void format_for_web()
        {
            Thread thread = new Thread(delegate()
            {
                // Clipboard.SetText("hello");
                if (!Clipboard.ContainsText())
                    return;
                string text = Clipboard.GetText();

                if (text == "")
                    return;

                // Paragraphs
                text = Regex.Replace(text, @"(\S[^\S\r\n]*)\r\n([^\S\r\n]*\S)", "$1<br/>$2", RegexOptions.Singleline);
                text = Regex.Replace(text, @"(?<=\r\n\s*\r\n(?!<span)|^|<span[^>]*>)([^\r\n]*\S+[^\r\n]*)(?:\r\n\s*\r\n|(?=</\s*span>))", @"<p>$1</p>", RegexOptions.None);

                // Punctuation
                text = Regex.Replace(text, @"(([\.\?!])[\s|(?:\n\r)]+)(?=\S)", "$1&nbsp; ");

                Clipboard.SetText(text);
            });

            thread.Name = "test";
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public static void send_text(string text)
        {
            Thread thread = new Thread(delegate(object parameter)
            {
                string text_value = parameter as string;
                //string storage = "";

                try
                {
                    object storage = store_clipboard();

                    Clipboard.SetText(text_value);
                    string test = Clipboard.GetText();
                    //   System.Threading.Thread.Sleep(250);
                    Automation.sendkeys("^v");
                    restore_clipboard(storage);
                }
                catch (Exception ex)
                {
                    Feedback.print(ex);
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Name = "send_text";

            thread.Start(text);

        }

        private static object store_clipboard()
        {
            Dictionary<string, object> storage = new Dictionary<string, object>();
            IDataObject objects = Clipboard.GetDataObject();
            foreach (string format in objects.GetFormats(false))
            {
                storage.Add(format, objects.GetData(format));
            }

            return storage;
        }

        private static void restore_clipboard(object storage_object)
        {
            Dictionary<string, object> storage = (Dictionary<string, object>)storage_object;
            foreach (string format in storage.Keys)
            {
                Clipboard.SetData(format, storage[format]);
            }
        }
         
        public static string grab_text()
        {
            Thread thread = new Thread(delegate(object parameter)
           {
               StringBuilder builder = parameter as StringBuilder;

               try
               {
                   object storage = store_clipboard();

                   Automation.sendkeys("^c");
                   builder.Append(Clipboard.GetText());
                   restore_clipboard(storage);
               }
               catch (Exception ex)
               {
                   Feedback.print(ex);
               }
           });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Name = "send_text";

            StringBuilder result = new StringBuilder();
            thread.Start(result);
            System.Threading.Thread.Sleep(150);
            return result.ToString();
        }

        private bool window_match(Sentence result, Window_Info info)
        {
            foreach (Token parameter in result.parameters["words"])
            {
                if (!info.title.ToLower().Contains(parameter.text.ToLower()))
                    return false;
            }

            Windows.set_foreground_window(info.handle);
            Windows.bring_window_to_front(info.handle);

            return true;
        }

        public void switch_window(Sentence result)
        {
            List<Window_Info> windows = Windows.enum_windows();
            foreach (Window_Info info in windows)
            {
                if (window_match(result, info))
                    return;
            }

        }

        public void initialize_external_programs(Familiar_Document new_library)
        {
            Familiar_Document library = new_library;
            Element_Choice choices = library.get_rule_choice("programs");
            choices.children.Clear();
            programs.Clear();

            XmlDocument document = new XmlDocument();
            document.Load(Global.configuration.settings_path("external_programs.xml"));
            foreach (XmlElement element in document.ChildNodes[1].ChildNodes)
            {
                Target_Path program = new Target_Path();
                program.path = element.GetAttribute("path");
                program.arguments = get_element_text(element, "arguments");
                program.working_directory = get_element_text(element, "directory");
                string command = get_element_text(element, "command");
                programs.Add(command, program);
                Element_Base new_item = choices.create_child(typeof(Element_Item));
                new_item.text = command;
            }
        }

        public string get_element_text(XmlElement parent, string tag)
        {
            XmlNodeList children = parent.GetElementsByTagName(tag);
            if (children.Count == 0)
                return "";

            return children[0].InnerText;
        }


        public void run_program(Sentence result)
        {
            string command = result.parameter["path"].text;

            if (programs.ContainsKey(command))
            {
                ProcessStartInfo program = programs[command].get_process_start_info();
                program.FileName = program.FileName.Replace("%SystemRoot%", Environment.SystemDirectory);

                System.Diagnostics.Process.Start(program);
            }
        }

    }
}
