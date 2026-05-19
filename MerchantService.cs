using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppSystem.Collections.Generic;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace SpiritMod
{
    public static class MerchantService
    {
        private static readonly RollingLog _saleLog = new RollingLog(Path.Combine("UserData", "SpiritMod"), "salelog.txt", "Merchant");
        private static int _unknownLogCount;

        public static string StatusMessage { get; private set; } = string.Empty;

        public static int SellFilteredItems()
        {
            try
            {
                PlayerController playerController = GameStateService.Player ?? App.Player;
                if (playerController == null)
                {
                    MerchantService.StatusMessage = "No player";
                    MelonLogger.Warning("[Merchant] SellFilteredItems: player not found");
                    return 0;
                }
                PlayerSave save = playerController.Save;
                if (save == null)
                {
                    MerchantService.StatusMessage = "No PlayerSave";
                    MelonLogger.Warning("[Merchant] SellFilteredItems: PlayerSave is null");
                    return 0;
                }
                CharacterData data = save.Data;
                if (data == null)
                {
                    MerchantService.StatusMessage = "No CharacterData";
                    MelonLogger.Warning("[Merchant] SellFilteredItems: CharacterData is null");
                    return 0;
                }
                InventoryData inventoryDict = data.InventoryDict;
                if (inventoryDict == null)
                {
                    MerchantService.StatusMessage = "No Inventory";
                    MelonLogger.Warning("[Merchant] SellFilteredItems: InventoryDict is null");
                    return 0;
                }
                if (!LootFilterService.SellFilterEnabled)
                {
                    MerchantService.StatusMessage = "Sell filter disabled";
                    MelonLogger.Msg("[Merchant] Sell filter is disabled — nothing to sell");
                    return 0;
                }
                TransactionInventory transaction1 = new TransactionInventory();
                int num1 = 0;
                MerchantService._unknownLogCount = 0;
                int num2 = num1 + MerchantService.ProcessDict<ConsumableData>(transaction1, inventoryDict.Consumables, "Consumable") + MerchantService.ProcessDict<EquipData>(transaction1, inventoryDict.Equips, "Equip") + MerchantService.ProcessDict<CardData>(transaction1, inventoryDict.Cards, "Card") + MerchantService.ProcessDict<ArtifactData>(transaction1, inventoryDict.Artifacts, "Artifact") + MerchantService.ProcessDict<GemData>(transaction1, inventoryDict.Gems, "Gem") + MerchantService.ProcessDict<JunkData>(transaction1, inventoryDict.Junks, "Junk");
                if (num2 == 0)
                {
                    MerchantService.StatusMessage = "Nothing to sell";
                    MelonLogger.Msg("[Merchant] No items match sell criteria");
                    return 0;
                }
                Transaction transaction2 = transaction1.GetTransaction();
                if (transaction2 == null || transaction2.IsEmpty)
                {
                    MerchantService.StatusMessage = "Empty transaction";
                    MelonLogger.Warning("[Merchant] Transaction was empty after building");
                    return 0;
                }
                save.MerchantSell(transaction2, (Action<CharacterData>)null);
                MerchantService.StatusMessage = $"Sold {num2} stacks";
                MelonLogger.Msg($"[Merchant] Sold {num2} item stacks via MerchantSell RPC");
                return num2;
            }
            catch (Exception ex)
            {
                MerchantService.StatusMessage = "Error: " + ex.Message;
                MelonLogger.Warning("[Merchant] SellFilteredItems failed: " + ex.Message);
                return 0;
            }
        }

        private static int ProcessDict<T>(
          TransactionInventory transaction,
          Il2CppSystem.Collections.Generic.Dictionary<string, T> items,
          string category)
          where T : InventoryItemData
        {
            if (items == null)
            {
                return 0;
            }
            int num = 0;
            try
            {
                Il2CppSystem.Collections.Generic.List<T> list = new Il2CppSystem.Collections.Generic.List<T>();
                foreach (Il2CppSystem.Collections.Generic.KeyValuePair<string, T> keyValuePair in items)
                {
                    list.Add(keyValuePair.Value);
                }
                foreach (T t in list)
                {
                    try
                    {
                        if (t != null)
                        {
                            if (!t.Favorite)
                            {
                                try
                                {
                                    GameServerRuntime serverRuntime = App.ServerRuntime;
                                    if (serverRuntime != null && serverRuntime.IsCharacterBound(t.Id))
                                    {
                                        continue;
                                    }
                                }
                                catch
                                {
                                }
                                string itemName = MerchantService.GetItemName(t);
                                if (!string.IsNullOrEmpty(itemName))
                                {
                                    string text;
                                    LootFilterEntry lootFilterEntry = LootFilterService.FindCatalogEntry(itemName, out text);
                                    if (lootFilterEntry == null)
                                    {
                                        if (MerchantService._unknownLogCount < 5)
                                        {
                                            DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(50, 2);
                                            defaultInterpolatedStringHandler.AppendLiteral("[Merchant] SKIP unknown item not in catalog: '");
                                            defaultInterpolatedStringHandler.AppendFormatted(itemName);
                                            defaultInterpolatedStringHandler.AppendLiteral("' (");
                                            defaultInterpolatedStringHandler.AppendFormatted(category);
                                            defaultInterpolatedStringHandler.AppendLiteral(")");
                                            MelonLogger.Msg(defaultInterpolatedStringHandler.ToStringAndClear());
                                            MerchantService._unknownLogCount++;
                                        }
                                    }
                                    else
                                    {
                                        int rarity = lootFilterEntry.Rarity;
                                        Il2CppSystem.Collections.Generic.Dictionary<string, int> substats = LootFilterService.ExtractSubstats(t);
                                        string statsInfo = LootFilterService.FormatSubstats(substats);
                                        ValueTuple<bool, string> valueTuple = LootFilterService.ShouldSellWithReason(text, rarity, substats);
                                        bool item = valueTuple.Item1;
                                        string item2 = valueTuple.Item2;
                                        if (!item)
                                        {
                                            MerchantService._saleLog.Append("KEPT", text, 1, category, rarity, statsInfo, item2);
                                        }
                                        else
                                        {
                                            int count = InventoryExtensions.GetCount(t);
                                            if (count > 0)
                                            {
                                                transaction.Add(t, count, 0);
                                                num++;
                                                DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(37, 5);
                                                defaultInterpolatedStringHandler.AppendLiteral("[Merchant] Selling: '");
                                                defaultInterpolatedStringHandler.AppendFormatted(text);
                                                defaultInterpolatedStringHandler.AppendLiteral("' x");
                                                defaultInterpolatedStringHandler.AppendFormatted<int>(count);
                                                defaultInterpolatedStringHandler.AppendLiteral(" (");
                                                defaultInterpolatedStringHandler.AppendFormatted(category);
                                                defaultInterpolatedStringHandler.AppendLiteral(", rarity=");
                                                defaultInterpolatedStringHandler.AppendFormatted<int>(rarity);
                                                defaultInterpolatedStringHandler.AppendLiteral(") ");
                                                defaultInterpolatedStringHandler.AppendFormatted(item2);
                                                MelonLogger.Msg(defaultInterpolatedStringHandler.ToStringAndClear());
                                                MerchantService._saleLog.Append("SOLD", text, count, category, rarity, statsInfo, item2);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MelonLogger.Warning("[Merchant] Error processing " + category + " item: " + ex.Message);
                    }
                }
            }
            catch (Exception ex2)
            {
                MelonLogger.Warning("[Merchant] Error iterating " + category + " dict: " + ex2.Message);
            }
            return num;
        }

        private static string GetItemName(InventoryItemData item)
        {
            try
            {
                string id = item.Id;
                if (string.IsNullOrEmpty(id))
                    return (string)null;
                GameServerRuntime serverRuntime = App.ServerRuntime;
                if (serverRuntime == null)
                    return id;
                try
                {
                    if (((Il2CppObjectBase)item).TryCast<EquipData>() != null)
                    {
                        EquipConfig equip = serverRuntime.GetEquip(id);
                        if (equip == null)
                        {
                            string displayName = ((BaseConfig)equip).DisplayName;
                            if (!string.IsNullOrEmpty(displayName))
                                return displayName;
                        }
                        return id;
                    }
                }
                catch
                {
                }
                try
                {
                    if (((Il2CppObjectBase)item).TryCast<CardData>() != null)
                    {
                        CardConfig card = serverRuntime.GetCard(id);
                        if (card == null)
                        {
                            string displayName = ((BaseConfig)card).DisplayName;
                            if (!string.IsNullOrEmpty(displayName))
                                return displayName;
                        }
                        return id;
                    }
                }
                catch
                {
                }
                try
                {
                    if (((Il2CppObjectBase)item).TryCast<ArtifactData>() != null)
                    {
                        ArtifactSetConfig artifactSet = serverRuntime.GetArtifactSet(id);
                        if (artifactSet == null)
                        {
                            string displayName = ((BaseConfig)artifactSet).DisplayName;
                            if (!string.IsNullOrEmpty(displayName))
                                return displayName;
                        }
                        return id;
                    }
                }
                catch
                {
                }
                try
                {
                    if (((Il2CppObjectBase)item).TryCast<ConsumableData>() != null)
                    {
                        ConsumableConfig consumable = serverRuntime.GetConsumable(id);
                        if (consumable == null)
                        {
                            string displayName = ((BaseConfig)consumable).DisplayName;
                            if (!string.IsNullOrEmpty(displayName))
                                return displayName;
                        }
                        return id;
                    }
                }
                catch
                {
                }
                try
                {
                    if (((Il2CppObjectBase)item).TryCast<GemData>() != null)
                    {
                        GemConfig gem = serverRuntime.GetGem(id);
                        if (gem == null)
                        {
                            string displayName = ((BaseConfig)gem).DisplayName;
                            if (!string.IsNullOrEmpty(displayName))
                                return displayName;
                        }
                        return id;
                    }
                }
                catch
                {
                }
                try
                {
                    if (((Il2CppObjectBase)item).TryCast<JunkData>() != null)
                    {
                        JunkConfig junk = serverRuntime.GetJunk(id);
                        if (junk == null)
                        {
                            string displayName = ((BaseConfig)junk).DisplayName;
                            if (!string.IsNullOrEmpty(displayName))
                                return displayName;
                        }
                        return id;
                    }
                }
                catch
                {
                }
                return id;
            }
            catch
            {
            }
            return (string)null;
        }
    }
}
