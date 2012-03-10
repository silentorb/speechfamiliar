using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using SpeechFamiliar;
using SpeechLib;
using System.Runtime.InteropServices;
using SilentOrb.Utility;
using MetaHub.Core;
using MetaHub.Core.Nodes;

namespace SpeechFamiliar.Engine
{
    public class Meta_Grammar
    {
        ISpeechRecoGrammar grammar;
        //ISpeechRecoGrammar grammar2;
        public Speech_Recognizer engine;

        public Dictionary<string, Meta_Document> documents = new Dictionary<string, Meta_Document>();

        public bool enabled
        {
            get { return grammar.State == SpeechGrammarState.SGSEnabled; }
            set
            {
                if (value)
                    grammar.State = SpeechGrammarState.SGSEnabled;
                else
                    grammar.State = SpeechGrammarState.SGSDisabled;
            }
        }

        public Meta_Grammar(Speech_Recognizer new_engine)
        {
            engine = new_engine;
            grammar = engine.context.CreateGrammar(100);
            //grammar2 = engine.context.CreateGrammar(101);
            //grammar2.State = SpeechGrammarState.SGSEnabled;
            enabled = true;
        }


        public void add(Meta_Document document)
        {
            documents.Add(document.name, document);

            load(document);
        }

        public void release()
        {
            Marshal.ReleaseComObject(grammar);
            //Marshal.ReleaseComObject(grammar2);

        }

        public void reload()
        {

        }

        static int cx = 1;

        void load(Meta_Document document)
        {
            var temp = enabled;
            enabled = false;
            //int cx = 1;
            var rule = grammar.Rules.Add(document.name, SpeechRuleAttributes.SRADynamic | SpeechRuleAttributes.SRATopLevel, 1);

            foreach (var item in document.children)
            {
                add_node(item, rule.InitialState, null);
            }

            //var direction = grammar2.Rules.Add("direction", SpeechRuleAttributes.SRADynamic | SpeechRuleAttributes.SRAExport, 2);
            
            //add_transition(document.nodes[0], rule.InitialState, null, rule2);
            //add_node(document.nodes[0], rule2.InitialState, null);
            //add_node(document.nodes[1], rule.InitialState, null);

            //var rule2 = grammar.Rules.Add("testing", SpeechRuleAttributes.SRADynamic, 2);
            //add_transition(document.nodes[0], rule.InitialState, null, rule2);
            //var start = rule.AddState();
            //var start2 = rule.AddState();
            //var done = rule.AddState();
            //var cool = rule.AddState();
            //add_node("count", rule.InitialState, start);
            //add_node("wow", rule.InitialState, start2);
            //add_transition("", start, done, direction);
            //add_transition("", start2, cool, direction);
            //add_node("north", direction.InitialState, null);
            //add_node("south", direction.InitialState, null);
            //add_node("period", done, null);
            //add_node("cool", cool, null);
    
            grammar.Rules.Commit();
            //grammar2.Rules.Commit();            
           // grammar2.CmdSetRuleState("direction", SpeechRuleState.SGDSActive);
            enabled = temp;
        }

        void add_node(string item, ISpeechGrammarRuleState source, ISpeechGrammarRuleState destination)
        {
            object prop = item;
            source.AddWordTransition(destination, item, " ", SpeechGrammarWordType.SGLexical, (string)prop, ++cx, ref prop, 1.0f);
        }

        void add_node(MetaNode item, ISpeechGrammarRuleState source, ISpeechGrammarRuleState destination)
        {
            object prop = item.get_path();
            source.AddWordTransition(destination, (string)item["words"].value, " ", SpeechGrammarWordType.SGLexical, (string)prop, ++cx, ref prop, 1.0f);
        }

        void add_transition(object prop, ISpeechGrammarRuleState source, ISpeechGrammarRuleState destination, ISpeechGrammarRule rule)
        {
            source.AddRuleTransition(destination, rule, (string)prop, ++cx, ref prop, 1.0f);
        }

        void add_transition(MetaNode item, ISpeechGrammarRuleState source, ISpeechGrammarRuleState destination, ISpeechGrammarRule rule)
        {
            object prop = item.get_path();
            source.AddRuleTransition(destination, rule, (string)prop, ++cx, ref prop, 1.0f);
        }

        public void update()
        {
            enabled = false;
            if (grammar.Rules.Count == 0)
                return;

            grammar.Rules.Commit();
            enabled = true;
        }

        public void select_documents(ICollection keys)
        {
            foreach (var document in documents.Values)
            {
                grammar.CmdSetRuleState(document.name, SpeechRuleState.SGDSInactive);
            }

            foreach (string key in keys)
            {
                grammar.CmdSetRuleState(key, SpeechRuleState.SGDSActive);
            }
        }


    }
}
