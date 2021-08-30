using System;
using System.Collections.Generic;
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
            tabControl1.SelectedIndexChanged += TabControl1_SelectedIndexChanged;
            bindingSourceInventory.Clear();

            foreach (var bagSlot in InventoryManager.EquippedItems.Where(MateriaBase.HasMateria))
            {
                bindingSourceInventory.Add(bagSlot);
            }

            itemCb.DisplayMember = "Name";
            itemCb.DataSource = bindingSourceInventory;

            if (_selectedBagSlot != null)
            {
                materiaListBox.DataSource = MateriaBase.Materia(_selectedBagSlot);
                materiaListBox.DisplayMember = "ItemName";
            }
        }

        private void itemCb_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _selectedBagSlot = (BagSlot)itemCb.SelectedItem;
            materiaListBox.DataSource = MateriaBase.Materia(_selectedBagSlot);
            materiaListBox.DisplayMember = "ItemName";
        }

        private void affixCb_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _selectedBagSlotAffix = (BagSlot)affixCb.SelectedItem;
            // affixLb.DataSource = MateriaBase.Materia(_selectedBagSlotAffix);
            // affixLb.DisplayMember = "ItemName";
            SetComboBoxes(_selectedBagSlotAffix);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MateriaBase.ItemToRemoveMateria = _selectedBagSlot;
            MateriaBase.MateriaTask = MateriaTask.Remove;
            Logger.External("Materia Settings", "Click", Colors.Blue);

            if (BotManager.Current.Name == "Materia")
            {
                BotManager.Current.Start();
                TreeRoot.Start();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bindingSourceInventory.Clear();
            foreach (var bagSlot in InventoryManager.EquippedItems.Where(i => i.HasMateria()))
            {
                bindingSourceInventory.Add(bagSlot);
            }

            itemCb.DisplayMember = "Name";
            itemCb.DataSource = bindingSourceInventory;
            itemCb.Update();
            itemCb_SelectionChangeCommitted(this, null);

            if (_selectedBagSlot != null)
            {
                materiaListBox.DataSource = MateriaBase.Materia(_selectedBagSlot);
                materiaListBox.DisplayMember = "ItemName";
            }
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {
        }

        private void button3_Click(object sender, EventArgs e)
        {
            bindingSourceAffix.Clear();
            foreach (var bagSlot in InventoryManager.EquippedItems.Where(i => i.IsFilled && i.Item.MateriaSlots > 0 && i.MateriaCount() < 5))
            {
                bindingSourceAffix.Add(bagSlot);
            }

            if (_selectedBagSlotAffix != null)
            {
                affixCb.DisplayMember = "Name";
                affixCb.DataSource = bindingSourceAffix;
                SetComboBoxes(_selectedBagSlotAffix);
            }

            //materiaListBox.DataSource = MateriaBase.Materia(_selectedBagSlot);
            // materiaListBox.DisplayMember = "ItemName";
        }

        private void affixCb_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedBagSlotAffix = (BagSlot)affixCb.SelectedItem;
            // affixLb.DataSource = MateriaBase.Materia(_selectedBagSlotAffix);
            // affixLb.DisplayMember = "ItemName";
            SetComboBoxes(_selectedBagSlotAffix);
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void TabControl1_SelectedIndexChanged(Object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {
                bindingSourceInventory.Clear();
                foreach (var bagSlot in InventoryManager.EquippedItems.Where(i => i.HasMateria()))
                {
                    bindingSourceInventory.Add(bagSlot);
                }

                itemCb.DisplayMember = "Name";
                itemCb.DataSource = bindingSourceInventory;

                if (_selectedBagSlot != null)
                {
                    materiaListBox.DataSource = MateriaBase.Materia(_selectedBagSlot);
                    materiaListBox.DisplayMember = "ItemName";
                }
            }

            if (tabControl1.SelectedIndex == 1)
            {
                bindingSourceAffix.Clear();
                foreach (var bagSlot in InventoryManager.EquippedItems.Where(i => i.IsFilled && i.Item.MateriaSlots > 0 && i.MateriaCount() < 5))
                {
                    bindingSourceAffix.Add(bagSlot);
                }

                if (_selectedBagSlotAffix != null)
                {
                    affixCb.DisplayMember = "Name";
                    affixCb.DataSource = bindingSourceAffix;
                    SetComboBoxes(_selectedBagSlotAffix);
                }
            }
            //MessageBox.Show("You are in the TabControl.SelectedIndexChanged event.");
        }

        private void SetComboBoxes(BagSlot slot)
        {
            var list = MateriaBase.Materia(slot);
            //var inventoryMateria =
            bindingSourceInventoryMateria.DataSource = InventoryManager.FilledSlots.Where(i => i.Item.EquipmentCatagory == ItemUiCategory.Materia);
            var materia = InventoryManager.FilledSlots.Where(i => i.Item.EquipmentCatagory == ItemUiCategory.Materia).Select(r => new MateriaToAffix(r)).ToList();
            materia.Add(new MateriaToAffix(null));
            switch (list.Count)
            {
                case 0:
                    MateriaCb1.DataSource = materia.ToList();//InventoryManager.FilledSlots.Where(i => i.Item.EquipmentCatagory == ItemUiCategory.Materia).Select(r => new MateriaToAffix(r)).ToList(); ;
                    MateriaCb1.Enabled = true;
                    MateriaCb1.DisplayMember = "Display";
                    MateriaCb2.DataSource = materia.ToList();//InventoryManager.FilledSlots.Where(i => i.Item.EquipmentCatagory == ItemUiCategory.Materia).Select(r => new MateriaToAffix(r)).ToList();
                    MateriaCb2.Enabled = true;
                    MateriaCb2.DisplayMember = "Display";
                    MateriaCb3.DataSource = materia.ToList();//InventoryManager.FilledSlots.Where(i => i.Item.EquipmentCatagory == ItemUiCategory.Materia).Select(r => new MateriaToAffix(r)).ToList();
                    MateriaCb3.Enabled = true;
                    MateriaCb3.DisplayMember = "Display";
                    MateriaCb4.DataSource = materia.ToList();//InventoryManager.FilledSlots.Where(i => i.Item.EquipmentCatagory == ItemUiCategory.Materia).Select(r => new MateriaToAffix(r)).ToList();
                    MateriaCb4.Enabled = true;
                    MateriaCb4.DisplayMember = "Display";
                    MateriaCb5.DataSource = materia.ToList();//InventoryManager.FilledSlots.Where(i => i.Item.EquipmentCatagory == ItemUiCategory.Materia).Select(r => new MateriaToAffix(r)).ToList();
                    MateriaCb5.Enabled = true;
                    MateriaCb5.DisplayMember = "Display";
                    break;

                case 1:
                    MateriaCb1.DataSource = list.ToArray();
                    MateriaCb1.SelectedIndex = 0;
                    MateriaCb1.DisplayMember = "ItemName";
                    MateriaCb1.Refresh();
                    MateriaCb1.Enabled = false;
                    MateriaCb2.DataSource = materia.ToList();//InventoryManager.FilledSlots.Where(i => i.Item.EquipmentCatagory == ItemUiCategory.Materia).Select(r => new MateriaToAffix(r)).ToList();
                    MateriaCb2.DisplayMember = "Display";
                    MateriaCb3.DataSource = materia.ToList();//InventoryManager.FilledSlots.Where(i => i.Item.EquipmentCatagory == ItemUiCategory.Materia).Select(r => new MateriaToAffix(r)).ToList();
                    MateriaCb3.DisplayMember = "Display";
                    MateriaCb4.DataSource = materia.ToList();//InventoryManager.FilledSlots.Where(i => i.Item.EquipmentCatagory == ItemUiCategory.Materia).Select(r => new MateriaToAffix(r)).ToList();
                    MateriaCb4.DisplayMember = "Display";
                    MateriaCb5.DataSource = materia.ToList();//InventoryManager.FilledSlots.Where(i => i.Item.EquipmentCatagory == ItemUiCategory.Materia).Select(r => new MateriaToAffix(r)).ToList();
                    MateriaCb5.DisplayMember = "Display";
                    MateriaCb2.Enabled = true;
                    MateriaCb3.Enabled = true;
                    MateriaCb4.Enabled = true;
                    MateriaCb5.Enabled = true;
                    break;

                case 2:
                    MateriaCb1.DataSource = list.ToArray();
                    MateriaCb1.SelectedIndex = 0;
                    MateriaCb1.DisplayMember = "ItemName";
                    MateriaCb1.Refresh();
                    MateriaCb1.Enabled = false;

                    MateriaCb2.DataSource = list.ToArray();
                    MateriaCb2.SelectedIndex = 1;
                    MateriaCb2.DisplayMember = "ItemName";
                    MateriaCb2.Enabled = false;
                    MateriaCb3.DataSource = materia.ToList();//InventoryManager.FilledSlots.Where(i => i.Item.EquipmentCatagory == ItemUiCategory.Materia).Select(r => new MateriaToAffix(r)).ToList();
                    MateriaCb3.DisplayMember = "Display";
                    MateriaCb4.DataSource = materia.ToList();//InventoryManager.FilledSlots.Where(i => i.Item.EquipmentCatagory == ItemUiCategory.Materia).Select(r => new MateriaToAffix(r)).ToList();
                    MateriaCb4.DisplayMember = "Display";
                    MateriaCb5.DataSource = materia.ToList();//InventoryManager.FilledSlots.Where(i => i.Item.EquipmentCatagory == ItemUiCategory.Materia).Select(r => new MateriaToAffix(r)).ToList();
                    MateriaCb5.DisplayMember = "Display";
                    MateriaCb3.Enabled = true;
                    MateriaCb4.Enabled = true;
                    MateriaCb5.Enabled = true;
                    break;

                case 3:

                    MateriaCb1.DataSource = list.ToArray();
                    MateriaCb1.SelectedIndex = 0;
                    MateriaCb1.DisplayMember = "ItemName";
                    MateriaCb1.Refresh();
                    MateriaCb1.Enabled = false;

                    MateriaCb2.DataSource = list.ToArray();
                    MateriaCb2.SelectedIndex = 1;
                    MateriaCb2.DisplayMember = "ItemName";
                    MateriaCb2.Enabled = false;

                    MateriaCb3.DataSource = list.ToArray();
                    MateriaCb3.SelectedIndex = 2;
                    MateriaCb3.DisplayMember = "ItemName";
                    MateriaCb3.Enabled = false;
                    MateriaCb4.DataSource = materia.ToList();//InventoryManager.FilledSlots.Where(i => i.Item.EquipmentCatagory == ItemUiCategory.Materia).Select(r => new MateriaToAffix(r)).ToList();
                    MateriaCb4.DisplayMember = "Display";
                    MateriaCb5.DataSource = materia.ToList();//InventoryManager.FilledSlots.Where(i => i.Item.EquipmentCatagory == ItemUiCategory.Materia).Select(r => new MateriaToAffix(r)).ToList();
                    MateriaCb5.DisplayMember = "Display";
                    MateriaCb4.Enabled = true;
                    MateriaCb5.Enabled = true;
                    break;

                case 4:

                    MateriaCb1.DataSource = list.ToArray();
                    MateriaCb1.SelectedIndex = 0;
                    MateriaCb1.DisplayMember = "ItemName";
                    MateriaCb1.Refresh();
                    MateriaCb1.Enabled = false;

                    MateriaCb2.DataSource = list.ToArray();
                    MateriaCb2.SelectedIndex = 1;
                    MateriaCb2.DisplayMember = "ItemName";
                    MateriaCb2.Enabled = false;

                    MateriaCb3.DataSource = list.ToArray();
                    MateriaCb3.SelectedIndex = 2;
                    MateriaCb3.DisplayMember = "ItemName";
                    MateriaCb3.Enabled = false;

                    MateriaCb4.Enabled = false;
                    MateriaCb4.DataSource = list.ToArray();
                    MateriaCb4.SelectedIndex = 3;
                    MateriaCb4.DisplayMember = "ItemName";
                    MateriaCb5.DataSource = materia.ToList();//InventoryManager.FilledSlots.Where(i => i.Item.EquipmentCatagory == ItemUiCategory.Materia).Select(r => new MateriaToAffix(r)).ToList();
                    MateriaCb5.DisplayMember = "Display";
                    MateriaCb5.Enabled = true;
                    break;

                default:
                    break;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var list = MateriaBase.Materia(_selectedBagSlotAffix);
            List<BagSlot> materiaToAdd = new List<BagSlot>();
            //var inventoryMateria =
            //bindingSourceInventoryMateria.DataSource =InventoryManager.FilledSlots.Where(i=> i.Item.EquipmentCatagory == ItemUiCategory.Materia);

            switch (list.Count)
            {
                case 0:
                    materiaToAdd.Add(((MateriaToAffix)MateriaCb1.SelectedItem).BagSlot);
                    materiaToAdd.Add(((MateriaToAffix)MateriaCb2.SelectedItem).BagSlot);
                    materiaToAdd.Add(((MateriaToAffix)MateriaCb3.SelectedItem).BagSlot);
                    materiaToAdd.Add(((MateriaToAffix)MateriaCb4.SelectedItem).BagSlot);
                    materiaToAdd.Add(((MateriaToAffix)MateriaCb5.SelectedItem).BagSlot);
                    break;

                case 1:
                    materiaToAdd.Add(((MateriaToAffix)MateriaCb2.SelectedItem).BagSlot);
                    materiaToAdd.Add(((MateriaToAffix)MateriaCb3.SelectedItem).BagSlot);
                    materiaToAdd.Add(((MateriaToAffix)MateriaCb4.SelectedItem).BagSlot);
                    materiaToAdd.Add(((MateriaToAffix)MateriaCb5.SelectedItem).BagSlot);
                    break;

                case 2:
                    materiaToAdd.Add(((MateriaToAffix)MateriaCb3.SelectedItem).BagSlot);
                    materiaToAdd.Add(((MateriaToAffix)MateriaCb4.SelectedItem).BagSlot);
                    materiaToAdd.Add(((MateriaToAffix)MateriaCb5.SelectedItem).BagSlot);
                    break;

                case 3:
                    materiaToAdd.Add(((MateriaToAffix)MateriaCb4.SelectedItem).BagSlot);
                    materiaToAdd.Add(((MateriaToAffix)MateriaCb5.SelectedItem).BagSlot);
                    break;

                case 4:
                    materiaToAdd.Add(((MateriaToAffix)MateriaCb5.SelectedItem).BagSlot);
                    break;

                default:
                    break;
            }

            MateriaBase.MateriaToAdd = materiaToAdd;
            MateriaBase.MateriaTask = MateriaTask.Affix;
            MateriaBase.ItemToAffixMateria = _selectedBagSlotAffix;

            if (BotManager.Current.Name == "Materia")
            {
                BotManager.Current.Start();
                TreeRoot.Start();
            }
        }

        private void MateriaCb1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
    }

    public class MateriaToAffix
    {
        public BagSlot BagSlot;
        public MateriaItem MateriaItem;

        public string Display => BagSlot == null ? "Stop" : $"{BagSlot.Item.CurrentLocaleName} +{MateriaItem.Value} {MateriaItem.Stat}";

        public MateriaToAffix(BagSlot slot)
        {
            if (slot == null)
            {
                BagSlot = null;
                return;
            }

            BagSlot = slot;
            MateriaItem = MateriaBase.MateriaList.SelectMany(i => i.Value).FirstOrDefault(r => r.Key == slot.RawItemId);


        }
    }
}