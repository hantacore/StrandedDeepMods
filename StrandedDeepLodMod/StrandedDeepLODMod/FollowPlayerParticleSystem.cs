using Beam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StrandedDeepLODMod
{
    public class FollowPlayerParticleSystem : MonoBehaviour
    {
        public const string ParticleSystemName = "oceanParticles";
        private int maxParticles = 500;
        protected ParticleSystem ps;

        protected void Start()
        {
            try
            {
                foreach (ParticleSystem psystem in GetComponents<ParticleSystem>())
                {
                    Debug.Log("Stranded Deep LOD Mod : particle system Start system found : " + psystem.name);
                    if (psystem.name == ParticleSystemName)
                    {
                        ps = psystem;
                    }
                }

                //ps.SetParticles()

                // https://docs.unity3d.com/ScriptReference/ParticleSystem.MainModule.html
                ParticleSystem.MainModule mm = ps.main;
                mm.duration = 5f;
                mm.loop = true;
                mm.startDelay = 0;
                mm.startLifetime = new ParticleSystem.MinMaxCurve(10f, 20f);
                mm.startSpeed = 1f;
                mm.startSize = new ParticleSystem.MinMaxCurve(0.2f, 1f);
                mm.startRotation = new ParticleSystem.MinMaxCurve(0f, 359f);//new ParticleSystem.MinMaxCurve(-20.0F * Mathf.Deg2Rad, 20.0F * Mathf.Deg2Rad);//works, flips the sprite;
                //mm.startColor = new ParticleSystem.MinMaxGradient(Color.white, Color.gray);
                mm.simulationSpeed = 0.5f;
                mm.cullingMode = ParticleSystemCullingMode.AlwaysSimulate;
                //mm.flipRotation = 1f; //(0 to 1)
                mm.prewarm = true;
                mm.maxParticles = maxParticles;
                //mm.playOnAwake = true;
                //mm.randomizeRotationDirection = 40;
                //mm.gravityModifier = new ParticleSystem.MinMaxCurve(1.05f, 1.2f);
                //mm.gravityModifier = new ParticleSystem.MinMaxCurve(0.1f, 0.3f); // floating

                ParticleSystem.ShapeModule sm = ps.shape;
                sm.shapeType = ParticleSystemShapeType.Cone;
                sm.angle = 10.0f;
                sm.scale = new Vector3(2, 2, 1);
                sm.rotation = new Vector3(0, 180, 0);
                sm.position = PlayerRegistry.LocalPlayer.transform.forward * 4;
                sm.enabled = true;

                ParticleSystem.NoiseModule nm = ps.noise;
                nm.enabled = true;
                nm.strength = 0.1f;
                nm.frequency = 0.5f;
                nm.damping = true;
                nm.octaveCount = 1;
                nm.quality = ParticleSystemNoiseQuality.High;

                try
                {
                    Material mat = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended"));
                    //Material mat = new Material(Shader.Find("Standard"));
                    mat.SetColor("_Color", new Color(0.03684584f, 0.07751789f, 0.1132075f, 1.000f));
                    //mat.SetColor("_TintColor", Color.gray);
                    mat.SetTexture("_MainTex", Main._indexedTextures["StrandedDeepLODMod.assets.Textures.particles.png"]);

                    mat.SetFloat("_Mode", 2);
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mat.SetInt("_ZWrite", 0);
                    mat.DisableKeyword("_ALPHATEST_ON");
                    mat.EnableKeyword("_ALPHABLEND_ON");
                    mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    mat.renderQueue = 3500;

                    mat.EnableKeyword("_SPECULARHIGHLIGHTS_ON");
                    mat.EnableKeyword("_GLOSSYREFLECTIONS_OFF");
                    mat.EnableKeyword("_ENVIRONMENTREFLECTIONS_ON");

                    mat.doubleSidedGI = true;

                    foreach (ParticleSystemRenderer psr in ps.GetComponents<ParticleSystemRenderer>())
                    {
                        Debug.Log("Stranded Deep LOD Mod : particle system FollowPlayerParticleSystem renderer found " + psr.name);
                        ParticleSystemRenderer r = psr;
                        r.enabled = true;
                        r.renderMode = ParticleSystemRenderMode.Billboard;
                        r.material = mat;
                        r.alignment = ParticleSystemRenderSpace.Facing;

                        r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
                        r.receiveShadows = true;
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep LOD Mod : particle system Start 2 failed : " + e);
                }
                try
                {
                    var shape = ps.shape;
                    shape.enabled = true;
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep LOD Mod : particle system Start 3 failed : " + e);
                }

                Debug.Log("Stranded Deep LOD Mod : FollowPlayerParticleSystem Start");
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep LOD Mod : particle system Start global failed : " + e);
            }

            
        }

        private FieldInfo fi_calculatedVelocity = typeof(Movement).GetField("calculatedVelocity", BindingFlags.NonPublic | BindingFlags.Instance);

        protected void Update()
        {
            if (!PlayerRegistry.LocalPlayer.Movement.IsUnderwater)
            {
                ps.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
            else
            {
                ps.Play(false);
            }

            ParticleSystem.MainModule mm = ps.main;
            Vector3 vel = (Vector3)fi_calculatedVelocity.GetValue(PlayerRegistry.LocalPlayer.Movement);
            mm.simulationSpeed = Mathf.Max(vel.magnitude, 0.1f);

            if (ps.isPaused)
                ps.Play();

            // rotate the sprite
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
                    //Debug.Log("Stranded Deep LOD Mod : small fishes particle system check depth : " + ps.transform.TransformPoint(particles[i].position).y);

                    if (ps.transform.TransformPoint(particles[i].position).y >= -0.5)
                    {
                        particles[i].remainingLifetime = 0f;
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log("Stranded Deep LOD Mod : small fishes depth control error : " + ex);
                    return;
                }
            }
            ps.SetParticles(particles);
        }

        //protected virtual bool CheckDistance()
        //{
        //    try
        //    {
        //        if (PlayerRegistry.LocalPlayer == null)
        //            return false;

        //        float magnitude = Vector3.Magnitude(this.gameObject.transform.position - PlayerRegistry.LocalPlayer.transform.position);
        //        //Debug.Log("Stranded Deep AnimatedFoliage : CheckDistance magnitude = " + magnitude);
        //        if (magnitude > 20f)
        //            return false;
        //    }
        //    catch (Exception e)
        //    {
        //        Debug.Log("Stranded Deep AnimatedFoliage mod error on TreeBender CheckDistance : " + e);
        //    }
        //    return true;
        //}
    }
}
