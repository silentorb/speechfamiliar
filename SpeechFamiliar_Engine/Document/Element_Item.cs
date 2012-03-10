using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web;
using SpeechFamiliar.Forms;

namespace SpeechFamiliar
{
    public class Element_Item : Element_Base
    {
        [UIAttribute("Display")]
        public string display = null;

        [UIAttribute("Suffixes")]
        public List<string> suffixes = new List<string>();

        public Dictionary<string, string> properties = new Dictionary<string, string>();

        public uint id;
        //private List <Action> parameters = new List <object>();
        //  _action = null;

        //public Old_Action action
        //{
        //    get { return null; }
        //}

        public List<Action> actions = new List<Action>();

        //public static void init()
        //{
        //    id = 0;
        //}

        //public void initialize()
        //{
        //    type = Element_Type.item;
        //}

        public static Element_Item create_rule(Familiar_Document new_document, XmlElement element)
        {
            Element_Item result = new Element_Item();
            result.document = new_document;
            result.initialize(element);
            return result;
        }

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

        //public void initialize(Familiar_Document new_document, string new_text)
        //{
        //    document = new_document;
        //    text = new_text;
        //    }

        static List<string> internal_attributes =
            new List<string>(new string[] { "display", "id", "repeat", "weight", "parameter" });

        protected override void initialize(XmlElement element)
        {
            load(element);

            name = get_attribute(element, "id");
            //weight = get_attribute(element, "weight");

            if (element.HasAttribute("display"))
                display = element.GetAttribute("display");

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
                    Element_Item item = (Element_Item)create_child(typeof(Element_Item));
                    item.text = child.InnerText.Trim();
                    //    word += child.InnerText.Trim();
                }
            }

            if (children.Count > 0 && children[0].GetType() == typeof(Element_Item) && children.Count == 1
                && children[0].children.Count == 0 && !document.all_rules.Contains(this))
            {
                Element_Item child = children[0] as Element_Item;

                if (child.display != null)
                    display = child.display;

                text = child.text;
                children.Clear();
            }

            //parse_properties(element);

        }

        public override XmlElement create_grammar(XmlDocument writer)
        {
            XmlElement result = null;
            id = document.item_id;
            document.items.Add(this);
            ++document.item_id;

            string reference_display = "::" + document.name + "." + id.ToString();

            if (document.all_rules.Contains(this))
            {
                result = writer.CreateElement("RULE");

                if (name == "")
                {
                    result.SetAttribute("name", id.ToString());
                }
                else
                    result.SetAttribute("name", name);

                if (name == "root")
                    result.SetAttribute("toplevel", "Active");

                result.SetAttribute("export", "true");

                XmlElement result_wrapper = result;
                if (key.Length > 0 || properties.Count > 0)// || filters.Count > 0)
                {
                    result_wrapper = writer.CreateElement("PHRASE");
                    result_wrapper.SetAttribute("propname", "#word");
                    result_wrapper.SetAttribute("valstr", reference_display);
                    result.AppendChild(result_wrapper);
                }

                foreach (Element_Base child in children)
                {
                    XmlElement item = child.create_grammar(writer);
                    if (item != null)
                        result_wrapper.AppendChild(item);
                }

                //result.SetAttribute("propname", "#word");
                //result.SetAttribute("valstr", reference_display);
                return result;
            }
            else
            {
                result = writer.CreateElement("PHRASE");

                //if (display != "")
                //    result.SetAttribute("disp", HttpUtility.HtmlEncode(display));

                if (min != 1)
                    result.SetAttribute("min", min.ToString());

                if (max != 1)
                    result.SetAttribute("max", max.ToString());

                if (weight != 1.0f)
                    result.SetAttribute("weight", weight.ToString());

                foreach (Element_Base child in children)
                {
                    XmlElement item = child.create_grammar(writer);
                    if (item != null)
                        result.AppendChild(item);
                }

                //Action _action=  actions.Add(Action.c);

                foreach (Element_Base child in children)
                {
                    if (child.GetType() == typeof(Element_Tag))
                    {
                        actions.Add(Action.create(child.name, child.text, document));
                        //if (((Element_Tag)child).is_global)
                        //    _action.is_global = true;
                    }
                }

                //if (_action.steps.Count == 0)
                //{
                //    _action = null;
                //    if (text.Length > 0 || display != null)
                //    {
                //        if (suffixes.Count == 0)
                //            result.SetAttribute("disp", reference_display);
                //    }
                //    else if (options.count == 0 && key == "")
                //    {
                //        return result;
                //    }
                //}

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
                        result_wrapper.AppendChild(writer.CreateTextNode(text + suffixes[x]));
                    }
                }
                else
                {
                    result.SetAttribute("propname", "#word");
                    result.SetAttribute("valstr", reference_display);

                    if (text.Length > 0)
                        result.AppendChild(writer.CreateTextNode(HttpUtility.HtmlEncode(text)));
                }
            }

            return result;
        }

        internal override XmlElement save(XmlDocument writer)
        {
            XmlElement result = base.save(writer);
            add_attribute(result, "id", name);
            if (display != null)
                add_attribute(result, "display", display);

            if (children.Count == 1 && children[0].text != "" && children[0].children.Count == 0)
            {
                result.AppendChild(writer.CreateTextNode(children[0].text));
                return result;
            }

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


            foreach (Element_Base child in children)
            {
                result.AppendChild(child.save(writer));
            }

            return result;
        }

        public void run_action(Sentence result)
        {
            foreach (var action in actions)
            {
                action.run(result);
            }
        }

        //protected void parse_properties(XmlElement element)
        //{
        //    if (!element.HasAttributes)
        //        return;

        //    foreach (string property_name in Enum.GetNames(typeof(Word_Options.property)))
        //    {
        //        if (get_boolean_attribute(element, property_name, false))
        //            options[(Word_Options.property)Enum.Parse(typeof(Word_Options.property), property_name)] = true;
        //    }
        //}

    }

}
