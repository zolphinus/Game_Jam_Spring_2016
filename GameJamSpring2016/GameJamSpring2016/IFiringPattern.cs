using Box2DX.Dynamics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TwistedLogik.Ultraviolet;

namespace GameJamSpring2016
{
    interface IFiringPattern
    {
        Projectile[] ShootWeapon(Vector2 position, Vector2 direction) 
        {
            Projectile[] projectiles = null;
            return projectiles;
        }
    }
}
