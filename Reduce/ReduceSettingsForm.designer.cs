using System.ComponentModel;

namespace LlamaLibrary.Reduce
{
    partial class Settings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            this.ArmoryCheck = new System.Windows.Forms.CheckBox();
            this.DEindexCheck = new System.Windows.Forms.CheckBox();
            this.RunCheck = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.setCurrentZoneBtn = new System.Windows.Forms.Button();
            this.setZoneBtn = new System.Windows.Forms.Button();
            this.textZone = new System.Windows.Forms.TextBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ArmoryCheck
            // 
            this.ArmoryCheck.Location = new System.Drawing.Point(8, 22);
            this.ArmoryCheck.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ArmoryCheck.Name = "ArmoryCheck";
            this.ArmoryCheck.Size = new System.Drawing.Size(136, 28);
            this.ArmoryCheck.TabIndex = 0;
            this.ArmoryCheck.Text = "Include Armory";
            this.ArmoryCheck.UseVisualStyleBackColor = true;
            this.ArmoryCheck.CheckedChanged += new System.EventHandler(this.ArmoryCheck_CheckedChanged);
            // 
            // DEindexCheck
            // 
            this.DEindexCheck.Location = new System.Drawing.Point(8, 44);
            this.DEindexCheck.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.DEindexCheck.Name = "DEindexCheck";
            this.DEindexCheck.Size = new System.Drawing.Size(200, 57);
            this.DEindexCheck.TabIndex = 1;
            this.DEindexCheck.Text = "Include everything under 10000 DE index";
            this.DEindexCheck.UseVisualStyleBackColor = true;
            this.DEindexCheck.CheckedChanged += new System.EventHandler(this.DEindexCheck_CheckedChanged);
            // 
            // RunCheck
            // 
            this.RunCheck.Location = new System.Drawing.Point(8, 96);
            this.RunCheck.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.RunCheck.Name = "RunCheck";
            this.RunCheck.Size = new System.Drawing.Size(187, 37);
            this.RunCheck.TabIndex = 2;
            this.RunCheck.Text = "StayRunning";
            this.RunCheck.UseVisualStyleBackColor = true;
            this.RunCheck.CheckedChanged += new System.EventHandler(this.RunCheck_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.setCurrentZoneBtn);
            this.groupBox1.Controls.Add(this.setZoneBtn);
            this.groupBox1.Controls.Add(this.textZone);
            this.groupBox1.Controls.Add(this.checkBox1);
            this.groupBox1.Location = new System.Drawing.Point(229, 12);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox1.Size = new System.Drawing.Size(230, 158);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "AE Reduction";
            // 
            // setCurrentZoneBtn
            // 
            this.setCurrentZoneBtn.Location = new System.Drawing.Point(17, 106);
            this.setCurrentZoneBtn.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.setCurrentZoneBtn.Name = "setCurrentZoneBtn";
            this.setCurrentZoneBtn.Size = new System.Drawing.Size(130, 27);
            this.setCurrentZoneBtn.TabIndex = 3;
            this.setCurrentZoneBtn.Text = "Set Current Zone";
            this.setCurrentZoneBtn.UseVisualStyleBackColor = true;
            this.setCurrentZoneBtn.Click += new System.EventHandler(this.setCurrentZoneBtn_Click);
            // 
            // setZoneBtn
            // 
            this.setZoneBtn.Location = new System.Drawing.Point(95, 77);
            this.setZoneBtn.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.setZoneBtn.Name = "setZoneBtn";
            this.setZoneBtn.Size = new System.Drawing.Size(52, 23);
            this.setZoneBtn.TabIndex = 2;
            this.setZoneBtn.Text = "Set";
            this.setZoneBtn.UseVisualStyleBackColor = true;
            this.setZoneBtn.Click += new System.EventHandler(this.setZoneBtn_Click);
            // 
            // textZone
            // 
            this.textZone.Location = new System.Drawing.Point(18, 77);
            this.textZone.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textZone.Name = "textZone";
            this.textZone.Size = new System.Drawing.Size(68, 23);
            this.textZone.TabIndex = 1;
            // 
            // checkBox1
            // 
            this.checkBox1.Location = new System.Drawing.Point(8, 22);
            this.checkBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(139, 49);
            this.checkBox1.TabIndex = 0;
            this.checkBox1.Text = "Only Reduce in zone";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.ArmoryCheck);
            this.groupBox2.Controls.Add(this.DEindexCheck);
            this.groupBox2.Controls.Add(this.RunCheck);
            this.groupBox2.Location = new System.Drawing.Point(13, 12);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox2.Size = new System.Drawing.Size(206, 150);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Desynthesis";
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(502, 181);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "Settings";
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.Settings_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.CheckBox ArmoryCheck;
        private System.Windows.Forms.CheckBox DEindexCheck;
        private System.Windows.Forms.CheckBox RunCheck;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.TextBox textZone;
        private System.Windows.Forms.Button setZoneBtn;
        private System.Windows.Forms.Button setCurrentZoneBtn;
    }
}