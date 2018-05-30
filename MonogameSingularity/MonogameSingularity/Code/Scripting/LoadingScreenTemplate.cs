namespace Singularity.Scripting
{
    using Microsoft.Xna.Framework;

    public abstract class LoadingScreenTemplate : GameScene
    {
        public static bool DoneLoading = false;

	    protected LoadingScreenTemplate(SingularityGame game) : base(game, "loadingScene")
	    {

	    }

	    public virtual void LoadingDone()
	    {
	        DoneLoading = true;
	        ScriptManager.RegisterAllScripts();
            SceneManager.CloseScene();
        }

	    public virtual void CurrentlyLoading(string path)
	    {

	    }
	}
}
