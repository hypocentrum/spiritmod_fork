using System;
using System.Runtime.CompilerServices;
using MelonLoader;

namespace SpiritMod.States
{
	// Token: 0x02000031 RID: 49
	public class SellingState : IBotState
	{
		// Token: 0x1700003B RID: 59
		// (get) Token: 0x06000164 RID: 356 RVA: 0x0000FEAA File Offset: 0x0000E0AA
		public BotState Id
		{
			get
			{
				return BotState.Selling;
			}
		}

		// Token: 0x06000165 RID: 357 RVA: 0x0000FEAD File Offset: 0x0000E0AD
		public void Enter(BotContext ctx)
		{
		}

		// Token: 0x06000166 RID: 358 RVA: 0x0000FEB0 File Offset: 0x0000E0B0
		public void Tick(BotContext ctx)
		{
			try
			{
				int num = MerchantService.SellFilteredItems();
				ctx.Status.SellCount += num;
				if (num > 0)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(23, 1);
					defaultInterpolatedStringHandler.AppendLiteral("[Bot] Sold ");
					defaultInterpolatedStringHandler.AppendFormatted<int>(num);
					defaultInterpolatedStringHandler.AppendLiteral(" item stacks");
					MelonLogger.Msg(defaultInterpolatedStringHandler.ToStringAndClear());
				}
				else
				{
					MelonLogger.Msg("[Bot] No items to sell");
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Warning("[Bot] TickSelling failed: " + ex.Message);
			}
			ctx.Status.SellCooldown = 5f;
			BotController.TransitionTo(BotState.Idle);
			ctx.Status.ActionTimer = 0.5f;
		}

		// Token: 0x06000167 RID: 359 RVA: 0x0000FF6C File Offset: 0x0000E16C
		public void Exit(BotContext ctx)
		{
		}
	}
}
