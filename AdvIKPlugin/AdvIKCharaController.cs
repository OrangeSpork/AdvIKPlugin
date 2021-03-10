using System;


using KKAPI.Chara;
using KKAPI;
using RootMotion.FinalIK;
using UnityEngine;
using ExtensibleSaveFormat;
using System.Collections;

using KKAPI.Studio;
using Studio;
using AdvIKPlugin.Algos;
using KKABMX.Core;

namespace AdvIKPlugin
{
    public class AdvIKCharaController : CharaCustomFunctionController
    {
        private BreathingBoneEffect _breathing;

        private bool _shoulderRotationEnabled = false;
        private bool _reverseShoulderL = false;
        private bool _reverseShoulderR = false;
        private bool _enableSpineFKHints = false;
        private bool _enableShoulderFKHints = false;
        private float _shoulderWeight = 1.5f;
        private float _shoulderOffset = .2f;

        private bool _independentShoulders = false;
        private float _shoulderRightWeight = 1.5f;
        private float _shoulderRightOffset = .2f;

        private float _spineStiffness = 0;

        private AdvIKShoulderRotator _shoulderRotator;

        private bool _loaded = false;


        public BreathingBoneEffect BreathingController
        {
            get => _breathing;
        }

        public bool ShoulderRotationEnabled
        {
            get => _shoulderRotationEnabled;
            set
            {
                _shoulderRotationEnabled = value;
                if (_shoulderRotationEnabled)
                {
                    AddShoulderRotator();
                    _shoulderRotator.weight = _shoulderWeight;
                    _shoulderRotator.offset = _shoulderOffset;

                    if (_independentShoulders)
                    {
                        _shoulderRotator.weightR = _shoulderRightWeight;
                        _shoulderRotator.offsetR = _shoulderRightOffset;
                    }
                    else
                    {
                        _shoulderRotator.weightR = _shoulderWeight;
                        _shoulderRotator.offsetR = _shoulderOffset;
                    }
                }
                else
                {
                    RemoveShoulderRotator();
                    _shoulderRotator = null;
                }

            }
        }

        public bool ReverseShoulderL
        {
            get => _reverseShoulderL;
            set
            {
                _reverseShoulderL = value;
            }
        }
        public bool ReverseShoulderR
        {
            get => _reverseShoulderR;
            set
            {
                _reverseShoulderR = value;
            }
        }


        public bool EnableSpineFKHints
        {
            get => _enableSpineFKHints;
            set
            {
                _enableSpineFKHints = value;
            }
        }

        public bool EnableShoulderFKHints
        {
            get => _enableShoulderFKHints;
            set
            {
                _enableShoulderFKHints = value;
            }
        }

        public bool IndependentShoulders
        {
            get => _independentShoulders;
            set
            {
                _independentShoulders = value;
                if (_shoulderRotator != null)
                {
                    if (_independentShoulders)
                    {
                        _shoulderRotator.weightR = _shoulderRightWeight;
                        _shoulderRotator.offsetR = _shoulderRightOffset;
                    }
                    else
                    {
                        _shoulderRotator.weightR = _shoulderWeight;
                        _shoulderRotator.offsetR = _shoulderOffset;
                    }
                }
            }
        }

        public float ShoulderWeight
        {
            get => _shoulderWeight;
            set
            {
                _shoulderWeight = value;
                if (_shoulderRotator != null)
                {
                    _shoulderRotator.weight = _shoulderWeight;
                    if (!_independentShoulders)
                    {
                        _shoulderRotator.weightR = _shoulderWeight;
                    }
                }
            }
        }

        public float ShoulderRightWeight
        {
            get => _shoulderRightWeight;
            set
            {
                _shoulderRightWeight = value;
                if (_shoulderRotator != null && _independentShoulders)
                {
                    _shoulderRotator.weightR = _shoulderRightWeight;
                }
            }
        }

        public float ShoulderOffset
        {
            get => _shoulderOffset;
            set
            {
                _shoulderOffset = value;
                if (_shoulderRotator != null)
                {
                    _shoulderRotator.offset = _shoulderOffset;
                    if (!_independentShoulders)
                    {
                        _shoulderRotator.offsetR = _shoulderOffset;
                    }
                }
            }
        }

