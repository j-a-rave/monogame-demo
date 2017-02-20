/*A basic 2D camera with pan, zoom, and translation, some follow methods, for monogame.
 Jacob Rave Nov 2015*/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MGLib
{
    public static class Camera2D
    {
        public static float zoom = 0.7f;
        public static Matrix transform;
        public static Vector2 position = Vector2.Zero;
        public static float rotation = 0.0f;

        public static void Move(Vector2 amount)
        {
            position += amount;
        }

        public static Matrix GetTransformation(GraphicsDevice gDevice)
        {
            return Matrix.CreateTranslation(new Vector3(-position.X, -position.Y, 0)) *
                Matrix.CreateRotationZ(rotation) *
                Matrix.CreateScale(new Vector3(zoom, zoom, 1)) *
                Matrix.CreateTranslation(new Vector3(gDevice.Viewport.Width * 0.5f, gDevice.Viewport.Height * 0.5f, 0));
        }

        public static void SmoothFollow(Vector2 follow)
        {
            position = Vector2.Lerp(position, follow, 0.35f);
        }

        public static void SmoothRotate(float follow)
        {
            rotation = MathHelper.Lerp(rotation, follow, 0.35f);
        }
    }
}
