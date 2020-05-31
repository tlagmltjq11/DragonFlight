using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util : MonoBehaviour
{
    public static EventDelegate.Parameter MakeParameter(UnityEngine.Object _value, System.Type _type)
    {
        EventDelegate.Parameter param = new EventDelegate.Parameter();
        // 이벤트 parameter 생성.     
        param.obj = _value;
        // 이벤트 함수에 전달하고 싶은 값.     
        param.expectedType = _type;
        // 값의 타입.       
        return param;
    }

    public static GameObject FindChildObject(GameObject gObj, string name)
    {
        //파라메터로 true로 주는것은 액티브가 꺼져있는 자식들도 찾겠다는것임.
        var childObjects = gObj.GetComponentsInChildren<Transform>(true);
        for(int i=0; i<childObjects.Length; i++)
        {
            if(childObjects[i].name.Equals(name))
            {
                return childObjects[i].gameObject;
            }
        }

        return null;
    }

    //어떤 아이템인지 해당 아이템의 index를 반환함.
    public static int GetPriorities(int[] table)
    {
        int sum = 0;
        int num = 0;
        if(table == null || table.Length <= 0)
        {
            return -1;
        }

        for(int i=0; i<table.Length; i++)
        {
            sum += table[i];
        }

        num = UnityEngine.Random.Range(1, sum + 1);

        sum = 0;
        for(int i=0; i<table.Length; i++)
        {
            if(num > sum && num <= sum + table[i])
            {
                return i;
            }

            sum += table[i];
        }

        return -1;

    }
}
