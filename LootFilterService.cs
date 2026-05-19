using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Il2Cpp;
using Il2CppSystem.Collections.Generic;
using MelonLoader;

namespace SpiritMod
{
    // Token: 0x0200001A RID: 26
    public static class LootFilterService
    {
        // Token: 0x060000C3 RID: 195 RVA: 0x0000A653 File Offset: 0x00008853
        public static void MarkDirty()
        {
            LootFilterService._dirty = true;
            LootFilterService._autosaveTimer = 5f;
        }

        // Token: 0x060000C4 RID: 196 RVA: 0x0000A665 File Offset: 0x00008865
        public static void TickAutosave(float dt)
        {
            if (!LootFilterService._dirty)
            {
                return;
            }
            LootFilterService._autosaveTimer -= dt;
            if (LootFilterService._autosaveTimer > 0f)
            {
                return;
            }
            LootFilterService._dirty = false;
            LootFilterService.Save();
        }

        // Token: 0x17000020 RID: 32
        // (get) Token: 0x060000C5 RID: 197 RVA: 0x0000A693 File Offset: 0x00008893
        public static Il2CppSystem.Collections.Generic.Dictionary<string, LootFilterEntry> Items
        {
            get
            {
                Il2CppSystem.Collections.Generic.Dictionary<string, LootFilterEntry> result = new Il2CppSystem.Collections.Generic.Dictionary<string, LootFilterEntry>();
                foreach(var kvp in LootFilterService._data.Items)
                    result.Add(kvp.Key, kvp.Value);
                return result;
            }
        }

        // Token: 0x17000021 RID: 33
        // (get) Token: 0x060000C6 RID: 198 RVA: 0x0000A69F File Offset: 0x0000889F
        public static int ItemCount
        {
            get
            {
                return LootFilterService._data.Items.Count;
            }
        }

        // Token: 0x17000022 RID: 34
        // (get) Token: 0x060000C7 RID: 199 RVA: 0x0000A6B0 File Offset: 0x000088B0
        // (set) Token: 0x060000C8 RID: 200 RVA: 0x0000A6BC File Offset: 0x000088BC
        public static bool SellFilterEnabled
        {
            get
            {
                return LootFilterService._data.SellFilterEnabled;
            }
            set
            {
                if (LootFilterService._data.SellFilterEnabled != value)
                {
                    LootFilterService._data.SellFilterEnabled = value;
                    LootFilterService.MarkDirty();
                }
            }
        }

        // Token: 0x17000023 RID: 35
        // (get) Token: 0x060000C9 RID: 201 RVA: 0x0000A6DB File Offset: 0x000088DB
        public static bool[] SellRarityEnabled
        {
            get
            {
                return LootFilterService._data.SellRarityEnabled;
            }
        }

        // Token: 0x17000024 RID: 36
        // (get) Token: 0x060000CA RID: 202 RVA: 0x0000A6E7 File Offset: 0x000088E7
        // (set) Token: 0x060000CB RID: 203 RVA: 0x0000A6F3 File Offset: 0x000088F3
        public static bool StatFilterEnabled
        {
            get
            {
                return LootFilterService._data.StatFilterEnabled;
            }
            set
            {
                if (LootFilterService._data.StatFilterEnabled != value)
                {
                    LootFilterService._data.StatFilterEnabled = value;
                    LootFilterService.MarkDirty();
                }
            }
        }

        // Token: 0x17000025 RID: 37
        // (get) Token: 0x060000CC RID: 204 RVA: 0x0000A712 File Offset: 0x00008912
        // (set) Token: 0x060000CD RID: 205 RVA: 0x0000A720 File Offset: 0x00008920
        public static int SecondaryMinMatch
        {
            get
            {
                return LootFilterService._data.SecondaryMinMatch;
            }
            set
            {
                int num = Math.Max(1, value);
                if (LootFilterService._data.SecondaryMinMatch != num)
                {
                    LootFilterService._data.SecondaryMinMatch = num;
                    LootFilterService.MarkDirty();
                }
            }
        }

