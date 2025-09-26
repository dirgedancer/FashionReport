using System;
using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Component.GUI;
#pragma warning disable IDE1006


namespace FashionReportCalculator;

internal static class DyeProcessing
{
    internal static List<DyeStruct> DyeInfo = new();
    internal static DyeColor Weapon = new();
    internal static DyeColor Head = new();
    internal static DyeColor Body = new();
    internal static DyeColor Gloves = new();
    internal static DyeColor Legs = new();
    internal static DyeColor Boots = new();

    internal static DyeStruct ProcessData(DyeStruct data)
    {
        uint Score = data.Score;
        if ((data.WeaponPicture == 5 && data.WeaponItemId != 0) || data.WeaponPicture == 3) Score -= 10;
        if ((data.HeadPicture == 5 && data.HeadItemId != 0) || data.HeadPicture == 3) Score -= 10;
        if ((data.BodyPicture == 5 && data.BodyItemId != 0) || data.BodyPicture == 3) Score -= 10;
        if ((data.GlovesPicture == 5 && data.GlovesItemId != 0) || data.GlovesPicture == 3) Score -= 10;
        if ((data.LegsPicture == 5 && data.LegsItemId != 0) || data.LegsPicture == 3) Score -= 10;
        if ((data.BootsPicture == 5 && data.BootsItemId != 0) || data.BootsPicture == 3) Score -= 10;
        if ((data.EarringsPicture == 5 && data.EarringsItemId != 0) || data.EarringsPicture == 3) Score -= 8;
        if ((data.NecklacePicture == 5 && data.NecklaceItemId != 0) || data.NecklacePicture == 3) Score -= 8;
        if ((data.BraceletPicture == 5 && data.BraceletItemId != 0) || data.BraceletPicture == 3) Score -= 8;
        if ((data.LeftRingPicture == 5 && data.LeftRingItemId != 0) || data.LeftRingPicture == 3) Score -= 8;
        if ((data.RightRingPicture == 5 && data.RightRingItemId != 0) || data.RightRingPicture == 3) Score -= 8;

        if (data.WeaponPicture == 3) Score -= 1;
        if (data.HeadPicture == 3) Score -= 1;
        if (data.BodyPicture == 3) Score -= 1;
        if (data.GlovesPicture == 3) Score -= 1;
        if (data.LegsPicture == 3) Score -= 1;
        if (data.BootsPicture == 3) Score -= 1;

        if (data.WeaponPicture == 4)
            ApplyNotColor("Weapon", data.WeaponDye1, data.WeaponDye2, c => ApplyNotAction(Weapon, c));
        if (data.HeadPicture == 4)
            ApplyNotColor("Head", data.HeadDye1, data.HeadDye2, c => ApplyNotAction(Head, c));
        if (data.BodyPicture == 4)
            ApplyNotColor("Body", data.BodyDye1, data.BodyDye2, c => ApplyNotAction(Body, c));
        if (data.GlovesPicture == 4)
            ApplyNotColor("Gloves", data.GlovesDye1, data.GlovesDye2, c => ApplyNotAction(Gloves, c));
        if (data.LegsPicture == 4)
            ApplyNotColor("Legs", data.LegsDye1, data.LegsDye2, c => ApplyNotAction(Legs, c));
        if (data.BootsPicture == 4)
            ApplyNotColor("Boots", data.BootsDye1, data.BootsDye2, c => ApplyNotAction(Boots, c));

        if (Score == 0)
        {
            if (data.WeaponPicture == 3)
            {
                bool hasDye1 = data.WeaponDye1 != null && data.WeaponDye1 != 0;
                bool hasDye2 = data.WeaponDye2 != null && data.WeaponDye2 != 0;
                if (hasDye1 && !hasDye2)
                    ApplyExactColor(data.WeaponDye1, c => ApplyAction(Weapon, c));
                else if (hasDye2 && !hasDye1)
                    ApplyExactColor(data.WeaponDye2, c => ApplyAction(Weapon, c));
                else if (hasDye1 && hasDye2)
                    ApplyDualColor(data.WeaponDye1, data.WeaponDye2, c => ApplyNotAction(Weapon, c));
            }
            if (data.HeadPicture == 3)
            {
                bool hasDye1 = data.HeadDye1 != null && data.HeadDye1 != 0;
                bool hasDye2 = data.HeadDye2 != null && data.HeadDye2 != 0;
                if (hasDye1 && !hasDye2)
                    ApplyExactColor(data.HeadDye1, c => ApplyAction(Head, c));
                else if (hasDye2 && !hasDye1)
                    ApplyExactColor(data.HeadDye2, c => ApplyAction(Head, c));
                else if (hasDye1 && hasDye2)
                    ApplyDualColor(data.HeadDye1, data.HeadDye2, c => ApplyNotAction(Head, c));
            }
            if (data.BodyPicture == 3)
            {
                bool hasDye1 = data.BodyDye1 != null && data.BodyDye1 != 0;
                bool hasDye2 = data.BodyDye2 != null && data.BodyDye2 != 0;
                if (hasDye1 && !hasDye2)
                    ApplyExactColor(data.BodyDye1, c => ApplyAction(Body, c));
                else if (hasDye2 && !hasDye1)
                    ApplyExactColor(data.BodyDye2, c => ApplyAction(Body, c));
                else if (hasDye1 && hasDye2)
                    ApplyDualColor(data.BodyDye1, data.BodyDye2, c => ApplyNotAction(Body, c));
            }
            if (data.GlovesPicture == 3)
            {
                bool hasDye1 = data.GlovesDye1 != null && data.GlovesDye1 != 0;
                bool hasDye2 = data.GlovesDye2 != null && data.GlovesDye2 != 0;
                if (hasDye1 && !hasDye2)
                    ApplyExactColor(data.GlovesDye1, c => ApplyAction(Gloves, c));
                else if (hasDye2 && !hasDye1)
                    ApplyExactColor(data.GlovesDye2, c => ApplyAction(Gloves, c));
                else if (hasDye1 && hasDye2)
                    ApplyDualColor(data.GlovesDye1, data.GlovesDye2, c => ApplyNotAction(Gloves, c));
            }
            if (data.LegsPicture == 3)
            {
                bool hasDye1 = data.LegsDye1 != null && data.LegsDye1 != 0;
                bool hasDye2 = data.LegsDye2 != null && data.LegsDye2 != 0;
                if (hasDye1 && !hasDye2)
                    ApplyExactColor(data.LegsDye1, c => ApplyAction(Legs, c));
                else if (hasDye2 && !hasDye1)
                    ApplyExactColor(data.LegsDye2, c => ApplyAction(Legs, c));
                else if (hasDye1 && hasDye2)
                    ApplyDualColor(data.LegsDye1, data.LegsDye2, c => ApplyNotAction(Legs, c));
            }
            if (data.BootsPicture == 3)
            {
                bool hasDye1 = data.BootsDye1 != null && data.BootsDye1 != 0;
                bool hasDye2 = data.BootsDye2 != null && data.BootsDye2 != 0;
                if (hasDye1 && !hasDye2)
                    ApplyExactColor(data.BootsDye1, c => ApplyAction(Boots, c));
                else if (hasDye2 && !hasDye1)
                    ApplyExactColor(data.BootsDye2, c => ApplyAction(Boots, c));
                else if (hasDye1 && hasDye2)
                    ApplyDualColor(data.BootsDye1, data.BootsDye2, c => ApplyNotAction(Boots, c));
            }
        }
        else if (Score == 1)
        {
            if (data.WeaponPicture == 3)
            {
                if (data.WeaponDye1 != null && data.WeaponDye1 != 0)
                    data.Results += $"{SERVICES.StainTable.FirstOrDefault(d => d.Key == data.WeaponDye1).Value.Name} possibly the weapon stain";
                if (data.WeaponDye2 != null && data.WeaponDye2 != 0)
                    data.Results += $"{SERVICES.StainTable.FirstOrDefault(d => d.Key == data.WeaponDye2).Value.Name} possibly the weapon stain";
            }
            else if (data.WeaponPicture == 5)
            {
                if (data.WeaponDye1 != null && data.WeaponDye1 != 0)
                    data.Results += $"Weapon possibly {GetColor((SERVICES.StainTable.FirstOrDefault(d => d.Key == data.WeaponDye1).Value.ItemId - 22804) % 9)}";
                if (data.WeaponDye2 != null && data.WeaponDye2 != 0)
                    data.Results += $"Weapon possibly {GetColor((SERVICES.StainTable.FirstOrDefault(d => d.Key == data.WeaponDye2).Value.ItemId - 22804) % 9)}";
            }
            if (data.HeadPicture == 3)
            {
                if (data.HeadDye1 != null && data.HeadDye1 != 0)
                    data.Results += $"{SERVICES.StainTable.FirstOrDefault(d => d.Key == data.HeadDye1).Value.Name} possibly the head stain";
                if (data.HeadDye2 != null && data.HeadDye2 != 0)
                    data.Results += $"{SERVICES.StainTable.FirstOrDefault(d => d.Key == data.HeadDye2).Value.Name} possibly the head stain";
            }
            else if (data.HeadPicture == 5)
            {
                if (data.HeadDye1 != null && data.HeadDye1 != 0)
                    data.Results += $"Head possibly {GetColor((SERVICES.StainTable.FirstOrDefault(d => d.Key == data.HeadDye1).Value.ItemId - 22804) % 9)}";
                if (data.HeadDye2 != null && data.HeadDye2 != 0)
                    data.Results += $"Head possibly {GetColor((SERVICES.StainTable.FirstOrDefault(d => d.Key == data.HeadDye2).Value.ItemId - 22804) % 9)}";
            }
            if (data.BodyPicture == 3)
            {
                if (data.BodyDye1 != null && data.BodyDye1 != 0)
                    data.Results += $"{SERVICES.StainTable.FirstOrDefault(d => d.Key == data.BodyDye1).Value.Name} possibly the body stain";
                if (data.BodyDye2 != null && data.BodyDye2 != 0)
                    data.Results += $"{SERVICES.StainTable.FirstOrDefault(d => d.Key == data.BodyDye2).Value.Name} possibly the body stain";
            }
            else if (data.BodyPicture == 5)
            {
                if (data.BodyDye1 != null && data.BodyDye1 != 0)
                    data.Results += $"Body possibly {GetColor((SERVICES.StainTable.FirstOrDefault(d => d.Key == data.BodyDye1).Value.ItemId - 22804) % 9)}";
                if (data.BodyDye2 != null && data.BodyDye2 != 0)
                    data.Results += $"Body possibly {GetColor((SERVICES.StainTable.FirstOrDefault(d => d.Key == data.BodyDye2).Value.ItemId - 22804) % 9)}";
            }
            if (data.GlovesPicture == 3)
            {
                if (data.GlovesDye1 != null && data.GlovesDye1 != 0)
                    data.Results += $"{SERVICES.StainTable.FirstOrDefault(d => d.Key == data.GlovesDye1).Value.Name} possibly the gloves stain";
                if (data.GlovesDye2 != null && data.GlovesDye2 != 0)
                    data.Results += $"{SERVICES.StainTable.FirstOrDefault(d => d.Key == data.GlovesDye2).Value.Name} possibly the gloves stain";
            }
            else if (data.GlovesPicture == 5)
            {
                if (data.GlovesDye1 != null && data.GlovesDye1 != 0)
                    data.Results += $"Gloves possibly {GetColor((SERVICES.StainTable.FirstOrDefault(d => d.Key == data.GlovesDye1).Value.ItemId - 22804) % 9)}";
                if (data.GlovesDye2 != null && data.GlovesDye2 != 0)
                    data.Results += $"Gloves possibly {GetColor((SERVICES.StainTable.FirstOrDefault(d => d.Key == data.GlovesDye2).Value.ItemId - 22804) % 9)}";
            }
            if (data.LegsPicture == 3)
            {
                if (data.LegsDye1 != null && data.LegsDye1 != 0)
                    data.Results += $"{SERVICES.StainTable.FirstOrDefault(d => d.Key == data.LegsDye1).Value.Name} possibly the legs stain";
                if (data.LegsDye2 != null && data.LegsDye2 != 0)
                    data.Results += $"{SERVICES.StainTable.FirstOrDefault(d => d.Key == data.LegsDye2).Value.Name} possibly the legs stain";
            }
            else if (data.LegsPicture == 5)
            {
                if (data.LegsDye1 != null && data.LegsDye1 != 0)
                    data.Results += $"Legs possibly {GetColor((SERVICES.StainTable.FirstOrDefault(d => d.Key == data.LegsDye1).Value.ItemId - 22804) % 9)}";
                if (data.LegsDye2 != null && data.LegsDye2 != 0)
                    data.Results += $"Legs possibly {GetColor((SERVICES.StainTable.FirstOrDefault(d => d.Key == data.LegsDye2).Value.ItemId - 22804) % 9)}";
            }
            if (data.BootsPicture == 3)
            {
                if (data.BootsDye1 != null && data.BootsDye1 != 0)
                    data.Results += $"{SERVICES.StainTable.FirstOrDefault(d => d.Key == data.BootsDye1).Value.Name} possibly the boots stain";
                if (data.BootsDye2 != null && data.BootsDye2 != 0)
                    data.Results += $"{SERVICES.StainTable.FirstOrDefault(d => d.Key == data.BootsDye2).Value.Name} possibly the boots stain";
            }
            else if (data.BootsPicture == 5)
            {
                if (data.BootsDye1 != null && data.BootsDye1 != 0)
                    data.Results += $"Boots possibly {GetColor((SERVICES.StainTable.FirstOrDefault(d => d.Key == data.BootsDye1).Value.ItemId - 22804) % 9)}";
                if (data.BootsDye2 != null && data.BootsDye2 != 0)
                    data.Results += $"Boots possibly {GetColor((SERVICES.StainTable.FirstOrDefault(d => d.Key == data.BootsDye2).Value.ItemId - 22804) % 9)}";
            }
        }
        else if (Score == AllPossible(data))
        {
            if (data.WeaponPicture == 3 || data.WeaponPicture == 5)
            {
                if (data.WeaponDye1 != null && data.WeaponDye1 != 0 && (data.WeaponDye2 == null || data.WeaponDye2 == 0))
                    data.Results += $"{SERVICES.StainTable.FirstOrDefault(d => d.Key == data.WeaponDye1).Value.Name} is the weapon stain";
                else if ((data.WeaponDye1 == null || data.WeaponDye1 == 0) && data.WeaponDye2 != null && data.WeaponDye2 != 0)
                    data.Results += $"{SERVICES.StainTable.FirstOrDefault(d => d.Key == data.WeaponDye2).Value.Name} is the weapon stain";
                else if (data.WeaponDye1 != null && data.WeaponDye1 != 0 && data.WeaponDye2 != null && data.WeaponDye2 != 0)
                    data.Results += data.WeaponDye1 == data.WeaponDye2
                        ? $"{SERVICES.StainTable.FirstOrDefault(d => d.Key == data.WeaponDye1).Value.Name} is the weapon stain"
                        : $"{SERVICES.StainTable.FirstOrDefault(d => d.Key == data.WeaponDye1).Value.Name} or {SERVICES.StainTable.FirstOrDefault(d => d.Key == data.WeaponDye2).Value.Name} is the weapon stain";
            }

            if (data.HeadPicture == 3 || data.HeadPicture == 5)
            {
                if (data.HeadDye1 != null && data.HeadDye1 != 0 && (data.HeadDye2 == null || data.HeadDye2 == 0))
                    data.Results += $"{SERVICES.StainTable.FirstOrDefault(d => d.Key == data.HeadDye1).Value.Name} is the head stain";
                else if ((data.HeadDye1 == null || data.HeadDye1 == 0) && data.HeadDye2 != null && data.HeadDye2 != 0)
                    data.Results += $"{SERVICES.StainTable.FirstOrDefault(d => d.Key == data.HeadDye2).Value.Name} is the head stain";
                else if (data.HeadDye1 != null && data.HeadDye1 != 0 && data.HeadDye2 != null && data.HeadDye2 != 0)
                    data.Results += data.HeadDye1 == data.HeadDye2
                        ? $"{SERVICES.StainTable.FirstOrDefault(d => d.Key == data.HeadDye1).Value.Name} is the head stain"
                        : $"{SERVICES.StainTable.FirstOrDefault(d => d.Key == data.HeadDye1).Value.Name} or {SERVICES.StainTable.FirstOrDefault(d => d.Key == data.HeadDye2).Value.Name} is the head stain";
            }

            if (data.BodyPicture == 3 || data.BodyPicture == 5)
            {
                if (data.BodyDye1 != null && data.BodyDye1 != 0 && (data.BodyDye2 == null || data.BodyDye2 == 0))
                    data.Results += $"{SERVICES.StainTable.FirstOrDefault(d => d.Key == data.BodyDye1).Value.Name} is the body stain";
                else if ((data.BodyDye1 == null || data.BodyDye1 == 0) && data.BodyDye2 != null && data.BodyDye2 != 0)
                    data.Results += $"{SERVICES.StainTable.FirstOrDefault(d => d.Key == data.BodyDye2).Value.Name} is the body stain";
                else if (data.BodyDye1 != null && data.BodyDye1 != 0 && data.BodyDye2 != null && data.BodyDye2 != 0)
                    data.Results += data.BodyDye1 == data.BodyDye2
                        ? $"{SERVICES.StainTable.FirstOrDefault(d => d.Key == data.BodyDye1).Value.Name} is the body stain"
                        : $"{SERVICES.StainTable.FirstOrDefault(d => d.Key == data.BodyDye1).Value.Name} or {SERVICES.StainTable.FirstOrDefault(d => d.Key == data.BodyDye2).Value.Name} is the body stain";
            }

            if (data.GlovesPicture == 3 || data.GlovesPicture == 5)
            {
                if (data.GlovesDye1 != null && data.GlovesDye1 != 0 && (data.GlovesDye2 == null || data.GlovesDye2 == 0))
                    data.Results += $"{SERVICES.StainTable.FirstOrDefault(d => d.Key == data.GlovesDye1).Value.Name} is the gloves stain";
                else if ((data.GlovesDye1 == null || data.GlovesDye1 == 0) && data.GlovesDye2 != null && data.GlovesDye2 != 0)
                    data.Results += $"{SERVICES.StainTable.FirstOrDefault(d => d.Key == data.GlovesDye2).Value.Name} is the gloves stain";
                else if (data.GlovesDye1 != null && data.GlovesDye1 != 0 && data.GlovesDye2 != null && data.GlovesDye2 != 0)
                    data.Results += data.GlovesDye1 == data.GlovesDye2
                        ? $"{SERVICES.StainTable.FirstOrDefault(d => d.Key == data.GlovesDye1).Value.Name} is the gloves stain"
                        : $"{SERVICES.StainTable.FirstOrDefault(d => d.Key == data.GlovesDye1).Value.Name} or {SERVICES.StainTable.FirstOrDefault(d => d.Key == data.GlovesDye2).Value.Name} is the gloves stain";
            }

            if (data.LegsPicture == 3 || data.LegsPicture == 5)
            {
                if (data.LegsDye1 != null && data.LegsDye1 != 0 && (data.LegsDye2 == null || data.LegsDye2 == 0))
                    data.Results += $"{SERVICES.StainTable.FirstOrDefault(d => d.Key == data.LegsDye1).Value.Name} is the legs stain";
                else if ((data.LegsDye1 == null || data.LegsDye1 == 0) && data.LegsDye2 != null && data.LegsDye2 != 0)
                    data.Results += $"{SERVICES.StainTable.FirstOrDefault(d => d.Key == data.LegsDye2).Value.Name} is the legs stain";
                else if (data.LegsDye1 != null && data.LegsDye1 != 0 && data.LegsDye2 != null && data.LegsDye2 != 0)
                    data.Results += data.LegsDye1 == data.LegsDye2
                        ? $"{SERVICES.StainTable.FirstOrDefault(d => d.Key == data.LegsDye1).Value.Name} is the legs stain"
                        : $"{SERVICES.StainTable.FirstOrDefault(d => d.Key == data.LegsDye1).Value.Name} or {SERVICES.StainTable.FirstOrDefault(d => d.Key == data.LegsDye2).Value.Name} is the legs stain";
            }

            if (data.BootsPicture == 3 || data.BootsPicture == 5)
            {
                if (data.BootsDye1 != null && data.BootsDye1 != 0 && (data.BootsDye2 == null || data.BootsDye2 == 0))
                    data.Results += $"{SERVICES.StainTable.FirstOrDefault(d => d.Key == data.BootsDye1).Value.Name} is the boots stain";
                else if ((data.BootsDye1 == null || data.BootsDye1 == 0) && data.BootsDye2 != null && data.BootsDye2 != 0)
                    data.Results += $"{SERVICES.StainTable.FirstOrDefault(d => d.Key == data.BootsDye2).Value.Name} is the boots stain";
                else if (data.BootsDye1 != null && data.BootsDye1 != 0 && data.BootsDye2 != null && data.BootsDye2 != 0)
                    data.Results += data.BootsDye1 == data.BootsDye2
                        ? $"{SERVICES.StainTable.FirstOrDefault(d => d.Key == data.BootsDye1).Value.Name} is the boots stain"
                        : $"{SERVICES.StainTable.FirstOrDefault(d => d.Key == data.BootsDye1).Value.Name} or {SERVICES.StainTable.FirstOrDefault(d => d.Key == data.BootsDye2).Value.Name} is the boots stain";
            }
        }
        return data;
    }

