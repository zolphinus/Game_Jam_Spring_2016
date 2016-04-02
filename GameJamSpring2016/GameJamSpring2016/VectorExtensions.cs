using Box2DX.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TwistedLogik.Ultraviolet;
using GameJamSpring2016;

namespace ExtensionMethods
{
    public static class VectorExtensions
    {
        public static Vector2 ToScreenVector(this Vec2 v)
        {
            return new Vector2(v.X*Game.pixelsToMeters + (float)Game.windowWidth / 2F, v.Y * Game.pixelsToMeters + (float)Game.windowHeight / 2F);
        }

        public static Vec2 ToWorldVector(this Vector2 v)
        {
            return new Vec2((v.X - (float)Game.windowWidth / 2F) / Game.pixelsToMeters, (v.Y - (float)Game.windowHeight / 2F) / Game.pixelsToMeters);
        }
    }
}
