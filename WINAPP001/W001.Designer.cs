namespace WINAPP001
{
    partial class W001
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(W001));
            this.linklTitle = new System.Windows.Forms.LinkLabel();
            this.lvO001 = new System.Windows.Forms.ListView();
            this.btAction001 = new System.Windows.Forms.Button();
            this.btAction002 = new System.Windows.Forms.Button();
            this.btAction003 = new System.Windows.Forms.Button();
            this.lvO002 = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // linklTitle
            // 
            resources.ApplyResources(this.linklTitle, "linklTitle");
            this.linklTitle.ActiveLinkColor = System.Drawing.Color.Black;
            this.linklTitle.Name = "linklTitle";
            this.linklTitle.TabStop = true;
            this.linklTitle.Tag = "https://nft.io/profile/ahz00x86/listed";
            // 
            // lvO001
            // 
            resources.ApplyResources(this.lvO001, "lvO001");
            this.lvO001.HideSelection = false;
            this.lvO001.Name = "lvO001";
            this.lvO001.UseCompatibleStateImageBehavior = false;
            // 
            // btAction001
            // 
            resources.ApplyResources(this.btAction001, "btAction001");
            this.btAction001.BackColor = System.Drawing.Color.Azure;
            this.btAction001.Name = "btAction001";
            this.btAction001.UseVisualStyleBackColor = false;
            this.btAction001.Click += new System.EventHandler(this.btAction001_Click);
            // 
            // btAction002
            // 
            resources.ApplyResources(this.btAction002, "btAction002");
            this.btAction002.BackColor = System.Drawing.Color.Azure;
            this.btAction002.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btAction002.Name = "btAction002";
            this.btAction002.UseVisualStyleBackColor = false;
            this.btAction002.Click += new System.EventHandler(this.btAction002_Click);
            // 
            // btAction003
            // 
            resources.ApplyResources(this.btAction003, "btAction003");
            this.btAction003.BackColor = System.Drawing.Color.Azure;
            this.btAction003.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btAction003.Name = "btAction003";
            this.btAction003.UseVisualStyleBackColor = false;
            this.btAction003.Click += new System.EventHandler(this.btAction003_Click);
            // 
            // lvO002
            // 
            resources.ApplyResources(this.lvO002, "lvO002");
            this.lvO002.HideSelection = false;
            this.lvO002.Name = "lvO002";
            this.lvO002.UseCompatibleStateImageBehavior = false;
            // 
            // W001
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lvO002);
            this.Controls.Add(this.btAction003);
            this.Controls.Add(this.btAction002);
            this.Controls.Add(this.btAction001);
            this.Controls.Add(this.lvO001);
            this.Controls.Add(this.linklTitle);
            this.Name = "W001";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel linklTitle;
        private System.Windows.Forms.ListView lvO001;
        private System.Windows.Forms.Button btAction001;
        private System.Windows.Forms.Button btAction002;
        private System.Windows.Forms.Button btAction003;
        private System.Windows.Forms.ListView lvO002;
    }
}