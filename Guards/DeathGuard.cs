using System;
using System.Collections.Generic;
using Il2Cpp;
using MelonLoader;

namespace SpiritMod.Guards
{
	// Token: 0x02000035 RID: 53
	public class DeathGuard : IBotGuard
	{
		// Token: 0x06000172 RID: 370 RVA: 0x000100C4 File Offset: 0x0000E2C4
		public bool Evaluate(BotContext ctx)
		{
			if (!ctx.Config.EnableAutoRevive)
			{
				return false;
			}
			if (ctx.Status.State == BotState.Reviving)
			{
				return false;
			}
			PlayerController player = ctx.Player;
			if (player == null)
			{
				return false;
			}
			if (!CombatService.IsPlayerDead(player))
			{
				return false;
			}
			if (ctx.Status.IsPlayerDead)
			{
				return true;
			}
			ctx.Status.IsPlayerDead = true;
			MelonLogger.Msg("[Bot] Player died! Starting auto-revive sequence...");
			Il2CppSystem.Collections.Generic.List<string> mapNames = TeleporterService.GetMapNames();
			if (mapNames != null && ctx.Config.FarmingMapIndex >= 0 && ctx.Config.FarmingMapIndex < mapNames.Count)
			{
				ctx.Status.FarmingReturnMap = mapNames[ctx.Config.FarmingMapIndex];
			}
			else
			{
				ctx.Status.FarmingReturnMap = string.Empty;
				MelonLogger.Warning("[Bot] Invalid farming map index, will not teleport after revive");
			}
			BotController.ClearTarget();
			ctx.Status.WaitingForSceneLoad = true;
			ctx.Status.WaitingForFullResources = false;
			ctx.Status.ReviveTimer = 0.5f;
			ctx.Status.HealCooldown = 0f;
			BotController.TransitionTo(BotState.Reviving);
			return true;
		}
	}
}