    private static uint AllPossible(DyeStruct data)
    {
        uint Possible = 0;
        if (data.WeaponPicture == 3 || data.WeaponPicture == 5) Possible += 2;
        if (data.HeadPicture == 3 || data.HeadPicture == 5) Possible += 2;
        if (data.BodyPicture == 3 || data.BodyPicture == 5) Possible += 2;
        if (data.GlovesPicture == 3 || data.GlovesPicture == 5) Possible += 2;
        if (data.LegsPicture == 3 || data.LegsPicture == 5) Possible += 2;
        if (data.BootsPicture == 3 || data.BootsPicture == 5) Possible += 2;
        return Possible;
    }

    private static string GetColor(uint x)
    {
        return x switch
        {
            0 => "Blue",
            1 => "Red",
            2 => "Yellow",
            3 => "Black",
            4 => "Grey",
            5 => "Brown",
            6 => "Green",
            7 => "White",
            8 => "Purple",
            _ => string.Empty
        };
    }

    private static void HandleDye(uint? dye, Action<int> action)
    {
        if (dye == null || dye == 0) return;
        uint iconId = SERVICES.StainTable.FirstOrDefault(d => d.Key == dye).Value.IconId;
        int color = (int)((iconId - 22804) % 9);
        action(color);
    }

    private static void ApplyNotColor(string slot, uint? dye1, uint? dye2, Action<int> action)
    {
        HandleDye(dye1, action);
        HandleDye(dye2, action);
    }

