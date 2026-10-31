using System;
using System.Collections.Generic;
using System.Linq;
using NuclearOption.SavedMission;

namespace MissionPresets
{
    /// <summary>What the game's "Customize mission" window edits, minus mission-specific things
    /// (description, tags, camera start positions, roads).</summary>
    [Serializable]
    public class ParametersData
    {
        public bool allowEventContent;
        public int playerMode;
        public int playerStartingRank;
        public float rankMultiplier = 1f;
        public bool allowRespawn;
        public float successfulSortieBonus = 0.25f;
        public float nuclearEscalationThreshold;
        public float strategicEscalationThreshold;
        public int minRankTacticalWarhead;
        public int minRankStrategicWarhead;
        public int wrecksMaxNumber;
        public float wrecksDecayTime;

        public static ParametersData From(MissionSettings s) => new ParametersData
        {
            allowEventContent = s.allowEventContent,
            playerMode = (int)s.playerMode,
            playerStartingRank = s.playerStartingRank,
            rankMultiplier = s.rankMultiplier,
            allowRespawn = s.allowRespawn,
            successfulSortieBonus = s.successfulSortieBonus,
            nuclearEscalationThreshold = s.nuclearEscalationThreshold,
            strategicEscalationThreshold = s.strategicEscalationThreshold,
            minRankTacticalWarhead = s.minRankTacticalWarhead,
            minRankStrategicWarhead = s.minRankStrategicWarhead,
            wrecksMaxNumber = s.wrecksMaxNumber,
            wrecksDecayTime = s.wrecksDecayTime
        };

        public void ApplyTo(MissionSettings s)
        {
            s.allowEventContent = allowEventContent;
            s.playerMode = (PlayerMode)playerMode;
            s.playerStartingRank = playerStartingRank;
            s.rankMultiplier = rankMultiplier;
            s.allowRespawn = allowRespawn;
            s.successfulSortieBonus = successfulSortieBonus;
            s.nuclearEscalationThreshold = nuclearEscalationThreshold;
            s.strategicEscalationThreshold = strategicEscalationThreshold;
            s.minRankTacticalWarhead = minRankTacticalWarhead;
            s.minRankStrategicWarhead = minRankStrategicWarhead;
            s.wrecksMaxNumber = wrecksMaxNumber;
            s.wrecksDecayTime = wrecksDecayTime;
        }
    }

    [Serializable]
    public class EnvironmentData
    {
        public float timeOfDay = 10f;
        public float timeFactor;
        public float weatherIntensity;
        public float cloudAltitude = 1800f;
        public float windSpeed;
        public float windTurbulence;
        public float windHeading;
        public float windRandomHeading;
        public float moonPhase = 14f;

        public static EnvironmentData From(MissionEnvironment e) => new EnvironmentData
        {
            timeOfDay = e.timeOfDay,
            timeFactor = e.timeFactor,
            weatherIntensity = e.weatherIntensity,
            cloudAltitude = e.cloudAltitude,
            windSpeed = e.windSpeed,
            windTurbulence = e.windTurbulence,
            windHeading = e.windHeading,
            windRandomHeading = e.windRandomHeading,
            moonPhase = e.moonPhase
        };

        public void ApplyTo(MissionEnvironment e)
        {
            e.timeOfDay = timeOfDay;
            e.timeFactor = timeFactor;
            e.weatherIntensity = weatherIntensity;
            e.cloudAltitude = cloudAltitude;
            e.windSpeed = windSpeed;
            e.windTurbulence = windTurbulence;
            e.windHeading = windHeading;
            e.windRandomHeading = windRandomHeading;
            e.moonPhase = moonPhase;
        }
    }

    [Serializable]
    public class SupplyData
    {
        public string unit = "";
        public int count;
    }

    [Serializable]
    public class FactionData
    {
        public string factionName = "";
        public bool preventJoin;
        public bool preventDonation;
        public float startingBalance;
        public float playerJoinAllowance = 20f;
        public float playerTaxRate = 0.2f;
        public float regularIncome = 5f;
        public float excessFundsDistributePercent = 0.25f;
        public float killReward = 1f;
        public float airSkillMultiplier = 1f;
        public float surfaceSkillMultiplier = 1f;
        public int startingWarheads;
        public int reserveWarheads;
        public int reserveAirframes;
        public int extraReservesPerPlayer = 1;
        public int AIAircraftLimit = 6;
        public float reduceAIPerFriendlyPlayer = 1f;
        public float addAIPerEnemyPlayer = 1f;
        public List<SupplyData> supplies = new List<SupplyData>();
        public List<string> restrictedAircraft = new List<string>();
        public List<string> restrictedWeapons = new List<string>();

