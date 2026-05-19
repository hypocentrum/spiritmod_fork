using System;

namespace SpiritMod.States
{
	// Token: 0x0200002C RID: 44
	public interface IBotState
	{
		// Token: 0x17000035 RID: 53
		// (get) Token: 0x06000146 RID: 326
		BotState Id { get; }

		// Token: 0x06000147 RID: 327
		void Enter(BotContext ctx);

		// Token: 0x06000148 RID: 328
		void Tick(BotContext ctx);

		// Token: 0x06000149 RID: 329
		void Exit(BotContext ctx);
	}
}
