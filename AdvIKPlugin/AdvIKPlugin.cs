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

namespace AdvIKPlugin
{
    [BepInPlugin(GUID, PluginName, Version)]
    [BepInDependency(KoikatuAPI.GUID, KoikatuAPI.VersionConst)]
    [BepInDependency(ExtensibleSaveFormat.ExtendedSave.GUID)]
    [BepInProcess("StudioNEOV2")]
    [BepInProcess("CharaStudio")]
    public partial class AdvIKPlugin : BaseUnityPlugin
    {
        public const string GUID = "orange.spork.advikplugin";
        public const string PluginName = "AdvIKPlugin";
        public const string Version = "0.0.1";

        public static AdvIKPlugin Instance { get; set; }

        internal BepInEx.Logging.ManualLogSource Log => Logger;

        public AdvIKPlugin()
        {
            if (Instance != null)
            {
                throw new InvalidOperationException("Singleton only.");
            }

            Instance = this;

            var harmony = Harmony.CreateAndPatchAll(typeof(Hooks));
            harmony.Patch(typeof(MPCharCtrl).GetNestedType("IKInfo", AccessTools.all).GetMethod("Init"), null, new HarmonyMethod(typeof(AdvIKGUI).GetMethod(nameof(AdvIKGUI.InitUI), AccessTools.all)));
            harmony.Patch(typeof(MPCharCtrl).GetNestedType("IKInfo", AccessTools.all).GetMethod("UpdateInfo"), null, new HarmonyMethod(typeof(AdvIKGUI).GetMethod(nameof(AdvIKGUI.UpdateUI), AccessTools.all)));

            Log.LogInfo("AdvIKPlugin Loaded");

        }

        private void Start()
        {
            CharacterApi.RegisterExtraBehaviour<AdvIKCharaController>(GUID);

            Log.LogInfo("AdvIKPlugin Started");

        }

        private bool StudioIsLoaded()
        {
            return StudioAPI.StudioLoaded;
        }
    }
}