using System;
using Il2Cpp;

namespace SpiritMod
{
    /// <summary>
    /// Central rules for skills that should NOT appear in Buff Maintenance
    /// and should NOT be handled by the buff maintenance recast logic.
    /// </summary>
    public static class BuffMaintenanceRules
    {
        public static bool IsExcludedBuffMaintenanceSkill(SkillConfig config)
        {
            if (config == null)
                return false;

            string id = config.Id ?? string.Empty;
            string name = config.DisplayName ?? string.Empty;

            // Conviction Aura creates/relates to Might, but Might is also affected by other sources.
            // Excluding it avoids confusing the UI state and prevents unwanted recast behavior.
            return id.Equals("Conviction", StringComparison.OrdinalIgnoreCase) ||
                   name.Equals("Conviction Aura", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsExcludedBuffMaintenanceSkill(SkillState skillState)
        {
            return skillState != null &&
                   IsExcludedBuffMaintenanceSkill(skillState.Config);
        }

        public static bool IsExcludedBuffMaintenanceSkill(SkillSlotInfo info)
        {
            if (info == null || string.IsNullOrEmpty(info.Name))
                return false;

            return info.Name.Equals("Conviction", StringComparison.OrdinalIgnoreCase) ||
                   info.Name.Equals("Conviction Aura", StringComparison.OrdinalIgnoreCase);
        }
    }
}
