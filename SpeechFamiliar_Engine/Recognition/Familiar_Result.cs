using System;
using System.Collections.Generic;
using System.Text;
using SpeechFamiliar.Scripting;
using SilentOrb.Utility;
using System.Threading;
using SpeechLib;
using System.Text.RegularExpressions;
using SpeechFamiliar;
using System.IO;
//using NAudio.Wave;

namespace SpeechFamiliar
{
    public partial class Familiar_Result
    {
        internal Familiar_Phrase chosen_phrase;
        public List<Familiar_Phrase> options = new List<Familiar_Phrase>();
        static int current = 0;

        public string text
        {
            get { return chosen_phrase.text; }
        }

        public Familiar_Result(SpInProcRecoContext context, ISpeechRecoResult speech_result)
        {
            try
            {
                //options.Clear();

                //ISpeechPhraseAlternates phrase_alternatives = speech_result.Alternates(10, 0, -1);
                ////string text = context.Recognizer.Recognizer.GetAttribute("CGFAlternates");

                //if (phrase_alternatives == null || phrase_alternatives.Count < 2)
                //{
                //    chosen_phrase = new Familiar_Phrase(speech_result.PhraseInfo, this);
                //}
                //else
                //{
                //    Feedback.print("domains ing!", Feedback.Status.debug);

                //    foreach (ISpeechPhraseAlternate alternative in phrase_alternatives)
                //    {
                //        options.Add(new Familiar_Phrase(alternative.PhraseInfo, this));

                //    }

                //    chosen_phrase = options[0];
                //}

                chosen_phrase = new Familiar_Phrase(speech_result.PhraseInfo, this);

                //save_audio(speech_result);
                // Feedback.print(speech_result.Times.Length.ToString());
            }
            catch (Exception ex)
            {
                Feedback.print(ex);
                //words.Clear();
            }
        }

        public void display(string info)
        {
            //foreach (Transition_Word word in words)
            //{
            //    //info += word.text + "(" + word.confidence + ", " + word.document_name + ") ";
            //    info += word.text + "(";
            //    bool first = true;
            //    foreach (Transition_Word argument in word.arguments.Values)
            //    {
            //        if (first)
            //            first = false;
            //        else
            //            info += " ";

            //        info += argument.text;
            //    }
            //    info += string.Format(")[{0}, {1}]\r\n", word.confidence, word.document_name);
            //}
            info += "\r\n";
            Feedback.print(info, Feedback.Status.info);
        }

        public void run()
        {
            if (Global.speech.standby)
            {
                if (text == "start speech familiar" || text == "go to sleep")
                    Automation.wake_up();
                return;
            }

            ThreadPool.QueueUserWorkItem(new WaitCallback(Run), this);
        }

        static private void Run(object state_info)
        {
            Familiar_Result result = (Familiar_Result)state_info;

            //foreach (Transition_Word word in result.transition_words)
            //{
            //    word.run();
            //}


            //Feedback.print("\r\nAlternatives = " + result.options.Count.ToString() + "\r\n", Feedback.Status.debug);
            //int x = 1;
            //foreach (var alternative in result.options)
            //{
            //    //Profiler.trace("run result");
            //    //Feedback.print("\r\nalt " + x.ToString() + "\r\n", Feedback.Status.debug);
            //    ++x;
            //    alternative.run();
            //    //Profiler.trace("finished run result");
            //}

            result.chosen_phrase.run();

            Profiler.print_result();

            //  result.display("> ");

        }

    }

}
