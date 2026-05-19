using System;
using System.Collections.Generic;

namespace SpiritMod.Guards
{
	// Token: 0x02000034 RID: 52
	public class GuardPipeline
	{
		// Token: 0x0600016F RID: 367 RVA: 0x00010042 File Offset: 0x0000E242
		public GuardPipeline Add(IBotGuard guard)
		{
			this._guards.Add(guard);
			return this;
		}

		// Token: 0x06000170 RID: 368 RVA: 0x00010054 File Offset: 0x0000E254
		public bool Evaluate(BotContext ctx)
		{
			using (List<IBotGuard>.Enumerator enumerator = this._guards.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Evaluate(ctx))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x04000120 RID: 288
		private readonly List<IBotGuard> _guards = new List<IBotGuard>();
	}
}
