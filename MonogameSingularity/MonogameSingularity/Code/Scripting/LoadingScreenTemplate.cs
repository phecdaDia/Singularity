namespace Singularity.Scripting
{
    using Microsoft.Xna.Framework;

    public abstract class LoadingScreenTemplate : GameScene
	{
	    protected LoadingScreenTemplate(SingularityGame game) : base(game, "loadingScene")
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
