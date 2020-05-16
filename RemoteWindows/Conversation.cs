using System.Collections.Generic;
using ff14bot.RemoteWindows;

namespace LlamaLibrary.RemoteWindows
{
    public static class Conversation
    {
        public static bool IsOpen => SelectString.IsOpen || SelectIconString.IsOpen || CutSceneSelectString.IsOpen;

        public static List<string> GetConversationList
        {
            get
            {
                if (SelectString.IsOpen)
                    return SelectString.Lines();
                if (SelectIconString.IsOpen)
                    return SelectIconString.Lines();
                return new List<string>();
            }
        }

        public static void SelectLine(uint line)
        {
            if (SelectString.IsOpen)
                SelectString.ClickSlot(line);
            else if (SelectIconString.IsOpen)
                SelectIconString.ClickSlot(line);
            else if (CutSceneSelectString.IsOpen)
                CutSceneSelectString.ClickSlot(line);
        }
    }
}