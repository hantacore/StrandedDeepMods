using Beam;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace StrandedDeepAnimatedFoliageMod
{
    // TEST : reparente les noix de coco sous leur palmier pour qu'elles suivent son oscillation.
    //
    // Ce que fait le jeu : InteractiveObject_PALM.AddFruit() détache chaque noix (parent = null),
    // la range dans le SaveContainer de la zone (TransformParent + Parent()), puis appelle AttachFruit().
    // On se greffe sur AttachFruit() : à ce moment la noix est déjà rangée dans le conteneur,
    // on mémorise ce parent d'origine et on la place sous le palmier (worldPositionStays = true).
    //
    // Ce qu'il faut vérifier en jeu :
    //   1. nouvelle partie : les noix suivent le palmier
    //   2. sauvegarde puis chargement : les noix sont toujours là, au bon endroit, et suivent toujours
    //   3. ramasser / faire tomber une noix : elle se comporte normalement
    //   4. abattre le palmier : les noix tombent au lieu de disparaître
    //
    // reparentPalmFruits = false désactive le reparentage (les noix ne suivent alors plus le palmier).
    static partial class Main
    {
        private static bool reparentPalmFruits = true;

        private class FruitParentInfo
        {
            public Transform OriginalParent;
            public Transform Palm;
        }

        private static readonly ConditionalWeakTable<InteractiveObject_FOOD, FruitParentInfo> _fruitParents
            = new ConditionalWeakTable<InteractiveObject_FOOD, FruitParentInfo>();

        // Noix actuellement reparentées sous un palmier : nécessaire pour les retrouver après une
        // sauvegarde (une ConditionalWeakTable ne s'énumère pas).
        private static readonly HashSet<InteractiveObject_FOOD> _reparentedFruits
            = new HashSet<InteractiveObject_FOOD>();

        private static void ReparentFruitToPalm(InteractiveObject_PALM palm, InteractiveObject_FOOD fruit)
        {
            if (palm == null || fruit == null)
                return;

            Transform current = fruit.transform.parent;

            // Rien à faire si déjà sous le palmier. Pas de parent du tout = état inattendu : on ne touche à rien.
            if (current == null || current == palm.transform)
                return;

            _fruitParents.Remove(fruit);
            _fruitParents.Add(fruit, new FruitParentInfo { OriginalParent = current, Palm = palm.transform });
            _reparentedFruits.Add(fruit);

            fruit.transform.SetParent(palm.transform, true);

            // Si la noix se détache (ramassée, tombée, secouée) on la rend au jeu
            fruit.DetachedEvent -= OnPalmFruitDetached;
            fruit.DetachedEvent += OnPalmFruitDetached;

            if (debugMode)
                Debug.Log("Stranded Deep AnimatedFoliage mod : fruit " + fruit.name
                    + " reparented under " + palm.name + " (was under " + current.name + ")");
        }

        private static void RestoreFruitParent(InteractiveObject_FOOD fruit)
        {
            if (fruit == null)
                return;

            _reparentedFruits.Remove(fruit);

            FruitParentInfo info;
            if (!_fruitParents.TryGetValue(fruit, out info))
                return;

            _fruitParents.Remove(fruit);

            // Seulement si la noix est encore sous le palmier : si le jeu l'a déjà reparentée ailleurs
            // (main du joueur, conteneur...), on ne lui reprend pas.
            if (info.Palm != null && info.OriginalParent != null && fruit.transform.parent == info.Palm)
            {
                fruit.transform.SetParent(info.OriginalParent, true);

                if (debugMode)
                    Debug.Log("Stranded Deep AnimatedFoliage mod : fruit " + fruit.name
                        + " given back to " + info.OriginalParent.name);
            }
        }

        private static void OnPalmFruitDetached(IAttachable sender)
        {
            try
            {
                InteractiveObject_FOOD fruit = sender as InteractiveObject_FOOD;
                if (fruit == null)
                    return;

                fruit.DetachedEvent -= OnPalmFruitDetached;
                RestoreFruitParent(fruit);
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep AnimatedFoliage mod : error in OnPalmFruitDetached : " + e);
            }
        }

        // StrandedWorld.SaveZone() "aplatit" la hiérarchie : pour chaque SaveablePrefab trouvé dans le
        // SaveContainer (enfants imbriqués inclus, donc nos noix sont bien sauvegardées), elle appelle
        // ParentObjectToTransform(objet, conteneur) juste avant de le sérialiser. Les noix quittent alors
        // leur palmier. Les données sauvegardées sont celles du jeu d'origine : on n'a qu'à remettre
        // les noix encore attachées sous leur palmier une fois la sauvegarde terminée.
        private static void ReattachReparentedFruits()
        {
            if (_reparentedFruits.Count == 0)
                return;

            // Copie : on modifie l'ensemble pendant le parcours
            List<InteractiveObject_FOOD> snapshot = new List<InteractiveObject_FOOD>(_reparentedFruits);

            for (int i = 0; i < snapshot.Count; i++)
            {
                InteractiveObject_FOOD fruit = snapshot[i];

                FruitParentInfo info;
                if (fruit == null || !_fruitParents.TryGetValue(fruit, out info) || info.Palm == null)
                {
                    // noix ou palmier détruit (zone déchargée, noix ramassée puis détruite...) : on oublie
                    _reparentedFruits.Remove(fruit);
                    continue;
                }

                if (fruit.transform.parent != info.Palm)
                {
                    // le parent actuel est le conteneur choisi par le jeu : on le retient pour le détachement
                    if (fruit.transform.parent != null)
                        info.OriginalParent = fruit.transform.parent;

                    fruit.transform.SetParent(info.Palm, true);

                    if (debugMode)
                        Debug.Log("Stranded Deep AnimatedFoliage mod : fruit " + fruit.name
                            + " reparented under " + info.Palm.name + " after save");
                }
            }
        }

        // SaveZone est appelée pour chaque zone lors d'une sauvegarde (destroy = false) et lors du
        // déchargement d'une zone (destroy = true : tout est détruit, on ne fait alors que purger).
        [HarmonyPatch(typeof(StrandedWorld), "SaveZone")]
        class StrandedWorld_SaveZone_Patch
        {
            static void Postfix()
            {
                try
                {
                    if (!reparentPalmFruits)
                        return;

                    ReattachReparentedFruits();
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep AnimatedFoliage mod : error while patching StrandedWorld.SaveZone : " + e);
                }
            }
        }

        // AttachFruit est appelée par GenerateFruit/AddFruit (nouvelle partie, maître), par la synchro
        // multijoueur (client) et par Load() (chargement d'une sauvegarde).
        [HarmonyPatch(typeof(InteractiveObject_PALM), "AttachFruit")]
        class InteractiveObject_PALM_AttachFruit_Patch
        {
            static void Postfix(InteractiveObject_PALM __instance, InteractiveObject_FOOD fruit)
            {
                try
                {
                    if (!reparentPalmFruits)
                        return;

                    // Uniquement pour les palmiers que le mod anime
                    if (__instance.GetComponent<TreeBender>() == null)
                        return;

                    ReparentFruitToPalm(__instance, fruit);
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep AnimatedFoliage mod : error while patching InteractiveObject_PALM.AttachFruit : " + e);
                }
            }
        }

        // Lop() abat le palmier puis le détruit. DetachFruit() libère les noix, mais on ne sait pas si
        // DetachedEvent est déclenché à ce moment-là : par sécurité on rend toutes les noix avant,
        // sinon elles seraient détruites avec le palmier.
        [HarmonyPatch(typeof(InteractiveObject_PALM), "Lop")]
        class InteractiveObject_PALM_Lop_Patch
        {
            static void Prefix(InteractiveObject_PALM __instance)
            {
                try
                {
                    // Copie : on modifie la hiérarchie pendant le parcours
                    List<InteractiveObject_FOOD> fruits =
                        new List<InteractiveObject_FOOD>(__instance.GetComponentsInChildren<InteractiveObject_FOOD>(true));

                    for (int i = 0; i < fruits.Count; i++)
                    {
                        if (fruits[i] != null && fruits[i].transform.parent == __instance.transform)
                            RestoreFruitParent(fruits[i]);
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep AnimatedFoliage mod : error while patching InteractiveObject_PALM.Lop : " + e);
                }
            }
        }
    }
}