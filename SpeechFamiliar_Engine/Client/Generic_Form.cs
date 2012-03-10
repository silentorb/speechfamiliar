using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SpeechFamiliar.Scripting;
using SilentOrb.Utility;
using System.Reflection;
using SilentOrb.Reflection;

namespace SpeechFamiliar.Forms
{
    public partial class Generic_Form : Form
    {
        Dictionary<Data_Member, Control> connections = new Dictionary<Data_Member, Control>();
        //int control_height = 30;
        int initial_control_count = 0;
        const int border = 10;
        public Generic_Form(object new_receiver, string new_title)
        {
            InitializeComponent();
            initial_control_count = Controls.Count;

            receiver = new_receiver;
            Text = new_title;

            Data_Member[] members = Data_Member.get_data_members(receiver.GetType());

            Dictionary<UIAttribute, Data_Member> dictionary = new Dictionary<UIAttribute, Data_Member>();
            List<UIAttribute> items = new List<UIAttribute>();

            foreach (Data_Member item in members)
            {
                UIAttribute attribute = item.get_custom_attribute(typeof(UIAttribute)) as UIAttribute;

                if (attribute != null)
                {
                    dictionary.Add(attribute, item);
                    items.Add(attribute);
                }
            }

            items.Sort(delegate(UIAttribute first, UIAttribute second)
            { return first.order.CompareTo(second.order); });

            foreach (UIAttribute attribute in items)
            {
                Data_Member item = dictionary[attribute];

                if (item.member_type.IsGenericType)
                {
                    create_list_control(attribute, item);
                }
                else
                {
                    Label label = new Label();
                    label.Left = border;
                    label.Text = attribute.text;
                    label.Width = 60;
                    add_below(label);

                    TextBox box = new TextBox();
                    //add_below(label).Top = box.Top = 20 + (control_height * count);
                    //box.Left = 140;
                    box.ReadOnly = false;
                    box.Width = 120;
                    box.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    box.Text = (string)item.get_value(receiver);
                    add_beside(box);
                    connections.Add(item, box);
                }

            }
            Button button = new Button();
            button.Text = "&Accept";
            button.Click += new EventHandler(button_Add_Click);
            AcceptButton = button;
            add_below(button);

            button = new Button();
            button.Text = "&Cancel";
            button.Click += new EventHandler(button_Cancel_Click);
            CancelButton = button;
            add_beside(button);

            //            button_Accept.Top = button_Cancel.Top = count * control_height;
            this.Height = Controls[Controls.Count-1].Bottom + 30;

            foreach (Control  control in Controls)
            {
                if (control.Right + 5 > Width)
                {
                    Width = control.Right + border;
                }
            }
            Activate();
            Windows.bring_window_to_front(this.Handle);
            Windows.set_foreground_window(this.Handle);
        }

        public object receiver;

        public void create_list_control(UIAttribute attribute, Data_Member item)
        {
            List_Control control = new List_Control(attribute, (List<string>)item.get_value(receiver));
            add_below(control);

            connections.Add(item, control);
        }

        public void add_below(Control control)
        {
            if (initial_control_count == Controls.Count)
                control.Top = border;
            else
                control.Top = Controls[Controls.Count - 1].Bottom + 5;

            Controls.Add(control);
        }

        public void add_beside(Control control)
        {
            if (initial_control_count == Controls.Count)
                control.Top = border;
            else
            {
                control.Top = Controls[Controls.Count - 1].Top;
                control.Left = Controls[Controls.Count - 1].Right + border;
            }

            Controls.Add(control);
        }

        private void button_Add_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            foreach (Data_Member member in connections.Keys)
            {
                if (member.member_type.IsGenericType)
                {
                    List<string> list = member.get_value(receiver) as List<string>;
                    List_Control list_control = connections[member] as List_Control;
                    list.Clear();
                    foreach (ListViewItem item in list_control.list.Items)
                    {
                        list.Add(item.Text);
                    }
                }
                else
                    member.set_value(receiver, connections[member].Text);
            }

            Close();
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
        
        public void show()
        {
            ShowDialog();
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class UIAttribute : Attribute
    {
        readonly string _text;
        public int order = 100;

        //public int order
        //{
        //    get { return _order; }
        //    set { _order = value; }
        //}

        public string text
        {
            get
            {
                return this._text;
            }
        }

        public UIAttribute(string new_text)
        {
            _text = new_text;
        }

        //public UIAttribute(string new_text, int new_order)
        //{
        //    _text = new_text;
        //    _order = new_order;
        //}

         
    }
}