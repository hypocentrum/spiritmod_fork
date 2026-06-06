using System;
using System.Runtime.CompilerServices;
using MelonLoader;

namespace SpiritMod.Guards
{
	// Token: 0x02000036 RID: 54
	public class ZoneGuard : IBotGuard
	{
		// Token: 0x06000174 RID: 372 RVA: 0x000101E0 File Offset: 0x0000E3E0
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
			if (string.IsNullOrEmpty(ctx.Config.FarmingMapName))
			{
				return false;
			}
			if (ctx.Status.ZonePendingTeleport)
			{
				if (!BotController.TryTickSummons(ctx.DeltaTime, "zone-guard"))
				{
					return true;
				}
				MelonLogger.Msg("[Bot] Summons ready, teleporting to '" + ctx.Config.FarmingMapName + "'");
				TeleporterService.WarpToMap(ctx.Config.FarmingMapName);
				ctx.Status.ZoneTeleportCooldown = 10f;
				ctx.Status.ZonePendingTeleport = false;
				BotController.TransitionTo(BotState.Idle);
				ctx.Status.ActionTimer = 3f;
				return true;
			}
			else
			{
				ctx.Status.ZoneTeleportCooldown -= ctx.DeltaTime;
				ctx.Status.ZoneCheckTimer -= ctx.DeltaTime;
				if (ctx.Status.ZoneCheckTimer > 0f)
				{
					return false;
				}
				ctx.Status.ZoneCheckTimer = 3f;
				if (ctx.Status.ZoneTeleportCooldown > 0f)
				{
					return false;
				}
				try
				{
					string currentMapName = TeleporterService.GetCurrentMapName();
					if (!IsSameMap(currentMapName, ctx.Config.FarmingMapName))
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(63, 2);
						defaultInterpolatedStringHandler.AppendLiteral("[Bot] Wrong zone detected: '");
						defaultInterpolatedStringHandler.AppendFormatted(currentMapName);
						defaultInterpolatedStringHandler.AppendLiteral("' — summoning before teleport to '");
						defaultInterpolatedStringHandler.AppendFormatted(ctx.Config.FarmingMapName);
						defaultInterpolatedStringHandler.AppendLiteral("'");
						MelonLogger.Msg(defaultInterpolatedStringHandler.ToStringAndClear());
						BotController.ClearTarget();
						ctx.Status.ZonePendingTeleport = true;
						ctx.Status.SkillTimer = 0f;
						return true;
					}
				}
				catch (Exception ex)
				{
					MelonLogger.Warning("[Bot] Zone check failed: " + ex.Message);
				}
				return false;
			}
		}

        private static string NormalizeMapName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return string.Empty;

            int bracket = name.IndexOf(" [", StringComparison.Ordinal);
            if (bracket >= 0)
                name = name.Substring(0, bracket);

            return name.Trim();
        }

        private static bool IsSameMap(string current, string target)
        {
            return string.Equals(
                NormalizeMapName(current),
                NormalizeMapName(target),
                StringComparison.OrdinalIgnoreCase);
        }
    }
}
