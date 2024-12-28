using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace CodeBlaze.Vloxy.Demo {
    /// <summary>
    /// Since Animation Curve is not thread safe this is required
    /// </summary>
    public class BakedAnimationCurve {
        private readonly List<float> _points;
        private readonly int _resolution;

        public BakedAnimationCurve(AnimationCurve curve, int resolution) {
            _points = new List<float>(resolution);
            _resolution = resolution;

            for (int i = -(resolution/2); i < resolution/2; i++)
            {
                _points.Add(curve.Evaluate(i / ((float) (resolution/2))));            
            }  
        }

        public float Evaluate(float x, float tmin = 0.0f, float tmax = 1.0f) {
            x = (x + math.abs(tmin)) / (math.abs(tmin) + math.abs(tmax));
            return _points[math.clamp((int) math.round(x * _resolution), 0, _resolution-1)];
        }
    }
}