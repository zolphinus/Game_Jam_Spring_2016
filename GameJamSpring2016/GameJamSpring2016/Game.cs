using System;
using System.IO;
using System.Text;
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
using Box2DX.Dynamics;
using Box2DX.Collision;
using Box2DX.Common;
using TwistedLogik.Ultraviolet.Input;
using ExtensionMethods;
using TwistedLogik.Ultraviolet.Graphics;
using System.Collections;
using System.Collections.Generic;

namespace GameJamSpring2016
{
    /// <summary>
    /// Represents the main application object.
    /// </summary>
#if ANDROID
    [Android.App.Activity(Label = "GameActivity", MainLauncher = true, ConfigurationChanges = 
        Android.Content.PM.ConfigChanges.Orientation | 
        Android.Content.PM.ConfigChanges.ScreenSize | 
        Android.Content.PM.ConfigChanges.KeyboardHidden)]
    public class Game : UltravioletActivity
#else
    public class Game : UltravioletApplication
#endif
    {
        //Test comment
        /// <summary>
        /// Initializes a new instance of the <see cref="Game"/> class.
        /// </summary>
        public Game() : base("YOUR_ORGANIZATION", "GameJamSpring2016") { }

        /// <summary>
        /// The application's entry point.
        /// </summary>
        /// <param name="args">An array containing the application's command line arguments.</param>
        public static void Main(String[] args)
        {
            using (var game = new Game())
            {
                game.Run();
            }
        }

        /// <summary>
        /// Called when the application is creating its Ultraviolet context.
        /// </summary>
        /// <returns>The Ultraviolet context.</returns>
        protected override UltravioletContext OnCreatingUltravioletContext()
        {
            var configuration = new OpenGLUltravioletConfiguration();
            PopulateConfiguration(configuration);

#if DEBUG
            configuration.Debug = true;
            configuration.DebugLevels = DebugLevels.Error | DebugLevels.Warning;
            configuration.DebugCallback = (uv, level, message) =>
            {
                System.Diagnostics.Debug.WriteLine(message);
            };
#endif

            return new OpenGLUltravioletContext(this, configuration);
        }

        /// <summary>
        /// Called after the application has been initialized.
        /// </summary>
        protected override void OnInitialized()
        {
            SetFileSourceFromManifestIfExists("UltravioletGame.Content.uvarc");

            MouseInputBinding mib = new MouseInputBinding(this.Ultraviolet, MouseButton.Left);
            mouse = mib.Mouse;

            //initialize the firingBullet bool
            firingBullet = false;

            //Setup Box2D world
            worldAABB = new AABB();
            worldAABB.Center.Set(0F, 0F);
            worldAABB.LowerBound.Set(-100F);
            worldAABB.UpperBound.Set(100F);
            physicsWorld = new World(worldAABB, new Vec2(0F, 9.8F), true);

            BodyDef physicsTestDef = new BodyDef();
            physicsTestDef.Position.Set(0F, 0F);

            //physicsTestObject = new GameObject();
            //physicsTestObject.body2D = physicsWorld.CreateBody(physicsTestDef);

            PolygonDef physicsTestShapeDef = new PolygonDef();

            physicsTestShapeDef.SetAsBox(1F, 1F);
            physicsTestShapeDef.Density = 1F;

            //physicsTestObject.body2D.CreateFixture(physicsTestShapeDef);
            //physicsTestObject.body2D.SetMassFromShapes();

            BodyDef groundDef = new BodyDef();
            groundDef.Position.Set(0F, 3F);

            groundTestObject = new GameObject();
            groundTestObject.body2D = physicsWorld.CreateBody(groundDef);

            PolygonDef groundShapeDef = new PolygonDef();

            groundShapeDef.SetAsBox(1F, 1F);
            groundTestObject.body2D.CreateFixture(groundShapeDef);

            base.OnInitialized();
        }

        /// <summary>
        /// Called when the application is loading content.
        /// </summary>
        protected override void OnLoadingContent()
        {
            this.content = ContentManager.Create("Content");

            LoadLocalizationDatabases();
            //LoadInputBindings();
            LoadContentManifests();
            LoadCursors();

            //player object initialization
            this.sprite = this.content.Load<Sprite>(GlobalSpriteID.Green_Corn);
            this.playerObj = new PlayerObject();
            this.playerObj.playerHealth = 20;
            this.playerObj.position = new Vector2(32, 32);
            this.playerObj.sprite = this.sprite;
            this.playerObj.animationIndex = 0;
            this.playerObj.animations = new SpriteAnimationController[] { sprite["Green_Corn"].Controller };
            this.playerObj.SetupBodyFromSprite(physicsWorld);

            //set up the queue for projectiles
            this.gameProjectiles = new Queue<Projectile>();

            this.spriteBatch = SpriteBatch.Create();
            this.spriteFont = this.content.Load<SpriteFont>(GlobalFontID.SegoeUI);

            this.textRenderer = new TextRenderer();
            this.textFormatter = new StringFormatter();
            this.textBuffer = new StringBuilder();

            GC.Collect(2);

            base.OnLoadingContent();
        }

        /// <summary>
        /// Loads the application's localization databases.
        /// </summary>
        protected void LoadLocalizationDatabases()
        {
            var fss = FileSystemService.Create();
            var databases = content.GetAssetFilePathsInDirectory("Localization", "*.xml");
            foreach (var database in databases)
            {
                using (var stream = fss.OpenRead(database))
                {
                    Localization.Strings.LoadFromStream(stream);
                }
            }
        }

        /// <summary>
        /// Loads the game's input bindings.
        /// </summary>
        protected void LoadInputBindings()
        {
            var inputBindingsPath = Path.Combine(GetRoamingApplicationSettingsDirectory(), "InputBindings.xml");
            Ultraviolet.GetInput().GetActions().Load(inputBindingsPath, throwIfNotFound: false);
        }

        /// <summary>
        /// Saves the game's input bindings.
        /// </summary>
        protected void SaveInputBindings()
        {
            var inputBindingsPath = Path.Combine(GetRoamingApplicationSettingsDirectory(), "InputBindings.xml");
            Ultraviolet.GetInput().GetActions().Save(inputBindingsPath);
        }

        /// <summary>
        /// Loads the game's content manifest files.
        /// </summary>
        protected void LoadContentManifests()
        {
            var uvContent = Ultraviolet.GetContent();

            var contentManifestFiles = this.content.GetAssetFilePathsInDirectory("Manifests");
            uvContent.Manifests.Load(contentManifestFiles);

            uvContent.Manifests["Global"]["Fonts"].PopulateAssetLibrary(typeof(GlobalFontID));
            uvContent.Manifests["Global"]["Sprites"].PopulateAssetLibrary(typeof(GlobalSpriteID));
        }

        /// <summary>
        /// Loads the game's cursors.
        /// </summary>
        protected void LoadCursors()
        {
            this.cursors = this.content.Load<CursorCollection>("Cursors/Cursors");
            Ultraviolet.GetPlatform().Cursor = this.cursors["Normal"];
        }

        /// <summary>
        /// Called when the application state is being updated.
        /// </summary>
        /// <param name="uv">The Ultraviolet context.</param>
        /// <param name="time">Time elapsed since the last call to Update.</param>
        protected override void OnUpdating(UltravioletTime time)
        {
            if (Ultraviolet.GetInput().GetActions().MoveLeft.IsPressed() || Ultraviolet.GetInput().GetActions().MoveLeft.IsDown())
            {
                MoveGameObject(new Vec2(-30, 0), playerObj);
            }
            if (Ultraviolet.GetInput().GetActions().MoveRight.IsPressed() || Ultraviolet.GetInput().GetActions().MoveRight.IsDown())
            {
                MoveGameObject(new Vec2(+30, 0), playerObj);
            }
            if (Ultraviolet.GetInput().GetActions().Shoot.IsPressed())
            {
                //retrieve projectiles from shooting
                Projectile[] temp = playerObj.playerWeapon.pattern.ShootWeapon(playerObj.position, new Vector2(mouse.X, mouse.Y));

                //add returned projectiles to the queue
                for (int i = 0; i < temp.Length; i++)
                {
                    gameProjectiles.Enqueue(temp[i]);
                }
            }
            if (Ultraviolet.GetInput().GetActions().ExitApplication.IsPressed())
            {
                Exit();
            }

            if(Ultraviolet.GetInput().GetActions().Jump.IsPressed())
            {
                //physicsTestObject.body2D.ApplyImpulse(new Vec2(0F, -20F), new Vec2(0F, 0F));
                
            }

            //FIRE THE BULLETS (just 1) (and apply sprites as they appear)
            if (gameProjectiles.Count >= 1)
            {
                firedBullet = gameProjectiles.Dequeue();
                firedBullet.sprite = content.Load<Sprite>(GlobalSpriteID.Brown_Kernel);
                firedBullet.animationIndex = 0;
                firedBullet.animations = new SpriteAnimationController[] { sprite["Brown_Kernel"].Controller };
                firedBullet.SetupBodyFromSprite(physicsWorld);
                firedBullet.body2D.ApplyForce(firedBullet.fireDirection.ToWorldVector(), firedBullet.position.ToWorldVector());

                firingBullet = true;
            }
            else
            {
                firingBullet = false;
            }

            physicsWorld.Step(1F / 60F, 10, 10);
            playerObj.position = playerObj.body2D.GetPosition().ToScreenVector();

            base.OnUpdating(time);
        }
        
        /// <summary>
        /// Called when the scene is being rendered.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to Draw.</param>
        protected override void OnDrawing(UltravioletTime time)
        {
            spriteBatch.Begin();

            //player sprite draw
            spriteBatch.DrawSprite(this.playerObj.animations[this.playerObj.animationIndex], this.playerObj.position);

            //if a bullet is being fired, draw it
            if (firingBullet)
            {
                spriteBatch.DrawSprite(this.firedBullet.animations[this.firedBullet.animationIndex], this.firedBullet.position);
            }

            textFormatter.Reset();
            textFormatter.AddArgument(Ultraviolet.GetGraphics().FrameRate);
            textFormatter.AddArgument(GC.GetTotalMemory(false) / 1024);
            textFormatter.AddArgument(Environment.Is64BitProcess ? "64-bit" : "32-bit");
            textFormatter.Format("{0:decimals:2} FPS\nAllocated: {1:decimals:2} kb\n{2}", textBuffer);

            spriteBatch.DrawString(spriteFont, textBuffer, Vector2.One * 8f, TwistedLogik.Ultraviolet.Color.White);

            var size = Ultraviolet.GetPlatform().Windows.GetCurrent().ClientSize;
            var settings = new TextLayoutSettings(spriteFont, size.Width, size.Height, TextFlags.AlignCenter | TextFlags.AlignMiddle);
            textRenderer.Draw(spriteBatch, "Welcome to the |c:FFFF00C0|Ultraviolet Framework|c|!", Vector2.Zero, TwistedLogik.Ultraviolet.Color.White, settings);

            //physicsTestObject.DrawObject(spriteBatch, true, textRenderer, settings);
            groundTestObject.DrawObject(spriteBatch, true, textRenderer, settings);

            spriteBatch.End();

            base.OnDrawing(time);
        }

        protected override void OnWindowDrawing(UltravioletTime time, IUltravioletWindow window)
        {
            windowHeight = window.ClientSize.Height;
            windowWidth = window.ClientSize.Width;

            base.OnWindowDrawing(time, window);
        }

        /// <summary>
        /// Called when the application is being shut down.
        /// </summary>
        protected override void OnShutdown()
        {
            SaveInputBindings();

            base.OnShutdown();
        }

        /// <summary>
        /// Releases resources associated with the object.
        /// </summary>
        /// <param name="disposing">true if the object is being disposed; false if the object is being finalized.</param>
        protected override void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                SafeDispose.DisposeRef(ref content);
            }
            base.Dispose(disposing);
        }

        private void MoveGameObject(Vec2 direction, GameObject gameObj)
        {
            gameObj.body2D.ApplyForce(direction, gameObj.position.ToWorldVector());
        }

        // The global content manager.  Manages any content that should remain loaded for the duration of the game's execution.
        public ContentManager content;

        private MouseDevice mouse;

        // Game resources.
        private CursorCollection cursors;
        private SpriteFont spriteFont;
        private SpriteBatch spriteBatch;
        private TextRenderer textRenderer;
        private StringFormatter textFormatter;
        private StringBuilder textBuffer;

        //Physics variables
        public static readonly float pixelsToMeters = 128;
        private static int _windowHeight;
        public static int windowHeight
        {
            get
            {
                return _windowHeight;
            }

            private set
            {
                _windowHeight = value;
            }
        }

        private static int _windowWidth;
        public static int windowWidth
        {
            get
            {
                return _windowWidth;
            }

            private set
            {
                _windowWidth = value;
            }
        }

        private World physicsWorld;
        private AABB worldAABB;

        //private GameObject physicsTestObject;
        private GameObject groundTestObject;

        //Player Object
        private PlayerObject playerObj;
        private Sprite sprite;

        //Queue for the projectiles in game
        private Queue<Projectile> gameProjectiles;
        private Projectile firedBullet;
        private bool firingBullet;
    }
}
