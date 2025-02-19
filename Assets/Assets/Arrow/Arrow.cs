using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class Arrow
{

    /*private GameObject parent;*/
    private GameObject _parent;
    private GameObject _tail;
    private GameObject _head;
    private Vector3 _arrowheadDirection;
    private float _currentTailLength;
    private Vector3 _lastParentPosition;
    private Quaternion lastParentRotation;
    private Vector3 _lastParentScale;
    private float _lastParentArea;

    public Arrow(GameObject parent, GameObject head, GameObject tail, Vector3 initialDirection, float lengthOfTail)
    {
        this._parent = parent;
        this._head = head;
        this._tail = tail;
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
            //Debug.Log("Tail Length: " + this._currentTailLength);

        }

    }


    public void UpdateParentTransform()
    {
        this._lastParentPosition = this._parent.transform.position;
        this.lastParentRotation = this._parent.transform.rotation;
        this._lastParentScale = this._parent.transform.localScale;

        //Debug.Log(this._lastParentPosition + "," + this.lastParentRotation);
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
        //Debug.Log(IsParentTransformChanged());
        {
            this._tail.transform.position = CalculateTailOffsetPosition();
            this._tail.transform.rotation = this._parent.transform.rotation;

            float x = this._tail.transform.localScale.x;
            float z = this._tail.transform.localScale.z;
            this._tail.transform.localScale = new Vector3(x, this._currentTailLength, z);

            //Debug.Log("UpdateHead() done");
        }
    }


    public static float CalculateLengthByValue(float value)
    {
        if (value > 0)
        {//max area is 100 unit-squared and min is 1 unit-squared
            float maxLength = 10f;
            float length = (maxLength / 100) * value;

            return length;
        }
        return 0;
    }

    private Vector3 CalculateTailOffsetPosition()
    {
        // Calculate half the Y scale
        float halfYScale = this._tail.transform.localScale.y / 2;

        // Get the direction vector from the tail's rotation
        Vector3 direction = this._tail.transform.rotation * Vector3.up;

        // Scale the direction vector by half the Y scale
        Vector3 offset = direction * halfYScale;

        // Calculate the new tail position
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

        // Get the bounds of the tail mesh
        Bounds tailBounds = this._tail.GetComponent<MeshFilter>().mesh.bounds;
        Vector3 tailEndWorldPosition = GetTailEndWorldPosition(tailBounds);

        // Get the bounds of the parent object
        Bounds parentBounds = this._parent.GetComponent<MeshFilter>().mesh.bounds;
        Vector3 parentTopWorldPosition = GetParentTopWorldPosition(parentBounds);

        // Calculate the offset to align the arrow's bottom with the parent's top
        Vector3 arrowBottomWorldPosition = GetArrowBottomWorldPosition(tailBounds);
        Vector3 offset = parentTopWorldPosition - arrowBottomWorldPosition;

        // Position the head mesh with the calculated offset
        /*this.head.transform.position = tailEndWorldPosition + offset;*/
        this._head.transform.position = CalculateHeadOffsetPosition();
        if (this._tail.transform.localScale.y < 0)
        {
            this._head.transform.rotation = Quaternion.AngleAxis(180, Vector3.up) * this._tail.transform.rotation;
        }
        else
        {
            this._head.transform.rotation = this._tail.transform.rotation;
        }


        //Debug.Log("UpdateHead() done");

    }



    private void EnsureHeadIsEnabled()
    {
        // Check if the local scale of the tail in the Y direction is zero
        if (this._tail.transform.localScale.y == 0)
        {
            this._head.gameObject.SetActive(false);
        }
        else if (this._tail.transform.localScale.y != 0 /*&& this._head.gameObject.active == false*/)
        {
            this._head.gameObject.SetActive(true);
        }
    }


    private Vector3 GetTailEndWorldPosition(Bounds tailBounds)
    {
        Vector3 tailEndLocalPosition = tailBounds.center + new Vector3(0, 0, tailBounds.extents.z);
        return this._tail.transform.TransformPoint(tailEndLocalPosition);
    }
     
    private Vector3 GetParentTopWorldPosition(Bounds parentBounds)
    {
        Vector3 parentTopLocalPosition = parentBounds.center + new Vector3(0, parentBounds.extents.y, 0);
        return this._parent.transform.TransformPoint(parentTopLocalPosition);
    }

    private Vector3 GetArrowBottomWorldPosition(Bounds tailBounds)
    {
        Vector3 arrowBottomLocalPosition = tailBounds.center - new Vector3(0, tailBounds.extents.y, 0);
        return this._tail.transform.TransformPoint(arrowBottomLocalPosition);
    }
}
