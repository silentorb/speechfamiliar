using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using SilentOrb.Utility;

namespace SpeechFamiliar
{

    public struct Sentence_Parser
    {
        public Token last_word;
        public LinkedList<Token> original_words;
    }

    public class Token
    {
        public List<Token> children = new List<Token>();
        public float confidence;
        protected string value;
        public string[] parameters = null;
        //ISpeechPhraseProperty property;
        public Token parent;
        internal string id = "";
        internal Familiar_Phrase original_phrase;
        public bool generated { get; private set; }
        public Element_Base source { get; protected set; }
        public List<Action> actions = new List<Action>();


        // last and previous words set each other while avoiding infinite recursion
        private Token _last_word;
        public Token last_word
        {
            get { return _last_word; }
            set
            {
                if (_last_word != null)
                    _last_word._next_word = null;

                _last_word = value;
                if (value != null)
                    value._next_word = this;
            }
        }

        private Token _next_word;
        public Token next_word
        {
            get { return _next_word; }
            set
            {
                if (_next_word != null)
                    _next_word._last_word = null;

                _next_word = value;
                if (value != null)
                    value._last_word = this;
            }
        }

        public Token(string new_text, Element_Word new_word)
        {
            generated = true;
            source = new_word;
            text = new_text;
            properties = new Singular_Indexer(this);
        }

        public Token(Familiar_Phrase new_info, string new_value)
        {
            text = "";
            original_phrase = new_info;
            generated = false;
            properties = new Singular_Indexer(this);

            value = new_value;

            initialize(value);
            //get_parameters(start, end);
        }

        public Token(Familiar_Phrase new_info, string new_value, int start, int end)
        {
            text = "";
            original_phrase = new_info;
            generated = false;
            properties = new Singular_Indexer(this);

            value = new_value;

            initialize(value);
            get_parameters(start, end);
        }

        public string text
        {
            get;
            set;
        }

        //private void create_final_word()
        //{
        //    text = "";
        //    if (value != null)
        //        text = value;

        //    if (text != null)
        //    {
        //        initialize(text);
        //    }
        //}

        public void initialize(string input_text)
        {
            id = input_text;
            Match match = Regex.Match(input_text, @"::([^.]+).(\d+)(?:\.(\d+))?");
            if (match.Success)
            {
                Familiar_Document item_document = Familiar_Document.find_document(match.Groups[1].Value);
                source = item_document.items[int.Parse(match.Groups[2].Value)];

                if (source.GetType() != typeof(Element_Word))
                {
                    text = source.text;
                    return;
                }

                Element_Word item = source as Element_Word;

                text = item.display;

                if (match.Groups[3].Value != "")
                {
                    text = item.get_suffix(item.suffixes[int.Parse(match.Groups[3].Value)]);
                }
            }
            else
            {
         //       var source = MetaHub.Core.Meta_Object.parse_absolute_path(input_text);
         //       if (source != null)
         //       {
         //           text = input_text;
         ////           Feedback.print(text + "\r\n", Feedback.Status.info);
         //       }
         //       else
         //           text = input_text;
            }
        }

        private void get_parameters(int start, int end)
        {
            string text = original_phrase.get_text(start, end);
            if (text == null)
                return;

            parameters = Regex.Split(text, @"(?=[^^]::)\s*");
        }

        public Sentence_Parser load(string key, Sentence_Parser sentence)
        {
            last_word = sentence.last_word;

            if (source.key.Length > 0)
                key = source.key;

            if (original_phrase.get_element(id) != null)
            {
                if (sentence.last_word != null)
                {
                    last_word = sentence.last_word;
                }

                foreach (var original in sentence.original_words)
                {
                    if (original.text == id)
                    {
                        confidence = original.confidence;
                        sentence.original_words.Remove(original);
                        break;
                    }
                }

                sentence.last_word = this;
            }
            else if (text != "" || source.GetType() == typeof(Element_Word))
                sentence.last_word = this;

            foreach (Token child in children)
            {
                sentence = child.load(key, sentence);
            }

            if (source.GetType() == typeof(Element_Item))
            {
                foreach (Action action in ((Element_Item)source).actions)
                {
                    actions.Add(action);
                }

                if (actions.Count > 0)
                    original_phrase.final_action = actions[actions.Count - 1];
            }

            return sentence;
        }

