using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StrandedDeepAlternativeEndgameMod
{
    public static class AudioSourceExtensions
    {
        public static void RealisticRolloff(this AudioSource AS)
        {
            var animCurve = new AnimationCurve(
                new Keyframe(AS.minDistance, 1f),
                new Keyframe(AS.minDistance + (AS.maxDistance - AS.minDistance) / 4f, .35f),
                new Keyframe(AS.maxDistance, 0f));

            AS.rolloffMode = AudioRolloffMode.Custom;
            animCurve.SmoothTangents(1, .025f);
            AS.SetCustomCurve(AudioSourceCurveType.CustomRolloff, animCurve);

            AS.dopplerLevel = 0f;
            AS.spread = 60f;
        }
    }
}
