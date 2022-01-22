using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdvIKPlugin
{
    public partial class AdvIKPlugin
    {
        internal static class Hooks
        {

#if !KKS && !KOIKATSU
            [HarmonyPrefix, HarmonyPatch(typeof(CharaCustom.CustomBase), "UpdateIKCalc")]
            public static bool OverrideCalcUpdate()
            {
                if (AdvIKPlugin.OverrideMakerIKHandling.Value && KKAPI.Maker.MakerAPI.InsideAndLoaded)
                    return false;
                else
                    return true;

            }
#else
            [HarmonyPrefix, HarmonyPatch(typeof(ChaCustom.CustomBase), "UpdateIKCalc")]
            public static bool OverrideCalcUpdate()
            {
                if (AdvIKPlugin.OverrideMakerIKHandling.Value && KKAPI.Maker.MakerAPI.InsideAndLoaded)
                    return false;
                else
                    return true;

            }
#endif

        }
    }
}