        public Singular_Indexer properties { get; private set; }

        public class Singular_Indexer
        {
            Token parent;
            Hashtable additions = new Hashtable();

            public Singular_Indexer(Token new_parent)
            {
                parent = new_parent;
            }

            public string check_source_for_property(Element_Base word, string key)
            {
                if (word == null)
                    return null;

                if (word.GetType() == typeof(Element_Item))
                {
                    Element_Item item = word as Element_Item;
                    if (!item.properties.ContainsKey(key))
                    {
                        return check_source_for_property(word.parent, key);
                    }

                    return item.properties[key];
                }
                else if (word.GetType() == typeof(Element_Word))
                {
                    Element_Word item = word as Element_Word;
                    if (!item.properties.ContainsKey(key))
                    {
                        return check_source_for_property(word.parent, key);
                    }

                    return item.properties[key];
                }
                else
                    return check_source_for_property(word.parent, key);
            }

            public string check_word_for_property(Token word, string key)
            {
                if (word == null)
                    return null;

                if (word.source.GetType() == typeof(Element_Item))
                {
                    Element_Item item = word.source as Element_Item;
                    if (!item.properties.ContainsKey(key))
                    {
                        return check_word_for_property(word.parent, key);
                    }

                    return item.properties[key];
                }
                else
                {
                    Element_Word item = word.source as Element_Word;
                    if (!item.properties.ContainsKey(key))
                    {
                        if (parent.generated)
                            return check_source_for_property(item.parent, key);
                        else
                            return check_word_for_property(word.parent, key);
                    }

                    return item.properties[key];
                }
            }

            public object this[string key]
            {
                get
                {
                    if (additions.ContainsKey(key))
                        return additions[key];

                    string value = check_word_for_property(parent, key);

                    if (value == null)
                        return null;

                    switch (value)
                    {
                        case "true":
                            return true;
                        case "false":
                            return false;
                    }

                    return value;
                }

            }

            public void add(string key)
            {
                additions[key] = true;
            }

            public void remove(string key)
            {
                additions.Remove(key);
            }

            public bool contains_key(string key)
            {
                if (additions.ContainsKey(key))
                    return true;

                if (parent.source.GetType() == typeof(Element_Item))
                {
                    Element_Item item = parent.source as Element_Item;
                    return item.properties.ContainsKey(key);
                }
                else
                {
                    Element_Word item = parent.source as Element_Word;
                    return item.properties.ContainsKey(key);
                }
            }
        }

        public Sentence run()
        {
            Sentence package = new Sentence();

            foreach (var child in children)
            {
                package.merge(child.run());
            }

            if (original_phrase.is_chosen)
            {
                bool actions_performed = false;

                foreach (var action in actions)
                {
                    package = action.run(package);
                    actions_performed = true;

                    if (package == null)
                        package = new Sentence();
                }

                if (actions_performed)
                {
                    return package;
                }
            }

            if (text != "" || source.GetType() == typeof(Element_Word))
            {
                package.words.Add(this);
                //if (original_phrase.is_chosen)
                //{
                //    Feedback.print(string.Format("{0} ({1}, {2})\r\n",text, confidence, source.document.name), Feedback.Status.debug);
                //}
            }
            if (source.key != "")
            {
                foreach (var child in package.words)
                {
                    package.add_parameter(source.key, child);
                }
            }

            return package;
        }

    }

    //if (source.GetType() == typeof(Element_Item))
    //{
    //    Element_Item item = source as Element_Item;

    //    if (item.display == null)
    //        text = item.text;
    //    else
    //        text = item.display;

    //    if (match.Groups[3].Value != "")
    //    {
    //        text += item.suffixes[int.Parse(match.Groups[3].Value)];
    //    }
    //}


}
