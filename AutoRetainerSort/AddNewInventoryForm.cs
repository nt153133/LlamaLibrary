using System;
using System.Linq;
using System.Windows.Forms;

namespace LlamaLibrary.AutoRetainerSort
{
    public partial class AddNewInventoryForm : Form
    {
        public AddNewInventoryForm()
        {
            InitializeComponent();
        }

        private void Add_Click(object sender, EventArgs e)
        {
            if (AutoRetainerSortSettings.Instance.InventoryOptions.Any(x => x.Key == Index))
            {
                DialogResult = DialogResult.None;
                MessageBox.Show("That index is already taken!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (Index < 0)
            {
                DialogResult = DialogResult.None;
                MessageBox.Show("Can't have a retainer index below 0!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (string.IsNullOrEmpty(RetainerName))
            {
                DialogResult = DialogResult.None;
                MessageBox.Show("Need a name!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Index_Changed(object sender, EventArgs e)
        {
            btnAdd.Enabled = AutoRetainerSortSettings.Instance.InventoryOptions.All(x => x.Key != Index) && Index >= 0;
        }

        public int Index => (int)numUpDownIndex.Value;

        public string RetainerName => txtBoxName.Text;
    }
}