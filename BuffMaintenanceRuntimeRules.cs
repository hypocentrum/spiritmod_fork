using System;
using Il2Cpp;

namespace SpiritMod
{
    public static class BuffMaintenanceRuntimeRules
    {
        public static bool IsPermanentUntilDeath(SkillConfig config)
        {
            return config.Duration.Value <= 0;
        }

        public static bool IsPermanentUntilDeath(SkillState skillState)
        {
            return skillState != null && IsPermanentUntilDeath(skillState.Config);
        }
    }
}