    private static void ApplyExactColor(uint? dye, Action<int> action)
    {
        HandleDye(dye, action);
    }

    private static void ApplyDualColor(uint? dye1, uint? dye2, Action<int> notAction)
    {
        if (dye1 == null || dye1 == 0 || dye2 == null || dye2 == 0) return;
        uint iconId1 = SERVICES.StainTable.FirstOrDefault(d => d.Key == dye1).Value.IconId;
        uint iconId2 = SERVICES.StainTable.FirstOrDefault(d => d.Key == dye2).Value.IconId;
        int color1 = (int)((iconId1 - 22804) % 9);
        int color2 = (int)((iconId2 - 22804) % 9);
        for (int c = 0; c < 9; c++)
            if (color1 != c && color2 != c) notAction(c);
    }

    private static void ApplyNotAction(object slot, int color)
    {
        switch (color)
        {
            case 0: slot.GetType().GetMethod("IsNotBlue")?.Invoke(slot, null); break;
            case 1: slot.GetType().GetMethod("IsNotRed")?.Invoke(slot, null); break;
            case 2: slot.GetType().GetMethod("IsNotYellow")?.Invoke(slot, null); break;
            case 3: slot.GetType().GetMethod("IsNotBlack")?.Invoke(slot, null); break;
            case 4: slot.GetType().GetMethod("IsNotGrey")?.Invoke(slot, null); break;
            case 5: slot.GetType().GetMethod("IsNotBrown")?.Invoke(slot, null); break;
            case 6: slot.GetType().GetMethod("IsNotGreen")?.Invoke(slot, null); break;
            case 7: slot.GetType().GetMethod("IsNotWhite")?.Invoke(slot, null); break;
            case 8: slot.GetType().GetMethod("IsNotPurple")?.Invoke(slot, null); break;
        }
    }

