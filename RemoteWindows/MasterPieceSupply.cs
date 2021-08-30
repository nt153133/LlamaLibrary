using System;
using System.Collections.Generic;
using System.Linq;
using ff14bot.Managers;
using ff14bot.RemoteWindows;

namespace LlamaLibrary.RemoteWindows
{
    public class MasterPieceSupply : RemoteWindow<MasterPieceSupply>
    {
        private const string WindowName = "MasterPieceSupply";

        public MasterPieceSupply() : base(WindowName)
        {
            _name = WindowName;
        }

        public int ClassSelected
        {
            get => ___Elements()[45].TrimmedData;
            set
            {
                if (WindowByName != null && ___Elements()[45].TrimmedData != value)
                    SendAction(2, 1, 2, 1, (ulong) value);
            }
        }

        public int GetNumberOfTurnins()
        {
            return IsOpen ? ___Elements()[0].TrimmedData : 0;
        }

        public List<Item> GetTurninItems()
        {
            var currentElements = ___Elements();

            var itemElements = new ArraySegment<TwoInt>(currentElements, 87, GetNumberOfTurnins());

            return itemElements.Select(item => DataManager.GetItem((uint) (item.TrimmedData - 500000))).ToList();
        }

        public Dictionary<Item, bool> GetTurninItemsStarred()
        {
            var result = new Dictionary<Item, bool>();

            var currentElements = ___Elements();

            var itemElements = new ArraySegment<TwoInt>(currentElements, 87, GetNumberOfTurnins()).ToArray();
            var starElements = new ArraySegment<TwoInt>(currentElements, 447, GetNumberOfTurnins()).ToArray();

            for (var i = 0; i < GetNumberOfTurnins(); i++)
            {
                result.Add(DataManager.GetItem((uint) (itemElements[i].TrimmedData - 500000)), starElements[i].TrimmedData == 1);
            }

            return result;
        }

        public void ClickItem(int index)
        {
            SendAction(2, 3, 1, 3, (ulong) index);
        }
    }
}