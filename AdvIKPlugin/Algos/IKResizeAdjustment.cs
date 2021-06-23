using ExtensibleSaveFormat;
using Studio;
using System.Collections.Generic;
using UnityEngine;
using BepInEx.Logging;
using System.Linq;
using System;
using System.Collections.Specialized;
#if !KOIKATSU
using AIChara;
#endif

namespace AdvIKPlugin.Algos
{
    public class IKResizeAdjustment
    {

        public OCIChar Self { get; set; }

        private IKResizeCentroid centroid;
        public IKResizeCentroid Centroid
        {
            get { return centroid; }
            set
            {
                centroid = value;
#if DEBUG
                Log.LogInfo($"Setting Centroid to {centroid} for {Self?.charInfo.fileParam.fullname}");
#endif
            }
        }

        public Dictionary<IKChain, IKResizeChainAdjustment> ChainAdjustments { get; set; } = new Dictionary<IKChain, IKResizeChainAdjustment>();

        private Dictionary<IKScale, float> CurrentScale { get; set; } = new Dictionary<IKScale, float>();
        private Dictionary<IKScale, float> PriorScale { get; set; } = new Dictionary<IKScale, float>();

        public bool AdjustmentApplied { get; set; }
        public bool PriorCharacter { get; set; }

        private IKResizeCentroid AppliedCentroid { get; set; }
        private Dictionary<IKChain, IKResizeChainAdjustment> AppliedChainAdjustments { get; set; }

        private ManualLogSource Log => AdvIKPlugin.Instance.Log;

        public IKResizeAdjustment()
        {

            // Defaults
            Centroid = IKResizeCentroid.AUTO;
            ChainAdjustments[IKChain.BODY] = IKResizeChainAdjustment.CHAIN;
            ChainAdjustments[IKChain.LEFT_ARM] = IKResizeChainAdjustment.CHAIN;
            ChainAdjustments[IKChain.RIGHT_ARM] = IKResizeChainAdjustment.CHAIN;
            ChainAdjustments[IKChain.LEFT_LEG] = IKResizeChainAdjustment.CHAIN;
            ChainAdjustments[IKChain.RIGHT_LEG] = IKResizeChainAdjustment.CHAIN;

            AdjustmentApplied = false;
            PriorCharacter = false;
        }


        public void OnReload(ChaControl chaControl)
        {
            AdjustmentApplied = false;

            Self = KKAPI.Studio.StudioObjectExtensions.GetOCIChar(chaControl);
            if (Self == null)
            {
                return;
            }

            // Previous Scale becomes Current Scale
            if (CurrentScale.ContainsKey(IKScale.BODY))
            {
                PriorScale = new Dictionary<IKScale, float>(CurrentScale);
                PriorCharacter = true;
            }

            // Update Current Scale
            PopulateCurrentScale();

            if (!AdvIKPlugin.StudioAutoApplyResize.Value)
            {
                return;
            }

            // Do Adjustments
#if DEBUG
            Log.LogInfo($"Checking For IK Target Adjustment Prior Scale { (PriorScale.ContainsKey(IKScale.BODY) ? PriorScale[IKScale.BODY] : 0) } Current Scale {CurrentScale[IKScale.BODY]}");
#endif
            ApplyAdjustment();
        }

        public void UndoAdjustment()
        {

            if (!AdjustmentApplied)
            {
                return;
            }
#if DEBUG
            Log.LogInfo($"Undoing Adjustment...reversing data.");
#endif
            Dictionary<IKScale, float> tempPriorScale = new Dictionary<IKScale, float>(PriorScale);
            Dictionary<IKScale, float> tempCurrentScale = new Dictionary<IKScale, float>(CurrentScale);
            Dictionary<IKChain, IKResizeChainAdjustment> tempChainAdjustments = new Dictionary<IKChain, IKResizeChainAdjustment>(ChainAdjustments);
            IKResizeCentroid tempCentroid = Centroid;

            Centroid = AppliedCentroid;
            ChainAdjustments = AppliedChainAdjustments;
            CurrentScale = tempPriorScale;
            PriorScale = tempCurrentScale;

            ApplyAdjustment();

            CurrentScale = new Dictionary<IKScale, float>(tempCurrentScale);
            PriorScale = new Dictionary<IKScale, float>(tempPriorScale);
            Centroid = tempCentroid;
            ChainAdjustments = new Dictionary<IKChain, IKResizeChainAdjustment>(tempChainAdjustments);

            AdjustmentApplied = false;
        }

        public void ApplyAdjustment()
        {
            if (!PriorScale.ContainsKey(IKScale.BODY) || !PriorScale.Where(entry => CurrentScale[entry.Key] != entry.Value).Any())
            {
                return;
            }

            IKResizeCentroid UseCentroid = Centroid;
            if (Centroid == IKResizeCentroid.AUTO)
            {
                UseCentroid = AutodetectResizeMode(Self.charInfo);
            }
#if DEBUG
            if (Centroid == IKResizeCentroid.AUTO)
                Log.LogInfo($"Applying Auto-Adjustment Using Centroid: {UseCentroid}");
            else
                Log.LogInfo($"Applying Adjustment Using Centroid: {UseCentroid}");
#endif
            if (UseCentroid == IKResizeCentroid.BODY)
            {
                ApplyBodyCentroidAdjustment();
            }
            else if (UseCentroid == IKResizeCentroid.FEET_CENTER || UseCentroid == IKResizeCentroid.FEET_LEFT || UseCentroid == IKResizeCentroid.FEET_RIGHT)
            {
                ApplyFeetCentroidAdjustment(UseCentroid);
            }
            else if (UseCentroid == IKResizeCentroid.HAND_CENTER || UseCentroid == IKResizeCentroid.HAND_LEFT || UseCentroid == IKResizeCentroid.HAND_RIGHT)
            {
                ApplyHandCentroidAdjustment(UseCentroid);
            }
            else if (UseCentroid == IKResizeCentroid.THIGH_CENTER || UseCentroid == IKResizeCentroid.THIGH_LEFT || UseCentroid == IKResizeCentroid.THIGH_RIGHT)
            {
                ApplyThighCentroidAdjustment(UseCentroid);
            }
            else if (UseCentroid == IKResizeCentroid.SHOULDER_CENTER || UseCentroid == IKResizeCentroid.SHOULDER_LEFT || UseCentroid == IKResizeCentroid.SHOULDER_RIGHT)
            {
                ApplyShoulderCentroidAdjustment(UseCentroid);
            }
            else if (UseCentroid == IKResizeCentroid.KNEE_CENTER || UseCentroid == IKResizeCentroid.KNEE_LEFT || UseCentroid == IKResizeCentroid.KNEE_RIGHT)
            {
                ApplyKneeCentroidAdjustment(UseCentroid);
            }
            else if (UseCentroid == IKResizeCentroid.ELBOW_CENTER || UseCentroid == IKResizeCentroid.ELBOW_LEFT || UseCentroid == IKResizeCentroid.ELBOW_RIGHT)
            {
                ApplyElbowCentroidAdjustment(UseCentroid);
            }


            if (UseCentroid != IKResizeCentroid.NONE)
            {
                AdjustmentApplied = true;
                AppliedCentroid = UseCentroid;
                AppliedChainAdjustments = new Dictionary<IKChain, IKResizeChainAdjustment>(ChainAdjustments);
            }
        }

        private void ApplyThighCentroidAdjustment(IKResizeCentroid centroid)
        {
            // Thigh Adjustment works down the leg chain from thigh
            // Then adjust Body relative to Thigh
            // Then Chain from Body

            Dictionary<IKTarget, Vector3> PriorLocations = FillPriorLocations();
            Vector3 baselineLocalPosition;
            if (centroid == IKResizeCentroid.THIGH_LEFT)
            {
                baselineLocalPosition = FindTargetTransform(IKTarget.LEFT_THIGH).localPosition;

                if (!SkipChain(IKChain.LEFT_LEG))
                {
                    ScaleIKTarget(IKTarget.LEFT_KNEE, baselineLocalPosition, PriorLocations[IKTarget.LEFT_THIGH], PriorLocations[IKTarget.LEFT_KNEE], PriorScale[IKScale.LEFT_UPPER_LEG], CurrentScale[IKScale.LEFT_UPPER_LEG]);
                    ScaleIKTarget(IKTarget.LEFT_FOOT, baselineLocalPosition, PriorLocations[IKTarget.LEFT_THIGH], PriorLocations[IKTarget.LEFT_FOOT], PriorScale[IKScale.LEFT_LEG], CurrentScale[IKScale.LEFT_LEG]);
                }

                ScaleIKTarget(IKTarget.BODY, baselineLocalPosition, PriorLocations[IKTarget.LEFT_THIGH], PriorLocations[IKTarget.BODY], PriorScale[IKScale.BODY], CurrentScale[IKScale.BODY]);

                AdjustBody(FindTargetTransform(IKTarget.BODY), new IKTarget[] { IKTarget.LEFT_THIGH }, PriorLocations);

                AdjustChain(IKChain.RIGHT_LEG, PriorLocations);
                AdjustChain(IKChain.LEFT_ARM, PriorLocations);
                AdjustChain(IKChain.RIGHT_ARM, PriorLocations);
            }
            else if (centroid == IKResizeCentroid.THIGH_RIGHT)
            {
                baselineLocalPosition = FindTargetTransform(IKTarget.RIGHT_THIGH).localPosition;

                if (!SkipChain(IKChain.RIGHT_LEG))
                {
                    ScaleIKTarget(IKTarget.RIGHT_KNEE, baselineLocalPosition, PriorLocations[IKTarget.RIGHT_THIGH], PriorLocations[IKTarget.RIGHT_KNEE], PriorScale[IKScale.RIGHT_UPPER_LEG], CurrentScale[IKScale.RIGHT_UPPER_LEG]);
                    ScaleIKTarget(IKTarget.RIGHT_FOOT, baselineLocalPosition, PriorLocations[IKTarget.RIGHT_THIGH], PriorLocations[IKTarget.RIGHT_FOOT], PriorScale[IKScale.RIGHT_LEG], CurrentScale[IKScale.RIGHT_LEG]);
                }

                ScaleIKTarget(IKTarget.BODY, baselineLocalPosition, PriorLocations[IKTarget.RIGHT_THIGH], PriorLocations[IKTarget.BODY], PriorScale[IKScale.BODY], CurrentScale[IKScale.BODY]);

                AdjustBody(FindTargetTransform(IKTarget.BODY), new IKTarget[] { IKTarget.RIGHT_THIGH }, PriorLocations);

                AdjustChain(IKChain.LEFT_LEG, PriorLocations);
                AdjustChain(IKChain.LEFT_ARM, PriorLocations);
                AdjustChain(IKChain.RIGHT_ARM, PriorLocations);
            }
            else
            {
                baselineLocalPosition = Vector3.Lerp(FindTargetTransform(IKTarget.LEFT_THIGH).localPosition, FindTargetTransform(IKTarget.RIGHT_THIGH).localPosition, .5f);
                Vector3 priorBaselineLocalPosition = Vector3.Lerp(PriorLocations[IKTarget.LEFT_THIGH], PriorLocations[IKTarget.RIGHT_THIGH], .5f);

                if (!SkipChain(IKChain.LEFT_LEG))
                {                    
                    ScaleIKTarget(IKTarget.LEFT_KNEE, baselineLocalPosition, priorBaselineLocalPosition, PriorLocations[IKTarget.LEFT_KNEE], PriorScale[IKScale.LEFT_UPPER_LEG], CurrentScale[IKScale.LEFT_UPPER_LEG]);
                    ScaleIKTarget(IKTarget.LEFT_FOOT, baselineLocalPosition, priorBaselineLocalPosition, PriorLocations[IKTarget.LEFT_FOOT], PriorScale[IKScale.LEFT_LEG], CurrentScale[IKScale.LEFT_LEG]);
                }

                if (!SkipChain(IKChain.RIGHT_LEG))
                {                    
                    ScaleIKTarget(IKTarget.RIGHT_KNEE, baselineLocalPosition, priorBaselineLocalPosition, PriorLocations[IKTarget.RIGHT_KNEE], PriorScale[IKScale.RIGHT_UPPER_LEG], CurrentScale[IKScale.RIGHT_UPPER_LEG]);
                    ScaleIKTarget(IKTarget.RIGHT_FOOT, baselineLocalPosition, priorBaselineLocalPosition, PriorLocations[IKTarget.RIGHT_FOOT], PriorScale[IKScale.RIGHT_LEG], CurrentScale[IKScale.RIGHT_LEG]);
                }

                ScaleIKTarget(IKTarget.LEFT_THIGH, baselineLocalPosition, priorBaselineLocalPosition, PriorLocations[IKTarget.LEFT_THIGH], PriorScale[IKScale.BODY], CurrentScale[IKScale.BODY]);
                ScaleIKTarget(IKTarget.RIGHT_THIGH, baselineLocalPosition, priorBaselineLocalPosition, PriorLocations[IKTarget.RIGHT_THIGH], PriorScale[IKScale.BODY], CurrentScale[IKScale.BODY]);
                ScaleIKTarget(IKTarget.BODY, baselineLocalPosition, priorBaselineLocalPosition, PriorLocations[IKTarget.BODY], PriorScale[IKScale.BODY], CurrentScale[IKScale.BODY]);

                AdjustBody(FindTargetTransform(IKTarget.BODY), new IKTarget[] { IKTarget.LEFT_THIGH, IKTarget.RIGHT_THIGH }, PriorLocations);
                AdjustChain(IKChain.LEFT_ARM, PriorLocations);
                AdjustChain(IKChain.RIGHT_ARM, PriorLocations);
            }
        }

