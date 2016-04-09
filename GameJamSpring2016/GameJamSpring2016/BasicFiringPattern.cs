using Box2DX.Dynamics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TwistedLogik.Ultraviolet;
using ExtensionMethods;
using Box2DX.Common;

namespace GameJamSpring2016
{
    class BasicFiringPattern : IFiringPattern
    {
        public Projectile[] ShootWeapon(object[] posAndDir)
        {
            Vector2 tempPos = (Vector2)posAndDir[0];
            Vector2 tempDir = (Vector2)posAndDir[1];

            //the signs of these tells the correct quadrant
            double difX = tempDir.X - tempPos.X;
            double difY = tempDir.Y - tempPos.Y;
            
            //the angle of the shot in radians
            double angle = System.Math.Atan(difY / difX);

            Projectile[] projectiles = null;

            Projectile projectile = new Projectile();
            projectile.damage = 1;
            projectile.speed = 20;
            projectile.position = (Vector2)posAndDir[0];
            projectile.fireDirection = new Vec2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle)).ToScreenVector();

            projectiles = new Projectile[1];
            projectiles[0] = projectile;
            return projectiles;
        }
    }
}
