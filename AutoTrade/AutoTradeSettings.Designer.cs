using System.ComponentModel;

namespace LlamaLibrary.AutoTrade
{
    partial class AutoTradeSettings
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
            this.listBoxInventory = new System.Windows.Forms.ListBox();
            this.bindingSourceInventory = new System.Windows.Forms.BindingSource(this.components);
            this.dataGridToTrade = new System.Windows.Forms.DataGridView();
            this.ItmName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TradeQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bindingSourceToTrade = new System.Windows.Forms.BindingSource(this.components);
            this.btnToTrade = new System.Windows.Forms.Button();
            this.cBoxReceive = new System.Windows.Forms.CheckBox();
            this.txtBoxGil = new System.Windows.Forms.TextBox();
            this.lblGil = new System.Windows.Forms.Label();
            this.btnRefreshList = new System.Windows.Forms.Button();
            this.txtBoxFilter = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize) (this.bindingSourceInventory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize) (this.dataGridToTrade)).BeginInit();
            ((System.ComponentModel.ISupportInitialize) (this.bindingSourceToTrade)).BeginInit();
            this.SuspendLayout();
            // 
            // listBoxInventory
            // 
            this.listBoxInventory.DataSource = this.bindingSourceInventory;
            this.listBoxInventory.FormattingEnabled = true;
            this.listBoxInventory.Location = new System.Drawing.Point(12, 51);
            this.listBoxInventory.Name = "listBoxInventory";
            this.listBoxInventory.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBoxInventory.Size = new System.Drawing.Size(433, 615);
            this.listBoxInventory.TabIndex = 0;
            // 
            // dataGridToTrade
            // 
            this.dataGridToTrade.AllowUserToAddRows = false;
            this.dataGridToTrade.AllowUserToDeleteRows = false;
            this.dataGridToTrade.AllowUserToResizeRows = false;
            this.dataGridToTrade.AutoGenerateColumns = false;
            this.dataGridToTrade.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridToTrade.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {this.ItmName, this.TradeQty});
            this.dataGridToTrade.DataSource = this.bindingSourceToTrade;
            this.dataGridToTrade.Location = new System.Drawing.Point(452, 51);
            this.dataGridToTrade.Name = "dataGridToTrade";
            this.dataGridToTrade.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridToTrade.Size = new System.Drawing.Size(433, 615);
            this.dataGridToTrade.TabIndex = 1;
            this.dataGridToTrade.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridToTrade_CellValueChanged);
            this.dataGridToTrade.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridToTrade_DataError);
            this.dataGridToTrade.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dataGridToTrade_KeyUp);
            // 
            // ItmName
            // 
            this.ItmName.DataPropertyName = "ItemName";
            this.ItmName.HeaderText = "Item Name";
            this.ItmName.Name = "ItmName";
            this.ItmName.ReadOnly = true;
            // 
            // TradeQty
            // 
            this.TradeQty.DataPropertyName = "QtyToTrade";
            this.TradeQty.HeaderText = "Trade Qty";
            this.TradeQty.Name = "TradeQty";
            // 
            // btnToTrade
            // 
            this.btnToTrade.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.btnToTrade.Location = new System.Drawing.Point(12, 12);
            this.btnToTrade.Name = "btnToTrade";
            this.btnToTrade.Size = new System.Drawing.Size(151, 33);
            this.btnToTrade.TabIndex = 2;
            this.btnToTrade.Text = "Add To Trade";
            this.btnToTrade.UseVisualStyleBackColor = true;
            this.btnToTrade.Click += new System.EventHandler(this.btnToTrade_Click);
            // 
            // cBoxReceive
            // 
            this.cBoxReceive.Checked = true;
            this.cBoxReceive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBoxReceive.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.cBoxReceive.Location = new System.Drawing.Point(760, 12);
            this.cBoxReceive.Name = "cBoxReceive";
            this.cBoxReceive.Size = new System.Drawing.Size(125, 33);
            this.cBoxReceive.TabIndex = 3;
            this.cBoxReceive.Text = "Accept Trades";
            this.cBoxReceive.UseVisualStyleBackColor = true;
            this.cBoxReceive.CheckedChanged += new System.EventHandler(this.cBoxReceive_CheckedChanged);
            // 
            // txtBoxGil
            // 
            this.txtBoxGil.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.txtBoxGil.Location = new System.Drawing.Point(492, 16);
            this.txtBoxGil.Name = "txtBoxGil";
            this.txtBoxGil.Size = new System.Drawing.Size(90, 22);
            this.txtBoxGil.TabIndex = 4;
            this.txtBoxGil.TextChanged += new System.EventHandler(this.txtBoxGil_TextChanged);
            this.txtBoxGil.Leave += new System.EventHandler(this.txtBoxGil_Leave);
            // 
            // lblGil
            // 
            this.lblGil.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.lblGil.Location = new System.Drawing.Point(400, 19);
            this.lblGil.Name = "lblGil";
            this.lblGil.Size = new System.Drawing.Size(86, 20);
            this.lblGil.TabIndex = 5;
            this.lblGil.Text = "Gil To Trade:";
            // 
            // btnRefreshList
            // 
            this.btnRefreshList.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.btnRefreshList.Location = new System.Drawing.Point(588, 11);
            this.btnRefreshList.Name = "btnRefreshList";
            this.btnRefreshList.Size = new System.Drawing.Size(151, 33);
            this.btnRefreshList.TabIndex = 6;
            this.btnRefreshList.Text = "Refresh Lists";
            this.btnRefreshList.UseVisualStyleBackColor = true;
            this.btnRefreshList.Click += new System.EventHandler(this.btnRefreshList_Click);
            // 
            // txtBoxFilter
            // 
            this.txtBoxFilter.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.txtBoxFilter.Location = new System.Drawing.Point(210, 16);
            this.txtBoxFilter.Name = "txtBoxFilter";
            this.txtBoxFilter.Size = new System.Drawing.Size(162, 22);
            this.txtBoxFilter.TabIndex = 7;
            this.txtBoxFilter.TextChanged += new System.EventHandler(this.txtBoxFilter_TextChanged);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.label1.Location = new System.Drawing.Point(169, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 20);
            this.label1.TabIndex = 8;
            this.label1.Text = "Filter:";
            // 
            // AutoTradeSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(897, 678);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtBoxFilter);
            this.Controls.Add(this.btnRefreshList);
            this.Controls.Add(this.lblGil);
            this.Controls.Add(this.txtBoxGil);
            this.Controls.Add(this.cBoxReceive);
            this.Controls.Add(this.btnToTrade);
            this.Controls.Add(this.dataGridToTrade);
            this.Controls.Add(this.listBoxInventory);
            this.Name = "AutoTradeSettings";
            this.Text = "AutoTradeSettings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AutoTradeSettings_FormClosing);
            this.Load += new System.EventHandler(this.AutoTradeSettings_Load);
            ((System.ComponentModel.ISupportInitialize) (this.bindingSourceInventory)).EndInit();
            ((System.ComponentModel.ISupportInitialize) (this.dataGridToTrade)).EndInit();
            ((System.ComponentModel.ISupportInitialize) (this.bindingSourceToTrade)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.TextBox txtBoxFilter;

        private System.Windows.Forms.Label label1;

        private System.Windows.Forms.Button btnRefreshList;

        private System.Windows.Forms.DataGridViewTextBoxColumn ItmName;
        private System.Windows.Forms.DataGridViewTextBoxColumn TradeQty;

        private System.Windows.Forms.BindingSource bindingSourceInventory;
        private System.Windows.Forms.BindingSource bindingSourceToTrade;

        private System.Windows.Forms.TextBox txtBoxGil;

        private System.Windows.Forms.Label lblGil;

        private System.Windows.Forms.Button btnToTrade;
        private System.Windows.Forms.CheckBox cBoxReceive;

        private System.Windows.Forms.DataGridView dataGridToTrade;

        private System.Windows.Forms.ListBox listBoxInventory;

        #endregion
    }
}