using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Core.Utilities;

namespace SingularityTest.ScreenEffect
{
	public class ShakeScreenEffect : Singularity.Core.Utilities.ScreenEffect
	{
		private readonly int _amplitude;
		private readonly float _duration;
		private double _timeCounter;

		public ShakeScreenEffect(float durationInSeconds, int amplitude)
		{
			_duration = durationInSeconds;
			_amplitude = amplitude;
			_timeCounter = 0;
		}

		public override ScreenEffectData GetEffectData(GameTime gameTime, Texture2D screen)
		{
			_timeCounter += gameTime.ElapsedGameTime.TotalSeconds;

			var outputData =
				new ScreenEffectData
				{
					Destination =
						new Rectangle(
							new Point((int) (_amplitude * Math.Sin(_timeCounter * 100)), (int) (_amplitude * Math.Sin(_timeCounter * 100))),
							new Point(screen.Width, screen.Height)),
					Source = new Rectangle(new Point(0, 0), new Point(screen.Width, screen.Height)),
					Color = null,
					IsDone = _timeCounter >= _duration,
					Caller = this,
					Effect = null,
					Origin = Vector2.Zero,
					Rotation = null
				};

			return outputData;
		}

		public static ShakeScreenEffect GetNewShakeScreenEffect(float durationInSeconds, int amplitude)
		{
			return new ShakeScreenEffect(durationInSeconds, amplitude);
		}
	}
}