using Dalamud.Plugin.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.Game;
using Dalamud.Game.Inventory;
using Dalamud.Game.Inventory.InventoryEventArgTypes;
#pragma warning disable IDE1006


namespace FashionReport;

public static class EquippedGearService
{
    public struct EquippedItemData
    {
        public InventoryItem Item;
        public string Name;
        public string Stain1;
        public string Stain2;
        public uint StainId1;
        public uint StainId2;
        public uint StainIcon1;
        public uint StainIcon2;
    }

    internal static Dictionary<string, EquippedItemData> CurrentEquippedGear = new();

    public static InventoryItem Weapon { get; private set; }
    public static string WeaponName => _weaponName;
    private static string _weaponName = string.Empty;
    public static unsafe InventoryItem* WeaponPtr => InventoryManager.Instance()->GetInventorySlot(InventoryType.EquippedItems, 0);

    public static InventoryItem Secondary { get; private set; }
    public static string SecondaryName => _secondaryName;
    private static string _secondaryName = string.Empty;
    public static unsafe InventoryItem* SecondaryPtr => InventoryManager.Instance()->GetInventorySlot(InventoryType.EquippedItems, 1);

    public static InventoryItem Head { get; private set; }
    public static string HeadName => _headName;
    private static string _headName = string.Empty;
    public static unsafe InventoryItem* HeadPtr => InventoryManager.Instance()->GetInventorySlot(InventoryType.EquippedItems, 2);

    public static InventoryItem Body { get; private set; }
    public static string BodyName => _bodyName;
    private static string _bodyName = string.Empty;
    public static unsafe InventoryItem* BodyPtr => InventoryManager.Instance()->GetInventorySlot(InventoryType.EquippedItems, 3);

    public static InventoryItem Gloves { get; private set; }
    public static string GlovesName => _glovesName;
    private static string _glovesName = string.Empty;
    public static unsafe InventoryItem* GlovesPtr => InventoryManager.Instance()->GetInventorySlot(InventoryType.EquippedItems, 4);

    public static InventoryItem Legs { get; private set; }
    public static string LegsName => _legsName;
    private static string _legsName = string.Empty;
    public static unsafe InventoryItem* LegsPtr => InventoryManager.Instance()->GetInventorySlot(InventoryType.EquippedItems, 6);

    public static InventoryItem Boots { get; private set; }
    public static string BootsName => _bootsName;
    private static string _bootsName = string.Empty;
    public static unsafe InventoryItem* BootsPtr => InventoryManager.Instance()->GetInventorySlot(InventoryType.EquippedItems, 7);

    public static InventoryItem Earrings { get; private set; }
    public static string EarringsName => _earringsName;
    private static string _earringsName = string.Empty;
    public static unsafe InventoryItem* EarringsPtr => InventoryManager.Instance()->GetInventorySlot(InventoryType.EquippedItems, 8);

    public static InventoryItem Necklace { get; private set; }
    public static string NecklaceName => _necklaceName;
    private static string _necklaceName = string.Empty;
    public static unsafe InventoryItem* NecklacePtr => InventoryManager.Instance()->GetInventorySlot(InventoryType.EquippedItems, 9);

    public static InventoryItem Bracelet { get; private set; }
    public static string BraceletName => _braceletName;
    private static string _braceletName = string.Empty;
    public static unsafe InventoryItem* BraceletPtr => InventoryManager.Instance()->GetInventorySlot(InventoryType.EquippedItems, 10);

    public static InventoryItem RightRing { get; private set; }
    public static string RightRingName => _rightRingName;
    private static string _rightRingName = string.Empty;
    public static unsafe InventoryItem* RightRingPtr => InventoryManager.Instance()->GetInventorySlot(InventoryType.EquippedItems, 11);

    public static InventoryItem LeftRing { get; private set; }
    public static string LeftRingName => _leftRingName;
    private static string _leftRingName = string.Empty;
    public static unsafe InventoryItem* LeftRingPtr => InventoryManager.Instance()->GetInventorySlot(InventoryType.EquippedItems, 12);

    public static void Initialize()
    {
        SERVICES.GameInventory.InventoryChanged += OnInventoryChanged;
        UpdateEquippedGear();
    }

    private static void OnInventoryChanged(IReadOnlyCollection<InventoryEventArgs> events)
    {
        if (events.Any(e => e.ToString().Contains("EquippedItems")))
            UpdateEquippedGear();
    }

    private unsafe static InventoryItem SafeRead(InventoryItem* ptr) => ptr != null && ptr->ItemId > 0 ? *ptr : new InventoryItem();

