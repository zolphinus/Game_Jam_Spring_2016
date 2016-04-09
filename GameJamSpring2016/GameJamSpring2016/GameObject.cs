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
using Box2DX.Collision;
using Box2DX.Dynamics;
using ExtensionMethods;

namespace GameJamSpring2016
{
    class GameObject
    {
        private Vector2 _position;
        private Sprite _sprite;
        private SpriteAnimationController[] _animations;
        private Body _body2D;
        private int _animationIndex;
        private ushort _categoryBit = 1;
        private ushort _personalBit;

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

        public int animationIndex
        {
            get { return _animationIndex; }
            set { _animationIndex = value; }
        }

        public ushort categoryBit
        {
            get { return _categoryBit++; }
            set { _categoryBit = value; }
        }

        public ushort personalBit
        {
            get { return _personalBit; }
            set { _personalBit = value; }
        }

        public void DrawObject(SpriteBatch batch, bool debug = false, TextRenderer rend = null, TextLayoutSettings settings = new TextLayoutSettings())
        {
            if(debug)
            {
                if (rend != null)
                {
                    rend.Draw(batch, "yo", body2D.GetPosition().ToScreenVector(), TwistedLogik.Ultraviolet.Color.Gold, settings);
                }
                else
                {
                    Console.WriteLine("Hey yo you didn't give me a TextRenderer");
                }
            }
        }

        /// <summary>
        /// Creates a box2D body based on the GameObject's current sprite.
        /// </summary>
        /// <param name="world">The physics world to create the body in.</param>
        public void SetupBodyFromSprite(World world)
        {
            BodyDef bDef = new BodyDef();
            bDef.Position = position.ToWorldVector();

            body2D = world.CreateBody(bDef);

            PolygonDef shapeDef = new PolygonDef();
            shapeDef.SetAsBox((float)animations[animationIndex].Width / Game.pixelsToMeters, (float)animations[animationIndex].Height / Game.pixelsToMeters);
            shapeDef.Density = 1F;
            shapeDef.Filter.CategoryBits = (ushort)(~(int)_personalBit);
            shapeDef.Filter.MaskBits = 0;

            body2D.CreateFixture(shapeDef);
            body2D.SetMassFromShapes();
        }
    }
}
