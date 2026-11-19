using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Entities.Sprites;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities.Enemies
{
    internal sealed class BulletBillCannon : Enemy
    {
        private const float FireIntervalSeconds = 5f;
        private const float CameraHalfWidth = 600f;
        private readonly int _bulletBillTileId;
        private readonly List<BulletBill> _pendingBullets = new List<BulletBill>();
        private float _fireTimer;

        public Vector2? ListenerPosition { get; set; }

        public override int Width
        {
            get { return 48; }
        }

        public override int Height
        {
            get { return 48; }
        }

        public override Rectangle CollisionBounds
        {
            get { return Rectangle.Empty; }
        }

        public BulletBillCannon(
            ISprite sprite,
            Vector2 position,
            int bulletBillTileId = BulletBill.DefaultTileId)
            : base(sprite, position)
        {
            _bulletBillTileId = bulletBillTileId;
        }

        public override void Update(GameTime gameTime)
        {
            if (!ListenerPosition.HasValue
                || Math.Abs(ListenerPosition.Value.X - Bounds.Center.X) > CameraHalfWidth + Width / 2f)
            {
                return;
            }

            _fireTimer += Math.Min(
                (float)gameTime.ElapsedGameTime.TotalSeconds, 1f / 30f);
            if (_fireTimer < FireIntervalSeconds)
            {
                return;
            }

            _fireTimer -= FireIntervalSeconds;
            float direction = RandomNumberGenerator.GetInt32(2) == 0 ? -1f : 1f;
            Vector2 bulletPosition = new Vector2(
                direction < 0f ? Position.X - BulletBill.BulletBillWidth : Position.X + Width,
                Position.Y);
            _pendingBullets.Add(
                EnemyFactory.CreateBulletBill(bulletPosition, direction, _bulletBillTileId));
        }

        public IReadOnlyList<BulletBill> DrainSpawns()
        {
            List<BulletBill> bullets = new List<BulletBill>(_pendingBullets);
            _pendingBullets.Clear();
            return bullets;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
        }

        public override void UpdateGravity(GameTime gameTime)
        {
        }

        public override void TakeDamage()
        {
        }

        public override void Kill()
        {
        }
    }
}
