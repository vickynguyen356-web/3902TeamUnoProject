namespace TeamUno.Mario.Interfaces
{
    internal interface IController
    {
        bool IsJumpHeld
        {
            get;
        }

        void Update();
    }
}
