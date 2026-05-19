using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Il2Cpp;
using UnityEngine;

namespace SpiritMod
{
	// Token: 0x0200002B RID: 43
	public static class WorldMapOverlayService
	{
		// Token: 0x17000033 RID: 51
		// (get) Token: 0x06000134 RID: 308 RVA: 0x0000E222 File Offset: 0x0000C422
		// (set) Token: 0x06000135 RID: 309 RVA: 0x0000E229 File Offset: 0x0000C429
		public static bool Enabled { get; set; }

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x06000136 RID: 310 RVA: 0x0000E231 File Offset: 0x0000C431
		// (set) Token: 0x06000137 RID: 311 RVA: 0x0000E238 File Offset: 0x0000C438
		public static float Opacity { get; set; } = 0.72f;

		// Token: 0x06000138 RID: 312 RVA: 0x0000E240 File Offset: 0x0000C440
		public static void Tick(float dt)
		{
			if (!WorldMapOverlayService.Enabled)
			{
				return;
			}
			WorldMapOverlayService._scanTimer -= dt;
			if (WorldMapOverlayService._scanTimer > 0f)
			{
				return;
			}
			WorldMapOverlayService._scanTimer = 0.5f;
			GameStateService.TryScanMonsters();
			GameStateService.TryScanLoot(false);
			WorldMapOverlayService.RefreshWaypoints();
		}

