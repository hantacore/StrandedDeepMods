﻿using Beam;
using Beam.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityModManagerNet;

namespace StrandedDeepSharksMod
{
    public static class WorldUtilities
    {
        private static bool worldLoaded = false;
        private static StrandedWorld previousInstance = null;

        public static bool IsStrandedWide()
        {
            UnityModManager.ModEntry mewide = UnityModManager.FindMod("StrandedWideMod");
            return (Game.FindObjectOfType<UMainMenuViewAdapter>().VersionNumberLabel.Text.Contains("Wide")
                    || mewide != null && mewide.Active && mewide.Loaded);
        }

        public static bool IsWorldLoaded()
        {
            if (Beam.Game.State == GameState.NEW_GAME
                || Beam.Game.State == GameState.LOAD_GAME)
            {
                // anti memory leak
                if (previousInstance != null
                && !System.Object.ReferenceEquals(previousInstance, StrandedWorld.Instance))
                {
                    Debug.Log("Stranded Deep Sharks Mod : world instance changed, clearing events");
                    previousInstance.WorldGenerated -= Instance_WorldGenerated;
                    previousInstance = null;
                    worldLoaded = false;
                }

                if (StrandedWorld.Instance != null
                    && !System.Object.ReferenceEquals(StrandedWorld.Instance, previousInstance))
                {
                    Debug.Log("Stranded Deep Sharks Mod : world instance found, registering events");
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

        private static void Reset()
        {
            worldLoaded = false;
        }

        private static void Instance_WorldGenerated()
        {
            Debug.Log("Stranded Deep Sharks Mod : World Loaded event");
            worldLoaded = true;
        }
    }
}