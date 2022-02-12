using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KKAPI.Studio;
using Studio;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Reflection;
using System.IO;
using UniRx;
using HarmonyLib;
using IllusionUtility.SetUtility;
using Illusion.Extensions;
using UniRx.Triggers;
using System.Collections;
using GUITree;

namespace AdvIKPlugin
{
    public partial class AdvIKPlugin
    {
        internal class AdvIKGUI
        {
            private static GameObject AdvIKPanel;
            private static GameObject BreathPanel;
            private static GameObject BreathShapePanel;
            private static GameObject ResizePanel;
            


            private static Toggle ShoulderRotatorToggle;
            private static Toggle IndependentShoulderToggle;
            private static Toggle ReverseShoulderLToggle;
            private static Toggle ReverseShoulderRToggle;
            private static Slider Weight;
            private static Slider WeightRight;
            private static Slider Offset;
            private static Slider OffsetRight;
            private static Slider SpineStiffness;
            private static Text weightSliderText;
            private static Text weightRightSliderText;
            private static Text offsetSliderText;
            private static Text offsetRightSliderText;
            private static Text spineSliderText;
            private static Toggle spineFKHintsToggle;
            private static Toggle shoulderFKHintsToggle;
            private static Toggle toeFKHintsToggle;
            private static Toggle heelzAllToggle;
            private static Toggle heelzLToggle;
            private static Toggle heelzRToggle;

            private static Toggle BreathingToggle;

            private static Text intakeText;
            private static Slider intakeSlider;
            private static Text holdText;
            private static Slider holdSlider;
            private static Text inhaleText;
            private static Slider inhaleSlider;
            private static Text bpmText;
            private static Slider bpmSlider;
            private static Text shoulderDampText;
            private static Slider shoulderDampSlider;
            private static Text magnitudeText;
            private static Slider magnitudeSlider;

            private static Text breathShapeX;
            private static Text breathShapeY;
            private static Text breathShapeZ;

            private static Slider breathShapeXSlider;
            private static Slider breathShapeYSlider;
            private static Slider breathShapeZSlider;

            private static Text upperChestShapeX;
            private static Text upperChestShapeY;
            private static Text upperChestShapeZ;

            private static Slider upperChestShapeXSlider;
            private static Slider upperChestShapeYSlider;
            private static Slider upperChestShapeZSlider;

            private static Text lowerChestShapeX;
            private static Text lowerChestShapeY;
            private static Text lowerChestShapeZ;

            private static Slider lowerChestShapeXSlider;
            private static Slider lowerChestShapeYSlider;
            private static Slider lowerChestShapeZSlider;

            private static Text abdomenShapeX;
            private static Text abdomenShapeY;
            private static Text abdomenShapeZ;

            private static Slider abdomenShapeXSlider;
            private static Slider abdomenShapeYSlider;
            private static Slider abdomenShapeZSlider;

            private static Slider NeckMotionSlider;

            // Resize Controls

            private static Toggle resizeCentroid_None;
            private static Toggle resizeCentroid_Auto;
            private static Toggle resizeCentroid_Body;

            private static Toggle resizeCentroid_FeetCenter;
            private static Toggle resizeCentroid_FeetLeft;
            private static Toggle resizeCentroid_FeetRight;

            private static Toggle resizeCentroid_ThighCenter;
            private static Toggle resizeCentroid_ThighLeft;
            private static Toggle resizeCentroid_ThighRight;

            private static Toggle resizeCentroid_ShoulderCenter;
            private static Toggle resizeCentroid_ShoulderLeft;
            private static Toggle resizeCentroid_ShoulderRight;

            private static Toggle resizeCentroid_HandCenter;
            private static Toggle resizeCentroid_HandLeft;
            private static Toggle resizeCentroid_HandRight;

            private static Toggle resizeCentroid_KneeCenter;
            private static Toggle resizeCentroid_KneeLeft;
            private static Toggle resizeCentroid_KneeRight;

            private static Toggle resizeCentroid_ElbowCenter;
            private static Toggle resizeCentroid_ElbowLeft;
            private static Toggle resizeCentroid_ElbowRight;
            private static Toggle resizeCentroid_Resize;

            private static Toggle resizeChainMode_LeftArm;
            private static Toggle resizeChainMode_RightArm;
            private static Toggle resizeChainMode_LeftLeg;
            private static Toggle resizeChainMode_RightLeg;            

            private static Button resizeButton;
            private static TextMeshProUGUI resizeButtonText;

            private static bool showAdvIKPanel = true;
            private static bool showResizePanel = false;

            private static Button breathPanelShowResizeButton;
            private static Button breathPanelShowIKOptsButton;
            private static Button openShapePanelButton;

            private static Button advIKPanelShowBreathButton;
            private static Button advIKPanelShowResizeButton;

            private static Button resizePanelShowIKOptsButton;
            private static Button resizePanelShowBreathButton;


            private static OCIChar selectedChar;

            internal static void InitUI()
            {
                if (AdvIKPanel == null)
                {
                    CreateMenuEntry();
                    CreatePanel(AdvIKPanel);
                    CreatePanel(BreathPanel);
                    CreatePanel(BreathShapePanel);
                    CreatePanel(ResizePanel);
                    SetupAdvIKControls();
                    SetupBreathControls();
                    SetupBreathShapeControls();
                    SetupResizeControls();
                }
            }

            internal static void UpdateUI(OCIChar _char)
            {
                selectedChar = _char;
#if DEBUG
                AdvIKPlugin.Instance.Log.LogInfo(string.Format("Selected Char: {0}", selectedChar?.charInfo.name));
#endif
                if (selectedChar != null)
                {
                    AdvIKCharaController advIKController = selectedChar.charInfo.gameObject.GetComponent<AdvIKCharaController>();
                    ShoulderRotatorToggle.isOn = advIKController.ShoulderRotationEnabled;
                    IndependentShoulderToggle.isOn = advIKController.IndependentShoulders;
                    ReverseShoulderLToggle.isOn = advIKController.ReverseShoulderL;
                    ReverseShoulderRToggle.isOn = advIKController.ReverseShoulderR;
                    Weight.value = advIKController.ShoulderWeight;
                    WeightRight.value = advIKController.ShoulderRightWeight;
                    Offset.value = advIKController.ShoulderOffset;
                    OffsetRight.value = advIKController.ShoulderRightOffset;
                    SpineStiffness.value = advIKController.SpineStiffness;
                    spineFKHintsToggle.isOn = advIKController.EnableSpineFKHints;
                    shoulderFKHintsToggle.isOn = advIKController.EnableShoulderFKHints;
                    toeFKHintsToggle.isOn = advIKController.EnableToeFKHints;
                    heelzAllToggle.isOn = advIKController.EnableHeelzHoverAll;
                    heelzLToggle.isOn = advIKController.EnableHeelzHoverLeftFoot;
                    heelzRToggle.isOn = advIKController.EnableHeelzHoverRightFoot;

                    if (advIKController.BreathingController != null)
                    {
                        BreathingToggle.isOn = advIKController.BreathingController.Enabled;
                        intakeSlider.value = advIKController.BreathingController.IntakePause;
                        holdSlider.value = advIKController.BreathingController.HoldPause;
                        inhaleSlider.value = advIKController.BreathingController.InhalePercentage;
                        bpmSlider.value = advIKController.BreathingController.BreathsPerMinute;
                        shoulderDampSlider.value = advIKController.BreathingController.ShoulderDampeningFactor;
                        magnitudeSlider.value = advIKController.BreathingController.MagnitudeFactor;

                        breathShapeXSlider.value = advIKController.BreathingController.BreathMagnitude.x;
                        breathShapeYSlider.value = advIKController.BreathingController.BreathMagnitude.y;
                        breathShapeZSlider.value = advIKController.BreathingController.BreathMagnitude.z;

                        upperChestShapeXSlider.value = advIKController.BreathingController.UpperChestRelativeScaling.x;
                        upperChestShapeYSlider.value = advIKController.BreathingController.UpperChestRelativeScaling.y;
                        upperChestShapeZSlider.value = advIKController.BreathingController.UpperChestRelativeScaling.z;

                        lowerChestShapeXSlider.value = advIKController.BreathingController.LowerChestRelativeScaling.x;
                        lowerChestShapeYSlider.value = advIKController.BreathingController.LowerChestRelativeScaling.y;
                        lowerChestShapeZSlider.value = advIKController.BreathingController.LowerChestRelativeScaling.z;

                        abdomenShapeXSlider.value = advIKController.BreathingController.AbdomenRelativeScaling.x;
                        abdomenShapeYSlider.value = advIKController.BreathingController.AbdomenRelativeScaling.y;
                        abdomenShapeZSlider.value = advIKController.BreathingController.AbdomenRelativeScaling.z;

                        NeckMotionSlider.value = advIKController.BreathingController.NeckMotionDampeningFactor;
                    }

                    updatingResizeCentroidControls = true;
                    ClearResizeCentroidControls();
                    if (advIKController.IKResizeController != null)
                    {
                        switch (advIKController.IKResizeController.Centroid)
                        {
                            case Algos.IKResizeCentroid.AUTO:
                                resizeCentroid_Auto.isOn = true;
                                break;
                            case Algos.IKResizeCentroid.NONE:
                                resizeCentroid_None.isOn = true;
                                break;
                            case Algos.IKResizeCentroid.BODY:
                                resizeCentroid_Body.isOn = true;
                                break;
                            case Algos.IKResizeCentroid.FEET_CENTER:
                                resizeCentroid_FeetCenter.isOn = true;
                                break;
                            case Algos.IKResizeCentroid.FEET_LEFT:
                                resizeCentroid_FeetLeft.isOn = true;
                                break;
                            case Algos.IKResizeCentroid.FEET_RIGHT:
                                resizeCentroid_FeetRight.isOn = true;
                                break;
                            case Algos.IKResizeCentroid.THIGH_CENTER:
                                resizeCentroid_ThighCenter.isOn = true;
                                break;
                            case Algos.IKResizeCentroid.THIGH_LEFT:
                                resizeCentroid_ThighLeft.isOn = true;
                                break;
                            case Algos.IKResizeCentroid.THIGH_RIGHT:
                                resizeCentroid_ThighRight.isOn = true;
                                break;
                            case Algos.IKResizeCentroid.HAND_CENTER:
                                resizeCentroid_HandCenter.isOn = true;
                                break;
                            case Algos.IKResizeCentroid.HAND_LEFT:
                                resizeCentroid_HandLeft.isOn = true;
                                break;
                            case Algos.IKResizeCentroid.HAND_RIGHT:
                                resizeCentroid_HandRight.isOn = true;
                                break;
                            case Algos.IKResizeCentroid.SHOULDER_CENTER:
                                resizeCentroid_ShoulderCenter.isOn = true;
                                break;
                            case Algos.IKResizeCentroid.SHOULDER_LEFT:
                                resizeCentroid_ShoulderLeft.isOn = true;
                                break;
                            case Algos.IKResizeCentroid.SHOULDER_RIGHT:
                                resizeCentroid_ShoulderRight.isOn = true;
                                break;
                            case Algos.IKResizeCentroid.KNEE_CENTER:
                                resizeCentroid_KneeCenter.isOn = true;
                                break;
                            case Algos.IKResizeCentroid.KNEE_LEFT:
                                resizeCentroid_KneeLeft.isOn = true;
                                break;
                            case Algos.IKResizeCentroid.KNEE_RIGHT:
                                resizeCentroid_KneeRight.isOn = true;
                                break;
                            case Algos.IKResizeCentroid.ELBOW_CENTER:
                                resizeCentroid_ElbowCenter.isOn = true;
                                break;
                            case Algos.IKResizeCentroid.ELBOW_LEFT:
                                resizeCentroid_ElbowLeft.isOn = true;
                                break;
                            case Algos.IKResizeCentroid.ELBOW_RIGHT:
                                resizeCentroid_ElbowRight.isOn = true;
                                break;
                            case Algos.IKResizeCentroid.RESIZE:
                                resizeCentroid_Resize.isOn = true;
                                break;

                        }
                        Algos.IKResizeChainAdjustment leftArmAdjustment;
                        if (!advIKController.IKResizeController.ChainAdjustments.TryGetValue(Algos.IKChain.LEFT_ARM, out leftArmAdjustment))
                            leftArmAdjustment = Algos.IKResizeChainAdjustment.CHAIN;
                        Algos.IKResizeChainAdjustment rightArmAdjustment;
                        if (!advIKController.IKResizeController.ChainAdjustments.TryGetValue(Algos.IKChain.RIGHT_ARM, out rightArmAdjustment))
                            rightArmAdjustment = Algos.IKResizeChainAdjustment.CHAIN;
                        Algos.IKResizeChainAdjustment leftLegAdjustment;
                        if (!advIKController.IKResizeController.ChainAdjustments.TryGetValue(Algos.IKChain.LEFT_LEG, out leftLegAdjustment))
                            leftLegAdjustment = Algos.IKResizeChainAdjustment.CHAIN;
                        Algos.IKResizeChainAdjustment rightLegAdjustment;
                        if (!advIKController.IKResizeController.ChainAdjustments.TryGetValue(Algos.IKChain.RIGHT_LEG, out rightLegAdjustment))
                            rightLegAdjustment = Algos.IKResizeChainAdjustment.CHAIN;

                        resizeChainMode_LeftArm.isOn = leftArmAdjustment == Algos.IKResizeChainAdjustment.CHAIN;
                        resizeChainMode_RightArm.isOn = rightArmAdjustment == Algos.IKResizeChainAdjustment.CHAIN;
                        resizeChainMode_LeftLeg.isOn = leftLegAdjustment == Algos.IKResizeChainAdjustment.CHAIN;
                        resizeChainMode_RightLeg.isOn = rightLegAdjustment == Algos.IKResizeChainAdjustment.CHAIN;

                        if (advIKController.IKResizeController.AdjustmentApplied)
                            resizeButtonText.text = "Undo Resize Adjustment";
                        else
                            resizeButtonText.text = "Apply Resize Adjustment";
                    }

                    updatingResizeCentroidControls = false;
                }
            }

