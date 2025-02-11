using Game.CubeGame.Models;
using Game.CubeGame.View;
using Game.Drag;
using UnityEngine;

namespace Game.CubeGame.Cube
{
    public class CubeController : MonoBehaviour, IDraggable
    {
        [SerializeField] private CubeItemView _itemView;
        [SerializeField] private RectTransform _rectTransform;

        private CubeModel _model;

        public Rect CubeRect
        {
            get => _rectTransform.rect;
        }

        public Transform DraggableTransform => _rectTransform;

        public bool IsDragAvalible => true;

        public virtual void SetModel(CubeModel model)
        {
            _model = model;

            if (_model == null)
            {
                ResetModel();
                return;
            }

            UpdateView(_model);
        }

        public virtual void ResetModel()
        {
            _itemView?.ResetModel();
            _model = null;
        }

        private void UpdateView(CubeModel model)
        {
            if(model == null)
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
