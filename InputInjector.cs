using System;
using HarmonyLib;
using Il2Cpp;
using Il2CppFishNet.Object;
using MelonLoader;
using UnityEngine;

namespace SpiritMod
{
	// Token: 0x0200000F RID: 15
	[HarmonyPatch(typeof(PlayerController), "CaptureInputs")]
	internal static class InputInjector
	{
		// Token: 0x0600005F RID: 95 RVA: 0x000048BC File Offset: 0x00002ABC
		public static void QueueClickUnit(BaseUnitController target, int clickSkillIndex = -1)
		{
			InputInjector._hasPendingAction = true;
			InputInjector._pendingUnitId = target.Cast<NetworkBehaviour>().ObjectId;
			InputInjector._pendingClickSkillIndex = clickSkillIndex;
			InputInjector._pendingClickPosition = null;
			InputInjector._pendingTarget = target;
		}

		// Token: 0x06000060 RID: 96 RVA: 0x000048EB File Offset: 0x00002AEB
		public static void QueueClickPosition(Vector3 position, int clickSkillIndex)
		{
			InputInjector._hasPendingAction = true;
			InputInjector._pendingUnitId = 0;
			InputInjector._pendingClickSkillIndex = clickSkillIndex;
			InputInjector._pendingClickPosition = new Vector3?(position);
			InputInjector._pendingTarget = null;
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00004910 File Offset: 0x00002B10
		public static void QueueSkillOnly(int clickSkillIndex)
		{
			InputInjector._hasPendingAction = true;
			InputInjector._pendingUnitId = 0;
			InputInjector._pendingClickSkillIndex = clickSkillIndex;
			InputInjector._pendingClickPosition = null;
			InputInjector._pendingTarget = null;
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00004935 File Offset: 0x00002B35
		public static void Clear()
		{
			InputInjector._hasPendingAction = false;
			InputInjector._pendingTarget = null;
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00004944 File Offset: 0x00002B44
		private static void Postfix(PlayerController __instance, ref bool __result)
		{
			if (!InputInjector._hasPendingAction)
			{
				return;
			}
			try
			{
				InputInjector._hasPendingAction = false;
				PlayerInputDto inputs = __instance.Inputs;
				inputs.Click = true;
				inputs.UnitId = InputInjector._pendingUnitId;
				inputs.ClickSkillIndex = InputInjector._pendingClickSkillIndex;
				if (InputInjector._pendingClickPosition != null)
				{
					inputs.ClickPosition = Extensions.CompressV3(InputInjector._pendingClickPosition.Value);
				}
				__instance.Inputs = inputs;
				if (InputInjector._pendingTarget != null)
				{
					__instance.ProcessClickedUnit(InputInjector._pendingTarget);
					InputInjector._pendingTarget = null;
				}
				else if (InputInjector._pendingClickPosition != null)
				{
					__instance.ProcessClickedPosition(InputInjector._pendingClickPosition.Value);
				}
				__result = true;
			}
			catch (Exception ex)
			{
				MelonLogger.Warning("[InputInjector] Postfix error: " + ex.Message);
				InputInjector._pendingTarget = null;
			}
		}

		// Token: 0x0400007D RID: 125
		private static bool _hasPendingAction;

		// Token: 0x0400007E RID: 126
		private static int _pendingUnitId;

		// Token: 0x0400007F RID: 127
		private static int _pendingClickSkillIndex = -1;

		// Token: 0x04000080 RID: 128
		private static Vector3? _pendingClickPosition;

		// Token: 0x04000081 RID: 129
		private static BaseUnitController _pendingTarget;
	}
}