    private static void ApplyAction(object slot, int color)
    {
        switch (color)
        {
            case 0: slot.GetType().GetMethod("IsBlue")?.Invoke(slot, null); break;
            case 1: slot.GetType().GetMethod("IsRed")?.Invoke(slot, null); break;
            case 2: slot.GetType().GetMethod("IsYellow")?.Invoke(slot, null); break;
            case 3: slot.GetType().GetMethod("IsBlack")?.Invoke(slot, null); break;
            case 4: slot.GetType().GetMethod("IsGrey")?.Invoke(slot, null); break;
            case 5: slot.GetType().GetMethod("IsBrown")?.Invoke(slot, null); break;
            case 6: slot.GetType().GetMethod("IsGreen")?.Invoke(slot, null); break;
            case 7: slot.GetType().GetMethod("IsWhite")?.Invoke(slot, null); break;
            case 8: slot.GetType().GetMethod("IsPurple")?.Invoke(slot, null); break;
        }
    }

    internal static void ResetDyeProcessing()
    {
        Weapon = new();
        Head = new();
        Body = new();
        Gloves = new();
        Legs = new();
        Boots = new();
    }

    internal static async Task LoadDyeData() => DyeInfo = await GoogleSheetData.GetDyeData();
}

internal class DyeColor
{
    private uint Blue { get; set; } = 33554431;
    private uint Green { get; set; } = 4194303;
    private uint Brown { get; set; } = 2097151;
    private uint Red { get; set; } = 262143;
    private uint Yellow { get; set; } = 32767;
    private uint Purple { get; set; } = 8191;
    private uint Grey { get; set; } = 31;
    private uint White { get; set; } = 7;
    private uint Black { get; set; } = 7;



