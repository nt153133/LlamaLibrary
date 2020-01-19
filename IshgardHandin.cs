using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Clio.Utilities;
using ff14bot;
using ff14bot.Behavior;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Navigation;
using ff14bot.NeoProfiles;
using ff14bot.Objects;
using ff14bot.Pathing;
using ff14bot.RemoteWindows;
using LlamaLibrary.RemoteWindows;

namespace LlamaLibrary
{
    public class IshgardHandin
    {
        //public uint NpcId = 1031690;
        public uint[] ids = new uint[] { 1031690, 1031677 };

        private GameObject Npc => GameObjectManager.GameObjects.FirstOrDefault(i => ids.Contains(i.NpcId) && i.IsVisible);
        public string EnglishName = "Potkin";
        public uint ZoneId;
        public uint FoundationZoneId;
        public uint AetheryteId = 70;
        


        public async Task<bool> HandInItem(uint itemId, int index, int job)
        {
            //GameObjectType.EventNpc;

            if (!HWDSupply.Instance.IsOpen && Npc == null)
            {
                await GetToNpc();
            }

            if (!HWDSupply.Instance.IsOpen && Npc.Location.Distance(Core.Me.Location) > 5f)
            {
                // NpcId = Npc.NpcId;
                var _target = Npc.Location;
                Navigator.PlayerMover.MoveTowards(_target);
                while (_target.Distance2D(Core.Me.Location) >= 4)
                {
                    Navigator.PlayerMover.MoveTowards(_target);
                    await Coroutine.Sleep(100);
                }
                Navigator.PlayerMover.MoveStop();
                await Coroutine.Sleep(1000);
            }

            if (!HWDSupply.Instance.IsOpen)
            {
                //NpcId = GameObjectManager.GameObjects.First(i => i.EnglishName == EnglishName).NpcId;
                Npc.Interact();
                await Coroutine.Wait(5000, () => HWDSupply.Instance.IsOpen || Talk.DialogOpen);
                await Coroutine.Sleep(1000);

                while (Talk.DialogOpen)
                {
                    Talk.Next();
                    await Coroutine.Wait(5000, () => !Talk.DialogOpen);
                }
                await Coroutine.Sleep(1000);
            }

            if (HWDSupply.Instance.IsOpen)
            {
                if (HWDSupply.Instance.ClassSelected != job)
                {
                    HWDSupply.Instance.ClassSelected = job;
                    await Coroutine.Sleep(1000);
                }
                //var item = InventoryManager.FilledSlots.FirstOrDefault(i => i.RawItemId == itemId);

                foreach (var item in InventoryManager.FilledSlots.Where(i => i.RawItemId == itemId))
                {
                    HWDSupply.Instance.ClickItem(index);

                    await Coroutine.Wait(5000, () => Request.IsOpen);
                    await Coroutine.Sleep(1000);
                    item.Handover();
                    await Coroutine.Sleep(200);
                    await Coroutine.Wait(5000, () => Request.HandOverButtonClickable);
                    Request.HandOver();

                    await Coroutine.Sleep(2000);
                }
            }

            return false;
        }

        public async Task<bool> GetToNpc()
        {
            if (WorldManager.ZoneId != ZoneId && WorldManager.ZoneId != 886)
            {
                while (Core.Me.IsCasting)
                {
                    await Coroutine.Sleep(1000);
                }

                if (!ConditionParser.HasAetheryte(AetheryteId))
                {
                    //Logger.Error($"We can't get to {Constants.EntranceZone.CurrentLocaleAethernetName}. You don't have that Aetheryte so do something about it...");
                    //TreeRoot.Stop();
                    return false;
                }

                if (!WorldManager.TeleportById(AetheryteId))
                {
                    //Logger.Error($"We can't get to {Constants.EntranceZone.CurrentLocaleAethernetName}. something is very wrong...");
                    //TreeRoot.Stop();
                    return false;
                }

                while (Core.Me.IsCasting)
                {
                    await Coroutine.Sleep(1000);
                }

                if (CommonBehaviors.IsLoading)
                {
                    await Coroutine.Wait(-1, () => !CommonBehaviors.IsLoading);
                }

                await Coroutine.Wait(10000, () => WorldManager.ZoneId == FoundationZoneId);
                await Coroutine.Sleep(3000);

                await Coroutine.Wait(10000, () => GameObjectManager.GetObjectByNPCId(70) != null);
                await Coroutine.Sleep(3000);

                var unit = GameObjectManager.GetObjectByNPCId(70);
                unit.Target();
                unit.Interact();
                await Coroutine.Sleep(1000);
                await Coroutine.Wait(5000, () => SelectString.IsOpen);
                await Coroutine.Sleep(500);
                if (SelectString.IsOpen)
                    SelectString.ClickSlot(1);

                await Coroutine.Sleep(5000);

                if (CommonBehaviors.IsLoading)
                {
                    await Coroutine.Wait(-1, () => !CommonBehaviors.IsLoading);
                }

                await Coroutine.Sleep(3000);
            }

            /*            if (GameObjectManager.GetObjectByNPCId(NpcId) != null)
                            await CommonTasks.MoveAndStop(
                                new MoveToParameters(GameObjectManager.GetObjectByNPCId(NpcId).Location,
                                    "Moving toward NPC"), 5f, true);*/
            //NpcId = GameObjectManager.GameObjects.First(i => i.EnglishName == EnglishName).NpcId;
            if (Npc.Location.Distance(Core.Me.Location) > 5f)
            {
                var _target = new Vector3(10.58188f, -15.96282f, 163.8702f);
                Navigator.PlayerMover.MoveTowards(_target);
                while (_target.Distance2D(Core.Me.Location) >= 4)
                {
                    Navigator.PlayerMover.MoveTowards(_target);
                    await Coroutine.Sleep(100);
                }

                //await Buddy.Coroutines.Coroutine.Sleep(1500); // (again, probably better to just wait until distance to destination is < 2.0f or something)
                Navigator.PlayerMover.MoveStop();

                _target = Npc.Location;
                Navigator.PlayerMover.MoveTowards(_target);
                while (_target.Distance2D(Core.Me.Location) >= 4)
                {
                    Navigator.PlayerMover.MoveTowards(_target);
                    await Coroutine.Sleep(100);
                }

                //await Buddy.Coroutines.Coroutine.Sleep(1500); // (again, probably better to just wait until distance to destination is < 2.0f or something)
                Navigator.PlayerMover.MoveStop();
            }

            return Npc.Location.Distance(Core.Me.Location) <= 5f;
        }
    }
}