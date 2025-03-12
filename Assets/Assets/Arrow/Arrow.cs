using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow
{
    private GameObject _parent;
    private GameObject _tailPrefab;
    private GameObject _headPrefab;

    private GameObject _tail;
    private GameObject _head;
    private Vector3 _arrowheadDirection;
    private float _currentTailLength;
    private Vector3 _lastParentPosition;
    private Quaternion lastParentRotation;
    private Vector3 _lastParentScale;
    private float _lastParentArea;

    public Arrow(GameObject parent, GameObject tailPrefab, GameObject headPrefab, Vector3 initialDirection, float lengthOfTail)
    {
        this._parent = parent;
        this._tailPrefab = tailPrefab;
        this._headPrefab = headPrefab;

        // Instantiate the tail and head prefabs
        this._tail = GameObject.Instantiate(_tailPrefab, _parent.transform.position, Quaternion.identity, _parent.transform);
        this._head = GameObject.Instantiate(_headPrefab, CalculateHeadOffsetPosition(), Quaternion.identity, _parent.transform);

        this._currentTailLength = lengthOfTail;
        this._arrowheadDirection = initialDirection;
        this._lastParentPosition = this._parent.transform.position;
        this.lastParentRotation = this._parent.transform.rotation;
        this._lastParentScale = this._parent.transform.localScale;
        this._lastParentArea = 0;

        Debug.Log("Arrow constructor called.");
    }

    public void SetParent(GameObject parent)
    {
        this._parent = parent;
    }

    public void SetInitialDirection(Vector3 direction)
    {
        this._arrowheadDirection = direction;
    }

    public void SetTailLength(float length)
    {
        if (length >= 0)
        {
            this._currentTailLength = length;
        }
    }

    public void UpdateParentTransform()
    {
        this._lastParentPosition = this._parent.transform.position;
        this.lastParentRotation = this._parent.transform.rotation;
        this._lastParentScale = this._parent.transform.localScale;
    }

    public void SetParentArea(float area)
    {
        this._lastParentArea = area;
    }

    public bool IsParentTransformChanged()
    {
        if (this._parent != null)
        {
            return
                this._lastParentPosition != this._parent.transform.position ||
                this.lastParentRotation != this._parent.transform.rotation ||
                this._lastParentScale != this._parent.transform.localScale;
        }
        return false;
    }

    public void SetScene()
    {
        UpdateTail();
        UpdateHead();
        EnsureHeadIsEnabled();
    }

    public void Update()
    {
        UpdateHead();
        UpdateTail();
        EnsureHeadIsEnabled();
    }

    private void UpdateTail()
    {
        this._tail.transform.position = CalculateTailOffsetPosition();
        this._tail.transform.rotation = this._parent.transform.rotation;

        float x = this._tail.transform.localScale.x;
        float z = this._tail.transform.localScale.z;
        this._tail.transform.localScale = new Vector3(x, this._currentTailLength, z);
    }

    public static float CalculateLengthByValue(float value)
    {
        if (value > 0)
        {
            float maxLength = 10f;
            float length = (maxLength / 100) * value;
            return length;
        }
        return 0;
    }

    private Vector3 CalculateTailOffsetPosition()
    {
        float halfYScale = this._tail.transform.localScale.y / 2;
        Vector3 direction = this._tail.transform.rotation * Vector3.up;
        Vector3 offset = direction * halfYScale;
        Vector3 tailPosition = this._parent.transform.position + offset;
        return tailPosition;
    }

    public Vector3 CalculateHeadOffsetPosition()
    {
        float halfYScale = this._tail.transform.localScale.y / 2;
        Vector3 direction = this._tail.transform.rotation * Vector3.up;
        Vector3 tailCenterPosition = this._tail.transform.position;
        Vector3 offset = direction * halfYScale;
        Vector3 headPosition = this._tail.transform.position + offset;
        return headPosition;
    }

    private void UpdateHead()
    {
        EnsureHeadIsEnabled();
        this._head.transform.position = CalculateHeadOffsetPosition();
        if (this._tail.transform.localScale.y < 0)
        {
            this._head.transform.rotation = Quaternion.AngleAxis(180, Vector3.up) * this._tail.transform.rotation;
        }
        else
        {
            this._head.transform.rotation = this._tail.transform.rotation;
        }
    }

    private void EnsureHeadIsEnabled()
    {
        if (this._tail.transform.localScale.y == 0)
        {
            this._head.gameObject.SetActive(false);
        }
        else
        {
            this._head.gameObject.SetActive(true);
        }
    }
}
