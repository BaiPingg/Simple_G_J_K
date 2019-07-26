using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhysicalSystem
{
    public class SemisphereSupport : MonoBehaviour, ISupport
    {
        public float radius;
        public float center_to_base;

        public Vector3 Support(Vector3 direction)
        {
            Vector3 base_center = transform.position + transform.right.normalized * center_to_base;

            if (direction.normalized == transform.right.normalized)
            {
                return base_center;
            }
            else if (Vector3.Dot(transform.right, direction) > 0)
            {
                Vector3 horiz = Vector3.ProjectOnPlane(direction.normalized, transform.right).normalized;
                return base_center + horiz * radius;
            }
            else
            {
                return base_center + direction.normalized * radius;
            }
        }
    }
}