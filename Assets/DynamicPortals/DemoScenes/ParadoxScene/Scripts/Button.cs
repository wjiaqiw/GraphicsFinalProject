using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DynamicPortals
{
    public class Button : MonoBehaviour
    {
        bool _isPressed = false;
        Animator _anim;
        public Animator _targetAnim;

        void Start()
        {
            _anim = GetComponent<Animator>();
        }

        void Update()
        {
            _anim.SetBool("IsPressed", _isPressed);
            _targetAnim.SetBool("IsPressed", _isPressed);
        }

        void OnTriggerEnter()
        {
            _isPressed = true;
        }

        void OnTriggerExit()
        {
            _isPressed = false;
        }
    }
}