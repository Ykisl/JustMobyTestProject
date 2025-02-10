using System;
using UnityEngine;

namespace Game.CubeGame.Models
{
    [Serializable]
    public class CubeModel
    {
        [SerializeField] private string _id;
        [SerializeField] private Color _color;

        public string Id => _id;

        public Color Color => _color;
    }
}
