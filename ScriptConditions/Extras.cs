using System.Linq;
using ff14bot.Managers;
using ff14bot.Objects;

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
    }
}