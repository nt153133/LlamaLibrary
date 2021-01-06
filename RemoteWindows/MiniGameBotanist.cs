using System;
using System.Text;
using System.Text.RegularExpressions;
using ff14bot;

namespace LlamaLibrary.RemoteWindows
{
    public class MiniGameBotanist : RemoteWindow<MiniGameBotanist>
    {
        private const string WindowName = "MiniGameBotanist";

        private readonly Regex _timeRegex = new Regex(@"(\d):(\d+).*", RegexOptions.Compiled);

        public MiniGameBotanist() : base(WindowName)
        {
            _name = WindowName;
        }

        public void PressButton()
        {
            SendAction(3,3,0xB,3,0,3,0);
        }
        
        public void PauseCursor()
        {
            SendAction(1,3,0xF);
        }
        
        public void ResumeCursor()
        {
            SendAction(1,3,0xF);
        }

        public int GetNumberOfTriesLeft => IsOpen ? ___Elements()[11].TrimmedData : 0;

        public int GetProgressLeft => IsOpen ? ___Elements()[12].TrimmedData : 0;

        public int GetProgressTotal => IsOpen ? ___Elements()[13].TrimmedData : 0;

        public int GetTimeLeft
        {
            get
            {
                var data = Core.Memory.ReadString((IntPtr) ___Elements()[15].Data, Encoding.UTF8);

                if (!_timeRegex.IsMatch(data)) return 0;

                var sec = int.Parse(_timeRegex.Match(data).Groups[2].Value.Trim());
                var min = int.Parse(_timeRegex.Match(data).Groups[1].Value.Trim());

                if (min > 0) return 60 + sec;

                return sec;
            }
        }
    }
}