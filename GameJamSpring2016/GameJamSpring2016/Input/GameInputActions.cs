using TwistedLogik.Ultraviolet;
using TwistedLogik.Ultraviolet.Input;

namespace GameJamSpring2016.Input
{
    /// <summary>
    /// Contains the game's input actions.
    /// </summary>
    public sealed class GameInputActions : InputActionCollection
    {
        /// <summary>
        /// Initializes a new instance of the GameInputActions class.
        /// </summary>
        /// <param name="uv">The Ultraviolet context.</param>
        public GameInputActions(UltravioletContext uv)
            : base(uv)
        {

        }

        /// <summary>
        /// Gets or sets the input binding which exits the application.
        /// </summary>
        public InputAction ExitApplication
        {
            get;
            private set;
        }

        public InputAction Jump
	{
	    get;
	    private set;
	}

        public InputAction MoveUp
        {
            get;
            private set;
        }

        public InputAction MoveDown
        {
            get;
            private set;
        }

        public InputAction MoveLeft
        {
            get;
            private set;
        }

        public InputAction MoveRight
        {
            get;
            private set;
        }

        /// <summary>
        /// Called when the collection is creating its actions.
        /// </summary>
        protected override void OnCreatingActions()
        {
            ExitApplication = CreateAction("EXIT_APPLICATION");

            Jump = CreateAction("JUMP");
            MoveUp = CreateAction("MOVE_UP");
            MoveDown = CreateAction("MOVE_DOWN");
            MoveLeft = CreateAction("MOVE_LEFT");
            MoveRight = CreateAction("MOVE_RIGHT");

            base.OnCreatingActions();
        }

        /// <summary>
        /// Called when the collection is being reset to its default values.
        /// </summary>
        protected override void OnResetting()
        {
#if ANDROID
            ExitApplication.Primary = CreateKeyboardBinding(Key.AppControlBack);
#else
            ExitApplication.Primary = CreateKeyboardBinding(Key.Escape);
            Jump.Primary = CreateMouseBinding(MouseButton.Left);
            MoveUp.Primary = CreateKeyboardBinding(Key.W);
            MoveUp.Secondary = CreateKeyboardBinding(Key.Up);
            MoveDown.Primary = CreateKeyboardBinding(Key.S);
            MoveDown.Secondary = CreateKeyboardBinding(Key.Down);
            MoveLeft.Primary = CreateKeyboardBinding(Key.A);
            MoveLeft.Secondary = CreateKeyboardBinding(Key.Left);
            MoveRight.Primary = CreateKeyboardBinding(Key.D);
            MoveRight.Secondary = CreateKeyboardBinding(Key.Right);
#endif

            base.OnResetting();
        }
    }
}
