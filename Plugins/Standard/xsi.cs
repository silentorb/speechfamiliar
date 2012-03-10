using System;
using System.Collections.Generic;
using System.Text;
using SpeechFamiliar.Scripting;
using SpeechFamiliar;
using SilentOrb.Utility;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text.RegularExpressions;
using SilentOrb.Reflection;

namespace SpeechFamiliar.Plugins
{
    [PluginInfo("xsi")]
    public class xsi : Action
    {
        protected override void initialize()
        {
            base.initialize();
        }

        public override Sentence run(Sentence result)
        {
            string output = text;
            foreach (string key in result.parameters.keys)
            {
                //output = output.Replace("%" + key + "%", result.parameters[key].final_word.text);
            }
            File.WriteAllText("c:\\xsi.txt", output);
            Automation.sendkeys("^%1");
            return null;
        }
    }

    [PluginInfo("xsi_view")]
    public class XSI_View_Settings : Context
    {
        public XSI_View_Settings(string new_name, Speech_Engine parent_engine)
            : base(new_name, parent_engine)
        {
        }

        public override bool match(Window_Info info)
        {
            //Window_Info info = Windo
            //Control control= Control.FromChildHandle(info2.handle);

            if (processes.Count > 0)
            {
                bool progress = false;
                foreach (string process_name in processes)
                {
                    Process process = Process.GetProcessById(info.process_id);
                    if (process.ProcessName == process_name)
                    {
                        progress = true;
                        break;
                    }
                }

                if (!progress)
                    return false;
            }

            List<Window_Info> children = Windows.enum_child_windows(info.handle);

            foreach (Window_Info child in children)
            {
                foreach (Regex pattern in title_patterns)
                {
                    if (pattern.Match(child.title).Success)
                        return true;
                }

                foreach (Regex pattern in class_patterns)
                {
                    if (pattern.Match(child.class_name).Success)
                        return true;
                }
            }

            return false;
        }
    }
}