        public float ShoulderRightOffset
        {
            get => _shoulderRightOffset;
            set
            {
                _shoulderRightOffset = value;
                if (_shoulderRotator != null && _independentShoulders)
                {
                    _shoulderRotator.offsetR = _shoulderRightOffset;
                }
            }
        }

        public float SpineStiffness
        {
            get => _spineStiffness;
            set
            {
                _spineStiffness = value;
                if (FindSolver() != null)
                {
                    FindSolver().spineStiffness = _spineStiffness;
                }
            }
        }

        protected override void OnCardBeingSaved(GameMode currentGameMode)
        {
            var data = new PluginData();

            data.data["ShoulderRotatorEnabled"] = _shoulderRotationEnabled;            
            data.data["IndependentShoulders"] = _independentShoulders;
            data.data["ReverseShoulderL"] = _reverseShoulderL;
            data.data["ReverseShoulderR"] = _reverseShoulderR;
            data.data["ShoulderWeight"] = _shoulderWeight;
            data.data["ShoulderRightWeight"] = _shoulderRightWeight;
            data.data["ShoulderOffset"] = _shoulderOffset;
            data.data["ShoulderRightOffset"] = _shoulderRightOffset;
            data.data["SpineStiffness"] = _spineStiffness;
            data.data["EnableSpineFKHints"] = _enableSpineFKHints;
            data.data["EnableShoulderFKHints"] = _enableShoulderFKHints;

            if (BreathingController != null) BreathingController.SaveConfig(data);

            SetExtendedData(data);

        }

        protected override void OnReload(GameMode currentGameMode, bool maintainState)
        {
#if DEBUG
            AdvIKPlugin.Instance.Log.LogInfo($"Loaded {_loaded} Reload {maintainState} {ChaControl.fileParam.fullname}");
            AdvIKPlugin.Instance.Log.LogInfo($"Before Shoulders: {ShoulderRotationEnabled} Breathing: {_breathing?.Enabled}");
#endif
            if (maintainState || (_loaded && StudioAPI.InsideStudio))
            {
                ResetBreathing();
                return;
            }

            var data = GetExtendedData();

            if (data != null)
            {

                if (data.data.TryGetValue("ShoulderRotatorEnabled", out var val1)) ShoulderRotationEnabled = (bool)val1;                
                if (data.data.TryGetValue("IndependentShoulders", out var val1a)) IndependentShoulders = (bool)val1a;
                if (data.data.TryGetValue("ShoulderWeight", out var val2)) ShoulderWeight = (float)val2;
                if (data.data.TryGetValue("ShoulderRightWeight", out var val2r)) ShoulderRightWeight = (float)val2r;
                if (data.data.TryGetValue("ShoulderOffset", out var val3)) ShoulderOffset = (float)val3;
                if (data.data.TryGetValue("ShoulderRightOffset", out var val3r)) ShoulderRightOffset = (float)val3r;                
                if (data.data.TryGetValue("SpineStiffness", out var val4)) StartCoroutine("setSpineStiffness", (float)val4);
                if (data.data.TryGetValue("EnableSpineFKHints", out var val5)) EnableSpineFKHints = (bool)val5;
                if (data.data.TryGetValue("EnableShoulderFKHints", out var val6)) EnableShoulderFKHints = (bool)val6;
                if (data.data.TryGetValue("ReverseShoulderL", out var val7)) ReverseShoulderL = (bool)val7;
                if (data.data.TryGetValue("ReverseShoulderR", out var val8)) ReverseShoulderR = (bool)val8;
                _breathing = null;
                StartCoroutine("StartBreathing", data);
            }
            else
            {
                ResetBreathing();
            }

#if DEBUG
            AdvIKPlugin.Instance.Log.LogInfo($"After, BC Shoulders: {ShoulderRotationEnabled} Breathing: {_breathing?.Enabled}");
#endif
            _loaded = true;
        }

        private IEnumerator StartBreathing(PluginData data)
        {
            yield return new WaitUntil(() => ChaControl != null && ChaControl.GetComponent<BoneController>() != null && ChaControl.objAnim != null);

            BoneController boneController = ChaControl.GetComponent<BoneController>();
            if (_breathing == null)
            {
                _breathing = new BreathingBoneEffect(FindUpperChestBone().name, FindLowerChestBone().name, FindAbdomenBone().name, FindBreastBone()?.name, FindLSBone().name, FindRSBone().name, FindLeftBreastBone()?.name, FindRightBreastBone()?.name);
                boneController.AddBoneEffect(_breathing);
            }
            
            if (data != null)
            {
                _breathing.LoadConfig(data);
            }

#if DEBUG
            AdvIKPlugin.Instance.Log.LogInfo($"After, AC Shoulders: {ShoulderRotationEnabled} Breathing: {_breathing?.Enabled} Data: {data}");
#endif

        }

