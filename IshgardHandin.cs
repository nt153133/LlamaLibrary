using System.CodeDom;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Buddy.Coroutines;
using Clio.Utilities;
using ff14bot;
using ff14bot.Behavior;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.Navigation;
using ff14bot.NeoProfiles;
using ff14bot.Objects;
using ff14bot.RemoteWindows;
using LlamaLibrary.Helpers;
using LlamaLibrary.RemoteWindows;

namespace LlamaLibrary
{
    public class IshgardHandin
    {
        public uint AetheryteId = 70;
        public string EnglishName = "Potkin";

        public uint FoundationZoneId;

        //public uint NpcId = 1031690;
        public uint[] ids = {1031690, 1031677};
        public uint ZoneId;

        private GameObject Npc => GameObjectManager.GameObjects.FirstOrDefault(i => ids.Contains(i.NpcId) && i.IsVisible);
        private GameObject VendorNpc => GameObjectManager.GameObjects.FirstOrDefault(i => i.NpcId == 1031680 && i.IsVisible);
        
        private GameObject GatherNpc => GameObjectManager.GameObjects.FirstOrDefault(i => i.NpcId == 1031693 && i.IsVisible);
        
        private GameObject KupoNpc => GameObjectManager.GameObjects.FirstOrDefault(i => i.NpcId == 1031692 && i.IsVisible);

        public async Task<bool> HandInGatheringItem(int job)
        {
            if ((!HWDGathereInspect.Instance.IsOpen && GatherNpc == null) || GatherNpc.Location.Distance(Core.Me.Location) > 5f) 
                await Navigation.GetTo(886,new Vector3(-20.04274f, -16f, 141.3337f));
            
            if (!HWDGathereInspect.Instance.IsOpen && GatherNpc.Location.Distance(Core.Me.Location) > 4f)
            {
                await Navigation.OffMeshMove(GatherNpc.Location);
                await Coroutine.Sleep(500);
            }

            if (!HWDGathereInspect.Instance.IsOpen)
            {
                GatherNpc.Interact();
                await Coroutine.Wait(5000, () => HWDGathereInspect.Instance.IsOpen || Talk.DialogOpen);
                await Coroutine.Sleep(100);

                while (Talk.DialogOpen)
                {
                    Talk.Next();
                    await Coroutine.Wait(5000, () => !Talk.DialogOpen);
                }

                await Coroutine.Wait(5000, () => HWDGathereInspect.Instance.IsOpen);
            }

            if (HWDGathereInspect.Instance.IsOpen)
            {
                HWDGathereInspect.Instance.ClickClass(job);
                await Coroutine.Sleep(500);
                
                if(HWDGathereInspect.Instance.CanAutoSubmit());
                {
                   HWDGathereInspect.Instance.ClickAutoSubmit();
                   await Coroutine.Wait(6000, () => HWDGathereInspect.Instance.CanRequestInspection());
                   if (HWDGathereInspect.Instance.CanRequestInspection())
                   {
                       HWDGathereInspect.Instance.ClickRequestInspection();
                       if (ScriptConditions.Helpers.GetSkybuilderScrips() > 9000 )
                           await Coroutine.Wait(2000, () => SelectYesno.IsOpen);
                       else
                       {
                           await Coroutine.Sleep(100);
                       }
                       if (SelectYesno.IsOpen)
                       {
                           SelectYesno.Yes();
                       }
                   }
                }
            }

            return false;
        }
        
