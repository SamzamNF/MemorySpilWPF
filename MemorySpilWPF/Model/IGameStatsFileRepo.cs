using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorySpilWPF.Model
{
    public class IGameStatsFileRepo : IGameStatsRepository
    {
        public readonly string _filepath = "gamestats.txt";
        
        public IGameStatsFileRepo(string filepath)
        {
            _filepath = filepath;
            if (File.Exists(_filepath))
            {
                File.Create(_filepath).Close();
            }
        }

        public List<GameStats> GetGamesByPlayer(string name)
        {
            //return GetAllGames().Where(n => n.PlayerName == name).ToList();
            return GetAllGames().Where(n => n.PlayerName.Equals(name, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public List<GameStats> GetTop10Scores()
        {
            return GetAllGames().OrderBy(g => g.Moves).ThenBy(g => g.GameTime).Take(10).ToList();
        }

        public void SaveGame(GameStats stats)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(_filepath, append: true))
                {
                    sw.WriteLine(stats.ToString());
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Fejl i at gemme spillet: {ex.Message}");
            }
        }

        // Ekstra metode tilføjet til at hente alle spillere så det ikke skal laves flere gange

        private List<GameStats> GetAllGames()
        {
            try
            {
                using (StreamReader sr = new StreamReader(_filepath))
                {
                    var games = new List<GameStats>();
                    string line;

                    while ((line = sr.ReadLine()) != null)
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            games.Add(GameStats.FromString(line));
                        }
                    }
                    return games;
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Fejl ved læsning af fil: {ex.Message}");
                return new List<GameStats>();
            }
        }
    }
}
