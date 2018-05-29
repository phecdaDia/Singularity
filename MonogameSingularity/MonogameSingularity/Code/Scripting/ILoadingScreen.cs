namespace Singularity.Scripting
{
	public interface ILoadingScreen
	{
	    void LoadingDone();
	    void CurrentlyLoading(string path);
	}
}