		// Token: 0x06000139 RID: 313 RVA: 0x0000E280 File Offset: 0x0000C480
		public static void Draw()
		{
			if (!WorldMapOverlayService.Enabled)
			{
				return;
			}
			PlayerController player = GameStateService.Player;
			if (player == null)
			{
				return;
			}
			Vector3 position;
			try
			{
				position = player.Cast<BaseUnitController>().Position;
			}
			catch
			{
				return;
			}
			WorldMapOverlayService.EnsureResources();
			List<WorldMapOverlayService.MapPoint> points = WorldMapOverlayService._points;
			points.Clear();
			points.Add(new WorldMapOverlayService.MapPoint
			{
				Name = "You",
				Position = position,
				Color = new Color(0.25f, 1f, 0.35f),
				Size = 8f,
				DrawLabel = true
			});
			int num = 0;
			while (num < GameStateService.Monsters.Count && num < 80)
			{
				MonsterInfo monsterInfo = GameStateService.Monsters[num];
				if (monsterInfo != null && !(monsterInfo.Controller == null))
				{
					points.Add(new WorldMapOverlayService.MapPoint
					{
						Name = monsterInfo.Name,
						Position = monsterInfo.Position,
						Color = new Color(1f, 0.25f, 0.25f, 0.95f),
						Size = 4f
					});
				}
				num++;
			}
			int num2 = 0;
			while (num2 < GameStateService.Loot.Count && num2 < 80)
			{
				LootInfo lootInfo = GameStateService.Loot[num2];
				if (lootInfo != null && !(lootInfo.Drop == null))
				{
					points.Add(new WorldMapOverlayService.MapPoint
					{
						Name = lootInfo.Name,
						Position = lootInfo.Position,
						Color = WorldMapOverlayService.GetLootColor(lootInfo.Rarity),
						Size = 3f
					});
				}
				num2++;
			}
			for (int i = 0; i < WorldMapOverlayService._waypoints.Count; i++)
			{
				WorldMapOverlayService.WaypointPoint waypointPoint = WorldMapOverlayService._waypoints[i];
				points.Add(new WorldMapOverlayService.MapPoint
				{
					Name = waypointPoint.Name,
					Position = waypointPoint.Position,
					Color = new Color(1f, 0.85f, 0.2f),
					Size = 7f,
					DrawLabel = true
				});
			}
			if (points.Count == 0)
			{
				return;
			}
			Rect rect = new((float)(Screen.width - 700) * 0.5f, (float)(Screen.height - 500) * 0.5f, 700f, 500f);
			Color color = GUI.color;
			GUI.color = new Color(1f, 1f, 1f, Mathf.Clamp01(WorldMapOverlayService.Opacity));
			GUI.DrawTexture(rect, WorldMapOverlayService._bgTex);
			GUI.color = color;
			GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, 2f), WorldMapOverlayService._gridTex);
			GUI.DrawTexture(new Rect(rect.x, rect.yMax - 2f, rect.width, 2f), WorldMapOverlayService._gridTex);
			GUI.DrawTexture(new Rect(rect.x, rect.y, 2f, rect.height), WorldMapOverlayService._gridTex);
			GUI.DrawTexture(new Rect(rect.xMax - 2f, rect.y, 2f, rect.height), WorldMapOverlayService._gridTex);
			float num3 = float.MaxValue;
			float num4 = float.MinValue;
			float num5 = float.MaxValue;
			float num6 = float.MinValue;
			for (int j = 0; j < points.Count; j++)
			{
				Vector3 position2 = points[j].Position;
				if (position2.x < num3)
				{
					num3 = position2.x;
				}
				if (position2.x > num4)
				{
					num4 = position2.x;
				}
				if (position2.z < num5)
				{
					num5 = position2.z;
				}
				if (position2.z > num6)
				{
					num6 = position2.z;
				}
			}
			if (Mathf.Abs(num4 - num3) < 5f)
			{
				num4 += 3f;
				num3 -= 3f;
			}
			if (Mathf.Abs(num6 - num5) < 5f)
			{
				num6 += 3f;
				num5 -= 3f;
			}
			float num7 = (num4 - num3) * 0.12f;
			float num8 = (num6 - num5) * 0.12f;
			float num9 = num3 - num7;
			float num10 = num4 + num7;
			float num11 = num5 - num8;
			float num12 = num6 + num8;
			Rect rect2 = new(rect.x + 16f, rect.y + 38f, rect.width - 32f, rect.height - 54f);
			GUI.DrawTexture(new Rect(rect2.x, rect2.center.y, rect2.width, 1f), WorldMapOverlayService._gridTex);
			GUI.DrawTexture(new Rect(rect2.center.x, rect2.y, 1f, rect2.height), WorldMapOverlayService._gridTex);
			for (int k = 0; k < points.Count; k++)
			{
				WorldMapOverlayService.MapPoint mapPoint = points[k];
				float num13 = Mathf.InverseLerp(num9, num10, mapPoint.Position.x);
				float num14 = Mathf.InverseLerp(num11, num12, mapPoint.Position.z);
				float num15 = rect2.x + rect2.width * num13;
				float num16 = rect2.yMax - rect2.height * num14;
				Color color2 = GUI.color;
				GUI.color = mapPoint.Color;
				GUI.DrawTexture(new Rect(num15 - mapPoint.Size * 0.5f, num16 - mapPoint.Size * 0.5f, mapPoint.Size, mapPoint.Size), WorldMapOverlayService._dotTex);
				GUI.color = color2;
				if (mapPoint.DrawLabel)
				{
					GUI.Label(new Rect(num15 + 6f, num16 - 10f, 220f, 20f), mapPoint.Name ?? "?", WorldMapOverlayService._labelStyle);
				}
			}
			string value = TeleporterService.GetCurrentMapName() ?? "UnknownMap";
			Rect rect3 = new Rect(rect.x + 12f, rect.y + 8f, rect.width - 24f, 24f);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(35, 2);
			defaultInterpolatedStringHandler.AppendLiteral("World Map Overlay - ");
			defaultInterpolatedStringHandler.AppendFormatted(value);
			defaultInterpolatedStringHandler.AppendLiteral("  (Waypoints: ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(WorldMapOverlayService._waypoints.Count);
			defaultInterpolatedStringHandler.AppendLiteral(")");
			GUI.Label(rect3, defaultInterpolatedStringHandler.ToStringAndClear(), WorldMapOverlayService._titleStyle);
			if (!string.IsNullOrEmpty(WorldMapOverlayService._waypointScanStatus))
			{
				GUI.Label(new Rect(rect.x + 12f, rect.yMax - 18f, rect.width - 24f, 16f), WorldMapOverlayService._waypointScanStatus, WorldMapOverlayService._hintStyle);
			}
		}

