<div align="center">

# ORTIZ Mission Presets

**Save your favourite mission settings once — apply them to any mission in one click**

[![Game](https://img.shields.io/badge/Nuclear%20Option-0.34-2b5797)](https://store.steampowered.com/app/2168680/Nuclear_Option/)
[![BepInEx](https://img.shields.io/badge/BepInEx-5.4-6f42c1)](https://github.com/BepInEx/BepInEx)
[![NOMM](https://img.shields.io/badge/install%20via-NOMM-2ea44f)](https://github.com/Combat787/NOMM)
[![License](https://img.shields.io/badge/license-MIT-lightgrey)](LICENSE)

**English** · [Русский](#-русский)

</div>

---

Tired of setting starting money, income, AI skill, weather and time of day every time you start **Escalation** or **Terminal Control**? Configure a mission once in the game's own **Customize Mission** window, save it as a preset, and apply it to any mission — by hand or automatically every time you pick one.

> [!NOTE]
> The in-game interface is in **Russian**.

## Features
- 💾 **Save** the current mission settings as a named preset.
- ⚡ **Apply** a preset to the selected mission with one click.
- 🔁 **Auto-apply** — the chosen preset is applied every time you select any mission. You can still tweak things in the game's Customize window afterwards.
- 🧩 **Choose what to apply** per preset: parameters, environment, factions, faction unit supplies, aircraft/weapon restrictions.
- ✏️ **Edit** every value in the built-in editor, **overwrite** a preset from the current mission, **rename** or **delete** it.
- 🛡️ Mission-specific things are never touched: description, tags, camera start positions, roads, units.

## What a preset contains

| Section | Values |
|---|---|
| **Parameters** | Event content, player mode, starting rank, rank multiplier, respawn, successful sortie bonus, tactical / strategic escalation thresholds, minimum rank for tactical / strategic warheads, max wrecks and wreck decay time |
| **Environment** | Time of day, time speed, weather intensity, cloud altitude, wind speed / turbulence / heading / random heading, moon phase |
| **Factions** *(matched by name, e.g. BDF / PALA)* | Starting balance, regular income, player join allowance, player tax rate, excess funds distribution, kill reward, air / surface AI skill, starting and reserve warheads, reserve airframes, extra reserves per player, AI aircraft limit, AI per friendly / enemy player, prevent join / donation |
| **Supplies** *(optional)* | Faction unit supplies (unit type + count) |
| **Restrictions** *(optional)* | Restricted aircraft and weapons |

## How to use

1. Open the mission selection screen. The **ORTIZ Mission Presets** panel appears next to it (**F9** shows / hides it).
2. Pick a mission and set everything up in the game's **Customize Mission** window.
3. Press **Save current mission settings as new preset**.
4. Next time: pick any mission → **Apply** — or choose the preset in **Apply automatically** with ◄ ►, and it will be applied by itself.

### Panel buttons
| Button | Action |
|---|---|
| **Apply automatically ◄ ►** | Choose the preset that is applied to every selected mission (or *off*) |
| **Apply** | Apply the preset to the selected mission now |
| Section checkboxes | Which parts of the preset are applied |
| **Edit** → **Save** | Change any value of the preset |
| **Overwrite from current mission** | Replace the preset's values with the current mission's settings |
| **Rename** / **Delete** | Manage presets |
| **Save current mission settings as new preset** | Create a new preset |

## Settings
| Setting | Default |
|---|---|
| Show / hide panel | **F9** |

Presets are stored in `BepInEx/config/MissionPresets/presets.json` — you can back it up or share it with friends.

## Installation

**Recommended:** install with [Nuclear Option Mod Manager (NOMM)](https://github.com/Combat787/NOMM) — search for **ORTIZ Mission Presets**.

**Manually:** install [BepInEx 5](https://github.com/BepInEx/BepInEx/releases), download `ORTIZ.MissionPresets.dll` from [Releases](../../releases) and put it in `Nuclear Option/BepInEx/plugins/ORTIZ.MissionPresets/`.

> Upgrading from an old build named *MissionPresets*: delete the old `BepInEx/plugins/MissionPresets` folder. Presets and settings are kept.

## Building from source
```
dotnet build -c Release -p:GameDir="C:\Program Files (x86)\Steam\steamapps\common\Nuclear Option"
```

---

<a id="-русский"></a>
## 🇷🇺 Русский

Надоело каждый раз выставлять стартовые деньги, доход, навык ИИ, погоду и время суток перед **Escalation** или **Terminal Control**? Настрой миссию один раз в игровом окне **Customize Mission**, сохрани как пресет и применяй к любой миссии — вручную или автоматически при каждом выборе миссии.

### Возможности
- 💾 **Сохранение** текущих настроек миссии в именованный пресет.
- ⚡ **Применение** пресета к выбранной миссии в один клик.
- 🔁 **Автоприменение** — выбранный пресет применяется к каждой выбранной миссии; потом можно что-то поправить в окне игры.
- 🧩 **Что применять** — для каждого пресета: параметры, окружение, фракции, запасы техники фракций, запреты самолётов/оружия.
- ✏️ **Редактор** всех значений, **перезапись** из текущей миссии, **переименование** и **удаление**.
- 🛡️ Описание, теги, стартовые позиции камеры, дороги и юниты миссии не трогаются.

### Что входит в пресет
| Раздел | Значения |
|---|---|
| **Параметры** | Событийный контент, режим игрока, стартовое звание, множитель званий, респаун, бонус за вылет, пороги тактической/стратегической эскалации, мин. звание для тактических/стратегических боеголовок, обломки |
| **Окружение** | Время суток, скорость времени, погода, высота облаков, ветер (скорость, турбулентность, направление), фаза луны |
| **Фракции** *(по названию)* | Стартовый баланс, доход, налог, награда за уничтожение, навык ИИ (воздух/земля), боеголовки, резервы, лимит самолётов ИИ, запрет вступления/пожертвований |
| **Запасы** *(по желанию)* | Запасы техники фракций |
| **Запреты** *(по желанию)* | Запрещённые самолёты и оружие |

### Как пользоваться
1. Открой экран выбора миссии — рядом появится панель **ORTIZ Mission Presets** (**F9** — показать/скрыть).
2. Выбери миссию и настрой её в игровом окне **Customize Mission**.
3. Нажми **«Сохранить текущие настройки миссии как новый пресет»**.
4. Дальше: выбираешь любую миссию → **«Применить»**, или выбери пресет в **«Применять автоматически»** кнопками ◄ ► — он будет применяться сам.

Пресеты хранятся в `BepInEx/config/MissionPresets/presets.json` — их можно сохранить или отдать друзьям.

### Установка
Через **NOMM** — найди **ORTIZ Mission Presets**. Вручную — положи `ORTIZ.MissionPresets.dll` из [Releases](../../releases) в `BepInEx/plugins/ORTIZ.MissionPresets/`. Если стояла старая версия *MissionPresets* — удали её папку, пресеты сохранятся.

---

<div align="center">

Made by **ORTIZ** · [MIT License](LICENSE)

</div>
