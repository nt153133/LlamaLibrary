namespace LlamaLibrary.RemoteWindows
{
    public class Character: RemoteWindow<Character>
    {

        private const string WindowName = "Character";
        
        public Character() : base(WindowName)
        {
            _name = WindowName;
        }

        public void UpdateGearSet()
        {
            SendAction(1,3,0xF);
        }

        public bool CanUpdateGearSet()
        {
            var button = WindowByName.FindButton(18);

            if (button == null)
                return false;

            return button.Clickable;
        }
    }
}