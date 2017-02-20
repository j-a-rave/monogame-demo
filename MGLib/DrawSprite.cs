using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MGLib
{
    //a drawablegamecomponent with its own spritebatch
    //implements Sprite
    public class DrawSprite : Sprite
    {
        protected SpriteBatch spriteBatch;

        public DrawSprite(Game game)
            : base(game)
        {
            //child component constructors go here
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            this.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}
