using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhysicalSystem
{
    public class CylinderSupport : MonoBehaviour, ISupport
    {
        public float radius;
        public float height;

        public Vector3 Support(Vector3 direction)
        {
            direction = direction.normalized;

            Vector3 w = direction - Vector3.Dot(transform.up, direction) * transform.up;

            float sign = Mathf.Sin(Vector3.Dot(transform.up, direction)) > 0 ? 1 : -1;

            Vector3 temp = transform.position + sign * height / 2 * transform.up;

            if (w != Vector3.zero)
            {
                return temp + radius * w.normalized;
            }
            else
            {
                return temp;
            }
        }
    }
}