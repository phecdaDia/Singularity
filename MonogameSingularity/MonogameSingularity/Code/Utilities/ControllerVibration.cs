using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace Singularity.Utilities
{
	public class ControllerVibration : GameObject
	{
		private TimeSpan timeRemaining;
		private bool isPatternObject;
		private IVibrationPattern pattern;
		private Func<GameTime, TimeSpan, VibrationStatus> patternFunc;

		private ControllerVibration()
		{
			this.isPatternObject = false;
			this.pattern = null;
			this.patternFunc = null;
		}

		private ControllerVibration(float timeInSeconds) : this()
		{
			this.timeRemaining = TimeSpan.FromSeconds(timeInSeconds);
		}

		public ControllerVibration(float timeInSeconds, IVibrationPattern vibrationPattern) : this(timeInSeconds)
		{
			this.pattern = vibrationPattern;
			this.isPatternObject = true;
		}

		public ControllerVibration(float timeInSeconds,
			Func<GameTime, TimeSpan, VibrationStatus> vibrationPattern) : this(timeInSeconds)
		{
			this.patternFunc = vibrationPattern;
		}

		public override void Update(GameScene scene, GameTime gameTime)
		{
			timeRemaining = timeRemaining.Subtract(gameTime.ElapsedGameTime);

			VibrationStatus status;
			if (isPatternObject)
				status = pattern.GetStatus(gameTime, timeRemaining);
			else
				status = patternFunc.Invoke(gameTime, timeRemaining);

			InputManager.SetVibration(status.LowFrequenzyStrength, status.HighFrequenzyStrength);

			if (timeRemaining.TotalMilliseconds <= 0)
			{
				InputManager.SetVibration(0,0);
				scene.RemoveObject(this);
			}
		}
	}

	public interface IVibrationPattern
	{
		VibrationStatus GetStatus(GameTime runningTime, TimeSpan remainingTime);
	}

	public struct VibrationStatus
	{
		public float LowFrequenzyStrength;
		public float HighFrequenzyStrength;
	}

	public class StaticVibration : IVibrationPattern
	{
		private VibrationStatus vibObj;

		public StaticVibration(float HFstrength, float LFstrength)
		{
			vibObj = new VibrationStatus
			{
				HighFrequenzyStrength = HFstrength,
				LowFrequenzyStrength = LFstrength
			};
		}

		public VibrationStatus GetStatus(GameTime runningTime, TimeSpan remainingTime)
		{
			return vibObj;
		}
	}
}
