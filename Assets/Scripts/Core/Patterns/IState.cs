namespace Core.Patterns {
    public interface IState {
        void OnEnter();
        
        void Tick();

        void OnExit();
    }
}
