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

namespace AdvIKPlugin
{
    public class AdvIKCharaController : CharaCustomFunctionController
    {
        private Breathing _breathing;

        private bool _shoulderRotationEnabled = false;
        private bool _enableSpineFKHints = false;
        private float _shoulderWeight = 1.5f;
        private float _shoulderOffset = .2f;

        private bool _independentShoulders = false;
        private float _shoulderRightWeight = 1.5f;
        private float _shoulderRightOffset = .2f;

        private float _spineStiffness = 0;

        private AdvIKShoulderRotator _shoulderRotator;


        public Breathing BreathingController
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

        public bool EnableSpineFKHints
        {
            get => _enableSpineFKHints;
            set
            {
                _enableSpineFKHints = value;
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
            data.data["ShoulderWeight"] = _shoulderWeight;
            data.data["ShoulderRightWeight"] = _shoulderRightWeight;
            data.data["ShoulderOffset"] = _shoulderOffset;
            data.data["ShoulderRightOffset"] = _shoulderRightOffset;
            data.data["SpineStiffness"] = _spineStiffness;
            data.data["EnableSpineFKHints"] = _enableSpineFKHints;

            if (BreathingController != null) BreathingController.SaveConfig(data);

            SetExtendedData(data);

        }

        protected override void OnReload(GameMode currentGameMode, bool maintainState)
        {
            if (maintainState)
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
                _breathing = null;
                StartCoroutine("StartBreathing", data);
            }
            else
            {
                ResetBreathing();
            } 
        }

        private IEnumerator StartBreathing(PluginData data)
        {
            yield return new WaitForSeconds(1);

            if (_breathing != null)
            {
                _breathing.RestoreOriginalSnapshot();
            }

            _breathing = new Breathing(FindUpperChestBone(), FindLowerChestBone(), FindAbdomenBone(), FindBreastBone(), FindLSBone(), FindRSBone());
            if (data != null)
            {
                _breathing.LoadConfig(data);
            }
        }

        private void ResetBreathing()
        {
            PluginData tempData = new PluginData();
            if (_breathing != null)
            {
                _breathing.SaveConfig(tempData);
                _breathing = null;
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
            if (_breathing != null && _breathing.Enabled)
            {
                _breathing.RestorePriorSnapshot();
            }
        }

        protected void LateUpdate()
        {
            if (_breathing != null && _breathing.Enabled)
            {
                _breathing.Perform();
            }

            if (FindSolver().OnPreSolve == null)
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

        private Vector3 FindFKRotation(Transform t)
        {
            foreach (OCIChar.BoneInfo bone in this.ChaControl.GetOCIChar().listBones)
            {
                if (bone.guideObject.transformTarget.name.Equals(t.name))
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

        private Transform FindBreastBone()
        {
            if (FindAnimator())
            {
#if KOIKATSU
                return FindDescendant(FindAnimator().transform, "cf_d_bust00");
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
