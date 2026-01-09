//using System;
//using System.Collections.Generic;
//using UnityEngine;

//public class LineRendererPath : MonoBehaviour
//{
//    public enum PathType { Linear, Curved }

//    [SerializeField] private PathType pathType = PathType.Curved;

//    // Only used if Linear
//    [SerializeField] private List<LineRenderer> segmentLines;

//    public bool UsesSegments => pathType == PathType.Linear;

//    public int SegmentCount =>
//        UsesSegments && segmentLines != null ? segmentLines.Count : 0;


//    public void Init(PathType pathType, List<LineRenderer> segmentLines = null)
//    {
//        this.pathType = pathType;

//        if (pathType == PathType.Linear)
//        {
//            this.segmentLines = segmentLines ?? new List<LineRenderer>();
//        }
//        if (pathType == PathType.Curved)
//        {
//            this.segmentLines = null;
//            GenerateCurvedPath();
//        }
//    }

//    private void GenerateCurvedPath()
//    {
//        throw new NotImplementedException();
//    }
//}
