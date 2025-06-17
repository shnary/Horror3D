using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Patterns {
    public class StateMachine {
        private IState _currentState;

        private readonly Dictionary<Type, List<Transition>> _transitionsDictionary;
        private List<Transition> _currentTransitions;
        private readonly List<Transition> _anyTransitions;

        private static readonly List<Transition> _emptyTransitions = new List<Transition>(0);

        public StateMachine() {
            _transitionsDictionary = new Dictionary<Type, List<Transition>>();
            _currentTransitions = new List<Transition>();
            _anyTransitions = new List<Transition>();
        }

        public void Tick() {
            var transition = GetTransition();
            if (transition != null) {
                SetState(transition.To);
            }

            _currentState?.Tick();
        }

        public void SetState(IState state) {
            if (state == _currentState) {
                return;
            }

            _currentState?.OnExit();
            _currentState = state;

            _transitionsDictionary.TryGetValue(_currentState.GetType(), out _currentTransitions);
            if (_currentTransitions == null) {
                _currentTransitions = _emptyTransitions;
            }

            _currentState.OnEnter();
        }

        public void AddTransition(IState from, IState to, Func<bool> predicate) {
            if (_transitionsDictionary.TryGetValue(from.GetType(), out var transitions) == false) {
                transitions = new List<Transition>();
                _transitionsDictionary[from.GetType()] = transitions;
            }

            transitions.Add(new Transition(to, predicate));
        }

        public void AddAnyTransition(IState state, Func<bool> predicate) {
            _anyTransitions.Add(new Transition(state, predicate));
        }

        private Transition GetTransition() {
            foreach (var transition in _anyTransitions) {
                if (transition.Condition()) {
                    return transition;
                }
            }

            foreach (var transition in _currentTransitions) {
                if (transition.Condition()) {
                    return transition;
                }
            }

            return null;
        }

        private class Transition {
            public Func<bool> Condition {get;}
            public IState To {get;}

            public Transition(IState to, Func<bool> condition) {
                To = to;
                Condition = condition;
            }
        }
    }
}