        private void ResetBreathing()
        {
            PluginData tempData = new PluginData();
            if (_breathing != null)
            {
                _breathing.SaveConfig(tempData);
            }
            StartCoroutine("StartBreathing", tempData);
        }

        private IEnumerator setSpineStiffness(float spineStiffnessValue)
        {
            SpineStiffness = spineStiffnessValue;

            yield return new WaitForSeconds(1);     // This is a horrible hack, but something after load is resetting this and I need to get to make sure this runs afterwards and can't find a better hook point...       

            SpineStiffness = spineStiffnessValue;
        }

        protected override void Update()
        {
            if (_breathing != null) 
                _breathing.FrameEffects = null;

            base.Update();
        }

        protected void LateUpdate()
        {

            if (FindSolver() != null && FindSolver().OnPreSolve == null)
            {

                FindSolver().OnPreSolve = (IKSolver.UpdateDelegate)Delegate.Combine(FindSolver().OnPreSolve, new IKSolver.UpdateDelegate(() => {
                    if (EnableSpineFKHints)
                    {
                        Vector3 spine2targetRotation = FindFKRotation(FindSpine2());
                        Vector3 spine1targetRotation = FindFKRotation(FindSpine());

                        FindSolver().GetSpineMapping().spineBones[2].Rotate(spine2targetRotation, Space.Self);
                        FindSolver().GetSpineMapping().spineBones[1].Rotate(spine1targetRotation, Space.Self);
                        FindSolver().GetSpineMapping().ReadPose();
                    }
                }));              
            }
        }

        public Vector3 FindFKRotation(Transform t)
        {
            foreach (OCIChar.BoneInfo bone in this.ChaControl.GetOCIChar().listBones)
            {
                if (bone.guideObject.transformTarget.name.Equals(t.name, StringComparison.OrdinalIgnoreCase))
                {
                    return bone.guideObject.changeAmount.rot;
                }
            }
            return Vector3.zero;
        }

   

        protected override void OnEnable()
        {
            StartCoroutine("StartBreathing", new PluginData());
        }


        private void AddShoulderRotator()
        {
            _shoulderRotator = FindShoulderRotator();
            if (_shoulderRotator == null)
            {
                GameObject animator = FindAnimator();
                if (animator != null)
                {
                    _shoulderRotator = animator.AddComponent(typeof(AdvIKShoulderRotator)) as AdvIKShoulderRotator;
                    _shoulderRotator.advIKCharaController = this;
                }
            }
        }

        private void RemoveShoulderRotator()
        {
            _shoulderRotator = FindShoulderRotator();
            if (_shoulderRotator != null)
            {
                _shoulderRotator.enabled = false;
                UnityEngine.Object.Destroy(_shoulderRotator);
            }
        }

        private AdvIKShoulderRotator FindShoulderRotator()
        {
            GameObject animator = FindAnimator();
            if (animator != null)
            {
                return animator.GetComponent<AdvIKShoulderRotator>();
            }
            return null;
        }

        private IKSolverFullBodyBiped FindSolver()
        {
            GameObject animator = FindAnimator();
            if (animator != null)
            {
                FullBodyBipedIK fbbik = animator.GetComponent<FullBodyBipedIK>();
                if (fbbik != null)
                {
                    return fbbik.solver;
                }
            }
            return null;
        }


        private GameObject FindAnimator()
        {
            return ChaControl.objAnim;
        }

        private Transform FindHips()
        {
            if (FindAnimator())
            {
#if KOIKATSU
                return FindDescendant(FindAnimator().transform, "cf_j_hips");
#else
                return FindDescendant(FindAnimator().transform, "cf_J_Hips");
#endif
            }
            else
            {
                return null;
            }
        }

        private Transform FindSpine()
        {

            if (FindAnimator())
            {
#if KOIKATSU
                return FindDescendant(FindAnimator().transform, "cf_j_spine01");
#else
                return FindDescendant(FindAnimator().transform, "cf_J_Spine01");
#endif
            }
            else
            {
                return null;
            }
        }

