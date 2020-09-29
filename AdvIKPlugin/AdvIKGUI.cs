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

namespace AdvIKPlugin
{
    public partial class AdvIKPlugin
    {
        internal class AdvIKGUI
        {
            private static GameObject AdvIKPanel;
            private static Toggle ShoulderRotatorToggle;
            private static Slider Weight;
            private static Slider Offset;
            private static Slider SpineStiffness;
            private static Text weightSliderText;
            private static Text offsetSliderText;
            private static Text spineSliderText;

            private static OCIChar selectedChar;

            internal static void InitUI()
            {
                if (AdvIKPanel == null)
                {
                    CreateMenuEntry();
                    CreatePanel();
                    SetupControls();
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
                    Weight.value = advIKController.ShoulderWeight;
                    Offset.value = advIKController.ShoulderOffset;
                    SpineStiffness.value = advIKController.SpineStiffness;
                }
            }

            public static void SetupControls()
            {
                Text shoulderToggleText = SetupText("ShoulderRotationEnabled", -50, "Shoulder Rotation");
                shoulderToggleText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
                shoulderToggleText.fontSize = 16;
                ShoulderRotatorToggle = SetupToggle("ShoulderRotationEnabledToggle", -50);

                ShoulderRotatorToggle.onValueChanged.AddListener(delegate (bool value)
                {
                    if (selectedChar != null)
                    {
                        selectedChar.charInfo.gameObject.GetComponent<AdvIKCharaController>().ShoulderRotationEnabled = value;
                    }
                });

                var sldSize = GetPanelObject<Slider>("Slider Size");

                weightSliderText = SetupText("WeightSlider", -90, "Shoulder Weight");
                weightSliderText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 200);
                weightSliderText.fontSize = 20;
                Weight = Instantiate(sldSize, AdvIKPanel.transform);
                Weight.name = "WeightOffset";
                Weight.transform.SetLocalScale(1.5f, 1.0f, 1.0f);
                Weight.transform.SetLocalPosition(20, -120, 0);
                Weight.minValue = 0;
                Weight.maxValue = 5;

                offsetSliderText = SetupText("OffsetSlider", -150, "Shoulder Offset");
                offsetSliderText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 200);
                offsetSliderText.fontSize = 20;
                Offset = Instantiate(sldSize, AdvIKPanel.transform);
                Offset.name = "ShoulderOffset";
                Offset.transform.SetLocalScale(1.5f, 1.0f, 1.0f);
                Offset.transform.SetLocalPosition(20, -180, 0);
                Offset.minValue = 0;
                Offset.maxValue = 1;

                spineSliderText = SetupText("SpineSlider", -220, "Spine Stiffness");
                spineSliderText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 200);
                spineSliderText.fontSize = 20;
                SpineStiffness = Instantiate(sldSize, AdvIKPanel.transform);
                SpineStiffness.name = "SpineStiffness";
                SpineStiffness.transform.SetLocalScale(1.5f, 1.0f, 1.0f);
                SpineStiffness.transform.SetLocalPosition(20, -250, 0);
                SpineStiffness.minValue = 0;
                SpineStiffness.maxValue = 1;

                Weight.onValueChanged.RemoveAllListeners();
                Offset.onValueChanged.RemoveAllListeners();
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
                        && child.gameObject != weightSliderText.gameObject && child.gameObject != offsetSliderText.gameObject && child.gameObject != spineSliderText.gameObject)
                    {
                        Destroy(child.gameObject);
                    }
                }
            }

            private static T GetPanelObject<T>(string name) => AdvIKPanel.GetComponentsInChildren<RectTransform>(true).First(x => x.name == name).GetComponent<T>();

            private static Text SetupText(string name, int pos, string text)
            {
                Text txt = Instantiate(GetPanelObject<Text>("Text Neck"), AdvIKPanel.transform);
                txt.name = name;
                txt.text = text;
                txt.transform.localPosition = new Vector3(txt.transform.localPosition.x, pos, txt.transform.localPosition.z);
                return txt;
            }

            private static Toggle SetupToggle(string name, int pos)
            {
                Toggle tglOriginal = GetPanelObject<Toggle>("Toggle Neck");
                Toggle tgl = Instantiate(tglOriginal, AdvIKPanel.transform);
                tgl.name = name;
                tgl.transform.localPosition = new Vector3(tgl.transform.localPosition.x + 70, pos, tgl.transform.localPosition.z);
                tgl.onValueChanged.RemoveAllListeners();
                return tgl;

            }

            public static void CreatePanel()
            {
                Texture2D tex = new Texture2D(152, 272);
                tex.LoadImage(LoadPanel());
                Image backingPanel = AdvIKPanel.GetComponent<Image>();
                Sprite replacement = Sprite.Create(tex, backingPanel.sprite.rect, backingPanel.sprite.pivot);
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

                RectTransform rect = AdvIKPanel.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, 395);

                Button fkikSelectButton = newSelect.GetComponent<Button>();

                foreach (Button button in buttons)
                {
                    button.onClick.AddListener(delegate ()
                    {
                        AdvIKPanel.SetActive(false);
                        fkikSelectButton.image.color = Color.white;
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
                    AdvIKPanel.SetActive(true);
                    fkikSelectButton.image.color = Color.green;
                });
            }
        }
    }
}
