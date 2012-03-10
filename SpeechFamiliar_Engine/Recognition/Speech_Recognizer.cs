using System;
using System.Collections.Generic;
using System.Text;
using SpeechLib;
using SilentOrb.Utility;
using SpeechFamiliar;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Xml;

namespace SpeechFamiliar.Engine
{
    public delegate void Recognition_Event(object sender, Familiar_Result result);

    public class Speech_Recognizer
    {
        internal SpInProcRecoContext context;
        public event Recognition_Event recognized;
        public Dictionary<object, Familiar_Grammar> grammars = new Dictionary<object, Familiar_Grammar>();

        public void initialize()
        {
            context = new SpInProcRecoContext();
            grammars.Clear();

            context.Recognition += new _ISpeechRecoContextEvents_RecognitionEventHandler(context_Recognition);
            context.FalseRecognition += new _ISpeechRecoContextEvents_FalseRecognitionEventHandler(context_FalseRecognition);
            context.Interference += new _ISpeechRecoContextEvents_InterferenceEventHandler(context_Interference);
            //      grammar.DictationSetState(SpeechRuleState.SGDSActive);
            context.CmdMaxAlternates = 5;
            context.State = SpeechRecoContextState.SRCS_Enabled;
            context.RetainedAudio = SpeechRetainedAudioOptions.SRAORetainAudio;
            //if (get_property_value("PersistedBackgroundAdaptation") == 1)
            //Feedback.print("cpu: " + get_property_value("ResourceUsage") + "\r\n", Feedback.Status.debug);
            //Feedback.print("high: " + get_property_value("ResourceUsage") + "\r\n", Feedback.Status.debug);
            //Feedback.print("medium: " + get_property_value("ResourceUsage") + "\r\n", Feedback.Status.debug);
            //Feedback.print("low: " + get_property_value("ResourceUsage") + "\r\n", Feedback.Status.debug);
            //Feedback.print("adaptation: " + get_property_value("AdaptationOn") + "\r\n", Feedback.Status.debug);
            //set_property_value("PersistedBackgroundAdaptation", 0);
            //     set_property_value("AdaptationOn", 0);
            //    set_property_value("PersistedLanguageModelAdaptation", 0);            
            initialize_audio();

        }

        void context_Interference(int StreamNumber, object StreamPosition, SpeechInterference Interference)
        {
            string text = Interference.ToString();
            if (text != "SITooQuiet")
                Feedback.print(string.Format("{0} {1}\r\n", StreamNumber, text), Feedback.Status.warning);

        }

        public void initialize_audio()
        {
            SpObjectTokenCategory category = new SpObjectTokenCategory();
            category.SetId(SpeechStringConstants.SpeechCategoryAudioIn, false);

            SpObjectToken token = new SpObjectToken();
            token.SetId(category.EnumerateTokens().Item(0).Id, "", false);
            //token.SetId(category.Default, "", false);
            context.Recognizer.AudioInput = token;
        }

        //public Familiar_Grammar create_dictation_grammar( )
        //{
        //    Familiar_Grammar new_grammar = new Familiar_Grammar();
        //    new_grammar.create_dictation(this);
        //    return new_grammar;
        //}

        void context_FalseRecognition(int StreamNumber, object StreamPosition, ISpeechRecoResult Result)
        {
            Feedback.print("!\r\n", Feedback.Status.debug);
            Familiar_Result result = new Familiar_Result(context, Result);

            result.display("(!) ");
        }

        void context_Recognition(int StreamNumber, object StreamPosition, SpeechRecognitionType RecognitionType, ISpeechRecoResult Result)
        {
            if (context.AudioInputInterferenceStatus == SpeechInterference.SINoise)
                return;

            //      Feedback.print(string.Format("Result = {0} {1}\r\n", StreamNumber, context.AudioInputInterferenceStatus), Feedback.Status.debug);            
            Feedback.print(string.Format("Result = {0}, {1}\r\n", Result.PhraseInfo.GetText(0, -1, true), Result.PhraseInfo.Elements.Item(0).EngineConfidence), Feedback.Status.debug);
            Profiler.initialize();
            Familiar_Result result = new Familiar_Result(context, Result);
            //            Familiar_Result result = get_result("> ", Result);
            //          result.display("> ");
            Profiler.trace("result created");

            Feedback.print(string.Format("Element Count = {0}\r\n", Result.PhraseInfo.Elements.Count), Feedback.Status.debug);
            foreach (Token word in result.chosen_phrase.words)
            {
                Feedback.print(string.Format("{0} ({1}, {2})\r\n", word.text, word.confidence, word.source.document.name), Feedback.Status.debug);
            }

            //if (Result.PhraseInfo.Elements.Count == 1 && result.chosen_phrase.words[0].confidence < 0.6)            
            //    return;

            result.run();

            if (recognized != null)
            {
                recognized.Invoke(this, result);
            }

        }

        public void unload_all_grammars()
        {
            context.State = SpeechRecoContextState.SRCS_Disabled;

            foreach (Familiar_Grammar grammar in grammars.Values)
            {
                grammar.release();
            }

            grammars.Clear();

            //        Marshal.ReleaseComObject(context);
        }

        public void start()
        {
            context.State = SpeechRecoContextState.SRCS_Enabled;
        }

        private XmlElement log_properties(ISpeechPhraseProperty property, XmlDocument writer, ISpeechPhraseInfo info)
        {
            XmlElement element = writer.CreateElement("property");
            element.SetAttribute("name", property.Name);
            element.SetAttribute("value", property.Value.ToString());
            element.SetAttribute("confidence", property.EngineConfidence.ToString());
            string text = info.GetText(property.FirstElement, property.NumberOfElements, true);
            if (text != null)
            {
                XmlElement text_element = writer.CreateElement("text");
                element.AppendChild(text_element);
            }

            if (property.Children != null)
            {
                foreach (ISpeechPhraseProperty child in property.Children)
                {
                    element.AppendChild(log_properties(child, writer, info));
                }

            }
            return element;
        }

        public object get_property_value(string key)
        {
            int value = 0;
            var result = context.Recognizer.GetPropertyNumber(key, ref value);
            if (result)
                return value;

            return false;
        }

        public object get_property_string(string key)
        {
            string value = "";
            var result = context.Recognizer.GetPropertyString(key, ref value);
            if (result)
                return value;

            return "{Nothing}";
        }

        public object set_property_value(string key, int value)
        {
            int old_value = 0;
            var result = context.Recognizer.GetPropertyNumber(key, ref old_value);
            if (!result)
            {
                Feedback.print(key + " not supported.", Feedback.Status.warning);
                return false;
            }
            //if (!result || old_value == value)
            //    return false;

            if (value == old_value)
            {
                Feedback.print(key + " was already " + old_value.ToString() + "\r\n", Feedback.Status.debug);
                return value;
            }
            Feedback.print(key + " was " + old_value.ToString() + " and was set to " + value.ToString() + "\r\n", Feedback.Status.debug);
            return context.Recognizer.SetPropertyNumber(key, value);
        }

    }
}
