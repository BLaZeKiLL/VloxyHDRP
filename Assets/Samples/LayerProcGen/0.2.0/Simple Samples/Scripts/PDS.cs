using System.Collections.Generic;
using Gists;
using Runevision.Common;
using UnityEngine;

public class PDS : MonoBehaviour
{
    private List<Vector2> pts;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pts = FastPoissonDiskSampling.Sampling(Vector2.one * -1000, Vector2.one * 1000, 20);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var p in pts)
        {
            DebugDrawer.DrawCross(p, 2f, UnityEngine.Color.red);
        }
    }
}
