using System;
using System.Runtime.CompilerServices;
using Il2Cpp;
using MelonLoader;

namespace SpiritMod.States
{
	// Token: 0x02000030 RID: 48
	public class RevivingState : IBotState
	{
		// Token: 0x1700003A RID: 58
		// (get) Token: 0x0600015F RID: 351 RVA: 0x0000FB4B File Offset: 0x0000DD4B
		public BotState Id
		{
			get
			{
				return BotState.Reviving;
			}
		}

		// Token: 0x06000160 RID: 352 RVA: 0x0000FB4E File Offset: 0x0000DD4E
		public void Enter(BotContext ctx)
		{
			MelonLogger.Msg("[Bot] Entered Reviving state");
		}

		// Token: 0x06000161 RID: 353 RVA: 0x0000FB5C File Offset: 0x0000DD5C
		public void Tick(BotContext ctx)
		{
			PlayerController player = ctx.Player;
			if (player == null)
			{
				ctx.Status.ReviveTimer -= ctx.DeltaTime;
				if (ctx.Status.ReviveTimer <= 0f)
				{
					ctx.Status.ReviveTimer = 1f;
					MelonLogger.Msg("[Bot] Reviving: waiting for player object...");
				}
				return;
			}
			if (ctx.Status.WaitingForSceneLoad)
			{
				if (!CombatService.IsPlayerDead(player))
				{
					MelonLogger.Msg("[Bot] Player is alive at full HP (ReviveSelf 1.0f)");
					ctx.Status.WaitingForSceneLoad = false;
					ctx.Status.WaitingForFullResources = false;
					ctx.Status.ReviveTimer = ctx.Config.ReviveDelay;
					return;
				}
				ctx.Status.ReviveTimer -= ctx.DeltaTime;
				if (ctx.Status.ReviveTimer <= 0f)
				{
					try
					{
						int num = -1;
						if (!string.IsNullOrEmpty(ctx.Status.FarmingReturnMap))
						{
							num = TeleporterService.GetMapId(ctx.Status.FarmingReturnMap);
							if (num > 0)
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(43, 2);
								defaultInterpolatedStringHandler.AppendLiteral("[Bot] ReviveSelf with teleport to '");
								defaultInterpolatedStringHandler.AppendFormatted(ctx.Status.FarmingReturnMap);
								defaultInterpolatedStringHandler.AppendLiteral("' (ID: ");
								defaultInterpolatedStringHandler.AppendFormatted<int>(num);
								defaultInterpolatedStringHandler.AppendLiteral(")");
								MelonLogger.Msg(defaultInterpolatedStringHandler.ToStringAndClear());
								ctx.Status.FarmingReturnMap = string.Empty;
							}
						}
						Cheats.ReviveSelf(num);
						MelonLogger.Msg("[Bot] Calling ReviveSelf(1.0f) — player HP is still 0");
					}
					catch (Exception ex)
					{
						MelonLogger.Warning("[Bot] ReviveSelf call failed: " + ex.Message);
					}
					ctx.Status.ReviveTimer = 2f;
				}
				return;
			}
			else
			{
				if (!BotController.TryTickSummons(ctx.DeltaTime, "pre-teleport"))
				{
					return;
				}
				ctx.Status.ReviveTimer -= ctx.DeltaTime;
				if (ctx.Status.ReviveTimer > 0f)
				{
					return;
				}
				if (!string.IsNullOrEmpty(ctx.Status.ReviveTargetMap))
				{
					string currentMapName = TeleporterService.GetCurrentMapName();
					if (string.Equals(currentMapName, ctx.Status.ReviveTargetMap, StringComparison.OrdinalIgnoreCase))
					{
						MelonLogger.Msg("[Bot] Arrived on map: " + currentMapName);
					}
					else
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(50, 2);
						defaultInterpolatedStringHandler.AppendLiteral("[Bot] Expected map '");
						defaultInterpolatedStringHandler.AppendFormatted(ctx.Status.ReviveTargetMap);
						defaultInterpolatedStringHandler.AppendLiteral("' but on '");
						defaultInterpolatedStringHandler.AppendFormatted(currentMapName);
						defaultInterpolatedStringHandler.AppendLiteral("', proceeding anyway");
						MelonLogger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
					}
					ctx.Status.ReviveTargetMap = string.Empty;
				}
				if (!string.IsNullOrEmpty(ctx.Status.FarmingReturnMap))
				{
					MelonLogger.Msg("[Bot] Teleporting back to farming map: " + ctx.Status.FarmingReturnMap);
					ctx.Status.ReviveTargetMap = ctx.Status.FarmingReturnMap;
					TeleporterService.WarpToMap(ctx.Status.FarmingReturnMap);
					ctx.Status.FarmingReturnMap = string.Empty;
					ctx.Status.ReviveTimer = ctx.Config.ReviveDelay;
					return;
				}
				MelonLogger.Msg("[Bot] Auto-revive complete, returning to IDLE");
				BotController.TransitionTo(BotState.Idle);
				ctx.Status.ActionTimer = 0.5f;
				ctx.Status.IsPlayerDead = false;
				return;
			}
		}

		// Token: 0x06000162 RID: 354 RVA: 0x0000FEA0 File Offset: 0x0000E0A0
		public void Exit(BotContext ctx)
		{
		}
	}
}
