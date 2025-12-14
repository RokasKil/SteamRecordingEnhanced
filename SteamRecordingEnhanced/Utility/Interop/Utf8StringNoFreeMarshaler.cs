using System;
using System.Runtime.InteropServices;

namespace SteamRecordingEnhanced.Interop;

public class Utf8StringNoFreeMarshaler : ICustomMarshaler
{
    private static Utf8StringNoFreeMarshaler? Instance;

    public static ICustomMarshaler GetInstance(string cookie)
    {
        return Instance ??= new Utf8StringNoFreeMarshaler();
    }

    public object MarshalNativeToManaged(IntPtr pNativeData)
    {
        if (pNativeData == IntPtr.Zero)
            return string.Empty;

        return Marshal.PtrToStringUTF8(pNativeData) ?? string.Empty;
    }

    public IntPtr MarshalManagedToNative(object managedObj)
    {
        throw new NotImplementedException();
    }

    // Don't free data we don't own
    public void CleanUpNativeData(IntPtr pNativeData) { }

    public void CleanUpManagedData(object managedObj) { }

    public int GetNativeDataSize()
    {
        return IntPtr.Size;
    }
}
