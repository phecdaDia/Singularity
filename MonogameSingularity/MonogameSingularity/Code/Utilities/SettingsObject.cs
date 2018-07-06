namespace Singularity.Utilities
{
	/// <summary>
	/// Base object for all settings
	/// </summary>
	/// <typeparam name="T">type to be stored</typeparam>
	public class SettingsObject<T>
	{
		public T Setting;

		public SettingsObject() { }

		public SettingsObject(T value)
		{
			Setting = value;
		}

		/// <summary>
		/// Implicit Conversion of this SettingsObject to contained value for ease of use on QuickAccess
		/// </summary>
		/// <param name="obj"></param>
		public static implicit operator T(SettingsObject<T> obj) 
			=> obj.GetValue();

		public void SetValue(T value) 
			=> Setting = value;

		public T GetValue() 
			=> Setting;
	}
}