﻿//
// LICENSE:
// This work is licensed under the
//     Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.
// also known as CC-BY-NC-SA.  To view a copy of this license, visit
//      http://creativecommons.org/licenses/by-nc-sa/3.0/
// or send a letter to
//      Creative Commons // 171 Second Street, Suite 300 // San Francisco, California, 94105, USA.
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Media;
using Buddy.Coroutines;
using Clio.Utilities;
using Clio.XmlEngine;
using ff14bot.Behavior;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.Navigation;
using ff14bot.Objects;
using ff14bot.RemoteWindows;
using NeoGaia.ConnectionHandler;
using QuickGraph;
using QuickGraph.Algorithms;
using QuickGraph.Algorithms.Observers;
using QuickGraph.Algorithms.ShortestPath;
using SQLite;
using TreeSharp;
using Action = TreeSharp.Action;
using LlamaLibrary.Helpers;

namespace ff14bot.NeoProfiles.Tags
{

    [XmlElement("LLGetTo")]
    public class GetTo : ProfileBehavior
    {
        [XmlAttribute("XYZ")]
        public Vector3 XYZ { get; set; }

        [XmlAttribute("ZoneId")]
        public int ZoneId { get; set; }


        private bool _generatedNodes = false;
        private bool _isdone;
        public override bool IsDone
        {
            get
            {
                return FinalizedPath?.Count == 0;
            }
        }

        public override bool HighPriority
        {
            get { return true; }
        }

        protected override void OnResetCachedDone()
        {
            _isdone = false;
            _generatedNodes = false;
            FinalizedPath = null;
        }

        public Queue<NavGraph.INode> FinalizedPath;
        protected override void OnStart()
        {
        }

        protected override void OnDone()
        {
        }



        private async Task<bool> GenerateNodes()
        {
            var path = await NavGraph.GetPathAsync((uint)ZoneId, XYZ);
            if (path == null)
            {
                LogError($"Couldn't get a path to {XYZ} on {ZoneId}, Stopping.");
                return true;
            }
            _generatedNodes = true;
            FinalizedPath = path;
            return true;
        }

        protected override Composite CreateBehavior()
        {
            return new PrioritySelector(
                CommonBehaviors.HandleLoading,

                //new Decorator(r => !_generatedNodes, new ActionRunCoroutine(r => GenerateNodes())),
                //NavGraph.NavGraphConsumer(r => FinalizedPath)
                new Decorator(r => !_generatedNodes, new ActionRunCoroutine(t => Lisbeth.TravelTo(ZoneId.ToString(), XYZ)))
                );
        }


    }
}
