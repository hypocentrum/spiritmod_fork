using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SpiritMod
{
	// Token: 0x02000026 RID: 38
	public static class CombatTelemetryService
	{
		// Token: 0x1700002D RID: 45
		// (get) Token: 0x06000114 RID: 276 RVA: 0x0000D2D0 File Offset: 0x0000B4D0
		// (set) Token: 0x06000115 RID: 277 RVA: 0x0000D2D7 File Offset: 0x0000B4D7
		public static bool Enabled { get; set; } = true;

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x06000116 RID: 278 RVA: 0x0000D2DF File Offset: 0x0000B4DF
		// (set) Token: 0x06000117 RID: 279 RVA: 0x0000D2E6 File Offset: 0x0000B4E6
		public static string LastEvent { get; private set; } = "n/a";

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x06000118 RID: 280 RVA: 0x0000D2EE File Offset: 0x0000B4EE
		public static IReadOnlyDictionary<string, int> Counters
		{
			get
			{
				return CombatTelemetryService._counters;
			}
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x06000119 RID: 281 RVA: 0x0000D2F5 File Offset: 0x0000B4F5
		public static string[] Recent
		{
			get
			{
				return CombatTelemetryService._recent;
			}
		}

		// Token: 0x0600011A RID: 282 RVA: 0x0000D2FC File Offset: 0x0000B4FC
		public static void Reset()
		{
			CombatTelemetryService.LastEvent = "n/a";
			CombatTelemetryService._recentIndex = 0;
			for (int i = 0; i < CombatTelemetryService._recent.Length; i++)
			{
				CombatTelemetryService._recent[i] = null;
			}
			CombatTelemetryService._counters.Clear();
		}

		// Token: 0x0600011B RID: 283 RVA: 0x0000D33D File Offset: 0x0000B53D
		public static void RecordBlocked(string reason)
		{
			if (!CombatTelemetryService.Enabled)
			{
				return;
			}
			CombatTelemetryService.Record("BLOCK", reason);
		}

		// Token: 0x0600011C RID: 284 RVA: 0x0000D352 File Offset: 0x0000B552
		public static void RecordSent(string detail)
		{
			if (!CombatTelemetryService.Enabled)
			{
				return;
			}
			CombatTelemetryService.Record("SEND", detail);
		}

		// Token: 0x0600011D RID: 285 RVA: 0x0000D368 File Offset: 0x0000B568
		private static void Record(string kind, string detail)
		{
			string text = kind + ":" + (detail ?? "unknown");
			CombatTelemetryService.LastEvent = text;
			int num;
			CombatTelemetryService._counters.TryGetValue(text, out num);
			CombatTelemetryService._counters[text] = num + 1;
			string[] recent = CombatTelemetryService._recent;
			int recentIndex = CombatTelemetryService._recentIndex;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
			defaultInterpolatedStringHandler.AppendFormatted<DateTime>(DateTime.Now, "HH:mm:ss");
			defaultInterpolatedStringHandler.AppendLiteral(" ");
			defaultInterpolatedStringHandler.AppendFormatted(text);
			recent[recentIndex] = defaultInterpolatedStringHandler.ToStringAndClear();
			CombatTelemetryService._recentIndex = (CombatTelemetryService._recentIndex + 1) % CombatTelemetryService._recent.Length;
		}

		// Token: 0x040000EA RID: 234
		private static readonly Dictionary<string, int> _counters = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

		// Token: 0x040000EB RID: 235
		private static readonly string[] _recent = new string[8];

		// Token: 0x040000EC RID: 236
		private static int _recentIndex;
	}
}
