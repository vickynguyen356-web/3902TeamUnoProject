using System;

namespace TeamUno.Mario.World
{
    internal class GameProgress
    {
        private const int CoinScore = 200;

        public int Score { get; private set; }
        public int Coins { get; private set; }
        public int Lives { get; private set; } = 3;

        public void AddCoin()
        {
            AddScore(CoinScore);
            Coins = Coins + 1;
            if (Coins == 100)
            {
                Coins = 0;
                AddLife();
            }
        }

        public void AddScore(int points)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(points);
            Score = Score + points;
        }

        public void AddLife()
        {
            Lives = Lives + 1;
        }

        public void LoseLife()
        {
            if (Lives > 0)
            {
                Lives = Lives - 1;
            }
        }

        public void Reset()
        {
            Score = 0;
            Coins = 0;
            Lives = 3;
        }
    }
}
