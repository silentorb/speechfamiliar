using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using SilentOrb.Reflection;
using SilentOrb.Utility;
using SilentOrb.Xml;

namespace MetaHub
{
 
    public class MetaNode
    {
        public static Dictionary<string, Type> types = new Dictionary<string, Type>();
        public Message_Manager messenger;
        public Operation_Status status = Operation_Status.Waiting;
        
        protected MetaNode container;


        public virtual void initialize()
        {

        }

        protected virtual void run()
        {
            
        }

        public void execute()
        {
            Finish_EventArgs ev = new Finish_EventArgs();

            try
            {
                try
                {
                    run();
                }
                catch (Exception ex)
                {
                    ev = on_error(ex);
                }
                finally
                {
                    //Finish(this, ev);
                    messenger.send("item_finished", this, ev);
                }
            }
            catch (Exception ex)
            {
                Feedback.print(ex);
            }
        }

        protected virtual Finish_EventArgs on_error(Exception ex)
        {
            //Feedback.print(input);
            Feedback.print(ex);
            //status = Operation_Status.Failure;
            Finish_EventArgs ev = new Finish_EventArgs();
            ev.success = false;
            //ev.errorMessage = "Error with item # " + index.ToString() + "";
            return ev;
        }

        public virtual string display_name
        {
            get { return ""; }
        }

        public virtual MetaNode clone()
        {
            MetaNode new_node = (MetaNode)Activator.CreateInstance(GetType());

            foreach (Data_Member member in Data_Member.get_data_members(GetType()))
            {
                if (member.can_write)
                {
                    member.set_value(new_node, member.get_value(this));
                }
            }

            return new_node;
        }

        public virtual void update_view()
        {

        }


        public static MetaNode create_node(Type type, Message_Manager new_messenger, MetaNode new_project)
        {
            MetaNode result = (MetaNode)Activator.CreateInstance(type);
            result.messenger = new_messenger;
            result.container = new_project;
            result.initialize();
            return result;
        }

        public static MetaNode create_node(string type_name, Message_Manager new_messenger, MetaNode new_project)
        {
            return create_node(types[type_name], new_messenger, new_project);
        }

        private bool _modified = false;
        public bool modified
        {
            set
            {
                modified = value;
                if (container != null && value == true)
                    container.modified = true;
            }
            get
            {
                return _modified;
            }
        }

        //public virtual Type view_type
        //{
        //get
        //{
        //    Assembly assembly = Assembly.GetAssembly(this.GetType());
        //    foreach (Type view_type in assembly.GetTypes())
        //    {
        //        if (view_type.Name == this.GetType().Name + "_View")
        //            return view_type;
        //    }

        //    return typeof(Node_View);
        //}
        //}

        //public virtual Node_View create_view(Type type)
        //{
        //Node_View result = (Node_View)Activator.CreateInstance(type);
        //result.generic_source = this;

        //Data_Member member = Data_Member.get_data_member(result, "source");
        //if (member != null)
        //{
        //    member.set_value(result, this);
        //}

        //result.initialize();

        //return result;
        //}

        //[Xml_Binding("type", Xml_Element_Settings.attribute)]
        //public string type_name
        //{
        //    get { return GetType().Name; }
        //}
        
        //public virtual bool show_editor()
        //{
        //    return true;
        //}

    }
}
