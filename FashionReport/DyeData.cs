using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace FashionReportCalculator;

public static class GeneralData
{
    private static readonly string ConfigFolder = Path.Combine(SERVICES.Interface.GetPluginConfigDirectory(), "DiscordPlugin");
    private static readonly string ConfigPath = Path.Combine(ConfigFolder, "DyeData.bin");

    public static bool IsLongDisplay { get; set; } = true;
    public static uint LastWeekChecked { get; set; } = 0;
    public static List<DyeStruct> UserAttempts { get; set; } = new();

    static GeneralData() => Directory.CreateDirectory(ConfigFolder);

    public static void Save()
    {
        Directory.CreateDirectory(ConfigFolder);
        using FileStream fs = new FileStream(ConfigPath, FileMode.Create, FileAccess.Write, FileShare.None);
        using BinaryWriter writer = new BinaryWriter(fs);
        writer.Write(IsLongDisplay);
        writer.Write(LastWeekChecked);
        writer.Write(UserAttempts.Count);
        foreach (DyeStruct attempt in UserAttempts)
            WriteDyeStruct(writer, attempt);
    }

    public static void Load()
    {
        Directory.CreateDirectory(ConfigFolder);
        if (!File.Exists(ConfigPath))
        {
            IsLongDisplay = true;
            LastWeekChecked = 0;
            UserAttempts = new List<DyeStruct>();
            return;
        }
        using FileStream fs = new FileStream(ConfigPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using BinaryReader reader = new BinaryReader(fs);
        IsLongDisplay = reader.ReadBoolean();
        LastWeekChecked = reader.ReadUInt32();
        int count = reader.ReadInt32();
        List<DyeStruct> attempts = new List<DyeStruct>(count);
        for (int i = 0; i < count; i++)
            attempts.Add(ReadDyeStruct(reader));
        UserAttempts = attempts;
    }

    private static void WriteNullableUInt(BinaryWriter writer, uint? value)
    {
        writer.Write(value.HasValue);
        if (value.HasValue)
            writer.Write(value.Value);
    }

    private static uint? ReadNullableUInt(BinaryReader reader) => reader.ReadBoolean() ? reader.ReadUInt32() : (uint?)null;

    private static void WriteDyeStruct(BinaryWriter writer, DyeStruct d)
    {
        writer.Write(d.Week);
        writer.Write(d.Score);
        writer.Write(d.WeaponItemId);
        writer.Write(d.WeaponTheme);
        WriteNullableUInt(writer, d.WeaponDye1);
        WriteNullableUInt(writer, d.WeaponDye2);
        writer.Write(d.WeaponGlamourId);
        writer.Write(d.WeaponPicture);
        writer.Write(d.WeaponPictureInfo);
        writer.Write(d.HeadItemId);
        writer.Write(d.HeadTheme);
        WriteNullableUInt(writer, d.HeadDye1);
        WriteNullableUInt(writer, d.HeadDye2);
        writer.Write(d.HeadGlamourId);
        writer.Write(d.HeadPicture);
        writer.Write(d.HeadPictureInfo);
        writer.Write(d.BodyItemId);
        writer.Write(d.BodyTheme);
        WriteNullableUInt(writer, d.BodyDye1);
        WriteNullableUInt(writer, d.BodyDye2);
        writer.Write(d.BodyGlamourId);
        writer.Write(d.BodyPicture);
        writer.Write(d.BodyPictureInfo);
        writer.Write(d.GlovesItemId);
        writer.Write(d.GlovesTheme);
        WriteNullableUInt(writer, d.GlovesDye1);
        WriteNullableUInt(writer, d.GlovesDye2);
        writer.Write(d.GlovesGlamourId);
        writer.Write(d.GlovesPicture);
        writer.Write(d.GlovesPictureInfo);
        writer.Write(d.LegsItemId);
        writer.Write(d.LegsTheme);
        WriteNullableUInt(writer, d.LegsDye1);
        WriteNullableUInt(writer, d.LegsDye2);
        writer.Write(d.LegsGlamourId);
        writer.Write(d.LegsPicture);
        writer.Write(d.LegsPictureInfo);
        writer.Write(d.BootsItemId);
        writer.Write(d.BootsTheme);
        WriteNullableUInt(writer, d.BootsDye1);
        WriteNullableUInt(writer, d.BootsDye2);
        writer.Write(d.BootsGlamourId);
        writer.Write(d.BootsPicture);
        writer.Write(d.BootsPictureInfo);
        writer.Write(d.EarringsItemId);
        writer.Write(d.EarringsTheme);
        writer.Write(d.EarringsGlamourId);
        writer.Write(d.EarringsPicture);
        writer.Write(d.EarringsPictureInfo);
        writer.Write(d.NecklaceItemId);
        writer.Write(d.NecklaceTheme);
        writer.Write(d.NecklaceGlamourId);
        writer.Write(d.NecklacePicture);
        writer.Write(d.NecklacePictureInfo);
        writer.Write(d.BraceletItemId);
        writer.Write(d.BraceletTheme);
        writer.Write(d.BraceletGlamourId);
        writer.Write(d.BraceletPicture);
        writer.Write(d.BraceletPictureInfo);
        writer.Write(d.RightRingItemId);
        writer.Write(d.RightRingTheme);
        writer.Write(d.RightRingGlamourId);
        writer.Write(d.RightRingPicture);
        writer.Write(d.RightRingPictureInfo);
        writer.Write(d.LeftRingItemId);
        writer.Write(d.LeftRingTheme);
        writer.Write(d.LeftRingGlamourId);
        writer.Write(d.LeftRingPicture);
        writer.Write(d.LeftRingPictureInfo);
        writer.Write(d.Results ?? string.Empty);
    }

    private static DyeStruct ReadDyeStruct(BinaryReader reader)
    {
        DyeStruct d = new DyeStruct
        {
            Week = reader.ReadUInt32(),
            Score = reader.ReadUInt32(),
            WeaponItemId = reader.ReadUInt32(),
            WeaponTheme = reader.ReadUInt32(),
            WeaponDye1 = ReadNullableUInt(reader),
            WeaponDye2 = ReadNullableUInt(reader),
            WeaponGlamourId = reader.ReadUInt32(),
            WeaponPicture = reader.ReadUInt32(),
            WeaponPictureInfo = reader.ReadUInt32(),
            HeadItemId = reader.ReadUInt32(),
            HeadTheme = reader.ReadUInt32(),
            HeadDye1 = ReadNullableUInt(reader),
            HeadDye2 = ReadNullableUInt(reader),
            HeadGlamourId = reader.ReadUInt32(),
            HeadPicture = reader.ReadUInt32(),
            HeadPictureInfo = reader.ReadUInt32(),
            BodyItemId = reader.ReadUInt32(),
            BodyTheme = reader.ReadUInt32(),
            BodyDye1 = ReadNullableUInt(reader),
            BodyDye2 = ReadNullableUInt(reader),
            BodyGlamourId = reader.ReadUInt32(),
            BodyPicture = reader.ReadUInt32(),
            BodyPictureInfo = reader.ReadUInt32(),
            GlovesItemId = reader.ReadUInt32(),
            GlovesTheme = reader.ReadUInt32(),
            GlovesDye1 = ReadNullableUInt(reader),
            GlovesDye2 = ReadNullableUInt(reader),
            GlovesGlamourId = reader.ReadUInt32(),
            GlovesPicture = reader.ReadUInt32(),
            GlovesPictureInfo = reader.ReadUInt32(),
            LegsItemId = reader.ReadUInt32(),
            LegsTheme = reader.ReadUInt32(),
            LegsDye1 = ReadNullableUInt(reader),
            LegsDye2 = ReadNullableUInt(reader),
            LegsGlamourId = reader.ReadUInt32(),
            LegsPicture = reader.ReadUInt32(),
            LegsPictureInfo = reader.ReadUInt32(),
            BootsItemId = reader.ReadUInt32(),
            BootsTheme = reader.ReadUInt32(),
            BootsDye1 = ReadNullableUInt(reader),
            BootsDye2 = ReadNullableUInt(reader),
            BootsGlamourId = reader.ReadUInt32(),
            BootsPicture = reader.ReadUInt32(),
            BootsPictureInfo = reader.ReadUInt32(),
            EarringsItemId = reader.ReadUInt32(),
            EarringsTheme = reader.ReadUInt32(),
            EarringsGlamourId = reader.ReadUInt32(),
            EarringsPicture = reader.ReadUInt32(),
            EarringsPictureInfo = reader.ReadUInt32(),
            NecklaceItemId = reader.ReadUInt32(),
            NecklaceTheme = reader.ReadUInt32(),
            NecklaceGlamourId = reader.ReadUInt32(),
            NecklacePicture = reader.ReadUInt32(),
            NecklacePictureInfo = reader.ReadUInt32(),
            BraceletItemId = reader.ReadUInt32(),
            BraceletTheme = reader.ReadUInt32(),
            BraceletGlamourId = reader.ReadUInt32(),
            BraceletPicture = reader.ReadUInt32(),
            BraceletPictureInfo = reader.ReadUInt32(),
            RightRingItemId = reader.ReadUInt32(),
            RightRingTheme = reader.ReadUInt32(),
            RightRingGlamourId = reader.ReadUInt32(),
            RightRingPicture = reader.ReadUInt32(),
            RightRingPictureInfo = reader.ReadUInt32(),
            LeftRingItemId = reader.ReadUInt32(),
            LeftRingTheme = reader.ReadUInt32(),
            LeftRingGlamourId = reader.ReadUInt32(),
            LeftRingPicture = reader.ReadUInt32(),
            LeftRingPictureInfo = reader.ReadUInt32(),
            Results = reader.ReadString()
        };
        return d;
    }
}