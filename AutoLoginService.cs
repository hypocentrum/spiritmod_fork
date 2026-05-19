using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace SpiritMod
{
    public static class AutoLoginService
    {
        public static string StatusMessage { get; private set; } = "";

        private static readonly string ServerConfigPath =
            Path.Combine("UserData", "SpiritMod", "servers.json");

        public static string GetInstanceId(string serverName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(serverName))
                    return null;

                if (!File.Exists(ServerConfigPath))
                {
                    MelonLogger.Warning("[AutoLogin] servers.json not found.");
                    return null;
                }

                var json = File.ReadAllText(ServerConfigPath);
                var config = JsonSerializer.Deserialize<ServerListConfig>(json);

                if (config?.Servers == null)
                    return null;

                foreach (var server in config.Servers)
                {
                    if (server == null)
                        continue;

                    if (string.Equals(server.ServerName, serverName, StringComparison.OrdinalIgnoreCase))
                        return server.InstanceId;
                }

                MelonLogger.Warning("[AutoLogin] Server name not found in servers.json: " + serverName);
                return null;
            }
            catch (Exception ex)
            {
                MelonLogger.Warning("[AutoLogin] Failed reading servers.json: " + ex.Message);
                return null;
            }
        }

        public static void CharacterSelection(UIManager uiManager)
        {
            try
            {
                var current = uiManager?.Current;
                var characterSelect = current != null
                    ? current.GetComponent<UICharacterSelect>()
                    : null;

                if (characterSelect == null)
                {
                    MelonLogger.Error("[AutoLogin] UICharacterSelect component not found.");
                    StatusMessage = "Error: UICharacterSelect not found";
                    return;
                }

                string characterName = ConfigService.CharacterName;

                if (string.IsNullOrEmpty(characterName))
                {
                    MelonLogger.Warning("[AutoLogin] No character name configured.");
                    StatusMessage = "No character name set";
                    return;
                }

                var character = FindCharacterByName(characterName);

                if (character == null)
                {
                    MelonLogger.Error("[AutoLogin] Character '" + characterName + "' not found.");
                    StatusMessage = "Character '" + characterName + "' not found";
                    return;
                }

                characterSelect.PlayCharacter(character);
                MelonLogger.Msg("[AutoLogin] Selected character: " + character.Name);
                StatusMessage = "Selected '" + character.Name + "'";
            }
            catch (Exception ex)
            {
                MelonLogger.Warning("[AutoLogin] CharacterSelection failed: " + ex.Message);
                StatusMessage = "Error: " + ex.Message;
            }
        }

        private static CharacterData FindCharacterByName(string name)
        {
            if (!GameCache.Characters.IsValid())
                return null;

            var characters = GameCache.Characters.Value;

            for (int i = 0; i < characters.Count; i++)
            {
                CharacterData character = characters[i];

                if (character != null &&
                    string.Equals(character.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    return character;
                }
            }

            return null;
        }

        public static void ServerAutoConnect()
        {
            MelonCoroutines.Start(ServerAutoConnectCoroutine());
        }

        private static IEnumerator ServerAutoConnectCoroutine()
        {
            StatusMessage = "Waiting for server list...";
            MelonLogger.Msg("[AutoLogin] Waiting for server list to populate...");

            UILogin uiLogin = null;
            dynamic serverMap = null;

            float elapsed = 0f;

            while (elapsed < 10f)
            {
                yield return new WaitForSeconds(0.5f);
                elapsed += 0.5f;

                try
                {
                    uiLogin = UnityEngine.Object.FindObjectOfType<UILogin>();

                    if (uiLogin == null)
                    {
                        MelonLogger.Msg("[AutoLogin] UILogin disappeared, aborting.");
                        StatusMessage = "";
                        yield break;
                    }

                    serverMap = uiLogin.serverUIMap;

                    if (serverMap != null && serverMap.Count > 0)
                        break;
                }
                catch
                {
                }
            }

            if (serverMap == null || serverMap.Count == 0)
            {
                MelonLogger.Warning("[AutoLogin] Server list never populated.");
                StatusMessage = "Server list timeout";
                yield break;
            }

            try
            {
                string wantedServerName = BotController.Config.AutoLoginServerName;
                string instanceId = GetInstanceId(wantedServerName);

                if (string.IsNullOrEmpty(instanceId))
                {
                    foreach (var pair in serverMap)
                    {
                        instanceId = pair.key.ToString();
                        break;
                    }

                    MelonLogger.Msg(
                        $"[AutoLogin] Server '{wantedServerName}' unknown, falling back to instance '{instanceId}'."
                    );
                }
                else
                {
                    MelonLogger.Msg(
                        $"[AutoLogin] Selecting server '{wantedServerName}' with instance id '{instanceId}'."
                    );
                }

                StatusMessage = $"Connecting to '{wantedServerName}'...";

                uiLogin.SetSelected(instanceId);
                uiLogin.Connect();

                MelonLogger.Msg("[AutoLogin] Connect called.");
                StatusMessage = "Connecting...";
            }
            catch (Exception ex)
            {
                MelonLogger.Warning("[AutoLogin] Server selection failed: " + ex.Message);
                StatusMessage = "Error: " + ex.Message;
            }
        }
        public static string[] GetServerNames()
        {
            try
            {
                if (!File.Exists(ServerConfigPath))
                    return Array.Empty<string>();

                var json = File.ReadAllText(ServerConfigPath);
                var config = JsonSerializer.Deserialize<ServerListConfig>(json);

                if (config?.Servers == null)
                    return Array.Empty<string>();

                var names = new List<string>();

                foreach (var server in config.Servers)
                {
                    if (!string.IsNullOrWhiteSpace(server.ServerName))
                        names.Add(server.ServerName);
                }

                return names.ToArray();
            }
            catch (Exception ex)
            {
                MelonLogger.Warning("[AutoLogin] Failed loading server names: " + ex.Message);
                return Array.Empty<string>();
            }
        }
        private class ServerListConfig
        {
            public string UpdatedAt { get; set; } = "";
            public List<ServerEntryConfig> Servers { get; set; } = new();
        }

        private class ServerEntryConfig
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