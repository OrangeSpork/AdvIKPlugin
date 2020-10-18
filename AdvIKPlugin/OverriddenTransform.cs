using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace AdvIKPlugin
{
    public class OverriddenTransform
    {
        public OverriddenTransform(Transform target, string name, bool position = true, bool rotation = false, bool scale = true)
        {
            Transform = target;
            Name = name;

            OriginalPosition = target.localPosition;
            OriginalRotation = target.localRotation;
            OriginalScale = target.localScale;

            OverridePosition = position;
            OverrideRotation = rotation;
            OverrideScale = scale;

          //  AdvIKPlugin.Instance.Log.LogInfo(string.Format("Recorded {0} Pos: {1} Rot: {2} Sca: {3}", name, OriginalPosition, OriginalRotation.eulerAngles, OriginalScale));
        }

        public void RecordPriorSnapshot()
        {
            PriorPosition = Transform.localPosition;
            PriorRotation = Transform.localRotation;
            PriorScale = Transform.localScale;
        }

        public void RecordFrameSnapshot()
        {
            LastFramePosition = Transform.localPosition;
            LastFrameRotation = Transform.localRotation;
            LastFrameScale = Transform.localScale;
        }

        public void RestorePriorSnapshot()
        {

            if (OverridePosition) Transform.localPosition = PriorPosition + (Transform.localPosition - LastFramePosition);
            if (OverrideRotation) Transform.localRotation = PriorRotation * (Transform.localRotation * Quaternion.Inverse(LastFrameRotation));
            if (OverrideScale) Transform.localScale = PriorScale + (Transform.localScale - LastFrameScale);
        }

        public void RecordOriginalSnapshot()
        {
            OriginalPosition = Transform.localPosition;
            OriginalRotation = Transform.localRotation;
            OriginalScale = Transform.localScale;
        }

        public void RestoreOriginalSnapshot()
        {
            if (OverridePosition) Transform.localPosition = OriginalPosition;
            if (OverrideRotation) Transform.localRotation = OriginalRotation;
            if (OverrideScale) Transform.localScale = OriginalScale;
        }

        public bool OverridePosition { get; set; }
        public bool OverrideRotation { get; set; }
        public bool OverrideScale { get; set; }

        public Vector3 OriginalPosition { get; set; }

        public Vector3 OriginalScale { get; set; }

        public Quaternion OriginalRotation { get; set; }

        public Vector3 TargetPosition { get; set; }

        public Vector3 TargetScale { get; set; }

        public Quaternion TargetRotation { get; set; }

        public Vector3 PriorPosition { get; set; }

        public Vector3 PriorScale { get; set; }

        public Quaternion PriorRotation { get; set; }

        public Vector3 LastFramePosition { get; set; }

        public Vector3 LastFrameScale { get; set; }

        public Quaternion LastFrameRotation { get; set; }

        public Transform Transform { get; set; }

        public string Name { get; set; }

    }
}
