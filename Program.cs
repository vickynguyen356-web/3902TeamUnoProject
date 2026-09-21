namespace Sprint0
{
    public class Program
    {
        public static void Main()
        {
            using (Game1 marioGame = new Game1())
            {
                marioGame.Run();
            }
        }
    }
}
