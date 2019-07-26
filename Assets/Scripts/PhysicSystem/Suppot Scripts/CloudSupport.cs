using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhysicalSystem
{
    public class CloudSupport : MonoBehaviour, ISupport
    {
        public List<Vector3> vertices;

        public Vector3 Support(Vector3 direction)
        {
            List<Vector3> world_verts = new List<Vector3>();

            foreach (Vector3 vert in vertices)
            {
                Vector3 world_vert = transform.rotation * vert;
                world_vert += transform.position;
                world_verts.Add(world_vert);
            }

            List<Vector3> best_verts = new List<Vector3>();
            float best_dot = Mathf.NegativeInfinity;

            foreach (Vector3 vert in world_verts)
            {
                float dot = Vector3.Dot(vert, direction);
                if (dot == best_dot)
                {
                    best_verts.Add(vert);
                }
                else if (dot > best_dot)
                {
                    best_verts.Clear();
                    best_verts.Add(vert);
                    best_dot = dot;
                }
            }

            return best_verts[0];
        }
    }
}