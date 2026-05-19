using System;

namespace SpiritMod
{
	// Token: 0x0200000C RID: 12
	public class BotStatus
	{
		// Token: 0x0600002B RID: 43 RVA: 0x00002DE0 File Offset: 0x00000FE0
		public void Reset()
		{
			this.State = BotState.Disabled;
			this.TargetName = string.Empty;
			this.TargetHealth = 0;
			this.TargetMaxHp = 0;
			this.PlayerHealth = 0;
			this.PlayerMaxHp = 0;
			this.PlayerHPNorm = 1f;
			this.KillCount = 0;
			this.LootCount = 0;
			this.SkillsCastCount = 0;
			this.ActionTimer = 0f;
			this.AttackTimer = 0f;
			this.SkillTimer = 0f;
			this.LootTimer = 0f;
			this.LootPhaseTimer = 0f;
			this.IsPlayerDead = false;
			this.ReviveTimer = 0f;
			this.WaitingForSceneLoad = false;
			this.WaitingForFullResources = false;
			this.FarmingReturnMap = string.Empty;
			this.ReviveTargetMap = string.Empty;
			this.ZoneCheckTimer = 0f;
			this.ZoneTeleportCooldown = 0f;
			this.ZonePendingTeleport = false;
			this.SellCount = 0;
			this.SellCooldown = 0f;
			this.SellPeriodicTimer = 0f;
			this.StoreCount = 0;
			this.StoreCooldown = 0f;
			this.StorePeriodicTimer = 0f;
			this.HealCount = 0;
			this.HealCooldown = 0f;
			this.AttackAttempts = 0;
			this.AttackRequestsSent = 0;
			this.SkillAttempts = 0;
			this.SkillRequestsSent = 0;
			this.AttackAttemptsPerSec = 0f;
			this.AttackRequestsPerSec = 0f;
			this.SkillAttemptsPerSec = 0f;
			this.SkillRequestsPerSec = 0f;
			this._benchWindowTimer = 0f;
			this._lastAttackAttempts = 0;
			this._lastAttackRequestsSent = 0;
			this._lastSkillAttempts = 0;
			this._lastSkillRequestsSent = 0;
			this.StuckRecoveries = 0;
			this.StuckSecondsObserved = 0f;
			this.LastRecoveryMessage = string.Empty;
			this.LastTargetPriorityMode = string.Empty;
			this.LastTargetDistance = 0f;
			this.LastLootDistance = 0f;
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00002FC0 File Offset: 0x000011C0
		public void TickBenchmark(float dt)
		{
			this._benchWindowTimer += dt;
			if (this._benchWindowTimer < 1f)
			{
				return;
			}
			float benchWindowTimer = this._benchWindowTimer;
			this.AttackAttemptsPerSec = (float)(this.AttackAttempts - this._lastAttackAttempts) / benchWindowTimer;
			this.AttackRequestsPerSec = (float)(this.AttackRequestsSent - this._lastAttackRequestsSent) / benchWindowTimer;
			this.SkillAttemptsPerSec = (float)(this.SkillAttempts - this._lastSkillAttempts) / benchWindowTimer;
			this.SkillRequestsPerSec = (float)(this.SkillRequestsSent - this._lastSkillRequestsSent) / benchWindowTimer;
			this._lastAttackAttempts = this.AttackAttempts;
			this._lastAttackRequestsSent = this.AttackRequestsSent;
			this._lastSkillAttempts = this.SkillAttempts;
			this._lastSkillRequestsSent = this.SkillRequestsSent;
			this._benchWindowTimer = 0f;
		}

		// Token: 0x04000041 RID: 65
		public BotState State;

		// Token: 0x04000042 RID: 66
		public string TargetName = string.Empty;

		// Token: 0x04000043 RID: 67
		public int TargetHealth;

		// Token: 0x04000044 RID: 68
		public int TargetMaxHp;

		// Token: 0x04000045 RID: 69
		public int PlayerHealth;

		// Token: 0x04000046 RID: 70
		public int PlayerMaxHp;

		// Token: 0x04000047 RID: 71
		public float PlayerHPNorm = 1f;

		// Token: 0x04000048 RID: 72
		public int KillCount;

		// Token: 0x04000049 RID: 73
		public int LootCount;

		// Token: 0x0400004A RID: 74
		public int SkillsCastCount;

		// Token: 0x0400004B RID: 75
		public int SellCount;

		// Token: 0x0400004C RID: 76
		public float ActionTimer;

		// Token: 0x0400004D RID: 77
		public float AttackTimer;

		// Token: 0x0400004E RID: 78
		public float SkillTimer;

		// Token: 0x0400004F RID: 79
		public float LootTimer;

		// Token: 0x04000050 RID: 80
		public float LootPhaseTimer;

		// Token: 0x04000051 RID: 81
		public bool IsPlayerDead;

		// Token: 0x04000052 RID: 82
		public float ReviveTimer;

		// Token: 0x04000053 RID: 83
		public bool WaitingForSceneLoad;

		// Token: 0x04000054 RID: 84
		public bool WaitingForFullResources;

		// Token: 0x04000055 RID: 85
		public string FarmingReturnMap;

		// Token: 0x04000056 RID: 86
		public string ReviveTargetMap;

		// Token: 0x04000057 RID: 87
		public float ZoneCheckTimer;

		// Token: 0x04000058 RID: 88
		public float ZoneTeleportCooldown;

		// Token: 0x04000059 RID: 89
		public bool ZonePendingTeleport;

		// Token: 0x0400005A RID: 90
		public float SellCooldown;

		// Token: 0x0400005B RID: 91
		public float SellPeriodicTimer;

		// Token: 0x0400005C RID: 92
		public int StoreCount;

		// Token: 0x0400005D RID: 93
		public float StoreCooldown;

		// Token: 0x0400005E RID: 94
		public float StorePeriodicTimer;

		// Token: 0x0400005F RID: 95
		public int HealCount;

		// Token: 0x04000060 RID: 96
		public float HealCooldown;

		// Token: 0x04000061 RID: 97
		public int AttackAttempts;

		// Token: 0x04000062 RID: 98
		public int AttackRequestsSent;

		// Token: 0x04000063 RID: 99
		public int SkillAttempts;

		// Token: 0x04000064 RID: 100
		public int SkillRequestsSent;

		// Token: 0x04000065 RID: 101
		public float AttackAttemptsPerSec;

		// Token: 0x04000066 RID: 102
		public float AttackRequestsPerSec;

		// Token: 0x04000067 RID: 103
		public float SkillAttemptsPerSec;

		// Token: 0x04000068 RID: 104
		public float SkillRequestsPerSec;

		// Token: 0x04000069 RID: 105
		private float _benchWindowTimer;

		// Token: 0x0400006A RID: 106
		private int _lastAttackAttempts;

		// Token: 0x0400006B RID: 107
		private int _lastAttackRequestsSent;

		// Token: 0x0400006C RID: 108
		private int _lastSkillAttempts;

		// Token: 0x0400006D RID: 109
		private int _lastSkillRequestsSent;

		// Token: 0x0400006E RID: 110
		public int StuckRecoveries;

		// Token: 0x0400006F RID: 111
		public float StuckSecondsObserved;

		// Token: 0x04000070 RID: 112
		public string LastRecoveryMessage = string.Empty;

		// Token: 0x04000071 RID: 113
		public string LastTargetPriorityMode = string.Empty;

		// Token: 0x04000072 RID: 114
		public float LastTargetDistance;

		// Token: 0x04000073 RID: 115
		public float LastLootDistance;
	}
}
