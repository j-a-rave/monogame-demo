/*a class for parallax-scrolling environment elements and backgrounds, as well as a manager for them, using Camera2D.
Jacob Rave Dec 2015*/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MGLib
{
    public static class ParallaxManager
    {
        //list of backgrounds, arbitrary #
        public static List<ParallaxBackground> Backgrounds = new List<ParallaxBackground>();
        public static Vector2 prevPos = Vector2.Zero; //storing camera positions.
        public static Rectangle viewportRect = new Rectangle(-2560, -2560, 5120, 5120); //TODO figure out infinite scrolling...
        public static Vector2 viewportLoc = new Vector2(viewportRect.X, viewportRect.Y);

        public static void UpdateParallaxScrolling(GraphicsDevice gDevice)
        {
            foreach (ParallaxBackground pb in Backgrounds)
            {
                pb.loc += (Camera2D.position - prevPos) * pb.distanceScale;
            }

            prevPos = Camera2D.position;
        }

        #region AddBackground overloads

        public static void AddBackground(Game game, string textureName, float distanceScale)
        {
            AddBackground(game, textureName, distanceScale, Vector2.Zero, true);
        }

        public static void AddBackground(Game game, string textureName, float distanceScale, Vector2 loc, bool isTiled)
        {
            AddBackground(new ParallaxBackground(game, textureName, distanceScale, loc, isTiled));
        }

        public static void AddBackground(ParallaxBackground pb)
        {
            Backgrounds.Add(pb);
        }

        #endregion
    }

    public class ParallaxBackground : DrawableGameComponent
    {
        public Vector2 loc; //the initial position it's drawn at.
        public float distanceScale; //usually between 0.0 & 1.0: 0.0 is at player distance, 1.0 is a static background. > 1.0 runs ahead of the player, < 0.0 would pass backwards i.e. foreground.

        SpriteBatch spriteBatch;
        Texture2D texture;
        
        string textureName; //saved to be referenced later on in LoadContent.
        SamplerState tileMode; //we'll set to LinearWrap if it's tiled, and null if it isn't.

        public ParallaxBackground(Game game, string textureName, float distanceScale, Vector2 loc, bool isTiled)
            : base(game)
        {
            this.distanceScale = distanceScale;
            this.loc = loc;
            this.textureName = textureName;
            if (isTiled)
            {
                tileMode = SamplerState.LinearWrap;
            }
            else
            {
                tileMode = null;
            }

            //finally, adds itself to the components
            game.Components.Add(this);
        }

        protected override void LoadContent()
        {
            this.spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            this.texture = Game.Content.Load<Texture2D>(this.textureName);
            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                this.tileMode,
                DepthStencilState.Default,
                null, null,
                MGLib.Camera2D.GetTransformation(Game.GraphicsDevice));
            spriteBatch.Draw(this.texture, ParallaxManager.viewportLoc + this.loc, ParallaxManager.viewportRect, Color.DarkSlateGray);
            spriteBatch.End();
            base.Draw(gameTime);
        }

    }
}
