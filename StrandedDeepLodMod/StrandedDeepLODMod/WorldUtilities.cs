using Beam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StrandedDeepLODMod
{
    public static class WorldUtilities
    {
        private static bool worldLoaded = false;
        private static StrandedWorld previousInstance = null;

        public static bool IsStrandedWide()
        {
            if (FilePath.SAVE_FOLDER.Contains("Wide"))
            {
                return true;
            }

            return false;
        }

        public static bool IsWorldLoaded()
        {
            try
            {
                if (Beam.Game.State == GameState.NEW_GAME
                    || Beam.Game.State == GameState.LOAD_GAME)
                {
                    // anti memory leak
                    if (previousInstance != null
                        && !System.Object.ReferenceEquals(previousInstance, StrandedWorld.Instance))
                    {
                        Debug.Log("Stranded Deep LOD Mod : world instance changed, clearing events");
                        previousInstance.WorldGenerated -= Instance_WorldGenerated;
                        previousInstance = null;
                        worldLoaded = false;
                    }

                    if (StrandedWorld.Instance != null
                        && !System.Object.ReferenceEquals(StrandedWorld.Instance, previousInstance))
                    {
                        Debug.Log("Stranded Deep LOD Mod : world instance found, registering events");
                        previousInstance = StrandedWorld.Instance;
                        worldLoaded = false;
                        StrandedWorld.Instance.WorldGenerated -= Instance_WorldGenerated;
                        StrandedWorld.Instance.WorldGenerated += Instance_WorldGenerated;
                    }
                }
                else
                {
                    Reset();
                }

                return worldLoaded;
            }
            catch(Exception e)
            {
                Debug.Log("Stranded Deep LOD Mod : IsWorldLoaded error : " + e);
            }

            return false;
        }

        private static void Reset()
        {
            worldLoaded = false;
        }

        private static void Instance_WorldGenerated()
        {
            Debug.Log("Stranded Deep RaftStructures Mod : World Loaded event");
            worldLoaded = true;
        }
    }
}