        // Token: 0x17000026 RID: 38
        // (get) Token: 0x060000CE RID: 206 RVA: 0x0000A752 File Offset: 0x00008952
        public static Il2CppSystem.Collections.Generic.List<StatFilterRule> MainStatRules
        {
            get
            {
                Il2CppSystem.Collections.Generic.List<StatFilterRule> result = new Il2CppSystem.Collections.Generic.List<StatFilterRule>();
                foreach (var kvp in LootFilterService._data.MainStatRules)
                    result.Add(kvp);
                return result;
            }
        }

        // Token: 0x17000027 RID: 39
        // (get) Token: 0x060000CF RID: 207 RVA: 0x0000A75E File Offset: 0x0000895E
        public static Il2CppSystem.Collections.Generic.List<StatFilterRule> SecondaryStatRules
        {
            get
            {
                Il2CppSystem.Collections.Generic.List<StatFilterRule> result = new Il2CppSystem.Collections.Generic.List<StatFilterRule>();
                foreach (var kvp in LootFilterService._data.SecondaryStatRules)
                    result.Add(kvp);
                return result;
            }
        }

        // Token: 0x060000D0 RID: 208 RVA: 0x0000A76C File Offset: 0x0000896C
        public static bool ShouldSell(string name, int rarity)
        {
            if (!LootFilterService._data.SellFilterEnabled)
            {
                return false;
            }
            LootFilterEntry lootFilterEntry;
            if (LootFilterService._data.Items.TryGetValue(name, out lootFilterEntry))
            {
                if (lootFilterEntry.SellOverride == 1)
                {
                    return true;
                }
                if (lootFilterEntry.SellOverride == 2)
                {
                    return false;
                }
            }
            return rarity >= 0 && rarity < LootFilterService._data.SellRarityEnabled.Length && LootFilterService._data.SellRarityEnabled[rarity];
        }

        public static ValueTuple<bool, string> ShouldSellWithReason(string name, int rarity, Il2CppSystem.Collections.Generic.Dictionary<string, int> substats)
        {
            if (!LootFilterService._data.SellFilterEnabled)
            {
                return new ValueTuple<bool, string>(false, "sell-filter-off");
            }
            LootFilterEntry lootFilterEntry;
            if (LootFilterService._data.Items.TryGetValue(name, out lootFilterEntry))
            {
                if (lootFilterEntry.SellOverride == 1)
                {
                    return new ValueTuple<bool, string>(true, "override:AlwaysSell");
                }
                if (lootFilterEntry.SellOverride == 2)
                {
                    return new ValueTuple<bool, string>(false, "override:NeverSell");
                }
            }
            bool flag = false;
            string str = LootFilterService.RarityName(rarity);
            if (rarity >= 0 && rarity < LootFilterService._data.SellRarityEnabled.Length)
            {
                flag = LootFilterService._data.SellRarityEnabled[rarity];
            }
            if (!flag)
            {
                return new ValueTuple<bool, string>(false, "rarity:" + str + "/KEEP");
            }
            if (LootFilterService._data.StatFilterEnabled && substats != null && substats.Count > 0 && LootFilterService.PassesStatFilter(substats))
            {
                string str2 = LootFilterService.FormatSubstats(substats);
                return new ValueTuple<bool, string>(false, "stat-filter:KEEP [" + str2 + "]");
            }
            return new ValueTuple<bool, string>(true, "rarity:" + str + "/SELL");
        }

        // Token: 0x060000D2 RID: 210 RVA: 0x0000A8D0 File Offset: 0x00008AD0
        public static LootFilterEntry FindCatalogEntry(string itemName, out string catalogName)
        {
            catalogName = null;
            LootFilterEntry result;
            if (LootFilterService._data.Items.TryGetValue(itemName, out result))
            {
                catalogName = itemName;
                return result;
            }
            foreach (System.Collections.Generic.KeyValuePair<string, LootFilterEntry> keyValuePair in LootFilterService._data.Items)
            {
                if (keyValuePair.Key.IndexOf(itemName, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    catalogName = keyValuePair.Key;
                    return keyValuePair.Value;
                }
            }
            return null;
        }

        // Token: 0x060000D3 RID: 211 RVA: 0x0000A964 File Offset: 0x00008B64
        public static void RecordItem(string name, int rarity)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }
            LootFilterEntry lootFilterEntry;
            if (LootFilterService._data.Items.TryGetValue(name, out lootFilterEntry))
            {
                LootFilterEntry lootFilterEntry2 = lootFilterEntry;
                int timesSeen = lootFilterEntry2.TimesSeen;
                lootFilterEntry2.TimesSeen = timesSeen + 1;
                if (lootFilterEntry.Rarity == 0 && rarity > 0)
                {
                    lootFilterEntry.Rarity = rarity;
                    return;
                }
            }
            else
            {
                LootFilterService._data.Items[name] = new LootFilterEntry
                {
                    Name = name,
                    Rarity = rarity,
                    TimesSeen = 1
                };
                LootFilterService.MarkDirty();
            }
        }

