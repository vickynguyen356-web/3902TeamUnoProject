using TeamUno.Mario.Collision;

namespace TeamUno.Mario.Collision
{
    /// <summary>
    /// Applies the rule for one type of collision.
    /// Fill in handle, call methods on entities to change their state (damage, stomp, kill, collect).
    /// Don't add or remove entities from Level lists here, mark as dead/expired and let Level.Update clean them up.
    /// Pairs are gathered before any handler runs, so an entity may already be dead/expired by the time the handler 
    /// sees it. Check its state first if that matters.
    /// </summary>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    internal interface ICollisionHandler<TFirst, TSecond>
    {
        void Handle(TFirst first, TSecond second, CollisionInfo info);
    }
}
