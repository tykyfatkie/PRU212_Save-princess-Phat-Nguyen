//https://www.youtube.com/watch?v=KPoeNZZ6H4s

using UnityEngine;

namespace Cainos.Common
{
    [System.Serializable]
    public struct SecondOrderDynamics
    {
        private Vector3 xp;
        private Vector3 y, yd;
        private float k1, k2, k3;
        private Vector3 xd;

        private float k2_stable;

        [SerializeField] private float f;
        [SerializeField] private float d;
        [SerializeField] private float r;

        public float Frequency
        {
            get { return f; }
            set
            {
                if (value <= 0.01f) value = 0.01f;

                f = value;
                UpdateInnerParams();
            }
        }


        public float Damping
        {
            get { return d; }
            set
            {
                if (value <= 0.01f) value = 0.01f;

                d = value;
                UpdateInnerParams();
            }
        }

        public float Response
        {
            get { return r; }
            set
            {
                r = value;
                UpdateInnerParams();
            }
        }

        public SecondOrderDynamics(float frequency, float damping, float response)
        {
            this.f = 1.0f;
            this.d = 0.0f;
            this.r = 0.0f;
            xp = Vector3.zero;
            y = Vector3.zero;
            xd = Vector3.zero;
            yd = Vector3.zero;
            k1 = 0.0f;
            k2 = 0.0f;
            k3 = 0.0f;
            k2_stable = 0.0f;

            Reset(frequency, damping, response, Vector3.zero);
        }

        public void Reset(float frequency, float damping, float response, Vector3 x0)
        {
            f = frequency;
            d = damping;
            r = response;

            xp = x0;
            y = x0;
            xd = Vector3.zero;
            yd = Vector3.zero;

            UpdateInnerParams();
        }

        public void Reset(Vector3 x0)
        {
            Reset(f, d, r, x0);
        }

        public void Reset(Vector2 x0)
        {
            Reset(f, d, r, x0);
        }

        public void Reset(float x0)
        {
            Reset(f, d, r, Vector3.one * x0);
        }

        public Vector3 Update(Vector3 x, float t)
        {
            if (t < Mathf.Epsilon) return y;

            xd = (x - xp) / t;
            xp = x;

            k2_stable = Mathf.Max(k2, 1.1f * (t * t * 0.25f + t * k1 * 0.5f));
            y += t * yd;
            yd += t * (x + k3 * xd - y - k1 * yd) / k2_stable;

            return y;
        }

        public Vector2 Update(Vector2 x, float t)
        {
            return Update(new Vector3(x.x,x.y,0.0f), t);
        }

        public float Update(float x, float t)
        {
            return Update(Vector3.one * x, t).x;
        }

        private void UpdateInnerParams()
        {
            k1 = d / (Mathf.PI * f);
            k2 = 1.0f / ((2.0f * Mathf.PI * f) * (2.0f * Mathf.PI * f));
            k3 = r * d / (2.0f * Mathf.PI * f);
        }
    }
}
