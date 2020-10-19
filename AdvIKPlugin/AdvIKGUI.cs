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

namespace AdvIKPlugin
{
    public partial class AdvIKPlugin
    {
        internal class AdvIKGUI
        {
            private static GameObject AdvIKPanel;
            private static GameObject BreathPanel;
            private static GameObject BreathShapePanel;            
            private static Toggle ShoulderRotatorToggle;
            private static Toggle IndependentShoulderToggle;
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


            private static bool showAdvIKPanel = true;

            private static Button breathPanelShowBreathButton;
            private static Button breathPanelShowIKOptsButton;
            private static Button openShapePanelButton;

            private static Button advIKPanelShowBreathButton;
            private static Button advIKPanelShowIKOptsButton;


            private static OCIChar selectedChar;

            internal static void InitUI()
            {
                if (AdvIKPanel == null)
                {
                    CreateMenuEntry();
                    CreatePanel(AdvIKPanel);
                    CreatePanel(BreathPanel);
                    CreatePanel(BreathShapePanel);
                    SetupAdvIKControls();
                    SetupBreathControls();
                    SetupBreathShapeControls();
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
                    Weight.value = advIKController.ShoulderWeight;
                    WeightRight.value = advIKController.ShoulderRightWeight;
                    Offset.value = advIKController.ShoulderOffset;
                    OffsetRight.value = advIKController.ShoulderRightOffset;
                    SpineStiffness.value = advIKController.SpineStiffness;
                    spineFKHintsToggle.isOn = advIKController.EnableSpineFKHints;

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
                        selectedChar.charInfo.gameObject.GetComponent<AdvIKCharaController>().BreathingController.BreathMagnitude = new Vector3(value, currentBreathingMagnitude.y, currentBreathingMagnitude.z);
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
                        selectedChar.charInfo.gameObject.GetComponent<AdvIKCharaController>().BreathingController.BreathMagnitude = new Vector3(currentBreathingMagnitude.x, value, currentBreathingMagnitude.z);
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
                        selectedChar.charInfo.gameObject.GetComponent<AdvIKCharaController>().BreathingController.BreathMagnitude = new Vector3(currentBreathingMagnitude.x, currentBreathingMagnitude.y, value);
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
                        selectedChar.charInfo.gameObject.GetComponent<AdvIKCharaController>().BreathingController.UpperChestRelativeScaling = new Vector3(value, upperChestRelative.y, upperChestRelative.z);
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
                        selectedChar.charInfo.gameObject.GetComponent<AdvIKCharaController>().BreathingController.UpperChestRelativeScaling = new Vector3(upperChestRelative.x, value, upperChestRelative.z);
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
                        selectedChar.charInfo.gameObject.GetComponent<AdvIKCharaController>().BreathingController.UpperChestRelativeScaling = new Vector3(upperChestRelative.x, upperChestRelative.y, value);
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
                        selectedChar.charInfo.gameObject.GetComponent<AdvIKCharaController>().BreathingController.LowerChestRelativeScaling = new Vector3(value, lowerChestRelativeScaling.y, lowerChestRelativeScaling.z);
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
                        selectedChar.charInfo.gameObject.GetComponent<AdvIKCharaController>().BreathingController.LowerChestRelativeScaling = new Vector3(lowerChestRelativeScaling.x, value, lowerChestRelativeScaling.z);
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
                        selectedChar.charInfo.gameObject.GetComponent<AdvIKCharaController>().BreathingController.LowerChestRelativeScaling = new Vector3(lowerChestRelativeScaling.x, lowerChestRelativeScaling.y, value);
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
                        selectedChar.charInfo.gameObject.GetComponent<AdvIKCharaController>().BreathingController.AbdomenRelativeScaling = new Vector3(value, abdomenRelativeScaling.y, abdomenRelativeScaling.z);
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
                        selectedChar.charInfo.gameObject.GetComponent<AdvIKCharaController>().BreathingController.AbdomenRelativeScaling = new Vector3(abdomenRelativeScaling.x, value, abdomenRelativeScaling.z);
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
                        selectedChar.charInfo.gameObject.GetComponent<AdvIKCharaController>().BreathingController.AbdomenRelativeScaling = new Vector3(abdomenRelativeScaling.x, abdomenRelativeScaling.y, value);
                        abdomenShapeZ.text = string.Format("Z ({0:0.000})", value);
                    }
                });

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
                        )
                    {
                        Destroy(child.gameObject);
                    }
                }
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
                breathPanelShowIKOptsButton.onClick.RemoveAllListeners();
                breathPanelShowIKOptsButton.image.color = Color.white;

                var breathOptsButtonGO = Instantiate(fkButton, BreathPanel.transform);
                breathOptsButtonGO.name = "Breath Opts";
                breathOptsButtonGO.transform.localPosition = new Vector3(145, -65, 0);

                tmp = breathOptsButtonGO.transform.GetChild(0).GetComponentInChildren(typeof(TextMeshProUGUI)) as TextMeshProUGUI;
                tmp.text = "Breath";
                breathPanelShowBreathButton = breathOptsButtonGO.GetComponent<Button>();
                breathPanelShowBreathButton.onClick.RemoveAllListeners();
                breathPanelShowBreathButton.image.color = Color.green;

                breathPanelShowIKOptsButton.onClick.AddListener(() =>
                {
                    showAdvIKPanel = true;
                    BreathPanel.SetActive(false);
                    AdvIKPanel.SetActive(true);
                    advIKPanelShowIKOptsButton.image.color = Color.green;
                    breathPanelShowIKOptsButton.image.color = Color.green;
                    advIKPanelShowBreathButton.image.color = Color.white;
                    breathPanelShowBreathButton.image.color = Color.white;
                });

                breathPanelShowBreathButton.onClick.AddListener(() =>
                {
                    showAdvIKPanel = false;
                    BreathPanel.SetActive(true);
                    AdvIKPanel.SetActive(false);
                    advIKPanelShowIKOptsButton.image.color = Color.white;
                    breathPanelShowIKOptsButton.image.color = Color.white;
                    advIKPanelShowBreathButton.image.color = Color.green;
                    breathPanelShowBreathButton.image.color = Color.green;
                });


                Text breathingToggleText = SetupText("BreathingEnabled", -80, "Enable Breathing", BreathPanel);
                breathingToggleText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
                breathingToggleText.fontSize = 16;
                BreathingToggle = SetupToggle("BreathingToggle", -80, BreathPanel);

                BreathingToggle.onValueChanged.AddListener(delegate (bool value)
                {
                    if (selectedChar != null)
                    {
                        selectedChar.charInfo.gameObject.GetComponent<AdvIKCharaController>().BreathingController.Enabled = value;
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
                        selectedChar.charInfo.gameObject.GetComponent<AdvIKCharaController>().BreathingController.MagnitudeFactor = value;
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
                        selectedChar.charInfo.gameObject.GetComponent<AdvIKCharaController>().BreathingController.IntakePause = value;
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
                        selectedChar.charInfo.gameObject.GetComponent<AdvIKCharaController>().BreathingController.HoldPause = value;
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
                        selectedChar.charInfo.gameObject.GetComponent<AdvIKCharaController>().BreathingController.InhalePercentage = value;
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
                        selectedChar.charInfo.gameObject.GetComponent<AdvIKCharaController>().BreathingController.BreathsPerMinute = (int)value;
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
                        selectedChar.charInfo.gameObject.GetComponent<AdvIKCharaController>().BreathingController.ShoulderDampeningFactor = value;
                        shoulderDampText.text = string.Format("Shldr Damp % ({0:0.000})", value);
                    }
                });

                var restoreBreathDefaults = Instantiate(breathOptsButtonGO, BreathPanel.transform);
                restoreBreathDefaults.name = "Restore Default";
                restoreBreathDefaults.transform.localPosition = new Vector3(45, -465, 0);

                tmp = restoreBreathDefaults.transform.GetChild(0).GetComponentInChildren(typeof(TextMeshProUGUI)) as TextMeshProUGUI;
                tmp.text = "Restore Default";
                Button restoreBreathDefaultsButton = restoreBreathDefaults.GetComponent<Button>();
                restoreBreathDefaultsButton.onClick.RemoveAllListeners();
                restoreBreathDefaultsButton.image.color = Color.white;

                restoreBreathDefaultsButton.onClick.AddListener(() =>
                {
                    if (selectedChar != null)
                    {
                        selectedChar.charInfo.gameObject.GetComponent<AdvIKCharaController>().BreathingController.RestoreDefaults();
                        UpdateUI(selectedChar);
                    }
                });

                var openShapePanel = Instantiate(breathOptsButtonGO, BreathPanel.transform);
                openShapePanel.name = "OpenShapePanel";
                openShapePanel.transform.localPosition = new Vector3(145, -465, 0);

                tmp = openShapePanel.transform.GetChild(0).GetComponentInChildren(typeof(TextMeshProUGUI)) as TextMeshProUGUI;
                tmp.text = "Adv Shape Opts >>";
                openShapePanelButton = openShapePanel.GetComponent<Button>();
                openShapePanelButton.onClick.RemoveAllListeners();
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
                tmp.text = "IK Solver";
                advIKPanelShowIKOptsButton = ikOptsButtonGO.GetComponent<Button>();
                advIKPanelShowIKOptsButton.onClick.RemoveAllListeners();
                advIKPanelShowIKOptsButton.image.color = Color.green;

                var breathOptsButtonGO = Instantiate(fkButton, AdvIKPanel.transform);
                breathOptsButtonGO.name = "Breath Opts";
                breathOptsButtonGO.transform.localPosition = new Vector3(145, -65, 0);

                tmp = breathOptsButtonGO.transform.GetChild(0).GetComponentInChildren(typeof(TextMeshProUGUI)) as TextMeshProUGUI;
                tmp.text = "Breath";
                advIKPanelShowBreathButton = breathOptsButtonGO.GetComponent<Button>();
                advIKPanelShowBreathButton.onClick.RemoveAllListeners();
                advIKPanelShowBreathButton.image.color = Color.white;

                advIKPanelShowIKOptsButton.onClick.AddListener(() =>
                {
                    showAdvIKPanel = true;
                    BreathPanel.SetActive(false);
                    AdvIKPanel.SetActive(true);
                    advIKPanelShowIKOptsButton.image.color = Color.green;
                    breathPanelShowIKOptsButton.image.color = Color.green;
                    advIKPanelShowBreathButton.image.color = Color.white;
                    breathPanelShowBreathButton.image.color = Color.white;
                });

                advIKPanelShowBreathButton.onClick.AddListener(() =>
                {
                    showAdvIKPanel = false;
                    BreathPanel.SetActive(true);
                    AdvIKPanel.SetActive(false);
                    advIKPanelShowIKOptsButton.image.color = Color.white;
                    breathPanelShowIKOptsButton.image.color = Color.white;
                    advIKPanelShowBreathButton.image.color = Color.green;
                    breathPanelShowBreathButton.image.color = Color.green;
                });

