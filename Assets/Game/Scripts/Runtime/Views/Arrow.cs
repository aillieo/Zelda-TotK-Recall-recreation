namespace AillieoTech.Game.Views
{
    using System.Collections.Generic;
    using UnityEngine;

    public class Arrow : MonoBehaviour
    {
        [SerializeField]
        private GameObject head;

        [SerializeField]
        private LineRenderer body;

        public void SetPositions(IEnumerable<Vector3> positions)
        {
        }
    }
}
