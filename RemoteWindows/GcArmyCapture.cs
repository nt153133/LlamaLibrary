namespace LlamaLibrary.RemoteWindows
{
    public class GcArmyCapture: RemoteWindow<GcArmyCapture>
    {
        private const string WindowName = "GcArmyCapture";

        public GcArmyCapture() : base(WindowName)
        {
            _name = WindowName;
        }

        public void Commence()
        {
            SendAction(1, 3, 0xd);
        }

        /// <summary>
        ///     Sets the squadron command mission
        /// </summary>
        /// <param name="index">The duty index from the list starting at 0.</param>
        public void SelectDuty(int index)
        {
            SendAction(2, 3, 0xB, 4, (ulong) index);
        }
    }
}