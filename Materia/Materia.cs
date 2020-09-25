using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.AClasses;
using ff14bot.Behavior;
using ff14bot.Helpers;
using ff14bot.Managers;
using LlamaLibrary.Extensions;
using LlamaLibrary.Memory;
using LlamaLibrary.Memory.Attributes;
using LlamaLibrary.Properties;
using LlamaLibrary.RemoteAgents;
using LlamaLibrary.RemoteWindows;
using Newtonsoft.Json;
using TreeSharp;

namespace LlamaLibrary.Materia
{
    public class MateriaBase : BotBase
    {
        public static Dictionary<int, List<MateriaItem>> MateriaList;
        private static bool _init;
        private PulseFlags _pulseFlags;
        private Composite _root;
        private MateriaSettingsFrm _settings;
        internal static BagSlot ItemToRemoveMateria;
        internal static BagSlot ItemToAffixMateria;
        internal static List<BagSlot> MateriaToAdd;
        internal static MateriaTask MateriaTask = MateriaTask.None;

        public MateriaBase()
        {
            Task.Factory.StartNew(() =>
            {
                init();
                _init = true;
                Log("INIT DONE");
            });
        }

        public override string Name { get; } = "Materia";

        public override PulseFlags PulseFlags { get; } = PulseFlags.All;

        public override Composite Root => _root;
        public override bool IsAutonomous => true;
        public override bool RequiresProfile => false;
        public override bool WantButton => true;

        public override void OnButtonPress()
        {
            if (_settings == null || _settings.IsDisposed)
                _settings = new MateriaSettingsFrm();
            try
            {
                _settings.Show();
                _settings.Activate();
            }
            catch (ArgumentOutOfRangeException ee)
            {
            }
        }

        public override void Start()
        {
            _root = new ActionRunCoroutine(r => TestTask1());
        }

        private async Task<bool> TestTask1()
        {
            if (!_init)
            {
                Log("Wait for initialization to finish");
                return false;
            }

            if (MateriaTask == MateriaTask.Remove)
            {
                if (ItemToRemoveMateria != null && ItemToRemoveMateria.IsValid)
                    await RemoveMateria(ItemToRemoveMateria);
                else
                {
                    Log("Error: Choose an item in the settings and click Remove Materia");
                }
            }

            if (MateriaTask == MateriaTask.Affix)
            {
                if (ItemToAffixMateria != null && ItemToAffixMateria.IsValid)
                    await AffixMateria(ItemToAffixMateria, MateriaToAdd);
                else
                {
                    Log("Error: Choose an item in the settings and click Affix Materia");
                }
            }

            MateriaTask = MateriaTask.None;

            TreeRoot.Stop("Done playing with Materia");
            return false;
        }

        private static void Log(string text)
        {
            var msg = string.Format("[Materia] " + text);
            Logging.Write(Colors.Fuchsia, msg);
        }

        private void Log1(string text)
        {
            var msg = string.Format("[" + Name + "] " + text);
            Logging.Write(Colors.Aquamarine, msg);
        }

        internal void init()
        {
            OffsetManager.Init();

            Log("Load Materia.json");
            MateriaList = loadResource<Dictionary<int, List<MateriaItem>>>(Resources.Materia);
            Log("Loaded Materia.json");
        }

        private static T loadResource<T>(string text)
        {
            return JsonConvert.DeserializeObject<T>(text);
        }

