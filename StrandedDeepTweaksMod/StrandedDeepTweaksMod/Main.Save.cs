using Beam;
using Beam.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StrandedDeepTweaksMod
{
    public static partial class Main
    {
        public static bool CanSave()
        {
            if (LevelLoader.IsCreatingOrLoading)
            {
                return false;
            }
            if (AnyPlayerIsBusy())
            {
                return false;
            }
            if (AnyPlayerKnockedOut())
            {
                return false;
            }
#warning anti-purpose but...
            //if (!AllPlayersOverLand())
            //{
            //    return false;
            //}
            return true;
        }

        public static bool AnyPlayerIsBusy()
        {
            foreach (IPlayer player in PlayerRegistry.AllPlayers)
            {
                if (player.Crafter.IsPlacing)
                {
                    return true;
                }
                if (player.Lighter.IsLightingFire)
                {
                    return true;
                }
                if (player.Skinner.IsSkinning)
                {
                    return true;
                }
                if (player.Movement.CheckVehicle())
                {
                    return true;
                }
                if (player.Movement.groundedAnimal)
                {
                    return true;
                }
                if (player.IsBusy)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool AnyPlayerKnockedOut()
        {
            foreach (IPlayer player in PlayerRegistry.AllPlayers)
            {
                Player player2 = (Player)player;
                if (!player2.IsNullOrDestroyed() && player2.Statistics.IsKnockedOut)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool AllPlayersOverLand()
        {
            Zone y = Singleton<StrandedWorld>.Instance.FindEndGameZone();
            foreach (IPlayer player in PlayerRegistry.AllPlayers)
            {
                Zone zone = StrandedWorld.GetZone(player.Movement.transform.position, false);
                if (zone == null || (zone.Biome == Zone.BiomeType.MISSION && zone != y))
                {
                    return false;
                }
                RaycastHit raycastHit;
                if (!Physics.Raycast(player.Movement.transform.position, Vector3.down, out raycastHit, 256f, 1 << Layers.TERRAIN))
                {
                    return false;
                }
                if (raycastHit.point.y < -5f)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