		// Token: 0x0600013A RID: 314 RVA: 0x0000E9A0 File Offset: 0x0000CBA0
		private static void RefreshWaypoints()
		{
			try
			{
				string currentMapName = TeleporterService.GetCurrentMapName();
				if (currentMapName != null)
				{
					if (!(WorldMapOverlayService._cachedMapName == currentMapName) || Time.time - WorldMapOverlayService._lastWaypointScanTime >= 1f)
					{
						WorldMapOverlayService._cachedMapName = currentMapName;
						WorldMapOverlayService._lastWaypointScanTime = Time.time;
						WorldMapOverlayService._waypoints.Clear();
						WorldMapOverlayService._waypointScanStatus = "Scanning waypoint/portal data...";
						Game game = App.Game;
						MapManager mapManager = (game != null) ? game.Map : null;
						Map map = (mapManager != null) ? mapManager.CurrentMap : null;
						if (map == null)
						{
							WorldMapOverlayService._waypointScanStatus = "CurrentMap unavailable";
						}
						else if (!WorldMapOverlayService.TryExtractWaypointLikeObjects(map) && !WorldMapOverlayService.TryExtractWaypointLikeObjects(map.Config))
						{
							WorldMapOverlayService._waypointScanStatus = "Waypoint fields not discovered (reflection fallback active)";
						}
						else
						{
							WorldMapOverlayService._waypointScanStatus = "Waypoint scan OK";
						}
					}
				}
			}
			catch (Exception ex)
			{
				WorldMapOverlayService._waypointScanStatus = "Waypoint scan failed: " + ex.Message;
			}
		}

		// Token: 0x0600013B RID: 315 RVA: 0x0000EA9C File Offset: 0x0000CC9C
		private static bool TryExtractWaypointLikeObjects(object root)
		{
			if (root == null)
			{
				return false;
			}
			bool flag = false;
			foreach (MemberInfo memberInfo in root.GetType().GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			{
				string name = memberInfo.Name;
				if (!string.IsNullOrEmpty(name))
				{
					string text = name.ToLowerInvariant();
					if (text.Contains("waypoint") || text.Contains("portal") || text.Contains("warp"))
					{
						object memberValue = WorldMapOverlayService.GetMemberValue(root, memberInfo);
						flag |= WorldMapOverlayService.TryReadWaypointCollection(memberValue, name);
					}
				}
			}
			return flag;
		}

		// Token: 0x0600013C RID: 316 RVA: 0x0000EB2C File Offset: 0x0000CD2C
		private static bool TryReadWaypointCollection(object obj, string sourceName)
		{
			if (obj == null)
			{
				return false;
			}
			IDictionary dictionary = obj as IDictionary;
			if (dictionary != null)
			{
				bool flag = false;
				foreach (object obj2 in dictionary)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj2;
					flag |= WorldMapOverlayService.TryAddWaypointFromUnknown(dictionaryEntry.Value, dictionaryEntry.Key as string, sourceName);
				}
				return flag;
			}
			IEnumerable enumerable = obj as IEnumerable;
			if (enumerable != null && !(obj is string))
			{
				bool flag2 = false;
				int num = 0;
				foreach (object obj3 in enumerable)
				{
					bool flag3 = flag2;
					object obj4 = obj3;
					string fallbackName = null;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
					defaultInterpolatedStringHandler.AppendFormatted(sourceName);
					defaultInterpolatedStringHandler.AppendLiteral("#");
					defaultInterpolatedStringHandler.AppendFormatted<int>(num);
					flag2 = (flag3 | WorldMapOverlayService.TryAddWaypointFromUnknown(obj4, fallbackName, defaultInterpolatedStringHandler.ToStringAndClear()));
					if (++num > 128)
					{
						break;
					}
				}
				return flag2;
			}
			return WorldMapOverlayService.TryAddWaypointFromUnknown(obj, null, sourceName);
		}

