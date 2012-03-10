using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Xml;
using System.Text.RegularExpressions;
using System.Web;
using System.IO;
using SpeechFamiliar.Forms;

namespace SpeechFamiliar
{
    //public enum Element_Type
    //{
    //    none = 0,
    //    choice,
    //    tag,
    //    item,
    //    reference,
    //    dictation
    //}

    public class Element_Base
    {
        //Object element;
        //public bool repeat = false;

        //public List<Familiar_Word.Filter> filters = new List<Familiar_Word.Filter>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public int min = 1;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public int max = 1;

        public override string ToString()
        {
            if (name.Length > 0)
                return "Element_Base - " + GetType().Name + " (" + name + ")";

            return "Element_Base - " + GetType().Name;
        }

        //public bool is_root
        //{
        //    get 
        //    {
        //        if (this.GetType() == typeof(Element_Item) && parent == null)
        //            return true;

        //        return false;
        //    }
        //}	

        protected virtual void initialize(XmlElement element)
        {

        }

        public void add(Element_Base new_child)
        {
            children.Add(new_child);
            new_child.parent = this;
            new_child.document = document;
            //new_child.on_parented();
        }

        //public Old_Action action
        //{
        //    get { return null; }
        //}

        //public Word_Options options = new Word_Options();
        public Element_Base parent;
        public List<Element_Base> children = new List<Element_Base>();

        [UIAttribute("Text", order = 0)]
        public string text = "";
        public string name = "";
        public string key = "";
        public float weight = 1.0f;

        public string word_text
        {
            get
            {
                if (text != "")
                    return text;

                string result = "";
                foreach (Element_Base child in children)
                {
                    if (child.GetType() == typeof(Element_Item))
                        result += child.text;
                }

                return result;
            }
        }

        public Element_Base create_child(Type type)
        {
            Element_Base result = (Element_Base)System.Activator.CreateInstance(type);
            add(result);
            //result.initialize(element);
            return result;
        }

        public Element_Base create_child(XmlElement element)
        {
            Element_Base result = null;

            switch (element.Name.ToLower())
            {
                case "rule":
                    {
                        result = new Element_Item();
                        document.all_rules.Add((Element_Item)result);
                        break;
                    }
                case "ruleref":
                case "reference":
                    result = new Element_Reference();
                    break;

                case "tag":
                    result = new Element_Tag();
                    break;

                case "item":
                    result = new Element_Item();
                    if (result.name != "")
                        document.all_rules.Add((Element_Item)result);
                    break;

                case "one-of":
                case "choice":
                    result = new Element_Choice();
                    break;

                case "dictation":
                    result = new Element_Dictation();
                    break;

                case "word":
                    result = new Element_Word();
                    break;

                //case "filter":
                //    if (Familiar_Word.Filter.filters.has_key(element.InnerText))
                //        filters.Add(Familiar_Word.Filter.filters[element.InnerText]);
                //    return null;
                default:
                    return null;
            }

            add(result);
            result.initialize(element);
            return result;
        }

        public Familiar_Document document;

        //protected virtual void on_parented()
        //{
        //}

        public virtual bool match(string text)
        {
            return text == word_text;
        }

        public List<Element_Base> get_children(Type child_type)
        {
            List<Element_Base> result = new List<Element_Base>();

            foreach (Element_Base child in children)
            {
                if (child.GetType() == child_type)
                    result.Add(child);
            }

            return result;
        }

        public virtual void load(XmlElement element)
        {

            if (element.HasAttribute("weight"))
            {
                weight = float.Parse(element.GetAttribute("weight"));
            }

            if (element.GetAttribute("repeat").Length > 0)
            {
                string repeat = element.GetAttribute("repeat");
                string[] values = repeat.Split('-');
                min = int.Parse(values[0].Trim());
                max = int.Parse(values[1].Trim());
            }

            key = get_attribute(element, "parameter");
        }

        protected void load_children(XmlElement element)
        {
            foreach (XmlNode child in element.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Element)
                {
                    create_child((XmlElement)child);
                }
                else if (child.NodeType == XmlNodeType.Text)
                {
                    Element_Item item = (Element_Item)create_child(typeof(Element_Item));
                    item.text = child.InnerText.Trim();
                    //    word += child.InnerText.Trim();
                }
            }
        }

        protected string get_attribute(XmlElement element, string key)
        {
            if (element.HasAttribute(key))
            {
                return element.GetAttribute(key);
            }

            return "";
        }

        protected bool get_boolean_attribute(XmlElement element, string key, bool default_value)
        {
            if (element.HasAttribute(key))
            {
                bool result = default_value;
                if (bool.TryParse(element.GetAttribute(key), out result))
                    return result;
            }

            return default_value;
        }

        protected void add_attribute(XmlElement node, string key, string value)
        {
            if (value == null || value == "")
                return;

            node.SetAttribute(key, value);

        }

        public virtual XmlElement create_grammar(XmlDocument writer)
        {
            return null;
        }

        protected XmlElement create_grammar(XmlElement result, XmlDocument writer)
        {
            if (key.Length > 0)
            {
                //result = new SemanticResultKey(key, result);
                result.SetAttribute("propname", key);
                return result;
            }

            return result;
        }

