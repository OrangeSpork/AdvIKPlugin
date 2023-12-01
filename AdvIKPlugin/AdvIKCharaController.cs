using System;
using System.Linq;

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
using System.Collections.Generic;
using HarmonyLib;
using System.Reflection;
using System.Diagnostics;

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
        private bool _enableToeFKHints = false;
        private float _shoulderWeight = 1.5f;
        private float _shoulderOffset = .2f;

        private bool _independentShoulders = false;
        private float _shoulderRightWeight = 1.5f;
        private float _shoulderRightOffset = .2f;

        private float _spineStiffness = 0;

        private bool _enableHeelzHoverLeftFoot = false;
        private bool _enableHeelzHoverRightFoot = false;
        private bool _enableHeelzHoverAll = false;

        private AdvIKShoulderRotator _shoulderRotator;
        private IKResizeAdjustment _iKResizeAdjustment;

        private bool _loaded = false;


        public BreathingBoneEffect BreathingController
        {
            get => _breathing;
        }

        public IKResizeAdjustment IKResizeController
        {
            get => _iKResizeAdjustment;
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

        public bool EnableToeFKHints
        {
            get => _enableToeFKHints;
            set
            {
                _enableToeFKHints = value;
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

        public bool EnableHeelzHoverAll { get => _enableHeelzHoverAll; set => _enableHeelzHoverAll = value; }
        public bool EnableHeelzHoverLeftFoot { get => _enableHeelzHoverLeftFoot; set => _enableHeelzHoverLeftFoot = value; }
        public bool EnableHeelzHoverRightFoot { get => _enableHeelzHoverRightFoot; set => _enableHeelzHoverRightFoot = value; }

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
            data.data["EnableToeFKHints"] = _enableToeFKHints;
            data.data["EnableHeelzHoverAll"] = _enableHeelzHoverAll;
            data.data["EnableHeelzHoverLeftFoot"] = _enableHeelzHoverLeftFoot;
            data.data["EnableHeelzHoverRightFoot"] = _enableHeelzHoverRightFoot;

            if (BreathingController != null) BreathingController.SaveConfig(data);
            if (IKResizeController != null) IKResizeController.SaveConfig(data);

            SetExtendedData(data);

        }

        protected override void OnReload(GameMode currentGameMode, bool maintainState)
        {
#if DEBUG
            AdvIKPlugin.Instance.Log.LogInfo($"Loaded {_loaded} Reload {maintainState} {ChaControl.fileParam.fullname}");
            AdvIKPlugin.Instance.Log.LogInfo($"Before Shoulders: {ShoulderRotationEnabled} Breathing: {_breathing?.Enabled}");
#endif
            if (_iKResizeAdjustment == null)
            {
                _iKResizeAdjustment = new IKResizeAdjustment();
            }
            StartCoroutine("AdjustIKTargets");            

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
                if (data.data.TryGetValue("EnableToeFKHints", out var val9)) EnableToeFKHints = (bool)val9;
                if (data.data.TryGetValue("EnableHeelzHoverAll", out var val10)) EnableHeelzHoverAll = (bool)val10;
                if (data.data.TryGetValue("EnableHeelzHoverLeftFoot", out var val11)) EnableHeelzHoverLeftFoot = (bool)val11;
                if (data.data.TryGetValue("EnableHeelzHoverRightFoot", out var val12)) EnableHeelzHoverRightFoot = (bool)val12;
                StartCoroutine("StartBreathing", data);
                if (IKResizeController != null) IKResizeController.LoadConfig(data);
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

        private IEnumerator AdjustIKTargets()
        {
#if DEBUG
            AdvIKPlugin.Instance.Log.LogInfo($"Resize Coroutine Firing");
#endif
            yield return new WaitUntil(() => ChaControl != null && ChaControl.objAnim != null);

            #if DEBUG
            AdvIKPlugin.Instance.Log.LogInfo($"Triggering Resize Adjustment for {ChaControl.chaFile.parameter.fullname}");
#endif


            // Wait for ABMX to run
            yield return null;
            yield return null;

            _iKResizeAdjustment.OnReload(ChaControl);


        }

        private IEnumerator StartBreathing(PluginData data)
        {
            yield return new WaitUntil(() => ChaControl != null && ChaControl.GetComponent<BoneController>() != null && ChaControl.objAnim != null);

            BoneController boneController = ChaControl.GetComponent<BoneController>();
            if (_breathing == null)
            {
#if DEBUG
                AdvIKPlugin.Instance.Log.LogInfo("Adding Bone Effect");
#endif
                _breathing = new BreathingBoneEffect(FindUpperChestBone().name, FindLowerChestBone().name, FindAbdomenBone().name, FindBreastBone()?.name, FindLSBone().name, FindRSBone().name, FindLeftBreastBone()?.name, FindRightBreastBone()?.name, FindNeck()?.name);
                boneController.AddBoneEffect(_breathing);
            }
            else
            {
                _breathing.Initialize(FindUpperChestBone().name, FindLowerChestBone().name, FindAbdomenBone().name, FindBreastBone()?.name, FindLSBone().name, FindRSBone().name, FindLeftBreastBone()?.name, FindRightBreastBone()?.name, FindNeck()?.name);
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

            yield return null;    // This is a horrible hack, but something after load is resetting this and I need to get to make sure this runs afterwards and can't find a better hook point...       
            yield return null;
            yield return null;

            SpineStiffness = spineStiffnessValue;

            yield return new WaitForSeconds(1);     // This is a horrible hack, but something after load is resetting this and I need to get to make sure this runs afterwards and can't find a better hook point...       

            SpineStiffness = spineStiffnessValue;

        }

        private bool toeAdjustmentApplied;

        private void RecurseForName(TreeNodeObject tno)
        {
            if (tno.textName != null && tno.textName.ToUpper().StartsWith("-RESIZE"))
                flagNodeObjectList.Add(tno);

            foreach (TreeNodeObject child in tno.child)
            {
                RecurseForName(child);                
            }
        }

        private IEnumerator ScanForFlagCo()
        {
            yield return new WaitUntil(() => Studio.Studio.Instance != null && Studio.Studio.Instance.dicObjectCtrl != null);
            while (this.enabled)
            {                
                flagNodeObjectList.Clear();
                if (AdvIKPlugin.EnableResizeOnFolder.Value)
                {
                    OCIChar me = StudioObjectExtensions.GetOCIChar(ChaControl);
                    if(me != null)
                        RecurseForName(me.treeNodeObject);                    
                }
                yield return new WaitForSeconds(5);
            }
        }


        private List<TreeNodeObject> flagNodeObjectList = new List<TreeNodeObject>();

        protected override void Update()
        {
            toeAdjustmentApplied = false;

            if (_breathing != null) 
                _breathing.FrameEffects = null;

            if (KKAPI.Studio.StudioAPI.InsideStudio && AdvIKPlugin.EnableResizeOnFolder.Value && flagNodeObjectList.Count > 0)
            {
                foreach (TreeNodeObject flagNodeObject in flagNodeObjectList)
                {
                    if (flagNodeObject != null && flagNodeObject.visible)
                    {
                        try
                        {
                            if (flagNodeObject.textName == null || !flagNodeObject.textName.ToUpper().StartsWith("-RESIZE"))
                            {
                                flagNodeObjectList.Remove(flagNodeObject);
                                break;
                            }

                            string reqCentroidMode = flagNodeObject.textName.ToUpper().Substring(flagNodeObject.textName.LastIndexOf(":") + 1);
#if DEBUG
                    AdvIKPlugin.Instance.Log.LogInfo($"Folder Requested Adjustment via: {reqCentroidMode}");
#endif
                            IKResizeCentroid currentCentroid = _iKResizeAdjustment.Centroid;
                            IKResizeCentroid requestedCentroid = (IKResizeCentroid)Enum.Parse(typeof(IKResizeCentroid), reqCentroidMode);
                            _iKResizeAdjustment.Centroid = requestedCentroid;
                            _iKResizeAdjustment.ApplyAdjustment(true);
                            _iKResizeAdjustment.Centroid = currentCentroid;
                            flagNodeObject.SetVisible(false);
                        }
                        catch (Exception errAny)
                        {
#if DEBUG
                    AdvIKPlugin.Instance.Log.LogInfo($"Error checking resize flag: {errAny.Message}\n{errAny.StackTrace}");
#endif
                            try
                            {
                                flagNodeObjectList.Remove(flagNodeObject);
                            }
                            catch { }
                        }
                    }
                }
            }

            base.Update();
        }

        private FullBodyBipedIK ik;
        protected void LateUpdate()
        {

            if (AdvIKPlugin.OverrideMakerIKHandling.Value && KKAPI.Maker.MakerAPI.InsideAndLoaded)
            {
                if (!ik)
                    ik = ChaControl.objAnim.GetComponent<FullBodyBipedIK>();

                if (ik && ik.enabled)
                    ik.enabled = false;
            }

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
#if !KOIKATSU && !KKS

                    ClearHoverAdjustment();
                    ApplyHeelzHoverAdjustment();

#endif
                }));
                FindSolver().OnPostSolve = (IKSolver.UpdateDelegate)Delegate.Combine(FindSolver().OnPostSolve, new IKSolver.UpdateDelegate(() =>
                    {
                        if (!toeAdjustmentApplied)
                        {
                            Transform lToe = FindLToeBone();
                            Transform rToe = FindRToeBone();
                            if (lToe != null && rToe != null)
                            {
                                if (EnableToeFKHints)
                                {
                                    Vector3 lToeTargetRotation = FindFKRotation(lToe);
                                    Vector3 rToeTargetRotation = FindFKRotation(rToe);

                                    lToe.Rotate(lToeTargetRotation, Space.Self);
                                    rToe.Rotate(rToeTargetRotation, Space.Self);
                                }
                                else
                                {
                                    lToe.Rotate(Vector3.zero, Space.Self);
                                    rToe.Rotate(Vector3.zero, Space.Self);
                                }
                            }
                            toeAdjustmentApplied = true;                            
                        }
                    }));
            }
        }

