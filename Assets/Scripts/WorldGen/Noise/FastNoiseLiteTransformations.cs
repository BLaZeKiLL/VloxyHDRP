using Unity.Mathematics;

namespace CodeBlaze.Vloxy.Game
{
    public static class FastNoiseLiteTransformations
    {
        /// <summary>
        /// noise = [-1, 1] -scale-> [-scale, scale] -shift-> [-scale+shift, scale+shift]
        /// </summary>
        /// <param name="noise"></param>
        /// <param name="scale"></param>
        /// <param name="shift"></param>
        /// <returns></returns>
        public static int NoiseScaleShiftInt(this FastNoiseLite fnl, float x, float z, int scale = 1, int shift = 0)
        {
            return math.clamp((int)math.round(fnl.GetNoise(x, z) * scale), -scale, scale) + shift;
        }

        /// <summary>
        /// noise = [-1, 1] -scale-> [-scale, scale] -shift-> [-scale+shift, scale+shift]
        /// </summary>
        /// <param name="noise"></param>
        /// <param name="scale"></param>
        /// <param name="shift"></param>
        /// <returns></returns>
        public static float NoiseScaleShiftFloat(this FastNoiseLite fnl, float x, float z, float scale = 1f, float shift = 0f)
        {
            return math.clamp(fnl.GetNoise(x, z) * scale, -scale, scale) + shift;
        }

        /// <summary>
        /// noise = [-1, 1] -scale-> [-scale, scale] -shift-> [-scale+shift, scale+shift]
        /// </summary>
        /// <param name="noise"></param>
        /// <param name="scale"></param>
        /// <param name="shift"></param>
        /// <returns></returns>
        public static int NoiseScaleShiftInt(this FastNoiseLite fnl, float x, float y, float z, int scale = 1, int shift = 0)
        {
            return math.clamp((int)math.round(fnl.GetNoise(x, y, z) * scale), -scale, scale) + shift;
        }

        /// <summary>
        /// noise = [-1, 1] -scale-> [-scale, scale] -shift-> [-scale+shift, scale+shift]
        /// </summary>
        /// <param name="noise"></param>
        /// <param name="scale"></param>
        /// <param name="shift"></param>
        /// <returns></returns>
        public static float NoiseScaleShiftFloat(this FastNoiseLite fnl, float x, float y, float z, float scale = 1f, float shift = 0f)
        {
            return math.clamp(fnl.GetNoise(x, y, z) * scale, -scale, scale) + shift;
        }

        /// <summary>
        /// noise = [-1, 1] -curve-> [0, 1] -scale-> [0, scale] -shift-> [shift, scale+shift]
        /// </summary>
        /// <param name="noise"></param>
        /// <param name="curve"></param>
        /// <param name="scale"></param>
        /// <param name="shift"></param>
        /// <returns></returns>
        public static int CurveSampleNoiseScaleShiftInt(
            this FastNoiseLite fnl, float x, float z, BakedAnimationCurve curve, int scale = 1, int shift = 0
        )
        {
            var remap_val = curve.Evaluate(fnl.GetNoise(x, z), -1.0f, 1.0f);
            return math.clamp((int)math.round(remap_val * scale), 0, scale) + shift;
        }

        /// <summary>
        /// noise = [-1, 1] -curve-> [0, 1] -scale-> [0, scale] -shift-> [shift, scale+shift]
        /// </summary>
        /// <param name="noise"></param>
        /// <param name="curve"></param>
        /// <param name="scale"></param>
        /// <param name="shift"></param>
        /// <returns></returns>
        public static float CurveSampleNoiseScaleShiftFloat(
            this FastNoiseLite fnl, float x, float z, BakedAnimationCurve curve, float scale = 1f, float shift = 0
        )
        {
            var remap_val = curve.Evaluate(fnl.GetNoise(x, z), -1.0f, 1.0f);
            return math.clamp(remap_val * scale, 0f, scale) + shift;
        }

        /// <summary>
        /// noise = [-1, 1] -curve-> [0, 1] -scale-> [0, scale] -shift-> [shift, scale+shift]
        /// </summary>
        /// <param name="noise"></param>
        /// <param name="curve"></param>
        /// <param name="scale"></param>
        /// <param name="shift"></param>
        /// <returns></returns>
        public static int CurveSampleNoiseScaleShiftInt(
            this FastNoiseLite fnl, float x, float y, float z, BakedAnimationCurve curve, int scale = 1, int shift = 0
        )
        {
            var remap_val = curve.Evaluate(fnl.GetNoise(x, y, z), -1.0f, 1.0f);
            return math.clamp((int)math.round(remap_val * scale), 0, scale) + shift;
        }

        /// <summary>
        /// noise = [-1, 1] -curve-> [0, 1] -scale-> [0, scale] -shift-> [shift, scale+shift]
        /// </summary>
        /// <param name="noise"></param>
        /// <param name="curve"></param>
        /// <param name="scale"></param>
        /// <param name="shift"></param>
        /// <returns></returns>
        public static float CurveSampleNoiseScaleShiftFloat(
            this FastNoiseLite fnl, float x, float y, float z, BakedAnimationCurve curve, float scale = 1f, float shift = 0
        )
        {
            var remap_val = curve.Evaluate(fnl.GetNoise(x, y, z), -1.0f, 1.0f);
            return math.clamp(remap_val * scale, 0f, scale) + shift;
        }
    }
}