using System;
using System.Collections.Generic;
using System.Text;
using SpeechFamiliar.Scripting;
using SpeechFamiliar;
using System.IO;
using Ruby;
using SilentOrb.Utility;
using SilentOrb.Reflection;

namespace SpeechFamiliar.Plugins
{
    [PluginInfo("ruby")]
    public class Ruby : Plugin
    {
        static public Ruby_Engine engine;
        public string result;

        public Ruby()
        {
            engine = new Ruby_Engine();
            //engine.require_assembly(System.Reflection.Assembly.GetExecutingAssembly());
            engine.load_assembly(System.Reflection.Assembly.GetExecutingAssembly().Location);
            //Feedback.print(Global.configuration.settings_path("MetaHub/MetaHub.rb"));
            engine.execute_file(Global.configuration.settings_path("initialize.rb"));
            engine.execute_file(Global.configuration.settings_path("MetaHub/MetaHub.rb"));
            
            foreach (string file in Directory.GetFiles(Global.configuration.settings_path("MetaHub/nodes"), "*.rb"))
            {
                engine.execute_file(file);
            }
        }
    }

    [PluginInfo("ruby")]
    public class RubyStep : Action
    {

        public override Sentence run(Sentence result)
        {
            //EngineModule module = Ruby.ruby.default_module;
            //module.Globals["grammar"] = library;
            Ruby.engine.add_variable("grammar", library);
            if (result != null)
            {
                Ruby.engine.add_variable("sentence", result);
                //Ruby.engine.add_variable("result", result.result);
            }

            //foreach (string key in Global.plugins.keys)
            //{
            //    Plugin plugin = Global.plugins[key];
            //    Ruby.engine.add_variable(key.ToLower(), plugin);
            //    //module.Globals[key.ToLower()] = plugin;
            //}
            
            object product = Ruby.engine.execute(text);
            if (product != null && product.GetType() == typeof(Sentence))
                return product as Sentence;

            return null;
            //Ruby.ruby.Run(code);
        }
    }

    [PluginInfo("node")]
    public class MetaNodeStep : Action
    {

        public override Sentence run(Sentence result)
        {
            //Ruby.engine.add_variable("grammar", library);
            //if (result != null)
            //{
            //    Ruby.engine.add_variable("word", result);
            //    Ruby.engine.add_variable("result", result.result);
            //}

            foreach (string key in Global.plugins.Keys)
            {
                Plugin plugin = Global.plugins[key];
                Ruby.engine.add_variable(key.ToLower(), plugin);
            }

            Ruby.engine.execute_thread("puts MetaNode.run_node('" + text + "')");
            return null;
        }
    }

    //public class Ruby_Filter : Familiar_Word.Filter
    //{
    //    public string key = "";
    //   static public object test;

    //    public override void run()
    //    {
    //        Ruby.engine.execute("Filter_Manager.run '" + key + "'");
    //    }
    //}
}
