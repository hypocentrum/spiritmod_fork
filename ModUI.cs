using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Il2Cpp;
using MelonLoader;
using SpiritMod.States;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SpiritMod
{
	// Token: 0x02000014 RID: 20
	public static class ModUI
	{
		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600007A RID: 122 RVA: 0x00005A47 File Offset: 0x00003C47
		public static bool Visible
		{
			get
			{
				return ModUI._visible;
			}
		}

		// Token: 0x0600007B RID: 123 RVA: 0x00005A50 File Offset: 0x00003C50
		public static void Toggle()
		{
			ModUI._visible = !ModUI._visible;
			if (ModUI._visible)
			{
				ModUI._savedLockState = Cursor.lockState;
				ModUI._savedCursorVisible = Cursor.visible;
				ModUI._cursorCached = true;
				return;
			}
			if (ModUI._cursorCached)
			{
				Cursor.lockState = ModUI._savedLockState;
				Cursor.visible = ModUI._savedCursorVisible;
				ModUI._cursorCached = false;
			}
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00005AB0 File Offset: 0x00003CB0
		public static void Draw()
		{
			if (!ModUI._visible)
			{
				return;
			}
			Cursor.lockState = 0;
			Cursor.visible = true;
			ModUI.EnsureResources();
			ModUI._windowRect = GUI.Window(98765, ModUI._windowRect, new Action<int>(ModUI.DrawWindowContent), "SpiritMod");
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00005B00 File Offset: 0x00003D00
		private static void DrawWindowContent(int windowId)
		{
			try
			{
				ModUI._nextScrollId = 1;
				GUI.DrawTexture(new Rect(0f, 0f, ModUI._windowRect.width, ModUI._windowRect.height), ModUI._windowBgTex);
				GUI.Label(new Rect(20f, 30f, 400f, 40f), "SpiritBot 2.1.0 By CVSeason");
				if (GUI.Button(new Rect(20f, 75f, 100f, 35f), "Cheats"))
				{
					ModUI._currentTab = 0;
				}
				if (GUI.Button(new Rect(130f, 75f, 110f, 35f), "Teleporter"))
				{
					ModUI._currentTab = 1;
				}
				if (GUI.Button(new Rect(250f, 75f, 70f, 35f), "Bot"))
				{
					ModUI._currentTab = 2;
				}
				if (GUI.Button(new Rect(330f, 75f, 120f, 35f), "Skill Config"))
				{
					ModUI._currentTab = 3;
				}
				if (GUI.Button(new Rect(460f, 75f, 110f, 35f), "Loot Filter"))
				{
					ModUI._currentTab = 4;
				}
				if (GUI.Button(new Rect(580f, 75f, 100f, 35f), "Visuals"))
				{
					ModUI._currentTab = 5;
				}
				if (GUI.Button(new Rect(690f, 75f, 100f, 35f), "Settings"))
				{
					ModUI._currentTab = 6;
				}
				float startY = 120f;
				if (ModUI._currentTab == 0)
				{
					ModUI.DrawCheatsTab(startY);
				}
				else if (ModUI._currentTab == 1)
				{
					ModUI.DrawTeleporterTab(startY);
				}
				else if (ModUI._currentTab == 2)
				{
					ModUI.DrawBotTabManual(startY);
				}
				else if (ModUI._currentTab == 3)
				{
					ModUI.DrawSkillConfigTab(startY);
				}
				else if (ModUI._currentTab == 4)
				{
					ModUI.DrawLootFilterTab(startY);
				}
				else if (ModUI._currentTab == 5)
				{
					ModUI.DrawVisualsTab(startY);
				}
				else if (ModUI._currentTab == 6)
				{
					ModUI.DrawSettingsTab(startY);
				}
			}
			catch (Exception ex)
			{
				GUI.Label(new Rect(20f, 30f, 850f, 200f), "UI Error: " + ex.Message + "\n" + ex.StackTrace);
			}
			GUI.DragWindow(new Rect(0f, 0f, 10000f, 30f));
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00005D7C File Offset: 0x00003F7C
		private static float BeginScrollRegion(ref float scrollY, float startY, float contentHeight, float regionX = 20f, float regionWidth = 820f)
		{
			int scrollId = ModUI._nextScrollId++;
			float num = ModUI._windowRect.height - startY - 20f;
			if (num < 100f)
			{
				num = 100f;
			}
			float num2 = regionX + regionWidth;
			float num3 = 30f;
			GUI.DrawTexture(new Rect(num2, startY, num3, num), ModUI._scrollbarBgTex);
			if (contentHeight > num)
			{
				float num4 = contentHeight - num;
				float num5 = Mathf.Max(30f, num * (num / contentHeight));
				float num6 = startY + scrollY / num4 * (num - num5);
				GUI.DrawTexture(new Rect(num2, num6, num3, num5), ModUI._scrollbarThumbTex);
				ModUI.HandleScrollInput(ref scrollY, scrollId, num4, num5, num2, num3, startY, num, regionX, regionWidth);
			}
			else
			{
				scrollY = 0f;
			}
			GUI.BeginGroup(new Rect(regionX, startY, regionWidth, num));
			return -scrollY;
		}

		// Token: 0x0600007F RID: 127 RVA: 0x00005E40 File Offset: 0x00004040
		private static void EndScrollRegion()
		{
			GUI.EndGroup();
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00005E48 File Offset: 0x00004048
		private static void HandleScrollInput(ref float scrollY, int scrollId, float maxScroll, float thumbH, float scrollbarX, float scrollbarW, float startY, float viewportHeight, float regionX, float regionWidth)
		{
			Event current = Event.current;
			if (current.type == (EventType)6 && new Rect(regionX, startY, regionWidth + scrollbarW, viewportHeight).Contains(current.mousePosition))
			{
				scrollY += current.delta.y * 30f;
				scrollY = Mathf.Clamp(scrollY, 0f, maxScroll);
				current.Use();
			}
			Rect rect = new Rect(scrollbarX, startY, scrollbarW, viewportHeight);
			if (current.button == 0 && rect.Contains(current.mousePosition))
			{
				ModUI._dragScrollId = scrollId;
				float num = Mathf.Clamp01((current.mousePosition.y - startY - thumbH / 2f) / (viewportHeight - thumbH));
				scrollY = num * maxScroll;
				current.Use();
			}
			if (ModUI._dragScrollId == scrollId && current.type == (EventType)3 && current.button == 0)
			{
				float num2 = Mathf.Clamp01((current.mousePosition.y - startY - thumbH / 2f) / (viewportHeight - thumbH));
				scrollY = num2 * maxScroll;
				current.Use();
			}
			if (ModUI._dragScrollId == scrollId && current.type == (EventType)1 && current.button == 0)
			{
				ModUI._dragScrollId = 0;
			}
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00005F78 File Offset: 0x00004178
		private static void DrawCheatsTab(float startY)
		{
			float contentHeight = ModUI.CalculateCheatsContentHeight();
			float num = ModUI.BeginScrollRegion(ref ModUI._tabScrollY[0], startY, contentHeight, 20f, 820f);
			ModUI.SectionHeaderManual(ref num, "CHEATS");
			Cheats.BaseSpeedOverride = GUI.Toggle(new Rect(0f, num, 300f, 30f), Cheats.BaseSpeedOverride, "Base Speed Override");
			num += 40f;
			if (Cheats.BaseSpeedOverride)
			{
				GUI.Label(new Rect(0f, num, 150f, 30f), "Base Speed:");
				Cheats.BaseSpeed = GUI.HorizontalSlider(new Rect(160f, num + 10f, 250f, 20f), Cheats.BaseSpeed, 1f, 30f);
				Rect rect = new Rect(420f, num, 100f, 30f);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
				defaultInterpolatedStringHandler.AppendFormatted<float>(Cheats.BaseSpeed, "F1");
				GUI.Label(rect, defaultInterpolatedStringHandler.ToStringAndClear());
				num += 40f;
			}
			if (GUI.Button(new Rect(0f, num, 250f, 35f), "Unlock All Waypoints"))
			{
				Cheats.UnlockAllWaypoints();
			}
			num += 40f;
			if (GUI.Button(new Rect(0f, num, 250f, 35f), "Warp Home"))
			{
				Cheats.WarpHome();
			}
			num += 55f;
			ModUI.SectionHeaderManual(ref num, "CONFIGURATION");
			string characterName = ConfigService.CharacterName;
			GUI.Label(new Rect(0f, num, 120f, 30f), "Character:");
			GUI.Label(new Rect(125f, num, 250f, 30f), string.IsNullOrEmpty(characterName) ? "<none>" : characterName);
			if (GUI.Button(new Rect(400f, num, 120f, 30f), "Detect"))
			{
				ConfigService.DetectCharacterName();
			}
			num += 40f;
			if (GUI.Button(new Rect(0f, num, 180f, 35f), "Save Config"))
			{
				ConfigService.Save();
			}
			num += 40f;
			List<string> savedCharacters = ConfigService.GetSavedCharacters();
			if (savedCharacters.Count > 0)
			{
				GUI.Label(new Rect(0f, num, 200f, 30f), "Load Character:");
				num += 35f;
				using (List<string>.Enumerator enumerator = savedCharacters.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string text = enumerator.Current;
						if (GUI.Button(new Rect(0f, num, 350f, 30f), text))
						{
							ConfigService.CharacterName = text;
							ConfigService.Load();
						}
						num += 35f;
					}
					goto IL_2D1;
				}
			}
			GUI.Label(new Rect(0f, num, 400f, 30f), "No saved configs yet.");
			num += 35f;
			IL_2D1:
			if (!string.IsNullOrEmpty(ConfigService.StatusMessage))
			{
				GUI.Label(new Rect(0f, num, 500f, 30f), ConfigService.StatusMessage);
			}
			num += 40f;
			ModUI.SectionHeaderManual(ref num, "AUTO-LOGIN");
			BotConfig config = BotController.Config;
			GUI.Label(new Rect(0f, num, 80f, 30f), "Server:");
			string text2 = string.IsNullOrEmpty(config.AutoLoginServerName) ? "(select server)" : config.AutoLoginServerName;
			if (GUI.Button(new Rect(80f, num, 200f, 30f), text2))
			{
				ModUI._showServerDropdown = !ModUI._showServerDropdown;
			}
			num += 35f;
			if (ModUI._showServerDropdown)
			{
				foreach (string text3 in AutoLoginService.GetServerNames())
				{
					string text4 = string.Equals(text3, config.AutoLoginServerName, StringComparison.OrdinalIgnoreCase) ? ("▸ " + text3) : ("   " + text3);
					if (GUI.Button(new Rect(20f, num, 260f, 25f), text4))
					{
						config.AutoLoginServerName = text3;
						ModUI._showServerDropdown = false;
					}
					num += 28f;
				}
			}
			bool enableAutoLogin = config.EnableAutoLogin;
			config.EnableAutoLogin = GUI.Toggle(new Rect(0f, num, 300f, 30f), config.EnableAutoLogin, "Enable Auto-Login");
			num += 35f;
			if (config.EnableAutoLogin && !enableAutoLogin)
			{
				UILogin uilogin = Object.FindObjectOfType<UILogin>();
				if (uilogin != null && uilogin.gameObject.activeInHierarchy)
				{
					AutoLoginService.ServerAutoConnect();
				}
			}
			if (!string.IsNullOrEmpty(AutoLoginService.StatusMessage))
			{
				GUI.Label(new Rect(0f, num, 500f, 30f), "Status: " + AutoLoginService.StatusMessage);
				num += 30f;
			}
			ModUI.EndScrollRegion();
		}

		// Token: 0x06000082 RID: 130 RVA: 0x0000645C File Offset: 0x0000465C
		private static float CalculateCheatsContentHeight()
		{
			float num = 0f;
			num += 45f;
			num += 40f;
			if (Cheats.BaseSpeedOverride)
			{
				num += 40f;
			}
			num += 40f;
			num += 55f;
			num += 45f;
			num += 40f;
			num += 40f;
			List<string> savedCharacters = ConfigService.GetSavedCharacters();
			if (savedCharacters.Count > 0)
			{
				num += 35f;
				num += (float)(savedCharacters.Count * 35);
			}
			else
			{
				num += 35f;
			}
			num += 40f;
			num += 45f;
			num += 35f;
			if (ModUI._showServerDropdown)
			{
				num += (float)(AutoLoginService.GetServerNames().Length * 28);
			}
			num += 35f;
			if (!string.IsNullOrEmpty(AutoLoginService.StatusMessage))
			{
				num += 30f;
			}
			return num;
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00006530 File Offset: 0x00004730
		private static void DrawTeleporterTab(float startY)
		{
			float contentHeight = ModUI.CalculateTeleporterContentHeight();
			float num = ModUI.BeginScrollRegion(ref ModUI._tabScrollY[1], startY, contentHeight, 20f, 820f);
			ModUI.SectionHeaderManual(ref num, "TELEPORTER");
			Il2CppSystem.Collections.Generic.List<string> mapDisplayNames = TeleporterService.GetMapDisplayNames();
			TeleporterService.GetMapNames();
			if (mapDisplayNames == null || mapDisplayNames.Count == 0)
			{
				GUI.Label(new Rect(0f, num, 500f, 35f), "No maps available (App.Game.Map.MapDictionary not ready)");
				ModUI.EndScrollRegion();
				return;
			}
			if (ModUI._selectedMapIndex < 0 || ModUI._selectedMapIndex >= mapDisplayNames.Count)
			{
				ModUI._selectedMapIndex = 0;
			}
			GUI.Label(new Rect(0f, num, 150f, 35f), "Selected Map:");
			num += 40f;
			string str = mapDisplayNames[ModUI._selectedMapIndex];
			if (GUI.Button(new Rect(0f, num, 400f, 35f), str + " ▼"))
			{
				ModUI._dropdownOpen = !ModUI._dropdownOpen;
			}
			num += 40f;
			if (ModUI._dropdownOpen)
			{
				float num2 = 0f;
				float num3 = num;
				float num4 = 400f;
				float num5 = (float)mapDisplayNames.Count * 35f;
				float num6 = Mathf.Min(num5, 400f);
				GUI.Box(new Rect(num2, num3, num4, num6), "");
				float num7 = num2 + num4 - 20f;
				float num8 = num4 - 20f - 5f;
				Event current = Event.current;
				if (current.type == (EventType)6)
				{
					Rect rect = new(num2, num3, num4, num6);
					if (rect.Contains(current.mousePosition))
					{
						ModUI._dropdownScrollY += current.delta.y * 10f;
						ModUI._dropdownScrollY = Mathf.Clamp(ModUI._dropdownScrollY, 0f, Mathf.Max(0f, num5 - num6));
						current.Use();
					}
				}
				if (num5 > num6)
				{
					GUI.DrawTexture(new Rect(num7, num3, 20f, num6), ModUI._scrollbarBgTex);
					float num9 = Mathf.Max(30f, num6 * (num6 / num5));
					float num10 = num5 - num6;
					float num11 = num3 + ModUI._dropdownScrollY / num10 * (num6 - num9);
					GUI.DrawTexture(new Rect(num7, num11, 20f, num9), ModUI._scrollbarThumbTex);
					if (current.type == (EventType)3 && current.button == 0)
					{
						Rect rect2 = new(num7, num3, 20f, num6);
						if (rect2.Contains(current.mousePosition))
						{
							ModUI._dropdownScrollY = Mathf.Clamp((current.mousePosition.y - num3 - num9 / 2f) / (num6 - num9) * num10, 0f, num10);
							current.Use();
						}
					}
				}
				float num12 = num3 - ModUI._dropdownScrollY;
				for (int i = 0; i < mapDisplayNames.Count; i++)
				{
					float num13 = num12 + (float)i * 35f;
					if (num13 + 35f >= num3 && num13 < num3 + num6 && GUI.Button(new Rect(num2, num13, num8, 35f), mapDisplayNames[i]))
					{
						ModUI._selectedMapIndex = i;
						ModUI._dropdownOpen = false;
						ModUI._dropdownScrollY = 0f;
					}
				}
				num += num6 + 10f;
			}
			if (GUI.Button(new Rect(0f, num, 200f, 35f), "Teleport"))
			{
				string mapKeyByIndex = TeleporterService.GetMapKeyByIndex(ModUI._selectedMapIndex);
				if (mapKeyByIndex != null)
				{
					TeleporterService.WarpToMap(mapKeyByIndex);
				}
			}
			if (GUI.Button(new Rect(210f, num, 200f, 35f), "Set Farming Map"))
			{
				BotConfig config = BotController.Config;
				config.FarmingMapIndex = ModUI._selectedMapIndex;
				string mapKeyByIndex2 = TeleporterService.GetMapKeyByIndex(ModUI._selectedMapIndex);
				config.FarmingMapName = (mapKeyByIndex2 ?? mapDisplayNames[ModUI._selectedMapIndex]);
				MelonLogger.Msg("[AutoRevive] Farming map set to '" + mapDisplayNames[ModUI._selectedMapIndex] + "'");
			}
			num += 45f;
			ModUI.SectionHeaderManual(ref num, "AUTO-REVIVE");
			BotConfig config2 = BotController.Config;
			config2.EnableAutoRevive = GUI.Toggle(new Rect(0f, num, 300f, 30f), config2.EnableAutoRevive, "Enable Auto-Revive");
			num += 35f;
			string str2 = "(none)";
			if (!string.IsNullOrEmpty(config2.FarmingMapName) && config2.FarmingMapIndex >= 0 && config2.FarmingMapIndex < mapDisplayNames.Count)
			{
				str2 = mapDisplayNames[config2.FarmingMapIndex];
			}
			GUI.Label(new Rect(0f, num, 500f, 30f), "Farming Map: " + str2);
			num += 35f;
			ModUI.LabeledSliderManual(ref num, "Revive Delay", ref config2.ReviveDelay, 1f, 10f, "s");
			GUI.Label(new Rect(20f, num, 600f, 25f), "Increase if the bot gets stuck in a teleport loop after reviving.");
			num += 30f;
			ModUI.EndScrollRegion();
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00006A24 File Offset: 0x00004C24
		private static float CalculateTeleporterContentHeight()
		{
			float num = 0f;
			num += 45f;
			Il2CppSystem.Collections.Generic.List<string> mapDisplayNames = TeleporterService.GetMapDisplayNames();
			if (mapDisplayNames == null || mapDisplayNames.Count == 0)
			{
				return num + 35f;
			}
			num += 40f;
			num += 40f;
			if (ModUI._dropdownOpen)
			{
				float num2 = (float)mapDisplayNames.Count * 35f;
				num += Mathf.Min(num2, 400f) + 10f;
			}
			num += 45f;
			num += 45f;
			num += 35f;
			num += 35f;
			num += 35f;
			return num + 30f;
		}

		// Token: 0x06000085 RID: 133 RVA: 0x00006AC4 File Offset: 0x00004CC4
		private static void DrawBotTabManual(float startY)
		{
			BotConfig config = BotController.Config;
			BotStatus status = BotController.Status;
			bool isEnabled = BotController.IsEnabled;
			Color backgroundColor = GUI.backgroundColor;
			GUI.backgroundColor = (isEnabled ? new Color(0.8f, 0.2f, 0.2f) : new Color(0.2f, 0.7f, 0.2f));
			if (GUI.Button(new Rect(20f, startY, 200f, 40f), isEnabled ? "Disable Bot" : "Enable Bot"))
			{
				BotController.Toggle();
			}
			GUI.backgroundColor = backgroundColor;
			float startY2 = startY + 50f;
			float contentHeight = ModUI.CalculateBotContentHeight(status);
			float num = ModUI.BeginScrollRegion(ref ModUI._tabScrollY[2], startY2, contentHeight, 20f, 820f);
			ModUI.SectionHeaderManual(ref num, "STATUS");
			GUI.Label(new Rect(0f, num, 120f, 30f), "State:");
			GUI.Label(new Rect(130f, num, 300f, 30f), ModUI.StateString(status.State));
			num += 35f;
			if (status.State != BotState.Disabled)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
				if (status.PlayerMaxHp > 0)
				{
					float pct = (float)status.PlayerHealth / (float)status.PlayerMaxHp;
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(7, 3);
					defaultInterpolatedStringHandler.AppendFormatted<int>(status.PlayerHealth);
					defaultInterpolatedStringHandler.AppendLiteral(" / ");
					defaultInterpolatedStringHandler.AppendFormatted<int>(status.PlayerMaxHp);
					defaultInterpolatedStringHandler.AppendLiteral(" (");
					defaultInterpolatedStringHandler.AppendFormatted<float>(status.PlayerHPNorm * 100f, "F1");
					defaultInterpolatedStringHandler.AppendLiteral("%)");
					string overlay = defaultInterpolatedStringHandler.ToStringAndClear();
					ModUI.ProgressBarManual(ref num, "Player HP", pct, overlay, ModUI._barHpTex);
				}
				GUI.Label(new Rect(0f, num, 120f, 30f), "Target:");
				GUI.Label(new Rect(130f, num, 500f, 30f), string.IsNullOrEmpty(status.TargetName) ? "<none>" : status.TargetName);
				num += 35f;
				if (status.TargetMaxHp > 0)
				{
					float pct2 = (float)status.TargetHealth / (float)status.TargetMaxHp;
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 2);
					defaultInterpolatedStringHandler.AppendFormatted<int>(status.TargetHealth);
					defaultInterpolatedStringHandler.AppendLiteral(" / ");
					defaultInterpolatedStringHandler.AppendFormatted<int>(status.TargetMaxHp);
					string overlay2 = defaultInterpolatedStringHandler.ToStringAndClear();
					ModUI.ProgressBarManual(ref num, "Target HP", pct2, overlay2, ModUI._barTargetTex);
				}
				Rect rect = new Rect(0f, num, 800f, 30f);
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(30, 3);
				defaultInterpolatedStringHandler.AppendLiteral("Action: ");
				defaultInterpolatedStringHandler.AppendFormatted<float>(status.ActionTimer, "F2");
				defaultInterpolatedStringHandler.AppendLiteral("s  Attack: ");
				defaultInterpolatedStringHandler.AppendFormatted<float>(status.AttackTimer, "F2");
				defaultInterpolatedStringHandler.AppendLiteral("s  Skill: ");
				defaultInterpolatedStringHandler.AppendFormatted<float>(status.SkillTimer, "F2");
				defaultInterpolatedStringHandler.AppendLiteral("s");
				GUI.Label(rect, defaultInterpolatedStringHandler.ToStringAndClear());
				num += 35f;
				Rect rect2 = new Rect(0f, num, 800f, 30f);
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(50, 5);
				defaultInterpolatedStringHandler.AppendLiteral("Kills: ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(status.KillCount);
				defaultInterpolatedStringHandler.AppendLiteral("  Looted: ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(status.LootCount);
				defaultInterpolatedStringHandler.AppendLiteral("  Skills Cast: ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(status.SkillsCastCount);
				defaultInterpolatedStringHandler.AppendLiteral("  Stored: ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(status.StoreCount);
				defaultInterpolatedStringHandler.AppendLiteral("  Sold: ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(status.SellCount);
				GUI.Label(rect2, defaultInterpolatedStringHandler.ToStringAndClear());
				num += 35f;
				Rect rect3 = new Rect(0f, num, 820f, 30f);
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(35, 8);
				defaultInterpolatedStringHandler.AppendLiteral("Atk req ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(status.AttackRequestsSent);
				defaultInterpolatedStringHandler.AppendLiteral("/");
				defaultInterpolatedStringHandler.AppendFormatted<int>(status.AttackAttempts);
				defaultInterpolatedStringHandler.AppendLiteral(" (");
				defaultInterpolatedStringHandler.AppendFormatted<float>(status.AttackRequestsPerSec, "F1");
				defaultInterpolatedStringHandler.AppendLiteral("/");
				defaultInterpolatedStringHandler.AppendFormatted<float>(status.AttackAttemptsPerSec, "F1");
				defaultInterpolatedStringHandler.AppendLiteral(" s)   ");
				defaultInterpolatedStringHandler.AppendLiteral("Skill req ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(status.SkillRequestsSent);
				defaultInterpolatedStringHandler.AppendLiteral("/");
				defaultInterpolatedStringHandler.AppendFormatted<int>(status.SkillAttempts);
				defaultInterpolatedStringHandler.AppendLiteral(" (");
				defaultInterpolatedStringHandler.AppendFormatted<float>(status.SkillRequestsPerSec, "F1");
				defaultInterpolatedStringHandler.AppendLiteral("/");
				defaultInterpolatedStringHandler.AppendFormatted<float>(status.SkillAttemptsPerSec, "F1");
				defaultInterpolatedStringHandler.AppendLiteral(" s)");
				GUI.Label(rect3, defaultInterpolatedStringHandler.ToStringAndClear());
				num += 40f;
			}
			else
			{
				num += 15f;
			}
			ModUI.SectionHeaderManual(ref num, "CONFIGURATION");
			config.EnableAfkLeech = GUI.Toggle(new Rect(20f, num, 300f, 30f), config.EnableAfkLeech, "AFK Mode (no combat but still revive)");
			num += 35f;
			ModUI.LabeledSliderManual(ref num, "Action Interval", ref config.ActionInterval, 0.3f, 5f, "s");
			ModUI.LabeledSliderManual(ref num, "Attack Interval", ref config.AttackInterval, 0.5f, 5f, "s");
			ModUI.LabeledSliderManual(ref num, "Search Range", ref config.SearchRange, 0f, 200f, "");
			num += 15f;
			ModUI.SectionHeaderManual(ref num, "LOOTING");
			config.EnableLooting = GUI.Toggle(new Rect(20f, num, 300f, 30f), config.EnableLooting, "Enable Looting");
			num += 35f;
			ModUI.LabeledSliderManual(ref num, "Loot Delay", ref config.LootDelay, 0.1f, 3f, "s");
			ModUI.LabeledSliderManual(ref num, "Loot Range", ref config.LootRange, 5f, 50f, "");
			num += 15f;
			ModUI.SectionHeaderManual(ref num, "AUTO-STORE");
			config.EnableAutoStore = GUI.Toggle(new Rect(0f, num, 300f, 30f), config.EnableAutoStore, "Enable Auto-Store");
			num += 35f;
			if (config.EnableAutoStore && status.State != BotState.Disabled)
			{
				Rect rect4 = new Rect(0f, num, 600f, 30f);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(42, 3);
				defaultInterpolatedStringHandler.AppendLiteral("Stacks Stored: ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(status.StoreCount);
				defaultInterpolatedStringHandler.AppendLiteral("    Cooldown: ");
				defaultInterpolatedStringHandler.AppendFormatted<float>(status.StoreCooldown, "F1");
				defaultInterpolatedStringHandler.AppendLiteral("s    Timer: ");
				defaultInterpolatedStringHandler.AppendFormatted<float>(status.StorePeriodicTimer, "F0");
				defaultInterpolatedStringHandler.AppendLiteral("s");
				GUI.Label(rect4, defaultInterpolatedStringHandler.ToStringAndClear());
				num += 35f;
			}
			if (GUI.Button(new Rect(0f, num, 200f, 30f), "Store Now"))
			{
				int num2 = StorageService.StoreFilteredItems();
				status.StoreCount += num2;
			}
			if (!string.IsNullOrEmpty(StorageService.StatusMessage))
			{
				GUI.Label(new Rect(210f, num, 400f, 30f), StorageService.StatusMessage);
			}
			num += 40f;
			ModUI.SectionHeaderManual(ref num, "AUTO-SELL");
			config.EnableAutoSell = GUI.Toggle(new Rect(0f, num, 300f, 30f), config.EnableAutoSell, "Enable Auto-Sell");
			num += 35f;
			if (config.EnableAutoSell && status.State != BotState.Disabled)
			{
				Rect rect5 = new Rect(0f, num, 600f, 30f);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(40, 3);
				defaultInterpolatedStringHandler.AppendLiteral("Stacks Sold: ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(status.SellCount);
				defaultInterpolatedStringHandler.AppendLiteral("    Cooldown: ");
				defaultInterpolatedStringHandler.AppendFormatted<float>(status.SellCooldown, "F1");
				defaultInterpolatedStringHandler.AppendLiteral("s    Timer: ");
				defaultInterpolatedStringHandler.AppendFormatted<float>(status.SellPeriodicTimer, "F0");
				defaultInterpolatedStringHandler.AppendLiteral("s");
				GUI.Label(rect5, defaultInterpolatedStringHandler.ToStringAndClear());
				num += 35f;
			}
			if (GUI.Button(new Rect(0f, num, 200f, 30f), "Sell Now"))
			{
				int num3 = MerchantService.SellFilteredItems();
				status.SellCount += num3;
			}
			if (!string.IsNullOrEmpty(MerchantService.StatusMessage))
			{
				GUI.Label(new Rect(210f, num, 400f, 30f), MerchantService.StatusMessage);
			}
			num += 40f;
			ModUI.SectionHeaderManual(ref num, "SKILLS");
			config.EnableSkills = GUI.Toggle(new Rect(0f, num, 300f, 30f), config.EnableSkills, "Enable Auto-Skills");
			num += 35f;
			ModUI.LabeledSliderManual(ref num, "Skill Interval", ref config.SkillInterval, 0.1f, 5f, "s");
			GUI.Label(new Rect(0f, num, 600f, 30f), "→ Configure skills in the Skill Config tab");
			num += 35f;
			ModUI.SectionHeaderManual(ref num, "BOT AI");
			GUI.Label(new Rect(0f, num, 140f, 25f), "Target Priority:");
			if (GUI.Button(new Rect(150f, num, 140f, 25f), IdleState.TargetPriority.ToString()))
			{
				IdleState.TargetPriority = (TargetPriorityMode)((int)(IdleState.TargetPriority + 1) % 4);
            }
			num += 30f;
			GUI.Label(new Rect(0f, num, 780f, 25f), "Modes: Nearest → LowestHpPercent → LowestHp → Hybrid");
			num += 35f;
			ModUI.EndScrollRegion();
		}

		// Token: 0x06000086 RID: 134 RVA: 0x000074F4 File Offset: 0x000056F4
		private static void DrawSkillConfigTab(float startY)
		{
			BotConfig config = BotController.Config;
			SkillSlotInfo[] skillSlotInfos = CombatService.GetSkillSlotInfos(GameStateService.Player);
			if (GUI.Button(new Rect(720f, startY, 130f, 22f), "⟳ Refresh Skills"))
			{
				CombatService.ForceRefreshSkillInfos();
			}
			if (GUI.Button(new Rect(570f, startY, 140f, 22f), "\ud83d\udccb Dump CSV"))
			{
				ModUI._lastDumpResult = SkillDumpService.DumpEquippedSkills();
			}
			if (!string.IsNullOrEmpty(ModUI._lastDumpResult))
			{
				GUI.Label(new Rect(20f, startY, 540f, 22f), ModUI._lastDumpResult);
			}
			startY += 26f;
			int num = 0;
			int num2 = 0;
			config.EnsureArrays();
			for (int i = 0; i < 20; i++)
			{
				if (skillSlotInfos[i].Assigned)
				{
					num++;
					if (skillSlotInfos[i].IsBuff || skillSlotInfos[i].IsBond || config.TreatAsBuff[i])
					{
						num2++;
					}
				}
			}
			float num3 = 75f + (float)num * 35f;
			num3 += 110f;
			if (num2 == 0)
			{
				num3 += 30f;
			}
			else
			{
				num3 += (float)(33 + num2 * 35);
			}
			num3 += 40f;
			float num4 = ModUI.BeginScrollRegion(ref ModUI._tabScrollY[3], startY, num3, 20f, 820f);
			GUI.Label(new Rect(0f, num4, 600f, 30f), "Skill Slots  (toggle, priority, healing)");
			num4 += 40f;
			GUI.Label(new Rect(0f, num4, 350f, 25f), "Slot / Skill", ModUI._headerStyle);
			GUI.Label(new Rect(360f, num4, 100f, 25f), "Priority", ModUI._headerStyle);
			GUI.Label(new Rect(580f, num4, 80f, 25f), "Heal", ModUI._headerStyle);
			GUI.Label(new Rect(665f, num4, 150f, 25f), "Heal Threshold", ModUI._headerStyle);
			num4 += 30f;
			GUI.DrawTexture(new Rect(0f, num4, 810f, 1f), ModUI._barBgTex);
			num4 += 5f;

            string[] listBuffs = ["Haste", "Benediction", "Divine Grace"];
            for (int j = 0; j < 20; j++)
			{
				SkillSlotInfo skillSlotInfo = skillSlotInfos[j];
				if (skillSlotInfo.Assigned)
				{
					string text = "";
					if (skillSlotInfo.IsSummon)
					{
						text += "[SUM]";
					}
					if (skillSlotInfo.IsBuff || listBuffs.Contains(skillSlotInfo.Name))
					{
						text += "[BUF]";
					}
					if (skillSlotInfo.IsBond)
					{
						text += "[BND]";
					}
					if (skillSlotInfo.IsMount)
					{
						text += "[MNT]";
					}
					if (skillSlotInfo.IsHealing || config.TreatAsHealing[j])
					{
						text += "[HEAL]";
					}
					if (skillSlotInfo.IsOnCooldown)
					{
						text += "[CD]";
					}
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(8, 4);
					defaultInterpolatedStringHandler.AppendLiteral("[");
					defaultInterpolatedStringHandler.AppendFormatted<int>(j);
					defaultInterpolatedStringHandler.AppendLiteral("] ");
					defaultInterpolatedStringHandler.AppendFormatted(skillSlotInfo.Name);
					defaultInterpolatedStringHandler.AppendLiteral(" (");
					defaultInterpolatedStringHandler.AppendFormatted<int>(skillSlotInfo.ManaCost);
					defaultInterpolatedStringHandler.AppendLiteral("MP)");
					defaultInterpolatedStringHandler.AppendFormatted(text);
					string text2 = defaultInterpolatedStringHandler.ToStringAndClear();
					config.EnabledSkillSlots[j] = GUI.Toggle(new Rect(0f, num4, 350f, 30f), config.EnabledSkillSlots[j], text2);
					GUI.Label(new Rect(360f, num4, 50f, 30f), "Prio:");
					if (config.SkillPriorities[j] <= 0)
					{
						config.SkillPriorities[j] = 50;
					}
					config.SkillPriorities[j] = (int)GUI.HorizontalSlider(new Rect(415f, num4 + 10f, 100f, 20f), (float)config.SkillPriorities[j], 1f, 100f);
					Rect rect = new Rect(525f, num4, 50f, 30f);
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
					defaultInterpolatedStringHandler.AppendFormatted<int>(config.SkillPriorities[j]);
					GUI.Label(rect, defaultInterpolatedStringHandler.ToStringAndClear());
					if (skillSlotInfo.IsHealing)
					{
						GUI.enabled = false;
						GUI.Toggle(new Rect(580f, num4, 80f, 30f), true, "Heal");
						GUI.enabled = true;
					}
					else
					{
						config.TreatAsHealing[j] = GUI.Toggle(new Rect(580f, num4, 80f, 30f), config.TreatAsHealing[j], "Heal");
					}
					if (skillSlotInfo.IsHealing || config.TreatAsHealing[j])
					{
						if (config.HealingThresholds[j] <= 0)
						{
							config.HealingThresholds[j] = 90;
						}
						GUI.Label(new Rect(665f, num4, 50f, 30f), "HP<");
						config.HealingThresholds[j] = (int)GUI.HorizontalSlider(new Rect(720f, num4 + 10f, 80f, 20f), (float)config.HealingThresholds[j], 10f, 100f);
						Rect rect2 = new Rect(810f, num4, 60f, 30f);
						defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 1);
						defaultInterpolatedStringHandler.AppendFormatted<int>(config.HealingThresholds[j]);
						defaultInterpolatedStringHandler.AppendLiteral("%");
						GUI.Label(rect2, defaultInterpolatedStringHandler.ToStringAndClear());
					}
					num4 += 35f;
				}
			}
			ModUI.SectionHeaderManual(ref num4, "BUFF MAINTENANCE");
			config.EnableBuffMaintenance = GUI.Toggle(new Rect(0f, num4, 350f, 30f), config.EnableBuffMaintenance, "Enable Auto Buff Maintenance");
			num4 += 35f;
			GUI.Label(new Rect(0f, num4, 780f, 25f), "Buffs will not recast while active. If remaining duration is readable, they refresh near expiry.");
			num4 += 30f;
			SkillSlotInfo[] array = skillSlotInfos;
			if (num2 == 0)
			{
				GUI.Label(new Rect(0f, num4, 780f, 25f), "No buff/bond skills detected in assigned slots.");
				num4 += 30f;
			}
			else
			{
				GUI.Label(new Rect(0f, num4, 420f, 25f), "Slot / Buff Skill", ModUI._headerStyle);
				GUI.Label(new Rect(430f, num4, 80f, 25f), "Auto", ModUI._headerStyle);
				GUI.Label(new Rect(520f, num4, 220f, 25f), "Refresh Before End", ModUI._headerStyle);
				GUI.Label(new Rect(745f, num4, 70f, 25f), "State", ModUI._headerStyle);
				num4 += 28f;
				GUI.DrawTexture(new Rect(0f, num4, 810f, 1f), ModUI._barBgTex);
				num4 += 5f;
				for (int k = 0; k < 20; k++)
				{
					SkillSlotInfo skillSlotInfo2 = array[k];
					if (skillSlotInfo2.Assigned && (skillSlotInfo2.IsBuff || listBuffs.Contains(skillSlotInfo2.Name) || skillSlotInfo2.IsBond || config.TreatAsBuff[k]))
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 2);
						defaultInterpolatedStringHandler.AppendLiteral("[");
						defaultInterpolatedStringHandler.AppendFormatted<int>(k);
						defaultInterpolatedStringHandler.AppendLiteral("] ");
						defaultInterpolatedStringHandler.AppendFormatted(skillSlotInfo2.Name);
						string text3 = defaultInterpolatedStringHandler.ToStringAndClear();
						if (config.TreatAsBuff[k] && !skillSlotInfo2.IsBuff && !skillSlotInfo2.IsBond)
						{
							text3 += " [manual]";
						}
						config.EnabledBuffSlots[k] = GUI.Toggle(new Rect(0f, num4, 420f, 30f), config.EnabledBuffSlots[k], text3);
						GUI.Label(new Rect(430f, num4, 40f, 25f), config.EnabledBuffSlots[k] ? "ON" : "OFF");
						config.BuffRefreshLeadSeconds[k] = GUI.HorizontalSlider(new Rect(520f, num4 + 10f, 160f, 20f), config.BuffRefreshLeadSeconds[k], 0.5f, 10f);
						Rect rect3 = new Rect(685f, num4, 55f, 25f);
						defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 1);
						defaultInterpolatedStringHandler.AppendFormatted<float>(config.BuffRefreshLeadSeconds[k], "F1");
						defaultInterpolatedStringHandler.AppendLiteral("s");
						GUI.Label(rect3, defaultInterpolatedStringHandler.ToStringAndClear());
						float num5;
						if (CombatService.TryGetBuffRemainingSecondsForSlot(GameStateService.Player, k, out num5))
						{
							Rect rect4 = new Rect(745f, num4, 70f, 25f);
							string text4;
							if (num5 < 0f)
							{
								text4 = "Active";
							}
							else
							{
								defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 1);
								defaultInterpolatedStringHandler.AppendFormatted<float>(num5, "F1");
								defaultInterpolatedStringHandler.AppendLiteral("s");
								text4 = defaultInterpolatedStringHandler.ToStringAndClear();
							}
							GUI.Label(rect4, text4);
						}
						else
						{
							GUI.Label(new Rect(745f, num4, 70f, 25f), "Missing");
						}
						num4 += 35f;
					}
				}
			}
			ModUI.EndScrollRegion();
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00007E34 File Offset: 0x00006034
		private static void DrawLootFilterTab(float startY)
		{
			float contentHeight = ModUI.CalculateLootFilterContentHeight();
			float num = ModUI.BeginScrollRegion(ref ModUI._tabScrollY[4], startY, contentHeight, 20f, 820f);
			string[] array = new string[]
			{
				"Common",
				"Rare",
				"Unique",
				"Legendary"
			};
			Color[] array2 = new Color[]
			{
				new Color(0.7f, 0.7f, 0.7f),
				new Color(0.3f, 0.6f, 1f),
				new Color(0.8f, 0.4f, 1f),
				new Color(1f, 0.7f, 0.2f)
			};
			ModUI.SectionHeaderManual(ref num, "SELL FILTER");
			LootFilterService.SellFilterEnabled = GUI.Toggle(new Rect(0f, num, 300f, 30f), LootFilterService.SellFilterEnabled, "Enable Sell Filter");
			num += 40f;
			bool[] sellRarityEnabled = LootFilterService.SellRarityEnabled;
			for (int k = 0; k < 4; k++)
			{
				float num2 = (float)(k * 200);
				Color contentColor = GUI.contentColor;
				GUI.contentColor = array2[k];
				bool flag = sellRarityEnabled[k];
				sellRarityEnabled[k] = GUI.Toggle(new Rect(num2, num, 180f, 30f), sellRarityEnabled[k], "Sell " + array[k]);
				if (sellRarityEnabled[k] != flag)
				{
					LootFilterService.MarkDirty();
				}
				GUI.contentColor = contentColor;
			}
			num += 40f;
			int value = LootFilterService.SellOverrideCount(SellOverride.AlwaysSell);
			int value2 = LootFilterService.SellOverrideCount(SellOverride.NeverSell);
			Rect rect = new Rect(0f, num, 800f, 30f);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(29, 2);
			defaultInterpolatedStringHandler.AppendLiteral("Always Sell: ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(value);
			defaultInterpolatedStringHandler.AppendLiteral("    Never Sell: ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(value2);
			GUI.Label(rect, defaultInterpolatedStringHandler.ToStringAndClear());
			num += 40f;
			ModUI.SectionHeaderManual(ref num, "STAT FILTER");
			LootFilterService.StatFilterEnabled = GUI.Toggle(new Rect(0f, num, 350f, 30f), LootFilterService.StatFilterEnabled, "Enable Stat Filter (Equipment & Artifacts)");
			num += 40f;
			if (false) //(LootFilterService.StatFilterEnabled)
			{
				GUI.Label(new Rect(0f, num, 700f, 25f), "MAIN STAT  (any rule matches = keep)", ModUI._sectionStyle);
				num += 30f;
				Il2CppSystem.Collections.Generic.List<StatFilterRule> mainStats = new Il2CppSystem.Collections.Generic.List<StatFilterRule>();
				foreach(var rule in LootFilterService.MainStatRules)
                    mainStats.Add(rule);

                ModUI.DrawStatRuleGroup(ref num, "main", mainStats, LootFilterService.MainStatTypes, 3, delegate
				{
					LootFilterService.AddMainStatRule();
				}, delegate(int i)
				{
					LootFilterService.RemoveMainStatRule(i);
				});
				num += 10f;
				GUI.Label(new Rect(0f, num, 500f, 25f), "SECONDARY STATS  (at least N must match)", ModUI._sectionStyle);
				GUI.Label(new Rect(520f, num, 100f, 25f), "Min Match:");
				if (GUI.Button(new Rect(625f, num, 28f, 25f), "-"))
				{
					LootFilterService.SecondaryMinMatch--;
				}
				GUI.Label(new Rect(658f, num, 30f, 25f), LootFilterService.SecondaryMinMatch.ToString());
				if (GUI.Button(new Rect(693f, num, 28f, 25f), "+"))
				{
					LootFilterService.SecondaryMinMatch++;
				}
				num += 30f;
                Il2CppSystem.Collections.Generic.List<StatFilterRule> secStats = new Il2CppSystem.Collections.Generic.List<StatFilterRule>();
                foreach (var rule in LootFilterService.SecondaryStatRules)
                    secStats.Add(rule);
                ModUI.DrawStatRuleGroup(ref num, "sec", secStats, LootFilterService.SecondaryStatTypes, 200, delegate
				{
					LootFilterService.AddSecondaryStatRule();
				}, delegate(int i)
				{
					LootFilterService.RemoveSecondaryStatRule(i);
				});
				num += 10f;
				if (GUI.Button(new Rect(300f, num, 200f, 30f), "Clear All Rules"))
				{
					LootFilterService.ClearAllStatRules();
				}
				num += 40f;
			}
			if (GUI.Button(new Rect(0f, num, 140f, 30f), "Save Filter"))
			{
				LootFilterService.Save();
			}
			if (GUI.Button(new Rect(150f, num, 160f, 30f), "Reset Sell"))
			{
				LootFilterService.ResetAllSellOverrides();
			}
			if (!string.IsNullOrEmpty(LootFilterService.StatusMessage))
			{
				GUI.Label(new Rect(320f, num, 300f, 30f), LootFilterService.StatusMessage);
			}
			num += 40f;
			GUI.Label(new Rect(0f, num, 60f, 30f), "Search:");
			GUI.DrawTexture(new Rect(65f, num, 300f, 25f), ModUI._searchFocused ? ModUI._scrollbarThumbTex : ModUI._barBgTex);
			string text = (ModUI._filterSearch.Length > 0) ? ModUI._filterSearch : (ModUI._searchFocused ? "_" : "(click to type)");
			GUI.Label(new Rect(70f, num, 280f, 25f), text);
			if (GUI.Button(new Rect(65f, num, 300f, 25f), GUIContent.none, GUIStyle.none))
			{
				ModUI._searchFocused = !ModUI._searchFocused;
			}
			if (ModUI._filterSearch.Length > 0 && GUI.Button(new Rect(370f, num, 25f, 25f), "X"))
			{
				ModUI._filterSearch = "";
				ModUI._searchFocused = false;
			}
			if (ModUI._searchFocused && Event.current.type == (EventType)4)
			{
				KeyCode keyCode = Event.current.keyCode;
				if (keyCode == (KeyCode)8 && ModUI._filterSearch.Length > 0)
				{
					ModUI._filterSearch = ModUI._filterSearch.Substring(0, ModUI._filterSearch.Length - 1);
					Event.current.Use();
				}
				else if (keyCode == (KeyCode)27 || keyCode == (KeyCode)13)
				{
					ModUI._searchFocused = false;
					Event.current.Use();
				}
				else
				{
					char character = Event.current.character;
					if (character >= ' ' && character < '\u007f')
					{
						ModUI._filterSearch += character.ToString();
						Event.current.Use();
					}
				}
			}
			num += 35f;
			GUI.Label(new Rect(5f, num, 80f, 25f), "Sell", ModUI._headerStyle);
			GUI.Label(new Rect(90f, num, 350f, 25f), "Item Name", ModUI._headerStyle);
			GUI.Label(new Rect(445f, num, 60f, 25f), "Rarity", ModUI._headerStyle);
			GUI.Label(new Rect(515f, num, 80f, 25f), "Seen", ModUI._headerStyle);
			num += 28f;
			GUI.DrawTexture(new Rect(0f, num, 810f, 1f), ModUI._barBgTex);
			num += 5f;
			float num3 = num;
			float num4 = 400f;
			List<LootFilterEntry> list = new List<LootFilterEntry>();
			string text2 = (ModUI._filterSearch ?? "").Trim();
			foreach (System.Collections.Generic.KeyValuePair<string, LootFilterEntry> keyValuePair in LootFilterService.Items)
			{
				if (text2.Length <= 0 || keyValuePair.Key.IndexOf(text2, StringComparison.OrdinalIgnoreCase) >= 0)
				{
					list.Add(keyValuePair.Value);
				}
			}
			list.Sort(delegate(LootFilterEntry a, LootFilterEntry b)
			{
				int num12 = b.Rarity.CompareTo(a.Rarity);
				if (num12 == 0)
				{
					return string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase);
				}
				return num12;
			});
			float num5 = (float)list.Count * 30f;
			float num6 = Mathf.Max(0f, num5 - num4);
			ModUI._filterItemScrollY = Mathf.Clamp(ModUI._filterItemScrollY, 0f, num6);
			if (num5 > num4)
			{
				float num7 = 790f;
				float num8 = 20f;
				GUI.DrawTexture(new Rect(num7, num3, num8, num4), ModUI._scrollbarBgTex);
				float num9 = Mathf.Max(30f, num4 * (num4 / num5));
				float num10 = num3 + ModUI._filterItemScrollY / num6 * (num4 - num9);
				GUI.DrawTexture(new Rect(num7, num10, num8, num9), ModUI._scrollbarThumbTex);
				int scrollId = ModUI._nextScrollId++;
				ModUI.HandleScrollInput(ref ModUI._filterItemScrollY, scrollId, num6, num9, num7, num8, num3, num4, 0f, 810f);
			}
			else if (Event.current.type == (EventType)6 && new Rect(0f, num3, 810f, num4).Contains(Event.current.mousePosition))
			{
				Event.current.Use();
			}
			GUI.BeginGroup(new Rect(0f, num3, 810f, num4));
			for (int j = 0; j < list.Count; j++)
			{
				float num11 = (float)j * 30f - ModUI._filterItemScrollY;
				if (num11 + 30f >= 0f && num11 <= num4)
				{
					LootFilterEntry lootFilterEntry = list[j];
					int sellOverride = lootFilterEntry.SellOverride;
					string text3 = LootFilterService.SellOverrideLabel(sellOverride);
					Color color;
					if (sellOverride != 1)
					{
						if (sellOverride != 2)
						{
							color = new Color(0.5f, 0.5f, 0.5f);
						}
						else
						{
							color = new Color(0.2f, 0.5f, 0.8f);
						}
					}
					else
					{
						color = new Color(1f, 0.6f, 0.2f);
					}
					Color backgroundColor = color;
					Color backgroundColor2 = GUI.backgroundColor;
					GUI.backgroundColor = backgroundColor;
					if (GUI.Button(new Rect(5f, num11, 75f, 28f), text3))
					{
						LootFilterService.CycleSellOverride(lootFilterEntry.Name);
					}
					GUI.backgroundColor = backgroundColor2;
					GUI.Label(new Rect(90f, num11, 350f, 30f), lootFilterEntry.Name);
					int rarity = lootFilterEntry.Rarity;
					Color contentColor2 = GUI.contentColor;
					GUI.contentColor = ((rarity >= 0 && rarity < 4) ? array2[rarity] : Color.white);
					GUI.Label(new Rect(445f, num11, 60f, 30f), "[" + LootFilterService.RarityName(rarity) + "]");
					GUI.contentColor = contentColor2;
					Rect rect2 = new Rect(515f, num11, 100f, 30f);
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 1);
					defaultInterpolatedStringHandler.AppendLiteral("(");
					defaultInterpolatedStringHandler.AppendFormatted<int>(lootFilterEntry.TimesSeen);
					defaultInterpolatedStringHandler.AppendLiteral("x)");
					GUI.Label(rect2, defaultInterpolatedStringHandler.ToStringAndClear());
				}
			}
			GUI.EndGroup();
			ModUI.EndScrollRegion();
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00008910 File Offset: 0x00006B10
		private static float CalculateLootFilterContentHeight()
		{
			float num = 0f;
			num += 45f;
			num += 40f;
			num += 40f;
			num += 40f;
			num += 45f;
			num += 40f;
			if (false)// (LootFilterService.StatFilterEnabled)
			{
				num += 30f;
				num += (float)(LootFilterService.MainStatRules.Count * 35);
				num += 35f;
				num += 10f;
				num += 30f;
				num += (float)(LootFilterService.SecondaryStatRules.Count * 35);
				num += 35f;
				num += 10f;
				num += 40f;
			}
			num += 40f;
			num += 35f;
			num += 33f;
			return num + 400f;
		}

		// Token: 0x06000089 RID: 137 RVA: 0x000089D8 File Offset: 0x00006BD8
		private static void DrawStatRuleGroup(ref float y, string groupTag, Il2CppSystem.Collections.Generic.List<StatFilterRule> rules, string[] statTypes, int sliderMax, Action addRule, Action<int> removeRule)
		{
			int num = -1;
			for (int i = 0; i < rules.Count; i++)
			{
				StatFilterRule statFilterRule = rules[i];
				float num2 = 0f;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
				defaultInterpolatedStringHandler.AppendFormatted(groupTag);
				defaultInterpolatedStringHandler.AppendLiteral("_");
				defaultInterpolatedStringHandler.AppendFormatted<int>(i);
				string text = defaultInterpolatedStringHandler.ToStringAndClear();
				int num3 = LootFilterService.FindStatIndex(statTypes, statFilterRule.StatType);
				if (GUI.Button(new Rect(num2, y, 100f, 28f), "▼ " + statFilterRule.StatType))
				{
					if (ModUI._openDropdownTag == text)
					{
						ModUI._openDropdownTag = null;
						ModUI._openDropdownIdx = -1;
					}
					else
					{
						ModUI._openDropdownTag = text;
						ModUI._openDropdownIdx = i;
					}
				}
				num2 += 105f;
				GUI.Label(new Rect(num2, y, 25f, 28f), "≥");
				num2 += 28f;
				if (GUI.Button(new Rect(num2, y, 28f, 28f), "-"))
				{
					statFilterRule.Value = Math.Max(0, statFilterRule.Value - 1);
					LootFilterService.MarkDirty();
				}
				num2 += 30f;
				GUI.Label(new Rect(num2, y, 45f, 28f), statFilterRule.Value.ToString());
				num2 += 48f;
				if (GUI.Button(new Rect(num2, y, 28f, 28f), "+"))
				{
					statFilterRule.Value = Math.Min(sliderMax, statFilterRule.Value + 1);
					LootFilterService.MarkDirty();
				}
				num2 += 32f;
				int value = statFilterRule.Value;
				statFilterRule.Value = (int)GUI.HorizontalSlider(new Rect(num2, y + 10f, 250f, 20f), (float)statFilterRule.Value, 0f, (float)sliderMax);
				if (statFilterRule.Value != value)
				{
					LootFilterService.MarkDirty();
				}
				num2 += 260f;
				if (GUI.Button(new Rect(num2, y, 28f, 28f), "✕"))
				{
					num = i;
				}
				y += 35f;
				if (ModUI._openDropdownTag == text)
				{
					float num4 = 0f;
					float num5 = 120f;
					float num6 = 25f;
					float num7 = (float)statTypes.Length * num6;
					if (num7 > 300f)
					{
						num7 = 300f;
					}
					GUI.DrawTexture(new Rect(num4, y, num5, num7), ModUI._windowBgTex);
					GUI.Box(new Rect(num4, y, num5, num7), "");
					for (int j = 0; j < statTypes.Length; j++)
					{
						float num8 = y + (float)j * num6;
						if (num8 + num6 >= y && num8 <= y + num7)
						{
							string text2 = (j == num3) ? ("▸ " + statTypes[j]) : ("   " + statTypes[j]);
							if (GUI.Button(new Rect(num4, num8, num5, num6), text2))
							{
								statFilterRule.StatType = statTypes[j];
								ModUI._openDropdownTag = null;
								ModUI._openDropdownIdx = -1;
								LootFilterService.MarkDirty();
							}
						}
					}
					y += num7 + 5f;
				}
			}
			if (num >= 0)
			{
				removeRule(num);
				if (ModUI._openDropdownTag != null && ModUI._openDropdownTag.StartsWith(groupTag))
				{
					ModUI._openDropdownTag = null;
					ModUI._openDropdownIdx = -1;
				}
			}
			if (GUI.Button(new Rect(0f, y, 180f, 28f), "+ Add Rule"))
			{
				addRule();
			}
			y += 35f;
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00008D54 File Offset: 0x00006F54
		private static void SectionHeaderManual(ref float y, string text)
		{
			GUI.Label(new Rect(0f, y, 800f, 30f), text, ModUI._headerStyle);
			y += 35f;
			GUI.DrawTexture(new Rect(0f, y, 800f, 2f), ModUI._barBgTex);
			y += 10f;
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00008DB8 File Offset: 0x00006FB8
		private static void LabeledSliderManual(ref float y, string label, ref float value, float min, float max, string suffix)
		{
			GUI.Label(new Rect(0f, y, 180f, 30f), label);
			value = GUI.HorizontalSlider(new Rect(190f, y + 10f, 250f, 20f), value, min, max);
			Rect rect = new Rect(450f, y, 120f, 30f);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 2);
			defaultInterpolatedStringHandler.AppendFormatted<float>(value, "F1");
			defaultInterpolatedStringHandler.AppendFormatted(suffix);
			GUI.Label(rect, defaultInterpolatedStringHandler.ToStringAndClear());
			y += 35f;
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00008E58 File Offset: 0x00007058
		private static void ProgressBarManual(ref float y, string label, float pct, string overlay, Texture2D fgTex)
		{
			pct = Mathf.Clamp01(pct);
			GUI.Label(new Rect(0f, y, 120f, 30f), label);
			Rect rect = new(130f, y, 500f, 25f);
			GUI.DrawTexture(rect, ModUI._barBgTex);
			GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width * pct, rect.height), fgTex);
			TextAnchor alignment = GUI.skin.label.alignment;
			GUI.skin.label.alignment = (TextAnchor)4;
			GUI.Label(rect, overlay);
			GUI.skin.label.alignment = alignment;
			y += 35f;
		}

        // Token: 0x0600008D RID: 141 RVA: 0x00008F18 File Offset: 0x00007118
        private static float CalculateBotContentHeight(BotStatus status)
        {
            float num = 0f;

            // STATUS header
            num += 45f;

            // State row
            num += 35f;

            if (status != null && status.State != BotState.Disabled)
            {
                // Player HP bar, if visible
                if (status.PlayerMaxHp > 0)
                    num += 35f;

                // Target row
                num += 35f;

                // Target HP bar, if visible
                if (status.TargetMaxHp > 0)
                    num += 35f;

                // Timers row
                num += 35f;

                // Counters row
                num += 35f;

                // Request telemetry row
                num += 40f;
            }
            else
            {
                num += 15f;
            }

            // CONFIGURATION
            num += 45f; // section header
            num += 35f; // AFK toggle
            num += 35f; // Action Interval
            num += 35f; // Attack Interval
            num += 35f; // Search Range
            num += 15f;

            // LOOTING
            num += 45f; // section header
            num += 35f; // Enable Looting
            num += 35f; // Loot Delay
            num += 35f; // Loot Range
            num += 15f;

            // AUTO-STORE
            num += 45f; // section header
            num += 35f; // Enable Auto-Store
            if (BotController.Config != null && BotController.Config.EnableAutoStore && status != null && status.State != BotState.Disabled)
                num += 35f; // Store status row
            num += 40f; // Store Now button/status row

            // AUTO-SELL
            num += 45f; // section header
            num += 35f; // Enable Auto-Sell
            if (BotController.Config != null && BotController.Config.EnableAutoSell && status != null && status.State != BotState.Disabled)
                num += 35f; // Sell status row
            num += 40f; // Sell Now button/status row

            // SKILLS
            num += 45f; // section header
            num += 35f; // Enable Auto-Skills
            num += 35f; // Skill Interval
            num += 35f; // "Configure skills" label

            // BOT AI / BOT BEHAVIOUR
            num += 45f; // section header
            num += 30f; // Target Priority row
            num += 35f; // Modes description row

            // Important: bottom padding so last row is not hidden behind clipping / scrollbar limit
            num += 60f;

            return num;
        }


        // Token: 0x0600008E RID: 142 RVA: 0x0000904C File Offset: 0x0000724C
        private static string StateString(BotState s)
		{
			string result;
			switch (s)
			{
			case BotState.Disabled:
				result = "Disabled";
				break;
			case BotState.Idle:
				result = "Idle";
				break;
			case BotState.Combat:
				result = "Combat";
				break;
			case BotState.Looting:
				result = "Looting";
				break;
			case BotState.Reviving:
				result = "Reviving";
				break;
			case BotState.Selling:
				result = "Selling";
				break;
			case BotState.Storing:
				result = "Storing";
				break;
			default:
				result = "Unknown";
				break;
			}
			return result;
		}

		// Token: 0x0600008F RID: 143 RVA: 0x000090BC File Offset: 0x000072BC
		private static void DrawVisualsTab(float startY)
		{
			float contentHeight = 400f;
			float num = ModUI.BeginScrollRegion(ref ModUI._tabScrollY[5], startY, contentHeight, 20f, 820f);
			ModUI.SectionHeaderManual(ref num, "ESP / OVERLAYS");
			MonsterEspService.Enabled = GUI.Toggle(new Rect(0f, num, 300f, 30f), MonsterEspService.Enabled, "Monster ESP");
			LootEspService.Enabled = GUI.Toggle(new Rect(310f, num, 300f, 30f), LootEspService.Enabled, "Loot ESP");
			num += 35f;
			WorldMapOverlayService.Enabled = GUI.Toggle(new Rect(0f, num, 300f, 30f), WorldMapOverlayService.Enabled, "World Map Overlay");
			num += 35f;
			if (WorldMapOverlayService.Enabled)
			{
				float opacity = WorldMapOverlayService.Opacity;
				ModUI.LabeledSliderManual(ref num, "Map Opacity", ref opacity, 0.1f, 1f, "");
				WorldMapOverlayService.Opacity = opacity;
			}
			ModUI.EndScrollRegion();
		}

		// Token: 0x06000090 RID: 144 RVA: 0x000091B8 File Offset: 0x000073B8
		private static void DrawSettingsTab(float startY)
		{
			float contentHeight = 1200f;
			float num = ModUI.BeginScrollRegion(ref ModUI._tabScrollY[6], startY, contentHeight, 20f, 820f);
			ModUI.SectionHeaderManual(ref num, "AUTO DASH EVADE");
			BotConfig config = BotController.Config;
			config.EnableAutoDashEvade = GUI.Toggle(new Rect(0f, num, 380f, 30f), config.EnableAutoDashEvade, "Auto Dash Evade (smart cast-time dodge)");
			num += 35f;
			if (config.EnableAutoDashEvade)
			{
				ModUI.LabeledSliderManual(ref num, "Dash Trigger Dist", ref config.AutoDashTriggerDistance, 3f, 20f, "m");
				ModUI.LabeledSliderManual(ref num, "Dash Cooldown", ref config.AutoDashCooldown, 0.5f, 8f, "s");
				ModUI.LabeledSliderManual(ref num, "Dodge Lead Time", ref config.DodgeLeadTime, 0.05f, 1f, "s");
				GUI.Label(new Rect(0f, num, 780f, 25f), "  ↳ Fire dodge this many seconds before enemy cast finishes.");
				num += 28f;
				ModUI.LabeledSliderManual(ref num, "Fallback Delay", ref config.DodgeFallbackDelay, 0.1f, 2f, "s");
				GUI.Label(new Rect(0f, num, 780f, 25f), "  ↳ Delay before dodging when cast duration is unknown.");
				num += 28f;
				config.DodgeSkipAttached = GUI.Toggle(new Rect(0f, num, 380f, 30f), config.DodgeSkipAttached, "Skip Attached/Tracking Skills");
				num += 35f;
				config.DodgeCheckAOERadius = GUI.Toggle(new Rect(0f, num, 380f, 30f), config.DodgeCheckAOERadius, "Check AOE Radius (skip if safely out of range)");
				num += 35f;
			}
			ModUI.SectionHeaderManual(ref num, "STUCK RECOVERY");
			StuckRecoveryService.Enabled = GUI.Toggle(new Rect(0f, num, 300f, 30f), StuckRecoveryService.Enabled, "Stuck Recovery");
			num += 35f;
			GUI.Label(new Rect(0f, num, 110f, 25f), "Stuck Sec:");
			StuckRecoveryService.StuckSeconds = GUI.HorizontalSlider(new Rect(110f, num + 10f, 200f, 20f), StuckRecoveryService.StuckSeconds, 2f, 10f);
			Rect rect = new Rect(320f, num, 60f, 25f);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
			defaultInterpolatedStringHandler.AppendFormatted<float>(StuckRecoveryService.StuckSeconds, "F1");
			GUI.Label(rect, defaultInterpolatedStringHandler.ToStringAndClear());
			num += 30f;
			GUI.Label(new Rect(0f, num, 150f, 25f), "Recovery Cooldown:");
			StuckRecoveryService.CooldownSeconds = GUI.HorizontalSlider(new Rect(150f, num + 10f, 200f, 20f), StuckRecoveryService.CooldownSeconds, 1f, 12f);
			Rect rect2 = new Rect(360f, num, 60f, 25f);
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
			defaultInterpolatedStringHandler.AppendFormatted<float>(StuckRecoveryService.CooldownSeconds, "F1");
			GUI.Label(rect2, defaultInterpolatedStringHandler.ToStringAndClear());
			num += 40f;
			ModUI.SectionHeaderManual(ref num, "STATUS");
			BotStatus status = BotController.Status;
			if (status != null)
			{
				Rect rect3 = new Rect(0f, num, 780f, 25f);
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(32, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Target Mode: ");
				defaultInterpolatedStringHandler.AppendFormatted(string.IsNullOrEmpty(status.LastTargetPriorityMode) ? IdleState.TargetPriority.ToString() : status.LastTargetPriorityMode);
				defaultInterpolatedStringHandler.AppendLiteral("  ");
				defaultInterpolatedStringHandler.AppendLiteral("LastTargetDist: ");
				defaultInterpolatedStringHandler.AppendFormatted<float>(status.LastTargetDistance, "F1");
				defaultInterpolatedStringHandler.AppendLiteral("m");
				GUI.Label(rect3, defaultInterpolatedStringHandler.ToStringAndClear());
				num += 28f;
				Rect rect4 = new Rect(0f, num, 780f, 25f);
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(43, 3);
				defaultInterpolatedStringHandler.AppendLiteral("LastLootDist: ");
				defaultInterpolatedStringHandler.AppendFormatted<float>(status.LastLootDistance, "F1");
				defaultInterpolatedStringHandler.AppendLiteral("m  StuckSeen: ");
				defaultInterpolatedStringHandler.AppendFormatted<float>(status.StuckSecondsObserved, "F1");
				defaultInterpolatedStringHandler.AppendLiteral("s  Recoveries: ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(status.StuckRecoveries);
				GUI.Label(rect4, defaultInterpolatedStringHandler.ToStringAndClear());
				num += 28f;
				GUI.Label(new Rect(0f, num, 780f, 25f), "LastRecovery: " + (string.IsNullOrEmpty(status.LastRecoveryMessage) ? "-" : status.LastRecoveryMessage));
				num += 35f;
			}
			ModUI.SectionHeaderManual(ref num, "COMBAT TELEMETRY");
			CombatTelemetryService.Enabled = GUI.Toggle(new Rect(0f, num, 280f, 30f), CombatTelemetryService.Enabled, "Enable Combat Telemetry");
			if (GUI.Button(new Rect(300f, num, 120f, 28f), "Reset Telemetry"))
			{
				CombatTelemetryService.Reset();
			}
			num += 35f;
			GUI.Label(new Rect(0f, num, 780f, 25f), "Last Event: " + CombatTelemetryService.LastEvent);
			num += 30f;
			int num2 = 0;
			foreach (KeyValuePair<string, int> keyValuePair in CombatTelemetryService.Counters)
			{
				if (num2 >= 8)
				{
					break;
				}
				Rect rect5 = new Rect(0f, num, 780f, 22f);
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 2);
				defaultInterpolatedStringHandler.AppendFormatted(keyValuePair.Key);
				defaultInterpolatedStringHandler.AppendLiteral(" = ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(keyValuePair.Value);
				GUI.Label(rect5, defaultInterpolatedStringHandler.ToStringAndClear());
				num += 24f;
				num2++;
			}
			if (num2 == 0)
			{
				GUI.Label(new Rect(0f, num, 780f, 22f), "No telemetry yet.");
				num += 24f;
			}
			num += 10f;
			GUI.Label(new Rect(0f, num, 780f, 25f), "Recent:");
			num += 25f;
			string[] recent = CombatTelemetryService.Recent;
			for (int i = 0; i < recent.Length; i++)
			{
				int num3 = (recent.Length + i - 1) % recent.Length;
				string text = recent[num3];
				if (!string.IsNullOrEmpty(text))
				{
					GUI.Label(new Rect(0f, num, 800f, 22f), text);
					num += 22f;
				}
			}
			ModUI.EndScrollRegion();
		}

		// Token: 0x06000091 RID: 145 RVA: 0x00009860 File Offset: 0x00007A60
		private static void EnsureResources()
		{
			if (ModUI._windowBgTex == null)
			{
				ModUI._windowBgTex = ModUI.MakeTex(new Color(0.25f, 0.25f, 0.25f, 1f));
			}
			if (ModUI._barBgTex == null)
			{
				ModUI._barBgTex = ModUI.MakeTex(new Color(0.15f, 0.15f, 0.15f, 0.95f));
			}
			if (ModUI._barHpTex == null)
			{
				ModUI._barHpTex = ModUI.MakeTex(new Color(0.2f, 0.7f, 0.2f, 0.9f));
			}
			if (ModUI._barTargetTex == null)
			{
				ModUI._barTargetTex = ModUI.MakeTex(new Color(0.8f, 0.3f, 0.3f, 0.9f));
			}
			if (ModUI._scrollbarBgTex == null)
			{
				ModUI._scrollbarBgTex = ModUI.MakeTex(new Color(0.2f, 0.2f, 0.2f, 0.5f));
			}
			if (ModUI._scrollbarThumbTex == null)
			{
				ModUI._scrollbarThumbTex = ModUI.MakeTex(new Color(0.5f, 0.5f, 0.5f, 0.8f));
			}
			if (!ModUI._stylesReady)
			{
				ModUI._headerStyle = new GUIStyle();
				ModUI._headerStyle.fontSize = 18;
				ModUI._headerStyle.fontStyle = (FontStyle)1;
				ModUI._headerStyle.richText = true;
				ModUI._headerStyle.normal.textColor = GUI.skin.label.normal.textColor;
				ModUI._richLabel = new GUIStyle();
				ModUI._richLabel.fontSize = 14;
				ModUI._richLabel.richText = true;
				ModUI._richLabel.normal.textColor = GUI.skin.label.normal.textColor;
				ModUI._sectionStyle = new GUIStyle();
				ModUI._sectionStyle.fontSize = 16;
				ModUI._sectionStyle.fontStyle = (FontStyle)1;
				ModUI._sectionStyle.richText = true;
				ModUI._sectionStyle.normal.textColor = GUI.skin.label.normal.textColor;
				ModUI._stylesReady = true;
			}
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00009A7F File Offset: 0x00007C7F
		private static Texture2D MakeTex(Color col)
		{
			Texture2D texture2D = new Texture2D(1, 1, (TextureFormat)4, false);
			texture2D.SetPixel(0, 0, col);
			texture2D.Apply();
			return texture2D;
		}

		// Token: 0x04000095 RID: 149
		private static bool _visible;

		// Token: 0x04000096 RID: 150
		private static CursorLockMode _savedLockState;

		// Token: 0x04000097 RID: 151
		private static bool _savedCursorVisible;

		// Token: 0x04000098 RID: 152
		private static bool _cursorCached;

		// Token: 0x04000099 RID: 153
		private static Rect _windowRect = new Rect(100f, 100f, 1050f, 1000f);

		// Token: 0x0400009A RID: 154
		private static int _currentTab;

		// Token: 0x0400009B RID: 155
		private static float[] _tabScrollY = new float[7];

		// Token: 0x0400009C RID: 156
		private static bool _showServerDropdown;

		// Token: 0x0400009D RID: 157
		private static float _filterItemScrollY = 0f;

		// Token: 0x0400009E RID: 158
		private static string _filterSearch = "";

		// Token: 0x0400009F RID: 159
		private static bool _searchFocused = false;

		// Token: 0x040000A0 RID: 160
		private static string _openDropdownTag = null;

		// Token: 0x040000A1 RID: 161
		private static string _lastDumpResult = null;

		// Token: 0x040000A2 RID: 162
		private static int _openDropdownIdx = -1;

		// Token: 0x040000A3 RID: 163
		private static int _dragScrollId = 0;

		// Token: 0x040000A4 RID: 164
		private static int _nextScrollId = 1;

		// Token: 0x040000A5 RID: 165
		private static readonly string[] TabLabels = new string[]
		{
			"Cheats",
			"Teleporter",
			"Bot",
			"Skill Config",
			"Loot Filter",
			"Visuals",
			"Settings"
		};

		// Token: 0x040000A6 RID: 166
		private static int _selectedMapIndex = 0;

		// Token: 0x040000A7 RID: 167
		private static bool _dropdownOpen = false;

		// Token: 0x040000A8 RID: 168
		private static float _dropdownScrollY = 0f;

		// Token: 0x040000A9 RID: 169
		private static Texture2D _windowBgTex;

		// Token: 0x040000AA RID: 170
		private static Texture2D _barBgTex;

		// Token: 0x040000AB RID: 171
		private static Texture2D _barHpTex;

		// Token: 0x040000AC RID: 172
		private static Texture2D _barTargetTex;

		// Token: 0x040000AD RID: 173
		private static Texture2D _scrollbarBgTex;

		// Token: 0x040000AE RID: 174
		private static Texture2D _scrollbarThumbTex;

		// Token: 0x040000AF RID: 175
		private static GUIStyle _headerStyle;

		// Token: 0x040000B0 RID: 176
		private static GUIStyle _richLabel;

		// Token: 0x040000B1 RID: 177
		private static GUIStyle _sectionStyle;

		// Token: 0x040000B2 RID: 178
		private static bool _stylesReady;
	}
}