        // Token: 0x060000D4 RID: 212 RVA: 0x0000A9E4 File Offset: 0x00008BE4
        public static void CycleSellOverride(string name)
        {
            LootFilterEntry lootFilterEntry;
            if (!LootFilterService._data.Items.TryGetValue(name, out lootFilterEntry))
            {
                return;
            }
            LootFilterEntry lootFilterEntry2 = lootFilterEntry;
            int sellOverride;
            switch (lootFilterEntry.SellOverride)
            {
                case 0:
                    sellOverride = 1;
                    break;
                case 1:
                    sellOverride = 2;
                    break;
                case 2:
                    sellOverride = 0;
                    break;
                default:
                    sellOverride = 0;
                    break;
            }
            lootFilterEntry2.SellOverride = sellOverride;
            LootFilterService.MarkDirty();
        }

        // Token: 0x060000D5 RID: 213 RVA: 0x0000AA40 File Offset: 0x00008C40
        public static void ResetAllSellOverrides()
        {
            foreach (System.Collections.Generic.KeyValuePair<string, LootFilterEntry> keyValuePair in LootFilterService._data.Items)
            {
                keyValuePair.Value.SellOverride = 0;
            }
            LootFilterService.MarkDirty();
        }

        // Token: 0x060000D6 RID: 214 RVA: 0x0000AAA4 File Offset: 0x00008CA4
        public static int SellOverrideCount(SellOverride type)
        {
            int num = 0;
            foreach (System.Collections.Generic.KeyValuePair<string, LootFilterEntry> keyValuePair in LootFilterService._data.Items)
            {
                if (keyValuePair.Value.SellOverride == (int)type)
                {
                    num++;
                }
            }
            return num;
        }

        // Token: 0x17000028 RID: 40
        // (get) Token: 0x060000D7 RID: 215 RVA: 0x0000AB0C File Offset: 0x00008D0C
        // (set) Token: 0x060000D8 RID: 216 RVA: 0x0000AB13 File Offset: 0x00008D13
        public static string StatusMessage { get; private set; } = "";

        // Token: 0x060000D9 RID: 217 RVA: 0x0000AB1C File Offset: 0x00008D1C
        public static void Save()
        {
            try
            {
                Directory.CreateDirectory(LootFilterService._directory);
                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                string contents = JsonSerializer.Serialize<LootFilterData>(LootFilterService._data, options);
                File.WriteAllText(LootFilterService._filePath, contents);
                LootFilterService.StatusMessage = "Saved";
                DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(29, 2);
                defaultInterpolatedStringHandler.AppendLiteral("[LootFilter] Saved ");
                defaultInterpolatedStringHandler.AppendFormatted<int>(LootFilterService._data.Items.Count);
                defaultInterpolatedStringHandler.AppendLiteral(" items to ");
                defaultInterpolatedStringHandler.AppendFormatted(LootFilterService._filePath);
                MelonLogger.Msg(defaultInterpolatedStringHandler.ToStringAndClear());
            }
            catch (Exception ex)
            {
                LootFilterService.StatusMessage = "Save failed: " + ex.Message;
                MelonLogger.Warning("[LootFilter] Save failed: " + ex.Message);
            }
        }

