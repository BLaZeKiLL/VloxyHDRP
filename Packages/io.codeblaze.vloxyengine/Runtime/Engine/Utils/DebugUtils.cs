using Runevision.Common;
using UnityEngine;

public static class DebugUtils {
    public static void DrawBounds(GridBounds bounds, Color color) {
        DebugDrawer.DrawRect(bounds.min.x, bounds.min.y, bounds.max.x, bounds.max.y, color);
    }
}