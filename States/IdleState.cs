using System;
using System.Collections.Generic;
using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace SpiritMod.States
{
    public class IdleState : IBotState
    {
        public static TargetPriorityMode TargetPriority { get; set; }

        public BotState Id
        {
            get { return BotState.Idle; }
        }

        public void Enter(BotContext ctx)
        {
        }

        public void Tick(BotContext ctx)
        {
            ctx.Status.ActionTimer -= ctx.DeltaTime;
            if (ctx.Status.ActionTimer > 0f)
                return;

            ctx.Status.ActionTimer = ctx.Config.ActionInterval;

            if (ctx.Config.EnableAfkLeech)
                return;

            TargetBlacklistService.CleanupExpired();

            GameStateService.TryScanMonsters();
            List<MonsterInfo> monsters = GameStateService.Monsters;
            if (monsters.Count == 0)
                return;

            PlayerController player = ctx.Player;
            if (player == null)
                return;

            try
            {
                Vector3 playerPosition = player.Cast<BaseUnitController>().Position;

                BaseUnitController bestTarget = null;
                float bestScore = float.MaxValue;
                float bestDistanceSqr = float.MaxValue;
                string bestName = null;

                foreach (MonsterInfo monsterInfo in monsters)
                {
                    if (monsterInfo == null || monsterInfo.Controller == null)
                        continue;

                    BaseUnitController monster = monsterInfo.Controller;

                    if (TargetBlacklistService.IsBlacklisted(monster))
                        continue;

                    if (!CombatService.IsTargetAlive(monster))
                        continue;

                    float distanceSqr = (playerPosition - monsterInfo.Position).sqrMagnitude;

                    int hp;
                    int maxHp;
                    CombatService.ReadTargetHealth(monster, out hp, out maxHp);

                    float hpPct = maxHp > 0 ? (float)hp / (float)maxHp : 1f;
                    float score = ScoreTarget(distanceSqr, hp, hpPct);

                    if (score < bestScore)
                    {
                        bestScore = score;
                        bestDistanceSqr = distanceSqr;
                        bestTarget = monster;
                        bestName = monsterInfo.Name;
                    }
                }

                if (bestTarget == null)
                    return;

                if (ctx.Config.EnableSkills)
                {
                    Il2CppSystem.Collections.Generic.List<int> missingSummonSlots =
                        SummonService.GetMissingSummonSlots(player, ctx.Config);

                    if (missingSummonSlots.Count > 0)
                    {
                        if (CombatService.TryCastSkill(player, ctx.Config, ctx.Status))
                        {
                            MelonLogger.Msg("[Bot] Summoning (missing slots: " +
                                            string.Join(",", missingSummonSlots) + ")");
                        }

                        return;
                    }
                }

                CombatService.ClickTarget(player, bestTarget);
                BotController.SetTarget(bestTarget);

                ctx.Status.TargetName = bestName ?? "???";
                ctx.Status.LastTargetPriorityMode = TargetPriority.ToString();
                ctx.Status.LastTargetDistance = Mathf.Sqrt(bestDistanceSqr);
                ctx.Status.AttackTimer = ctx.Config.AttackInterval;

                BotController.TransitionTo(BotState.Combat);
            }
            catch (Exception ex)
            {
                MelonLogger.Warning("[Bot] TickIdle failed: " + ex.Message);
            }
        }

        public void Exit(BotContext ctx)
        {
        }

        private static float ScoreTarget(float sqrDistance, int hp, float hpPct)
        {
            switch (TargetPriority)
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
