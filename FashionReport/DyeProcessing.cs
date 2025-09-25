using System;
using System.Collections.Generic;
#pragma warning disable IDE1006


namespace FashionReportCalculator;

internal static class DyeProcessing
{
    internal static List<DyeStruct> DyeInfo = new();

    internal static async Task LoadDyeData() => DyeInfo = await GoogleSheetData.GetDyeData();
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