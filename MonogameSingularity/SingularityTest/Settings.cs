using Microsoft.Xna.Framework.Input;

using Singularity;
using Singularity.Utilities;

namespace SingularityTest
{
	using System.Collections.Generic;

	/*
	 * This is an example for a Settings Class
	 */

	public class Settings : SettingsTemplate
	{
		//"Quick Access" Properties
		public static SettingsObject<Keys> UpKey { get; private set; }
		public static SettingsObject<Keys> DownKey { get; private set; }
		public static SettingsObject<Keys> LeftKey { get; private set; }
		public static SettingsObject<Keys> RightKey { get; private set; }
		public static SettingsObject<Keys> ExitKey { get; private set; }

		public override void SetDefaultSettings()
		{
			//Add all default Settings here
			//Simple combination from string-identifier & Object to store
			AddSetting("upKey", Keys.W);
			AddSetting("downKey", Keys.S);
			AddSetting("leftKey", Keys.A);
			AddSetting("rightKey", Keys.D);
			AddSetting("version", "0.1");
			AddSetting("exitKey", Keys.Escape);
		}

		public override void SetQuickAccessProperties()
		{
			//Create QuickAccessProperties - Are set as SettingsObject<T> so it doesn't need to be reapplied on Changing Settings
			//Usable like Settings.RightKey
			//Use this:
			UpKey = GetSetting<Keys>("upKey");
			DownKey = GetSetting<Keys>("downKey");
			LeftKey = GetSetting<Keys>("leftKey");
			RightKey = GetSetting<Keys>("rightKey");
			ExitKey = GetSetting<Keys>("exitKey");
		}

		public override bool CheckForUserSettings()
		{
			return Properties.Settings.Default.ExistUserSettings;
		}

		public override void LoadUserSettings()
		{
			//Load String from Application Settings & Deserialize it
			var data = Properties.Settings.Default.UserSettings;
			SettingsList = Deserialize<Dictionary<string, object>>(data, KnownTypes);
		}

		public override bool SaveUserSettings()
		{
			//Serialize SettingsList & save it to Application User settings
			var saveData = Serialize(SettingsList, KnownTypes);
			Properties.Settings.Default.UserSettings = saveData;
			Properties.Settings.Default.ExistUserSettings = true;
			Properties.Settings.Default.Save();
			return true;
		}

	}
}
