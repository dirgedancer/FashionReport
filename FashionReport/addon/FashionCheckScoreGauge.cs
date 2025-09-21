using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Component.GUI;


namespace FashionReportCalculator;

public class FashionCheckScoreGauge : FCAddon
{
    public FashionCheckScoreGauge() : base(SERVICES.GameGui.GetAddonByName("FashionCheckScoreGauge")) { }

    public FashionCheckScoreGauge(nint AddonPtr) : base(AddonPtr) { WindowName = "FashionCheckScoreGauge"; }

    public uint Score { get { return TryGetAtkValue<uint>(0); } }
}
