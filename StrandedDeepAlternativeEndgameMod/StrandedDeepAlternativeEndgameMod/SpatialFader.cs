using Beam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Funlabs;

namespace StrandedDeepAlternativeEndgameMod
{
    public class SpatialFader : MonoBehaviour
    {
        private Transform Listener;
        private AudioSource source;

        private float sqrMinDist;
        private float sqrMaxDist;

        private DateTime nextHorn = DateTime.MaxValue;
        private DateTime nextMessage = DateTime.MaxValue;
        private System.Random r = new System.Random();
        public int MINIMAL_SOUND_EFFECT_INTERVAL_MILLISECONDS = 20000;
        public int MAXIMAL_SOUND_EFFECT_INTERVAL_MILLISECONDS = 60000;

        void Start()
        {
            Listener = FindObjectOfType<AudioListener>().transform;
            if (Listener == null)
            {
                Debug.Log("Stranded Deep AlternativeEndgame Mod : SpatialFader = listener not found");
                return;
            }
            source = GetComponent<AudioSource>();
            if (source == null)
            {
                Debug.Log("Stranded Deep AlternativeEndgame Mod : SpatialFader = source not found");
                return;
            }

            source.RealisticRolloff();
            //source.spatializePostEffects = true;
            //source.spatialize = true;
            //source.spread = 30;
            //source.spatialBlend = 1;

            sqrMinDist = source.minDistance * source.minDistance;
            sqrMaxDist = source.maxDistance * source.maxDistance;

            nextHorn = DateTime.Now;
            nextMessage = DateTime.Now;
        }

        //public static float EasedLerp(Ease ease, float from, float to, float t)
        //{
        //    return ease(t, from, to - from, 1f);
        //}

        void Update()
        {
            //IPlayer[] buffer = new IPlayer[Game.Mode.GetLocalPlayersCount()];
            //int listeners = PlayerUtilities.FindPlayers_NonAlloc(buffer, transform.position, source.maxDistance, Game.Mode.IsMultiplayer() ? PlayerUtilities.LocalPlayers() : PlayerUtilities.AllPlayers());
            //if (listeners == 0)
            //{
            //    return;
            //}

            if (source == null || Listener == null || Camera.main == null)
            {
                Debug.Log("Stranded Deep AlternativeEndgame Mod : SpatialFader = update skip");
                return;
            }
            Vector3 t = (transform.position - Listener.position);
            float distance = Vector3.Distance(Camera.main.transform.position, transform.position);//(t.sqrMagnitude - sqrMinDist) / (sqrMaxDist - sqrMinDist);

            if (!source.isPlaying 
                && distance > source.maxDistance
                || Main.isInEndgame
                || Singleton<GameTime>.Instance.Paused)
            {
                return;
            }

            // underwater filter to add ?

            // linear fadeout
            source.volume = Mathf.Lerp(1, 0, (3.5f * (float)distance / (float)source.maxDistance));
            //Debug.Log("Stranded Deep AlternativeEndgame Mod : SpatialFader volume = " + source.volume);

            Vector3 targetToSource = (source.transform.position - Listener.position).normalized;//(Listener.position - source.transform.position).normalized;
            //Debug.Log("Stranded Deep AlternativeEndgame Mod : SpatialFader target to source : " + targetToSource);
            Vector3 targetHeadOrientation = Camera.main.transform.forward.normalized;//Listener.TransformDirection(Vector3.forward);
            //Debug.Log("Stranded Deep AlternativeEndgame Mod : SpatialFader listener forward : " + targetHeadOrientation);

            float angle = Vector3.SignedAngle(targetHeadOrientation, targetToSource, Vector3.up);//Vector3.Angle(sourceToTarget, targetHeadOrientation);
            //Debug.Log("Stranded Deep AlternativeEndgame Mod : SpatialFader = angle : " + angle);
            // -1.0 = Full left 0.0 = center 1.0 = full right
            float stereoBias = Mathf.Lerp(0.8f, 1.0f, ((float)distance / (float)source.maxDistance));//1 to 0.8
            source.panStereo = stereoBias * Mathf.Sin(Mathf.Deg2Rad * angle);
            //Debug.Log("Stranded Deep AlternativeEndgame Mod : SpatialFader = pan : " + source.panStereo);
            source.velocityUpdateMode = AudioVelocityUpdateMode.Dynamic;

            if (!source.isPlaying
                && DateTime.Compare(DateTime.Now, nextHorn) >= 0)
            {
                source.Play();
                nextHorn.AddMilliseconds(r.Next(MINIMAL_SOUND_EFFECT_INTERVAL_MILLISECONDS, MAXIMAL_SOUND_EFFECT_INTERVAL_MILLISECONDS));
                if (DateTime.Compare(DateTime.Now, nextMessage) >= 0
                    && ! Main.isInEndgame
                    && source.volume >= 0.1f)
                {
                    Main.ShowSubtitles(PlayerRegistry.AllPlayers[0], "Is this a ship I'm hearing ? Maybe they'll see me if I get close enough and shoot a flare ?", 5000);
                    nextMessage.AddMilliseconds(r.Next(5 * MINIMAL_SOUND_EFFECT_INTERVAL_MILLISECONDS, 5 * MAXIMAL_SOUND_EFFECT_INTERVAL_MILLISECONDS));
                }
            }
        }
    }
}
