using TwistedLogik.Ultraviolet;
using TwistedLogik.Ultraviolet.Input;
using TwistedLogik.Ultraviolet.Graphics.Graphics2D;
using Box2DX.Dynamics;
using ExtensionMethods;

namespace GameJamSpring2016
{
    class PlayerObject : GameObject
    {
        private int _playerHealth;
        private Weapon _playerWeapon;

        public int playerHealth
        {
            get { return _playerHealth; }
            set { _playerHealth = value; }
        }

        public Weapon playerWeapon
        {
            get { return _playerWeapon; }
            set { _playerWeapon = value; }
        }
    }
}
