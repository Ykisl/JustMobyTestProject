using UnityEngine;
using UnityEngine.UI;

namespace Game.CubeGame.View
{
    public class CubeItemViewModel
    {
        public Color CubeColor;
    }
    public class CubeItemView : MonoBehaviour
    {
        [SerializeField] protected Image _cubeImage;

        protected CubeItemViewModel _activeModel;

        public virtual void SetModel(CubeItemViewModel model)
        {
            _activeModel = model;
            if(_activeModel == null)
            {
                ResetModel();
                return;
            }

            _cubeImage.color = _activeModel.CubeColor;
        }

        public virtual void ResetModel()
        {
            _cubeImage.color = Color.white;
        }
    }
}
