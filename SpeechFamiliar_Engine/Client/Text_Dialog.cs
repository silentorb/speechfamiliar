using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SpeechFamiliar
{
    public partial class Text_Dialog : Form
    {
        public Text_Dialog()
        {
            InitializeComponent();
        }
        public bool Result = false;

        private void button_Okay_Click(object sender, EventArgs e)
        {
            Result = true;
            Close();
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {            
            Close();
        }
    }
}