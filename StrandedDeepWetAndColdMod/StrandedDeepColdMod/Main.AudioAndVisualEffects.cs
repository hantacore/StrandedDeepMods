using Beam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StrandedDeepWetAndColdMod
{
    static partial class Main
    {
        private static DateTime nextCough = DateTime.MaxValue;
        private static DateTime nextGripe = DateTime.MaxValue;
        private static AudioClip coughMan = null;
        private static AudioClip coughWoman = null;
        private static AudioClip gripeMan = null;
        private static AudioClip gripeWoman = null;

        private static AudioClip shiverMan = null;
        private static AudioClip shiverWoman = null;
        private static AudioClip reliefMan = null;
        private static AudioClip reliefWoman = null;

        internal static void LoadAudioEffects()
        {
            coughMan = WavUtility.ToAudioClip(ExtractResource("StrandedDeepWetAndColdMod.audio.cough_man.wav"));
            coughWoman = WavUtility.ToAudioClip(ExtractResource("StrandedDeepWetAndColdMod.audio.cough_woman.wav"));
            gripeMan = WavUtility.ToAudioClip(ExtractResource("StrandedDeepWetAndColdMod.audio.gripe_man.wav"));
            gripeWoman = WavUtility.ToAudioClip(ExtractResource("StrandedDeepWetAndColdMod.audio.gripe_woman.wav"));

            shiverMan = WavUtility.ToAudioClip(ExtractResource("StrandedDeepWetAndColdMod.audio.cold_man.wav"));
            shiverWoman = WavUtility.ToAudioClip(ExtractResource("StrandedDeepWetAndColdMod.audio.cold_woman.wav"));
            reliefMan = WavUtility.ToAudioClip(ExtractResource("StrandedDeepWetAndColdMod.audio.relief_man.wav"));
            reliefWoman = WavUtility.ToAudioClip(ExtractResource("StrandedDeepWetAndColdMod.audio.relief_woman.wav"));
        }

        #region Audio effects

        private static void PlayCough(IPlayer p)
        {
            if (p.Gender == 0)
                AudioManager.GetAudioPlayer().Play2D(coughMan, AudioMixerChannels.Voice, AudioPlayMode.Single);
            else
                AudioManager.GetAudioPlayer().Play2D(coughWoman, AudioMixerChannels.Voice, AudioPlayMode.Single);
        }

        private static void PlayGripe(IPlayer p)
        {
            if (p.Gender == 0)
                AudioManager.GetAudioPlayer().Play2D(gripeMan, AudioMixerChannels.Voice, AudioPlayMode.Single);
            else
                AudioManager.GetAudioPlayer().Play2D(gripeWoman, AudioMixerChannels.Voice, AudioPlayMode.Single);
        }

        private static void PlayShiver(IPlayer p)
        {
            if (p.Gender == 0)
                AudioManager.GetAudioPlayer().Play2D(shiverMan, AudioMixerChannels.Voice, AudioPlayMode.Single);
            else
                AudioManager.GetAudioPlayer().Play2D(shiverWoman, AudioMixerChannels.Voice, AudioPlayMode.Single);
        }

        private static void PlayRelief(IPlayer p)
        {
            if (p.Gender == 0)
                AudioManager.GetAudioPlayer().Play2D(reliefMan, AudioMixerChannels.Voice, AudioPlayMode.Single);
            else
                AudioManager.GetAudioPlayer().Play2D(reliefWoman, AudioMixerChannels.Voice, AudioPlayMode.Single);
        }

        #endregion

        internal static void UpdateAudioAndVisualEffects(Player p, int currentPlayerIndex)
        {
            if (p.Statistics.HasStatusEffect(sickEffect[currentPlayerIndex])
                                && DateTime.Compare(DateTime.Now, nextCough) >= 0)
            {
                PlayCough(p);
                nextCough = nextCough.AddMilliseconds(r.Next(ParameterValues.MINIMAL_SOUND_EFFECT_INTERVAL_MILLISECONDS, ParameterValues.MAXIMAL_SOUND_EFFECT_INTERVAL_MILLISECONDS));
            }

            if (p.Statistics.HasStatusEffect(feverEffect[currentPlayerIndex]))
            {
                // set effect
                //Debug.Log("Stranded Deep "+MODNAME+" Mod : original focal length : " + Camera.current.focalLength);
                feverOverlayImage.enabled = true;
                if (DateTime.Compare(DateTime.Now, nextGripe) >= 0)
                {
                    PlayGripe(p);
                    nextGripe = nextGripe.AddMilliseconds(r.Next(ParameterValues.MINIMAL_SOUND_EFFECT_INTERVAL_MILLISECONDS, ParameterValues.MAXIMAL_SOUND_EFFECT_INTERVAL_MILLISECONDS));
                }
                else
                {
                    if (nextGripe.CompareTo(DateTime.MaxValue) == 0)
                    {
                        nextGripe = DateTime.Now.AddMilliseconds(r.Next(ParameterValues.MINIMAL_SOUND_EFFECT_INTERVAL_MILLISECONDS, ParameterValues.MAXIMAL_SOUND_EFFECT_INTERVAL_MILLISECONDS));
                    }
                }
            }
            else
            {
                // reset effect
                feverOverlayImage.enabled = false;
            }
        }
    }
}
