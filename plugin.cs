using BepInEx;
using BepInEx.Configuration;
using GorillaNetworking;
using HarmonyLib;
using System.IO;
using UnityEngine;
using UnityEngine.XR;
using Utilla;
using System;


namespace GorillaNostalgia
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public static ConfigEntry<bool> UseEarlyAccessDLC;
        public static ConfigEntry<bool> RemoveRocketObject;
        public static ConfigEntry<bool> RemoveMouthAnimations;

        public void Awake()
        {

            InitializeConfig();
            GorillaTagger.OnPlayerSpawned(InitializeObjects);

            Harmony.CreateAndPatchAll(typeof(Plugin).Assembly, PluginInfo.GUID);
        }

        public void InitializeConfig()
        {
            UseEarlyAccessDLC = Config.Bind("Cosmetics", "UseEarlyAccessCosmetics", false, "Limit the cosmetics in the game to the ones in the Early Access DLC.");
            RemoveMouthAnimations = Config.Bind("Players", "RemoveMouthAnimations", true, "Removes the speaking animations from player faces.");
            RemoveRocketObject = Config.Bind("Maps", "RemoveRocket", true, "Removes the rocket from City.");

        }

        [HarmonyPatch(typeof(VRRig), "LateUpdate")]
        static class RigPatch
        {
            static void Prefix(VRRig __instance)
            {
                __instance.replacementVoiceLoudnessThreshold = float.MaxValue;
            }
        }

        public void InitializeObjects()
        {
            try
            {
                if (RemoveRocketObject.Value)
                {
                    Destroy(GameObject.Find("Environment Objects/LocalObjects_Prefab/City/CosmeticsRoomAnchor/RocketShip_IdleDummy"));
                    Destroy(GameObject.Find("Environment Objects/05Maze_PersistientObjects/RocketShip_Prefab"));
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error for initializing objects: {ex}");
            }
                {
                foreach (Renderer renderer in Resources.FindObjectsOfTypeAll<Renderer>()) // Use FindObjectsOfTypeAll to get all objects, including disabled objects, such as the cave map. Then just loop through them all.
                {
                    if (renderer.sharedMaterial != null && renderer.sharedMaterial.mainTexture != null)
                    {
                        // Save the original names for the material and its texture, then change the filter mode.
                        string objectName = renderer.sharedMaterial.mainTexture.name;
                        string materialName = renderer.sharedMaterial.name;
                        renderer.sharedMaterial.mainTexture.filterMode = FilterMode.Bilinear;

                        // Reset the names for the material and its texture to the names we saved prior to the filtering change. Removing this might cause slippery objects to not function as intended.
                        renderer.sharedMaterial.mainTexture.name = objectName;
                        renderer.sharedMaterial.name = materialName;
                    
                    }
                }
            }
        }
    }
}
