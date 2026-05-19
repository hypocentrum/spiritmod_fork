using System;

namespace SpiritMod
{
	// Token: 0x02000012 RID: 18
	public class SkillSlotInfo
	{
		// Token: 0x0400008A RID: 138
		public bool Assigned;

		// Token: 0x0400008B RID: 139
		public string Name = string.Empty;

		// Token: 0x0400008C RID: 140
		public int ManaCost;

		// Token: 0x0400008D RID: 141
		public bool IsOnCooldown;

		// Token: 0x0400008E RID: 142
		public int TargetType = -1;

		// Token: 0x0400008F RID: 143
		public bool IsHealing;

		// Token: 0x04000090 RID: 144
		public bool IsSummon;

		// Token: 0x04000091 RID: 145
		public bool IsBuff;

		// Token: 0x04000092 RID: 146
		public bool IsBond;

		// Token: 0x04000093 RID: 147
		public bool IsMount;
	}
}