#if !KOIKATSU && !KKS

        private void ClearHoverAdjustment()
        {
            IKSolverFullBodyBiped solver = FindSolver();
            if (solver != null)
            {
                if (currentHoverAdjustment == null)
                {
                    currentHoverAdjustment = new float[solver.effectors.Length];
                    for (int i = 0; i < solver.effectors.Length; i++)
                    {
                        currentHoverAdjustment[i] = 0.0f;
                    }
                }
                for (int i = 0; i < solver.effectors.Length; i++)
                {
                    IKEffector effector = solver.effectors[i];
                    effector.target.localPosition += new Vector3(0, -1 * currentHoverAdjustment[i], 0);
                    currentHoverAdjustment[i] = 0.0f;
                }
            }
        }

        private float[] currentHoverAdjustment;
        private void ApplyHeelzHoverAdjustment()
        {
            if (ChaControl.fileStatus.clothesState[7] != 0)
                return;

            IKSolverFullBodyBiped solver = FindSolver();
            if (solver != null)
            {
                if (currentHoverAdjustment == null)
                {
                    currentHoverAdjustment = new float[solver.effectors.Length];
                    for (int i = 0; i < solver.effectors.Length; i++)
                    {
                        currentHoverAdjustment[i] = 0.0f;
                    }
                }

                if (_enableHeelzHoverAll)
                {
                    float hoverAdjustment = FindHeelzHoverAdjustment();
                    for (int i = 0; i < solver.effectors.Length; i++)
                    {
                        currentHoverAdjustment[i] = hoverAdjustment;                        
                        solver.effectors[i].target.localPosition += new Vector3(0, currentHoverAdjustment[i], 0);
                    }
                }
                else
                {
                    if (_enableHeelzHoverLeftFoot)
                    {
                        float hoverAdj = FindHeelzHoverAdjustment();
                        currentHoverAdjustment[(int)FullBodyBipedEffector.LeftFoot] = hoverAdj;
                        solver.effectors[(int)FullBodyBipedEffector.LeftFoot].target.localPosition += new Vector3(0, currentHoverAdjustment[(int)FullBodyBipedEffector.LeftFoot], 0);                        
                    }
                    if (_enableHeelzHoverRightFoot)
                    {
                        float hoverAdj = FindHeelzHoverAdjustment();
                        currentHoverAdjustment[(int)FullBodyBipedEffector.RightFoot] = hoverAdj;
                        solver.effectors[(int)FullBodyBipedEffector.RightFoot].target.localPosition += new Vector3(0, currentHoverAdjustment[(int)FullBodyBipedEffector.RightFoot], 0);
                    }
                }
            }
        }

        private static Type HeelzValueDictTypeV1 = AccessTools.TypeByName("Heels.Values");
        private static Type HeelzValueDictTypeV2 = AccessTools.TypeByName("Values");
        private static FieldInfo HeelzValueDictField;
        private static Type HeelzConfigTypeV1 = AccessTools.TypeByName("Heels.Struct.HeelsConfig");
        private static Type HeelzConfigTypeV2 = AccessTools.TypeByName("HeelConfig");
        private static FieldInfo HeelzConfigRootFieldV1;
        private static FieldInfo HeelzConfigRootFieldV2;
        private static IDictionary HeelzDict;

        private float FindHeelzHoverAdjustment()
        {
            if (HeelzValueDictTypeV1 == null && HeelzValueDictTypeV2 == null)
                return 0.0f;

            if (HeelzDict == null)
                FindHeelzDict();

            if (HeelzDict == null)
            {
                return 0.0f;
            }
            else
            {
                if (HeelzDict.Contains(ChaControl.nowCoordinate.clothes.parts[7].id))
                {
                    var shoeConfig = HeelzDict[ChaControl.nowCoordinate.clothes.parts[7].id];
                    if (HeelzConfigRootFieldV1 != null)
                        return ((Vector3)HeelzConfigRootFieldV1.GetValue(shoeConfig)).y;
                    else
                        return ((Vector3)HeelzConfigRootFieldV2.GetValue(shoeConfig)).y;
                }
                else
                {
                    return 0.0f;
                }
            }
        }

        private void FindHeelzDict()
        { 
            if (HeelzValueDictTypeV1 != null && HeelzConfigTypeV1 != null)
            {
                if (HeelzValueDictField == null)
                    HeelzValueDictField = AccessTools.Field(HeelzValueDictTypeV1, "Configs");
                if (HeelzConfigRootFieldV1 == null)
                    HeelzConfigRootFieldV1 = AccessTools.Field(HeelzConfigTypeV1, "Root");
                if (HeelzDict == null)
                    HeelzDict = (Dictionary<int, object>)HeelzValueDictField.GetValue(null);

            }
            else if (HeelzValueDictTypeV2 != null && HeelzConfigTypeV2 != null)
            {
                if (HeelzValueDictField == null)
                    HeelzValueDictField = AccessTools.Field(HeelzValueDictTypeV2, "Configs");
                if (HeelzConfigRootFieldV2 == null)
                    HeelzConfigRootFieldV2 = AccessTools.Field(HeelzConfigTypeV2, "rootMove");
                if (HeelzDict == null)
                {
                    HeelzDict = (IDictionary)HeelzValueDictField.GetValue(null);
                }
            }             
        }

