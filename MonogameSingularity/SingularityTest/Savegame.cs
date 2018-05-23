using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Singularity.Utilities;

namespace SingularityTest
{
	public class Savegame : XmlManager<Savegame>
	{
		[IgnoreDataMember] // ignore Savegame when saving
		private static Savegame Instance;

		[DataMember]
		public Boolean IsValidSavegame;

		[DataMember]
		public Vector3 Position;

		[DataMember]
		public Vector3 CameraTarget;

		protected override void SetDefaultValues()
		{
			// set the instance
			Instance = this;
			
			this.IsValidSavegame = false;

			this.Position = new Vector3();


			this.XmlSaveEvent += (o, e) => this.IsValidSavegame = true;
		}

		public static Savegame GetSavegame()
		{
			return Instance;
		}

	}
}
