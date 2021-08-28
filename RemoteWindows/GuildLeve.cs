using System;
using System.Text;
using ff14bot;

namespace LlamaLibrary.RemoteWindows
{
    public class GuildLeve: RemoteWindow<GuildLeve>
    {
        private const string WindowName = "GuildLeve";
        
        public GuildLeve() : base(WindowName)
        {
            _name = WindowName;
        }
        
        public LeveWindow Window =>  (LeveWindow) ___Elements()[6].TrimmedData;
        
        

        public string PrintWindow()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(Window.ToString());

            for (int i = 0; i < 5; i++)
            {
                var leveBlock = GetLeveGroup(i);
                
                //sb.AppendLine("Block " + i);
                
                foreach (var leve in leveBlock)
                {
                    if (!leve.Contains("Level "))
                        sb.AppendLine(leve);
                }
            }

            return sb.ToString();
        }

        public string[] GetLeveGroup(int index)
        {
            string[] names = new string[3];
            
            names[0] = Core.Memory.ReadString((IntPtr) ___Elements()[((index * 8) + 628)].Data, Encoding.UTF8);
            names[1] = Core.Memory.ReadString((IntPtr) ___Elements()[((index * 8) + 628) + 2].Data, Encoding.UTF8);
            names[2] = Core.Memory.ReadString((IntPtr) ___Elements()[((index * 8) + 628) + 4].Data, Encoding.UTF8);

            return names;
        }

        public void SwitchType(int index)
        {
            SendAction(3, 3,9,3,(ulong) index,3,0);
        }
        
        public void SwitchClass(int index)
        {
            SendAction(2, 3,0xB,3,(ulong) index);
        }
        
    }

    public enum LeveWindow
    {
        Battle = 0,
        Gathering = 3,
        Crafting = 8
    }
}