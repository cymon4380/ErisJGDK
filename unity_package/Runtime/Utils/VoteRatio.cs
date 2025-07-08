using System;

namespace ErisJGDK.Base.Utils
{
    public static class VoteRatio
    {
        /// <summary>
        /// Gets a vote ratio (0 to 1). You must provide either players who voted against or all players.
        /// </summary>
        /// <param name="votedPlayers">Players who voted for</param>
        /// <param name="votedPlayersOpponent">Players who voted against</param>
        /// <param name="allPlayers">All players</param>
        /// <param name="defaultValue">Ratio if nobody voted against anything</param>
        /// <exception cref="ArgumentException">You must specify either votedPlayersOpponent or allPlayers.</exception>
        public static float Get(
            Player[] votedPlayers,
            Player[] votedPlayersOpponent = null,
            Player[] allPlayers = null,
            float defaultValue = .5f
        )
        {
            if (votedPlayersOpponent == null && allPlayers == null)
                throw new ArgumentException("You must specify either votedPlayersOpponent or allPlayers");

            int totalPlayers = votedPlayersOpponent != null ? votedPlayers.Length + votedPlayersOpponent.Length
                : allPlayers.Length;

            if (totalPlayers == 0)
                return defaultValue;

            return (float) votedPlayers.Length / totalPlayers;
        }
    }
}
