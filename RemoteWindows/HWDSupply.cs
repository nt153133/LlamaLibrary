using System;
using System.Text;
using ff14bot;
using ff14bot.Enums;
using LlamaLibrary.Helpers;

namespace LlamaLibrary.RemoteWindows
{
    public class HWDSupply : RemoteWindow<HWDSupply>
    {
        private const string WindowName = "HWDSupply";

        public HWDSupply() : base(WindowName)
        {
            _name = WindowName;
        }

        public int CurrentClassSelected()
        {
            if (Translator.Language == Language.Chn)
                return ___Elements()[29].TrimmedData;
            else
            {
                return ___Elements()[62].TrimmedData;
            }
        }
        
        public int GetAccumulatedScore()
        {
            return ___Elements()[17+CurrentClassSelected()].TrimmedData;
        }
        
        public int NumberOfKupoTickets()
        {
            if (Translator.Language == Language.Chn)
                return 0;
            else
            {
                var data = Core.Memory.ReadString((IntPtr) ___Elements()[3].Data, Encoding.UTF8).Split('/');
                return data.Length < 2 ? 0 : int.Parse(data[0].Trim());
            }
        }

        public int ClassSelected
        {
            get => CurrentClassSelected();
            set
            {
                if (WindowByName != null && CurrentClassSelected() != value)
                    SendAction(2, 0, 1, 1, (ulong)value);
            }
        }

        public void ClickItem(int index)
        {
            SendAction(2, 3, 1, 3, (ulong)index);
        }

        public override void Close()
        {
            SendAction(1, 3, UInt64.MaxValue);
        }
    }
}