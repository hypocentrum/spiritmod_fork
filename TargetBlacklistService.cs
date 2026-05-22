using System;
using System.Collections.Generic;
using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace SpiritMod
{
    public static class TargetBlacklistService
    {
        private static readonly Dictionary<BaseUnitController, float> _blacklist =
            new Dictionary<BaseUnitController, float>();

        private static float _nextCleanupTime;

        public static int Count
        {
            get { return _blacklist.Count; }
        }

        public static bool IsBlacklisted(BaseUnitController target)
        {
            if (target == null)
                return false;

            float expireTime;
            if (!_blacklist.TryGetValue(target, out expireTime))
                return false;

            if (Time.time >= expireTime)
            {
                _blacklist.Remove(target);
                return false;
            }

            return true;
        }

        public static void Blacklist(BaseUnitController target, float durationSeconds = 60f, string reason = "")
        {
            if (target == null)
                return;

            float duration = Mathf.Max(1f, durationSeconds);
            _blacklist[target] = Time.time + duration;

            string targetName = GetTargetName(target);
            if (string.IsNullOrEmpty(reason))
                MelonLogger.Warning("[Blacklist] Added target '" + targetName + "' for " + duration.ToString("F0") + "s.");
            else
                MelonLogger.Warning("[Blacklist] Added target '" + targetName + "' for " + duration.ToString("F0") + "s. Reason: " + reason);
        }

        public static void Remove(BaseUnitController target)
        {
            if (target == null)
                return;

            _blacklist.Remove(target);
        }

        public static void Clear()
        {
            _blacklist.Clear();
            _nextCleanupTime = 0f;
        }

        public static void CleanupExpired(float intervalSeconds = 3f)
        {
            if (_blacklist.Count == 0)
                return;

            float now = Time.time;
            if (now < _nextCleanupTime)
                return;

            _nextCleanupTime = now + Mathf.Max(0.5f, intervalSeconds);

            List<BaseUnitController> expired = null;

            foreach (KeyValuePair<BaseUnitController, float> pair in _blacklist)
            {
                if (pair.Key == null || now >= pair.Value)
                {
                    if (expired == null)
                        expired = new List<BaseUnitController>();

                    expired.Add(pair.Key);
                }
            }

            if (expired == null)
                return;

            foreach (BaseUnitController target in expired)
                _blacklist.Remove(target);
        }

        private static string GetTargetName(BaseUnitController target)
        {
            if (target == null)
                return "Unknown";

            try
            {
                string objectName = target.name;
                if (!string.IsNullOrEmpty(objectName))
                    return objectName;
            }
            catch
            {
            }

            return "Unknown";
        }
    }
}
