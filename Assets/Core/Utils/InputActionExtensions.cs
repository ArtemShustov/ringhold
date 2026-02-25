using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;

namespace Core.Utils {
	public static class InputActionExtensions {
		/// <summary>
		///     Subscribes to all phases of the input action (started, performed, canceled).
		/// </summary>
		/// <param name="inputAction">The input action to subscribe to.</param>
		/// <param name="callback">The callback to invoke for all phases.</param>
		public static void SubscribeAll(this InputAction inputAction, Action<InputAction.CallbackContext> callback) {
			inputAction.started += callback;
			inputAction.performed += callback;
			inputAction.canceled += callback;
		}
		/// <summary>
		///     Unsubscribes from all phases of the input action (started, performed, canceled).
		/// </summary>
		/// <param name="inputAction">The input action to unsubscribe from.</param>
		/// <param name="callback">The callback to remove from all phases.</param>
		public static void UnsubscribeAll(this InputAction inputAction, Action<InputAction.CallbackContext> callback) {
			inputAction.started -= callback;
			inputAction.performed -= callback;
			inputAction.canceled -= callback;
		}

		/// <summary>
		///     Determines whether the input device is a keyboard or mouse.
		/// </summary>
		/// <param name="device">The input device to check.</param>
		/// <returns>True if the device is a keyboard or mouse; otherwise, false.</returns>
		public static bool IsKeyboardOrMouse(this InputDevice device) {
			return device is Keyboard or Mouse;
		}

		/// <summary>
		///     Asynchronously waits for the input action to be performed.
		/// </summary>
		/// <param name="action">The input action to wait for.</param>
		/// <param name="token">Cancellation token to cancel the wait operation.</param>
		/// <param name="onPerformed">Optional callback invoked when the action is performed.</param>
		/// <returns>A task that completes with the callback context when the action is performed.</returns>
		/// <exception cref="OperationCanceledException">Thrown when the operation is canceled via the cancellation token.</exception>
		public static async UniTask<InputAction.CallbackContext> WaitForPerformed(this InputAction action, CancellationToken token, Action<InputAction.CallbackContext> onPerformed = default) {
			UniTaskCompletionSource<InputAction.CallbackContext> tcs = new UniTaskCompletionSource<InputAction.CallbackContext>();
			action.performed += Handler;

			await using (token.Register(Cancel)) {
				return await tcs.Task;
			}

			void Cancel() {
				action.performed -= Handler;
				tcs.TrySetCanceled(token);
			}

			void Handler(InputAction.CallbackContext context) {
				onPerformed?.Invoke(context);
				action.performed -= Handler;
				tcs.TrySetResult(context);
			}
		}
		/// <summary>
		///     Asynchronously waits for the input action to be performed without throwing exceptions on cancellation.
		/// </summary>
		/// <param name="action">The input action to wait for.</param>
		/// <param name="token">Cancellation token to cancel the wait operation.</param>
		/// <returns>True if the action was performed; false if the operation was canceled.</returns>
		public static async UniTask<bool> WaitForPerformedSafe(this InputAction action, CancellationToken token) {
			try {
				await action.WaitForPerformed(token);
				return true;
			}
			catch (OperationCanceledException) {
				return false;
			}
		}
	}
}