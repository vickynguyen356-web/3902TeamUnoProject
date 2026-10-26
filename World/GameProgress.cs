namespace TeamUno.Mario.World
{
    internal class GameProgress
    {
        public int Coins { get; private set; }
        public int Lives { get; private set; } = 3;

        public void AddCoin()
        {
            Coins = Coins + 1;
            if (Coins == 100)
            {
                Coins = 0;
                AddLife();
            }
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
            Coins = 0;
            Lives = 3;
        }
    }
}
