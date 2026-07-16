using System;
using UnityEngine;

namespace HueSeek.Paint
{
    public enum BrushTool
    {
        Freehand,
        BucketFill,
        PatternStamp
    }

    public enum PatternStampType
    {
        Stripes,
        Dots,
        Checker,
        WoodGrain
    }

    [Serializable]
    public struct PaintStroke
    {
        public int PlayerId;
        public long TimestampMs;
        public Vector3 WorldHitPoint;
        public Vector3 WorldNormal;
        public Color Color;
        public PaintMaterialProperties Material;
        public BrushTool Tool;
        public PatternStampType Pattern;
        public float BrushRadius;
        public float BrushPressure;
        public int SequenceNumber;
    }
}