        private void ApplyShoulderCentroidAdjustment(IKResizeCentroid centroid)
        {
            // Work down arm from shoulder
            // Body from shoulder
            // Then chain from body

            Dictionary<IKTarget, Vector3> PriorLocations = FillPriorLocations();
            Vector3 baselineLocalPosition;
            if (centroid == IKResizeCentroid.SHOULDER_LEFT)
            {
                baselineLocalPosition = FindTargetTransform(IKTarget.LEFT_SHOULDER).localPosition;

                if (!SkipChain(IKChain.LEFT_ARM))
                {
                    ScaleIKTarget(IKTarget.LEFT_ELBOW, baselineLocalPosition, PriorLocations[IKTarget.LEFT_SHOULDER], PriorLocations[IKTarget.LEFT_ELBOW], PriorScale[IKScale.LEFT_UPPER_ARM], CurrentScale[IKScale.LEFT_UPPER_ARM]);
                    ScaleIKTarget(IKTarget.LEFT_HAND, baselineLocalPosition, PriorLocations[IKTarget.LEFT_SHOULDER], PriorLocations[IKTarget.LEFT_HAND], PriorScale[IKScale.LEFT_ARM], CurrentScale[IKScale.LEFT_ARM]);
                }

                ScaleIKTarget(IKTarget.BODY, baselineLocalPosition, PriorLocations[IKTarget.LEFT_SHOULDER], PriorLocations[IKTarget.BODY], PriorScale[IKScale.BODY], CurrentScale[IKScale.BODY]);

                AdjustBody(FindTargetTransform(IKTarget.BODY), new IKTarget[] { IKTarget.LEFT_SHOULDER }, PriorLocations);

                AdjustChain(IKChain.RIGHT_ARM, PriorLocations);
                AdjustChain(IKChain.LEFT_LEG, PriorLocations);
                AdjustChain(IKChain.RIGHT_LEG, PriorLocations);
            }
            else if (centroid == IKResizeCentroid.SHOULDER_RIGHT)
            {
                baselineLocalPosition = FindTargetTransform(IKTarget.RIGHT_SHOULDER).localPosition;

                if (!SkipChain(IKChain.RIGHT_ARM))
                {
                    ScaleIKTarget(IKTarget.RIGHT_ELBOW, baselineLocalPosition, PriorLocations[IKTarget.RIGHT_SHOULDER], PriorLocations[IKTarget.RIGHT_ELBOW], PriorScale[IKScale.RIGHT_UPPER_ARM], CurrentScale[IKScale.RIGHT_UPPER_ARM]);
                    ScaleIKTarget(IKTarget.RIGHT_HAND, baselineLocalPosition, PriorLocations[IKTarget.RIGHT_SHOULDER], PriorLocations[IKTarget.RIGHT_HAND], PriorScale[IKScale.RIGHT_ARM], CurrentScale[IKScale.RIGHT_ARM]);
                }

                ScaleIKTarget(IKTarget.BODY, baselineLocalPosition, PriorLocations[IKTarget.RIGHT_SHOULDER], PriorLocations[IKTarget.BODY], PriorScale[IKScale.BODY], CurrentScale[IKScale.BODY]);

                AdjustBody(FindTargetTransform(IKTarget.BODY), new IKTarget[] { IKTarget.RIGHT_SHOULDER }, PriorLocations);

                AdjustChain(IKChain.LEFT_ARM, PriorLocations);
                AdjustChain(IKChain.LEFT_LEG, PriorLocations);
                AdjustChain(IKChain.RIGHT_LEG, PriorLocations);
            }
            else
            {
                baselineLocalPosition = Vector3.Lerp(FindTargetTransform(IKTarget.LEFT_SHOULDER).localPosition, FindTargetTransform(IKTarget.RIGHT_SHOULDER).localPosition, .5f);
                Vector3 priorBaselineLocalPosition = Vector3.Lerp(PriorLocations[IKTarget.LEFT_SHOULDER], PriorLocations[IKTarget.RIGHT_SHOULDER], .5f);

                if (!SkipChain(IKChain.LEFT_ARM))
                {                    
                    ScaleIKTarget(IKTarget.LEFT_ELBOW, baselineLocalPosition, priorBaselineLocalPosition, PriorLocations[IKTarget.LEFT_ELBOW], PriorScale[IKScale.LEFT_UPPER_ARM], CurrentScale[IKScale.LEFT_UPPER_ARM]);
                    ScaleIKTarget(IKTarget.LEFT_HAND, baselineLocalPosition, priorBaselineLocalPosition, PriorLocations[IKTarget.LEFT_HAND], PriorScale[IKScale.LEFT_ARM], CurrentScale[IKScale.LEFT_ARM]);
                }

                if (!SkipChain(IKChain.RIGHT_ARM))
                {                    
                    ScaleIKTarget(IKTarget.RIGHT_ELBOW, baselineLocalPosition, priorBaselineLocalPosition, PriorLocations[IKTarget.RIGHT_ELBOW], PriorScale[IKScale.RIGHT_UPPER_ARM], CurrentScale[IKScale.RIGHT_UPPER_ARM]);
                    ScaleIKTarget(IKTarget.RIGHT_HAND, baselineLocalPosition, priorBaselineLocalPosition, PriorLocations[IKTarget.RIGHT_HAND], PriorScale[IKScale.RIGHT_ARM], CurrentScale[IKScale.RIGHT_ARM]);
                }

                ScaleIKTarget(IKTarget.LEFT_SHOULDER, baselineLocalPosition, priorBaselineLocalPosition, PriorLocations[IKTarget.LEFT_SHOULDER], PriorScale[IKScale.BODY], CurrentScale[IKScale.BODY]);
                ScaleIKTarget(IKTarget.RIGHT_SHOULDER, baselineLocalPosition, priorBaselineLocalPosition, PriorLocations[IKTarget.RIGHT_SHOULDER], PriorScale[IKScale.BODY], CurrentScale[IKScale.BODY]);
                ScaleIKTarget(IKTarget.BODY, baselineLocalPosition, priorBaselineLocalPosition, PriorLocations[IKTarget.BODY], PriorScale[IKScale.BODY], CurrentScale[IKScale.BODY]);

                AdjustBody(FindTargetTransform(IKTarget.BODY), new IKTarget[] { IKTarget.LEFT_SHOULDER, IKTarget.RIGHT_SHOULDER }, PriorLocations);
                AdjustChain(IKChain.LEFT_LEG, PriorLocations);
                AdjustChain(IKChain.RIGHT_LEG, PriorLocations);
            }
        }

