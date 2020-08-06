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
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ArmoryCheck
            // 
            this.ArmoryCheck.Location = new System.Drawing.Point(7, 19);
            this.ArmoryCheck.Name = "ArmoryCheck";
            this.ArmoryCheck.Size = new System.Drawing.Size(117, 24);
            this.ArmoryCheck.TabIndex = 0;
            this.ArmoryCheck.Text = "Include Armory";
            this.ArmoryCheck.UseVisualStyleBackColor = true;
            this.ArmoryCheck.CheckedChanged += new System.EventHandler(this.ArmoryCheck_CheckedChanged);
            // 
            // DEindexCheck
            // 
            this.DEindexCheck.Location = new System.Drawing.Point(7, 38);
            this.DEindexCheck.Name = "DEindexCheck";
            this.DEindexCheck.Size = new System.Drawing.Size(171, 49);
            this.DEindexCheck.TabIndex = 1;
            this.DEindexCheck.Text = "Include everything under 10000 DE index";
            this.DEindexCheck.UseVisualStyleBackColor = true;
            this.DEindexCheck.CheckedChanged += new System.EventHandler(this.DEindexCheck_CheckedChanged);
            // 
            // RunCheck
            // 
            this.RunCheck.Location = new System.Drawing.Point(7, 83);
            this.RunCheck.Name = "RunCheck";
            this.RunCheck.Size = new System.Drawing.Size(160, 32);
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
            this.groupBox1.Location = new System.Drawing.Point(196, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(197, 137);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "AE Reduction";
            // 
            // setCurrentZoneBtn
            // 
            this.setCurrentZoneBtn.Location = new System.Drawing.Point(15, 92);
            this.setCurrentZoneBtn.Name = "setCurrentZoneBtn";
            this.setCurrentZoneBtn.Size = new System.Drawing.Size(111, 23);
            this.setCurrentZoneBtn.TabIndex = 3;
            this.setCurrentZoneBtn.Text = "Set Current Zone";
            this.setCurrentZoneBtn.UseVisualStyleBackColor = true;
            this.setCurrentZoneBtn.Click += new System.EventHandler(this.setCurrentZoneBtn_Click);
            // 
            // setZoneBtn
            // 
            this.setZoneBtn.Location = new System.Drawing.Point(81, 67);
            this.setZoneBtn.Name = "setZoneBtn";
            this.setZoneBtn.Size = new System.Drawing.Size(45, 20);
            this.setZoneBtn.TabIndex = 2;
            this.setZoneBtn.Text = "Set";
            this.setZoneBtn.UseVisualStyleBackColor = true;
            this.setZoneBtn.Click += new System.EventHandler(this.setZoneBtn_Click);
            // 
            // textZone
            // 
            this.textZone.Location = new System.Drawing.Point(15, 67);
            this.textZone.Name = "textZone";
            this.textZone.Size = new System.Drawing.Size(59, 20);
            this.textZone.TabIndex = 1;
            // 
            // checkBox1
            // 
            this.checkBox1.Location = new System.Drawing.Point(7, 19);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(119, 42);
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
            this.groupBox2.Location = new System.Drawing.Point(11, 10);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(177, 130);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Desynthesis";
            // 
            // checkBox2
            // 
            this.checkBox2.Location = new System.Drawing.Point(404, 23);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(94, 29);
            this.checkBox2.TabIndex = 6;
            this.checkBox2.Text = "Open Coffers";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(520, 157);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Settings";
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.Settings_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.CheckBox ArmoryCheck;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox DEindexCheck;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox RunCheck;
        private System.Windows.Forms.Button setCurrentZoneBtn;
        private System.Windows.Forms.Button setZoneBtn;
        private System.Windows.Forms.TextBox textZone;

        #endregion
    }
}