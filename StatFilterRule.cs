using System;

namespace SpiritMod
{
	// Token: 0x02000017 RID: 23
	public class StatFilterRule
	{
		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600009E RID: 158 RVA: 0x0000A4BA File Offset: 0x000086BA
		// (set) Token: 0x0600009F RID: 159 RVA: 0x0000A4C2 File Offset: 0x000086C2
		public string StatType { get; set; } = "";

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x060000A0 RID: 160 RVA: 0x0000A4CB File Offset: 0x000086CB
		// (set) Token: 0x060000A1 RID: 161 RVA: 0x0000A4D3 File Offset: 0x000086D3
		public int Condition { get; set; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x060000A2 RID: 162 RVA: 0x0000A4DC File Offset: 0x000086DC
		// (set) Token: 0x060000A3 RID: 163 RVA: 0x0000A4E4 File Offset: 0x000086E4
		public int Value { get; set; }
	}
}
