using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using MemorySpilWPF.Model;
using MemorySpilWPF.MVVM;

namespace MemorySpilWPF.ViewModel
{
    public class GameViewModel : ViewModelBase
    {
        // Properties
        public ObservableCollection<Card> Cards { get; set; }
        public string PlayerName { get; set; }
        public int MoveCount { get; set; }
        public string GameTime { get; set; }
        private bool IsGameCompleted;
        public Card FirstSelectedCard { get; set; }
        public Card SecondSelectedCard { get; set; }
        
        private DateTime _gameStartTime;
        
        //Timer til at tælle spillet, opdatere GameTime i intervaller
        private DispatcherTimer _timer;

        //Felt til at pause spil mens match tjekkes
        private bool _isBusy;


        private readonly IGameStatsRepository gameStatsRepository;

        // Konstruktør
        public GameViewModel()
        {
            Cards = new ObservableCollection<Card>();
            gameStatsRepository = new IGameStatsFileRepo("gamestats.txt");
            NewGame();

        }




        public bool GameCompleted
        {
            get => IsGameCompleted;
            set
            {
                IsGameCompleted = value;
                OnPropertyChanged(nameof(GameCompleted));
            }
        }

        public string GameStatus
        {
            get
            {
                if (GameCompleted)
                {
                    var playerDisplay = string.IsNullOrWhiteSpace(PlayerName) ? "Du" : PlayerName;
                    return $"{playerDisplay} vandt på {MoveCount} træk i {GameTime:mm\\:ss}!";
                }

                var currentPlayer = string.IsNullOrWhiteSpace(PlayerName) ? "Spiller" : PlayerName;
                return $"{currentPlayer} - Træk: {MoveCount} | Tid: {GameTime:mm\\:ss}";
            }
        }


        // Metoder
        private async void FlipCard(Card selectedCard)
        {
            // Ignorere metoden hvis kortet er vendt allerede, eller spillet er slut
            if (_isBusy || selectedCard.IsFlipped || selectedCard.IsMatched || GameCompleted)
                return;

            // Flipper kortet
            selectedCard.IsFlipped = true;
            RefreshCards();

            // Logik til første og andet valgte kort
            if (FirstSelectedCard == null)
            {
                FirstSelectedCard = selectedCard;
                return;
            }
            else if (SecondSelectedCard == null && selectedCard != FirstSelectedCard)
            {
                SecondSelectedCard = selectedCard;
                MoveCount++;
                OnPropertyChanged(nameof(MoveCount));
                OnPropertyChanged(nameof(GameStatus));

                _isBusy = true;

                // Tjekker match

                if (FirstSelectedCard.Symbol == SecondSelectedCard.Symbol)
                {
                    FirstSelectedCard.IsMatched = true;
                    SecondSelectedCard.IsMatched = true;
                    FirstSelectedCard = null;
                    SecondSelectedCard = null;
                    RefreshCards();

                    // Tjekker om alle par er fundet
                    if (Cards.All(c => c.IsMatched))
                    {
                        EndGameTimer();
                    }
                    _isBusy = false;
                }

                // Gør så korterne vendes tilbage hvis spillet ikke er slut
                else
                {
                    await Task.Delay(1000);
                    FirstSelectedCard.IsFlipped = false;
                    SecondSelectedCard.IsFlipped = false;
                    FirstSelectedCard = null;
                    SecondSelectedCard = null;
                    RefreshCards();
                    _isBusy = false;
                }
            }

        }

        private void NewGame()
        {
            //Nulstil spil
            _gameStartTime = DateTime.Now;
            MoveCount = 0;
            OnPropertyChanged(nameof(MoveCount));

            GameCompleted = false;
            OnPropertyChanged(nameof(GameCompleted));
            FirstSelectedCard = null;
            SecondSelectedCard = null;

            //Resetter kort
            var newCards = Card.GenerateCards();
            Cards.Clear();
            foreach (var card in newCards)
                Cards.Add(card);
            OnPropertyChanged(nameof(Cards));


            // Start timer til at opdatere GameTime
            if (_timer == null)
            {
                _timer = new DispatcherTimer();
                _timer.Interval = TimeSpan.FromSeconds(1);
                _timer.Tick += (s, e) =>
                {
                    GameTime = (DateTime.Now - _gameStartTime).ToString(@"mm\:ss");
                    OnPropertyChanged(nameof(GameTime));
                    OnPropertyChanged(nameof(GameStatus));
                };
            }
            GameTime = "00:00";
            OnPropertyChanged(nameof(GameTime));
            _timer.Start();
        }

        private void EndGameTimer()
        {
            _timer?.Stop();
            GameTime = (DateTime.Now - _gameStartTime).ToString(@"mm\:ss");
            OnPropertyChanged(nameof(GameTime));
            OnPropertyChanged(nameof(IsGameCompleted));
            GameCompleted = true;
        }

        private void SaveStats()
        {
            if (gameStatsRepository == null)
                return;

            var stats = new GameStats
            {
                PlayerName = string.IsNullOrWhiteSpace(PlayerName) ? "Ukendt" : PlayerName,
                Moves = MoveCount,
                GameTime = TimeSpan.TryParse(GameTime, out var t) ? t : TimeSpan.Zero,
                CompletedAt = DateTime.Now

            };

            gameStatsRepository.SaveGame(stats);
        }

        private void RefreshCards()
        {
            // Tvinger UI til at opdatere ved at reassign'e hele listen
            var temp = new ObservableCollection<Card>(Cards);
            Cards = temp;
            OnPropertyChanged(nameof(Cards));
        }


        // RelayCommands

        public RelayCommand SaveStatsCommand => new RelayCommand(execute => SaveStats());
        public RelayCommand NewGameCommand => new RelayCommand(execute => NewGame());
        public RelayCommand FlipCardCommand => new RelayCommand(param => FlipCard(param as Card));


    }
}
