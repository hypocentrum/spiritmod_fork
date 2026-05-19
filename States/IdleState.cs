using System;
using System.Collections.Generic;
using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace SpiritMod.States
{
	// Token: 0x0200002D RID: 45
	public class IdleState : IBotState
	{
		// Token: 0x17000036 RID: 54
		// (get) Token: 0x0600014A RID: 330 RVA: 0x0000F10E File Offset: 0x0000D30E
		// (set) Token: 0x0600014B RID: 331 RVA: 0x0000F115 File Offset: 0x0000D315
		public static TargetPriorityMode TargetPriority { get; set; }

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x0600014C RID: 332 RVA: 0x0000F11D File Offset: 0x0000D31D
		public BotState Id
		{
			get
			{
				return BotState.Idle;
			}
		}

		// Token: 0x0600014D RID: 333 RVA: 0x0000F120 File Offset: 0x0000D320
		public void Enter(BotContext ctx)
		{
		}

		// Token: 0x0600014E RID: 334 RVA: 0x0000F124 File Offset: 0x0000D324
		public void Tick(BotContext ctx)
		{
			ctx.Status.ActionTimer -= ctx.DeltaTime;
			if (ctx.Status.ActionTimer > 0f)
			{
				return;
			}
			ctx.Status.ActionTimer = ctx.Config.ActionInterval;
			if (ctx.Config.EnableAfkLeech)
			{
				return;
			}
			GameStateService.TryScanMonsters();
			List<MonsterInfo> monsters = GameStateService.Monsters;
			if (monsters.Count == 0)
			{
				return;
			}
			PlayerController player = ctx.Player;
			if (player == null)
			{
				return;
			}
			try
			{
				Vector3 position = player.Cast<BaseUnitController>().Position;
				BaseUnitController baseUnitController = null;
				float num = float.MaxValue;
				float num2 = float.MaxValue;
				string text = null;
				foreach (MonsterInfo monsterInfo in monsters)
				{
					if (CombatService.IsTargetAlive(monsterInfo.Controller))
					{
						float sqrMagnitude = (position - monsterInfo.Position).sqrMagnitude;
						int num3;
						int num4;
						CombatService.ReadTargetHealth(monsterInfo.Controller, out num3, out num4);
						float hpPct = (num4 > 0) ? ((float)num3 / (float)num4) : 1f;
						float num5 = IdleState.ScoreTarget(sqrMagnitude, num3, hpPct);
						if (num5 < num)
						{
							num = num5;
							num2 = sqrMagnitude;
							baseUnitController = monsterInfo.Controller;
							text = monsterInfo.Name;
						}
					}
				}
				if (!(baseUnitController == null))
				{
					if (ctx.Config.EnableSkills)
					{
						Il2CppSystem.Collections.Generic.List<int> missingSummonSlots = SummonService.GetMissingSummonSlots(player, ctx.Config);
						if (missingSummonSlots.Count > 0)
						{
							if (CombatService.TryCastSkill(player, ctx.Config, ctx.Status))
							{
								MelonLogger.Msg("[Bot] Summoning (missing slots: " + string.Join(",", missingSummonSlots) + ")");
							}
							return;
						}
					}
					CombatService.ClickTarget(player, baseUnitController);
					BotController.SetTarget(baseUnitController);
					ctx.Status.TargetName = (text ?? "???");
					ctx.Status.LastTargetPriorityMode = IdleState.TargetPriority.ToString();
					ctx.Status.LastTargetDistance = Mathf.Sqrt(num2);
					ctx.Status.AttackTimer = ctx.Config.AttackInterval;
					BotController.TransitionTo(BotState.Combat);
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Warning("[Bot] TickIdle failed: " + ex.Message);
			}
		}

		// Token: 0x0600014F RID: 335 RVA: 0x0000F39C File Offset: 0x0000D59C
		public void Exit(BotContext ctx)
		{
		}

		// Token: 0x06000150 RID: 336 RVA: 0x0000F3A0 File Offset: 0x0000D5A0
		private static float ScoreTarget(float sqrDistance, int hp, float hpPct)
		{
			switch (IdleState.TargetPriority)
			{
			case TargetPriorityMode.LowestHpPercent:
				return hpPct * 100000f + sqrDistance * 0.001f;
			case TargetPriorityMode.LowestHp:
				return (float)Math.Max(0, hp) * 10f + sqrDistance * 0.001f;
			case TargetPriorityMode.Hybrid:
				return sqrDistance * 0.65f + hpPct * 35f;
			default:
				return sqrDistance;
			}
		}
	}
}
