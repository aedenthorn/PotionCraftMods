using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using ObjectBased.UIElements.ButtonConsole;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace DevConsole
{
    [BepInPlugin("aedenthorn.DevConsole", "Dev Console", "0.1.0")]
    public partial class BepInExPlugin : BaseUnityPlugin
    {
        private static BepInExPlugin context;

        public static ConfigEntry<bool> modEnabled;
        public static ConfigEntry<bool> isDebug;
        public static ConfigEntry<int> nexusID;
        
        public static ConfigEntry<float> levelBasedLootWeighting;
        public static ConfigEntry<float> goldMult;
        public static ConfigEntry<float> crystalMult;
        public static ConfigEntry<float> lootAmountMult;
        
        public static ConfigEntry<bool> enableForCamps;
        public static ConfigEntry<bool> enableForRescue;
        public static ConfigEntry<bool> enableForFieldEncounters;
        public static ConfigEntry<bool> enableForSurvival;
        public static ConfigEntry<bool> enableForCity;
        public static ConfigEntry<bool> enableForHelllegion;
        public static ConfigEntry<bool> enableForSiege;
        public static ConfigEntry<bool> enableForFixedRewards;

        public static void Dbgl(string str = "", bool pref = true)
        {
            if (isDebug.Value)
                Debug.Log((pref ? typeof(BepInExPlugin).Namespace + " " : "") + str);
        }
        private void Awake()
        {

            context = this;
            modEnabled = Config.Bind<bool>("General", "Enabled", true, "Enable this mod");
            isDebug = Config.Bind<bool>("General", "IsDebug", true, "Enable debug logs");
            nexusID = Config.Bind<int>("General", "NexusID", 51, "Nexus mod ID for updates");

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
            Dbgl("Plugin awake");

        }
        [HarmonyPatch(typeof(ButtonConsole), "CanUseConsole")]
        static class ButtonConsole_CanUseConsole_Patch
        {
            static bool Prefix(ref bool __result)
            {
                if (!modEnabled.Value)
                    return true;

                __result = true;
                if (!Managers.Application.settings.developerBuild)
                {
                    Dbgl("Enabling developer build");
                    Managers.Application.settings.developerBuild = true;
                }

                return false;
            }
        }
        
        [HarmonyPatch(typeof(DebugManager), "Awake")]
        static class DebugManager_Awake_Patch
        {
            static void Postfix(DebugManager __instance)
            {
                if (!modEnabled.Value)
                    return;


                foreach (Transform t in GameObject.Find("Camera/Console/ConsoleCanvas/ConsolePanel/SideButtons/").transform)
                {
                    t.GetComponent<GridLayoutGroup>().cellSize = new Vector2(200, 50);
                }
            }
        }
    }
}
