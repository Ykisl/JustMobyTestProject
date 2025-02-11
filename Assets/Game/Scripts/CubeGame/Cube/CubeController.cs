using DG.Tweening;
using Game.CubeGame.Models;
using Game.CubeGame.View;
using Game.Drag;
using System;
using UnityEngine;

namespace Game.CubeGame.Cube
{
    public class CubeController : MonoBehaviour, IDraggable
    {
        [SerializeField] private CubeItemView _itemView;
        [SerializeField] private RectTransform _rectTransform;
        [Space]
        [Header("Fade out")]
        [SerializeField] private CanvasGroup _fadeCanvasGorup;
        [SerializeField] private float _fadeOutTime = 0.5f;

        private CubeModel _model;

        private Sequence _fadeSequence;

        public Rect CubeRect
        {
            get => _rectTransform.rect;
        }

        public Transform DraggableTransform => _rectTransform;

        public bool IsDragAvalible => _fadeSequence == null || !_fadeSequence.IsPlaying();

        public event Action<CubeController> OnRemove;

        public virtual void SetModel(CubeModel model)
        {
            _model = model;

            if (_model == null)
            {
                ResetModel();
                return;
            }

            ResetFadeTween();
            UpdateView(_model);
        }

        public virtual void ResetModel()
        {
            ResetFadeTween();

            _itemView?.ResetModel();
            _model = null;
        }

        public virtual void Remove()
        {
            OnRemove?.Invoke(this);
        }

        public void RemoveWithFade()
        {
            ResetFadeTween();

            _fadeSequence = DOTween.Sequence();
            _fadeSequence.Append(_fadeCanvasGorup.DOFade(0f, _fadeOutTime));
            _fadeSequence.OnComplete(() =>
            {
                Remove();
            });

            _fadeSequence.Play();
        }

        protected void ResetFadeTween()
        {
            _fadeSequence?.Kill(false);
            _fadeCanvasGorup.alpha = 1f;
        }

        protected void UpdateView(CubeModel model)
        {
            if (model == null)
            {
                return;
            }

            var viewModel = new CubeItemViewModel
            {
                CubeColor = model.Color,
            };

            _itemView?.SetModel(viewModel);
        }
    }
}
