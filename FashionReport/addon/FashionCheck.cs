using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FashionReport;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Component.GUI;


namespace FashionReport;

public class FashionCheck : FCAddon
{
    public FashionCheck() : base(SERVICES.GameGui.GetAddonByName("FashionCheck")) { }

    public FashionCheck(nint AddonPtr) : base(AddonPtr) { WindowName = "FashionCheck"; }

    public string WeeklyTheme { get { return TryGetAtkValue<string>(0) ?? ""; } }
    public bool IsWeaponThemed { get { return TryGetAtkValue<bool>(1); } }
    public string WeaponTheme { get { return TryGetAtkValue<string>(2) ?? ""; } }
    public uint WeaponPicture { get { return TryGetAtkValue<uint>(3); } }
    public uint WeaponPictureInfo { get { return TryGetAtkValue<uint>(4); } }
    public uint WeaponGameName { get { return TryGetAtkValue<uint>(5); } }
    public uint WeaponItemId { get { return TryGetAtkValue<uint>(6); } }
    public uint WeaponGlamourId { get { return TryGetAtkValue<uint>(7); } }
    public uint WeaponShade1 { get { return TryGetAtkValue<uint>(8); } }
    public uint WeaponShade2 { get { return TryGetAtkValue<uint>(9); } }
    public uint WeaponUnknown1 { get { return TryGetAtkValue<uint>(10); } }
    public uint WeaponUnknown2 { get { return TryGetAtkValue<uint>(11); } }
    public bool IsHeadThemed { get { return TryGetAtkValue<bool>(12); } }
    public string HeadTheme { get { return TryGetAtkValue<string>(13) ?? ""; } }
    public uint HeadPicture { get { return TryGetAtkValue<uint>(14); } }
    public uint HeadPictureInfo { get { return TryGetAtkValue<uint>(15); } }
    public uint HeadGameName { get { return TryGetAtkValue<uint>(16); } }
    public uint HeadItemId { get { return TryGetAtkValue<uint>(17); } }
    public uint HeadGlamourId { get { return TryGetAtkValue<uint>(18); } }
    public uint HeadShade1 { get { return TryGetAtkValue<uint>(19); } }
    public uint HeadShade2 { get { return TryGetAtkValue<uint>(20); } }
    public uint HeadUnknown1 { get { return TryGetAtkValue<uint>(21); } }
    public uint HeadUnknown2 { get { return TryGetAtkValue<uint>(22); } }
    public bool IsBodyThemed { get { return TryGetAtkValue<bool>(23); } }
    public string BodyTheme { get { return TryGetAtkValue<string>(24) ?? ""; } }
    public uint BodyPicture { get { return TryGetAtkValue<uint>(25); } }
    public uint BodyPictureInfo { get { return TryGetAtkValue<uint>(26); } }
    public uint BodyGameName { get { return TryGetAtkValue<uint>(27); } }
    public uint BodyItemId { get { return TryGetAtkValue<uint>(28); } }
    public uint BodyGlamourId { get { return TryGetAtkValue<uint>(29); } }
    public uint BodyShade1 { get { return TryGetAtkValue<uint>(30); } }
    public uint BodyShade2 { get { return TryGetAtkValue<uint>(31); } }
    public uint BodyUnknown1 { get { return TryGetAtkValue<uint>(32); } }
    public uint BodyUnknown2 { get { return TryGetAtkValue<uint>(33); } }
    public bool IsHandsThemed { get { return TryGetAtkValue<bool>(34); } }
    public string HandsTheme { get { return TryGetAtkValue<string>(35) ?? ""; } }
    public uint HandsPicture { get { return TryGetAtkValue<uint>(36); } }
    public uint HandsPictureInfo { get { return TryGetAtkValue<uint>(37); } }
    public uint HandsGameName { get { return TryGetAtkValue<uint>(38); } }
    public uint HandsItemId { get { return TryGetAtkValue<uint>(39); } }
    public uint HandsGlamourId { get { return TryGetAtkValue<uint>(40); } }
    public uint HandsShade1 { get { return TryGetAtkValue<uint>(41); } }
    public uint HandsShade2 { get { return TryGetAtkValue<uint>(42); } }
    public uint HandsUnknown1 { get { return TryGetAtkValue<uint>(43); } }
    public uint HandsUnknown2 { get { return TryGetAtkValue<uint>(44); } }
    public bool IsLegsThemed { get { return TryGetAtkValue<bool>(45); } }
    public string LegsTheme { get { return TryGetAtkValue<string>(46) ?? ""; } }
    public uint LegsPicture { get { return TryGetAtkValue<uint>(47); } }
    public uint LegsPictureInfo { get { return TryGetAtkValue<uint>(48); } }
    public uint LegsGameName { get { return TryGetAtkValue<uint>(49); } }
    public uint LegsItemId { get { return TryGetAtkValue<uint>(50); } }
    public uint LegsGlamourId { get { return TryGetAtkValue<uint>(51); } }
    public uint LegsShade1 { get { return TryGetAtkValue<uint>(52); } }
    public uint LegsShade2 { get { return TryGetAtkValue<uint>(53); } }
    public uint LegsUnknown1 { get { return TryGetAtkValue<uint>(54); } }
    public uint LegsUnknown2 { get { return TryGetAtkValue<uint>(55); } }
    public bool IsFeetThemed { get { return TryGetAtkValue<bool>(56); } }
    public string FeetTheme { get { return TryGetAtkValue<string>(57) ?? ""; } }
    public uint FeetPicture { get { return TryGetAtkValue<uint>(58); } }
    public uint FeetPictureInfo { get { return TryGetAtkValue<uint>(59); } }
    public uint FeetGameName { get { return TryGetAtkValue<uint>(60); } }
    public uint FeetItemId { get { return TryGetAtkValue<uint>(61); } }
    public uint FeetGlamourId { get { return TryGetAtkValue<uint>(62); } }
    public uint FeetShade1 { get { return TryGetAtkValue<uint>(63); } }
    public uint FeetShade2 { get { return TryGetAtkValue<uint>(64); } }
    public uint FeetUnknown1 { get { return TryGetAtkValue<uint>(65); } }
    public uint FeetUnknown2 { get { return TryGetAtkValue<uint>(66); } }
    public bool IsEarringsThemed { get { return TryGetAtkValue<bool>(67); } }
    public string EarringsTheme { get { return TryGetAtkValue<string>(68) ?? ""; } }
    public uint EarringsPicture { get { return TryGetAtkValue<uint>(69); } }
    public uint EarringsPictureInfo { get { return TryGetAtkValue<uint>(70); } }
    public uint EarringsGameName { get { return TryGetAtkValue<uint>(71); } }
    public uint EarringsItemId { get { return TryGetAtkValue<uint>(72); } }
    public uint EarringsGlamourId { get { return TryGetAtkValue<uint>(73); } }
    public uint EarringsShade1 { get { return TryGetAtkValue<uint>(74); } }
    public uint EarringsShade2 { get { return TryGetAtkValue<uint>(75); } }
    public uint EarringsUnknown1 { get { return TryGetAtkValue<uint>(76); } }
    public uint EarringsUnknown2 { get { return TryGetAtkValue<uint>(77); } }
    public bool IsNeckThemed { get { return TryGetAtkValue<bool>(78); } }
    public string NeckTheme { get { return TryGetAtkValue<string>(79) ?? ""; } }
    public uint NeckPicture { get { return TryGetAtkValue<uint>(80); } }
    public uint NeckPictureInfo { get { return TryGetAtkValue<uint>(81); } }
    public uint NeckGameName { get { return TryGetAtkValue<uint>(82); } }
    public uint NeckItemId { get { return TryGetAtkValue<uint>(83); } }
    public uint NeckGlamourId { get { return TryGetAtkValue<uint>(84); } }
    public uint NeckShade1 { get { return TryGetAtkValue<uint>(85); } }
    public uint NeckShade2 { get { return TryGetAtkValue<uint>(86); } }
    public uint NeckUnknown1 { get { return TryGetAtkValue<uint>(87); } }
    public uint NeckUnknown2 { get { return TryGetAtkValue<uint>(88); } }
    public bool IsWristThemed { get { return TryGetAtkValue<bool>(89); } }
    public string WristTheme { get { return TryGetAtkValue<string>(90) ?? ""; } }
    public uint WristPicture { get { return TryGetAtkValue<uint>(91); } }
    public uint WristPictureInfo { get { return TryGetAtkValue<uint>(92); } }
    public uint WristGameName { get { return TryGetAtkValue<uint>(93); } }
    public uint WristItemId { get { return TryGetAtkValue<uint>(94); } }
    public uint WristGlamourId { get { return TryGetAtkValue<uint>(95); } }
    public uint WristShade1 { get { return TryGetAtkValue<uint>(96); } }
    public uint WristShade2 { get { return TryGetAtkValue<uint>(97); } }
    public uint WristUnknown1 { get { return TryGetAtkValue<uint>(98); } }
    public uint WristUnknown2 { get { return TryGetAtkValue<uint>(99); } }
    public bool IsRightRingThemed { get { return TryGetAtkValue<bool>(100); } }
    public string RightRingTheme { get { return TryGetAtkValue<string>(101) ?? ""; } }
    public uint RightRingPicture { get { return TryGetAtkValue<uint>(102); } }
    public uint RightRingPictureInfo { get { return TryGetAtkValue<uint>(103); } }
    public uint RightRingGameName { get { return TryGetAtkValue<uint>(104); } }
    public uint RightRingItemId { get { return TryGetAtkValue<uint>(105); } }
    public uint RightRingGlamourId { get { return TryGetAtkValue<uint>(106); } }
    public uint RightRingShade1 { get { return TryGetAtkValue<uint>(107); } }
    public uint RightRingShade2 { get { return TryGetAtkValue<uint>(108); } }
    public uint RightRingUnknown1 { get { return TryGetAtkValue<uint>(109); } }
    public uint RightRingUnknown2 { get { return TryGetAtkValue<uint>(110); } }
    public bool IsLeftRingThemed { get { return TryGetAtkValue<bool>(111); } }
    public string LeftRingTheme { get { return TryGetAtkValue<string>(112) ?? ""; } }
    public uint LeftRingPicture { get { return TryGetAtkValue<uint>(113); } }
    public uint LeftRingPictureInfo { get { return TryGetAtkValue<uint>(114); } }
    public uint LeftRingGameName { get { return TryGetAtkValue<uint>(115); } }
    public uint LeftRingItemId { get { return TryGetAtkValue<uint>(116); } }
    public uint LeftRingGlamourId { get { return TryGetAtkValue<uint>(117); } }
    public uint LeftRingShade1 { get { return TryGetAtkValue<uint>(118); } }
    public uint LeftRingShade2 { get { return TryGetAtkValue<uint>(119); } }
    public uint LeftRingUnknown1 { get { return TryGetAtkValue<uint>(120); } }
    public uint LeftRingUnknown2 { get { return TryGetAtkValue<uint>(121); } }
    public string RemainingString { get { return TryGetAtkValue<string>(122) ?? ""; } }
    public uint RemainingAttempts
    {
        get
        {
            string s = TryGetAtkValue<string>(122)?.Replace("Remaining: ", "") ?? "";
            uint x = uint.Parse(s);
            x--;
            return x;
        }
    }
    public string HighScoreString { get { return TryGetAtkValue<string>(123) ?? ""; } }
    public uint HighScore
    {
        get
        {
            string s = TryGetAtkValue<string>(123)?.Replace("High Score: ", "") ?? "";
            return uint.Parse(s);
        }
    }
}
