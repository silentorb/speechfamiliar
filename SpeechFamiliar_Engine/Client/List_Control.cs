using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace SpeechFamiliar.Forms
{
    public partial class List_Control : UserControl
    {
        public List_Control(UIAttribute attribute, List<string> items)
        {
            InitializeComponent();

            title.Text = attribute.text;

            foreach (string item in items)
            {
                list.Items.Add(item);
            }
        }

        private void add_Click(object sender, EventArgs e)
        {
            list.Items.Add(new ListViewItem(selected_text.Text)).Selected = true;
            selected_text.Focus();
        }

        private void remove_Click(object sender, EventArgs e)
        {
            if (list.SelectedItems.Count > 0)
            {
                list.Items.RemoveAt(list.SelectedIndices[0]);
            }
        }

        private void list_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (list.SelectedIndices.Count == 0)
                selected_text.Text = "";
            else
                selected_text.Text = list.SelectedItems[0].Text;
        }

        private void selected_text_TextChanged(object sender, EventArgs e)
        {
            if (list.SelectedIndices.Count > 0)
                list.SelectedItems[0].Text = selected_text.Text;
        }
    }
}
