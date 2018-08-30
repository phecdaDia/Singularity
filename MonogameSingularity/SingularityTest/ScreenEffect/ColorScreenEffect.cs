using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Core.Utilities;

namespace SingularityTest.ScreenEffect
{
	public class ColorScreenEffect : Singularity.Core.Utilities.ScreenEffect
	{
		private readonly Color _color;
		private readonly float _duration;
		private double _timeCounter;

		public ColorScreenEffect(float durationInSeconds, Color color)
		{
			_duration = durationInSeconds;
			_color = color;
			_timeCounter = 0f;
		}

		public override ScreenEffectData GetEffectData(GameTime gameTime, Texture2D screen)
		{
			_timeCounter += gameTime.ElapsedGameTime.TotalSeconds;

			var outputData =
				new ScreenEffectData
				{
					Destination = new Rectangle(new Point(0), new Point(screen.Width, screen.Height)),
					Source = new Rectangle(new Point(0), new Point(screen.Width, screen.Height)),
					Color = _color,
					IsDone = _timeCounter >= _duration,
					Caller = this,
					Effect = null,
					Origin = Vector2.Zero,
					Rotation = null
				};

			return outputData;
		}

		public static ColorScreenEffect GetNewColorScreenEffect(float durationInSeconds, Color color)
		{
			return new ColorScreenEffect(durationInSeconds, color);
		}
	}
}