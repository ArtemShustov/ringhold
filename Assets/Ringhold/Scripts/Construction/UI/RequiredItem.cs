using Ringhold.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ringhold.Construction.UI {
	public class RequiredItem: MonoBehaviour {
		[Header("Settings")]
		[SerializeField] private string _pattern = "{0}/{1}";
		[SerializeField] private Color _incompleteColor = Color.red;
		[SerializeField] private Color _completeColor = Color.green;
		
		[Header("Components")]
		[SerializeField] private Image _icon;
		[SerializeField] private TMP_Text _countLabel;

		private int _current;
		private int _required;

		public void SetItem(ItemDefinition item, int required) {
			// TODO: _icon = stack.Icon;
			_required = required;
			
			Refresh();
		}
		public void SetCurrent(int current) {
			_current = current;
			
			Refresh();
		}

		public void Refresh() {
			UpdateCountLabel();
		}

		private void UpdateCountLabel() {
			_countLabel.color = _current >= _required ? _completeColor : _incompleteColor;
			_countLabel.text = string.Format(_pattern, _current, _required);
		}
	}
}