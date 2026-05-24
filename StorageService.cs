using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Il2Cpp;
using Il2CppSystem.Collections.Generic;
using MelonLoader;

namespace SpiritMod
{
	// Token: 0x0200001C RID: 28
	public static class StorageService
	{
		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060000EF RID: 239 RVA: 0x0000BD4D File Offset: 0x00009F4D
		// (set) Token: 0x060000F0 RID: 240 RVA: 0x0000BD54 File Offset: 0x00009F54
		public static string StatusMessage { get; private set; } = string.Empty;

		// Token: 0x060000F1 RID: 241 RVA: 0x0000BD5C File Offset: 0x00009F5C
		public static int StoreFilteredItems()
		{
			int result;
			try
			{
				PlayerController playerController = GameStateService.Player ?? App.Player;
				if (playerController == null)
				{
					StorageService.StatusMessage = "No player";
					MelonLogger.Warning("[Storage] StoreFilteredItems: player not found");
					result = 0;
				}
				else
				{
					PlayerSave save = playerController.Save;
					if (save == null)
					{
						StorageService.StatusMessage = "No PlayerSave";
						MelonLogger.Warning("[Storage] StoreFilteredItems: PlayerSave is null");
						result = 0;
					}
					else
					{
						CharacterData data = save.Data;
						if (data == null)
						{
							StorageService.StatusMessage = "No CharacterData";
							MelonLogger.Warning("[Storage] StoreFilteredItems: CharacterData is null");
							result = 0;
						}
						else
						{
							InventoryData inventoryDict = data.InventoryDict;
							if (inventoryDict == null)
							{
								StorageService.StatusMessage = "No Inventory";
								MelonLogger.Warning("[Storage] StoreFilteredItems: InventoryDict is null");
								result = 0;
							}
							else if (!LootFilterService.SellFilterEnabled)
							{
								StorageService.StatusMessage = "Sell filter disabled";
								MelonLogger.Msg("[Storage] Sell filter is disabled — nothing to store");
								result = 0;
							}
							else
							{
								try
								{
									PlayerData playerData = save.PlayerData;
									if (playerData == null)
									{
										StorageService.StatusMessage = "No PlayerData";
										MelonLogger.Warning("[Storage] StoreFilteredItems: PlayerData is null");
										return 0;
									}
									InventoryData storageDict = playerData.StorageDict;
									if (storageDict != null)
									{
										int weightCount = Formula.GetWeightCount(storageDict);
										DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 1);
										defaultInterpolatedStringHandler.AppendLiteral("[Storage] Current storage weight: ");
										defaultInterpolatedStringHandler.AppendFormatted<int>(weightCount);
										MelonLogger.Msg(defaultInterpolatedStringHandler.ToStringAndClear());
									}
								}
								catch (Exception ex)
								{
									MelonLogger.Warning("[Storage] Storage capacity check failed: " + ex.Message);
								}
								Transaction transaction = new Transaction();
								int num = 0;
								StorageService._unknownLogCount = 0;
								num += StorageService.ProcessDict<ConsumableData>(transaction, inventoryDict.Consumables, "Consumable");
								num += StorageService.ProcessDict<EquipData>(transaction, inventoryDict.Equips, "Equip");
								num += StorageService.ProcessDict<CardData>(transaction, inventoryDict.Cards, "Card");
								num += StorageService.ProcessDict<ArtifactData>(transaction, inventoryDict.Artifacts, "Artifact");
								num += StorageService.ProcessDict<GemData>(transaction, inventoryDict.Gems, "Gem");
								num += StorageService.ProcessDict<JunkData>(transaction, inventoryDict.Junks, "Junk");
								if (num == 0)
								{
									StorageService.StatusMessage = "Nothing to store";
									MelonLogger.Msg("[Storage] No items match store criteria");
									result = 0;
								}
								else if (transaction.IsEmpty)
								{
									StorageService.StatusMessage = "Empty transaction";
									MelonLogger.Warning("[Storage] Transaction was empty after building");
									result = 0;
								}
								else
								{
									save.StorageTransaction(transaction, null);
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(14, 1);
									defaultInterpolatedStringHandler.AppendLiteral("Stored ");
									defaultInterpolatedStringHandler.AppendFormatted<int>(num);
									defaultInterpolatedStringHandler.AppendLiteral(" stacks");
									StorageService.StatusMessage = defaultInterpolatedStringHandler.ToStringAndClear();
									defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(56, 1);
									defaultInterpolatedStringHandler.AppendLiteral("[Storage] Stored ");
									defaultInterpolatedStringHandler.AppendFormatted<int>(num);
									defaultInterpolatedStringHandler.AppendLiteral(" item stacks via StorageTransaction RPC");
									MelonLogger.Msg(defaultInterpolatedStringHandler.ToStringAndClear());
									result = num;
								}
							}
						}
					}
				}
			}
			catch (Exception ex2)
			{
				StorageService.StatusMessage = "Error: " + ex2.Message;
				MelonLogger.Warning("[Storage] StoreFilteredItems failed: " + ex2.Message);
				result = 0;
			}
			return result;
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x0000C084 File Offset: 0x0000A284
		private static int ProcessDict<T>(Transaction transaction, Il2CppSystem.Collections.Generic.Dictionary<string, T> items, string category) where T : InventoryItemData
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
								string itemName = StorageService.GetItemName(t);
								if (!string.IsNullOrEmpty(itemName))
								{
									string text;
									LootFilterEntry lootFilterEntry = LootFilterService.FindCatalogEntry(itemName, out text);
									if (lootFilterEntry == null)
									{
										if (StorageService._unknownLogCount < 5)
										{
											DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(49, 2);
											defaultInterpolatedStringHandler.AppendLiteral("[Storage] SKIP unknown item not in catalog: '");
											defaultInterpolatedStringHandler.AppendFormatted(itemName);
											defaultInterpolatedStringHandler.AppendLiteral("' (");
											defaultInterpolatedStringHandler.AppendFormatted(category);
											defaultInterpolatedStringHandler.AppendLiteral(")");
											MelonLogger.Msg(defaultInterpolatedStringHandler.ToStringAndClear());
											StorageService._unknownLogCount++;
										}
									}
									else
									{
										int rarity = lootFilterEntry.Rarity;
                                        System.Collections.Generic.Dictionary<string, int> substats = LootFilterService.ExtractSubstats(t);
										string statsInfo = LootFilterService.FormatSubstats(substats);
										ValueTuple<bool, string> valueTuple = LootFilterService.ShouldSellWithReason(text, rarity, substats);
										bool item = valueTuple.Item1;
										string item2 = valueTuple.Item2;
										if (item)
										{
											StorageService._storageLog.Append("SKIP-SELL", text, 1, category, rarity, statsInfo, item2);
										}
										else
										{
											int count = InventoryExtensions.GetCount(t);
											if (count > 0)
											{
												ItemType itemType = InventoryExtensions.GetItemType(t);
												string instanceId = t.GetInstanceId();
												transaction.Add(instanceId, itemType, count, 0);
												num++;
												DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(36, 5);
												defaultInterpolatedStringHandler.AppendLiteral("[Storage] Storing: '");
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
												StorageService._storageLog.Append("STORED", text, count, category, rarity, statsInfo, item2);
											}
										}
									}
								}
							}
						}
					}
					catch (Exception ex)
					{
						MelonLogger.Warning("[Storage] Error processing " + category + " item: " + ex.Message);
					}
				}
			}
			catch (Exception ex2)
			{
				MelonLogger.Warning("[Storage] Error iterating " + category + " dict: " + ex2.Message);
			}
			return num;
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x0000C3B4 File Offset: 0x0000A5B4
		private static string GetItemName(InventoryItemData item)
		{
			try
			{
				string id = item.Id;
				if (string.IsNullOrEmpty(id))
				{
					return null;
				}
				GameServerRuntime serverRuntime = App.ServerRuntime;
				if (serverRuntime == null)
				{
					return id;
				}
				try
				{
					if (item.TryCast<EquipData>() != null)
					{
						EquipConfig equip = serverRuntime.GetEquip(id);
						if (equip != null)
						{
							string displayName = equip.DisplayName;
							if (!string.IsNullOrEmpty(displayName))
							{
								return displayName;
							}
						}
						return id;
					}
				}
				catch
				{
				}
				try
				{
					if (item.TryCast<CardData>() != null)
					{
						CardConfig card = serverRuntime.GetCard(id);
						if (card != null)
						{
							string displayName2 = card.DisplayName;
							if (!string.IsNullOrEmpty(displayName2))
							{
								return displayName2;
							}
						}
						return id;
					}
				}
				catch
				{
				}
				try
				{
					if (item.TryCast<ArtifactData>() != null)
					{
						ArtifactSetConfig artifactSet = serverRuntime.GetArtifactSet(id);
						if (artifactSet != null)
						{
							string displayName3 = artifactSet.DisplayName;
							if (!string.IsNullOrEmpty(displayName3))
							{
								return displayName3;
							}
						}
						return id;
					}
				}
				catch
				{
				}
				try
				{
					if (item.TryCast<ConsumableData>() != null)
					{
						ConsumableConfig consumable = serverRuntime.GetConsumable(id);
						if (consumable != null)
						{
							string displayName4 = consumable.DisplayName;
							if (!string.IsNullOrEmpty(displayName4))
							{
								return displayName4;
							}
						}
						return id;
					}
				}
				catch
				{
				}
				try
				{
					if (item.TryCast<GemData>() != null)
					{
						GemConfig gem = serverRuntime.GetGem(id);
						if (gem != null)
						{
							string displayName5 = gem.DisplayName;
							if (!string.IsNullOrEmpty(displayName5))
							{
								return displayName5;
							}
						}
						return id;
					}
				}
				catch
				{
				}
				try
				{
					if (item.TryCast<JunkData>() != null)
					{
						JunkConfig junk = serverRuntime.GetJunk(id);
						if (junk != null)
						{
							string displayName6 = junk.DisplayName;
							if (!string.IsNullOrEmpty(displayName6))
							{
								return displayName6;
							}
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
			return null;
		}

		// Token: 0x040000D8 RID: 216
		private static readonly RollingLog _storageLog = new RollingLog(Path.Combine("UserData", "SpiritMod"), "storagelog.txt", "Storage", 524288L);

		// Token: 0x040000DA RID: 218
		private static int _unknownLogCount;
	}
}
