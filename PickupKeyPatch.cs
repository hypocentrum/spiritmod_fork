using System;
using HarmonyLib;
using Il2Cpp;

namespace SpiritMod
{
	// Token: 0x02000011 RID: 17
	[HarmonyPatch(typeof(HotkeyManager), "GetKeyDown")]
	internal static class PickupKeyPatch
	{
		// Token: 0x06000073 RID: 115 RVA: 0x00004DD3 File Offset: 0x00002FD3
		private static void Postfix(int __0, ref bool __result)
		{
			if (__0 == 47 && BotController._simulatePickup)
			{
				__result = true;
			}
		}
	}
}
