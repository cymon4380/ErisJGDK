using System;
using System.Collections.Generic;
using System.Linq;

namespace ErisJGDK.Base.Utils
{
    public struct Pair
    {
        public Player Player1;
        public Player Player2;

        public Pair(Player player1, Player player2)
        {
            Player1 = player1;
            Player2 = player2;
        }
    }

    public static class PairMaker
    {
        /// <summary>
        /// Makes pairs of players.
        /// </summary>
        /// <param name="twoPairs">If true, each player will have two pairs (works with at least 3 players).
        /// Otherwise, only one pair will be assigned (works only with even number of players).</param>
        /// <exception cref="ArgumentException">Invalid number of players was provided.</exception>
        public static Pair[] Make(Player[] players, bool twoPairs = true)
        {
            List<Pair> pairs = new();

            if (twoPairs)
            {
                if (players.Length < 3)
                    throw new ArgumentException("At least 3 players required to make two pairs each");

                Player[] shuffledPlayers = players.OrderBy(_ => Guid.NewGuid()).ToArray();

                for(int i = 0; i < shuffledPlayers.Length; i++)
                {
                    int opponentIndex = (i + 1) % shuffledPlayers.Length;
                    pairs.Add(new(shuffledPlayers[i], shuffledPlayers[opponentIndex]));
                }
            }
            else
            {
                if (players.Length < 2)
                    throw new ArgumentException("At least 2 players required to make pairs");
                if (players.Length % 2 != 0)
                    throw new ArgumentException("Player count must be an even number");

                players = players.OrderBy(_ => Guid.NewGuid()).ToArray();

                for(int i = 0; i < players.Length; i += 2)
                    pairs.Add(new Pair(players[i], players[i + 1]));
            }

            return pairs.ToArray();
        }
    }
}
