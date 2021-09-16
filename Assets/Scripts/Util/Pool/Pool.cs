using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for pooling Poolables.
/// </summary>
/// <typeparam name="T">The type that this pool manages.</typeparam>
public abstract class Pool<T> : MonoBehaviour where T : IPoolable
{
    [SerializeField] protected T prefab;
    [SerializeField] protected int startingAmount;
    [SerializeField] protected int maxCapacity;

    private Queue<T> poolQueue;

    private void Awake()
    {
        poolQueue = new Queue<T>(maxCapacity);
        for (int i = 0; i < startingAmount; i++)
        {
            T t = Create();
            t.Hide();
            poolQueue.Enqueue(t);
        }

        InnerAwake();
    }

    /// <summary>
    /// Called on awake. The pool should already be at starting amount.
    /// </summary>
    protected abstract void InnerAwake();

    /// <summary>
    /// Gets a poolable from the pool.
    /// </summary>
    public T Get()
    {
        if (poolQueue.Count == 0)
            return Create();
        return poolQueue.Dequeue();
    }

    /// <summary>
    /// Creates a new instance of a poolable.
    /// </summary>
    protected abstract T Create();

    /// <summary>
    /// Places the poolable into the pool or deletes it if too many instances were
    /// in the pool already.
    /// </summary>
    public void Free(T t)
    {
        if (poolQueue.Count == maxCapacity)
        {
            Debug.LogWarning("More bullets to free than there is capacity!");
            t.Delete();
            return;
        }
        t.Hide();
        poolQueue.Enqueue(t);
    }
}
