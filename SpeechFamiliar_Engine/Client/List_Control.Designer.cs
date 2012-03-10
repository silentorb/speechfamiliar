namespace SpeechFamiliar.Forms
{
    partial class List_Control
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.add = new System.Windows.Forms.Button();
            this.remove = new System.Windows.Forms.Button();
            this.title = new System.Windows.Forms.GroupBox();
            this.list = new System.Windows.Forms.ListView();
            this.selected_text = new System.Windows.Forms.TextBox();
            this.title.SuspendLayout();
            this.SuspendLayout();
            // 
            // add
            // 
            this.add.Location = new System.Drawing.Point(134, 80);
            this.add.Name = "add";
            this.add.Size = new System.Drawing.Size(75, 23);
            this.add.TabIndex = 2;
            this.add.Text = "A&dd";
            this.add.UseVisualStyleBackColor = true;
            this.add.Click += new System.EventHandler(this.add_Click);
            // 
            // remove
            // 
            this.remove.Location = new System.Drawing.Point(134, 109);
            this.remove.Name = "remove";
            this.remove.Size = new System.Drawing.Size(75, 23);
            this.remove.TabIndex = 3;
            this.remove.Text = "&Remove";
            this.remove.UseVisualStyleBackColor = true;
            this.remove.Click += new System.EventHandler(this.remove_Click);
            // 
            // title
            // 
            this.title.Controls.Add(this.selected_text);
            this.title.Controls.Add(this.list);
            this.title.Controls.Add(this.remove);
            this.title.Controls.Add(this.add);
            this.title.Location = new System.Drawing.Point(3, 3);
            this.title.Name = "title";
            this.title.Size = new System.Drawing.Size(225, 138);
            this.title.TabIndex = 3;
            this.title.TabStop = false;
            this.title.Text = "groupBox1";
            // 
            // list
            // 
            this.list.AutoArrange = false;
            this.list.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.list.HideSelection = false;
            this.list.LabelEdit = true;
            this.list.LabelWrap = false;
            this.list.Location = new System.Drawing.Point(6, 19);
            this.list.MultiSelect = false;
            this.list.Name = "list";
            this.list.ShowGroups = false;
            this.list.Size = new System.Drawing.Size(111, 113);
            this.list.TabIndex = 0;
            this.list.UseCompatibleStateImageBehavior = false;
            this.list.View = System.Windows.Forms.View.List;
            this.list.SelectedIndexChanged += new System.EventHandler(this.list_SelectedIndexChanged);
            // 
            // selected_text
            // 
            this.selected_text.Location = new System.Drawing.Point(124, 20);
            this.selected_text.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.selected_text.Name = "selected_text";
            this.selected_text.Size = new System.Drawing.Size(95, 20);
            this.selected_text.TabIndex = 1;
            this.selected_text.TextChanged += new System.EventHandler(this.selected_text_TextChanged);
            // 
            // List_Control
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.title);
            this.Name = "List_Control";
            this.Size = new System.Drawing.Size(231, 144);
            this.title.ResumeLayout(false);
            this.title.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button add;
        private System.Windows.Forms.Button remove;
        public System.Windows.Forms.GroupBox title;
        public System.Windows.Forms.ListView list;
        private System.Windows.Forms.TextBox selected_text;
    }
}
