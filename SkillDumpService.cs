using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppSystem.Collections.Generic;
using MelonLoader;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace SpiritMod
{
    public static class SkillDumpService
    {
        private const string Separator = ",";

        public static string DumpEquippedSkills()
        {
            try
            {
                PlayerController player = GameStateService.Player;
                if (player == null)
                    return "Error: No player found";
                BaseUnitController baseUnitController = ((Il2CppObjectBase)player).Cast<BaseUnitController>();
                SkillsComponent skills = baseUnitController.Skills;
                if (skills == null)
                    return "Error: Skills component null";
                Il2CppSystem.Collections.Generic.Dictionary<string, StatusEffectState> effectDisplaysC = baseUnitController.Status?.EffectsDictionary;
                System.Collections.Generic.List<SkillData> assignedSkillList = CombatService.GetAssignedSkillList(player);
                if (assignedSkillList == null || assignedSkillList.Count == 0)
                    return "Error: No assigned skills";
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine(string.Join(",", "Slot", "SkillId", "DisplayName", "Level", "SS_Cooldown", "SS_Cost", "SS_CastTime", "SS_Duration", "SS_Delay", "SS_Area", "SS_Hits", "SS_Autocast", "SS_IsOnCooldown", "SS_IsHealing", "SS_IsDamage", "SC_CastType", "SC_TargetType", "SC_DamageType", "SC_Element", "SC_Range", "SC_Bond", "SC_Attached", "SC_Teleport", "SC_Piercing", "SC_TriggerHit", "SC_TriggerAutocast", "SC_Hybrid", "SC_MaxLv", "Mod_IsBuff", "Mod_IsSummon", "Mod_IsBond", "Mod_IsMount", "Mod_IsPermanent", "StatusEffects", "SelfStatusEffects", "LiveEffects", "USER_IsBuff", "USER_IsPermanent", "USER_Notes"));
                int num = Math.Min(assignedSkillList.Count, 20);
                for (int index1 = 0; index1 < num; ++index1)
                {
                    SkillData skillData = assignedSkillList[index1];
                    if (skillData != null && !string.IsNullOrEmpty(skillData.Id))
                    {
                        string id = skillData.Id;
                        SkillState skillState = (SkillState)null;
                        try
                        {
                            skillState = skills.GetAnySkill(id);
                        }
                        catch
                        {
                        }
                        SkillConfig cfg = (SkillConfig)null;
                        try
                        {
                            cfg = skillState?.Config;
                        }
                        catch
                        {
                        }
                        string s = "";
                        try
                        {
                            s = ((BaseConfig)cfg)?.DisplayName ?? "";
                        }
                        catch
                        {
                        }
                        stringBuilder.Append(index1);
                        stringBuilder.Append(",");
                        stringBuilder.Append(SkillDumpService.Esc(id));
                        stringBuilder.Append(",");
                        stringBuilder.Append(SkillDumpService.Esc(s));
                        stringBuilder.Append(",");
                        stringBuilder.Append(skillState != null ? skillState.Level : skillData.Level);
                        stringBuilder.Append(",");
                        if (skillState != null)
                        {
                            stringBuilder.Append(skillState.Cooldown);
                            stringBuilder.Append(",");
                            stringBuilder.Append(skillState.Cost);
                            stringBuilder.Append(",");
                            stringBuilder.Append(skillState.CastTime);
                            stringBuilder.Append(",");
                            stringBuilder.Append(skillState.Duration);
                            stringBuilder.Append(",");
                            stringBuilder.Append(skillState.Delay);
                            stringBuilder.Append(",");
                            stringBuilder.Append(skillState.Area);
                            stringBuilder.Append(",");
                            stringBuilder.Append(skillState.Hits);
                            stringBuilder.Append(",");
                            stringBuilder.Append(skillState.Autocast);
                            stringBuilder.Append(",");
                            stringBuilder.Append(skillState.IsOnCooldown);
                            stringBuilder.Append(",");
                            stringBuilder.Append(skillState.IsHealing);
                            stringBuilder.Append(",");
                            stringBuilder.Append(skillState.IsDamage);
                            stringBuilder.Append(",");
                        }
                        else
                        {
                            for (int index2 = 0; index2 < 11; ++index2)
                                stringBuilder.Append(",");
                        }
                        if (cfg == null)
                        {
                            stringBuilder.Append((int)cfg.CastType);
                            stringBuilder.Append(",");
                            stringBuilder.Append((int)cfg.TargetType);
                            stringBuilder.Append(",");
                            stringBuilder.Append((int)cfg.DamageType);
                            stringBuilder.Append(",");
                            stringBuilder.Append((int)cfg.Element);
                            stringBuilder.Append(",");
                            stringBuilder.Append(cfg.Range);
                            stringBuilder.Append(",");
                            stringBuilder.Append(cfg.Bond);
                            stringBuilder.Append(",");
                            stringBuilder.Append(cfg.Attached);
                            stringBuilder.Append(",");
                            stringBuilder.Append(cfg.Teleport);
                            stringBuilder.Append(",");
                            stringBuilder.Append(cfg.Piercing);
                            stringBuilder.Append(",");
                            stringBuilder.Append(cfg.TriggerHit);
                            stringBuilder.Append(",");
                            stringBuilder.Append(cfg.TriggerAutocast);
                            stringBuilder.Append(",");
                            stringBuilder.Append(cfg.Hybrid);
                            stringBuilder.Append(",");
                            stringBuilder.Append(((BaseSkillConfig)cfg).MaxLv);
                            stringBuilder.Append(",");
                        }
                        else
                        {
                            for (int index3 = 0; index3 < 13; ++index3)
                                stringBuilder.Append(",");
                        }
                        stringBuilder.Append(cfg == null && CombatService.IsBuffSkill(cfg));
                        stringBuilder.Append(",");
                        stringBuilder.Append(cfg == null && CombatService.IsSummonSkill(cfg));
                        stringBuilder.Append(",");
                        stringBuilder.Append(cfg == null && CombatService.IsBondSkill(cfg));
                        stringBuilder.Append(",");
                        stringBuilder.Append(cfg == null && CombatService.IsMountSkill(cfg));
                        stringBuilder.Append(",");
                        stringBuilder.Append(cfg == null && CombatService.IsPermanentBuff(cfg));
                        stringBuilder.Append(",");
                        stringBuilder.Append(SkillDumpService.Esc(SkillDumpService.FormatStatusList(cfg?.StatusEffects, skillState != null ? skillState.Level : 1)));
                        stringBuilder.Append(",");
                        stringBuilder.Append(SkillDumpService.Esc(SkillDumpService.FormatStatusList(cfg?.SelfStatusEffects, skillState != null ? skillState.Level : 1)));
                        stringBuilder.Append(",");
                        stringBuilder.Append(SkillDumpService.Esc(SkillDumpService.FormatLiveEffects(effectDisplaysC, cfg)));
                        stringBuilder.Append(",");
                        stringBuilder.Append(",");
                        stringBuilder.Append(",");
                        stringBuilder.AppendLine();
                    }
                }
                string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".", "skill_dump.csv");
                File.WriteAllText(path, stringBuilder.ToString());
                MelonLogger.Msg("[SkillDump] Wrote " + path);
                return path;
            }
            catch (Exception ex)
            {
                MelonLogger.Warning("[SkillDump] Failed: " + ex.Message);
                return "Error: " + ex.Message;
            }
        }

        private static string FormatStatusList(Il2CppSystem.Collections.Generic.List<SkillStatus> list, int level)
        {
            if (list == null || list.Count == 0)
                return "";
            StringBuilder stringBuilder1 = new StringBuilder();
            for (int index = 0; index < list.Count; ++index)
            {
                SkillStatus skillStatus = list[index];
                if (skillStatus != null)
                {
                    if (stringBuilder1.Length > 0)
                        stringBuilder1.Append(" | ");
                    float num1 = skillStatus.Duration + skillStatus.DurationLv * (float)level;
                    float num2 = skillStatus.Chance + skillStatus.ChanceLv * (float)level;
                    StringBuilder stringBuilder2 = stringBuilder1;
                    StringBuilder stringBuilder3 = stringBuilder2;
                    StringBuilder.AppendInterpolatedStringHandler interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(43, 10, stringBuilder2);
                    interpolatedStringHandler.AppendFormatted(skillStatus.Id);
                    interpolatedStringHandler.AppendLiteral("(dur=");
                    interpolatedStringHandler.AppendFormatted<float>(skillStatus.Duration);
                    interpolatedStringHandler.AppendLiteral("+");
                    interpolatedStringHandler.AppendFormatted<float>(skillStatus.DurationLv);
                    interpolatedStringHandler.AppendLiteral("/lv=");
                    interpolatedStringHandler.AppendFormatted<float>(num1, "F1");
                    interpolatedStringHandler.AppendLiteral(" ");
                    interpolatedStringHandler.AppendLiteral("chance=");
                    interpolatedStringHandler.AppendFormatted<float>(skillStatus.Chance);
                    interpolatedStringHandler.AppendLiteral("+");
                    interpolatedStringHandler.AppendFormatted<float>(skillStatus.ChanceLv);
                    interpolatedStringHandler.AppendLiteral("/lv=");
                    interpolatedStringHandler.AppendFormatted<float>(num2, "F1");
                    interpolatedStringHandler.AppendLiteral(" ");
                    interpolatedStringHandler.AppendLiteral("stacks=");
                    interpolatedStringHandler.AppendFormatted<int>(skillStatus.Stacks);
                    interpolatedStringHandler.AppendLiteral("+");
                    interpolatedStringHandler.AppendFormatted<int>(skillStatus.StacksLv);
                    interpolatedStringHandler.AppendLiteral("/lv ");
                    interpolatedStringHandler.AppendLiteral("fixed=");
                    interpolatedStringHandler.AppendFormatted<bool>(skillStatus.FixedDuration);
                    interpolatedStringHandler.AppendLiteral(")");
                    ref StringBuilder.AppendInterpolatedStringHandler local = ref interpolatedStringHandler;
                    stringBuilder3.Append(ref local);
                }
            }
            return stringBuilder1.ToString();
        }

        private static string FormatLiveEffects(
          Il2CppSystem.Collections.Generic.Dictionary<string, StatusEffectState> displays,
          SkillConfig cfg)
        {
            if (displays == null || cfg == null)
                return "";
            StringBuilder sb = new StringBuilder();
            try
            {
                Check(cfg.StatusEffects, "SE", ref sb);
            }
            catch
            {
            }
            try
            {
                Check(cfg.SelfStatusEffects, "Self", ref sb);
            }
            catch
            {
            }
            return sb.ToString();

            void Check(Il2CppSystem.Collections.Generic.List<SkillStatus> list, string tag, ref StringBuilder sb_)
            {
                if (list == null)
                    return;
                for (int index = 0; index < list.Count; ++index)
                {
                    SkillStatus skillStatus = list[index];
                    if (skillStatus != null && !string.IsNullOrEmpty(skillStatus.Id) && displays.ContainsKey(skillStatus.Id))
                    {
                        StatusEffectState display = displays[skillStatus.Id];
                        if (display != null)
                        {
                            if (sb_.Length > 0)
                                sb_.Append(" | ");
                            StringBuilder stringBuilder = sb_;
                            StringBuilder.AppendInterpolatedStringHandler interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(26, 5, sb);
                            interpolatedStringHandler.AppendFormatted(tag);
                            interpolatedStringHandler.AppendLiteral(":");
                            interpolatedStringHandler.AppendFormatted(skillStatus.Id);
                            interpolatedStringHandler.AppendLiteral("(dur=");
                            interpolatedStringHandler.AppendFormatted<float>(display.Duration, "F2");
                            interpolatedStringHandler.AppendLiteral(" stacks=");
                            interpolatedStringHandler.AppendFormatted<int>(display.Stacks);
                            interpolatedStringHandler.AppendLiteral(" stackable=");
                            interpolatedStringHandler.AppendFormatted<bool>(display.Stackable);
                            interpolatedStringHandler.AppendLiteral(")");
                            ref StringBuilder.AppendInterpolatedStringHandler local = ref interpolatedStringHandler;
                            stringBuilder.Append(ref local);
                        }
                    }
                }
            }
        }

        private static string Esc(string s)
        {
            if (string.IsNullOrEmpty(s))
                return "";
            return s.Contains(",") || s.Contains("\"") || s.Contains("\n") ? $"\"{s.Replace("\"", "\"\"")}\"" : s;
        }
    }
}
