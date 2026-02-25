namespace Core.Events {
	public static class EventBus<T> where T: IGameEvent {
		private static event GameEventHandler<T> _event;
		
		public static event GameEventHandler<T> Event {
			add => _event += value;
			remove => _event -= value;
		}

		public static void Raise(T @event) {
			_event?.Invoke(@event);
		}
		public static void AddListener(GameEventHandler<T> handler) {
			_event += handler;
		}
		public static void RemoveListener(GameEventHandler<T> handler) {
			_event -= handler;
		}
		public static void RemoveAllListeners() {
			_event = null;
		}
	}
}