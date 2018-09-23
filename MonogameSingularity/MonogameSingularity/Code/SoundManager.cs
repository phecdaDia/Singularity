using System;
using System.Collections.Generic;
using System.IO;
using IrrKlang;

namespace Singularity
{
	using System.Runtime.CompilerServices;
	using Microsoft.Xna.Framework;

	public class SoundManager
	{
		#region Singleton

		private static SoundManager _instance;
		protected static  SoundManager Instance => _instance ?? (_instance = new SoundManager());

		#endregion

		private readonly ISoundEngine _engine;
		private readonly ISoundEngine _secondaryEngine;
		private bool _activePrimaryEngine;
		private readonly Dictionary<string, ISoundSource> _sounds;
		private ISound _currentMusic;
		private ISound _currentSecondaryMusic;
		private float _effectVolume;
		private float _musicVolume;

		private SoundManager()
		{
			_engine = new ISoundEngine();
			_secondaryEngine = new ISoundEngine();
			_sounds = new Dictionary<string, ISoundSource>();
			_currentMusic = null;
			_currentSecondaryMusic = null;
			_effectVolume = 1f;
			_musicVolume = 1f;
		}

		#region Register Sounds

		/// <summary>
		/// Register Sound from File
		/// </summary>
		/// <param name="name">Name under which it will be registred</param>
		/// <param name="path">Path to File</param>
		/// <param name="defaultVolume">Default Volume of Sound</param>
		public static void RegisterSoundFromFile(string name, string path, float defaultVolume = 1f) => Instance._registerSoundFromFile(name, path, defaultVolume);
		private void _registerSoundFromFile(string name, string path, float defaultVolume)
		{
			if (_sounds.ContainsKey(name))
			{
				Console.WriteLine("Sound already registred");
				return;
			}

			_sounds.Add(name, _engine.AddSoundSourceFromFile(path));
			_sounds[name].DefaultVolume = defaultVolume;
		}

		/// <summary>
		/// Register Sound from Memory
		/// </summary>
		/// <param name="name">Name under which it will be registred</param>
		/// <param name="data">Data of Sound</param>
		/// <param name="defaultVolume">Default Volume of Sound</param>
		public static void RegisterSoundFromMemory(string name, byte[] data, float defaultVolume = 1f) => Instance._registerSoundFromMemory(name, data, defaultVolume);
		private void _registerSoundFromMemory(string name, byte[] data, float defaultVolume)
		{
			if (_sounds.ContainsKey(name))
			{
				Console.WriteLine("Sound already registred");
				return;
			}

			_sounds.Add(name, _engine.AddSoundSourceFromMemory(data, name));
			_sounds[name].DefaultVolume = defaultVolume;
		}

		/// <summary>
		/// Register Sound from IOStream
		/// </summary>
		/// <param name="name">Name under which it will be registred</param>
		/// <param name="stream">Stream containing Sound</param>
		/// <param name="defaultVolume">Default Volume of Sound</param>
		public static void RegisterSoundFromIOStream(string name, Stream stream, float defaultVolume = 1f) => Instance._registerSoundFromIOStream(stream, name, defaultVolume);
		private void _registerSoundFromIOStream(Stream stream, string name, float defaultVolume)
		{
			if (_sounds.ContainsKey(name))
			{
				Console.WriteLine("Sound already registred");
				return;
			}

			_sounds.Add(name, _engine.AddSoundSourceFromIOStream(stream, name));
			_sounds[name].DefaultVolume = defaultVolume;
		}

		/// <summary>
		/// Remove registered Sound
		/// </summary>
		/// <param name="name">Name of Sound</param>
		public static void UnregisterSound(string name) => Instance._unregisterSound(name);
		private void _unregisterSound(string name)
		{
			if (_sounds.ContainsKey(name))
				_sounds.Remove(name);
		}

		#endregion

		#region Volume

		/// <summary>
		/// Global Volume
		/// </summary>
		public static float GlobalVolume
		{
			get => Instance._engine.SoundVolume;
			set
			{
				if (value > 1f || value < 0f)
				{
					Console.Error.WriteLine("Error setting Volume. Illegal Value: " + value);
					return;
				}

				Instance._engine.SoundVolume = value;
			}
		}

