using System;
using System.Collections.Generic;
using System.Text;

namespace SpeechFamiliar
{
    public class Sentence
    {
        public string key;
        public List<Token> words = new List<Token>();
        protected Dictionary<string, List<Token>> real_parameters = new Dictionary<string, List<Token>>();

        public Singular_Indexer parameter { get; private set; }
        public Plural_Indexer parameters { get; private set; }

        public Sentence()
        {
            parameter = new Singular_Indexer(this);
            parameters = new Plural_Indexer(this);
        }

        public void merge(Sentence other)
        {
            if (other == null)
                return;

            words.AddRange(other.words);
            foreach (var collection in other.real_parameters)
            {
                if (real_parameters.ContainsKey(collection.Key))
                    real_parameters[collection.Key].AddRange(collection.Value);
                else
                    real_parameters.Add(collection.Key, collection.Value);
            }

            //real_parameters.Concat(other.real_parameters);
        }

        public void add_parameter(string new_key, Token value)
        {
            if (!real_parameters.ContainsKey(new_key))
                real_parameters.Add(new_key, new List<Token>());

            real_parameters[new_key].Add(value);
        }

        public class Singular_Indexer
        {
            Sentence parent;

            public Singular_Indexer(Sentence new_parent)
            {
                parent = new_parent;
            }
            public Token this[string key]
            {
                get { return parent.parameters[key][0]; }
            }

            public bool contains_key(string key)
            {
                return parent.real_parameters.ContainsKey(key);
            }
        }

        public class Plural_Indexer
        {
            Sentence parent;

            public Plural_Indexer(Sentence new_parent)
            {
                parent = new_parent;

            }

            public List<Token> this[string key]
            {
                get
                {
                    if (parent.real_parameters.ContainsKey(key))
                        return parent.real_parameters[key];
                    else
                        return new List<Token>();
                }
            }

            public string[] keys
            {
                get { 
                    var result = new string[parent.real_parameters.Count];
                    parent.real_parameters.Keys.CopyTo(result, 0);
                    return result; }
            }

            public bool contains_key(string key)
            {
                return parent.real_parameters.ContainsKey(key);
            }

        }
    }

}
