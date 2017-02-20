using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final
{
    class ExplosionManager : DrawableGameComponent
    {
        public List<Explosion> explosions;
        List<Explosion> explosionsToBeDestroyed;

        public Texture2D explosionTexture;
        SpriteBatch spriteBatch;
        string spriteName;

        public ExplosionManager(Game game) : this(game, "splode") { }

        public ExplosionManager(Game game, string spriteName)
            : base(game)
        {
            this.explosions = new List<Explosion>();
            this.explosionsToBeDestroyed = new List<Explosion>();
            this.spriteName = spriteName;

            Game.Components.Add(this);
        }

        protected override void LoadContent()
        {
            this.spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            this.explosionTexture = Game.Content.Load<Texture2D>(spriteName);
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (Explosion e in this.explosions)
            {
                if (e.lifespan <= 0)
                {
                    QueueDestroyExplosion(e);
                }
            }

            foreach (Explosion e in this.explosionsToBeDestroyed)
            {
                DestroyExplosion(e);
            }

            explosionsToBeDestroyed.Clear();

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {

            spriteBatch.Begin(SpriteSortMode.BackToFront,
                BlendState.AlphaBlend,
                null, null, null, null,
                MGLib.Camera2D.GetTransformation(Game.GraphicsDevice));
            foreach (Explosion e in explosions)
            {
                spriteBatch.Draw(e.spriteTexture, e.loc, Color.White);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void AddExplosion(Game game, Vector2 loc)
        {
            int index = explosions.Count;
            explosions.Insert(index, new Explosion(game, loc));
            explosions[index].SetTexture(this.explosionTexture);
            Game.Components.Add(explosions[index]);
        }

        void QueueDestroyExplosion(Explosion e)
        {
            explosionsToBeDestroyed.Add(e);
        }

        void DestroyExplosion(Explosion e)
        {
            e.Enabled = false;
            explosions.Remove(e);
        }
    }

    class Explosion : GameComponent
    {
        public int lifespan;
        public Texture2D spriteTexture;
        public Vector2 loc;

        public Explosion(Game game) : this(game, Vector2.Zero) { }

        public Explosion(Game game, Vector2 loc)
            : base(game)
        {
            this.loc = loc;
            this.lifespan = 20;
        }

        public override void Update(GameTime gameTime)
        {
            this.lifespan--;
            base.Update(gameTime);
        }

        public void SetTexture(Texture2D texture)
        {
            this.spriteTexture = texture;
            this.loc -= new Vector2(texture.Width / 2, texture.Height / 2); //origin correction
        }

    }
}
