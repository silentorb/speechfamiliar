using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Xml;
using SpeechFamiliar.Forms;

namespace SpeechFamiliar
{
    public class Target_Path
    {
        public string path = "";
        public string arguments = "";
        public string working_directory = "";

        [UIAttribute("&Command")]
        public string command = "";

        public ProcessStartInfo get_process_start_info()
        {
            ProcessStartInfo program = new ProcessStartInfo();
            program.FileName = path;
            program.Arguments = arguments;
            program.WorkingDirectory = working_directory;

            return program;
        }

        public XmlElement save(XmlDocument document )
        {
            XmlElement result = document.CreateElement("application");
            save_member(command, "command", result, document);
            save_member(arguments, "arguments", result, document);
            save_member(working_directory , "directory", result, document);
            result.SetAttribute("path", path);

            return result;
        }

        private void save_member(string source,string name,XmlElement destination,XmlDocument document)
        {
            if (source == "")
                return;

            XmlElement child = document.CreateElement(name);
            child.InnerText = source;
            destination.AppendChild(child);
        }
         
    }
}
