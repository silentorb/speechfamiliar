namespace SpeechFamiliar
{
    partial class New_Word
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.text_Word = new System.Windows.Forms.TextBox();
            this.button_Add = new System.Windows.Forms.Button();
            this.text_Display = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button_Cancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // text_Word
            // 
            this.text_Word.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.text_Word.Location = new System.Drawing.Point(64, 12);
            this.text_Word.Name = "text_Word";
            this.text_Word.Size = new System.Drawing.Size(194, 22);
            this.text_Word.TabIndex = 0;
            // 
            // button_Add
            // 
            this.button_Add.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button_Add.Location = new System.Drawing.Point(35, 71);
            this.button_Add.Name = "button_Add";
            this.button_Add.Size = new System.Drawing.Size(94, 25);
            this.button_Add.TabIndex = 1;
            this.button_Add.Text = "&Add";
            this.button_Add.UseVisualStyleBackColor = true;
            this.button_Add.Click += new System.EventHandler(this.button_Add_Click);
            // 
            // text_Display
            // 
            this.text_Display.Font = new System.Drawing.Font("Courier New", 9.75F);
            this.text_Display.Location = new System.Drawing.Point(64, 44);
            this.text_Display.Name = "text_Display";
            this.text_Display.Size = new System.Drawing.Size(194, 22);
            this.text_Display.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "word";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "display";
            // 
            // button_Cancel
            // 
            this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button_Cancel.Location = new System.Drawing.Point(144, 70);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(94, 25);
            this.button_Cancel.TabIndex = 5;
            this.button_Cancel.Text = "&Cancel";
            this.button_Cancel.UseVisualStyleBackColor = true;
            this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
            // 
            // New_Word
            // 
            this.AcceptButton = this.button_Add;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button_Cancel;
            this.ClientSize = new System.Drawing.Size(270, 108);
            this.ControlBox = false;
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.text_Display);
            this.Controls.Add(this.button_Add);
            this.Controls.Add(this.text_Word);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "New_Word";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Add New Word";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox text_Word;
        private System.Windows.Forms.Button button_Add;
        private System.Windows.Forms.TextBox text_Display;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button_Cancel;
    }
}