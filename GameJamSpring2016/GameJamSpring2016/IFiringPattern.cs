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
        Projectile[] ShootWeapon(object[] parameters);
    }
}
