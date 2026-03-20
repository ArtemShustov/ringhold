using UnityEngine;

namespace Ringhold.Utils {
	public class Bar: MonoBehaviour {
		[SerializeField, Range(0, 1)] private float _value = 1;
		[SerializeField] private RectTransform _fill;

		private void Awake() {
			_fill.offsetMax = Vector2.zero;
			_fill.offsetMin = Vector2.zero;
		}

		public void SetFill(float fill) {
			fill = Mathf.Clamp01(fill);
          
			_fill.anchorMax = new Vector2(fill, 1f);

			_value = fill;
		}
		
		private void OnValidate() {
			if (!_fill) {
				return;
			}
			_fill.offsetMax = Vector2.zero;
			_fill.offsetMin = Vector2.zero;
			SetFill(_value);
		}
	}
}