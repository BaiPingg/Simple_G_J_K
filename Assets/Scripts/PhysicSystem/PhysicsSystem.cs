using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace PhysicalSystem
{
    public class PhysicsSystem : MonoBehaviour
    {
        public Color colliding_color;
        public Color not_colliding_color;
        public Vector3 gravity;

        private void Start()
        {
        }

        private void FixedUpdate()
        {
            GJKCollider[] colliders = FindObjectsOfType<GJKCollider>();

            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].gameObject.GetComponent<Renderer>().material.color = not_colliding_color;
            }

            for (int i = 0; i < colliders.Length; i++)
            {
                for (int j = i + 1; j < colliders.Length; j++)
                {
                    bool collides = colliders[i].CollidesWithOther(colliders[j]);

                    if (collides)
                    {
                        MethodInfo info = colliders[i].gameObject.GetComponent(typeof(ISupport)).GetType().GetMethod("OnMyCollisionEnter");
                        if (info != null)
                        {
                            object[] parameters = new object[] { colliders[j] };
                            info.Invoke(colliders[i].gameObject.GetComponent<ISupport>(), parameters);
                        }
                        MethodInfo info1 = colliders[j].gameObject.GetComponent(typeof(ISupport)).GetType().GetMethod("OnMyCollisionEnter");
                        if (info1 != null)
                        {
                            object[] parameters = new object[] { colliders[i] };
                            info1.Invoke(colliders[j].gameObject.GetComponent<ISupport>(), parameters);
                        }
                        colliders[i].gameObject.GetComponent<Renderer>().material.color = colliding_color;
                        colliders[j].gameObject.GetComponent<Renderer>().material.color = colliding_color;
                    }
                }
            }
        }
    }
}