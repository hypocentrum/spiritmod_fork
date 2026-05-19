using System;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Il2Cpp;
using Il2CppSystem.Collections.Generic;
using MelonLoader;

namespace SpiritMod
{
	// Token: 0x02000022 RID: 34
	[HarmonyPatch(typeof(PlayerController), "Load")]
	internal static class PlayerControllerLoadPatch
	{
		// Token: 0x06000110 RID: 272 RVA: 0x0000D088 File Offset: 0x0000B288
		private static void Postfix(PlayerController __instance, CharacterUpdateType __1)
		{
			try
			{
				if (!GameCache.PlayerController.IsValid())
				{
					GameCache.PlayerController.Update(__instance);
					MelonLogger.Msg("[GameCache] PlayerController cached.");
					PlayerSave save = __instance.Save;
					Il2CppSystem.Collections.Generic.List<SkillData> list;
					if (save == null)
					{
						list = null;
					}
					else
					{
						CharacterData data = save.Data;
						if (data == null)
						{
							list = null;
						}
						else
						{
							SkillSystemData skills = data.Skills;
							list = ((skills != null) ? skills.Assigned : null);
						}
					}
					Il2CppSystem.Collections.Generic.List<SkillData> list2 = list;
					if (list2 != null)
					{
						GameCache.Skills.Update(list2);
                        CombatService.ForceRefreshSkillInfos();
                        MelonLogger.Msg($"[GameCache] Skills cached ({list2.Count} slots).");
                    }
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Warning("[Patch] PlayerController.Load postfix error: " + ex.Message);
			}
		}
	}
}
