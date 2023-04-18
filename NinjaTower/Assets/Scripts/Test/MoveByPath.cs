using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using PathCreation;
using UnityEngine;
using Object = UnityEngine.Object;

public class MoveByPath : MonoBehaviour
{
    public PathCreator[] paths;

    public GameObject prefab;
    public void Awake()
    {
        foreach (var path in paths)
        {
            var go = Object.Instantiate(prefab);
            go.SetActive(true);
            var points = path.path.localPoints.Reverse().ToArray();
            go.transform.localPosition = points[0];
            var tweener = go.transform.DOLocalPath(points, 1f);
            tweener.SetEase(Ease.OutCubic);
            tweener.SetLoops(-1, LoopType.Restart);
        }
    }
}
