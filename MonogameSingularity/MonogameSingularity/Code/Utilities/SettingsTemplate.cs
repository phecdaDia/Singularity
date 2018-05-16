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
		public List<Type> KnownTypes = new List<Type>();

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
		/// Load Usersettings
		/// </summary>
		public abstract void LoadUserSettings();

		/// <summary>
		/// Save Usersettings
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
	}
}
