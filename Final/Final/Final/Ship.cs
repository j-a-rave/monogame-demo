using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Final
{
    class Ship : MGLib.DrawSprite, MGLib.IPlayer
    {
        Color color;

        float speed, maxSpeed, acceleration, angleStep;
        int playerIndex;
        Vector2 movement;

        public Gun gun;

        #region IPlayer implementation

        public int PlayerIndex { get { return playerIndex; } }
        public float Speed { get { return this.speed; } set { this.speed = value; } }
        public float MaxSpeed { get { return this.maxSpeed; } }
        public float Acceleration { get { return this.acceleration; } }
        public float Angle { get { return this.rotation; } set { this.rotation = value; } }
        public float AngleStep { get { return this.angleStep; } }

        public float MovementX { get { return this.movement.X; } set { this.movement.X = value; } }
        public float MovementY { get { return this.movement.Y; } set { this.movement.Y = value; } }

        #endregion

        public Ship(Game game) : this(game, 0, Vector2.Zero, Color.White, 1) { }

        public Ship(Game game, int playerIndex, Vector2 loc, Color color, float scale):base(game)
        {
            //controlmanager
            this.Speed = 0.0f;
            maxSpeed = 300.0f;
            acceleration = 10.0f;
            angleStep = 1.8f;
            this.playerIndex = playerIndex;

            //sprite
            this.loc = loc;
            this.color = color;
            this.scale = scale;
            this.Angle = 0.0f;

            this.gun = new Gun(game);

            Game.Components.Add(this);
            Game.Components.Add(gun);
        }

        protected override void LoadContent()
        {
            this.SpriteTexture = Game.Content.Load<Texture2D>("basicShip");
            origin = new Vector2(this.SpriteTexture.Bounds.Width / 2, this.SpriteTexture.Bounds.Height / 2);
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            MGLib.ControlManager.setMovement(this);
            this.loc += this.movement * gameTime.ElapsedGameTime.Milliseconds / 1000;

            gun.SetParameters(this.loc, this.speed);

            MGLib.Camera2D.SmoothFollow(this.loc);
            MGLib.Camera2D.SmoothRotate(-MathHelper.ToRadians(this.rotation));

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront,
                BlendState.AlphaBlend,
                null, null, null, null,
                MGLib.Camera2D.GetTransformation(Game.GraphicsDevice));

            Draw(spriteBatch);

            spriteBatch.End();
        }
    }
}
