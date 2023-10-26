﻿using Microsoft.AspNetCore.WebUtilities;
using System.Globalization;
using System.Net.Http.Headers;

namespace SharpTournament
{
    internal class ApiStats : IDisposable
    {
        private readonly HttpClient _HttpClient;

        public ApiStats(string apiStatsUrl, string apiStatsKey)
        {
            _HttpClient = new HttpClient()
            {
                BaseAddress = new Uri(apiStatsUrl)
            };

            _HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiStatsKey);
        }

        public async Task SendGoingLiveAsync(GoingLiveParams goingLiveParams, CancellationToken cancellationToken)
        {
            var queryParams = new Dictionary<string, string>
            {
                {ApiStatsConstants.StatsMapName, goingLiveParams.MapName}
            };

            var uri = QueryHelpers.AddQueryString($"/golive/{goingLiveParams.MapNumber}", queryParams);

            var response = await _HttpClient.PostAsync(uri, null, cancellationToken);

            await HandleResponseAsync(response, cancellationToken);
        }

        public async Task SendMapResultAsync(MapResultParams mapResultParams, CancellationToken cancellationToken)
        {
            var queryParams = new Dictionary<string, string>
            {
                {"team1score", CreateIntParam(mapResultParams.Team1Score)},
                {"team2score", CreateIntParam(mapResultParams.Team2Score)},
                {ApiStatsConstants.StatsMapWinner, mapResultParams.WinnerTeamName}
            };

            var uri = QueryHelpers.AddQueryString($"/finalize/{mapResultParams.MapNumber}", queryParams);

            var response = await _HttpClient.PostAsync(uri, null, cancellationToken);

            await HandleResponseAsync(response, cancellationToken);
        }

        public async Task SendRoundStatsUpdateAsync(RoundStatusUpdateParams roundStatusUpdateParams, CancellationToken cancellationToken)
        {
            var queryParams = new Dictionary<string, string>
            {
                {"team1score", CreateIntParam(roundStatusUpdateParams.CurrentMap.Team1.Score)},
                {"team2score", CreateIntParam(roundStatusUpdateParams.CurrentMap.Team2.Score)},
            };

            var uri = QueryHelpers.AddQueryString($"/updateround/{roundStatusUpdateParams.MapNumber}", queryParams);

            var response = await _HttpClient.PostAsync(uri, null, cancellationToken);

            await HandleResponseAsync(response, cancellationToken);

            await UpdatePlayerStatsInternalAsync(roundStatusUpdateParams.MapNumber, roundStatusUpdateParams.TeamInfo1, roundStatusUpdateParams.TeamInfo2, roundStatusUpdateParams.CurrentMap, cancellationToken);
        }

        private async Task UpdatePlayerStatsInternalAsync(int mapNumber, TeamInfo teamInfo1, TeamInfo teamInfo2, Map currentMap, CancellationToken cancellationToken)
        {
            var dict = new Dictionary<string, MapTeamInfo>()
            {
                { teamInfo1.TeamName, currentMap.Team1 },
                { teamInfo2.TeamName, currentMap.Team2 },
            };

            foreach (var team in dict)
            {
                var teamName = team.Key;

                var players = team.Value.Players;

                foreach (var player in players)
                {
                    var playerStatistics = player.Value;

                    var queryParams = new Dictionary<string, string>
                    {
                        {ApiStatsConstants.StatsTeamName, teamName},
                        {ApiStatsConstants.StatsName, playerStatistics.Name},
                        {ApiStatsConstants.StatsKills, CreateIntParam(playerStatistics.Kills)},
                        {ApiStatsConstants.StatsDeaths, CreateIntParam(playerStatistics.Deaths)},
                        {ApiStatsConstants.StatsAssists, CreateIntParam(playerStatistics.Assists)},
                        {ApiStatsConstants.StatsFlashbangAssists, CreateIntParam(playerStatistics.FlashbangAssists)},
                        {ApiStatsConstants.StatsTeamKills, CreateIntParam(playerStatistics.TeamKills)},
                        {ApiStatsConstants.StatsSuicides, CreateIntParam(playerStatistics.Suicides)},
                        {ApiStatsConstants.StatsDamage, CreateIntParam(playerStatistics.Damage)},
                        {ApiStatsConstants.StatsUtilityDamage, CreateIntParam(playerStatistics.UtilityDamage)},
                        {ApiStatsConstants.StatsEnemiesFlashed, CreateIntParam(playerStatistics.EnemiesFlashed)},
                        {ApiStatsConstants.StatsFriendliesFlashed, CreateIntParam(playerStatistics.FriendliesFlashed)},
                        {ApiStatsConstants.StatsKnifeKills, CreateIntParam(playerStatistics.KnifeKills)},
                        {ApiStatsConstants.StatsHeadshotKills, CreateIntParam(playerStatistics.HeadshotKills)},
                        {ApiStatsConstants.StatsRoundsPlayed, CreateIntParam(playerStatistics.RoundsPlayed)},
                        {ApiStatsConstants.StatsBombPlants, CreateIntParam(playerStatistics.BombPlants)},
                        {ApiStatsConstants.StatsBombDefuses, CreateIntParam(playerStatistics.BombDefuses)},
                        {ApiStatsConstants.Stats1K, CreateIntParam(playerStatistics.Count1K)},
                        {ApiStatsConstants.Stats2K, CreateIntParam(playerStatistics.Count2K)},
                        {ApiStatsConstants.Stats3K, CreateIntParam(playerStatistics.Count3K)},
                        {ApiStatsConstants.Stats4K, CreateIntParam(playerStatistics.Count4K)},
                        {ApiStatsConstants.Stats5K, CreateIntParam(playerStatistics.Count5K)},
                        {ApiStatsConstants.StatsV1, CreateIntParam(playerStatistics.V1)},
                        {ApiStatsConstants.StatsV2, CreateIntParam(playerStatistics.V2)},
                        {ApiStatsConstants.StatsV3, CreateIntParam(playerStatistics.V3)},
                        {ApiStatsConstants.StatsV4, CreateIntParam(playerStatistics.V4)},
                        {ApiStatsConstants.StatsV5, CreateIntParam(playerStatistics.V5)},
                        {ApiStatsConstants.StatsFirstKillT, CreateIntParam(playerStatistics.FirstKillT)},
                        {ApiStatsConstants.StatsFirstKillCt, CreateIntParam(playerStatistics.FirstKillCt)},
                        {ApiStatsConstants.StatsFirstDeathT, CreateIntParam(playerStatistics.FirstDeathT)},
                        {ApiStatsConstants.StatsFirstDeathCt, CreateIntParam(playerStatistics.FirstDeathCt)},
                        {ApiStatsConstants.StatsTradeKill, CreateIntParam(playerStatistics.TradeKill)},
                        {ApiStatsConstants.StatsKast, CreateIntParam(playerStatistics.Kast)},
                        {ApiStatsConstants.StatsContributionScore, CreateIntParam(playerStatistics.ContributionScore)},
                        {ApiStatsConstants.StatsMvp, CreateIntParam(playerStatistics.Mvp)},
                    };

                    var uri = QueryHelpers.AddQueryString($"/updateplayer/{mapNumber}", queryParams);

                    var response = await _HttpClient.PostAsync(uri, null, cancellationToken);

                    await HandleResponseAsync(response, cancellationToken);
                }
            }
        }