        internal virtual XmlElement save(XmlDocument writer)
        {
            XmlElement result = writer.CreateElement(GetType().Name.Substring(8).ToLower());
            add_attribute(result, "parameter", key);
            if (min != 1 || max != 1)
                result.SetAttribute("repeat", min.ToString() + "-" + max.ToString());

            //foreach (var filter in filters)
            //{
            //    XmlElement filter_element = writer.CreateElement("filter");
            //    filter_element.Value = filter.name;
            //    result.AppendChild(filter_element);
            //}

            return result;
        }

    }

    public class Element_Reference : Element_Base
    {
        public uint id;

        protected override void initialize(XmlElement element)
        {
            load(element);

            string reference = element.GetAttribute("uri");
            text = reference;

            if (reference[0] == '#')
            {
                document.all_local_references.Add(this);
            }
            else
            {
                string[] parameters = reference.Split(new char[] { '#' });
                if (parameters.Length > 1)
                {
                    reference = parameters[0];
                }

                Familiar_Document external_document = document.add_external_document(reference);
                if (!external_document.dependants.ContainsKey(key))
                    external_document.dependants.Add(key, document);

                if (!document.referenced_documents.Contains(external_document))
                    document.referenced_documents.Add(external_document);

                if (parameters.Length > 1)
                {
                    foreach (Element_Base rule in external_document.all_rules)
                    {
                        if (rule.name == parameters[1])
                        {
                            children.Add(rule);
                            return;
                        }
                    }

                    throw new Exception("External rule was not found!");
                }
                else
                {
                    children.Add(external_document.rules[0]);
                }
            }
        }

        public override XmlElement create_grammar(XmlDocument writer)
        {
            string reference_key = children[0].document.name;
            string target = Global.configuration.temp_path + "/" + reference_key + ".xml";

            if (!File.Exists(target) && reference_key != document.name)
                Familiar_Document.all_documents[reference_key].create_cfg_file(); //.create_grammar(parent.engine);

            XmlElement result = writer.CreateElement("RULEREF");
            result.SetAttribute("name", children[0].name);
            if (reference_key != document.name)
                result.SetAttribute("url", reference_key + ".xml");

            if (key.Length > 0)
            {
                id = document.item_id;
                document.items.Add(this);
                ++document.item_id;

                XmlElement result_wrapper = writer.CreateElement("PHRASE");
                result_wrapper.AppendChild(result);

                string reference_display = "::" + document.name + "." + id.ToString();

                result_wrapper.SetAttribute("propname", "#word");
                result_wrapper.SetAttribute("valstr", reference_display);

                return result_wrapper;
            }

            return result;
        }

        internal override XmlElement save(XmlDocument writer)
        {
            XmlElement result = base.save(writer);
            result.SetAttribute("uri", text);
            return result;
        }
    }

    public class Element_Choice : Element_Base
    {
        public uint id;

        public void initialize(Familiar_Document new_document)
        {
            document = new_document;
        }

        protected override void initialize(XmlElement element)
        {
            load(element);
            load_children(element);
        }

        public override XmlElement create_grammar(XmlDocument writer)
        {
            XmlElement result = writer.CreateElement("LIST");

            foreach (Element_Base child in children)
            {
                XmlElement item = child.create_grammar(writer);
                if (item != null)
                    result.AppendChild(item);
            }

            if (key.Length > 0)
            {
                id = document.item_id;
                document.items.Add(this);
                ++document.item_id;
                XmlElement result_wrapper = writer.CreateElement("PHRASE");
                result_wrapper.AppendChild(result);

                string reference_display = "::" + document.name + "." + id.ToString();

                result_wrapper.SetAttribute("propname", "#word");
                result_wrapper.SetAttribute("valstr", reference_display);

                return result_wrapper;
            }

            //   result = create_grammar(result, writer);

            return result;
        }

        internal override XmlElement save(XmlDocument writer)
        {
            XmlElement result = base.save(writer);

            //if (text != "")
            //{
            //    result.AppendChild(writer.CreateTextNode(text));
            //}

            foreach (Element_Base child in children)
            {
                result.AppendChild(child.save(writer));
            }

            return result;
        }
    }

    public class Element_Dictation : Element_Base
    {
        protected override void initialize(XmlElement element)
        {
            load(element);
        }

        public override XmlElement create_grammar(XmlDocument writer)
        {
            XmlElement result = writer.CreateElement("dictation");

            if (min != 1)
                result.SetAttribute("min", min.ToString());

            if (max != 1)
                result.SetAttribute("max", max.ToString());

            return result;
        }
    }

    public class Element_Tag : Element_Base
    {
        //public bool is_global = false;

        protected override void initialize(XmlElement element)
        {
            load(element);
            //is_global = base.get_boolean_attribute(element, "global", false);

            if (element.HasAttribute("type"))
            {
                name = element.GetAttribute("type");
                text = element.InnerText;
            }
            else if (element.ChildNodes.Count == 1)
            {
                string[] parameters = element.InnerText.Split(new char[] { '=' });
                name = parameters[0];
                text = parameters[1].Trim('"');
            }
            else if (element.ChildNodes.Count > 1)
            {
                name = Regex.Match(element.ChildNodes[0].InnerText, @"[^\s=]+(?==)").Groups[0].Value;
                text = element.ChildNodes[1].Value;
            }
            else
            {
                throw new Exception("Improper format for tag.");
            }

        }

        internal override XmlElement save(XmlDocument writer)
        {
            XmlElement result = base.save(writer);

            if (text != "")
            {
                if (text.Contains("\n"))
                {
                    result.AppendChild(writer.CreateTextNode(name + "="));
                    result.AppendChild(writer.CreateCDataSection(text));
                }
                else
                    result.AppendChild(writer.CreateTextNode(name + "=" + text));
            }

            return result;
        }

    }

}
