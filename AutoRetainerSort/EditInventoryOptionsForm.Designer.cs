using System.ComponentModel;

namespace LlamaLibrary.AutoRetainerSort
{
    partial class EditInventoryOptionsForm
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
            this.listBoxSortTypes = new System.Windows.Forms.ListBox();
            this.listBoxItemIds = new System.Windows.Forms.ListBox();
            this.txtBoxName = new System.Windows.Forms.TextBox();
            this.labelName = new System.Windows.Forms.Label();
            this.labelSortTypes = new System.Windows.Forms.Label();
            this.labelItemIds = new System.Windows.Forms.Label();
            this.comboBoxSortType = new System.Windows.Forms.ComboBox();
            this.btnAddSortType = new System.Windows.Forms.Button();
            this.btnAddItemId = new System.Windows.Forms.Button();
            this.numUpDownItemId = new System.Windows.Forms.NumericUpDown();
            this.btnDeleteSortType = new System.Windows.Forms.Button();
            this.btnDeleteItemId = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownItemId)).BeginInit();
            this.SuspendLayout();
            // 
            // listBoxSortTypes
            // 
            this.listBoxSortTypes.FormattingEnabled = true;
            this.listBoxSortTypes.Location = new System.Drawing.Point(12, 65);
            this.listBoxSortTypes.Name = "listBoxSortTypes";
            this.listBoxSortTypes.ScrollAlwaysVisible = true;
            this.listBoxSortTypes.Size = new System.Drawing.Size(240, 329);
            this.listBoxSortTypes.TabIndex = 0;
            this.listBoxSortTypes.TabStop = false;
            // 
            // listBoxItemIds
            // 
            this.listBoxItemIds.FormattingEnabled = true;
            this.listBoxItemIds.Location = new System.Drawing.Point(288, 65);
            this.listBoxItemIds.Name = "listBoxItemIds";
            this.listBoxItemIds.ScrollAlwaysVisible = true;
            this.listBoxItemIds.Size = new System.Drawing.Size(175, 329);
            this.listBoxItemIds.TabIndex = 1;
            this.listBoxItemIds.TabStop = false;
            // 
            // txtBoxName
            // 
            this.txtBoxName.Location = new System.Drawing.Point(48, 9);
            this.txtBoxName.Name = "txtBoxName";
            this.txtBoxName.Size = new System.Drawing.Size(204, 20);
            this.txtBoxName.TabIndex = 2;
            this.txtBoxName.TextChanged += new System.EventHandler(this.Name_TextChanged);
            // 
            // labelName
            // 
            this.labelName.Location = new System.Drawing.Point(7, 12);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(35, 19);
            this.labelName.TabIndex = 3;
            this.labelName.Text = "Name";
            // 
            // labelSortTypes
            // 
            this.labelSortTypes.Location = new System.Drawing.Point(12, 43);
            this.labelSortTypes.Name = "labelSortTypes";
            this.labelSortTypes.Size = new System.Drawing.Size(66, 19);
            this.labelSortTypes.TabIndex = 4;
            this.labelSortTypes.Text = "Sort Types";
            // 
            // labelItemIds
            // 
            this.labelItemIds.Location = new System.Drawing.Point(285, 41);
            this.labelItemIds.Name = "labelItemIds";
            this.labelItemIds.Size = new System.Drawing.Size(74, 19);
            this.labelItemIds.TabIndex = 5;
            this.labelItemIds.Text = "True Item Ids";
            // 
            // comboBoxSortType
            // 
            this.comboBoxSortType.FormattingEnabled = true;
            this.comboBoxSortType.Location = new System.Drawing.Point(12, 400);
            this.comboBoxSortType.Name = "comboBoxSortType";
            this.comboBoxSortType.Size = new System.Drawing.Size(152, 21);
            this.comboBoxSortType.TabIndex = 6;
            this.comboBoxSortType.TabStop = false;
            // 
            // btnAddSortType
            // 
            this.btnAddSortType.Location = new System.Drawing.Point(170, 399);
            this.btnAddSortType.Name = "btnAddSortType";
            this.btnAddSortType.Size = new System.Drawing.Size(82, 21);
            this.btnAddSortType.TabIndex = 3;
            this.btnAddSortType.Text = "Add";
            this.btnAddSortType.UseVisualStyleBackColor = true;
            this.btnAddSortType.Click += new System.EventHandler(this.AddNewSortType_Click);
            // 
            // btnAddItemId
            // 
            this.btnAddItemId.Location = new System.Drawing.Point(402, 398);
            this.btnAddItemId.Name = "btnAddItemId";
            this.btnAddItemId.Size = new System.Drawing.Size(61, 21);
            this.btnAddItemId.TabIndex = 4;
            this.btnAddItemId.Text = "Add";
            this.btnAddItemId.UseVisualStyleBackColor = true;
            this.btnAddItemId.Click += new System.EventHandler(this.AddNewItemId_Click);
            // 
            // numUpDownItemId
            // 
            this.numUpDownItemId.Location = new System.Drawing.Point(288, 398);
            this.numUpDownItemId.Maximum = new decimal(new int[] {
            4000000,
            0,
            0,
            0});
            this.numUpDownItemId.Name = "numUpDownItemId";
            this.numUpDownItemId.Size = new System.Drawing.Size(108, 20);
            this.numUpDownItemId.TabIndex = 10;
            // 
            // btnDeleteSortType
            // 
            this.btnDeleteSortType.Location = new System.Drawing.Point(84, 39);
            this.btnDeleteSortType.Name = "btnDeleteSortType";
            this.btnDeleteSortType.Size = new System.Drawing.Size(98, 21);
            this.btnDeleteSortType.TabIndex = 0;
            this.btnDeleteSortType.Text = "Delete Selected";
            this.btnDeleteSortType.UseVisualStyleBackColor = true;
            this.btnDeleteSortType.Click += new System.EventHandler(this.DeleteSelectedSortType_Click);
            // 
            // btnDeleteItemId
            // 
            this.btnDeleteItemId.Location = new System.Drawing.Point(365, 39);
            this.btnDeleteItemId.Name = "btnDeleteItemId";
            this.btnDeleteItemId.Size = new System.Drawing.Size(98, 21);
            this.btnDeleteItemId.TabIndex = 1;
            this.btnDeleteItemId.Text = "Delete Selected";
            this.btnDeleteItemId.UseVisualStyleBackColor = true;
            this.btnDeleteItemId.Click += new System.EventHandler(this.DeleteSelectedItemId_Click);
            // 
            // EditInventoryOptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(475, 432);
            this.Controls.Add(this.btnDeleteItemId);
            this.Controls.Add(this.btnDeleteSortType);
            this.Controls.Add(this.numUpDownItemId);
            this.Controls.Add(this.btnAddItemId);
            this.Controls.Add(this.btnAddSortType);
            this.Controls.Add(this.comboBoxSortType);
            this.Controls.Add(this.labelItemIds);
            this.Controls.Add(this.labelSortTypes);
            this.Controls.Add(this.labelName);
            this.Controls.Add(this.txtBoxName);
            this.Controls.Add(this.listBoxItemIds);
            this.Controls.Add(this.listBoxSortTypes);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditInventoryOptionsForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Sort Options";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Form_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownItemId)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Button btnDeleteSortType;

        private System.Windows.Forms.Button btnAddItemId;
        private System.Windows.Forms.NumericUpDown numUpDownItemId;
        private System.Windows.Forms.ComboBox comboBoxSortType;
        private System.Windows.Forms.Button btnDeleteItemId;
        
        private System.Windows.Forms.Button btnAddSortType;

        private System.Windows.Forms.TextBox txtBoxName;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.Label labelSortTypes;
        private System.Windows.Forms.Label labelItemIds;

        private System.Windows.Forms.ListBox listBoxSortTypes;
        private System.Windows.Forms.ListBox listBoxItemIds;

        #endregion
    }
}