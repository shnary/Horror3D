
namespace Core.Patterns {
    public class ClickModeStateMachine<T> {
        private IClickState<T> _currentState;

        public void SetState(IClickState<T> state) {
            if (_currentState != null) {
                _currentState.OnExit();
            }
            state.OnEnter();
            _currentState = state;
        }

        public void OnClick(T target) {
            _currentState.OnClick(target);
        }
    }
}