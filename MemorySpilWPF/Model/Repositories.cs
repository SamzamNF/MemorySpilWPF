using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorySpilWPF.Model
{
    public interface IGameStatsRepository
    {
        public void SaveGame(GameStats stats);
        List<GameStats> GetTop10Scores();
        List<GameStats> GetGamesByPlayer(string name);
    }
}