		// Token: 0x0600013D RID: 317 RVA: 0x0000EC60 File Offset: 0x0000CE60
		private static bool TryAddWaypointFromUnknown(object obj, string fallbackName, string sourceTag)
		{
			if (obj == null)
			{
				return false;
			}
			Vector3? vector = WorldMapOverlayService.TryReadVector3(obj);
			if (vector == null)
			{
				return false;
			}
			string name = WorldMapOverlayService.TryReadName(obj) ?? (fallbackName ?? (sourceTag ?? "Waypoint"));
			for (int i = 0; i < WorldMapOverlayService._waypoints.Count; i++)
			{
				if ((WorldMapOverlayService._waypoints[i].Position - vector.Value).sqrMagnitude < 0.05f)
				{
					return false;
				}
			}
			WorldMapOverlayService._waypoints.Add(new WorldMapOverlayService.WaypointPoint
			{
				Name = name,
				Position = vector.Value
			});
			return true;
		}

		// Token: 0x0600013E RID: 318 RVA: 0x0000ED0C File Offset: 0x0000CF0C
		private static Vector3? TryReadVector3(object obj)
		{
			Type type = obj.GetType();
			foreach (string name in new string[]
			{
				"Position",
				"WorldPosition",
				"Pos",
				"SpawnPosition",
				"WarpPosition",
				"Point"
			})
			{
				object memberValueByName = WorldMapOverlayService.GetMemberValueByName(obj, type, name);
				if (memberValueByName is Vector3)
				{
					Vector3 value = (Vector3)memberValueByName;
					return new Vector3?(value);
				}
			}
			Transform transform = (WorldMapOverlayService.GetMemberValueByName(obj, type, "transform") ?? WorldMapOverlayService.GetMemberValueByName(obj, type, "Transform")) as Transform;
			if (transform != null)
			{
				return new Vector3?(transform.position);
			}
			return null;
		}

		// Token: 0x0600013F RID: 319 RVA: 0x0000EDCC File Offset: 0x0000CFCC
		private static string TryReadName(object obj)
		{
			Type type = obj.GetType();
			foreach (string name in new string[]
			{
				"DisplayName",
				"Name",
				"Id",
				"Key"
			})
			{
				string text = WorldMapOverlayService.GetMemberValueByName(obj, type, name) as string;
				if (text != null && !string.IsNullOrEmpty(text))
				{
					return text;
				}
			}
			return null;
		}

		// Token: 0x06000140 RID: 320 RVA: 0x0000EE3C File Offset: 0x0000D03C
		private static object GetMemberValueByName(object root, Type type, string name)
		{
			PropertyInfo property = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (property != null)
			{
				try
				{
					return property.GetValue(root);
				}
				catch
				{
				}
			}
			FieldInfo field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (field != null)
			{
				try
				{
					return field.GetValue(root);
				}
				catch
				{
				}
			}
			return null;
		}

		// Token: 0x06000141 RID: 321 RVA: 0x0000EEA8 File Offset: 0x0000D0A8
		private static object GetMemberValue(object root, MemberInfo member)
		{
			try
			{
				PropertyInfo propertyInfo = member as PropertyInfo;
				if (propertyInfo != null)
				{
					return propertyInfo.GetValue(root);
				}
				FieldInfo fieldInfo = member as FieldInfo;
				if (fieldInfo != null)
				{
					return fieldInfo.GetValue(root);
				}
			}
			catch
			{
			}
			return null;
		}

		// Token: 0x06000142 RID: 322 RVA: 0x0000EEF8 File Offset: 0x0000D0F8
		private static Color GetLootColor(int rarity)
		{
			Color result;
			switch (rarity)
			{
			case 1:
				result = new Color(0.3f, 0.6f, 1f, 0.95f);
				break;
			case 2:
				result = new Color(0.8f, 0.4f, 1f, 0.95f);
				break;
			case 3:
				result = new Color(1f, 0.75f, 0.2f, 0.95f);
				break;
			default:
				result = new Color(0.85f, 0.85f, 0.85f, 0.8f);
				break;
			}
			return result;
		}

