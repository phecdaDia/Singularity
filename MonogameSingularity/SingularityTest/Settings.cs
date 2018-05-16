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
		public static Keys UpKey { get; private set; }
		public static Keys DownKey { get; private set; }
		public static Keys LeftKey { get; private set; }
		public static Keys RightKey { get; private set; }
		public static Keys ExitKey { get; private set; }

		public override void SetDefaultSettings()
		{
			//Add all default Settings here
			//Simple combination from string-identifier & Object to store
			SettingsList.Add("upKey", Keys.W);
			SettingsList.Add("downKey", Keys.S);
			SettingsList.Add("leftKey", Keys.A);
			SettingsList.Add("rightKey", Keys.D);
			SettingsList.Add("version", "0.1");
			SettingsList.Add("exitKey", Keys.Escape);

			//Add the type of objects used above here
			KnownTypes.Add(typeof(Keys));
			KnownTypes.Add(typeof(string));
		}

		public override void SetQuickAccessProperties()
		{
			//Create QuickAccessProperties
			//Usable like Settings.RightKey
			//Use this:
			UpKey = SettingsManager.GetSetting<Keys>("upKey");
			//Or this:
			DownKey = (Keys) SettingsList["downKey"];
			LeftKey = (Keys) SettingsList["leftKey"];
			RightKey = (Keys) SettingsList["rightKey"];
			ExitKey = (Keys) SettingsList["exitKey"];
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
