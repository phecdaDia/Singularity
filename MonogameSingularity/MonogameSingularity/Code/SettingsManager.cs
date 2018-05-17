using System;

using Singularity.Utilities;

namespace Singularity
{

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
				_settings.LoadUserSettings();

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
			//Check if SetUp
			//Check if Exist & is assumed type
			//Return
			CheckSetUp();

			if (_settings.SettingsList.TryGetValue(name, out var data))
			{
				if (data is T setting)
					return setting;
				throw new Exception("Setting not of specified type");
			}

			throw new Exception("Named Setting not found");
		}

		/// <summary>
		/// Set a specific Setting
		/// </summary>
		/// <typeparam name="T">Assumed Type of setting</typeparam>
		/// <param name="name">Identifier/Name of Setting</param>
		/// <param name="value"></param>
		public static void SetSetting<T>(string name, T value)
		{
			//Check Setup
			//Check if exists & is of Type
			//Then set
			CheckSetUp();

			if(_settings.SettingsList.ContainsKey(name))
			{
				if (_settings.SettingsList[name] is T)
				{
					_settings.SettingsList[name] = value;
				}
				else throw new Exception("Setting not of specified type");
			}
			else throw new Exception("Named Setting not found");

			_settings.SetQuickAccessProperties();
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

			_settings.LoadUserSettings();
			_settings.SetQuickAccessProperties();
		}
	}
}
