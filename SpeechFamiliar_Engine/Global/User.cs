using System;
using System.Collections.Generic;
using System.Text;

namespace SpeechFamiliar
{
    public class User
    {
        public string name = "default";
        private string _path = "";

        public string path
        {
            get { return _path; }
            set { _path = value; }
        }

        public User()
        {

        }

        public User(string new_name, string new_path)
        {
            name = new_name;
            _path = new_path;
        }
    }
}
