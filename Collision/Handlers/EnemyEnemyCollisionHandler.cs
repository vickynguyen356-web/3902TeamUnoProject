using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities.Enemies;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Collision.Handlers
{
    internal class EnemyEnemyCollisionHandler : ICollisionHandler<IEnemy, IEnemy>
    {
        public void Handle(IEnemy firstEnemy, IEnemy secondEnemy, CollisionInfo info)
        {
            bool firstCanTurn = firstEnemy is Goomba || firstEnemy is Koopa || firstEnemy is HammerBro;
            bool secondCanTurn = secondEnemy is Goomba || secondEnemy is Koopa || secondEnemy is HammerBro;
            if (firstEnemy.IsDead || secondEnemy.IsDead || !firstCanTurn || !secondCanTurn)
            {
                return;
            }

            if (info.Side != CollisionSide.Left && info.Side != CollisionSide.Right)
            {
                return;
            }

            Vector2 firstPosition = firstEnemy.Position;
            Vector2 secondPosition = secondEnemy.Position;
            Vector2 firstVelocity = firstEnemy.Velocity;
            Vector2 secondVelocity = secondEnemy.Velocity;
            float separation = info.OverlapWidth / 2f;

            if (info.Side == CollisionSide.Right)
            {
                firstPosition.X = firstPosition.X - separation;
                secondPosition.X = secondPosition.X + separation;
                if (firstVelocity.X > 0f)
                {
                    firstVelocity.X = -firstVelocity.X;
                }
                if (secondVelocity.X < 0f)
                {
                    secondVelocity.X = -secondVelocity.X;
                }
            }
            else
            {
                firstPosition.X = firstPosition.X + separation;
                secondPosition.X = secondPosition.X - separation;
                if (firstVelocity.X < 0f)
                {
                    firstVelocity.X = -firstVelocity.X;
                }
                if (secondVelocity.X > 0f)
                {
                    secondVelocity.X = -secondVelocity.X;
                }
            }

            firstEnemy.Position = firstPosition;
            secondEnemy.Position = secondPosition;
            firstEnemy.Velocity = firstVelocity;
            secondEnemy.Velocity = secondVelocity;
        }
    }
}
