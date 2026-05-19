using System;

namespace SpiritMod
{
	// Token: 0x0200000A RID: 10
	public class BotConfig
	{
		// Token: 0x06000024 RID: 36 RVA: 0x00002B28 File Offset: 0x00000D28
		private static bool[] CreateDefaultEnabled()
		{
			bool[] array = new bool[20];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = true;
			}
			return array;
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00002B50 File Offset: 0x00000D50
		private static float[] CreateDefaultBuffRefreshLead()
		{
			float[] array = new float[20];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = 2f;
			}
			return array;
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00002B7C File Offset: 0x00000D7C
		public void EnsureArrays()
		{
			BotConfig.EnsureArray(ref this.EnabledSkillSlots, true);
			BotConfig.EnsureArray(ref this.SkillPriorities);
			BotConfig.EnsureArray(ref this.TreatAsHealing, false);
			BotConfig.EnsureArray(ref this.TreatAsBuff, false);
			BotConfig.EnsureArray(ref this.HealingThresholds);
			BotConfig.EnsureArray(ref this.EnabledBuffSlots, true);
			BotConfig.EnsureArray(ref this.BuffRefreshLeadSeconds, 2f);
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002BE0 File Offset: 0x00000DE0
		private static void EnsureArray(ref bool[] arr, bool defaultValue = false)
		{
			if (arr != null && arr.Length == 20)
			{
				return;
			}
			bool[] array = new bool[20];
			if (defaultValue)
			{
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = true;
				}
			}
			if (arr != null)
			{
				Array.Copy(arr, array, Math.Min(arr.Length, array.Length));
			}
			arr = array;
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002C34 File Offset: 0x00000E34
		private static void EnsureArray(ref int[] arr)
		{
			if (arr != null && arr.Length == 20)
			{
				return;
			}
			int[] array = new int[20];
			if (arr != null)
			{
				Array.Copy(arr, array, Math.Min(arr.Length, array.Length));
			}
			arr = array;
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00002C74 File Offset: 0x00000E74
		private static void EnsureArray(ref float[] arr, float defaultValue = 0f)
		{
			if (arr != null && arr.Length == 20)
			{
				return;
			}
			float[] array = new float[20];
			if (defaultValue != 0f)
			{
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = defaultValue;
				}
			}
			if (arr != null)
			{
				Array.Copy(arr, array, Math.Min(arr.Length, array.Length));
			}
			arr = array;
		}

		// Token: 0x04000015 RID: 21
		public const int MaxSkillSlots = 20;

		// Token: 0x04000016 RID: 22
		public const int DefaultPriority = 50;

		// Token: 0x04000017 RID: 23
		public const int HealingPriorityBoost = 100;

		// Token: 0x04000018 RID: 24
		public const int DefaultHealThreshold = 90;

		// Token: 0x04000019 RID: 25
		public float ActionInterval = 1f;

		// Token: 0x0400001A RID: 26
		public float AttackInterval = 1.5f;

		// Token: 0x0400001B RID: 27
		public float SearchRange = 100f;

		// Token: 0x0400001C RID: 28
		public bool EnableLooting;

		// Token: 0x0400001D RID: 29
		public float LootDelay = 0.5f;

		// Token: 0x0400001E RID: 30
		public float LootRange = 20f;

		// Token: 0x0400001F RID: 31
		public bool EnableSkills = true;

		// Token: 0x04000020 RID: 32
		public float SkillInterval = 0.1f;

		// Token: 0x04000021 RID: 33
		public bool[] EnabledSkillSlots = BotConfig.CreateDefaultEnabled();

		// Token: 0x04000022 RID: 34
		public int[] SkillPriorities = new int[20];

		// Token: 0x04000023 RID: 35
		public bool[] TreatAsHealing = new bool[20];

		// Token: 0x04000024 RID: 36
		public int[] HealingThresholds = new int[20];

		// Token: 0x04000025 RID: 37
		public bool EnableBuffMaintenance = true;

		// Token: 0x04000026 RID: 38
		public bool[] TreatAsBuff = new bool[20];

		// Token: 0x04000027 RID: 39
		public bool[] EnabledBuffSlots = BotConfig.CreateDefaultEnabled();

		// Token: 0x04000028 RID: 40
		public float[] BuffRefreshLeadSeconds = BotConfig.CreateDefaultBuffRefreshLead();

		// Token: 0x04000029 RID: 41
		public bool EnableAutoDashEvade;

		// Token: 0x0400002A RID: 42
		public float AutoDashTriggerDistance = 8f;

		// Token: 0x0400002B RID: 43
		public float AutoDashCooldown = 2f;

		// Token: 0x0400002C RID: 44
		public float DodgeLeadTime = 0.35f;

		// Token: 0x0400002D RID: 45
		public float DodgeFallbackDelay = 0.5f;

		// Token: 0x0400002E RID: 46
		public bool DodgeSkipAttached = true;

		// Token: 0x0400002F RID: 47
		public bool DodgeCheckAOERadius = true;

		// Token: 0x04000030 RID: 48
		public bool EnableAfkLeech;

		// Token: 0x04000031 RID: 49
		public bool EnableAutoRevive;

		// Token: 0x04000032 RID: 50
		public int FarmingMapIndex;

		// Token: 0x04000033 RID: 51
		public string FarmingMapName = string.Empty;

		// Token: 0x04000034 RID: 52
		public float ReviveDelay = 1f;

		// Token: 0x04000035 RID: 53
		public bool EnableAutoSell;

		// Token: 0x04000036 RID: 54
		public bool EnableAutoStore;

		// Token: 0x04000037 RID: 55
		public bool EnableAutoLogin;

		// Token: 0x04000038 RID: 56
		public string AutoLoginServerName = "";
	}
}
