using CodeBlaze.Vloxy.Game;
using CodeBlaze.Vloxy.Engine;
using CodeBlaze.Vloxy.Engine.Settings;
using CodeBlaze.Vloxy.Engine.Utils;
using CodeBlaze.Vloxy.Engine.Utils.Extensions;
using Runevision.Common;
using Unity.Mathematics;
using Unity.Mathematics.Geometry;
using UnityEngine;

public class DiffBoundTest : MonoBehaviour
{
    [SerializeField] private VloxySettings _Settings;
    private int3 FocusChunkCoord;
    private GridBounds LastUpdateBound;
    private GridBounds DiffBounds; // this will be used to query chunks for meshing

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FocusChunkCoord = new int3(1,1,1) * int.MinValue;

        VloxyProvider.Initialize(new(), provider => {
            provider.Settings = _Settings;
        });

        LastUpdateBound = new(int.MinValue, int.MinValue, 160, 160); // size 128 * 128
        DiffBounds = LastUpdateBound;
    }

    // Update is called once per frame
    void Update()
    {
        // GetChunk coords does a floor which is not the same as LPG update so there is a issue between 0 -> 1
        // not major and can be fixed by having raster bound be 1 chunk larger than focus bound which is required by meshing anyway
        var NewFocusChunkCoord = VloxyUtils.GetChunkCoords(transform.position);

        if (!(NewFocusChunkCoord == FocusChunkCoord).AndReduce()) {
            FocusChunkCoord = NewFocusChunkCoord;
            GridBounds NewUpdateBound = new(-64 + FocusChunkCoord.x, -64 + FocusChunkCoord.z, 160, 160);
            DiffBounds = Diff(NewUpdateBound, LastUpdateBound);
            LastUpdateBound = NewUpdateBound;
        }

        DrawBounds(DiffBounds, Color.green);
        DrawBounds(LastUpdateBound, Color.cyan);
    }

    private void DrawBounds(GridBounds bounds, Color color) {
        DebugDrawer.DrawRect(bounds.min.x, bounds.min.y, bounds.max.x, bounds.max.y, color);
    }

    private GridBounds Diff(GridBounds new_bounds, GridBounds old_bounds) {
        var inter = new_bounds.GetIntersection(old_bounds);
        
        if (inter.empty)
            return new_bounds;

        if (inter.min.x > new_bounds.min.x) // Left side remains
            return GridBounds.MinMax(new_bounds.min.x, new_bounds.min.y, inter.min.x, new_bounds.max.y);
        else if (inter.max.x < new_bounds.max.x) // Right side remains
            return GridBounds.MinMax(inter.max.x, new_bounds.min.y, new_bounds.max.x, new_bounds.max.y);
        else if (inter.min.y > new_bounds.min.y) // Bottom side remains
            return GridBounds.MinMax(inter.min.x, new_bounds.min.y, inter.max.x, inter.min.y);
        else if (inter.max.y < new_bounds.max.y) // Top side remains
            return GridBounds.MinMax(inter.min.x, inter.max.y, inter.max.x, new_bounds.max.y);
        
        return GridBounds.Empty();
    }
}
