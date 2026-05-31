using System;
using System.IO;
using System.Reflection;
using System.Text;
using Il2Cpp;
using MelonLoader;

namespace SpiritMod
{
    public static class BuffLiveDumpService
    {
        public static string DumpLiveBuffDictionaries()
        {
            try
            {
                PlayerController player = GameStateService.Player;
                if (player == null)
                    return "No player";

                BaseUnitController baseUnit = player.Cast<BaseUnitController>();
                StatusComponent status = baseUnit != null ? baseUnit.Status : null;

                if (status == null)
                {
                    try { status = player.Status; }
                    catch { }
                }

                if (status == null)
                    return "No status";

                StringBuilder sb = new StringBuilder();

                sb.AppendLine("=== EffectsDictionary ===");
                try { DumpDictionary(status.EffectsDictionary, sb); }
                catch (Exception ex) { sb.AppendLine("EffectsDictionary error: " + ex.Message); }

                sb.AppendLine();
                sb.AppendLine("=== SkillDisplays_C ===");
                try { DumpDictionary(status.SkillDisplays_C, sb); }
                catch (Exception ex) { sb.AppendLine("SkillDisplays_C error: " + ex.Message); }

                sb.AppendLine();
                sb.AppendLine("=== StatusDisplays_C ===");
                try { DumpDictionary(status.StatusDisplays_C, sb); }
                catch (Exception ex) { sb.AppendLine("StatusDisplays_C error: " + ex.Message); }

                string path = Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".",
                    "live_buff_dump.txt");

                File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
                MelonLogger.Msg("[BuffLiveDump] Wrote " + path);
                return path;
            }
            catch (Exception ex)
            {
                MelonLogger.Warning("[BuffLiveDump] Failed: " + ex);
                return "Error: " + ex.Message;
            }
        }

        private static void DumpDictionary(
            Il2CppSystem.Collections.Generic.Dictionary<string, StatusEffectState> dictionary,
            StringBuilder sb)
        {
            if (dictionary == null)
            {
                sb.AppendLine("null");
                return;
            }

            int count = 0;

            foreach (var pair in dictionary)
            {
                count++;

                sb.Append(pair.key != null ? pair.key.ToString() : "<null key>");
                sb.Append(" => ");

                var value = pair.value;
                if (value == null)
                {
                    sb.AppendLine("<null value>");
                    continue;
                }

                sb.Append(value.GetType().FullName);

                TryAppendProperty(value, "Duration", sb);
                TryAppendProperty(value, "Stacks", sb);
                TryAppendProperty(value, "Stackable", sb);
                TryAppendProperty(value, "Time", sb);
                TryAppendProperty(value, "Timer", sb);
                TryAppendProperty(value, "IsToggle", sb);
                TryAppendProperty(value, "InfiniteDuration", sb);

                sb.AppendLine();
            }

            sb.AppendLine("Count=" + count);
        }

        private static void TryAppendProperty(object value, string propertyName, StringBuilder sb)
        {
            try
            {
                var prop = value.GetType().GetProperty(propertyName);
                if (prop == null)
                    return;

                object propValue = prop.GetValue(value, null);
                sb.Append(" ");
                sb.Append(propertyName);
                sb.Append("=");
                sb.Append(propValue != null ? propValue.ToString() : "null");
            }
            catch
            {
            }
        }
    }
}
