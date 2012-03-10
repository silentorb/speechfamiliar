using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE80;
using EnvDTE;
using EnvDTE90a;
using SpeechFamiliar;
using SpeechFamiliar.Scripting;
using SilentOrb.Utility;
using System.IO;
using System.Collections;
using SilentOrb.Reflection;

namespace Visual_Studio
{
    [PluginInfo("vs_context")]
    public class Visual_Studio_Context : Context
    {
        public Visual_Studio_Context(string new_name, Speech_Engine parent_engine)
            : base(new_name, parent_engine)
        {
        }

        public override string get_pretext()
        {
            try
            {
              //  string kind = visual_studio.studio.ActiveWindow.Kind;
                if (visual_studio.studio.ActiveWindow.Kind != "Document")
                    return null;

                TextDocument document = visual_studio.studio.ActiveDocument.Object("TextDocument") as TextDocument;
                EditPoint point = document.Selection.TopPoint.CreateEditPoint();
                EditPoint point2 = document.Selection.TopPoint.CreateEditPoint();
                point.StartOfLine();
                return point.GetText(point2);
            }
            catch
            {
                return null;
            }
        }

    }
}
