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
            this.listBoxItems = new System.Windows.Forms.ListBox();
            this.txtBoxName = new System.Windows.Forms.TextBox();
            this.labelName = new System.Windows.Forms.Label();
            this.labelSortTypes = new System.Windows.Forms.Label();
            this.labelItemIds = new System.Windows.Forms.Label();
            this.comboBoxSortType = new System.Windows.Forms.ComboBox();
            this.btnAddSortType = new System.Windows.Forms.Button();
            this.btnAddNewItem = new System.Windows.Forms.Button();
            this.btnDeleteSortType = new System.Windows.Forms.Button();
            this.btnDeleteItemId = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listBoxSortTypes
            // 
            this.listBoxSortTypes.FormattingEnabled = true;
            this.listBoxSortTypes.Location = new System.Drawing.Point(12, 65);
            this.listBoxSortTypes.Name = "listBoxSortTypes";
            this.listBoxSortTypes.ScrollAlwaysVisible = true;
            this.listBoxSortTypes.Size = new System.Drawing.Size(224, 329);
            this.listBoxSortTypes.TabIndex = 0;
            this.listBoxSortTypes.TabStop = false;
            // 
            // listBoxItems
            // 
            this.listBoxItems.FormattingEnabled = true;
            this.listBoxItems.Location = new System.Drawing.Point(288, 65);
            this.listBoxItems.Name = "listBoxItems";
            this.listBoxItems.ScrollAlwaysVisible = true;
            this.listBoxItems.Size = new System.Drawing.Size(315, 329);
            this.listBoxItems.TabIndex = 1;
            this.listBoxItems.TabStop = false;
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
            this.labelItemIds.Location = new System.Drawing.Point(288, 41);
            this.labelItemIds.Name = "labelItemIds";
            this.labelItemIds.Size = new System.Drawing.Size(32, 19);
            this.labelItemIds.TabIndex = 5;
            this.labelItemIds.Text = "Items";
            // 
            // comboBoxSortType
            // 
            this.comboBoxSortType.FormattingEnabled = true;
            this.comboBoxSortType.Location = new System.Drawing.Point(12, 400);
            this.comboBoxSortType.Name = "comboBoxSortType";
            this.comboBoxSortType.Size = new System.Drawing.Size(136, 21);
            this.comboBoxSortType.TabIndex = 6;
            this.comboBoxSortType.TabStop = false;
            // 
            // btnAddSortType
            // 
            this.btnAddSortType.Location = new System.Drawing.Point(154, 400);
            this.btnAddSortType.Name = "btnAddSortType";
            this.btnAddSortType.Size = new System.Drawing.Size(82, 21);
            this.btnAddSortType.TabIndex = 3;
            this.btnAddSortType.Text = "Add";
            this.btnAddSortType.UseVisualStyleBackColor = true;
            this.btnAddSortType.Click += new System.EventHandler(this.AddNewSortType_Click);
            // 
            // btnAddNewItem
            // 
            this.btnAddNewItem.Location = new System.Drawing.Point(288, 400);
            this.btnAddNewItem.Name = "btnAddNewItem";
            this.btnAddNewItem.Size = new System.Drawing.Size(91, 21);
            this.btnAddNewItem.TabIndex = 4;
            this.btnAddNewItem.Text = "Add New";
            this.btnAddNewItem.UseVisualStyleBackColor = true;
            this.btnAddNewItem.Click += new System.EventHandler(this.AddNewItem_Click);
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
            this.btnDeleteItemId.Location = new System.Drawing.Point(326, 39);
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
            this.ClientSize = new System.Drawing.Size(615, 435);
            this.Controls.Add(this.btnDeleteItemId);
            this.Controls.Add(this.btnDeleteSortType);
            this.Controls.Add(this.btnAddNewItem);
            this.Controls.Add(this.btnAddSortType);
            this.Controls.Add(this.comboBoxSortType);
            this.Controls.Add(this.labelItemIds);
            this.Controls.Add(this.labelSortTypes);
            this.Controls.Add(this.labelName);
            this.Controls.Add(this.txtBoxName);
            this.Controls.Add(this.listBoxItems);
            this.Controls.Add(this.listBoxSortTypes);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditInventoryOptionsForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Edit Sort Options";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Form_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Button btnDeleteSortType;

        private System.Windows.Forms.Button btnAddNewItem;
        private System.Windows.Forms.ComboBox comboBoxSortType;
        private System.Windows.Forms.Button btnDeleteItemId;
        
        private System.Windows.Forms.Button btnAddSortType;

        private System.Windows.Forms.TextBox txtBoxName;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.Label labelSortTypes;
        private System.Windows.Forms.Label labelItemIds;

        private System.Windows.Forms.ListBox listBoxSortTypes;
        private System.Windows.Forms.ListBox listBoxItems;

        #endregion
    }
}