using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigStorage : Storage
{
    int _size = 16;
    private void Start()
    {
        _storageSize = _size;
        _items = new Item[_size];
    }
}
