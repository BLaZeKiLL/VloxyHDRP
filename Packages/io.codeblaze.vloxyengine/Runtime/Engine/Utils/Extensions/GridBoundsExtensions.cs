using Runevision.Common;

public static class GridBoundsExtensions {
    public static GridBounds DiffBounds(this GridBounds A, GridBounds B) {
        var inter = A.GetIntersection(B);
        
        if (inter.empty)
            return A;

        if (inter.min.x > A.min.x) // Left side remains
            return GridBounds.MinMax(A.min.x, A.min.y, inter.min.x, A.max.y);
        else if (inter.max.x < A.max.x) // Right side remains
            return GridBounds.MinMax(inter.max.x, A.min.y, A.max.x, A.max.y);
        else if (inter.min.y > A.min.y) // Bottom side remains
            return GridBounds.MinMax(inter.min.x, A.min.y, inter.max.x, inter.min.y);
        else if (inter.max.y < A.max.y) // Top side remains
            return GridBounds.MinMax(inter.min.x, inter.max.y, inter.max.x, A.max.y);
        
        return GridBounds.Empty();
    }
}