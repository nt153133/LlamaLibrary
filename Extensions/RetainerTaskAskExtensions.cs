﻿using ff14bot.Managers;
using ff14bot.RemoteWindows;

namespace LlamaLibrary.Extensions
{
    public static class RetainerTaskAskExtensions
    {
        public static bool CanAssign(this RetainerTaskAsk retainerTaskAsk)
        {
            var WindowByName = RaptureAtkUnitManager.GetWindowByName("RetainerTaskAsk");
            if (WindowByName == null) return false;
            var remoteButton = WindowByName.FindButton(40);
            return remoteButton != null && remoteButton.Clickable;
        }

        public static string GetErrorReason(this RetainerTaskAsk retainerTaskAsk)
        {
            var WindowByName = RaptureAtkUnitManager.GetWindowByName("RetainerTaskAsk");
            if (WindowByName == null || WindowByName.FindLabel(39) == null) return "";
            return WindowByName.FindLabel(39).Text;
        }
    }
}