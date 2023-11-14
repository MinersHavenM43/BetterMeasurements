using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace BetterMeasurements.Content {
	public class ModifyUnits : GlobalInfoDisplay {
		public override void ModifyDisplayParameters(InfoDisplay currentDisplay, ref string displayValue, ref string displayName, ref Color displayColor, ref Color displayShadowColor) {
			Player player = Main.player[Main.myPlayer];
			Color grayColor = new Color(100, 100, 100, Main.mouseTextColor);
			if(currentDisplay == InfoDisplay.Stopwatch) {
				Vector2 vector = player.velocity + player.instantMovementAccumulatedThisFrame;
				if (player.mount.Active && player.mount.IsConsideredASlimeMount && player.velocity.Y != 0 && !player.SlimeDontHyperJump)
					vector.Y += player.velocity.Y;
				int num15 = (int)(1f + vector.Length() * 6f);
				if (num15 > player.speedSlice.Length)
					num15 = player.speedSlice.Length;
				float num16 = 0f;
				for (int num17 = num15 - 1; num17 > 0; num17--)
					player.speedSlice[num17] = player.speedSlice[num17 - 1];
				player.speedSlice[0] = vector.Length();
				for (int m = 0; m < player.speedSlice.Length; m++) {
					if(m < num15)
						num16 += player.speedSlice[m];
					else
						player.speedSlice[m] = num16/num15;
				}
				num16 /= num15;
				if (!player.merman && !player.ignoreWater) {
					if (player.honeyWet) {
						num16 /= 4f;
					}
					else if (player.wet){
						num16 /= 2f;
					}
				}
				// the above code wall can mostly be disregarded, but it is absolutely necessary
				
				float velRounded = ConvertUnits.SpeedRound(ConvertUnits.ConvertSpeed(num16));
				displayValue = ConvertUnits.JoinSpeed(velRounded);
			}
			else if(currentDisplay == InfoDisplay.Compass) {
				float pos = player.position.X + player.width/2 - Main.maxTilesX*16 / 2; // in px
				float posRounded = ConvertUnits.DistanceFloor(Math.Abs(ConvertUnits.ConvertDistance(pos)));
				
				if(posRounded == 0)
					displayValue = "Center";
				else {
					if(pos < 0)
						displayValue = ConvertUnits.JoinDistance(posRounded, "West");
					else
						displayValue = ConvertUnits.JoinDistance(posRounded, "East");
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
					displayValue = "Level Surface";
				else {
					float internalDepth = (int)((double)((player.position.Y + player.height) * 2f / 16f) - Main.worldSurface * 2.0);
					float widthParam = Main.maxTilesX / 4200f;
					widthParam *= widthParam;
					float spaceLayerParam = (player.Center.Y / 16f - (65f + 10f * widthParam)) / (float)(Main.worldSurface / 5.0f);
					if(player.position.Y > (Main.maxTilesY - 204) * 16)
						depthLayer = "Underworld";
					else if(player.position.Y > Main.rockLayer * 16.0 + 632)
						depthLayer = "Caverns";
					else if(internalDepth > 0)
						depthLayer = "Underground";
					else if(!(spaceLayerParam >= 1f))
						depthLayer = "Space";
					else
						depthLayer = "Surface";
					displayValue = ConvertUnits.JoinDistance(depthPxRounded, depthLayer);
				}
			}
			else if(currentDisplay == InfoDisplay.DPSMeter) {
				player.checkDPSTime();
				int DPS = player.getDPS();
				
				if(DPS == 0) {
					displayValue = "No DPS";
					displayColor = grayColor;
				}
				else {
					displayValue = ConvertUnits.JoinDPS(DPS);
				}
			}
		}
	}
}