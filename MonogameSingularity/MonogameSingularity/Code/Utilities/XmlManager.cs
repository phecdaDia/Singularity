using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.Utilities
{
	[DataContract(IsReference = true)]
	public abstract class XmlManager<T>
	{
		public XmlManager()
		{
			SetDefaultValues();
		}
		protected abstract void SetDefaultValues();

		public void SaveToXml(string filePath) => SaveToXml(filePath, this);

		public static void SaveToXml(string filePath, XmlManager<T> sourceObject)
		{
			sourceObject.OnXmlSave();

			try
			{
				// create the directory
				new FileInfo(filePath).Directory?.Create();

				using (var writer = File.Create(filePath))
				{
					DataContractSerializer xmlSerializer = new DataContractSerializer(sourceObject.GetType());
					xmlSerializer.WriteObject(writer, sourceObject);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Unable to save file\t{0}", filePath);
				Console.WriteLine(ex.StackTrace);
			}
		}

		protected event EventHandler<EventArgs> XmlSaveEvent;
		public void OnXmlSave() => OnXmlSave(EventArgs.Empty);
		public void OnXmlSave(EventArgs e) =>
			XmlSaveEvent?.Invoke(this, e);

		public static T LoadFromXml(string filePath)
		{

			try
			{
				using (var stream = File.OpenRead(filePath))
				{
					DataContractSerializer xmlSerializer = new DataContractSerializer(typeof(T));
					return (T)xmlSerializer.ReadObject(stream); ;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Unable to load file\t{0}", filePath);
				Console.WriteLine(ex.Message);
				return Activator.CreateInstance<T>();
			}
		}
	}
}
