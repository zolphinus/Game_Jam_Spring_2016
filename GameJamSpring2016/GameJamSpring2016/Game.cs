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

            //Setup Box2D world
            worldAABB = new AABB();
            worldAABB.Center.Set(0F, 0F);
            worldAABB.LowerBound.Set(-100F);
            worldAABB.UpperBound.Set(100F);
            physicsWorld = new World(worldAABB, new Vec2(0F, 9.8F), true);

            BodyDef physicsTestDef = new BodyDef();
            physicsTestDef.Position.Set(0F, 0F);

            physicsTestObject = new GameObject();
            physicsTestObject.body2D = physicsWorld.CreateBody(physicsTestDef);

            PolygonDef physicsTestShapeDef = new PolygonDef();

            physicsTestShapeDef.SetAsBox(1F, 1F);
            physicsTestShapeDef.Density = 1F;

            physicsTestObject.body2D.CreateFixture(physicsTestShapeDef);
            physicsTestObject.body2D.SetMassFromShapes();

            BodyDef groundDef = new BodyDef();
            groundDef.Position.Set(0F, 3F);

            groundTestObject = new GameObject(); 
            groundTestObject.body2D = physicsWorld.CreateBody(groundDef);

            PolygonDef groundShapeDef = new PolygonDef();

            groundShapeDef.SetAsBox(1F, 1F);
            groundTestObject.body2D.CreateFixture(groundShapeDef);

            return new OpenGLUltravioletContext(this, configuration);
        }

        /// <summary>
        /// Called after the application has been initialized.
        /// </summary>
        protected override void OnInitialized()
        {
            SetFileSourceFromManifestIfExists("UltravioletGame.Content.uvarc");

            base.OnInitialized();
        }

        /// <summary>
        /// Called when the application is loading content.
        /// </summary>
        protected override void OnLoadingContent()
        {
            this.content = ContentManager.Create("Content");

            LoadLocalizationDatabases();
            LoadInputBindings();
            LoadContentManifests();
            LoadCursors();

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
            if (Ultraviolet.GetInput().GetActions().ExitApplication.IsPressed())
            {
                Exit();
            }

            physicsWorld.Step(1F / 60F, 10, 10);

            base.OnUpdating(time);
        }

        /// <summary>
        /// Called when the scene is being rendered.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to Draw.</param>
        protected override void OnDrawing(UltravioletTime time)
        {
            spriteBatch.Begin();

            textFormatter.Reset();
            textFormatter.AddArgument(Ultraviolet.GetGraphics().FrameRate);
            textFormatter.AddArgument(GC.GetTotalMemory(false) / 1024);
            textFormatter.AddArgument(Environment.Is64BitProcess ? "64-bit" : "32-bit");
            textFormatter.Format("{0:decimals:2} FPS\nAllocated: {1:decimals:2} kb\n{2}", textBuffer);

            spriteBatch.DrawString(spriteFont, textBuffer, Vector2.One * 8f, TwistedLogik.Ultraviolet.Color.White);

            var size = Ultraviolet.GetPlatform().Windows.GetCurrent().ClientSize;
            var settings = new TextLayoutSettings(spriteFont, size.Width, size.Height, TextFlags.AlignCenter | TextFlags.AlignMiddle);
            textRenderer.Draw(spriteBatch, "Welcome to the |c:FFFF00C0|Ultraviolet Framework|c|!", Vector2.Zero, TwistedLogik.Ultraviolet.Color.White, settings);

            physicsTestObject.DrawObject(spriteBatch, true, textRenderer, settings);
            groundTestObject.DrawObject(spriteBatch, true, textRenderer, settings);

            spriteBatch.End();

            base.OnDrawing(time);
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

        // The global content manager.  Manages any content that should remain loaded for the duration of the game's execution.
        private ContentManager content;

        // Game resources.
        private CursorCollection cursors;
        private SpriteFont spriteFont;
        private SpriteBatch spriteBatch;
        private TextRenderer textRenderer;
        private StringFormatter textFormatter;
        private StringBuilder textBuffer;

        //Physics variables
        public static readonly float pixelsToMeters = 128;

        private World physicsWorld;
        private AABB worldAABB;

        private GameObject physicsTestObject;
        private GameObject groundTestObject;
    }
}
