using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace StreamingLibrary
{
    public class InternalEventBehaviour : MonoBehaviour
    {
        public event Action OnStart;

        public event Action OnUpdate;

        public event Action OnFixedUpdate;

        public event Action OnLateUpdate;

        private void Start()
        {
            this.OnStart?.Invoke();
        }

        private void Update()
        {
            this.OnUpdate?.Invoke();
        }

        private void FixedUpdate()
        {
            this.OnFixedUpdate?.Invoke();
        }

        private void LateUpdate()
        {
            this.OnLateUpdate?.Invoke();
        }
    }
}
