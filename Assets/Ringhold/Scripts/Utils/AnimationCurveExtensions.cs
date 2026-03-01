using UnityEngine;

namespace Ringhold.Utils {
	public static class AnimationCurveExtensions {
		public static float InverseSampleCurve(this AnimationCurve curve, float value, int samples = 64) {
			var bestT = 0f;
			var bestDiff = float.MaxValue;

			for (int i = 0; i <= samples; i++) {
				var t = (float)i / samples;
				var diff = Mathf.Abs(curve.Evaluate(t) - value);
				if (diff < bestDiff) {
					bestDiff = diff;
					bestT = t;
				}
			}

			return bestT;
		}
	}
}