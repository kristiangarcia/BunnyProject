using System;
using System.Collections.Generic;

namespace BunnyGame.Data
{
    /// <summary>
    /// Representa una entrada en el leaderboard
    /// </summary>
    [Serializable]
    public class LeaderboardEntry
    {
        public string playerName;
        public int score;

        public LeaderboardEntry(string playerName, int score)
        {
            this.playerName = playerName;
            this.score = score;
        }

        /// <summary>
        /// Parsea la respuesta de DreamLo en formato pipe-separated
        /// Formato: PLAYER1|2350
        /// </summary>
        public static List<LeaderboardEntry> ParseDreamLoResponse(string response)
        {
            List<LeaderboardEntry> entries = new List<LeaderboardEntry>();

            if (string.IsNullOrEmpty(response))
                return entries;

            string[] lines = response.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                string[] parts = line.Split('|');

                if (parts.Length >= 2)
                {
                    try
                    {
                        string name = parts[0].Trim();
                        int score = int.Parse(parts[1].Trim());

                        entries.Add(new LeaderboardEntry(name, score));
                    }
                    catch (Exception)
                    {
                        // Ignorar lineas con formato incorrecto
                        continue;
                    }
                }
            }

            return entries;
        }
    }
}
