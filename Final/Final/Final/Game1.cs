using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Final
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        Color clearColor;
        enum GameState { Level, Menu };
        GameState currentState;
        MenuScreen menu;
        Level level;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferredBackBufferWidth = 1280;
            Content.RootDirectory = "Content";

            MGLib.ControlManager.addHandler(this);
            MGLib.ParallaxManager.AddBackground(this, "bigStar", 0.65f);
            MGLib.ParallaxManager.AddBackground(this, "smallStars", 0.8f);

            menu = new MenuScreen(this);

            currentState = GameState.Menu;

            clearColor = new Color(.102f, .098f, .125f);
        }

        protected override void Initialize()
        {
            TargetElapsedTime = TimeSpan.FromTicks(166666);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            switch (currentState)
            {
                case GameState.Menu:
                    if (Keyboard.GetState().IsKeyDown(Keys.Z))
                    {
                        InitializeLevel();
                        currentState = GameState.Level;
                        menu.currentState = MenuScreen.DrawState.Skip;
                    }
                    break;
                case GameState.Level:
                    if (level.returnCounter >= 300)
                    {
                        DisableLevel();
                        currentState = GameState.Menu;
                        menu.currentState = MenuScreen.DrawState.Draw;
                    }
                    break;
            }

            MGLib.ParallaxManager.UpdateParallaxScrolling(this.GraphicsDevice);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(clearColor);

            base.Draw(gameTime);
        }

        void InitializeLevel()
        {
            level = new Level(this);
            level.enemyBaseManager.AddEnemyBase(new Vector2(2000, 2400));
            level.enemyBaseManager.AddEnemyBase(new Vector2(2000, -2000));
            level.enemyBaseManager.AddEnemyBase(new Vector2(-2000, 2000));
            level.enemyBaseManager.AddEnemyBase(new Vector2(-2000, -2400));
        }

        void DisableLevel()
        {
            level.enemyManager.Visible = false;
            level.enemyManager.Enabled = false;

            level.enemyBaseManager.Enabled = false;
            foreach (EnemyBase eb in level.enemyBaseManager.enemyBases)
            {
                eb.Visible = false;
                eb.Enabled = false;
            }
            level.enemyBaseManager.enemyBases.Clear();

            level.ship.gun.bulletManager.Visible = false;
            level.ship.gun.bulletManager.Enabled = false;
            level.ship.gun.Enabled = false;
            level.ship.Visible = false;
            level.ship.Enabled = false;

            level.explosionManager.Visible = false;
            level.explosionManager.Enabled = false;

            level.asteroidManager.Visible = false;
            level.asteroidManager.Enabled = false;

            level.boundary.Visible = false;
            level.boundary.Enabled = false;
            
            level.scoreKeeper.Visible = false;
            level.scoreKeeper.Enabled = false;

            level.minimap.Visible = false;
            level.minimap.Enabled = false;

            level.Enabled = false;

        }
    }
}
