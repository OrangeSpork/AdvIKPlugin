using KKABMX.Core;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace AdvIKPlugin.Algos
{
    public class ResizeBoneEffect : BoneEffect
    {
        public bool Enabled {
            get { return resizeAmount == Vector3.one; } 
        }
        private Vector3 resizeAmount = Vector3.one;
        public Vector3 ResizeAmount 
        {
            get { return resizeAmount; } 
            set 
            {
                resizeAmount = value;
                resizeModifier.ScaleModifier = resizeAmount;
            } 
        }

#if KOIKATSU || KKS
        private static string[] bonesEffected = { "cf_n_height" };        
#else
        private static string[] bonesEffected = { "cf_N_height" };
#endif

        private BoneModifierData resizeModifier = new BoneModifierData(Vector3.one, 1f);


        public override IEnumerable<string> GetAffectedBones(BoneController origin)
        {
            return bonesEffected;
        }

#if KOIKATSU || KKS
        public override BoneModifierData GetEffect(string bone, BoneController origin, ChaFileDefine.CoordinateType coordinate)
#else
        public override BoneModifierData GetEffect(string bone, BoneController origin, CoordinateType coordinate)
#endif
        {
            return resizeModifier;
        }
    }
}
