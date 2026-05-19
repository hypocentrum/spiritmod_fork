using System;
using System.Collections.Generic;
using System.IO;
using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace SpiritMod.States
{
	// Token: 0x0200002F RID: 47
	public class LootingState : IBotState
	{
		// Token: 0x17000039 RID: 57
		// (get) Token: 0x06000158 RID: 344 RVA: 0x0000F828 File Offset: 0x0000DA28
		public BotState Id
		{
			get
			{
				return BotState.Looting;
			}
		}

		// Token: 0x06000159 RID: 345 RVA: 0x0000F82B File Offset: 0x0000DA2B
		public void Enter(BotContext ctx)
		{
		}

		// Token: 0x0600015A RID: 346 RVA: 0x0000F830 File Offset: 0x0000DA30
		public void Tick(BotContext ctx)
		{
			ctx.Status.LootPhaseTimer += ctx.DeltaTime;
			if (ctx.Status.LootPhaseTimer > 3f)
			{
				BotController.TransitionTo(BotState.Idle);
				ctx.Status.ActionTimer = 0.2f;
				return;
			}
			ctx.Status.LootTimer -= ctx.DeltaTime;
			if (ctx.Status.LootTimer > 0f)
			{
				return;
			}
			BaseUnitController playerCombatTarget = GameStateService.GetPlayerCombatTarget();
			if (playerCombatTarget != null)
			{
				PlayerController player = ctx.Player;
				if (player != null)
				{
					try
					{
						string text = playerCombatTarget.DisplayName ?? "???";
						CombatService.ClickTarget(player, playerCombatTarget);
						BotController.SetTarget(playerCombatTarget);
						ctx.Status.TargetName = text;
						ctx.Status.AttackTimer = ctx.Config.AttackInterval;
						BotController.TransitionTo(BotState.Combat);
						MelonLogger.Msg("[Bot] Aggroed during looting → COMBAT: \"" + text + "\"");
					}
					catch (Exception ex)
					{
						MelonLogger.Warning("[Bot] Aggro handling failed: " + ex.Message);
					}
				}
				return;
			}
			ctx.Status.LootTimer = ctx.Config.LootDelay;
			GameStateService.TryScanLoot(true);
			List<LootInfo> loot = GameStateService.Loot;
			if (loot.Count == 0)
			{
				BotController.TransitionTo(BotState.Idle);
				ctx.Status.ActionTimer = 0.2f;
				return;
			}
			PlayerController player2 = ctx.Player;
			if (player2 == null)
			{
				return;
			}
			try
			{
				this.PickupClosestLoot(ctx, player2, loot);
			}
			catch (Exception ex2)
			{
				MelonLogger.Warning("[Bot] TickLooting failed: " + ex2.Message);
			}
		}

		// Token: 0x0600015B RID: 347 RVA: 0x0000F9DC File Offset: 0x0000DBDC
		public void Exit(BotContext ctx)
		{
		}

		// Token: 0x0600015C RID: 348 RVA: 0x0000F9E0 File Offset: 0x0000DBE0
		private void PickupClosestLoot(BotContext ctx, PlayerController player, List<LootInfo> loot)
		{
			Vector3 position = player.Cast<BaseUnitController>().Position;
			LootDrop lootDrop = null;
			float num = ctx.Config.LootRange * ctx.Config.LootRange;
			string itemName = null;
			int rarity = 0;
			Vector3 position2 = default(Vector3);
			foreach (LootInfo lootInfo in loot)
			{
				if (!(lootInfo.Drop == null))
				{
					float sqrMagnitude = (position - lootInfo.Position).sqrMagnitude;
					if (sqrMagnitude < num)
					{
						num = sqrMagnitude;
						lootDrop = lootInfo.Drop;
						itemName = lootInfo.Name;
						rarity = lootInfo.Rarity;
						position2 = lootInfo.Position;
					}
				}
			}
			if (lootDrop == null)
			{
				BotController.TransitionTo(BotState.Idle);
				ctx.Status.ActionTimer = 0.2f;
				return;
			}
			if (Mathf.Sqrt(num) < 5f)
			{
				BotController.SimulatePickup();
				ctx.Status.LootCount++;
				LootingState._lootLog.Append("LOOTED", itemName, 1, "Drop", rarity, null, null);
				return;
			}
			CombatService.ClickPosition(player, position2);
		}

		// Token: 0x0400011F RID: 287
		private static readonly RollingLog _lootLog = new RollingLog(Path.Combine("UserData", "SpiritMod"), "lootlog.txt", "Bot", 524288L);
	}
}
