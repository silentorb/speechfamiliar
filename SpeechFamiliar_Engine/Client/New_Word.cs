using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SpeechFamiliar.Scripting;
//using System.Speech.Recognition.SrgsGrammar;
using SilentOrb.Utility;

namespace SpeechFamiliar
{
    public partial class New_Word : Form
    {
        public New_Word()
        {
            InitializeComponent();
            Activate();
            Windows.bring_window_to_front(this.Handle);
            Windows.set_foreground_window(this.Handle);
        }

        public bool Result = false;
        public string word;

        private void button_Add_Click(object sender, EventArgs e)
        {
            if (text_Word.Text.Length > 0)
            {
                //SrgsItem item = new SrgsItem();
                //if (text_Display.Text.Length > 0)
                //{
                //    SrgsToken token = new SrgsToken(text_Word.Text);
                //    token.Display = text_Display.Text;
                //    item.Add(token);
                //}
                //else
                //    item = new SrgsItem(text_Word.Text);

                //Automation.add_new_word(text_Word.Text, grammar);
                word = text_Word.Text;

                DialogResult = DialogResult.OK;
            }
            Close();
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}