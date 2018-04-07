using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeSnake
{
    public static class Sound
    {
        #region Fields & Properties

        public static bool ShowSubtitles;

        public static Dictionary<Sounds, SoundEffect> SoundEffects = new Dictionary<Sounds, SoundEffect>();

        static List<SoundEffectInstance> currentSoundEffects = new List<SoundEffectInstance>();
        static List<Sounds> currentSounds = new List<Sounds>();

        static float systemVolume = 1.0f;
        public static float SystemVolume
        {
            get
            {
                return systemVolume;
            }
            set
            {
                systemVolume = value;
                SetVolumeOfExistingSounds();
            }
        }
        static float sfxVolume = 1.0f;
        public static float SoundEffectsVolume
        {
            get
            {
                return sfxVolume;
            }
            set
            {
                sfxVolume = value;
                SetVolumeOfExistingSounds();
            }
        }

        static bool playSounds = true;
        public static bool PlaySounds
        {
            get
            {
                return playSounds;
            }
            set
            {
                playSounds = value;
                if (value == false)
                {
                    PauseAllSounds();
                }
                else
                {
                    ResumeAllSounds();
                }
            }
        }

        const int SPACING = 14;

        #endregion

        #region Public Methods

        public static void Update()
        {
            for (int i = 0; i < currentSoundEffects.Count; i++)
            {
                if (currentSoundEffects[i].State != SoundState.Playing)
                {
                    currentSoundEffects.RemoveAt(i);
                    currentSounds.RemoveAt(i);
                }
            }
        }

        public static void PlaySound(Sounds sound)
        {
            if (SoundEffects.ContainsKey(sound) && playSounds)
            {
                SoundEffectInstance newS = SoundEffects[sound].CreateInstance();
                newS.Volume = GetVolumeForSound(sound);
                newS.Play();
                currentSoundEffects.Add(newS);
                currentSounds.Add(sound);
            }
        }
        public static bool IsPlaying(Sounds sound)
        {
            return currentSounds.Contains(sound);
        }
        public static void Stop(Sounds sound)
        {
            if (IsPlaying(sound))
            {
                int index = currentSounds.IndexOf(sound);
                currentSoundEffects[index].Stop(true);
                currentSounds.RemoveAt(index);
                currentSoundEffects.RemoveAt(index);
            }
        }
        public static void StopAllSounds()
        {
            for (int i = 0; i < currentSoundEffects.Count; i++)
            {
                currentSoundEffects[i].Stop();
            }
            currentSoundEffects.Clear();
            currentSounds.Clear();
        }
        public static void Pause(Sounds sound)
        {
            if (IsPlaying(sound))
            {
                int index = currentSounds.IndexOf(sound);
                currentSoundEffects[index].Pause();
            }
        }
        public static void PauseAllSounds()
        {
            for (int i = 0; i < currentSoundEffects.Count; i++)
            {
                currentSoundEffects[i].Pause();
            }
        }
        public static void ResumeAllSounds()
        {
            if (playSounds)
            {
                for (int i = 0; i < currentSounds.Count; i++)
                {
                    if (currentSoundEffects[i].State == SoundState.Paused)
                    {
                        currentSoundEffects[i].Resume();
                    }
                }
            }
        }

        public static void Initialize(ContentManager content, Dictionary<Sounds, string> assets)
        {
            foreach (KeyValuePair<Sounds, string> kv in assets)
            {
                SoundEffects.Add(kv.Key, content.Load<SoundEffect>(kv.Value));
            }
        }
        public static void Update(GameTime gameTime)
        {
            for (int i = 0; i < currentSoundEffects.Count; i++)
            {
                if (currentSoundEffects[i].State == SoundState.Stopped)
                {
                    currentSoundEffects.RemoveAt(i);
                    currentSounds.RemoveAt(i);
                }
            }
        }

        #endregion

        #region Private Methods

        private static float GetVolumeForSound(Sounds sound)
        {
            //switch (sound)
            //{

            //}
            return 1.0f;
        }

        private static void SetVolumeOfExistingSounds()
        {
            for (int i = 0; i < currentSoundEffects.Count; i++)
            {
                currentSoundEffects[i].Volume = GetVolumeForSound(currentSounds[i]);
            }
        }

        #endregion
    }

    public enum Sounds
    {
        Achievement,
        Click,
        Coin,
        Error,
        Explosion,
        Nature,
        WallBreaker,
        Speed,
        Teleport,
        Buzzing,
        Theme,
        Forcefield,
        Lose,
        Freeze,
        Star,
    }
}
