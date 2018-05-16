using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Singularity.Utilities;

namespace Singularity
{
	/// <summary>
	/// Call SetUp on Game Construction to activate
	/// </summary>
	public static class SettingsManager
	{
		private static bool _isSetup = false;
		private static SettingsTemplate _settings;

		/// <summary>
		/// Needs to be called on GameStart. Best in Constructor on Custom SingularityGameClass
		/// </summary>
		/// <param name="settings">typeof the custom Settings</param>
		public static void SetUp(Type settings)
		{
			_settings = (SettingsTemplate) Activator.CreateInstance(settings);

			if(_settings.CheckForUserSettings())
				_settings.LoadUserSettings();
			else
				_settings.SetDefaultSettings();
			_settings.ApplyQuickAccess();
			_settings.SaveUserSettings();

			_isSetup = true;
		}

		/// <summary>
		/// Return Current Instance of Settings with automatic Cast
		/// </summary>
		/// <returns>correctly casted settings instance</returns>
		public static T GetSettingsInstance<T>() where T : SettingsTemplate
		{
			if (!_isSetup) throw new Exception("Settings were not set up. Use SettingsManager.SetUp(SettingsTemplate)");

			if (_settings is T s)
				return s;
			throw new Exception("Setting not of specified Type");
		}

		/// <summary>
		/// Return Current Instance of Settings
		/// </summary>
		/// <returns>settings instance</returns>
		public static SettingsTemplate GetSettingsInstance()
		{
			if (!_isSetup) throw new Exception("Settings were not set up. Use SettingsManager.SetUp(SettingsTemplate)");
			return _settings;
		}

		/// <summary>
		/// Set a specific setting
		/// </summary>
		/// <typeparam name="T">Type of setting</typeparam>
		/// <param name="name">Identifier of Setting</param>
		/// <param name="value">Value to be set</param>
		public static void SetSetting<T>(string name, T value)
		{
			if (!_isSetup) throw new Exception("Settings were not set up. Use SettingsManager.SetUp(SettingsTemplate)");

			if (_settings.SettingsList.TryGetValue(name, out var set))
				set.SetSetting(value);
			else
				throw new Exception("Specified setting doesn't exist");
		}

		/// <summary>
		/// Get a specific setting
		/// </summary>
		/// <typeparam name="T">Type of setting</typeparam>
		/// <param name="name">Identifier of Setting</param>
		/// <returns>correctly casted Setting-Object</returns>
		public static T GetSetting<T>(string name)
		{
			if (!_isSetup) throw new Exception("Settings were not set up. Use SettingsManager.SetUp(SettingsTemplate)");

			if(_settings.SettingsList.TryGetValue(name, out var set))
				return set.GetSetting<T>();
			else
				throw new Exception("Specified setting doesn't exist");

		}

		/// <summary>
		/// Save Settings
		/// </summary>
		public static void SaveSettings()
		{
			if (!_isSetup) throw new Exception("Settings were not set up. Use SettingsManager.SetUp(SettingsTemplate)");

			_settings.SaveUserSettings();
		}

		/// <summary>
		/// Reload saved (User)Settings
		/// </summary>
		public static void ReloadSettings()
		{
			if (!_isSetup) throw new Exception("Settings were not set up. Use SettingsManager.SetUp(SettingsTemplate)");

			_settings.LoadUserSettings();
		}
	}
}
