using System.ComponentModel;

namespace LlamaLibrary.AutoRetainerSort
{
    partial class AutoRetainerSortForm
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
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabPageMain = new System.Windows.Forms.TabPage();
            this.btnAutoSetup = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnAddNew = new System.Windows.Forms.Button();
            this.labelInventoryInfo = new System.Windows.Forms.Label();
            this.listBoxInventoryOptions = new System.Windows.Forms.ListBox();
            this.tabPageSettings = new System.Windows.Forms.TabPage();
            this.propertyGridSettings = new System.Windows.Forms.PropertyGrid();
            this.tabControlMain.SuspendLayout();
            this.tabPageMain.SuspendLayout();
            this.tabPageSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlMain
            // 
            this.tabControlMain.Controls.Add(this.tabPageMain);
            this.tabControlMain.Controls.Add(this.tabPageSettings);
            this.tabControlMain.Location = new System.Drawing.Point(0, 0);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(321, 356);
            this.tabControlMain.TabIndex = 0;
            this.tabControlMain.TabStop = false;
            // 
            // tabPageMain
            // 
            this.tabPageMain.Controls.Add(this.btnAutoSetup);
            this.tabPageMain.Controls.Add(this.btnDelete);
            this.tabPageMain.Controls.Add(this.btnAddNew);
            this.tabPageMain.Controls.Add(this.labelInventoryInfo);
            this.tabPageMain.Controls.Add(this.listBoxInventoryOptions);
            this.tabPageMain.Location = new System.Drawing.Point(4, 22);
            this.tabPageMain.Name = "tabPageMain";
            this.tabPageMain.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMain.Size = new System.Drawing.Size(313, 330);
            this.tabPageMain.TabIndex = 0;
            this.tabPageMain.Text = "Main";
            this.tabPageMain.UseVisualStyleBackColor = true;
            // 
            // btnAutoSetup
            // 
            this.btnAutoSetup.Location = new System.Drawing.Point(232, 6);
            this.btnAutoSetup.Name = "btnAutoSetup";
            this.btnAutoSetup.Size = new System.Drawing.Size(73, 22);
            this.btnAutoSetup.TabIndex = 4;
            this.btnAutoSetup.TabStop = false;
            this.btnAutoSetup.Text = "Auto Setup";
            this.btnAutoSetup.UseVisualStyleBackColor = true;
            this.btnAutoSetup.Click += new System.EventHandler(this.AutoSetup_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(75, 300);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(99, 22);
            this.btnDelete.TabIndex = 3;
            this.btnDelete.TabStop = false;
            this.btnDelete.Text = "Delete Selected";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.Delete_Click);
            // 
            // btnAddNew
            // 
            this.btnAddNew.Location = new System.Drawing.Point(8, 300);
            this.btnAddNew.Name = "btnAddNew";
            this.btnAddNew.Size = new System.Drawing.Size(61, 22);
            this.btnAddNew.TabIndex = 2;
            this.btnAddNew.TabStop = false;
            this.btnAddNew.Text = "Add New";
            this.btnAddNew.UseVisualStyleBackColor = true;
            this.btnAddNew.Click += new System.EventHandler(this.AddNew_Click);
            // 
            // labelInventoryInfo
            // 
            this.labelInventoryInfo.AutoSize = true;
            this.labelInventoryInfo.Location = new System.Drawing.Point(8, 27);
            this.labelInventoryInfo.Name = "labelInventoryInfo";
            this.labelInventoryInfo.Size = new System.Drawing.Size(178, 13);
            this.labelInventoryInfo.TabIndex = 1;
            this.labelInventoryInfo.Text = "Per-Inventory Options (Double Click)";
            // 
            // listBoxInventoryOptions
            // 
            this.listBoxInventoryOptions.FormattingEnabled = true;
            this.listBoxInventoryOptions.Location = new System.Drawing.Point(8, 43);
            this.listBoxInventoryOptions.Name = "listBoxInventoryOptions";
            this.listBoxInventoryOptions.Size = new System.Drawing.Size(297, 251);
            this.listBoxInventoryOptions.TabIndex = 0;
            this.listBoxInventoryOptions.TabStop = false;
            this.listBoxInventoryOptions.DoubleClick += new System.EventHandler(this.Listbox_DoubleClick);
            // 
            // tabPageSettings
            // 
            this.tabPageSettings.Controls.Add(this.propertyGridSettings);
            this.tabPageSettings.Location = new System.Drawing.Point(4, 22);
            this.tabPageSettings.Name = "tabPageSettings";
            this.tabPageSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSettings.Size = new System.Drawing.Size(313, 330);
            this.tabPageSettings.TabIndex = 1;
            this.tabPageSettings.Text = "Settings";
            this.tabPageSettings.UseVisualStyleBackColor = true;
            // 
            // propertyGridSettings
            // 
            this.propertyGridSettings.Location = new System.Drawing.Point(6, 6);
            this.propertyGridSettings.Name = "propertyGridSettings";
            this.propertyGridSettings.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.propertyGridSettings.Size = new System.Drawing.Size(304, 321);
            this.propertyGridSettings.TabIndex = 0;
            this.propertyGridSettings.TabStop = false;
            // 
            // AutoRetainerSortForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(321, 356);
            this.Controls.Add(this.tabControlMain);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AutoRetainerSortForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Retainer Organizing";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form_Close);
            this.Load += new System.EventHandler(this.Form_Load);
            this.tabControlMain.ResumeLayout(false);
            this.tabPageMain.ResumeLayout(false);
            this.tabPageMain.PerformLayout();
            this.tabPageSettings.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabPageMain;
        private System.Windows.Forms.TabPage tabPageSettings;
        private System.Windows.Forms.PropertyGrid propertyGridSettings;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnAddNew;
        private System.Windows.Forms.Label labelInventoryInfo;
        private System.Windows.Forms.ListBox listBoxInventoryOptions;
        private System.Windows.Forms.Button btnAutoSetup;
    }
}