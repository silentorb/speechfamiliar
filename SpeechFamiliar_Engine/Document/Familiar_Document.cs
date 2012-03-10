using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;
using SpeechFamiliar.Engine;

namespace SpeechFamiliar
{
    public class Familiar_Document
    {
        public bool no_spaces = false;
        public List<Element_Base> rules = new List<Element_Base>();
        public int priority = 0;
        public string file_name;
        public Old_Action onload;
        public string name = "";
        internal uint item_id;
        public List<Element_Base> items = new List<Element_Base>();
        static internal Dictionary<string, Familiar_Document> all_documents = new Dictionary<string, Familiar_Document>();
        public Familiar_Grammar grammar;
        internal List<Element_Item> all_rules = new List<Element_Item>();
        internal List<Element_Reference> all_local_references = new List<Element_Reference>();
        internal Dictionary<string, Familiar_Document> dependants = new Dictionary<string, Familiar_Document>();
        public String_Dictionary<object> vocabulary = new String_Dictionary<object>();
        public List<Familiar_Document> referenced_documents = new List<Familiar_Document>();
        
        public static Familiar_Document get_document(string document_name)
        {
            var result = find_document(document_name);
            if (result == null)
            {
                result = new Familiar_Document(document_name);
            }

            return result;
        }

        protected Familiar_Document(string document_name)
        {
            file_name = Global.configuration.settings_path("grammars/" + document_name + ".sfg");
            if (!File.Exists(file_name))
                file_name = Global.configuration.settings_path("grammars/common/" + document_name + ".sfg");

            name = document_name;
            load();

            if (onload != null)
                onload.run();

            create_cfg_file();
        }

        public override string ToString()
        {
            return "Familiar_Document " + file_name;
        }

        public static void finalize_all_grammars(Speech_Recognizer engine)
        {
            foreach (Familiar_Document library in all_documents.Values)
            {
                library.create_grammar(engine);
            }
        }

        public static void reset()
        {
            all_documents.Clear();

            if (Directory.Exists(Global.configuration.temp_path))
            {
                foreach (string file in Directory.GetFiles(Global.configuration.temp_path))
                {
                    File.Delete(file);
                }
            }
        }

        public void load()
        {
            rules.Clear();
            all_rules.Clear();
            all_local_references.Clear();
            vocabulary.clear();
            referenced_documents.Clear();

            if (all_documents.Count == 0)
                reset();

            all_documents.Add(name, this);

            XmlDocument document = new XmlDocument();
            document.Load(file_name);
            XmlElement node = document.ChildNodes[1] as XmlElement;

            if (node.GetAttribute("priority").Length > 0)
            {
                priority = int.Parse(node.GetAttribute("priority"));
            }

            if (node.GetAttribute("no_spaces").Length > 0)
            {
                no_spaces = bool.Parse(node.GetAttribute("no_spaces"));
            }

            foreach (XmlElement rule in node.ChildNodes)
            {
                if (rule.Name.ToLower() == "rule" || rule.Name.ToLower() == "item")
                {
                    Element_Item result = Element_Item.create_rule(this, rule);
                    if (result != null)
                    {
                        rules.Add(result);
                        if (!all_rules.Contains(result))
                            all_rules.Add(result);
                    }
                }
            }

            synchronize_local_references();

            XmlNodeList commands = node.GetElementsByTagName("onload");
            foreach (XmlElement command in commands)
            {
                string type = command.GetAttribute("type");
                string code = "";
                foreach (XmlNode child in command.ChildNodes)
                {
                    code += child.InnerText;
                }

                onload = new Old_Action(this);
                onload.AddStep(type, code);
            }

        }

        protected string get_attribute(XmlElement element, string key)
        {
            if (element.GetAttribute(key).Length > 0)
            {
                return element.GetAttribute(key);
            }

            return "";
        }


