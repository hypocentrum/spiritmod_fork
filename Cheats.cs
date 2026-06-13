using System;
using System.Runtime.CompilerServices;
using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace SpiritMod
{
	// Token: 0x02000005 RID: 5
	public static class Cheats
	{
		// Token: 0x06000009 RID: 9 RVA: 0x00002170 File Offset: 0x00000370
		public static void Tick()
		{
			try
			{
				Cheats.TickAntiIdle();
				PlayerController player = GameStateService.Player;
				if (!(player == null))
				{
					Cheats.TickSpeedOverride(player);
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Warning("[Cheats] " + ex.Message);
			}
		}

		// Token: 0x0600000A RID: 10 RVA: 0x000021C4 File Offset: 0x000003C4
		private static void TickSpeedOverride(PlayerController player)
		{
			MoveComponent move = player.Move;
			if (move == null)
			{
				return;
			}
			if (Cheats.BaseSpeedOverride)
			{
				if (!Cheats._baseSpeedSaved)
				{
					Cheats._originalBaseSpeed = move.BaseSpeed;
					Cheats._baseSpeedSaved = true;
				}
				move.BaseSpeed = Cheats.BaseSpeed;
				return;
			}
			if (Cheats._baseSpeedSaved)
			{
				move.BaseSpeed = Cheats._originalBaseSpeed;
				Cheats._baseSpeedSaved = false;
			}
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002228 File Offset: 0x00000428
		private static void TickAntiIdle()
		{
			Cheats._antiIdleTimer -= Time.deltaTime;
			if (Cheats._antiIdleTimer > 0f)
			{
				return;
			}
			Cheats._antiIdleTimer = 60f;
			PlayerController player = GameStateService.Player;
			if (player == null)
			{
				return;
			}
			player.idleLastActivityTime = Time.time;
			MelonLogger.Msg("[Cheats] Anti-idle: reset idle timer");
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00002284 File Offset: 0x00000484
		public static void UnlockAllWaypoints()
		{
			try
			{
				PlayerController player = GameStateService.Player;
				if (player == null)
				{
					MelonLogger.Warning("[Cheats] Cannot unlock waypoints: player not found");
				}
				else
				{
					PlayerSave save = player.Save;
					if (save == null)
					{
						MelonLogger.Warning("[Cheats] Cannot unlock waypoints: PlayerSave is null");
					}
					else
					{
						foreach(var map in App.Game.Map.Maps)
                        {
                            save.UnlockWaypoint_S(map.GetInstanceID());
                            MelonLogger.Msg($"[Cheats] Unlocked {map.GetInstanceID()}:{map.name} waypoint ({map.Id})");
                        }
					}
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Warning("[Cheats] UnlockAllWaypoints failed: " + ex.Message);
			}
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00002304 File Offset: 0x00000504
		public static void ReviveSelf(int mapId = -1)
		{
			try
			{
				PlayerController playerController = GameStateService.Player ?? App.Player;
				if (playerController == null)
				{
					MelonLogger.Warning("[Cheats] ReviveSelf: player not found");
				}
				else
				{
					playerController.ReviveSelf(1.0f);
                    MelonLogger.Msg($"[Cheats] ReviveSelf(1.0f, {mapId}) RPC sent");
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Warning("[Cheats] ReviveSelf failed: " + ex.Message);
			}
		}

		// Token: 0x0600000E RID: 14 RVA: 0x000023A0 File Offset: 0x000005A0
		public static void WarpHome()
		{
			try
			{
				PlayerController player = GameStateService.Player;
				if (player == null)
				{
					MelonLogger.Warning("[Cheats] WarpHome: player not found");
				}
				else
				{
					player.WarpHome_Rpc();
					MelonLogger.Msg("[Cheats] WarpHome RPC invoked");
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Warning("[Cheats] WarpHome failed: " + ex.Message);
			}
		}

		// Token: 0x0600000F RID: 15 RVA: 0x00002404 File Offset: 0x00000604
		public static void FullHeal()
		{
			try
			{
				PlayerController player = GameStateService.Player;
				if (player == null)
				{
					MelonLogger.Warning("[Cheats] FullHeal: player not found");
				}
				else
				{
					player.FullHealByHealer();
					MelonLogger.Msg("[Cheats] FullHeal RPC invoked");
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Warning("[Cheats] FullHeal failed: " + ex.Message);
			}
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002468 File Offset: 0x00000668
		public static void InvalidateCache()
		{
			Cheats._baseSpeedSaved = false;
			Cheats._originalBaseSpeed = 0f;
		}

		// Token: 0x04000003 RID: 3
		public static bool BaseSpeedOverride = false;

		// Token: 0x04000004 RID: 4
		public static float BaseSpeed = 25f;

		// Token: 0x04000005 RID: 5
		private static float _originalBaseSpeed;

		// Token: 0x04000006 RID: 6
		private static bool _baseSpeedSaved;

		// Token: 0x04000007 RID: 7
		private static float _antiIdleTimer;

		// Token: 0x04000008 RID: 8
		private const float AntiIdleInterval = 60f;
	}
}
