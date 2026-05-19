using System;
using System.Runtime.CompilerServices;
using Il2Cpp;
using UnityEngine;

namespace SpiritMod
{
	// Token: 0x0200002A RID: 42
	public static class LootEspService
	{
		// Token: 0x17000032 RID: 50
		// (get) Token: 0x0600012B RID: 299 RVA: 0x0000DC9E File Offset: 0x0000BE9E
		// (set) Token: 0x0600012C RID: 300 RVA: 0x0000DCA5 File Offset: 0x0000BEA5
		public static bool Enabled { get; set; }

		// Token: 0x0600012D RID: 301 RVA: 0x0000DCAD File Offset: 0x0000BEAD
		public static void Tick(float dt)
		{
			if (!LootEspService.Enabled)
			{
				return;
			}
			LootEspService._scanTimer -= dt;
			if (LootEspService._scanTimer > 0f)
			{
				return;
			}
			LootEspService._scanTimer = 0.25f;
			GameStateService.TryScanLoot(false);
		}

		// Token: 0x0600012E RID: 302 RVA: 0x0000DCE0 File Offset: 0x0000BEE0
		public static void Draw()
		{
			if (!LootEspService.Enabled)
			{
				return;
			}
			Camera main = Camera.main;
			if (main == null)
			{
				return;
			}
			LootEspService.EnsureResources();
			Vector3 vector = Vector3.zero;
			PlayerController player = GameStateService.Player;
			if (player != null)
			{
				try
				{
					vector = player.Cast<BaseUnitController>().Position;
				}
				catch
				{
				}
			}
			for (int i = 0; i < 12; i++)
			{
				LootEspService._nearest[i] = null;
				LootEspService._distances[i] = float.MaxValue;
			}
			foreach (LootInfo lootInfo in GameStateService.Loot)
			{
				if (lootInfo != null && !(lootInfo.Drop == null))
				{
					float sqrMagnitude = (lootInfo.Position - vector).sqrMagnitude;
					LootEspService.TryInsert(lootInfo, sqrMagnitude);
				}
			}
			Vector2 from = new Vector2((float)Screen.width * 0.5f, (float)Screen.height * 0.58f);
			for (int j = 0; j < 12; j++)
			{
				LootInfo lootInfo2 = LootEspService._nearest[j];
				if (lootInfo2 != null && !(lootInfo2.Drop == null))
				{
					Vector3 vector2 = main.WorldToScreenPoint(lootInfo2.Position + new Vector3(0f, 0.4f, 0f));
					if (vector2.z > 0f)
					{
						float x = vector2.x;
						float num = (float)Screen.height - vector2.y;
						if (x >= -50f && x <= (float)(Screen.width + 50) && num >= -50f && num <= (float)(Screen.height + 50))
						{
							float value = Mathf.Sqrt(LootEspService._distances[j]);
							Color rarityColor = LootEspService.GetRarityColor(lootInfo2.Rarity);
							LootEspService.DrawLine(from, new Vector2(x, num - 8f), new Color(rarityColor.r, rarityColor.g, rarityColor.b, 0.65f), 1.25f);
							Rect rect = new Rect(x - 90f, num - 14f, 180f, 18f);
							GUI.DrawTexture(new Rect(rect.x - 2f, rect.y - 1f, rect.width + 4f, rect.height + 2f), LootEspService._bgTex);
							Color contentColor = GUI.contentColor;
							GUI.contentColor = rarityColor;
							Rect rect2 = rect;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 2);
							defaultInterpolatedStringHandler.AppendFormatted(lootInfo2.Name ?? "Loot");
							defaultInterpolatedStringHandler.AppendLiteral(" [");
							defaultInterpolatedStringHandler.AppendFormatted<float>(value, "F0");
							defaultInterpolatedStringHandler.AppendLiteral("m]");
							GUI.Label(rect2, defaultInterpolatedStringHandler.ToStringAndClear(), LootEspService._labelStyle);
							GUI.contentColor = contentColor;
						}
					}
				}
			}
		}

		// Token: 0x0600012F RID: 303 RVA: 0x0000DFE0 File Offset: 0x0000C1E0
		private static void TryInsert(LootInfo loot, float distSqr)
		{
			for (int i = 0; i < 12; i++)
			{
				if (distSqr < LootEspService._distances[i])
				{
					for (int j = 11; j > i; j--)
					{
						LootEspService._nearest[j] = LootEspService._nearest[j - 1];
						LootEspService._distances[j] = LootEspService._distances[j - 1];
					}
					LootEspService._nearest[i] = loot;
					LootEspService._distances[i] = distSqr;
					return;
				}
			}
		}

		// Token: 0x06000130 RID: 304 RVA: 0x0000E044 File Offset: 0x0000C244
		private static void DrawLine(Vector2 from, Vector2 to, Color color, float thickness)
		{
			Vector2 vector = to - from;
			float magnitude = vector.magnitude;
			if (magnitude <= 0.01f)
			{
				return;
			}
			Color color2 = GUI.color;
			Matrix4x4 matrix = GUI.matrix;
			GUI.color = color;
			GUIUtility.RotateAroundPivot(Mathf.Atan2(vector.y, vector.x) * 57.29578f, from);
			GUI.DrawTexture(new Rect(from.x, from.y, magnitude, thickness), LootEspService._lineTex);
			GUI.matrix = matrix;
			GUI.color = color2;
		}

		// Token: 0x06000131 RID: 305 RVA: 0x0000E0C0 File Offset: 0x0000C2C0
		private static Color GetRarityColor(int rarity)
		{
			Color result;
			switch (rarity)
			{
			case 1:
				result = new Color(0.3f, 0.6f, 1f, 1f);
				break;
			case 2:
				result = new Color(0.8f, 0.4f, 1f, 1f);
				break;
			case 3:
				result = new Color(1f, 0.75f, 0.2f, 1f);
				break;
			default:
				result = new Color(0.85f, 0.85f, 0.85f, 1f);
				break;
			}
			return result;
		}

		// Token: 0x06000132 RID: 306 RVA: 0x0000E154 File Offset: 0x0000C354
		private static void EnsureResources()
		{
			if (LootEspService._bgTex == null)
			{
				LootEspService._bgTex = new Texture2D(1, 1, (TextureFormat)4, false);
				LootEspService._bgTex.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.5f));
				LootEspService._bgTex.Apply();
			}
			if (LootEspService._lineTex == null)
			{
				LootEspService._lineTex = new Texture2D(1, 1, (TextureFormat)4, false);
				LootEspService._lineTex.SetPixel(0, 0, Color.white);
				LootEspService._lineTex.Apply();
			}
			if (LootEspService._labelStyle == null)
			{
				LootEspService._labelStyle = new GUIStyle
				{
					alignment = (TextAnchor)4,
					fontSize = 11,
					fontStyle = (FontStyle)1
				};
			}
		}

		// Token: 0x04000104 RID: 260
		private const int MaxDrawn = 12;

		// Token: 0x04000105 RID: 261
		private static float _scanTimer;

		// Token: 0x04000106 RID: 262
		private static Texture2D _bgTex;

		// Token: 0x04000107 RID: 263
		private static Texture2D _lineTex;

		// Token: 0x04000108 RID: 264
		private static GUIStyle _labelStyle;

		// Token: 0x04000109 RID: 265
		private static readonly LootInfo[] _nearest = new LootInfo[12];

		// Token: 0x0400010A RID: 266
		private static readonly float[] _distances = new float[12];
	}
}
