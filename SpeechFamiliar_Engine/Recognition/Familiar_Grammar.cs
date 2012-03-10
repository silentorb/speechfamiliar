using System;
using System.Collections.Generic;
using System.Text;
using SpeechFamiliar;
using SpeechLib;
using System.Runtime.InteropServices;
using SilentOrb.Utility;

namespace SpeechFamiliar.Engine
{
    public class Familiar_Grammar
    {
        ISpeechRecoGrammar grammar;
        public string name;
        //public int priority;
        public string file_name = "";
        public Familiar_Document parent_document = null;
        //public bool is_dictation = false;
        public Speech_Recognizer engine;

        public bool enabled
        {
            get { return grammar.State == SpeechGrammarState.SGSEnabled; }
            set
            {
                //       grammar.DictationSetState(SpeechRuleState.SGDSActive);
                if (value)
                    grammar.State = SpeechGrammarState.SGSEnabled;
                else
                    grammar.State = SpeechGrammarState.SGSDisabled;
            }
        }

        //public void create_dictation(Action_Library library)
        //{            
        //    grammar.DictationLoad("", SpeechLoadOption.SLOStatic);
        //    grammar.DictationSetState(SpeechRuleState.SGDSActive);
        //    grammar.State = SpeechGrammarState.SGSDisabled;
        //    is_dictation = true;
        //    engine.grammars.Add((Decimal)0, this);
        //    //recognized += new Recognition_Event(library.Familiar_Grammar_recognized);
        //}

        //public void create(Speech_Recognizer engine, Familiar_Document document)
        //{
        //    grammar = engine.context.CreateGrammar(0);
        //    grammar.State = SpeechGrammarState.SGSDisabled;
        //    ISpeechGrammarRule rule = grammar.Rules.Add("root", SpeechRuleAttributes.SRATopLevel | SpeechRuleAttributes.SRADefaultToActive, 0);
        //    //ISpeechGrammarRuleState state = grammar.Rules.Item(0).AddState();
        //    //ISpeechGrammarRuleState state2 = grammar.Rules.Item(0).AddState();

        //    object temp = "";
        //    rule.InitialState.AddWordTransition(null, "bob it", " ", SpeechGrammarWordType.SGLexical, "", 0, ref temp, 1.0f);
        //    //string error;
        //    grammar.Rules.Commit();//(out error);
        //    //foreach (Element_Base rule in document.rules)
        //    //{
        //    //    //grammar.Rules.Add(
        //    //    //choice.Add(create_choices(rule));
        //    //}
        //    grammar.State = SpeechGrammarState.SGSEnabled;
        //    //engine.context.
        //    grammar.CmdSetRuleIdState(0, SpeechRuleState.SGDSActive);
        //}

        //    static protected Decimal next_id = 1;

        public Familiar_Grammar(Speech_Recognizer new_engine)
        {
            engine = new_engine;

            Decimal next_id = 1;
            while (engine.grammars.ContainsKey(next_id))
                ++next_id;

            grammar = engine.context.CreateGrammar(next_id);
            //      grammar.DictationSetState(SpeechRuleState.SGDSActive);
            engine.grammars.Add(next_id, this);
        }

        public void create(Familiar_Document document, string filename)
        {
            parent_document = document;
            name = parent_document.name;
            file_name = filename;

#if !DEBUG
            try
            {
#endif
                grammar.CmdLoadFromFile(file_name, SpeechLoadOption.SLOStatic);

#if !DEBUG
            }
            catch (Exception ex)
            {
                Feedback.print(file_name, Feedback.Status.info);
                Feedback.print(ex);
                return;
            }
#endif

            grammar.CmdSetRuleIdState(0, SpeechRuleState.SGDSActive);
            grammar.State = SpeechGrammarState.SGSDisabled;
            //          ++next_id;
        }

        static bool sapi_53 = true;

        private int _priority;
        public int priority
        {
            get { return _priority; }
            set
            {
                //if (is_dictation)
                //{
                //    try
                //    {
                //        ((ISpRecoGrammar2)grammar).SetRulePriority("", 0, value);
                //    }
                //    catch
                //    {
                //        SilentOrb.Utility.Feedback.print("dictation prioritization didn't work.\r\n");                        
                //    }

                //    return;
                //}

                if (sapi_53)
                {
                    try
                    {
                        //throw new Exception();
                        var grammar2 = ((ISpRecoGrammar2)grammar);
                        grammar2.SetRulePriority("root", 0, value);
                    }
                    catch
                    {
                        Feedback.print(file_name + "\r\n", Feedback.Status.debug);
                        Feedback.print("SAPI 5.3 not available.  Using SAPI 5.1 instead.\r\nGrammar prioritization will not be available.\r\n", Feedback.Status.debug);
                        sapi_53 = false;
                    }
                }
                _priority = value;
            }
        }

        public void release()
        {
            Marshal.ReleaseComObject(grammar);
        }

        public void reload()
        {

        }


    }
}
