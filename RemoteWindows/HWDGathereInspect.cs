using System;

namespace LlamaLibrary.RemoteWindows
{
    public class HWDGathereInspect: RemoteWindow<HWDGathereInspect>
    {

        private const string WindowName = "HWDGathereInspect";

        public HWDGathereInspect() : base(WindowName)
        {
            _name = WindowName;
        }

        public void ClickAutoSubmit()
        {
            if (CanAutoSubmit())
                SendAction(1, 3, 0xC);
        }

        public void ClickRequestInspection()
        {
            if (CanRequestInspection())
                SendAction(1, 3, 0xB);
        }

        public void ClickClass(int index)
        {
            SendAction(2, 3,0xE,4,(ulong) index);
        }

        public bool CanAutoSubmit()
        {
            var button = WindowByName.FindButton(8);

            if (button != null)
                return button.Clickable;

            return false;
        }

        public bool CanRequestInspection()
        {
            var button = WindowByName.FindButton(10);

            if (button != null)
                return button.Clickable;

            return false;
        }

        public override void Close()
        {
            SendAction(1, 3, UInt64.MaxValue);
        }
    }
}