        private void ApplyElbowCentroidAdjustment(IKResizeCentroid centroid)
        {
            // From elbow to hand
            // From elbow to shoulder
            // From shoulder to body
            // then chain out from body

            Dictionary<IKTarget, Vector3> PriorLocations = FillPriorLocations();
            Vector3 baselineLocalPosition;
            if (centroid == IKResizeCentroid.ELBOW_LEFT)
            {
                baselineLocalPosition = FindTargetTransform(IKTarget.LEFT_ELBOW).localPosition;

                if (!SkipChain(IKChain.LEFT_ARM))
                {
                    ScaleIKTarget(IKTarget.LEFT_HAND, baselineLocalPosition, PriorLocations[IKTarget.LEFT_ELBOW], PriorLocations[IKTarget.LEFT_HAND], PriorScale[IKScale.LEFT_LOWER_ARM], CurrentScale[IKScale.LEFT_LOWER_ARM]);
                }

                ScaleIKTarget(IKTarget.LEFT_SHOULDER, baselineLocalPosition, PriorLocations[IKTarget.LEFT_ELBOW], PriorLocations[IKTarget.LEFT_SHOULDER], PriorScale[IKScale.LEFT_UPPER_ARM], CurrentScale[IKScale.LEFT_UPPER_ARM]);
                ScaleIKTarget(IKTarget.BODY, FindTargetTransform(IKTarget.LEFT_SHOULDER).localPosition, PriorLocations[IKTarget.LEFT_SHOULDER], PriorLocations[IKTarget.BODY], PriorScale[IKScale.BODY], CurrentScale[IKScale.BODY]);

                AdjustBody(FindTargetTransform(IKTarget.BODY), new IKTarget[] { IKTarget.LEFT_SHOULDER }, PriorLocations);

                AdjustChain(IKChain.RIGHT_ARM, PriorLocations);
                AdjustChain(IKChain.RIGHT_LEG, PriorLocations);
                AdjustChain(IKChain.LEFT_LEG, PriorLocations);
            }
            else if (centroid == IKResizeCentroid.ELBOW_RIGHT)
            {
                baselineLocalPosition = FindTargetTransform(IKTarget.RIGHT_ELBOW).localPosition;

                if (!SkipChain(IKChain.RIGHT_ARM))
                {
                    ScaleIKTarget(IKTarget.RIGHT_HAND, baselineLocalPosition, PriorLocations[IKTarget.RIGHT_ELBOW], PriorLocations[IKTarget.RIGHT_HAND], PriorScale[IKScale.RIGHT_LOWER_ARM], CurrentScale[IKScale.RIGHT_LOWER_ARM]);
                }

                ScaleIKTarget(IKTarget.RIGHT_SHOULDER, baselineLocalPosition, PriorLocations[IKTarget.RIGHT_ELBOW], PriorLocations[IKTarget.RIGHT_SHOULDER], PriorScale[IKScale.RIGHT_UPPER_ARM], CurrentScale[IKScale.RIGHT_UPPER_ARM]);
                ScaleIKTarget(IKTarget.BODY, FindTargetTransform(IKTarget.RIGHT_SHOULDER).localPosition, PriorLocations[IKTarget.RIGHT_SHOULDER], PriorLocations[IKTarget.BODY], PriorScale[IKScale.BODY], CurrentScale[IKScale.BODY]);

                AdjustBody(FindTargetTransform(IKTarget.BODY), new IKTarget[] { IKTarget.RIGHT_SHOULDER }, PriorLocations);

                AdjustChain(IKChain.LEFT_ARM, PriorLocations);
                AdjustChain(IKChain.LEFT_LEG, PriorLocations);
                AdjustChain(IKChain.RIGHT_LEG, PriorLocations);

            }
            else
            {
                baselineLocalPosition = Vector3.Lerp(FindTargetTransform(IKTarget.LEFT_ELBOW).localPosition, FindTargetTransform(IKTarget.RIGHT_ELBOW).localPosition, .5f);
                Vector3 priorBaselineLocalPosition = Vector3.Lerp(PriorLocations[IKTarget.LEFT_ELBOW], PriorLocations[IKTarget.RIGHT_ELBOW], .5f);

                if (!SkipChain(IKChain.LEFT_ARM))
                {
                    ScaleIKTarget(IKTarget.LEFT_HAND, baselineLocalPosition, priorBaselineLocalPosition, PriorLocations[IKTarget.LEFT_HAND], PriorScale[IKScale.LEFT_LOWER_ARM], CurrentScale[IKScale.LEFT_LOWER_ARM]);
                    ScaleIKTarget(IKTarget.LEFT_ELBOW, baselineLocalPosition, priorBaselineLocalPosition, PriorLocations[IKTarget.LEFT_ELBOW], PriorScale[IKScale.LEFT_UPPER_ARM], CurrentScale[IKScale.LEFT_UPPER_ARM]);
                }

                if (!SkipChain(IKChain.RIGHT_ARM))
                {
                    ScaleIKTarget(IKTarget.RIGHT_HAND, baselineLocalPosition, priorBaselineLocalPosition, PriorLocations[IKTarget.RIGHT_HAND], PriorScale[IKScale.RIGHT_LOWER_ARM], CurrentScale[IKScale.RIGHT_LOWER_ARM]);
                    ScaleIKTarget(IKTarget.RIGHT_ELBOW, baselineLocalPosition, priorBaselineLocalPosition, PriorLocations[IKTarget.RIGHT_ELBOW], PriorScale[IKScale.RIGHT_UPPER_ARM], CurrentScale[IKScale.RIGHT_UPPER_ARM]);
                }

                ScaleIKTarget(IKTarget.LEFT_SHOULDER, baselineLocalPosition, priorBaselineLocalPosition, PriorLocations[IKTarget.LEFT_SHOULDER], PriorScale[IKScale.LEFT_UPPER_ARM], CurrentScale[IKScale.LEFT_UPPER_ARM]);
                ScaleIKTarget(IKTarget.RIGHT_SHOULDER, baselineLocalPosition, priorBaselineLocalPosition, PriorLocations[IKTarget.RIGHT_SHOULDER], PriorScale[IKScale.RIGHT_UPPER_ARM], CurrentScale[IKScale.RIGHT_UPPER_ARM]);

                Vector3 shoulderBaseline = Vector3.Lerp(FindTargetTransform(IKTarget.LEFT_SHOULDER).localPosition, FindTargetTransform(IKTarget.RIGHT_SHOULDER).localPosition, 0.5f);
                Vector3 priorShoulderBaseline = Vector3.Lerp(PriorLocations[IKTarget.LEFT_SHOULDER], PriorLocations[IKTarget.RIGHT_SHOULDER], .5f);
                ScaleIKTarget(IKTarget.BODY, shoulderBaseline, priorShoulderBaseline, PriorLocations[IKTarget.BODY], PriorScale[IKScale.BODY], CurrentScale[IKScale.BODY]);

                AdjustBody(FindTargetTransform(IKTarget.BODY), new IKTarget[] { IKTarget.LEFT_SHOULDER, IKTarget.RIGHT_SHOULDER }, PriorLocations);
                AdjustChain(IKChain.LEFT_LEG, PriorLocations);
                AdjustChain(IKChain.RIGHT_LEG, PriorLocations);

            }
        }

        private void ApplyHandCentroidAdjustment(IKResizeCentroid centroid)
        {
            // From hand to shoulder
            // shoulder to body
            // then chain out from body

            Dictionary<IKTarget, Vector3> PriorLocations = FillPriorLocations();
            Vector3 baselineLocalPosition;
            if (centroid == IKResizeCentroid.HAND_LEFT)
            {
                baselineLocalPosition = FindTargetTransform(IKTarget.LEFT_HAND).localPosition;

                if (!SkipChain(IKChain.LEFT_ARM))
                {
                    ScaleIKTarget(IKTarget.LEFT_ELBOW, baselineLocalPosition, PriorLocations[IKTarget.LEFT_HAND], PriorLocations[IKTarget.LEFT_ELBOW], PriorScale[IKScale.LEFT_LOWER_ARM], CurrentScale[IKScale.LEFT_LOWER_ARM]);                    
                }

                ScaleIKTarget(IKTarget.LEFT_SHOULDER, baselineLocalPosition, PriorLocations[IKTarget.LEFT_HAND], PriorLocations[IKTarget.LEFT_SHOULDER], PriorScale[IKScale.LEFT_ARM], CurrentScale[IKScale.LEFT_ARM]);
                ScaleIKTarget(IKTarget.BODY, FindTargetTransform(IKTarget.LEFT_SHOULDER).localPosition, PriorLocations[IKTarget.LEFT_SHOULDER], PriorLocations[IKTarget.BODY], PriorScale[IKScale.BODY], CurrentScale[IKScale.BODY]);

                AdjustBody(FindTargetTransform(IKTarget.BODY), new IKTarget[] { IKTarget.LEFT_SHOULDER }, PriorLocations);

                AdjustChain(IKChain.RIGHT_ARM, PriorLocations);
                AdjustChain(IKChain.RIGHT_LEG, PriorLocations);
                AdjustChain(IKChain.LEFT_LEG, PriorLocations);
            }
            else if (centroid == IKResizeCentroid.HAND_RIGHT)
            {
                baselineLocalPosition = FindTargetTransform(IKTarget.RIGHT_HAND).localPosition;

                if (!SkipChain(IKChain.RIGHT_ARM))
                {
                    ScaleIKTarget(IKTarget.RIGHT_ELBOW, baselineLocalPosition, PriorLocations[IKTarget.RIGHT_HAND], PriorLocations[IKTarget.RIGHT_ELBOW], PriorScale[IKScale.RIGHT_LOWER_ARM], CurrentScale[IKScale.RIGHT_LOWER_ARM]);                    
                }

                ScaleIKTarget(IKTarget.RIGHT_SHOULDER, baselineLocalPosition, PriorLocations[IKTarget.RIGHT_HAND], PriorLocations[IKTarget.RIGHT_SHOULDER], PriorScale[IKScale.RIGHT_ARM], CurrentScale[IKScale.RIGHT_ARM]);
                ScaleIKTarget(IKTarget.BODY, FindTargetTransform(IKTarget.RIGHT_SHOULDER).localPosition, PriorLocations[IKTarget.RIGHT_SHOULDER], PriorLocations[IKTarget.BODY], PriorScale[IKScale.BODY], CurrentScale[IKScale.BODY]);

                AdjustBody(FindTargetTransform(IKTarget.BODY), new IKTarget[] { IKTarget.RIGHT_SHOULDER }, PriorLocations);

                AdjustChain(IKChain.LEFT_ARM, PriorLocations);
                AdjustChain(IKChain.LEFT_LEG, PriorLocations);
                AdjustChain(IKChain.RIGHT_LEG, PriorLocations);

            }
            else
            {
                baselineLocalPosition = Vector3.Lerp(FindTargetTransform(IKTarget.LEFT_HAND).localPosition, FindTargetTransform(IKTarget.RIGHT_HAND).localPosition, .5f);
                Vector3 priorBaselineLocalPosition = Vector3.Lerp(PriorLocations[IKTarget.LEFT_HAND], PriorLocations[IKTarget.RIGHT_HAND], .5f);

                if (!SkipChain(IKChain.LEFT_ARM))
                {
                    ScaleIKTarget(IKTarget.LEFT_HAND, baselineLocalPosition, priorBaselineLocalPosition, PriorLocations[IKTarget.LEFT_HAND], PriorScale[IKScale.LEFT_ARM], CurrentScale[IKScale.LEFT_ARM]);
                    ScaleIKTarget(IKTarget.LEFT_ELBOW, baselineLocalPosition, priorBaselineLocalPosition, PriorLocations[IKTarget.LEFT_ELBOW], PriorScale[IKScale.LEFT_LOWER_ARM], CurrentScale[IKScale.LEFT_LOWER_ARM]);                    
                }

                if (!SkipChain(IKChain.RIGHT_ARM))
                {
                    ScaleIKTarget(IKTarget.RIGHT_HAND, baselineLocalPosition, priorBaselineLocalPosition, PriorLocations[IKTarget.RIGHT_HAND], PriorScale[IKScale.RIGHT_ARM], CurrentScale[IKScale.RIGHT_ARM]);
                    ScaleIKTarget(IKTarget.RIGHT_ELBOW, baselineLocalPosition, priorBaselineLocalPosition, PriorLocations[IKTarget.RIGHT_ELBOW], PriorScale[IKScale.RIGHT_LOWER_ARM], CurrentScale[IKScale.RIGHT_LOWER_ARM]);                    
                }

                ScaleIKTarget(IKTarget.LEFT_SHOULDER, baselineLocalPosition, priorBaselineLocalPosition, PriorLocations[IKTarget.LEFT_SHOULDER], PriorScale[IKScale.LEFT_ARM], CurrentScale[IKScale.LEFT_ARM]);
                ScaleIKTarget(IKTarget.RIGHT_SHOULDER, baselineLocalPosition, priorBaselineLocalPosition, PriorLocations[IKTarget.RIGHT_SHOULDER], PriorScale[IKScale.RIGHT_ARM], CurrentScale[IKScale.RIGHT_ARM]);

                Vector3 shoulderBaseline = Vector3.Lerp(FindTargetTransform(IKTarget.LEFT_SHOULDER).localPosition, FindTargetTransform(IKTarget.RIGHT_SHOULDER).localPosition, 0.5f);
                Vector3 priorShoulderBaseline = Vector3.Lerp(PriorLocations[IKTarget.LEFT_SHOULDER], PriorLocations[IKTarget.RIGHT_SHOULDER], .5f);
                ScaleIKTarget(IKTarget.BODY, shoulderBaseline, priorShoulderBaseline, PriorLocations[IKTarget.BODY], PriorScale[IKScale.BODY], CurrentScale[IKScale.BODY]);

                AdjustBody(FindTargetTransform(IKTarget.BODY), new IKTarget[] { IKTarget.LEFT_SHOULDER, IKTarget.RIGHT_SHOULDER }, PriorLocations);
                AdjustChain(IKChain.LEFT_LEG, PriorLocations);
                AdjustChain(IKChain.RIGHT_LEG, PriorLocations);

            }
        }

