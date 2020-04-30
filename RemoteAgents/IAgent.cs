using System;

namespace LlamaLibrary.RemoteAgents
{
    public interface IAgent
    {
        IntPtr RegisteredVtable { get; }
    }
}