        private Transform FindSpine2()
        {

            if (FindAnimator())
            {
#if KOIKATSU
                return FindDescendant(FindAnimator().transform, "cf_j_spine02");
#else
                return FindDescendant(FindAnimator().transform, "cf_J_Spine02");
#endif
            }
            else
            {
                return null;
            }
        }

        private Transform FindAbdomenBone()
        {
            if (FindAnimator())
            {
#if KOIKATSU
                return FindDescendant(FindAnimator().transform, "cf_s_spine01");
#else
                return FindDescendant(FindAnimator().transform, "cf_J_Spine01_s");
#endif
            }
            else
            {
                return null;
            }
        }

        private Transform FindLowerChestBone()
        {
            if (FindAnimator())
            {
#if KOIKATSU
                return FindDescendant(FindAnimator().transform, "cf_s_spine02");
#else
                return FindDescendant(FindAnimator().transform, "cf_J_Spine02_s");
#endif
            }
            else
            {
                return null;
            }
        }

        private Transform FindUpperChestBone()
        {
            if (FindAnimator())
            {
#if KOIKATSU
                return FindDescendant(FindAnimator().transform, "cf_s_spine03");
#else
                return FindDescendant(FindAnimator().transform, "cf_J_Spine03_s");
#endif
            }
            else
            {
                return null;
            }
        }

        private Transform FindLeftBreastBone()
        {
            if (FindAnimator())
            {
#if KOIKATSU
                //  return FindDescendant(FindAnimator().transform, "cf_s_bust00_L");
                return FindDescendant(FindAnimator().transform, "cf_j_bust01_L");
#else
                return null;
#endif
            }
            else
            {
                return null;
            }
        }

        private Transform FindRightBreastBone()
        {
            if (FindAnimator())
            {
#if KOIKATSU
                //return FindDescendant(FindAnimator().transform, "cf_s_bust00_R");
                return FindDescendant(FindAnimator().transform, "cf_j_bust01_R");
#else
                return null;
#endif
            }
            else
            {
                return null;
            }
        }

        private Transform FindBreastBone()
        {
            if (FindAnimator())
            {
#if KOIKATSU
                return null;
#else
                return FindDescendant(FindAnimator().transform, "cf_J_Mune00");
#endif
            }
            else
            {
                return null;
            }
        }

        private Transform FindLSBone()
        {
            if (FindAnimator())
            {
#if KOIKATSU
                return FindDescendant(FindAnimator().transform, "cf_j_shoulder_L");
#else
                return FindDescendant(FindAnimator().transform, "cf_J_ShoulderIK_L");
#endif
            }
            else
            {
                return null;
            }
        }
        private Transform FindRSBone()
        {
            if (FindAnimator())
            {
#if KOIKATSU
                return FindDescendant(FindAnimator().transform, "cf_j_shoulder_R");
#else
                return FindDescendant(FindAnimator().transform, "cf_J_ShoulderIK_R");
#endif
            }
            else
            {
                return null;
            }
        }

        public Transform FindLSFKBone()
        {
            if (FindAnimator())
            {
#if KOIKATSU
                return FindDescendant(FindAnimator().transform, "cf_j_shoulder_L");
#else
                return FindDescendant(FindAnimator().transform, "cf_J_Shoulder_L");
#endif
            }
            else
            {
                return null;
            }
        }
        public Transform FindRSFKBone()
        {
            if (FindAnimator())
            {
#if KOIKATSU
                return FindDescendant(FindAnimator().transform, "cf_j_shoulder_R");
#else
                return FindDescendant(FindAnimator().transform, "cf_J_Shoulder_R");
#endif
            }
            else
            {
                return null;
            }
        }

        public Transform FindDescendant(Transform start, string name)
        {
            if (start == null)
            {
                return null;
            }

            if (start.name.Equals(name))
                return start;
            foreach (Transform t in start)
            {
                Transform res = FindDescendant(t, name);
                if (res != null)
                    return res;
            }
            return null;
        }

        System.Random rand = new System.Random();
        private double randomGaussian(double mean, double stdDev)
        {
            double u1 = rand.NextDouble();
            double u2 = rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0d * Math.Log(u1)) * Math.Sin(2.0d * Math.PI * u2);
            double randNormal = mean + stdDev * randStdNormal;
            return randNormal;
        }
    }
}
