using System;
using HarmonyLib;
using Il2Cpp;
using MelonLoader;

namespace SpiritMod
{
	// Token: 0x02000025 RID: 37
	[HarmonyPatch(typeof(UIManager), "ShowLogin")]
	internal static class UIManagerShowLoginPatch
	{
		// Token: 0x06000113 RID: 275 RVA: 0x0000D270 File Offset: 0x0000B470
		private static void Postfix(UIManager __instance)
		{
			try
			{
				GameCache.UIManager.Update(__instance);
				MelonLogger.Msg("[GameCache] UIManager cached (login screen).");
				if (BotController.Config.EnableAutoLogin)
				{
					AutoLoginService.ServerAutoConnect();
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Warning("[Patch] UIManager.ShowLogin postfix error: " + ex.Message);
			}
		}
	}
}
