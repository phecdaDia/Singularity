namespace Singularity.Scripting
{
	public abstract class LoadingScreenTemplate : GameScene
	{
		public static bool DoneLoading;

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