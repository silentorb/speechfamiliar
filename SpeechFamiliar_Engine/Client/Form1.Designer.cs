namespace SpeechFamiliar.Forms
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.text_Main = new System.Windows.Forms.TextBox();
            this.text_active_window = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // text_Main
            // 
            this.text_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.text_Main.Location = new System.Drawing.Point(0, 0);
            this.text_Main.Multiline = true;
            this.text_Main.Name = "text_Main";
            this.text_Main.ReadOnly = true;
            this.text_Main.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.text_Main.Size = new System.Drawing.Size(479, 36);
            this.text_Main.TabIndex = 0;
            // 
            // text_active_window
            // 
            this.text_active_window.Location = new System.Drawing.Point(14, 80);
            this.text_active_window.Name = "text_active_window";
            this.text_active_window.ReadOnly = true;
            this.text_active_window.Size = new System.Drawing.Size(766, 20);
            this.text_active_window.TabIndex = 1;
            this.text_active_window.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(479, 36);
            this.Controls.Add(this.text_active_window);
            this.Controls.Add(this.text_Main);
            this.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Speech Familiar";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox text_Main;
        public System.Windows.Forms.TextBox text_active_window;
    }
}

