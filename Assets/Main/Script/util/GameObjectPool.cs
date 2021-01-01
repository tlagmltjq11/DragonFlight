using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//T에는 class인 애들만 들어올 수 있게함. -> 게임오브젝트니까
//또한 start나 update를 쓸일이 없어서 모노비헤이비어를 상속안시킴
public class GameObjectPool<T>  where T : class
{
    #region Field
    //몇개를 만들것인가
    int m_count;

    //무엇을 만들것인가, 해당 T를 생성해서 반환
    public delegate T Func();
    Func CreateFunc;
    Queue<T> m_objectPool;

    public int Count { get { return m_objectPool.Count; } }
    #endregion

    #region Public Methods
    public GameObjectPool(int count, Func createFunc)
    {
        m_count = count;
        CreateFunc = createFunc;

        //count만큼 미리 자리를 잡아놓음
        m_objectPool = new Queue<T>(count);
        //갯수만큼 생성해서 큐에 넣음.
        Allocate();
    }

    //메모리를 생성
    public void Allocate()
    {
        //갯수만큼 생성해서 큐에 집어넣음
        for(int i=0; i<m_count; i++)
        {
            m_objectPool.Enqueue(CreateFunc());
        }
    }

    public T Peek()
    {
        return m_objectPool.Peek();
    }

    public T Get()
    {
        if(m_objectPool.Count > 0)
        {
            return m_objectPool.Dequeue();
        }
        //만약 큐에있는 모든 객체를 다 써버려서 비어있다면, 다시 메모리를 잡아서 생성해주어야 할 것이다.★★★★★★★★
        else
        {
            //Allocate();
            //return m_objectPool.Dequeue();
            //위 방식대로 했을때, 만약 m_count가 100이었다면 런타임 도중에 100개를 한번에 인스턴스하므로 렉이 걸릴 수 있다. 혹은 1개가 모자랐는데 100개를 만들어버리는 비효율이 존재함.
            //고로 필요할때마다 한개씩 추가하는게 편하다.
            m_objectPool.Enqueue(CreateFunc());
            return m_objectPool.Dequeue();
        }
    }

    public void Set(T item)
    {
        m_objectPool.Enqueue(item);
    }
    #endregion
}
