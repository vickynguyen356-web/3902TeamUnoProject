namespace TeamUno.Mario.Interfaces
{
    internal interface IPlayer
    {
        void Move(float movementDirection);
        void Climb(float direction);
        void Jump();
        void SetCrouching(bool crouching);
        void ThrowFireball();
        void RequestPipeEntry();
    }
}
