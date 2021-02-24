using ExtensibleSaveFormat;
using KKABMX.Core;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace AdvIKPlugin.Algos
{
    public class BreathingBoneEffect : BoneEffect
    {

        private List<string> affectedBones = new List<string>();

        public BreathingBoneEffect(string upperChest, string lowerChest, string abdomen, string breasts, string leftShoulder, string rightShoulder, string leftBreast, string rightBreast)
        {
            UpperChest = upperChest;
            LowerChest = lowerChest;
            Abdomen = abdomen;
            Breasts = breasts;
            LeftShoulder = leftShoulder;
            RightShoulder = rightShoulder;
            LeftBreast = leftBreast;
            RightBreast = rightBreast;


            affectedBones.Add(UpperChest);
            affectedBones.Add(LowerChest);
            affectedBones.Add(Abdomen);            
            affectedBones.Add(LeftShoulder);
            affectedBones.Add(RightShoulder);
            if (Breasts != null)
            {
                affectedBones.Add(Breasts);
            }
            if (LeftBreast != null && RightBreast != null)
            {
                affectedBones.Add(LeftBreast);
                affectedBones.Add(RightBreast);
            }

            RestoreDefaults();

        }

        public override IEnumerable<string> GetAffectedBones(BoneController origin)
        {
            return affectedBones;
        }


#if KOIKATSU
        public override BoneModifierData GetEffect(string bone, BoneController origin, ChaFileDefine.CoordinateType coordinate)
#else
        public override BoneModifierData GetEffect(string bone, BoneController origin, CoordinateType coordinate)
#endif
        {
            if (!Enabled)
            {
                return null;
            }

            // determine position in cycle
            float seconds = Time.time % 60f;

            // Calculate our actual breath length
            float breathLength = 60f / BreathsPerMinute;

            // Where are we in the cycle?
            float timePosition = (seconds % breathLength) / breathLength;

            // Inhale/Exhale length
            float inhaleLength = (breathLength * InhalePercentage) - (breathLength * InhalePercentage * HoldPause);
            float exhaleLength = (breathLength * (1 - InhalePercentage)) - (breathLength * (1 - InhalePercentage) * IntakePause);

            // And finally compute our exact breathing cycle location in % of breathing to apply
            float breathPosition = 0f;
            if (timePosition < InhalePercentage)
            {
                breathPosition = Math.Min(timePosition / (inhaleLength / breathLength), 1f);

            }
            else
            {
                breathPosition = 1f - Math.Min((timePosition - InhalePercentage) / (exhaleLength / breathLength), 1f);
            }

            // Create the adjusted scales (combining magnitude and the individual adjustment factor)
            Vector3 appliedBreathMagnitude = BreathMagnitude;
            appliedBreathMagnitude = appliedBreathMagnitude * MagnitudeFactor * gameScale;

            Vector3 adjustedUpperBreathScale = UpperChestRelativeScaling;
            adjustedUpperBreathScale.Scale(appliedBreathMagnitude);

            Vector3 adjustedLowerBreathScale = LowerChestRelativeScaling;
            adjustedLowerBreathScale.Scale(appliedBreathMagnitude);

            Vector3 adjustedAbsBreathScale = AbdomenRelativeScaling;
            adjustedAbsBreathScale.Scale(appliedBreathMagnitude);

            // And apply breath cycle adjustment
            Vector3 appliedUpperBreathScale = Vector3.one + (adjustedUpperBreathScale * breathPosition);
            Vector3 appliedLowerBreathScale = Vector3.one + (adjustedLowerBreathScale * breathPosition);
            Vector3 appliedAbsScale = Vector3.one + (adjustedAbsBreathScale * breathPosition);
           

            // Handle Translations

            // Upper Chest slides forward and up to keep back and bottom in the prior position
            Vector3 newUpperChestPos = new Vector3(0, 0, 0);

            newUpperChestPos.z = ((1 + newUpperChestPos.z) * appliedUpperBreathScale.z) - ((1 + newUpperChestPos.z));
            newUpperChestPos.y = ((1 + newUpperChestPos.y) * appliedUpperBreathScale.y) - ((1 + newUpperChestPos.y));

            // Same with lower chest
            Vector3 newLowerChestPos = new Vector3(0, 0, 0);
            newLowerChestPos.z = ((1 + newLowerChestPos.z) * appliedLowerBreathScale.z) - ((1 + newLowerChestPos.z));
            newLowerChestPos.y = ((1 + newLowerChestPos.y) * appliedLowerBreathScale.y) - ((1 + newLowerChestPos.y));

            // And abdomen
            Vector3 newAbdomenPos = new Vector3(0, 0, 0);
            newAbdomenPos.z = ((1 + newAbdomenPos.z) * appliedAbsScale.z) - ((1 + newAbdomenPos.z));
            newAbdomenPos.y = ((1 + newAbdomenPos.y) * appliedAbsScale.y) - ((1 + newAbdomenPos.y));

            // Breasts move forward on an average of the lower/upper movement
            Vector3 newBreastDelta = (newLowerChestPos + newUpperChestPos) / 2f;

            // Left and Right Shoulders move on the X and Y, muted as desired by the Dampening Factor
            Vector3 ucDelta = newUpperChestPos;
            float UCXExpansion = (((1 + newUpperChestPos.x) * appliedUpperBreathScale.x) - (1 + newUpperChestPos.x)) / 2f;

            Vector3 newLSPosition = new Vector3(0, 0, 0);
            newLSPosition.y = (newLSPosition.y + ucDelta.y);
            newLSPosition.z = (newLSPosition.z + ucDelta.z);
            newLSPosition.x = UCXExpansion * -1f * (1f - ShoulderDampeningFactor);

            Vector3 newRSPosition = new Vector3(0, 0, 0);
            newRSPosition.y = (newRSPosition.y + ucDelta.y);
            newRSPosition.z = (newRSPosition.z + ucDelta.z);
            newRSPosition.x = (-1 * UCXExpansion) * -1f * (1f - ShoulderDampeningFactor);

            if (bone.Equals(UpperChest))
            {
#if KOIKATSU
                return new BoneModifierData(appliedUpperBreathScale, 1f, Vector3.zero, Vector3.zero);
#else
                return new BoneModifierData(appliedUpperBreathScale, 1f, newUpperChestPos, Vector3.zero);
#endif
            }
            else if (bone.Equals(LowerChest))
            {
#if KOIKATSU
                return new BoneModifierData(appliedLowerBreathScale, 1f, Vector3.zero, Vector3.zero);
#else
                return new BoneModifierData(appliedLowerBreathScale, 1f, newLowerChestPos, Vector3.zero);
#endif
            }
            else if (bone.Equals(Abdomen))
            {
#if KOIKATSU
                return new BoneModifierData(appliedAbsScale, 1f, Vector3.zero, Vector3.zero);
#else
                return new BoneModifierData(appliedAbsScale, 1f, newAbdomenPos, Vector3.zero);
#endif
            }
            else if (bone.Equals(Breasts))
            {
#if KOIKATSU
                return null;
#else
                return new BoneModifierData(Vector3.one, 1f, newBreastDelta, Vector3.zero);
#endif
            }
#if KOIKATSU
            else if (bone.Equals(LeftBreast))
            {
                return new BoneModifierData(Vector3.one, 1f, newBreastDelta, Vector3.zero);
            }
            else if (bone.Equals(RightBreast))
            {
                return new BoneModifierData(Vector3.one, 1f, newBreastDelta, Vector3.zero);
            }
#endif
            else if (bone.Equals(LeftShoulder))
            {
                return new BoneModifierData(Vector3.one, 1f, newLSPosition, Vector3.zero);
            }
            else if (bone.Equals(RightShoulder))
            {
                return new BoneModifierData(Vector3.one, 1f, newRSPosition, Vector3.zero);                
            }
            else
            {
                return null;
            }
        }

        public void RestoreDefaults()
        {
            IntakePause = defaultIntakePause;
            HoldPause = defaultHoldPause;
            InhalePercentage = defaultInhalePercentage;
            BreathsPerMinute = defaultBreathsPerMinute;
            BreathMagnitude = defaultBreathMagnitude;
            UpperChestRelativeScaling = defaultUpperBreathScale;
            LowerChestRelativeScaling = defaultLowerBreathScale;
            AbdomenRelativeScaling = defaultAbdomenScale;
            ShoulderDampeningFactor = defaultShoulderDampeningFactor;
            MagnitudeFactor = defaultMagnitudeFactor;
        }

        public void SaveConfig(PluginData data)
        {
            data.data["BreathingEnabled"] = Enabled;
            data.data["BreathingIntakePause"] = IntakePause;
            data.data["BreathingHoldPause"] = HoldPause;
            data.data["BreathingInhalePercentage"] = InhalePercentage;
            data.data["BreathingBPM"] = BreathsPerMinute;

            data.data["MagnitudeData"] = true;
            SaveVector3(BreathMagnitude, "BreathingMagnitude", data);
            SaveVector3(UpperChestRelativeScaling, "BreathingUpperChestScaling", data);
            SaveVector3(LowerChestRelativeScaling, "BreathingLowerChestScaling", data);
            SaveVector3(AbdomenRelativeScaling, "BreathingAbdomenScaling", data);

            data.data["BreathingShoulderDampeningFactor"] = ShoulderDampeningFactor;
            data.data["MagnitudeFactor"] = MagnitudeFactor;
        }

        private void SaveVector3(Vector3 vector, string prefix, PluginData data)
        {
            data.data[prefix + ".x"] = vector.x;
            data.data[prefix + ".y"] = vector.y;
            data.data[prefix + ".z"] = vector.z;
        }

        public void LoadConfig(PluginData data)
        {
            if (data.data.TryGetValue("BreathingEnabled", out var val1)) Enabled = (bool)val1;
            if (data.data.TryGetValue("BreathingIntakePause", out var val2)) IntakePause = (float)val2;
            if (data.data.TryGetValue("BreathingHoldPause", out var val3)) HoldPause = (float)val3;
            if (data.data.TryGetValue("BreathingInhalePercentage", out var val4)) InhalePercentage = (float)val4;
            if (data.data.TryGetValue("BreathingBPM", out var val5)) BreathsPerMinute = (int)val5;

            if (data.data.TryGetValue("MagnitudeData", out var val6))
            {
                BreathMagnitude = LoadVector3("BreathingMagnitude", data);
                UpperChestRelativeScaling = LoadVector3("BreathingUpperChestScaling", data);
                LowerChestRelativeScaling = LoadVector3("BreathingLowerChestScaling", data);
                AbdomenRelativeScaling = LoadVector3("BreathingAbdomenScaling", data);
            }


            if (data.data.TryGetValue("BreathingShoulderDampeningFactor", out var val10)) ShoulderDampeningFactor = (float)val10;
            if (data.data.TryGetValue("MagnitudeFactor", out var val11)) MagnitudeFactor = (float)val11;
        }

        private Vector3 LoadVector3(string prefix, PluginData data)
        {
            float x = float.NaN, y = float.NaN, z = float.NaN;

            if (data.data.TryGetValue(prefix + ".x", out var val1)) x = (float)val1;
            if (data.data.TryGetValue(prefix + ".y", out var val2)) y = (float)val2;
            if (data.data.TryGetValue(prefix + ".z", out var val3)) z = (float)val3;

            return new Vector3(x, y, z);
        }

        private bool _enabled = false;
        public bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
            }
        }


        // Required Bones
        public string UpperChest { get; set; }
        public string LowerChest { get; set; }
        public string Abdomen { get; set; }
        public string Breasts { get; set; }
        public string LeftShoulder { get; set; }
        public string RightShoulder { get; set; }
        public string LeftBreast { get; set; }
        public string RightBreast { get; set; }

        // Configuration
        public float IntakePause { get; set; }  // Percentage of breath time spent waiting on empty lungs (0.0-1.0) -- larger time spent here and HoldPause makes for sharper breathing
        public float HoldPause { get; set; }  // Percentage of breath time spent waiting with full lungs (0.0-1.0)
        public float InhalePercentage { get; set; } // Percentage of breath time spent inhaling vs. exhaling (.5 is even time spent)
        public int BreathsPerMinute { get; set; }
        public Vector3 BreathMagnitude { get; set; } // Overall size of the breaths (chest expansion)
        public Vector3 UpperChestRelativeScaling { get; set; } // Relative Scaling of the Upper Chest Area
        public Vector3 LowerChestRelativeScaling { get; set; } // Ditto, Lower Chest
        public Vector3 AbdomenRelativeScaling { get; set; } // Ditto, Abdomen - note use negative x and z for diaphragm breathing, positive for belly breathing
        public float ShoulderDampeningFactor { get; set; } // Dampens Shoulder movement rather than moving shoulders with upper chest. Use 0 for no motion, 1 for full motion
        public float MagnitudeFactor { get; set; } // Single number dial for breath size

#if KOIKATSU
        private static float gameScale = .3f;
#else
        private static float gameScale = 1f;
#endif

        // Defaults
        private float defaultIntakePause = .05f;
        private float defaultHoldPause = .10f;
        private float defaultInhalePercentage = .6f;
        private int defaultBreathsPerMinute = 15;
        private Vector3 defaultBreathMagnitude = new Vector3(1.0f, 1.0f, 1.0f);
#if KOIKATSU
        private Vector3 defaultUpperBreathScale = new Vector3(.04f, 0.035f, .025f);
        private Vector3 defaultLowerBreathScale = new Vector3(.04f, 0.10f, .035f);
        private Vector3 defaultAbdomenScale = new Vector3(-0.045f, 0f, -0.15f);
#else
        private Vector3 defaultUpperBreathScale = new Vector3(.065f, 0.03f, .045f);
        private Vector3 defaultLowerBreathScale = new Vector3(.06f, 0.13f, .085f);
        private Vector3 defaultAbdomenScale = new Vector3(-0.045f, 0f, -0.045f);
#endif
        private float defaultShoulderDampeningFactor = .5f;
        private float defaultMagnitudeFactor = 1.0f;
    }
}
