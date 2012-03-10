using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using SilentOrb.Reflection;

namespace SpeechFamiliar
{
    //public enum Step_Type
    //{
    //    none = 0,
    //    sendkeys,
    //    python,
    //    plugin
    //}

    public class Action
    {
        public static Dictionary<string, Type> types = new Dictionary<string, Type>();
        
        public string text;
        public Familiar_Document library;

        //virtual public string class_name
        //{
        //    get { return "none"; }
        //}


        virtual protected void initialize()
        {

        }


        //static public Step_Type StringToType(string type)
        //{
        //    return (Step_Type)Enum.Parse(typeof(Step_Type), type.ToLower());
        //}

        //public Step(string new_type, string new_text)
        //{
        //    type = StringToType(new_type);
        //    text = new_text;
        //    initialize();
        //}

        public void create(string new_text)
        {
            text = new_text;
            initialize();
        }

        virtual public Sentence run(Sentence sentence)
        {
            return null;
        }

        static public Action create(string type, string text, Familiar_Document document)
        {
            if (!types.ContainsKey(type))
                return null;
            
                Action item = (Action)Activator.CreateInstance(types[type]);
                item.library = document;
                item.create(text);
                return item;            
        }
    }

    public class Old_Action
    {

        public List<Action> steps = new List<Action>();
        public Element_Base parent;
        public Familiar_Document document;
        public string id = "";
        public bool is_global = false;

        public Old_Action(Element_Base new_parent)
        {
            parent = new_parent;
            document = parent.document;
        }

        public Old_Action(Familiar_Document new_document)
        {
            document = new_document;
        }

        public void run( )
        {            
            foreach (Action step in steps)
            {
                step.run(null);
            }
        }
         
        public void run(Sentence result)
        {
            foreach (Action step in steps)
            {
                step.run(result);
            }
        }

        public void AddStep(string type, string text)
        {
            if (Action.types.ContainsKey(type))
            {                
                Action item = (Action)Activator.CreateInstance(Action.types[type]);
                item.library = document;
                item.create(text);
                steps.Add(item);
            }
        }

        ////public void AddStep(string type, string text)
        //{
        //    AddStep(Step.StringToType(type), text);

        //}
    }
}
