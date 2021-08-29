using System;
using System.Windows.Forms;

namespace LlamaLibrary.AutoRetainerSort
{
    public partial class EditInventoryOptionsForm : Form
    {
        public EditInventoryOptionsForm()
        {
            InitializeComponent();
        }

        public EditInventoryOptionsForm(InventorySortInfo sortInfo, int index)
        {
            InitializeComponent();
            _sortInfo = sortInfo;
            _index = index;
        }

        private readonly InventorySortInfo _sortInfo;
        private readonly int _index = int.MinValue;

        private void Form_Load(object sender, EventArgs e)
        {
            if (_sortInfo == null || _index == int.MinValue)
            {
                MessageBox.Show("Couldn't load the sorting info correctly?", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }

            txtBoxName.Text = _sortInfo.Name;
            
            _bsSortTypes = new BindingSource(_sortInfo.SortTypes, "");
            _bsItemIds = new BindingSource(_sortInfo.TrueItemIds, "");

            listBoxSortTypes.DataSource = _bsSortTypes;
            listBoxItemIds.DataSource = _bsItemIds;

            comboBoxSortType.DataSource = Enum.GetValues(typeof(SortType));
        }

        private BindingSource _bsSortTypes;

        private BindingSource _bsItemIds;

        private SortType NewSortType => Enum.TryParse(comboBoxSortType.SelectedValue.ToString(), out SortType sortType) ? sortType : SortType.UNKNOWN;

        private uint NewItemId => (uint)numUpDownItemId.Value;

        private void Name_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtBoxName.Text)) return;
            _sortInfo.Name = txtBoxName.Text;
        }

        private void AddNewSortType_Click(object sender, EventArgs e)
        {
            SortType selected = NewSortType;
            if (_sortInfo.ContainsType(selected)) return;

            foreach (var pair in AutoRetainerSortSettings.Instance.InventoryOptions)
            {
                if (pair.Key == _index) continue;

                if (!pair.Value.ContainsType(selected)) continue;
                
                
                DialogResult dr = MessageBox.Show(
                    $"{pair.Value.Name} already contains {selected.ToString()}!\r\nDo you want to remove it from {pair.Value.Name} and add it to {_sortInfo.Name}?",
                    "Warning",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (dr != DialogResult.Yes) return;

                pair.Value.SortTypes.Remove(selected);
                break;
            }

            _bsSortTypes.Add(selected);
            AutoRetainerSortSettings.Instance.Save();
        }

        private void AddNewItemId_Click(object sender, EventArgs e)
        {
            uint itemId = NewItemId;
            if (_sortInfo.ContainsId(itemId)) return;

            foreach (var pair in AutoRetainerSortSettings.Instance.InventoryOptions)
            {
                if (pair.Key == _index) continue;

                if (!pair.Value.ContainsId(itemId)) continue;
                
                DialogResult dr = MessageBox.Show(
                    $"{pair.Value.Name} already contains {itemId.ToString()}!\r\nDo you want to remove it from {pair.Value.Name} and add it to {_sortInfo.Name}?",
                    "Warning",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (dr != DialogResult.Yes) return;
                
                pair.Value.TrueItemIds.Remove(itemId);
                break;
            }

            _bsItemIds.Add(itemId);
            AutoRetainerSortSettings.Instance.Save();
        }

        private void DeleteSelectedSortType_Click(object sender, EventArgs e)
        {
            _bsSortTypes.Remove(listBoxSortTypes.SelectedItem);
        }

        private void DeleteSelectedItemId_Click(object sender, EventArgs e)
        {
            _bsItemIds.Remove(listBoxItemIds.SelectedItem);
        }
    }
}