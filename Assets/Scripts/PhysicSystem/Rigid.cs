using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  F=ma;
 *  Vt = V0 + a *t;
 *  Vt = V0 + F/m*t;
 *
 */

namespace PhysicalSystem
{
    public class Rigid : MonoBehaviour
    {
        public float mass = 1;//质量

        public bool useGravity = true;
        private Vector3 force;//力
        private Vector3 velocity;//速度
        private Vector3 angularVelocity;//角速度
        private Vector3 acceleratedSpeed;//加速度

        [Tooltip("空气阻力系数")]
        [Range(0, 1)]
        public float drag;

        [Tooltip("角阻力阻力系数")]
        [Range(0, 1)]
        public float angularDrag;

        public Vector3 Force { get { return force; } }

        private void Start()
        {
            if (useGravity)
            {
                AddForce(FindObjectOfType<PhysicsSystem>().gravity);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                AddForce(Vector3.forward * 2);
            }
            if (Input.GetKeyUp(KeyCode.W))
            {
                SetForce(Vector3.zero);
            }
        }

        private void FixedUpdate()
        {
            velocity *= (1 - drag);//线性衰减
            angularVelocity *= (1 - angularDrag);//角速度衰减
            velocity = velocity + acceleratedSpeed * Time.fixedDeltaTime; //VT = V0
            velocity = velocity.magnitude < 0.01 ? Vector3.zero : velocity;
            angularVelocity = angularVelocity.magnitude < 0.01 ? Vector3.zero : angularVelocity;
            transform.position += velocity;
        }

        private void AddForce(Vector3 vector)
        {
            SetForce(Force + vector);
        }

        private void SetForce(Vector3 force)
        {
            this.force = force;
            this.acceleratedSpeed = this.force / mass;
        }

        private void AddVelocity()
        {
        }
    }
}