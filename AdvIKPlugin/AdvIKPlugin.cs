using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BepInEx;
using KKAPI;
using KKAPI.Chara;
using KKAPI.Studio;

using UnityEngine;
using RootMotion;
using System.Collections;

using HarmonyLib;
using Studio;
using BepInEx.Configuration;

namespace AdvIKPlugin
{
    [BepInPlugin(GUID, PluginName, Version)]
    [BepInDependency(KoikatuAPI.GUID, KoikatuAPI.VersionConst)]
    [BepInDependency(ExtensibleSaveFormat.ExtendedSave.GUID)]
    [BepInDependency(KKABMX.Core.KKABMX_Core.GUID, KKABMX.Core.KKABMX_Core.Version)]
    public partial class AdvIKPlugin : BaseUnityPlugin
    {
        public const string GUID = "orange.spork.advikplugin";
        public const string PluginName = "AdvIKPlugin";
        public const string Version = "1.6.7";

        public static AdvIKPlugin Instance { get; set; }

        // Config
        public static ConfigEntry<bool> MainGameBreathing { get; set; }
        public static ConfigEntry<float> MainGameBreathScale { get; set; }
        public static ConfigEntry<float> MainGameBreathRateScale { get; set; }
        public static ConfigEntry<float> HSceneBreathRateAdjustment { get; set; }
        public static ConfigEntry<float> HSceneBreathSizeAdjustment { get; set; }
        public static ConfigEntry<bool> MakerBreathing { get; set; }
        public static ConfigEntry<float> MakerBreathScale { get; set; }
        public static ConfigEntry<float> MakerBreathRateScale { get; set; }
        public static ConfigEntry<bool> StudioAutoApplyResize { get; set; }
        public static ConfigEntry<bool> EnableResizeOnFolder { get; set; }

        internal BepInEx.Logging.ManualLogSource Log => Logger;

        public AdvIKPlugin()
        {
            if (Instance != null)
            {
                throw new InvalidOperationException("Singleton only.");
            }

            Instance = this;

            MainGameBreathing = Config.Bind("Options", "Breathe in Main Game", false, new ConfigDescription("Characters in the Main Game will Breathe", null, new ConfigurationManagerAttributes { Order = 9 }));
            MainGameBreathScale = Config.Bind("Options", "Main Game Breath Size Scale", 1.0f, new ConfigDescription("Multiplier applied to default breath sizing", new AcceptableValueRange<float>(.25f, 3f), new ConfigurationManagerAttributes { Order = 8 }));
            MainGameBreathRateScale = Config.Bind("Options", "Main Game Breath Rate Scale", 1.0f, new ConfigDescription("Multiplier applied to default breath rate (BPM)", new AcceptableValueRange<float>(.25f, 3f), new ConfigurationManagerAttributes { Order = 7 }));

            HSceneBreathSizeAdjustment = Config.Bind("Options", "HScene Size Adjustment", 0.75f, new ConfigDescription("Multiplier applied (on top of above option) in HScenes to breath size", new AcceptableValueRange<float>(.25f, 3f), new ConfigurationManagerAttributes { Order = 6 }));
            HSceneBreathRateAdjustment = Config.Bind("Options", "HScene Rate Adjustment", 2.0f, new ConfigDescription("Multiplier applied (on top of above option) in HScenes to breath rate", new AcceptableValueRange<float>(.25f, 3f), new ConfigurationManagerAttributes { Order = 5 }));

            MakerBreathing = Config.Bind("Options", "Breathe in Maker", false, new ConfigDescription("Characters in the Maker will Breathe", null, new ConfigurationManagerAttributes { Order = 4 }));
            MakerBreathScale = Config.Bind("Options", "Maker Breath Scale", 1.0f, new ConfigDescription("Multiplier applied to default breath sizing", new AcceptableValueRange<float>(.25f, 3f), new ConfigurationManagerAttributes { Order = 3 }));
            MakerBreathRateScale = Config.Bind("Options", "Maker Breath Rate Scale", 1.0f, new ConfigDescription("Multiplier applied to default breath rate (BPM)", new AcceptableValueRange<float>(.25f, 3f), new ConfigurationManagerAttributes { Order = 2 }));

            StudioAutoApplyResize = Config.Bind("Options", "Studio - Auto Resize on Reload", true, "Automatically Apply Configured IK Adjustment on Reloading Characters");

            EnableResizeOnFolder = Config.Bind("Options", "Studio - Enable Resize on Folder Control", true, "Trigger resize with a folder of name -RESIZE:CENTROID_NAME - see readme for list of valid centroid names");

            var harmony = Harmony.CreateAndPatchAll(typeof(Hooks));
            harmony.Patch(typeof(MPCharCtrl).GetNestedType("IKInfo", AccessTools.all).GetMethod("Init"), null, new HarmonyMethod(typeof(AdvIKGUI).GetMethod(nameof(AdvIKGUI.InitUI), AccessTools.all)));
            harmony.Patch(typeof(MPCharCtrl).GetNestedType("IKInfo", AccessTools.all).GetMethod("UpdateInfo"), null, new HarmonyMethod(typeof(AdvIKGUI).GetMethod(nameof(AdvIKGUI.UpdateUI), AccessTools.all)));

#if DEBUG
            Log.LogInfo("AdvIKPlugin Loaded");
#endif
        }

        private void Start()
        {
            CharacterApi.RegisterExtraBehaviour<AdvIKCharaController>(GUID);

#if DEBUG
            Log.LogInfo("AdvIKPlugin Started");
#endif
        }

        private bool StudioIsLoaded()
        {
            return StudioAPI.StudioLoaded;
        }
    }
}