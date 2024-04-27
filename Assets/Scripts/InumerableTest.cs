using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DefaultNamespace
{
    public class InumerableTest:MonoBehaviour
    {
        private void Awake()
        {
            var numbers =  new[] { 1, 2, 3, 4, 5 };
            var maxNumber = 3;
            IEnumerable<int> ienumerable1 = numbers.Where(x => x < maxNumber);//1 2
            maxNumber = 6;
            IEnumerable<int> ienumerable2 = ienumerable1.Where(x => x < maxNumber); // 1 2
            
            Debug.Log(ienumerable1.Count()); //2
            Debug.Log(ienumerable2.Count());//2
        }
    }
}