using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MyTest : MonoBehaviour {

    public RectTransform prefab;

    int row = 10;       // 行
    int column = 20;        // 列

    int startXPos = 60;
    int startZPos = -100;

    float distanceRandomMinX = 55;
    float distanceRandomMaxX = 65;

    float distanceRandomMinY = 60;
    float distanceRandomMaxY = 60;

    float initMoveDistance = 1200;

    float enlargeSize = 5;

    float radiateSize = 220;

    List<List<RectTransform>> goList;
    Dictionary<RectTransform, Vector2> itemPosDict;
    List<RectTransform> changedItemList;

    // Use this for initialization
    void Start () {

        goList = new List<List<RectTransform>>();
        itemPosDict = new Dictionary<RectTransform, Vector2>();
        changedItemList = new List<RectTransform>();

        CreateGos();
    }


    void CreateGos()
    {
        // 生成所有物体，并添加到字典
        for (int i = 0; i < row; i++)
        {
            List<RectTransform> gos = new List<RectTransform>();
            goList.Add(gos);
            float lastPosX = 0;
            for (int j = 0; j < column; j++)
            {
                RectTransform item = (Instantiate(prefab.gameObject) as GameObject).GetComponent<RectTransform>();
                item.name = i + " " + j;
                item.transform.SetParent(transform);
                Vector2 startPos = new Vector3(Random.Range(distanceRandomMinX, distanceRandomMaxX) + lastPosX, startZPos - i * Random.Range(distanceRandomMinY, distanceRandomMaxY));
                item.anchoredPosition = startPos;
                Vector2 endPos = new Vector3(startPos.x - initMoveDistance, startZPos - i * Random.Range(distanceRandomMinY, distanceRandomMaxY));
                Tweener tweener = item.DOAnchorPosX(endPos.x, Random.Range(1.8f, 2f));  // 缓动到目标位置
                tweener.SetDelay(j * 0.1f + (row - i) * 0.1f);      // 延时
                tweener.SetEase(Ease.InSine);           // 缓动效果
                item.gameObject.SetActive(true);
                gos.Add(item);
                itemPosDict.Add(item, endPos);

                lastPosX = item.anchoredPosition.x;
            }
        }
    }


    public void OnMousePointEnter(RectTransform item)
    {
        // 缓动改变中心物体尺寸
        item.DOScale(enlargeSize, 0.5f);

        Vector2 pos = itemPosDict[item];

        changedItemList = new List<RectTransform>();

        // 添加扩散物体到集合
        foreach (KeyValuePair<RectTransform, Vector2> i in itemPosDict)
        {
            if(Vector2.Distance(i.Value, pos) < radiateSize)
            {
                changedItemList.Add(i.Key);
            }
        }

        // 缓动来解决扩散物体的动画
        for (int i = 0; i < changedItemList.Count; i++)
        {
            Vector2 targetPos = itemPosDict[item] + (itemPosDict[changedItemList[i]] - itemPosDict[item]).normalized * radiateSize;
            changedItemList[i].DOAnchorPos(targetPos, 0.8f);
        }
    }

    public void OnMousePointExit(RectTransform go)
    {
        // 缓动恢复中心物体尺寸
        go.DOScale(1, 1);
        // 缓动将扩散物体恢复到初始位置
        for (int i = 0; i < changedItemList.Count; i++)
        {
            changedItemList[i].DOAnchorPos(itemPosDict[changedItemList[i]], 0.8f);
        }
    }
}
