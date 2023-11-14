using System;
using System.Collections.Generic;

namespace BetterMeasurements.Content {
	public class Unit {
		public float fromPxUnits;
		public string symbol;
		public int specialProp;
		public Unit(string symbolArg, float fromPxUnitsArg, int specialPropArg = 0) {
			fromPxUnits = fromPxUnitsArg;
			symbol = symbolArg;
			specialProp = specialPropArg;
		}
	}
	public class ConvertUnits {
		private static readonly List<Unit> distanceUnitList = new() {
			new Unit("'",  0.125f),
			new Unit(" ft",  0.125f),
			new Unit(" tl",  0.0625f),
			new Unit("*",  0.0625f),
			new Unit(" m",  0.0381f, 1),
			new Unit(" px",  1f),
			new Unit(".",  1f)
		};
		private static readonly List<Unit> speedUnitList = new() {
			new Unit(" mph",  5.11363636f),
			new Unit(" mi/h",  5.11363636f),
			new Unit(" km/h", 8.2296f),
			new Unit(" m/s",  2.286f),
			new Unit(" tl/s",  3.75f),
			new Unit(" px/tc", 1f, 1)
		};
		private static readonly List<Unit> DPSUnitList = new() {
			new Unit(" damage per second",  1f),
			new Unit(" DPS",  1f),
			new Unit(" d/s", 1f),
			new Unit(" D/s",  1f)
		};
		
		public static void VerifyConfig(UnitConfig unitConfigArg, ref bool isDistanceUnitBroken, ref bool isSpeedUnitBroken, ref bool isDPSUnitBroken) {
			if(unitConfigArg.distanceUnitID < 1 || unitConfigArg.distanceUnitID > distanceUnitList.Count) {
				isDistanceUnitBroken = true;
				return;
			}
			if(unitConfigArg.speedUnitID < 1 || unitConfigArg.speedUnitID > speedUnitList.Count) {
				isSpeedUnitBroken = true;
				return;
			}
			if(unitConfigArg.DPSUnitID < 1 || unitConfigArg.DPSUnitID > DPSUnitList.Count) {
				isDPSUnitBroken = true;
				return;
			}
		}
		
		public static UnitConfig unitConfig = new UnitConfig();
		private static Unit DistanceUnit() => distanceUnitList[unitConfig.distanceUnitID-1];
		private static Unit SpeedUnit() => speedUnitList[unitConfig.speedUnitID-1];
		private static Unit DPSUnit() => DPSUnitList[unitConfig.DPSUnitID-1];
		
		public static float ConvertDistance(float value) {
			return value * DistanceUnit().fromPxUnits;
		}
		public static float ConvertSpeed(float value) {
			return value * SpeedUnit().fromPxUnits;
		}
		
		public static string JoinDistance(float value, string direction) {
			return value.ToString() + DistanceUnit().symbol + " " + direction;
		}
		public static string JoinSpeed(float value) {
			return value.ToString() + SpeedUnit().symbol;
		}
		public static string JoinDPS(int value) {
			return value.ToString() + DPSUnit().symbol;
		}

		public static float DistanceFloor(float value) {
			if(!unitConfig.rounding)
				return value;
			else {
				if(DistanceUnit().specialProp == 1)
					return (float)Math.Floor(10f*value) / 10f;
				else
					return (float)Math.Floor(value);
			}
		}
		public static float DistanceCeil(float value) {
			if(!unitConfig.rounding)
				return value;
			else {
				if(DistanceUnit().specialProp == 1)
					return (float)Math.Floor(10f*value) / 10f;
				else
					return (float)Math.Ceiling(value);
			}
		}
		public static float SpeedRound(float value) {
			if(!unitConfig.rounding)
				return value;
			else {
				if(SpeedUnit().specialProp == 1)
					return (float)Math.Round(10f*value) / 10f;
				else
					return (float)Math.Round(value);
			}
		}
	}
}