#if KOIKATSU
                breathOptsButtonGO.SetActive(false);
#endif


                Text shoulderToggleText = SetupText("ShoulderRotationEnabled", -100, "Shoulder Rotation", AdvIKPanel);
                shoulderToggleText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
                shoulderToggleText.fontSize = 16;
                ShoulderRotatorToggle = SetupToggle("ShoulderRotationEnabledToggle", -100, AdvIKPanel);

                ShoulderRotatorToggle.onValueChanged.AddListener(delegate (bool value)
                {
                    if (selectedChar != null)
                    {
                        selectedChar.charInfo.gameObject.GetComponent<AdvIKCharaController>().ShoulderRotationEnabled = value;
                    }
                });

                Text independentShoulderText = SetupText("IndependentShoulders", -130, "Independent Shoulders", AdvIKPanel);
                independentShoulderText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
                independentShoulderText.fontSize = 16;
                IndependentShoulderToggle = SetupToggle("IndependentShoulderToggle", -130, AdvIKPanel);

                IndependentShoulderToggle.onValueChanged.AddListener(delegate (bool value)
                {
                    if (selectedChar != null)
                    {
                        selectedChar.charInfo.gameObject.GetComponent<AdvIKCharaController>().IndependentShoulders = value;
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
                        selectedChar.charInfo.gameObject.GetComponent<AdvIKCharaController>().EnableSpineFKHints = value;
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
                        selectedChar.charInfo.gameObject.GetComponent<AdvIKCharaController>().ShoulderWeight = value;
                        weightSliderText.text = string.Format("Shoulder Weight ({0:0.000})", Weight.value);
                    }
                });

                Offset.onValueChanged.AddListener(delegate (float value)
                {
                    if (selectedChar != null)
                    {
                        selectedChar.charInfo.gameObject.GetComponent<AdvIKCharaController>().ShoulderOffset = value;
                        offsetSliderText.text = string.Format("Shoulder Offset ({0:0.000})", Offset.value);
                    }
                });
                WeightRight.onValueChanged.AddListener(delegate (float value)
                {
                    if (selectedChar != null)
                    {
                        selectedChar.charInfo.gameObject.GetComponent<AdvIKCharaController>().ShoulderRightWeight = value;
                        weightRightSliderText.text = string.Format("R Shoulder Weight ({0:0.000})", WeightRight.value);
                    }
                });

                OffsetRight.onValueChanged.AddListener(delegate (float value)
                {
                    if (selectedChar != null)
                    {
                        selectedChar.charInfo.gameObject.GetComponent<AdvIKCharaController>().ShoulderRightOffset = value;
                        offsetRightSliderText.text = string.Format("R Shoulder Offset ({0:0.000})", OffsetRight.value);
                    }
                });

                SpineStiffness.onValueChanged.AddListener(delegate (float value)
                {
                    if (selectedChar != null)
                    {
                        selectedChar.charInfo.gameObject.GetComponent<AdvIKCharaController>().SpineStiffness = value;
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

            private static Toggle SetupToggle(string name, int pos, GameObject parent)
            {
                Toggle tglOriginal = GetPanelObject<Toggle>("Toggle Neck", parent);
                Toggle tgl = Instantiate(tglOriginal, parent.transform);
                tgl.name = name;
                tgl.transform.localPosition = new Vector3(tgl.transform.localPosition.x + 80, pos, tgl.transform.localPosition.z);
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

                Button[] buttons = listmenu.GetComponentsInChildren<Button>();

                GameObject originalPanel = GameObject.Find("StudioScene/Canvas Main Menu/02_Manipulate/00_Chara/02_Kinematic/00_FK");
                GameObject kineMenu = GameObject.Find("StudioScene/Canvas Main Menu/02_Manipulate/00_Chara/02_Kinematic");

                AdvIKPanel = Instantiate(originalPanel, kineMenu.transform, true);
                BreathPanel = Instantiate(originalPanel, kineMenu.transform, true);
                BreathShapePanel = Instantiate(originalPanel, kineMenu.transform, true);

                RectTransform rect = AdvIKPanel.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(202, 490);

                rect = BreathPanel.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(202, 490);

                rect = BreathShapePanel.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector3(222, 340);

                Button fkikSelectButton = newSelect.GetComponent<Button>();

                foreach (Button button in buttons)
                {
                    button.onClick.AddListener(delegate ()
                    {
                        AdvIKPanel.SetActive(false);
                        BreathPanel.SetActive(false);
                        BreathShapePanel.SetActive(false);
                        fkikSelectButton.image.color = Color.white;
                        openShapePanelButton.image.color = Color.white;
                    });
                }

                fkikSelectButton.onClick.RemoveAllListeners();
                fkikSelectButton.onClick.AddListener(delegate ()
                {
                    foreach (Transform child in kineMenu.transform)
                    {
                        if (child.name != "Viewport" && child.name != "Scrollbar Vertical")
                        {
                            child.gameObject.SetActive(false);
                        }
                    }
                    foreach (Button button in buttons)
                    {
                        button.image.color = Color.white;
                    }
                    if (showAdvIKPanel)
                    {
                        AdvIKPanel.SetActive(true);
                    }
                    else
                    {
                        BreathPanel.SetActive(true);
                    }
                    fkikSelectButton.image.color = Color.green;
                });
            }
        }
    }
}
