namespace TeamUno.Mario.World
{
    internal class GameProgress
    {
        public int Coins { get; private set; }
        public int Lives { get; private set; } = 3;

        public void AddCoin()
        {
            Coins = Coins + 1;
        }

        public void AddLife()
        {
            Lives = Lives + 1;
        }

        public void Reset()
        {
            Coins = 0;
            Lives = 3;
        }
    }
}
