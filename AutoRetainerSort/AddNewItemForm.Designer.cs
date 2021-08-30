
namespace LlamaLibrary.AutoRetainerSort
{
    partial class AddNewItemForm
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
            this.labelSearch = new System.Windows.Forms.Label();
            this.textBoxSearch = new System.Windows.Forms.TextBox();
            this.listBoxSearchResults = new System.Windows.Forms.ListBox();
            this.txtBoxItem = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBoxModifiers = new System.Windows.Forms.GroupBox();
            this.rdBtnCollectable = new System.Windows.Forms.RadioButton();
            this.rdBtnHQ = new System.Windows.Forms.RadioButton();
            this.rdBtnNone = new System.Windows.Forms.RadioButton();
            this.groupBoxAlsoInclude = new System.Windows.Forms.GroupBox();
            this.checkBoxCollectable = new System.Windows.Forms.CheckBox();
            this.checkBoxHQ = new System.Windows.Forms.CheckBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBoxModifiers.SuspendLayout();
            this.groupBoxAlsoInclude.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelSearch
            // 
            this.labelSearch.AutoSize = true;
            this.labelSearch.Location = new System.Drawing.Point(12, 11);
            this.labelSearch.Name = "labelSearch";
            this.labelSearch.Size = new System.Drawing.Size(44, 13);
            this.labelSearch.TabIndex = 0;
            this.labelSearch.Text = "Search:";
            // 
            // textBoxSearch
            // 
            this.textBoxSearch.Location = new System.Drawing.Point(62, 8);
            this.textBoxSearch.Name = "textBoxSearch";
            this.textBoxSearch.Size = new System.Drawing.Size(364, 20);
            this.textBoxSearch.TabIndex = 1;
            this.textBoxSearch.TextChanged += new System.EventHandler(this.SearchBox_TextChanged);
            // 
            // listBoxSearchResults
            // 
            this.listBoxSearchResults.FormattingEnabled = true;
            this.listBoxSearchResults.Location = new System.Drawing.Point(12, 34);
            this.listBoxSearchResults.Name = "listBoxSearchResults";
            this.listBoxSearchResults.Size = new System.Drawing.Size(413, 134);
            this.listBoxSearchResults.TabIndex = 2;
            this.listBoxSearchResults.TabStop = false;
            this.listBoxSearchResults.SelectedIndexChanged += new System.EventHandler(this.SearchResults_SelectionChanged);
            // 
            // txtBoxItem
            // 
            this.txtBoxItem.Location = new System.Drawing.Point(48, 198);
            this.txtBoxItem.Name = "txtBoxItem";
            this.txtBoxItem.ReadOnly = true;
            this.txtBoxItem.Size = new System.Drawing.Size(378, 20);
            this.txtBoxItem.TabIndex = 4;
            this.txtBoxItem.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 201);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Item:";
            // 
            // groupBoxModifiers
            // 
            this.groupBoxModifiers.Controls.Add(this.rdBtnCollectable);
            this.groupBoxModifiers.Controls.Add(this.rdBtnHQ);
            this.groupBoxModifiers.Controls.Add(this.rdBtnNone);
            this.groupBoxModifiers.Location = new System.Drawing.Point(12, 227);
            this.groupBoxModifiers.Name = "groupBoxModifiers";
            this.groupBoxModifiers.Size = new System.Drawing.Size(412, 49);
            this.groupBoxModifiers.TabIndex = 5;
            this.groupBoxModifiers.TabStop = false;
            this.groupBoxModifiers.Text = "Modifiers";
            // 
            // rdBtnCollectable
            // 
            this.rdBtnCollectable.AutoSize = true;
            this.rdBtnCollectable.Location = new System.Drawing.Point(110, 19);
            this.rdBtnCollectable.Name = "rdBtnCollectable";
            this.rdBtnCollectable.Size = new System.Drawing.Size(77, 17);
            this.rdBtnCollectable.TabIndex = 2;
            this.rdBtnCollectable.TabStop = true;
            this.rdBtnCollectable.Text = "Collectable";
            this.rdBtnCollectable.UseVisualStyleBackColor = true;
            this.rdBtnCollectable.CheckedChanged += new System.EventHandler(this.ModifierRadioButton_Changed);
            // 
            // rdBtnHQ
            // 
            this.rdBtnHQ.AutoSize = true;
            this.rdBtnHQ.Location = new System.Drawing.Point(63, 19);
            this.rdBtnHQ.Name = "rdBtnHQ";
            this.rdBtnHQ.Size = new System.Drawing.Size(41, 17);
            this.rdBtnHQ.TabIndex = 1;
            this.rdBtnHQ.TabStop = true;
            this.rdBtnHQ.Text = "HQ";
            this.rdBtnHQ.UseVisualStyleBackColor = true;
            this.rdBtnHQ.CheckedChanged += new System.EventHandler(this.ModifierRadioButton_Changed);
            // 
            // rdBtnNone
            // 
            this.rdBtnNone.AutoSize = true;
            this.rdBtnNone.Checked = true;
            this.rdBtnNone.Location = new System.Drawing.Point(6, 19);
            this.rdBtnNone.Name = "rdBtnNone";
            this.rdBtnNone.Size = new System.Drawing.Size(51, 17);
            this.rdBtnNone.TabIndex = 0;
            this.rdBtnNone.TabStop = true;
            this.rdBtnNone.Text = "None";
            this.rdBtnNone.UseVisualStyleBackColor = true;
            this.rdBtnNone.CheckedChanged += new System.EventHandler(this.ModifierRadioButton_Changed);
            // 
            // groupBoxAlsoInclude
            // 
            this.groupBoxAlsoInclude.Controls.Add(this.checkBoxCollectable);
            this.groupBoxAlsoInclude.Controls.Add(this.checkBoxHQ);
            this.groupBoxAlsoInclude.Location = new System.Drawing.Point(13, 282);
            this.groupBoxAlsoInclude.Name = "groupBoxAlsoInclude";
            this.groupBoxAlsoInclude.Size = new System.Drawing.Size(412, 49);
            this.groupBoxAlsoInclude.TabIndex = 6;
            this.groupBoxAlsoInclude.TabStop = false;
            this.groupBoxAlsoInclude.Text = "Also Include";
            // 
            // checkBoxCollectable
            // 
            this.checkBoxCollectable.AutoSize = true;
            this.checkBoxCollectable.Location = new System.Drawing.Point(62, 19);
            this.checkBoxCollectable.Name = "checkBoxCollectable";
            this.checkBoxCollectable.Size = new System.Drawing.Size(78, 17);
            this.checkBoxCollectable.TabIndex = 2;
            this.checkBoxCollectable.Text = "Collectable";
            this.checkBoxCollectable.UseVisualStyleBackColor = true;
            // 
            // checkBoxHQ
            // 
            this.checkBoxHQ.AutoSize = true;
            this.checkBoxHQ.Location = new System.Drawing.Point(6, 19);
            this.checkBoxHQ.Name = "checkBoxHQ";
            this.checkBoxHQ.Size = new System.Drawing.Size(42, 17);
            this.checkBoxHQ.TabIndex = 1;
            this.checkBoxHQ.Text = "HQ";
            this.checkBoxHQ.UseVisualStyleBackColor = true;
            // 
            // btnAdd
            // 
            this.btnAdd.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnAdd.Location = new System.Drawing.Point(12, 352);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(77, 23);
            this.btnAdd.TabIndex = 0;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(348, 352);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(77, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // AddNewItemForm
            // 
            this.AcceptButton = this.btnAdd;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(437, 387);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.groupBoxAlsoInclude);
            this.Controls.Add(this.groupBoxModifiers);
            this.Controls.Add(this.txtBoxItem);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listBoxSearchResults);
            this.Controls.Add(this.textBoxSearch);
            this.Controls.Add(this.labelSearch);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddNewItemForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Add New Item";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Form_Load);
            this.groupBoxModifiers.ResumeLayout(false);
            this.groupBoxModifiers.PerformLayout();
            this.groupBoxAlsoInclude.ResumeLayout(false);
            this.groupBoxAlsoInclude.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelSearch;
        private System.Windows.Forms.TextBox textBoxSearch;
        private System.Windows.Forms.ListBox listBoxSearchResults;
        private System.Windows.Forms.TextBox txtBoxItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBoxModifiers;
        private System.Windows.Forms.RadioButton rdBtnCollectable;
        private System.Windows.Forms.RadioButton rdBtnHQ;
        private System.Windows.Forms.RadioButton rdBtnNone;
        private System.Windows.Forms.GroupBox groupBoxAlsoInclude;
        private System.Windows.Forms.CheckBox checkBoxCollectable;
        private System.Windows.Forms.CheckBox checkBoxHQ;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnCancel;
    }
}