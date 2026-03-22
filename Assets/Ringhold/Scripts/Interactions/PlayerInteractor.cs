using System.Linq;
using Core.Utils;
using Ringhold.Utils;
using UnityEngine;

namespace Ringhold.Interactions {
	public class PlayerInteractor: MonoBehaviour {
		[SerializeField] private float _radius = 5f;
		[SerializeField] private float _interactionRadius = 2.5f;
		[SerializeField] private LayerMask _mask = ~0;
		[SerializeField] private InteractionContext _context;

		private InteractionOverlap _overlap;
		private IInteraction _selected;

		private void Awake() {
			_overlap = new InteractionOverlap(_radius, _mask);
		}

		private void FixedUpdate() {
			_overlap.Update(transform.position);
			var best = _overlap.GetBestInRange(transform.position, _interactionRadius, _context);
			var added = _overlap.AddBuffer;
			var removed = _overlap.RemoveBuffer;

			foreach (var interaction in removed) {
				if (interaction != best) {
					interaction.SetInteractionState(InteractionHighlightState.None);
				}
			}

			if (best != _selected) {
				if (_selected != null && !removed.Contains(_selected)) {
					_selected.SetInteractionState(InteractionHighlightState.Visible);
				}
				_selected = best;
			}

			foreach (var interaction in added) {
				if (interaction != _selected) {
					interaction.SetInteractionState(InteractionHighlightState.Visible);
				}
			}

			if (_selected != null) {
				_selected.SetInteractionState(InteractionHighlightState.Selected);
			}
		}

		private void OnInteract() {
			if (_selected != null && _selected.CanInteract(_context)) {
				_selected.Interact(_context);
				return;
			}
			if (_context.Hand.Current != null) {
				_context.Hand.Drop();
				return;
			}
		}
		
		private void OnEnable() {
			_context.Character.Input.Interact += OnInteract;
		}
		private void OnDisable() {
			_context.Character.Input.Interact -= OnInteract;
		}

		#if DEBUG
		private void OnGUI() {
			foreach (var interaction in _overlap.Current.Keys) {
				if (interaction is MonoBehaviour mono && mono) {
					var canInteract = interaction.CanInteract(_context);
					var isSelected = interaction == _selected;
					var state = canInteract switch {
						true when isSelected => "<color=#00FF00>SELECTED</color>",
						false when isSelected => "<color=#FF00FF>SELECTED BLOCKED</color>",
						true => "<color=#FFFF00>VISIBLE</color>",
						false => "<color=#FF0000>BLOCKED</color>"
					};
					var type = interaction.GetType();
					DebugText.Draw($"[ {state} ]\n{type}", mono.transform.position, Color.white);
				}
			}
		}
		#endif

		private class InteractionOverlap: ComponentOverlapSphere<IInteraction> {
			public InteractionOverlap(float radius, LayerMask mask): base(radius, mask) { }

			public IInteraction GetBestInRange(Vector3 position, float maxRadius, InteractionContext context) {
				IInteraction best = null;
				var bestPriority = (InteractionPriority)(-1);
				var bestSqrDistance = float.MaxValue;
				var sqrMaxRadius = maxRadius * maxRadius;

				foreach (var (interaction, info) in Current) {
					var sqrDistance = (info.Transform.position - position).sqrMagnitude;
					if (sqrDistance > sqrMaxRadius) {
						continue;
					}
					if (!interaction.CanInteract(context)) {
						continue;
					}

					var priority = interaction.Priority;
					if (sqrDistance < bestSqrDistance || (Mathf.Approximately(sqrDistance, bestSqrDistance) && priority > bestPriority)) {
						best = interaction;
						bestPriority = priority;
						bestSqrDistance = sqrDistance;
					}
				}

				return best;
			}
		}
	}
}
