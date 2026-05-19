using Il2Cpp;
using Il2CppTMPro;
using MelonLoader;
using System.Reflection;
using System.Text.Json;
using UnityEngine;

namespace SpiritMod
{
    public static class ServerDumpService
    {
        public static void DebugServerUIEntry(object obj)
        {
            if (obj == null)
                return;

            var type = obj.GetType();

            MelonLogger.Msg("[ServerList] ServerUIEntry type = " + type.FullName);

            foreach (var f in type.GetFields(
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance))
            {
                try
                {
                    var value = f.GetValue(obj);
                    MelonLogger.Msg($"FIELD {f.Name} = {value}");
                }
                catch (Exception ex)
                {
                    MelonLogger.Msg($"FIELD {f.Name} ERROR {ex.Message}");
                }
            }
        }
        public static void DebugUILogin()
        {
            try
            {
                var loginObj = UnityEngine.Object.FindObjectOfType(
                    Il2CppInterop.Runtime.Il2CppType.Of<UILogin>()
                );

                var login = loginObj?.TryCast<UILogin>();

                if (login == null)
                {
                    MelonLogger.Warning("[AutoLogin] UILogin not found");
                    return;
                }

                var type = login.GetType();

                MelonLogger.Msg("===== UILogin Fields =====");
                var serverList = login.serverUIMap;

                foreach (var f in serverList)
                {
                    try
                    {
                        DebugServerUIEntry(f.value);
                        MelonLogger.Msg(
                            $"FIELD {f.key} = {f.value}"
                        );
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex.ToString());
            }
        }
    }
}