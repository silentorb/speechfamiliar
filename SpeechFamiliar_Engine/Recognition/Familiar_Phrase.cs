using System;
using System.Collections.Generic;
using SpeechLib;
using System.Text;

namespace SpeechFamiliar
{
    public class Familiar_Phrase
    {
        ISpeechPhraseInfo phrase;
        public int id;
        public List<Token> words = new List<Token>();
        public string text { get; private set; }
        public Familiar_Result result { get; private set; }
        public Familiar_Document document { get; private set; }
        public Action final_action;

        public Familiar_Phrase(ISpeechPhraseInfo new_phrase, Familiar_Result new_result)
        {
            phrase = new_phrase;
            result = new_result;
            // This is a little hacked up but it's very efficient
            id = result.options.Count + 1;

            process();
        }

        public ISpeechPhraseElement get_element(string word_id)
        {
            foreach (ISpeechPhraseElement element in phrase.Elements)
            {
                if (element.DisplayText == word_id)
                    return element;
            }

            return null;
        }

        public bool is_chosen
        {
            get { return id == result.chosen_phrase.id; }
        }

        public string get_text(int start, int end)
        {
            return phrase.GetText(start, end, true);
        }

        public void process()
        {
            words = new List<Token>();
            text = get_text(0, -1);

            LinkedList<Token> original_words = new LinkedList<Token>();

            foreach (ISpeechPhraseElement element in phrase.Elements)
            {
                original_words.AddLast(initialize_word(element));
            }

            if (phrase.Properties != null)
            {
                foreach (ISpeechPhraseProperty property in phrase.Properties)
                {
                    //     if (property.EngineConfidence >= 0.4)
                    {
                        Token transition = initialize_word(property);
                        if (transition != null)
                            words.Add(transition);
                        else
                        {
                            //SilentOrb.Utility.Feedback.print("error", SilentOrb.Utility.Feedback.Status.debug);
                        }
                    }
                    //break;
                }

                if (words.Count != 0)
                {
                    if (words[words.Count - 1].source == null)
                        return;

                    document = words[words.Count - 1].source.document;
                    Sentence_Parser sentence = new Sentence_Parser();
                    sentence.last_word = null;
                    sentence.original_words = original_words;

                    foreach (Token transition in words)
                    {
                        sentence = transition.load("", sentence);
                    }
                }
            }
        }

        Token initialize_word(ISpeechPhraseElement element)
        {
            var result = new Token(this, element.DisplayText);
            result.confidence = element.EngineConfidence;
            return result;
        }

        Token initialize_word(ISpeechPhraseProperty property)
        {
            if (property.NumberOfElements == 0)
                return null;

            var result = new Token(this, property.Value.ToString(), property.FirstElement, property.NumberOfElements);
            result.confidence = property.EngineConfidence;

            if (property.Children != null)
            {
                foreach (ISpeechPhraseProperty child in property.Children)
                {
                    if (child.NumberOfElements > 0)
                    {
                        var new_word = initialize_word(child);
                        new_word.parent = result;
                        result.children.Add(new_word);
                    }
                }
            }

            return result;
        }


        public void run()
        {
            foreach (Token word in words)
            {
                word.run();
            }
        }

    }


}
