using TeamUno.Mario.Entities;

namespace TeamUno.Mario.Interfaces
{
    internal interface IPlayerMovement
    {
        void Move(MarioPlayer player, float elapsedSeconds);
    }
}
