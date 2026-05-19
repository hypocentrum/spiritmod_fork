using System;
using System.Collections;
using Il2Cpp;
using Il2CppSystem.Collections.Generic;
using MelonLoader;
using UnityEngine;

namespace SpiritMod
{
	// Token: 0x0200001D RID: 29
	public static class AutoLoginService
	{
		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060000F5 RID: 245 RVA: 0x0000C651 File Offset: 0x0000A851
		// (set) Token: 0x060000F6 RID: 246 RVA: 0x0000C658 File Offset: 0x0000A858
		public static string StatusMessage { get; private set; } = "";

		// Token: 0x060000F7 RID: 247 RVA: 0x0000C660 File Offset: 0x0000A860
		public static string GetInstanceId(string serverName)
		{
			for (int i = 0; i < AutoLoginService.ServerNames.Length; i++)
			{
				if (string.Equals(AutoLoginService.ServerNames[i], serverName, StringComparison.OrdinalIgnoreCase))
				{
					return AutoLoginService.ServerInstanceIds[i];
				}
			}
			return null;
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x0000C698 File Offset: 0x0000A898
		public static void CharacterSelection(UIManager uiManager)
		{
			try
			{
				UICharacterSelect uicharacterSelect;
				if (uiManager == null)
				{
					uicharacterSelect = null;
				}
				else
				{
					GameObject gameObject = uiManager.Current;
					uicharacterSelect = ((gameObject != null) ? gameObject.GetComponent<UICharacterSelect>() : null);
				}
				UICharacterSelect uicharacterSelect2 = uicharacterSelect;
				if (uicharacterSelect2 == null)
				{
					MelonLogger.Error("[AutoLogin] UICharacterSelect component not found.");
					AutoLoginService.StatusMessage = "Error: UICharacterSelect not found";
				}
				else
				{
					string characterName = ConfigService.CharacterName;
					if (string.IsNullOrEmpty(characterName))
					{
						MelonLogger.Warning("[AutoLogin] No character name configured.");
						AutoLoginService.StatusMessage = "No character name set";
					}
					else
					{
						CharacterData characterData = AutoLoginService.FindCharacterByName(characterName);
						if (characterData == null)
						{
							MelonLogger.Error("[AutoLogin] Character '" + characterName + "' not found.");
							AutoLoginService.StatusMessage = "Character '" + characterName + "' not found";
						}
						else
						{
							uicharacterSelect2.PlayCharacter(characterData);
							MelonLogger.Msg("[AutoLogin] Selected character: " + characterData.Name);
							AutoLoginService.StatusMessage = "Selected '" + characterData.Name + "'";
						}
					}
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Warning("[AutoLogin] CharacterSelection failed: " + ex.Message);
				AutoLoginService.StatusMessage = "Error: " + ex.Message;
			}
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x0000C7B4 File Offset: 0x0000A9B4
		private static CharacterData FindCharacterByName(string name)
		{
			if (!GameCache.Characters.IsValid())
			{
				return null;
			}
			Il2CppSystem.Collections.Generic.List<CharacterData> value = GameCache.Characters.Value;
			for (int i = 0; i < value.Count; i++)
			{
				CharacterData characterData = value[i];
				if (characterData != null && string.Equals(characterData.Name, name, StringComparison.OrdinalIgnoreCase))
				{
					return characterData;
				}
			}
			return null;
		}

		// Token: 0x060000FA RID: 250 RVA: 0x0000C808 File Offset: 0x0000AA08
		public static void ServerAutoConnect()
		{
			MelonCoroutines.Start(AutoLoginService.ServerAutoConnectCoroutine());
		}

        // Token: 0x060000FB RID: 251 RVA: 0x0000C815 File Offset: 0x0000AA15
        private static IEnumerator ServerAutoConnectCoroutine()
        {
            AutoLoginService.StatusMessage = "Waiting for server list...";
            MelonLogger.Msg("[AutoLogin] Waiting for server list to populate...");
            UILogin uiLogin = (UILogin)null;
            Il2CppSystem.Collections.Generic.Dictionary<string, ServerUIEntry> serverMap = new Il2CppSystem.Collections.Generic.Dictionary<string, ServerUIEntry>();
            float elapsed = 0.0f;
            while ((double)elapsed < 10.0)
            {
                yield return (object)new WaitForSeconds(0.5f);
                elapsed += 0.5f;
                try
                {
                    uiLogin = UnityEngine.Object.FindObjectOfType<UILogin>();
                    if (uiLogin == null)
                    {
                        MelonLogger.Msg("[AutoLogin] UILogin disappeared, aborting.");
                        AutoLoginService.StatusMessage = "";
                        yield break;
                    }
                    serverMap = uiLogin.serverUIMap;
                    if (serverMap != null)
                    {
                        if (serverMap.Count > 0)
                            break;
                    }
                }
                catch
                {
                }
            }
            if (serverMap != null)
            {
                if (serverMap.Count != 0)
                {
                    try
                    {
                        string autoLoginServerName = BotController.Config.AutoLoginServerName;
                        string str = AutoLoginService.GetInstanceId(autoLoginServerName);
                        if (str == null)
                        {
                            Il2CppSystem.Collections.Generic.Dictionary<string, ServerUIEntry>.Enumerator enumerator = serverMap.GetEnumerator();
                            if (enumerator.MoveNext())
                                str = enumerator.Current.Key;
                            MelonLogger.Msg($"[AutoLogin] Server '{autoLoginServerName}' unknown, falling back to '{str}'.");
                        }
                        else
                            MelonLogger.Msg($"[AutoLogin] Selecting server '{autoLoginServerName}' (id: {str}).");
                        AutoLoginService.StatusMessage = $"Connecting to '{autoLoginServerName}'...";
                        uiLogin.SetSelected(str);
                        uiLogin.Connect();
                        MelonLogger.Msg("[AutoLogin] Connect called.");
                        AutoLoginService.StatusMessage = "Connecting...";
                        yield break;
                    }
                    catch (Exception ex)
                    {
                        MelonLogger.Warning("[AutoLogin] Server selection failed: " + ex.Message);
                        AutoLoginService.StatusMessage = "Error: " + ex.Message;
                        yield break;
                    }
                }
            }
            MelonLogger.Warning("[AutoLogin] Server list never populated.");
            AutoLoginService.StatusMessage = "Server list timeout";
        }

        // Token: 0x040000DC RID: 220
        public static readonly string[] ServerNames = new string[]
		{
			"Star",
			"Sun",
			"Moon",
			"Luna",
			"Comet",
			"Aurora"
		};

		// Token: 0x040000DD RID: 221
		private static readonly string[] ServerInstanceIds = new string[]
		{
			"hetzner-eu-1",
			"hetzner-us-1",
			"hetzner-sea-1",
			"ovh-sea-1",
			"wyze-sa-1",
			"ovh-aus-1"
		};
	}
}
