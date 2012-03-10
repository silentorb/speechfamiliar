using System;
using System.Collections.Generic;
using System.Text;

namespace SpeechFamiliar
{
    public class String_Dictionary<T>
    {
        protected class Table_Node<T>
        {
            public Dictionary<char, Table_Node<T>> table = new Dictionary<char, Table_Node<T>>();
            public T value;

            public void clear()
            {
                foreach (var item in table.Values)
                {
                    item.clear();
                }

                table.Clear();
            }

        }

        protected Table_Node<T> table = new Table_Node<T>();

        public void add(string key, T value)
        {
            key = key.ToLower();
            var current_table = table;

            for (int x = 0; x < key.Length; x++)
            {
                char character = key[x];

                if (!current_table.table.ContainsKey(character))
                {
                    var new_table = new Table_Node<T>();
                    current_table.table.Add(character, new_table);
                }

                current_table = current_table.table[character];
            }

            current_table.value = value;
        }

        public void clear()
        {
            table.clear();
        }

        public T get(string key)
        {
            key = key.ToLower();
            var current_table = table;

            for (int x = 0; x < key.Length; x++)
            {
                char character = key[x];

                if (current_table.table.ContainsKey(character))
                    current_table = current_table.table[character];
                else
                    return default(T);
            }

            return current_table.value;
        }

    }
}
