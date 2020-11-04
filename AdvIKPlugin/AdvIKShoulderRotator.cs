using RootMotion.FinalIK;
using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

namespace AdvIKPlugin
{
    class AdvIKShoulderRotator : MonoBehaviour
    {

        public float weight = 1.5f;
        public float offset = 0.2f;

        public float weightR = 1.5f;
        public float offsetR = 0.2f;

        private FullBodyBipedIK ik;

        private bool skip;

        public AdvIKCharaController advIKCharaController;

        private void Start()
        {
            ik = GetComponent<FullBodyBipedIK>();
            IKSolverFullBodyBiped solver = ik.solver;
            solver.OnPostUpdate = (IKSolver.UpdateDelegate)Delegate.Combine(solver.OnPostUpdate, new IKSolver.UpdateDelegate(RotateShoulders));
        }

        private void RotateShoulders()
        {
            if (!(ik == null) && !(ik.solver.IKPositionWeight <= 0f))
            {
                if (skip)
                {
                    skip = false;
                    return;
                }
                RotateShoulder(FullBodyBipedChain.LeftArm, weight, offset);
                RotateShoulder(FullBodyBipedChain.RightArm, weightR, offsetR);
                skip = true;
                ik.solver.Update();
            }
        }

        private void RotateShoulder(FullBodyBipedChain chain, float weight, float offset)
        {
            if (advIKCharaController.EnableShoulderFKHints)
            {
                Vector3 baseRotation = Vector3.zero;
                if (chain == FullBodyBipedChain.LeftArm)
                {
                    baseRotation = advIKCharaController.FindFKRotation(advIKCharaController.FindLSFKBone());
                }
                else if (chain == FullBodyBipedChain.RightArm)
                {
                    baseRotation = advIKCharaController.FindFKRotation(advIKCharaController.FindRSFKBone());
                }

                ik.solver.GetLimbMapping(chain).parentBone.Rotate(baseRotation);
            }

            Quaternion b = Quaternion.FromToRotation(GetParentBoneMap(chain).swingDirection, ik.solver.GetEndEffector(chain).position - GetParentBoneMap(chain).transform.position);
            Vector3 vector = ik.solver.GetEndEffector(chain).position - ik.solver.GetLimbMapping(chain).bone1.position;
            float num = ik.solver.GetChain(chain).nodes[0].length + ik.solver.GetChain(chain).nodes[1].length;
            float num2 = vector.magnitude / num - 1f + offset;
            num2 = Mathf.Clamp(num2 * weight, 0f, 1f);
            Quaternion lhs = Quaternion.Lerp(Quaternion.identity, b, num2 * ik.solver.GetEndEffector(chain).positionWeight * ik.solver.IKPositionWeight);
            if ( (advIKCharaController.ReverseShoulderL && chain == FullBodyBipedChain.LeftArm) || (advIKCharaController.ReverseShoulderR && chain == FullBodyBipedChain.RightArm))
            {
                lhs = Quaternion.Inverse(lhs);
            }
            ik.solver.GetLimbMapping(chain).parentBone.rotation = lhs * ik.solver.GetLimbMapping(chain).parentBone.rotation;

        }

        private IKMapping.BoneMap GetParentBoneMap(FullBodyBipedChain chain)
        {
            return ik.solver.GetLimbMapping(chain).GetBoneMap(IKMappingLimb.BoneMapType.Parent);
        }

        private void OnDestroy()
        {
            if (ik != null)
            {
                IKSolverFullBodyBiped solver = ik.solver;
                solver.OnPostUpdate = (IKSolver.UpdateDelegate)Delegate.Remove(solver.OnPostUpdate, new IKSolver.UpdateDelegate(RotateShoulders));
            }
        }
    }
}
