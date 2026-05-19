using System;
using System.Runtime.CompilerServices;
using MelonLoader;

namespace SpiritMod.States
{
	// Token: 0x02000032 RID: 50
	public class StoringState : IBotState
	{
		// Token: 0x1700003C RID: 60
		// (get) Token: 0x06000169 RID: 361 RVA: 0x0000FF76 File Offset: 0x0000E176
		public BotState Id
		{
			get
			{
				return BotState.Storing;
			}
		}

		// Token: 0x0600016A RID: 362 RVA: 0x0000FF79 File Offset: 0x0000E179
		public void Enter(BotContext ctx)
		{
		}

		// Token: 0x0600016B RID: 363 RVA: 0x0000FF7C File Offset: 0x0000E17C
		public void Tick(BotContext ctx)
		{
			try
			{
				int num = StorageService.StoreFilteredItems();
				ctx.Status.StoreCount += num;
				if (num > 0)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(25, 1);
					defaultInterpolatedStringHandler.AppendLiteral("[Bot] Stored ");
					defaultInterpolatedStringHandler.AppendFormatted<int>(num);
					defaultInterpolatedStringHandler.AppendLiteral(" item stacks");
					MelonLogger.Msg(defaultInterpolatedStringHandler.ToStringAndClear());
				}
				else
				{
					MelonLogger.Msg("[Bot] No items to store");
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Warning("[Bot] TickStoring failed: " + ex.Message);
			}
			ctx.Status.StoreCooldown = 5f;
			BotController.TransitionTo(BotState.Idle);
			ctx.Status.ActionTimer = 0.5f;
		}

		// Token: 0x0600016C RID: 364 RVA: 0x00010038 File Offset: 0x0000E238
		public void Exit(BotContext ctx)
		{
		}
	}
}
