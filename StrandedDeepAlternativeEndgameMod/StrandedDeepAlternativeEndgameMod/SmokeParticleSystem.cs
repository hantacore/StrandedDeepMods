using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

namespace StrandedDeepAlternativeEndgameMod
{
    public class SmokeParticleSystem : MonoBehaviour
    {
        private ParticleSystem ps;
        public bool enter;
        public bool exit;
        public bool inside;
        public bool outside;

        private int maxParticles = 1000;

        void Start()
        {
            try
            {
                ps = GetComponent<ParticleSystem>();

                //ps.SetParticles()

                // https://docs.unity3d.com/ScriptReference/ParticleSystem.MainModule.html
                ParticleSystem.MainModule mm = ps.main;
                mm.duration = 5f;
                mm.loop = true;
                mm.prewarm = true;
                mm.startDelay = 0;
                mm.startLifetime = new ParticleSystem.MinMaxCurve(8, 13);
                mm.startSpeed = new ParticleSystem.MinMaxCurve(1, 5);
                mm.startSize = new ParticleSystem.MinMaxCurve(1, 2);
                mm.playOnAwake = true;
                mm.startRotation = 0;
                //mm.simulationSpeed = 5;
                mm.cullingMode = ParticleSystemCullingMode.Automatic;
                mm.maxParticles = maxParticles;

                ParticleSystem.EmissionModule em = ps.emission;
                em.enabled = true;
                em.rateOverTime = 5;
                em.SetBursts(new ParticleSystem.Burst[]
                {
                    new ParticleSystem.Burst(0.10f, 30),
                    new ParticleSystem.Burst(0.60f, 45),
                    new ParticleSystem.Burst(0.92f, 63)
                });

                ParticleSystem.ShapeModule sm = ps.shape;
                sm.enabled = true;
                sm.shapeType = ParticleSystemShapeType.Cone;
                sm.position = new Vector3(0, 20, -12);
                sm.rotation = new Vector3(-90, 90, 0);
                //sm.angle = 90.0f;
                sm.radius = 3f;

                sm.arc = 5.0f;
                sm.randomDirectionAmount = 0;
                sm.sphericalDirectionAmount = 0;

                ParticleSystem.ColorOverLifetimeModule col = ps.colorOverLifetime;
                col.enabled = true;
                Gradient gradient = new Gradient();
                gradient.SetKeys(
                    new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(Color.gray, 1.0f) },
                    new GradientAlphaKey[] { new GradientAlphaKey(0.5f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
                );
                col.color = gradient;

                ParticleSystem.SizeOverLifetimeModule sol = ps.sizeOverLifetime;
                sol.enabled = true;
                AnimationCurve ACTemp = new AnimationCurve(new Keyframe[] {
                    new Keyframe (0, 0.1f), new Keyframe (0.5f, 0.3f) , new Keyframe (1, 1) });
                sol.size = new ParticleSystem.MinMaxCurve(3, ACTemp);

                //ParticleSystem.SizeBySpeedModule sbs = ps.sizeBySpeed;
                //sbs.enabled = true;
                //ACTemp = new AnimationCurve(new Keyframe[] {
                //    new Keyframe (0, 0) , new Keyframe (1, 1) });
                //sbs.size = new ParticleSystem.MinMaxCurve(1, 0);

                try
                {
                    Material mat = new Material(Shader.Find("Standard (Specular setup)"));
                    //Material mat = new Material(Shader.Find("Particle/Alpha Blended"));
                    //Material mat = new Material(Shader.Find("Beam Team/ Particles / Alpha Blended"));
                    //mat.SetColor("_Color", new Color(1f, 1f, 1f, 1.000f));
                    mat.SetTexture("_MainTex", Main._indexedTextures["StrandedDeepAlternativeEndgameMod.assets.textures.smoke.png"]);

                    mat.SetFloat("_Mode", 2);
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mat.SetInt("_ZWrite", 0);
                    mat.DisableKeyword("_ALPHATEST_ON");
                    mat.EnableKeyword("_ALPHABLEND_ON");
                    mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    mat.renderQueue = 3000;

                    ParticleSystemRenderer r = ps.GetComponent<ParticleSystemRenderer>();
                    r.material = mat;
                    r.renderMode = ParticleSystemRenderMode.Billboard;
                    r.maxParticleSize = 15;
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep LOD Mod : particle system Start 2 failed : " + e);
                }
                try
                {
                    //var shape = ps.shape;
                    //shape.enabled = false;
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep LOD Mod : particle system Start 3 failed : " + e);
                }

                //Debug.Log("Stranded Deep LOD Mod : particle system Start global init success");
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep LOD Mod : particle system Start global failed : " + e);
            }
        }

        void Update()
        {
            //ParticleSystem.Particle[] particles = new ParticleSystem.Particle[maxParticles];
            //int count = ps.GetParticles(particles);
            ////Debug.Log("Stranded Deep LOD Mod : small fishes particle system check depth");
            //for (int i = count - 1; i >= 0; i--)
            //{
            //    try
            //    {
            //        //Vector3 velocity = particles[i].velocity;
            //        //float perceivedAngle = (float)Math.Asin(velocity.y) * Mathf.Deg2Rad; //Mathf.Sign(velocity.y) * Mathf.Rad2Deg;
            //        //particles[i].rotation = perceivedAngle;
            //        //ps.SetParticles(particles, count);
            //        //Debug.Log("Stranded Deep LOD Mod : jellyfishes particle system check depth : " + ps.transform.TransformPoint(particles[i].position).y);
            //        if (ps.transform.TransformPoint(particles[i].position).y >= -0.5)
            //        {
            //            particles[i].remainingLifetime = 0f;
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Debug.Log("Stranded Deep LOD Mod : jellyfishes depth control error : " + ex);
            //        return;
            //    }
            //}
            //ps.SetParticles(particles);
        }

        void OnGUI()
        {
            //enter = GUI.Toggle(new Rect(25, 40, 200, 30), enter, "Enter Callback");
            //exit = GUI.Toggle(new Rect(25, 80, 200, 30), exit, "Exit Callback");
            //inside = GUI.Toggle(new Rect(25, 120, 200, 30), inside, "Inside Callback");
            //outside = GUI.Toggle(new Rect(25, 160, 200, 30), outside, "Outside Callback");
        }

        void OnParticleTrigger()
        {

        }
    }
}
