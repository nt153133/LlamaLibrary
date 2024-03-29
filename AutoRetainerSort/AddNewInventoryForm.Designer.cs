﻿using System.ComponentModel;

namespace LlamaLibrary.AutoRetainerSort
{
    partial class AddNewInventoryForm
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
            this.numUpDownIndex = new System.Windows.Forms.NumericUpDown();
            this.txtBoxName = new System.Windows.Forms.TextBox();
            this.labelIndex = new System.Windows.Forms.Label();
            this.labelName = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cmbRetainers = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownIndex)).BeginInit();
            this.SuspendLayout();
            // 
            // numUpDownIndex
            // 
            this.numUpDownIndex.Location = new System.Drawing.Point(52, 39);
            this.numUpDownIndex.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numUpDownIndex.Name = "numUpDownIndex";
            this.numUpDownIndex.Size = new System.Drawing.Size(49, 20);
            this.numUpDownIndex.TabIndex = 0;
            this.numUpDownIndex.ValueChanged += new System.EventHandler(this.Index_Changed);
            // 
            // txtBoxName
            // 
            this.txtBoxName.Location = new System.Drawing.Point(52, 65);
            this.txtBoxName.Name = "txtBoxName";
            this.txtBoxName.Size = new System.Drawing.Size(125, 20);
            this.txtBoxName.TabIndex = 1;
            // 
            // labelIndex
            // 
            this.labelIndex.AutoSize = true;
            this.labelIndex.Location = new System.Drawing.Point(12, 41);
            this.labelIndex.Name = "labelIndex";
            this.labelIndex.Size = new System.Drawing.Size(33, 13);
            this.labelIndex.TabIndex = 2;
            this.labelIndex.Text = "Index";
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Location = new System.Drawing.Point(12, 68);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(35, 13);
            this.labelName.TabIndex = 3;
            this.labelName.Text = "Name";
            // 
            // btnAdd
            // 
            this.btnAdd.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnAdd.Location = new System.Drawing.Point(12, 95);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(49, 20);
            this.btnAdd.TabIndex = 2;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.Add_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(129, 95);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(49, 20);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.TabStop = false;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // cmbRetainers
            // 
            this.cmbRetainers.FormattingEnabled = true;
            this.cmbRetainers.Location = new System.Drawing.Point(12, 12);
            this.cmbRetainers.Name = "cmbRetainers";
            this.cmbRetainers.Size = new System.Drawing.Size(237, 21);
            this.cmbRetainers.TabIndex = 5;
            this.cmbRetainers.SelectedIndexChanged += new System.EventHandler(this.cmbRetainers_SelectedIndexChanged);
            // 
            // AddNewInventoryForm
            // 
            this.AcceptButton = this.btnAdd;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(269, 125);
            this.Controls.Add(this.cmbRetainers);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.labelName);
            this.Controls.Add(this.labelIndex);
            this.Controls.Add(this.txtBoxName);
            this.Controls.Add(this.numUpDownIndex);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddNewInventoryForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Add New Retainer";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.AddNewInventoryForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownIndex)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.ComboBox cmbRetainers;

        #endregion

        private System.Windows.Forms.NumericUpDown numUpDownIndex;
        private System.Windows.Forms.TextBox txtBoxName;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label labelIndex;
    }
}