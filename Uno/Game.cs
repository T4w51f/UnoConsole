using System;
using System.Collections.Generic;
using System.Linq;

namespace Uno
{
    public class Game
    {
        private List<Color> _colors = new List<Color>
        {
            Color.Blue,
            Color.Green,
            Color.Yellow,
            Color.Red,
        };

        private List<Number> _pairNumbers = new List<Number>
        {
            Number.One,
            Number.Two,
            Number.Three,
            Number.Four,
            Number.Five,
            Number.Six,
            Number.Seven,
            Number.Eight,
            Number.Nine,
            Number.Reverse,
            Number.Skip,
            Number.DrawTwo
        };

        private int _numberOfPlayers;
        private List<Player> _players;
        private List<Card> _cards;
        private Stack<Card> _shuffleDeck;
        private Floor _floor;

        public Game(int numberOfPlayers)
        {
            _numberOfPlayers = numberOfPlayers;
            GenerateDeck();
            ShuffleDeck();
            GenerateFloor();
            GeneratePlayers();
            DealHands();
            PlayTurns();
        }

        private void PlayTurns()
        {
            Console.WriteLine("Starting game...");
            var gameEnded = false;
            var isDrawTwo = false;
            var isDrawFour = false;
            var isReverse = false;

            _floor.playedCards.Push(_floor.deck.Pop());
            while (!gameEnded)
            {
                for (var i = 0; i < _numberOfPlayers; i++)
                {
                    var playedCard = _floor.playedCards.Peek();
                    Console.WriteLine($"Top card: [{playedCard.Color.ToString()}, {playedCard.Number.ToString()}]");
                    var player = _players[i % _numberOfPlayers];
                    Console.WriteLine($"{player.name}'s turn...");

                    if (isDrawTwo)
                    {
                        Console.WriteLine($"Player {player.name} draws two cards");
                        DrawCard(player, 2);
                        isDrawTwo = false;
                    }
                    else if (isDrawFour)
                    {
                        Console.WriteLine($"Player {player.name} draws four cards");
                        DrawCard(player, 4);
                        isDrawFour = false;
                    }
                    else
                    {
                        Console.WriteLine($"{player.name}'s hand: ");
                        var hand = player.hand;
                        var j = 0;
                        foreach (var card in hand)
                        {
                            j++;
                            Console.WriteLine($"[{j}. {card.Color.ToString()}, {card.Number.ToString()}]");
                        }

                        bool isCardValid;
                        Card cardToPlay;

                        do
                        {
                            Console.WriteLine("Pick a valid card: ");
                            
                            //TODO draw a card?
                            //TODO play the drawn card?
                            
                            string userResponse = Console.ReadLine();
                            cardToPlay = player.hand[int.Parse(userResponse ?? throw new Exception("Failed to parse user response")) - 1];
                            isCardValid = CheckCardValidity(cardToPlay);
                        } while (!isCardValid);

                        _floor.playedCards.Push(cardToPlay);

                        switch (cardToPlay.Number)
                        {
                            case Number.Reverse:
                                isReverse = !isReverse;
                                break;
                            case Number.Skip:
                                i++;
                                break;
                            case Number.DrawTwo:
                                // TODO stack +2
                                isDrawTwo = true;
                                break;
                            case Number.DrawFour:
                                isDrawFour = true;
                                break;
                            case Number.WildColor:
                            {
                                Console.WriteLine("Pick a color from:\n 1. Red\n 2. Blue\n 3. Green\n 4. Yellow\n");
                                string colorPicked = Console.ReadLine()?.ToLower();

                                // TODO allow black cards to override
                                _floor.playedCards.Peek().Color = colorPicked switch
                                {
                                    "red" => Color.Red,
                                    "blue" => Color.Blue,
                                    "green" => Color.Green,
                                    "yellow" => Color.Yellow,
                                    _ => _floor.playedCards.Peek().Color
                                };
                                break;
                            }
                        }

                        // Find alternative for reverse
                        if (isReverse)
                        {
                            i -= 2;
                        }
                    }

                    if (player.handCount != 0) continue;
                    Console.WriteLine($"{player.name} won the game");
                    gameEnded = true;

                    // TODO say Uno!
                }
            }
        }

        private void DrawCard(Player player, int count)
        {
            for (var i = 0; i < count; i++)
            {
                var card = _floor.deck.Pop();
                player.hand.Add(card);
            }
        }

        private bool CheckCardValidity(Card cardToPlay)
        {
            var playedCard = _floor.playedCards.Peek();
            return playedCard.Color == cardToPlay.Color || playedCard.Number == cardToPlay.Number;
        }

        private void GenerateDeck()
        {
            Console.WriteLine("Generating cards...");
            _cards = (
                from color in _colors
                from number in _pairNumbers
                select new Card {Color = color, Number = number}).ToList();
            _cards.AddRange(_colors.Select(color => new Card {Color = color, Number = Number.Zero}));

            for (var i = 0; i < 4; i++)
            {
                _cards.Add(new Card {Color = Color.Black, Number = Number.DrawFour});
                _cards.Add(new Card {Color = Color.Black, Number = Number.WildColor});
            }
        }

        private void ShuffleDeck()
        {
            Console.WriteLine("Shuffling cards...");
            _shuffleDeck = new Stack<Card>();
            while (_cards.Any())
            {
                int size = _cards.Count;
                int index = new Random().Next(0, size - 1);
                _shuffleDeck.Push(_cards[index]);
                _cards.RemoveAt(index);
            }
        }

        private void GeneratePlayers()
        {
            Console.WriteLine("Generating players...");
            _players = new List<Player>();
            for (var i = 0; i < _numberOfPlayers; i++)
            {
                Console.WriteLine("Enter player name: ");
                string playerName = Console.ReadLine();
                _players.Add(new Player {name = playerName});
            }
        }

        private void GenerateFloor()
        {
            Console.WriteLine("Generating floor...");
            _floor = new Floor
            {
                deck = _shuffleDeck,
                playedCards = new Stack<Card>()
            };
        }

        private void DealHands()
        {
            Console.WriteLine("Dealing cards...");
            foreach (var player in _players)
            {
                player.hand = new List<Card>();
                for (var i = 0; i < 7; i++)
                {
                    player.hand.Add(_shuffleDeck.Pop());
                }

                player.handCount = player.hand.Count;
            }
        }
    }
}