    internal uint TurnBitOff(uint value, int position) => value & ~(1u << position);
    internal uint FlipBit(uint value, int position) => value ^ (1u << position);
    internal void IsNotBlue() => Blue = 0;
    internal void IsNotGreen() => Green = 0;
    internal void IsNotBrown() => Brown = 0;
    internal void IsNotRed() => Red = 0;
    internal void IsNotYellow() => Yellow = 0;
    internal void IsNotPurple() => Purple = 0;
    internal void IsNotGrey() => Grey = 0;
    internal void IsNotWhite() => White = 0;
    internal void IsNotBlack() => Black = 0;
    internal void IsBlue() => Green = Brown = Red = Yellow = Purple = Grey = White = Black = 0;
    internal void IsGreen() => Blue = Brown = Red = Yellow = Purple = Grey = White = Black = 0;
    internal void IsBrown() => Blue = Green = Red = Yellow = Purple = Grey = White = Black = 0;
    internal void IsRed() => Blue = Green = Brown = Yellow = Purple = Grey = White = Black = 0;
    internal void IsYellow() => Blue = Green = Brown = Red = Purple = Grey = White = Black = 0;
    internal void IsPurple() => Blue = Green = Brown = Red = Yellow = Grey = White = Black = 0;
    internal void IsGrey() => Blue = Green = Brown = Red = Yellow = Purple = White = Black = 0;
    internal void IsWhite() => Blue = Green = Brown = Red = Yellow = Purple = Grey = Black = 0;
    internal void IsBlack() => Blue = Green = Brown = Red = Yellow = Purple = Grey = White = 0;
    internal void IsNotIceBlue() => Blue = TurnBitOff(Blue, 0);
    internal void IsNotSkyBlue() => Blue = TurnBitOff(Blue, 1);
    internal void IsNotSeaFogBlue() => Blue = TurnBitOff(Blue, 2);
    internal void IsNotPeacockBlue() => Blue = TurnBitOff(Blue, 3);
    internal void IsNotRhotanoBlue() => Blue = TurnBitOff(Blue, 4);
    internal void IsNotCorpseBlue() => Blue = TurnBitOff(Blue, 5);
    internal void IsNotCeruleumBlue() => Blue = TurnBitOff(Blue, 6);
    internal void IsNotWoadBlue() => Blue = TurnBitOff(Blue, 7);
    internal void IsNotInkBlue() => Blue = TurnBitOff(Blue, 8);
    internal void IsNotRaptorBlue() => Blue = TurnBitOff(Blue, 9);
    internal void IsNotOthardBlue() => Blue = TurnBitOff(Blue, 10);
    internal void IsNotStormBlue() => Blue = TurnBitOff(Blue, 11);
    internal void IsNotRoyalBlue() => Blue = TurnBitOff(Blue, 12);
    internal void IsNotMidnightBlue() => Blue = TurnBitOff(Blue, 13);
    internal void IsNotShadowBlue() => Blue = TurnBitOff(Blue, 14);
    internal void IsNotAbyssalBlue() => Blue = TurnBitOff(Blue, 15);
    internal void IsNotDragoonBlue() => Blue = TurnBitOff(Blue, 16);
    internal void IsNotTurquoiseBlue() => Blue = TurnBitOff(Blue, 17);
    internal void IsNotAzureBlue() => Blue = TurnBitOff(Blue, 18);
    internal void IsNotPastelBlue() => Blue = TurnBitOff(Blue, 19);
    internal void IsNotDarkBlue() => Blue = TurnBitOff(Blue, 20);
    internal void IsNotMetallicSkyBlue() => Blue = TurnBitOff(Blue, 21);
    internal void IsNotMetallicBlue() => Blue = TurnBitOff(Blue, 22);
    internal void IsNotMetallicDarkBlue() => Blue = TurnBitOff(Blue, 23);
    internal void IsNotMudGreen() => Green = TurnBitOff(Green, 0);
    internal void IsNotSylphGreen() => Green = TurnBitOff(Green, 1);
    internal void IsNotLimeGreen() => Green = TurnBitOff(Green, 2);
    internal void IsNotMossGreen() => Green = TurnBitOff(Green, 3);
    internal void IsNotMeadowGreen() => Green = TurnBitOff(Green, 4);
    internal void IsNotOliveGreen() => Green = TurnBitOff(Green, 5);
    internal void IsNotMarshGreen() => Green = TurnBitOff(Green, 6);
    internal void IsNotAppleGreen() => Green = TurnBitOff(Green, 7);
    internal void IsNotCactuarGreen() => Green = TurnBitOff(Green, 8);
    internal void IsNotHunterGreen() => Green = TurnBitOff(Green, 9);
    internal void IsNotOchuGreen() => Green = TurnBitOff(Green, 10);
    internal void IsNotAdamantoiseGreen() => Green = TurnBitOff(Green, 11);
    internal void IsNotNophicaGreen() => Green = TurnBitOff(Green, 12);
    internal void IsNotDeepwoodGreen() => Green = TurnBitOff(Green, 13);
    internal void IsNotCelesteGreen() => Green = TurnBitOff(Green, 14);
    internal void IsNotTurquoiseGreen() => Green = TurnBitOff(Green, 15);
    internal void IsNotMorbolGreen() => Green = TurnBitOff(Green, 16);
    internal void IsNotNeonGreen() => Green = TurnBitOff(Green, 17);
    internal void IsNotPastelGreen() => Green = TurnBitOff(Green, 18);
    internal void IsNotDarkGreen() => Green = TurnBitOff(Green, 19);
    internal void IsNotMetallicGreen() => Green = TurnBitOff(Green, 20);
    internal void IsNotMetallicCobaltGreen() => Green = TurnBitOff(Green, 21);
    internal void IsNotSunsetOrange() => Brown = TurnBitOff(Brown, 0);
    internal void IsNotMesaRed() => Brown = TurnBitOff(Brown, 1);
    internal void IsNotBarkBrown() => Brown = TurnBitOff(Brown, 2);
    internal void IsNotChocolateBrown() => Brown = TurnBitOff(Brown, 3);
    internal void IsNotRussetBrown() => Brown = TurnBitOff(Brown, 4);
    internal void IsNotKoboldBrown() => Brown = TurnBitOff(Brown, 5);
    internal void IsNotCorkBrown() => Brown = TurnBitOff(Brown, 6);
    internal void IsNotQiqirnBrown() => Brown = TurnBitOff(Brown, 7);
    internal void IsNotOpoopoBrown() => Brown = TurnBitOff(Brown, 8);
    internal void IsNotAldgoatBrown() => Brown = TurnBitOff(Brown, 9);
    internal void IsNotPumpkinOrange() => Brown = TurnBitOff(Brown, 10);
    internal void IsNotAcornBrown() => Brown = TurnBitOff(Brown, 11);
    internal void IsNotOrchardBrown() => Brown = TurnBitOff(Brown, 12);
    internal void IsNotChestnutBrown() => Brown = TurnBitOff(Brown, 13);
    internal void IsNotGobbiebagBrown() => Brown = TurnBitOff(Brown, 14);
    internal void IsNotShaleBrown() => Brown = TurnBitOff(Brown, 15);
    internal void IsNotMoleBrown() => Brown = TurnBitOff(Brown, 16);
    internal void IsNotLoamBrown() => Brown = TurnBitOff(Brown, 17);
    internal void IsNotBrightOrange() => Brown = TurnBitOff(Brown, 18);
    internal void IsNotDarkBrown() => Brown = TurnBitOff(Brown, 19);
    internal void IsNotMetallicOrange() => Brown = TurnBitOff(Brown, 20);
    internal void IsNotRosePink() => Red = TurnBitOff(Red, 0);
    internal void IsNotLilacPurple() => Red = TurnBitOff(Red, 1);
    internal void IsNotRolanberryRed() => Red = TurnBitOff(Red, 2);
    internal void IsNotDalamudRed() => Red = TurnBitOff(Red, 3);
    internal void IsNotRustRed() => Red = TurnBitOff(Red, 4);
    internal void IsNotWineRed() => Red = TurnBitOff(Red, 5);
    internal void IsNotCoralPink() => Red = TurnBitOff(Red, 6);
    internal void IsNotBloodRed() => Red = TurnBitOff(Red, 7);
    internal void IsNotSalmonPink() => Red = TurnBitOff(Red, 8);
    internal void IsNotRubyRed() => Red = TurnBitOff(Red, 9);
    internal void IsNotCherryPink() => Red = TurnBitOff(Red, 10);
    internal void IsNotCarmineRed() => Red = TurnBitOff(Red, 11);
    internal void IsNotNeonPink() => Red = TurnBitOff(Red, 12);
    internal void IsNotPastelPink() => Red = TurnBitOff(Red, 13);
    internal void IsNotDarkRed() => Red = TurnBitOff(Red, 14);
    internal void IsNotMetallicRed() => Red = TurnBitOff(Red, 15);
    internal void IsNotMetallicPink() => Red = TurnBitOff(Red, 16);
    internal void IsNotMetallicRubyRed() => Red = TurnBitOff(Red, 17);
    internal void IsNotBoneWhite() => Yellow = TurnBitOff(Yellow, 0);
    internal void IsNotUlBrown() => Yellow = TurnBitOff(Yellow, 1);
    internal void IsNotDesertYellow() => Yellow = TurnBitOff(Yellow, 2);
    internal void IsNotHoneyYellow() => Yellow = TurnBitOff(Yellow, 3);
    internal void IsNotMillioncornYellow() => Yellow = TurnBitOff(Yellow, 4);
    internal void IsNotCoeurlYellow() => Yellow = TurnBitOff(Yellow, 5);
    internal void IsNotCreamYellow() => Yellow = TurnBitOff(Yellow, 6);
    internal void IsNotHalataliYellow() => Yellow = TurnBitOff(Yellow, 7);
    internal void IsNotRaisinBrown() => Yellow = TurnBitOff(Yellow, 8);
    internal void IsNotCanaryYellow() => Yellow = TurnBitOff(Yellow, 9);
    internal void IsNotVanillaYellow() => Yellow = TurnBitOff(Yellow, 10);
    internal void IsNotMetallicBrass() => Yellow = TurnBitOff(Yellow, 11);
    internal void IsNotNeonYellow() => Yellow = TurnBitOff(Yellow, 12);
    internal void IsNotMetallicGold() => Yellow = TurnBitOff(Yellow, 13);
    internal void IsNotMetallicYellow() => Yellow = TurnBitOff(Yellow, 14);
    internal void IsNotLavenderPurple() => Purple = TurnBitOff(Purple, 0);
    internal void IsNotGloomPurple() => Purple = TurnBitOff(Purple, 1);
    internal void IsNotCurrantPurple() => Purple = TurnBitOff(Purple, 2);
    internal void IsNotIrisPurple() => Purple = TurnBitOff(Purple, 3);
    internal void IsNotGrapePurple() => Purple = TurnBitOff(Purple, 4);
    internal void IsNotLotusPink() => Purple = TurnBitOff(Purple, 5);
    internal void IsNotColibriPink() => Purple = TurnBitOff(Purple, 6);
    internal void IsNotPlumPurple() => Purple = TurnBitOff(Purple, 7);
    internal void IsNotRegalPurple() => Purple = TurnBitOff(Purple, 8);
    internal void IsNotPastelPurple() => Purple = TurnBitOff(Purple, 9);
    internal void IsNotDarkPurple() => Purple = TurnBitOff(Purple, 10);
    internal void IsNotMetallicPurple() => Purple = TurnBitOff(Purple, 11);
    internal void IsNotVioletPurple() => Purple = TurnBitOff(Purple, 12);
    internal void IsNotAshGrey() => Grey = TurnBitOff(Grey, 0);
    internal void IsNotGoobbueGrey() => Grey = TurnBitOff(Grey, 1);
    internal void IsNotSlateGrey() => Grey = TurnBitOff(Grey, 2);
    internal void IsNotCharcoalGrey() => Grey = TurnBitOff(Grey, 3);
    internal void IsNotMetallicSilver() => Grey = TurnBitOff(Grey, 4);
    internal void IsNotSootBlack() => Black = TurnBitOff(Black, 0);
    internal void IsNotGunmetalBlack() => Black = TurnBitOff(Black, 1);
    internal void IsNotJetBlack() => Black = TurnBitOff(Black, 2);
    internal void IsNotSnowWhite() => White = TurnBitOff(White, 0);
    internal void IsNotPearlWhite() => White = TurnBitOff(White, 1);
    internal void IsNotPureWhite() => White = TurnBitOff(White, 2);
}

