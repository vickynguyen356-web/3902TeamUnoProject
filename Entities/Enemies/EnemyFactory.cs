using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Entities.Projectiles;
using TeamUno.Mario.Entities.Sprites;
using TeamUno.Mario.Interfaces;
using TeamUno.Mario.World;

namespace TeamUno.Mario.Entities.Enemies
{
    internal static class EnemyFactory
    {
        private static Texture2D _enemyTexture;
        private static Texture2D _enemyIdTexture;
        private static Texture2D _tileSheetTexture;

        public static void Initialize(
            Texture2D enemyTexture,
            Texture2D tileSheetTexture,
            Texture2D enemyIdTexture)
        {
            ArgumentNullException.ThrowIfNull(enemyTexture);
            ArgumentNullException.ThrowIfNull(tileSheetTexture);
            ArgumentNullException.ThrowIfNull(enemyIdTexture);

            _enemyTexture = enemyTexture;
            _tileSheetTexture = tileSheetTexture;
            _enemyIdTexture = enemyIdTexture;
        }

        public static IEnemy Create(
            EnemyType type,
            Vector2 position,
            int sourceSpriteTileId = -1)
        {
            ISprite sprite = new SpriteSheetSprite(_enemyTexture, 3f, true);

            switch (type)
            {
                case EnemyType.Goomba:
                    return new Goomba(sprite, position);
                case EnemyType.Koopa:
                    return new Koopa(sprite, position, CreateKoopaShell);
                case EnemyType.PiranhaPlant:
                    return CreatePiranhaPlant(
                        position,
                        sourceSpriteTileId < 0
                            ? PiranhaPlant.ReferenceTileId
                            : sourceSpriteTileId);
                case EnemyType.HammerBro:
                    return new HammerBro(sprite, position);
                case EnemyType.Bowser:
                    return new Bowser(sprite, position);
                case EnemyType.Lakitu:
                    return new Lakitu(CreateEnemyIdSprite(), position);
                case EnemyType.Spiny:
                    return new Spiny(CreateEnemyIdSprite(), position, -90f);
                case EnemyType.Blooper:
                    return new Blooper(CreateEnemyIdSprite(), position);
                case EnemyType.CheepCheep:
                    return CreateCheepCheep(
                        position,
                        sourceSpriteTileId < 0
                            ? CheepCheepSpriteFactory.GreenTileId
                            : sourceSpriteTileId);
                case EnemyType.BulletBill:
                    return new BulletBill(
                        sprite,
                        position,
                        -1f,
                        sourceSpriteTileId < 0 ? BulletBill.DefaultTileId : sourceSpriteTileId);
                case EnemyType.BulletBillCannon:
                    return CreateBulletBillCannon(
                        position,
                        sourceSpriteTileId < 0 ? BulletBill.DefaultTileId : sourceSpriteTileId);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }
        }

        public static IEnemy Create(
            EnemyType type,
            Vector2 position,
            int sourceSpriteTileId,
            CheepCheepMovementMode movementMode)
        {
            if (type != EnemyType.CheepCheep)
            {
                throw new ArgumentException(
                    "A Cheep Cheep movement mode requires the CheepCheep enemy type.",
                    nameof(type));
            }

            return CreateCheepCheep(position, sourceSpriteTileId, movementMode);
        }

        internal static Spiny CreateSpiny(Vector2 position, float horizontalVelocity)
        {
            ISprite sprite = CreateEnemyIdSprite();
            return new Spiny(sprite, position, horizontalVelocity);
        }

        internal static BulletBill CreateBulletBill(
            Vector2 position,
            float direction,
            int sourceTileId = BulletBill.DefaultTileId)
        {
            ArgumentNullException.ThrowIfNull(_enemyTexture);
            return new BulletBill(
                new SpriteSheetSprite(_enemyTexture, 3f, useSpriteEffects: true),
                position,
                direction,
                sourceTileId);
        }

        internal static BulletBillCannon CreateBulletBillCannon(
            Vector2 position,
            int bulletBillTileId)
        {
            ArgumentNullException.ThrowIfNull(_enemyTexture);
            return new BulletBillCannon(
                new SpriteSheetSprite(_enemyTexture, 3f),
                position,
                bulletBillTileId);
        }

        internal static CheepCheep CreateCheepCheep(
            Vector2 position,
            int sourceTileId,
            CheepCheepMovementMode movementMode = CheepCheepMovementMode.Swimming)
        {
            return new CheepCheep(
                CreateEnemyIdSprite(),
                position,
                sourceTileId,
                movementMode);
        }

        internal static PiranhaPlant CreatePiranhaPlant(Vector2 position, int sourceTileId)
        {
            return new PiranhaPlant(
                CreateEnemyIdSprite(),
                position,
                sourceTileId);
        }

        private static ISprite CreateEnemyIdSprite()
        {
            ArgumentNullException.ThrowIfNull(_enemyIdTexture);
            return new SpriteSheetSprite(_enemyIdTexture, 3f, useSpriteEffects: true);
        }

        internal static FireBarSegment CreateFireBarSegment(
            Vector2 rotationPoint,
            int segmentIndex,
            float startingAngle,
            string rotationDirection,
            bool isHazard)
        {
            ArgumentNullException.ThrowIfNull(_tileSheetTexture);
            ISprite sprite = new SpriteSheetSprite(_tileSheetTexture, 3f);
            return new FireBarSegment(
                sprite,
                rotationPoint,
                segmentIndex,
                startingAngle,
                rotationDirection,
                isHazard);
        }

        private static KoopaShell CreateKoopaShell(Vector2 shellPos)
        {
            ISprite shellSprite = ProjectileSpriteFactory.Create(
        ProjectileType.KoopaShell, _enemyTexture, null);

            return new KoopaShell(shellSprite, shellPos, Vector2.Zero, isEnemyProjectile: false);
        }
    }
}
