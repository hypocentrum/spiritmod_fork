using System;
using System.Runtime.CompilerServices;
using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace SpiritMod
{
	// Token: 0x02000029 RID: 41
	public static class MonsterEspService
	{
		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06000122 RID: 290 RVA: 0x0000D756 File Offset: 0x0000B956
		// (set) Token: 0x06000123 RID: 291 RVA: 0x0000D75D File Offset: 0x0000B95D
		public static bool Enabled { get; set; }

		// Token: 0x06000124 RID: 292 RVA: 0x0000D765 File Offset: 0x0000B965
		public static void Tick(float dt)
		{
			if (!MonsterEspService.Enabled)
			{
				return;
			}
			MonsterEspService._scanTimer -= dt;
			if (MonsterEspService._scanTimer > 0f)
			{
				return;
			}
			MonsterEspService._scanTimer = 0.25f;
			GameStateService.TryScanMonsters();
		}

		// Token: 0x06000125 RID: 293 RVA: 0x0000D798 File Offset: 0x0000B998
		public static void Draw()
		{
			if (!MonsterEspService.Enabled)
			{
				return;
			}
			Camera main = Camera.main;
			if (main == null)
			{
				return;
			}
			PlayerController player = GameStateService.Player;
			Vector3 vector = (player != null) ? player.Cast<BaseUnitController>().Position : Vector3.zero;
			MonsterEspService.EnsureResources();
			for (int i = 0; i < 10; i++)
			{
				MonsterEspService._nearest[i] = null;
				MonsterEspService._distances[i] = float.MaxValue;
			}
			foreach (MonsterInfo monsterInfo in GameStateService.Monsters)
			{
				if (monsterInfo != null && !(monsterInfo.Controller == null))
				{
					float sqrMagnitude = (monsterInfo.Controller.Cast<BaseUnitController>().Position - vector).sqrMagnitude;
					if (sqrMagnitude > 0f)
					{
						MonsterEspService.TryInsert(monsterInfo, sqrMagnitude);
					}
				}
			}
			Vector2 from = new((float)Screen.width * 0.5f, (float)Screen.height * 0.55f);
			for (int j = 0; j < 10; j++)
			{
				MonsterInfo monsterInfo2 = MonsterEspService._nearest[j];
				if (monsterInfo2 != null && !(monsterInfo2.Controller == null))
				{
					Vector3 position = monsterInfo2.Controller.Cast<BaseUnitController>().Position;
					Vector3 vector2 = main.WorldToScreenPoint(position + new Vector3(0f, 1.6f, 0f));
					if (vector2.z > 0f)
					{
						float x = vector2.x;
						float num = (float)Screen.height - vector2.y;
						if (x >= -50f && x <= (float)(Screen.width + 50) && num >= -50f && num <= (float)(Screen.height + 50))
						{
							float value = Mathf.Sqrt(MonsterEspService._distances[j]);
							Vector2 to = new (x, num - 8f);
							MonsterEspService.DrawLine(from, to, new Color(1f, 0.25f, 0.25f, 0.75f), 1.5f);
							Rect rect = new(x - 80f, num - 16f, 160f, 20f);
							GUI.DrawTexture(new Rect(rect.x - 2f, rect.y - 1f, rect.width + 4f, rect.height + 2f), MonsterEspService._bgTex);
							Rect rect2 = rect;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 2);
							defaultInterpolatedStringHandler.AppendFormatted(monsterInfo2.Name ?? "Mob");
							defaultInterpolatedStringHandler.AppendLiteral(" [");
							defaultInterpolatedStringHandler.AppendFormatted<float>(value, "F0");
							defaultInterpolatedStringHandler.AppendLiteral("m]");
							GUI.Label(rect2, defaultInterpolatedStringHandler.ToStringAndClear(), MonsterEspService._labelStyle);
						}
					}
				}
			}
		}

		// Token: 0x06000126 RID: 294 RVA: 0x0000DA88 File Offset: 0x0000BC88
		public static void Toggle(MelonLogger.Instance logger = null)
		{
			MonsterEspService.Enabled = !MonsterEspService.Enabled;
			MonsterEspService._scanTimer = 0f;
			if (logger != null)
			{
				logger.Msg("Monster ESP: " + (MonsterEspService.Enabled ? "ENABLED" : "DISABLED"));
			}
		}

		// Token: 0x06000127 RID: 295 RVA: 0x0000DAC8 File Offset: 0x0000BCC8
		private static void TryInsert(MonsterInfo monster, float distSqr)
		{
			for (int i = 0; i < 10; i++)
			{
				if (distSqr < MonsterEspService._distances[i])
				{
					for (int j = 9; j > i; j--)
					{
						MonsterEspService._nearest[j] = MonsterEspService._nearest[j - 1];
						MonsterEspService._distances[j] = MonsterEspService._distances[j - 1];
					}
					MonsterEspService._nearest[i] = monster;
					MonsterEspService._distances[i] = distSqr;
					return;
				}
			}
		}

		// Token: 0x06000128 RID: 296 RVA: 0x0000DB2C File Offset: 0x0000BD2C
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
			GUI.DrawTexture(new Rect(from.x, from.y, magnitude, thickness), MonsterEspService._lineTex);
			GUI.matrix = matrix;
			GUI.color = color2;
		}

		// Token: 0x06000129 RID: 297 RVA: 0x0000DBA8 File Offset: 0x0000BDA8
		private static void EnsureResources()
		{
			if (MonsterEspService._bgTex == null)
			{
				MonsterEspService._bgTex = new Texture2D(1, 1, (TextureFormat)4, false);
				MonsterEspService._bgTex.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.55f));
				MonsterEspService._bgTex.Apply();
			}
			if (MonsterEspService._lineTex == null)
			{
				MonsterEspService._lineTex = new Texture2D(1, 1, (TextureFormat)4, false);
				MonsterEspService._lineTex.SetPixel(0, 0, Color.white);
				MonsterEspService._lineTex.Apply();
			}
			if (MonsterEspService._labelStyle == null)
			{
				MonsterEspService._labelStyle = new GUIStyle
				{
					alignment = (TextAnchor)4,
					fontSize = 12,
					fontStyle = (FontStyle)1
				};
				MonsterEspService._labelStyle.normal.textColor = new Color(1f, 0.35f, 0.35f, 1f);
			}
		}

		// Token: 0x040000FC RID: 252
		private const int MaxDrawn = 10;

		// Token: 0x040000FD RID: 253
		private static float _scanTimer;

		// Token: 0x040000FE RID: 254
		private static Texture2D _bgTex;

		// Token: 0x040000FF RID: 255
		private static Texture2D _lineTex;

		// Token: 0x04000100 RID: 256
		private static GUIStyle _labelStyle;

		// Token: 0x04000101 RID: 257
		private static readonly MonsterInfo[] _nearest = new MonsterInfo[10];

		// Token: 0x04000102 RID: 258
		private static readonly float[] _distances = new float[10];
	}
}