            public static void SetupBreathShapeControls()
            {

                BreathShapePanel.transform.localPosition = new Vector3(340, BreathShapePanel.transform.localPosition.y);
                BreathShapePanel.SetActive(false);

                Text breathShape = SetupText("BreathShape", -30, "Overall Breath Scale", BreathShapePanel);
                breathShape.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 300);
                breathShape.fontSize = 16;

                breathShapeX = SetupText("BreathShapeX", -50, "X", BreathShapePanel);
                breathShapeX.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100);
                breathShapeX.fontSize = 16;

                breathShapeY = SetupText("BreathShapeY", -50, "Y", BreathShapePanel);
                breathShapeY.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100);
                breathShapeY.transform.localPosition = new Vector3(80, breathShapeY.transform.localPosition.y);
                breathShapeY.fontSize = 16;

                breathShapeZ = SetupText("BreathShapeZ", -50, "Z", BreathShapePanel);
                breathShapeZ.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100);
                breathShapeZ.transform.localPosition = new Vector3(150, breathShapeZ.transform.localPosition.y);
                breathShapeZ.fontSize = 16;

                var sldSize = GetPanelObject<Slider>("Slider Size", BreathShapePanel);

                breathShapeXSlider = Instantiate(sldSize, BreathShapePanel.transform);
                breathShapeXSlider.name = "BreathShapeX";
                breathShapeXSlider.GetComponent<RectTransform>().sizeDelta = new Vector2(75, 20);
                breathShapeXSlider.transform.SetLocalScale(1f, 1.0f, 1.0f);
                breathShapeXSlider.transform.SetLocalPosition(0, -70, 0);
                breathShapeXSlider.minValue = 0f;
                breathShapeXSlider.maxValue = 3f;

                breathShapeXSlider.onValueChanged.RemoveAllListeners();
                breathShapeXSlider.onValueChanged.AddListener(delegate (float value)
                {
                    if (selectedChar != null)
                    {
                        Vector3 currentBreathingMagnitude = selectedChar.charInfo.gameObject.GetComponent<AdvIKCharaController>().BreathingController.BreathMagnitude;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.BreathingController.BreathMagnitude = new Vector3(value, currentBreathingMagnitude.y, currentBreathingMagnitude.z);
                        }                        
                        breathShapeX.text = string.Format("X ({0:0.000})", value);
                    }
                });

                breathShapeYSlider = Instantiate(sldSize, BreathShapePanel.transform);
                breathShapeYSlider.name = "BreathShapeY";
                breathShapeYSlider.GetComponent<RectTransform>().sizeDelta = new Vector2(75, 20);
                breathShapeYSlider.transform.SetLocalScale(1f, 1.0f, 1.0f);
                breathShapeYSlider.transform.SetLocalPosition(70, -70, 0);
                breathShapeYSlider.minValue = 0f;
                breathShapeYSlider.maxValue = 3f;

                breathShapeYSlider.onValueChanged.RemoveAllListeners();
                breathShapeYSlider.onValueChanged.AddListener(delegate (float value)
                {
                    if (selectedChar != null)
                    {
                        Vector3 currentBreathingMagnitude = selectedChar.charInfo.gameObject.GetComponent<AdvIKCharaController>().BreathingController.BreathMagnitude;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.BreathingController.BreathMagnitude = new Vector3(currentBreathingMagnitude.x, value, currentBreathingMagnitude.z);
                        }                        
                        breathShapeY.text = string.Format("Y ({0:0.000})", value);
                    }
                });

                breathShapeZSlider = Instantiate(sldSize, BreathShapePanel.transform);
                breathShapeZSlider.name = "BreathShapeZ";
                breathShapeZSlider.GetComponent<RectTransform>().sizeDelta = new Vector2(75, 20);
                breathShapeZSlider.transform.SetLocalScale(1f, 1.0f, 1.0f);
                breathShapeZSlider.transform.SetLocalPosition(140, -70, 0);
                breathShapeZSlider.minValue = 0f;
                breathShapeZSlider.maxValue = 3f;

                breathShapeZSlider.onValueChanged.RemoveAllListeners();
                breathShapeZSlider.onValueChanged.AddListener(delegate (float value)
                {
                    if (selectedChar != null)
                    {
                        Vector3 currentBreathingMagnitude = selectedChar.charInfo.gameObject.GetComponent<AdvIKCharaController>().BreathingController.BreathMagnitude;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.BreathingController.BreathMagnitude = new Vector3(currentBreathingMagnitude.x, currentBreathingMagnitude.y, value);
                        }
                        breathShapeZ.text = string.Format("Z ({0:0.000})", value);
                    }
                });


                Text upperChestShape = SetupText("UpperChestShape", -100, "Upper Chest Scale", BreathShapePanel);
                upperChestShape.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 300);
                upperChestShape.fontSize = 16;

                upperChestShapeX = SetupText("UpperChestX", -120, "X", BreathShapePanel);
                upperChestShapeX.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100);
                upperChestShapeX.fontSize = 16;

                upperChestShapeY = SetupText("UpperChestY", -120, "Y", BreathShapePanel);
                upperChestShapeY.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100);
                upperChestShapeY.transform.localPosition = new Vector3(80, upperChestShapeY.transform.localPosition.y);
                upperChestShapeY.fontSize = 16;

                upperChestShapeZ = SetupText("UpperChestZ", -120, "Z", BreathShapePanel);
                upperChestShapeZ.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100);
                upperChestShapeZ.transform.localPosition = new Vector3(150, upperChestShapeZ.transform.localPosition.y);
                upperChestShapeZ.fontSize = 16;

                upperChestShapeXSlider = Instantiate(sldSize, BreathShapePanel.transform);
                upperChestShapeXSlider.name = "UpperChestX";
                upperChestShapeXSlider.GetComponent<RectTransform>().sizeDelta = new Vector2(75, 20);
                upperChestShapeXSlider.transform.SetLocalScale(1f, 1.0f, 1.0f);
                upperChestShapeXSlider.transform.SetLocalPosition(0, -140, 0);
                upperChestShapeXSlider.minValue = 0f;
                upperChestShapeXSlider.maxValue = 0.3f;

                upperChestShapeXSlider.onValueChanged.RemoveAllListeners();
                upperChestShapeXSlider.onValueChanged.AddListener(delegate (float value)
                {
                    if (selectedChar != null)
                    {
                        Vector3 upperChestRelative = selectedChar.charInfo.gameObject.GetComponent<AdvIKCharaController>().BreathingController.UpperChestRelativeScaling;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.BreathingController.UpperChestRelativeScaling = new Vector3(value, upperChestRelative.y, upperChestRelative.z);
                        }
                        upperChestShapeX.text = string.Format("X ({0:0.000})", value);
                    }
                });

                upperChestShapeYSlider = Instantiate(sldSize, BreathShapePanel.transform);
                upperChestShapeYSlider.name = "UpperChestY";
                upperChestShapeYSlider.GetComponent<RectTransform>().sizeDelta = new Vector2(75, 20);
                upperChestShapeYSlider.transform.SetLocalScale(1f, 1.0f, 1.0f);
                upperChestShapeYSlider.transform.SetLocalPosition(70, -140, 0);
                upperChestShapeYSlider.minValue = 0f;
                upperChestShapeYSlider.maxValue = 0.3f;

                upperChestShapeYSlider.onValueChanged.RemoveAllListeners();
                upperChestShapeYSlider.onValueChanged.AddListener(delegate (float value)
                {
                    if (selectedChar != null)
                    {
                        Vector3 upperChestRelative = selectedChar.charInfo.gameObject.GetComponent<AdvIKCharaController>().BreathingController.UpperChestRelativeScaling;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.BreathingController.UpperChestRelativeScaling = new Vector3(upperChestRelative.x, value, upperChestRelative.z);
                        }
                        upperChestShapeY.text = string.Format("Y ({0:0.000})", value);
                    }
                });

                upperChestShapeZSlider = Instantiate(sldSize, BreathShapePanel.transform);
                upperChestShapeZSlider.name = "UpperChestZ";
                upperChestShapeZSlider.GetComponent<RectTransform>().sizeDelta = new Vector2(75, 20);
                upperChestShapeZSlider.transform.SetLocalScale(1f, 1.0f, 1.0f);
                upperChestShapeZSlider.transform.SetLocalPosition(140, -140, 0);
                upperChestShapeZSlider.minValue = 0f;
                upperChestShapeZSlider.maxValue = 0.3f;

                upperChestShapeZSlider.onValueChanged.RemoveAllListeners();
                upperChestShapeZSlider.onValueChanged.AddListener(delegate (float value)
                {
                    if (selectedChar != null)
                    {
                        Vector3 upperChestRelative = selectedChar.charInfo.gameObject.GetComponent<AdvIKCharaController>().BreathingController.UpperChestRelativeScaling;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.BreathingController.UpperChestRelativeScaling = new Vector3(upperChestRelative.x, upperChestRelative.y, value);
                        }
                        upperChestShapeZ.text = string.Format("Z ({0:0.000})", value);
                    }
                });


                Text lowerChestShape = SetupText("LowerChestShape", -170, "Lower Chest Scale", BreathShapePanel);
                lowerChestShape.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 300);
                lowerChestShape.fontSize = 16;

                lowerChestShapeX = SetupText("LowerChestX", -190, "X", BreathShapePanel);
                lowerChestShapeX.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100);
                lowerChestShapeX.fontSize = 16;

                lowerChestShapeY = SetupText("LowerChestY", -190, "Y", BreathShapePanel);
                lowerChestShapeY.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100);
                lowerChestShapeY.transform.localPosition = new Vector3(80, lowerChestShapeY.transform.localPosition.y);
                lowerChestShapeY.fontSize = 16;

                lowerChestShapeZ = SetupText("LowerChestZ", -190, "Z", BreathShapePanel);
                lowerChestShapeZ.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100);
                lowerChestShapeZ.transform.localPosition = new Vector3(150, lowerChestShapeZ.transform.localPosition.y);
                lowerChestShapeZ.fontSize = 16;

                lowerChestShapeXSlider = Instantiate(sldSize, BreathShapePanel.transform);
                lowerChestShapeXSlider.name = "LowerChestX";
                lowerChestShapeXSlider.GetComponent<RectTransform>().sizeDelta = new Vector2(75, 20);
                lowerChestShapeXSlider.transform.SetLocalScale(1f, 1.0f, 1.0f);
                lowerChestShapeXSlider.transform.SetLocalPosition(0, -210, 0);
                lowerChestShapeXSlider.minValue = 0f;
                lowerChestShapeXSlider.maxValue = 0.3f;

                lowerChestShapeXSlider.onValueChanged.RemoveAllListeners();
                lowerChestShapeXSlider.onValueChanged.AddListener(delegate (float value)
                {
                    if (selectedChar != null)
                    {
                        Vector3 lowerChestRelativeScaling = selectedChar.charInfo.gameObject.GetComponent<AdvIKCharaController>().BreathingController.LowerChestRelativeScaling;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.BreathingController.LowerChestRelativeScaling = new Vector3(value, lowerChestRelativeScaling.y, lowerChestRelativeScaling.z);
                        }
                        lowerChestShapeX.text = string.Format("X ({0:0.000})", value);
                    }
                });

                lowerChestShapeYSlider = Instantiate(sldSize, BreathShapePanel.transform);
                lowerChestShapeYSlider.name = "LowerChestY";
                lowerChestShapeYSlider.GetComponent<RectTransform>().sizeDelta = new Vector2(75, 20);
                lowerChestShapeYSlider.transform.SetLocalScale(1f, 1.0f, 1.0f);
                lowerChestShapeYSlider.transform.SetLocalPosition(70, -210, 0);
                lowerChestShapeYSlider.minValue = 0f;
                lowerChestShapeYSlider.maxValue = 0.3f;

                lowerChestShapeYSlider.onValueChanged.RemoveAllListeners();
                lowerChestShapeYSlider.onValueChanged.AddListener(delegate (float value)
                {
                    if (selectedChar != null)
                    {
                        Vector3 lowerChestRelativeScaling = selectedChar.charInfo.gameObject.GetComponent<AdvIKCharaController>().BreathingController.LowerChestRelativeScaling;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.BreathingController.LowerChestRelativeScaling = new Vector3(lowerChestRelativeScaling.x, value, lowerChestRelativeScaling.z);
                        }
                        lowerChestShapeY.text = string.Format("Y ({0:0.000})", value);
                    }
                });

                lowerChestShapeZSlider = Instantiate(sldSize, BreathShapePanel.transform);
                lowerChestShapeZSlider.name = "LowerChestZ";
                lowerChestShapeZSlider.GetComponent<RectTransform>().sizeDelta = new Vector2(75, 20);
                lowerChestShapeZSlider.transform.SetLocalScale(1f, 1.0f, 1.0f);
                lowerChestShapeZSlider.transform.SetLocalPosition(140, -210, 0);
                lowerChestShapeZSlider.minValue = 0f;
                lowerChestShapeZSlider.maxValue = 0.3f;

                lowerChestShapeZSlider.onValueChanged.RemoveAllListeners();
                lowerChestShapeZSlider.onValueChanged.AddListener(delegate (float value)
                {
                    if (selectedChar != null)
                    {
                        Vector3 lowerChestRelativeScaling = selectedChar.charInfo.gameObject.GetComponent<AdvIKCharaController>().BreathingController.LowerChestRelativeScaling;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.BreathingController.LowerChestRelativeScaling = new Vector3(lowerChestRelativeScaling.x, lowerChestRelativeScaling.y, value);
                        }
                        lowerChestShapeZ.text = string.Format("Z ({0:0.000})", value);
                    }
                });


                Text abdomenShape = SetupText("AbdomenShape", -240, "Abdomen Scaling", BreathShapePanel);
                abdomenShape.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 300);
                abdomenShape.fontSize = 16;

                abdomenShapeX = SetupText("AbdomenShapeX", -260, "X", BreathShapePanel);
                abdomenShapeX.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100);
                abdomenShapeX.fontSize = 16;

                abdomenShapeY = SetupText("AbdomenShapeY", -260, "Y", BreathShapePanel);
                abdomenShapeY.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100);
                abdomenShapeY.transform.localPosition = new Vector3(80, abdomenShapeY.transform.localPosition.y);
                abdomenShapeY.fontSize = 16;

                abdomenShapeZ = SetupText("AbdomenShapeZ", -260, "Z", BreathShapePanel);
                abdomenShapeZ.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100);
                abdomenShapeZ.transform.localPosition = new Vector3(150, abdomenShapeZ.transform.localPosition.y);
                abdomenShapeZ.fontSize = 16;

                abdomenShapeXSlider = Instantiate(sldSize, BreathShapePanel.transform);
                abdomenShapeXSlider.name = "AbdomenShapeX";
                abdomenShapeXSlider.GetComponent<RectTransform>().sizeDelta = new Vector2(75, 20);
                abdomenShapeXSlider.transform.SetLocalScale(1f, 1.0f, 1.0f);
                abdomenShapeXSlider.transform.SetLocalPosition(0, -280, 0);
                abdomenShapeXSlider.minValue = -0.2f;
                abdomenShapeXSlider.maxValue = 0.2f;

                abdomenShapeXSlider.onValueChanged.RemoveAllListeners();
                abdomenShapeXSlider.onValueChanged.AddListener(delegate (float value)
                {
                    if (selectedChar != null)
                    {
                        Vector3 abdomenRelativeScaling = selectedChar.charInfo.gameObject.GetComponent<AdvIKCharaController>().BreathingController.AbdomenRelativeScaling;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.BreathingController.AbdomenRelativeScaling = new Vector3(value, abdomenRelativeScaling.y, abdomenRelativeScaling.z);
                        }
                        abdomenShapeX.text = string.Format("X ({0:0.000})", value);
                    }
                });

                abdomenShapeYSlider = Instantiate(sldSize, BreathShapePanel.transform);
                abdomenShapeYSlider.name = "AbdomenShapeY";
                abdomenShapeYSlider.GetComponent<RectTransform>().sizeDelta = new Vector2(75, 20);
                abdomenShapeYSlider.transform.SetLocalScale(1f, 1.0f, 1.0f);
                abdomenShapeYSlider.transform.SetLocalPosition(70, -280, 0);
                abdomenShapeYSlider.minValue = -0.2f;
                abdomenShapeYSlider.maxValue = 0.2f;

                abdomenShapeYSlider.onValueChanged.RemoveAllListeners();
                abdomenShapeYSlider.onValueChanged.AddListener(delegate (float value)
                {
                    if (selectedChar != null)
                    {
                        Vector3 abdomenRelativeScaling = selectedChar.charInfo.gameObject.GetComponent<AdvIKCharaController>().BreathingController.AbdomenRelativeScaling;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.BreathingController.AbdomenRelativeScaling = new Vector3(abdomenRelativeScaling.x, value, abdomenRelativeScaling.z);
                        }
                        abdomenShapeY.text = string.Format("Y ({0:0.000})", value);
                    }
                });

                abdomenShapeZSlider = Instantiate(sldSize, BreathShapePanel.transform);
                abdomenShapeZSlider.name = "AbdomenShapeZ";
                abdomenShapeZSlider.GetComponent<RectTransform>().sizeDelta = new Vector2(75, 20);
                abdomenShapeZSlider.transform.SetLocalScale(1f, 1.0f, 1.0f);
                abdomenShapeZSlider.transform.SetLocalPosition(140, -280, 0);
                abdomenShapeZSlider.minValue = -0.2f;
                abdomenShapeZSlider.maxValue = 0.2f;

                abdomenShapeZSlider.onValueChanged.RemoveAllListeners();
                abdomenShapeZSlider.onValueChanged.AddListener(delegate (float value)
                {
                    if (selectedChar != null)
                    {
                        Vector3 abdomenRelativeScaling = selectedChar.charInfo.gameObject.GetComponent<AdvIKCharaController>().BreathingController.AbdomenRelativeScaling;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.BreathingController.AbdomenRelativeScaling = new Vector3(abdomenRelativeScaling.x, abdomenRelativeScaling.y, value);
                        }
                        abdomenShapeZ.text = string.Format("Z ({0:0.000})", value);
                    }
                });

                Text neckMotionSliderText = SetupText("Neck Motion", -310, "Neck Motion", BreathShapePanel);
                neckMotionSliderText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
                neckMotionSliderText.fontSize = 16;
                NeckMotionSlider = Instantiate(sldSize, BreathShapePanel.transform);
                NeckMotionSlider.name = "NeckMotion";
                NeckMotionSlider.GetComponent<RectTransform>().sizeDelta = new Vector2(75, 20);
                NeckMotionSlider.transform.SetLocalScale(1f, 1.0f, 1.0f);
                NeckMotionSlider.transform.SetLocalPosition(150, -310, 0);
                NeckMotionSlider.minValue = 0.0f;
                NeckMotionSlider.maxValue = 1.0f;

                NeckMotionSlider.onValueChanged.RemoveAllListeners();
                NeckMotionSlider.onValueChanged.AddListener(delegate (float value)
                {
                    if (selectedChar != null)
                    {
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.BreathingController.NeckMotionDampeningFactor = value;
                        }
                        neckMotionSliderText.text = string.Format("Neck Motion: ({0:0.00})", value);
                    }
                });

