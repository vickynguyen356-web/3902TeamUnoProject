using Sprint0.Entities;

namespace Sprint0.Interfaces
{
    public interface IPlayerMovement
    {
        void Move(MarioPlayer player, float elapsedSeconds);
    }
}
