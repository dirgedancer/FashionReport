using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Runtime.InteropServices;
using ValueType = FFXIVClientStructs.FFXIV.Component.GUI.AtkValueType;

namespace FashionReportCalculator;

public unsafe class FCAddon
{
    protected AtkUnitBase* _addon { get; set; } = null;
    public nint addon => (nint)_addon;
    public string? WindowName { get; protected set; }

    public FCAddon(nint AddonPtr) => _addon = (AtkUnitBase*)AddonPtr;
    public FCAddon(AtkUnitBase* AddonPtr) => _addon = AddonPtr;
    public FCAddon(string WinName)
    {
        if (SERVICES.GameGui == null || string.IsNullOrEmpty(WinName))
        {
            LOG.Error($"GameWindow - Constructor: GameGui is null or WindowName is empty. Cannot create GameWindow for '{WinName}'.");
            return;
        }
        _addon = (AtkUnitBase*)SERVICES.GameGui.GetAddonByName(WinName).Address;
        WindowName = WinName;
        if (_addon == null)
            LOG.Error($"GameWindow - Constructor: Addon with name '{WinName}' was not found.");
    }

    public bool IsNull
    {
        get
        {
            if (_addon == null && string.IsNullOrEmpty(WindowName)) return true;
            if (_addon == null && !string.IsNullOrEmpty(WindowName))
                _addon = (AtkUnitBase*)SERVICES.GameGui.GetAddonByName(WindowName).Address;
            return _addon == null;
        }
    }
    public bool IsReady => _addon != null && _addon->IsReady;
    public bool IsVisible { get => _addon != null && _addon->IsVisible; set { if (_addon != null) _addon->IsVisible = value; } }
    public uint AtkCount => _addon->AtkValuesCount;

    public T? TryGetAtkValue<T>(uint index)
    {
        if (_addon == null || !_addon->IsReady || index >= AtkCount) return default;
        ref AtkValue atkValueRef = ref _addon->AtkValues[index];
        try
        {
            switch (atkValueRef.Type)
            {
                case ValueType.String:
                case ValueType.ManagedString:
                case ValueType.ConstString:
                    return typeof(T) == typeof(string)
                        ? (T)(object)atkValueRef.String.ToString()
                        : default;
                case ValueType.WideString:
                    if (atkValueRef.WideString == (char*)IntPtr.Zero) return typeof(T) == typeof(string) ? (T)(object)string.Empty : default;
                    return typeof(T) == typeof(string) ? (T)(object)Marshal.PtrToStringUni((nint)atkValueRef.WideString)! : default;

                case ValueType.Int: return typeof(T) == typeof(int) ? (T)(object)atkValueRef.Int : typeof(T) == typeof(long) ? (T)(object)(long)atkValueRef.Int : default;
                case ValueType.UInt: return typeof(T) == typeof(uint) ? (T)(object)atkValueRef.UInt : typeof(T) == typeof(ulong) ? (T)(object)(ulong)atkValueRef.UInt : default;
                case ValueType.Int64: return typeof(T) == typeof(long) ? (T)(object)atkValueRef.Int64 : default;
                case ValueType.UInt64: return typeof(T) == typeof(ulong) ? (T)(object)atkValueRef.UInt64 : default;
                case ValueType.Float: return typeof(T) == typeof(float) ? (T)(object)atkValueRef.Float : typeof(T) == typeof(double) ? (T)(object)(double)atkValueRef.Float : default;
                case ValueType.Bool: return typeof(T) == typeof(bool) ? (T)(object)atkValueRef.Bool : default;
                case ValueType.Pointer:
                case ValueType.Vector:
                case ValueType.AtkValues: return typeof(T) == typeof(nint) || typeof(T) == typeof(IntPtr) ? (T)(object)(nint)atkValueRef.Pointer : typeof(T) == typeof(string) ? (T)(object)$"0x{(ulong)atkValueRef.Pointer:X}" : default;
                case ValueType.Null: return default;
                default: return default;
            }
        }
        catch (InvalidCastException) { LOG.Error($"GetAtkValue: Internal cast failed for {typeof(T).Name} at {index}."); return default; }
        catch (Exception ex) { LOG.Error($"GetAtkValue: Unexpected error for {typeof(T).Name} at {index}. {ex.Message}"); return default; }
    }
}