        private void ApplyKneeCentroidAdjustment(IKResizeCentroid centroid)
        {
            // From knee to foot
            // From knee to thigh
            // then thigh to body
            // then chain out from body

            Dictionary<IKTarget, Vector3> PriorLocations = FillPriorLocations();
            Vector3 baselineLocalPosition;

            if (centroid == IKResizeCentroid.KNEE_LEFT)
            {
                baselineLocalPosition = FindTargetTransform(IKTarget.LEFT_KNEE).localPosition;
                if (!SkipChain(IKChain.LEFT_LEG))
                {
                    ScaleIKTarget(IKTarget.LEFT_FOOT, baselineLocalPosition, PriorLocations[IKTarget.LEFT_KNEE], PriorLocations[IKTarget.LEFT_FOOT], PriorScale[IKScale.LEFT_LOWER_LEG], CurrentScale[IKScale.LEFT_LOWER_LEG]);                    
                }

                ScaleIKTarget(IKTarget.LEFT_THIGH, baselineLocalPosition, PriorLocations[IKTarget.LEFT_KNEE], PriorLocations[IKTarget.LEFT_THIGH], PriorScale[IKScale.LEFT_UPPER_LEG], CurrentScale[IKScale.LEFT_UPPER_LEG]);
                ScaleIKTarget(IKTarget.BODY, FindTargetTransform(IKTarget.LEFT_THIGH).localPosition, PriorLocations[IKTarget.LEFT_THIGH], PriorLocations[IKTarget.BODY], PriorScale[IKScale.BODY], CurrentScale[IKScale.BODY]);

                AdjustBody(FindTargetTransform(IKTarget.BODY), new IKTarget[] { IKTarget.LEFT_THIGH }, PriorLocations);

                AdjustChain(IKChain.RIGHT_LEG, PriorLocations);
                AdjustChain(IKChain.LEFT_ARM, PriorLocations);
                AdjustChain(IKChain.RIGHT_ARM, PriorLocations);
            }
            else if (centroid == IKResizeCentroid.KNEE_RIGHT)
            {
                baselineLocalPosition = FindTargetTransform(IKTarget.RIGHT_KNEE).localPosition;
                if (!SkipChain(IKChain.RIGHT_LEG))
                {
                    ScaleIKTarget(IKTarget.RIGHT_FOOT, baselineLocalPosition, PriorLocations[IKTarget.RIGHT_KNEE], PriorLocations[IKTarget.RIGHT_FOOT], PriorScale[IKScale.RIGHT_LOWER_LEG], CurrentScale[IKScale.RIGHT_LOWER_LEG]);
                }

                ScaleIKTarget(IKTarget.RIGHT_THIGH, baselineLocalPosition, PriorLocations[IKTarget.RIGHT_KNEE], PriorLocations[IKTarget.RIGHT_THIGH], PriorScale[IKScale.RIGHT_UPPER_LEG], CurrentScale[IKScale.RIGHT_UPPER_LEG]);
                ScaleIKTarget(IKTarget.BODY, FindTargetTransform(IKTarget.RIGHT_THIGH).localPosition, PriorLocations[IKTarget.RIGHT_THIGH], PriorLocations[IKTarget.BODY], PriorScale[IKScale.BODY], CurrentScale[IKScale.BODY]);

                AdjustBody(FindTargetTransform(IKTarget.BODY), new IKTarget[] { IKTarget.RIGHT_THIGH }, PriorLocations);

                AdjustChain(IKChain.LEFT_LEG, PriorLocations);
                AdjustChain(IKChain.LEFT_ARM, PriorLocations);
                AdjustChain(IKChain.RIGHT_ARM, PriorLocations);
            }
            else
            {
                baselineLocalPosition = Vector3.Lerp(FindTargetTransform(IKTarget.LEFT_KNEE).localPosition, FindTargetTransform(IKTarget.RIGHT_KNEE).localPosition, .5f);
                Vector3 priorBaselineLocalPosition = Vector3.Lerp(PriorLocations[IKTarget.LEFT_KNEE], PriorLocations[IKTarget.RIGHT_KNEE], .5f);

                if (!SkipChain(IKChain.LEFT_LEG))
                {
                    ScaleIKTarget(IKTarget.LEFT_FOOT, baselineLocalPosition, priorBaselineLocalPosition, PriorLocations[IKTarget.LEFT_FOOT], PriorScale[IKScale.LEFT_LOWER_LEG], CurrentScale[IKScale.LEFT_LOWER_LEG]);
                }
                if (!SkipChain(IKChain.RIGHT_LEG))
                {
                    ScaleIKTarget(IKTarget.RIGHT_FOOT, baselineLocalPosition, priorBaselineLocalPosition, PriorLocations[IKTarget.RIGHT_FOOT], PriorScale[IKScale.RIGHT_LOWER_LEG], CurrentScale[IKScale.RIGHT_LOWER_LEG]);
                }
                ScaleIKTarget(IKTarget.LEFT_THIGH, baselineLocalPosition, priorBaselineLocalPosition, PriorLocations[IKTarget.LEFT_THIGH], PriorScale[IKScale.LEFT_UPPER_LEG], CurrentScale[IKScale.LEFT_UPPER_LEG]);
                ScaleIKTarget(IKTarget.RIGHT_THIGH, baselineLocalPosition, priorBaselineLocalPosition, PriorLocations[IKTarget.RIGHT_THIGH], PriorScale[IKScale.RIGHT_UPPER_LEG], CurrentScale[IKScale.RIGHT_UPPER_LEG]);
                Vector3 thighBaseline = Vector3.Lerp(FindTargetTransform(IKTarget.LEFT_THIGH).localPosition, FindTargetTransform(IKTarget.RIGHT_THIGH).localPosition, 0.5f);
                Vector3 priorThighBaseline = Vector3.Lerp(PriorLocations[IKTarget.LEFT_THIGH], PriorLocations[IKTarget.RIGHT_THIGH], .5f);
                ScaleIKTarget(IKTarget.BODY, thighBaseline, priorThighBaseline, PriorLocations[IKTarget.BODY], PriorScale[IKScale.BODY], CurrentScale[IKScale.BODY]);

                AdjustBody(FindTargetTransform(IKTarget.BODY), new IKTarget[] { IKTarget.LEFT_THIGH, IKTarget.RIGHT_THIGH }, PriorLocations);
                AdjustChain(IKChain.LEFT_ARM, PriorLocations);
                AdjustChain(IKChain.RIGHT_ARM, PriorLocations);
            }

        }

