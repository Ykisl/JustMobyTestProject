using DG.Tweening;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization;
using Zenject;

namespace Game.CubeGame.GameMessage
{
    public class GameMessageView : MonoBehaviour
    {
        [SerializeField] private LocalizeStringEvent _localziedView;
        [SerializeField] private CanvasGroup _canvasGroup;
        [Space]
        [SerializeField] private float _waitTime = 3f;
        [SerializeField] private float _fadeTime = 0.5f;

        private IGameMessageSystem _messageSystem;

        private Sequence _fadeSequence;

        private const string LOCALE_TABLE_NAME = "GameMessages";

        [Inject]
        private void Construct(IGameMessageSystem messageSystem)
        {
            _messageSystem = messageSystem;
        }

        private void OnEnable()
        {
            ClearView();

            _messageSystem.OnMessageReceived += HandleMessageReceived;
        }

        private void OnDisable() 
        {
            _messageSystem.OnMessageReceived -= HandleMessageReceived;

            ClearView();
        }

        private void ClearView()
        {
            _fadeSequence?.Kill(true);

            _localziedView.StringReference = null;
            _canvasGroup.alpha = 0f;
        }

        private void HandleMessageReceived(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                ClearView();
                return;
            }

            var localizedString = new LocalizedString(LOCALE_TABLE_NAME, message);
            _localziedView.StringReference = localizedString;

            _fadeSequence?.Kill(true);
            _fadeSequence = DOTween.Sequence();

            _fadeSequence.AppendInterval(_waitTime);
            var fadeTween = _canvasGroup.DOFade(0f, _fadeTime).From(1f);
            _fadeSequence.Append(fadeTween);
            _fadeSequence.Play();
        }
    }
}
