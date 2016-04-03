using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameJamSpring2016
{
    class Weapon
    {
        private int _ammoCost;
        private int _knockback;
        private float _damageCooldown;
        private int _rofInterval;
        private IFiringPattern _pattern;

        public int ammoCost
        {
            get { return _ammoCost; }
            set { _ammoCost = value; }
        }
        public int knockback
        {
            get { return _knockback; }
            set { _knockback = value; }
        }
        public float damageCooldown
        {
            get { return _damageCooldown; }
            set { _damageCooldown = value; }
        }
        public int rofInterval
        {
            get { return _rofInterval; }
            set { _rofInterval = value; }
        }
        public IFiringPattern pattern
        {
            get { return _pattern; }
            set { _pattern = value; }
        }
    }
}
