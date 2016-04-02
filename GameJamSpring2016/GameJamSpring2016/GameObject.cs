using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GameJamSpring2016.Assets;
using GameJamSpring2016.Input;
using TwistedLogik.Nucleus;
using TwistedLogik.Nucleus.Text;
using TwistedLogik.Ultraviolet;
using TwistedLogik.Ultraviolet.Content;
using TwistedLogik.Ultraviolet.Graphics.Graphics2D;
using TwistedLogik.Ultraviolet.Graphics.Graphics2D.Text;
using TwistedLogik.Ultraviolet.OpenGL;
using TwistedLogik.Ultraviolet.Platform;

namespace SafeProjectName
{
    class GameObject
    {
        private Vector2 _position;
        private Sprite _sprite;
        private SpriteAnimationController[] _animations;
        private Body _body2D;

        public Vector2 position
        {
            get { return _position; }
            set { _position = value; }
        }

        public Sprite sprite
        {
            get { return _sprite; }
            set { _sprite = value; }
        }

        public SpriteAnimationController[] animations
        {
            get { return _animations; }
            set { _animations = value; }
        }

        public Body body2D
        {
            get { return _body2D; }
            set { _body2D = value; }
        }
    }
}