        public static async Task<bool> AffixMateria(BagSlot bagSlot, List<BagSlot> materiaList)
        {
            Log($"MateriaList count {materiaList.Count}");
            if (bagSlot != null && bagSlot.IsValid)
            {
                Log($"Want to affix Materia to {bagSlot}");

                for (int i = 0; i < materiaList.Count; i++)
                {
                    if (materiaList[i] == null)
                        break;

                    Log($"Want to affix materia {i} {materiaList[i]}");

                    if (!materiaList[i].IsFilled) continue;

                    int count = MateriaCount(bagSlot);

                    while (materiaList[i].IsFilled && (count == MateriaCount(bagSlot)))
                    {
                        if (!MateriaAttach.Instance.IsOpen)
                        {
                            bagSlot.OpenMeldInterface();
                            await Coroutine.Wait(5000, () => MateriaAttach.Instance.IsOpen);

                            if (!MateriaAttach.Instance.IsOpen)
                            {
                                Log($"Can't open meld window");
                                return false;
                            }

                            // MateriaAttach.Instance.ClickItem(0);
                            // await Coroutine.Sleep(1000);
                            MateriaAttach.Instance.ClickMateria(0);
                            await Coroutine.Wait(7000, () => AgentMeld.Instance.Ready);
                            await Coroutine.Wait(5000, () => MateriaAttachDialog.Instance.IsOpen);
                        }

                        if (!MateriaAttachDialog.Instance.IsOpen)
                        {
                            // MateriaAttach.Instance.ClickItem(0);
                            //await Coroutine.Sleep(1000);
                            MateriaAttach.Instance.ClickMateria(0);
                            await Coroutine.Wait(7000, () => AgentMeld.Instance.Ready);
                            await Coroutine.Wait(5000, () => MateriaAttachDialog.Instance.IsOpen);
                            await Coroutine.Wait(7000, () => AgentMeld.Instance.Ready);
                            if (!MateriaAttachDialog.Instance.IsOpen)
                            {
                                Log($"Can't open meld dialog");
                                return false;
                            }
                        }

                        //Log($"{Offsets.AffixMateriaFunc.ToInt64():X}  {Offsets.AffixMateriaParam.ToInt64():X}   {bagSlot.Pointer.ToInt64():X}  {materiaList[i].Pointer.ToInt64():X}");
                        Log("Wait Ready");
                        await Coroutine.Wait(7000, () => AgentMeld.Instance.Ready);
                        Log("Wait CanMeld");
                        await Coroutine.Wait(7000, () => AgentMeld.Instance.CanMeld);
                        bagSlot.AffixMateria(materiaList[i]);
                        Log("Clicked affix wait not Ready");
                        await Coroutine.Wait(7000, () => AgentMeld.Instance.Ready);
                        Log("Clicked affix wait Ready");
                        await Coroutine.Wait(7000, () => !AgentMeld.Instance.Ready);
                        // await Coroutine.Sleep(7000);
                        Log("Clicked wait window");
                        await Coroutine.Wait(7000, () => !MateriaAttachDialog.Instance.IsOpen);
                        Log("Wait 2 windows");
                        await Coroutine.Wait(5000, () => MateriaAttachDialog.Instance.IsOpen || MateriaAttach.Instance.IsOpen);
                        //    await Coroutine.Sleep(1000);


                        while (MateriaAttachDialog.Instance.IsOpen)
                        {
                            Log("While window");
                            MateriaAttachDialog.Instance.ClickAttach();
                            await Coroutine.Wait(7000, () => !AgentMeld.Instance.CanMeld);
                            await Coroutine.Wait(7000, () => AgentMeld.Instance.CanMeld);
                            //await Coroutine.Wait(7000, () => !MateriaAttachDialog.Instance.IsOpen);
                            await Coroutine.Wait(7000, () => MateriaAttachDialog.Instance.IsOpen || MateriaAttach.Instance.IsOpen);
                        }


                        if (MateriaAttach.Instance.IsOpen)
                        {
                            Log("Closing window");
                            MateriaAttach.Instance.Close();
                            await Coroutine.Wait(7000, () => !MateriaAttach.Instance.IsOpen);
                            //await Coroutine.Wait(7000, () => !AgentMeld.Instance.Ready);
                            //await Coroutine.Sleep(1000);
                        }
                    }


                    if (!materiaList[i].IsFilled)
                        return false;
                }
            }

            return true;
        }

        public static async Task<bool> RemoveMateria(BagSlot bagSlot)
        {
            if (bagSlot != null && bagSlot.IsValid)
            {
                Log($"Want to remove Materia from {bagSlot}");
                int count = MateriaCount(bagSlot);
                for (int i = 0; i < count; i++)
                {
                    Log($"Removing materia {count - i}");
                    bagSlot.RemoveMateria();
                    await Coroutine.Sleep(6000);
                }
            }

            Log($"Materia now has {MateriaCount(ItemToRemoveMateria)}");

            return true;
        }

        public static List<MateriaItem> Materia(BagSlot bagSlot)
        {
            var materiaType = Core.Memory.ReadArray<ushort>(bagSlot.Pointer + 0x20, 5);
            var materiaLevel = Core.Memory.ReadArray<byte>(bagSlot.Pointer + 0x2A, 5);
            var materia = new List<MateriaItem>();

            for (var i = 0; i < 5; i++)
            {
                if (materiaType[i] > 0)
                    materia.Add(MateriaList[materiaType[i]].First(j => j.Tier == materiaLevel[i]));
            }

            return materia;
        }

        public static bool HasMateria(BagSlot bagSlot)
        {
            var materiaType = Core.Memory.ReadArray<ushort>(bagSlot.Pointer + 0x20, 5);
            for (var i = 0; i < 5; i++)
            {
                if (materiaType[i] > 0) return true;
            }

            return false;
        }

        public static int MateriaCount(BagSlot bagSlot)
        {
            var materiaType = Core.Memory.ReadArray<ushort>(bagSlot.Pointer + 0x20, 5);
            int count = 0;
            for (var i = 0; i < 5; i++)
            {
                if (materiaType[i] > 0) count++;
            }

            return count;
        }
    }

    public enum MateriaTask
    {
        None,
        Remove,
        Affix
    }
}