        // Token: 0x060000DA RID: 218 RVA: 0x0000ABF0 File Offset: 0x00008DF0
        public static void Load()
        {
            try
            {
                if (!File.Exists(LootFilterService._filePath))
                {
                    LootFilterService.StatusMessage = "No filter file";
                }
                else
                {
                    LootFilterData lootFilterData = JsonSerializer.Deserialize<LootFilterData>(File.ReadAllText(LootFilterService._filePath));
                    if (lootFilterData != null)
                    {
                        LootFilterService._data = lootFilterData;
                        if (LootFilterService._data.SellRarityEnabled == null || LootFilterService._data.SellRarityEnabled.Length < 4)
                        {
                            LootFilterData data = LootFilterService._data;
                            bool[] array = new bool[4];
                            array[0] = true;
                            array[1] = true;
                            data.SellRarityEnabled = array;
                        }
                        if (LootFilterService._data.Items == null)
                        {
                            LootFilterService._data.Items = new System.Collections.Generic.Dictionary<string, LootFilterEntry>();
                        }
                        if (LootFilterService._data.MainStatRules == null)
                        {
                            LootFilterService._data.MainStatRules = new System.Collections.Generic.List<StatFilterRule>();
                        }
                        if (LootFilterService._data.SecondaryStatRules == null)
                        {
                            LootFilterService._data.SecondaryStatRules = new System.Collections.Generic.List<StatFilterRule>();
                        }
                        if (LootFilterService._data.SecondaryMinMatch < 1)
                        {
                            LootFilterService._data.SecondaryMinMatch = 1;
                        }
                        DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(15, 1);
                        defaultInterpolatedStringHandler.AppendLiteral("Loaded (");
                        defaultInterpolatedStringHandler.AppendFormatted<int>(LootFilterService._data.Items.Count);
                        defaultInterpolatedStringHandler.AppendLiteral(" items)");
                        LootFilterService.StatusMessage = defaultInterpolatedStringHandler.ToStringAndClear();
                        defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(32, 2);
                        defaultInterpolatedStringHandler.AppendLiteral("[LootFilter] Loaded ");
                        defaultInterpolatedStringHandler.AppendFormatted<int>(LootFilterService._data.Items.Count);
                        defaultInterpolatedStringHandler.AppendLiteral(" items from ");
                        defaultInterpolatedStringHandler.AppendFormatted(LootFilterService._filePath);
                        MelonLogger.Msg(defaultInterpolatedStringHandler.ToStringAndClear());
                    }
                }
            }
            catch (Exception ex)
            {
                LootFilterService.StatusMessage = "Load failed: " + ex.Message;
                MelonLogger.Warning("[LootFilter] Load failed: " + ex.Message);
            }
        }

        // Token: 0x060000DB RID: 219 RVA: 0x0000ADB0 File Offset: 0x00008FB0
        public static bool PassesStatFilter(Il2CppSystem.Collections.Generic.Dictionary<string, int> substats)
        {
            if (!LootFilterService._data.StatFilterEnabled)
            {
                return true;
            }
            if (substats == null || substats.Count == 0)
            {
                return LootFilterService._data.MainStatRules.Count == 0 && LootFilterService._data.SecondaryStatRules.Count == 0;
            }
            if (LootFilterService._data.MainStatRules.Count > 0)
            {
                bool flag = false;
                foreach (StatFilterRule statFilterRule in LootFilterService._data.MainStatRules)
                {
                    int num;
                    if (substats.TryGetValue(statFilterRule.StatType, out num) && num >= statFilterRule.Value)
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    return false;
                }
            }
            if (LootFilterService._data.SecondaryStatRules.Count > 0)
            {
                int num2 = 0;
                foreach (StatFilterRule statFilterRule2 in LootFilterService._data.SecondaryStatRules)
                {
                    int num3;
                    if (substats.TryGetValue(statFilterRule2.StatType, out num3) && num3 >= statFilterRule2.Value)
                    {
                        num2++;
                    }
                }
                if (num2 < LootFilterService._data.SecondaryMinMatch)
                {
                    return false;
                }
            }
            return true;
        }

