using System;
using System.Collections.Generic;
using System.Text;
using SpeechFamiliar.Scripting;
using SpeechFamiliar;
using SilentOrb.Utility;
using System.Windows.Forms;
using System.Reflection;
using SilentOrb.Reflection;

namespace SpeechFamiliar.Plugins
{
    [PluginInfo("sendkeys")]
    public class SendKeyStep : Action
    {
        public override Sentence run(Sentence result)
        {
            Automation.sendkeys(text);
            return null;
        }
    }

    [PluginInfo("plugin")]
    public class PluginStep : Action
    {
        MethodInfo method = null;
        Plugin plugin;

        protected override void initialize()
        {
            string[] info = text.Split(new char[] { '.' });
            if (info.Length > 1)
            {
                if (Global.plugins.ContainsKey(info[0]))
                {
                    plugin = Global.plugins[info[0]];
                    //try
                    //{
                    method = plugin.GetType().GetMethod(info[1]);
                    //   }
                    //catch(AmbiguousMatchException)
                    //{
                    //    foreach (var item in plugin.GetType().GetMethods())
                    //    {

                    //        if (item.Name == info[1])
                    //        {
                    //            var parameters = item.GetParameters();
                    //            if (parameters.Length > 0 && parameters[0].ParameterType == typeof(Sentence))
                    //            {
                    //                method = item;
                    //                break;
                    //            }
                    //        }
                    //    }                        
                    //}

                    //    object result = plugin.get_method(info[1]);
                    //    Feedback.print(result.GetType().Name);
                    //}
                }
                else
                    throw new Exception("Plugin not found: " + info[0] + ".");

                if (method == null)
                {
                    throw new Exception("Method not found: " + text);
                }
            }
        }

        public override Sentence run(Sentence result)
        {

            if (method != null)
            {
                try
                {
                    ParameterInfo[] parameters = method.GetParameters();
                    if (parameters.Length > 0)
                    {
                        //if (parameters[0].ParameterType == typeof(Familiar_Word))
                        return (Sentence)method.Invoke(plugin, new object[] { result });
                        //else
                        //    method.Invoke(plugin, new object[] { result.result });
                    }
                    else
                        return (Sentence)method.Invoke(plugin, null);
                }
                catch (Exception exception)
                {
                    Feedback.print(exception);
                }
            }
            //else
            //{
            //    Ruby.engine.add_variable("sentence", result);
            //    object product = Ruby.engine.execute(text + "(sentence)");
            //    if (product != null && product.GetType() == typeof(Sentence))
            //        return product as Sentence;
            //}

            return null;
        }
    }

}
