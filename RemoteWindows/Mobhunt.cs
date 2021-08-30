using System.Linq;
using ff14bot.Managers;

namespace LlamaLibrary.RemoteWindows
{
    public class Mobhunt: RemoteWindow<Mobhunt>
    {
        private AtkAddonControl _windowByName;
        private const string WindowName = "Mobhunt";

        public override AtkAddonControl WindowByName => RaptureAtkUnitManager.Controls.FirstOrDefault(i => i.Name.Contains(Name) && i.IsVisible);

        public Mobhunt() : base(WindowName)
        {
            _name = WindowName;

        }

        public void Accept()
        {
            SendAction(1,3,0);
        }
    }
}