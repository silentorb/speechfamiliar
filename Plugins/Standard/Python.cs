using System;
using System.Collections.Generic;
using System.Text;
using SpeechFamiliar.Scripting;
using SpeechFamiliar;
using IronPython.Hosting;
using IronPython.Compiler;
using IronPython.Modules;
using SilentOrb.Utility;
using SilentOrb.Reflection; 

namespace SpeechFamiliar.Plugins
{
    [PluginInfo("Python")]
    public class Python : Plugin
    {
        static public Python_Adapter python = new Python_Adapter();

        public Python()
        {
            python.engine.ExecuteFile(Global.configuration.settings_path("python_init.py"), python.default_module);
        }
    }

    [PluginInfo("python")]
    public class PythonStep : Action
    {
        CompiledCode code;

        protected override void initialize()
        {
            code = Python.python.engine.Compile(text);
        }

        public override Sentence run(Sentence result)
        {
            EngineModule module = Python.python.default_module;
            module.Globals["grammar"] = library;
            if (result != null)
            {
                module.Globals["word"] = result;
                //module.Globals["result"] = result.result;
            }

            foreach (string key in Global.plugins.Keys)
            {
                Plugin plugin = Global.plugins[key];
                module.Globals[key.ToLower()] = plugin;
            }

            //Feedback.print(text);
            Python.python.Run(code);
            return null;
        }
    }

}
