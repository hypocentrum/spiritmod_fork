using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Il2Cpp;
using Il2CppFishNet.Object;
using Il2CppSystem.Collections.Generic;
using MelonLoader;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SpiritMod
{
    // Token: 0x0200000E RID: 14
    public static class CombatService
    {
        // Token: 0x06000037 RID: 55 RVA: 0x00003104 File Offset: 0x00001304
        public static void ClickTarget(PlayerController player, BaseUnitController target)
        {
            try
            {
                InputInjector.QueueClickUnit(target, -1);
            }
            catch (Exception ex)
            {
                MelonLogger.Warning("[Combat] ClickTarget failed: " + ex.Message);
            }
        }

        // Token: 0x06000038 RID: 56 RVA: 0x00003144 File Offset: 0x00001344
        public static void ClickPosition(PlayerController player, Vector3 position)
        {
            try
            {
                InputInjector.QueueClickPosition(position, -1);
            }
            catch (Exception ex)
            {
                MelonLogger.Warning("[Combat] ClickPosition failed: " + ex.Message);
            }
        }

        // Token: 0x06000039 RID: 57 RVA: 0x00003184 File Offset: 0x00001384
        public static bool IsAlreadyEngaged(PlayerController player, BaseUnitController target)
        {
            bool result;
            try
            {
                CombatComponent combat = player.Cast<BaseUnitController>().Combat;
                if (combat == null)
                {
                    result = false;
                }
                else
                {
                    BaseUnitController target2 = combat.Target;
                    result = (target2 != null && target2.Pointer == target.Pointer);
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        // Token: 0x0600003A RID: 58 RVA: 0x000031E8 File Offset: 0x000013E8
        public static bool IsTargetAlive(BaseUnitController unit)
        {
            if (unit == null)
            {
                return false;
            }
            bool result;
            try
            {
                HealthComponent health = unit.Health;
                result = (health != null && health.IsAlive);
            }
            catch
            {
                result = false;
            }
            return result;
        }

        // Token: 0x0600003B RID: 59 RVA: 0x00003234 File Offset: 0x00001434
        public static void ReadTargetHealth(BaseUnitController unit, out int health, out int maxHp)
        {
            health = 0;
            maxHp = 0;
            if (unit == null)
            {
                return;
            }
            try
            {
                HealthComponent health2 = unit.Health;
                if (!(health2 == null))
                {
                    health = health2.Health;
                    maxHp = health2.MaxHealth;
                }
            }
            catch
            {
            }
        }

        // Token: 0x0600003C RID: 60 RVA: 0x0000328C File Offset: 0x0000148C
        public static float GetPlayerHPNormalised(PlayerController player)
        {
            float result;
            try
            {
                HealthComponent health = player.Cast<BaseUnitController>().Health;
                result = ((health != null) ? health.HealthNormalised : 1f);
            }
            catch
            {
                result = 1f;
            }
            return result;
        }

        // Token: 0x0600003D RID: 61 RVA: 0x000032D8 File Offset: 0x000014D8
        public static bool IsPlayerDead(PlayerController player)
        {
            if (player == null)
            {
                return false;
            }
            bool result;
            try
            {
                HealthComponent health = player.Cast<BaseUnitController>().Health;
                if (health == null)
                {
                    result = false;
                }
                else
                {
                    result = (!health.IsAlive || health.Health <= 0);
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        // Token: 0x0600003E RID: 62 RVA: 0x0000333C File Offset: 0x0000153C
        public static bool IsPlayerFullyRestored(PlayerController player)
        {
            if (player == null)
            {
                return false;
            }
            bool result;
            try
            {
                BaseUnitController baseUnitController = player.Cast<BaseUnitController>();
                HealthComponent health = baseUnitController.Health;
                if (health == null)
                {
                    result = false;
                }
                else if (health.Health < health.MaxHealth)
                {
                    result = false;
                }
                else
                {
                    SkillsComponent skills = baseUnitController.Skills;
                    if (skills == null)
                    {
                        result = false;
                    }
                    else if (skills.Mana < skills.MaxMana)
                    {
                        result = false;
                    }
                    else
                    {
                        result = true;
                    }
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        // Token: 0x0600003F RID: 63 RVA: 0x000033C4 File Offset: 0x000015C4
        public static System.Collections.Generic.List<SkillData> GetAssignedSkillList(PlayerController player)
        {
            var result = new System.Collections.Generic.List<SkillData>();

            try
            {
                if (player == null)
                    player = GameStateService.Player;

                if (player == null || player.Save == null || player.Save.Data == null)
                    return result;

                var skills = player.Save.Data.Skills;

                if (skills == null || skills.Assigned == null)
                    return result;

                for (int i = 0; i < skills.Assigned.Count; i++)
                {
                    SkillData skill = skills.Assigned[i];

                    if (skill != null)
                        result.Add(skill);
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Warning("[Combat] GetAssignedSkillList failed: " + ex.Message);
            }

            return result;
        }

        // Token: 0x06000040 RID: 64 RVA: 0x00003420 File Offset: 0x00001620
        public static float GetHealThreshold(BotConfig cfg, int slot)
        {
            int num = cfg.HealingThresholds[slot];
            if (num <= 0)
            {
                num = 90;
            }
            return (float)num / 100f;
        }

        // Token: 0x06000041 RID: 65 RVA: 0x00003448 File Offset: 0x00001648
        public static bool IsSummonSkill(SkillConfig cfg)
        {
            if (cfg == null)
            {
                return false;
            }
            if (CombatService.IsMountSkill(cfg))
            {
                return false;
            }
            string id = cfg.Id;
            return (!string.IsNullOrEmpty(id) && id.Contains("Summon")) || (int)cfg.ExclusiveType == 4;
        }

        // Token: 0x06000042 RID: 66 RVA: 0x00003494 File Offset: 0x00001694
        public static bool IsMountSkill(SkillConfig cfg)
        {
            if (cfg == null)
            {
                return false;
            }
            string displayName = cfg.DisplayName;
            return !string.IsNullOrEmpty(displayName) && displayName.Equals("Summon Mount", StringComparison.OrdinalIgnoreCase);
        }

        // Token: 0x06000043 RID: 67 RVA: 0x000034CC File Offset: 0x000016CC
        public static bool IsBuffSkill(SkillConfig cfg)
        {
            if (cfg == null)
            {
                return false;
            }
            if (CombatService.IsSummonSkill(cfg))
            {
                return false;
            }
            if (CombatService.IsBondSkill(cfg))
            {
                return false;
            }
            int castType = (int)cfg.CastType;
            return castType == 3 || (castType == 0 && (int)cfg.TargetType == 3);
        }

        // Token: 0x06000044 RID: 68 RVA: 0x00003513 File Offset: 0x00001713
        public static bool IsBondSkill(SkillConfig cfg)
        {
            return !(cfg == null) && cfg.Bond;
        }

        // Token: 0x06000045 RID: 69 RVA: 0x00003528 File Offset: 0x00001728
        public static BaseUnitController GetPrimarySummon(PlayerController player)
        {
            BaseUnitController result;
            try
            {
                SummoningComponent summoning = player.Cast<BaseUnitController>().Summoning;
                result = ((summoning != null) ? summoning.Primary : null);
            }
            catch
            {
                result = null;
            }
            return result;
        }

        // Token: 0x06000046 RID: 70 RVA: 0x00003568 File Offset: 0x00001768
        public static bool IsBuffAlreadyApplied(
    PlayerController player,
    SkillState skillState)
        {
            return TryGetBuffRemainingSeconds(
                player,
                skillState,
                out _
            );
        }
        // Token: 0x06000047 RID: 71 RVA: 0x000035F0 File Offset: 0x000017F0
        private static bool AnyEffectPresent(
    Il2CppSystem.Collections.Generic.Dictionary<string, StatusEffectState> displays,
    Il2CppSystem.Collections.Generic.List<SkillStatus> effects)
        {
            if (displays == null || effects == null || effects.Count == 0)
                return false;

            for (int i = 0; i < effects.Count; i++)
            {
                SkillStatus effect = effects[i];

                if (effect == null)
                    continue;

                string id = effect.Id;

                if (string.IsNullOrEmpty(id))
                    continue;

                foreach (var pair in displays)
                {
                    string activeId = pair.key?.ToString() ?? "";

                    if (string.IsNullOrEmpty(activeId))
                        continue;

                    if (activeId.Equals(id, StringComparison.OrdinalIgnoreCase) ||
                        activeId.Contains(id) ||
                        id.Contains(activeId))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        // Token: 0x06000048 RID: 72 RVA: 0x00003644 File Offset: 0x00001844
        public static int GetEffectivePriority(BotConfig cfg, int slot, SkillState skillState, float playerHPNorm)
        {
            int num = cfg.SkillPriorities[slot];
            if (num <= 0)
            {
                num = 50;
            }
            if ((skillState.IsHealing || cfg.TreatAsHealing[slot]) && playerHPNorm < CombatService.GetHealThreshold(cfg, slot))
            {
                return num + 100;
            }
            return num;
        }

        // Token: 0x06000049 RID: 73 RVA: 0x00003688 File Offset: 0x00001888
        public static bool TryCastSkill(PlayerController player, BotConfig cfg, BotStatus status)
        {
            bool result;
            try
            {
                result = CombatService.TryCastSkillInternal(player, cfg, status, false);
            }
            catch (Exception ex)
            {
                MelonLogger.Warning("[Combat] TryCastSkill failed: " + ex.Message);
                result = false;
            }
            return result;
        }

        // Token: 0x0600004A RID: 74 RVA: 0x000036CC File Offset: 0x000018CC
        public static bool TryCastHealingSkills(PlayerController player, BotConfig cfg, BotStatus status)
        {
            bool result;
            try
            {
                result = CombatService.TryCastSkillInternal(player, cfg, status, true);
            }
            catch (Exception ex)
            {
                MelonLogger.Warning("[Combat] TryCastHealingSkills failed: " + ex.Message);
                result = false;
            }
            return result;
        }

        // Token: 0x0600004B RID: 75 RVA: 0x00003710 File Offset: 0x00001910
        private static bool TryCastSkillInternal(PlayerController player, BotConfig cfg, BotStatus status, bool healingOnly)
        {
            BaseUnitController baseUnitController = player.Cast<BaseUnitController>();
            SkillsComponent skills = baseUnitController.Skills;
            if (skills == null)
            {
                return false;
            }
            if (skills.IsCasting)
            {
                CombatTelemetryService.RecordBlocked("casting");
                return false;
            }
            if (!skills.CanCast(null))
            {
                CombatTelemetryService.RecordBlocked("cant_cast");
                return false;
            }
            int mana = skills.Mana;
            System.Collections.Generic.List<SkillData> assignedSkillList = CombatService.GetAssignedSkillList(player);
            if (assignedSkillList == null || assignedSkillList.Count == 0)
            {
                return false;
            }
            float playerHPNormalised = CombatService.GetPlayerHPNormalised(player);
            CombatService._candidates.Clear();
            int num = Math.Min(assignedSkillList.Count, 20);
            for (int i = 0; i < num; i++)
            {
                if (cfg.EnabledSkillSlots[i])
                {
                    SkillData skillData = assignedSkillList[i];
                    if (skillData != null)
                    {
                        string id = skillData.Id;
                        if (!string.IsNullOrEmpty(id))
                        {
                            SkillState anySkill = skills.GetAnySkill(id);
                            if (anySkill != null && !anySkill.IsOnCooldown && anySkill.Cost <= mana)
                            {
                                SkillConfig config = anySkill.Config;
                                if (!(config == null))
                                {
                                    bool flag = anySkill.IsHealing || cfg.TreatAsHealing[i];
                                    if (!healingOnly || flag)
                                    {
                                        if (CombatService.IsMountSkill(config))
                                        {
                                            SummoningComponent summoning = baseUnitController.Summoning;
                                            if ((summoning != null && summoning.IsMounting) || !SummonService.HasAlivePrimarySummon(player))
                                            {
                                                //goto IL_1FB;
                                                continue;
                                            }
                                        }
                                        if ((!CombatService.IsSummonSkill(config) || !SummonService.IsSlotSatisfied(player, cfg, i)) && (!CombatService.IsManagedBuffSlot(cfg, i, config) || !CombatService.ShouldSkipBuffCast(player, anySkill, cfg, i)))
                                        {
                                            int castType = (int)config.CastType;
                                            int targetType = (int)config.TargetType;
                                            if (healingOnly || !flag || playerHPNormalised < CombatService.GetHealThreshold(cfg, i))
                                            {
                                                int effectivePriority = CombatService.GetEffectivePriority(cfg, i, anySkill, playerHPNormalised) + CombatService.GetBuffPriorityBoost(player, cfg, i, anySkill);
                                                CombatService._candidates.Add(new CombatService.SkillCandidate
                                                {
                                                    Slot = i,
                                                    EffectivePriority = effectivePriority,
                                                    State = anySkill,
                                                    CastType = castType,
                                                    TargetType = targetType
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            //IL_1FB:;
            }
            if (CombatService._candidates.Count == 0)
            {
                CombatTelemetryService.RecordBlocked("no_candidates");
                return false;
            }
            CombatService._candidates.Sort((CombatService.SkillCandidate a, CombatService.SkillCandidate b) => b.EffectivePriority.CompareTo(a.EffectivePriority));
            foreach (CombatService.SkillCandidate skillCandidate in CombatService._candidates)
            {
                CombatComponent combat = baseUnitController.Combat;
                BaseUnitController baseUnitController2 = (combat != null) ? combat.Target : null;
                if (CombatService.IsBondSkill(skillCandidate.State.Config))
                {
                    baseUnitController2 = CombatService.GetPrimarySummon(player);
                    if (baseUnitController2 == null)
                    {
                        player.SkillReady = null;
                        continue;
                    }
                }
                else if (skillCandidate.TargetType == 1 || skillCandidate.TargetType == 3)
                {
                    baseUnitController2 = baseUnitController;
                }
                player.SkillReady = skillCandidate.State;
                if (skillCandidate.CastType == 1)
                {
                    if (baseUnitController2 == null)
                    {
                        CombatTelemetryService.RecordBlocked($"no_target:slot{skillCandidate.Slot}");
                        player.SkillReady = null;
                        continue;
                    }
                    InputInjector.QueueClickUnit(baseUnitController2, skillCandidate.Slot);
                }
                else if (skillCandidate.CastType == 2)
                {
                    if (baseUnitController2 == null)
                    {
                        CombatTelemetryService.RecordBlocked($"no_ground_target:slot{skillCandidate.Slot}");
                        player.SkillReady = null;
                        continue;
                    }
                    InputInjector.QueueClickPosition(baseUnitController2.Position, skillCandidate.Slot);
                }
                else
                {
                    InputInjector.QueueSkillOnly(skillCandidate.Slot);
                    player.SkillReady = null;
                }
                SkillConfig config2 = skillCandidate.State.Config;
                string detail;
                if ((detail = ((config2 != null) ? config2.DisplayName : null)) == null)
                {
                    detail = $"slot{skillCandidate.Slot}";
                }
                CombatTelemetryService.RecordSent(detail);
                CombatService.RecordSkillCastTime(skillCandidate.Slot);
                if (status != null)
                {
                    status.SkillsCastCount++;
                    status.SkillRequestsSent++;
                }
                return true;
            }
            return false;
        }

        // Token: 0x0600004C RID: 76 RVA: 0x00003B7C File Offset: 0x00001D7C
        public static SkillSlotInfo[] GetSkillSlotInfos(PlayerController player)
        {
            float time = Time.time;
            if (CombatService._cachedSkillInfos != null && time - CombatService._skillInfoCacheTime < 60f)
            {
                return CombatService._cachedSkillInfos;
            }
            CombatService._cachedSkillInfos = CombatService.QuerySkillSlotInfos(player);
            CombatService._skillInfoCacheTime = time;
            return CombatService._cachedSkillInfos;
        }

        // Token: 0x0600004D RID: 77 RVA: 0x00003BC0 File Offset: 0x00001DC0
        public static void ForceRefreshSkillInfos()
        {
            CombatService._skillInfoCacheTime = -1f;
            CombatService._cachedSkillInfos = null;
        }

        // Token: 0x0600004E RID: 78 RVA: 0x00003BD4 File Offset: 0x00001DD4
        private static SkillSlotInfo[] QuerySkillSlotInfos(PlayerController player)
        {
            SkillSlotInfo[] array = new SkillSlotInfo[20];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new SkillSlotInfo();
            }
            if (player == null)
            {
                player = GameStateService.Player;
            }
            if (player == null)
            {
                return array;
            }
            try
            {
                System.Collections.Generic.List<SkillData> assignedSkillList = CombatService.GetAssignedSkillList(player);
                if (assignedSkillList == null)
                {
                    return array;
                }
                SkillsComponent skills = player.Cast<BaseUnitController>().Skills;
                int num = Math.Min(assignedSkillList.Count, 20);
                for (int j = 0; j < num; j++)
                {
                    SkillData skillData = assignedSkillList[j];
                    if (skillData != null)
                    {
                        string id = skillData.Id;
                        if (!string.IsNullOrEmpty(id) && !(id == "0"))
                        {
                            array[j].Assigned = true;
                            array[j].Name = id;
                            if (!(skills == null))
                            {
                                try
                                {
                                    SkillState anySkill = skills.GetAnySkill(id);
                                    if (anySkill != null)
                                    {
                                        array[j].ManaCost = anySkill.Cost;
                                        array[j].IsOnCooldown = anySkill.IsOnCooldown;
                                        array[j].IsHealing = anySkill.IsHealing;
                                        SkillConfig config = anySkill.Config;
                                        if (config != null)
                                        {
                                            array[j].TargetType = (int)config.TargetType;
                                            array[j].IsSummon = CombatService.IsSummonSkill(config);
                                            array[j].IsBuff = CombatService.IsBuffSkill(config);
                                            array[j].IsBond = CombatService.IsBondSkill(config);
                                            array[j].IsMount = CombatService.IsMountSkill(config);
                                            string displayName = config.DisplayName;
                                            if (!string.IsNullOrEmpty(displayName))
                                            {
                                                array[j].Name = displayName;
                                            }
                                        }
                                    }
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Warning("[Combat] GetSkillSlotInfos failed: " + ex.Message);
            }
            return array;
        }

        // Token: 0x0600004F RID: 79 RVA: 0x00003DD4 File Offset: 0x00001FD4
        private static float GetLastSkillCastTime(int slot)
        {
            if (slot < 0 || slot >= CombatService._lastSkillCastTimes.Length)
            {
                return -9999f;
            }
            return CombatService._lastSkillCastTimes[slot];
        }

        // Token: 0x06000050 RID: 80 RVA: 0x00003DF1 File Offset: 0x00001FF1
        private static void RecordSkillCastTime(int slot)
        {
            if (slot >= 0 && slot < CombatService._lastSkillCastTimes.Length)
            {
                CombatService._lastSkillCastTimes[slot] = Time.time;
            }
        }

        // Token: 0x06000051 RID: 81 RVA: 0x00003E10 File Offset: 0x00002010
        public static float GetBuffRefreshLeadSeconds(BotConfig cfg, int slot)
        {
            if (cfg == null)
            {
                return 2f;
            }
            cfg.EnsureArrays();
            float num = cfg.BuffRefreshLeadSeconds[slot];
            if (num <= 0f)
            {
                return 2f;
            }
            return num;
        }

        // Token: 0x06000052 RID: 82 RVA: 0x00003E44 File Offset: 0x00002044
        private static bool IsManagedBuffSlot(BotConfig cfg, int slot, SkillConfig config)
        {
            if (config == null)
                return false;

            if (BuffMaintenanceRules.IsExcludedBuffMaintenanceSkill(config))
                return false;

            if (CombatService.IsBuffSkill(config) || CombatService.IsBondSkill(config))
                return true;

            if (cfg == null)
                return false;

            cfg.EnsureArrays();
            return slot >= 0 &&
                   slot < cfg.TreatAsBuff.Length &&
                   cfg.TreatAsBuff[slot];
        }

        // Token: 0x06000053 RID: 83 RVA: 0x00003E7C File Offset: 0x0000207C
        private static bool ShouldSkipBuffCast(PlayerController player, SkillState skillState, BotConfig cfg, int slot)
        {
            if (cfg == null || !cfg.EnableBuffMaintenance)
            {
                return CombatService.IsBuffAlreadyApplied(player, skillState);
            }
            cfg.EnsureArrays();
            if (slot >= 0 && slot < cfg.EnabledBuffSlots.Length && !cfg.EnabledBuffSlots[slot])
            {
                return true;
            }
            float num;
            bool flag = CombatService.TryGetBuffRemainingSeconds(player, skillState, out num);
            if (CombatService.IsPermanentBuff(skillState.Config))
            {
                return flag;
            }
            return flag && num > CombatService.GetBuffRefreshLeadSeconds(cfg, slot);
        }

        // Token: 0x06000054 RID: 84 RVA: 0x00003EE5 File Offset: 0x000020E5
        public static bool IsPermanentBuff(SkillConfig cfg)
        {
            return !(cfg == null) && (CombatService.HasOnlyPermanentEffects(cfg.StatusEffects) || CombatService.HasOnlyPermanentEffects(cfg.SelfStatusEffects));
        }

        // Token: 0x06000055 RID: 85 RVA: 0x00003F0C File Offset: 0x0000210C
        private static bool HasOnlyPermanentEffects(Il2CppSystem.Collections.Generic.List<SkillStatus> effects)
        {
            if (effects == null || effects.Count == 0)
            {
                return false;
            }
            for (int i = 0; i < effects.Count; i++)
            {
                SkillStatus skillStatus = effects[i];
                if (skillStatus != null)
                {
                    if (skillStatus.DurationLv > 0f)
                    {
                        return false;
                    }
                    if (skillStatus.Duration <= 0f)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        // Token: 0x06000056 RID: 86 RVA: 0x00003F64 File Offset: 0x00002164
        private static int GetBuffPriorityBoost(
    PlayerController player,
    BotConfig cfg,
    int slot,
    SkillState skillState)
        {
            if (cfg == null || !cfg.EnableBuffMaintenance || skillState == null)
                return 0;

            cfg.EnsureArrays();

            if (slot < 0 || slot >= cfg.EnabledBuffSlots.Length)
                return 0;

            if (!cfg.EnabledBuffSlots[slot])
                return 0;

            SkillConfig config = skillState.Config;

            if (!CombatService.IsManagedBuffSlot(cfg, slot, config))
                return 0;

            float remaining;
            bool active = CombatService.TryGetBuffRemainingSeconds(
                player,
                skillState,
                out remaining
            );

            if (CombatService.IsPermanentBuff(config))
                return active ? 0 : 220;

            if (!active)
                return 220;

            float lead = CombatService.GetBuffRefreshLeadSeconds(cfg, slot);

            if (remaining <= lead)
                return 180 + Mathf.Clamp((int)((lead - remaining) * 10f), 0, 40);

            return 0;
        }

        // Token: 0x06000057 RID: 87 RVA: 0x00003FD8 File Offset: 0x000021D8
        public static bool TryGetBuffRemainingSeconds(
    PlayerController player,
    SkillState skillState,
    out float remainingSeconds)
        {
            remainingSeconds = 0f;

            try
            {
                if (player == null || skillState == null)
                    return false;

                SkillConfig config = skillState.Config;
                if (config == null)
                    return false;

                // Mount/Gryphon Riding does not always appear as a normal buff effect.
                // Treat the mount skill as active when the player is already mounting.
                try
                {
                    if (CombatService.IsMountSkill(config) ||
                        string.Equals(config.Id, "MountMastery", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(config.DisplayName, "Gryphon Riding", StringComparison.OrdinalIgnoreCase))
                    {
                        BaseUnitController mountUnit = player.Cast<BaseUnitController>();
                        SummoningComponent summoning = mountUnit != null ? mountUnit.Summoning : null;

                        if (summoning != null && summoning.IsMounting)
                        {
                            remainingSeconds = -1f;
                            return true;
                        }
                    }
                }
                catch
                {
                }

                StatusComponent status = null;

                // Important:
                // player.Status can miss data. The reliable source is usually BaseUnitController.Status.
                try
                {
                    BaseUnitController baseUnit = player.Cast<BaseUnitController>();
                    if (baseUnit != null)
                        status = baseUnit.Status;
                }
                catch
                {
                }

                if (status == null)
                {
                    try { status = player.Status; }
                    catch { }
                }

                if (status == null)
                    return false;

                bool foundAny = false;
                float maxRemaining = 0f;

                // Match by every possible id:
                // - skill id: Conviction
                // - display name: Conviction Aura
                // - status effect ids: Might, SpearQuicken, HolyShield, Endure, etc.
                CombatService.MatchBuffIdInAllKnownStatusDictionaries(
                    status,
                    config.Id,
                    ref foundAny,
                    ref maxRemaining);

                CombatService.MatchBuffIdInAllKnownStatusDictionaries(
                    status,
                    config.DisplayName,
                    ref foundAny,
                    ref maxRemaining);

                CombatService.MatchBuffEffectListInAllKnownStatusDictionaries(
                    status,
                    config.StatusEffects,
                    ref foundAny,
                    ref maxRemaining);

                CombatService.MatchBuffEffectListInAllKnownStatusDictionaries(
                    status,
                    config.SelfStatusEffects,
                    ref foundAny,
                    ref maxRemaining);

                if (!foundAny)
                    return false;

                remainingSeconds = maxRemaining <= 0f ? -1f : maxRemaining;
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Warning("[Buff] TryGetBuffRemainingSeconds failed: " + ex.Message);
                return false;
            }
        }

        private static void MatchBuffEffectListInAllKnownStatusDictionaries(
            StatusComponent status,
            Il2CppSystem.Collections.Generic.List<SkillStatus> effects,
            ref bool foundAny,
            ref float maxRemaining)
        {
            if (status == null || effects == null || effects.Count == 0)
                return;

            for (int i = 0; i < effects.Count; i++)
            {
                SkillStatus effect = effects[i];
                if (effect == null)
                    continue;

                string id = effect.Id;
                if (string.IsNullOrEmpty(id))
                    continue;

                CombatService.MatchBuffIdInAllKnownStatusDictionaries(
                    status,
                    id,
                    ref foundAny,
                    ref maxRemaining);
            }
        }

        private static void MatchBuffIdInAllKnownStatusDictionaries(
            StatusComponent status,
            string expectedId,
            ref bool foundAny,
            ref float maxRemaining)
        {
            if (status == null || string.IsNullOrEmpty(expectedId))
                return;

            try
            {
                CombatService.MatchBuffIdInDictionary(
                    status.EffectsDictionary,
                    expectedId,
                    ref foundAny,
                    ref maxRemaining);
            }
            catch
            {
            }

            try
            {
                CombatService.MatchBuffIdInDictionary(
                    status.SkillDisplays_C,
                    expectedId,
                    ref foundAny,
                    ref maxRemaining);
            }
            catch
            {
            }

            try
            {
                CombatService.MatchBuffIdInDictionary(
                    status.StatusDisplays_C,
                    expectedId,
                    ref foundAny,
                    ref maxRemaining);
            }
            catch
            {
            }
        }

        private static void MatchBuffIdInDictionary(
            Il2CppSystem.Collections.Generic.Dictionary<string, StatusEffectState> dictionary,
            string expectedId,
            ref bool foundAny,
            ref float maxRemaining)
        {
            if (dictionary == null || string.IsNullOrEmpty(expectedId))
                return;

            try
            {
                if (dictionary.ContainsKey(expectedId))
                {
                    foundAny = true;
                    StatusEffectState state = dictionary[expectedId];
                    maxRemaining = Math.Max(maxRemaining, CombatService.ReadBuffDurationSeconds(state));
                    return;
                }
            }
            catch
            {
            }

            try
            {
                foreach (var pair in dictionary)
                {
                    string activeId = pair.key != null ? pair.key.ToString() : "";
                    if (!CombatService.BuffIdsMatch(activeId, expectedId))
                        continue;

                    foundAny = true;
                    maxRemaining = Math.Max(
                        maxRemaining,
                        CombatService.ReadBuffDurationSeconds(pair.value));
                }
            }
            catch
            {
            }
        }

        private static void MatchBuffIdInDictionary(
            Il2CppSystem.Collections.Generic.Dictionary<string, SkillState> dictionary,
            string expectedId,
            ref bool foundAny,
            ref float maxRemaining)
        {
            if (dictionary == null || string.IsNullOrEmpty(expectedId))
                return;

            try
            {
                if (dictionary.ContainsKey(expectedId))
                {
                    foundAny = true;
                    var state = dictionary[expectedId];
                    maxRemaining = Math.Max(maxRemaining, CombatService.ReadBuffDurationSeconds(state));
                    return;
                }
            }
            catch
            {
            }

            try
            {
                foreach (var pair in dictionary)
                {
                    string activeId = pair.key != null ? pair.key.ToString() : "";
                    if (!CombatService.BuffIdsMatch(activeId, expectedId))
                        continue;

                    foundAny = true;
                    maxRemaining = Math.Max(
                        maxRemaining,
                        CombatService.ReadBuffDurationSeconds(pair.value));
                }
            }
            catch
            {
            }
        }

        private static bool BuffIdsMatch(string activeId, string expectedId)
        {
            if (string.IsNullOrEmpty(activeId) || string.IsNullOrEmpty(expectedId))
                return false;

            if (activeId.Equals(expectedId, StringComparison.OrdinalIgnoreCase))
                return true;

            return activeId.IndexOf(expectedId, StringComparison.OrdinalIgnoreCase) >= 0 ||
                   expectedId.IndexOf(activeId, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static float ReadBuffDurationSeconds(StatusEffectState state)
        {
            if (state == null)
                return -1f;

            try { return state.Duration; }
            catch { return -1f; }
        }

        private static float ReadBuffDurationSeconds(SkillState state)
        {
            if (state == null)
                return -1f;

            try { return state.Duration; }
            catch { return -1f; }
        }
        private static bool TryMatchStatusDisplay(
    StatusComponent status,
    string id,
    out float remainingSeconds)
        {
            remainingSeconds = 0f;

            if (status == null || string.IsNullOrEmpty(id))
                return false;

            var displays = status.StatusDisplays_C;
            if (displays == null)
                return false;

            if (!displays.ContainsKey(id))
                return false;

            var state = displays[id];
            if (state == null)
            {
                remainingSeconds = -1f;
                return true;
            }

            remainingSeconds = state.Duration <= 0f ? -1f : state.Duration;
            return true;
        }

        // Token: 0x06000058 RID: 88 RVA: 0x0000407C File Offset: 0x0000227C
        private static void CheckEffectDurations(
    Il2CppSystem.Collections.Generic.Dictionary<string, StatusEffectState> displays,
    Il2CppSystem.Collections.Generic.List<SkillStatus> effects,
    ref bool foundAny,
    ref float maxRemaining)
        {
            if (displays == null || effects == null || effects.Count == 0)
                return;

            for (int i = 0; i < effects.Count; i++)
            {
                SkillStatus effect = effects[i];

                if (effect == null)
                    continue;

                string id = effect.Id;

                if (string.IsNullOrEmpty(id))
                    continue;

                foreach (var pair in displays)
                {
                    string activeId = pair.key?.ToString() ?? "";

                    if (string.IsNullOrEmpty(activeId))
                        continue;

                    if (!activeId.Equals(id, StringComparison.OrdinalIgnoreCase) &&
                        !activeId.Contains(id) &&
                        !id.Contains(activeId))
                    {
                        continue;
                    }

                    foundAny = true;

                    var state = pair.value;

                    if (state != null)
                    {
                        float duration = state.Duration;

                        if (duration > maxRemaining)
                            maxRemaining = duration;
                    }
                    else
                    {
                        maxRemaining = -1f;
                    }

                    return;
                }
            }
        }
        // Token: 0x06000059 RID: 89 RVA: 0x000040EC File Offset: 0x000022EC
        public static bool TryGetBuffRemainingSecondsForSlot(PlayerController player, int slot, out float remainingSeconds)
        {
            remainingSeconds = -1f;
            bool result;
            try
            {
                if (player == null || slot < 0 || slot >= 20)
                {
                    result = false;
                }
                else
                {
                    System.Collections.Generic.List<SkillData> assignedSkillList = CombatService.GetAssignedSkillList(player);
                    SkillsComponent skills = player.Cast<BaseUnitController>().Skills;
                    if (assignedSkillList == null || skills == null || slot >= assignedSkillList.Count)
                    {
                        result = false;
                    }
                    else
                    {
                        SkillData skillData = assignedSkillList[slot];
                        if (skillData == null || string.IsNullOrEmpty(skillData.Id))
                        {
                            result = false;
                        }
                        else
                        {
                            SkillState anySkill = skills.GetAnySkill(skillData.Id);
                            result = (anySkill != null && CombatService.TryGetBuffRemainingSeconds(player, anySkill, out remainingSeconds));
                        }
                    }
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        // Token: 0x0600005A RID: 90 RVA: 0x00004198 File Offset: 0x00002398
        public static CombatService.DodgeResult TryAutoDashEvade(PlayerController player, BaseUnitController threatTarget, BotConfig cfg, BotStatus status)
        {
            try
            {
                if (player == null || threatTarget == null || cfg == null || !cfg.EnableAutoDashEvade)
                {
                    return CombatService.DodgeResult.None;
                }
                BaseUnitController baseUnitController = player.Cast<BaseUnitController>();
                SkillsComponent skills = baseUnitController.Skills;
                SkillsComponent skills2 = threatTarget.Skills;
                if (skills == null || skills2 == null)
                {
                    return CombatService.DodgeResult.None;
                }
                float num = 0f;
                float num2 = 0f;
                bool flag = false;
                try
                {
                    num = skills2.CastTime;
                    num2 = skills2.CastTimeMax;
                    flag = (num2 > 0f && num > 0f);
                }
                catch
                {
                }
                if (!flag)
                {
                    try
                    {
                        flag = skills2.IsCasting;
                    }
                    catch
                    {
                    }
                }
                if (!flag)
                {
                    return CombatService.DodgeResult.None;
                }
                float num3 = Vector3.Distance(baseUnitController.Position, threatTarget.Position);
                if (num3 > Mathf.Max(1f, cfg.AutoDashTriggerDistance))
                {
                    return CombatService.DodgeResult.None;
                }
                if (cfg.DodgeCheckAOERadius)
                {
                    try
                    {
                        SkillCast skillCasting = skills2.SkillCasting;
                        SkillState skillState = (skillCasting != null) ? skillCasting.Skill : null;
                        if (skillState != null)
                        {
                            float area = skillState.Area;
                            float num4 = 0f;
                            try
                            {
                                SkillConfig config = skillState.Config;
                                num4 = ((config != null) ? config.Range : 0f);
                            }
                            catch
                            {
                            }
                            float num5 = Mathf.Max(area, num4);
                            if (num5 > 0f && num3 > num5 + 2f)
                            {
                                return CombatService.DodgeResult.None;
                            }
                            if (cfg.DodgeSkipAttached)
                            {
                                try
                                {
                                    SkillConfig config2 = skillState.Config;
                                    if (config2 != null && config2.Attached)
                                    {
                                        return CombatService.DodgeResult.None;
                                    }
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                }
                float num6 = Mathf.Max(0.05f, cfg.DodgeLeadTime);
                if (num2 > 0f && num > 0f && num > num6)
                {
                    return CombatService.DodgeResult.Pending;
                }
                if (skills.IsCasting || !skills.CanCast(null))
                {
                    return CombatService.DodgeResult.Pending;
                }
                System.Collections.Generic.List<SkillData> assignedSkillList = CombatService.GetAssignedSkillList(player);
                if (assignedSkillList != null && assignedSkillList.Count > 0)
                {
                    int mana = skills.Mana;
                    int num7 = Math.Min(assignedSkillList.Count, 20);
                    for (int i = 0; i < num7; i++)
                    {
                        SkillData skillData = assignedSkillList[i];
                        if (skillData != null && !string.IsNullOrEmpty(skillData.Id))
                        {
                            SkillState anySkill = skills.GetAnySkill(skillData.Id);
                            if (anySkill != null && !anySkill.IsOnCooldown && anySkill.Cost <= mana)
                            {
                                SkillConfig config3 = anySkill.Config;
                                if (!(config3 == null) && CombatService.IsDashLikeSkill(anySkill, config3) && CombatService.SendDashSkill(player, baseUnitController, threatTarget, anySkill, i, status))
                                {
                                    return CombatService.DodgeResult.Executed;
                                }
                            }
                        }
                    }
                }
                if (CombatService.TryFallbackDodgeRoll(baseUnitController, threatTarget, status))
                {
                    return CombatService.DodgeResult.Executed;
                }
                return CombatService.DodgeResult.Pending;
            }
            catch (Exception ex)
            {
                MelonLogger.Warning("[Combat] TryAutoDashEvade failed: " + ex.Message);
            }
            return CombatService.DodgeResult.None;
        }

        // Token: 0x0600005B RID: 91 RVA: 0x00004500 File Offset: 0x00002700
        private static bool TryFallbackDodgeRoll(BaseUnitController self, BaseUnitController threatTarget, BotStatus status)
        {
            bool result;
            try
            {
                MoveComponent move = self.Move;
                if (move == null || move.DodgeCooldown > 0f)
                {
                    result = false;
                }
                else
                {
                    Vector3 position = self.Position;
                    Vector3 vector = position - threatTarget.Position;
                    vector.y = 0f;
                    if (vector.sqrMagnitude < 0.01f)
                    {
                        vector = Vector3.right;
                    }
                    Vector3 normalized = vector.normalized;
                    self.DodgeRoll(position, normalized);
                    PlayerController player = GameStateService.Player;
                    if (player != null)
                    {
                        player.SendInputsToServer(new PlayerInputDto
                        {
                            DodgeStartPosition = Extensions.CompressV3(position),
                            DodgeDirection = Extensions.CompressV3(normalized)
                        });
                    }
                    if (status != null)
                    {
                        status.SkillsCastCount++;
                    }
                    CombatTelemetryService.RecordSent("dodge-roll");
                    result = true;
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Warning("[Combat] Fallback DodgeRoll failed: " + ex.Message);
                result = false;
            }
            return result;
        }

        // Token: 0x0600005C RID: 92 RVA: 0x00004600 File Offset: 0x00002800
        private static bool IsDashLikeSkill(SkillState state, SkillConfig config)
        {
            string str = null;
            string str2 = null;
            try
            {
                str = config.DisplayName;
            }
            catch
            {
            }
            try
            {
                str2 = config.Id;
            }
            catch
            {
            }
            string text = (str + " " + str2).ToLowerInvariant();
            return text.Contains("dash") || text.Contains("blink") || text.Contains("dodge") || text.Contains("evade") || text.Contains("roll") || text.Contains("step");
        }

        // Token: 0x0600005D RID: 93 RVA: 0x000046A8 File Offset: 0x000028A8
        private static bool SendDashSkill(PlayerController player, BaseUnitController self, BaseUnitController threatTarget, SkillState skillState, int slot, BotStatus status)
        {
            SkillConfig config = skillState.Config;
            if (config == null)
            {
                return false;
            }
            int castType = (int)config.CastType;
            int targetType = (int)config.TargetType;
            player.SkillReady = skillState;
            if (castType == 1)
            {
                BaseUnitController baseUnitController = (targetType == 1 || targetType == 3) ? self : threatTarget;
                if (baseUnitController == null)
                {
                    player.SkillReady = null;
                    return false;
                }
                player.ProcessClickedUnit(baseUnitController);
                player.SendInputsToServer(new PlayerInputDto
                {
                    Click = true,
                    UnitId = baseUnitController.Cast<NetworkBehaviour>().ObjectId,
                    ClickSkillIndex = slot
                });
                player.SkillReady = null;
            }
            else if (castType == 2)
            {
                Vector3 vector = self.Position - threatTarget.Position;
                vector.y = 0f;
                if (vector.sqrMagnitude < 0.01f)
                {
                    vector = Vector3.right;
                }
                Vector3 normalized = vector.normalized;
                Vector3 vector2 = Vector3.Cross(Vector3.up, normalized).normalized * ((Random.value < 0.5f) ? -1f : 1f);
                Vector3 vector3 = self.Position + normalized * 6f + vector2 * 2f;
                player.ProcessClickedPosition(vector3);
                player.SendInputsToServer(new PlayerInputDto
                {
                    Click = true,
                    ClickPosition = Extensions.CompressV3(vector3),
                    ClickSkillIndex = slot
                });
                player.SkillReady = null;
            }
            else
            {
                player.SendInputsToServer(new PlayerInputDto
                {
                    ClickSkillIndex = slot
                });
                player.SkillReady = null;
            }
            if (status != null)
            {
                status.SkillRequestsSent++;
                status.SkillsCastCount++;
            }
            CombatService.RecordSkillCastTime(slot);
            DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 1);
            defaultInterpolatedStringHandler.AppendLiteral("dash-slot=");
            defaultInterpolatedStringHandler.AppendFormatted<int>(slot);
            CombatTelemetryService.RecordSent(defaultInterpolatedStringHandler.ToStringAndClear());
            return true;
        }

        public static void DebugBuffDisplayContents()
        {
            try
            {
                var player = GameStateService.Player;
                var status = player?.Status;

                if (status == null)
                {
                    MelonLogger.Warning("[BuffDebug] status null");
                    return;
                }

                MelonLogger.Msg("========== BUFF DISPLAY CONTENTS ==========");

                DumpStatusDict("SkillDisplays_C", status.SkillDisplays_C);
                DumpStatusDict("StatusDisplays_C", status.StatusDisplays_C);
                DumpStatusDict("EffectsDictionary", status.EffectsDictionary);

                MelonLogger.Msg("===========================================");
            }
            catch (Exception ex)
            {
                MelonLogger.Error("[BuffDebug] failed: " + ex);
            }
        }

        private static void DumpStatusDict(
            string label,
            Il2CppSystem.Collections.Generic.Dictionary<string, StatusEffectState> dict)
        {
            try
            {
                if (dict == null)
                {
                    MelonLogger.Msg($"[BuffDebug] {label}=null");
                    return;
                }

                MelonLogger.Msg($"[BuffDebug] {label}.Count={dict.Count}");

                foreach (var pair in dict)
                {
                    string key = pair.key?.ToString() ?? "";
                    var value = pair.value;

                    MelonLogger.Msg(
                        $"[BuffDebug] {label} key='{key}' value='{value}' duration='{GetStatusDuration(value)}'"
                    );
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"[BuffDebug] {label} dump failed: {ex.Message}");
            }
        }

        private static float GetStatusDuration(StatusEffectState state)
        {
            if (state == null)
                return -1f;

            try { return state.Duration; } catch { }

            return -1f;
        }

        // Token: 0x04000078 RID: 120
        private static readonly System.Collections.Generic.List<CombatService.SkillCandidate> _candidates = new System.Collections.Generic.List<CombatService.SkillCandidate>();

        // Token: 0x04000079 RID: 121
        private static SkillSlotInfo[] _cachedSkillInfos;

        // Token: 0x0400007A RID: 122
        private static float _skillInfoCacheTime = -1f;

        // Token: 0x0400007B RID: 123
        private const float SkillInfoCacheDuration = 60f;

        // Token: 0x0400007C RID: 124
        private static readonly float[] _lastSkillCastTimes = new float[20];

        // Token: 0x02000038 RID: 56
        public enum DodgeResult
        {
            // Token: 0x04000122 RID: 290
            None,
            // Token: 0x04000123 RID: 291
            Pending,
            // Token: 0x04000124 RID: 292
            Executed
        }

        // Token: 0x02000039 RID: 57
        private struct SkillCandidate
        {
            // Token: 0x04000125 RID: 293
            public int Slot;

            // Token: 0x04000126 RID: 294
            public int EffectivePriority;

            // Token: 0x04000127 RID: 295
            public SkillState State;

            // Token: 0x04000128 RID: 296
            public int CastType;

            // Token: 0x04000129 RID: 297
            public int TargetType;
        }
    }
}
