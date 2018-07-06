namespace Singularity.Scripting
{
	internal struct ScriptData
	{
		public ScriptingTemplate Template;
		public ScriptScene Scene;
		public bool IsLoaded;
		public bool IsRegistered;

		public static implicit operator ScriptingTemplate(ScriptData obj)
		{
			return obj.Template;
		}
	}
}