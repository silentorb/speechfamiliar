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
using System.Xml;
using System.Collections;
using SilentOrb.Reflection;
using SpeechFamiliar.Forms;
using System.Windows.Forms;

namespace Visual_Studio
{
    [PluginInfo("visual_studio")]
    public class visual_studio : Plugin
    {
        static internal DTE2 studio;
        int process = 0;

        public visual_studio()
        {
            Automation.Active_Window_Changed += new Automation.Window_Event(on_window_changed);
        }

        public void initialize_constructor()
        {
            TextSelection selection = studio.ActiveDocument.Selection as TextSelection;
            CodeClass2 class_object = (CodeClass2)selection.ActivePoint.get_CodeElement(vsCMElement.vsCMElementClass);
            CodeFunction2 constructor = class_object.AddFunction(class_object.Name, vsCMFunction.vsCMFunctionConstructor,
                   vsCMTypeRef.vsCMTypeRefVoid, -1, vsCMAccess.vsCMAccessPublic, 0) as CodeFunction2;

            string text = "";

            foreach (CodeElement2 member in class_object.Members)
            {
                if (member.Kind == vsCMElement.vsCMElementVariable)
                {
                    CodeVariable2 variable = member as CodeVariable2;
                    CodeParameter2 parameter = constructor.AddParameter("new_" + variable.Name, variable.Type, -1) as CodeParameter2;
                    text += "\r\n" + variable.Name + " = " + parameter.Name + ";";
                }
                else if (member.Kind == vsCMElement.vsCMElementProperty)
                {
                    var variable = member as CodeProperty;
                    // CodeTypeRef new_type =
                    CodeParameter2 parameter = constructor.AddParameter("new_" + variable.Name, variable.Type, -1) as CodeParameter2;
                    text += "\r\n" + variable.Name + " = " + parameter.Name + ";";
                }
            }

            EditPoint2 point = constructor.EndPoint.CreateEditPoint() as EditPoint2;
            point.LineUp(1);
            point.Insert(text);
            selection.MoveToPoint(constructor.StartPoint, false);
            selection.MoveToPoint(constructor.EndPoint, true);
            selection.SmartFormat();
            selection.MoveToPoint(point, false);
        }

        public DTE2 get_studio()
        {
            return studio;
        }

        protected void on_window_changed(Window_Info window_info)
        {
            var process = System.Diagnostics.Process.GetProcessById(window_info.process_id);

            if (process.ProcessName == "devenv")
            {
                if (window_info.class_name == "wndclass_desked_gsk")
                {
                    studio = (DTE2)SilentOrb.Utility.Windows.SeekObjectInstanceFromROT("!VisualStudio.DTE.9.0:" + window_info.process_id.ToString());
                }
                else
                    studio = (DTE2)SilentOrb.Utility.Windows.SeekObjectInstanceFromROT("!VisualStudio.DTE.10.0:" + window_info.process_id.ToString());
            }
        }

        public void output_commands()
        {
            Feedback.print("Outputting...", Feedback.Status.story);
            studio = (DTE2)SilentOrb.Utility.Windows.SeekObjectInstanceFromROT("!VisualStudio.DTE.8.0:" + process.ToString());
            Dictionary<string, List<Command>> command_list = new Dictionary<string, List<Command>>();
            //System.Threading.Thread.Sleep(2000);
            foreach (Command command in studio.Commands)
            {
                // System.Threading.Thread.Sleep(300);

                string rootname = "";
                if (command.Name.Contains("."))
                    rootname = command.Name.Split('.')[0];
                else
                    rootname = "base";

                if (!command_list.ContainsKey(rootname))
                    command_list.Add(rootname, new List<Command>());
                command_list[rootname].Add(command);
            }

            Feedback.print("Saving...", Feedback.Status.story);
            XmlDocument document = new XmlDocument();
            document.AppendChild(document.CreateXmlDeclaration("1.0", Encoding.UTF8.HeaderName, null));
            XmlElement root = (XmlElement)document.AppendChild(document.CreateElement("commands"));
            foreach (string key in command_list.Keys)
            {
                XmlElement category = document.CreateElement("category");
                category.SetAttribute("name", key);
                root.AppendChild(category);

                foreach (Command command in command_list[key])
                {
                    if (command.IsAvailable)
                    {
                        XmlElement element = document.CreateElement("command");
                        if (command.Name.Contains("."))
                            element.SetAttribute("name", command.Name.Substring(command.Name.IndexOf('.') + 1));
                        else
                            element.SetAttribute("name", command.Name);

                        element.SetAttribute("available", command.IsAvailable.ToString());
                        category.AppendChild(element);
                    }
                }
            }

            document.Save("debug/visual_studio_available_commands.xml");
            Feedback.print("Success!", Feedback.Status.story);
        }

        class Regular_Expression_Dialog
        {
            [UIAttribute("&Text")]
            public string text;

            [UIAttribute("&Expression")]
            public string expression;
        }

        public void replace_regular_expression()
        {
            Regular_Expression_Dialog result = new Regular_Expression_Dialog();
            Generic_Form form = new Generic_Form(result, "new folder");

            if (form.ShowDialog() == DialogResult.OK)
            {
                TextDocument document = get_document();
            }
        }

        TextDocument get_document()
        {
            return visual_studio.studio.ActiveDocument.Object("TextDocument") as TextDocument;
        }

        public void goto_line(Sentence sentence)
        {
            var document = get_document();
            var line = int.Parse(sentence.parameter["line"].text);
            var point = document.CreateEditPoint(document.StartPoint);
            point.LineDown(line);

            //document.CreateEditPoint(.Line = 
        }


    }
}
