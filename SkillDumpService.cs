using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppSystem.Collections.Generic;
using MelonLoader;
using System;
using System.IO;
using System.Reflection;
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

                BaseUnitController baseUnitController = null;

                try
                {
                    baseUnitController = ((Il2CppObjectBase)player).Cast<BaseUnitController>();
                }
                catch
                {
                    try
                    {
                        baseUnitController = player.Cast<BaseUnitController>();
                    }
                    catch
                    {
                        return "Error: Could not cast player to BaseUnitController";
                    }
                }

                if (baseUnitController == null)
                    return "Error: BaseUnitController null";

                SkillsComponent skills = baseUnitController.Skills;
                if (skills == null)
                    return "Error: Skills component null";

                Il2CppSystem.Collections.Generic.Dictionary<string, StatusEffectState> effectDisplays = null;

                try
                {
                    effectDisplays = baseUnitController.Status?.EffectsDictionary;
                }
                catch
                {
                    effectDisplays = null;
                }

                System.Collections.Generic.List<SkillData> assignedSkillList = CombatService.GetAssignedSkillList(player);
                if (assignedSkillList == null || assignedSkillList.Count == 0)
                    return "Error: No assigned skills";

                StringBuilder stringBuilder = new StringBuilder();

                stringBuilder.AppendLine(string.Join(Separator,
                    "Slot",
                    "SkillId",
                    "DisplayName",
                    "Level",
                    "SS_Cooldown",
                    "SS_Cost",
                    "SS_CastTime",
                    "SS_Duration",
                    "SS_Delay",
                    "SS_Area",
                    "SS_Hits",
                    "SS_Autocast",
                    "SS_IsOnCooldown",
                    "SS_IsHealing",
                    "SS_IsDamage",
                    "SC_CastType",
                    "SC_TargetType",
                    "SC_DamageType",
                    "SC_Element",
                    "SC_Range",
                    "SC_Bond",
                    "SC_Attached",
                    "SC_Teleport",
                    "SC_Piercing",
                    "SC_TriggerHit",
                    "SC_TriggerAutocast",
                    "SC_Hybrid",
                    "SC_MaxLv",
                    "Mod_IsBuff",
                    "Mod_IsSummon",
                    "Mod_IsBond",
                    "Mod_IsMount",
                    "Mod_IsPermanent",
                    "StatusEffects",
                    "SelfStatusEffects",
                    "LiveEffects",
                    "USER_IsBuff",
                    "USER_IsPermanent",
                    "USER_Notes"));

                int count = Math.Min(assignedSkillList.Count, 20);

                for (int slot = 0; slot < count; ++slot)
                {
                    SkillData skillData = assignedSkillList[slot];
                    if (skillData == null || string.IsNullOrEmpty(skillData.Id))
                        continue;

                    string id = skillData.Id;
                    SkillState skillState = null;

                    try
                    {
                        skillState = skills.GetAnySkill(id);
                    }
                    catch
                    {
                        skillState = null;
                    }

                    SkillConfig cfg = null;

                    try
                    {
                        cfg = skillState?.Config;
                    }
                    catch
                    {
                        cfg = null;
                    }

                    string displayName = string.Empty;

                    try
                    {
                        displayName = ((BaseConfig)cfg)?.DisplayName ?? string.Empty;
                    }
                    catch
                    {
                        displayName = string.Empty;
                    }

                    int level = 1;
                    try
                    {
                        level = skillState != null ? skillState.Level : skillData.Level;
                    }
                    catch
                    {
                        level = 1;
                    }

                    stringBuilder.Append(slot);
                    stringBuilder.Append(Separator);
                    stringBuilder.Append(Esc(id));
                    stringBuilder.Append(Separator);
                    stringBuilder.Append(Esc(displayName));
                    stringBuilder.Append(Separator);
                    stringBuilder.Append(level);
                    stringBuilder.Append(Separator);

                    AppendSkillStateColumns(stringBuilder, skillState);
                    AppendSkillConfigColumns(stringBuilder, cfg);

                    stringBuilder.Append(SafeBool(() => cfg != null && CombatService.IsBuffSkill(cfg)));
                    stringBuilder.Append(Separator);
                    stringBuilder.Append(SafeBool(() => cfg != null && CombatService.IsSummonSkill(cfg)));
                    stringBuilder.Append(Separator);
                    stringBuilder.Append(SafeBool(() => cfg != null && CombatService.IsBondSkill(cfg)));
                    stringBuilder.Append(Separator);
                    stringBuilder.Append(SafeBool(() => cfg != null && CombatService.IsMountSkill(cfg)));
                    stringBuilder.Append(Separator);
                    stringBuilder.Append(SafeBool(() => cfg != null && CombatService.IsPermanentBuff(cfg)));
                    stringBuilder.Append(Separator);

                    stringBuilder.Append(Esc(FormatStatusList(cfg?.StatusEffects, level)));
                    stringBuilder.Append(Separator);
                    stringBuilder.Append(Esc(FormatStatusList(cfg?.SelfStatusEffects, level)));
                    stringBuilder.Append(Separator);
                    stringBuilder.Append(Esc(FormatLiveEffects(effectDisplays, cfg)));
                    stringBuilder.Append(Separator);

                    // User-editable columns in CSV.
                    stringBuilder.Append(Separator); // USER_IsBuff
                    stringBuilder.Append(Separator); // USER_IsPermanent
                    stringBuilder.AppendLine();      // USER_Notes
                }

                string path = Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".",
                    "skill_dump.csv");

                File.WriteAllText(path, stringBuilder.ToString());
                MelonLogger.Msg("[SkillDump] Wrote " + path);

                return path;
            }
            catch (Exception ex)
            {
                MelonLogger.Warning("[SkillDump] Failed: " + ex);
                return "Error: " + ex.Message;
            }
        }

        private static void AppendSkillStateColumns(StringBuilder sb, SkillState skillState)
        {
            if (skillState != null)
            {
                sb.Append(SafeValue(() => skillState.Cooldown));
                sb.Append(Separator);
                sb.Append(SafeValue(() => skillState.Cost));
                sb.Append(Separator);
                sb.Append(SafeValue(() => skillState.CastTime));
                sb.Append(Separator);
                sb.Append(SafeValue(() => skillState.Duration));
                sb.Append(Separator);
                sb.Append(SafeValue(() => skillState.Delay));
                sb.Append(Separator);
                sb.Append(SafeValue(() => skillState.Area));
                sb.Append(Separator);
                sb.Append(SafeValue(() => skillState.Hits));
                sb.Append(Separator);
                sb.Append(SafeValue(() => skillState.Autocast));
                sb.Append(Separator);
                sb.Append(SafeValue(() => skillState.IsOnCooldown));
                sb.Append(Separator);
                sb.Append(SafeValue(() => skillState.IsHealing));
                sb.Append(Separator);
                sb.Append(SafeValue(() => skillState.IsDamage));
                sb.Append(Separator);
            }
            else
            {
                AppendEmptyColumns(sb, 11);
            }
        }

        private static void AppendSkillConfigColumns(StringBuilder sb, SkillConfig cfg)
        {
            // Important fix: this must be cfg != null.
            // The old repo version used cfg == null and then dereferenced cfg, causing NullReferenceException.
            if (cfg != null)
            {
                sb.Append(SafeValue(() => (int)cfg.CastType));
                sb.Append(Separator);
                sb.Append(SafeValue(() => (int)cfg.TargetType));
                sb.Append(Separator);
                sb.Append(SafeValue(() => (int)cfg.DamageType));
                sb.Append(Separator);
                sb.Append(SafeValue(() => (int)cfg.Element));
                sb.Append(Separator);
                sb.Append(SafeValue(() => cfg.Range));
                sb.Append(Separator);
                sb.Append(SafeValue(() => cfg.Bond));
                sb.Append(Separator);
                sb.Append(SafeValue(() => cfg.Attached));
                sb.Append(Separator);
                sb.Append(SafeValue(() => cfg.Teleport));
                sb.Append(Separator);
                sb.Append(SafeValue(() => cfg.Piercing));
                sb.Append(Separator);
                sb.Append(SafeValue(() => cfg.TriggerHit));
                sb.Append(Separator);
                sb.Append(SafeValue(() => cfg.TriggerAutocast));
                sb.Append(Separator);
                sb.Append(SafeValue(() => cfg.Hybrid));
                sb.Append(Separator);
                sb.Append(SafeValue(() => ((BaseSkillConfig)cfg).MaxLv));
                sb.Append(Separator);
            }
            else
            {
                AppendEmptyColumns(sb, 13);
            }
        }

        private static void AppendEmptyColumns(StringBuilder sb, int count)
        {
            for (int i = 0; i < count; ++i)
                sb.Append(Separator);
        }

        private static string FormatStatusList(Il2CppSystem.Collections.Generic.List<SkillStatus> list, int level)
        {
            if (list == null || list.Count == 0)
                return string.Empty;

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < list.Count; ++i)
            {
                SkillStatus skillStatus = null;

                try
                {
                    skillStatus = list[i];
                }
                catch
                {
                    skillStatus = null;
                }

                if (skillStatus == null)
                    continue;

                if (sb.Length > 0)
                    sb.Append(" | ");

                float totalDuration = SafeValue(() => skillStatus.Duration + skillStatus.DurationLv * level, 0f);
                float totalChance = SafeValue(() => skillStatus.Chance + skillStatus.ChanceLv * level, 0f);

                sb.Append(SafeValue(() => skillStatus.Id, string.Empty));
                sb.Append("(dur=");
                sb.Append(SafeValue(() => skillStatus.Duration, 0f));
                sb.Append("+");
                sb.Append(SafeValue(() => skillStatus.DurationLv, 0f));
                sb.Append("/lv=");
                sb.Append(totalDuration.ToString("F1"));
                sb.Append(" chance=");
                sb.Append(SafeValue(() => skillStatus.Chance, 0f));
                sb.Append("+");
                sb.Append(SafeValue(() => skillStatus.ChanceLv, 0f));
                sb.Append("/lv=");
                sb.Append(totalChance.ToString("F1"));
                sb.Append(" stacks=");
                sb.Append(SafeValue(() => skillStatus.Stacks, 0));
                //sb.Append("+");
                //sb.Append(SafeValue(() => skillStatus.StacksLv, 0));
                //sb.Append("/lv fixed=");
                //sb.Append(SafeValue(() => skillStatus.FixedDuration, false));
                sb.Append(")");
            }

            return sb.ToString();
        }

        private static string FormatLiveEffects(Il2CppSystem.Collections.Generic.Dictionary<string, StatusEffectState> displays, SkillConfig cfg)
        {
            if (displays == null || cfg == null)
                return string.Empty;

            StringBuilder sb = new StringBuilder();

            try
            {
                AppendLiveEffectMatches(displays, cfg.StatusEffects, "SE", sb);
            }
            catch
            {
                // Ignore individual dump failures; this is a diagnostic exporter.
            }

            try
            {
                AppendLiveEffectMatches(displays, cfg.SelfStatusEffects, "Self", sb);
            }
            catch
            {
                // Ignore individual dump failures; this is a diagnostic exporter.
            }

            return sb.ToString();
        }

        private static void AppendLiveEffectMatches(
            Il2CppSystem.Collections.Generic.Dictionary<string, StatusEffectState> displays,
            Il2CppSystem.Collections.Generic.List<SkillStatus> list,
            string tag,
            StringBuilder sb)
        {
            if (displays == null || list == null)
                return;

            for (int i = 0; i < list.Count; ++i)
            {
                SkillStatus skillStatus = null;

                try
                {
                    skillStatus = list[i];
                }
                catch
                {
                    skillStatus = null;
                }

                if (skillStatus == null || string.IsNullOrEmpty(skillStatus.Id))
                    continue;

                bool contains = false;

                try
                {
                    contains = displays.ContainsKey(skillStatus.Id);
                }
                catch
                {
                    contains = false;
                }

                if (!contains)
                    continue;

                StatusEffectState display = null;

                try
                {
                    display = displays[skillStatus.Id]?.Cast<StatusEffectState>();
                }
                catch
                {
                    try
                    {
                        display = displays[skillStatus.Id] as StatusEffectState;
                    }
                    catch
                    {
                        display = null;
                    }
                }

                if (display == null)
                    continue;

                if (sb.Length > 0)
                    sb.Append(" | ");

                sb.Append(tag);
                sb.Append(":");
                sb.Append(skillStatus.Id);
                sb.Append("(dur=");
                sb.Append(SafeValue(() => display.Duration, 0f).ToString("F2"));
                sb.Append(" stacks=");
                sb.Append(SafeValue(() => display.Stacks, 0));
                sb.Append(" stackable=");
                sb.Append(SafeValue(() => display.Stackable, false));
                sb.Append(")");
            }
        }

        private static string Esc(string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;

            return s.Contains(",") || s.Contains("\"") || s.Contains("\n") || s.Contains("\r")
                ? $"\"{s.Replace("\"", "\"\"")}\""
                : s;
        }

        private static bool SafeBool(Func<bool> getter)
        {
            try
            {
                return getter();
            }
            catch
            {
                return false;
            }
        }

        private static T SafeValue<T>(Func<T> getter, T fallback = default)
        {
            try
            {
                return getter();
            }
            catch
            {
                return fallback;
            }
        }
    }
}
