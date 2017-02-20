using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final
{
    class Boundary : DrawableGameComponent
    {
        SpriteBatch spriteBatch;
        public List<Wall> Walls;
        Texture2D horzText;
        Texture2D vertText;

        public Boundary(Game game)
            : base(game)
        {
            Walls = new List<Wall>();
            Game.Components.Add(this);
        }

        public override void Initialize()
        {
            //loading content here for wall setup in level
            horzText = Game.Content.Load<Texture2D>("cautiontape");
            vertText = Game.Content.Load<Texture2D>("cautiontapevert");
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            //draw wall w/ tiled texture
            spriteBatch.Begin(SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.LinearWrap,
                DepthStencilState.Default,
                RasterizerState.CullNone,
                null,
                MGLib.Camera2D.GetTransformation(Game.GraphicsDevice));
            foreach (Wall w in Walls)
            {
                w.Draw(spriteBatch);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

        public void AddWall(Vector2 topLeft, Vector2 bottomRight)
        {
            //adding wall, setting texture based on dimensions.
            int index = Walls.Count;
            Walls.Insert(index,new Wall(new Rectangle((int)topLeft.X, (int)topLeft.Y, (int)(bottomRight.X - topLeft.X), (int)(bottomRight.Y - topLeft.Y))));
            if (Walls[index].boundingRect.Width >= Walls[index].boundingRect.Height)
            {
                Walls[index].SetTexture(horzText);
            }
            else
            {
                Walls[index].SetTexture(vertText);
            }
        }
    }

    class Wall
    {
        public Rectangle boundingRect;
        Texture2D spriteTexture;

        public Wall(Rectangle boundingRect)
        {
            this.boundingRect = boundingRect;
        }

        public void SetTexture(Texture2D spriteTexture)
        {
            this.spriteTexture = spriteTexture;
        }

        public bool Intersects(MGLib.Sprite other)
        {
            return (this.boundingRect.Right > other.LocRect.Left && this.boundingRect.Left < other.LocRect.Right && this.boundingRect.Bottom > other.LocRect.Top && this.boundingRect.Top < other.LocRect.Bottom);
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(this.spriteTexture, this.boundingRect.Location.ToVector2(), this.boundingRect, Color.White);
        }

    }
}