        internal static string CreateIntParam(int param)
        {
            return param.ToString(CultureInfo.InvariantCulture);
        }


        public async Task SendSeriesResultAsync(SeriesResultParams seriesResultParams, CancellationToken cancellationToken)
        {
            var queryParams = new Dictionary<string, string>
            {
                {ApiStatsConstants.StatsSeriesWinner, seriesResultParams.WinnerTeamName},
                {ApiStatsConstants.StatsSeriesForfeit, CreateIntParam(Convert.ToInt32(seriesResultParams.Forfeit))},
            };

            var uri = QueryHelpers.AddQueryString($"/finalize", queryParams);

            var response = await _HttpClient.PostAsync(uri, null, cancellationToken);

            await HandleResponseAsync(response, cancellationToken);

            // Wait before freeing server
            await Task.Delay(TimeSpan.FromMilliseconds(seriesResultParams.TimeBeforeFreeingServerMs), cancellationToken);

            await SendFreeServerInternalAsync(cancellationToken);
        }

        internal async Task SendFreeServerInternalAsync(CancellationToken cancellationToken)
        {
            var response = await _HttpClient.PostAsync(new Uri("/freeserver"), null, cancellationToken);

            await HandleResponseAsync(response, cancellationToken);
        }

        internal static async Task HandleResponseAsync(HttpResponseMessage? httpResponseMessage, CancellationToken cancellationToken)
        {

            if (httpResponseMessage == null)
            {
                return;
            }

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                Console.WriteLine($"API request failed, HTTP status code = {httpResponseMessage.StatusCode}");

                try
                {
                    var responseContent = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);

                    Console.WriteLine($"ResponseContent: {responseContent}");
                }
                catch (Exception)
                {
                    // TODO
                }
            }

        }

        public void Dispose()
        {
            _HttpClient.Dispose();
        }

        private static class ApiStatsConstants
        {
            // Series stats(root section)
            public const string StatsSeriesWinner = "winner";
            //public const string StatsSeriesType = "series_type";
            //public const string StatsSeriesTeamId = "id";
            //public const string StatsSeriesTeamName = "name";
            public const string StatsSeriesForfeit = "forfeit";

            // Map stats (under "map0", "map1", etc.)
            public const string StatsMapName = "mapname";
            public const string StatsMapWinner = "winner";
            //public const string StatsDemoFilename = "demo_filename";

            // Team stats (under map section, then "team1" or "team2")
            //public const string StatsTeamScore = "score";
            //public const string StatsTeamScoreCt = "score_ct";
            //public const string StatsTeamScoreT = "score_t";
            //public const string StatsStartingSide = "starting_side";

            public const string StatsTeamName = "team";

            // Player stats (under map section, then team section, then player's steam64)
            // If adding stuff here, also add to the InitPlayerStats function!
            //public const string StatsInit = "init"; // used to zero-fill stats only. Not a real stat.
            public const string StatsCoaching = "coaching"; // indicates if the player is a coach.
            public const string StatsName = "name";
            public const string StatsKills = "kills";
            public const string StatsDeaths = "deaths";
            public const string StatsAssists = "assists";
            public const string StatsFlashbangAssists = "flashbang_assists";
            public const string StatsTeamKills = "teamkills";
            public const string StatsSuicides = "suicides";
            public const string StatsDamage = "damage";
            public const string StatsUtilityDamage = "util_damage";
            public const string StatsEnemiesFlashed = "enemies_flashed";
            public const string StatsFriendliesFlashed = "friendlies_flashed";
            public const string StatsKnifeKills = "knife_kills";
            public const string StatsHeadshotKills = "headshot_kills";
            public const string StatsRoundsPlayed = "roundsplayed";
            public const string StatsBombDefuses = "bomb_defuses";
            public const string StatsBombPlants = "bomb_plants";
            public const string Stats1K = "1kill_rounds";
            public const string Stats2K = "2kill_rounds";
            public const string Stats3K = "3kill_rounds";
            public const string Stats4K = "4kill_rounds";
            public const string Stats5K = "5kill_rounds";
            public const string StatsV1 = "v1";
            public const string StatsV2 = "v2";
            public const string StatsV3 = "v3";
            public const string StatsV4 = "v4";
            public const string StatsV5 = "v5";
            public const string StatsFirstKillT = "firstkill_t";
            public const string StatsFirstKillCt = "firstkill_ct";
            public const string StatsFirstDeathT = "firstdeath_t";
            public const string StatsFirstDeathCt = "firstdeath_ct";
            public const string StatsTradeKill = "tradekill";
            public const string StatsKast = "kast";
            public const string StatsContributionScore = "contribution_score";
            public const string StatsMvp = "mvp";
        }
    }

    internal record GoingLiveParams(string MapName, int MapNumber);

    internal record MapResultParams(string WinnerTeamName, int Team1Score, int Team2Score, int MapNumber);

    internal record SeriesResultParams(string WinnerTeamName, bool Forfeit, uint TimeBeforeFreeingServerMs);

    internal record RoundStatusUpdateParams(int MapNumber, TeamInfo TeamInfo1, TeamInfo TeamInfo2, Map CurrentMap);

    public class Series
    {
        public Dictionary<string, Map> Maps { get; set; }

        public TeamInfo Team1 { get; set; }

        public TeamInfo Team2 { get; set; }

        public string SeriesType => $"bo{Maps.Count}";

        public Config.Team Winner { get; set; }

        public bool Forfeit { get; set; }

        public Map GetMap(int mapNumber)
        {
            var mapNumberString = $"map{mapNumber}";

            return Maps[mapNumberString];
        }
    }

    public class TeamInfo
    {
        public string Id { get; set; }

        public string TeamName { get; set; }
    }

    public class Map
    {
        public string Name { get; set; }

        public string DemoFileName { get; set; }

        public Config.Team Winner { get; set; }

        public MapTeamInfo Team1 { get; set; }

        public MapTeamInfo Team2 { get; set; }
    }

    public class MapTeamInfo
    {
        public Dictionary<SteamId, PlayerStatistics> Players { get; set; }

        public int Score { get; set; }

        public int ScoreT { get; set; }

        public int ScoreCT { get; set; }

        public StartingSide StartingSide { get; set; }
    }

    public class PlayerStatistics
    {
        public bool Coaching { get; set; }

        public string Name { get; set; }

        public int Kills { get; set; }

        public int Deaths { get; set; }

        public int Assists { get; set; }

        public int FlashbangAssists { get; set; }

        public int TeamKills { get; set; }

        public int Suicides { get; set; }

        public int Damage { get; set; }

        public int UtilityDamage { get; set; }

        public int EnemiesFlashed { get; set; }

        public int FriendliesFlashed { get; set; }

        public int KnifeKills { get; set; }

        public int HeadshotKills { get; set; }

        public int RoundsPlayed { get; set; }

        public int BombDefuses { get; set; }

        public int BombPlants { get; set; }

        public int Count1K { get; set; }

        public int Count2K { get; set; }

        public int Count3K { get; set; }

        public int Count4K { get; set; }

        public int Count5K { get; set; }

        public int V1 { get; set; }

        public int V2 { get; set; }

        public int V3 { get; set; }

        public int V4 { get; set; }

        public int V5 { get; set; }

        public int FirstKillT { get; set; }

        public int FirstKillCt { get; set; }

        public int FirstDeathT { get; set; }

        public int FirstDeathCt { get; set; }

        public int TradeKill { get; set; }

        public int Kast { get; set; }

        public int ContributionScore { get; set; }

        public int Mvp { get; set; }
    }


    public enum StartingSide
    {
        None,
        T = 2,
        CT = 3
    }

    public record SteamId(string SteamId64);
}