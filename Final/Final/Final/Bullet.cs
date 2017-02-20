using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final
{
    class Bullet : MGLib.Sprite
    {

        Vector2 dir;
        float speed;

        public Bullet(Game game)
            : this(game, Vector2.Zero, Vector2.Zero, 0.0f)
        { }

        public Bullet(Game game, Vector2 loc, float rotation)
            : this(game, loc, new Vector2((float)Math.Cos(MathHelper.ToRadians(rotation)), (float)Math.Sin(MathHelper.ToRadians(rotation))), 0.0f)
        { }

        public Bullet(Game game, Vector2 loc, Vector2 dir, float speed)
            : base(game)
        {
            this.loc = loc;
            this.dir = dir;
            this.speed = 600.0f + speed;
        }

        public void SetTexture(Texture2D texture)
        {
            this.SpriteTexture = texture;
            this.origin = this.SpriteTexture.Bounds.Center.ToVector2();
        }

        public override void Update(GameTime gameTime)
        {
            loc += dir * speed * gameTime.ElapsedGameTime.Milliseconds / 1000;
            base.Update(gameTime);
        }
    }
}
