using System;
using System.Collections.Generic;
using System.IO;
using IrrKlang;

namespace Singularity.Core
{ 
	public class SoundManager
	{
		#region Singleton

		private static SoundManager _instance;
		protected static  SoundManager Instance => _instance ?? (_instance = new SoundManager());

		#endregion

		private readonly ISoundEngine _engine;
		private readonly Dictionary<string, ISoundSource> _sounds;
		private ISound _currentMusic;
		private float _effectVolume;
		private float _musicVolume;

		private SoundManager()
		{
			this._engine = new ISoundEngine();
			this._sounds = new Dictionary<string, ISoundSource>();
			this._currentMusic = null;
			this._effectVolume = 1f;
			this._musicVolume = 1f;
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
			if (this._sounds.ContainsKey(name))
			{
				Console.WriteLine("Sound already registred");
				return;
			}

			this._sounds.Add(name, this._engine.AddSoundSourceFromFile(path));
			this._sounds[name].DefaultVolume = defaultVolume;
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
			if (this._sounds.ContainsKey(name))
			{
				Console.WriteLine("Sound already registred");
				return;
			}

			this._sounds.Add(name, this._engine.AddSoundSourceFromMemory(data, name));
			this._sounds[name].DefaultVolume = defaultVolume;
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
			if (this._sounds.ContainsKey(name))
			{
				Console.WriteLine("Sound already registred");
				return;
			}

			this._sounds.Add(name, this._engine.AddSoundSourceFromIOStream(stream, name));
			this._sounds[name].DefaultVolume = defaultVolume;
		}

		/// <summary>
		/// Remove registered Sound
		/// </summary>
		/// <param name="name">Name of Sound</param>
		public static void UnregisterSound(string name) => Instance._unregisterSound(name);
		private void _unregisterSound(string name)
		{
			if (this._sounds.ContainsKey(name))
				this._sounds.Remove(name);
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
				if (Instance._currentMusic != null)
					Instance._currentMusic.Volume = value;
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
			if (!this._sounds.ContainsKey(name))
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
				this._onStop = onStop;
			}

			public void OnSoundStopped(ISound sound, StopEventCause reason, object userData)
			{
				if (sound == Instance._currentMusic)
					this.MusicStopped();

				this._onStop?.Invoke(reason);
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
			this.CheckSound(name);

			if (this._engine.IsCurrentlyPlaying(this._sounds[name].Name) && !enableReset)
				return;

			this._currentMusic?.Stop();

			this._currentMusic = this._engine.Play2D(this._sounds[name], loop, true, true);
			this._currentMusic.PlaybackSpeed = speed;
			this._currentMusic.Pan           = pan;
			this._currentMusic.Volume        = this._musicVolume;
			if(onStop != null)
				this._currentMusic.setSoundStopEventReceiver(new SoundStop(onStop));
			this._currentMusic.Paused = startPaused;
		}


		private class LoopHelper : ISoundStopEventReceiver
		{
			public LoopHelper(string nameLooppart, float speed, float pan)
			{
				this.nameLoop = nameLooppart;
				this.speed = speed;
				this.pan = pan;
			}

			private string nameLoop;
			private float speed;
			private float pan;

			public void OnSoundStopped(ISound sound, StopEventCause reason, object userData)
			{
				if (reason == StopEventCause.SoundFinishedPlaying)
				{
					PlayMusic(this.nameLoop, true, false, this.speed, this.pan, null, true);
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
		public static void PlayLoopMusic(string nameBeginning, string nameLooppart, bool startPaused = false,
		                                 float  speed = 1f,    float  pan = 0f,     bool enableReset = false)
			=> Instance._PlayLoopMusic(nameBeginning, nameLooppart, startPaused, speed, pan, enableReset);
		private void _PlayLoopMusic(string nameBeginning, string nameLooppart, bool startPaused, float speed, float pan,
		                            bool   enableReset)
		{
			this.CheckSound(nameBeginning);
			this.CheckSound(nameLooppart);

			if ((this._engine.IsCurrentlyPlaying(this._sounds[nameBeginning].Name) ||
			     this._engine.IsCurrentlyPlaying(this._sounds[nameLooppart].Name)) && !enableReset)
				return;

			this._currentMusic?.Stop();

			this._currentMusic = this._engine.Play2D(this._sounds[nameBeginning], false, true, true);
			this._currentMusic.PlaybackSpeed = speed;
			this._currentMusic.Pan = pan;
			this._currentMusic.Volume = this._musicVolume;
			this._currentMusic.setSoundStopEventReceiver(new LoopHelper(nameLooppart, speed, pan));
			this._currentMusic.Paused = startPaused;
		}

		/// <summary>
		/// Pause Music
		/// </summary>
		public static void PauseMusic() => Instance._PauseMusic();
		private void _PauseMusic()
		{
			if(this._currentMusic != null)
				this._currentMusic.Paused = true;
		}

		/// <summary>
		/// Continue Paused Music
		/// </summary>
		public static void ContinueMusic() => Instance._ContinueMusic();
		private void _ContinueMusic()
		{
			if (this._currentMusic != null)
				this._currentMusic.Paused = false;
		}

		/// <summary>
		/// Toggle Music between paused and playing
		/// </summary>
		public static void ToggleMusic() => Instance._ToggleMusic();
		private void _ToggleMusic()
		{
			if(this._currentMusic == null)
				return;

			if(this._currentMusic.Paused)
				this._ContinueMusic();
			else
				this._PauseMusic();
		}

		/// <summary>
		/// Stop Music
		/// </summary>
		public static void StopMusic() => Instance._StopMusic();
		private void _StopMusic()
		{
			this._currentMusic?.Stop();
		}

		/// <summary>
		/// Change Speed of playing Music
		/// </summary>
		/// <param name="speed"></param>
		public static void SetSpeed(float speed) => Instance._SetSpeed(speed);
		private void _SetSpeed(float speed)
		{
			if(this._currentMusic == null)
				return;
			this._currentMusic.PlaybackSpeed = speed;
		}

		/// <summary>
		/// Change pan of playing Music
		/// </summary>
		/// <param name="pan"></param>
		public static void SetPan(float pan) => Instance._SetPan(pan);
		private void _SetPan(float pan)
		{
			if (this._currentMusic == null)
				return;
			this._currentMusic.Pan = pan;
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
			this.CheckSound(name);

			var sound = this._engine.Play2D(this._sounds[name], false, true, true);
			sound.PlaybackSpeed = speed;
			sound.Pan           = pan;
			sound.Volume        = this._effectVolume;
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
		public static void PlayEffect3D(string name, Vector3D position, float speed = 1f, float pan = 0f, Action<StopEventCause> onStop = null) => Instance._playEffect3D(name, position, speed, pan, onStop);
		private void _playEffect3D(string name, Vector3D position, float speed, float pan, Action<StopEventCause> onStop)
		{
			this.CheckSound(name);

			var sound = this._engine.Play3D(this._sounds[name], position.X, position.Y, position.Z, false, true, true);
			sound.PlaybackSpeed = speed;
			sound.Pan           = pan;
			sound.Volume        = this._effectVolume;
			sound.setSoundStopEventReceiver(new SoundStop(onStop));
			sound.Paused = false;
		}

		#endregion
	}
}