internal class DyeStruct
{
    public uint Week { get; set; }
    public uint Score { get; set; }

    public uint WeaponItemId { get; set; }
    public uint WeaponTheme { get; set; }
    public uint? WeaponDye1 { get; set; }
    public uint? WeaponDye2 { get; set; }
    public uint WeaponGlamourId { get; set; }
    public uint WeaponPicture { get; set; }
    public uint WeaponPictureInfo { get; set; }

    public uint HeadItemId { get; set; }
    public uint HeadTheme { get; set; }
    public uint? HeadDye1 { get; set; }
    public uint? HeadDye2 { get; set; }
    public uint HeadGlamourId { get; set; }
    public uint HeadPicture { get; set; }
    public uint HeadPictureInfo { get; set; }

    public uint BodyItemId { get; set; }
    public uint BodyTheme { get; set; }
    public uint? BodyDye1 { get; set; }
    public uint? BodyDye2 { get; set; }
    public uint BodyGlamourId { get; set; }
    public uint BodyPicture { get; set; }
    public uint BodyPictureInfo { get; set; }

    public uint GlovesItemId { get; set; }
    public uint GlovesTheme { get; set; }
    public uint? GlovesDye1 { get; set; }
    public uint? GlovesDye2 { get; set; }
    public uint GlovesGlamourId { get; set; }
    public uint GlovesPicture { get; set; }
    public uint GlovesPictureInfo { get; set; }