        private void ApplyFeetCentroidAdjustment(IKResizeCentroid centroid)
        {
            // From feet up to thigh
            // thigh to body
            // then chain out from body

            Dictionary<IKTarget, Vector3> PriorLocations = FillPriorLocations();
            Vector3 baselineLocalPosition;
            if (centroid == IKResizeCentroid.FEET_LEFT)
            {
                baselineLocalPosition = FindTargetTransform(IKTarget.LEFT_FOOT).localPosition;

                if (!SkipChain(IKChain.LEFT_LEG))
                {
                    ScaleIKTarget(IKTarget.LEFT_KNEE, baselineLocalPosition, PriorLocations[IKTarget.LEFT_FOOT], PriorLocations[IKTarget.LEFT_KNEE], PriorScale[IKScale.LEFT_LOWER_LEG], CurrentScale[IKScale.LEFT_LOWER_LEG]);                    
                }

                ScaleIKTarget(IKTarget.LEFT_THIGH, baselineLocalPosition, PriorLocations[IKTarget.LEFT_FOOT], PriorLocations[IKTarget.LEFT_THIGH], PriorScale[IKScale.LEFT_LEG], CurrentScale[IKScale.LEFT_LEG]);
                ScaleIKTarget(IKTarget.BODY, FindTargetTransform(IKTarget.LEFT_THIGH).localPosition, PriorLocations[IKTarget.LEFT_THIGH], PriorLocations[IKTarget.BODY], PriorScale[IKScale.BODY], CurrentScale[IKScale.BODY]);

                AdjustBody(FindTargetTransform(IKTarget.BODY), new IKTarget[] { IKTarget.LEFT_THIGH }, PriorLocations);

                AdjustChain(IKChain.RIGHT_LEG, PriorLocations);
                AdjustChain(IKChain.LEFT_ARM, PriorLocations);
                AdjustChain(IKChain.RIGHT_ARM, PriorLocations);

            }
            else if (centroid == IKResizeCentroid.FEET_RIGHT)
            {
                baselineLocalPosition = FindTargetTransform(IKTarget.RIGHT_FOOT).localPosition;

                if (!SkipChain(IKChain.RIGHT_LEG))
                {
                    ScaleIKTarget(IKTarget.RIGHT_KNEE, baselineLocalPosition, PriorLocations[IKTarget.RIGHT_FOOT], PriorLocations[IKTarget.RIGHT_KNEE], PriorScale[IKScale.RIGHT_LOWER_LEG], CurrentScale[IKScale.RIGHT_LOWER_LEG]);                    
                }

                ScaleIKTarget(IKTarget.RIGHT_THIGH, baselineLocalPosition, PriorLocations[IKTarget.RIGHT_FOOT], PriorLocations[IKTarget.RIGHT_THIGH], PriorScale[IKScale.RIGHT_LEG], CurrentScale[IKScale.RIGHT_LEG]);
                ScaleIKTarget(IKTarget.BODY, FindTargetTransform(IKTarget.RIGHT_THIGH).localPosition, PriorLocations[IKTarget.RIGHT_THIGH], PriorLocations[IKTarget.BODY], PriorScale[IKScale.BODY], CurrentScale[IKScale.BODY]);

                AdjustBody(FindTargetTransform(IKTarget.BODY), new IKTarget[] { IKTarget.RIGHT_THIGH }, PriorLocations);

                AdjustChain(IKChain.LEFT_LEG, PriorLocations);
                AdjustChain(IKChain.LEFT_ARM, PriorLocations);
                AdjustChain(IKChain.RIGHT_ARM, PriorLocations);

            }
            else
            {
                baselineLocalPosition = Vector3.Lerp(FindTargetTransform(IKTarget.LEFT_FOOT).localPosition, FindTargetTransform(IKTarget.RIGHT_FOOT).localPosition, .5f);
                Vector3 priorBaselineLocalPosition = Vector3.Lerp(PriorLocations[IKTarget.LEFT_FOOT], PriorLocations[IKTarget.RIGHT_FOOT], .5f);

                if (!SkipChain(IKChain.LEFT_LEG))
                {
                    ScaleIKTarget(IKTarget.LEFT_FOOT, baselineLocalPosition, priorBaselineLocalPosition, PriorLocations[IKTarget.LEFT_FOOT], PriorScale[IKScale.LEFT_LEG], CurrentScale[IKScale.LEFT_LEG]);
                    ScaleIKTarget(IKTarget.LEFT_KNEE, baselineLocalPosition, priorBaselineLocalPosition, PriorLocations[IKTarget.LEFT_KNEE], PriorScale[IKScale.LEFT_LOWER_LEG], CurrentScale[IKScale.LEFT_LOWER_LEG]);                    
                }

                if (!SkipChain(IKChain.RIGHT_LEG))
                {
                    ScaleIKTarget(IKTarget.RIGHT_FOOT, baselineLocalPosition, priorBaselineLocalPosition, PriorLocations[IKTarget.RIGHT_FOOT], PriorScale[IKScale.RIGHT_LEG], CurrentScale[IKScale.RIGHT_LEG]);
                    ScaleIKTarget(IKTarget.RIGHT_KNEE, baselineLocalPosition, priorBaselineLocalPosition, PriorLocations[IKTarget.RIGHT_KNEE], PriorScale[IKScale.RIGHT_LOWER_LEG], CurrentScale[IKScale.RIGHT_LOWER_LEG]);
                    
                }
                ScaleIKTarget(IKTarget.LEFT_THIGH, baselineLocalPosition, priorBaselineLocalPosition, PriorLocations[IKTarget.LEFT_THIGH], PriorScale[IKScale.LEFT_LEG], CurrentScale[IKScale.LEFT_LEG]);
                ScaleIKTarget(IKTarget.RIGHT_THIGH, baselineLocalPosition, priorBaselineLocalPosition, PriorLocations[IKTarget.RIGHT_THIGH], PriorScale[IKScale.RIGHT_LEG], CurrentScale[IKScale.RIGHT_LEG]);

                Vector3 thighBaseline = Vector3.Lerp(FindTargetTransform(IKTarget.LEFT_THIGH).localPosition, FindTargetTransform(IKTarget.RIGHT_THIGH).localPosition, 0.5f);
                Vector3 priorThighBaseline = Vector3.Lerp(PriorLocations[IKTarget.LEFT_THIGH], PriorLocations[IKTarget.RIGHT_THIGH], .5f);
                ScaleIKTarget(IKTarget.BODY, thighBaseline, priorThighBaseline, PriorLocations[IKTarget.BODY], PriorScale[IKScale.BODY], CurrentScale[IKScale.BODY]);

                AdjustBody(FindTargetTransform(IKTarget.BODY), new IKTarget[] { IKTarget.LEFT_THIGH, IKTarget.RIGHT_THIGH }, PriorLocations);
                AdjustChain(IKChain.LEFT_ARM, PriorLocations);
                AdjustChain(IKChain.RIGHT_ARM, PriorLocations);

            }
        }

        private void ApplyBodyCentroidAdjustment()
        {
            // Simplest of the adjustments, all body nodes from body and then chain out

            Dictionary<IKTarget, Vector3> PriorLocations = FillPriorLocations();
            Transform body = FindTargetTransform(IKTarget.BODY);

            // Scale Central Body from Body
            AdjustBody(body, new IKTarget[] { }, PriorLocations);

            AdjustChain(IKChain.LEFT_ARM, PriorLocations);
            AdjustChain(IKChain.RIGHT_ARM, PriorLocations);
            AdjustChain(IKChain.LEFT_LEG, PriorLocations);
            AdjustChain(IKChain.RIGHT_LEG, PriorLocations);

        }

        private void AdjustBody(Transform body, IKTarget[] exceptions, Dictionary<IKTarget, Vector3> priorLocations)
        {
            foreach (IKTarget target in IKChainTargets[IKChain.BODY])
            {
                if (exceptions.Contains(target))
                {
                    continue;
                }

                ScaleIKTarget(target, body.localPosition, priorLocations[IKTarget.BODY], priorLocations[target], PriorScale[IKScale.BODY], CurrentScale[IKScale.BODY]);
            }
        }

        private bool SkipChain(IKChain chain)
        {
            bool found = ChainAdjustments.TryGetValue(chain, out IKResizeChainAdjustment adjustMethod);
            if (found && adjustMethod == IKResizeChainAdjustment.OFF)
                return true;
            else
                return false;
        }

        private void AdjustChain(IKChain chain, Dictionary<IKTarget, Vector3> priorLocations)
        {
            bool found = ChainAdjustments.TryGetValue(chain, out IKResizeChainAdjustment adjustmentMethod);
            if (!found)
            {
                adjustmentMethod = IKResizeChainAdjustment.CHAIN;
            }
            else if (adjustmentMethod == IKResizeChainAdjustment.OFF)
            {
                return;
            }
#if DEBUG
            Log.LogInfo($"Adjusting Chain {chain} Using Method {adjustmentMethod}");
#endif

            Transform baseline;
            Vector3 priorBaselineLocation;
            baseline = FindTargetTransform(IKChainBases[chain]);
            priorBaselineLocation = priorLocations[IKChainBases[chain]];

            bool first = true;
            foreach (IKTarget target in IKChainTargets[chain])
            {
                ScaleIKTarget(target, baseline.localPosition, priorBaselineLocation, priorLocations[target], PriorScale[first ? IKChainScale[chain][0] : IKChainScale[chain][1]], CurrentScale[first ? IKChainScale[chain][0] : IKChainScale[chain][1]]);
                first = false;
            }
        }

        private void ScaleIKTarget(IKTarget target, Vector3 baselineLocalPosition, Vector3 priorBaselineLocalPosition, Vector3 priorLocalPosition, float previousScale, float newScale)
        {
            Transform targetTransform = FindTargetTransform(target);

            OCIChar.IKInfo ikInfo = FindIKInfo(target);

            Vector3 priorDirection = priorLocalPosition - priorBaselineLocalPosition;

            targetTransform.localPosition = baselineLocalPosition + (priorDirection * (newScale / previousScale));
            ikInfo.guideObject.changeAmount.pos = baselineLocalPosition + (priorDirection * (newScale / previousScale));
#if DEBUG
            Log.LogInfo($"Adjusting {targetTransform.name} Prev Scale {previousScale} New Scale {newScale} Baseline {priorBaselineLocalPosition} -> {baselineLocalPosition} Direction {priorDirection} Adjusted {priorLocalPosition} -> {targetTransform.localPosition}");
#endif
        }

