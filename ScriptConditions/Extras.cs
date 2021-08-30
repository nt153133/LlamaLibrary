using System.Collections.Generic;
using System.Linq;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Objects;
using LlamaLibrary.Helpers;

namespace LlamaLibrary.ScriptConditions
{
    public static class Extras
    {
        public static int NumAttackableEnemies(float dist = 0, params uint[] ids)
        {
            if (ids.Length == 0)
            {
                if (dist > 0) return GameObjectManager.GetObjectsOfType<BattleCharacter>().Count(i => i.CanAttack && i.IsTargetable && i.Distance() < dist);
                else return GameObjectManager.GetObjectsOfType<BattleCharacter>().Count(i => i.CanAttack && i.IsTargetable);
            }
            else
            {
                if (dist > 0) return GameObjectManager.GetObjectsByNPCIds<BattleCharacter>(ids).Count(i => i.CanAttack && i.IsTargetable && i.Distance() < dist);
                else return GameObjectManager.GetObjectsByNPCIds<BattleCharacter>(ids).Count(i => i.CanAttack && i.IsTargetable);
            }
        }

        public static int SphereCompletion(int itemID)
        {
            return (int) (InventoryManager.FilledInventoryAndArmory.FirstOrDefault(i => i.RawItemId == (uint)itemID).SpiritBond);
        }

        public static int HighestILvl(ClassJobType job)
        {
            IEnumerable<GearSet> sets = GearsetManager.GearSets.Where(g => g.InUse && g.Class == job && g.Gear.Any());
            return sets.Any() ? sets.Max(GeneralFunctions.GetGearSetiLvl) : 0;
        }

        public static bool IsFateActive(int fateID)
        {
            return FateManager.ActiveFates.Any(i => i.Id == (uint)fateID);
        }

        public static bool HasLearnedMount(int mountID)
        {
            return ActionManager.AvailableMounts.Any(i=> i.Id == ((uint)mountID));
        }

        public static int BeastTribeRank(int tribeID)
        {
            return BeastTribeHelper.GetBeastTribeRank(tribeID);
        }

        public static int DailyQuestAllowance()
        {
            return (int) BeastTribeHelper.DailyQuestAllowance();
        }

		public static bool LisbethPresent()
        {
            var loader = BotManager.Bots
                .FirstOrDefault(c => c.Name == "Lisbeth");

            if (loader == null) return false;
			return true;

        }

        public static bool IsTargetableNPC(int npcID)
        {
            return GameObjectManager.GameObjects.Any(i => i.NpcId == (uint) npcID && i.IsVisible && i.IsTargetable);
        }	

        public static bool IsDutyEnded()
        {
			if (DirectorManager.ActiveDirector != null)
			{
				var instanceDirector = (ff14bot.Directors.InstanceContentDirector) DirectorManager.ActiveDirector;
				return instanceDirector.InstanceEnded;
			}

			return true;
        }		

    }
}