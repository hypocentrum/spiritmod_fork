using Il2Cpp;
using Il2CppSystem.Data;
using MelonLoader;
using System;
using UnityEngine;

[assembly: MelonInfo(typeof(SpiritMod.Core), "SpiritMod", "2.1.0", "Fariz", null)]
[assembly: MelonGame("Baikun", "SpiritVale")]

namespace SpiritMod
{
	// Token: 0x02000004 RID: 4
	public class Core : MelonMod
	{
		// Token: 0x06000003 RID: 3 RVA: 0x00002067 File Offset: 0x00000267
		public override void OnInitializeMelon()
		{
			base.LoggerInstance.Msg("Initialized.");
		}

        public override void OnLateInitializeMelon()
        {
            base.OnLateInitializeMelon();
            LootFilterService.Load();
        }

		// Token: 0x06000004 RID: 4 RVA: 0x0000207E File Offset: 0x0000027E
		public override void OnUpdate()
		{
			this.HandleKeybinds();
			Cheats.Tick();
			float deltaTime = Time.deltaTime;
			BotController.Tick(deltaTime);
			LootFilterService.TickAutosave(deltaTime);
			MonsterEspService.Tick(deltaTime);
			LootEspService.Tick(deltaTime);
			WorldMapOverlayService.Tick(deltaTime);
		}

		// Token: 0x06000005 RID: 5 RVA: 0x000020AD File Offset: 0x000002AD
		public override void OnGUI()
		{
			ModUI.Draw();
			MonsterEspService.Draw();
			LootEspService.Draw();
			WorldMapOverlayService.Draw();
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000020C3 File Offset: 0x000002C3
		public override void OnSceneWasInitialized(int buildIndex, string sceneName)
		{
			Cheats.InvalidateCache();
			GameStateService.InvalidateAll();
			GameCache.InvalidateAll();
			base.LoggerInstance.Msg("Scene loaded: " + sceneName);
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000020EC File Offset: 0x000002EC
		private void HandleKeybinds()
		{
			if (Input.GetKeyDown((KeyCode)277))
			{
				BotController.Toggle();
				base.LoggerInstance.Msg("Bot: " + (BotController.IsEnabled ? "ENABLED" : "DISABLED"));
			}
			if (Input.GetKeyDown((KeyCode)9))
			{
				ModUI.Toggle();
				base.LoggerInstance.Msg("Menu: " + (ModUI.Visible ? "VISIBLE" : "HIDDEN"));
			}
            if (Input.GetKeyDown(KeyCode.F8))
            {
				ServerDumpService.DebugUILogin();
				ServerListConfigService.RefreshAndSave();
            }
        }

		// Token: 0x04000002 RID: 2
		public const string Version = "2.1.0";
	}
}
