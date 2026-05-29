using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Il2Cpp;
using MelonLoader;
using SpiritMod.Guards;
using SpiritMod.States;

namespace SpiritMod
{
	// Token: 0x02000010 RID: 16
	public static class BotController
	{
		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000065 RID: 101 RVA: 0x00004A24 File Offset: 0x00002C24
		public static BotConfig Config
		{
			get
			{
				return BotController._config;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000066 RID: 102 RVA: 0x00004A2B File Offset: 0x00002C2B
		public static BotStatus Status
		{
			get
			{
				return BotController._status;
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000067 RID: 103 RVA: 0x00004A32 File Offset: 0x00002C32
		public static BaseUnitController CurrentTarget
		{
			get
			{
				return BotController._currentTarget;
			}
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00004A3C File Offset: 0x00002C3C
		public static void Enable()
		{
			if (BotController._status.State != BotState.Disabled)
			{
				return;
			}
			if (GameStateService.Monsters.Count == 0)
			{
				GameStateService.TryScanMonsters();
			}
			BotController._currentTarget = null;
			BotController._status.Reset();
			StuckRecoveryService.Reset();
			BotController.TransitionTo(BotState.Idle);
			BotController._status.ActionTimer = 0f;
			MelonLogger.Msg("[Bot] Enabled");
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00004A9B File Offset: 0x00002C9B
		public static void Disable()
		{
			BotController._simulatePickup = false;
			BotController._currentTarget = null;
			BotController._activeState = null;
			BotController._status.Reset();
			StuckRecoveryService.Reset();
			MelonLogger.Msg("[Bot] Disabled");
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600006A RID: 106 RVA: 0x00004AC8 File Offset: 0x00002CC8
		public static bool IsEnabled
		{
			get
			{
				return BotController._status.State > BotState.Disabled;
			}
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00004AD7 File Offset: 0x00002CD7
		public static void Toggle()
		{
			if (BotController.IsEnabled)
			{
				BotController.Disable();
				return;
			}
			BotController.Enable();
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00004AEB File Offset: 0x00002CEB
		public static void SetTarget(BaseUnitController target)
		{
			BotController._currentTarget = target;
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00004AF3 File Offset: 0x00002CF3
		public static void ClearTarget()
		{
			BotController._currentTarget = null;
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00004AFB File Offset: 0x00002CFB
		public static void SimulatePickup()
		{
			BotController._simulatePickup = true;
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00004B04 File Offset: 0x00002D04
		public static void TransitionTo(BotState newState)
		{
			if (BotController._ctx != null)
			{
				IBotState activeState = BotController._activeState;
				if (activeState != null)
				{
					activeState.Exit(BotController._ctx);
				}
			}
			BotController._status.State = newState;
			IBotState activeState2;
			if (BotController._states.TryGetValue(newState, out activeState2))
			{
				BotController._activeState = activeState2;
				if (BotController._ctx != null)
				{
					BotController._activeState.Enter(BotController._ctx);
					return;
				}
			}
			else
			{
				BotController._activeState = null;
			}
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00004B6C File Offset: 0x00002D6C
		public static bool TryTickSummons(float deltaTime, string context)
		{
			if (!BotController._config.EnableSkills)
			{
				return true;
			}
			PlayerController player = GameStateService.Player;
			if (player == null)
			{
				return true;
			}
			if (SummonService.AllSummonsSatisfied(player, BotController._config))
			{
				return true;
			}
			BotController._status.SkillTimer -= deltaTime;
			if (BotController._status.SkillTimer <= 0f)
			{
				BotController._status.SkillTimer = BotController._config.SkillInterval;
				try
				{
					if (CombatService.TryCastSkill(player, BotController._config, BotController._status))
					{
						Il2CppSystem.Collections.Generic.List<int> missingSummonSlots = SummonService.GetMissingSummonSlots(player, BotController._config);
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(35, 2);
						defaultInterpolatedStringHandler.AppendLiteral("[Bot] ");
						defaultInterpolatedStringHandler.AppendFormatted(context);
						defaultInterpolatedStringHandler.AppendLiteral(": summoning (missing slots: ");
						defaultInterpolatedStringHandler.AppendFormatted(string.Join(",", missingSummonSlots));
						defaultInterpolatedStringHandler.AppendLiteral(")");
						MelonLogger.Msg(defaultInterpolatedStringHandler.ToStringAndClear());
					}
				}
				catch (Exception ex)
				{
					MelonLogger.Warning("[Bot] " + context + ": summon failed: " + ex.Message);
				}
			}
			return false;
		}

        // Token: 0x06000071 RID: 113 RVA: 0x00004C84 File Offset: 0x00002E84
        private static float _missingPlayerTimer;

        public static void Tick(float deltaTime)
        {
            if (_status.State == BotState.Disabled)
                return;

            var player = GameStateService.Player;

            if (player == null)
            {
                _missingPlayerTimer += deltaTime;

                if (_missingPlayerTimer > 2f)
                {
                    _simulatePickup = false;
                    _currentTarget = null;
                    _activeState = null;
                    GameStateService.InvalidateAll();
                    GameCache.InvalidateAll();

                    // keep bot enabled, but reset transient state
                    _status.TargetName = string.Empty;
                    _status.TargetHealth = 0;
                    _status.TargetMaxHp = 0;
                    _status.ActionTimer = 1f;

                    // optional: prevent UI showing active farming state
                    _status.State = BotState.Idle;
                }

                return;
            }

            _missingPlayerTimer = 0f;

            _simulatePickup = false;
            _config.EnsureArrays();

            _ctx = new BotContext
            {
                Player = player,
                Config = _config,
                Status = _status,
                DeltaTime = deltaTime
            };

            if (_guards.Evaluate(_ctx))
            {
                _status.TickBenchmark(deltaTime);
                return;
            }

            bool skipStuckRecovery =
    _status.State == BotState.Combat &&
    CombatService.IsTargetAlive(_currentTarget);

            if (skipStuckRecovery)
            {
                // Important: prevent the old countdown from firing immediately after combat.
                StuckRecoveryService.Reset();
            }
            else if (StuckRecoveryService.Tick(_ctx))
            {
                _status.TickBenchmark(deltaTime);
                return;
            }

            // then keep the existing line after it:
            _activeState?.Tick(_ctx);
            _status.TickBenchmark(deltaTime);
        }

        // Token: 0x04000082 RID: 130
        internal static bool _simulatePickup;

		// Token: 0x04000083 RID: 131
		private static readonly BotConfig _config = new BotConfig();

		// Token: 0x04000084 RID: 132
		private static readonly BotStatus _status = new BotStatus();

		// Token: 0x04000085 RID: 133
		private static BaseUnitController _currentTarget;

		// Token: 0x04000086 RID: 134
		private static readonly GuardPipeline _guards = new GuardPipeline().Add(new DeathGuard()).Add(new ZoneGuard()).Add(new OverweightGuard());

		// Token: 0x04000087 RID: 135
		private static readonly Dictionary<BotState, IBotState> _states = new Dictionary<BotState, IBotState>
		{
			{
				BotState.Idle,
				new IdleState()
			},
			{
				BotState.Combat,
				new CombatState()
			},
			{
				BotState.Looting,
				new LootingState()
			},
			{
				BotState.Reviving,
				new RevivingState()
			},
			{
				BotState.Selling,
				new SellingState()
			},
			{
				BotState.Storing,
				new StoringState()
			}
		};

		// Token: 0x04000088 RID: 136
		private static IBotState _activeState;

		// Token: 0x04000089 RID: 137
		private static BotContext _ctx;
	}
}
