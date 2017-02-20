using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final
{
    class Minimap : DrawableGameComponent
    {
        SpriteBatch spriteBatch;
        Texture2D frame, ship, enemyBase;
        Vector2 shipLoc, frameLoc;
        List<Vector2> enemyBaseLoc;

        public Minimap(Game game)
            : base(game)
        {
            shipLoc = Vector2.Zero;
            enemyBaseLoc = new List<Vector2>();
            Game.Components.Add(this);
        }

        protected override void LoadContent()
        {
            frameLoc = new Vector2(Game.GraphicsDevice.Viewport.Width - 144, 16);
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            frame = Game.Content.Load<Texture2D>("mmFrame");
            ship = Game.Content.Load<Texture2D>("mmShip");
            enemyBase = Game.Content.Load<Texture2D>("mmBase");
            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(frame, frameLoc, Color.White);
            spriteBatch.Draw(ship, shipLoc + frameLoc + frame.Bounds.Center.ToVector2(), Color.White);
            foreach (Vector2 loc in enemyBaseLoc)
            {
                spriteBatch.Draw(enemyBase, loc + frameLoc + frame.Bounds.Center.ToVector2(), Color.White);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

        public void SetMMShipPosition(Vector2 loc)
        {
            this.shipLoc = loc / 68;
        }

        public void SetMMBasePosition(List<EnemyBase> enemyBases)
        {
            this.enemyBaseLoc.Clear();
            foreach (EnemyBase eb in enemyBases)
            {
                enemyBaseLoc.Add(eb.loc / 68);
            }
        }
    }
}
