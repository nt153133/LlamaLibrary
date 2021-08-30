using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using ff14bot.Enums;
using ff14bot.Managers;
using LlamaLibrary.Extensions;

namespace LlamaLibrary.AutoTrade
{
    public partial class AutoTradeSettings : Form
    {
        public AutoTradeSettings()
        {
            InitializeComponent();
            SetDoubleBuffer(listBoxInventory, true);
            SetDoubleBuffer(dataGridToTrade, true);
            bindingSourceToTrade.DataSource = ItemsToTrade;
        }

        private static void SetDoubleBuffer(Control dataGridView, bool doublebuffered)
        {
            typeof(Control).InvokeMember("DoubleBuffered",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                null,
                dataGridView,
                new object[] {doublebuffered});
        }

        private static void ResizeAndRefreshGrid(DataGridView grid)
        {
            foreach (DataGridViewColumn col in grid.Columns)
            {
                if (col.DataPropertyName == "ItemName") col.ReadOnly = true;
            }
            grid.ResetBindings();
            grid.AutoResizeColumns();
            grid.Refresh();
        }

        private void UpdateUI()
        {
            bindingSourceToTrade.DataSource = ItemsToTrade.OrderBy(x => x.ItemName);
            bindingSourceToTrade.ResetBindings(false);
            bindingSourceInventory.ResetBindings(false);
            listBoxInventory.Refresh();
            ResizeAndRefreshGrid(dataGridToTrade);
        }

        private static readonly InventoryBagId[] MainBags = {
            InventoryBagId.Bag1,
            InventoryBagId.Bag2,
            InventoryBagId.Bag3,
            InventoryBagId.Bag4,
            InventoryBagId.Crystals
        };

        public static ParallelQuery<BagSlot> MainBagsFilledSlots => InventoryManager.GetBagsByInventoryBagId(MainBags).AsParallel().SelectMany(x => x.FilledSlots);

        internal static int CurrentGil => ScriptConditions.Helpers.GilCount();

        private CultureInfo _culture;

        private List<ItemToTrade> _inventoryItems = new List<ItemToTrade>();
        public static readonly List<ItemToTrade> ItemsToTrade = new List<ItemToTrade>();
        private List<ItemToTrade> _crystalList;
        private bool _shouldSort;

        public static bool AcceptTrades { get; private set; } = true;
        public static int GilToTrade { get; private set; }

        private void AutoTradeSettings_Load(object sender, EventArgs e)
        {
            // Get Shards, Crystals, and Clusters.
            _crystalList = InventoryManager
                .GetBagByInventoryBagId(InventoryBagId.Crystals)
                .Where(x => x.IsFilled)
                .Select(crystalSlot => new ItemToTrade(crystalSlot))
                .OrderBy(x => x.TrueItemId)
                .ToList();

            // Assign current culture for number formatting.
            _culture = CultureInfo.CurrentCulture;

            AcceptTrades = cBoxReceive.Checked;

            RefreshLists();
        }

        private void RefreshLists()
        {
            // Remove any missing BagSlots from the inventory listbox if it's been previously generated already. Additionally remove Shards/etc. for before we sort & re-add.
            _inventoryItems.RemoveAll(lbItem => lbItem.BagSlot == null || !lbItem.BagSlot.IsFilled || lbItem.BagSlot.TrueItemId != lbItem.TrueItemId || lbItem.TrueItemId < 20 || lbItem.QtyAvailable == 0);
            // Fill inventory listbox with data.
            foreach (BagSlot slot in MainBagsFilledSlots.Where(i => i.CanTrade() && i.TrueItemId > 19))
            {
                if (_inventoryItems.Any(x => x.TrueItemId == slot.TrueItemId)) continue;
                _inventoryItems.Add(new ItemToTrade(slot));
                _shouldSort = true;
            }
            // Sort by category, then by name to make it pretty.
            if (_shouldSort)
            {
                _inventoryItems = _inventoryItems.OrderBy(x => x.BagSlot.Item.EquipmentCatagory).ThenBy(x => x.ItemName).ToList();
                _shouldSort = false;
            }
            // Add Shards/etc. to the end.
            _inventoryItems.AddRange(_crystalList);
            // Refresh Trade Qty values.
            ItemsToTrade.RemoveAll(x => x.QtyAvailable == 0);
            foreach (ItemToTrade item in ItemsToTrade.Where(item => item.QtyToTrade > item.QtyAvailable))
            {
                item.QtyToTrade = item.QtyAvailable;
            }

            // Assign data sources.
            bindingSourceInventory.DataSource = _inventoryItems;
            bindingSourceToTrade.DataSource = ItemsToTrade;
            listBoxInventory.DataSource = bindingSourceInventory;
            dataGridToTrade.DataSource = bindingSourceToTrade;

            UpdateUI();
        }

        // Warn us if we're trying to trade more gil than we have.
        private void txtBoxGil_Leave(object sender, EventArgs e)
        {
            if (int.Parse(txtBoxGil.Text, NumberStyles.AllowThousands, _culture) <= CurrentGil)
            {
                return;
            }
            MessageBox.Show("You're trying to trade more gil than you currently have!");
            txtBoxGil.Text = string.Format(_culture, "{0:N0}", CurrentGil);
            txtBoxGil.Focus();
            txtBoxGil.SelectAll();
        }

