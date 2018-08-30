using System;
using System.IO;
using System.Runtime.Serialization;

namespace Singularity.Core.Utilities
{
	[DataContract(IsReference = true)]
	public abstract class XmlManager<T> where T : XmlManager<T>
	{
		public XmlManager()
		{
			this.SetDefaultValues();
		}

		protected abstract void SetDefaultValues();

		public void SaveToXml(string filePath)
		{
			SaveToXml(filePath, this);
		}

		public static void SaveToXml(string filePath, XmlManager<T> sourceObject)
		{
			sourceObject.OnXmlSave();

			try
			{
				// create the directory
				new FileInfo(filePath).Directory?.Create();

				using (var writer = File.Create(filePath))
				{
					var xmlSerializer = new DataContractSerializer(sourceObject.GetType());
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

		public void OnXmlSave()
		{
			this.OnXmlSave(EventArgs.Empty);
		}

		public void OnXmlSave(EventArgs e)
		{
			this.XmlSaveEvent?.Invoke(this, e);
		}

		public static T LoadFromXml(string filePath)
		{
			try
			{
				using (var stream = File.OpenRead(filePath))
				{
					var xmlSerializer = new DataContractSerializer(typeof(T));
					var o = (T) xmlSerializer.ReadObject(stream);
					o.SetDefaultValues();
					return o;

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