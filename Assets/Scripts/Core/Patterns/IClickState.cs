namespace Core.Patterns {
    public interface IClickState<T> {
        void OnEnter();

        void OnClick(T target);

        void OnExit();
    }
}