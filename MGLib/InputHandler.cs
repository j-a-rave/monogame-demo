using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MGLib
{

    //handler interface

    public interface IInputHandler{
        bool wasPressed(int playerIndex, InputHandler.ButtonType button, Keys keys);
        bool wasButtonPressed(int playerIndex, InputHandler.ButtonType button);
        bool wasKeyPressed(Keys keys);

        KeyboardHandler kbs { get; }

        GamePadState[] gps { get; }
        GamePadHandler buttonHandler { get; }

#if !XBOX360
        MouseState mouseState {get;}
        MouseState prevMouseState{get;}
#endif
    }

    //handler class implementing interface
    public partial class InputHandler : GameComponent, IInputHandler
    {
        public enum ButtonType { A, B, Back, LeftShoulder, LeftStick, RightShoulder, RightStick, Start, X, Y }
        private KeyboardHandler keyboard;
        private GamePadHandler gamepadHandler = new GamePadHandler();
        private GamePadState[] gamePads = new GamePadState[4];

#if !XBOX360
        private MouseState ms;
        private MouseState prevMs;
#endif

        private bool allowsExiting; //uses back or esc to exit, xbox used to force

        public InputHandler(Game game) : this(game, false){ }
        public InputHandler(Game game, bool allowsExiting)
            : base(game)
        {
            this.allowsExiting = allowsExiting;

            game.Services.AddService(typeof(IInputHandler), this);

            keyboard = new KeyboardHandler();

#if !XBOX360
            Game.IsMouseVisible = true;
            prevMs = Mouse.GetState();
#endif
        }

        public override void Initialize()
        {
            //init code here
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            keyboard.Update();
            gamepadHandler.Update();

            if (allowsExiting)
            {
                if (keyboard.isKeyDown(Keys.Escape) || gamepadHandler.buttonPressed(0,ButtonType.Back))
                    Game.Exit();
            }

#if !XBOX360
            prevMs = ms;
            ms = Mouse.GetState();
#endif
            if (GamePad.GetState(PlayerIndex.One).IsConnected)
                gamePads[0] = gamepadHandler.gamePadStates[0];
            if (GamePad.GetState(PlayerIndex.Two).IsConnected)
                gamePads[1] = gamepadHandler.gamePadStates[1];
            if (GamePad.GetState(PlayerIndex.Three).IsConnected)
                gamePads[2] = gamepadHandler.gamePadStates[2];
            if (GamePad.GetState(PlayerIndex.Four).IsConnected)
                gamePads[3] = gamepadHandler.gamePadStates[3];

            base.Update(gameTime);
        }

        #region IInputHandler implementation
        public bool wasPressed(int playerIndex, ButtonType button, Keys keys)
        {
            if (keyboard.wasKeyPressed(keys) || gamepadHandler.buttonPressed(playerIndex, button))
                return true;
            else
                return false;
        }
        public bool wasButtonPressed(int playerIndex, ButtonType button)
        {
            return gamepadHandler.buttonPressed(playerIndex, button);
        }
        public bool wasKeyPressed(Keys keys)
        {
            return keyboard.wasKeyPressed(keys);
        }
        public KeyboardHandler kbs
        {
            get { return keyboard; }
        }
        public GamePadHandler buttonHandler
        {
            get { return gamepadHandler;}
        }
        public GamePadState[] gps
        {
            get { return gamePads; }
        }

#if !XBOX360
        public MouseState mouseState
        {
            get { return ms; }
        }
        public MouseState prevMouseState
        {
            get { return prevMs; }
        }
#endif
        #endregion

    }

    public class KeyboardHandler{
        private KeyboardState prevKbs;
        private KeyboardState kbs;

        public KeyboardHandler()
        {
            prevKbs = Keyboard.GetState();
        }
        //convenient key checks
        public bool isKeyDown(Keys key)
        {
            return kbs.IsKeyDown(key);
        }
        public bool isHoldingKey(Keys key)
        {
            return kbs.IsKeyDown(key) && prevKbs.IsKeyDown(key);
        }
        public bool wasKeyPressed(Keys key)
        {
            return kbs.IsKeyDown(key) && prevKbs.IsKeyUp(key);
        }
        public bool hasReleasedKey(Keys key)
        {
            return kbs.IsKeyDown(key) && prevKbs.IsKeyUp(key);
        }
        public bool wasAnyKeyPressed()
        {
            Keys[] keysPressed = kbs.GetPressedKeys();
            if (keysPressed.Length > 0)
            {
                foreach (Keys k in keysPressed)
                {
                    if (prevKbs.IsKeyUp(k))
                        return true;
                }
            }
            return false;
        }

        public void Update()
        {
            prevKbs = kbs;
            kbs = Keyboard.GetState();
        }
    }

    public class GamePadHandler
    {
        private GamePadState[] prevGps = new GamePadState[4];
        private GamePadState[] gps = new GamePadState[4];
        public GamePadState[] gamePadStates { get { return gps; } }

        public GamePadHandler()
        {
            if (GamePad.GetState(PlayerIndex.One).IsConnected)
                prevGps[0] = GamePad.GetState(PlayerIndex.One);
            if (GamePad.GetState(PlayerIndex.Two).IsConnected)
                prevGps[1] = GamePad.GetState(PlayerIndex.Two);
            if (GamePad.GetState(PlayerIndex.Three).IsConnected)
                prevGps[2] = GamePad.GetState(PlayerIndex.Three);
            if (GamePad.GetState(PlayerIndex.Four).IsConnected)
                prevGps[3] = GamePad.GetState(PlayerIndex.Four);
        }

        public void Update()
        {
            prevGps = gps;

            if (GamePad.GetState(PlayerIndex.One).IsConnected)
                gps[0] = GamePad.GetState(PlayerIndex.One);
            if (GamePad.GetState(PlayerIndex.Two).IsConnected)
                gps[1] = GamePad.GetState(PlayerIndex.Two);
            if (GamePad.GetState(PlayerIndex.Three).IsConnected)
                gps[2] = GamePad.GetState(PlayerIndex.Three);
            if (GamePad.GetState(PlayerIndex.Four).IsConnected)
                gps[3] = GamePad.GetState(PlayerIndex.Four);
        }

        public bool buttonPressed(int pi, InputHandler.ButtonType button)
        {
            switch (button)
            {
                case InputHandler.ButtonType.A:
                    {
                        return (gps[pi].Buttons.A == ButtonState.Pressed && prevGps[pi].Buttons.A == ButtonState.Released);
                    }
                case InputHandler.ButtonType.B:
                    {
                        return (gps[pi].Buttons.B == ButtonState.Pressed && prevGps[pi].Buttons.B == ButtonState.Released);
                    }
                case InputHandler.ButtonType.Back:
                    {
                        return (gps[pi].Buttons.Back == ButtonState.Pressed && prevGps[pi].Buttons.Back == ButtonState.Released);
                    }
                case InputHandler.ButtonType.LeftShoulder:
                    {
                        return (gps[pi].Buttons.LeftShoulder == ButtonState.Pressed && prevGps[pi].Buttons.LeftShoulder == ButtonState.Released);
                    }
                case InputHandler.ButtonType.LeftStick:
                    {
                        return (gps[pi].Buttons.LeftStick == ButtonState.Pressed && prevGps[pi].Buttons.LeftStick == ButtonState.Released);
                    }
                case InputHandler.ButtonType.RightShoulder:
                    {
                        return (gps[pi].Buttons.RightShoulder == ButtonState.Pressed && prevGps[pi].Buttons.RightShoulder == ButtonState.Released);
                    }
                case InputHandler.ButtonType.RightStick:
                    {
                        return (gps[pi].Buttons.RightStick == ButtonState.Pressed && prevGps[pi].Buttons.RightStick == ButtonState.Released);
                    }
                case InputHandler.ButtonType.Start:
                    {
                        return (gps[pi].Buttons.Start == ButtonState.Pressed && prevGps[pi].Buttons.Start == ButtonState.Released);
                    }
                case InputHandler.ButtonType.X:
                    {
                        return (gps[pi].Buttons.X == ButtonState.Pressed && prevGps[pi].Buttons.X == ButtonState.Released);
                    }
                case InputHandler.ButtonType.Y:
                    {
                        return (gps[pi].Buttons.Y == ButtonState.Pressed && prevGps[pi].Buttons.Y == ButtonState.Released);
                    }
                default:
                    throw (new ArgumentException());
            }

        }
    }
}
