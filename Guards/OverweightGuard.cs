using System;
using System.Runtime.CompilerServices;
using Il2Cpp;
using MelonLoader;

namespace SpiritMod.Guards
{
	// Token: 0x02000037 RID: 55
	public class OverweightGuard : IBotGuard
	{
		// Token: 0x06000176 RID: 374 RVA: 0x000103DC File Offset: 0x0000E5DC
		public bool Evaluate(BotContext ctx)
		{
			return ctx.Status.State != BotState.Reviving && ((ctx.Config.EnableAutoStore && ctx.Status.State != BotState.Storing && ctx.Status.State != BotState.Selling && OverweightGuard.EvaluateStore(ctx)) || (ctx.Config.EnableAutoSell && ctx.Status.State != BotState.Selling && ctx.Status.State != BotState.Storing && OverweightGuard.EvaluateSell(ctx)));
		}

		// Token: 0x06000177 RID: 375 RVA: 0x00010460 File Offset: 0x0000E660
		private static bool EvaluateStore(BotContext ctx)
		{
			ctx.Status.StoreCooldown -= ctx.DeltaTime;
			ctx.Status.StorePeriodicTimer -= ctx.DeltaTime;
			if (ctx.Status.StoreCooldown > 0f)
			{
				return false;
			}
			bool flag = false;
			string str = null;
			try
			{
				PlayerController player = ctx.Player;
				if (player != null && Formula.IsOverweight(player))
				{
					flag = true;
					str = "overweight";
				}
			}
			catch
			{
			}
			if (!flag && ctx.Status.StorePeriodicTimer <= 0f)
			{
				flag = true;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(12, 1);
				defaultInterpolatedStringHandler.AppendLiteral("periodic (");
				defaultInterpolatedStringHandler.AppendFormatted<float>(30f);
				defaultInterpolatedStringHandler.AppendLiteral("s)");
				str = defaultInterpolatedStringHandler.ToStringAndClear();
				ctx.Status.StorePeriodicTimer = 30f;
			}
			if (flag)
			{
				MelonLogger.Msg("[Bot] Auto-store triggered (" + str + ") → STORING");
				BotController.ClearTarget();
				BotController.TransitionTo(BotState.Storing);
				return true;
			}
			return false;
		}

		// Token: 0x06000178 RID: 376 RVA: 0x00010570 File Offset: 0x0000E770
		private static bool EvaluateSell(BotContext ctx)
		{
			ctx.Status.SellCooldown -= ctx.DeltaTime;
			ctx.Status.SellPeriodicTimer -= ctx.DeltaTime;
			if (ctx.Status.SellCooldown > 0f)
			{
				return false;
			}
			bool flag = false;
			string str = null;
			try
			{
				PlayerController player = ctx.Player;
				if (player != null && Formula.IsOverweight(player))
				{
					flag = true;
					str = "overweight";
				}
			}
			catch
			{
			}
			if (!flag && ctx.Status.SellPeriodicTimer <= 0f)
			{
				flag = true;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(12, 1);
				defaultInterpolatedStringHandler.AppendLiteral("periodic (");
				defaultInterpolatedStringHandler.AppendFormatted<float>(60f);
				defaultInterpolatedStringHandler.AppendLiteral("s)");
				str = defaultInterpolatedStringHandler.ToStringAndClear();
				ctx.Status.SellPeriodicTimer = 60f;
			}
			if (flag)
			{
				MelonLogger.Msg("[Bot] Auto-sell triggered (" + str + ") → SELLING");
				BotController.ClearTarget();
				BotController.TransitionTo(BotState.Selling);
				return true;
			}
			return false;
		}
	}
}
