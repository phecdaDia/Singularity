using ExampleMarble.GameScenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Singularity.Code;

namespace ExampleMarble
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MarbleGame : SingularityGame
    {
	    public MarbleGame()
	    {
			
	    }

	    protected override void Initialize()
	    {
		    base.Initialize();

			// create scenes and load them

			// main menu scene

			// game scene
		    SceneManager.RegisterScene(new MarbleScene(this));

			SceneManager.AddSceneToStack("marble");

	    }

	    protected override void LoadContent()
	    {
		    base.LoadContent();

			
	    }
    }
}
