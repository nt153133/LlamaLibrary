using System;
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