        internal Familiar_Document add_external_document(string filename)
        {
            string document_key = Path.GetFileNameWithoutExtension(filename);

            //if (references.ContainsKey(document_key))
            //    return references[document_key];

            //string destination = Path.GetDirectoryName(file_name);
            //if (destination.Length > 0)
            //{
            //    if (filename.StartsWith(".."))
            //        destination = "grammars/" + Path.GetFileName(filename);
            //    else
            //        destination += "/" + filename;
            //}
            //else
            //    destination = filename;

            //destination = destination.Replace("\\\\", "/");
            //Familiar_Document new_document = new Familiar_Document(destination);

            return get_document(document_key);
        }

        public Familiar_Grammar create_grammar(Speech_Recognizer engine)
        {
            //create_cfg_file();
            //Element_Item.init();

            string destination = Global.configuration.temp_path + "/" + Path.GetFileNameWithoutExtension(this.file_name) + ".xml";
            grammar = new Familiar_Grammar(engine);
            grammar.create(this, destination);
            grammar.priority = priority;
            //using (MemoryStream reader = new MemoryStream(System.Text.Encoding.Default.GetBytes(document.InnerXml)))
            //{
            //    reader.Position = 0;
            //    new_grammar = new Grammar(reader);
            //}
            //grammar.recognized += new Recognition_Event(((Action_Library)parent).Familiar_Grammar_recognized);

            return grammar;
        }


        public XmlDocument create_cfg_file()
        {
            item_id = 0;
            items.Clear();

            XmlDocument document = new XmlDocument();
            XmlElement grammar_element = document.CreateElement("GRAMMAR");
            document.AppendChild(grammar_element);

            foreach (Element_Base rule in rules)
            {
                grammar_element.AppendChild(rule.create_grammar(document));

            }
            if (!Directory.Exists(Global.configuration.temp_path))
                Directory.CreateDirectory(Global.configuration.temp_path);

            string destination = Global.configuration.temp_path + "/" + Path.GetFileNameWithoutExtension(this.file_name) + ".xml";
            //File.WriteAllText(destination, document.InnerXml);

            XmlTextWriter text_writer = new XmlTextWriter(destination, Encoding.Unicode);
            text_writer.Formatting = Formatting.Indented;
            document.WriteTo(text_writer);
            text_writer.Close();

            return document;
        }

        public void synchronize_local_references()
        {
            foreach (Element_Base reference in all_local_references)
            {
                foreach (Element_Base rule in all_rules)
                {
                    if (rule.name == reference.text.Replace("#", ""))
                    {
                        reference.children.Add(rule);
                    }
                }
            }
        }

        public void save()
        {
            save(file_name);
        }

        public void save(string filename)
        {
            XmlDocument writer = new XmlDocument();
            writer.AppendChild(writer.CreateXmlDeclaration("1.0", Encoding.Unicode.EncodingName, ""));

            XmlElement node = writer.CreateElement("grammar");
            node.SetAttribute("xmlns:xsi", @"http://www.w3.org/2001/XMLSchema-instance");
            //XmlAttribute attribute= writer.CreateAttribute("noNamespaceSchemaLocation");
            //attribute.NamespaceURI = ;
            //attribute.Prefix = "xsi";
            //attribute.Value=@"E:/Dev/SpeechFamiliar/SpeechFamiliar/Program/schemas/grammar.xsd";
            //      node.SetAttributeNode(attribute);
            //node.SetAttribute("xsi:noNamespaceSchemaLocation", );
            //node.Attributes.Append(attribute);

            writer.AppendChild(node);

            if (no_spaces)
                node.SetAttribute("no_spaces", "true");
            if (priority != 0)
                node.SetAttribute("priority", priority.ToString());

            foreach (Element_Base child in rules)
            {
                node.AppendChild(child.save(writer));
            }

            XmlTextWriter text_writer = new XmlTextWriter(filename, Encoding.Unicode);
            text_writer.Formatting = Formatting.Indented;
            writer.WriteTo(text_writer);
            text_writer.Close();
        }

