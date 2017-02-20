using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MGLib
{
    //handles input and movement, dependent on inputhandler
    public static class ControlManager
    {
        //keyboard controls for all 4 players: arrows, wasd, ijkl, tfgh
        static Keys[,] wasdIndices = new Keys[4,4] {{Keys.Up,Keys.Left,Keys.Down,Keys.Right},{Keys.W,Keys.A,Keys.S,Keys.D},{Keys.I,Keys.J,Keys.K,Keys.L},{Keys.T,Keys.F,Keys.G,Keys.H}};
        public enum IndexedKeys {Up, Left, Down, Right};
        static InputHandler iHandler;
        static bool[] controllerIsActive = new bool[4] { false, false, false, false };
        public static Vector2 currentRotation; //to pass to player constituents

        public static void addHandler(Game game)
        {
            iHandler = new InputHandler(game);
            game.Components.Add(iHandler);
        }


        public static void setMovement(IPlayer player)
        {
            //check for active gamepad if it is yet inactive. use kb.
            if (!controllerIsActive[player.PlayerIndex])
            {
                //controller support isn't necessary here, but if you want to add it, uncomment this.
                //useController(player.PlayerIndex);

                if (isIndexedKeyDown(player, IndexedKeys.Up))
                {
                    player.Speed += player.Acceleration;
                }
                if (isIndexedKeyDown(player, IndexedKeys.Down))
                {
                    player.Speed -= player.Acceleration;
                }

                if (isIndexedKeyDown(player, IndexedKeys.Left))
                {
                    player.Angle -= player.AngleStep;
                }
                if (isIndexedKeyDown(player, IndexedKeys.Right))
                {
                    player.Angle += player.AngleStep;
                }
            }

            //use gamepad
            else
            {
                //left thumb steers
                player.Angle += iHandler.gps[player.PlayerIndex].ThumbSticks.Left.X * player.AngleStep;

                //and accelerates
                player.Speed += iHandler.gps[player.PlayerIndex].ThumbSticks.Left.Y * player.Acceleration;
            }

            if (player.Speed > player.MaxSpeed)
            {
                player.Speed = player.MaxSpeed;
            }
            else if (player.Speed < 0)
            {
                player.Speed = 0;
            }

            //TODO: might need to add friction to make it less unwieldy, but this is in SPACE...
            //return new Vector2((float)Math.Cos(player.Angle), (float)Math.Sin(player.Angle)) * player.Speed; NO 'NEW' LOOPS

            //player.MovementX = (float)Math.Cos(MathHelper.ToRadians(player.Angle - 90)) * player.Speed;
            //player.MovementY = (float)Math.Sin(MathHelper.ToRadians(player.Angle - 90)) * player.Speed;

            //just for player constituents
            currentRotation.X = (float)Math.Cos(MathHelper.ToRadians(player.Angle - 90));
            currentRotation.Y = (float)Math.Sin(MathHelper.ToRadians(player.Angle - 90));

            player.MovementX = currentRotation.X * player.Speed;
            player.MovementY = currentRotation.Y * player.Speed;
        }

        public static bool isFiring() //TODO singleplayer keyboard only, make an enum of fire buttons if you ever support multiplayer.
        {
            return iHandler.kbs.isKeyDown(Keys.Z);
        }
        public static bool isAutoFiring()
        {
            return iHandler.kbs.isHoldingKey(Keys.Z);
        }

        public static bool useController(int index)
        {
            //continues to use my awful solution. A TO PLAY *rimshot*
            controllerIsActive[index] = iHandler.gps[index].IsButtonDown(Buttons.A);
            return controllerIsActive[index];
        }

        public static bool isIndexedKeyDown(IPlayer player, IndexedKeys key)
        {
            return iHandler.kbs.isKeyDown(wasdIndices[player.PlayerIndex, (int)key]);
        }
    }
}
