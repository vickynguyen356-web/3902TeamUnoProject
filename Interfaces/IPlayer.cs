namespace Sprint0.Interfaces
{
    public interface IPlayer
    {
        void Move(float movementDirection);
        void Jump();
        void SetCrouching(bool crouching);
        void ThrowFireball();
    }
}
