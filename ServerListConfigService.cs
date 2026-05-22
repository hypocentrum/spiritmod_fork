using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppTMPro;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using UnityEngine;

namespace SpiritMod
{
    public static class ServerListConfigService
    {
        private static readonly string Dir = Path.Combine("UserData", "SpiritMod");
        private static readonly string PathOut = Path.Combine(Dir, "servers.json");

        public static void RefreshAndSave()
        {
            try
            {
                var login = FindUILogin();

                if (login == null)
                {
                    MelonLogger.Warning("[ServerList] UILogin not found.");
                    return;
                }

                var uiServers = ParseServersFromVisibleUI();
                var instanceIds = ReadInstanceIdsFromLogin(login);

                var servers = Merge(uiServers, instanceIds);

                var config = new ServerListConfig
                {
                    UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Servers = servers
                };

                Directory.CreateDirectory(Dir);

                File.WriteAllText(
                    PathOut,
                    JsonSerializer.Serialize(
                        config,
                        new JsonSerializerOptions { WriteIndented = true }
                    )
                );

                MelonLogger.Msg($"[ServerList] Saved {servers.Count} servers to {PathOut}");
            }
            catch (Exception ex)
            {
                MelonLogger.Error("[ServerList] RefreshAndSave failed: " + ex);
            }
        }

        private static UILogin FindUILogin()
        {
            try
            {
                var obj = UnityEngine.Object.FindObjectOfType(
                    Il2CppType.Of<UILogin>()
                );

                return obj?.TryCast<UILogin>();
            }
            catch (Exception ex)
            {
                MelonLogger.Warning("[ServerList] FindUILogin failed: " + ex.Message);
                return null;
            }
        }

        private static List<ServerUiEntry> ParseServersFromVisibleUI()
        {
            var raw = new List<string>();
            var texts = UnityEngine.Object.FindObjectsOfType<TextMeshProUGUI>();

            foreach (var t in texts)
            {
                if (t == null)
                    continue;

                string value = t.text?.Trim();

                if (string.IsNullOrWhiteSpace(value))
                    continue;

                raw.Add(value);
            }

            var servers = new List<ServerUiEntry>();

            for (int i = 0; i < raw.Count - 3; i++)
            {
                string playersText = raw[i];
                string pingText = raw[i + 1];
                string region = raw[i + 2];
                string serverName = raw[i + 3];

                if (!int.TryParse(playersText, out int players))
                    continue;

                if (!IsPing(pingText))
                    continue;

                servers.Add(new ServerUiEntry
                {
                    Index = servers.Count,
                    ServerName = serverName,
                    Region = region,
                    PingMs = ParsePing(pingText),
                    Players = players
                });
            }

            return servers;
        }
        private static List<string> ReadInstanceIdsFromLogin(UILogin login)
        {
            var ids = new List<string>();

            try
            {
                var map = login.serverMap;

                if (map == null)
                {
                    MelonLogger.Warning("[ServerList] login.serverUIMap is null.");
                    return ids;
                }

                foreach (var pair in map)
                {
                    try
                    {
                        string instanceId = pair.key?.ToString() ?? "";

                        if (!string.IsNullOrWhiteSpace(instanceId))
                            ids.Add(instanceId);
                    }
                    catch (Exception ex)
                    {
                        MelonLogger.Warning("[ServerList] Failed reading serverUIMap key: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Warning("[ServerList] ReadInstanceIdsFromLogin failed: " + ex.Message);
            }

            return ids;
        }
        private static long ExtractInstanceId(object obj)
        {
            if (obj == null)
                return 0;

            long direct = TryExtractInstanceIdDirect(obj);
            if (direct > 0)
                return direct;

            var type = obj.GetType();

            foreach (var f in type.GetFields(
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance))
            {
                try
                {
                    var value = f.GetValue(obj);

                    if (value == null)
                        continue;

                    if (value.GetType().Name.Contains("ServerListEntry"))
                    {
                        long nested = TryExtractInstanceIdDirect(value);

                        if (nested > 0)
                            return nested;
                    }
                }
                catch { }
            }

            return 0;
        }

        private static long TryExtractInstanceIdDirect(object obj)
        {
            var type = obj.GetType();

            string[] names =
            {
        "instance_id",
        "InstanceId",
        "instanceId",
        "_instance_id",
        "<InstanceId>k__BackingField"
    };

            foreach (string name in names)
            {
                try
                {
                    var field = type.GetField(
                        name,
                        System.Reflection.BindingFlags.Public |
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Instance);

                    if (field != null)
                        return Convert.ToInt64(field.GetValue(obj));
                }
                catch { }
            }

            foreach (string name in names)
            {
                try
                {
                    var prop = type.GetProperty(
                        name,
                        System.Reflection.BindingFlags.Public |
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Instance);

                    if (prop != null && prop.CanRead)
                        return Convert.ToInt64(prop.GetValue(obj, null));
                }
                catch { }
            }

            return 0;
        }

        private static List<ServerEntryConfig> Merge(
     List<ServerUiEntry> uiServers,
     List<string> instanceIds)
        {
            var result = new List<ServerEntryConfig>();

            int count = Math.Min(uiServers.Count, instanceIds.Count);

            for (int i = 0; i < count; i++)
            {
                var ui = uiServers[i];

                result.Add(new ServerEntryConfig
                {
                    Index = i,
                    ServerName = ui.ServerName,
                    Region = ui.Region,
                    PingMs = ui.PingMs,
                    Players = ui.Players,
                    InstanceId = instanceIds[i]
                });
            }

            return result;
        }

        private static bool IsPing(string value)
        {
            return Regex.IsMatch(value ?? "", @"^\d+ms$");
        }

        private static int ParsePing(string value)
        {
            value = value.Replace("ms", "").Trim();

            if (int.TryParse(value, out int ping))
                return ping;

            return 0;
        }

        private class ServerUiEntry
        {
            public int Index { get; set; }
            public string ServerName { get; set; } = "";
            public string Region { get; set; } = "";
            public int PingMs { get; set; }
            public int Players { get; set; }
        }

        public class ServerListConfig
        {
            public string UpdatedAt { get; set; } = "";
            public List<ServerEntryConfig> Servers { get; set; } = new();
        }
        public class ServerEntryConfig
        {
            public int Index { get; set; }
            public string ServerName { get; set; } = "";
            public string Region { get; set; } = "";
            public int PingMs { get; set; }
            public int Players { get; set; }
            public string InstanceId { get; set; } = "";
        }
    }
}