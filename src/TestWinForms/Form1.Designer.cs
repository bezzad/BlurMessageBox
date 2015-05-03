namespace TestWinForms
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
            this.btnShowMessage = new System.Windows.Forms.Button();
            this.btnLongText = new System.Windows.Forms.Button();
            this.btnTestAll = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnShowMessage
            // 
            this.btnShowMessage.Location = new System.Drawing.Point(154, 12);
            this.btnShowMessage.Name = "btnShowMessage";
            this.btnShowMessage.Size = new System.Drawing.Size(178, 55);
            this.btnShowMessage.TabIndex = 0;
            this.btnShowMessage.Text = "Show Message";
            this.btnShowMessage.UseVisualStyleBackColor = true;
            this.btnShowMessage.Click += new System.EventHandler(this.btnShowMessage_Click);
            // 
            // btnLongText
            // 
            this.btnLongText.Location = new System.Drawing.Point(154, 73);
            this.btnLongText.Name = "btnLongText";
            this.btnLongText.Size = new System.Drawing.Size(178, 55);
            this.btnLongText.TabIndex = 0;
            this.btnLongText.Text = "Show Long Text Message";
            this.btnLongText.UseVisualStyleBackColor = true;
            this.btnLongText.Click += new System.EventHandler(this.btnLongText_Click);
            // 
            // btnTestAll
            // 
            this.btnTestAll.Location = new System.Drawing.Point(154, 134);
            this.btnTestAll.Name = "btnTestAll";
            this.btnTestAll.Size = new System.Drawing.Size(178, 55);
            this.btnTestAll.TabIndex = 0;
            this.btnTestAll.Text = "Show Test Message";
            this.btnTestAll.UseVisualStyleBackColor = true;
            this.btnTestAll.Click += new System.EventHandler(this.btnTestAll_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(514, 414);
            this.Controls.Add(this.btnTestAll);
            this.Controls.Add(this.btnLongText);
            this.Controls.Add(this.btnShowMessage);
            this.Name = "Form1";
            this.Text = "System.Windows.Forms.WinForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnShowMessage;
        private System.Windows.Forms.Button btnLongText;
        private System.Windows.Forms.Button btnTestAll;
    }
}

