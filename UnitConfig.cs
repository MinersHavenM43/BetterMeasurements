using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace BetterMeasurements.Content {
	public class UnitConfig : ModConfig {
		public override ConfigScope Mode => ConfigScope.ClientSide;
		
		[Header("$Mods.BetterMeasurements.Configs.Header.Units")]
		[DefaultValue(5)]
		public int distanceUnitID;
		[DefaultValue(3)]
		public int speedUnitID;
		[DefaultValue(1)]
		public int DPSUnitID;
		
		[Header("$Mods.BetterMeasurements.Configs.Header.Misc")]
		[DefaultValue(true)]
		public bool depthMeterRoundUp;
		[DefaultValue(true)]
		public bool rounding;
		
		public override void OnChanged() {
			base.OnChanged();
			
			bool isDistanceUnitBroken = false, isSpeedUnitBroken = false, isDPSUnitBroken = false;
			ConvertUnits.VerifyConfig(this, ref isDistanceUnitBroken, ref isSpeedUnitBroken, ref isDPSUnitBroken);
			if(isDistanceUnitBroken)
				distanceUnitID = 1;
			if(isSpeedUnitBroken)
				speedUnitID = 1;
			if(isDPSUnitBroken)
				DPSUnitID = 1;
			
			ConvertUnits.unitConfig = this;
		}
		public override void OnLoaded() {
			base.OnLoaded();
			ConvertUnits.unitConfig = this;
		}
	}
}