    private unsafe static void UpdateEquippedGear()
    {
        Weapon = SafeRead(WeaponPtr);
        Secondary = SafeRead(SecondaryPtr);
        Head = SafeRead(HeadPtr);
        Body = SafeRead(BodyPtr);
        Gloves = SafeRead(GlovesPtr);
        Legs = SafeRead(LegsPtr);
        Boots = SafeRead(BootsPtr);
        Earrings = SafeRead(EarringsPtr);
        Necklace = SafeRead(NecklacePtr);
        Bracelet = SafeRead(BraceletPtr);
        RightRing = SafeRead(RightRingPtr);
        LeftRing = SafeRead(LeftRingPtr);

        _weaponName = Weapon.ItemId > 0 ? SERVICES.AllItems.FirstOrDefault(x => x.RowId == Weapon.ItemId).Name.ToString() ?? string.Empty : string.Empty;
        _secondaryName = Secondary.ItemId > 0 ? SERVICES.AllItems.FirstOrDefault(x => x.RowId == Secondary.ItemId).Name.ToString() ?? string.Empty : string.Empty;
        _headName = Head.ItemId > 0 ? SERVICES.AllItems.FirstOrDefault(x => x.RowId == Head.ItemId).Name.ToString() ?? string.Empty : string.Empty;
        _bodyName = Body.ItemId > 0 ? SERVICES.AllItems.FirstOrDefault(x => x.RowId == Body.ItemId).Name.ToString() ?? string.Empty : string.Empty;
        _glovesName = Gloves.ItemId > 0 ? SERVICES.AllItems.FirstOrDefault(x => x.RowId == Gloves.ItemId).Name.ToString() ?? string.Empty : string.Empty;
        _legsName = Legs.ItemId > 0 ? SERVICES.AllItems.FirstOrDefault(x => x.RowId == Legs.ItemId).Name.ToString() ?? string.Empty : string.Empty;
        _bootsName = Boots.ItemId > 0 ? SERVICES.AllItems.FirstOrDefault(x => x.RowId == Boots.ItemId).Name.ToString() ?? string.Empty : string.Empty;
        _earringsName = Earrings.ItemId > 0 ? SERVICES.AllItems.FirstOrDefault(x => x.RowId == Earrings.ItemId).Name.ToString() ?? string.Empty : string.Empty;
        _necklaceName = Necklace.ItemId > 0 ? SERVICES.AllItems.FirstOrDefault(x => x.RowId == Necklace.ItemId).Name.ToString() ?? string.Empty : string.Empty;
        _braceletName = Bracelet.ItemId > 0 ? SERVICES.AllItems.FirstOrDefault(x => x.RowId == Bracelet.ItemId).Name.ToString() ?? string.Empty : string.Empty;
        _rightRingName = RightRing.ItemId > 0 ? SERVICES.AllItems.FirstOrDefault(x => x.RowId == RightRing.ItemId).Name.ToString() ?? string.Empty : string.Empty;
        _leftRingName = LeftRing.ItemId > 0 ? SERVICES.AllItems.FirstOrDefault(x => x.RowId == LeftRing.ItemId).Name.ToString() ?? string.Empty : string.Empty;

        CurrentEquippedGear.Clear();

        InventoryItem*[] ptrs = { WeaponPtr, HeadPtr, BodyPtr, GlovesPtr, LegsPtr, BootsPtr, EarringsPtr, NecklacePtr, BraceletPtr, RightRingPtr, LeftRingPtr };
        string[] keys = { "Weapon", "Head", "Body", "Gloves", "Legs", "Boots", "Earrings", "Necklace", "Bracelet", "RightRing", "LeftRing" };

        for (int i = 0; i < keys.Length; i++)
        {
            InventoryItem item = SafeRead(ptrs[i]);
            CurrentEquippedGear[keys[i]] = CreateEquippedData(item);
        }

    }

    private static EquippedItemData CreateEquippedData(InventoryItem item)
    {
        uint stain1 = item.GetStain(0);
        uint stain2 = item.GetStain(1);

        uint icon1 = 0;
        uint icon2 = 0;

        if (stain1 > 0 && SERVICES.StainTable.TryGetValue(stain1, out var stainData1))
            icon1 = stainData1.IconId;

        if (stain2 > 0 && SERVICES.StainTable.TryGetValue(stain2, out var stainData2))
            icon2 = stainData2.IconId;

        return new EquippedItemData
        {
            Item = item,
            Name = item.ItemId > 0 ? SERVICES.AllItems.FirstOrDefault(x => x.RowId == item.ItemId).Name.ToString() ?? string.Empty : string.Empty,
            Stain1 = stain1 > 0 ? SERVICES.AllStains.FirstOrDefault(x => x.RowId == stain1).Name.ToString() ?? string.Empty : string.Empty,
            Stain2 = stain2 > 0 ? SERVICES.AllStains.FirstOrDefault(x => x.RowId == stain2).Name.ToString() ?? string.Empty : string.Empty,
            StainId1 = stain1,
            StainId2 = stain2,
            StainIcon1 = icon1,
            StainIcon2 = icon2
        };
    }


    public unsafe static void Close() => SERVICES.GameInventory.InventoryChanged -= OnInventoryChanged;
}
