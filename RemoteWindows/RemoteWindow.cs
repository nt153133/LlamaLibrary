using System;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using ff14bot.RemoteWindows;

namespace LlamaLibrary.RemoteWindows
{
    public class RemoteWindow<T> : RemoteWindow where T : RemoteWindow<T>, new()
    {
        private static T _instance;
        public static T Instance => _instance ?? (_instance = new T());

        protected RemoteWindow(string name) : base(name)
        {
            // _instance = new T();
        }
        
        protected RemoteWindow(string name, AgentInterface agent) : base(name, agent)
        {
            // _instance = new T();
        }
    }

    public abstract class RemoteWindow
    {
        internal string _name;
        internal AgentInterface _agent;
        private const int Offset0 = 0x1CA;
        private const int Offset2 = 0x160;

        public virtual bool IsOpen => WindowByName != null;

        public virtual string Name => _name;
        
        public virtual AgentInterface Agent => _agent;

        public virtual AtkAddonControl WindowByName => RaptureAtkUnitManager.GetWindowByName(Name);

        protected bool HasAgentInterfaceId => GetAgentInterfaceId() != 0;

        protected RemoteWindow(string name)
        {
            _name = name;
        }

        protected RemoteWindow(string name, AgentInterface agent)
        {
            _agent = agent;
            _name = name;
        }

        public virtual void Close()
        {
            if (IsOpen)
                SendAction(1, 3uL, 0xFFFFFFFFuL);
        }

        public int GetAgentInterfaceId()
        {
            if (WindowByName == null)
                return 0;

            var test = WindowByName.TryFindAgentInterface();

            return test == null ? 0 : test.Id;
        }

        public async Task<bool> WaitTillWindowOpen(int maxTimeOut = 5000)
        {
            await Coroutine.Wait(maxTimeOut, () => IsOpen);
            return IsOpen;
        }

        protected TwoInt[] ___Elements()
        {
            if (WindowByName == null) return null;
            var elementCount = ElementCount();
            var addr = Core.Memory.Read<IntPtr>(WindowByName.Pointer + Offset2);
            return Core.Memory.ReadArray<TwoInt>(addr, elementCount);
        }

        protected ushort ElementCount()
        {
            return WindowByName != null ? Core.Memory.Read<ushort>(WindowByName.Pointer + Offset0) : (ushort)0;
        }

        protected void SendAction(int pairCount, params ulong[] param)
        {
            if (IsOpen)
                WindowByName.SendAction(pairCount, param);
        }
        
        public virtual async Task<bool> Open()
        {
            if (IsOpen) return true;
            
            Agent.Toggle();
            return await WaitTillWindowOpen(5000);
            //return SyncRoutines.WaitUntil(() => IsOpen, 50, 5000, true);
        }
    }
}