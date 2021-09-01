using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using LlamaLibrary.AutoRetainerSort.Classes;
using LlamaLibrary.RemoteWindows;
using LlamaLibrary.Structs;

namespace LlamaLibrary.AutoRetainerSort
{
    public partial class AutoRetainerSortForm : Form
    {
        public AutoRetainerSortForm()
        {
            InitializeComponent();
        }

        private BindingSource _bsInventories;

        private void Form_Load(object sender, EventArgs e)
        {
            if (AutoRetainerSortSettings.Instance.WindowPosition != Point.Empty)
            {
                Location = AutoRetainerSortSettings.Instance.WindowPosition;
            }

            _bsInventories = new BindingSource(AutoRetainerSortSettings.Instance, "InventoryOptions");
            listBoxInventoryOptions.DataSource = _bsInventories;
            listBoxInventoryOptions.DisplayMember = "Value";

            propertyGridSettings.SelectedObject = AutoRetainerSortSettings.Instance;
        }

        private void Form_Close(object sender, FormClosedEventArgs e)
        {
            AutoRetainerSortSettings.Instance.WindowPosition = Location;
            AutoRetainerSortSettings.Instance.Save();
        }

        private void Listbox_DoubleClick(object sender, EventArgs e)
        {
            var selectedItem = (KeyValuePair<int, InventorySortInfo>)listBoxInventoryOptions.SelectedItem;
            EditInventoryOptionsForm editForm = new EditInventoryOptionsForm(selectedItem.Value, selectedItem.Key);
            editForm.Show(this);
        }

        private void AddNew_Click(object sender, EventArgs e)
        {
            using (AddNewInventoryForm addNewForm = new AddNewInventoryForm())
            {
                DialogResult dr = addNewForm.ShowDialog(this);
                if (dr == DialogResult.Cancel) return;

                AutoRetainerSortSettings.Instance.InventoryOptions.Add(addNewForm.Index, new InventorySortInfo(addNewForm.RetainerName));
                ResetBindingSource();
            }
            AutoRetainerSortSettings.Instance.Save();
        }

        private void ResetBindingSource()
        {
            _bsInventories.DataSource = AutoRetainerSortSettings.Instance;
            _bsInventories.ResetBindings(true);
            listBoxInventoryOptions.ResetBindings();
            listBoxInventoryOptions.Refresh();
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            var selectedItem = (KeyValuePair<int, InventorySortInfo>)listBoxInventoryOptions.SelectedItem;
            if (selectedItem.Key >= ItemSortStatus.SaddlebagInventoryIndex)
            {
                DialogResult dr = MessageBox.Show(
                    $"Are you sure you want to delete {selectedItem.Value.Name}?",
                    "Really Delete?",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Exclamation);
                if (dr != DialogResult.Yes) return;
            }
            else
            {
                DialogResult dr = MessageBox.Show(
                    $"Are you REALLY sure you want to delete the Player Inventory from being sorted?{Environment.NewLine}This is probably going to break things... don't blame me.",
                    Strings.WarningCaption,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Hand);
                if (dr != DialogResult.Yes) return;
            }

            _bsInventories.Remove(listBoxInventoryOptions.SelectedItem);
            AutoRetainerSortSettings.Instance.Save();
            ResetBindingSource();
        }

        private void AutoSetup_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                Strings.AutoSetup_CacheAdvice,
                "Careful!",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            DialogResult warningResult = MessageBox.Show(
                Strings.AutoSetup_OverwriteWarning,
                "Warning!",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            if (warningResult == DialogResult.No) return;

            bool conflictUnsorted = MessageBox.Show(
                Strings.AutoSetup_ConflictQuestion,
                "Conflict?",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes;

            var newInventorySetup = AutoRetainerSortSettings.Instance.InventoryOptions;

            foreach (InventorySortInfo inventorySortInfo in newInventorySetup.Values)
            {
                inventorySortInfo.SortTypes.Clear();
            }

            var orderedRetainerList = RetainerList.Instance.OrderedRetainerList;

            for (var i = 0; i < orderedRetainerList.Length; i++)
            {
                if (newInventorySetup.ContainsKey(i)) continue;
                RetainerInfo retInfo = orderedRetainerList[i];
                if (!retInfo.Active) continue;

                newInventorySetup.Add(i, new InventorySortInfo(retInfo.Name));
            }

            AutoRetainerSortSettings.Instance.InventoryOptions = newInventorySetup;

            ItemSortStatus.UpdateFromCache(orderedRetainerList);

            var sortTypeCounts = new Dictionary<SortType, Dictionary<int, int>>();

            foreach (SortType sortType in Enum.GetValues(typeof(SortType)).Cast<SortType>())
            {
                sortTypeCounts[sortType] = new Dictionary<int, int>();
            }

            foreach (CachedInventory cachedInventory in ItemSortStatus.GetAllInventories())
            {
                foreach (SortType sortType in cachedInventory.ItemCounts.Select(x => ItemSortStatus.GetSortInfo(x.Key).SortType))
                {
                    var indexCountDic = sortTypeCounts[sortType];
                    if (indexCountDic.ContainsKey(cachedInventory.Index))
                    {
                        indexCountDic[cachedInventory.Index]++;
                    }
                    else
                    {
                        indexCountDic.Add(cachedInventory.Index, 1);
                    }
                }
            }

            foreach (var typeDicPair in sortTypeCounts)
            {
                SortType sortType = typeDicPair.Key;
                int indexCount = typeDicPair.Value.Keys.Count;
                if (indexCount == 0) continue;
                if (indexCount > 1 && conflictUnsorted) continue;

                int desiredIndex = typeDicPair.Value.OrderByDescending(x => x.Value).First().Key;
                AutoRetainerSortSettings.Instance.InventoryOptions[desiredIndex].SortTypes.Add(new SortTypeWithCount(sortType));
            }

            ResetBindingSource();
            AutoRetainerSortSettings.Instance.Save();
            AutoRetainerSort.LogSuccess("Auto-Setup done!");
        }
    }
}