using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MGLib
{
    public class ScoreKeeper : DrawableGameComponent
    {
        public string winMessage, loseMessage, fontName, iconName;

        SpriteFont font;
        Vector2 scoreString, livesString, livesIcon, levelString;
        SpriteBatch sb;
        Texture2D icon;

        public enum WinStates { None, Win, Lose };

        #region static members

        static int lives, score, level;

        public static WinStates winState;

        public static int Lives { get { return lives; } }
        public static int Score { get { return score; } }
        public static int Level { get { return level; } }

        public static void addScore(int val) { score += val; }
        public static bool die() {
            lives--;
            if (lives <= 0)
                return true;
            return false;
        }
        public static void oneUp() { lives++; }
        public static void levelUp() { level++; }
        public static void setInitialScoreAndLives(int score, int lives)
        {
            ScoreKeeper.score = score; //i'm guessing zero, but just in case. hey.
            ScoreKeeper.lives = lives;
            ScoreKeeper.level = 1; //why not
            winState = WinStates.None;
        }

        #endregion

        public ScoreKeeper(Game game)
            : this(game, "You win!", "You Lose!", "font", "genericLife") { }

        public ScoreKeeper(Game game, string winMessage, string loseMessage) : this(game, winMessage, loseMessage, "font", "genericLife") { }

        public ScoreKeeper(Game game, string winMessage, string loseMessage, string fontName, string iconName)
            : base(game)
        {
            this.winMessage = winMessage;
            this.loseMessage = loseMessage;
            this.fontName = fontName;
            this.iconName = iconName;

            game.Components.Add(this);
        }

        protected override void LoadContent()
        {
            sb = new SpriteBatch(this.Game.GraphicsDevice);

            font = this.Game.Content.Load<SpriteFont>(this.fontName);
            icon = this.Game.Content.Load<Texture2D>(this.iconName);

            this.scoreString = Vector2.Zero;
            this.livesString = new Vector2(this.Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height - (icon.Bounds.Height*1.5f));
            this.livesIcon = new Vector2(this.Game.GraphicsDevice.Viewport.Width - (8 + icon.Bounds.Width / 2), this.Game.GraphicsDevice.Viewport.Height - 24);
            this.levelString = new Vector2(0.0f, this.Game.GraphicsDevice.Viewport.Height - font.MeasureString("Level").Y);

            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            sb.Begin();

            sb.DrawString(font, "Score: " + ScoreKeeper.Score.ToString(), scoreString, Color.White);
            sb.DrawString(font, "Lives: " + ScoreKeeper.Lives.ToString(), livesString + new Vector2(-font.MeasureString("Lives: " + ScoreKeeper.Score.ToString()).X, 0.0f), Color.White);
            sb.DrawString(font, "Level " + ScoreKeeper.Level.ToString(), levelString, Color.White);

            for (int i = 0; i < ScoreKeeper.Lives; i++)
            {
                sb.Draw(icon, livesIcon - new Vector2((8 + icon.Bounds.Width / 2) * i, 0), null, Color.White, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.0f);
            }

            switch (winState)
            {
                case WinStates.Win:
                    sb.DrawString(font, winMessage, new Vector2((this.Game.GraphicsDevice.Viewport.Width / 2) - (font.MeasureString(winMessage).X / 2), (this.Game.GraphicsDevice.Viewport.Height / 2) - (font.MeasureString(winMessage).Y *4)), Color.White);
                    break;
                case WinStates.Lose:
                    sb.DrawString(font, loseMessage, new Vector2((this.Game.GraphicsDevice.Viewport.Width / 2) - (font.MeasureString(loseMessage).X / 2), (this.Game.GraphicsDevice.Viewport.Height / 2) - (font.MeasureString(loseMessage).Y *4)), Color.White);
                    break;
                default:
                    break;
            }

            sb.End();
            base.Draw(gameTime);
        }

    }
}
