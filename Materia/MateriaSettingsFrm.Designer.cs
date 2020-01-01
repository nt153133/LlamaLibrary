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
            this.panel1 = new System.Windows.Forms.Panel();
            this.MateriaCb5 = new System.Windows.Forms.ComboBox();
            this.MateriaCb4 = new System.Windows.Forms.ComboBox();
            this.MateriaCb3 = new System.Windows.Forms.ComboBox();
            this.MateriaCb2 = new System.Windows.Forms.ComboBox();
            this.MateriaCb1 = new System.Windows.Forms.ComboBox();
            this.affixCb = new System.Windows.Forms.ComboBox();
            this.bindingSourceAffix = new System.Windows.Forms.BindingSource(this.components);
            this.button3 = new System.Windows.Forms.Button();
            this.bindingSourceInventoryMateria = new System.Windows.Forms.BindingSource(this.components);
            this.button4 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize) (this.bindingSourceInventory)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) (this.bindingSourceAffix)).BeginInit();
            ((System.ComponentModel.ISupportInitialize) (this.bindingSourceInventoryMateria)).BeginInit();
            this.SuspendLayout();
            this.itemCb.BackColor = System.Drawing.SystemColors.Info;
            this.itemCb.DataSource = this.bindingSourceInventory;
            this.itemCb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.itemCb.FormattingEnabled = true;
            this.itemCb.Location = new System.Drawing.Point(9, 6);
            this.itemCb.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.itemCb.Name = "itemCb";
            this.itemCb.Size = new System.Drawing.Size(250, 23);
            this.itemCb.TabIndex = 0;
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
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(349, 302);
            this.tabControl1.TabIndex = 4;
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
            this.tabPage2.BackColor = System.Drawing.Color.SaddleBrown;
            this.tabPage2.Controls.Add(this.panel1);
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
            this.panel1.Controls.Add(this.button4);
            this.panel1.Controls.Add(this.MateriaCb5);
            this.panel1.Controls.Add(this.MateriaCb4);
            this.panel1.Controls.Add(this.MateriaCb3);
            this.panel1.Controls.Add(this.MateriaCb2);
            this.panel1.Controls.Add(this.MateriaCb1);
            this.panel1.Location = new System.Drawing.Point(9, 35);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(321, 173);
            this.panel1.TabIndex = 7;
            this.MateriaCb5.BackColor = System.Drawing.SystemColors.Info;
            this.MateriaCb5.Dock = System.Windows.Forms.DockStyle.Top;
            this.MateriaCb5.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.MateriaCb5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.MateriaCb5.FormattingEnabled = true;
            this.MateriaCb5.Location = new System.Drawing.Point(0, 92);
            this.MateriaCb5.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MateriaCb5.Name = "MateriaCb5";
            this.MateriaCb5.Size = new System.Drawing.Size(321, 23);
            this.MateriaCb5.TabIndex = 4;
            this.MateriaCb5.SelectedIndexChanged += new System.EventHandler(this.comboBox4_SelectedIndexChanged);
            this.MateriaCb4.BackColor = System.Drawing.SystemColors.Info;
            this.MateriaCb4.Dock = System.Windows.Forms.DockStyle.Top;
            this.MateriaCb4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.MateriaCb4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.MateriaCb4.FormattingEnabled = true;
            this.MateriaCb4.Location = new System.Drawing.Point(0, 69);
            this.MateriaCb4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MateriaCb4.Name = "MateriaCb4";
            this.MateriaCb4.Size = new System.Drawing.Size(321, 23);
            this.MateriaCb4.TabIndex = 3;
            this.MateriaCb3.BackColor = System.Drawing.SystemColors.Info;
            this.MateriaCb3.Dock = System.Windows.Forms.DockStyle.Top;
            this.MateriaCb3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.MateriaCb3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.MateriaCb3.FormattingEnabled = true;
            this.MateriaCb3.Location = new System.Drawing.Point(0, 46);
            this.MateriaCb3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MateriaCb3.Name = "MateriaCb3";
            this.MateriaCb3.Size = new System.Drawing.Size(321, 23);
            this.MateriaCb3.TabIndex = 2;
            this.MateriaCb2.BackColor = System.Drawing.SystemColors.Info;
            this.MateriaCb2.Dock = System.Windows.Forms.DockStyle.Top;
            this.MateriaCb2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.MateriaCb2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.MateriaCb2.FormattingEnabled = true;
            this.MateriaCb2.Location = new System.Drawing.Point(0, 23);
            this.MateriaCb2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MateriaCb2.Name = "MateriaCb2";
            this.MateriaCb2.Size = new System.Drawing.Size(321, 23);
            this.MateriaCb2.TabIndex = 1;
            this.MateriaCb1.BackColor = System.Drawing.SystemColors.Info;
            this.MateriaCb1.Dock = System.Windows.Forms.DockStyle.Top;
            this.MateriaCb1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.MateriaCb1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.MateriaCb1.FormattingEnabled = true;
            this.MateriaCb1.Location = new System.Drawing.Point(0, 0);
            this.MateriaCb1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MateriaCb1.Name = "MateriaCb1";
            this.MateriaCb1.Size = new System.Drawing.Size(321, 23);
            this.MateriaCb1.TabIndex = 0;
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
            this.button4.BackColor = System.Drawing.Color.Salmon;
            this.button4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.Location = new System.Drawing.Point(0, 144);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(321, 29);
            this.button4.TabIndex = 8;
            this.button4.Text = "Affix";
            this.button4.UseVisualStyleBackColor = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);
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
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize) (this.bindingSourceAffix)).EndInit();
            ((System.ComponentModel.ISupportInitialize) (this.bindingSourceInventoryMateria)).EndInit();
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
        private System.Windows.Forms.ComboBox MateriaCb1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox MateriaCb2;
        private System.Windows.Forms.ComboBox MateriaCb3;
        private System.Windows.Forms.ComboBox MateriaCb4;
        private System.Windows.Forms.ComboBox MateriaCb5;
        private System.Windows.Forms.BindingSource bindingSourceInventoryMateria;
        private System.Windows.Forms.Button button4;
    }
}