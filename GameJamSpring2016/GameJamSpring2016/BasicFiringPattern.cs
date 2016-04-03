using Box2DX.Dynamics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TwistedLogik.Ultraviolet;

namespace GameJamSpring2016
{
    class BasicFiringPattern : IFiringPattern
    {
        public Projectile[] ShootWeapon(Vector2 position, Vector2 direction)
        {
            Projectile[] projectiles = null;

            Projectile projectile = new Projectile();
            projectile.damage = 1;
            projectile.position = position;
            projectile.projectileSpeed = 10;
            projectile.fireDirection = direction;

            projectiles = new Projectile[1];
            projectiles[0] = projectile;
            return projectiles;
        }
    }
}
