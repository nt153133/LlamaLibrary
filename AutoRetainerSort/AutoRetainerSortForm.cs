using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

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
            editForm.Show();
        }

        private void AddNew_Click(object sender, EventArgs e)
        {
            using (AddNewInventoryForm addNewForm = new AddNewInventoryForm())
            {
                DialogResult dr = addNewForm.ShowDialog();
                if (dr == DialogResult.Cancel) return;

                AutoRetainerSortSettings.Instance.InventoryOptions.Add(addNewForm.Index, new InventorySortInfo(addNewForm.RetainerName));
                _bsInventories.ResetBindings(true);
                listBoxInventoryOptions.ResetBindings();
                listBoxInventoryOptions.Refresh();
            }
            AutoRetainerSortSettings.Instance.Save();
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            var selectedItem = (KeyValuePair<int, InventorySortInfo>)listBoxInventoryOptions.SelectedItem;
            if (selectedItem.Key >= 0)
            {
                DialogResult dr = MessageBox.Show(
                    $"Are you sure you want to delete {selectedItem.Value.Name}?",
                    "Really Delete?",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Exclamation);
                if (dr != DialogResult.Yes) return;
                _bsInventories.Remove(listBoxInventoryOptions.SelectedItem);
                AutoRetainerSortSettings.Instance.Save();
                listBoxInventoryOptions.Refresh();
            }
            else
            {
                MessageBox.Show("Can't delete Player Inventory or Chocobo Saddlebag!", "Can't Do That", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}