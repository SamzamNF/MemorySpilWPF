using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorySpilWPF.Model
{
    public class Card
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public bool IsFlipped { get; set; }
        public bool isMatched { get; set; }

        // Konstruktør laves here hvis brug for

        public override string ToString()
        {
            return $"{Id};{Symbol};{IsFlipped};{isMatched}";
        }

        public static Card FromString(string data)
        {
            if (string.IsNullOrEmpty(data))
                return null;

            var parts = data.Split(';');
            if (parts.Length != 4)
                return null;

            return new Card
            {
                Id = int.Parse(parts[0].Trim()),
                Symbol = parts[1].Trim(),
                IsFlipped = bool.Parse(parts[2].Trim()),
                isMatched = bool.Parse(parts[3].Trim())
            };
        }
    }
}
