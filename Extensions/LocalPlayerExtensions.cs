using ff14bot;
using ff14bot.Objects;
using LlamaLibrary.Memory;

namespace LlamaLibrary.Extensions
{
    public static class LocalPlayerExtensions
    {
        internal static byte GatheringStatus(this LocalPlayer player)
        {
            return Core.Memory.Read<byte>(player.Pointer + Offsets.GatheringStateOffset);
        }
    }
}