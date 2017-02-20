using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final
{
    class EnemyBase : MGLib.DrawSprite
    {
        public int life;

        public EnemyBase(Game game)
            : this(game, Vector2.Zero) { }

        public EnemyBase(Game game, Vector2 loc)
            : base(game)
        {
            this.loc = loc;
            this.life = 10;

            Game.Components.Add(this);
        }
        protected override void LoadContent()
        {
            this.SpriteTexture = Game.Content.Load<Texture2D>("basicBase");
            this.origin = this.SpriteTexture.Bounds.Center.ToVector2();
            base.LoadContent();
        }
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront,
                BlendState.AlphaBlend,
                null, null, null, null,
                MGLib.Camera2D.GetTransformation(Game.GraphicsDevice));

            Draw(spriteBatch);

            spriteBatch.End();
            //base.Draw(gameTime);
        }

        public void Damage()
        {
            life--;
        }
    }
}
