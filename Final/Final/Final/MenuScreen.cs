using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final
{
    class MenuScreen : DrawableGameComponent
    {
        SpriteBatch spriteBatch;
        Texture2D texture;
        Vector2 drawPos;
        public enum DrawState { Draw, Skip };
        public DrawState currentState;

        public MenuScreen(Game game)
            : base(game)
        {
            this.currentState = DrawState.Draw;
            Game.Components.Add(this);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            texture = Game.Content.Load<Texture2D>("mainmenu");
            drawPos = new Vector2((Game.GraphicsDevice.Viewport.Width - texture.Bounds.Width) / 2, (Game.GraphicsDevice.Viewport.Height - texture.Bounds.Height) / 2);
            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            if (currentState == DrawState.Draw)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(texture, drawPos, Color.White);
                spriteBatch.End();
            }
        }
    }
}
