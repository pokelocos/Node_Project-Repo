using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MicroFactory
{ 
    public class EffectController : MonoBehaviour
    {

        public delegate void EffectEvent(EffectData effect);
        public EffectEvent OnEndEffect;

        private EffectData data;
        [SerializeField] private EffectView view;

        private float currentTime = 0;

        public EffectData Data { get => data; }
        public float CurrentTime { get => currentTime; }

        void Update()
        {
            if (data == null)
                return;

            //timer
            if (currentTime >= data.duration)
            {
                OnEndEffect?.Invoke(data);
                view.animator.SetTrigger("close");
                view.animator.SetTrigger("finish");
            }
            currentTime += Time.deltaTime;
            view.SetTimer(data.duration, currentTime);
        }

        internal void Init(EffectData effectData,float startTime)
        {
            data = effectData;
            currentTime = startTime;

        }
    }
}
