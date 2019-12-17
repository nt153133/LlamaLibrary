﻿using System;
using System.Collections.Generic;
using ff14bot.Behavior;
using ff14bot.Managers;
using ff14bot.NeoProfiles;
using LlamaLibrary.Helpers;
using TreeSharp;
using static ff14bot.Behavior.TreeHooks;

namespace LlamaLibrary.RemoteWindows
{
    public class RetainerTaskList : RemoteWindow<RetainerTaskList>
    {
        private const string WindowName = "RetainerTaskList";

        public RetainerTaskList() : base(WindowName)
        {
            _name = WindowName;
        }

        public void SelectVenture(int taskId)
        {
            SendAction(2, 3, 0x0B, 03, (ulong) taskId);
        }
        
    }
}