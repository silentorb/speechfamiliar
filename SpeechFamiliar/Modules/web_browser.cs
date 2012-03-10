using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Speech.Recognition.SrgsGrammar;
using System.IO;
using SpeechFamiliar.Scripting;
using SpeechFamiliar;

namespace SpeechFamiliar.Plugins
{
    public class web_browser : Plugin
    {
        Dictionary<string, string> pages = new Dictionary<string, string>();
        Action_Library grammar;
        SrgsOneOf choices;

        public override string Name
        {
            get { return "Web Browser"; }
        }

        public override void Init()
        {
            grammar = Program.speech.vocabularies["web_browser"];
            SrgsRule rule = grammar.Get_Rule("pages");
            foreach (SrgsElement element in rule.Elements)
            {
                if (element.GetType() == typeof(SrgsOneOf))
                {
                    choices = (SrgsOneOf)element;
                    break;
                }
            }

            if (choices == null)
            {
                choices = new SrgsOneOf();
                rule.Add(choices);
            }

            if (!File.Exists("web_pages.txt"))
                initialize_page("google", "http://www.google.com");
            else
            {
                string[] lines = File.ReadAllLines("web_pages.txt");
                foreach (string line in lines)
                {
                    string[] info = System.Text.RegularExpressions.Regex.Split(line, @"\s*,\s*");
                    initialize_page(info[0], info[1]);
                }
            }

            grammar.Update();
        }

        public void initialize_page(string name, string url)
        {
            choices.Add(new SrgsItem(name));
            pages.Add(name, url);
        }

        public void goto_page(List<string> parameters)
        {
            try
            {
                string url = parameters[0];
                for (int x = 1; x < parameters.Count; ++x)
                    url += " " + parameters[x];

                if (pages.ContainsKey(url))
                {
                    //   
                    SendKeys.SendWait("%d");
                    SendKeys.SendWait("{ESC}");
                    //System.Threading.Thread.Sleep(500);
                    Automation.send_text(pages[url]);
                    SendKeys.Send("{ENTER}");
                }
            }
            catch
            {
            }

        }

        public void add_page()
        {
            try
            {
                SendKeys.Send("%d");
                SendKeys.Send("{ESC}");
                string url = Automation.grab_text();

                string name = Automation.text_dialog("Add New Web Page");

                if (name != "")
                {
                    initialize_page(name, url);

                    StreamWriter writer = new StreamWriter("web_pages.txt");
                    foreach (string key in pages.Keys)
                        writer.WriteLine(key + ", " + pages[key]);
                    writer.Close();

                    grammar.Update();
                }
            }
            catch
            {
            }
        }
    }
}
