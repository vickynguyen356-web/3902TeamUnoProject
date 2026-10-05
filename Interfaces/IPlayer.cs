namespace TeamUno.Mario.Interfaces
{
    internal interface IPlayer
    {
        void Move(float movementDirection);
        void Jump();
        void SetCrouching(bool crouching);
        void ThrowFireball();
    }
}
