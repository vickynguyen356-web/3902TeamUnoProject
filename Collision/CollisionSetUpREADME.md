Collision System set up for Mario project.

TL;DR: Delete the return; within your respective handler file and write what should occur when the two objects collide. Nothing else should need to be edited.

How it works:
For every frame, GameSession.Update does:
1. Remember where Mario was (previousPlayerBounds)
2. Move Mario, update the level
3. Call CollisionSystem.Update(...)

CollisionSystem:
1. Takes each pairing (Enemy vs Blocks, Enemy vs Projectiles, ...)
2. Finds every pair whose rectangles overlap OR touch
3. Finds out which side was hit 
4. Calls the matching handler for each pair

Implementing the handler method in the specific collision handler file:
public void Handle(IEnemy first, Block second, CollisionInfo info)
{
	// TODO: implement behavior
	return; // delete this & write logic
}
first and second arrive in the order in the class declaration. 

CollisionInfo:
Side: which side of first was touched (Top, Bottom, Left, Right)
OverlapWidth/OverlapHeight: How many pixels the rectangles overlap (can be 0)
FirstPreviousBounds: Where first was before it moved this frame. Only filled in for Mario, Rectangle.Empty otherwise

Ex: 
- MarioEnemyCollisionHandler, Side == Bottom means Mario's feet hit the enemy, so it's a stomp.
- MarioBlockCollisionHandler, Side == Top means Mario hit the block from below.

Extra Notes:
- Touching counts as a collision, so handler will fire when OverlapWidth or OverlapHeight is 0. Don't assume those values are automatically positive.
- FirstPreviousBounds is only meaningful when Mario is the first type. 
- Make sure to skip projectiles that're already dead (if it pertains to you).

## Projectile Handlers

| Handler | Keeps | Guard |
| --- | --- |
| MarioProjectileCollisionHandler | Enemy projectiles hitting Mario | if (!second.IsEnemyProjectile || second.IsDead || first.IsDead) return; |
| EnemyProjectileCollisionHandler | Mario’s projectiles hitting enemies | if (second.IsEnemyProjectile || second.IsDead) return; |
| ProjectileBlockCollisionHandler | Both kinds | Only if (first.IsDead) return;. Branch on IsEnemyProjectile and Type inside |