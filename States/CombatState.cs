using System;
using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace SpiritMod.States
{
	// Token: 0x0200002E RID: 46
	public class CombatState : IBotState
	{
		// Token: 0x17000038 RID: 56
		// (get) Token: 0x06000152 RID: 338 RVA: 0x0000F409 File Offset: 0x0000D609
		public BotState Id
		{
			get
			{
				return BotState.Combat;
			}
		}

		// Token: 0x06000153 RID: 339 RVA: 0x0000F40C File Offset: 0x0000D60C
		public void Enter(BotContext ctx)
		{
			this._dashEvadeCooldownTimer = 0f;
			this._fallbackCastStartTime = 0f;
			this._fallbackCastTracking = false;
			this._safetyReClickTimer = 0f;
		}

		// Token: 0x06000154 RID: 340 RVA: 0x0000F438 File Offset: 0x0000D638
		public void Tick(BotContext ctx)
		{
			BaseUnitController currentTarget = BotController.CurrentTarget;
			if (!CombatService.IsTargetAlive(currentTarget))
			{
				ctx.Status.KillCount++;
				BotController.ClearTarget();
				ctx.Status.TargetName = string.Empty;
				ctx.Status.TargetHealth = 0;
				ctx.Status.TargetMaxHp = 0;
				if (ctx.Config.EnableLooting)
				{
					BotController.TransitionTo(BotState.Looting);
					ctx.Status.LootTimer = 0f;
					ctx.Status.LootPhaseTimer = 0f;
					return;
				}
				BotController.TransitionTo(BotState.Idle);
				ctx.Status.ActionTimer = 0.2f;
				return;
			}
			else
			{
				CombatService.ReadTargetHealth(currentTarget, out ctx.Status.TargetHealth, out ctx.Status.TargetMaxHp);
				PlayerController player = ctx.Player;
				if (player == null)
				{
					return;
				}
				try
				{
					CombatService.ReadTargetHealth(player.Cast<BaseUnitController>(), out ctx.Status.PlayerHealth, out ctx.Status.PlayerMaxHp);
					ctx.Status.PlayerHPNorm = CombatService.GetPlayerHPNormalised(player);
				}
				catch
				{
				}
				this.TryAutoDashEvade(ctx, player, currentTarget);
				if (ctx.Config.EnableSkills)
				{
					ctx.Status.SkillTimer -= ctx.DeltaTime;
					if (ctx.Status.SkillTimer <= 0f)
					{
						ctx.Status.SkillTimer = ctx.Config.SkillInterval;
						try
						{
							ctx.Status.SkillAttempts++;
							CombatService.TryCastSkill(player, ctx.Config, ctx.Status);
						}
						catch (Exception ex)
						{
							MelonLogger.Warning("[Bot] TryCastSkill error: " + ex.Message);
						}
					}
				}
				ctx.Status.AttackTimer -= ctx.DeltaTime;
				if (ctx.Status.AttackTimer > 0f)
				{
					return;
				}
				ctx.Status.AttackTimer = ctx.Config.AttackInterval;
				try
				{
					ctx.Status.AttackAttempts++;
					this._safetyReClickTimer -= ctx.Config.AttackInterval;
					if (!CombatService.IsAlreadyEngaged(player, currentTarget) || this._safetyReClickTimer <= 0f)
					{
						CombatService.ClickTarget(player, currentTarget);
						ctx.Status.AttackRequestsSent++;
						this._safetyReClickTimer = 10f;
					}
				}
				catch (Exception ex2)
				{
					MelonLogger.Warning("[Bot] Re-click failed: " + ex2.Message);
				}
				return;
			}
		}

		// Token: 0x06000155 RID: 341 RVA: 0x0000F6D0 File Offset: 0x0000D8D0
		public void Exit(BotContext ctx)
		{
			this._dashEvadeCooldownTimer = 0f;
			this._fallbackCastTracking = false;
			this._safetyReClickTimer = 0f;
		}

		// Token: 0x06000156 RID: 342 RVA: 0x0000F6F0 File Offset: 0x0000D8F0
		private void TryAutoDashEvade(BotContext ctx, PlayerController player, BaseUnitController currentTarget)
		{
			if (((ctx != null) ? ctx.Config : null) == null || !ctx.Config.EnableAutoDashEvade)
			{
				return;
			}
			this._dashEvadeCooldownTimer -= ctx.DeltaTime;
			if (this._dashEvadeCooldownTimer > 0f)
			{
				return;
			}
			bool flag = false;
			try
			{
				SkillsComponent skillsComponent = (currentTarget != null) ? currentTarget.Skills : null;
				if (skillsComponent != null)
				{
					float castTime = skillsComponent.CastTime;
					flag = ((skillsComponent.CastTimeMax > 0f && castTime > 0f) || skillsComponent.IsCasting);
				}
			}
			catch
			{
			}
			if (!flag)
			{
				this._fallbackCastTracking = false;
				return;
			}
			if (!this._fallbackCastTracking)
			{
				this._fallbackCastTracking = true;
				this._fallbackCastStartTime = Time.time;
			}
			float num = Time.time - this._fallbackCastStartTime;
			Mathf.Max(0.1f, ctx.Config.DodgeFallbackDelay);
			switch (CombatService.TryAutoDashEvade(player, currentTarget, ctx.Config, ctx.Status))
			{
			case CombatService.DodgeResult.None:
			case CombatService.DodgeResult.Pending:
				break;
			case CombatService.DodgeResult.Executed:
				this._dashEvadeCooldownTimer = Mathf.Max(0.5f, ctx.Config.AutoDashCooldown);
				this._fallbackCastTracking = false;
				break;
			default:
				return;
			}
		}

		// Token: 0x0400011B RID: 283
		private float _dashEvadeCooldownTimer;

		// Token: 0x0400011C RID: 284
		private float _fallbackCastStartTime;

		// Token: 0x0400011D RID: 285
		private bool _fallbackCastTracking;

		// Token: 0x0400011E RID: 286
		private float _safetyReClickTimer;
	}
}
