using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final
{
    //TODO REVERT TO SPRITE, HANDLE ALL DRAW IN MANAGER
    class Enemy : MGLib.Sprite
    {
        Vector2 dir;
        Vector2 headingDir;
        float speed;
        public enum RelativeQuadrant { A, B, C, D }
        RelativeQuadrant relQuad;

        public Color color;

        int turningTimer;

        public Enemy /*number one*/ (Game game)
            : this(game, Vector2.Zero) { }

        public Enemy(Game game, Vector2 loc)
            : base(game)
        {
            this.loc = loc;
            this.speed = 340.0f;
            this.dir = Vector2.Zero;
            this.turningTimer = 30;

            Game.Components.Add(this);
        }

        public override void Update(GameTime gameTime)
        {
            if (turningTimer >= 30)
            {
                headingDir = WanderController.GetWanderDir(this.relQuad);
                turningTimer = 0;
            }
            else
            {
                turningTimer++;
            }

            this.dir = Vector2.Normalize(Vector2.Lerp(this.dir, this.headingDir, 0.1f));
            this.rotation = MathHelper.ToDegrees((float)Math.Atan2(this.dir.Y, this.dir.X)) + 90.0f;
            this.loc += this.dir * speed * gameTime.ElapsedGameTime.Milliseconds / 1000;
            base.Update(gameTime);
        }

        public void SetTexture(Texture2D texture)
        {
            this.SpriteTexture = texture;
            this.origin = this.SpriteTexture.Bounds.Center.ToVector2();
            switch (new Random().Next(3))
            {
                case 0:
                    color = Color.Peru;
                    break;
                case 1:
                    color = Color.DarkMagenta;
                    break;
                case 2:
                    color = Color.MediumSeaGreen;
                    break;
            }
        }

        public void SetRelativeQuadrant(Vector2 pos)
        {
            /*
             setting quadrant of the player's position relative to the enemy
             it's either positive or not, no neutral territory here.
             ---------
             | B | A |
             |---|---|
             | C | D |
             ---------
             */

            Vector2 difference = pos - this.loc;
            if (difference.X <= 0)
            {
                if (difference.Y <= 0)
                {
                    this.relQuad = RelativeQuadrant.C;
                }
                else
                {
                    this.relQuad = RelativeQuadrant.B;
                }
            }
            else
            {
                if (difference.Y <= 0)
                {
                    this.relQuad = RelativeQuadrant.D;
                }
                else
                {
                    this.relQuad = RelativeQuadrant.A;
                }
            }
        }
    }
}
