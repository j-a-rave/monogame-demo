using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
namespace Final
{
    static class WanderController
    {
        public static ProportionalRandom rand = new ProportionalRandom();

        public static Vector2 GetWanderDir(Enemy.RelativeQuadrant relQuad)
        {
            float wanderAngle = 0.0f;

            switch (relQuad)
            {
                case Enemy.RelativeQuadrant.A:
                    wanderAngle = rand.GetRandFloat(0.0f,(float)Math.PI / 2);
                    break;
                case Enemy.RelativeQuadrant.B:
                    wanderAngle = rand.GetRandFloat((float)Math.PI / 2, (float)Math.PI);
                    break;
                case Enemy.RelativeQuadrant.C:
                    wanderAngle = rand.GetRandFloat((float)Math.PI, (float)Math.PI * (3.0f / 2.0f));
                    break;
                case Enemy.RelativeQuadrant.D:
                    wanderAngle = rand.GetRandFloat((float)Math.PI * (3.0f / 2.0f), 2.0f * (float)Math.PI);
                    break;
                default:
                    break;
            }

            return new Vector2((float)Math.Cos(wanderAngle), (float)Math.Sin(wanderAngle));
        }

    }

    class ProportionalRandom : Random
    {
        public ProportionalRandom() { }

        public float GetRandFloat(float min, float max)
        {
            return (float)(Sample() * (max - min)) + min;
        }
    }
}
