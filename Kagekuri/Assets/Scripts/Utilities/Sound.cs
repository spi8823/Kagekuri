using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Kagekuri
{
    public static class Sound
    {
        private static SoundManager _SoundManager;

        public static void Initialize(SoundManager soundManager)
        {
            _SoundManager = soundManager;
        }

        public static void PlayEffect(Effect effect, bool isLoop = false, double loopTime = 1)
        {
            Debug.Log("実装されてないよ！！");
        }

        public static void PlayMusic(Music music, bool isLoop = true)
        {
            Debug.Log("実装されてないよ！！");
        }

        public enum Effect
        {
            OK, Cancel, Cursor, 
        }

        public enum Music
        {

        }
    }
}