        // Token: 0x060000DC RID: 220 RVA: 0x0000AF00 File Offset: 0x00009100
        public static string FormatSubstats(Il2CppSystem.Collections.Generic.Dictionary<string, int> substats)
        {
            if (substats == null || substats.Count == 0)
            {
                return "";
            }
            StringBuilder stringBuilder = new StringBuilder();
            foreach (Il2CppSystem.Collections.Generic.KeyValuePair<string, int> keyValuePair in substats)
            {
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Append(' ');
                }
                stringBuilder.Append(keyValuePair.Key).Append(':').Append(keyValuePair.Value);
            }
            return stringBuilder.ToString();
        }

        // Token: 0x060000DD RID: 221 RVA: 0x0000AF98 File Offset: 0x00009198
        public static void AddMainStatRule()
        {
            LootFilterService._data.MainStatRules.Add(new StatFilterRule
            {
                StatType = LootFilterService.MainStatTypes[0],
                Condition = 0,
                Value = 1
            });
            LootFilterService.MarkDirty();
        }

        // Token: 0x060000DE RID: 222 RVA: 0x0000AFCE File Offset: 0x000091CE
        public static void RemoveMainStatRule(int index)
        {
            if (index >= 0 && index < LootFilterService._data.MainStatRules.Count)
            {
                LootFilterService._data.MainStatRules.RemoveAt(index);
                LootFilterService.MarkDirty();
            }
        }

        // Token: 0x060000DF RID: 223 RVA: 0x0000AFFB File Offset: 0x000091FB
        public static void AddSecondaryStatRule()
        {
            LootFilterService._data.SecondaryStatRules.Add(new StatFilterRule
            {
                StatType = LootFilterService.SecondaryStatTypes[0],
                Condition = 0,
                Value = 1
            });
            LootFilterService.MarkDirty();
        }

        // Token: 0x060000E0 RID: 224 RVA: 0x0000B031 File Offset: 0x00009231
        public static void RemoveSecondaryStatRule(int index)
        {
            if (index >= 0 && index < LootFilterService._data.SecondaryStatRules.Count)
            {
                LootFilterService._data.SecondaryStatRules.RemoveAt(index);
                LootFilterService.MarkDirty();
            }
        }

        // Token: 0x060000E1 RID: 225 RVA: 0x0000B05E File Offset: 0x0000925E
        public static void ClearAllStatRules()
        {
            LootFilterService._data.MainStatRules.Clear();
            LootFilterService._data.SecondaryStatRules.Clear();
            LootFilterService.MarkDirty();
        }

        // Token: 0x060000E2 RID: 226 RVA: 0x0000B084 File Offset: 0x00009284
        public static int FindStatIndex(string[] statTypes, string statType)
        {
            for (int i = 0; i < statTypes.Length; i++)
            {
                if (statTypes[i] == statType)
                {
                    return i;
                }
            }
            return 0;
        }

        // Token: 0x060000E3 RID: 227 RVA: 0x0000B0B0 File Offset: 0x000092B0
        private static string StatTypeToDisplayName(StatType st)
        {
            string result;
            switch (st)
            {
                case 0:
                    result = "STR";
                    break;
                case (StatType)1:
                    result = "VIT";
                    break;
                case (StatType)2:
                    result = "AGI";
                    break;
                case (StatType)3:
                    result = "DEX";
                    break;
                case (StatType)4:
                    result = "INT";
                    break;
                case (StatType)5:
                    result = "LUK";
                    break;
                case (StatType)6:
                    result = "AllStats";
                    break;
                case (StatType)7:
                    result = "HP";
                    break;
                case (StatType)8:
                    result = "MP";
                    break;
                case (StatType)9:
                    result = "Atk";
                    break;
                case (StatType)10:
                    result = "MAtk";
                    break;
                case (StatType)11:
                    result = "Def";
                    break;
                case (StatType)12:
                    result = "MDef";
                    break;
                case (StatType)13:
                    result = "Hit";
                    break;
                case (StatType)14:
                    result = "Flee";
                    break;
                case (StatType)15:
                    result = "Crit";
                    break;
                default:
                    if (st != (StatType)52)
                    {
                        switch (st)
                        {
                            case (StatType)59:
                                return "HPRegen";
                            case (StatType)61:
                                return "MPRegen";
                            case (StatType)63:
                                return "AtkSpd";
                            case (StatType)64:
                                return "CastSpd";
                            case (StatType)65:
                                return "MoveSpd";
                        }
                        result = st.ToString();
                    }
                    else
                    {
                        result = "CritDmg";
                    }
                    break;
            }
            return result;
        }

        // Token: 0x060000E4 RID: 228 RVA: 0x0000B20C File Offset: 0x0000940C
        public static Il2CppSystem.Collections.Generic.Dictionary<string, int> ExtractSubstats(InventoryItemData item)
        {
            if (item == null)
            {
                return null;
            }
            try
            {
                EquipData equipData = item.TryCast<EquipData>();
                if (equipData != null)
                {
                    return LootFilterService.ReadStatValueList(Formula.GetSubstats(equipData));
                }
                ArtifactData artifactData = item.TryCast<ArtifactData>();
                if (artifactData != null)
                {
                    return LootFilterService.ReadStatValueList(Formula.GetSubstats(artifactData));
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Warning("[LootFilter] ExtractSubstats failed: " + ex.Message);
            }
            return null;
        }

        // Token: 0x060000E5 RID: 229 RVA: 0x0000B27C File Offset: 0x0000947C
        private static Il2CppSystem.Collections.Generic.Dictionary<string, int> ReadStatValueList(Il2CppSystem.Collections.Generic.List<StatValue> statList)
        {
            if (statList == null || statList.Count == 0)
            {
                return null;
            }
            Il2CppSystem.Collections.Generic.Dictionary<string, int> dictionary = new Il2CppSystem.Collections.Generic.Dictionary<string, int>();
            for (int i = 0; i < statList.Count; i++)
            {
                try
                {
                    StatValue statValue = statList[i];
                    if (statValue != null)
                    {
                        string key = LootFilterService.StatTypeToDisplayName(statValue.Type);
                        int num = (statValue.Value != null) ? ((int)Math.Round((double)statValue.Value.Value)) : 0;
                        if (num != 0)
                        {
                            dictionary[key] = num;
                        }
                    }
                }
                catch
                {
                }
            }
            if (dictionary.Count <= 0)
            {
                return null;
            }
            return dictionary;
        }

        // Token: 0x060000E6 RID: 230 RVA: 0x0000B314 File Offset: 0x00009514
        public static string RarityName(int rarity)
        {
            string result;
            switch (rarity)
            {
                case 0:
                    result = "COM";
                    break;
                case 1:
                    result = "RAR";
                    break;
                case 2:
                    result = "UNQ";
                    break;
                case 3:
                    result = "LEG";
                    break;
                default:
                    result = "???";
                    break;
            }
            return result;
        }

        // Token: 0x060000E7 RID: 231 RVA: 0x0000B360 File Offset: 0x00009560
        public static string SellOverrideLabel(int ov)
        {
            string result;
            if (ov != 1)
            {
                if (ov != 2)
                {
                    result = "Default";
                }
                else
                {
                    result = "Keep";
                }
            }
            else
            {
                result = "Sell";
            }
            return result;
        }

        // Token: 0x040000CC RID: 204
        private static readonly string _directory = Path.Combine("UserData", "SpiritMod");

        // Token: 0x040000CD RID: 205
        private static readonly string _filePath = Path.Combine(LootFilterService._directory, "lootfilter.json");

        // Token: 0x040000CE RID: 206
        private static LootFilterData _data = new LootFilterData();

        // Token: 0x040000CF RID: 207
        private static bool _dirty;

        // Token: 0x040000D0 RID: 208
        private static float _autosaveTimer;

        // Token: 0x040000D1 RID: 209
        private const float AutosaveDelay = 5f;

        // Token: 0x040000D3 RID: 211
        public static readonly string[] MainStatTypes = new string[]
        {
            "STR",
            "VIT",
            "AGI",
            "DEX",
            "INT",
            "LUK"
        };

        // Token: 0x040000D4 RID: 212
        public static readonly string[] SecondaryStatTypes = new string[]
        {
            "Atk",
            "MAtk",
            "Def",
            "MDef",
            "Hit",
            "Flee",
            "Crit",
            "CritDmg",
            "AtkSpd",
            "CastSpd",
            "HP",
            "MP",
            "HPRegen",
            "MPRegen",
            "FireAtk",
            "IceAtk",
            "LightAtk",
            "DarkAtk",
            "FireDef",
            "IceDef",
            "LightDef",
            "DarkDef",
            "MoveSpd",
            "ExpBonus"
        };
    }
}
