using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jois
{
    public class UParticleSystemSpeed : MonoBehaviour
    {
        [SerializeField] private float _speed = 1F;
        private ParticleSystem _ps;

        void Start()
        {
            //if (_ps == null)
            _ps = gameObject.GetComponent<ParticleSystem>();

            var main = _ps.main;
            main.simulationSpeed = _speed;
        }
    }
}