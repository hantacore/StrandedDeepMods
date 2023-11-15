using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;
using Beam;

namespace StrandedDeepLODMod
{
    public class ShrimpParticleSystem : MonoBehaviour
    {
        private ParticleSystem ps;
        public bool enter;
        public bool exit;
        public bool inside;
        public bool outside;

        private int maxParticles = 100;

        void Start()
        {
            try
            {
                ps = GetComponent<ParticleSystem>();

                //ps.SetParticles()

                // https://docs.unity3d.com/ScriptReference/ParticleSystem.MainModule.html
                ParticleSystem.MainModule mm = ps.main;
                mm.duration = 10f;
                mm.loop = true;
                mm.startDelay = 0;
                mm.startLifetime = new ParticleSystem.MinMaxCurve(10f, 20f);// 50;
                mm.startSpeed = 0f;
                mm.startSize = new ParticleSystem.MinMaxCurve(0.01f, 0.1f);// 10;
                mm.startRotation = new ParticleSystem.MinMaxCurve(-20.0F * Mathf.Deg2Rad, 20.0F * Mathf.Deg2Rad);//works, flips the sprite;
                mm.simulationSpeed = 0.1f;
                mm.cullingMode = ParticleSystemCullingMode.Automatic;
                mm.flipRotation = 1f; //(0 to 1)
                mm.prewarm = true;
                mm.maxParticles = maxParticles;
                //mm.playOnAwake = true;
                //mm.randomizeRotationDirection = 40;
                //mm.gravityModifier = new ParticleSystem.MinMaxCurve(1.05f, 1.2f);
                //mm.gravityModifier = new ParticleSystem.MinMaxCurve(0.1f, 0.3f); // floating

                ParticleSystem.ShapeModule sm = ps.shape;
                sm.shapeType = ParticleSystemShapeType.Box;
                sm.scale = new Vector3(20, 20, 20);
                sm.rotation = new Vector3(90, 0, 0);
                sm.position = new Vector3(0, 250, 0);
                sm.donutRadius = 50f;
                sm.radius = 50f;
                sm.radiusSpread = 50f;
                sm.radiusThickness = 50f;
                sm.randomDirectionAmount = 1f;
                sm.sphericalDirectionAmount = 1f;
                sm.enabled = true;

                ParticleSystem.VelocityOverLifetimeModule vol = ps.velocityOverLifetime;
                vol.y = new ParticleSystem.MinMaxCurve(-2, -1); // 1 -> 2 ?

                ParticleSystem.EmissionModule em = ps.emission;
                em.rateOverDistance = new ParticleSystem.MinMaxCurve(10f, 50f); ;
                em.enabled = true;

                ParticleSystem.NoiseModule nm = ps.noise;
                nm.enabled = true;
                nm.strength = 2.5f;
                nm.frequency = 0.6f;
                nm.damping = true;
                nm.octaveCount = 1;
                nm.quality = ParticleSystemNoiseQuality.High;
                //nm.rotationAmount = new ParticleSystem.MinMaxCurve(0.5f, 2.5f);
                //nm.separateAxes = true;

                var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

                try
                {
                    Material mat = new Material(Shader.Find("Standard (Specular setup)"));
                    //Material mat = new Material(Shader.Find("Beam Team/ Particles / Alpha Blended"));
                    //mat.SetColor("_Color", new Color(1f, 1f, 1f, 1.000f));
                    mat.SetTexture("_MainTex", Main._indexedTextures["StrandedDeepLODMod.assets.Textures.shrimp2.png"]);

                    mat.SetFloat("_Mode", 2);
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mat.SetInt("_ZWrite", 0);
                    mat.DisableKeyword("_ALPHATEST_ON");
                    mat.EnableKeyword("_ALPHABLEND_ON");
                    mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    mat.renderQueue = 3300;

                    ParticleSystemRenderer r = ps.GetComponent<ParticleSystemRenderer>();
                    r.alignment = ParticleSystemRenderSpace.Facing;
                    r.material = mat;
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep LOD Mod : particle system Start 2 failed : " + e);
                }
                try
                {
                    var shape = ps.shape;
                    shape.enabled = false;
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep LOD Mod : particle system Start 3 failed : " + e);
                }
                try
                {
                    var trigger = ps.trigger;
                    trigger.enabled = true;
                    trigger.SetCollider(0, sphere.GetComponent<Collider>());
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep LOD Mod : particle system Start 4 failed : " + e);
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
            if (!gameObject.activeSelf)
            {
                ps.Pause();
            }

            if (!PlayerRegistry.LocalPlayer.Movement.IsUnderwater
                || !CheckDistance())
            {
                ps.Pause();
            }

            if (ps.isPaused)
                ps.Play();

            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[maxParticles];
            int count = ps.GetParticles(particles);
            //Debug.Log("Stranded Deep LOD Mod : small fishes particle system check depth");
            for (int i = count - 1; i >= 0; i--)
            {
                try
                {
                    //Vector3 velocity = particles[i].velocity;
                    //float perceivedAngle = (float)Math.Asin(velocity.y) * Mathf.Deg2Rad; //Mathf.Sign(velocity.y) * Mathf.Rad2Deg;
                    //particles[i].rotation = perceivedAngle;
                    //ps.SetParticles(particles, count);
                    //Debug.Log("Stranded Deep LOD Mod : shrimp particle system check depth : " + ps.transform.TransformPoint(particles[i].position).y);
                    if (ps.transform.TransformPoint(particles[i].position).y >= -0.5)
                    {
                        particles[i].remainingLifetime = 0f;
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log("Stranded Deep LOD Mod : shrimp depth control error : " + ex);
                    return;
                }
            }
            ps.SetParticles(particles);
        }

        protected virtual bool CheckDistance()
        {
            try
            {
                if (PlayerRegistry.LocalPlayer == null)
                    return false;

                float magnitude = Vector3.Magnitude(this.gameObject.transform.position - PlayerRegistry.LocalPlayer.transform.position);
                //Debug.Log("Stranded Deep AnimatedFoliage : CheckDistance magnitude = " + magnitude);
                if (magnitude > 20f)
                    return false;
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep AnimatedFoliage mod error on TreeBender CheckDistance : " + e);
            }
            return true;
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
            //Debug.Log("Stranded Deep LOD Mod : particle system on particle trigger");

            //if (enter)
            //{
            //    List<ParticleSystem.Particle> enterList = new List<ParticleSystem.Particle>();
            //    int numEnter = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enterList);

            //    for (int i = 0; i < numEnter; i++)
            //    {
            //        ParticleSystem.Particle p = enterList[i];
            //        p.startColor = new Color32(255, 0, 0, 255);
            //        enterList[i] = p;
            //    }

            //    ps.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, enterList);
            //}

            //if (exit)
            //{
            //    List<ParticleSystem.Particle> exitList = new List<ParticleSystem.Particle>();
            //    int numExit = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Exit, exitList);

            //    for (int i = 0; i < numExit; i++)
            //    {
            //        ParticleSystem.Particle p = exitList[i];
            //        p.startColor = new Color32(0, 255, 0, 255);
            //        exitList[i] = p;
            //    }

            //    ps.SetTriggerParticles(ParticleSystemTriggerEventType.Exit, exitList);
            //}

            //if (inside)
            //{
            //    List<ParticleSystem.Particle> insideList = new List<ParticleSystem.Particle>();
            //    int numInside = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Inside, insideList);

            //    for (int i = 0; i < numInside; i++)
            //    {
            //        ParticleSystem.Particle p = insideList[i];
            //        p.startColor = new Color32(0, 0, 255, 255);
            //        insideList[i] = p;
            //    }

            //    ps.SetTriggerParticles(ParticleSystemTriggerEventType.Inside, insideList);
            //}

            //if (outside)
            //{
            //    List<ParticleSystem.Particle> outsideList = new List<ParticleSystem.Particle>();
            //    int numOutside = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Outside, outsideList);

            //    for (int i = 0; i < numOutside; i++)
            //    {
            //        ParticleSystem.Particle p = outsideList[i];
            //        p.startColor = new Color32(0, 255, 255, 255);
            //        outsideList[i] = p;
            //    }

            //    ps.SetTriggerParticles(ParticleSystemTriggerEventType.Outside, outsideList);
            //}
        }
    }
}
