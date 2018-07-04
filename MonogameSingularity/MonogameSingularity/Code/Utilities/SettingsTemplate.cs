using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace Singularity.Utilities
{
	/// <summary>
	/// Template for Settings
	/// </summary>
	public abstract class SettingsTemplate
	{
		public Dictionary<string, object> SettingsList = new Dictionary<string, object>();
		public List<Type> KnownTypes = new List<Type> {typeof(Dictionary<string, object>)};

		/// <summary>
		/// Set all default Settings und put Types into KnownType-List
		/// </summary>
		public abstract void SetDefaultSettings();

		/// <summary>
		/// See if there are usersettings
		/// </summary>
		/// <returns>true if they exist</returns>
		public abstract bool CheckForUserSettings();

		/// <summary>
		/// Set all created static Properties to the values from Settings
		/// </summary>
		public abstract void SetQuickAccessProperties();

		/// <summary>
		/// Get UserSettings/SettingsList and return them as Dictionary
		/// <returns></returns>
		/// </summary>
		protected abstract Dictionary<string, object> GetUserSettings();

		/// <summary>
		/// Load Usersettings and try to apply them
		/// </summary>
		public virtual void ApplyUserSettings()
		{
			//Try get Stuff from User
			var user = GetUserSettings();

			//Apply it to SettingsList. if it was delete -> ignore
			foreach (var key in user.Keys)
			{
				if(!SettingsList.ContainsKey(key)) continue;
				SettingsList[key] = user[key];
			}
		}

		/// <summary>
		/// Save SettingsList
		/// </summary>
		/// <returns>true if eveything went ok</returns>
		public abstract bool SaveUserSettings();

		/// <summary>
		/// Serialize instance of T
		/// </summary>
		/// <typeparam name="T">Type of Object</typeparam>
		/// <param name="obj">Object to serialize</param>
		/// <param name="knownTypes">Known Types of Object</param>
		/// <returns>serialized string</returns>
		protected static string Serialize<T>(T obj, IEnumerable<Type> knownTypes)
		{
			var serializer = new DataContractSerializer(obj.GetType(), knownTypes);
			string output;
			using (var stringWriter = new StringWriter())
			{
				using (var stm = new XmlTextWriter(stringWriter))
				{
					stm.Formatting = Formatting.Indented;
					serializer.WriteObject(stm, obj);
					output = stringWriter.ToString();
				}
			}

			return output;
		}

		/// <summary>
		/// Deserialize string
		/// </summary>
		/// <typeparam name="T">assumed type after deserialization</typeparam>
		/// <param name="serialized">serialized string</param>
		/// <param name="knownTypes">known types of object</param>
		/// <returns>deserialized object</returns>
		protected static T Deserialize<T>(string serialized, IEnumerable<Type> knownTypes)
		{
			var serializer = new DataContractSerializer(typeof(Dictionary<string, object>), knownTypes);
			using (var stringReader = new StringReader(serialized))
			using (var stm = new XmlTextReader(stringReader))
			{
				return (T)serializer.ReadObject(stm);
			}
		}

		/// <summary>
		/// Create Setting of type T with specified name/identifier and given Value
		/// </summary>
		/// <typeparam name="T">type of setting</typeparam>
		/// <param name="name">identifier/name of setting</param>
		/// <param name="setting">value of Setting</param>
		protected void AddSetting<T>(string name, T setting)
		{
			SettingsList.Add(name, new SettingsObject<T>(setting));
			KnownTypes.Add(typeof(T));
			KnownTypes.Add(typeof(SettingsObject<T>));
		}

		/// <summary>
		/// Get named setting & Check for correct type
		/// </summary>
		/// <typeparam name="T">assumed type</typeparam>
		/// <param name="name">name/identifier</param>
		/// <returns></returns>
		public SettingsObject<T> GetSetting<T>(string name)
		{
			if (SettingsList.ContainsKey(name))
			{
				if (SettingsList[name] is SettingsObject<T> data)
				{
					return data;
				}
				else throw new Exception("Setting not of specified type");
			}
			else throw new Exception("Named Setting not found");
		}

		/// <summary>
		/// Change Value of an existing setting
		/// </summary>
		/// <typeparam name="T">assumed type</typeparam>
		/// <param name="name">name/identifier</param>
		/// <param name="setting">new setting value</param>
		public void SetSetting<T>(string name, T setting)
		{
			if (SettingsList.ContainsKey(name))
			{
				if (SettingsList[name] is SettingsObject<T> data)
				{
					data.SetValue(setting);
				}
				else throw new Exception("Setting not of specified type");
			}
			else throw new Exception("Named Setting not found");
		}
	}

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
