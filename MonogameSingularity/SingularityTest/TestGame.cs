using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Singularity;
using SingularityTest.Scenes;

namespace SingularityTest
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class TestGame : SingularityGame
    {
	    private Savegame Savegame;

        public TestGame() : base()
        {
	        this.IsMouseVisible = true;
			SettingsManager.SetUp<Settings>();
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

			this.Savegame = Savegame.LoadFromXml("savefile1.xml");

			this.Exiting += this.GameExiting;

	        this.SceneManager.AddSceneToStack(new TestScene(this));
		}

		private void GameExiting(Object sender, EventArgs e)
		{
			Savegame.SaveToXml("savefile1.xml", Savegame);
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			base.LoadContent();
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
