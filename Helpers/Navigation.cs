using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Media;
using Buddy.Coroutines;
using Clio.Utilities;
using Clio.Utilities.Helpers;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.Navigation;
using ff14bot.Objects;
using ff14bot.Pathing;
using TreeSharp;

namespace LlamaLibrary.Helpers
{
    public static class Navigation
    {
        public static readonly WaitTimer waitTimer_0 = new WaitTimer(new TimeSpan(0, 0, 0, 15));
        private static async Task<Queue<NavGraph.INode>> GenerateNodes(uint ZoneId, Vector3 xyz)
        {
            return await NavGraph.GetPathAsync((uint)ZoneId, xyz);
        }

        public static async Task<bool> GetTo(uint ZoneId, Vector3 XYZ)
        {
            var path = await GenerateNodes(ZoneId, XYZ );
            
            if (path == null && WorldManager.ZoneId != ZoneId)
                return false;
            
            if (path == null)
            {
                return await FlightorMove(XYZ);
            }
            
            if (path.Count < 1)
            {
                LogCritical($"Couldn't get a path to {XYZ} on {ZoneId}, Stopping.");
                return false;
            }
            
            object object_0 = new object();
            var composite =  NavGraph.NavGraphConsumer(j => path);

            while (path.Count > 0)
            {
                composite.Start(object_0);
                await Coroutine.Yield();
                while (composite.Tick(object_0) == RunStatus.Running)
                {
                    await Coroutine.Yield();
                }
                composite.Stop(object_0);
                await Coroutine.Yield();
            }
            
            Navigator.Stop();

            return Navigator.InPosition(Core.Me.Location, XYZ, 3);
        }
        
        public static void LogCritical(string text)
        {
            Logging.Write(Colors.OrangeRed, text);
        }

        public static async Task OffMeshMove(Vector3 _target)
        {
            waitTimer_0.Reset();
            Navigator.PlayerMover.MoveTowards(_target);
            while (_target.Distance2D(Core.Me.Location) >= 4 && !waitTimer_0.IsFinished)
            {
                Navigator.PlayerMover.MoveTowards(_target);
                await Coroutine.Sleep(100);
            }

            Navigator.PlayerMover.MoveStop();
        }
        
        public static async Task<bool> OffMeshMoveInteract(GameObject _target)
        {
            waitTimer_0.Reset();
            Navigator.PlayerMover.MoveTowards(_target.Location);
            while (!_target.IsWithinInteractRange && !waitTimer_0.IsFinished)
            {
                Navigator.PlayerMover.MoveTowards(_target.Location);
                await Coroutine.Sleep(100);
            }

            Navigator.PlayerMover.MoveStop();
            return _target.IsWithinInteractRange;
        }
        
        internal static async Task<bool> FlightorMove(Vector3 loc)
        {
            var moving = MoveResult.GeneratingPath;
            while (!(moving == MoveResult.Done ||
                     moving == MoveResult.ReachedDestination ||
                     moving == MoveResult.Failed ||
                     moving == MoveResult.Failure ||
                     moving == MoveResult.PathGenerationFailed))
            {
                moving = Flightor.MoveTo(new FlyToParameters(loc));

                await Coroutine.Yield();
            }

            return moving == MoveResult.ReachedDestination;
        }
    }
}