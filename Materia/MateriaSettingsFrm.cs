using System;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Media;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using LlamaLibrary.Extensions;
using LlamaLibrary.Helpers;

namespace LlamaLibrary.Materia
{
    public partial class MateriaSettingsFrm : Form
    {
        private BagSlot _selectedBagSlot;
        private BagSlot _selectedBagSlotAffix;
        public MateriaSettingsFrm()
        {
            InitializeComponent();
        }

        private void MateriaSettingsFrm_Load(object sender, EventArgs e)
        {
            this.itemCb.SelectionChangeCommitted += new System.EventHandler(itemCb_SelectionChangeCommitted);
            this.affixCb.SelectionChangeCommitted += new System.EventHandler(affixCb_SelectionChangeCommitted);
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
        
        private void affixCb_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _selectedBagSlotAffix = (BagSlot) affixCb.SelectedItem;
            affixLb.DataSource = MateriaBase.Materia(_selectedBagSlotAffix);
            affixLb.DisplayMember = "ItemName";
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
            foreach (var bagSlot in InventoryManager.EquippedItems.Where(i=> i.HasMateria()))
            {
                bindingSourceInventory.Add(bagSlot);
            }

            itemCb.DisplayMember = "Name";
            itemCb.DataSource = bindingSourceInventory;
            
            materiaListBox.DataSource = MateriaBase.Materia(_selectedBagSlot);
            materiaListBox.DisplayMember = "ItemName";
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            bindingSourceAffix.Clear();
            foreach (var bagSlot in InventoryManager.EquippedItems.Where(i=> i.IsFilled && i.Item.MateriaSlots > 0 && i.MateriaCount() < 5))
            {
                bindingSourceAffix.Add(bagSlot);
            }

            affixCb.DisplayMember = "Name";
            affixCb.DataSource = bindingSourceAffix;
            
            //materiaListBox.DataSource = MateriaBase.Materia(_selectedBagSlot);
           // materiaListBox.DisplayMember = "ItemName";
        }

        private void affixCb_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }
    }
}