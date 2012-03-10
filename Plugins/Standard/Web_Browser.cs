using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using SpeechFamiliar.Scripting;
using SpeechFamiliar;
using SilentOrb.Utility;
using System.Xml;
using System.Diagnostics;
using SilentOrb.Reflection;
using System.Runtime.InteropServices;
using SpeechFamiliar.Forms;

namespace SpeechFamiliar.Plugins
{
    [PluginInfo("web_browser")]
    public class web_browser : Plugin
    {
        Dictionary<string, Target_Path> pages = new Dictionary<string, Target_Path>();
        Dictionary<string, Target_Path> folders = new Dictionary<string, Target_Path>();
        //Action_Library2 library;
        Element_Choice choices;

        public void initialize_web_pages(string new_grammar, string file_name)
        {
            initialize(new_grammar, file_name, pages);
        }

        public void initialize_folders(string new_grammar, string file_name)
        {
            initialize(new_grammar, file_name, folders);
        }

        private void initialize(string new_grammar, string filename, Dictionary<string, Target_Path> target)
        {
            //file_name = Global.configuration.settings_path(file_name);
            target.Clear();

            Global.configuration.load_block(filename, (file_name) =>
            {
                try
                {
                    Familiar_Document grammar_document = Familiar_Document.find_document(new_grammar);
                    if (grammar_document == null)
                        throw new Exception("Could not find Grammar!");

                    choices = grammar_document.get_rule_choice("pages");
                    //choices.children.Clear();
                    
                    //if (!File.Exists(file_name))
                    //    throw new Exception("Could not find File!");

                    XmlDocument document = new XmlDocument();
                    document.Load(file_name);
                    foreach (XmlElement element in document.ChildNodes[1].ChildNodes)
                    {
                        Target_Path program = new Target_Path();
                        program.path = element.GetAttribute("path");
                        program.command = element.FirstChild.InnerText;
                        if (target.ContainsKey(program.command))
                            continue;
                        target.Add(program.command, program);
                        Element_Base new_item = choices.create_child(typeof(Element_Item));
                        new_item.text = program.command;
                    }
                }
                catch
                {
                }
            });
        }

        public void goto_folder(Sentence result)
        {
            goto_target(result, folders);
        }

        public void goto_page(Sentence result)
        {
            goto_target(result, pages);
        }

        protected void goto_target(Sentence result, Dictionary<string, Target_Path> target)
        {
            try
            {
                string url = result.parameter["address"].text;
                //    for (int x = 1; x < parameters.Count; ++x)
                //        url += " " + parameters[x];

                if (target.ContainsKey(url))
                {
                    Automation.sendkeys("%d");
                    System.Threading.Thread.Sleep(10);
                    Automation.sendkeys("^a");
                    //Automation.sendkeys("{ESC}");
                    //System.Threading.Thread.Sleep(500);
                    Automation.execute("standard.send_text", target[url].path);
                    System.Threading.Thread.Sleep(50);
                    Automation.sendkeys("{ENTER}");
                }
            }
            catch (Exception ex)
            {
                Feedback.print(ex);
            }

        }

        public void insert_folder(Sentence result)
        {
            insert_target(result, folders);
        }

        protected void insert_target(Sentence result, Dictionary<string, Target_Path> target)
        {
            try
            {
                string url = result.parameter["address"].text;

                if (target.ContainsKey(url))
                {
                    Automation.execute("standard.send_text", target[url].path);
                    System.Threading.Thread.Sleep(50);
                    //         Automation.sendkeys("{ENTER}");
                }
            }
            catch (Exception ex)
            {
                Feedback.print(ex);
            }

        }

        public bool add_page()
        {
            try
            {

                Target_Path target = new Target_Path();
                //InternetExplorer explorer = get_browser();
                //if (explorer == null)
                //    return false;

                target.path = (string)Automation.execute("standard.grab_text");

                Generic_Form form = new Generic_Form(target, "new web page");

                if (form.ShowDialog() == DialogResult.OK)
                {
                    pages.Add(target.command, target);
                    save_dictionary(pages, Global.configuration.settings_path("web_pages.xml"));
                    Automation.reload();
                    return true;
                }

            }
            catch (Exception ex)
            {
                Feedback.print(ex);
            }
            return false;
        }

        public bool add_folder()
        {
            try
            {

                Target_Path target = new Target_Path();

                Automation.sendkeys("%d^a");
                target.path = (string)Automation.execute("standard.grab_text");

                Generic_Form form = new Generic_Form(target, "new folder");

                if (form.ShowDialog() == DialogResult.OK)
                {
                    folders.Add(target.command, target);
                    save_dictionary(folders, Global.configuration.settings_path("folders.xml"));
                    Automation.reload();
                    return true;
                }

            }
            catch (Exception ex)
            {
                Feedback.print(ex);
            }
            return false;
        }

        protected void save_dictionary(Dictionary<string, Target_Path> list, string filename)
        {
            XmlDocument writer = new XmlDocument();
            writer.AppendChild(writer.CreateXmlDeclaration("1.0", Encoding.Unicode.EncodingName, ""));

            XmlElement node = writer.CreateElement("list");
            writer.AppendChild(node);

            foreach (Target_Path child in list.Values)
            {
                node.AppendChild(child.save(writer));
            }

            XmlTextWriter text_writer = new XmlTextWriter(filename, Encoding.Unicode);
            text_writer.Formatting = Formatting.Indented;
            writer.WriteTo(text_writer);
            text_writer.Close();
        }

        //public InternetExplorer get_browser()
        //{
        //    ShellWindows shell = new ShellWindows();
        //    List<InternetExplorer> browsers = new List<InternetExplorer>();

        //    Window_Info foreground_window = Windows.GetForegroundWindow();
        //    List<Window_Info> children = Windows.enum_child_windows(foreground_window.handle);
        //    //       StreamWriter writer = new StreamWriter("debug/ie.txt");
        //    foreach (Window_Info child in children)
        //    {
        //        if (child.class_name == "TabWindowClass")
        //        {

        //        }
        //        //        writer.WriteLine(child.handle.ToString() + " - " + child.class_name + " - " + child.title);
        //    }

        //     writer.Close();

        //    foreach (InternetExplorer application in shell)
        //    {
        //        if (application.HWND == foreground_window.handle.ToInt32())
        //        {
        //            // if (foreground_window.title.Contains(application.LocationName))
        //            //     return application;
        //        }
        //    }

        //    return null;
        //}

        public void cd_target(Sentence result)
        {
            try
            {
                string url = result.parameter["address"].text;

                if (folders.ContainsKey(url))
                {
                    //string path = folders[url].path;
                    //string command = path[0] + ":{ENTER}";
                    //Automation.sendkeys(command);
                    //command = @"cd\" + path.Substring(3) + "{ENTER}";
                    string command = "cd \"" + folders[url].path + "\"{ENTER}";
                    Automation.sendkeys(command);
                }
            }
            catch (Exception ex)
            {
                Feedback.print(ex);
            }

        }

    }
}
