using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KKAPI.Chara;
using KKAPI;
using RootMotion.FinalIK;
using UnityEngine;
using ExtensibleSaveFormat;

namespace AdvIKPlugin
{
    public class AdvIKCharaController : CharaCustomFunctionController
    {
        private bool _shoulderRotationEnabled = false;
        private float _shoulderWeight = 1.5f;
        private float _shoulderOffset = .2f;

        private bool _independentShoulders = false;
        private float _shoulderRightWeight = 1.5f;
        private float _shoulderRightOffset = .2f;

        private float _spineStiffness = 0;

        private AdvIKShoulderRotator _shoulderRotator;
        private IKSolverFullBodyBiped _ikSolver;


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
                if (_ikSolver != null)
                {
                    _ikSolver.spineStiffness = _spineStiffness;
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

            SetExtendedData(data);

        }

        protected override void OnReload(GameMode currentGameMode, bool maintainState)
        {
            if (maintainState) return;

            _ikSolver = FindSolver();

            var data = GetExtendedData();

            if (data != null)
            {

                if (data.data.TryGetValue("ShoulderRotatorEnabled", out var val1)) ShoulderRotationEnabled = (bool)val1;
                if (data.data.TryGetValue("IndependentShoulders", out var val1a)) IndependentShoulders = (bool)val1a;
                if (data.data.TryGetValue("ShoulderWeight", out var val2)) ShoulderWeight = (float)val2;
                if (data.data.TryGetValue("ShoulderRightWeight", out var val2r)) ShoulderRightWeight = (float)val2r;
                if (data.data.TryGetValue("ShoulderOffset", out var val3)) ShoulderOffset = (float)val3;
                if (data.data.TryGetValue("ShoulderRightOffset", out var val3r)) ShoulderRightOffset = (float)val3r;
                if (data.data.TryGetValue("SpineStiffness", out var val4)) SpineStiffness = (float)val4;
            }

        }

        private GameObject FindAnimator()
        {
            return ChaControl.objAnim;
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

    }
}