        private void btnToTrade_Click(object sender, EventArgs e)
        {
            foreach (ItemToTrade lbItem in listBoxInventory.SelectedItems.Cast<ItemToTrade>())
            {
                if (ItemsToTrade.Contains(lbItem)) continue;
                ItemsToTrade.Add(lbItem);
            }
            UpdateUI();
        }

        private void dataGridToTrade_DataError(object sender, DataGridViewDataErrorEventArgs anError)
        {
            DataGridView view = (DataGridView) sender;
            view.Rows[anError.RowIndex].Cells[anError.ColumnIndex].Value = 0;
        }

        private void dataGridToTrade_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView view = (DataGridView) sender;
            if (e.ColumnIndex < 0 || e.RowIndex < 0) return;
            if (view.Columns[e.ColumnIndex].DataPropertyName != "QtyToTrade") return;
            ItemToTrade item = (ItemToTrade) view.Rows[e.RowIndex].DataBoundItem;
            if (item.QtyToTrade > item.QtyAvailable)
            {
                item.QtyToTrade = item.QtyAvailable;
            }
            UpdateUI();
        }

        private void cBoxReceive_CheckedChanged(object sender, EventArgs e)
        {
            AcceptTrades = cBoxReceive.Checked;
        }

        private void txtBoxGil_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtBoxGil.Text)) return;
            int selectionFromRight = txtBoxGil.Text.Length - txtBoxGil.SelectionStart + txtBoxGil.SelectionLength;
            string gilText = txtBoxGil.Text;
            if (gilText.Length <= 3) return;
            bool parsed = long.TryParse(gilText, NumberStyles.AllowThousands, _culture, out long valueBefore);
            if (!parsed)
            {
                gilText = new string(gilText.Where(char.IsDigit).ToArray());
                if (string.IsNullOrEmpty(gilText))
                {
                    txtBoxGil.Text = string.Empty;
                    return;
                }

                valueBefore = long.Parse(gilText, NumberStyles.AllowThousands, _culture);
            }

            valueBefore = Math.Min(valueBefore, 999999999);
            txtBoxGil.Text = string.Format(_culture, "{0:N0}", valueBefore);
            int newSelection = Math.Max(txtBoxGil.TextLength - selectionFromRight, 0);
            txtBoxGil.Select(newSelection, 0);

            int.TryParse(gilText, NumberStyles.AllowThousands, _culture, out int value);
            if (value > 0) GilToTrade = value;
        }

        private void dataGridToTrade_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Delete) return;
            DataGridView g = (DataGridView) sender;
            if (g.SelectedCells.Count <= 0) return;
            List<DataGridViewRow> rows = (from DataGridViewCell cell in g.SelectedCells let rowIndex = cell.RowIndex select g.Rows[cell.RowIndex]).Distinct().ToList();
            if (!rows.Any()) return;
            foreach (ItemToTrade rowData in rows.Select(row => (ItemToTrade) row.DataBoundItem))
            {
                ItemsToTrade.Remove(rowData);
            }
            UpdateUI();
        }

        private void btnRefreshList_Click(object sender, EventArgs e)
        {
            RefreshLists();
        }

        private void AutoTradeSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            AcceptTrades = cBoxReceive.Checked;
        }

        private void txtBoxFilter_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtBoxFilter.Text))
                bindingSourceInventory.DataSource = _inventoryItems;
            else
                bindingSourceInventory.DataSource = _inventoryItems
                    .Where(x => x.ItemName.IndexOf(txtBoxFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();

            UpdateUI();
        }
    }

    public class ItemToTrade
    {
        [Browsable(false)]
        internal readonly BagSlot BagSlot;

        [Browsable(false)]
        public readonly uint TrueItemId;

        public string ItemName { get; }

        public int QtyToTrade { get; set; }

        [Browsable(false)]
        public readonly int StackSize;

        [Browsable(false)]
        public int QtyAvailable => AutoTradeSettings.MainBagsFilledSlots.Where(x => x.TrueItemId == TrueItemId).Select(x => (int) x.Count).Sum();

        internal ItemToTrade(BagSlot bagSlot)
        {
            BagSlot = bagSlot;
            TrueItemId = bagSlot.TrueItemId;
            StackSize = (int) bagSlot.Item.StackSize;
            if (TrueItemId > 1000000) ItemName = bagSlot.Item.CurrentLocaleName + " [HQ]";
            else ItemName = bagSlot.Item.CurrentLocaleName;
            QtyToTrade = QtyAvailable;
        }

        public override string ToString()
        {
            return ItemName;
        }

        public override int GetHashCode()
        {
            return TrueItemId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj.GetType() == typeof(BagSlot))
                return TrueItemId == (obj as BagSlot)?.TrueItemId;

            if (obj != null && obj.GetType() == typeof(ItemToTrade))
                return TrueItemId == (obj as ItemToTrade)?.TrueItemId;

            return base.Equals(obj);
        }
    }
}