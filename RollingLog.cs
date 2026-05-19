using System;
using System.IO;
using System.Runtime.CompilerServices;
using MelonLoader;

namespace SpiritMod
{
	// Token: 0x0200001E RID: 30
	public class RollingLog
	{
		// Token: 0x060000FD RID: 253 RVA: 0x0000C8B0 File Offset: 0x0000AAB0
		public RollingLog(string directory, string fileName, string tag, long maxSizeBytes = 524288L)
		{
			this._tag = tag;
			this._maxSizeBytes = maxSizeBytes;
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
			string extension = Path.GetExtension(fileName);
			this._logPath = Path.Combine(directory, fileName);
			this._logPathOld = Path.Combine(directory, fileNameWithoutExtension + "_old" + extension);
		}

		// Token: 0x060000FE RID: 254 RVA: 0x0000C908 File Offset: 0x0000AB08
		public void Append(string action, string itemName, int count, string category, int rarity, string statsInfo = null, string reason = null)
		{
			try
			{
				Directory.CreateDirectory(Path.GetDirectoryName(this._logPath));
				if (File.Exists(this._logPath) && new FileInfo(this._logPath).Length >= this._maxSizeBytes)
				{
					if (File.Exists(this._logPathOld))
					{
						File.Delete(this._logPathOld);
					}
					File.Move(this._logPath, this._logPathOld);
				}
				string value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				string value2 = LootFilterService.RarityName(rarity);
				string value3 = string.IsNullOrEmpty(reason) ? "" : (" reason=" + reason);
				string value4 = string.IsNullOrEmpty(statsInfo) ? "" : (" [" + statsInfo + "]");
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(12, 8);
				defaultInterpolatedStringHandler.AppendLiteral("[");
				defaultInterpolatedStringHandler.AppendFormatted(value);
				defaultInterpolatedStringHandler.AppendLiteral("] ");
				defaultInterpolatedStringHandler.AppendFormatted(action);
				defaultInterpolatedStringHandler.AppendLiteral(": ");
				defaultInterpolatedStringHandler.AppendFormatted(itemName);
				defaultInterpolatedStringHandler.AppendLiteral(" x");
				defaultInterpolatedStringHandler.AppendFormatted<int>(count);
				defaultInterpolatedStringHandler.AppendLiteral(" (");
				defaultInterpolatedStringHandler.AppendFormatted(category);
				defaultInterpolatedStringHandler.AppendLiteral("/");
				defaultInterpolatedStringHandler.AppendFormatted(value2);
				defaultInterpolatedStringHandler.AppendLiteral(")");
				defaultInterpolatedStringHandler.AppendFormatted(value3);
				defaultInterpolatedStringHandler.AppendFormatted(value4);
				defaultInterpolatedStringHandler.AppendLiteral("\n");
				string contents = defaultInterpolatedStringHandler.ToStringAndClear();
				File.AppendAllText(this._logPath, contents);
			}
			catch (Exception ex)
			{
				MelonLogger.Warning("[" + this._tag + "] Log write failed: " + ex.Message);
			}
		}

		// Token: 0x040000DE RID: 222
		private readonly string _logPath;

		// Token: 0x040000DF RID: 223
		private readonly string _logPathOld;

		// Token: 0x040000E0 RID: 224
		private readonly long _maxSizeBytes;

		// Token: 0x040000E1 RID: 225
		private readonly string _tag;
	}
}
