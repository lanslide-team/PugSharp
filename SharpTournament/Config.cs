﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SharpTournament
{
    public class Config
    {
        [JsonPropertyName("maplist")]
        public string[] Maplist { get; set; }

        [JsonPropertyName("team1")]
        public Team Team1 { get; set; }

        [JsonPropertyName("team2")]
        public Team Team2 { get; set; }

        [JsonPropertyName("matchid")]
        public string MatchId { get; set; }

        [JsonPropertyName("num_maps")]
        public int NumMaps { get; set; }

        [JsonPropertyName("players_per_team")]
        public int PlayersPerTeam { get; set; }

        [JsonPropertyName("min_players_to_ready")]
        public int MinPlayersToReady { get; set; }

        [JsonPropertyName("eventula_apistats_url")]
        public string EventulaApistatsUrl { get; set; }

        [JsonPropertyName("eventula_demo_upload_url")]
        public string EventulaDemoUploadUrl { get; set; }
    }

    public class Team
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("tag")]
        public string Tag { get; set; }

        [JsonPropertyName("flag")]
        public string Flag { get; set; }

        [JsonPropertyName("players")]
        public Dictionary<ulong, string> Players { get; set; }
    }
}
