using System;
using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace SpiritMod.States
{
    public class CombatState : IBotState
    {
        public BotState Id
        {
            get { return BotState.Combat; }
        }

        private const float NoDamageAfterActionTimeout = 5f;
        private const float BlacklistDurationSeconds = 60f;

        private float _dashEvadeCooldownTimer;
        private float _fallbackCastStartTime;
        private bool _fallbackCastTracking;
        private float _safetyReClickTimer;

        private BaseUnitController _trackedTarget;
        private int _lastObservedHp;

        private bool _waitingForDamageAfterAction;
        private float _damageWatchTimer;
        private int _damageWatchStartHp;
        private string _lastDamageAction;

        public void Enter(BotContext ctx)
        {
            _dashEvadeCooldownTimer = 0f;
            _fallbackCastStartTime = 0f;
            _fallbackCastTracking = false;
            _safetyReClickTimer = 0f;

            ResetDamageWatch();
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

                ResetDamageWatch();

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

            CombatService.ReadTargetHealth(
                currentTarget,
                out ctx.Status.TargetHealth,
                out ctx.Status.TargetMaxHp);

            TrackTargetHp(currentTarget, ctx.Status.TargetHealth);

            PlayerController player = ctx.Player;
            if (player == null)
                return;

            try
            {
                CombatService.ReadTargetHealth(
                    player.Cast<BaseUnitController>(),
                    out ctx.Status.PlayerHealth,
                    out ctx.Status.PlayerMaxHp);

                ctx.Status.PlayerHPNorm = CombatService.GetPlayerHPNormalised(player);
            }
            catch
            {
            }

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

                        bool casted = CombatService.TryCastSkill(player, ctx.Config, ctx.Status);

                        if (casted)
                        {
                            StartDamageWatch(currentTarget, ctx.Status.TargetHealth, "skill cast");

                            // Important: prevent same-tick auto attack from overwriting skill input
                            ctx.Status.AttackTimer = ctx.Config.AttackInterval;
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        MelonLogger.Warning("[Bot] TryCastSkill error: " + ex.Message);
                    }
                }
            }

            if (CheckNoDamageAfterAction(ctx, currentTarget))
                return;

            ctx.Status.AttackTimer -= ctx.DeltaTime;
            if (ctx.Status.AttackTimer > 0f)
                return;

            ctx.Status.AttackTimer = ctx.Config.AttackInterval;

            try
            {
                ctx.Status.AttackAttempts++;
                _safetyReClickTimer -= ctx.Config.AttackInterval;

                if (!CombatService.IsAlreadyEngaged(player, currentTarget) || _safetyReClickTimer <= 0f)
                {
                    CombatService.ClickTarget(player, currentTarget);
                    ctx.Status.AttackRequestsSent++;
                    _safetyReClickTimer = 10f;

                    StartDamageWatch(
                        currentTarget,
                        ctx.Status.TargetHealth,
                        "attack/click target");
                }
                else
                {
                    // Already engaged still counts as an attack attempt. This catches cases where
                    // the bot keeps auto-attacking an unattackable monster forever.
                    StartDamageWatch(
                        currentTarget,
                        ctx.Status.TargetHealth,
                        "attack/engaged");
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

            ResetDamageWatch();
        }

        private void TrackTargetHp(BaseUnitController target, int currentHp)
        {
            if (_trackedTarget != target)
            {
                _trackedTarget = target;
                _lastObservedHp = currentHp;
                ResetDamageWatch();
                return;
            }

            if (currentHp < _lastObservedHp)
            {
                _lastObservedHp = currentHp;

                // HP decreased, so the target is damageable.
                ResetDamageWatch();
            }
            else if (currentHp > _lastObservedHp)
            {
                // Target healed/regenerated. Use the latest HP as the new baseline.
                _lastObservedHp = currentHp;
            }
        }

        private void StartDamageWatch(BaseUnitController target, int currentHp, string actionName)
        {
            if (target == null || currentHp <= 0)
                return;

            // Do not restart the 3-second timer every frame/action for the same target.
            // Otherwise the timer never reaches 5 seconds.
            if (_waitingForDamageAfterAction && _trackedTarget == target)
                return;

            _trackedTarget = target;
            _lastObservedHp = currentHp;
            _damageWatchStartHp = currentHp;
            _damageWatchTimer = 0f;
            _lastDamageAction = actionName;
            _waitingForDamageAfterAction = true;

            //MelonLogger.Msg(
            //    "[Combat] Watching target HP after " + actionName + ". Start HP=" + currentHp);
        }

        private bool CheckNoDamageAfterAction(BotContext ctx, BaseUnitController currentTarget)
        {
            if (!_waitingForDamageAfterAction)
                return false;

            if (currentTarget == null || _trackedTarget != currentTarget)
            {
                ResetDamageWatch();
                return false;
            }

            // Any HP drop after attack or skill means the target is valid.
            if (ctx.Status.TargetHealth < _damageWatchStartHp)
            {
                ResetDamageWatch();
                _lastObservedHp = ctx.Status.TargetHealth;
                return false;
            }

            _damageWatchTimer += ctx.DeltaTime;

            if (_damageWatchTimer < NoDamageAfterActionTimeout)
                return false;

            string reason =
                "Target HP did not decrease within 5 seconds after " +
                (_lastDamageAction ?? "attack/castskill") +
                ".";

            TargetBlacklistService.Blacklist(
                currentTarget,
                BlacklistDurationSeconds,
                reason);

            //MelonLogger.Warning("[Combat] Blacklisted target. " + reason);

            BotController.ClearTarget();
            ResetDamageWatch();

            ctx.Status.TargetName = string.Empty;
            ctx.Status.TargetHealth = 0;
            ctx.Status.TargetMaxHp = 0;
            ctx.Status.ActionTimer = 0.2f;

            BotController.TransitionTo(BotState.Idle);
            return true;
        }

        private void ResetDamageWatch()
        {
            _waitingForDamageAfterAction = false;
            _damageWatchTimer = 0f;
            _damageWatchStartHp = 0;
            _lastDamageAction = null;
        }

        private void TryAutoDashEvade(BotContext ctx, PlayerController player, BaseUnitController currentTarget)
        {
            if (ctx == null || ctx.Config == null || !ctx.Config.EnableAutoDashEvade)
                return;

            _dashEvadeCooldownTimer -= ctx.DeltaTime;
            if (_dashEvadeCooldownTimer > 0f)
                return;

            bool targetIsCasting = false;

            try
            {
                SkillsComponent skillsComponent = currentTarget != null ? currentTarget.Skills : null;
                if (skillsComponent != null)
                {
                    float castTime = skillsComponent.CastTime;
                    targetIsCasting =
                        (skillsComponent.CastTimeMax > 0f && castTime > 0f) ||
                        skillsComponent.IsCasting;
                }
            }
            catch
            {
            }

            if (!targetIsCasting)
            {
                _fallbackCastTracking = false;
                return;
            }

            if (!_fallbackCastTracking)
            {
                _fallbackCastTracking = true;
                _fallbackCastStartTime = Time.time;
            }

            Mathf.Max(0.1f, ctx.Config.DodgeFallbackDelay);

            CombatService.DodgeResult dodgeResult =
                CombatService.TryAutoDashEvade(player, currentTarget, ctx.Config, ctx.Status);

            switch (dodgeResult)
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
