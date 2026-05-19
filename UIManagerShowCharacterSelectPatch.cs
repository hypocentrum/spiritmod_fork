using System;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Il2Cpp;
using Il2CppSystem.Collections.Generic;
using MelonLoader;

namespace SpiritMod
{
	// Token: 0x02000024 RID: 36
	[HarmonyPatch(typeof(UIManager), "ShowCharacterSelect")]
	internal static class UIManagerShowCharacterSelectPatch
	{
		// Token: 0x06000112 RID: 274 RVA: 0x0000D1C8 File Offset: 0x0000B3C8
		private static void Postfix(UIManager __instance, Il2CppSystem.Collections.Generic.List<CharacterData> __0)
		{
			try
			{
				GameCache.UIManager.Update(__instance);
				MelonLogger.Msg("[GameCache] UIManager cached.");
				if (__0 != null)
				{
					GameCache.Characters.Update(__0);
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(44, 1);
					defaultInterpolatedStringHandler.AppendLiteral("[GameCache] Characters cached (");
					defaultInterpolatedStringHandler.AppendFormatted<int>(__0.Count);
					defaultInterpolatedStringHandler.AppendLiteral(" characters).");
					MelonLogger.Msg(defaultInterpolatedStringHandler.ToStringAndClear());
				}
				if (BotController.Config.EnableAutoLogin)
				{
					AutoLoginService.CharacterSelection(__instance);
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Warning("[Patch] UIManager.ShowCharacterSelect postfix error: " + ex.Message);
			}
		}
	}
}
