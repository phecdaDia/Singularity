using System;

using Singularity.Utilities;

namespace Singularity
{
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	public static class SettingsManager
	{
		private static SettingsTemplate _settings;
		private static bool _isSetUp = false;

		/// <summary>
		/// SetUp Settings in this Project - needs Type of Setting to Set Up
		/// </summary>
		/// <typeparam name="T">Setting to set up</typeparam>
		public static void SetUp<T>() where T : SettingsTemplate
		{
			//Create a Setting instance, apply default settings, look for user settings. if they exist, load them.
			//Save everything to be safe & Set up QuickAccess Properties
			_settings = Activator.CreateInstance<T>();

			_settings.SetDefaultSettings();

			if (_settings.CheckForUserSettings())
				try
				{
					_settings.ApplyUserSettings();
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
					Console.WriteLine("Applying default Settings and overwriting corrupted");
					_settings.SetDefaultSettings();
				}

			_settings.SaveUserSettings();

			_isSetUp = true;

			_settings.SetQuickAccessProperties();
		}

		/// <summary>
		/// Check if Settings have been set up
		/// </summary>
		private static void CheckSetUp()
		{
			if (!_isSetUp) throw new Exception("Settings not set up");
		}

		/// <summary>
		/// Get a specific Setting
		/// </summary>
		/// <typeparam name="T">Assumed Type of setting</typeparam>
		/// <param name="name">Identifier/Name of Setting</param>
		/// <returns>Setting if it exists and is same type as specified</returns>
		public static T GetSetting<T>(string name)
		{
			CheckSetUp();

			return _settings.GetSetting<T>(name);
		}

		/// <summary>
		/// Set a specific Setting
		/// </summary>
		/// <typeparam name="T">Assumed Type of setting</typeparam>
		/// <param name="name">Identifier/Name of Setting</param>
		/// <param name="value"></param>
		public static void SetSetting<T>(string name, T value)
		{
			CheckSetUp();

			_settings.SetSetting(name, value);
		}

		/// <summary>
		/// Save Settings as Specified in Settings-Class
		/// </summary>
		public static void SaveSetting()
		{
			CheckSetUp();

			_settings.SaveUserSettings();
		}

		/// <summary>
		/// Reload Settings
		/// </summary>
		public static void LoadSettings()
		{
			CheckSetUp();

			RestoreDefault();
			_settings.ApplyUserSettings();
			_settings.SetQuickAccessProperties();
		}

		public static void RestoreDefault()
		{
			CheckSetUp();

			_settings.SettingsList.Clear();
			_settings.SetDefaultSettings();
		}
	}
}
