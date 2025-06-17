using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Popup {
    public class PopupSequence {
        public event Action Completed;
        
        private readonly Queue<PopupBox> _sequence;

        private readonly Dictionary<PopupBox, bool> _completionDictionary;

        public PopupSequence() {
            _sequence = new Queue<PopupBox>();
            _completionDictionary = new Dictionary<PopupBox, bool>();
        }

        public void Append(PopupBox box) {
            _completionDictionary[box] = false;
            box.closed = () => _completionDictionary[box] = true;
            _sequence.Enqueue(box);
        }

        public void Execute() {
            if (_sequence.Count == 0) {
                Completed?.Invoke();
                return;
            }
            Popups.ExecuteSequence(this);
        }

        public IEnumerator ShowBox() {
            if (_sequence.Count == 0) {
                yield break;
            }

            while (_sequence.Count > 0) {
                var box = _sequence.Dequeue();
                box.Show();
                yield return new WaitUntil(() => _completionDictionary[box]);
            }
            Completed?.Invoke();
        }

        public void Flush() {
            _sequence.Clear();
            _completionDictionary.Clear();
        }
    }
}