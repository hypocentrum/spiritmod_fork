using System;
using HarmonyLib;
using Il2Cpp;
using Il2CppSystem.Collections.Generic;
using MelonLoader;

namespace SpiritMod
{
	// Token: 0x02000023 RID: 35
	[HarmonyPatch(typeof(PlayerSave), "AssignSkill")]
	internal static class PlayerSaveAssignSkillPatch
	{
		// Token: 0x06000111 RID: 273 RVA: 0x0000D158 File Offset: 0x0000B358
		private static void Postfix(PlayerSave __instance)
		{
			try
			{
				CharacterData data = __instance.Data;
				Il2CppSystem.Collections.Generic.List<SkillData> list;
				if (data == null)
				{
					list = null;
				}
				else
				{
					SkillSystemData skills = data.Skills;
					list = ((skills != null) ? skills.Assigned : null);
				}
				Il2CppSystem.Collections.Generic.List<SkillData> list2 = list;
				if (list2 != null)
				{
					GameCache.Skills.Update(list2);
					MelonLogger.Msg("[GameCache] Skills cache refreshed after AssignSkill.");
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Warning("[Patch] PlayerSave.AssignSkill postfix error: " + ex.Message);
			}
		}
	}
}
