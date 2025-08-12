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
        public bool IsMatched { get; set; }

        // Konstruktør laves here hvis brug for

        public override string ToString()
        {
            return $"{Id};{Symbol};{IsFlipped};{IsMatched}";
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
                IsMatched = bool.Parse(parts[3].Trim())
            };
        }

        //Metode til at generate kort med
        public static List<Card> GenerateCards()
        {
            //Symboler til 8 par
            string[] symbols = { "🍎", "🍌", "🍒", "🍇", "🍉", "🍍", "🥝", "🍓" };

            //Liste til at holde på kort
            var cards = new List<Card>();
            int cardId = 1;

            //Tilføjer 2 af hvert kort, da der skal være par
            foreach (string symbol in symbols)
            {
                cards.Add(new Card { Id = cardId++, Symbol = symbol });
                cards.Add(new Card { Id = cardId++, Symbol = symbol });
            }

            return ShuffleCards(cards);
        }

        //Metode til at blande kort
        private static List<Card> ShuffleCards(List<Card> cards)
        {
            var random = new Random();
            return cards.OrderBy(c => random.Next()).ToList();
        }
    }
}
