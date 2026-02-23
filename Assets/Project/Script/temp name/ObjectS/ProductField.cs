using System;
using UnityEngine;

public class ProductField : MonoBehaviour
{
    public event Action<GameObject> OnProductCreated;
    public event Action<GameObject> OnProductRemoved;
    public event Action<int, int>   OnCountChanged;
    public event Action<float>   OnIntervalChanged;
    public event Action<float>   OnNextTimeChanged;

    System.Collections.Generic.List<GameObject> products = new();

    Collider2D productArea = null;
    Rect productRect;
    [SerializeField] RectOffset padding;
    [SerializeField] GameObject productPrefab = null;
    [SerializeField] float productStart = 0.5f;
    [SerializeField] float productInterval = 5.0f;
                     float productNextTime = 0.0f;
    [SerializeField] int productCountMax = 5;
                     int productCountCurrent = 0;
    [SerializeField] bool isProducting = true;

    void Start()
    {
        OnProductCreated += OnProductCreate;
        OnProductRemoved += OnProductRemove;
        productNextTime = Time.time + productStart;
        productArea = GetComponent<Collider2D>();

        if(productArea)
        {
            //경계의 Size, Extents => Size는 Extents의 2배!
            //Extents는 반지름 : 원점에서 특정 방향으로 이동을 하면 그 지점의 끝을 알 수 있음!
            Vector3 center = productArea.bounds.center;
            Vector3 extents = productArea.bounds.extents;
            Vector3 size = productArea.bounds.size;

            //왼쪽 아래 : 중앙점에서 반지름만큼 뺀 거!
            Vector3 leftDown = center - extents;

            //오른쪽 위 : 중앙점에서 반지름만큼 더한 거!
            //Vector3 RightUp = center + extents;
            //오른쪽 위 : 왼쪽아래에서 지름만큼 더한 거!
            Vector3 rightUp = leftDown + size;

            leftDown.x += padding.left * 0.01f;
            leftDown.y += padding.bottom * 0.01f;
            Vector3 totalSize = size;
            totalSize.x -= (padding.left + padding.right) * 0.01f; 
            totalSize.y -= (padding.top + padding.bottom) * 0.01f; 
            productRect.Set(leftDown.x, leftDown.y, totalSize.x, totalSize.y);
        }
        else
        {
            //만들 공간을 따로 정의하지 않았다면 그냥 본인 위치에서 계속 생성!
            productRect.Set(transform.position.x, transform.position.y, 0,0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        TryProduct();
    }

    //Ctrl + Shift + M
    private void OnMouseEnter()
    {
        FieldManager.SetFieldFocus(this);
    }

    private void OnMouseExit()
    {
        FieldManager.SetFieldFocus(null);
    }

    //                                    조건식은 알아서 가져와라
    public virtual GameObject GetProduct(Predicate<GameObject> pred)
    {
        return products.Find(pred);
    }

    public virtual bool GetProductable() 
        => isProducting 
        && productCountCurrent < productCountMax 
        && productNextTime < Time.time;

    public virtual Vector3 GetProductLocation()
    {
        Vector3 result = Vector3.zero;
        result.x = UnityEngine.Random.Range(productRect.xMin, productRect.xMax);
        result.y = UnityEngine.Random.Range(productRect.yMin, productRect.yMax);
        return result;
    }

    public virtual float GetProductNextTime() => productNextTime;
    public virtual float GetProductInterval() => productInterval;
    public virtual int GetCountCurrent() => productCountCurrent;
    public virtual int GetCountMax() => productCountMax;

    protected virtual void SetNextProductTime(float offset)
    {
        productNextTime = offset + productInterval;
        OnNextTimeChanged?.Invoke(productNextTime);
    }

    void TryProduct()
    {
        if(GetProductable()) Product();
    }
    protected virtual void Product()
    {
        GameObject inst = Instantiate(productPrefab, GetProductLocation(), Quaternion.identity);
        if(inst)
        {
            OnProductCreated.Invoke(inst);
        }
        SetNextProductTime(Time.time);
    }

    protected virtual void ReceiveProductDestroy(GameObject from)
    {
        if(products.Find((target) => target == from))
        {
            OnProductRemove(from);
        }
    }

    protected virtual void OnProductCreate(GameObject newProduct)
    {
        products.Add(newProduct);
        if(newProduct.TryGetComponent(out Product asProduct))
        {
            asProduct.OnDestroyed += ReceiveProductDestroy;
        }
        productCountCurrent++;
        OnCountChanged?.Invoke(productCountCurrent, productCountMax);
    }

    protected virtual void OnProductRemove(GameObject removedProduct)
    {
        products.Remove(removedProduct);
        productCountCurrent--;
        OnCountChanged?.Invoke(productCountCurrent, productCountMax);
    }
}
