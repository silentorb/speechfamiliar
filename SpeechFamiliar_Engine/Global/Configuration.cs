using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using SilentOrb.Xml;
using System.IO;

namespace SpeechFamiliar
{
    public class Configuration
    {
        string _default_path = "default";
        [Xml_Binding()]
        public string default_path
        {
            get { return get_path(_default_path); }
            set { _default_path = value; }
        }

        string _log_path = "logs";
        [Xml_Binding()]
        public string log_path
        {
            get { return get_path(_log_path); }
            set { _log_path = value; }
        }

        string _plugins_path = "plugins";
        [Xml_Binding()]
        public string plugins_path
        {
            get { return get_path(_plugins_path); }
            set { _plugins_path = value; }
        }

        string _temp_path = "temp";
        [Xml_Binding()]
        public string temp_path
        {
            get { return get_path(_temp_path); }
            set { _temp_path = value; }
        }

        string _users_path = "users";
        [Xml_Binding()]
        public string users_path
        {
            get { return get_path(_users_path); }
            set { _users_path = value; }
        }

        string _root_path = "";

        [Xml_Binding()]
        public string root_path
        {
            get { return _root_path; }
            set { _root_path = value; }
        }

        [Xml_Binding()]
        public string default_user = "";

        [Xml_Binding()]
        public string  MetaHub_path { get; set; }

        public User current_user = new User();
        internal void load()
        {
            try
            {
                var loader = Xml_Member.create(this);
                Directory.SetCurrentDirectory(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase.Replace("file:///", "")));
                loader.load_from_file("../config.xml");


                _root_path = _root_path.Replace("\\", "/");

                if (_root_path.Length > 0)
                {
                    Directory.SetCurrentDirectory(_root_path);
                    _root_path = "";
                }

                if (Directory.Exists(users_path))
                {
                    foreach (var directory in Directory.GetDirectories(users_path))
                    {
                        string directory_name = Path.GetFileName(directory);
                        if (directory_name != default_user)
                            continue;

                        current_user = new User(directory_name, directory.Replace("\\", "/"));
                        break;
                    }
                }
            }
            catch
            {
            }
        }

        private string get_path(string path)
        {
            path = path.Replace("\\", "/");

            if (Path.IsPathRooted(path))
                return path;

            if (_root_path == "")
            {
                return "../" + path;
            }

            return _root_path + "/" + path;
        }

        public string settings_path(string file_name)
        {
            if (current_user.path != "")
            {
                if (File.Exists(current_user.path + "/merge/" + file_name))
                    return current_user.path + "/merge/" + file_name;
                else if (File.Exists(current_user.path + "/replace/" + file_name))
                    return current_user.path + "/replace/" + file_name;
            }

            return default_path + "/" + file_name;
        }

        public delegate void Load_File_Delegate(string file_name);

        public void load_block(string file_name, Load_File_Delegate block)
        {
            if (current_user.path != "")
            {
                if (File.Exists(current_user.path + "/merge/" + file_name))
                {
                    block.Invoke(current_user.path + "/merge/" + file_name);
                }
                else if (File.Exists(current_user.path + "/replace/" + file_name))
                {
                    block.Invoke(current_user.path + "/replace/" + file_name);
                    return;
                }
            }

            block.Invoke(default_path + "/" + file_name);
        }

    }
}
