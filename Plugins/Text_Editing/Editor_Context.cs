using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpeechFamiliar;
using SilentOrb.Reflection;

namespace Text_Editing
{
    [PluginInfo("text_editor_context")]
    public class Editor_Context : Context
    {
        public Editor_Context(string new_name, Speech_Engine parent_engine)
            : base(new_name, parent_engine)
        {
        }

        public override string get_pretext()
        {
            foreach (var window in Text_Editor.windows)
            {
                if (last_window.handle == window.handle)
                {
                    return window.get_pretext();
                }
            }

            return "";
        }
    }
}