    public uint LegsItemId { get; set; }
    public uint LegsTheme { get; set; }
    public uint? LegsDye1 { get; set; }
    public uint? LegsDye2 { get; set; }
    public uint LegsGlamourId { get; set; }
    public uint LegsPicture { get; set; }
    public uint LegsPictureInfo { get; set; }

    public uint BootsItemId { get; set; }
    public uint BootsTheme { get; set; }
    public uint? BootsDye1 { get; set; }
    public uint? BootsDye2 { get; set; }
    public uint BootsGlamourId { get; set; }
    public uint BootsPicture { get; set; }
    public uint BootsPictureInfo { get; set; }

    public uint EarringsItemId { get; set; }
    public uint EarringsTheme { get; set; }
    public uint EarringsGlamourId { get; set; }
    public uint EarringsPicture { get; set; }
    public uint EarringsPictureInfo { get; set; }

    public uint NecklaceItemId { get; set; }
    public uint NecklaceTheme { get; set; }
    public uint NecklaceGlamourId { get; set; }
    public uint NecklacePicture { get; set; }
    public uint NecklacePictureInfo { get; set; }

    public uint BraceletItemId { get; set; }
    public uint BraceletTheme { get; set; }
    public uint BraceletGlamourId { get; set; }
    public uint BraceletPicture { get; set; }
    public uint BraceletPictureInfo { get; set; }

    public uint RightRingItemId { get; set; }
    public uint RightRingTheme { get; set; }
    public uint RightRingGlamourId { get; set; }
    public uint RightRingPicture { get; set; }
    public uint RightRingPictureInfo { get; set; }

    public uint LeftRingItemId { get; set; }
    public uint LeftRingTheme { get; set; }
    public uint LeftRingGlamourId { get; set; }
    public uint LeftRingPicture { get; set; }
    public uint LeftRingPictureInfo { get; set; }
    public string Results { get; set; } = string.Empty;
}