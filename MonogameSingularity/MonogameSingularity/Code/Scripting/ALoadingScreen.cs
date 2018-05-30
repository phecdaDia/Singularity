namespace Singularity.Scripting
{
    using Microsoft.Xna.Framework;

    public abstract class ALoadingScreen : GameScene
	{
	    protected ALoadingScreen(SingularityGame game) : base(game, "loadingScene")
	    {

	    }

	    public virtual void LoadingDone()
	    {
	        ScriptManager.RegisterAllScripts();
	        SceneManager.CloseScene();
		}

	    public virtual void CurrentlyLoading(string path)
	    {

	    }
	}
}
