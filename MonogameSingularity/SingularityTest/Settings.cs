using System.Collections.Generic;

namespace SingularityTest
{
	using System;
	using System.Text;

	using Microsoft.Xna.Framework.Input;

	using Singularity.Utilities;

	internal class Settings : SettingsTemplate
	{
		//Quick access Settings
		public static SettingsKey UpKey { get; private set; }
		public static SettingsKey DownKey { get; private set; }
		public static SettingsKey LeftKey { get; private set; }
		public static SettingsKey RightKey { get; private set; }
		public static SettingsKey ExitKey { get; private set; }

		public override void SetDefaultSettings()
		{
			var UpKey = new SettingsKey("upKey", Keys.W);
			SettingsList.Add(UpKey.Name, UpKey);

			var DownKey = new SettingsKey("downKey", Keys.S);
			SettingsList.Add(DownKey.Name, DownKey);

			var LeftKey = new SettingsKey("leftKey", Keys.A);
			SettingsList.Add(LeftKey.Name, LeftKey);

			var RightKey = new SettingsKey("rightKey", Keys.D);
			SettingsList.Add(RightKey.Name, RightKey);

			SettingsList.Add("version", new SettingsString("version", "0.1"));

			var ExitKey = new SettingsKey("exitKey", Keys.Escape);
			SettingsList.Add(ExitKey.Name, ExitKey);
		}

		public override void ApplyQuickAccess()
		{
			UpKey = SettingsList["upKey"] as SettingsKey;
			DownKey = SettingsList["downKey"] as SettingsKey;
			LeftKey = SettingsList["leftKey"] as SettingsKey;
			RightKey = SettingsList["rightKey"] as SettingsKey;
			ExitKey = SettingsList["exitKey"] as SettingsKey;
		}

		public override bool CheckForUserSettings()
		{
			return Properties.Settings.Default.ExistUserSettings;
		}

		/// <summary>
		/// EXAMPLE ONLY - but it works (quite good actually)
		/// </summary>
		public override void LoadUserSettings()
		{
			var str = Properties.Settings.Default.UserSettings;

			var values = str.Split(';');
			foreach (var s in values)
			{
				if(s == "") continue;

				var data = s.Split(':');
				var type = IdentifyForLoading(data[0]);
				var obj = (SettingsObject) Activator.CreateInstance(type, data[1]);
				SettingsList.Add(obj.Name, obj);
			}
		}

		/// <summary>
		/// EXAMPLE ONLY - but it works (quite good actually)
		/// </summary>
		/// <returns></returns>
		public override bool SaveUserSettings()
		{
			var builder = new StringBuilder();
			foreach (var o in SettingsList.Values)
			{
				builder.Append(IdentifyForSaving(o) + ':' + o.GetSaveData() + ';');
			}

			var dataToSave = builder.ToString();

			Properties.Settings.Default.UserSettings = dataToSave;
			Properties.Settings.Default.ExistUserSettings = true;
			Properties.Settings.Default.Save();

			return true;
		}

		private string IdentifyForSaving(SettingsObject obj)
		{
			switch (obj)
			{
				case SettingsString _:
					return "0";
				case SettingsKey _:
					return "1";
			}

			return "-1";
		}

		private Type IdentifyForLoading(string i)
		{
			switch (i)
			{
				case "0":
					return typeof(SettingsString);
				case "1":
					return typeof(SettingsKey);
			}

			return typeof(SettingsObject);
		}
	}
}
