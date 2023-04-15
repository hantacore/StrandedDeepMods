using Beam;
using Beam.Utilities;
using SharpNeatLib.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace StrandedDeepSharksMod
{
    internal class SharkHandler : CollectionHandler<Piscus_Creature>
    {
        internal List<Piscus_Creature> Sharks
        {
            get
            {
                return _handledElements;
            }
        }

        //enum SharkSize
        //{
        //    Baby = 0,
        //    Small = 1,
        //    Medium = 2,
        //    Normal = 3,
        //    Big = 4,
        //    Huge = 5
        //}

        

        static SharkHandler()
        {

        }

    public SharkHandler()
            : base(2, true)
        {
            //Debug.Log("Stranded Deep Shark Mod : fi_canAttack " + fi_canAttack);
            //Debug.Log("Stranded Deep Shark Mod : fi_canAttackRafts " + fi_canAttackRafts);
            //Debug.Log("Stranded Deep Shark Mod : fi_canAttackNudge " + fi_canAttackNudge);
            //Debug.Log("Stranded Deep Shark Mod : fi_canAttackJump " + fi_canAttackJump);
            //Debug.Log("Stranded Deep Shark Mod : fi_attackDamage " + fi_attackDamage);
            //Debug.Log("Stranded Deep Shark Mod : fi_attackCritDamage " + fi_attackCritDamage);
            //Debug.Log("Stranded Deep Shark Mod : fi_minSpeed " + fi_minSpeed);
            //Debug.Log("Stranded Deep Shark Mod : fi_maxSpeed " + fi_maxSpeed);
        }

        public override void InitCollectionHandler()
        {
            base.InitCollectionHandler();
        }

        protected override void HandleOne(Piscus_Creature toHandle)
        {
            //base.HandleOne(toHandle);

            //toHandle.gameObject.SetLayerRecursively(Layers.WATER);

            //InteractiveObject_PISCUS ip = toHandle.GetComponentInParent<InteractiveObject_PISCUS>();
            //Debug.Log("Stranded Deep Shark Mod : name " + ip.gameObject.name);
            //Debug.Log("Stranded Deep Shark Mod : prefab " + ip.PrefabId);
            //// 10 TIGER
            //// 11 WHITE
            //// 34 WHALE
            //// 331 HAMMERHEAD
            //// 332 WHALESHARK
            //// 333 GOBLIN

            //if (ip.PrefabId == 10 || ip.PrefabId == 331)
            //{
            //    // random shark size
            //    SharkSize size = SharkSize.Normal;
            //    int sharksize = r.Next(100);
            //    if (sharksize < 15)
            //    {
            //        size = SharkSize.Baby;
            //    }
            //    else if (sharksize < 30)
            //    {
            //        size = SharkSize.Small;
            //    }
            //    else if (sharksize < 40)
            //    {
            //        size = SharkSize.Medium;
            //    }
            //    else if (sharksize < 90)
            //    {
            //        size = SharkSize.Normal;
            //    }
            //    else if (sharksize < 98)
            //    {
            //        size = SharkSize.Big;
            //    }
            //    else
            //    {
            //        size = SharkSize.Huge;
            //    }

            //    float scale = GetScaleBySize(size);
            //    // randomize scale
            //    Debug.Log("Stranded Deep Shark Mod : randomize scale for " + ip.gameObject.name + " : " + scale);
            //    ip.transform.localScale = new Vector3(scale, scale, scale);
            //    Piscus_Creature pc = fi_piscusCreature.GetValue(ip) as Piscus_Creature;
            //    if (pc != null)
            //    {
            //        SetAggressivityBySize(size, pc);
            //    }

            //    toHandle.gameObject.name = toHandle.gameObject.name + " - " + sharksize.ToString();
            //}
            //else if (ip.PrefabId == 332)
            //{
            //    if (ip.transform != null && fi_maxSpeed != null && fi_maxSpeed != null)
            //    {
            //        if (ip.transform != null)
            //        {
            //            ip.transform.localScale = new Vector3(2, 2.5f, 3f);
            //            Debug.Log("Stranded Deep Shark Mod : changing Whale Shark scale");
            //        }

            //        if (fi_maxSpeed != null)
            //        {
            //            Piscus_Creature pc = fi_piscusCreature.GetValue(ip) as Piscus_Creature;
            //            if (pc != null)
            //            {
            //                float currentSpeed = (float)fi_maxSpeed.GetValue(pc);
            //                Debug.Log("Stranded Deep Shark Mod : setting Whale Shark max speed to " + currentSpeed / 2f);
            //                fi_maxSpeed.SetValue(pc, currentSpeed / 2f);
            //            }
            //        }

            //        Debug.Log("Stranded Deep Shark Mod : Whale Shark updated ");
            //    }
            //}
        }

        public override void Reset()
        {
            base.Reset();
        }
    }
}
