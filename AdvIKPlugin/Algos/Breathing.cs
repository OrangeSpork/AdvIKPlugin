using ExtensibleSaveFormat;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UnityEngine;

namespace AdvIKPlugin.Algos
{
    public class Breathing
    {

        public Breathing(Transform upperChest, Transform lowerChest, Transform abdomen, Transform breasts, Transform leftShoulder, Transform rightShoulder)
        {
            UpperChest = new OverriddenTransform(upperChest, upperChest.name);
            LowerChest = new OverriddenTransform(lowerChest, lowerChest.name);
            Abdomen = new OverriddenTransform(abdomen, abdomen.name);
            Breasts = new OverriddenTransform(breasts, breasts.name);
            LeftShoulder = new OverriddenTransform(leftShoulder, leftShoulder.name);
            RightShoulder = new OverriddenTransform(rightShoulder, rightShoulder.name);
            RestoreDefaults();
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

        public void Perform()
        {

            if (!Enabled)
            {
                return;
            }

            RecordPriorSnapshot();

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
            appliedBreathMagnitude = appliedBreathMagnitude * MagnitudeFactor;
            
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

            // Scale from current scale
            Vector3 newUpperChestScale = UpperChest.PriorScale;
            newUpperChestScale.Scale(appliedUpperBreathScale);

            Vector3 newLowerChestScale = LowerChest.PriorScale;
            newLowerChestScale.Scale(appliedLowerBreathScale);

            Vector3 newAbdomenScale = Abdomen.PriorScale;
            newAbdomenScale.Scale(appliedAbsScale);            

            // Apply
            UpperChest.Transform.localScale = newUpperChestScale;
            LowerChest.Transform.localScale = newLowerChestScale;
            Abdomen.Transform.localScale = newAbdomenScale;

            // Handle Translations

            // Upper Chest slides forward and up to keep back and bottom in the prior position
            Vector3 newUpperChestPos = UpperChest.PriorPosition;
            newUpperChestPos.z = ((1 + newUpperChestPos.z) * appliedUpperBreathScale.z) - ((1 + UpperChest.PriorPosition.z));
            newUpperChestPos.y = ((1 + newUpperChestPos.y) * appliedUpperBreathScale.y) - ((1 + UpperChest.PriorPosition.y));
            UpperChest.Transform.localPosition = newUpperChestPos;

            // Same with lower chest
            Vector3 newLowerChestPos = LowerChest.PriorPosition;
            newLowerChestPos.z = ((1 + newLowerChestPos.z) * appliedLowerBreathScale.z) - ((1 + LowerChest.PriorPosition.z));
            newLowerChestPos.y = ((1 + newLowerChestPos.y) * appliedLowerBreathScale.y) - ((1 + LowerChest.PriorPosition.y));
            LowerChest.Transform.localPosition = newLowerChestPos;

            // And abdomen
            Vector3 newAbdomenPos = Abdomen.PriorPosition;
            newAbdomenPos.z = ((1 + newAbdomenPos.z) * appliedAbsScale.z) - ((1 + Abdomen.PriorPosition.z));
            newAbdomenPos.y = ((1 + newAbdomenPos.y) * appliedAbsScale.y) - ((1 + Abdomen.PriorPosition.y));
            Abdomen.Transform.localPosition = newAbdomenPos;

            // Breasts move forward on an average of the lower/upper movement
            Vector3 newBreastDelta = (newLowerChestPos - LowerChest.PriorPosition) + (newUpperChestPos - UpperChest.PriorPosition);
            Breasts.Transform.localPosition = (Breasts.PriorPosition + newBreastDelta);

            // Left and Right Shoulders move on the X and Y, muted as desired by the Dampening Factor
            Vector3 ucDelta = newUpperChestPos - UpperChest.PriorPosition;
            float UCXExpansion = ((1 + newUpperChestPos.x) * appliedUpperBreathScale.x) - (1 + UpperChest.PriorPosition.x);

            Vector3 newLSPosition = LeftShoulder.PriorPosition;
            newLSPosition.y = (LeftShoulder.PriorPosition.y + ucDelta.y);
            newLSPosition.z = (LeftShoulder.PriorPosition.z + ucDelta.z);
            Vector3 LSUCDelta = LeftShoulder.PriorPosition - UpperChest.PriorPosition;
            newLSPosition.x = ((UpperChest.PriorPosition.x + UCXExpansion) * -1f * (1f - ShoulderDampeningFactor)) + LSUCDelta.x;
            LeftShoulder.Transform.localPosition = newLSPosition;

            Vector3 newRSPosition = RightShoulder.PriorPosition;
            newRSPosition.y = (RightShoulder.PriorPosition.y + ucDelta.y);
            newRSPosition.z = (RightShoulder.PriorPosition.z + ucDelta.z);
            Vector3 RSUCDelta = RightShoulder.PriorPosition - UpperChest.PriorPosition;
            newRSPosition.x = ((UpperChest.PriorPosition.x - UCXExpansion) * -1f * (1f - ShoulderDampeningFactor)) + RSUCDelta.x;
            RightShoulder.Transform.localPosition = newRSPosition;

            RecordFrameSnapshot();
        }

        public void RestorePriorSnapshot()
        {
            UpperChest.RestorePriorSnapshot();
            LowerChest.RestorePriorSnapshot();
            Abdomen.RestorePriorSnapshot();
            Breasts.RestorePriorSnapshot();
            LeftShoulder.RestorePriorSnapshot();
            RightShoulder.RestorePriorSnapshot();

        }

        public void RecordOriginalSnapshot()
        {
            UpperChest.RecordOriginalSnapshot();
            LowerChest.RecordOriginalSnapshot();
            Abdomen.RecordOriginalSnapshot();
            Breasts.RecordOriginalSnapshot();
            LeftShoulder.RecordOriginalSnapshot();
            RightShoulder.RecordOriginalSnapshot();
        }

        public void RestoreOriginalSnapshot()
        {
            UpperChest.RestoreOriginalSnapshot();
            LowerChest.RestoreOriginalSnapshot();
            Abdomen.RestoreOriginalSnapshot();
            Breasts.RestoreOriginalSnapshot();
            LeftShoulder.RestoreOriginalSnapshot();
            RightShoulder.RestoreOriginalSnapshot();
        }

        public void RecordPriorSnapshot()
        {
            UpperChest.RecordPriorSnapshot();
            LowerChest.RecordPriorSnapshot();
            Abdomen.RecordPriorSnapshot();
            Breasts.RecordPriorSnapshot();
            LeftShoulder.RecordPriorSnapshot();
            RightShoulder.RecordPriorSnapshot();

        }

        public void RecordFrameSnapshot()
        {
            UpperChest.RecordFrameSnapshot();
            LowerChest.RecordFrameSnapshot();
            Abdomen.RecordFrameSnapshot();
            Breasts.RecordFrameSnapshot();
            LeftShoulder.RecordFrameSnapshot();
            RightShoulder.RecordFrameSnapshot();
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
        public bool Enabled { 
            get => _enabled; 
            set
            {
                _enabled = value;
                if (_enabled)
                {
                    RecordOriginalSnapshot();
                    RecordPriorSnapshot();
                    RecordFrameSnapshot();
                }
                else
                {
                    RestoreOriginalSnapshot();
                }
                
            }
        }
            

        // Required Bones
        public OverriddenTransform UpperChest { get; set; }
        public OverriddenTransform LowerChest { get; set; }
        public OverriddenTransform Abdomen { get; set; }
        public OverriddenTransform Breasts { get; set; }
        public OverriddenTransform LeftShoulder { get; set; }
        public OverriddenTransform RightShoulder { get; set; }

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

        // Defaults
        private float defaultIntakePause = .05f;
        private float defaultHoldPause = .10f;
        private float defaultInhalePercentage = .6f;
        private int defaultBreathsPerMinute = 15;
        private Vector3 defaultBreathMagnitude = new Vector3(1.0f, 1.0f, 1.0f);
        private Vector3 defaultUpperBreathScale = new Vector3(.065f, 0.05f, .045f);
        private Vector3 defaultLowerBreathScale = new Vector3(.06f, 0.15f, .085f);
        private Vector3 defaultAbdomenScale = new Vector3(-0.045f, 0f, -0.045f);
        private float defaultShoulderDampeningFactor = .5f;
        private float defaultMagnitudeFactor = 1.0f;
    }

    
}
