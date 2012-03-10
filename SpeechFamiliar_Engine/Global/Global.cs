using System;
using System.Collections.Generic;
using System.Text;
using SilentOrb.Reflection;
using SpeechFamiliar.Forms;
using SilentOrb.Utility;

namespace SpeechFamiliar
{
    public static class Global
    {
        static PluginHost plugin_manager;

        public static Configuration configuration = new Configuration();
        public static Dictionary<string, Plugin> plugins = new Dictionary<string, Plugin>();
        static public Form1 Main_Window;
        //#if DEBUG
        static public Speech_Engine speech;
        static public object thread_control = new object();
        static public bool thread_busy = false;
        static public string debug_string = "";

        static Global()
        {
            plugin_manager = MetaHub.View.Manager.View_Manager.create_plugin_manager();

            plugin_manager.add_type(typeof(Plugin), Global.plugins);
            plugin_manager.add_type(typeof(Context), Context.types);
            plugin_manager.add_type(typeof(Action), Action.types);

            //plugin_manager.add_type(typeof(Familiar_Word.Filter), Familiar_Word.Filter.filters);
        }

        public static void initialize()
        {
            configuration.load();
            Main_Window = new Form1();
            speech = new Speech_Engine();
            speech.Init();
        }

        static internal void load_plugins()
        {
            plugin_manager.add_local_assembly();

            plugin_manager.folders.Add(configuration.plugins_path);
      //      plugin_manager.folders.Add(configuration.MetaHub_path + "\\plugins");
            plugin_manager.load_plugins();
            //MetaHub.Core.Engine.path = configuration.MetaHub_path + "\\";
          //  MetaHub.Core.Engine.initialize(plugin_manager);

            if (Context.types.ContainsKey("default"))
                SpeechFamiliar.Scripting.Automation.default_context = Activator.CreateInstance(Context.types["default"]) as Context;
        }

        static public void init()
        {
            //#if DEBUG
            //#else
            //                        speech = new SpeechEngine(Main_Window);
            //#endif
#if !DEBUG
            try
            {
#endif
                //SpeechFamiliar.Plugins.PluginHost.Load(Global.configuration.plugins_path);
                load_plugins();
                speech.LoadInitialGrammars();
                //SpeechFamiliar.Scripting.Automation.controller = new SpeechFamiliar.UI.Controller();
                Global.Main_Window.Start();

#if !DEBUG
            }
            catch (Exception ex)
            {
                SilentOrb.Utility.Feedback.print(ex);
                return;
            }
#endif
            thread_busy = false;
            Feedback.print("\r\nFinished loading.\r\n", Feedback.Status.story);
        }

    }
}
