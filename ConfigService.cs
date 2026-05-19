using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Il2Cpp;
using MelonLoader;

namespace SpiritMod
{
	// Token: 0x02000015 RID: 21
	public static class ConfigService
	{
		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000094 RID: 148 RVA: 0x00009B63 File Offset: 0x00007D63
		// (set) Token: 0x06000095 RID: 149 RVA: 0x00009B6A File Offset: 0x00007D6A
		public static string StatusMessage { get; private set; } = string.Empty;

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000096 RID: 150 RVA: 0x00009B72 File Offset: 0x00007D72
		// (set) Token: 0x06000097 RID: 151 RVA: 0x00009B79 File Offset: 0x00007D79
		public static string CharacterName { get; set; } = string.Empty;

		// Token: 0x06000098 RID: 152 RVA: 0x00009B84 File Offset: 0x00007D84
		public static void DetectCharacterName()
		{
			try
			{
				PlayerController playerController = GameStateService.Player ?? App.Player;
				if (playerController != null)
				{
					string displayName = playerController.Cast<BaseUnitController>().DisplayName;
					if (!string.IsNullOrEmpty(displayName))
					{
						ConfigService.CharacterName = displayName;
					}
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Warning("[Config] Could not detect character name: " + ex.Message);
			}
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00009BF0 File Offset: 0x00007DF0
		public static void Save()
		{
			try
			{
				if (string.IsNullOrEmpty(ConfigService.CharacterName))
				{
					ConfigService.DetectCharacterName();
					if (string.IsNullOrEmpty(ConfigService.CharacterName))
					{
						ConfigService.StatusMessage = "Save failed: no character name.";
						return;
					}
				}
				BotConfig config = BotController.Config;
				ConfigService.SaveData value = new ConfigService.SaveData
				{
					ActionInterval = config.ActionInterval,
					AttackInterval = config.AttackInterval,
					SearchRange = config.SearchRange,
					EnableLooting = config.EnableLooting,
					LootDelay = config.LootDelay,
					LootRange = config.LootRange,
					EnableSkills = config.EnableSkills,
					SkillInterval = config.SkillInterval,
					EnabledSkillSlots = (bool[])config.EnabledSkillSlots.Clone(),
					SkillPriorities = (int[])config.SkillPriorities.Clone(),
					TreatAsHealing = (bool[])config.TreatAsHealing.Clone(),
					HealingThresholds = (int[])config.HealingThresholds.Clone(),
					EnableBuffMaintenance = config.EnableBuffMaintenance,
					TreatAsBuff = (bool[])config.TreatAsBuff.Clone(),
					EnabledBuffSlots = (bool[])config.EnabledBuffSlots.Clone(),
					BuffRefreshLeadSeconds = (float[])config.BuffRefreshLeadSeconds.Clone(),
					EnableAutoDashEvade = config.EnableAutoDashEvade,
					AutoDashTriggerDistance = config.AutoDashTriggerDistance,
					AutoDashCooldown = config.AutoDashCooldown,
					DodgeLeadTime = config.DodgeLeadTime,
					DodgeFallbackDelay = config.DodgeFallbackDelay,
					DodgeSkipAttached = config.DodgeSkipAttached,
					DodgeCheckAOERadius = config.DodgeCheckAOERadius,
					EnableMonsterEsp = MonsterEspService.Enabled,
					EnableLootEsp = LootEspService.Enabled,
					EnableWorldMapOverlay = WorldMapOverlayService.Enabled,
					WorldMapOpacity = WorldMapOverlayService.Opacity,
					EnableAfkLeech = config.EnableAfkLeech,
					EnableAutoRevive = config.EnableAutoRevive,
					FarmingMapIndex = config.FarmingMapIndex,
					FarmingMapName = (config.FarmingMapName ?? string.Empty),
					ReviveDelay = config.ReviveDelay,
					EnableAutoSell = config.EnableAutoSell,
					EnableAutoStore = config.EnableAutoStore,
					SellFilterEnabled = LootFilterService.SellFilterEnabled,
					SellRarityEnabled = (bool[])LootFilterService.SellRarityEnabled.Clone(),
					EnableAutoLogin = config.EnableAutoLogin,
					AutoLoginServerName = config.AutoLoginServerName,
					BaseSpeedOverride = Cheats.BaseSpeedOverride,
					BaseSpeed = Cheats.BaseSpeed
				};
				ConfigService.ConfigFile configFile = ConfigService.LoadConfigFile() ?? new ConfigService.ConfigFile();
				configFile.Characters[ConfigService.CharacterName] = value;
				Directory.CreateDirectory(ConfigService._directory);
				JsonSerializerOptions options = new JsonSerializerOptions
				{
					WriteIndented = true
				};
				string contents = JsonSerializer.Serialize<ConfigService.ConfigFile>(configFile, options);
				File.WriteAllText(ConfigService._filePath, contents);
				ConfigService.StatusMessage = "Saved (" + ConfigService.CharacterName + ")";
				MelonLogger.Msg("[Config] Saved '" + ConfigService.CharacterName + "' to " + ConfigService._filePath);
			}
			catch (Exception ex)
			{
				ConfigService.StatusMessage = "Save failed: " + ex.Message;
				MelonLogger.Warning("[Config] Save failed: " + ex.Message);
			}
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00009F34 File Offset: 0x00008134
		public static void Load()
		{
			try
			{
				if (string.IsNullOrEmpty(ConfigService.CharacterName))
				{
					ConfigService.DetectCharacterName();
					if (string.IsNullOrEmpty(ConfigService.CharacterName))
					{
						ConfigService.StatusMessage = "Load failed: no character name.";
						return;
					}
				}
				ConfigService.ConfigFile configFile = ConfigService.LoadConfigFile();
				ConfigService.SaveData saveData;
				if (configFile == null)
				{
					ConfigService.StatusMessage = "No config file found.";
				}
				else if (!configFile.Characters.TryGetValue(ConfigService.CharacterName, out saveData))
				{
					ConfigService.StatusMessage = "No config for '" + ConfigService.CharacterName + "'.";
					MelonLogger.Warning("[Config] No saved config for character '" + ConfigService.CharacterName + "'");
				}
				else
				{
					BotConfig config = BotController.Config;
					config.ActionInterval = saveData.ActionInterval;
					config.AttackInterval = saveData.AttackInterval;
					config.SearchRange = saveData.SearchRange;
					config.EnableLooting = saveData.EnableLooting;
					config.LootDelay = saveData.LootDelay;
					config.LootRange = saveData.LootRange;
					config.EnableSkills = saveData.EnableSkills;
					config.SkillInterval = saveData.SkillInterval;
					if (saveData.EnabledSkillSlots != null)
					{
						Array.Copy(saveData.EnabledSkillSlots, config.EnabledSkillSlots, Math.Min(saveData.EnabledSkillSlots.Length, config.EnabledSkillSlots.Length));
					}
					if (saveData.SkillPriorities != null)
					{
						Array.Copy(saveData.SkillPriorities, config.SkillPriorities, Math.Min(saveData.SkillPriorities.Length, config.SkillPriorities.Length));
					}
					if (saveData.TreatAsHealing != null)
					{
						Array.Copy(saveData.TreatAsHealing, config.TreatAsHealing, Math.Min(saveData.TreatAsHealing.Length, config.TreatAsHealing.Length));
					}
					if (saveData.HealingThresholds != null)
					{
						Array.Copy(saveData.HealingThresholds, config.HealingThresholds, Math.Min(saveData.HealingThresholds.Length, config.HealingThresholds.Length));
					}
					config.EnableBuffMaintenance = saveData.EnableBuffMaintenance;
					if (saveData.TreatAsBuff != null)
					{
						Array.Copy(saveData.TreatAsBuff, config.TreatAsBuff, Math.Min(saveData.TreatAsBuff.Length, config.TreatAsBuff.Length));
					}
					if (saveData.EnabledBuffSlots != null)
					{
						Array.Copy(saveData.EnabledBuffSlots, config.EnabledBuffSlots, Math.Min(saveData.EnabledBuffSlots.Length, config.EnabledBuffSlots.Length));
					}
					if (saveData.BuffRefreshLeadSeconds != null)
					{
						Array.Copy(saveData.BuffRefreshLeadSeconds, config.BuffRefreshLeadSeconds, Math.Min(saveData.BuffRefreshLeadSeconds.Length, config.BuffRefreshLeadSeconds.Length));
					}
					config.EnableAutoDashEvade = saveData.EnableAutoDashEvade;
					config.AutoDashTriggerDistance = ((saveData.AutoDashTriggerDistance > 0f) ? saveData.AutoDashTriggerDistance : 8f);
					config.AutoDashCooldown = ((saveData.AutoDashCooldown > 0f) ? saveData.AutoDashCooldown : 2f);
					config.DodgeLeadTime = ((saveData.DodgeLeadTime > 0f) ? saveData.DodgeLeadTime : 0.35f);
					config.DodgeFallbackDelay = ((saveData.DodgeFallbackDelay > 0f) ? saveData.DodgeFallbackDelay : 0.5f);
					config.DodgeSkipAttached = saveData.DodgeSkipAttached;
					config.DodgeCheckAOERadius = saveData.DodgeCheckAOERadius;
					MonsterEspService.Enabled = saveData.EnableMonsterEsp;
					LootEspService.Enabled = saveData.EnableLootEsp;
					WorldMapOverlayService.Enabled = saveData.EnableWorldMapOverlay;
					WorldMapOverlayService.Opacity = ((saveData.WorldMapOpacity > 0f) ? saveData.WorldMapOpacity : 0.72f);
					config.EnableAfkLeech = saveData.EnableAfkLeech;
					config.EnableAutoRevive = saveData.EnableAutoRevive;
					config.FarmingMapIndex = saveData.FarmingMapIndex;
					config.FarmingMapName = (saveData.FarmingMapName ?? string.Empty);
					config.ReviveDelay = ((saveData.ReviveDelay > 0f) ? saveData.ReviveDelay : 3f);
					config.EnableAutoSell = saveData.EnableAutoSell;
					config.EnableAutoStore = saveData.EnableAutoStore;
					LootFilterService.SellFilterEnabled = saveData.SellFilterEnabled;
					if (saveData.SellRarityEnabled != null && saveData.SellRarityEnabled.Length >= 4)
					{
						bool[] sellRarityEnabled = LootFilterService.SellRarityEnabled;
						Array.Copy(saveData.SellRarityEnabled, sellRarityEnabled, Math.Min(saveData.SellRarityEnabled.Length, sellRarityEnabled.Length));
					}
					config.EnableAutoLogin = saveData.EnableAutoLogin;
					config.AutoLoginServerName = (saveData.AutoLoginServerName ?? "");
					Cheats.BaseSpeedOverride = saveData.BaseSpeedOverride;
					Cheats.BaseSpeed = saveData.BaseSpeed;
					ConfigService.StatusMessage = "Loaded (" + ConfigService.CharacterName + ")";
					MelonLogger.Msg("[Config] Loaded '" + ConfigService.CharacterName + "' from " + ConfigService._filePath);
				}
			}
			catch (Exception ex)
			{
				ConfigService.StatusMessage = "Load failed: " + ex.Message;
				MelonLogger.Warning("[Config] Load failed: " + ex.Message);
			}
		}

		// Token: 0x0600009B RID: 155 RVA: 0x0000A3D8 File Offset: 0x000085D8
		public static List<string> GetSavedCharacters()
		{
			try
			{
				ConfigService.ConfigFile configFile = ConfigService.LoadConfigFile();
				if (configFile != null)
				{
					return new List<string>(configFile.Characters.Keys);
				}
			}
			catch
			{
			}
			return new List<string>();
		}

		// Token: 0x0600009C RID: 156 RVA: 0x0000A420 File Offset: 0x00008620
		private static ConfigService.ConfigFile LoadConfigFile()
		{
			ConfigService.ConfigFile result;
			try
			{
				if (!File.Exists(ConfigService._filePath))
				{
					result = null;
				}
				else
				{
					result = JsonSerializer.Deserialize<ConfigService.ConfigFile>(File.ReadAllText(ConfigService._filePath));
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Warning("[Config] Failed to read config file: " + ex.Message);
				result = null;
			}
			return result;
		}

		// Token: 0x040000B3 RID: 179
		private static readonly string _directory = Path.Combine("UserData", "SpiritMod");

		// Token: 0x040000B4 RID: 180
		private static readonly string _filePath = Path.Combine(ConfigService._directory, "config.json");

		// Token: 0x0200003D RID: 61
		private class SaveData
		{
			// Token: 0x1700003D RID: 61
			// (get) Token: 0x06000184 RID: 388 RVA: 0x0001071D File Offset: 0x0000E91D
			// (set) Token: 0x06000185 RID: 389 RVA: 0x00010725 File Offset: 0x0000E925
			public float ActionInterval { get; set; }

			// Token: 0x1700003E RID: 62
			// (get) Token: 0x06000186 RID: 390 RVA: 0x0001072E File Offset: 0x0000E92E
			// (set) Token: 0x06000187 RID: 391 RVA: 0x00010736 File Offset: 0x0000E936
			public float AttackInterval { get; set; }

			// Token: 0x1700003F RID: 63
			// (get) Token: 0x06000188 RID: 392 RVA: 0x0001073F File Offset: 0x0000E93F
			// (set) Token: 0x06000189 RID: 393 RVA: 0x00010747 File Offset: 0x0000E947
			public float SearchRange { get; set; }

			// Token: 0x17000040 RID: 64
			// (get) Token: 0x0600018A RID: 394 RVA: 0x00010750 File Offset: 0x0000E950
			// (set) Token: 0x0600018B RID: 395 RVA: 0x00010758 File Offset: 0x0000E958
			public bool EnableLooting { get; set; }

			// Token: 0x17000041 RID: 65
			// (get) Token: 0x0600018C RID: 396 RVA: 0x00010761 File Offset: 0x0000E961
			// (set) Token: 0x0600018D RID: 397 RVA: 0x00010769 File Offset: 0x0000E969
			public float LootDelay { get; set; }

			// Token: 0x17000042 RID: 66
			// (get) Token: 0x0600018E RID: 398 RVA: 0x00010772 File Offset: 0x0000E972
			// (set) Token: 0x0600018F RID: 399 RVA: 0x0001077A File Offset: 0x0000E97A
			public float LootRange { get; set; }

			// Token: 0x17000043 RID: 67
			// (get) Token: 0x06000190 RID: 400 RVA: 0x00010783 File Offset: 0x0000E983
			// (set) Token: 0x06000191 RID: 401 RVA: 0x0001078B File Offset: 0x0000E98B
			public bool EnableSkills { get; set; }

			// Token: 0x17000044 RID: 68
			// (get) Token: 0x06000192 RID: 402 RVA: 0x00010794 File Offset: 0x0000E994
			// (set) Token: 0x06000193 RID: 403 RVA: 0x0001079C File Offset: 0x0000E99C
			public float SkillInterval { get; set; }

			// Token: 0x17000045 RID: 69
			// (get) Token: 0x06000194 RID: 404 RVA: 0x000107A5 File Offset: 0x0000E9A5
			// (set) Token: 0x06000195 RID: 405 RVA: 0x000107AD File Offset: 0x0000E9AD
			public bool[] EnabledSkillSlots { get; set; }

			// Token: 0x17000046 RID: 70
			// (get) Token: 0x06000196 RID: 406 RVA: 0x000107B6 File Offset: 0x0000E9B6
			// (set) Token: 0x06000197 RID: 407 RVA: 0x000107BE File Offset: 0x0000E9BE
			public int[] SkillPriorities { get; set; }

			// Token: 0x17000047 RID: 71
			// (get) Token: 0x06000198 RID: 408 RVA: 0x000107C7 File Offset: 0x0000E9C7
			// (set) Token: 0x06000199 RID: 409 RVA: 0x000107CF File Offset: 0x0000E9CF
			public bool[] TreatAsHealing { get; set; }

			// Token: 0x17000048 RID: 72
			// (get) Token: 0x0600019A RID: 410 RVA: 0x000107D8 File Offset: 0x0000E9D8
			// (set) Token: 0x0600019B RID: 411 RVA: 0x000107E0 File Offset: 0x0000E9E0
			public int[] HealingThresholds { get; set; }

			// Token: 0x17000049 RID: 73
			// (get) Token: 0x0600019C RID: 412 RVA: 0x000107E9 File Offset: 0x0000E9E9
			// (set) Token: 0x0600019D RID: 413 RVA: 0x000107F1 File Offset: 0x0000E9F1
			public bool EnableBuffMaintenance { get; set; }

			// Token: 0x1700004A RID: 74
			// (get) Token: 0x0600019E RID: 414 RVA: 0x000107FA File Offset: 0x0000E9FA
			// (set) Token: 0x0600019F RID: 415 RVA: 0x00010802 File Offset: 0x0000EA02
			public bool[] TreatAsBuff { get; set; }

			// Token: 0x1700004B RID: 75
			// (get) Token: 0x060001A0 RID: 416 RVA: 0x0001080B File Offset: 0x0000EA0B
			// (set) Token: 0x060001A1 RID: 417 RVA: 0x00010813 File Offset: 0x0000EA13
			public bool[] EnabledBuffSlots { get; set; }

			// Token: 0x1700004C RID: 76
			// (get) Token: 0x060001A2 RID: 418 RVA: 0x0001081C File Offset: 0x0000EA1C
			// (set) Token: 0x060001A3 RID: 419 RVA: 0x00010824 File Offset: 0x0000EA24
			public float[] BuffRefreshLeadSeconds { get; set; }

			// Token: 0x1700004D RID: 77
			// (get) Token: 0x060001A4 RID: 420 RVA: 0x0001082D File Offset: 0x0000EA2D
			// (set) Token: 0x060001A5 RID: 421 RVA: 0x00010835 File Offset: 0x0000EA35
			public bool EnableAutoDashEvade { get; set; }

			// Token: 0x1700004E RID: 78
			// (get) Token: 0x060001A6 RID: 422 RVA: 0x0001083E File Offset: 0x0000EA3E
			// (set) Token: 0x060001A7 RID: 423 RVA: 0x00010846 File Offset: 0x0000EA46
			public float AutoDashTriggerDistance { get; set; }

			// Token: 0x1700004F RID: 79
			// (get) Token: 0x060001A8 RID: 424 RVA: 0x0001084F File Offset: 0x0000EA4F
			// (set) Token: 0x060001A9 RID: 425 RVA: 0x00010857 File Offset: 0x0000EA57
			public float AutoDashCooldown { get; set; }

			// Token: 0x17000050 RID: 80
			// (get) Token: 0x060001AA RID: 426 RVA: 0x00010860 File Offset: 0x0000EA60
			// (set) Token: 0x060001AB RID: 427 RVA: 0x00010868 File Offset: 0x0000EA68
			public float DodgeLeadTime { get; set; }

			// Token: 0x17000051 RID: 81
			// (get) Token: 0x060001AC RID: 428 RVA: 0x00010871 File Offset: 0x0000EA71
			// (set) Token: 0x060001AD RID: 429 RVA: 0x00010879 File Offset: 0x0000EA79
			public float DodgeFallbackDelay { get; set; }

			// Token: 0x17000052 RID: 82
			// (get) Token: 0x060001AE RID: 430 RVA: 0x00010882 File Offset: 0x0000EA82
			// (set) Token: 0x060001AF RID: 431 RVA: 0x0001088A File Offset: 0x0000EA8A
			public bool DodgeSkipAttached { get; set; }

			// Token: 0x17000053 RID: 83
			// (get) Token: 0x060001B0 RID: 432 RVA: 0x00010893 File Offset: 0x0000EA93
			// (set) Token: 0x060001B1 RID: 433 RVA: 0x0001089B File Offset: 0x0000EA9B
			public bool DodgeCheckAOERadius { get; set; }

			// Token: 0x17000054 RID: 84
			// (get) Token: 0x060001B2 RID: 434 RVA: 0x000108A4 File Offset: 0x0000EAA4
			// (set) Token: 0x060001B3 RID: 435 RVA: 0x000108AC File Offset: 0x0000EAAC
			public bool EnableAfkLeech { get; set; }

			// Token: 0x17000055 RID: 85
			// (get) Token: 0x060001B4 RID: 436 RVA: 0x000108B5 File Offset: 0x0000EAB5
			// (set) Token: 0x060001B5 RID: 437 RVA: 0x000108BD File Offset: 0x0000EABD
			public bool EnableAutoRevive { get; set; }

			// Token: 0x17000056 RID: 86
			// (get) Token: 0x060001B6 RID: 438 RVA: 0x000108C6 File Offset: 0x0000EAC6
			// (set) Token: 0x060001B7 RID: 439 RVA: 0x000108CE File Offset: 0x0000EACE
			public int FarmingMapIndex { get; set; }

			// Token: 0x17000057 RID: 87
			// (get) Token: 0x060001B8 RID: 440 RVA: 0x000108D7 File Offset: 0x0000EAD7
			// (set) Token: 0x060001B9 RID: 441 RVA: 0x000108DF File Offset: 0x0000EADF
			public string FarmingMapName { get; set; }

			// Token: 0x17000058 RID: 88
			// (get) Token: 0x060001BA RID: 442 RVA: 0x000108E8 File Offset: 0x0000EAE8
			// (set) Token: 0x060001BB RID: 443 RVA: 0x000108F0 File Offset: 0x0000EAF0
			public float ReviveDelay { get; set; }

			// Token: 0x17000059 RID: 89
			// (get) Token: 0x060001BC RID: 444 RVA: 0x000108F9 File Offset: 0x0000EAF9
			// (set) Token: 0x060001BD RID: 445 RVA: 0x00010901 File Offset: 0x0000EB01
			public bool EnableAutoSell { get; set; }

			// Token: 0x1700005A RID: 90
			// (get) Token: 0x060001BE RID: 446 RVA: 0x0001090A File Offset: 0x0000EB0A
			// (set) Token: 0x060001BF RID: 447 RVA: 0x00010912 File Offset: 0x0000EB12
			public bool EnableAutoStore { get; set; }

			// Token: 0x1700005B RID: 91
			// (get) Token: 0x060001C0 RID: 448 RVA: 0x0001091B File Offset: 0x0000EB1B
			// (set) Token: 0x060001C1 RID: 449 RVA: 0x00010923 File Offset: 0x0000EB23
			public bool SellFilterEnabled { get; set; }

			// Token: 0x1700005C RID: 92
			// (get) Token: 0x060001C2 RID: 450 RVA: 0x0001092C File Offset: 0x0000EB2C
			// (set) Token: 0x060001C3 RID: 451 RVA: 0x00010934 File Offset: 0x0000EB34
			public bool[] SellRarityEnabled { get; set; }

			// Token: 0x1700005D RID: 93
			// (get) Token: 0x060001C4 RID: 452 RVA: 0x0001093D File Offset: 0x0000EB3D
			// (set) Token: 0x060001C5 RID: 453 RVA: 0x00010945 File Offset: 0x0000EB45
			public bool EnableAutoLogin { get; set; }

			// Token: 0x1700005E RID: 94
			// (get) Token: 0x060001C6 RID: 454 RVA: 0x0001094E File Offset: 0x0000EB4E
			// (set) Token: 0x060001C7 RID: 455 RVA: 0x00010956 File Offset: 0x0000EB56
			public string AutoLoginServerName { get; set; } = "";

			// Token: 0x1700005F RID: 95
			// (get) Token: 0x060001C8 RID: 456 RVA: 0x0001095F File Offset: 0x0000EB5F
			// (set) Token: 0x060001C9 RID: 457 RVA: 0x00010967 File Offset: 0x0000EB67
			public bool BaseSpeedOverride { get; set; }

			// Token: 0x17000060 RID: 96
			// (get) Token: 0x060001CA RID: 458 RVA: 0x00010970 File Offset: 0x0000EB70
			// (set) Token: 0x060001CB RID: 459 RVA: 0x00010978 File Offset: 0x0000EB78
			public float BaseSpeed { get; set; }

			// Token: 0x17000061 RID: 97
			// (get) Token: 0x060001CC RID: 460 RVA: 0x00010981 File Offset: 0x0000EB81
			// (set) Token: 0x060001CD RID: 461 RVA: 0x00010989 File Offset: 0x0000EB89
			public bool EnableMonsterEsp { get; set; }

			// Token: 0x17000062 RID: 98
			// (get) Token: 0x060001CE RID: 462 RVA: 0x00010992 File Offset: 0x0000EB92
			// (set) Token: 0x060001CF RID: 463 RVA: 0x0001099A File Offset: 0x0000EB9A
			public bool EnableLootEsp { get; set; }

			// Token: 0x17000063 RID: 99
			// (get) Token: 0x060001D0 RID: 464 RVA: 0x000109A3 File Offset: 0x0000EBA3
			// (set) Token: 0x060001D1 RID: 465 RVA: 0x000109AB File Offset: 0x0000EBAB
			public bool EnableWorldMapOverlay { get; set; }

			// Token: 0x17000064 RID: 100
			// (get) Token: 0x060001D2 RID: 466 RVA: 0x000109B4 File Offset: 0x0000EBB4
			// (set) Token: 0x060001D3 RID: 467 RVA: 0x000109BC File Offset: 0x0000EBBC
			public float WorldMapOpacity { get; set; } = 0.72f;
		}

		// Token: 0x0200003E RID: 62
		private class ConfigFile
		{
			// Token: 0x17000065 RID: 101
			// (get) Token: 0x060001D5 RID: 469 RVA: 0x000109E3 File Offset: 0x0000EBE3
			// (set) Token: 0x060001D6 RID: 470 RVA: 0x000109EB File Offset: 0x0000EBEB
			public Dictionary<string, ConfigService.SaveData> Characters { get; set; } = new Dictionary<string, ConfigService.SaveData>();
		}
	}
}
