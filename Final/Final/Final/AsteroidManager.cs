using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final
{
    class AsteroidManager : DrawableGameComponent
    {
        public List<Asteroid> asteroids;
        List<Asteroid> asteroidsToBeDestroyed;
        SpriteBatch spriteBatch;
        enum SpawnState { Spawn };

        public AsteroidManager(Game game)
            : base(game)
        {
            asteroids = new List<Asteroid>();
            asteroidsToBeDestroyed = new List<Asteroid>();
            Game.Components.Add(this);
        }

        public override void Initialize()
        {
            GenerateField();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (Asteroid a in asteroidsToBeDestroyed)
            {
                Destroy(a);
            }
            asteroidsToBeDestroyed.Clear();
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront,
                   BlendState.AlphaBlend,
                   null, null, null, null,
                   MGLib.Camera2D.GetTransformation(Game.GraphicsDevice));
            foreach (Asteroid a in asteroids)
            {
                a.Draw(spriteBatch);
            }
            spriteBatch.End();
           //base.Draw(gameTime);
        }

        public void QueueDestroy(Asteroid a)
        {
            asteroidsToBeDestroyed.Add(a);
        }

        void Destroy(Asteroid a)
        {
            a.Visible = false;
            asteroids.Remove(a);
        }

        void GenerateField() //hardcoded level size, for this level.
        {
            Random rand = new Random();

            for (int i = 0; i < 34; i++)
            {
                for (int j = 0; j < 34; j++)
                {
                    if (i != 17 && j != 17)
                    {
                        switch (rand.Next(5))
                        {
                            case (int)SpawnState.Spawn:
                                asteroids.Add(new Asteroid(Game, new Vector2((i * 256) - 4352, (j * 256) - 4352)));
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

    }
    class Asteroid : MGLib.Sprite
    {

        public Asteroid(Game game) : this(game, Vector2.Zero) { }

        public Asteroid(Game game, Vector2 loc)
            : base(game)
        {
            this.loc = loc;
            this.SpriteTexture = Game.Content.Load<Texture2D>("asteroid2");
            this.origin = SpriteTexture.Bounds.Center.ToVector2();
            this.SetTransformAndRect();
        }
    }
}
