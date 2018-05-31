﻿using UnityEngine;
using System.Collections;

public class DontGoThroughThings : MonoBehaviour
{
    public LayerMask LayerMask = -1; //make sure we aren't in this layer 
    public float SkinWidth = 0.1f; //probably doesn't need to be changed 

    private float _minimumExtent;
    private float _partialExtent;
    private float _sqrMinimumExtent;
    private Vector3 _previousPosition;
    private Rigidbody _myRigidbody;
    private Collider _myCollider;

    //initialize values 
    void Start()
    {
        _myRigidbody = GetComponent<Rigidbody>();
        _myCollider = GetComponent<Collider>();
        _previousPosition = _myRigidbody.position;
        _minimumExtent = Mathf.Min(Mathf.Min(_myCollider.bounds.extents.x, _myCollider.bounds.extents.y),
            _myCollider.bounds.extents.z);
        _partialExtent = _minimumExtent * (1.0f - SkinWidth);
        _sqrMinimumExtent = _minimumExtent * _minimumExtent;
    }

    void FixedUpdate()
    {
        //have we moved more than our minimum extent? 
        var movementThisStep = _myRigidbody.position - _previousPosition;
        var movementSqrMagnitude = movementThisStep.sqrMagnitude;

        if (movementSqrMagnitude > _sqrMinimumExtent)
        {
            var movementMagnitude = Mathf.Sqrt(movementSqrMagnitude);
            RaycastHit hitInfo;

            //check for obstructions we might have missed 
            if (Physics.Raycast(_previousPosition, movementThisStep, out hitInfo, movementMagnitude, LayerMask.value))
            {
                if (!hitInfo.collider)
                    return;

                if (hitInfo.collider.isTrigger)
                    hitInfo.collider.SendMessage("OnTriggerEnter", _myCollider);

                if (!hitInfo.collider.isTrigger)
                    _myRigidbody.position = hitInfo.point - (movementThisStep / movementMagnitude) * _partialExtent;
            }
        }

        _previousPosition = _myRigidbody.position;
    }
}