using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ff14bot.Managers;
using static LlamaLibrary.AutoRetainerSort.Classes.ItemSortInfo;

namespace LlamaLibrary.AutoRetainerSort
{
    public partial class AddNewItemForm : Form
    {
        private static Dictionary<uint, string> _itemNameCache;
        public static Dictionary<uint, string> ItemNameCache
        {
            get
            {
                if (_itemNameCache == null)
                {
                    _itemNameCache = new Dictionary<uint, string>();

                    foreach (var idItemPair in DataManager.ItemCache)
                    {
                        uint itemId = idItemPair.Key;
                        Item itemInfo = idItemPair.Value;
                        if (itemId == 0 || itemId > QualityOffset || itemInfo == null) continue;
                        _itemNameCache[itemId] = itemInfo.CurrentLocaleName;
                    }
                }

                return _itemNameCache;
            }
        }

        public static string GetAdjustedItemName(uint rawItemId) => ItemNameCache.TryGetValue(rawItemId, out string name) ? name : "UNKNOWN";

        public AddNewItemForm()
        {
            InitializeComponent();
        }

        private void Form_Load(object sender, EventArgs e)
        {
            if (Owner != null)
            {
                int ownerCenterX = Owner.Location.X + (Owner.Width / 2) - (Width / 2);
                int ownerCenterY = Owner.Location.Y + (Owner.Height / 2) - (Width / 2);
                Location = new Point(ownerCenterX, ownerCenterY);
            }
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            var searchResults = new List<SearchResult>();
            if (string.IsNullOrEmpty(textBoxSearch.Text)) return;
            var splitSearchText = textBoxSearch.Text.Split(' ');
            int foundCount = 0;
            foreach (var idNamePair in ItemNameCache)
            {
                string itemName = idNamePair.Value;
                for (int i = 0; i < splitSearchText.Length; i++)
                {
                    if (itemName.IndexOf(splitSearchText[i], StringComparison.OrdinalIgnoreCase) < 0) continue;
                }
                searchResults.Add(new SearchResult(idNamePair.Key, itemName));
                if (++foundCount >= 10) break;
            }

            listBoxSearchResults.SelectedIndex = -1;
            listBoxSearchResults.DataSource = searchResults;
            listBoxSearchResults.ResetBindings();
            listBoxSearchResults.Refresh();
        }

        private void SearchResults_SelectionChanged(object sender, EventArgs e)
        {
            if (listBoxSearchResults.SelectedIndex < 0)
            {
                return;
            }

            if (!(listBoxSearchResults.SelectedItem is SearchResult selectedItem))
            {
                return;
            }

            SelectedSearchResult = selectedItem;
            txtBoxItem.Text = selectedItem.Name;
        }

        public SearchResult SelectedSearchResult;

        private void ModifierRadioButton_Changed(object sender, EventArgs e)
        {
            if (sender == rdBtnHQ)
            {
                checkBoxHQ.Checked = false;
                checkBoxHQ.Enabled = false;
                checkBoxCollectable.Enabled = true;
            }
            else if (sender == rdBtnCollectable)
            {
                checkBoxCollectable.Checked = false;
                checkBoxCollectable.Enabled = false;
                checkBoxHQ.Enabled = true;
            }
            else if (sender == rdBtnNone)
            {
                checkBoxCollectable.Enabled = true;
                checkBoxHQ.Enabled = true;
            }
        }

        public bool ModifierNone => rdBtnNone.Checked;

        public bool ModifierHQ => rdBtnHQ.Checked;

        public bool ModifierCollectable => rdBtnCollectable.Checked;

        public bool IncludeHQ => checkBoxHQ.Checked;

        public bool IncludeCollectable => checkBoxCollectable.Checked;
    }

    public class SearchResult
    {
        public readonly string Name; 

        public uint RawItemId;

        public override string ToString() => Name;

        public SearchResult(uint rawItemId, string name)
        {
            RawItemId = rawItemId;
            Name = name;
        }
    }
}
