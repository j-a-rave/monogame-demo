using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final
{
    class BulletManager : DrawableGameComponent
    {
        public List<Bullet> bullets;
        public List<Bullet> bulletsToBeDestroyed;
        Texture2D bulletTexture;
        SpriteBatch spriteBatch;

        public BulletManager(Game game)
            : base(game)
        {
            bullets = new List<Bullet>();
            bulletsToBeDestroyed = new List<Bullet>();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            bulletTexture = Game.Content.Load<Texture2D>("basicShot");
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            Matrix transformMatrix = MGLib.Camera2D.GetTransformation(Game.GraphicsDevice);

            foreach (Bullet b in bullets)
            {
                Vector2 transformedPosition = Vector2.Transform(b.loc, transformMatrix);
                if (transformedPosition.Y > GraphicsDevice.Viewport.Height || transformedPosition.Y < 0)
                {
                    QueueDestroyBullet(b);
                }
            }

            foreach (Bullet b in bulletsToBeDestroyed)
            {
                DestroyBullet(b);
            }

            bulletsToBeDestroyed.Clear();

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront,
                BlendState.AlphaBlend,
                null, null, null, null,
                MGLib.Camera2D.GetTransformation(Game.GraphicsDevice));
            foreach (Bullet b in bullets)
            {
                b.Draw(spriteBatch);
            }
            spriteBatch.End();
            //base.Draw(gameTime);
        }

        public void Shoot(Vector2 loc, Vector2 dir, float speed)
        {
            int index = bullets.Count;
            bullets.Insert(index, new Bullet(Game,loc,dir,speed));
            bullets[index].SetTexture(this.bulletTexture);
            Game.Components.Add(bullets[index]);
        }

        public void QueueDestroyBullet(Bullet b)
        {
            bulletsToBeDestroyed.Add(b);
        }

        void DestroyBullet(Bullet b)
        {
            b.Enabled = false;
            b.Visible = false;
            bullets.Remove(b);
        }
    }
}
