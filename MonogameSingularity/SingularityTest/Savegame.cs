using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Singularity.Utilities;

namespace SingularityTest
{
	public class Savegame : XmlManager<Savegame>
	{
		[IgnoreDataMember] // ignore Savegame when saving
		private static Savegame Instance;

		[DataMember] public Vector3 CameraTarget;

		[DataMember] public bool IsValidSavegame;

		[DataMember] public Vector3 Position;

		protected override void SetDefaultValues()
		{
			// set the instance
			Instance = this;

			IsValidSavegame = false;

			Position = new Vector3();


			XmlSaveEvent += (o, e) => IsValidSavegame = true;
		}

		public static Savegame GetSavegame()
		{
			return Instance;
		}
	}
}