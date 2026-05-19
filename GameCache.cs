using System;
using Il2Cpp;
using Il2CppSystem.Collections.Generic;
using MelonLoader;

namespace SpiritMod
{
	// Token: 0x02000021 RID: 33
	public static class GameCache
	{
		// Token: 0x0600010E RID: 270 RVA: 0x0000D028 File Offset: 0x0000B228
		public static void InvalidateAll()
		{
			GameCache.PlayerController.Invalidate();
			GameCache.Skills.Invalidate();
			GameCache.UIManager.Invalidate();
			GameCache.Characters.Invalidate();
			MelonLogger.Msg("[GameCache] All caches invalidated.");
		}

		// Token: 0x040000E6 RID: 230
		public static readonly CachedRef<PlayerController> PlayerController = new CachedRef<PlayerController>();

		// Token: 0x040000E7 RID: 231
		public static readonly CachedRef<Il2CppSystem.Collections.Generic.List<SkillData>> Skills = new CachedRef<Il2CppSystem.Collections.Generic.List<SkillData>>();

		// Token: 0x040000E8 RID: 232
		public static readonly CachedRef<UIManager> UIManager = new CachedRef<UIManager>();

		// Token: 0x040000E9 RID: 233
		public static readonly CachedRef<Il2CppSystem.Collections.Generic.List<CharacterData>> Characters = new CachedRef<Il2CppSystem.Collections.Generic.List<CharacterData>>();
	}
}
