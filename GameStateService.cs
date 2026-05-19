using System;
using System.Collections.Generic;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using MelonLoader;
using UnityEngine;

namespace SpiritMod
{
	// Token: 0x02000007 RID: 7
	public static class GameStateService
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x0600001A RID: 26 RVA: 0x00002868 File Offset: 0x00000A68
		public static PlayerController Player
		{
			get
			{
				PlayerController result;
				try
				{
					if (GameCache.PlayerController.IsValid())
					{
						result = GameCache.PlayerController.Value;
					}
					else
					{
						Game game = App.Game;
						result = ((game != null) ? game.Player : null);
					}
				}
				catch
				{
					result = null;
				}
				return result;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600001B RID: 27 RVA: 0x000028B8 File Offset: 0x00000AB8
		public static List<MonsterInfo> Monsters
		{
			get
			{
				return GameStateService._monsters;
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600001C RID: 28 RVA: 0x000028BF File Offset: 0x00000ABF
		public static List<LootInfo> Loot
		{
			get
			{
				return GameStateService._loot;
			}
		}

		// Token: 0x0600001D RID: 29 RVA: 0x000028C8 File Offset: 0x00000AC8
		public static void TryScanMonsters()
		{
			GameStateService._monsters.Clear();
			try
			{
				Il2CppArrayBase<MonsterController> il2CppArrayBase = UnityEngine.Object.FindObjectsOfType<MonsterController>();
				if (il2CppArrayBase != null)
				{
					for (int i = 0; i < il2CppArrayBase.Length; i++)
					{
						MonsterController monsterController = il2CppArrayBase[i];
						if (!(monsterController == null))
						{
							SummoningComponent summoning = monsterController.Summoning;
							if (!(summoning != null) || !(summoning.Summoner != null))
							{
								HealthComponent health = monsterController.Health;
								if (!(health == null) && health.IsAlive)
								{
									GameStateService._monsters.Add(new MonsterInfo
									{
										Controller = monsterController,
										Name = (monsterController.DisplayName ?? "???"),
										Position = monsterController.Position
									});
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Warning("[GameState] TryScanMonsters failed: " + ex.Message);
			}
		}

		// Token: 0x0600001E RID: 30 RVA: 0x000029B4 File Offset: 0x00000BB4
		public static void TryScanLoot(bool recordCatalog = true)
		{
			GameStateService._loot.Clear();
			try
			{
				Il2CppArrayBase<LootDrop> il2CppArrayBase = UnityEngine.Object.FindObjectsOfType<LootDrop>();
				if (il2CppArrayBase != null)
				{
					for (int i = 0; i < il2CppArrayBase.Length; i++)
					{
						LootDrop lootDrop = il2CppArrayBase[i];
						if (!(lootDrop == null))
						{
							int rarity = 0;
							try
							{
								rarity = (int)lootDrop.Dto.Value.Rarity;
							}
							catch
							{
							}
							string name = lootDrop.DisplayName ?? "loot";
							GameStateService._loot.Add(new LootInfo
							{
								Drop = lootDrop,
								Name = name,
								Position = lootDrop.transform.position,
								Rarity = rarity
							});
							if (recordCatalog)
							{
								LootFilterService.RecordItem(name, rarity);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Warning("[GameState] TryScanLoot failed: " + ex.Message);
			}
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002AA0 File Offset: 0x00000CA0
		public static BaseUnitController GetPlayerCombatTarget()
		{
			PlayerController player = GameStateService.Player;
			if (player == null)
			{
				return null;
			}
			BaseUnitController result;
			try
			{
				CombatComponent combat = player.Combat;
				result = ((combat != null) ? combat.Target : null);
			}
			catch
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002AEC File Offset: 0x00000CEC
		public static void InvalidateAll()
		{
			GameStateService._monsters.Clear();
			GameStateService._loot.Clear();
		}

		// Token: 0x0400000C RID: 12
		private static List<MonsterInfo> _monsters = new List<MonsterInfo>();

		// Token: 0x0400000D RID: 13
		private static List<LootInfo> _loot = new List<LootInfo>();
	}
}