#endif

        public Vector3 FindFKRotation(Transform t)
        {            
            if (StudioAPI.InsideStudio && t != null)
            {
                foreach (OCIChar.BoneInfo bone in this.ChaControl.GetOCIChar().listBones)
                {
                    if (bone.guideObject.transformTarget != null && bone.guideObject.transformTarget.name.Equals(t.name, StringComparison.OrdinalIgnoreCase))
                    {
                        return bone.guideObject.changeAmount.rot;
                    }
                }
            }
            return Vector3.zero;
        }

   

        protected override void OnEnable()
        {
            StartCoroutine("StartBreathing", new PluginData());
            if (KKAPI.Studio.StudioAPI.InsideStudio)
                StartCoroutine(ScanForFlagCo());
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
#if KOIKATSU || KKS
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
#if KOIKATSU || KKS
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
#if KOIKATSU || KKS
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

        private Transform FindNeck()
        {

            if (FindAnimator())
            {
#if KOIKATSU || KKS
                return FindDescendant(FindAnimator().transform, "cf_j_neck");
#else
                return FindDescendant(FindAnimator().transform, "cf_J_Neck");
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
#if KOIKATSU || KKS
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
#if KOIKATSU || KKS
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
#if KOIKATSU || KKS
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
#if KOIKATSU || KKS
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
#if KOIKATSU || KKS
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
#if KOIKATSU || KKS
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
#if KOIKATSU || KKS
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
#if KOIKATSU || KKS
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
#if KOIKATSU || KKS
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
#if KOIKATSU || KKS
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

        public Transform FindLToeBone()
        {
            if (FindAnimator())
            {
#if KOIKATSU || KKS
                return FindDescendant(FindAnimator().transform, "cf_j_toes_L");
#else
                return FindDescendant(FindAnimator().transform, "cf_J_Toes01_L");
#endif
            }
            else
            {
                return null;
            }
        }

        public Transform FindRToeBone()
        {
            if (FindAnimator())
            {
#if KOIKATSU || KKS
                return FindDescendant(FindAnimator().transform, "cf_j_toes_R");
#else
                return FindDescendant(FindAnimator().transform, "cf_J_Toes01_R");
#endif
            }
            else
            {
                return null;
            }
        }

        public Transform FindDescendant(Transform start, string name)
        {
            if (boneCache.TryGetValue(name, out Transform descendant))
                return descendant;

            if (start == null)
            {
                return null;
            }

            if (start.name.Equals(name))
            {
                boneCache.Add(name, start);
                return start;
            }
            foreach (Transform t in start)
            {
                Transform res = FindDescendant(t, name);
                if (res != null)
                {
                    if (!boneCache.ContainsKey(name))
                        boneCache.Add(name, res);
                    return res;
                }
            }
            return null;
        }

        private Dictionary<string, Transform> boneCache = new Dictionary<string, Transform>();

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