		/// <summary>
		/// Volume of Music (dependent on Global Volume)
		/// </summary>
		public static float MusicVolume
		{
			get => Instance._musicVolume;
			set
			{
				if (value > 1f || value < 0f)
				{
					Console.Error.WriteLine("Error setting Volume. Illegal Value: " + value);
					return;
				}

				Instance._musicVolume =value;
				if(Instance._activePrimaryEngine && Instance._currentMusic != null)
					Instance._currentMusic.Volume = value;
				else if (!Instance._activePrimaryEngine && Instance._currentSecondaryMusic != null)
					Instance._currentSecondaryMusic.Volume = value;
			}
		}

		/// <summary>
		/// Volume of Effect (dependent on Global Volume)
		/// </summary>
		public static float EffectVolume
		{
			get => Instance._effectVolume;
			set
			{
				if (value > 1f || value < 0f)
				{
					Console.Error.WriteLine("Error setting Volume. Illegal Value" + value);
					return;
				}

				Instance._effectVolume = value;
			}
		}

		#endregion

		#region Helper

		private void CheckSound(string name)
		{
			if (!_sounds.ContainsKey(name))
				throw new ArgumentException(name + " not registred as Sound");
		}

		/// <summary>
		/// Notifier for Stop of Music/Effect
		/// </summary>
		private class SoundStop : ISoundStopEventReceiver
		{
			private readonly Action<StopEventCause> _onStop;

			public SoundStop(Action<StopEventCause> onStop = null)
			{
				_onStop = onStop;
			}

			public void OnSoundStopped(ISound sound, StopEventCause reason, object userData)
			{
				if (sound == Instance._currentMusic)
					MusicStopped();

				_onStop?.Invoke(reason);
			}

			private void MusicStopped()
			{
				Instance._currentMusic = null;
			}

		}

		#endregion

		#region Music

		/// <summary>
		/// Play Music
		/// </summary>
		/// <param name="name">Name of registred Sound</param>
		/// <param name="loop">Should it loop?</param>
		/// <param name="startPaused">Should it start with paused status</param>
		/// <param name="speed">Playbackspeed</param>
		/// <param name="pan">Pan</param>
		/// <param name="onStop">Action which is called on Stop</param>
		/// <param name="enableReset">Reset Music if already running</param>
		public static void PlayMusic(string name, bool loop, bool startPaused = false, float speed = 1f, float pan = 0f, Action<StopEventCause> onStop = null, bool enableReset = false) => Instance._PlayMusic(name, loop, startPaused, speed, pan, onStop, enableReset);
		private void _PlayMusic(string name, bool loop, bool startPaused, float speed, float pan, Action<StopEventCause> onStop, bool enableReset)
		{
			CheckSound(name);

			if (_engine.IsCurrentlyPlaying(_sounds[name].Name) && !enableReset)
				return;

			_currentMusic?.Stop();

			_currentMusic = _engine.Play2D(_sounds[name], loop, true, true);
			_currentMusic.PlaybackSpeed = speed;
			_currentMusic.Pan           = pan;
			_currentMusic.Volume        = _activePrimaryEngine? _musicVolume : 0f;
			if(onStop != null)
				_currentMusic.setSoundStopEventReceiver(new SoundStop(onStop));
			_currentMusic.Paused = startPaused;
		}

		public static void PlaySecondaryMusic(string name, bool loop, bool startPaused = false, float speed = 1f, float pan = 0f, Action<StopEventCause> onStop = null, bool enableReset = false) 
			=> Instance._PlaySecondaryMusic(name, loop, startPaused, speed, pan, onStop, enableReset);
		private void _PlaySecondaryMusic(string name, bool loop, bool startPaused, float speed, float pan, Action<StopEventCause> onStop, bool enableReset)
		{
			CheckSound(name);

			if (_secondaryEngine.IsCurrentlyPlaying(_sounds[name].Name) && !enableReset)
				return;

			_currentSecondaryMusic?.Stop();

			_currentSecondaryMusic = _engine.Play2D(_sounds[name], loop, true, true);
			_currentSecondaryMusic.PlaybackSpeed = speed;
			_currentSecondaryMusic.Pan           = pan;
			_currentSecondaryMusic.Volume        = !_activePrimaryEngine? _musicVolume : 0f;
			if (onStop != null)
				_currentSecondaryMusic.setSoundStopEventReceiver(new SoundStop(onStop));
			_currentSecondaryMusic.Paused = startPaused;
		}

		private class LoopHelper : ISoundStopEventReceiver
		{
			public LoopHelper(string nameLooppart, float speed, float pan, bool primary)
			{
				nameLoop = nameLooppart;
				this.speed = speed;
				this.pan = pan;
				this.primary = primary;
			}

