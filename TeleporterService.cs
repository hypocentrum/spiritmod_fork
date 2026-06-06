using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Il2Cpp;
using Il2CppSystem.Collections.Generic;
using MelonLoader;

namespace SpiritMod
{
	// Token: 0x02000006 RID: 6
	public static class TeleporterService
	{
		// Token: 0x06000012 RID: 18 RVA: 0x0000248C File Offset: 0x0000068C
		public static Il2CppSystem.Collections.Generic.List<string> GetMapNames()
		{
			if (TeleporterService._mapsCached && TeleporterService._mapKeys != null)
			{
				return TeleporterService._mapKeys;
			}
			TeleporterService.CacheMaps();
			return TeleporterService._mapKeys ?? new Il2CppSystem.Collections.Generic.List<string>();
		}

		// Token: 0x06000013 RID: 19 RVA: 0x000024B5 File Offset: 0x000006B5
		public static Il2CppSystem.Collections.Generic.List<string> GetMapDisplayNames()
		{
			if (TeleporterService._mapsCached && TeleporterService._mapDisplayNames != null)
			{
				return TeleporterService._mapDisplayNames;
			}
			TeleporterService.CacheMaps();
			return TeleporterService._mapDisplayNames ?? new Il2CppSystem.Collections.Generic.List<string>();
		}

		// Token: 0x06000014 RID: 20 RVA: 0x000024E0 File Offset: 0x000006E0
		public static string GetMapKeyByIndex(int index)
		{
			Il2CppSystem.Collections.Generic.List<string> mapNames = TeleporterService.GetMapNames();
			if (index >= 0 && index < mapNames.Count)
			{
				return mapNames[index];
			}
			return null;
		}

		// Token: 0x06000015 RID: 21 RVA: 0x0000250C File Offset: 0x0000070C
		public static int GetMapId(string mapKey)
		{
			if (string.IsNullOrEmpty(mapKey))
			{
				return -1;
			}
			Il2CppSystem.Collections.Generic.List<string> mapNames = TeleporterService.GetMapNames();
			if (mapNames == null)
			{
				return -1;
			}
			int num = mapNames.IndexOf(mapKey);
			if (num >= 0)
			{
				return num + 1;
			}
			return -1;
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00002540 File Offset: 0x00000740
		private static void CacheMaps()
		{
			try
			{
				Game game = App.Game;
				if (game == null)
				{
					MelonLogger.Warning("[Teleporter] App.Game is null");
				}
				else
				{
					MapManager map = game.Map;
					if (map == null)
					{
						MelonLogger.Warning("[Teleporter] App.Game.Map is null");
					}
					else
					{
						Il2CppSystem.Collections.Generic.Dictionary<string, Map> mapDictionary = map.MapDictionary;
						if (mapDictionary == null)
						{
							MelonLogger.Warning("[Teleporter] MapDictionary is null");
						}
						else
						{
							TeleporterService._mapKeys = new Il2CppSystem.Collections.Generic.List<string>();
							TeleporterService._mapDisplayNames = new Il2CppSystem.Collections.Generic.List<string>();
							foreach (Il2CppSystem.Collections.Generic.KeyValuePair<string, Map> keyValuePair in mapDictionary)
							{
								string key = keyValuePair.Key;
								if (!string.IsNullOrEmpty(key))
								{
									TeleporterService._mapKeys.Add(key);
									string item = key;
									try
									{
										Map value = keyValuePair.Value;
										if (value != null && value.Config != null)
										{
											string displayName = value.Config.DisplayName;
											if (!string.IsNullOrEmpty(displayName))
											{
												item = displayName;
											}
										}
									}
									catch
									{
									}
									TeleporterService._mapDisplayNames.Add(item);
								}
							}
							TeleporterService._mapsCached = true;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(25, 1);
							defaultInterpolatedStringHandler.AppendLiteral("[Teleporter] Cached ");
							defaultInterpolatedStringHandler.AppendFormatted<int>(TeleporterService._mapKeys.Count);
							defaultInterpolatedStringHandler.AppendLiteral(" maps");
							MelonLogger.Msg(defaultInterpolatedStringHandler.ToStringAndClear());
						}
					}
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Warning("[Teleporter] CacheMaps failed: " + ex.Message);
			}
		}

		// Token: 0x06000017 RID: 23 RVA: 0x000026D8 File Offset: 0x000008D8
		public static void WarpToMap(string mapKey)
		{
			try
			{
				Il2CppSystem.Collections.Generic.List<string> mapNames = TeleporterService.GetMapNames();
				if (mapNames == null || mapNames.Count == 0)
				{
					MelonLogger.Warning("[Teleporter] No maps available");
				}
				else
				{
					int num = mapNames.IndexOf(mapKey);
					if (num < 0)
					{
						MelonLogger.Warning("[Teleporter] Map '" + mapKey + "' not found in dictionary");
					}
					else
					{
						int num2 = num + 1;
						PlayerController player = GameStateService.Player;
						if (player == null)
						{
							MelonLogger.Warning("[Teleporter] Cannot warp: player not found");
						}
						else
						{
							PlayerSave save = player.Save;
							if (save == null)
							{
								MelonLogger.Warning("[Teleporter] Cannot warp: PlayerSave is null");
							}
							else
							{
								save.WarpWaypoint_S(num2);
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(32, 2);
								defaultInterpolatedStringHandler.AppendLiteral("[Teleporter] Warped to '");
								defaultInterpolatedStringHandler.AppendFormatted(mapKey);
								defaultInterpolatedStringHandler.AppendLiteral("' (ID: ");
								defaultInterpolatedStringHandler.AppendFormatted<int>(num2);
								defaultInterpolatedStringHandler.AppendLiteral(")");
								MelonLogger.Msg(defaultInterpolatedStringHandler.ToStringAndClear());
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Warning("[Teleporter] WarpToMap failed: " + ex.Message);
			}
		}

		// Token: 0x06000018 RID: 24 RVA: 0x000027EC File Offset: 0x000009EC
		public static string GetCurrentMapName()
		{
			string result;
			try
			{
				Game game = App.Game;
				if (game == null)
				{
					result = null;
				}
				else
				{
					MapManager map = game.Map;
					if (map == null)
					{
						result = null;
					}
					else
					{
						Map currentMap = map.CurrentMap;
						if (currentMap == null)
						{
							result = null;
						}
						else
						{
							result = currentMap.name;
						}
					}
				}
			}
			catch
			{
				result = null;
			}
			return result;
		}

        // Token: 0x06000019 RID: 25 RVA: 0x00002854 File Offset: 0x00000A54
        public static void InvalidateCache()
		{
			TeleporterService._mapsCached = false;
			TeleporterService._mapKeys = null;
			TeleporterService._mapDisplayNames = null;
		}

		// Token: 0x04000009 RID: 9
		private static Il2CppSystem.Collections.Generic.List<string> _mapKeys;

		// Token: 0x0400000A RID: 10
		private static Il2CppSystem.Collections.Generic.List<string> _mapDisplayNames;

		// Token: 0x0400000B RID: 11
		private static bool _mapsCached;
	}
}