        public async Task<bool> HandInKupoTicket(int slot)
        {
            if ((!HWDLottery.Instance.IsOpen && KupoNpc == null) || KupoNpc.Location.Distance(Core.Me.Location) > 5f) 
                await Navigation.GetTo(886,new Vector3(43.59162f, -16f, 170.3864f));

            if (!HWDLottery.Instance.IsOpen && KupoNpc.Location.Distance(Core.Me.Location) > 4f)
            {
                await Navigation.OffMeshMove(KupoNpc.Location);
                await Coroutine.Sleep(500);
            }
            
            if (!HWDLottery.Instance.IsOpen && KupoNpc != null)
            {
                KupoNpc.Interact();
                Log("Interact with npc");
                await Coroutine.Wait(5000, () => HWDLottery.Instance.IsOpen || Talk.DialogOpen);
                await Coroutine.Sleep(100);

                while (Talk.DialogOpen)
                {
                    Talk.Next();
                    await Coroutine.Wait(5000, () => !Talk.DialogOpen);
                }
                Log("Talking done");
                await Coroutine.Wait(2000, () => SelectYesno.IsOpen);

                if (SelectYesno.IsOpen)
                {
                    SelectYesno.Yes();
                    Log("Select Yes/No open");
                    await Coroutine.Wait(5000, () => HWDLottery.Instance.IsOpen);
                    await Coroutine.Sleep(4000);
                    Log("Ticket Should be loaded");
                }
            }

            if (HWDLottery.Instance.IsOpen)
            {
                Log("Clicking");
                await HWDLottery.Instance.ClickSpot(slot);
                await Coroutine.Sleep(1000);
                Log("Closeing");
                HWDLottery.Instance.Close();
                await Coroutine.Wait(2000, () => !HWDLottery.Instance.IsOpen);
                if (HWDLottery.Instance.IsOpen)
                {
                    Log("Closeing Again");
                    HWDLottery.Instance.Close();
                }

                
                
                await Coroutine.Wait(5000, () => SelectYesno.IsOpen || Talk.DialogOpen);
                Log($"Select Yes/No {SelectYesno.IsOpen} Talk {Talk.DialogOpen}");
                while (Talk.DialogOpen)
                {
                    Talk.Next();
                    await Coroutine.Wait(2000, () => !Talk.DialogOpen);
                    await Coroutine.Wait(2000, () => Talk.DialogOpen || SelectYesno.IsOpen);
                }

                await Coroutine.Sleep(1000);

                await HandInKupoTicket(slot);

            }
            else
            {
                Log("Out of Tickets");
            }
            Log("Done with Kupo Tickets");
            return false;
        }
        public async Task<bool> HandInItem(uint itemId, int index, int job)
        {
            if ((!HWDSupply.Instance.IsOpen && Npc == null) || Npc.Location.Distance(Core.Me.Location) > 5f) 
                await Navigation.GetTo(886,new Vector3(43.59162f, -16f, 170.3864f));

            if (!HWDSupply.Instance.IsOpen && Npc.Location.Distance(Core.Me.Location) > 4f)
            {
                await Navigation.OffMeshMove(Npc.Location);
                await Coroutine.Sleep(500);
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
                    await Coroutine.Sleep(700);
                    item.Handover();
                    await Coroutine.Sleep(100);
                    await Coroutine.Wait(5000, () => Request.HandOverButtonClickable);
                    Request.HandOver();
                    
                    if (ScriptConditions.Helpers.GetSkybuilderScrips() > 9000 )
                        await Coroutine.Wait(2000, () => SelectYesno.IsOpen);
                    else
                    {
                        await Coroutine.Sleep(100);
                    }

                    if (Translator.Language != Language.Chn)
                    {
                        Log($"Kupo Tickets: {HWDSupply.Instance.NumberOfKupoTickets()}");

                        if (HWDSupply.Instance.NumberOfKupoTickets() >= 9)
                        {
                            Log($"Going to turn in Kupo Tickets: {HWDSupply.Instance.NumberOfKupoTickets()}");
                            if (SelectYesno.IsOpen)
                            {
                                SelectYesno.Yes();
                                await Coroutine.Sleep(1000);
                            }

                            HWDSupply.Instance.Close();
                            await Coroutine.Sleep(2000);
                            await HandInKupoTicket(1);
                            break;

                        }
                    }

                    if (!SelectYesno.IsOpen)
                    {
                        continue;
                    }

                    SelectYesno.Yes();
                    await Coroutine.Sleep(2000);
                }
            }

