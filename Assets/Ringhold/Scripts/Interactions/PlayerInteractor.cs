using System.Collections.Generic;
using Core.Utils;
using UnityEngine;

namespace Ringhold.Interactions {
	public class PlayerInteractor: MonoBehaviour {
		[SerializeField] private float _radius = 5f;
		[SerializeField] private float _interactionRadius = 2.5f;
		[SerializeField] private LayerMask _mask = ~0;
		[SerializeField] private InteractionContext _context;

		private readonly List<IInteraction> _visible = new List<IInteraction>();
		private readonly List<IInteraction> _buffer = new List<IInteraction>();
		private readonly Collider[] _hits = new Collider[16];
		private IInteraction _selected;

		private void Update() {
			UpdateVisible();
			UpdateSelected();
		}

		private void UpdateVisible() {
			foreach (var interaction in _visible) {
				interaction.SetInteractionState(InteractionHighlightState.None);
			}
			_visible.Clear();

			var size = Physics.OverlapSphereNonAlloc(transform.position, _radius, _hits, _mask);
			for (var i = 0; i < size; i++) {
				_hits[i].GetComponents(_buffer);
				_visible.AddRange(_buffer);
			}

			foreach (var interaction in _visible) {
				interaction.SetInteractionState(InteractionHighlightState.Visible);
			}
		}

		private void UpdateSelected() {
			IInteraction nearest = null;
			var minDist = float.MaxValue;
			var maxPriority = InteractionPriority.Lowest;
			var interactionRadiusSqr = _interactionRadius * _interactionRadius;
			
			foreach (var interaction in _visible) {
				if (!interaction.CanInteract(_context) || interaction is not MonoBehaviour mono) {
					continue;
				}
				var dist = (transform.position - mono.transform.position).sqrMagnitude;
				if (dist > interactionRadiusSqr) {
					continue;
				}
				var priority = interaction.Priority;
				if (priority < maxPriority) {
					continue;
				}
				if (priority == maxPriority && dist >= minDist) {
					continue;
				}
				minDist = dist;
				maxPriority = priority;
				nearest = interaction;
			}

			if (_selected != nearest) {
				_selected?.SetInteractionState(InteractionHighlightState.Visible);
				_selected = nearest;
				_selected?.SetInteractionState(InteractionHighlightState.Selected);
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
			foreach (var interaction in _visible) {
				if (interaction is MonoBehaviour mono) {
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
	}
}