		// Token: 0x06000143 RID: 323 RVA: 0x0000EF8C File Offset: 0x0000D18C
		private static void EnsureResources()
		{
			if (WorldMapOverlayService._bgTex == null)
			{
				WorldMapOverlayService._bgTex = WorldMapOverlayService.MakeTex(new Color(0f, 0f, 0f, 0.65f));
			}
			if (WorldMapOverlayService._gridTex == null)
			{
				WorldMapOverlayService._gridTex = WorldMapOverlayService.MakeTex(new Color(1f, 1f, 1f, 0.12f));
			}
			if (WorldMapOverlayService._dotTex == null)
			{
				WorldMapOverlayService._dotTex = WorldMapOverlayService.MakeTex(Color.white);
			}
			if (WorldMapOverlayService._titleStyle == null)
			{
				WorldMapOverlayService._titleStyle = new GUIStyle
				{
					fontSize = 15,
					fontStyle = (FontStyle)1
				};
				WorldMapOverlayService._titleStyle.normal.textColor = new Color(0.95f, 0.95f, 0.95f);
			}
			if (WorldMapOverlayService._labelStyle == null)
			{
				WorldMapOverlayService._labelStyle = new GUIStyle
				{
					fontSize = 11
				};
				WorldMapOverlayService._labelStyle.normal.textColor = Color.white;
			}
			if (WorldMapOverlayService._hintStyle == null)
			{
				WorldMapOverlayService._hintStyle = new GUIStyle
				{
					fontSize = 10,
					alignment = (TextAnchor)8
				};
				WorldMapOverlayService._hintStyle.normal.textColor = new Color(0.8f, 0.8f, 0.8f, 0.85f);
			}
		}

		// Token: 0x06000144 RID: 324 RVA: 0x0000F0CD File Offset: 0x0000D2CD
		private static Texture2D MakeTex(Color c)
		{
			Texture2D texture2D = new Texture2D(1, 1, (TextureFormat)4, false);
			texture2D.SetPixel(0, 0, c);
			texture2D.Apply();
			return texture2D;
		}

		// Token: 0x0400010C RID: 268
		private static float _scanTimer;

		// Token: 0x0400010D RID: 269
		private static float _lastWaypointScanTime;

		// Token: 0x0400010E RID: 270
		private static string _cachedMapName;

		// Token: 0x0400010F RID: 271
		private static string _waypointScanStatus;

		// Token: 0x04000110 RID: 272
		private static Texture2D _bgTex;

		// Token: 0x04000111 RID: 273
		private static Texture2D _gridTex;

		// Token: 0x04000112 RID: 274
		private static Texture2D _dotTex;

		// Token: 0x04000113 RID: 275
		private static GUIStyle _titleStyle;

		// Token: 0x04000114 RID: 276
		private static GUIStyle _labelStyle;

		// Token: 0x04000115 RID: 277
		private static GUIStyle _hintStyle;

		// Token: 0x04000116 RID: 278
		private static readonly List<WorldMapOverlayService.MapPoint> _points = new List<WorldMapOverlayService.MapPoint>(256);

		// Token: 0x04000117 RID: 279
		private static readonly List<WorldMapOverlayService.WaypointPoint> _waypoints = new List<WorldMapOverlayService.WaypointPoint>(32);

		// Token: 0x02000040 RID: 64
		private struct WaypointPoint
		{
			// Token: 0x04000162 RID: 354
			public string Name;

			// Token: 0x04000163 RID: 355
			public Vector3 Position;
		}

		// Token: 0x02000041 RID: 65
		private struct MapPoint
		{
			// Token: 0x04000164 RID: 356
			public string Name;

			// Token: 0x04000165 RID: 357
			public Vector3 Position;

			// Token: 0x04000166 RID: 358
			public Color Color;

			// Token: 0x04000167 RID: 359
			public float Size;

			// Token: 0x04000168 RID: 360
			public bool DrawLabel;
		}
	}
}
