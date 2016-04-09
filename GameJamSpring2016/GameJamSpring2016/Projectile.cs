using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TwistedLogik.Ultraviolet;

namespace GameJamSpring2016
{
    class Projectile : GameObject
    {
        private int _damage;
        private float _speed;
        private Vector2 _fireDirection;
        private bool _canDamage;

        public int damage
        {
            get { return _damage; }
            set { _damage = value; }
        }
        public float speed
        {
            get { return _speed; }
            set { _speed = value; }
        }
        public Vector2 fireDirection
        {
            get { return _fireDirection; }
            set { _fireDirection = value; }
        }
        public bool canDamage
        {
            get { return _canDamage; }
            set { _canDamage = value; }
        }
    }
}
