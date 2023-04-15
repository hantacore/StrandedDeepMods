using Beam;
using Beam.Utilities;
using SharpNeatLib.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace StrandedDeepWhaleSharkMod
{
    internal class SharkHandler : CollectionHandler<Piscus_Creature>
    {
        FastRandom r = new FastRandom();
        //prefabs.Add("SHARK - WHALE", "7b304494-2996-42d1-a442-0d02c6cebaf5");//332"
        private static string whaleSharkPrefabGuid = "7b304494-2996-42d1-a442-0d02c6cebaf5";
        private static uint whaleSharkPrefabId = 332;

        private static FieldInfo fi_piscusCreature = typeof(InteractiveObject_PISCUS).GetField("_piscusCreature", BindingFlags.NonPublic | BindingFlags.Instance);

        private static FieldInfo fi_minSpeed = typeof(Piscus_Creature).GetField("_minSpeed", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo fi_maxSpeed = typeof(Piscus_Creature).GetField("_maxSpeed", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo fi_canAttack = typeof(Piscus_Creature).GetField("_canAttack", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo fi_canAttackRafts = typeof(Piscus_Creature).GetField("_canAttackRafts", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo fi_canAttackJump = typeof(Piscus_Creature).GetField("_canAttackJump", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo fi_canAttackNudge = typeof(Piscus_Creature).GetField("_canAttackNudge", BindingFlags.NonPublic | BindingFlags.Instance);

        //private static FieldInfo fi_attackDamage = typeof(Piscus_Creature).GetField("_attackDamage ", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo fi_attackCritDamage = typeof(Piscus_Creature).GetField("_attackCritDamage", BindingFlags.NonPublic | BindingFlags.Instance);

        enum SharkSize
        {
            Baby = 0,
            Small = 1,
            Medium = 2,
            Normal = 3,
            Big = 4,
            Huge = 5
        }

        private static float GetScaleBySize(SharkSize size)
        {
            switch(size)
            {
                case SharkSize.Baby:
                    {
                        return 0.5f;
                    }
                case SharkSize.Small :
                    {
                        return 0.7f;
                    }
                case SharkSize.Medium:
                    {
                        return 0.85f;
                    }
                case SharkSize.Normal:
                    {
                        return 1.0f;
                    }
                case SharkSize.Big:
                    {
                        return 1.1f;
                    }
                case SharkSize.Huge:
                    {
                        return 1.2f;
                    }
                default:
                    {
                        break;
                    }
            }
            return 1.0f;
        }

        private static void SetAggressivityBySize(SharkSize size, Piscus_Creature piscus)
        {
            if (piscus == null)
                return;

            float multiplier = 1.0f;
            if (PlayerUtilities.GetServerGameDifficulty() == EGameDifficultyMode.Hard)
            {
                multiplier = 1.2f;
            }

                switch (size)
            {
                case SharkSize.Baby:
                case SharkSize.Small:
                    {
                        fi_canAttack.SetValue(piscus, false);
                        fi_canAttackRafts.SetValue(piscus, false);
                        fi_canAttackNudge.SetValue(piscus, false);
                        fi_canAttackJump.SetValue(piscus, false);
                        //fi_attackDamage.SetValue(piscus, 0.0f);
                        fi_attackCritDamage.SetValue(piscus, 0.0f);
                        break;
                    }
                case SharkSize.Medium:
                    {
                        fi_canAttack.SetValue(piscus, true);
                        fi_canAttackRafts.SetValue(piscus, false);
                        fi_canAttackNudge.SetValue(piscus, false);
                        fi_canAttackJump.SetValue(piscus, false);
                        //fi_attackDamage.SetValue(piscus, 80.0f * multiplier);
                        fi_attackCritDamage.SetValue(piscus, 100.0f * multiplier);
                        break;
                    }
                case SharkSize.Normal:
                    {
                        fi_canAttack.SetValue(piscus, true);
                        fi_canAttackRafts.SetValue(piscus, false);
                        fi_canAttackNudge.SetValue(piscus, true);
                        fi_canAttackJump.SetValue(piscus, false);
                        //fi_attackDamage.SetValue(piscus, 100.0f * multiplier);
                        fi_attackCritDamage.SetValue(piscus, 200.0f * multiplier);
                        break;
                    }
                case SharkSize.Big:
                    {
                        fi_canAttack.SetValue(piscus, true);
                        fi_canAttackRafts.SetValue(piscus, true);
                        fi_canAttackNudge.SetValue(piscus, true);
                        fi_canAttackJump.SetValue(piscus, false);
                        //fi_attackDamage.SetValue(piscus, 150.0f * multiplier);
                        fi_attackCritDamage.SetValue(piscus, 300.0f * multiplier);
                        break;
                    }
                case SharkSize.Huge:
                    {
                        fi_canAttack.SetValue(piscus, true);
                        fi_canAttackRafts.SetValue(piscus, true);
                        fi_canAttackNudge.SetValue(piscus, true);
                        fi_canAttackJump.SetValue(piscus, true);
                        //fi_attackDamage.SetValue(piscus, 200.0f * multiplier);
                        fi_attackCritDamage.SetValue(piscus, 400.0f * multiplier);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        public SharkHandler()
            : base(2, true)
        {
            Debug.Log("Stranded Deep Shark Mod : fi_canAttack " + fi_canAttack);
            Debug.Log("Stranded Deep Shark Mod : fi_canAttackRafts " + fi_canAttackRafts);
            Debug.Log("Stranded Deep Shark Mod : fi_canAttackNudge " + fi_canAttackNudge);
            Debug.Log("Stranded Deep Shark Mod : fi_canAttackJump " + fi_canAttackJump);
            //Debug.Log("Stranded Deep Shark Mod : fi_attackDamage " + fi_attackDamage);
            Debug.Log("Stranded Deep Shark Mod : fi_attackCritDamage " + fi_attackCritDamage);
            Debug.Log("Stranded Deep Shark Mod : fi_minSpeed " + fi_minSpeed);
            Debug.Log("Stranded Deep Shark Mod : fi_maxSpeed " + fi_maxSpeed);
        }

        public override void InitCollectionHandler()
        {
            base.InitCollectionHandler();
        }

        protected override void HandleOne(Piscus_Creature toHandle)
        {
            base.HandleOne(toHandle);

            toHandle.gameObject.SetLayerRecursively(Layers.WATER);

            InteractiveObject_PISCUS ip = toHandle.GetComponentInParent<InteractiveObject_PISCUS>();
            Debug.Log("Stranded Deep Shark Mod : name " + ip.gameObject.name);
            Debug.Log("Stranded Deep Shark Mod : prefab " + ip.PrefabId);
            // 10 TIGER
            // 11 WHITE
            // 34 WHALE
            // 331 HAMMERHEAD
            // 332 WHALESHARK
            // 333 GOBLIN

            if (ip.PrefabId == 10 || ip.PrefabId == 331)
            {
                // random shark size
                SharkSize size = SharkSize.Normal;
                int sharksize = r.Next(100);
                if (sharksize < 15)
                {
                    size = SharkSize.Baby;
                }
                else if (sharksize < 30)
                {
                    size = SharkSize.Small;
                }
                else if (sharksize < 40)
                {
                    size = SharkSize.Medium;
                }
                else if (sharksize < 90)
                {
                    size = SharkSize.Normal;
                }
                else if (sharksize < 98)
                {
                    size = SharkSize.Big;
                }
                else
                {
                    size = SharkSize.Huge;
                }

                float scale = GetScaleBySize(size);
                // randomize scale
                Debug.Log("Stranded Deep Shark Mod : randomize scale for " + ip.gameObject.name + " : " + scale);
                ip.transform.localScale = new Vector3(scale, scale, scale);
                Piscus_Creature pc = fi_piscusCreature.GetValue(ip) as Piscus_Creature;
                if (pc != null)
                {
                    SetAggressivityBySize(size, pc);
                }

                toHandle.gameObject.name = toHandle.gameObject.name + " - " + sharksize.ToString();
            }
            else if (ip.PrefabId == 332)
            {
                if (ip.transform != null && fi_maxSpeed != null && fi_maxSpeed != null)
                {
                    if (ip.transform != null)
                    {
                        ip.transform.localScale = new Vector3(2, 2.5f, 3f);
                        Debug.Log("Stranded Deep Shark Mod : changing Whale Shark scale");
                    }

                    if (fi_maxSpeed != null)
                    {
                        Piscus_Creature pc = fi_piscusCreature.GetValue(ip) as Piscus_Creature;
                        if (pc != null)
                        {
                            float currentSpeed = (float)fi_maxSpeed.GetValue(pc);
                            Debug.Log("Stranded Deep Shark Mod : setting Whale Shark max speed to " + currentSpeed / 2f);
                            fi_maxSpeed.SetValue(pc, currentSpeed / 2f);
                        }
                    }

                    Debug.Log("Stranded Deep Shark Mod : Whale Shark updated ");
                }
            }
        }

        public override void Reset()
        {
            base.Reset();
        }
    }
}