#if KOIKATSU || KKS
// KK Skeleton does not require neck adjustments for upper chest movement as the upper chest has a different scaling center system.

                neckMotionSliderText.gameObject.SetActive(false);
                NeckMotionSlider.gameObject.SetActive(false);

#endif

                // Clear controls
                foreach (Transform child in BreathShapePanel.transform)
                {
                    if (child.gameObject != breathShapeX.gameObject && child.gameObject != breathShapeY.gameObject && child.gameObject != breathShapeZ.gameObject && child.gameObject != breathShape.gameObject
                         && child.gameObject != breathShapeXSlider.gameObject && child.gameObject != breathShapeYSlider.gameObject && child.gameObject != breathShapeZSlider.gameObject

                         && child.gameObject != upperChestShapeX.gameObject && child.gameObject != upperChestShapeXSlider.gameObject && child.gameObject != upperChestShape.gameObject
                         && child.gameObject != upperChestShapeY.gameObject && child.gameObject != upperChestShapeYSlider.gameObject
                         && child.gameObject != upperChestShapeZ.gameObject && child.gameObject != upperChestShapeZSlider.gameObject
                         && child.gameObject != lowerChestShape.gameObject && child.gameObject != lowerChestShapeX.gameObject && child.gameObject != lowerChestShapeXSlider.gameObject
                         && child.gameObject != lowerChestShapeY.gameObject && child.gameObject != lowerChestShapeYSlider.gameObject
                         && child.gameObject != lowerChestShapeZ.gameObject && child.gameObject != lowerChestShapeZSlider.gameObject
                         && child.gameObject != abdomenShape.gameObject && child.gameObject != abdomenShapeX.gameObject && child.gameObject != abdomenShapeXSlider.gameObject
                         && child.gameObject != abdomenShapeY.gameObject && child.gameObject != abdomenShapeYSlider.gameObject
                         && child.gameObject != abdomenShapeZ.gameObject && child.gameObject != abdomenShapeZSlider.gameObject
                         && child.gameObject != neckMotionSliderText.gameObject && child.gameObject != NeckMotionSlider.gameObject
                        )
                    {
                        Destroy(child.gameObject);
                    }
                }
            }

            public static void SetupResizeControls()
            {
                GameObject fkButton = GameObject.Find("StudioScene/Canvas Main Menu/02_Manipulate/00_Chara/02_Kinematic/Viewport/Content/FK");
                var ikOptsButtonGO = Instantiate(fkButton, ResizePanel.transform);
                ikOptsButtonGO.name = "IKS Opts";
                ikOptsButtonGO.transform.localPosition = new Vector3(45, -65, 0);


                TextMeshProUGUI tmp = ikOptsButtonGO.transform.GetChild(0).GetComponentInChildren(typeof(TextMeshProUGUI)) as TextMeshProUGUI;
                tmp.text = "IK Solver";
                resizePanelShowIKOptsButton = ikOptsButtonGO.GetComponent<Button>();
                ClearButtonOnClick(resizePanelShowIKOptsButton);
                resizePanelShowIKOptsButton.image.color = Color.white;

                var breathOptsButtonGO = Instantiate(fkButton, ResizePanel.transform);
                breathOptsButtonGO.name = "Breath Opts";
                breathOptsButtonGO.transform.localPosition = new Vector3(145, -65, 0);

                tmp = breathOptsButtonGO.transform.GetChild(0).GetComponentInChildren(typeof(TextMeshProUGUI)) as TextMeshProUGUI;
                tmp.text = "Breath";
                resizePanelShowBreathButton = breathOptsButtonGO.GetComponent<Button>();
                ClearButtonOnClick(resizePanelShowBreathButton);
                resizePanelShowBreathButton.image.color = Color.white;

                resizePanelShowIKOptsButton.onClick.AddListener(() =>
                {
                    showAdvIKPanel = true;
                    showResizePanel = false;
                    BreathPanel.SetActive(false);
                    ResizePanel.SetActive(false);
                    AdvIKPanel.SetActive(true);
                });

                resizePanelShowBreathButton.onClick.AddListener(() =>
                {
                    showAdvIKPanel = false;
                    showResizePanel = false;
                    BreathPanel.SetActive(true);
                    AdvIKPanel.SetActive(false);
                    ResizePanel.SetActive(false);
                });

                Text resizeCentroidText = SetupText("ResizeCentroidDesc", -80, "Center - Adjust In/Out From Here", ResizePanel);
                resizeCentroidText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 190);
                resizeCentroidText.fontSize = 16;

                Text resizeCentroidAdtlText = SetupText("ResizeCentroidDesc", -110, "Body - For Lying Poses", ResizePanel);
                resizeCentroidAdtlText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 190);
                resizeCentroidAdtlText.fontSize = 16;

                // Row 1

                Text resizeCentroidNoneText = SetupText("ResizeCentroid", -135, "Off:", ResizePanel);
                resizeCentroidNoneText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
                resizeCentroidNoneText.fontSize = 16;
                resizeCentroid_None = SetupToggle("ResizeCentroid", -135, resizeCentroidNoneText.transform, 30, ResizePanel);
                resizeCentroid_None.onValueChanged.AddListener(delegate (bool value)
                {
                    if (updatingResizeCentroidControls)
                    {
                        return;
                    }
                    updatingResizeCentroidControls = true;
                    ClearResizeCentroidControls();
                    if (selectedChar != null && value)
                    {
                        resizeCentroid_None.isOn = true;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.Centroid = Algos.IKResizeCentroid.NONE;
                        }
                        
                    }
                    else if (selectedChar != null)
                    {
                        resizeCentroid_Auto.isOn = true;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.Centroid = Algos.IKResizeCentroid.AUTO;
                        }
                    }
                    updatingResizeCentroidControls = false;
                });

                Text resizeCentroidAutoText = SetupText("ResizeCentroid", -135, 57, "Auto:", ResizePanel);
                resizeCentroidAutoText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
                resizeCentroidAutoText.fontSize = 16;
                resizeCentroid_Auto = SetupToggle("ResizeCentroid", -135, resizeCentroidAutoText.transform, 40, ResizePanel);
                resizeCentroid_Auto.onValueChanged.AddListener(delegate (bool value)
                {
                    if (updatingResizeCentroidControls)
                    {
                        return;
                    }
                    updatingResizeCentroidControls = true;
                    ClearResizeCentroidControls();
                    if (selectedChar != null && value)
                    {
                        resizeCentroid_Auto.isOn = true;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.Centroid = Algos.IKResizeCentroid.AUTO;
                        }

                    }
                    else if (selectedChar != null)
                    {
                        resizeCentroid_None.isOn = true;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.Centroid = Algos.IKResizeCentroid.NONE;
                        }
                    }
                    updatingResizeCentroidControls = false;
                });

                Text resizeCentroidBodyText = SetupText("ResizeCentroid", -135, 125, "Body:", ResizePanel);
                resizeCentroidBodyText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
                resizeCentroidBodyText.fontSize = 16;
                resizeCentroid_Body = SetupToggle("ResizeCentroid", -135, resizeCentroidBodyText.transform, 40, ResizePanel);                
                resizeCentroid_Body.onValueChanged.AddListener(delegate (bool value)
                {
                    if (updatingResizeCentroidControls)
                    {
                        return;
                    }
                    updatingResizeCentroidControls = true;
                    ClearResizeCentroidControls();
                    if (selectedChar != null && value)
                    {
                        resizeCentroid_Body.isOn = true;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.Centroid = Algos.IKResizeCentroid.BODY;
                        }

                    }
                    else if (selectedChar != null)
                    {
                        resizeCentroid_Auto.isOn = true;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.Centroid = Algos.IKResizeCentroid.AUTO;
                        }
                    }
                    updatingResizeCentroidControls = false;
                });

                // Row 2

                Text resizeFeetCentroidText = SetupText("ResizeFeetDesc", -160, "Feet - Standing Poses", ResizePanel);
                resizeFeetCentroidText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 190);
                resizeFeetCentroidText.fontSize = 16;

                Text resizeCentroidFeetLeftText = SetupText("ResizeCentroid", -185, "Left:", ResizePanel);
                resizeCentroidFeetLeftText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
                resizeCentroidFeetLeftText.fontSize = 16;
                resizeCentroid_FeetLeft = SetupToggle("ResizeCentroid", -185, resizeCentroidFeetLeftText.transform, 35, ResizePanel);
                resizeCentroid_FeetLeft.onValueChanged.AddListener(delegate (bool value)
                {
                    if (updatingResizeCentroidControls)
                    {
                        return;
                    }
                    updatingResizeCentroidControls = true;
                    ClearResizeCentroidControls();
                    if (selectedChar != null && value)
                    {
                        resizeCentroid_FeetLeft.isOn = true;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.Centroid = Algos.IKResizeCentroid.FEET_LEFT;
                        }

                    }
                    else if (selectedChar != null)
                    {
                        resizeCentroid_Auto.isOn = true;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.Centroid = Algos.IKResizeCentroid.AUTO;
                        }
                    }
                    updatingResizeCentroidControls = false;
                });

                Text resizeCentroidFeetCntrText = SetupText("ResizeCentroid", -185, 60, "Cntr:", ResizePanel);
                resizeCentroidFeetCntrText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
                resizeCentroidFeetCntrText.fontSize = 16;
                resizeCentroid_FeetCenter = SetupToggle("ResizeCentroid", -185, resizeCentroidFeetCntrText.transform, 40, ResizePanel);
                resizeCentroid_FeetCenter.onValueChanged.AddListener(delegate (bool value)
                {
                    if (updatingResizeCentroidControls)
                    {
                        return;
                    }
                    updatingResizeCentroidControls = true;
                    ClearResizeCentroidControls();
                    if (selectedChar != null && value)
                    {
                        resizeCentroid_FeetCenter.isOn = true;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.Centroid = Algos.IKResizeCentroid.FEET_CENTER;
                        }

                    }
                    else if (selectedChar != null)
                    {
                        resizeCentroid_None.isOn = true;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.Centroid = Algos.IKResizeCentroid.NONE;
                        }
                    }
                    updatingResizeCentroidControls = false;
                });

                Text resizeCentroidFeetRghtText = SetupText("ResizeCentroid", -185, 125, "Rgt:", ResizePanel);
                resizeCentroidFeetRghtText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
                resizeCentroidFeetRghtText.fontSize = 16;
                resizeCentroid_FeetRight = SetupToggle("ResizeCentroid", -185, resizeCentroidFeetRghtText.transform, 40, ResizePanel);
                resizeCentroid_FeetRight.onValueChanged.AddListener(delegate (bool value)
                {
                    if (updatingResizeCentroidControls)
                    {
                        return;
                    }
                    updatingResizeCentroidControls = true;
                    ClearResizeCentroidControls();
                    if (selectedChar != null && value)
                    {
                        resizeCentroid_FeetRight.isOn = true;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.Centroid = Algos.IKResizeCentroid.FEET_RIGHT;
                        }

                    }
                    else if (selectedChar != null)
                    {
                        resizeCentroid_Auto.isOn = true;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.Centroid = Algos.IKResizeCentroid.AUTO;
                        }
                    }
                    updatingResizeCentroidControls = false;
                });

                // Row 3

                Text resizeThighCentroidText = SetupText("ResizeThighDesc", -210, "Thigh - Seated Poses", ResizePanel);
                resizeThighCentroidText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 190);
                resizeThighCentroidText.fontSize = 16;

                Text resizeCentroidThighLeftText = SetupText("ResizeCentroid", -235, "Left:", ResizePanel);
                resizeCentroidThighLeftText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
                resizeCentroidThighLeftText.fontSize = 16;
                resizeCentroid_ThighLeft = SetupToggle("ResizeCentroid", -235, resizeCentroidThighLeftText.transform, 35, ResizePanel);
                resizeCentroid_ThighLeft.onValueChanged.AddListener(delegate (bool value)
                {
                    if (updatingResizeCentroidControls)
                    {
                        return;
                    }
                    updatingResizeCentroidControls = true;
                    ClearResizeCentroidControls();
                    if (selectedChar != null && value)
                    {
                        resizeCentroid_ThighLeft.isOn = true;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.Centroid = Algos.IKResizeCentroid.THIGH_LEFT;
                        }

                    }
                    else if (selectedChar != null)
                    {
                        resizeCentroid_Auto.isOn = true;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.Centroid = Algos.IKResizeCentroid.AUTO;
                        }
                    }
                    updatingResizeCentroidControls = false;
                });

                Text resizeCentroidThighCntrText = SetupText("ResizeCentroid", -235, 60, "Cntr:", ResizePanel);
                resizeCentroidThighCntrText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
                resizeCentroidThighCntrText.fontSize = 16;
                resizeCentroid_ThighCenter = SetupToggle("ResizeCentroid", -235, resizeCentroidThighCntrText.transform, 40, ResizePanel);
                resizeCentroid_ThighCenter.onValueChanged.AddListener(delegate (bool value)
                {
                    if (updatingResizeCentroidControls)
                    {
                        return;
                    }
                    updatingResizeCentroidControls = true;
                    ClearResizeCentroidControls();
                    if (selectedChar != null && value)
                    {
                        resizeCentroid_ThighCenter.isOn = true;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.Centroid = Algos.IKResizeCentroid.THIGH_CENTER;
                        }

                    }
                    else if (selectedChar != null)
                    {
                        resizeCentroid_None.isOn = true;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.Centroid = Algos.IKResizeCentroid.NONE;
                        }
                    }
                    updatingResizeCentroidControls = false;
                });

                Text resizeCentroidThighRgtText = SetupText("ResizeCentroid", -235, 125, "Rgt:", ResizePanel);
                resizeCentroidThighRgtText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
                resizeCentroidThighRgtText.fontSize = 16;
                resizeCentroid_ThighRight = SetupToggle("ResizeCentroid", -235, resizeCentroidThighRgtText.transform, 40, ResizePanel);
                resizeCentroid_ThighRight.onValueChanged.AddListener(delegate (bool value)
                {
                    if (updatingResizeCentroidControls)
                    {
                        return;
                    }
                    updatingResizeCentroidControls = true;
                    ClearResizeCentroidControls();
                    if (selectedChar != null && value)
                    {
                        resizeCentroid_ThighRight.isOn = true;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.Centroid = Algos.IKResizeCentroid.BODY;
                        }

                    }
                    else if (selectedChar != null)
                    {
                        resizeCentroid_Auto.isOn = true;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.Centroid = Algos.IKResizeCentroid.AUTO;
                        }
                    }
                    updatingResizeCentroidControls = false;
                });

                // Row 4

                Text resizeHandCentroidText = SetupText("ResizeHandDesc", -260, "Hands - Dangling Poses", ResizePanel);
                resizeHandCentroidText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 190);
                resizeHandCentroidText.fontSize = 16;

                Text resizeCentroidHandLeftText = SetupText("ResizeCentroid", -285, "Left:", ResizePanel);
                resizeCentroidHandLeftText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
                resizeCentroidHandLeftText.fontSize = 16;
                resizeCentroid_HandLeft = SetupToggle("ResizeCentroid", -285, resizeCentroidHandLeftText.transform, 35, ResizePanel);
                resizeCentroid_HandLeft.onValueChanged.AddListener(delegate (bool value)
                {
                    if (updatingResizeCentroidControls)
                    {
                        return;
                    }
                    updatingResizeCentroidControls = true;
                    ClearResizeCentroidControls();
                    if (selectedChar != null && value)
                    {
                        resizeCentroid_HandLeft.isOn = true;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.Centroid = Algos.IKResizeCentroid.HAND_LEFT;
                        }

                    }
                    else if (selectedChar != null)
                    {
                        resizeCentroid_Auto.isOn = true;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.Centroid = Algos.IKResizeCentroid.AUTO;
                        }
                    }
                    updatingResizeCentroidControls = false;
                });

                Text resizeCentroidHandCntrText = SetupText("ResizeCentroid", -285, 60, "Cntr:", ResizePanel);
                resizeCentroidHandCntrText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
                resizeCentroidHandCntrText.fontSize = 16;
                resizeCentroid_HandCenter = SetupToggle("ResizeCentroid", -285, resizeCentroidHandCntrText.transform, 40, ResizePanel);
                resizeCentroid_HandCenter.onValueChanged.AddListener(delegate (bool value)
                {
                    if (updatingResizeCentroidControls)
                    {
                        return;
                    }
                    updatingResizeCentroidControls = true;
                    ClearResizeCentroidControls();
                    if (selectedChar != null && value)
                    {
                        resizeCentroid_HandCenter.isOn = true;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.Centroid = Algos.IKResizeCentroid.HAND_CENTER;
                        }

                    }
                    else if (selectedChar != null)
                    {
                        resizeCentroid_None.isOn = true;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.Centroid = Algos.IKResizeCentroid.NONE;
                        }
                    }
                    updatingResizeCentroidControls = false;
                });

                Text resizeCentroidHandRgtText = SetupText("ResizeCentroid", -285, 125, "Rgt:", ResizePanel);
                resizeCentroidHandRgtText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
                resizeCentroidHandRgtText.fontSize = 16;
                resizeCentroid_HandRight = SetupToggle("ResizeCentroid", -285, resizeCentroidHandRgtText.transform, 40, ResizePanel);
                resizeCentroid_HandRight.onValueChanged.AddListener(delegate (bool value)
                {
                    if (updatingResizeCentroidControls)
                    {
                        return;
                    }
                    updatingResizeCentroidControls = true;
                    ClearResizeCentroidControls();
                    if (selectedChar != null && value)
                    {
                        resizeCentroid_HandRight.isOn = true;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.Centroid = Algos.IKResizeCentroid.HAND_RIGHT;
                        }

                    }
                    else if (selectedChar != null)
                    {
                        resizeCentroid_Auto.isOn = true;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.Centroid = Algos.IKResizeCentroid.AUTO;
                        }
                    }
                    updatingResizeCentroidControls = false;
                });

                // Row 5

                Text resizeShldrCentroidText = SetupText("ResizeShldrDesc", -310, "Shoulder - Some Back Poses", ResizePanel);
                resizeShldrCentroidText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 190);
                resizeShldrCentroidText.fontSize = 16;

                Text resizeCentroidShldrLeftText = SetupText("ResizeCentroid", -335, "Left:", ResizePanel);
                resizeCentroidShldrLeftText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
                resizeCentroidShldrLeftText.fontSize = 16;
                resizeCentroid_ShoulderLeft = SetupToggle("ResizeCentroid", -335, resizeCentroidShldrLeftText.transform, 35, ResizePanel);
                resizeCentroid_ShoulderLeft.onValueChanged.AddListener(delegate (bool value)
                {
                    if (updatingResizeCentroidControls)
                    {
                        return;
                    }
                    updatingResizeCentroidControls = true;
                    ClearResizeCentroidControls();
                    if (selectedChar != null && value)
                    {
                        resizeCentroid_ShoulderLeft.isOn = true;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.Centroid = Algos.IKResizeCentroid.SHOULDER_LEFT;
                        }

                    }
                    else if (selectedChar != null)
                    {
                        resizeCentroid_Auto.isOn = true;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.Centroid = Algos.IKResizeCentroid.AUTO;
                        }
                    }
                    updatingResizeCentroidControls = false;
                });

                Text resizeCentroidShldrCntrText = SetupText("ResizeCentroid", -335, 60, "Cntr:", ResizePanel);
                resizeCentroidShldrCntrText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
                resizeCentroidShldrCntrText.fontSize = 16;
                resizeCentroid_ShoulderCenter = SetupToggle("ResizeCentroid", -335, resizeCentroidShldrCntrText.transform, 40, ResizePanel);
                resizeCentroid_ShoulderCenter.onValueChanged.AddListener(delegate (bool value)
                {
                    if (updatingResizeCentroidControls)
                    {
                        return;
                    }
                    updatingResizeCentroidControls = true;
                    ClearResizeCentroidControls();
                    if (selectedChar != null && value)
                    {
                        resizeCentroid_ShoulderCenter.isOn = true;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.Centroid = Algos.IKResizeCentroid.SHOULDER_CENTER;
                        }

                    }
                    else if (selectedChar != null)
                    {
                        resizeCentroid_None.isOn = true;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.Centroid = Algos.IKResizeCentroid.NONE;
                        }
                    }
                    updatingResizeCentroidControls = false;
                });

                Text resizeCentroidShldrRgtText = SetupText("ResizeCentroid", -335, 125, "Rgt:", ResizePanel);
                resizeCentroidShldrRgtText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
                resizeCentroidShldrRgtText.fontSize = 16;
                resizeCentroid_ShoulderRight = SetupToggle("ResizeCentroid", -335, resizeCentroidShldrRgtText.transform, 40, ResizePanel);
                resizeCentroid_ShoulderRight.onValueChanged.AddListener(delegate (bool value)
                {
                    if (updatingResizeCentroidControls)
                    {
                        return;
                    }
                    updatingResizeCentroidControls = true;
                    ClearResizeCentroidControls();
                    if (selectedChar != null && value)
                    {
                        resizeCentroid_ShoulderRight.isOn = true;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.Centroid = Algos.IKResizeCentroid.SHOULDER_RIGHT;
                        }

                    }
                    else if (selectedChar != null)
                    {
                        resizeCentroid_Auto.isOn = true;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.Centroid = Algos.IKResizeCentroid.AUTO;
                        }
                    }
                    updatingResizeCentroidControls = false;
                });

                // Row 6

                Text resizeKneeCentroidText = SetupText("ResizeShldrDesc", -360, "Knee - Kneeling Poses", ResizePanel);
                resizeKneeCentroidText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 190);
                resizeKneeCentroidText.fontSize = 16;

                Text resizeCentroidKneeLeftText = SetupText("ResizeCentroid", -385, "Left:", ResizePanel);
                resizeCentroidKneeLeftText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
                resizeCentroidKneeLeftText.fontSize = 16;
                resizeCentroid_KneeLeft = SetupToggle("ResizeCentroid", -385, resizeCentroidKneeLeftText.transform, 35, ResizePanel);
                resizeCentroid_KneeLeft.onValueChanged.AddListener(delegate (bool value)
                {
                    if (updatingResizeCentroidControls)
                    {
                        return;
                    }
                    updatingResizeCentroidControls = true;
                    ClearResizeCentroidControls();
                    if (selectedChar != null && value)
                    {
                        resizeCentroid_KneeLeft.isOn = true;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.Centroid = Algos.IKResizeCentroid.KNEE_LEFT;
                        }

                    }
                    else if (selectedChar != null)
                    {
                        resizeCentroid_Auto.isOn = true;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.Centroid = Algos.IKResizeCentroid.AUTO;
                        }
                    }
                    updatingResizeCentroidControls = false;
                });

                Text resizeCentroidKneeCntrText = SetupText("ResizeCentroid", -385, 60, "Cntr:", ResizePanel);
                resizeCentroidKneeCntrText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
                resizeCentroidKneeCntrText.fontSize = 16;
                resizeCentroid_KneeCenter = SetupToggle("ResizeCentroid", -385, resizeCentroidKneeCntrText.transform, 40, ResizePanel);
                resizeCentroid_KneeCenter.onValueChanged.AddListener(delegate (bool value)
                {
                    if (updatingResizeCentroidControls)
                    {
                        return;
                    }
                    updatingResizeCentroidControls = true;
                    ClearResizeCentroidControls();
                    if (selectedChar != null && value)
                    {
                        resizeCentroid_KneeCenter.isOn = true;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.Centroid = Algos.IKResizeCentroid.KNEE_CENTER;
                        }

                    }
                    else if (selectedChar != null)
                    {
                        resizeCentroid_None.isOn = true;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.Centroid = Algos.IKResizeCentroid.NONE;
                        }
                    }
                    updatingResizeCentroidControls = false;
                });

                Text resizeCentroidKneeRgtText = SetupText("ResizeCentroid", -385, 125, "Rgt:", ResizePanel);
                resizeCentroidKneeRgtText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
                resizeCentroidKneeRgtText.fontSize = 16;
                resizeCentroid_KneeRight = SetupToggle("ResizeCentroid", -385, resizeCentroidKneeRgtText.transform, 40, ResizePanel);
                resizeCentroid_KneeRight.onValueChanged.AddListener(delegate (bool value)
                {
                    if (updatingResizeCentroidControls)
                    {
                        return;
                    }
                    updatingResizeCentroidControls = true;
                    ClearResizeCentroidControls();
                    if (selectedChar != null && value)
                    {
                        resizeCentroid_KneeRight.isOn = true;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.Centroid = Algos.IKResizeCentroid.KNEE_RIGHT;
                        }

                    }
                    else if (selectedChar != null)
                    {
                        resizeCentroid_Auto.isOn = true;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.Centroid = Algos.IKResizeCentroid.AUTO;
                        }
                    }
                    updatingResizeCentroidControls = false;
                });

                // Row 7

                Text resizeElbowCentroidText = SetupText("ResizeShldrDesc", -410, "Elbow - Some Back Poses", ResizePanel);
                resizeElbowCentroidText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 190);
                resizeElbowCentroidText.fontSize = 16;

                Text resizeCentroidElbowLeftText = SetupText("ResizeCentroid", -435, "Left:", ResizePanel);
                resizeCentroidElbowLeftText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
                resizeCentroidElbowLeftText.fontSize = 16;
                resizeCentroid_ElbowLeft = SetupToggle("ResizeCentroid", -435, resizeCentroidElbowLeftText.transform, 35, ResizePanel);
                resizeCentroid_ElbowLeft.onValueChanged.AddListener(delegate (bool value)
                {
                    if (updatingResizeCentroidControls)
                    {
                        return;
                    }
                    updatingResizeCentroidControls = true;
                    ClearResizeCentroidControls();
                    if (selectedChar != null && value)
                    {
                        resizeCentroid_ElbowLeft.isOn = true;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.Centroid = Algos.IKResizeCentroid.ELBOW_LEFT;
                        }

                    }
                    else if (selectedChar != null)
                    {
                        resizeCentroid_Auto.isOn = true;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.Centroid = Algos.IKResizeCentroid.AUTO;
                        }
                    }
                    updatingResizeCentroidControls = false;
                });

                Text resizeCentroidElbowCntrText = SetupText("ResizeCentroid", -435, 60, "Cntr:", ResizePanel);
                resizeCentroidElbowCntrText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
                resizeCentroidElbowCntrText.fontSize = 16;
                resizeCentroid_ElbowCenter = SetupToggle("ResizeCentroid", -435, resizeCentroidElbowCntrText.transform, 40, ResizePanel);
                resizeCentroid_ElbowCenter.onValueChanged.AddListener(delegate (bool value)
                {
                    if (updatingResizeCentroidControls)
                    {
                        return;
                    }
                    updatingResizeCentroidControls = true;
                    ClearResizeCentroidControls();
                    if (selectedChar != null && value)
                    {
                        resizeCentroid_ElbowCenter.isOn = true;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.Centroid = Algos.IKResizeCentroid.ELBOW_CENTER;
                        }

                    }
                    else if (selectedChar != null)
                    {
                        resizeCentroid_None.isOn = true;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.Centroid = Algos.IKResizeCentroid.NONE;
                        }
                    }
                    updatingResizeCentroidControls = false;
                });

                Text resizeCentroidElbowRgtText = SetupText("ResizeCentroid", -435, 125, "Rgt:", ResizePanel);
                resizeCentroidElbowRgtText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
                resizeCentroidElbowRgtText.fontSize = 16;
                resizeCentroid_ElbowRight = SetupToggle("ResizeCentroid", -435, resizeCentroidElbowRgtText.transform, 40, ResizePanel);
                resizeCentroid_ElbowRight.onValueChanged.AddListener(delegate (bool value)
                {
                    if (updatingResizeCentroidControls)
                    {
                        return;
                    }
                    updatingResizeCentroidControls = true;
                    ClearResizeCentroidControls();
                    if (selectedChar != null && value)
                    {
                        resizeCentroid_ElbowRight.isOn = true;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.Centroid = Algos.IKResizeCentroid.ELBOW_RIGHT;
                        }

                    }
                    else if (selectedChar != null)
                    {
                        resizeCentroid_Auto.isOn = true;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.Centroid = Algos.IKResizeCentroid.AUTO;
                        }
                    }
                    updatingResizeCentroidControls = false;
                });

                // Full Chara Resize
                Text resizeCentroidResize = SetupText("ResizeCentroid", -460, "Rescale Chara:", ResizePanel);
                resizeCentroidResize.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 190);
                resizeCentroidResize.fontSize = 16;
                resizeCentroid_Resize = SetupToggle("ResizeCentroid", -460, resizeCentroidResize.transform, 105, ResizePanel);
                resizeCentroid_Resize.onValueChanged.AddListener(delegate (bool value)
                {
                    if (updatingResizeCentroidControls)
                    {
                        return;
                    }
                    updatingResizeCentroidControls = true;
                    ClearResizeCentroidControls();
                    if (selectedChar != null && value)
                    {
                        resizeCentroid_Resize.isOn = true;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.Centroid = Algos.IKResizeCentroid.RESIZE;
                        }

                    }
                    else if (selectedChar != null)
                    {
                        resizeCentroid_Auto.isOn = true;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.Centroid = Algos.IKResizeCentroid.AUTO;
                        }
                    }
                    updatingResizeCentroidControls = false;
                });


                // Left Arm Chain Mode

                Text resizeLeftArmChainText = SetupText("ResizeLAChainDesc", -485, "Turn On/Off Ind. Limbs", ResizePanel);
                resizeLeftArmChainText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 190);
                resizeLeftArmChainText.fontSize = 16;

                Text resizeLeftArmChainOffText = SetupText("ResizeCentroid", -510, "Left Arm:", ResizePanel);
                resizeLeftArmChainOffText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
                resizeLeftArmChainOffText.fontSize = 16;
                resizeChainMode_LeftArm = SetupToggle("ResizeCentroid", -510, resizeLeftArmChainOffText.transform, 65, ResizePanel);
                resizeChainMode_LeftArm.onValueChanged.AddListener(delegate (bool value)
                {
                    if (updatingResizeCentroidControls)
                    {
                        return;
                    }
                    updatingResizeCentroidControls = true;
                    
                    if (selectedChar != null && value)
                    {
                        resizeChainMode_LeftArm.isOn = true;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.ChainAdjustments[Algos.IKChain.LEFT_ARM] = Algos.IKResizeChainAdjustment.CHAIN;
                        }

                    }
                    else if (selectedChar != null)
                    {
                        resizeChainMode_LeftArm.isOn = false;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.ChainAdjustments[Algos.IKChain.LEFT_ARM] = Algos.IKResizeChainAdjustment.OFF;
                        }
                    }
                    updatingResizeCentroidControls = false;
                });

                Text resizeRightArmChainOffText = SetupText("ResizeCentroid", -510, 90, "Right Arm:", ResizePanel);
                resizeRightArmChainOffText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
                resizeRightArmChainOffText.fontSize = 16;
                resizeChainMode_RightArm = SetupToggle("ResizeCentroid", -510, resizeRightArmChainOffText.transform, 75, ResizePanel);
                resizeChainMode_RightArm.onValueChanged.AddListener(delegate (bool value)
                {
                    if (updatingResizeCentroidControls)
                    {
                        return;
                    }
                    updatingResizeCentroidControls = true;

                    if (selectedChar != null && value)
                    {
                        resizeChainMode_RightArm.isOn = true;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.ChainAdjustments[Algos.IKChain.RIGHT_ARM] = Algos.IKResizeChainAdjustment.CHAIN;
                        }

                    }
                    else if (selectedChar != null)
                    {
                        resizeChainMode_RightArm.isOn = false;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.ChainAdjustments[Algos.IKChain.RIGHT_ARM] = Algos.IKResizeChainAdjustment.OFF;
                        }
                    }
                    updatingResizeCentroidControls = false;
                });                

                Text resizeLeftLegChainOffText = SetupText("ResizeCentroid", -535, "Left Leg:", ResizePanel);
                resizeLeftLegChainOffText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
                resizeLeftLegChainOffText.fontSize = 16;
                resizeChainMode_LeftLeg = SetupToggle("ResizeCentroid", -535, resizeLeftLegChainOffText.transform, 65, ResizePanel);
                resizeChainMode_LeftLeg.onValueChanged.AddListener(delegate (bool value)
                {
                    if (updatingResizeCentroidControls)
                    {
                        return;
                    }
                    updatingResizeCentroidControls = true;

                    if (selectedChar != null && value)
                    {
                        resizeChainMode_LeftLeg.isOn = true;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.ChainAdjustments[Algos.IKChain.LEFT_LEG] = Algos.IKResizeChainAdjustment.CHAIN;
                        }

                    }
                    else if (selectedChar != null)
                    {
                        resizeChainMode_LeftLeg.isOn = false;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.ChainAdjustments[Algos.IKChain.LEFT_LEG] = Algos.IKResizeChainAdjustment.OFF;
                        }
                    }
                    updatingResizeCentroidControls = false;
                });

                Text resizeRightLegChainOffText = SetupText("ResizeCentroid", -535, 90, "Right Leg:", ResizePanel);
                resizeRightLegChainOffText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
                resizeRightLegChainOffText.fontSize = 16;
                resizeChainMode_RightLeg = SetupToggle("ResizeCentroid", -535, resizeRightLegChainOffText.transform, 75, ResizePanel);
                resizeChainMode_RightLeg.onValueChanged.AddListener(delegate (bool value)
                {
                    if (updatingResizeCentroidControls)
                    {
                        return;
                    }
                    updatingResizeCentroidControls = true;

                    if (selectedChar != null && value)
                    {
                        resizeChainMode_RightLeg.isOn = true;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.ChainAdjustments[Algos.IKChain.RIGHT_LEG] = Algos.IKResizeChainAdjustment.CHAIN;
                        }

                    }
                    else if (selectedChar != null)
                    {
                        resizeChainMode_RightLeg.isOn = false;
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IKResizeController.ChainAdjustments[Algos.IKChain.RIGHT_LEG] = Algos.IKResizeChainAdjustment.OFF;
                        }
                    }
                    updatingResizeCentroidControls = false;
                });

                // Undo/Redo Button

                var resizeButtonGO = Instantiate(fkButton, ResizePanel.transform);
                resizeButtonGO.name = "Undo Resize";
                resizeButtonGO.transform.localPosition = new Vector3(100, -575, 0);
                resizeButtonGO.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 20);
                resizeButtonGO.GetComponent<PreferredSizeFitter>().preferredWidth = 200;

                resizeButtonText = resizeButtonGO.transform.GetChild(0).GetComponentInChildren(typeof(TextMeshProUGUI)) as TextMeshProUGUI;
                resizeButtonText.text = "Undo Resize Adjustment";
                resizeButtonText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 200);
                resizeButton = resizeButtonGO.GetComponent<Button>();
                ClearButtonOnClick(resizeButton);
                resizeButton.image.color = Color.white;
                resizeButton.image.rectTransform.sizeDelta = new Vector2(180, 20);

                resizeButton.onClick.AddListener(() =>
                {
                    foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                    {
                        if (controller.IKResizeController.AdjustmentApplied)
                            controller.IKResizeController.UndoAdjustment();
                        else
                            controller.IKResizeController.ApplyAdjustment();

                        if (controller.IKResizeController.AdjustmentApplied)
                            resizeButtonText.text = "Undo Resize Adjustment";
                        else
                            resizeButtonText.text = "Apply Resize Adjustment";
                    }
                });

                // Clear controls
                foreach (Transform child in ResizePanel.transform)
                {
                    if (child.gameObject != ikOptsButtonGO && child.gameObject != breathOptsButtonGO && child.gameObject != resizeCentroidText.gameObject && child.gameObject != resizeCentroidAdtlText.gameObject
                        && child.gameObject != resizeButtonGO && child.gameObject != resizeButton.gameObject
                        && child.gameObject != resizePanelShowBreathButton.gameObject && child.gameObject != resizePanelShowIKOptsButton.gameObject
                        && child.gameObject != resizeCentroidBodyText.gameObject && child.gameObject != resizeCentroid_Body.gameObject
                        && child.gameObject != resizeCentroidAutoText.gameObject && child.gameObject != resizeCentroid_Auto.gameObject
                        && child.gameObject != resizeCentroidNoneText.gameObject && child.gameObject != resizeCentroid_None.gameObject

                        && child.gameObject != resizeCentroidFeetCntrText.gameObject && child.gameObject != resizeCentroidFeetLeftText.gameObject && child.gameObject != resizeCentroidFeetRghtText.gameObject && child.gameObject != resizeFeetCentroidText.gameObject
                        && child.gameObject != resizeCentroid_FeetCenter.gameObject && child.gameObject != resizeCentroid_FeetLeft.gameObject && child.gameObject != resizeCentroid_FeetRight.gameObject

                        && child.gameObject != resizeCentroidThighCntrText.gameObject && child.gameObject != resizeCentroidThighLeftText.gameObject && child.gameObject != resizeCentroidThighRgtText.gameObject && child.gameObject != resizeThighCentroidText.gameObject
                        && child.gameObject != resizeCentroid_ThighCenter.gameObject && child.gameObject != resizeCentroid_ThighLeft.gameObject && child.gameObject != resizeCentroid_ThighRight.gameObject

                        && child.gameObject != resizeCentroidHandCntrText.gameObject && child.gameObject != resizeCentroidHandLeftText.gameObject && child.gameObject != resizeCentroidHandRgtText.gameObject && child.gameObject != resizeHandCentroidText.gameObject
                        && child.gameObject != resizeCentroid_HandCenter.gameObject && child.gameObject != resizeCentroid_HandLeft.gameObject && child.gameObject != resizeCentroid_HandRight.gameObject

                        && child.gameObject != resizeCentroidShldrCntrText.gameObject && child.gameObject != resizeCentroidShldrLeftText.gameObject && child.gameObject != resizeCentroidShldrRgtText.gameObject && child.gameObject != resizeShldrCentroidText.gameObject
                        && child.gameObject != resizeCentroid_ShoulderCenter.gameObject && child.gameObject != resizeCentroid_ShoulderLeft.gameObject && child.gameObject != resizeCentroid_ShoulderRight.gameObject

                        && child.gameObject != resizeCentroidKneeCntrText.gameObject && child.gameObject != resizeCentroidKneeLeftText.gameObject && child.gameObject != resizeCentroidKneeRgtText.gameObject && child.gameObject != resizeKneeCentroidText.gameObject
                        && child.gameObject != resizeCentroid_KneeCenter.gameObject && child.gameObject != resizeCentroid_KneeLeft.gameObject && child.gameObject != resizeCentroid_KneeRight.gameObject

                        && child.gameObject != resizeCentroidElbowCntrText.gameObject && child.gameObject != resizeCentroidElbowLeftText.gameObject && child.gameObject != resizeCentroidElbowRgtText.gameObject && child.gameObject != resizeElbowCentroidText.gameObject
                        && child.gameObject != resizeCentroid_ElbowCenter.gameObject && child.gameObject != resizeCentroid_ElbowLeft.gameObject && child.gameObject != resizeCentroid_ElbowRight.gameObject

                        && child.gameObject != resizeCentroidResize.gameObject && child.gameObject != resizeCentroid_Resize.gameObject

                        && child.gameObject != resizeLeftArmChainOffText.gameObject && child.gameObject != resizeLeftArmChainText.gameObject
                        && child.gameObject != resizeChainMode_LeftArm.gameObject

                        && child.gameObject != resizeRightArmChainOffText.gameObject
                        && child.gameObject != resizeChainMode_RightArm.gameObject

                        && child.gameObject != resizeLeftLegChainOffText.gameObject
                        && child.gameObject != resizeChainMode_LeftLeg.gameObject

                        && child.gameObject != resizeRightLegChainOffText.gameObject
                        && child.gameObject != resizeChainMode_RightLeg.gameObject

                        )
                    {
                        Destroy(child.gameObject);
                    }
                }
            }

            private static bool updatingResizeCentroidControls = false;
            private static void ClearResizeCentroidControls()
            {
                resizeCentroid_None.isOn = false;
                resizeCentroid_Auto.isOn = false;
                resizeCentroid_Body.isOn = false;

                resizeCentroid_FeetCenter.isOn = false;
                resizeCentroid_FeetLeft.isOn = false;
                resizeCentroid_FeetRight.isOn = false;

                resizeCentroid_ThighCenter.isOn = false;
                resizeCentroid_ThighLeft.isOn = false;
                resizeCentroid_ThighRight.isOn = false;

                resizeCentroid_ShoulderCenter.isOn = false;
                resizeCentroid_ShoulderLeft.isOn = false;
                resizeCentroid_ShoulderRight.isOn = false;

                resizeCentroid_HandCenter.isOn = false;
                resizeCentroid_HandLeft.isOn = false;
                resizeCentroid_HandRight.isOn = false;

                resizeCentroid_KneeCenter.isOn = false;
                resizeCentroid_KneeLeft.isOn = false;
                resizeCentroid_KneeRight.isOn = false;

                resizeCentroid_ElbowCenter.isOn = false;
                resizeCentroid_ElbowLeft.isOn = false;
                resizeCentroid_ElbowRight.isOn = false;

                resizeCentroid_Resize.isOn = false;
            }

            public static void SetupBreathControls()
            {
                GameObject fkButton = GameObject.Find("StudioScene/Canvas Main Menu/02_Manipulate/00_Chara/02_Kinematic/Viewport/Content/FK");
                var ikOptsButtonGO = Instantiate(fkButton, BreathPanel.transform);
                ikOptsButtonGO.name = "IKS Opts";
                ikOptsButtonGO.transform.localPosition = new Vector3(45, -65, 0);


                TextMeshProUGUI tmp = ikOptsButtonGO.transform.GetChild(0).GetComponentInChildren(typeof(TextMeshProUGUI)) as TextMeshProUGUI;
                tmp.text = "IK Solver";
                breathPanelShowIKOptsButton = ikOptsButtonGO.GetComponent<Button>();
                ClearButtonOnClick(breathPanelShowIKOptsButton);
                breathPanelShowIKOptsButton.image.color = Color.white;

                var breathOptsButtonGO = Instantiate(fkButton, BreathPanel.transform);
                breathOptsButtonGO.name = "Breath Opts";
                breathOptsButtonGO.transform.localPosition = new Vector3(145, -65, 0);

                tmp = breathOptsButtonGO.transform.GetChild(0).GetComponentInChildren(typeof(TextMeshProUGUI)) as TextMeshProUGUI;
                tmp.text = "Resize";
                breathPanelShowResizeButton = breathOptsButtonGO.GetComponent<Button>();
                ClearButtonOnClick(breathPanelShowResizeButton);
                breathPanelShowResizeButton.image.color = Color.white;

                breathPanelShowIKOptsButton.onClick.AddListener(() =>
                {
                    showAdvIKPanel = true;
                    showResizePanel = false;
                    BreathPanel.SetActive(false);
                    AdvIKPanel.SetActive(true);
                    ResizePanel.SetActive(false);
                });

                breathPanelShowResizeButton.onClick.AddListener(() =>
                {
                    showAdvIKPanel = false;
                    showResizePanel = true;
                    BreathPanel.SetActive(false);
                    AdvIKPanel.SetActive(false);
                    ResizePanel.SetActive(true);
                });


                Text breathingToggleText = SetupText("BreathingEnabled", -80, "Enable Breathing", BreathPanel);
                breathingToggleText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
                breathingToggleText.fontSize = 16;
                BreathingToggle = SetupToggle("BreathingToggle", -80, BreathPanel);

                BreathingToggle.onValueChanged.AddListener(delegate (bool value)
                {
                    if (selectedChar != null)
                    {
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.BreathingController.Enabled = value;
                        }
                    }
                });

                var sldSize = GetPanelObject<Slider>("Slider Size", BreathPanel);

                magnitudeText = SetupText("BreathMagnitude", -105, "Breath Size %", BreathPanel);
                magnitudeText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 200);
                magnitudeText.fontSize = 20;
                magnitudeSlider = Instantiate(sldSize, BreathPanel.transform);
                magnitudeSlider.name = "BreathMagnitude";
                magnitudeSlider.transform.SetLocalScale(1.5f, 1.0f, 1.0f);
                magnitudeSlider.transform.SetLocalPosition(30, -135, 0);
                magnitudeSlider.minValue = 0f;
                magnitudeSlider.maxValue = 3f;

                magnitudeSlider.onValueChanged.RemoveAllListeners();
                magnitudeSlider.onValueChanged.AddListener(delegate (float value)
                {
                    if (selectedChar != null)
                    {
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.BreathingController.MagnitudeFactor = value;
                        }
                        magnitudeText.text = string.Format("Breath Size % ({0:0.000})", value);
                    }
                });

                intakeText = SetupText("IntakePause", -165, "Intake Pause %", BreathPanel);
                intakeText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 200);
                intakeText.fontSize = 20;
                intakeSlider = Instantiate(sldSize, BreathPanel.transform);
                intakeSlider.name = "IntakePause";
                intakeSlider.transform.SetLocalScale(1.5f, 1.0f, 1.0f);
                intakeSlider.transform.SetLocalPosition(30, -195, 0);
                intakeSlider.minValue = 0;
                intakeSlider.maxValue = 1;

                intakeSlider.onValueChanged.RemoveAllListeners();
                intakeSlider.onValueChanged.AddListener(delegate (float value)
                {
                    if (selectedChar != null)
                    {
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.BreathingController.IntakePause = value;
                        }
                        intakeText.text = string.Format("Intake Pause % ({0:0.000})", value);
                    }
                });

                holdText = SetupText("HoldPause", -225, "Hold Pause %", BreathPanel);
                holdText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 200);
                holdText.fontSize = 20;
                holdSlider = Instantiate(sldSize, BreathPanel.transform);
                holdSlider.name = "HoldPause";
                holdSlider.transform.SetLocalScale(1.5f, 1.0f, 1.0f);
                holdSlider.transform.SetLocalPosition(30, -255, 0);
                holdSlider.minValue = 0;
                holdSlider.maxValue = 1;

                holdSlider.onValueChanged.RemoveAllListeners();
                holdSlider.onValueChanged.AddListener(delegate (float value)
                {
                    if (selectedChar != null)
                    {
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.BreathingController.HoldPause = value;
                        }
                        holdText.text = string.Format("Hold Pause % ({0:0.000})", value);
                    }
                });

                inhaleText = SetupText("InhalePercent", -285, "Inhale %", BreathPanel);
                inhaleText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 200);
                inhaleText.fontSize = 20;
                inhaleSlider = Instantiate(sldSize, BreathPanel.transform);
                inhaleSlider.name = "InhalePercent";
                inhaleSlider.transform.SetLocalScale(1.5f, 1.0f, 1.0f);
                inhaleSlider.transform.SetLocalPosition(30, -315, 0);
                inhaleSlider.minValue = 0;
                inhaleSlider.maxValue = 1;

                inhaleSlider.onValueChanged.RemoveAllListeners();
                inhaleSlider.onValueChanged.AddListener(delegate (float value)
                {
                    if (selectedChar != null)
                    {
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.BreathingController.InhalePercentage = value;
                        }
                        inhaleText.text = string.Format("Inhale % ({0:0.000})", value);
                    }
                });

                bpmText = SetupText("BPM", -345, "Breath Per Min", BreathPanel);
                bpmText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 200);
                bpmText.fontSize = 20;
                bpmSlider = Instantiate(sldSize, BreathPanel.transform);
                bpmSlider.name = "BPM";
                bpmSlider.transform.SetLocalScale(1.5f, 1.0f, 1.0f);
                bpmSlider.transform.SetLocalPosition(30, -375, 0);
                bpmSlider.minValue = 1;
                bpmSlider.maxValue = 80;

                bpmSlider.onValueChanged.RemoveAllListeners();
                bpmSlider.onValueChanged.AddListener(delegate (float value)
                {
                    if (selectedChar != null)
                    {
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.BreathingController.BreathsPerMinute = (int)value;
                        }
                        bpmText.text = string.Format("Breath Per Min ({0:0})", value);
                    }
                });

                shoulderDampText = SetupText("ShoulderDamp", -395, "Shldr Damp %", BreathPanel);
                shoulderDampText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 200);
                shoulderDampText.fontSize = 20;
                shoulderDampSlider = Instantiate(sldSize, BreathPanel.transform);
                shoulderDampSlider.name = "ShoulderDamp";
                shoulderDampSlider.transform.SetLocalScale(1.5f, 1.0f, 1.0f);
                shoulderDampSlider.transform.SetLocalPosition(30, -425, 0);
                shoulderDampSlider.minValue = 0;
                shoulderDampSlider.maxValue = 1;

                shoulderDampSlider.onValueChanged.RemoveAllListeners();
                shoulderDampSlider.onValueChanged.AddListener(delegate (float value)
                {
                    if (selectedChar != null)
                    {
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.BreathingController.ShoulderDampeningFactor = value;
                        }
                        shoulderDampText.text = string.Format("Shldr Damp % ({0:0.000})", value);
                    }
                });

                var restoreBreathDefaults = Instantiate(breathOptsButtonGO, BreathPanel.transform);
                restoreBreathDefaults.name = "Restore Default";
                restoreBreathDefaults.transform.localPosition = new Vector3(45, -465, 0);

                tmp = restoreBreathDefaults.transform.GetChild(0).GetComponentInChildren(typeof(TextMeshProUGUI)) as TextMeshProUGUI;
                tmp.text = "Restore Default";
                Button restoreBreathDefaultsButton = restoreBreathDefaults.GetComponent<Button>();
                ClearButtonOnClick(restoreBreathDefaultsButton);
                restoreBreathDefaultsButton.image.color = Color.white;

                restoreBreathDefaultsButton.onClick.AddListener(() =>
                {
                    if (selectedChar != null)
                    {
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.BreathingController.RestoreDefaults();
                        }
                        UpdateUI(selectedChar);
                    }
                });

                var openShapePanel = Instantiate(breathOptsButtonGO, BreathPanel.transform);
                openShapePanel.name = "OpenShapePanel";
                openShapePanel.transform.localPosition = new Vector3(145, -465, 0);

                tmp = openShapePanel.transform.GetChild(0).GetComponentInChildren(typeof(TextMeshProUGUI)) as TextMeshProUGUI;
                tmp.text = "Adv Shape Opts >>";
                openShapePanelButton = openShapePanel.GetComponent<Button>();
                ClearButtonOnClick(openShapePanelButton);
                openShapePanelButton.image.color = Color.white;

                openShapePanelButton.onClick.AddListener(() =>
                {
                    if (BreathShapePanel.activeSelf)
                    {
                        BreathShapePanel.SetActive(false);
                        openShapePanelButton.image.color = Color.white;
                    }
                    else
                    {
                        BreathShapePanel.SetActive(true);
                        openShapePanelButton.image.color = Color.green;
                    } 
                });



                // Clear controls
                foreach (Transform child in BreathPanel.transform)
                {
                    if (child.gameObject != ikOptsButtonGO && child.gameObject != breathOptsButtonGO
                        && child.gameObject != breathingToggleText.gameObject && child.gameObject != BreathingToggle.gameObject
                        && child.gameObject != intakeText.gameObject && child.gameObject != intakeSlider.gameObject
                        && child.gameObject != holdText.gameObject && child.gameObject != holdSlider.gameObject
                        && child.gameObject != inhaleText.gameObject && child.gameObject != inhaleSlider.gameObject
                        && child.gameObject != bpmText.gameObject && child.gameObject != bpmSlider.gameObject
                        && child.gameObject != shoulderDampText.gameObject && child.gameObject != shoulderDampSlider.gameObject
                        && child.gameObject != magnitudeText.gameObject && child.gameObject != magnitudeSlider.gameObject
                        && child.gameObject != restoreBreathDefaults && child.gameObject != openShapePanel
                        )
                    {
                        Destroy(child.gameObject);
                    }
                }
            }

            public static void SetupAdvIKControls()
            {
                GameObject fkButton = GameObject.Find("StudioScene/Canvas Main Menu/02_Manipulate/00_Chara/02_Kinematic/Viewport/Content/FK");
                var ikOptsButtonGO = Instantiate(fkButton, AdvIKPanel.transform);
                ikOptsButtonGO.name = "IKS Opts";
                ikOptsButtonGO.transform.localPosition = new Vector3(45, -65, 0);


                TextMeshProUGUI tmp = ikOptsButtonGO.transform.GetChild(0).GetComponentInChildren(typeof(TextMeshProUGUI)) as TextMeshProUGUI;
                tmp.text = "Resize";
                advIKPanelShowResizeButton = ikOptsButtonGO.GetComponent<Button>();
                ClearButtonOnClick(advIKPanelShowResizeButton);
                advIKPanelShowResizeButton.image.color = Color.white;

                var breathOptsButtonGO = Instantiate(fkButton, AdvIKPanel.transform);
                breathOptsButtonGO.name = "Breath Opts";
                breathOptsButtonGO.transform.localPosition = new Vector3(145, -65, 0);

                tmp = breathOptsButtonGO.transform.GetChild(0).GetComponentInChildren(typeof(TextMeshProUGUI)) as TextMeshProUGUI;
                tmp.text = "Breath";
                advIKPanelShowBreathButton = breathOptsButtonGO.GetComponent<Button>();
                ClearButtonOnClick(advIKPanelShowBreathButton);
                advIKPanelShowBreathButton.image.color = Color.white;

                advIKPanelShowResizeButton.onClick.AddListener(() =>
                {
                    showAdvIKPanel = false;
                    showResizePanel = true;
                    BreathPanel.SetActive(false);
                    ResizePanel.SetActive(true);
                    AdvIKPanel.SetActive(false);
                });

                advIKPanelShowBreathButton.onClick.AddListener(() =>
                {
                    showAdvIKPanel = false;
                    showResizePanel = false;
                    BreathPanel.SetActive(true);
                    AdvIKPanel.SetActive(false);
                    ResizePanel.SetActive(false);
                });

                Text shoulderToggleText = SetupText("ShoulderRotationEnabled", -80, "Shoulder Rotation", AdvIKPanel);
                shoulderToggleText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
                shoulderToggleText.fontSize = 16;
                ShoulderRotatorToggle = SetupToggle("ShoulderRotationEnabledToggle", -80, AdvIKPanel);

                ShoulderRotatorToggle.onValueChanged.AddListener(delegate (bool value)
                {
                    if (selectedChar != null)
                    {
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.ShoulderRotationEnabled = value;
                        }
                    }
                });

                Text independentShoulderText = SetupText("IndependentShoulders", -105, "Independent Shoulders", AdvIKPanel);
                independentShoulderText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
                independentShoulderText.fontSize = 16;
                IndependentShoulderToggle = SetupToggle("IndependentShoulderToggle", -105, AdvIKPanel);

                IndependentShoulderToggle.onValueChanged.AddListener(delegate (bool value)
                {
                    if (selectedChar != null)
                    {
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.IndependentShoulders = value;
                        }
                    }
                });

                Text reverseShoulderText = SetupText("ReverseShoulders", -130, "Rev Shlder Reach L", AdvIKPanel);
                reverseShoulderText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
                reverseShoulderText.fontSize = 16;
                ReverseShoulderLToggle = SetupToggle("ReverseShoulderLToggle", -130, AdvIKPanel);
                ReverseShoulderLToggle.transform.Translate(-25, 0, 0, Space.Self);
                ReverseShoulderLToggle.onValueChanged.AddListener(delegate (bool value)
                {
                    if (selectedChar != null)
                    {
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.ReverseShoulderL = value;
                        }
                    }
                });
                Text reverseShoulderRText = SetupText("ReverseShouldersR", -130, "R", AdvIKPanel);
                reverseShoulderRText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
                reverseShoulderRText.transform.Translate(155, 0, 0, Space.Self);
                reverseShoulderRText.fontSize = 16;
                ReverseShoulderRToggle = SetupToggle("ReverseShoulderRToggle", -130, AdvIKPanel);
                ReverseShoulderRToggle.transform.Translate(13, 0, 0, Space.Self);
                ReverseShoulderRToggle.onValueChanged.AddListener(delegate (bool value)
                {
                    if (selectedChar != null)
                    {
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.ReverseShoulderR = value;
                        }
                    }
                });



                var sldSize = GetPanelObject<Slider>("Slider Size", AdvIKPanel);

                weightSliderText = SetupText("WeightSlider", -165, "Shoulder Weight", AdvIKPanel);
                weightSliderText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 200);
                weightSliderText.fontSize = 20;
                Weight = Instantiate(sldSize, AdvIKPanel.transform);
                Weight.name = "WeightOffset";
                Weight.transform.SetLocalScale(1.5f, 1.0f, 1.0f);
                Weight.transform.SetLocalPosition(30, -195, 0);
                Weight.minValue = 0;
                Weight.maxValue = 5;

                offsetSliderText = SetupText("OffsetSlider", -225, "Shoulder Offset", AdvIKPanel);
                offsetSliderText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 200);
                offsetSliderText.fontSize = 20;
                Offset = Instantiate(sldSize, AdvIKPanel.transform);
                Offset.name = "ShoulderOffset";
                Offset.transform.SetLocalScale(1.5f, 1.0f, 1.0f);
                Offset.transform.SetLocalPosition(30, -255, 0);
                Offset.minValue = 0;
                Offset.maxValue = 1;

                weightRightSliderText = SetupText("WeightRightSlider", -285, "R Shoulder Weight", AdvIKPanel);
                weightRightSliderText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 200);
                weightRightSliderText.fontSize = 20;
                WeightRight = Instantiate(sldSize, AdvIKPanel.transform);
                WeightRight.name = "WeightOffsetRight";
                WeightRight.transform.SetLocalScale(1.5f, 1.0f, 1.0f);
                WeightRight.transform.SetLocalPosition(30, -315, 0);
                WeightRight.minValue = 0;
                WeightRight.maxValue = 5;

                offsetRightSliderText = SetupText("OffsetRightSlider", -345, "R Shoulder Offset", AdvIKPanel);
                offsetRightSliderText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 200);
                offsetRightSliderText.fontSize = 20;
                OffsetRight = Instantiate(sldSize, AdvIKPanel.transform);
                OffsetRight.name = "ShoulderOffsetRight";
                OffsetRight.transform.SetLocalScale(1.5f, 1.0f, 1.0f);
                OffsetRight.transform.SetLocalPosition(30, -375, 0);
                OffsetRight.minValue = 0;
                OffsetRight.maxValue = 1;

                spineSliderText = SetupText("SpineSlider", -395, "Spine Stiffness", AdvIKPanel);
                spineSliderText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 200);
                spineSliderText.fontSize = 20;
                SpineStiffness = Instantiate(sldSize, AdvIKPanel.transform);
                SpineStiffness.name = "SpineStiffness";
                SpineStiffness.transform.SetLocalScale(1.5f, 1.0f, 1.0f);
                SpineStiffness.transform.SetLocalPosition(30, -425, 0);
                SpineStiffness.minValue = 0;
                SpineStiffness.maxValue = 1;

                Text spineFKHintsText = SetupText("Enable Spine FK Hints", -455, "Enable Spine FK Hints", AdvIKPanel);
                spineFKHintsText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
                spineFKHintsText.fontSize = 16;
                spineFKHintsToggle = SetupToggle("SpineFKHints", -455, AdvIKPanel); 

                spineFKHintsToggle.onValueChanged.AddListener(delegate (bool value)
                {
                    if (selectedChar != null)
                    {
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.EnableSpineFKHints = value;
                        }
                    }
                });

                Text shoulderFKHintsText = SetupText("Enable Shoulder FK Hints", -485, "Enable Shoulder FK Hints", AdvIKPanel);
                shoulderFKHintsText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
                shoulderFKHintsText.fontSize = 16;
                shoulderFKHintsToggle = SetupToggle("ShoulderFKHints", -485, AdvIKPanel); 

                shoulderFKHintsToggle.onValueChanged.AddListener(delegate (bool value)
                {
                    if (selectedChar != null)
                    {
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.EnableShoulderFKHints = value;
                        }
                    }
                });

                Text toeFKHintsText = SetupText("Enable Toe FK Hints", -515, "Enable Toe FK Hints", AdvIKPanel);
                toeFKHintsText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
                toeFKHintsText.fontSize = 16;
                toeFKHintsToggle = SetupToggle("ToeFKHints", -515, AdvIKPanel);

                toeFKHintsToggle.onValueChanged.AddListener(delegate (bool value)
                {
                    if (selectedChar != null)
                    {
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.EnableToeFKHints = value;
                        }
                    }
                });

                Text heelzAllText = SetupText("Heelz All ", -545, "Heelz All ", AdvIKPanel);
                heelzAllText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
                heelzAllText.fontSize = 16;
                heelzAllToggle = SetupToggle("HeelzAll", -545, AdvIKPanel);
                heelzAllToggle.transform.Translate(-95, 0, 0, Space.Self);

                heelzAllToggle.onValueChanged.AddListener(delegate (bool value)
                {
                    if (selectedChar != null)
                    {
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.EnableHeelzHoverAll = value;
                        }
                    }
                });

                Text heelzLText = SetupText("L ", -545, "L ", AdvIKPanel);
                heelzLText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
                heelzLText.fontSize = 16;
                heelzLText.transform.Translate(100, 0, 0, Space.Self);
                heelzLToggle = SetupToggle("HeelzL", -545, AdvIKPanel);
                heelzLToggle.transform.Translate(-45, 0, 0, Space.Self);

                heelzLToggle.onValueChanged.AddListener(delegate (bool value)
                {
                    if (selectedChar != null)
                    {
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.EnableHeelzHoverLeftFoot = value;
                        }
                    }
                });

                Text heelzRText = SetupText("R ", -545, "R ", AdvIKPanel);
                heelzRText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
                heelzRText.fontSize = 16;
                heelzRText.transform.Translate(145, 0, 0, Space.Self);
                heelzRToggle = SetupToggle("HeelzR", -545, AdvIKPanel);
                heelzRToggle.transform.Translate(0, 0, 0, Space.Self);
                heelzRToggle.onValueChanged.AddListener(delegate (bool value)
                {
                    if (selectedChar != null)
                    {
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.EnableHeelzHoverRightFoot = value;
                        }
                    }
                });

                Weight.onValueChanged.RemoveAllListeners();
                WeightRight.onValueChanged.RemoveAllListeners();
                Offset.onValueChanged.RemoveAllListeners();
                OffsetRight.onValueChanged.RemoveAllListeners();
                SpineStiffness.onValueChanged.RemoveAllListeners();

           
                Weight.onValueChanged.AddListener(delegate (float value)
                {
                    if (selectedChar != null)
                    {
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.ShoulderWeight = value;
                        }
                        weightSliderText.text = string.Format("Shoulder Weight ({0:0.000})", Weight.value);
                    }
                });

                Offset.onValueChanged.AddListener(delegate (float value)
                {
                    if (selectedChar != null)
                    {
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.ShoulderOffset = value;
                        }
                        offsetSliderText.text = string.Format("Shoulder Offset ({0:0.000})", Offset.value);
                    }
                });
                WeightRight.onValueChanged.AddListener(delegate (float value)
                {
                    if (selectedChar != null)
                    {
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.ShoulderRightWeight = value;
                        }
                        weightRightSliderText.text = string.Format("R Shoulder Weight ({0:0.000})", WeightRight.value);
                    }
                });

                OffsetRight.onValueChanged.AddListener(delegate (float value)
                {
                    if (selectedChar != null)
                    {
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.ShoulderRightOffset = value;
                        }
                        offsetRightSliderText.text = string.Format("R Shoulder Offset ({0:0.000})", OffsetRight.value);
                    }
                });

                SpineStiffness.onValueChanged.AddListener(delegate (float value)
                {
                    if (selectedChar != null)
                    {
                        foreach (AdvIKCharaController controller in StudioAPI.GetSelectedControllers<AdvIKCharaController>())
                        {
                            controller.SpineStiffness = value;
                        }
                        spineSliderText.text = string.Format("Spine Stiffness ({0:0.000})", SpineStiffness.value);
                    }
                });

                // Clear controls
                foreach (Transform child in AdvIKPanel.transform)
                {
                    if (child.gameObject != shoulderToggleText.gameObject && child.gameObject != ShoulderRotatorToggle.gameObject
                        && child.gameObject != Weight.gameObject && child.gameObject != Offset.gameObject && child.gameObject != SpineStiffness.gameObject
                        && child.gameObject != weightSliderText.gameObject && child.gameObject != offsetSliderText.gameObject && child.gameObject != spineSliderText.gameObject
                        && child.gameObject != independentShoulderText.gameObject && child.gameObject != IndependentShoulderToggle.gameObject
                        && child.gameObject != weightRightSliderText.gameObject && child.gameObject != WeightRight.gameObject
                        && child.gameObject != offsetRightSliderText.gameObject && child.gameObject != OffsetRight.gameObject
                        && child.gameObject != spineFKHintsText.gameObject && child.gameObject != spineFKHintsToggle.gameObject
                        && child.gameObject != shoulderFKHintsText.gameObject && child.gameObject != shoulderFKHintsToggle.gameObject
                        && child.gameObject != toeFKHintsText.gameObject && child.gameObject != toeFKHintsToggle.gameObject
                        && child.gameObject != heelzAllText.gameObject && child.gameObject != heelzAllToggle.gameObject
                        && child.gameObject != heelzLText.gameObject && child.gameObject != heelzLToggle.gameObject
                        && child.gameObject != heelzRText.gameObject && child.gameObject != heelzRToggle.gameObject
                        && child.gameObject != reverseShoulderText.gameObject && child.gameObject != ReverseShoulderLToggle.gameObject && child.gameObject != ReverseShoulderRToggle.gameObject && child.gameObject != reverseShoulderRText.gameObject
                        && child.gameObject != ikOptsButtonGO && child.gameObject != breathOptsButtonGO
                        )
                    {
                        Destroy(child.gameObject);
                    }
                }
            }

            private static T GetPanelObject<T>(string name, GameObject parent) => parent.GetComponentsInChildren<RectTransform>(true).First(x => x.name == name).GetComponent<T>();

            private static Text SetupText(string name, int pos, string text, GameObject parent)
            {
                Text txt = Instantiate(GetPanelObject<Text>("Text Neck", parent), parent.transform);
                txt.name = name;
                txt.text = text;
                txt.transform.localPosition = new Vector3(txt.transform.localPosition.x + 10, pos, txt.transform.localPosition.z);
                return txt;
            }

            private static Text SetupText(string name, int pos, int xoffset, string text, GameObject parent)
            {
                Text txt = Instantiate(GetPanelObject<Text>("Text Neck", parent), parent.transform);
                txt.name = name;
                txt.text = text;
                txt.transform.localPosition = new Vector3(txt.transform.localPosition.x + xoffset + 10, pos, txt.transform.localPosition.z);
                return txt;
            }

            private static Toggle SetupToggle(string name, int pos, GameObject parent)
            {
                Toggle tglOriginal = GetPanelObject<Toggle>("Toggle Neck", parent);
                Toggle tgl = Instantiate(tglOriginal, parent.transform);
                tgl.name = name;
                tgl.transform.localPosition = new Vector3(tgl.transform.localPosition.x + 80, pos, tgl.transform.localPosition.z);
                tgl.onValueChanged.RemoveAllListeners();
                return tgl;

            }
            private static Toggle SetupToggle(string name, int pos, Transform xFrom, int xoffset, GameObject parent)
            {
                Toggle tglOriginal = GetPanelObject<Toggle>("Toggle Neck", parent);
                Toggle tgl = Instantiate(tglOriginal, parent.transform);
                tgl.name = name;
                tgl.transform.localPosition = new Vector3(xFrom.localPosition.x + xoffset, pos, tgl.transform.localPosition.z);
                tgl.onValueChanged.RemoveAllListeners();
                return tgl;

            }


            public static void CreatePanel(GameObject obj)
            {
                Texture2D tex = new Texture2D(172, 322);
                tex.LoadImage(LoadPanel());
                Image backingPanel = obj.GetComponent<Image>();
                Sprite replacement = Sprite.Create(tex, new Rect(0, 0, 172, 322), backingPanel.sprite.pivot);
                backingPanel.sprite = replacement;
                backingPanel.name = "AdvIK";
            }

            private static byte[] LoadPanel()
            {
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"AdvIKPlugin.Resources.Panel.png"))
                {
                    byte[] bytesInStream = new byte[stream.Length];
                    stream.Read(bytesInStream, 0, bytesInStream.Length);
                    return bytesInStream;
                }
            }

            private static void CreateMenuEntry()
            {
                GameObject listmenu = GameObject.Find("StudioScene/Canvas Main Menu/02_Manipulate/00_Chara/02_Kinematic/Viewport/Content");
                GameObject fkButton = GameObject.Find("StudioScene/Canvas Main Menu/02_Manipulate/00_Chara/02_Kinematic/Viewport/Content/FK");
                var newSelect = Instantiate(fkButton, listmenu.transform, true);
                newSelect.name = "Adv IK";

                TextMeshProUGUI tmp = newSelect.transform.GetChild(0).GetComponentInChildren(typeof(TextMeshProUGUI)) as TextMeshProUGUI;
                tmp.text = "Adv IK";

               

                GameObject originalPanel = GameObject.Find("StudioScene/Canvas Main Menu/02_Manipulate/00_Chara/02_Kinematic/00_FK");
                GameObject kineMenu = GameObject.Find("StudioScene/Canvas Main Menu/02_Manipulate/00_Chara/02_Kinematic");

                AdvIKPanel = Instantiate(originalPanel, kineMenu.transform, true);
                BreathPanel = Instantiate(originalPanel, kineMenu.transform, true);
                BreathShapePanel = Instantiate(originalPanel, kineMenu.transform, true);
                ResizePanel = Instantiate(originalPanel, kineMenu.transform, true);

                RectTransform rect = AdvIKPanel.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(202, 600);

                rect = BreathPanel.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(202, 490);

                rect = BreathShapePanel.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector3(222, 340);

                rect = ResizePanel.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(202, 600);
             
                Button fkikSelectButton = newSelect.GetComponent<Button>();
                ClearButtonOnClick(fkikSelectButton);

                foreach (Button button in listmenu.GetComponentsInChildren<Button>())
                {
                    if (!button.Equals(fkikSelectButton))
                    {
                        button.onClick.AddListener(delegate ()
                        {
                            if (AdvIKPanel.activeSelf || BreathPanel.activeSelf || BreathShapePanel.activeSelf || ResizePanel.activeSelf)
                            {
                                AdvIKPanel.SetActive(false);
                                BreathPanel.SetActive(false);
                                BreathShapePanel.SetActive(false);
                                ResizePanel.SetActive(false);
                                fkikSelectButton.image.color = Color.white;
                                openShapePanelButton.image.color = Color.white;
                                if (lastPanel != null && button.gameObject.Equals(lastButton))
                                {
                                    lastPanel.gameObject.SetActive(true);
                                    button.image.color = Color.green;
                                    return;
                                }
                            }
                            foreach (Transform child in kineMenu.transform)
                            {
                                if (child.name != "Viewport" && child.name != "Scrollbar Vertical" && child.gameObject.activeSelf)
                                {
                                    lastPanel = child.gameObject;
                                    lastButton = button.gameObject;
                                    break;
                                }
                            }
                        });
                    }
                } 
                
                fkikSelectButton.onClick.AddListener(delegate ()
                {
                    foreach (Transform child in kineMenu.transform)
                    {
                        if (child.name != "Viewport" && child.name != "Scrollbar Vertical")
                        {
                            child.gameObject.SetActive(false);
                        }
                    }
                    foreach (Button button in listmenu.GetComponentsInChildren<Button>())
                    {
                        button.image.color = Color.white;
                    }
                    if (showAdvIKPanel)
                    {
                        AdvIKPanel.SetActive(true);
                    }
                    else if (showResizePanel)
                    {
                        ResizePanel.SetActive(true);
                    }
                    else
                    {
                        BreathPanel.SetActive(true);
                    }
                    fkikSelectButton.image.color = Color.green;
                }); 
            }
        }

        // Yeah, I give up, I have no idea what's going on in the Illusion permanent button handlers. For some reason the button -> advik -> first button refuses to enable
        // the panel. I think there is some kind of active panel state I can't see to clear somewhere that thinks the panel is still active...have to fix the fun way.
        private static GameObject lastPanel;
        private static GameObject lastButton;

        private static IEnumerator WaitAndClick(Button button)
        {
            yield return null;
            button.onClick.Invoke();
        }

        private static void ClearButtonOnClick(Button clicky)
        {
            clicky.onClick = new Button.ButtonClickedEvent();
        }
    }
}