			private string nameLoop;
			private float speed;
			private float pan;
			private bool primary;

			public void OnSoundStopped(ISound sound, StopEventCause reason, object userData)
			{
				if (reason == StopEventCause.SoundFinishedPlaying)
				{
					if (primary)
						PlayMusic(nameLoop, true, false, speed, pan, null, true);
					else
						PlaySecondaryMusic(nameLoop, true, false, speed, pan, null, true);
				}
			}
		}

		/// <summary>
		/// Play looped Music
		/// </summary>
		/// <param name="nameBeginning">BeginningPart - will be played once on begin</param>
		/// <param name="nameLooppart">LoopPart - will be looped after beginning</param>
		/// <param name="startPaused">start paused - need to manually call play</param>
		/// <param name="speed">playback speed</param>
		/// <param name="pan">Pan</param>
		/// <param name="enableReset">If true - reset if already playing</param>
		public static void PlayLoopMusic(string nameBeginning, string nameLoopPart, bool startPaused = false,
		                                 float  speed = 1f,    float  pan = 0f,     bool enableReset = false)
			=> Instance._PlayLoopMusic(nameBeginning, nameLoopPart, startPaused, speed, pan, enableReset);
		private void _PlayLoopMusic(string nameBeginning, string nameLoopPart, bool startPaused, float speed, float pan,
		                            bool   enableReset)
		{
			CheckSound(nameBeginning);
			CheckSound(nameLoopPart);

			if ((_engine.IsCurrentlyPlaying(_sounds[nameBeginning].Name) ||
			     _engine.IsCurrentlyPlaying(_sounds[nameLoopPart].Name)) && !enableReset)
				return;

			_currentMusic?.Stop();

			_currentMusic = _engine.Play2D(_sounds[nameBeginning], false, true, true);
			_currentMusic.PlaybackSpeed = speed;
			_currentMusic.Pan = pan;
			_currentMusic.Volume = _musicVolume;
			_currentMusic.setSoundStopEventReceiver(new LoopHelper(nameLoopPart, speed, pan, true));
			_currentMusic.Paused = startPaused;
		}

		public static void PlayDualLoopMusic(string nameBeginningPrimary,   string nameLoopPartPrimary,
		                                     string nameBeginningSecondary, string nameLoopPartSecondary,
		                                     float  speed = 1f,             float  pan = 0f, bool enableReset = false)
			=> Instance._PlayDualLoopMusic(nameBeginningPrimary, nameLoopPartPrimary, nameBeginningSecondary,
			                               nameLoopPartSecondary, speed, pan, enableReset);

		private void _PlayDualLoopMusic(string nameBeginningPrimary, string nameLoopPartPrimary, string nameBeginningSecondary, string nameLoopPartSecondary, float speed, float pan, bool enableReset)
		{
			CheckSound(nameBeginningPrimary);
			CheckSound(nameLoopPartPrimary);
			CheckSound(nameBeginningSecondary);
			CheckSound(nameLoopPartSecondary);

			if((_engine.IsCurrentlyPlaying(_sounds[nameBeginningPrimary].Name) ||
			    _engine.IsCurrentlyPlaying(_sounds[nameLoopPartPrimary].Name)) && !enableReset)
				return;
			if((_secondaryEngine.IsCurrentlyPlaying(_sounds[nameBeginningSecondary].Name) ||
			    _secondaryEngine.IsCurrentlyPlaying(_sounds[nameLoopPartSecondary].Name)) && !enableReset)
				return;

			_currentMusic?.Stop();
			_currentSecondaryMusic?.Stop();

			_currentMusic = _engine.Play2D(_sounds[nameBeginningPrimary], false, true, true);
			_currentMusic.PlaybackSpeed = speed;
			_currentMusic.Pan = pan;
			_currentMusic.Volume = _musicVolume;
			_currentMusic.setSoundStopEventReceiver(new LoopHelper(nameLoopPartPrimary, speed, pan, true));

			_currentSecondaryMusic = _secondaryEngine.Play2D(_sounds[nameBeginningSecondary], false, true, true);
			_currentSecondaryMusic.PlaybackSpeed = speed;
			_currentSecondaryMusic.Pan = pan;
			_currentSecondaryMusic.Volume = 0;
			_currentSecondaryMusic.setSoundStopEventReceiver(new LoopHelper(nameLoopPartSecondary, speed, pan, false));

			_activePrimaryEngine = true;

			_currentMusic.Paused = false;
			_currentSecondaryMusic.Paused = false;
		}

