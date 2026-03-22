using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Ringhold.Interactions;
using Ringhold.Items;
using Ringhold.Utils;
using UnityEngine;

namespace Ringhold.Construction.UI {
	public class ConstructionGhostUI: MonoBehaviour {
		[Header("Settings")]
		[SerializeField] private RequiredItem _itemPrefab;
		[SerializeField] private float _animationDuration = 0.25f;
		[SerializeField] private AnimationCurve _scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

		[Header("Components")]
		[SerializeField] private Transform _viewRoot;
		[SerializeField] private ConstructionGhost _building;
		[SerializeField] private Transform _itemsContainer;
		[SerializeField] private Bar _progressBar;

		private readonly Dictionary<ItemDefinition, RequiredItem> _items = new();
		private CancellationTokenSource _animationCts;
		private bool _isShown;

		private void OnEnable() {
			_building.SchemeChanged += OnSchemeChanged;
			_building.ProgressChanged += OnProgressChanged;
			_building.StackChanged += OnStackChanged;
			_building.InteractionStateChanged += OnInteractionStateChanged;

			_viewRoot.localScale = Vector3.zero;
			_viewRoot.gameObject.SetActive(false);
		}
		private void OnDisable() {
			_building.SchemeChanged -= OnSchemeChanged;
			_building.ProgressChanged -= OnProgressChanged;
			_building.StackChanged -= OnStackChanged;
			_building.InteractionStateChanged -= OnInteractionStateChanged;

			_animationCts?.Cancel();
			_animationCts?.Dispose();
		}

		private void OnInteractionStateChanged(InteractionHighlightState state) {
			Debug.Log($"OnInteractionStateChanged: {state}");
			var shouldShow = state is InteractionHighlightState.Visible or InteractionHighlightState.Selected;
			if (shouldShow == _isShown) {
				return;
			}

			_isShown = shouldShow;
			Animate(shouldShow).Forget();
		}

		private async UniTask Animate(bool visible) {
			_animationCts?.Cancel();
			_animationCts?.Dispose();
			_animationCts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);

			if (visible) {
				_viewRoot.gameObject.SetActive(true);
			}

			try {
				var time = 0f;
				while (time < _animationDuration) {
					time += Time.deltaTime;

					var progress = Mathf.Clamp01(time / _animationDuration);
					var evaluateTime = visible ? progress : (1f - progress);
					_viewRoot.localScale = Vector3.one * _scaleCurve.Evaluate(evaluateTime);
					
					await UniTask.NextFrame(_animationCts.Token);
				}
				
				_viewRoot.localScale = visible ? Vector3.one : Vector3.zero;
				if (!visible) {
					_viewRoot.gameObject.SetActive(false);
				}
			}
			catch (OperationCanceledException) { }
		}

		private void OnSchemeChanged(ConstructionSchemeDefinition scheme) {
			foreach (var view in _items.Values) {
				Destroy(view.gameObject);
			}
			_items.Clear();

			foreach (var requiredStack in scheme.RequiredItems) {
				var view = Instantiate(_itemPrefab, _itemsContainer);
				view.gameObject.SetActive(true);
				_items.Add(requiredStack.Item, view);

				var existingCount = _building.Stored.FirstOrDefault(s => s.Item == requiredStack.Item)?.Count ?? 0;
				view.SetCurrent(existingCount);
				view.SetItem(requiredStack.Item, requiredStack.Count);
			}

			_progressBar.SetFill(_building.GetProgress());
		}
		private void OnProgressChanged(float value) {
			_progressBar.SetFill(value);
		}
		private void OnStackChanged(ItemStack stack) {
			if (!_items.TryGetValue(stack.Item, out var view)) {
				return;
			}
			view.SetCurrent(stack.Count);
		}
	}
}