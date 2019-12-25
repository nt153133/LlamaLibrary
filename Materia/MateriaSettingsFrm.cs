using System;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Media;
using ff14bot;
using ff14bot.Managers;
using LlamaLibrary.Helpers;

namespace LlamaLibrary.Materia
{
    public partial class MateriaSettingsFrm : Form
    {
        private BagSlot _selectedBagSlot;
        public MateriaSettingsFrm()
        {
            InitializeComponent();
        }

        private void MateriaSettingsFrm_Load(object sender, EventArgs e)
        {
            this.itemCb.SelectionChangeCommitted += new System.EventHandler(itemCb_SelectionChangeCommitted);
            bindingSourceInventory.Clear();
            foreach (var bagSlot in InventoryManager.EquippedItems.Where(MateriaBase.HasMateria))
            {
                bindingSourceInventory.Add(bagSlot);
            }

            itemCb.DisplayMember = "Name";
            itemCb.DataSource = bindingSourceInventory;
            
            materiaListBox.DataSource = MateriaBase.Materia((BagSlot) itemCb.SelectedItem);
            materiaListBox.DisplayMember = "ItemName";
        }

        private void itemCb_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _selectedBagSlot = (BagSlot) itemCb.SelectedItem;
            materiaListBox.DataSource = MateriaBase.Materia(_selectedBagSlot);
            materiaListBox.DisplayMember = "ItemName";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MateriaBase.ItemToRemoveMateria = _selectedBagSlot;
            Logger.External("Materia Settings", "Click",Colors.Blue);
            if (BotManager.Current.Name == "Materia")
            {
                BotManager.Current.Start();
                TreeRoot.Start();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bindingSourceInventory.Clear();
            foreach (var bagSlot in InventoryManager.EquippedItems.Where(MateriaBase.HasMateria))
            {
                bindingSourceInventory.Add(bagSlot);
            }

            itemCb.DisplayMember = "Name";
            itemCb.DataSource = bindingSourceInventory;
            
            materiaListBox.DataSource = MateriaBase.Materia(_selectedBagSlot);
            materiaListBox.DisplayMember = "ItemName";
        }
    }
}