        public IKResizeCentroid AutodetectResizeMode(ChaControl chaControl)
        {
            // Get Shoulder Centroid
            Vector3 shoulderCentroid = Vector3.Lerp(FindTargetTransform(IKTarget.LEFT_SHOULDER).localPosition, FindTargetTransform(IKTarget.RIGHT_SHOULDER).localPosition, .5f);

            // Get Thigh Centroid
            Vector3 thighCentroid = Vector3.Lerp(FindTargetTransform(IKTarget.LEFT_THIGH).localPosition, FindTargetTransform(IKTarget.RIGHT_THIGH).localPosition, .5f);

            // Get Feet Centroid
            Vector3 feetCentroid = Vector3.Lerp(FindTargetTransform(IKTarget.LEFT_FOOT).localPosition, FindTargetTransform(IKTarget.RIGHT_FOOT).localPosition, .5f);

            Vector3 shoulderToThigh = thighCentroid - shoulderCentroid;
            Vector3 thighToFeet = feetCentroid - thighCentroid;
            Vector3 shoulderToFeet = feetCentroid - shoulderCentroid;

            float shoulderToFeetVariFromDown = Vector3.Angle(shoulderToFeet, Vector3.down);
            float shoulderToThighVarFromDown = Vector3.Angle(shoulderToThigh, Vector3.down);
            float thighToFeetVarFromDown = Vector3.Angle(thighToFeet, Vector3.down);

#if DEBUG
            Log.LogInfo($"StF {shoulderToFeetVariFromDown} StT {shoulderToThighVarFromDown} TtF {thighToFeetVarFromDown}");
#endif

            // Shoulder/Thigh/Feet In line - Use Standing (FEET_CENTER)
            if (shoulderToThighVarFromDown < 10 && thighToFeetVarFromDown < 10 && shoulderToFeetVariFromDown < 10)
            {
                // Determine Foot
                Vector3 footLine = FindTargetTransform(IKTarget.LEFT_FOOT).localPosition - FindTargetTransform(IKTarget.RIGHT_FOOT).localPosition;
                float footVarRight = Vector3.Angle(footLine, Vector3.right);
                float footVarLeft = Vector3.Angle(footLine, Vector3.left);
#if DEBUG
                Log.LogInfo($"Foot Var L {footVarLeft} R {footVarRight}");
#endif

                if (footVarLeft < 10 || footVarRight < 10)
                {
                    return IKResizeCentroid.FEET_CENTER;
                }
                else if (FindTargetTransform(IKTarget.LEFT_FOOT).localPosition.y < FindTargetTransform(IKTarget.RIGHT_FOOT).localPosition.y)
                {
                    return IKResizeCentroid.FEET_LEFT;
                }
                else
                {
                    return IKResizeCentroid.FEET_RIGHT;
                }
            }

            // Shoulder/Thigh In Line - Use Sitting (THIGH_CENTER)
            if (shoulderToThighVarFromDown < 10)
            {
                // Use Thigh
                Vector3 thighLine = FindTargetTransform(IKTarget.LEFT_THIGH).localPosition - FindTargetTransform(IKTarget.RIGHT_THIGH).localPosition;
                float thighVarRight = Vector3.Angle(thighLine, Vector3.right);
                float thighVarLeft = Vector3.Angle(thighLine, Vector3.left);
#if DEBUG
                Log.LogInfo($"Foot Var L {thighVarLeft} R {thighVarRight}");
#endif

                if (thighVarLeft < 10 || thighVarRight < 10)
                {
                    return IKResizeCentroid.THIGH_CENTER;
                }
                else if (FindTargetTransform(IKTarget.LEFT_THIGH).localPosition.y < FindTargetTransform(IKTarget.RIGHT_THIGH).localPosition.y)
                {
                    return IKResizeCentroid.THIGH_LEFT;
                }
                else
                {
                    return IKResizeCentroid.THIGH_RIGHT;
                }
            }

            if (thighToFeetVarFromDown < 10)
            {
                // Determine Foot
                Vector3 footLine = FindTargetTransform(IKTarget.LEFT_FOOT).localPosition - FindTargetTransform(IKTarget.RIGHT_FOOT).localPosition;
                float footVarRight = Vector3.Angle(footLine, Vector3.right);
                float footVarLeft = Vector3.Angle(footLine, Vector3.left);
#if DEBUG
                Log.LogInfo($"Foot Var L {footVarLeft} R {footVarRight}");
#endif

                if (footVarLeft < 10 || footVarRight < 10)
                {
                    return IKResizeCentroid.FEET_CENTER;
                }
                else if (FindTargetTransform(IKTarget.LEFT_FOOT).localPosition.y < FindTargetTransform(IKTarget.RIGHT_FOOT).localPosition.y)
                {
                    return IKResizeCentroid.FEET_LEFT;
                }
                else
                {
                    return IKResizeCentroid.FEET_RIGHT;
                }
            }

            // Else use Body
            return IKResizeCentroid.BODY;
        }

        public void SaveConfig(PluginData data)
        {
            data.data["ResizeCentroid"] = Centroid;
            data.data["ResizeChainAdjustments"] = MessagePack.MessagePackSerializer.Serialize<Dictionary<IKChain, IKResizeChainAdjustment>>(ChainAdjustments);
        }

        public void LoadConfig(PluginData data)
        {
            if (data.data.TryGetValue("ResizeCentroid", out object resizeCentroidValue)) Centroid = (IKResizeCentroid)resizeCentroidValue;
            if (data.data.TryGetValue("ResizeChainAdjustments", out object resizeChainAdjustmentsValue))
            {
                ChainAdjustments = MessagePack.MessagePackSerializer.Deserialize<Dictionary<IKChain, IKResizeChainAdjustment>>((byte[])resizeChainAdjustmentsValue);
            }

#if DEBUG
            Log.LogInfo($"Loading Centroid {Centroid} Chain Adjustments {ChainAdjustments} for Character {Self?.charInfo.fileParam.fullname}");
#endif
        }


        private void PopulateCurrentScale()
        {
            foreach (IKScale scale in Enum.GetValues(typeof(IKScale)))
            {
                Log.LogInfo($"Looking up scale: {scale}");
                CurrentScale[scale] = FindChainScale(scale);
#if DEBUG
                Log.LogInfo($"Scale {scale} is {CurrentScale[scale]}");
#endif
            }
        }

        private Dictionary<IKTarget, Vector3> FillPriorLocations()
        {
            Dictionary<IKTarget, Vector3> priorLocations = new Dictionary<IKTarget, Vector3>();
            foreach (IKTarget target in Enum.GetValues(typeof(IKTarget)))
            {
                priorLocations[target] = FindTargetTransform(target).localPosition;
            }

            return priorLocations;
        }

        private Vector3 FindCurrentScale()
        {
#if KOIKATSU
            return Self.charInfo.objAnim.transform.Find("cf_j_root/cf_n_height").localScale;
#else
            return Self.charInfo.objAnim.transform.Find("cf_J_Root/cf_N_height").localScale;
#endif
        }

