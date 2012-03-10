namespace SpeechFamiliar
{
    partial class Text_Dialog
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
            this.text_Input = new System.Windows.Forms.TextBox();
            this.button_Okay = new System.Windows.Forms.Button();
            this.button_Cancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // text_Input
            // 
            this.text_Input.Location = new System.Drawing.Point(7, 13);
            this.text_Input.Name = "text_Input";
            this.text_Input.Size = new System.Drawing.Size(368, 20);
            this.text_Input.TabIndex = 0;
            // 
            // button_Okay
            // 
            this.button_Okay.Location = new System.Drawing.Point(107, 39);
            this.button_Okay.Name = "button_Okay";
            this.button_Okay.Size = new System.Drawing.Size(74, 24);
            this.button_Okay.TabIndex = 1;
            this.button_Okay.Text = "OK";
            this.button_Okay.UseVisualStyleBackColor = true;
            this.button_Okay.Click += new System.EventHandler(this.button_Okay_Click);
            // 
            // button_Cancel
            // 
            this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_Cancel.Location = new System.Drawing.Point(203, 39);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(74, 24);
            this.button_Cancel.TabIndex = 2;
            this.button_Cancel.Text = "Cancel";
            this.button_Cancel.UseVisualStyleBackColor = true;
            this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
            // 
            // Text_Dialog
            // 
            this.AcceptButton = this.button_Okay;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button_Cancel;
            this.ClientSize = new System.Drawing.Size(387, 67);
            this.ControlBox = false;
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.button_Okay);
            this.Controls.Add(this.text_Input);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Text_Dialog";
            this.Text = "Text_Dialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox text_Input;
        private System.Windows.Forms.Button button_Okay;
        private System.Windows.Forms.Button button_Cancel;
    }
}