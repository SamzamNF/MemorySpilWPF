using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorySpilWPF.Model
{
    public class GameStats
    {
        public string PlayerName {  get; set; }
        public int Moves { get; set; }
        public TimeSpan GameTime { get; set; }
        public DateTime CompletedAt { get; set; }

        // Konstruktør laves here hvis brug for

        public override string ToString()
        {
            return $"{PlayerName};{Moves};{GameTime};{CompletedAt}";
        }

        public static GameStats FromString(string data)
        {
            if (string.IsNullOrEmpty(data))
                return null;

            var parts = data.Split(';');
            if (parts.Length != 4)
                return null;

            return new GameStats
            {
                PlayerName = parts[0].Trim(),
                Moves = int.Parse(parts[1].Trim()),
                GameTime = TimeSpan.Parse(parts[2].Trim()),
                CompletedAt = DateTime.Parse(parts[3].Trim())
            };
        }
    }
}
