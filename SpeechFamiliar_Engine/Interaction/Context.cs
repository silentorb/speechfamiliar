using System;
using System.Collections.Generic;
using System.Text;
using SilentOrb.Xml;
using SilentOrb.Utility;
using SilentOrb.Reflection;
using System.Text.RegularExpressions;
using System.Diagnostics;
using SpeechFamiliar.Engine;
using MetaHub.Core.Nodes;

namespace SpeechFamiliar
{

    //public enum Program_Match
    //{
    //    title_only=0,
    //    class_only,
    //    title_and_class,
    //    title_or_class
    //}

    public class Context
    {
        public Dictionary<string, Familiar_Document> vocabularies = new Dictionary<string, Familiar_Document>();
        public Dictionary<string, Meta_Document> documents = new Dictionary<string, Meta_Document>();
        Speech_Engine engine;
        public string name;
        public List<Regex> title_patterns = new List<Regex>();
        public List<Regex> class_patterns = new List<Regex>();
        public List<string> processes = new List<string>();
        public Window_Info last_window;

        public static Dictionary<string, Type> types = new Dictionary<string, Type>();

        public Context()
        {

        }

        public Context(string new_name, Speech_Engine parent_engine)
        {
            engine = parent_engine;
            name = new_name;
        }

        public virtual void Activate()
        {
            engine.active_profile = this;
            foreach (var library in engine.vocabularies.Values)
            {
                library.grammar.enabled = vocabularies.ContainsKey(library.name);
            }

            engine.main_grammar.select_documents(documents.Keys);

            if (Global.Main_Window != null)
                engine.set_title();
        }

        public virtual bool match(Window_Info info)
        {

            foreach (string process_name in processes)
            {
                Process process = Process.GetProcessById(info.process_id);
                if (process.ProcessName.ToLower() == process_name.ToLower() ||
                    (process.ProcessName.Length > process_name.Length
                    && process.ProcessName.Substring(0,process_name.Length) == process_name
                    ))
                {
                    last_window = info;
                    return true;
                }
            }

            foreach (Regex pattern in title_patterns)
            {
                if (pattern.Match(info.title).Success)
                {
                    last_window = info;
                    return true;
                }
            }

            foreach (Regex pattern in class_patterns)
            {
                if (pattern.Match(info.class_name).Success)
                {
                    last_window = info;
                    return true;
                }
            }

            return false;
        }

        virtual public string get_pretext()
        {
            return null;
        }
    }
}
