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
            ((System.ComponentModel.ISupportInitialize) (this.bindingSourceInventory)).BeginInit();
            this.SuspendLayout();
            // 
            // itemCb
            // 
            this.itemCb.BackColor = System.Drawing.SystemColors.Info;
            this.itemCb.DataSource = this.bindingSourceInventory;
            this.itemCb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.itemCb.FormattingEnabled = true;
            this.itemCb.Location = new System.Drawing.Point(12, 15);
            this.itemCb.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.itemCb.Name = "itemCb";
            this.itemCb.Size = new System.Drawing.Size(196, 23);
            this.itemCb.TabIndex = 0;
            // 
            // materiaListBox
            // 
            this.materiaListBox.BackColor = System.Drawing.SystemColors.Info;
            this.materiaListBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.materiaListBox.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.materiaListBox.FormattingEnabled = true;
            this.materiaListBox.ItemHeight = 17;
            this.materiaListBox.Location = new System.Drawing.Point(12, 45);
            this.materiaListBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.materiaListBox.Name = "materiaListBox";
            this.materiaListBox.Size = new System.Drawing.Size(272, 138);
            this.materiaListBox.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Salmon;
            this.button1.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.button1.Location = new System.Drawing.Point(12, 189);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(271, 31);
            this.button1.TabIndex = 2;
            this.button1.Text = "Remove All Materia";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.LimeGreen;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Location = new System.Drawing.Point(216, 15);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(66, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "Refresh";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // MateriaSettingsFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.SaddleBrown;
            this.ClientSize = new System.Drawing.Size(290, 232);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.materiaListBox);
            this.Controls.Add(this.itemCb);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "MateriaSettingsFrm";
            this.Text = "Materia Settings";
            this.Load += new System.EventHandler(this.MateriaSettingsFrm_Load);
            ((System.ComponentModel.ISupportInitialize) (this.bindingSourceInventory)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.BindingSource bindingSourceInventory;
        private System.Windows.Forms.ComboBox itemCb;
        private System.Windows.Forms.ListBox materiaListBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}