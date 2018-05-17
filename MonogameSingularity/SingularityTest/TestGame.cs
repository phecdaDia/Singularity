using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Singularity;
using SingularityTest.Scenes;

namespace SingularityTest
{
	using System.Reflection;

	using Singularity.Scripting;

	/// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class TestGame : SingularityGame
    {
        public TestGame() : base()
        {
	        this.IsMouseVisible = true;
	        IsFixedTimeStep = false;
			GraphicsDeviceManager.SynchronizeWithVerticalRetrace = false;
			GraphicsDeviceManager.ApplyChanges();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
		{
			base.LoadContent();

			this.SceneManager.AddSceneToStack(new ScriptLoadingScene(this, @"Scripts\TestScene.csx", Assembly.GetExecutingAssembly()));
		}

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
    }
}
