using System.Collections;
using UnityEngine;

namespace VisualComponents.Blink {
    public class SpriteBlinkAnimation : MonoBehaviour {
        [SerializeField] 
        private SpriteRenderer _sprite;
        [SerializeField] 
        private Color _from = Color.white;
        [SerializeField] 
        private Color _to = Color.red;
        [SerializeField]
        private AnimationCurve _curve = AnimationCurve.Constant(0, 1, 1);
        [SerializeField] 
        private float _time = 0.5f;

        private Coroutine _currentBlink;

        [ContextMenu("Blink")]
        public void TryBlink() {
            if (_currentBlink == null) {
                _currentBlink = StartCoroutine(DoBlink());
            }
        }

        private IEnumerator DoBlink() {
            _sprite.color = _from;
            var time = 0f;
            while (time < _time) {
                time += Time.deltaTime;
                yield return new WaitForEndOfFrame();
                _sprite.color = Color.Lerp(_from, _to, _curve.Evaluate(time / _time));
            }
            _sprite.color = _from;
            _currentBlink = null;
        }
    }
}
