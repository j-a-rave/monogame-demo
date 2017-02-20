using Microsoft.Xna.Framework;

namespace MGLib
{
    public interface IPlayer
    {
        int PlayerIndex { get; }
        float Speed { get; set; }
        float MaxSpeed { get; }
        float Acceleration { get; }
        float Angle { get; set; }
        float AngleStep { get; }
        float MovementX { get; set; }
        float MovementY { get; set; }
    }
}
