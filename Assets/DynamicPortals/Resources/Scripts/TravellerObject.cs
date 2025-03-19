using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DynamicPortals
{
    public class TravellerObject : MonoBehaviour
    {
        [SerializeField] GameObject _clonePrefab;

        GameObject _clone;
        public GameObject Clone => _clone;
        
        bool _previousSide;
        public bool PreviousSide
        {
            get => _previousSide;
            set { _previousSide = value; }
        }

        public void EnableClone()
        {
            if (_clone == null) _clone = Instantiate(_clonePrefab);
            else _clone.SetActive(true);
        }

        public void DisableClone()
        {
            _clone.SetActive(false);
        }
    }
}