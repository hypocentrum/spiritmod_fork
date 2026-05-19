using System;
using System.Runtime.CompilerServices;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SpiritMod
{
	// Token: 0x02000027 RID: 39
	public static class StuckRecoveryService
	{
		// Token: 0x0600011F RID: 287 RVA: 0x0000D42D File Offset: 0x0000B62D
		public static void Reset()
		{
			StuckRecoveryService._lastPosition = Vector3.zero;
			StuckRecoveryService._stillTimer = 0f;
			StuckRecoveryService._cooldownTimer = 0f;
			StuckRecoveryService._hasLastPosition = false;
		}

		// Token: 0x06000120 RID: 288 RVA: 0x0000D454 File Offset: 0x0000B654
		public static bool Tick(BotContext ctx)
		{
			if (!StuckRecoveryService.Enabled)
			{
				return false;
			}
			BotState state = ctx.Status.State;
			if (state == BotState.Reviving || state == BotState.Selling || state == BotState.Storing || state == BotState.Disabled)
			{
				return false;
			}
			PlayerController player = ctx.Player;
			if (player == null)
			{
				return false;
			}
			Vector3 position;
			try
			{
				position = player.Cast<BaseUnitController>().Position;
			}
			catch
			{
				return false;
			}
			float deltaTime = ctx.DeltaTime;
			if (StuckRecoveryService._cooldownTimer > 0f)
			{
				StuckRecoveryService._cooldownTimer -= deltaTime;
				StuckRecoveryService._stillTimer = 0f;
				StuckRecoveryService._lastPosition = position;
				return false;
			}
			if (!StuckRecoveryService._hasLastPosition)
			{
				StuckRecoveryService._lastPosition = position;
				StuckRecoveryService._hasLastPosition = true;
				return false;
			}
			float num = Vector3.Distance(position, StuckRecoveryService._lastPosition);
			StuckRecoveryService._lastPosition = position;
			if (num >= StuckRecoveryService.MoveThreshold)
			{
				StuckRecoveryService._stillTimer = 0f;
				return false;
			}
			StuckRecoveryService._stillTimer += deltaTime;
			ctx.Status.StuckSecondsObserved = StuckRecoveryService._stillTimer;
			if (StuckRecoveryService._stillTimer < StuckRecoveryService.StuckSeconds)
			{
				return false;
			}
			StuckRecoveryService._stillTimer = 0f;
			StuckRecoveryService._cooldownTimer = StuckRecoveryService.CooldownSeconds;
			ctx.Status.StuckRecoveries++;
			try
			{
				BotController.ClearTarget();
				ctx.Status.TargetName = string.Empty;
				Vector2 vector = Random.insideUnitCircle * 5f;
				Vector3 vector2 = position + new Vector3(vector.x, 0f, vector.y);
				CombatService.ClickPosition(player, vector2);
				BotStatus status = ctx.Status;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(15, 3);
				defaultInterpolatedStringHandler.AppendLiteral("[");
				defaultInterpolatedStringHandler.AppendFormatted<DateTime>(DateTime.Now, "HH:mm:ss");
				defaultInterpolatedStringHandler.AppendLiteral("] Moved to (");
				defaultInterpolatedStringHandler.AppendFormatted<float>(vector2.x, "F0");
				defaultInterpolatedStringHandler.AppendLiteral(",");
				defaultInterpolatedStringHandler.AppendFormatted<float>(vector2.z, "F0");
				defaultInterpolatedStringHandler.AppendLiteral(")");
				status.LastRecoveryMessage = defaultInterpolatedStringHandler.ToStringAndClear();
				BotController.TransitionTo(BotState.Idle);
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(64, 1);
				defaultInterpolatedStringHandler.AppendLiteral("[StuckRecovery] Triggered after ");
				defaultInterpolatedStringHandler.AppendFormatted<float>(StuckRecoveryService.StuckSeconds, "F1");
				defaultInterpolatedStringHandler.AppendLiteral("s still — moving to random point");
				MelonLogger.Msg(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			catch (Exception ex)
			{
				BotStatus status2 = ctx.Status;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 2);
				defaultInterpolatedStringHandler.AppendLiteral("[");
				defaultInterpolatedStringHandler.AppendFormatted<DateTime>(DateTime.Now, "HH:mm:ss");
				defaultInterpolatedStringHandler.AppendLiteral("] Error: ");
				defaultInterpolatedStringHandler.AppendFormatted(ex.Message);
				status2.LastRecoveryMessage = defaultInterpolatedStringHandler.ToStringAndClear();
				MelonLogger.Warning("[StuckRecovery] Recovery failed: " + ex.Message);
			}
			return true;
		}

		// Token: 0x040000EF RID: 239
		public static bool Enabled = true;

		// Token: 0x040000F0 RID: 240
		public static float StuckSeconds = 4f;

		// Token: 0x040000F1 RID: 241
		public static float CooldownSeconds = 5f;

		// Token: 0x040000F2 RID: 242
		public static float MoveThreshold = 1.5f;

		// Token: 0x040000F3 RID: 243
		private static Vector3 _lastPosition;

		// Token: 0x040000F4 RID: 244
		private static float _stillTimer;

		// Token: 0x040000F5 RID: 245
		private static float _cooldownTimer;

		// Token: 0x040000F6 RID: 246
		private static bool _hasLastPosition;
	}
}
