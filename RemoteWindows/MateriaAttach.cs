namespace LlamaLibrary.RemoteWindows
{
    public class MateriaAttach: RemoteWindow<MateriaAttach>
    {
        private const string WindowName = "MateriaAttach";

        public MateriaAttach() : base(WindowName)
        {
            _name = WindowName;
        }

        public void ClickItem(int index)
        {
            //SendAction( 2, 3uL, 6,3,(ulong) index);
            SendAction( 3, 3uL, 1,3,(ulong) index,3,1);
        }
        
        public void ClickMateria(int index)
        {
            SendAction( 3, 3uL, 2,3,(ulong) index, 3,1); 
        }
    }
}