using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private float _spineStiffness = 0;

        private ShoulderRotator _shoulderRotator;
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
                }
                else
                {
                    RemoveShoulderRotator();
                    _shoulderRotator = null;
                }
                
            }
        }

        public float ShoulderWeight
        {
            get => _shoulderWeight;
            set
            {
                _shoulderWeight = value;
                if (_shoulderRotator!= null)
                {
                    _shoulderRotator.weight = _shoulderWeight;
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
            data.data["ShoulderWeight"] = _shoulderWeight;
            data.data["ShoulderOffset"] = _shoulderOffset;
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
                if (data.data.TryGetValue("ShoulderWeight", out var val2)) ShoulderWeight = (float)val2;
                if (data.data.TryGetValue("ShoulderOffset", out var val3)) ShoulderOffset = (float)val3;
                if (data.data.TryGetValue("SpineStiffness", out var val4)) SpineStiffness = (float)val4;
            }

        }

        private GameObject FindAnimator()
        {
            return this.gameObject.transform.Find("BodyTop/p_cf_anim")?.gameObject;
        }

        private void AddShoulderRotator()
        {
            _shoulderRotator = FindShoulderRotator();
            if (_shoulderRotator == null)
            {
                GameObject animator = FindAnimator();
                if (animator != null)
                {
                    _shoulderRotator = animator.AddComponent(typeof(ShoulderRotator)) as ShoulderRotator;
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

        private ShoulderRotator FindShoulderRotator()
        {
            GameObject animator = FindAnimator();
            if (animator != null)
            {
                return animator.GetComponent<ShoulderRotator>();
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