        private float FindChainScale(IKScale scale)
        {
            switch (scale)
            {
#if KOIKATSU
                case IKScale.LEFT_UPPER_LEG:
                    return Vector3.Distance(Self.charInfo.objAnim.transform.Find("cf_j_root/cf_n_height/cf_j_hips/cf_j_waist01/cf_j_waist02/cf_j_thigh00_L").transform.position, Self.charInfo.objAnim.transform.Find("cf_j_root/cf_n_height/cf_j_hips/cf_j_waist01/cf_j_waist02/cf_j_thigh00_L/cf_j_leg01_L").transform.position);
                case IKScale.LEFT_LOWER_LEG:
                    return Vector3.Distance(Self.charInfo.objAnim.transform.Find("cf_j_root/cf_n_height/cf_j_hips/cf_j_waist01/cf_j_waist02/cf_j_thigh00_L/cf_j_leg01_L/cf_j_leg03_L/cf_j_foot_L").transform.position, Self.charInfo.objAnim.transform.Find("cf_j_root/cf_n_height/cf_j_hips/cf_j_waist01/cf_j_waist02/cf_j_thigh00_L/cf_j_leg01_L").transform.position);
                case IKScale.LEFT_LEG:
                    return Vector3.Distance(Self.charInfo.objAnim.transform.Find("cf_j_root/cf_n_height/cf_j_hips/cf_j_waist01/cf_j_waist02/cf_j_thigh00_L").transform.position, Self.charInfo.objAnim.transform.Find("cf_j_root/cf_n_height/cf_j_hips/cf_j_waist01/cf_j_waist02/cf_j_thigh00_L/cf_j_leg01_L").transform.position) + Vector3.Distance(Self.charInfo.objAnim.transform.Find("cf_j_root/cf_n_height/cf_j_hips/cf_j_waist01/cf_j_waist02/cf_j_thigh00_L/cf_j_leg01_L/cf_j_leg03_L/cf_j_foot_L").transform.position, Self.charInfo.objAnim.transform.Find("cf_j_root/cf_n_height/cf_j_hips/cf_j_waist01/cf_j_waist02/cf_j_thigh00_L/cf_j_leg01_L").transform.position);
                case IKScale.RIGHT_UPPER_LEG:
                    return Vector3.Distance(Self.charInfo.objAnim.transform.Find("cf_j_root/cf_n_height/cf_j_hips/cf_j_waist01/cf_j_waist02/cf_j_thigh00_R").transform.position, Self.charInfo.objAnim.transform.Find("cf_j_root/cf_n_height/cf_j_hips/cf_j_waist01/cf_j_waist02/cf_j_thigh00_R/cf_j_leg01_R").transform.position);
                case IKScale.RIGHT_LOWER_LEG:
                    return Vector3.Distance(Self.charInfo.objAnim.transform.Find("cf_j_root/cf_n_height/cf_j_hips/cf_j_waist01/cf_j_waist02/cf_j_thigh00_R/cf_j_leg01_R/cf_j_leg03_R/cf_j_foot_R").transform.position, Self.charInfo.objAnim.transform.Find("cf_j_root/cf_n_height/cf_j_hips/cf_j_waist01/cf_j_waist02/cf_j_thigh00_R/cf_j_leg01_R").transform.position);
                case IKScale.RIGHT_LEG:
                    return Vector3.Distance(Self.charInfo.objAnim.transform.Find("cf_j_root/cf_n_height/cf_j_hips/cf_j_waist01/cf_j_waist02/cf_j_thigh00_R").transform.position, Self.charInfo.objAnim.transform.Find("cf_j_root/cf_n_height/cf_j_hips/cf_j_waist01/cf_j_waist02/cf_j_thigh00_R/cf_j_leg01_R").transform.position) + Vector3.Distance(Self.charInfo.objAnim.transform.Find("cf_j_root/cf_n_height/cf_j_hips/cf_j_waist01/cf_j_waist02/cf_j_thigh00_R/cf_j_leg01_R/cf_j_leg03_R/cf_j_foot_R").transform.position, Self.charInfo.objAnim.transform.Find("cf_j_root/cf_n_height/cf_j_hips/cf_j_waist01/cf_j_waist02/cf_j_thigh00_R/cf_j_leg01_R").transform.position);
                case IKScale.LEFT_UPPER_ARM:
                    return Vector3.Distance(Self.charInfo.objAnim.transform.Find("cf_j_root/cf_n_height/cf_j_hips/cf_j_spine01/cf_j_spine02/cf_j_spine03/cf_d_shoulder_L/cf_j_shoulder_L/cf_j_arm00_L").transform.position, Self.charInfo.objAnim.transform.Find("cf_j_root/cf_n_height/cf_j_hips/cf_j_spine01/cf_j_spine02/cf_j_spine03/cf_d_shoulder_L/cf_j_shoulder_L/cf_j_arm00_L/cf_j_forearm01_L").transform.position);
                case IKScale.LEFT_LOWER_ARM:
                    return Vector3.Distance(Self.charInfo.objAnim.transform.Find("cf_j_root/cf_n_height/cf_j_hips/cf_j_spine01/cf_j_spine02/cf_j_spine03/cf_d_shoulder_L/cf_j_shoulder_L/cf_j_arm00_L/cf_j_forearm01_L/cf_j_hand_L").transform.position, Self.charInfo.objAnim.transform.Find("cf_j_root/cf_n_height/cf_j_hips/cf_j_spine01/cf_j_spine02/cf_j_spine03/cf_d_shoulder_L/cf_j_shoulder_L/cf_j_arm00_L/cf_j_forearm01_L").transform.position);
                case IKScale.LEFT_ARM:
                    return Vector3.Distance(Self.charInfo.objAnim.transform.Find("cf_j_root/cf_n_height/cf_j_hips/cf_j_spine01/cf_j_spine02/cf_j_spine03/cf_d_shoulder_L/cf_j_shoulder_L/cf_j_arm00_L").transform.position, Self.charInfo.objAnim.transform.Find("cf_j_root/cf_n_height/cf_j_hips/cf_j_spine01/cf_j_spine02/cf_j_spine03/cf_d_shoulder_L/cf_j_shoulder_L/cf_j_arm00_L/cf_j_forearm01_L").transform.position) + Vector3.Distance(Self.charInfo.objAnim.transform.Find("cf_j_root/cf_n_height/cf_j_hips/cf_j_spine01/cf_j_spine02/cf_j_spine03/cf_d_shoulder_L/cf_j_shoulder_L/cf_j_arm00_L/cf_j_forearm01_L/cf_j_hand_L").transform.position, Self.charInfo.objAnim.transform.Find("cf_j_root/cf_n_height/cf_j_hips/cf_j_spine01/cf_j_spine02/cf_j_spine03/cf_d_shoulder_L/cf_j_shoulder_L/cf_j_arm00_L/cf_j_forearm01_L").transform.position);
                case IKScale.RIGHT_UPPER_ARM:
                    return Vector3.Distance(Self.charInfo.objAnim.transform.Find("cf_j_root/cf_n_height/cf_j_hips/cf_j_spine01/cf_j_spine02/cf_j_spine03/cf_d_shoulder_R/cf_j_shoulder_R/cf_j_arm00_R").transform.position, Self.charInfo.objAnim.transform.Find("cf_j_root/cf_n_height/cf_j_hips/cf_j_spine01/cf_j_spine02/cf_j_spine03/cf_d_shoulder_R/cf_j_shoulder_R/cf_j_arm00_R/cf_j_forearm01_R").transform.position);
                case IKScale.RIGHT_LOWER_ARM:
                    return Vector3.Distance(Self.charInfo.objAnim.transform.Find("cf_j_root/cf_n_height/cf_j_hips/cf_j_spine01/cf_j_spine02/cf_j_spine03/cf_d_shoulder_R/cf_j_shoulder_R/cf_j_arm00_R/cf_j_forearm01_R/cf_j_hand_R").transform.position, Self.charInfo.objAnim.transform.Find("cf_j_root/cf_n_height/cf_j_hips/cf_j_spine01/cf_j_spine02/cf_j_spine03/cf_d_shoulder_R/cf_j_shoulder_R/cf_j_arm00_R/cf_j_forearm01_R").transform.position);
                case IKScale.RIGHT_ARM:
                    return Vector3.Distance(Self.charInfo.objAnim.transform.Find("cf_j_root/cf_n_height/cf_j_hips/cf_j_spine01/cf_j_spine02/cf_j_spine03/cf_d_shoulder_R/cf_j_shoulder_R/cf_j_arm00_R").transform.position, Self.charInfo.objAnim.transform.Find("cf_j_root/cf_n_height/cf_j_hips/cf_j_spine01/cf_j_spine02/cf_j_spine03/cf_d_shoulder_R/cf_j_shoulder_R/cf_j_arm00_R/cf_j_forearm01_R").transform.position) + Vector3.Distance(Self.charInfo.objAnim.transform.Find("cf_j_root/cf_n_height/cf_j_hips/cf_j_spine01/cf_j_spine02/cf_j_spine03/cf_d_shoulder_R/cf_j_shoulder_R/cf_j_arm00_R/cf_j_forearm01_R/cf_j_hand_R").transform.position, Self.charInfo.objAnim.transform.Find("cf_j_root/cf_n_height/cf_j_hips/cf_j_spine01/cf_j_spine02/cf_j_spine03/cf_d_shoulder_R/cf_j_shoulder_R/cf_j_arm00_R/cf_j_forearm01_R").transform.position);
#else
                case IKScale.LEFT_UPPER_LEG:
                    return Vector3.Distance(Self.charInfo.objAnim.transform.Find("cf_J_Root/cf_N_height/cf_J_Hips/cf_J_Kosi01/cf_J_Kosi02/cf_J_LegUp00_L").transform.position, Self.charInfo.objAnim.transform.Find("cf_J_Root/cf_N_height/cf_J_Hips/cf_J_Kosi01/cf_J_Kosi02/cf_J_LegUp00_L/cf_J_LegLow01_L").transform.position);
                case IKScale.LEFT_LOWER_LEG:
                    return Vector3.Distance(Self.charInfo.objAnim.transform.Find("cf_J_Root/cf_N_height/cf_J_Hips/cf_J_Kosi01/cf_J_Kosi02/cf_J_LegUp00_L/cf_J_LegLow01_L/cf_J_LegLowRoll_L/cf_J_Foot01_L").transform.position, Self.charInfo.objAnim.transform.Find("cf_J_Root/cf_N_height/cf_J_Hips/cf_J_Kosi01/cf_J_Kosi02/cf_J_LegUp00_L/cf_J_LegLow01_L").transform.position);
                case IKScale.LEFT_LEG:
                    return Vector3.Distance(Self.charInfo.objAnim.transform.Find("cf_J_Root/cf_N_height/cf_J_Hips/cf_J_Kosi01/cf_J_Kosi02/cf_J_LegUp00_L").transform.position, Self.charInfo.objAnim.transform.Find("cf_J_Root/cf_N_height/cf_J_Hips/cf_J_Kosi01/cf_J_Kosi02/cf_J_LegUp00_L/cf_J_LegLow01_L").transform.position) + Vector3.Distance(Self.charInfo.objAnim.transform.Find("cf_J_Root/cf_N_height/cf_J_Hips/cf_J_Kosi01/cf_J_Kosi02/cf_J_LegUp00_L/cf_J_LegLow01_L/cf_J_LegLowRoll_L/cf_J_Foot01_L").transform.position, Self.charInfo.objAnim.transform.Find("cf_J_Root/cf_N_height/cf_J_Hips/cf_J_Kosi01/cf_J_Kosi02/cf_J_LegUp00_L/cf_J_LegLow01_L").transform.position);
                case IKScale.RIGHT_UPPER_LEG:
                    return Vector3.Distance(Self.charInfo.objAnim.transform.Find("cf_J_Root/cf_N_height/cf_J_Hips/cf_J_Kosi01/cf_J_Kosi02/cf_J_LegUp00_R").transform.position, Self.charInfo.objAnim.transform.Find("cf_J_Root/cf_N_height/cf_J_Hips/cf_J_Kosi01/cf_J_Kosi02/cf_J_LegUp00_R/cf_J_LegLow01_R").transform.position);
                case IKScale.RIGHT_LOWER_LEG:
                    return Vector3.Distance(Self.charInfo.objAnim.transform.Find("cf_J_Root/cf_N_height/cf_J_Hips/cf_J_Kosi01/cf_J_Kosi02/cf_J_LegUp00_R/cf_J_LegLow01_R/cf_J_LegLowRoll_R/cf_J_Foot01_R").transform.position, Self.charInfo.objAnim.transform.Find("cf_J_Root/cf_N_height/cf_J_Hips/cf_J_Kosi01/cf_J_Kosi02/cf_J_LegUp00_R/cf_J_LegLow01_R").transform.position);
                case IKScale.RIGHT_LEG:
                    return Vector3.Distance(Self.charInfo.objAnim.transform.Find("cf_J_Root/cf_N_height/cf_J_Hips/cf_J_Kosi01/cf_J_Kosi02/cf_J_LegUp00_R").transform.position, Self.charInfo.objAnim.transform.Find("cf_J_Root/cf_N_height/cf_J_Hips/cf_J_Kosi01/cf_J_Kosi02/cf_J_LegUp00_R/cf_J_LegLow01_R").transform.position) + Vector3.Distance(Self.charInfo.objAnim.transform.Find("cf_J_Root/cf_N_height/cf_J_Hips/cf_J_Kosi01/cf_J_Kosi02/cf_J_LegUp00_R/cf_J_LegLow01_R/cf_J_LegLowRoll_R/cf_J_Foot01_R").transform.position, Self.charInfo.objAnim.transform.Find("cf_J_Root/cf_N_height/cf_J_Hips/cf_J_Kosi01/cf_J_Kosi02/cf_J_LegUp00_R/cf_J_LegLow01_R").transform.position);
                case IKScale.LEFT_UPPER_ARM:
                    return Vector3.Distance(Self.charInfo.objAnim.transform.Find("cf_J_Root/cf_N_height/cf_J_Hips/cf_J_Spine01/cf_J_Spine02/cf_J_Spine03/cf_J_ShoulderIK_L/cf_J_Shoulder_L/cf_J_ArmUp00_L").transform.position, Self.charInfo.objAnim.transform.Find("cf_J_Root/cf_N_height/cf_J_Hips/cf_J_Spine01/cf_J_Spine02/cf_J_Spine03/cf_J_ShoulderIK_L/cf_J_Shoulder_L/cf_J_ArmUp00_L/cf_J_ArmLow01_L").transform.position);
                case IKScale.LEFT_LOWER_ARM:
                    return Vector3.Distance(Self.charInfo.objAnim.transform.Find("cf_J_Root/cf_N_height/cf_J_Hips/cf_J_Spine01/cf_J_Spine02/cf_J_Spine03/cf_J_ShoulderIK_L/cf_J_Shoulder_L/cf_J_ArmUp00_L/cf_J_ArmLow01_L/cf_J_Hand_L").transform.position, Self.charInfo.objAnim.transform.Find("cf_J_Root/cf_N_height/cf_J_Hips/cf_J_Spine01/cf_J_Spine02/cf_J_Spine03/cf_J_ShoulderIK_L/cf_J_Shoulder_L/cf_J_ArmUp00_L/cf_J_ArmLow01_L").transform.position);
                case IKScale.LEFT_ARM:
                    return Vector3.Distance(Self.charInfo.objAnim.transform.Find("cf_J_Root/cf_N_height/cf_J_Hips/cf_J_Spine01/cf_J_Spine02/cf_J_Spine03/cf_J_ShoulderIK_L/cf_J_Shoulder_L/cf_J_ArmUp00_L").transform.position, Self.charInfo.objAnim.transform.Find("cf_J_Root/cf_N_height/cf_J_Hips/cf_J_Spine01/cf_J_Spine02/cf_J_Spine03/cf_J_ShoulderIK_L/cf_J_Shoulder_L/cf_J_ArmUp00_L/cf_J_ArmLow01_L").transform.position) + Vector3.Distance(Self.charInfo.objAnim.transform.Find("cf_J_Root/cf_N_height/cf_J_Hips/cf_J_Spine01/cf_J_Spine02/cf_J_Spine03/cf_J_ShoulderIK_L/cf_J_Shoulder_L/cf_J_ArmUp00_L/cf_J_ArmLow01_L/cf_J_Hand_L").transform.position, Self.charInfo.objAnim.transform.Find("cf_J_Root/cf_N_height/cf_J_Hips/cf_J_Spine01/cf_J_Spine02/cf_J_Spine03/cf_J_ShoulderIK_L/cf_J_Shoulder_L/cf_J_ArmUp00_L/cf_J_ArmLow01_L").transform.position);
                case IKScale.RIGHT_UPPER_ARM:
                    return Vector3.Distance(Self.charInfo.objAnim.transform.Find("cf_J_Root/cf_N_height/cf_J_Hips/cf_J_Spine01/cf_J_Spine02/cf_J_Spine03/cf_J_ShoulderIK_R/cf_J_Shoulder_R/cf_J_ArmUp00_R").transform.position, Self.charInfo.objAnim.transform.Find("cf_J_Root/cf_N_height/cf_J_Hips/cf_J_Spine01/cf_J_Spine02/cf_J_Spine03/cf_J_ShoulderIK_R/cf_J_Shoulder_R/cf_J_ArmUp00_R/cf_J_ArmLow01_R").transform.position);
                case IKScale.RIGHT_LOWER_ARM:
                    return Vector3.Distance(Self.charInfo.objAnim.transform.Find("cf_J_Root/cf_N_height/cf_J_Hips/cf_J_Spine01/cf_J_Spine02/cf_J_Spine03/cf_J_ShoulderIK_R/cf_J_Shoulder_R/cf_J_ArmUp00_R/cf_J_ArmLow01_R/cf_J_Hand_R").transform.position, Self.charInfo.objAnim.transform.Find("cf_J_Root/cf_N_height/cf_J_Hips/cf_J_Spine01/cf_J_Spine02/cf_J_Spine03/cf_J_ShoulderIK_R/cf_J_Shoulder_R/cf_J_ArmUp00_R/cf_J_ArmLow01_R").transform.position);
                case IKScale.RIGHT_ARM:
                    return Vector3.Distance(Self.charInfo.objAnim.transform.Find("cf_J_Root/cf_N_height/cf_J_Hips/cf_J_Spine01/cf_J_Spine02/cf_J_Spine03/cf_J_ShoulderIK_R/cf_J_Shoulder_R/cf_J_ArmUp00_R").transform.position, Self.charInfo.objAnim.transform.Find("cf_J_Root/cf_N_height/cf_J_Hips/cf_J_Spine01/cf_J_Spine02/cf_J_Spine03/cf_J_ShoulderIK_R/cf_J_Shoulder_R/cf_J_ArmUp00_R/cf_J_ArmLow01_R").transform.position) + Vector3.Distance(Self.charInfo.objAnim.transform.Find("cf_J_Root/cf_N_height/cf_J_Hips/cf_J_Spine01/cf_J_Spine02/cf_J_Spine03/cf_J_ShoulderIK_R/cf_J_Shoulder_R/cf_J_ArmUp00_R/cf_J_ArmLow01_R/cf_J_Hand_R").transform.position, Self.charInfo.objAnim.transform.Find("cf_J_Root/cf_N_height/cf_J_Hips/cf_J_Spine01/cf_J_Spine02/cf_J_Spine03/cf_J_ShoulderIK_R/cf_J_Shoulder_R/cf_J_ArmUp00_R/cf_J_ArmLow01_R").transform.position);
#endif
            }
            return FindCurrentScale().y;
        }

