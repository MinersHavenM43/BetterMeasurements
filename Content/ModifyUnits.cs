using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BetterMeasurements.Content {
	public class ModifyUnits : GlobalInfoDisplay {
		public override void ModifyDisplayParameters(InfoDisplay currentDisplay, ref string displayValue, ref string displayName, ref Color displayColor, ref Color displayShadowColor) {
			Player player = Main.player[Main.myPlayer];
			Color grayColor = new Color(100, 100, 100, Main.mouseTextColor);
			if(currentDisplay == InfoDisplay.Stopwatch) {
				float vel = 0f;
				Vector2 vector = player.velocity + player.instantMovementAccumulatedThisFrame;
				if (player.mount.Active && player.mount.IsConsideredASlimeMount && player.velocity.Y != 0 && !player.SlimeDontHyperJump)
					vector.Y += player.velocity.Y;
				int num15 = (int)(1f + vector.Length() * 6f);
				if (num15 > player.speedSlice.Length)
					num15 = player.speedSlice.Length;
				for (int num17 = num15 - 1; num17 > 0; num17--)
					player.speedSlice[num17] = player.speedSlice[num17 - 1];
				player.speedSlice[0] = vector.Length();
				for (int m = 0; m < player.speedSlice.Length; m++) {
					if(m < num15)
						vel += player.speedSlice[m];
					else
						player.speedSlice[m] = vel/num15;
				}
				vel /= num15;
				if (!player.merman && !player.ignoreWater) {
					if (player.honeyWet) {
						vel /= 4f;
					}
					else if (player.wet){
						vel /= 2f;
					}
				}
				// the above code wall can mostly be disregarded, but it is absolutely necessary
				
				float velRounded = ConvertUnits.SpeedRound(ConvertUnits.ConvertSpeed(vel));
				displayValue = ConvertUnits.JoinSpeed(velRounded);
			}
		else if(currentDisplay == InfoDisplay.WeatherRadio) {
			string status;
			
			if(Main.IsItStorming) {
				status = Language.GetTextValue("GameUI.Storm");
			}
			else if(Main.maxRaining > 0.6) {
				status = Language.GetTextValue("GameUI.HeavyRain");
			}
			else if(Main.maxRaining >= 0.2) {
				status = Language.GetTextValue("GameUI.Rain");
			}
			else if(Main.maxRaining > 0) {
				status = Language.GetTextValue("GameUI.LightRain");
			}
			else if(Main.cloudBGActive > 0f) {
				status = Language.GetTextValue("GameUI.Overcast");
			}
			else if(Main.numClouds > 90) {
				status = Language.GetTextValue("GameUI.MostlyCloudy");
			}
			else if(Main.numClouds > 55) {
				status = Language.GetTextValue("GameUI.Cloudy");
			}
			else if(Main.numClouds > 15) {
				status = Language.GetTextValue("GameUI.PartlyCloudy");
			}
			else {
				status = Language.GetTextValue("GameUI.Clear");
			}
			
			float windSpeed = Main.windSpeedCurrent * 9.777777f;
			float windSpeedRounded = ConvertUnits.SpeedRound(ConvertUnits.ConvertSpeed(windSpeed));
			if(windSpeedRounded < 0) {
				status += Language.GetTextValue("Mods.BetterMeasurements.GameUI.EastWind", ConvertUnits.JoinSpeed(-windSpeedRounded));
			}
			else if(windSpeedRounded > 0) {
				status += Language.GetTextValue("Mods.BetterMeasurements.GameUI.WestWind", ConvertUnits.JoinSpeed(windSpeedRounded));
			}
			if(Sandstorm.Happening) {
				if(Main.GlobalTimeWrappedHourly % 10f >= 5f && ConvertUnits.unitConfig.weatherRadioCycle)
					status = Language.GetTextValue("GameUI.Sandstorm");
				status += " +";
			}
			
			displayValue = status;
		}
			else if(currentDisplay == InfoDisplay.Compass) {
				float pos = player.position.X + player.width/2 - Main.maxTilesX*16 / 2; // in px
				float posRounded = ConvertUnits.DistanceFloor(Math.Abs(ConvertUnits.ConvertDistance(pos)));
				
				if(posRounded == 0)
					displayValue = Language.GetTextValue("GameUI.CompassCenter");
				else {
					if(pos < 0)
						displayValue = Language.GetTextValue("Mods.BetterMeasurements.GameUI.CompassWest", ConvertUnits.JoinDistance(posRounded));
					else
						displayValue = Language.GetTextValue("Mods.BetterMeasurements.GameUI.CompassEast", ConvertUnits.JoinDistance(posRounded));
				}
			}
			else if(currentDisplay == InfoDisplay.DepthMeter) {
				float depthPx = (float)Main.worldSurface * 16 - player.position.Y - player.height;
				float depthPxRounded = ConvertUnits.ConvertDistance(depthPx);
				string depthLayer;
				
				if(ConvertUnits.unitConfig.depthMeterRoundUp)
					depthPxRounded = ConvertUnits.DistanceCeil(depthPxRounded);
				else
					depthPxRounded = ConvertUnits.DistanceFloor(depthPxRounded);
				depthPxRounded = Math.Abs(depthPxRounded);
				
				if(depthPxRounded == 0)
					displayValue = Language.GetTextValue("GameUI.DepthLevel") + " " + Language.GetTextValue("GameUI.LayerSurface");
				else {
					// float internalDepth = (int)((double)((player.position.Y + player.height) * 2f / 16f) - Main.worldSurface * 2.0);
					float widthParam = Main.maxTilesX / 4200f;
					widthParam *= widthParam;
					float spaceLayerParam = (player.Center.Y / 16f - (65f + 10f * widthParam)) / (float)(Main.worldSurface / 5.0f);
					if(player.position.Y > (Main.maxTilesY - 204) * 16)
						depthLayer = Language.GetTextValue("GameUI.LayerUnderworld");
					else if(player.position.Y > Main.rockLayer * 16.0 + 632)
						depthLayer = Language.GetTextValue("GameUI.LayerCaverns");
					else if(depthPx < 0)
						depthLayer = Language.GetTextValue("GameUI.LayerUnderground");
					else if(!(spaceLayerParam >= 1f))
						depthLayer = Language.GetTextValue("GameUI.LayerSpace");
					else
						depthLayer = Language.GetTextValue("GameUI.LayerSurface");
					displayValue = ConvertUnits.JoinDistance(depthPxRounded) + " " + depthLayer;
				}
			}
			else if(currentDisplay == InfoDisplay.DPSMeter) {
				player.checkDPSTime();
				int DPS = player.getDPS();
				
				if(DPS == 0) {
					displayValue = Language.GetTextValue("GameUI.NoDPS");
					displayColor = grayColor;
				}
				else {
					displayValue = ConvertUnits.JoinDPS(DPS);
				}
			}
		}
	}
}