            if (Request.IsOpen)
            {
                Request.Cancel();
                await Coroutine.Sleep(2000);
            }
            
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == itemId))
                await HandInItem(itemId, index, job);
            return false;
        }

        public async Task<bool> BuyItem(uint itemId, int SelectStringLine = 0)
        {
            
            if ((!ShopExchangeCurrency.Open && VendorNpc == null) || VendorNpc.Location.Distance(Core.Me.Location) > 5f) 
                await Navigation.GetTo(886,new Vector3(36.33978f, -16f, 145.3877f));

            if (!ShopExchangeCurrency.Open && VendorNpc.Location.Distance(Core.Me.Location) > 4f)
            {
                await Navigation.OffMeshMove(VendorNpc.Location);
                await Coroutine.Sleep(500);
            }

            if (!ShopExchangeCurrency.Open)
            {
                VendorNpc.Interact();
                await Coroutine.Wait(5000, () => ShopExchangeCurrency.Open || Talk.DialogOpen || Conversation.IsOpen);
                if (Conversation.IsOpen)
                {
                    Conversation.SelectLine((uint) SelectStringLine);
                    await Coroutine.Wait(5000, () => ShopExchangeCurrency.Open);
                }
            }

            if (ShopExchangeCurrency.Open)
            {
                var items = SpecialShopManager.Items;
                var specialShopItem = items?.Cast<SpecialShopItem?>().FirstOrDefault(i => i.HasValue && i.Value.ItemIds.Contains(itemId));

                if (!specialShopItem.HasValue) return false;

                var count = CanAffordScrip(specialShopItem.Value);

                if (count > 0) Purchase(itemId, count);

                await Coroutine.Wait(5000, () => SelectYesno.IsOpen);

                SelectYesno.ClickYes();

                await Coroutine.Sleep(1000);

                ShopExchangeCurrency.Close();

                return true;
            }

            return false;
        }

        internal static uint Purchase(uint itemId, uint itemCount)
        {
            var windowByName = RaptureAtkUnitManager.GetWindowByName("ShopExchangeCurrency");
            if (windowByName == null) return 0u;

            var items = SpecialShopManager.Items;

            var specialShopItem = items?.Cast<SpecialShopItem?>().FirstOrDefault(i => i.HasValue && i.Value.ItemIds.Contains(itemId));

            if (!specialShopItem.HasValue) return 0u;

            if (itemCount > specialShopItem.Value.Item0.StackSize) itemCount = specialShopItem.Value.Item0.StackSize;

            var count = CanAffordScrip(specialShopItem.Value);

            if (itemCount > count) itemCount = count;
            var index = items.IndexOf(specialShopItem.Value);
            var obj = new ulong[8]
            {
                3uL,
                0uL,
                3uL,
                0uL,
                3uL,
                0uL,
                0uL,
                0uL
            };
            obj[3] = (uint) index;
            obj[5] = itemCount;
            windowByName.SendAction(4, obj);
            return itemCount;
        }

        private static uint CanAffordScrip(SpecialShopItem item)
        {
            var scrips = SpecialCurrencyManager.GetCurrencyCount((SpecialCurrency) 28063);
            if (scrips == 0) return 0u;
            return scrips / item.CurrencyCosts[0];
        }

        public async Task<bool> GetToVendorNpc()
        {
            if (WorldManager.ZoneId != ZoneId && WorldManager.ZoneId != 886)
            {
                while (Core.Me.IsCasting)
                {
                    await Coroutine.Sleep(1000);
                }

                if (!ConditionParser.HasAetheryte(AetheryteId))
                {
                    Logger.LogCritical("We can't get to Foundation. You don't have that Aetheryte so do something about it...");
                    TreeRoot.Stop();
                    return false;
                }

                if (!WorldManager.TeleportById(AetheryteId))
                {
                    Logger.LogCritical($"We can't get to {AetheryteId}. something is very wrong...");
                    TreeRoot.Stop();
                    return false;
                }

                while (Core.Me.IsCasting)
                {
                    await Coroutine.Sleep(1000);
                }

                if (CommonBehaviors.IsLoading) await Coroutine.Wait(-1, () => !CommonBehaviors.IsLoading);

                await Coroutine.Wait(10000, () => WorldManager.ZoneId == FoundationZoneId);
                await Coroutine.Sleep(3000);

                await Coroutine.Wait(10000, () => GameObjectManager.GetObjectByNPCId(70) != null);
                await Coroutine.Sleep(3000);

                var unit = GameObjectManager.GetObjectByNPCId(70);

                if (!unit.IsWithinInteractRange)
                {
                    var _target = unit.Location;
                    Navigator.PlayerMover.MoveTowards(_target);
                    while (!unit.IsWithinInteractRange)
                    {
                        Navigator.PlayerMover.MoveTowards(_target);
                        await Coroutine.Sleep(100);
                    }

                    Navigator.PlayerMover.MoveStop();
                }

                unit.Target();
                unit.Interact();
                await Coroutine.Sleep(1000);
                await Coroutine.Wait(5000, () => SelectString.IsOpen);
                await Coroutine.Sleep(500);
                if (SelectString.IsOpen)
                    SelectString.ClickSlot(1);

                await Coroutine.Sleep(5000);

                if (CommonBehaviors.IsLoading) await Coroutine.Wait(-1, () => !CommonBehaviors.IsLoading);

                await Coroutine.Sleep(3000);
            }

            if (!(VendorNpc.Location.Distance(Core.Me.Location) > 5f)) return Npc.Location.Distance(Core.Me.Location) <= 5f;

            var target = new Vector3(10.58188f, -15.96282f, 163.8702f);
            Navigator.PlayerMover.MoveTowards(target);
            while (target.Distance2D(Core.Me.Location) >= 4)
            {
                Navigator.PlayerMover.MoveTowards(target);
                await Coroutine.Sleep(100);
            }

            Navigator.PlayerMover.MoveStop();

            
            target = VendorNpc.Location;
            Navigator.PlayerMover.MoveTowards(target);
            while (target.Distance2D(Core.Me.Location) >= 4)
            {
                Navigator.PlayerMover.MoveTowards(target);
                await Coroutine.Sleep(100);
            }

            //await CommonTasks.MoveTo(VendorNpc.Location, "Moving To HandinVendor");

            // await CommonTasks.MoveAndStop(
            //      new MoveToParameters(VendorNpc.Location, "Moving To HandinVendor"), 2f);

            Navigator.PlayerMover.MoveStop();

            return Npc.Location.Distance(Core.Me.Location) <= 5f;
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
                    //Logger.Error($"We can't get to {Constants.EntranceZone.CurrentLocaleAethernetName}. You don't have that Aetheryte so do something about it...");
                    //TreeRoot.Stop();
                    return false;

                if (!WorldManager.TeleportById(AetheryteId))
                    //Logger.Error($"We can't get to {Constants.EntranceZone.CurrentLocaleAethernetName}. something is very wrong...");
                    //TreeRoot.Stop();
                    return false;

                while (Core.Me.IsCasting)
                {
                    await Coroutine.Sleep(1000);
                }

                if (CommonBehaviors.IsLoading) await Coroutine.Wait(-1, () => !CommonBehaviors.IsLoading);

                await Coroutine.Wait(10000, () => WorldManager.ZoneId == FoundationZoneId);
                await Coroutine.Sleep(3000);

                await Coroutine.Wait(10000, () => GameObjectManager.GetObjectByNPCId(70) != null);
                await Coroutine.Sleep(3000);

                var unit = GameObjectManager.GetObjectByNPCId(70);

                if (!unit.IsWithinInteractRange)
                {
                    var _target = unit.Location;
                    Navigator.PlayerMover.MoveTowards(_target);
                    while (!unit.IsWithinInteractRange)
                    {
                        Navigator.PlayerMover.MoveTowards(_target);
                        await Coroutine.Sleep(100);
                    }

                    Navigator.PlayerMover.MoveStop();
                }

                unit.Target();
                unit.Interact();
                await Coroutine.Sleep(1000);
                await Coroutine.Wait(5000, () => SelectString.IsOpen);
                await Coroutine.Sleep(500);
                if (SelectString.IsOpen)
                    SelectString.ClickSlot(1);

                await Coroutine.Sleep(5000);

                if (CommonBehaviors.IsLoading) await Coroutine.Wait(-1, () => !CommonBehaviors.IsLoading);

                await Coroutine.Sleep(3000);
            }

            //await CommonTasks.MoveTo(Npc.Location, "Moving To HandinVendor");


            if (Npc.Location.Distance(Core.Me.Location) > 5f)
            {
                var _target = new Vector3(10.58188f, -15.96282f, 163.8702f);
                Navigator.PlayerMover.MoveTowards(_target);
                while (_target.Distance2D(Core.Me.Location) >= 4)
                {
                    Navigator.PlayerMover.MoveTowards(_target);
                    await Coroutine.Sleep(100);
                }

                Navigator.PlayerMover.MoveStop();

                _target = Npc.Location;
                Navigator.PlayerMover.MoveTowards(_target);
                while (_target.Distance2D(Core.Me.Location) >= 4)
                {
                    Navigator.PlayerMover.MoveTowards(_target);
                    await Coroutine.Sleep(100);
                }

                Navigator.PlayerMover.MoveStop();
            }

            return Npc.Location.Distance(Core.Me.Location) <= 5f;
        }
        
        public async Task<bool> GetToGatherNpc()
        {
            if (WorldManager.ZoneId != ZoneId && WorldManager.ZoneId != 886)
            {
                while (Core.Me.IsCasting)
                {
                    await Coroutine.Sleep(1000);
                }

                if (!ConditionParser.HasAetheryte(AetheryteId))
                    //Logger.Error($"We can't get to {Constants.EntranceZone.CurrentLocaleAethernetName}. You don't have that Aetheryte so do something about it...");
                    //TreeRoot.Stop();
                    return false;

                if (!WorldManager.TeleportById(AetheryteId))
                    //Logger.Error($"We can't get to {Constants.EntranceZone.CurrentLocaleAethernetName}. something is very wrong...");
                    //TreeRoot.Stop();
                    return false;

                while (Core.Me.IsCasting)
                {
                    await Coroutine.Sleep(1000);
                }

                if (CommonBehaviors.IsLoading) await Coroutine.Wait(-1, () => !CommonBehaviors.IsLoading);

                await Coroutine.Wait(10000, () => WorldManager.ZoneId == FoundationZoneId);
                await Coroutine.Sleep(3000);

                await Coroutine.Wait(10000, () => GameObjectManager.GetObjectByNPCId(70) != null);
                await Coroutine.Sleep(3000);

                var unit = GameObjectManager.GetObjectByNPCId(70);

                if (!unit.IsWithinInteractRange)
                {
                    var _target = unit.Location;
                    Navigator.PlayerMover.MoveTowards(_target);
                    while (!unit.IsWithinInteractRange)
                    {
                        Navigator.PlayerMover.MoveTowards(_target);
                        await Coroutine.Sleep(100);
                    }

                    Navigator.PlayerMover.MoveStop();
                }

                unit.Target();
                unit.Interact();
                await Coroutine.Sleep(1000);
                await Coroutine.Wait(5000, () => SelectString.IsOpen);
                await Coroutine.Sleep(500);
                if (SelectString.IsOpen)
                    SelectString.ClickSlot(1);

                await Coroutine.Sleep(5000);

                if (CommonBehaviors.IsLoading) await Coroutine.Wait(-1, () => !CommonBehaviors.IsLoading);

                await Coroutine.Sleep(3000);
            }

            //await CommonTasks.MoveTo(Npc.Location, "Moving To HandinVendor");


            if (GatherNpc.Location.Distance(Core.Me.Location) > 5f)
            {
                var _target = new Vector3(-21.62485f, -16f, 141.3661f);
                Navigator.PlayerMover.MoveTowards(_target);
                while (_target.Distance2D(Core.Me.Location) >= 4)
                {
                    Navigator.PlayerMover.MoveTowards(_target);
                    await Coroutine.Sleep(100);
                }

                Navigator.PlayerMover.MoveStop();

                _target = GatherNpc.Location;
                Navigator.PlayerMover.MoveTowards(_target);
                while (_target.Distance2D(Core.Me.Location) >= 4)
                {
                    Navigator.PlayerMover.MoveTowards(_target);
                    await Coroutine.Sleep(100);
                }

                Navigator.PlayerMover.MoveStop();
            }

            return Npc.Location.Distance(Core.Me.Location) <= 5f;
        }
        
        private static void Log(string text, params object[] args)
        {
            var msg = string.Format("[Ishgard Handin] " + text, args);
            Logging.Write(Colors.Aquamarine, msg);
        }
    }
}
