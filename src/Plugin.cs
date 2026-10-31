using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Newtonsoft.Json;
using NuclearOption.SavedMission;
using UnityEngine;

namespace MissionPresets
{
    [Serializable]
    public class PresetStore
    {
        public string autoApply = "";
        public List<Preset> presets = new List<Preset>();
    }

    [BepInPlugin("ortiz.missionpresets", "ORTIZ Ops - MissionPresets", Version)]
    public class Plugin : BaseUnityPlugin
    {
        public const string Version = "1.1.0";
        internal static ManualLogSource Log;
        internal static Plugin I;
        internal static ConfigEntry<KeyboardShortcut> ToggleKey;

        internal PresetStore Store = new PresetStore();
        private string storePath;

        private void Awake()
        {
            Log = Logger;
            I = this;
            try
            {
                string oldCfg = Path.Combine(Paths.ConfigPath, "com.nikita.missionpresets.cfg");
                if (File.Exists(oldCfg) && !File.Exists(Config.ConfigFilePath + ".migrated"))
                {
                    File.Copy(oldCfg, Config.ConfigFilePath, true);
                    File.WriteAllText(Config.ConfigFilePath + ".migrated", "copied");
                    Config.Reload();
                }
            }
            catch
            {
            }
            L.BindLanguage(Config);
            ToggleKey = L.Bind(Config, "Controls", "Управление", "Show / hide panel", "Показать/скрыть панель", new KeyboardShortcut(KeyCode.F9),
                "Presets panel on the mission selection screen", "Панель пресетов на экране выбора миссии");
            storePath = Path.Combine(Paths.ConfigPath, "MissionPresets", "presets.json");
            Load();
            try
            {
                new Harmony("ortiz.missionpresets").PatchAll(typeof(Plugin).Assembly);
            }
            catch (Exception e)
            {
                Log.LogError("Harmony: " + e);
            }
            var go = new GameObject("[MissionPresets]");
            DontDestroyOnLoad(go);
            go.hideFlags = HideFlags.HideAndDontSave;
            go.AddComponent<PresetsUI>();
            Log.LogInfo($"ORTIZ Ops - MissionPresets v{Version} loaded, {Store.presets.Count} presets");
        }

        internal void Load()
        {
            try
            {
                if (File.Exists(storePath))
                {
                    Store = JsonConvert.DeserializeObject<PresetStore>(File.ReadAllText(storePath)) ?? new PresetStore();
                }
            }
            catch (Exception e)
            {
                Log.LogError("Presets load failed: " + e.Message);
                Store = new PresetStore();
            }
        }

        internal void Save()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(storePath));
                File.WriteAllText(storePath, JsonConvert.SerializeObject(Store, Formatting.Indented));
            }
            catch (Exception e)
            {
                Log.LogError("Presets save failed: " + e.Message);
            }
        }

        internal static MissionPicker Picker()
        {
            try
            {
                var p = SceneSingleton<MissionPicker>.i;
                return p != null && p.isActiveAndEnabled ? p : null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>Applies the preset to the mission chosen in the picker, the same way the game's own
        /// "Customize mission" window stores its changes (a temp copy of the mission).</summary>
        internal static bool ApplyToPicker(Preset preset, out string message)
        {
            message = null;
            var picker = SceneSingleton<MissionPicker>.i;
            var m = picker != null ? picker.Mission : null;
            if (m == null)
            {
                message = L.T("Select a mission first", "Сначала выбери миссию");
                return false;
            }
            try
            {
                // the game starts exactly this Mission object (SinglePlayerMenu / lobby), so editing it in place is enough
                string what = preset.ApplyTo(m);
                // keep the game's "Customize mission" window pointed at the same object, so it shows the preset values
                try
                {
                    foreach (var menu in Resources.FindObjectsOfTypeAll<CustomizeMissionMenu>())
                    {
                        if (menu != null && menu.gameObject.scene.IsValid()) menu.SetMission(m);
                    }
                }
                catch
                {
                }
                message = L.T($"Preset \"{preset.name}\" applied to \"{m.Name}\": {what}", $"Пресет «{preset.name}» применён к «{m.Name}»: {what}");
                Log.LogInfo(message);
                return true;
            }
            catch (Exception e)
            {
                Log.LogError("Apply failed: " + e);
                message = L.T("Error: ", "Ошибка: ") + e.Message;
                return false;
            }
        }
    }

    /// <summary>Auto-apply the chosen preset whenever a mission is selected.</summary>
    [HarmonyPatch(typeof(MissionPicker), nameof(MissionPicker.SelectMission))]
    internal static class MissionPicker_SelectMission_Patch
    {
        private static void Postfix(MissionPicker __instance)
        {
            try
            {
                var store = Plugin.I?.Store;
                if (store == null || string.IsNullOrEmpty(store.autoApply) || __instance.Mission == null) return;
                var p = store.presets.FirstOrDefault(x => x.name == store.autoApply);
                if (p == null) return;
                if (Plugin.ApplyToPicker(p, out var msg)) PresetsUI.Notify(msg);
            }
            catch (Exception e)
            {
                Plugin.Log.LogWarning("Auto-apply: " + e.Message);
            }
        }
    }
}