        public static FactionData From(MissionFaction f) => new FactionData
        {
            factionName = f.factionName,
            preventJoin = f.preventJoin,
            preventDonation = f.preventDonation,
            startingBalance = f.startingBalance,
            playerJoinAllowance = f.playerJoinAllowance,
            playerTaxRate = f.playerTaxRate,
            regularIncome = f.regularIncome,
            excessFundsDistributePercent = f.excessFundsDistributePercent,
            killReward = f.killReward,
            airSkillMultiplier = f.airSkillMultiplier,
            surfaceSkillMultiplier = f.surfaceSkillMultiplier,
            startingWarheads = f.startingWarheads,
            reserveWarheads = f.reserveWarheads,
            reserveAirframes = f.reserveAirframes,
            extraReservesPerPlayer = f.extraReservesPerPlayer,
            AIAircraftLimit = f.AIAircraftLimit,
            reduceAIPerFriendlyPlayer = f.reduceAIPerFriendlyPlayer,
            addAIPerEnemyPlayer = f.addAIPerEnemyPlayer,
            supplies = (f.supplies ?? new List<UnitCount>()).Where(u => u != null).Select(u => new SupplyData { unit = u.UnitType, count = u.Count }).ToList(),
            restrictedAircraft = f.restrictions?.aircraft != null ? new List<string>(f.restrictions.aircraft) : new List<string>(),
            restrictedWeapons = f.restrictions?.weapons != null ? new List<string>(f.restrictions.weapons) : new List<string>()
        };

        public void ApplyTo(MissionFaction f, bool supplies, bool restrictions)
        {
            f.preventJoin = preventJoin;
            f.preventDonation = preventDonation;
            f.startingBalance = startingBalance;
            f.playerJoinAllowance = playerJoinAllowance;
            f.playerTaxRate = playerTaxRate;
            f.regularIncome = regularIncome;
            f.excessFundsDistributePercent = excessFundsDistributePercent;
            f.killReward = killReward;
            f.airSkillMultiplier = airSkillMultiplier;
            f.surfaceSkillMultiplier = surfaceSkillMultiplier;
            f.startingWarheads = startingWarheads;
            f.reserveWarheads = reserveWarheads;
            f.reserveAirframes = reserveAirframes;
            f.extraReservesPerPlayer = extraReservesPerPlayer;
            f.AIAircraftLimit = AIAircraftLimit;
            f.reduceAIPerFriendlyPlayer = reduceAIPerFriendlyPlayer;
            f.addAIPerEnemyPlayer = addAIPerEnemyPlayer;
            if (supplies)
            {
                f.supplies = this.supplies.Select(s => new UnitCount(s.unit, s.count)).ToList();
            }
            if (restrictions)
            {
                if (f.restrictions == null) f.restrictions = new Restrictions();
                f.restrictions.aircraft = new List<string>(restrictedAircraft);
                f.restrictions.weapons = new List<string>(restrictedWeapons);
            }
        }
    }

    [Serializable]
    public class Preset
    {
        public string name = "New preset";
        public string capturedFrom = "";

        // which parts to apply
        public bool applyParameters = true;
        public bool applyEnvironment = true;
        public bool applyFactions = true;
        public bool applySupplies = true;
        public bool applyRestrictions = true;

        public ParametersData parameters = new ParametersData();
        public EnvironmentData environment = new EnvironmentData();
        public List<FactionData> factions = new List<FactionData>();

        public static Preset Capture(Mission m, string name)
        {
            return new Preset
            {
                name = name,
                capturedFrom = m.Name ?? "",
                parameters = ParametersData.From(m.missionSettings),
                environment = EnvironmentData.From(m.environment),
                factions = m.factions.Where(f => f != null).Select(FactionData.From).ToList()
            };
        }

        /// <summary>Copies the chosen sections into the mission. Factions are matched by name.</summary>
        public string ApplyTo(Mission m)
        {
            var report = new List<string>();
            if (applyParameters)
            {
                parameters.ApplyTo(m.missionSettings);
                report.Add(L.T("parameters", "параметры"));
            }
            if (applyEnvironment)
            {
                environment.ApplyTo(m.environment);
                report.Add(L.T("environment", "окружение"));
            }
            if (applyFactions)
            {
                int n = 0;
                foreach (var fd in factions)
                {
                    if (m.TryGetFaction(fd.factionName, out var mf) && mf != null)
                    {
                        fd.ApplyTo(mf, applySupplies, applyRestrictions);
                        n++;
                    }
                }
                report.Add(L.T($"factions ({n})", $"фракции ({n})"));
            }
            return string.Join(", ", report);
        }

        /// <summary>Overwrite values from a mission but keep this preset's name and section switches.</summary>
        public void RecaptureFrom(Mission m)
        {
            var p = Capture(m, name);
            capturedFrom = p.capturedFrom;
            parameters = p.parameters;
            environment = p.environment;
            factions = p.factions;
        }
    }
}
