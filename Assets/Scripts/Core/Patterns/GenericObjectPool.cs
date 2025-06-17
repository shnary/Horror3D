using System;
using System.Collections.Generic;
using UnityEngine;

public class GenericObjectPool<T> {
    private const int MaxObjectAmount = 150;

    private readonly List<T> _busyObjects = new List<T>();
    private readonly List<T> _freeObjects = new List<T>();

    public int StartSize { get; }
    public int SizeFreeObjects => _freeObjects.Count;
    public int SizeBusyObjects => _busyObjects.Count;

    public List<T> FreeObjects => _freeObjects;
    public List<T> BusyObjects => _busyObjects;

    public Type PoolType => typeof(T);

    private readonly Func<T> _onCreate;
    private readonly Action<T> _onRelease;

    /// <summary>
    /// Constructor, you must specify the starting size of the pool
    /// </summary>
    public GenericObjectPool(int startingSize, Func<T> onCreate, Action<T> onRelease=null) {
        StartSize = startingSize;
        _onCreate = onCreate;
        _onRelease = onRelease;
        for (int i = 0; i < startingSize; i++) {
            var obj = onCreate();
            _freeObjects.Add(obj);
        }
    }

    /// <summary>
    /// Constructor, you must specify the already created objects.
    /// </summary>
    public GenericObjectPool(int startingSize, T[] objArray, Func<T> onCreate, Action<T> onRelease=null) {
        StartSize = startingSize;
        _onCreate = onCreate;
        _onRelease = onRelease;
        for (int i = 0; i < startingSize; i++) {
            _freeObjects.Add(objArray[i]);
        }
    }

    public T Get() {
        T pooled = default(T);

        if (SizeFreeObjects > 0) {
            // pool the last object.
            pooled = _freeObjects[SizeFreeObjects - 1];
            _freeObjects.Remove(pooled);
        }
        else {
            if (SizeBusyObjects >= MaxObjectAmount) {
                return _busyObjects[SizeBusyObjects - 1];
            }
            // If can't pool then create a new object.
            pooled = _onCreate();
        }
    
        // Add it to the busy list.
        _busyObjects.Add(pooled);
        
        return pooled;
    }

    public void Release(T released) {
        if (released == null) return;
    
        // Reset object.
        _onRelease?.Invoke(released);
    
        // Add back to the freelist.
        _freeObjects.Add(released);
    
        // Remove from the busy list.
        _busyObjects.Remove(released);
    }
}