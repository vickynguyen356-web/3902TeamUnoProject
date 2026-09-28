namespace TeamUno.Mario
{
    public class Program
    {
        public static void Main()
        {
            using (MarioGame marioGame = new MarioGame())
            {
                marioGame.Run();
            }
        }
    }
}
