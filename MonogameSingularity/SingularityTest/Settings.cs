using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Input;
using Singularity.Utilities;

namespace SingularityTest
{
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
			return File.Exists("settings.xml");
		}

		protected override Dictionary<string, object> GetUserSettings()
		{
			var data = File.ReadAllText("settings.xml");
			return Deserialize<Dictionary<string, object>>(data, KnownTypes);
		}

		public override bool SaveUserSettings()
		{
			var saveData = Serialize(SettingsList, KnownTypes);
			File.WriteAllText("settings.xml", saveData);
			return true;
		}
	}
}