using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhysicalSystem
{
    public interface ISupport
    {
        //搜索direction方向最远的点
        Vector3 Support(Vector3 direction);
    }
}