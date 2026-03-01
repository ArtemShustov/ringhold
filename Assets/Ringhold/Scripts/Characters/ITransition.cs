namespace Ringhold.Characters {
	public interface ITransition {
		bool Check(ICharacterState current);
		void Execute();
	}
}