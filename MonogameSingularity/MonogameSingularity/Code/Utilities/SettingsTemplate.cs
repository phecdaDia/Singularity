namespace Singularity.Utilities
{
	using System;
	using System.Collections.Generic;

	using Microsoft.Xna.Framework.Input;

	/// <summary>
	/// Template for Settings - Inherit and apply what you need
	/// </summary>
	public abstract class SettingsTemplate
	{
		//List of all settings
		public readonly Dictionary<string, SettingsObject> SettingsList = new Dictionary<string, SettingsObject>();

		/// <summary>
		/// Set Default Settings
		/// </summary>
		public abstract void SetDefaultSettings();

		/// <summary>
		/// Apply all added Quicksettings
		/// </summary>
		public abstract void ApplyQuickAccess();

		/// <summary>
		/// Check if UserSettings exist somewhere
		/// </summary>
		/// <returns>true if exist else false</returns>
		public abstract bool CheckForUserSettings();

		/// <summary>
		/// Load Usersettings to SettingsList
		/// </summary>
		public abstract void LoadUserSettings();

		/// <summary>
		/// Save SettingsList
		/// </summary>
		/// <returns>true if everything went well</returns>
		public abstract bool SaveUserSettings();
	}

	/// <summary>
	/// Base Object for every kind of Setting
	/// </summary>
	public abstract class SettingsObject
	{
		/// <summary>
		/// Name/Identifier of Setting
		/// </summary>
		public string Name { get; protected set; }
		protected object Setting;

		protected SettingsObject(string name, object setting)
		{
			Name = name;
			Setting = setting;
		}

		/// <summary>
		/// Constructor for Saving/Loading purposes
		/// </summary>
		/// <param name="data"></param>
		protected SettingsObject(string data) { }

		/// <summary>
		/// Get casted Setting if possible - prefer non-generic GetSetting from the implemented SettingsObject
		/// </summary>
		/// <typeparam name="T">Assumed Type of Setting</typeparam>
		/// <returns>automatically casted Setting</returns>
		public T GetSetting<T>()
		{
			if (Setting is T variable)
				return variable;
			else
				throw new Exception("Setting not of specified Type");
		}

		/// <summary>
		/// Set Setting if possible - prefer non-generic SetSetting from the implemented SettingsObject
		/// </summary>
		/// <typeparam name="T">Assumed Type of Setting</typeparam>
		/// <param name="val">Value to be set</param>
		public void SetSetting<T>(T val)
		{
			if (Setting is T)
				Setting = val;
			else throw new Exception("Setting is not specified type");
		}

		/// <summary>
		/// Return serialized string containing important data to recreate
		/// </summary>
		/// <returns>serialized string</returns>
		public abstract string GetSaveData();
		/// <summary>
		/// Apply serialized string to recreate a setting
		/// </summary>
		/// <param name="str">serialized string</param>
		public abstract void SetSaveData(string str);
	}

	/// <summary>
	/// Setting for Keys
	/// </summary>
	public sealed class SettingsKey : SettingsObject
	{
		/// <summary>
		/// Normal Constructor
		/// </summary>
		/// <param name="name"></param>
		/// <param name="setting"></param>
		public SettingsKey(string name, Keys setting) : base(name, setting) { }

		/// <summary>
		/// Constructor for recreating from serialized string
		/// </summary>
		/// <param name="data"></param>
		public SettingsKey(string data) : base(data)
		{
			SetSaveData(data);
		}

		/// <summary>
		/// Use SettingsObject direct as Setting
		/// </summary>
		/// <param name="obj"></param>
		public static implicit operator Keys(SettingsKey obj)
		{
			return obj.GetSetting();
		}

		public Keys GetSetting() => GetSetting<Keys>();

		public void SetSetting(Keys value) => SetSetting<Keys>(value);

		public override string GetSaveData()
		{
			return Name + "|" + GetSetting();
		}

		public override void SetSaveData(string str)
		{
			var strParts = str.Split('|');
			Name = strParts[0];
			Setting = Enum.Parse(typeof(Keys), strParts[1]);
		}
	}

	public sealed class SettingsString : SettingsObject
	{
		public SettingsString(string name, string setting) : base(name, setting) { }

		public SettingsString(string data) : base(data)
		{
			SetSaveData(data);
		}

		public static implicit operator string(SettingsString obj)
		{
			return obj.GetSetting();
		}

		public string GetSetting() => GetSetting<string>();
		public void SetSetting(string value) => SetSetting<string>(value);

		public override string GetSaveData()
		{
			return Name + "|" + GetSetting();
		}

		public override void SetSaveData(string str)
		{
			var strParts = str.Split('|');
			Name = strParts[0];
			Setting = strParts[1];
		}
	}

	public sealed class SettingsInteger : SettingsObject
	{
		public SettingsInteger(string name, int setting) : base(name, setting) { }

		public SettingsInteger(string data) : base(data)
		{
			SetSaveData(data);
		}

		public static implicit operator int(SettingsInteger obj)
		{
			return obj.GetSetting();
		}

		public int GetSetting() => GetSetting<int>();
		public void SetSetting(int value) => SetSetting<int>(value);

		public override string GetSaveData()
		{
			return Name + "|" + GetSetting();
		}

		public override void SetSaveData(string str)
		{
			var strParts = str.Split('|');
			Name = strParts[0];
			Setting = int.Parse(strParts[1]);
		}
	}

	public sealed class SettingsFloat : SettingsObject
	{
		public SettingsFloat(string name, float setting) : base(name, setting) { }

		public SettingsFloat(string data) : base(data)
		{
			SetSaveData(data);
		}

		public static implicit operator float(SettingsFloat obj)
		{
			return obj.GetSetting();
		}

		public float GetSetting() => GetSetting<float>();
		public void SetSetting(float value) => SetSetting<float>(value);

		public override string GetSaveData()
		{
			return Name + "|" + GetSetting();
		}

		public override void SetSaveData(string str)
		{
			var strParts = str.Split('|');
			Name = strParts[0];
			Setting = float.Parse(strParts[1]);
		}
	}
}
