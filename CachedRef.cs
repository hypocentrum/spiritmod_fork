using System;
using UnityEngine;

namespace SpiritMod
{
	// Token: 0x02000020 RID: 32
	public sealed class CachedRef<T> where T : class
	{
		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000109 RID: 265 RVA: 0x0000CFC6 File Offset: 0x0000B1C6
		public T Value
		{
			get
			{
				return this._value;
			}
		}

		// Token: 0x0600010A RID: 266 RVA: 0x0000CFD0 File Offset: 0x0000B1D0
		public bool IsValid()
		{
			if (this._value == null)
			{
				return false;
			}
			UnityEngine.Object @object = this._value as UnityEngine.Object;
			return @object == null || @object != null;
		}

		// Token: 0x0600010B RID: 267 RVA: 0x0000D009 File Offset: 0x0000B209
		public void Update(T newValue)
		{
			this._value = newValue;
		}

		// Token: 0x0600010C RID: 268 RVA: 0x0000D012 File Offset: 0x0000B212
		public void Invalidate()
		{
			this._value = default(T);
		}

		// Token: 0x040000E5 RID: 229
		private T _value;
	}
}