        //private XmlElement save_element(XmlDocument writer, Element_Base element)
        //{
        //    XmlElement node = writer.CreateElement(Enum.GetName(typeof(Element_Type), element.type));
        //    add_attribute(node, "display", element.display);
        //    add_attribute(node, "parameter", element.key);
        //    if (element.min != 1 || element.max != 1)
        //        node.SetAttribute("repeat", element.min.ToString() + "-" + element.max.ToString());


        //    if (element.type == Element_Type.reference)
        //    {
        //        node.SetAttribute("uri", element.text);
        //        return node;
        //    }

        //    if (element.type == Element_Type.item)
        //    {
        //        add_attribute(node, "id", element.name);
        //        if (element.children.Count == 1 && element.children[0].text != "" && element.children[0].children.Count == 0)
        //        {
        //            node.AppendChild(writer.CreateTextNode(element.children[0].text));
        //            return node;
        //        }
        //    }

        //    if (element.text != "")
        //    {
        //        if (element.type == Element_Type.tag)
        //        {
        //            if (element.text.Contains("\n"))
        //            {
        //                node.AppendChild(writer.CreateTextNode(element.name + "="));
        //                node.AppendChild(writer.CreateCDataSection(element.text));
        //            }
        //            else
        //                node.AppendChild(writer.CreateTextNode(element.name + "=" + element.text));
        //        }
        //        else
        //            node.AppendChild(writer.CreateTextNode(element.text));
        //    }

        //    foreach (Element_Base child in element.children)
        //    {
        //        node.AppendChild(save_element(writer, child));
        //    }

        //    return node;
        //}

        private void add_attribute(XmlElement node, string key, string value)
        {
            if (value == null || value == "")
                return;

            node.SetAttribute(key, value);

        }

        public static Familiar_Document find_document(string document_name)
        {
            if (all_documents.ContainsKey(document_name))
                return all_documents[document_name];

            return null;
        }

        public void add_word(Element_Word word, string rule_name)
        {
            Element_Base rule = get_rule(rule_name);

            if (rule == null)
                return;

            add_word(word, rule.children[0]);
        }

        void add_word(Element_Word word, Element_Base choice)
        {
            foreach (Element_Base item in choice.children)
            {
                if (word.text == item.word_text)
                {
                    choice.children.Remove(item);
                }
            }

            choice.add(word);

            List<Element_Base> vocabulary = new List<Element_Base>();
            vocabulary.AddRange(choice.children);

            vocabulary.Sort(delegate(Element_Base first, Element_Base second)
            { return first.word_text.CompareTo(second.word_text); });

            choice.children.Clear();
            foreach (Element_Base item in vocabulary)
            {
                choice.children.Add(item);
            }
        }

        public Element_Word get_word(string text, string rule_name)
        {
            Element_Item rule = get_rule(rule_name);

            if (rule == null)
                return null;

            foreach (Element_Word item in rule.children[0].get_children(typeof(Element_Word)))
            {
                if (item.match(text))
                    return item;
            }

            return null;
        }

        public Element_Item get_rule(string rule_id)
        {
            foreach (Element_Item rule in rules)
            {
                if (rule.name == rule_id)
                    return rule;
            }

            return null;
        }

        public Element_Choice get_rule_choice(string rule_id)
        {
            Element_Item rule = get_rule(rule_id);
            if (rule == null)
                return null;

            foreach (Element_Base element in rule.children)
            {
                if (element.GetType() == typeof(Element_Choice))
                {
                    return (Element_Choice)element;
                }
            }

            Element_Choice choices = (Element_Choice)rule.create_child(typeof(Element_Choice));
            rule.children.Add(choices);
            return choices;
        }

        public Element_Word find_word(string text)
        {
            object item = vocabulary.get(text);
            if (item != null)
            {
                if (item.GetType() == typeof(Element_Word))
                {
                    return item as Element_Word;
                }
                else
                {
                    List<Element_Word> list = item as List<Element_Word>;
                    return list[0];
                }
            }

            foreach (Familiar_Document reference in referenced_documents)
            {
                Element_Word word = reference.find_word(text);
                if (word != null)
                    return word;
            }

            return null;
        }

    }
}
