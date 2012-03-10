using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SilentOrb.Reflection;
using SpeechFamiliar;
using SpeechFamiliar.Scripting;

namespace SpeechFamiliar.Plugins
{
    [PluginInfo("dictation")]
    class Dictation : Plugin
    {
        static List<Token> modifiers = new List<Token>();

        public void dictation(Sentence sentence)
        {
            if (sentence.words.Count == 0)
                return;

            parse_context(sentence.words[0]);

            string output = "";

            // Process each word
            foreach (var word in sentence.words)
            {
                output += format(word);
            }

            Automation.sendkeys(output);
        }

        //static Regex split_words = new Regex(@"\\b|(?=_)|(?<=_)|(?<=[^\sA-Za-z\d])(?=[^\sA-Za-z\d])");
        //static Regex split_words = new Regex(@"(?=_)|(?<=_)|(?<=[^\sA-Za-z\d])(?=[^\sA-Za-z\d])");
        //static Regex get_whitespace = new Regex(@"\A\s+\z");

        //private void parse_context(Token word)
        //{
        //    var text = Automation.get_pretext();
        //    if (text == null)
        //        return;

        //    var matches = split_words.Split(text);
        //    List<string> words = new List<string>();

        //    foreach (var match in matches)
        //    {
        //        words.Add(match);
        //    }

        //    for (int y = 0; y < words.Count; y++)
        //    {
        //        var current_word = words[y];
        //        if (get_whitespace.Match(current_word).Success || current_word.Length == 0)
        //        {
        //            if (y > 0)
        //                words[y - 1] += current_word;
        //            words.RemoveAt(y);
        //            --y;
        //        }
        //    }

        //    if (words.Count == 0)
        //        return;

        //    text = words[words.Count - 1];
        //    var result = Automation.find_word(text.Trim());
        //    if (result != null)
        //    {
        //        word.last_word = new Token(text, result);
        //    }
        //}

        static Regex get_last_word = new Regex(@"[A-Za-z\d]+", RegexOptions.RightToLeft);
        //static Regex get_whitespace = new Regex(@"\A\s+\z");

        private void parse_context(Token word)
        {
            var text = Automation.get_pretext();
            if (text == null)
                return;

            var match = get_last_word.Match(text);
            if (!match.Success)
                return;

            text = match.Value;
            var result = Automation.find_word(text);
            if (result != null)
            {
                word.last_word = new Token(text, result);
            }
        }

        private string format(Token word)
        {
            if (word.properties.contains_key("modifier"))
            {
                modifiers.Add(word);

                if (word.next_word != null)
                    word.next_word.last_word = word.last_word;

                return "";
            }

            var result = word.text.ToString();
            foreach (var modifier in modifiers)
            {
                if (modifier.properties.contains_key("capitalize_modifier"))
                    result = result.Substring(0, 1).ToUpper() + result.Substring(1);
                if (modifier.properties.contains_key("no_capitalize_modifier"))
                    result = result.ToLower();
                if (modifier.properties.contains_key("no_spaces_modifier"))
                    word.properties.add("no_preceding_space");
            }

            modifiers.Clear();

            if (!word.properties.contains_key("no_preceding_space"))
                result = calculate_space(word, true) + result;

            return result;
        }

        private int get_space(Token word, int default_result, bool last)
        {
            if (word.properties.contains_key("no_spaces"))
                return 0;

            if (last)
            {
                if (word.properties.contains_key("one_following_space"))
                    return 0;
            }
            else
            {
                if (word.properties.contains_key("one_following_space"))
                    return 1;
            }

            return default_result;
        }

        static Regex first_pattern = new Regex(@"\s+\z");
        static Regex second_pattern = new Regex(@"\A\s+");

        private string calculate_space(Token word, bool last)
        {
            if (word.properties.contains_key("no_spaces"))
                return "";

            var total_space = 0;
            var current_space = 0;
            Regex pattern;
            Token other;
            string undo;

            if (last)
            {
                pattern = first_pattern;
                other = word.last_word;
                undo = "{BACKSPACE}";
            }
            else
            {
                pattern = second_pattern;
                other = word.next_word;
                undo = "{DELETE}";
            }

            if (other != null)
            {
                var match = pattern.Match(other.text);
                if (match.Success)
                    current_space = match.Length;
                total_space = get_space(other, 1, last ^ true);
            }

            total_space = get_space(word, total_space, true);
            if (total_space > current_space)
            {
                total_space -= current_space;
                return "".PadLeft(total_space);
            }
            else if (current_space > total_space && total_space > 1)
            {
                total_space = current_space - total_space;
                StringBuilder builder = new StringBuilder(total_space * undo.Length);
                for (int i = 0; i < total_space; i++)
                {
                    builder.Append(undo);
                }

                return builder.ToString();
            }

            return "";
        }

        Element_Word find_word(string text)
        {
            foreach (var document in Automation.current_context.vocabularies.Values)
            {
                var result = document.find_word(text);
                if (result != null)
                    return result;
            }

            return null;
        }



        //def find_word text
        //    Automation.current_context.vocabularies.values.each do |document|
        //        #			print document.name.to_s + " #{document.vocabulary.count}\r\n"
        //        result = document.find_word text
        //        return result if result			
        //    end

        //    nil
        //end
    }
}
