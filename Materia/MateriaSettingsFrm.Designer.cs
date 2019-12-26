using System.ComponentModel;

namespace LlamaLibrary.Materia
{
    partial class MateriaSettingsFrm
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
            this.components = new System.ComponentModel.Container();
            this.bindingSourceInventory = new System.Windows.Forms.BindingSource(this.components);
            this.itemCb = new System.Windows.Forms.ComboBox();
            this.materiaListBox = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.affixLb = new System.Windows.Forms.ListBox();
            this.affixCb = new System.Windows.Forms.ComboBox();
            this.bindingSourceAffix = new System.Windows.Forms.BindingSource(this.components);
            this.button3 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize) (this.bindingSourceInventory)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) (this.bindingSourceAffix)).BeginInit();
            this.SuspendLayout();
            // 
            // itemCb
            // 
            this.itemCb.BackColor = System.Drawing.SystemColors.Info;
            this.itemCb.DataSource = this.bindingSourceInventory;
            this.itemCb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.itemCb.FormattingEnabled = true;
            this.itemCb.Location = new System.Drawing.Point(9, 6);
            this.itemCb.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.itemCb.Name = "itemCb";
            this.itemCb.Size = new System.Drawing.Size(250, 23);
            this.itemCb.TabIndex = 0;
            // 
            // materiaListBox
            // 
            this.materiaListBox.BackColor = System.Drawing.SystemColors.Info;
            this.materiaListBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.materiaListBox.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.materiaListBox.FormattingEnabled = true;
            this.materiaListBox.ItemHeight = 17;
            this.materiaListBox.Location = new System.Drawing.Point(8, 36);
            this.materiaListBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.materiaListBox.Name = "materiaListBox";
            this.materiaListBox.Size = new System.Drawing.Size(325, 189);
            this.materiaListBox.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Salmon;
            this.button1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.button1.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.button1.Location = new System.Drawing.Point(4, 240);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(333, 31);
            this.button1.TabIndex = 2;
            this.button1.Text = "Remove All Materia";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.LimeGreen;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Location = new System.Drawing.Point(267, 6);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(66, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "Refresh";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(349, 302);
            this.tabControl1.TabIndex = 4;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.SaddleBrown;
            this.tabPage1.Controls.Add(this.itemCb);
            this.tabPage1.Controls.Add(this.button2);
            this.tabPage1.Controls.Add(this.materiaListBox);
            this.tabPage1.Controls.Add(this.button1);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabPage1.Size = new System.Drawing.Size(341, 274);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Remove";
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.SaddleBrown;
            this.tabPage2.Controls.Add(this.affixLb);
            this.tabPage2.Controls.Add(this.affixCb);
            this.tabPage2.Controls.Add(this.button3);
            this.tabPage2.Location = new System.Drawing.Point(4, 24);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabPage2.Size = new System.Drawing.Size(341, 274);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Affix";
            this.tabPage2.Click += new System.EventHandler(this.tabPage2_Click);
            // 
            // affixLb
            // 
            this.affixLb.BackColor = System.Drawing.SystemColors.Info;
            this.affixLb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.affixLb.FormattingEnabled = true;
            this.affixLb.ItemHeight = 15;
            this.affixLb.Location = new System.Drawing.Point(8, 38);
            this.affixLb.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.affixLb.Name = "affixLb";
            this.affixLb.Size = new System.Drawing.Size(322, 152);
            this.affixLb.TabIndex = 6;
            // 
            // affixCb
            // 
            this.affixCb.BackColor = System.Drawing.SystemColors.Info;
            this.affixCb.DataSource = this.bindingSourceAffix;
            this.affixCb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.affixCb.FormattingEnabled = true;
            this.affixCb.Location = new System.Drawing.Point(9, 6);
            this.affixCb.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.affixCb.Name = "affixCb";
            this.affixCb.Size = new System.Drawing.Size(249, 23);
            this.affixCb.TabIndex = 4;
            this.affixCb.SelectedIndexChanged += new System.EventHandler(this.affixCb_SelectedIndexChanged);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.LimeGreen;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Location = new System.Drawing.Point(266, 6);
            this.button3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(66, 23);
            this.button3.TabIndex = 5;
            this.button3.Text = "Refresh";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // MateriaSettingsFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.SaddleBrown;
            this.ClientSize = new System.Drawing.Size(349, 302);
            this.Controls.Add(this.tabControl1);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "MateriaSettingsFrm";
            this.Text = "Materia Settings";
            this.Load += new System.EventHandler(this.MateriaSettingsFrm_Load);
            ((System.ComponentModel.ISupportInitialize) (this.bindingSourceInventory)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize) (this.bindingSourceAffix)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.BindingSource bindingSourceInventory;
        private System.Windows.Forms.ComboBox itemCb;
        private System.Windows.Forms.ListBox materiaListBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.BindingSource bindingSourceAffix;
        private System.Windows.Forms.ComboBox affixCb;
        private System.Windows.Forms.ListBox affixLb;
    }
}