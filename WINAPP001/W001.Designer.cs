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
            this.linklTitle = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // linklTitle
            // 
            this.linklTitle.ActiveLinkColor = System.Drawing.Color.Black;
            this.linklTitle.AutoSize = true;
            this.linklTitle.Location = new System.Drawing.Point(12, 9);
            this.linklTitle.Name = "linklTitle";
            this.linklTitle.Size = new System.Drawing.Size(179, 13);
            this.linklTitle.TabIndex = 0;
            this.linklTitle.TabStop = true;
            this.linklTitle.Tag = "https://nft.io/profile/ahz00x86/listed";
            this.linklTitle.Text = "https://nft.io/profile/ahz00x86/listed";
            // 
            // W001
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.linklTitle);
            this.Name = "W001";
            this.Text = "W001";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel linklTitle;
    }
}