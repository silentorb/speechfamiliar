using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Text_Editing
{
    /// <summary>
    /// Interaction logic for Text_Editor.xaml
    /// </summary>
    public partial class Text_Editor : Window
    {
        static public List<Text_Editor> windows = new List<Text_Editor>();

        public Text_Editor()
        {
            InitializeComponent();
            Title = "Text_Editor";
            windows.Add(this);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            windows.Remove(this);
            base.OnClosing(e);
        }

        public IntPtr handle
        {
            get
            {
                return new System.Windows.Interop.WindowInteropHelper(this).Handle;
            }
        }

        public string get_pretext()
        {
            int max = 20;
            //int length = text_box.text
            var document = text_box.Document;
            
            if (true)
            {
                
            }
            return "";
        }


    }
}
