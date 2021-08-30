using System;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot.RemoteWindows;

namespace LlamaLibrary.RemoteWindows
{
    public class GrandCompanySupplyList: RemoteWindow<GrandCompanySupplyList>
    {
        private const string WindowName = "GrandCompanySupplyList";

        public GrandCompanySupplyList() : base(WindowName)
        {
            _name = WindowName;
        }

        public int GetNumberOfTurnins()
        {
            return IsOpen ? ___Elements()[7].TrimmedData : 0;
        }

        public bool[] GetTurninBools()
        {
            var currentElements = ___Elements();
            var numberTurnins = GetNumberOfTurnins();

            var canHandInElements = new ArraySegment<TwoInt>(currentElements, 346, numberTurnins).Select(i=> (uint)i.TrimmedData).ToArray();
            var reqElements = new ArraySegment<TwoInt>(currentElements, 386, numberTurnins).Select(i=> (uint)i.TrimmedData).ToArray();

            bool[] turins = new bool[numberTurnins];

            for (int i = 0; i < numberTurnins; i++)
            {
                turins[i] = canHandInElements[i] >= reqElements[i];
            }

            return turins;
        }

        public uint[] GetTurninItemsIds()
        {
            var currentElements = ___Elements();
            var numberTurnins = GetNumberOfTurnins();

            var turninIdElements = new ArraySegment<TwoInt>(currentElements, 426, numberTurnins).Select(i=> (uint)i.TrimmedData).ToArray();

            return turninIdElements;
        }

        public uint[] GetTurninRequired()
        {
            var currentElements = ___Elements();
            var numberTurnins = GetNumberOfTurnins();

            var reqElements = new ArraySegment<TwoInt>(currentElements, 386, numberTurnins).Select(i=> (uint)i.TrimmedData).ToArray();

            return reqElements;
        }

        public void ClickItem(int index)
        {
            SendAction(2, 3, 1, 3, (ulong) index);
        }

        public async Task SwitchToExpertDelivery()
        {
            SendAction(2, 3,0,3,2);
            await Coroutine.Sleep(500);
            SendAction(2, 3,5,3,2);
            await Coroutine.Sleep(500);
        }

        public async Task SwitchToProvisioning()
        {
            //var button = WindowByName.FindButton(12);
            SendAction(2, 3,0,3,1);
            await Coroutine.Sleep(500);
        }

        public async Task SwitchToSupply()
        {
            SendAction(2, 3,0,3,0);
            await Coroutine.Sleep(500);
        }

    }
}