		public static void TogglePrimarySecondary() => Instance._togglePrimarySecondary();

		private void _togglePrimarySecondary()
		{
			if(_currentSecondaryMusic == null || _currentMusic == null)
				return;

			if (_activePrimaryEngine)
			{
				_currentSecondaryMusic.Volume = _currentMusic.Volume;
				_currentMusic.Volume          = 0;
				_activePrimaryEngine = false;
			}
			else
			{
				_currentMusic.Volume = _currentSecondaryMusic.Volume;
				_currentSecondaryMusic.Volume = 0;
				_activePrimaryEngine = true;
			}

		}

		/// <summary>
		/// Pause Music
		/// </summary>
		public static void PauseMusic() => Instance._PauseMusic();
		private void _PauseMusic()
		{
			if(_currentMusic != null)
				_currentMusic.Paused = true;
		}

		/// <summary>
		/// Continue Paused Music
		/// </summary>
		public static void ContinueMusic() => Instance._ContinueMusic();
		private void _ContinueMusic()
		{
			if (_currentMusic != null)
				_currentMusic.Paused = false;
		}

		/// <summary>
		/// Toggle Music between paused and playing
		/// </summary>
		public static void ToggleMusic() => Instance._ToggleMusic();
		private void _ToggleMusic()
		{
			if(_currentMusic == null)
				return;

			if(_currentMusic.Paused)
				_ContinueMusic();
			else
				_PauseMusic();
		}

		/// <summary>
		/// Stop Music
		/// </summary>
		public static void StopMusic() => Instance._StopMusic();
		private void _StopMusic()
		{
			_currentMusic?.Stop();
		}

		/// <summary>
		/// Change Speed of playing Music
		/// </summary>
		/// <param name="speed"></param>
		public static void SetSpeed(float speed) => Instance._SetSpeed(speed);
		private void _SetSpeed(float speed)
		{
			if(_currentMusic == null)
				return;
			_currentMusic.PlaybackSpeed = speed;
		}

		/// <summary>
		/// Change pan of playing Music
		/// </summary>
		/// <param name="pan"></param>
		public static void SetPan(float pan) => Instance._SetPan(pan);
		private void _SetPan(float pan)
		{
			if (_currentMusic == null)
				return;
			_currentMusic.Pan = pan;
		}

		#endregion

		#region Effect

		/// <summary>
		/// Play Soundeffect
		/// </summary>
		/// <param name="name">Name of registered Effect</param>
		/// <param name="speed">Playbackspeed</param>
		/// <param name="pan">Pan</param>
		/// <param name="onStop">Action which is called on Stop</param>
		public static void PlayEffect(string name, float speed = 1f, float pan = 0f, Action<StopEventCause> onStop = null) => Instance._playEffect(name, speed, pan, onStop);
		private void _playEffect(string name, float speed, float pan, Action<StopEventCause> onStop)
		{
			CheckSound(name);

			var sound = _engine.Play2D(_sounds[name], false, true, true);
			sound.PlaybackSpeed = speed;
			sound.Pan           = pan;
			sound.Volume        = _effectVolume;
			sound.setSoundStopEventReceiver(new SoundStop(onStop));
			sound.Paused = false;
		}

		/// <summary>
		/// Play Soundeffect in 3D Space
		/// </summary>
		/// <param name="name">Name of registered Effect</param>
		/// <param name="position">Position in relation to hearing Thing</param>
		/// <param name="speed">Playbackspeed</param>
		/// <param name="pan">Pan</param>
		/// <param name="onStop">Action which is called on Stop</param>
		public static void PlayEffect3D(string name, Vector3 position, float speed = 1f, float pan = 0f, Action<StopEventCause> onStop = null) => Instance._playEffect3D(name, position, speed, pan, onStop);
		private void _playEffect3D(string name, Vector3 position, float speed, float pan, Action<StopEventCause> onStop)
		{
			CheckSound(name);
			var sound = _engine.Play3D(_sounds[name], position.X, position.Y, position.Z, false, true, true);
			sound.PlaybackSpeed = speed;
			sound.Pan           = pan;
			sound.Volume        = _effectVolume;
			sound.setSoundStopEventReceiver(new SoundStop(onStop));
			sound.Paused = false;
		}

		#endregion
	}
}
