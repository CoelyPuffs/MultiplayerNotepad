namespace MultiplayerNotepad
{
    partial class DocumentEditor
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DocumentEditor));
            this.letterbox = new System.Windows.Forms.RichTextBox();
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            this.fontButton = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.BtnBold = new System.Windows.Forms.Button();
            this.BtnItalic = new System.Windows.Forms.Button();
            this.BtnUnderline = new System.Windows.Forms.Button();
            this.BtnInc = new System.Windows.Forms.Button();
            this.BtnDec = new System.Windows.Forms.Button();
            this.BtnSave = new System.Windows.Forms.Button();
            this.BtnLeft = new System.Windows.Forms.Button();
            this.BtnCenter = new System.Windows.Forms.Button();
            this.BtnRight = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // letterbox
            // 
            this.letterbox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.letterbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.letterbox.Location = new System.Drawing.Point(11, 46);
            this.letterbox.Margin = new System.Windows.Forms.Padding(2);
            this.letterbox.Name = "letterbox";
            this.letterbox.Size = new System.Drawing.Size(596, 344);
            this.letterbox.TabIndex = 0;
            this.letterbox.Text = "";
            // 
            // fontButton
            // 
            this.fontButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.fontButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.fontButton.Location = new System.Drawing.Point(11, 14);
            this.fontButton.Margin = new System.Windows.Forms.Padding(2);
            this.fontButton.Name = "fontButton";
            this.fontButton.Size = new System.Drawing.Size(81, 23);
            this.fontButton.TabIndex = 1;
            this.fontButton.Text = "Change Font";
            this.fontButton.UseVisualStyleBackColor = true;
            this.fontButton.Click += new System.EventHandler(this.fontButton_Click);
            // 
            // BtnBold
            // 
            this.BtnBold.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.BtnBold.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnBold.Location = new System.Drawing.Point(97, 14);
            this.BtnBold.Name = "BtnBold";
            this.BtnBold.Size = new System.Drawing.Size(63, 23);
            this.BtnBold.TabIndex = 2;
            this.BtnBold.Text = "Bold";
            this.BtnBold.UseVisualStyleBackColor = true;
            // 
            // BtnItalic
            // 
            this.BtnItalic.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.BtnItalic.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnItalic.Location = new System.Drawing.Point(166, 14);
            this.BtnItalic.Name = "BtnItalic";
            this.BtnItalic.Size = new System.Drawing.Size(63, 23);
            this.BtnItalic.TabIndex = 3;
            this.BtnItalic.Text = "Italic";
            this.BtnItalic.UseVisualStyleBackColor = true;
            // 
            // BtnUnderline
            // 
            this.BtnUnderline.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.BtnUnderline.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnUnderline.Location = new System.Drawing.Point(235, 14);
            this.BtnUnderline.Name = "BtnUnderline";
            this.BtnUnderline.Size = new System.Drawing.Size(63, 23);
            this.BtnUnderline.TabIndex = 4;
            this.BtnUnderline.Text = "Underline";
            this.BtnUnderline.UseVisualStyleBackColor = true;
            // 
            // BtnInc
            // 
            this.BtnInc.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.BtnInc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnInc.Image = ((System.Drawing.Image)(resources.GetObject("BtnInc.Image")));
            this.BtnInc.Location = new System.Drawing.Point(315, 14);
            this.BtnInc.Name = "BtnInc";
            this.BtnInc.Size = new System.Drawing.Size(30, 23);
            this.BtnInc.TabIndex = 5;
            this.BtnInc.UseVisualStyleBackColor = true;
            // 
            // BtnDec
            // 
            this.BtnDec.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.BtnDec.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnDec.Image = ((System.Drawing.Image)(resources.GetObject("BtnDec.Image")));
            this.BtnDec.Location = new System.Drawing.Point(351, 14);
            this.BtnDec.Name = "BtnDec";
            this.BtnDec.Size = new System.Drawing.Size(30, 23);
            this.BtnDec.TabIndex = 6;
            this.BtnDec.UseVisualStyleBackColor = true;
            // 
            // BtnSave
            // 
            this.BtnSave.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.BtnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnSave.Location = new System.Drawing.Point(534, 14);
            this.BtnSave.Name = "BtnSave";
            this.BtnSave.Size = new System.Drawing.Size(65, 23);
            this.BtnSave.TabIndex = 7;
            this.BtnSave.Text = "Save";
            this.BtnSave.UseVisualStyleBackColor = true;
            // 
            // BtnLeft
            // 
            this.BtnLeft.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.BtnLeft.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnLeft.Image = ((System.Drawing.Image)(resources.GetObject("BtnLeft.Image")));
            this.BtnLeft.Location = new System.Drawing.Point(387, 14);
            this.BtnLeft.Name = "BtnLeft";
            this.BtnLeft.Size = new System.Drawing.Size(40, 23);
            this.BtnLeft.TabIndex = 8;
            this.BtnLeft.UseVisualStyleBackColor = true;
            // 
            // BtnCenter
            // 
            this.BtnCenter.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.BtnCenter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnCenter.Image = ((System.Drawing.Image)(resources.GetObject("BtnCenter.Image")));
            this.BtnCenter.Location = new System.Drawing.Point(433, 14);
            this.BtnCenter.Name = "BtnCenter";
            this.BtnCenter.Size = new System.Drawing.Size(40, 23);
            this.BtnCenter.TabIndex = 9;
            this.BtnCenter.UseVisualStyleBackColor = true;
            // 
            // BtnRight
            // 
            this.BtnRight.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.BtnRight.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnRight.Image = ((System.Drawing.Image)(resources.GetObject("BtnRight.Image")));
            this.BtnRight.Location = new System.Drawing.Point(479, 14);
            this.BtnRight.Name = "BtnRight";
            this.BtnRight.Size = new System.Drawing.Size(40, 23);
            this.BtnRight.TabIndex = 10;
            this.BtnRight.UseVisualStyleBackColor = true;
            // 
            // DocumentEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(611, 395);
            this.Controls.Add(this.BtnRight);
            this.Controls.Add(this.BtnCenter);
            this.Controls.Add(this.BtnLeft);
            this.Controls.Add(this.BtnSave);
            this.Controls.Add(this.BtnDec);
            this.Controls.Add(this.BtnInc);
            this.Controls.Add(this.BtnUnderline);
            this.Controls.Add(this.BtnItalic);
            this.Controls.Add(this.BtnBold);
            this.Controls.Add(this.fontButton);
            this.Controls.Add(this.letterbox);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "DocumentEditor";
            this.Text = "Multiplayer Notepad";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox letterbox;
        private System.Windows.Forms.FontDialog fontDialog1;
        private System.Windows.Forms.Button fontButton;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button BtnBold;
        private System.Windows.Forms.Button BtnItalic;
        private System.Windows.Forms.Button BtnUnderline;
        private System.Windows.Forms.Button BtnInc;
        private System.Windows.Forms.Button BtnDec;
        private System.Windows.Forms.Button BtnSave;
        private System.Windows.Forms.Button BtnLeft;
        private System.Windows.Forms.Button BtnCenter;
        private System.Windows.Forms.Button BtnRight;
    }
}

