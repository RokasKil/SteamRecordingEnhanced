using System;
using System.Collections.Generic;
using Dalamud.Hooking;
using static Dalamud.Plugin.Services.IGameInteropProvider;

namespace SteamRecordingEnhanced.PluginServices;

public class HookOwner : IDisposable
{
    private readonly List<HookWrapper> hooks = [];

    public virtual void Dispose()
    {
        hooks.ForEach(hook => hook.Dispose());
    }

    protected Hook<T> Hook<T>(string signature, T detour, HookBackend backend = HookBackend.Automatic) where T : Delegate
    {
        var hook = Utility.Services.GameInteropProvider.HookFromSignature(signature, detour, backend);
        if (hook == null) throw new Exception($"Failed to hook '{signature}'");
        hooks.Add(new HookWrapper<T>(hook));
        return hook;
    }

    protected Hook<T> HookFromFunctionPointerVariable<T>(nint address, T detour) where T : Delegate
    {
        var hook = Utility.Services.GameInteropProvider.HookFromFunctionPointerVariable(address, detour);
        if (hook == null) throw new Exception($"Failed to hookFromFunctionPointerVariable '{address:X}'");

        hooks.Add(new HookWrapper<T>(hook));
        return hook;
    }

    protected void EnableHooks()
    {
        hooks.ForEach(hook => hook.Enable());
    }

    protected void DisableHooks()
    {
        hooks.ForEach(hook => hook.Disable());
    }

    protected abstract class HookWrapper : IDisposable
    {
        public abstract void Dispose();
        public abstract void Enable();
        public abstract void Disable();
    }

    private class HookWrapper<T> : HookWrapper where T : Delegate
    {
        public HookWrapper(Hook<T> hook)
        {
            Hook = hook;
        }

        public Hook<T> Hook { get; }


        public override void Enable()
        {
            Hook.Enable();
        }

        public override void Disable()
        {
            Hook.Disable();
        }

        public override void Dispose()
        {
            Hook.Dispose();
        }
    }
}
