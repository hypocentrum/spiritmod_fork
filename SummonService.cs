using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Collections.Generic;
using MelonLoader;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Object = UnityEngine.Object;

namespace SpiritMod
{
	// Token: 0x0200001F RID: 31
	public static class SummonService
	{
		// Token: 0x060000FF RID: 255 RVA: 0x0000CAD4 File Offset: 0x0000ACD4
		public static bool AllSummonsSatisfied(PlayerController player, BotConfig cfg)
		{
			return SummonService.GetMissingSummonSlots(player, cfg).Count == 0;
		}

		// Token: 0x06000100 RID: 256 RVA: 0x0000CAE8 File Offset: 0x0000ACE8
		public static Il2CppSystem.Collections.Generic.List<int> GetMissingSummonSlots(PlayerController player, BotConfig cfg)
		{
			SummonService._missingSlots.Clear();
			try
			{
				int maxCloneLimit = 3;
                Il2CppSystem.Collections.Generic.Dictionary<string, int> cachedSummons = SummonService.GetCachedSummons(player);
                System.Collections.Generic.List<SkillData> assignedSkillList = CombatService.GetAssignedSkillList(player);
				if (assignedSkillList == null)
				{
					return SummonService._missingSlots;
				}
				SkillsComponent skills = player.Cast<BaseUnitController>().Skills;
				if (skills == null)
				{
					return SummonService._missingSlots;
				}
				bool flag = false;
				int num = Math.Min(assignedSkillList.Count, 20);
				for (int i = 0; i < num; i++)
				{
					if (cfg.EnabledSkillSlots[i])
					{
						SkillData skillData = assignedSkillList[i];
						if (skillData != null)
						{
							string id = skillData.Id;
							if (!string.IsNullOrEmpty(id))
							{
								SkillState anySkill = skills.GetAnySkill(id);
								if (anySkill != null)
								{
									SkillConfig config = anySkill.Config;
									if (!(config == null) && CombatService.IsSummonSkill(config))
									{
										if (config.ExclusiveType == (SkillCategory)4)
										{
											if (!flag)
											{
												flag = true;
												if (!SummonService.HasAlivePrimarySummon(player))
                                                {
                                                    SummonService._missingSlots.Add(i);
												}
											}
										}
										else if (config.DisplayName == "Shadow Seal")
                                        {
                                            int cloneCount = SummonService.CountAlivePlayerClones(player);

                                            if (cloneCount < 3)
                                            {
                                                SummonService._missingSlots.Add(i);
                                            }

                                            continue;
                                        }
                                    }
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Warning("[Summon] GetMissingSummonSlots error: " + ex.Message);
			}
			return SummonService._missingSlots;
		}

		// Token: 0x06000101 RID: 257 RVA: 0x0000CC60 File Offset: 0x0000AE60
		public static bool IsSlotSatisfied(PlayerController player, BotConfig cfg, int slotIndex)
		{
			bool result;
			try
			{
				System.Collections.Generic.List<SkillData> assignedSkillList = CombatService.GetAssignedSkillList(player);
				if (assignedSkillList == null || slotIndex >= assignedSkillList.Count)
				{
					result = true;
				}
				else
				{
					SkillData skillData = assignedSkillList[slotIndex];
					if (skillData == null)
					{
						result = true;
					}
					else
					{
						SkillsComponent skills = player.Cast<BaseUnitController>().Skills;
						if (skills == null)
						{
							result = true;
						}
						else
						{
							SkillState anySkill = skills.GetAnySkill(skillData.Id);
							if (anySkill == null)
							{
								result = true;
							}
							else
							{
								SkillConfig config = anySkill.Config;
								if (config == null || !CombatService.IsSummonSkill(config))
								{
									result = true;
								}
								else if (config.ExclusiveType == SkillCategory.Summon)
								{
									result = SummonService.HasAlivePrimarySummon(player);
								}
								else
                                {
									if (config.DisplayName == "Shadow Seal")
									{
										result = SummonService.CountAlivePlayerClones(player) >= 3;
									}
									else
                                    {
                                        Il2CppSystem.Collections.Generic.Dictionary<string, int> cachedSummons = SummonService.GetCachedSummons(player);
                                        string monsterKey = SummonService.ExtractMonsterKey(config.DisplayName);
                                        int num = config.DisplayName == "Shadow Seal" ? 3 : Math.Max(1, anySkill.BaseLevel);
                                        result = (SummonService.CountMatchingSummons(cachedSummons, monsterKey) >= num);

                                    }
								}
							}
						}
					}
				}
			}
			catch
			{
				result = true;
			}
			return result;
		}

		// Token: 0x06000102 RID: 258 RVA: 0x0000CD50 File Offset: 0x0000AF50
		public static void InvalidateCache()
		{
			SummonService._cachedFrame = -1;
			SummonService._cachedSummons = null;
		}

		// Token: 0x06000103 RID: 259 RVA: 0x0000CD60 File Offset: 0x0000AF60
		public static bool HasAlivePrimarySummon(PlayerController player)
		{
			bool result;
			try
			{
				BaseUnitController primarySummon = CombatService.GetPrimarySummon(player);
				result = (primarySummon != null && CombatService.IsTargetAlive(primarySummon));
			}
			catch
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000104 RID: 260 RVA: 0x0000CDA0 File Offset: 0x0000AFA0
		private static Il2CppSystem.Collections.Generic.Dictionary<string, int> GetCachedSummons(PlayerController player)
		{
			int frameCount = Time.frameCount;
			if (frameCount == SummonService._cachedFrame && SummonService._cachedSummons != null)
			{
				return SummonService._cachedSummons;
			}
			SummonService._cachedSummons = SummonService.ScanPlayerSummons(player);
			SummonService._cachedFrame = frameCount;
			return SummonService._cachedSummons;
        }

        // Token: 0x06000105 RID: 261 RVA: 0x0000CDE0 File Offset: 0x0000AFE0
        private static Il2CppSystem.Collections.Generic.Dictionary<string, int> ScanPlayerSummons(PlayerController player)
		{
			Il2CppSystem.Collections.Generic.Dictionary<string, int> dictionary = new ();
			try
			{
				BaseUnitController baseUnitController = player.Cast<BaseUnitController>();
				Il2CppArrayBase<MonsterController> il2CppArrayBase = Object.FindObjectsOfType<MonsterController>();
				
				if (il2CppArrayBase == null)
				{
					return dictionary;
				}
				for (int i = 0; i < il2CppArrayBase.Length; i++)
				{
					try
					{
						MonsterController monsterController = il2CppArrayBase[i];
						if (!(monsterController == null))
						{
							SummoningComponent summoning = monsterController.Summoning;
							if (!(summoning == null))
							{
								BaseUnitController summoner = summoning.Summoner;
								if (!(summoner == null))
								{
									if (summoner.GetInstanceID() == baseUnitController.GetInstanceID())
									{
										if (CombatService.IsTargetAlive(monsterController.Cast<BaseUnitController>()))
										{
											string text = monsterController.DisplayName ?? "???";
                                            if (dictionary.ContainsKey(text))
                                            {
                                                Il2CppSystem.Collections.Generic.Dictionary<string, int> dictionary2 = dictionary;
                                                string key = text;
                                                int num = dictionary2[key];
                                                dictionary2[key] = num + 1;
                                            }
                                            else
                                            {
                                                dictionary[text] = 1;
                                            }
                                        }
									}
								}
							}
						}
					}
					catch
					{
					}
				}
            }
			catch
			{
			}
			return dictionary;
		}

        public static int CountAlivePlayerClones(PlayerController player)
        {
            int count = 0;

            try
            {
                if (player == null) return 0;

                BaseUnitController self = player.Cast<BaseUnitController>();
                if (self == null) return 0;

                string playerName = "";
                try { playerName = self.DisplayName ?? ""; } catch { }

                var units = UnityEngine.Object.FindObjectsOfType<BaseUnitController>();
                if (units == null) return 0;

                for (int i = 0; i < units.Length; i++)
                {
                    BaseUnitController unit = units[i];
                    if (unit == null) continue;
                    if (unit.GetInstanceID() == self.GetInstanceID()) continue;
                    if (!CombatService.IsTargetAlive(unit)) continue;

                    bool sameName = false;
                    try
                    {
                        string unitName = unit.DisplayName ?? "";
                        sameName =
                            !string.IsNullOrEmpty(playerName) &&
                            unitName.Equals(playerName, StringComparison.OrdinalIgnoreCase);
                    }
                    catch { }

                    bool ownedByPlayer = false;
                    try
                    {
                        var summoning = unit.Summoning;
                        var summoner = summoning != null ? summoning.Summoner : null;
                        ownedByPlayer =
                            summoner != null &&
                            summoner.GetInstanceID() == self.GetInstanceID();
                    }
                    catch { }

                    bool isClone = false;
                    try
                    {
                        isClone = unit.IsPlayerClone();
                    }
                    catch { }

                    if ((isClone || sameName) && (ownedByPlayer || sameName))
                        count++;
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Warning("[Summon] CountAlivePlayerClones failed: " + ex.Message);
            }

            return count;
        }

        // Token: 0x06000106 RID: 262 RVA: 0x0000CEFC File Offset: 0x0000B0FC
        private static string ExtractMonsterKey(string skillDisplayName)
		{
			if (string.IsNullOrEmpty(skillDisplayName))
			{
				return "";
			}
			if (skillDisplayName.StartsWith("Summon ", StringComparison.OrdinalIgnoreCase))
			{
				return skillDisplayName.Substring("Summon ".Length).Trim();
            }
            if (skillDisplayName.StartsWith("Shadow Seal", StringComparison.OrdinalIgnoreCase))
            {
                return App.Player.DisplayName;
            }
            return skillDisplayName;
		}

		// Token: 0x06000107 RID: 263 RVA: 0x0000CF34 File Offset: 0x0000B134
		private static int CountMatchingSummons(Il2CppSystem.Collections.Generic.Dictionary<string, int> liveSummons, string monsterKey)
		{
			if (string.IsNullOrEmpty(monsterKey))
			{
				return 0;
			}
			int num = 0;
			foreach (Il2CppSystem.Collections.Generic.KeyValuePair<string, int> keyValuePair in liveSummons)
			{
				if (keyValuePair.Key.IndexOf(monsterKey, StringComparison.OrdinalIgnoreCase) >= 0 || monsterKey.IndexOf(keyValuePair.Key, StringComparison.OrdinalIgnoreCase) >= 0)
				{
					num += keyValuePair.Value;
				}
			}
			return num;
		}

		// Token: 0x040000E2 RID: 226
		private static Il2CppSystem.Collections.Generic.Dictionary<string, int> _cachedSummons;

        // Token: 0x040000E3 RID: 227
        private static int _cachedFrame = -1;

		// Token: 0x040000E4 RID: 228
		private static readonly Il2CppSystem.Collections.Generic.List<int> _missingSlots = new Il2CppSystem.Collections.Generic.List<int>();
	}
}
