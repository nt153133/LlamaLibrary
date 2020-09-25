namespace LlamaLibrary.RemoteWindows
{
    public class MateriaAttachDialog: RemoteWindow<MateriaAttachDialog>
    {
        private const string WindowName = "MateriaAttachDialog";

        public MateriaAttachDialog() : base(WindowName)
        {
            _name = WindowName;
        }

        public void ClickAttach()
        {
            SendAction(1,3,0);
        }
    }
}