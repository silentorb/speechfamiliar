using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Xml;
using System.Web;
using SpeechFamiliar.Forms;

namespace SpeechFamiliar
{
    public class Element_Word : Element_Base
    {
        string _display = null;
        [UIAttribute("Display")]
         public string display
        {
            get
            {
                if (_display == null)
                    return text;

                return _display;
            }
            set
            {
                _display = value;
            }
        }

        [UIAttribute("Suffixes")]
        public List<string> suffixes = new List<string>();

        public Dictionary<string, string> properties = new Dictionary<string, string>();
        public uint id;

        public override bool match(string text)
        {
            if (text == word_text)
                return true;

            foreach (string suffix in suffixes)
            {
                if (text == word_text + suffix)
                    return true;
            }

            return false;
        }

        static List<string> internal_attributes =
            new List<string>(new string[] { "display", "id", "repeat", "weight", "parameter" });

        protected override void initialize(XmlElement element)
        {
            load(element);

            name = get_attribute(element, "id");
            //weight = get_attribute(element, "weight");

            foreach (XmlAttribute attribute in element.Attributes)
            {
                if (!internal_attributes.Contains(attribute.Name))
                    properties.Add(attribute.Name, attribute.Value);
            }

            foreach (XmlNode child in element.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Element)
                {
                    XmlElement child_element = child as XmlElement;
                    if (child_element.Name.ToLower() == "suffix")
                        suffixes.Add(child_element.InnerText);
                    else
                        create_child(child_element);
                }
                else if (child.NodeType == XmlNodeType.Text)
                {
                    text = child.Value.Trim();
                }
            }

            if (element.HasAttribute("display"))
                display = element.GetAttribute("display");

            register_key(display);
            foreach (var suffix in suffixes)
            {
                register_key(get_suffix(suffix));
            }
        }

        protected void register_key(string key)
        {
            var vocabulary = document.vocabulary;

            object item = vocabulary.get(key);
            if (item != null)
            {
                if (item.GetType() == typeof(Element_Word))
                {
                    List<Element_Word> words = new List<Element_Word>();
                    words.Add(item as Element_Word);
                    words.Add(this);
                    vocabulary.add(key, words);
                }
                else
                {
                    List<Element_Word> words = item as List<Element_Word>;
                    words.Add(this);
                }
            }
            else
            {
                vocabulary.add(key,this);
            }
        }

        public string get_suffix(string suffix)
        {
            if (suffix[0] == '-')
                return text + suffix.Substring(1);

            if (suffix[suffix.Length - 1] == '-')
                return suffix.Substring(0, suffix.Length - 1) + text;

            return suffix;
        }

        public override XmlElement create_grammar(XmlDocument writer)
        {
            XmlElement result = null;
            id = document.item_id;
            document.items.Add(this);
            ++document.item_id;

            string reference_display = "::" + document.name + "." + id.ToString();

            result = writer.CreateElement("PHRASE");

            if (weight != 1.0f)
                result.SetAttribute("weight", weight.ToString());

            if (suffixes.Count > 0)
            {
                XmlElement choices = writer.CreateElement("LIST");
                result.AppendChild(choices);

                XmlElement result_wrapper = writer.CreateElement("PHRASE");
                choices.AppendChild(result_wrapper);

                result_wrapper.SetAttribute("propname", "#word");
                result_wrapper.SetAttribute("valstr", reference_display);
                result_wrapper.SetAttribute("disp", reference_display);
                result_wrapper.AppendChild(writer.CreateTextNode(text));

                for (int x = 0; x < suffixes.Count; ++x)
                {
                    result_wrapper = writer.CreateElement("PHRASE");
                    choices.AppendChild(result_wrapper);

                    result_wrapper.SetAttribute("propname", "#word");
                    result_wrapper.SetAttribute("valstr", reference_display + "." + x.ToString());
                    result_wrapper.SetAttribute("disp", reference_display + "." + x.ToString());
                    result_wrapper.AppendChild(writer.CreateTextNode(get_suffix(suffixes[x])));
                }
            }
            else
            {
                result.SetAttribute("propname", "#word");
                result.SetAttribute("valstr", reference_display);

                if (text.Length > 0)
                    result.AppendChild(writer.CreateTextNode(HttpUtility.HtmlEncode(text)));
            }

            return result;
        }

        internal override XmlElement save(XmlDocument writer)
        {
            XmlElement result = base.save(writer);
            add_attribute(result, "id", name);
            if (_display != null && _display != text)
                add_attribute(result, "display", _display);

            foreach (var attribute in properties)
            {
                add_attribute(result, attribute.Key, attribute.Value);
            }

            //if (children.Count == 1 && children[0].text != "" && children[0].children.Count == 0)
            //{
            //    result.AppendChild(writer.CreateTextNode(children[0].text));
            //    return result;
            //}

            if (text != "")
            {
                result.AppendChild(writer.CreateTextNode(text));
            }

            foreach (string suffix in suffixes)
            {
                XmlElement element = writer.CreateElement("suffix");
                element.InnerText = suffix;
                result.AppendChild(element);
            }

            //foreach (Element_Base child in children)
            //{
            //    result.AppendChild(child.save(writer));
            //}

            return result;
        }


    }

}
