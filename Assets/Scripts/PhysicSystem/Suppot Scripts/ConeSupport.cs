using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhysicalSystem
{
    public class ConeSupport : MonoBehaviour, ISupport
    {
        public float radius;
        public float height;

        public Vector3 Support(Vector3 direction)
        {
            Vector3 local_direction = transform.InverseTransformDirection(direction);

            float x = local_direction.x;
            float y = local_direction.y;
            float z = local_direction.z;

            float sin_top_angle = radius / Mathf.Sqrt(radius * radius + height * height);
            float direction_to_z_axis_dist = Mathf.Sqrt(x * x + y * y);

            Vector3 local_result;
            if (z > local_direction.magnitude * sin_top_angle)
            {
                local_result = new Vector3(0, 0, height / 2);
            }
            else if (direction_to_z_axis_dist > 0)
            {
                float mult = radius / direction_to_z_axis_dist;
                local_result = new Vector3(mult * x, mult * y, -height / 2);
            }
            else
            {
                local_result = new Vector3(0, 0, -height / 2);
            }

            return transform.rotation * local_result + transform.position;
        }
    }
}