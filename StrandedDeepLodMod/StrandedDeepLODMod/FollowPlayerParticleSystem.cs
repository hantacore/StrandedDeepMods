using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StrandedDeepLODMod
{
    public class FollowPlayerParticleSystem : SmallfishesParticleSystem
    {
        protected override void Start()
        {
            base.Start();

            ParticleSystem.MainModule mm = ps.main;
            mm.cullingMode = ParticleSystemCullingMode.AlwaysSimulate;

            ParticleSystem.ShapeModule sm = ps.shape;
            sm.shapeType = ParticleSystemShapeType.Box;
            sm.scale = new Vector3(12, 1, 12);
            sm.rotation = new Vector3(90, 0, 0);
            sm.position = new Vector3(10, -5, 10);

            ParticleSystem.TrailModule trail = ps.trails;
            trail.worldSpace = true;
        }
    }
}
