using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritMod
{
    public static class Il2CppConverter
    {
        public static Il2CppSystem.Collections.Generic.Dictionary<TKey, TValue> ToIl2CppDictionary<TKey, TValue>(
            System.Collections.Generic.Dictionary<TKey, TValue> source)
        {
            var result = new Il2CppSystem.Collections.Generic.Dictionary<TKey, TValue>();

            if (source == null)
                return result;

            foreach (var kv in source)
            {
                result.Add(kv.Key, kv.Value);
            }

            return result;
        }

        public static System.Collections.Generic.Dictionary<TKey, TValue> ToManagedDictionary<TKey, TValue>(
            Il2CppSystem.Collections.Generic.Dictionary<TKey, TValue> source)
        {
            var result = new System.Collections.Generic.Dictionary<TKey, TValue>();

            if (source == null)
                return result;

            foreach (var kv in source)
            {
                result.Add(kv.Key, kv.Value);
            }

            return result;
        }
    }
}
