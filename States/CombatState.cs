using System;
using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace SpiritMod.States
{
    public class CombatState : IBotState
    {
        private const float NoDamageBlacklistSeconds = 8f;
        private const float NotEngagedBlacklistSeconds = 4f;
        private const float BlacklistDurationSeconds = 60f;

        private float _dashEvadeCooldownTimer;
        private float _fallbackCastStartTime;
        private bool _fallbackCastTracking;
        private float _safetyReClickTimer;

        private int _lastTargetHp;
        private float _noDamageTimer;
        private float _notEngagedTimer;

        public BotState Id
        {
            get { return BotState.Combat; }
        }

        public void Enter(BotContext ctx)
        {
            _dashEvadeCooldownTimer = 0f;
            _fallbackCastStartTime = 0f;
            _fallbackCastTracking = false;
            _safetyReClickTimer = 0f;

            _lastTargetHp = -1;
            _noDamageTimer = 0f;
            _notEngagedTimer = 0f;
        }

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

            CombatService.ReadTargetHealth(currentTarget, out ctx.Status.TargetHealth, out ctx.Status.TargetMaxHp);

            PlayerController player = ctx.Player;
            if (player == null)
                return;

            try
            {
                CombatService.ReadTargetHealth(player.Cast<BaseUnitController>(), out ctx.Status.PlayerHealth, out ctx.Status.PlayerMaxHp);
                ctx.Status.PlayerHPNorm = CombatService.GetPlayerHPNormalised(player);
            }
            catch
            {
            }

            bool alreadyEngaged = false;
            try
            {
                alreadyEngaged = CombatService.IsAlreadyEngaged(player, currentTarget);
            }
            catch
            {
                alreadyEngaged = false;
            }

            if (ShouldBlacklistCurrentTarget(ctx, currentTarget, alreadyEngaged))
                return;

            TryAutoDashEvade(ctx, player, currentTarget);

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
                return;

            ctx.Status.AttackTimer = ctx.Config.AttackInterval;

            try
            {
                ctx.Status.AttackAttempts++;
                _safetyReClickTimer -= ctx.Config.AttackInterval;

                if (!alreadyEngaged || _safetyReClickTimer <= 0f)
                {
                    CombatService.ClickTarget(player, currentTarget);
                    ctx.Status.AttackRequestsSent++;
                    _safetyReClickTimer = 10f;
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Warning("[Bot] Re-click failed: " + ex.Message);
            }
        }

        public void Exit(BotContext ctx)
        {
            _dashEvadeCooldownTimer = 0f;
            _fallbackCastTracking = false;
            _safetyReClickTimer = 0f;

            _lastTargetHp = -1;
            _noDamageTimer = 0f;
            _notEngagedTimer = 0f;
        }

        private bool ShouldBlacklistCurrentTarget(BotContext ctx, BaseUnitController currentTarget, bool alreadyEngaged)
        {
            int currentHp = ctx.Status.TargetHealth;

            if (_lastTargetHp < 0)
            {
                _lastTargetHp = currentHp;
                return false;
            }

            if (currentHp < _lastTargetHp)
            {
                _lastTargetHp = currentHp;
                _noDamageTimer = 0f;
                _notEngagedTimer = 0f;
                return false;
            }

            _noDamageTimer += ctx.DeltaTime;

            if (!alreadyEngaged)
                _notEngagedTimer += ctx.DeltaTime;
            else
                _notEngagedTimer = 0f;

            if (_notEngagedTimer >= NotEngagedBlacklistSeconds)
            {
                BlacklistAndReturnToIdle(
                    ctx,
                    currentTarget,
                    "Not engaged for " + _notEngagedTimer.ToString("F1") + "s");
                return true;
            }

            if (_noDamageTimer >= NoDamageBlacklistSeconds)
            {
                BlacklistAndReturnToIdle(
                    ctx,
                    currentTarget,
                    "No HP reduction for " + _noDamageTimer.ToString("F1") + "s");
                return true;
            }

            return false;
        }

        private void BlacklistAndReturnToIdle(BotContext ctx, BaseUnitController currentTarget, string reason)
        {
            TargetBlacklistService.Blacklist(currentTarget, BlacklistDurationSeconds, reason);

            BotController.ClearTarget();
            ctx.Status.TargetName = string.Empty;
            ctx.Status.TargetHealth = 0;
            ctx.Status.TargetMaxHp = 0;
            ctx.Status.ActionTimer = 0.2f;

            _lastTargetHp = -1;
            _noDamageTimer = 0f;
            _notEngagedTimer = 0f;

            BotController.TransitionTo(BotState.Idle);
        }

        private void TryAutoDashEvade(BotContext ctx, PlayerController player, BaseUnitController currentTarget)
        {
            if (ctx == null || ctx.Config == null || !ctx.Config.EnableAutoDashEvade)
                return;

            _dashEvadeCooldownTimer -= ctx.DeltaTime;
            if (_dashEvadeCooldownTimer > 0f)
                return;

            bool isCasting = false;

            try
            {
                SkillsComponent skillsComponent = currentTarget != null ? currentTarget.Skills : null;
                if (skillsComponent != null)
                {
                    float castTime = skillsComponent.CastTime;
                    isCasting = (skillsComponent.CastTimeMax > 0f && castTime > 0f) || skillsComponent.IsCasting;
                }
            }
            catch
            {
            }

            if (!isCasting)
            {
                _fallbackCastTracking = false;
                return;
            }

            if (!_fallbackCastTracking)
            {
                _fallbackCastTracking = true;
                _fallbackCastStartTime = Time.time;
            }

            float elapsed = Time.time - _fallbackCastStartTime;
            Mathf.Max(0.1f, ctx.Config.DodgeFallbackDelay);

            switch (CombatService.TryAutoDashEvade(player, currentTarget, ctx.Config, ctx.Status))
            {
                case CombatService.DodgeResult.None:
                case CombatService.DodgeResult.Pending:
                    break;

                case CombatService.DodgeResult.Executed:
                    _dashEvadeCooldownTimer = Mathf.Max(0.5f, ctx.Config.AutoDashCooldown);
                    _fallbackCastTracking = false;
                    break;
            }
        }
    }
}
