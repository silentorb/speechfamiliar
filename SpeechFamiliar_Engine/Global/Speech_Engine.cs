using System;
using System.Collections.Generic;
using System.Text;
//using System.Speech.Recognition;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using SilentOrb.Utility;
using System.IO;
using SpeechFamiliar.Engine;
using System.Reflection;

namespace SpeechFamiliar
{
    public class Speech_Engine
    {
        public Speech_Recognizer engine = new Speech_Recognizer();
        protected Dictionary<string, Context> applications = new Dictionary<string, Context>();
        public Context active_profile;
        public Meta_Grammar main_grammar;

        private Dictionary<string, Familiar_Document> _vocabularies = new Dictionary<string, Familiar_Document>();
        public Dictionary<string, Familiar_Document> vocabularies
        {
            get { return _vocabularies; }
            set { _vocabularies = value; }
        }

        //public Dictionary<string, Familiar_Grammar> grammars { get; set; }

        public Speech_Engine()
        {
            SpeechFamiliar.Scripting.Automation.engine = this;
            //recognizer.Enabled = false;

            SpeechFamiliar.Scripting.Automation.Active_Window_Changed += new SpeechFamiliar.Scripting.Automation.Window_Event(Active_Application_Changed);

        }

        public void Init()
        {
            engine.initialize();
            main_grammar = new Meta_Grammar(engine);
        }

        protected bool _standby = false;
        public bool standby
        {
            get { return _standby; }
            set
            {
                _standby = value;
                set_title();
            }
        }

        public void set_title()
        {

            if (standby)
            {
                Global.Main_Window.manager.send("set_title", "*Speech Familiar - standby");
                Global.Main_Window.manager.send("set_icon", "sleep.ico");
            }
            else
            {
                Global.Main_Window.manager.send("set_title", "Speech Familiar (" + Global.configuration.current_user.name + ") - " + active_profile.name);
                Global.Main_Window.manager.send("set_icon", "active.ico");             
            }
        }

        public void reload()
        {
            engine.unload_all_grammars();
            //main_grammar.release();
            LoadInitialGrammars();

            Global.Main_Window.active_window = new Window_Info();
        }


        public void LoadInitialGrammars()
        {
            Feedback.print("Loading Grammar Documents...\r\n", Feedback.Status.story);
            applications.Clear();
            vocabularies.Clear();

            Global.configuration.load_block("applications.xml", (file_name) =>
                {
                    XmlDocument document = new XmlDocument();
                    document.Load(Global.configuration.settings_path(file_name));
                    foreach (XmlElement element in document.ChildNodes[1].ChildNodes)
                    {
                        try
                        {
                            if (applications.ContainsKey(element.GetAttribute("name")))
                                continue;

                            Context settings = null;
                            if (element.HasAttribute("class"))
                            {
                                settings = (Context)Activator.CreateInstance(
                                    Context.types[element.GetAttribute("class")],
                                     element.GetAttribute("name"), this);
                            }
                            else
                                settings = new Context(element.GetAttribute("name"), this);

                            XmlNodeList titles = element.GetElementsByTagName("title");
                            foreach (XmlNode title in titles)
                            {
                                settings.title_patterns.Add(new System.Text.RegularExpressions.Regex(title.InnerText));
                            }

                            XmlNodeList items = element.GetElementsByTagName("process");
                            foreach (XmlNode item in items)
                            {
                                settings.processes.Add(item.InnerText);
                            }

                            items = element.GetElementsByTagName("class");
                            foreach (XmlNode item in items)
                            {
                                settings.class_patterns.Add(new System.Text.RegularExpressions.Regex(item.InnerText));
                            }

                            XmlNodeList grammar_list = element.GetElementsByTagName("grammar");
                            foreach (XmlNode grammar_name in grammar_list)
                            {
                                if (!vocabularies.ContainsKey(grammar_name.InnerText))
                                    LoadGrammar(grammar_name.InnerText);

                                settings.vocabularies.Add(grammar_name.InnerText, vocabularies[grammar_name.InnerText]);
                            }

                            XmlNodeList second_grammar_list = element.GetElementsByTagName("new_grammar");
                            foreach (XmlNode grammar_name in second_grammar_list)
                            {
                                var key = grammar_name.InnerText;
                                //if (!main_grammar.documents.ContainsKey(key))
                                //    LoadGrammar2(key);

                                if (main_grammar.documents.ContainsKey(key))
                                    settings.documents.Add(key, main_grammar.documents[key]);
                            }

                            applications.Add(settings.name, settings);
                        }
                        catch (Exception ex)
                        {
                            Feedback.print(ex);
                        }
                    }
                });

            Feedback.print("Initializing Grammars...", Feedback.Status.story);
            Familiar_Document.finalize_all_grammars(engine);
            Feedback.print("done.\r\n", Feedback.Status.story);
            main_grammar.update();
            applications["global"].Activate();
            engine.start();

            Feedback.print("Finished loading Grammar documents.\r\n", Feedback.Status.story);
        }

        public void LoadGrammar(string grammar_name)
        {
            Feedback.print("Loading " + grammar_name + "...", Feedback.Status.story);
            Familiar_Document library = Familiar_Document.get_document(grammar_name);
            vocabularies.Add(grammar_name, library);
            Feedback.print("done.\r\n", Feedback.Status.story);
        }

        public void LoadGrammar2(string grammar_name)
        {
            //return;
            Feedback.print("Loading " + grammar_name + "...", Feedback.Status.story);
            var library = new MetaHub.Core.Nodes.Meta_Document();
            var file_name = Global.configuration.settings_path("grammars/" + grammar_name + ".mh");
            library.open(file_name);
            library.name = grammar_name;
            main_grammar.add(library);
            Feedback.print("done.\r\n", Feedback.Status.story);
        }

        public void Active_Application_Changed(Window_Info window_info)
        {
            foreach (Context setting in applications.Values)
            {

                if (setting.match(window_info))
                {
                    if (active_profile != setting)
                        setting.Activate();

                    //      mainwindow.manager.send("set_title", "Speech Familiar - " + active_profile.name + "--" + window_info.title);

                    return;
                }
            }
            //      mainwindow.manager.send("set_title", "Speech Familiar - " + active_profile.name + "--" + window_info.title);

            if (applications.ContainsKey("global") && active_profile != applications["global"])
                applications["global"].Activate();

        }


    }
}
