using System;
using System.Collections.Generic;
using System.Text;
using SpeechFamiliar;
using SilentOrb.Reflection; 
//using SpeechFamiliar.Plugins;

namespace Visual_Studio
{
    [PluginInfo("vs")]
    public class Visual_Studio_Step : Action
    {
        public override Sentence run(Sentence result)
        {
            try
            {
                visual_studio.studio.ExecuteCommand(text, "");
            }
            catch (Exception exception)
            {
                SilentOrb.Utility.Feedback.print(exception);
            }

            return null;
        }
    }
}
