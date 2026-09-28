using TeamUno.Mario.Entities;

namespace TeamUno.Mario.Interfaces
{
    public interface IPlayerMovement
    {
        void Move(MarioPlayer player, float elapsedSeconds);
    }
}