        private OCIChar.IKInfo FindIKInfo(IKTarget target)
        {
            return Self.listIKTarget.Find(ikt => ikt.baseObject.name == IKTargetNames[target]);
        }

        private Transform FindBoneTransform(IKTarget target)
        {
            return Self.listIKTarget.Find(ikt => ikt.baseObject.name == IKTargetNames[target]).boneObject;
        }

        private Transform FindTargetTransform(IKTarget target)
        {
            return Self.listIKTarget.Find(ikt => ikt.baseObject.name == IKTargetNames[target]).targetObject;
        }

        private Transform FindGuideTransform(IKTarget target)
        {
            return Self.listIKTarget.Find(ikt => ikt.baseObject.name == IKTargetNames[target]).guideObject.transform;
        }

        private Transform FindGameTransform(IKTarget target)
        {
            return Self.listIKTarget.Find(ikt => ikt.baseObject.name == IKTargetNames[target]).gameObject.transform;
        }

        private static readonly Dictionary<IKTarget, string> IKTargetNames = new Dictionary<IKTarget, string>
        {
#if KOIKATSU
            { IKTarget.BODY, "cf_t_hips" },
            { IKTarget.LEFT_SHOULDER, "cf_t_shoulder_L"},
            { IKTarget.LEFT_ELBOW, "cf_t_elbo_L"},
            { IKTarget.LEFT_HAND, "cf_t_hand_L"},
            { IKTarget.RIGHT_SHOULDER, "cf_t_shoulder_R"},
            { IKTarget.RIGHT_ELBOW, "cf_t_elbo_R"},
            { IKTarget.RIGHT_HAND, "cf_t_hand_R"},
            { IKTarget.LEFT_THIGH, "cf_t_waist_L"},
            { IKTarget.LEFT_KNEE, "cf_t_knee_L"},
            { IKTarget.LEFT_FOOT, "cf_t_leg_L"},
            { IKTarget.RIGHT_THIGH, "cf_t_waist_R"},
            { IKTarget.RIGHT_KNEE, "cf_t_knee_R"},
            { IKTarget.RIGHT_FOOT, "cf_t_leg_R"},
#else
            { IKTarget.BODY, "f_t_hips" },
            { IKTarget.LEFT_SHOULDER, "f_t_shoulder_L"},
            { IKTarget.LEFT_ELBOW, "f_t_elbo_L"},
            { IKTarget.LEFT_HAND, "f_t_arm_L"},
            { IKTarget.RIGHT_SHOULDER, "f_t_shoulder_R"},
            { IKTarget.RIGHT_ELBOW, "f_t_elbo_R"},
            { IKTarget.RIGHT_HAND, "f_t_arm_R"},
            { IKTarget.LEFT_THIGH, "f_t_thigh_L"},
            { IKTarget.LEFT_KNEE, "f_t_knee_L"},
            { IKTarget.LEFT_FOOT, "f_t_leg_L"},
            { IKTarget.RIGHT_THIGH, "f_t_thigh_R"},
            { IKTarget.RIGHT_KNEE, "f_t_knee_R"},
            { IKTarget.RIGHT_FOOT, "f_t_leg_R"},
#endif
        };

        private static readonly Dictionary<IKChain, IKTarget[]> IKChainTargets = new Dictionary<IKChain, IKTarget[]>
        {
            { IKChain.BODY, new IKTarget[] {IKTarget.LEFT_SHOULDER, IKTarget.RIGHT_SHOULDER, IKTarget.LEFT_THIGH, IKTarget.RIGHT_THIGH} },
            { IKChain.LEFT_ARM, new IKTarget[] {IKTarget.LEFT_ELBOW, IKTarget.LEFT_HAND} },
            { IKChain.LEFT_LEG, new IKTarget[] {IKTarget.LEFT_KNEE, IKTarget.LEFT_FOOT} },
            { IKChain.RIGHT_ARM, new IKTarget[] {IKTarget.RIGHT_ELBOW, IKTarget.RIGHT_HAND} },
            { IKChain.RIGHT_LEG, new IKTarget[] {IKTarget.RIGHT_KNEE, IKTarget.RIGHT_FOOT} },
        };

        private static readonly Dictionary<IKChain, IKTarget> IKChainBases = new Dictionary<IKChain, IKTarget>
        {
            { IKChain.BODY, IKTarget.BODY },
            { IKChain.LEFT_ARM, IKTarget.LEFT_SHOULDER },
            { IKChain.RIGHT_ARM, IKTarget.RIGHT_SHOULDER },
            { IKChain.LEFT_LEG, IKTarget.LEFT_SHOULDER },
            { IKChain.RIGHT_LEG, IKTarget.RIGHT_SHOULDER },
        };

        private static readonly Dictionary<IKChain, IKScale[]> IKChainScale = new Dictionary<IKChain, IKScale[]>
        {
            { IKChain.BODY, new IKScale[] { IKScale.BODY, IKScale.BODY } },
            { IKChain.LEFT_ARM, new IKScale[] { IKScale.LEFT_UPPER_ARM, IKScale.LEFT_ARM } },
            { IKChain.RIGHT_ARM, new IKScale[] { IKScale.RIGHT_UPPER_ARM, IKScale.RIGHT_ARM } },
            { IKChain.LEFT_LEG, new IKScale[] { IKScale.LEFT_UPPER_LEG, IKScale.LEFT_LEG } },
            { IKChain.RIGHT_LEG, new IKScale[] { IKScale.RIGHT_UPPER_LEG, IKScale.RIGHT_LEG } }
        };
    }

    public enum IKTarget
    {
        BODY = 0,
        LEFT_SHOULDER = 1,
        LEFT_ELBOW = 2,
        LEFT_HAND = 3,
        RIGHT_SHOULDER = 4,
        RIGHT_ELBOW = 5,
        RIGHT_HAND = 6,
        LEFT_THIGH = 7,
        LEFT_KNEE = 8,
        LEFT_FOOT = 9,
        RIGHT_THIGH = 10,
        RIGHT_KNEE = 11,
        RIGHT_FOOT = 12
    }

    public enum IKScale
    {
        BODY = 0,
        LEFT_UPPER_ARM = 1,
        LEFT_LOWER_ARM = 2,
        LEFT_ARM = 3,
        RIGHT_UPPER_ARM = 4,
        RIGHT_LOWER_ARM = 5,
        RIGHT_ARM = 6,
        LEFT_UPPER_LEG = 7,
        LEFT_LOWER_LEG = 8,
        LEFT_LEG = 9,
        RIGHT_UPPER_LEG = 10,
        RIGHT_LOWER_LEG = 11,
        RIGHT_LEG = 12
    }

    public enum IKChain
    {
        BODY = 0,
        LEFT_ARM = 1,
        LEFT_LEG = 2,
        RIGHT_ARM = 3,
        RIGHT_LEG = 4
    }

    public enum IKResizeChainAdjustment
    {
        OFF = 1,
        CHAIN = 2
    }

    public enum IKResizeCentroid
    {
        NONE = 0,
        AUTO = 1,
        FEET_CENTER = 2,
        FEET_LEFT = 3,
        FEET_RIGHT = 4,
        THIGH_CENTER = 5,
        THIGH_LEFT = 6,
        THIGH_RIGHT = 7,
        BODY = 8,
        SHOULDER_CENTER = 9,
        SHOULDER_LEFT = 10,
        SHOULDER_RIGHT = 11,
        HAND_CENTER = 12,
        HAND_LEFT = 13,
        HAND_RIGHT = 14,
        KNEE_CENTER = 15,
        KNEE_LEFT = 16,
        KNEE_RIGHT = 17,
        ELBOW_CENTER = 18,
        ELBOW_LEFT = 19,
        ELBOW_RIGHT = 20
    }
}
