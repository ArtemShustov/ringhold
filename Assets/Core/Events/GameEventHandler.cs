namespace Core.Events {
	public delegate void GameEventHandler<T>(T gameEvent) where T: IGameEvent;
}