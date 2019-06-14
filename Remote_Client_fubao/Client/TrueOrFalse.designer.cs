namespace Remote_Client_fubao.Client
{
    partial class TrueOrFalse
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrueOrFalse));
            this.btn_true = new DSkin.Controls.DSkinButton();
            this.btn_false = new DSkin.Controls.DSkinButton();
            this.dSkinLabel1 = new DSkin.Controls.DSkinLabel();
            this.dSkinLabel2 = new DSkin.Controls.DSkinLabel();
            this.SuspendLayout();
            // 
            // btn_true
            // 
            this.btn_true.AdaptImage = true;
            this.btn_true.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(119)))), ((int)(((byte)(255)))));
            this.btn_true.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(119)))), ((int)(((byte)(255)))));
            this.btn_true.ButtonBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(119)))), ((int)(((byte)(255)))));
            this.btn_true.ButtonBorderWidth = 1;
            this.btn_true.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btn_true.ForeColor = System.Drawing.Color.White;
            this.btn_true.HoverColor = System.Drawing.Color.Empty;
            this.btn_true.HoverImage = null;
            this.btn_true.IsPureColor = true;
            this.btn_true.Location = new System.Drawing.Point(182, 176);
            this.btn_true.Name = "btn_true";
            this.btn_true.NormalImage = null;
            this.btn_true.PressColor = System.Drawing.Color.Empty;
            this.btn_true.PressedImage = null;
            this.btn_true.Radius = 10;
            this.btn_true.ShowButtonBorder = true;
            this.btn_true.Size = new System.Drawing.Size(125, 30);
            this.btn_true.TabIndex = 0;
            this.btn_true.Text = "接受";
            this.btn_true.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btn_true.TextPadding = 0;
            this.btn_true.Click += new System.EventHandler(this.btn_true_Click);
            // 
            // btn_false
            // 
            this.btn_false.AdaptImage = true;
            this.btn_false.BaseColor = System.Drawing.Color.White;
            this.btn_false.ButtonBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(119)))), ((int)(((byte)(255)))));
            this.btn_false.ButtonBorderWidth = 1;
            this.btn_false.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btn_false.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(119)))), ((int)(((byte)(255)))));
            this.btn_false.HoverColor = System.Drawing.Color.Empty;
            this.btn_false.HoverImage = null;
            this.btn_false.IsPureColor = true;
            this.btn_false.Location = new System.Drawing.Point(19, 176);
            this.btn_false.Name = "btn_false";
            this.btn_false.NormalImage = null;
            this.btn_false.PressColor = System.Drawing.Color.Empty;
            this.btn_false.PressedImage = null;
            this.btn_false.Radius = 10;
            this.btn_false.ShowButtonBorder = true;
            this.btn_false.Size = new System.Drawing.Size(125, 30);
            this.btn_false.TabIndex = 1;
            this.btn_false.Text = "不接受";
            this.btn_false.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btn_false.TextPadding = 0;
            this.btn_false.Click += new System.EventHandler(this.btn_false_Click);
            // 
            // dSkinLabel1
            // 
            this.dSkinLabel1.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold);
            this.dSkinLabel1.Location = new System.Drawing.Point(35, 26);
            this.dSkinLabel1.Name = "dSkinLabel1";
            this.dSkinLabel1.Size = new System.Drawing.Size(90, 29);
            this.dSkinLabel1.TabIndex = 2;
            this.dSkinLabel1.Text = "远程请求";
            // 
            // dSkinLabel2
            // 
            this.dSkinLabel2.Font = new System.Drawing.Font("微软雅黑", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dSkinLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(81)))), ((int)(((byte)(81)))), ((int)(((byte)(81)))));
            this.dSkinLabel2.Location = new System.Drawing.Point(47, 94);
            this.dSkinLabel2.Name = "dSkinLabel2";
            this.dSkinLabel2.Size = new System.Drawing.Size(232, 41);
            this.dSkinLabel2.TabIndex = 3;
            this.dSkinLabel2.Text = "工程师服宝请求与您开始远程服务\r\n是否接受？";
            this.dSkinLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TrueOrFalse
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(326, 241);
            this.ControlBox = false;
            this.Controls.Add(this.dSkinLabel2);
            this.Controls.Add(this.dSkinLabel1);
            this.Controls.Add(this.btn_false);
            this.Controls.Add(this.btn_true);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsLayeredWindowForm = false;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TrueOrFalse";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "远程请求";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DSkin.Controls.DSkinButton btn_true;
        private DSkin.Controls.DSkinButton btn_false;
        private DSkin.Controls.DSkinLabel dSkinLabel1;
        private DSkin.Controls.DSkinLabel dSkinLabel2;
    }
}