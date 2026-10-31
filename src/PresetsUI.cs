using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MissionPresets
{
    internal class PresetsUI : MonoBehaviour
    {
        private static string note;
        private static float noteUntil;

        public static void Notify(string text)
        {
            note = text;
            noteUntil = Time.unscaledTime + 5f;
        }

        private bool visible = true;
        private bool editing;
        private Rect rect = new Rect(Screen.width - 470, 90, 450, 560);
        private Vector2 scroll, editScroll;
        private int selected = -1;
        private string newName = "";
        private GUIStyle head, ok, small;
        private readonly Dictionary<string, string> textCache = new Dictionary<string, string>();

        private PresetStore Store => Plugin.I.Store;

        private void Update()
        {
            if (Plugin.Picker() != null && Plugin.ToggleKey.Value.IsDown())
            {
                visible = !visible;
            }
        }

        private void Styles()
        {
            if (head != null) return;
            head = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, fontSize = 14 };
            ok = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold };
            ok.normal.textColor = new Color(0.45f, 1f, 0.45f);
            small = new GUIStyle(GUI.skin.label) { fontSize = 11, wordWrap = true };
        }

        private void OnGUI()
        {
            var picker = Plugin.Picker();
            if (picker == null) return;
            Styles();
            if (!visible)
            {
                if (GUI.Button(new Rect(Screen.width - 190, 60, 180, 26), L.T("Mission presets (", "Пресеты миссии (") + Plugin.ToggleKey.Value + ")")) visible = true;
                return;
            }
            rect = GUILayout.Window(0x4D5053, rect, Draw, L.T("ORTIZ Ops - MissionPresets — mission settings presets", "ORTIZ Ops - MissionPresets — пресеты настроек миссии"));
            if (editing && selected >= 0 && selected < Store.presets.Count)
            {
                var er = new Rect(rect.x - 470, rect.y, 460, Mathf.Min(Screen.height - rect.y - 20, 760));
                if (er.x < 0) er.x = rect.xMax + 10;
                GUILayout.Window(0x4D5054, er, DrawEditor, L.T("Editor: ", "Редактор: ") + Store.presets[selected].name);
            }
            if (note != null && Time.unscaledTime < noteUntil)
            {
                GUI.Box(new Rect(Screen.width * 0.5f - 300, Screen.height - 90, 600, 30), note);
            }
        }

        private void Draw(int id)
        {
            var picker = Plugin.Picker();
            var m = picker?.Mission;
            GUILayout.Label(L.T("Mission: ", "Миссия: ") + (m != null ? m.Name : L.T("not selected", "не выбрана")), head);

            // ---- auto apply
            GUILayout.BeginHorizontal();
            GUILayout.Label(L.T("Apply automatically:", "Применять автоматически:"), GUILayout.Width(180));
            var names = new List<string> { L.T("off", "выкл") };
            names.AddRange(Store.presets.Select(p => p.name));
            int cur = string.IsNullOrEmpty(Store.autoApply) ? 0 : Mathf.Max(0, names.IndexOf(Store.autoApply));
            if (GUILayout.Button("◄", GUILayout.Width(26))) { int ni = (cur - 1 + names.Count) % names.Count; SetAuto(ni == 0 ? "" : names[ni]); }
            GUILayout.Label(names[cur], cur == 0 ? GUI.skin.label : ok, GUILayout.Width(170));
            if (GUILayout.Button("►", GUILayout.Width(26))) { int ni = (cur + 1) % names.Count; SetAuto(ni == 0 ? "" : names[ni]); }
            GUILayout.EndHorizontal();
            GUILayout.Label(L.T("This preset is applied by itself whenever you select a mission. You can still tweak things in the game's Customize Mission window afterwards.", "При выборе любой миссии этот пресет применяется сам. Потом можно поправить что-то в окне игры «Настроить миссию»."), small);

            // ---- list
            GUILayout.Space(6);
            scroll = GUILayout.BeginScrollView(scroll, GUILayout.ExpandHeight(true));
            if (Store.presets.Count == 0) GUILayout.Label(L.T("No presets yet. Set up a mission in the game's window and press \"Save current mission settings as a new preset\".", "Пресетов пока нет. Настрой миссию в окне игры и нажми «Сохранить текущие настройки миссии как новый пресет»."), small);
            for (int i = 0; i < Store.presets.Count; i++)
            {
                var p = Store.presets[i];
                GUILayout.BeginVertical(GUI.skin.box);
                GUILayout.BeginHorizontal();
                bool sel = GUILayout.Toggle(selected == i, "", GUILayout.Width(18));
                if (sel && selected != i) { selected = i; newName = p.name; textCache.Clear(); }
                GUILayout.Label(p.name + (Store.autoApply == p.name ? L.T("  [auto]", "  [авто]") : ""), selected == i ? ok : GUI.skin.label);
                GUILayout.FlexibleSpace();
                GUI.enabled = m != null;
                if (GUILayout.Button(L.T("Apply", "Применить"), GUILayout.Width(90)))
                {
                    Plugin.ApplyToPicker(p, out var msg);
                    Notify(msg);
                }
                GUI.enabled = true;
                GUILayout.EndHorizontal();
                if (selected == i)
                {
                    GUILayout.Label(L.T("Taken from mission: ", "Снят с миссии: ") + (string.IsNullOrEmpty(p.capturedFrom) ? "—" : p.capturedFrom), small);
                    GUILayout.BeginHorizontal();
                    p.applyParameters = GUILayout.Toggle(p.applyParameters, L.T(" Parameters", " Параметры"));
                    p.applyEnvironment = GUILayout.Toggle(p.applyEnvironment, L.T(" Environment", " Окружение"));
                    p.applyFactions = GUILayout.Toggle(p.applyFactions, L.T(" Factions", " Фракции"));
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUI.enabled = p.applyFactions;
                    p.applySupplies = GUILayout.Toggle(p.applySupplies, L.T(" Faction unit supplies", " Запасы техники фракций"));
                    p.applyRestrictions = GUILayout.Toggle(p.applyRestrictions, L.T(" Aircraft / weapon restrictions", " Запреты самолётов/оружия"));
                    GUI.enabled = true;
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    newName = GUILayout.TextField(newName ?? "", GUILayout.Width(200));
                    if (GUILayout.Button(L.T("Rename", "Переименовать"), GUILayout.Width(110)) && !string.IsNullOrWhiteSpace(newName))
                    {
                        if (Store.autoApply == p.name) Store.autoApply = newName.Trim();
                        p.name = newName.Trim();
                        Plugin.I.Save();
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUI.enabled = m != null;
                    if (GUILayout.Button(L.T("Overwrite from current mission", "Перезаписать из текущей миссии")))
                    {
                        p.RecaptureFrom(m);
                        textCache.Clear();
                        Plugin.I.Save();
                        Notify(L.T($"Preset \"{p.name}\" updated with the settings of \"{m.Name}\"", $"Пресет «{p.name}» обновлён настройками «{m.Name}»"));
                    }
                    GUI.enabled = true;
                    if (GUILayout.Button(editing ? L.T("Close editor", "Закрыть редактор") : L.T("Edit", "Редактировать"), GUILayout.Width(130))) editing = !editing;
                    if (GUILayout.Button(L.T("Delete", "Удалить"), GUILayout.Width(70)))
                    {
                        if (Store.autoApply == p.name) Store.autoApply = "";
                        Store.presets.RemoveAt(i);
                        textCache.Clear();
                        selected = -1;
                        editing = false;
                        Plugin.I.Save();
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                        break;
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndScrollView();

            // ---- new
            GUI.enabled = m != null;
            if (GUILayout.Button(L.T("Save current mission settings as a new preset", "Сохранить текущие настройки миссии как новый пресет"), GUILayout.Height(28)))
            {
                string baseName = m.Name ?? L.T("Preset", "Пресет");
                string name = baseName;
                int k = 2;
                while (Store.presets.Any(x => x.name == name)) name = baseName + " " + k++;
                Store.presets.Add(Preset.Capture(m, name));
                selected = Store.presets.Count - 1;
                newName = name;
                Plugin.I.Save();
                Notify(L.T($"Preset \"{name}\" saved", $"Сохранён пресет «{name}»"));
            }
            GUI.enabled = true;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(L.T("Save changes", "Сохранить изменения"))) { Plugin.I.Save(); Notify(L.T("Presets saved", "Пресеты сохранены")); }
            if (GUILayout.Button(L.Ru ? "Language: English" : "Язык: русский", GUILayout.Width(150))) L.Language.Value = L.Ru ? UiLanguage.English : UiLanguage.Russian;
            if (GUILayout.Button(L.T("Hide (", "Скрыть (") + Plugin.ToggleKey.Value + ")")) visible = false;
            GUILayout.EndHorizontal();
            GUI.DragWindow(new Rect(0, 0, 10000, 22));
        }

        private void SetAuto(string name)
        {
            Store.autoApply = name ?? "";
            Plugin.I.Save();
        }

        // ================= editor (all values of a preset) =================

        private static readonly Dictionary<string, (string en, string ru)> Labels = new Dictionary<string, (string en, string ru)>
        {
            { "allowEventContent", ("Event content", "Ивентовый контент") }, { "playerMode", ("Player mode (0/1/2)", "Режим игроков (0/1/2)") }, { "playerStartingRank", ("Starting rank", "Стартовый ранг") },
            { "rankMultiplier", ("Rank multiplier", "Множитель ранга") }, { "allowRespawn", ("Respawn", "Возрождение") }, { "successfulSortieBonus", ("Successful sortie bonus", "Бонус за успешный вылет") },
            { "nuclearEscalationThreshold", ("Nuclear escalation threshold", "Порог ядерной эскалации") }, { "strategicEscalationThreshold", ("Strategic escalation threshold", "Порог стратегической эскалации") },
            { "minRankTacticalWarhead", ("Min. rank: tactical warheads", "Мин. ранг: тактич. ЯО") }, { "minRankStrategicWarhead", ("Min. rank: strategic warheads", "Мин. ранг: стратег. ЯО") },
            { "wrecksMaxNumber", ("Max. wrecks", "Макс. обломков") }, { "wrecksDecayTime", ("Wreck decay time", "Время исчезновения обломков") },
            { "timeOfDay", ("Time of day (0–24)", "Время суток (0–24)") }, { "timeFactor", ("Time speed", "Скорость времени") }, { "weatherIntensity", ("Weather (0–1)", "Погода (0–1)") },
            { "cloudAltitude", ("Cloud altitude, m", "Высота облаков, м") }, { "windSpeed", ("Wind speed", "Скорость ветра") }, { "windTurbulence", ("Turbulence (0–1)", "Турбулентность (0–1)") },
            { "windHeading", ("Wind heading, °", "Направление ветра, °") }, { "windRandomHeading", ("Heading spread, °", "Разброс направления, °") }, { "moonPhase", ("Moon phase", "Фаза луны") },
            { "preventJoin", ("Prevent players joining", "Запрет входа игрокам") }, { "preventDonation", ("Prevent donations", "Запрет пожертвований") }, { "startingBalance", ("Starting balance", "Стартовая казна") },
            { "playerJoinAllowance", ("Money when a player joins", "Деньги при входе игрока") }, { "playerTaxRate", ("Player tax rate (0–1)", "Налог с игроков (0–1)") }, { "regularIncome", ("Regular income", "Регулярный доход") },
            { "excessFundsDistributePercent", ("Excess funds distribution (0–1)", "Раздача излишков (0–1)") }, { "killReward", ("Kill reward (1 = 100%)", "Награда за уничтожение (1 = 100%)") },
            { "airSkillMultiplier", ("AI skill: aircraft", "Навык ИИ: авиация") }, { "surfaceSkillMultiplier", ("AI skill: ground/naval", "Навык ИИ: наземка/флот") },
            { "startingWarheads", ("Starting warheads", "Стартовые боеголовки") }, { "reserveWarheads", ("Reserve warheads", "Резерв боеголовок") }, { "reserveAirframes", ("Reserve airframes", "Резерв самолётов") },
            { "extraReservesPerPlayer", ("Extra reserves per player", "Доп. резерв на игрока") }, { "AIAircraftLimit", ("AI aircraft limit", "Лимит самолётов ИИ") },
            { "reduceAIPerFriendlyPlayer", ("Less AI per friendly player", "Меньше ИИ за союзного игрока") }, { "addAIPerEnemyPlayer", ("More AI per enemy player", "Больше ИИ за вражеского игрока") }
        };

        private void DrawEditor(int id)
        {
            var p = Store.presets[selected];
            editScroll = GUILayout.BeginScrollView(editScroll, GUILayout.ExpandHeight(true));
            GUILayout.Label(L.T("— Parameters —", "— Параметры —"), head);
            Fields(p.parameters, selected + ".par");
            GUILayout.Label(L.T("— Environment —", "— Окружение —"), head);
            Fields(p.environment, selected + ".env");
            foreach (var f in p.factions)
            {
                GUILayout.Label(L.T("— Faction: ", "— Фракция: ") + f.factionName + " —", head);
                Fields(f, selected + ".fac." + f.factionName);
                GUILayout.Label(L.T($"Unit supplies: {f.supplies.Count} entries, restricted aircraft: {f.restrictedAircraft.Count}, weapons: {f.restrictedWeapons.Count} (changed via \"Overwrite from current mission\")", $"Запасы техники: {f.supplies.Count} позиций, запрещено самолётов: {f.restrictedAircraft.Count}, оружия: {f.restrictedWeapons.Count} (меняются через «Перезаписать из текущей миссии»)"), small);
            }
            GUILayout.EndScrollView();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(L.T("Save", "Сохранить"))) { Plugin.I.Save(); Notify(L.T("Preset saved", "Пресет сохранён")); }
            if (GUILayout.Button(L.T("Close", "Закрыть"))) { Plugin.I.Save(); editing = false; }
            GUILayout.EndHorizontal();
        }

        private void Fields(object obj, string prefix)
        {
            foreach (var fi in obj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (fi.FieldType != typeof(float) && fi.FieldType != typeof(int) && fi.FieldType != typeof(bool)) continue;
                string label = Labels.TryGetValue(fi.Name, out var l) ? L.T(l.en, l.ru) : fi.Name;
                GUILayout.BeginHorizontal();
                GUILayout.Label(label, GUILayout.Width(250));
                if (fi.FieldType == typeof(bool))
                {
                    bool v = (bool)fi.GetValue(obj);
                    bool nv = GUILayout.Toggle(v, v ? L.T(" yes", " да") : L.T(" no", " нет"));
                    if (nv != v) fi.SetValue(obj, nv);
                }
                else
                {
                    string key = prefix + "." + fi.Name;
                    string shown = fi.FieldType == typeof(int)
                        ? ((int)fi.GetValue(obj)).ToString(CultureInfo.InvariantCulture)
                        : ((float)fi.GetValue(obj)).ToString("0.###", CultureInfo.InvariantCulture);
                    if (!textCache.TryGetValue(key, out var text)) text = shown;
                    string nt = GUILayout.TextField(text, GUILayout.Width(120));
                    textCache[key] = nt;
                    string norm = nt.Replace(',', '.');
                    if (fi.FieldType == typeof(int) && int.TryParse(norm, NumberStyles.Integer, CultureInfo.InvariantCulture, out int iv)) fi.SetValue(obj, iv);
                    else if (fi.FieldType == typeof(float) && float.TryParse(norm, NumberStyles.Float, CultureInfo.InvariantCulture, out float fv)) fi.SetValue(obj, fv);
                }
                GUILayout.EndHorizontal();
            }
        }
    }
}
