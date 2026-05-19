using System;
using Il2Cpp;

namespace SpiritMod
{
	// Token: 0x0200000D RID: 13
	public sealed class BotContext
	{
		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600002E RID: 46 RVA: 0x000030B7 File Offset: 0x000012B7
		// (set) Token: 0x0600002F RID: 47 RVA: 0x000030BF File Offset: 0x000012BF
		public PlayerController Player { get; set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000030 RID: 48 RVA: 0x000030C8 File Offset: 0x000012C8
		// (set) Token: 0x06000031 RID: 49 RVA: 0x000030D0 File Offset: 0x000012D0
		public BotConfig Config { get; set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000032 RID: 50 RVA: 0x000030D9 File Offset: 0x000012D9
		// (set) Token: 0x06000033 RID: 51 RVA: 0x000030E1 File Offset: 0x000012E1
		public BotStatus Status { get; set; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000034 RID: 52 RVA: 0x000030EA File Offset: 0x000012EA
		// (set) Token: 0x06000035 RID: 53 RVA: 0x000030F2 File Offset: 0x000012F2
		public float DeltaTime { get; set; }
	}
}
