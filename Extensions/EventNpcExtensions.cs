using System;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Objects;
using LlamaLibrary.Memory.Attributes;

namespace LlamaLibrary.Extensions
{
    public static class EventNpcExtensions
    {
        internal static class Offsets
        {
            [Offset("Search 44 89 BF ?? ?? ?? ?? 83 BF ?? ?? ?? ?? ?? Add 3 Read32")]
            internal static int IconID;
        }
        
        internal static uint IconId(this GameObject eventNpc)
        {
            return eventNpc.Type == GameObjectType.EventNpc ? Core.Memory.Read<uint>(eventNpc.Pointer + Offsets.IconID) : (uint) 0;
        }
    }
}