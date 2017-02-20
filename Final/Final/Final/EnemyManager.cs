using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final
{
    class EnemyManager : DrawableGameComponent
    {
        public List<Enemy> enemies;
        List<Enemy> enemiesToBeDestroyed;

        public Texture2D enemyTexture;
        public Vector2 textureSize;
        SpriteBatch spriteBatch;
        Vector2 playerLoc;

        public EnemyManager(Game game) : this(game, Vector2.Zero) { }

        public EnemyManager(Game game, Vector2 playerLoc)
            : base(game)
        {
            enemies = new List<Enemy>();
            enemiesToBeDestroyed = new List<Enemy>();
            this.playerLoc = playerLoc;

            Game.Components.Add(this);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            enemyTexture = Game.Content.Load<Texture2D>("blankShip");
            textureSize = new Vector2(enemyTexture.Bounds.Width, enemyTexture.Bounds.Height);
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (Enemy e in enemies)
            {
                e.SetRelativeQuadrant(this.playerLoc);
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront,
                BlendState.AlphaBlend,
                null, null, null, null,
                MGLib.Camera2D.GetTransformation(Game.GraphicsDevice));
            foreach (Enemy e in enemies)
            {
                e.Draw(spriteBatch, e.color);
            }

            foreach (Enemy e in enemiesToBeDestroyed)
            {
                DestroyEnemy(e);
            }
            enemiesToBeDestroyed.Clear();

            spriteBatch.End();
            base.Draw(gameTime);
        }

        public void SpawnEnemy(Vector2 loc)
        {
            int index = enemies.Count;
            enemies.Insert(index, new Enemy(Game, loc));
            enemies[index].SetTexture(enemyTexture);
        }

        public void QueueDestroyEnemy(Enemy e)
        {
            enemiesToBeDestroyed.Add(e);
        }

        public void DestroyEnemy(Enemy e)
        {
            try
            {
                e.Visible = false;
                e.Enabled = false;
                enemies.Remove(e);
            }
            catch
            {
                enemiesToBeDestroyed.Remove(e);
            }
        }

        public void AccessPlayerLoc(Vector2 pLoc)
        {
            this.playerLoc = pLoc;
        }
    }
}
