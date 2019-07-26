using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhysicalSystem
{
    [RequireComponent(typeof(ISupport))]
    //必须包含ISupport组件
    public class GJKCollider : MonoBehaviour
    {
        private ISupport support;

        //最大迭代次数
        private const int MAX_ITERATIONS = 32;

        private void Start()
        {
            support = GetComponent<ISupport>();
        }

        /// <summary>
        /// 碰撞函数
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool CollidesWithOther(GJKCollider other)
        {
            Vector3 newest_point;

            //任意方向，暂定vector3.right
            Vector3 direction = Vector3.right;
            Vector3 C = MinkowskiDiffSupport(other, direction);
            //如果该点和原点方向相反，直接返回false
            if (Vector3.Dot(C, direction) < 0)
            {
                return false;
            }

            //取反
            direction = -C;
            Vector3 B = MinkowskiDiffSupport(other, direction);
            //如果该点和原点方向相反，直接返回false
            if (Vector3.Dot(B, direction) < 0)
            {
                return false;
            }

            //设置下一个方向为垂直于该线  BC X BO X BC
            direction = Cross_ABA(C - B, -B);
            //单形，已经包含两个点，B 和C
            List<Vector3> simplex = new List<Vector3>
        {
            B, C
        };

            //在最大迭代次数范围内迭代
            for (int i = 0; i < MAX_ITERATIONS; i++)
            {
                newest_point = MinkowskiDiffSupport(other, direction);

                //搜索方向的最远点仍然没有接近原点，false
                if (Vector3.Dot(newest_point, direction) < 0)
                {
                    return false;
                }

                //对单形状进行操作，
                //如果单形是包含原点的四面体，则返回true
                if (DoSimplex(newest_point, ref simplex, ref direction))
                {
                    return true;
                }
            }

            //超过迭代次数，如果还没检测到，直接返回false
            return false;
        }

        #region 私有函数

        /// <summary>
        /// 如果单纯形是包含原点的四面体，则返回true，否则返回false。
        /// </summary>
        /// <param name="newest_point"></param>
        /// <param name="simplex"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        private bool DoSimplex(Vector3 newest_point, ref List<Vector3> simplex, ref Vector3 direction)
        {
            if (simplex.Count == 1)//线段
            {
                return DoSimplexLine(newest_point, ref simplex, ref direction);
            }
            else if (simplex.Count == 2)//三角形
            {
                return DoSimplexTri(newest_point, ref simplex, ref direction);
            }
            else if (simplex.Count == 3)//四面体
            {
                return DoSimplexTetra(newest_point, ref simplex, ref direction);
            }
            else
            {
                print("simplex error. Count=" + simplex.Count);
                return false;
            }
        }

        //线段
        private bool DoSimplexLine(Vector3 A, ref List<Vector3> simplex, ref Vector3 direction)
        {
            Vector3 B = simplex[0];

            Vector3 AB = B - A;
            Vector3 AO = -A;

            if (Vector3.Dot(AB, AO) > 0)
            {
                //原点在A B之间
                simplex = new List<Vector3>
            {
                A, B
            };

                //新的搜索方向为垂直于直线，同时朝向原点的方向
                direction = Cross_ABA(AB, AO);
            }
            else
            {
                // 原点在AO以上区域
                //移除B点
                simplex = new List<Vector3>
            {
                A
            };

                //新的搜索方向为AO
                direction = AO;
            }

            return false;
        }

        //三角形
        private bool DoSimplexTri(Vector3 A, ref List<Vector3> simplex, ref Vector3 direction)
        {
            //取出第一， 第二个点
            Vector3 B = simplex[0];
            Vector3 C = simplex[1];

            Vector3 AO = -A;

            Vector3 AB = B - A;
            Vector3 AC = C - A;

            //三角形的法线
            Vector3 ABC = Vector3.Cross(AB, AC);

            //垂直于三角形AB的垂线
            Vector3 ABP = Vector3.Cross(AB, ABC);
            //如果垂线和AO不再同一方向，移除C点
            if (Vector3.Dot(ABP, AO) > 0)
            {
                simplex = new List<Vector3>
            {
                A, B
            };
                //新的搜索方向为AB在AO方向上的垂线
                direction = Cross_ABA(AB, AO);

                return false;
            }

            //垂直于三角形AC的垂线
            Vector3 ACP = Vector3.Cross(ABC, AC);
            //如果垂线和AO不再同一方向，移除B点
            if (Vector3.Dot(ACP, AO) > 0)
            {
                simplex = new List<Vector3>
            {
                A, C
            };

                direction = Cross_ABA(AC, AO);

                return false;
            }
            //这里不再需要判断第三个边

            //点在三角形上的投影，在三角形的内部，单形增加到三个点
            //法线和AO在相同方向
            if (Vector3.Dot(ABC, AO) > 0)
            {
                simplex = new List<Vector3>
            {
                A, B, C
            };

                //新的搜索方向为三角形的法线方向
                direction = ABC;
            }
            else
            {
                simplex = new List<Vector3>
            {
                A, C, B
            };

                //新的搜索方向为三角形的另一个法线方向
                direction = -ABC;
            }

            return false;
        }

        //四面体
        private bool DoSimplexTetra(Vector3 A, ref List<Vector3> simplex, ref Vector3 direction)
        {
            Vector3 B = simplex[0];
            Vector3 C = simplex[1];
            Vector3 D = simplex[2];

            Vector3 AO = -A;

            Vector3 AB = B - A;
            Vector3 AC = C - A;
            Vector3 AD = D - A;

            //三个平面指向外面的法线
            Vector3 ABC = Vector3.Cross(AB, AC);
            Vector3 ACD = Vector3.Cross(AC, AD);
            Vector3 ADB = Vector3.Cross(AD, AB);

            bool over_ABC = Vector3.Dot(ABC, AO) > 0;
            bool over_ACD = Vector3.Dot(ACD, AO) > 0;
            bool over_ADB = Vector3.Dot(ADB, AO) > 0;

            Vector3 rotA = A;
            Vector3 rotB = B;
            Vector3 rotC = C;
            Vector3 rotD = D;
            Vector3 rotAB = AB;
            Vector3 rotAC = AC;
            Vector3 rotAD = AD;
            Vector3 rotABC = ABC;
            Vector3 rotACD = ACD;

            if (!over_ABC && !over_ACD && !over_ADB)
            {
                //原点在三个面内部，所以原点在闵科夫斯基差之内
                //所以碰撞返回true
                return true;
            }
            else if (over_ABC && !over_ACD && !over_ADB)
            {
                //the origin is over ABC, but not ACD or ADB

                rotA = A;
                rotB = B;
                rotC = C;

                rotAB = AB;
                rotAC = AC;

                rotABC = ABC;

                goto check_one_face;
            }
            else if (!over_ABC && over_ACD && !over_ADB)
            {
                //the origin is over ACD, but not ABC or ADB

                rotA = A;
                rotB = C;
                rotC = D;

                rotAB = AC;
                rotAC = AD;

                rotABC = ACD;

                goto check_one_face;
            }
            else if (!over_ABC && !over_ACD && over_ADB)
            {
                //the origin is over ADB, but not ABC or ACD

                rotA = A;
                rotB = D;
                rotC = B;

                rotAB = AD;
                rotAC = AB;

                rotABC = ADB;

                goto check_one_face;
            }
            else if (over_ABC && over_ACD && !over_ADB)
            {
                rotA = A;
                rotB = B;
                rotC = C;
                rotD = D;

                rotAB = AB;
                rotAC = AC;
                rotAD = AD;

                rotABC = ABC;
                rotACD = ACD;

                goto check_two_faces;
            }
            else if (!over_ABC && over_ACD && over_ADB)
            {
                rotA = A;
                rotB = C;
                rotC = D;
                rotD = B;

                rotAB = AC;
                rotAC = AD;
                rotAD = AB;

                rotABC = ACD;
                rotACD = ADB;

                goto check_two_faces;
            }
            else if (over_ABC && !over_ACD && over_ADB)
            {
                rotA = A;
                rotB = D;
                rotC = B;
                rotD = C;

                rotAB = AD;
                rotAC = AB;
                rotAD = AC;

                rotABC = ADB;
                rotACD = ABC;

                goto check_two_faces;
            }

        check_one_face:

            if (Vector3.Dot(Vector3.Cross(rotABC, rotAC), AO) > 0)
            {
                simplex = new List<Vector3>
            {
                rotA, rotC
            };

                //新搜索方向 AC x AO x AC
                direction = Cross_ABA(rotAC, AO);

                return false;
            }

        check_one_face_part_2:

            if (Vector3.Dot(Vector3.Cross(rotAB, rotABC), AO) > 0)
            {
                simplex = new List<Vector3>
            {
                rotA, rotB
            };

                //新搜索方向 AB x AO x AB
                direction = Cross_ABA(rotAB, AO);

                return false;
            }

            simplex = new List<Vector3>
        {
            rotA, rotB, rotC
        };

            direction = rotABC;

            return false;

        check_two_faces:

            if (Vector3.Dot(Vector3.Cross(rotABC, rotAC), AO) > 0)
            {
                rotB = rotC;
                rotC = rotD;

                rotAB = rotAC;
                rotAC = rotAD;

                rotABC = rotACD;

                goto check_one_face;
            }

            goto check_one_face_part_2;
        }

        /// <summary>
        /// 返回一个最极端的闵科夫斯基差，根据前形状，和传入的形状
        /// </summary>
        /// <param name="other"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        private Vector3 MinkowskiDiffSupport(GJKCollider other, Vector3 direction)
        {
            //搜索direction方向最远的点
            Vector3 my_support = support.Support(direction);
            Vector3 other_support = other.support.Support(-direction);
            //闵可夫斯基差
            Vector3 result = my_support - other_support;

            return result;
        }

        /// <summary>
        /// AXBXA  的叉积
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        private Vector3 Cross_ABA(Vector3 A, Vector3 B)
        {
            return Vector3.Cross(Vector3.Cross(A, B), A);
        }

        #endregion 私有函数
    }
}