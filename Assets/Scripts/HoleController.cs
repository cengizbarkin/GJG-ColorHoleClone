using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HoleController : MonoBehaviour
{
    [SerializeField] private GameObject startPositionOfHole = default;
    [SerializeField] private GameObject animationTargetPoint = default;
    [SerializeField] private Vector2 moveLimit = default;
    [SerializeField] private float moveSpeed = default;
    [SerializeField] private float animationSpeed = default;
    [SerializeField] private float holeRadius = default;
    [SerializeField] private float magnetForce = default;
    
    private MeshFilter _meshFilter;
    private MeshCollider _meshCollider;
    private Transform _holeTransform;
    private Mesh _mesh;

    private List<int> _holeVertices;
    private List<Vector3> _offSets;
    
    private float _x, _y;
    private Vector3 _touch, _targetPosition;

    private bool _selfControl, _animateHole, _clearBridge;

    private List<Rigidbody> _affectedRigidbodies;
    
    // Start is called before the first frame update
    private void Start()
    {
        GameManager.Instance.OnGameStateStarted += GameStateStarted;
        GameManager.Instance.OnGameStateCompleted += GameStateCompleted;
        GameManager.Instance.OnGameOver += GameOver;

        _meshFilter = GetComponentInParent<MeshFilter>();
        _meshCollider = GetComponentInParent<MeshCollider>();
        _holeTransform = transform;
        _mesh = _meshFilter.mesh;
        
        _holeVertices = new List<int>();
        _offSets = new List<Vector3>();
        _affectedRigidbodies = new List<Rigidbody>();
        SelectHoleVertices();
        GoStartPositionOfHole();
        
        
        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            moveSpeed /= 100.0f;
        }
    }


    private void GoStartPositionOfHole()
    {
        _holeTransform.localPosition = startPositionOfHole.transform.localPosition;
        ManipulateHole("follow");
        ManipulateHole("close");
        _selfControl = false;
        _animateHole = false;
        _affectedRigidbodies.Clear();
        
    }
    
    private void GameStateStarted(string state)
    {
        var parentName = transform.parent.name;
        if (parentName == state)
        {
            switch (state)
            {
                case "Ground1":
                    _selfControl = true;
                    _clearBridge = false;
                    break;
                case "Bridge":
                    _clearBridge = true;
                    _selfControl = false;
                    break;
                case "Ground2":
                    _selfControl = true;
                    _clearBridge = false;
                    break;
            }
            ManipulateHole("follow");
        }
        else
        {
            GoStartPositionOfHole();
        }
    }
    
    private void GameStateCompleted(string state)
    {
        var parentName = transform.parent.name;
        if (parentName == state)
        {
            switch (state)
            {
                case "Ground1":
                    _selfControl = false;
                    _animateHole = true;
                    break;
                case "Bridge":
                    _selfControl = false;
                    _animateHole = false;
                    break;
                case "Ground2":
                    _animateHole = false;
                    ManipulateHole("close");
                    break;
            }
        }
    }
    private void SelectHoleVertices()
    {
        for (var i = 0; i < _mesh.vertices.Length; i++)
        {
            var distanceOfVertex = Vector3.Distance(_holeTransform.localPosition, _mesh.vertices[i]);
            if (!(distanceOfVertex < holeRadius)) continue;
            _holeVertices.Add(i);
            _offSets.Add(_mesh.vertices[i] - _holeTransform.position);
        }
    }
    
    private void MoveHoleGameObject()
    {
        _x = Input.GetAxis("Mouse X");
        _y = Input.GetAxis("Mouse Y");

        var position = _holeTransform.localPosition;
        _touch = Vector3.Lerp(position, position + new Vector3(_x, 0.0f, _y),
            moveSpeed * Time.deltaTime);

        _targetPosition = new Vector3(
            Mathf.Clamp(_touch.x, -moveLimit.x, moveLimit.x), 
            _touch.y, 
            Mathf.Clamp(_touch.z, -moveLimit.y, moveLimit.y));
        _holeTransform.localPosition = _targetPosition;
    }
    
    private void ManipulateHole(string type)
    {
        Vector3[] vertices = _mesh.vertices;
        switch (type)
        {
            case "close":
                for (var i = 0; i < _holeVertices.Count; i++)
                {
                    vertices[_holeVertices[i]] = new Vector3(0.0f, 0.08f, 0.0f);
                }
                break;
            case "follow":
                for (var i = 0; i < _holeVertices.Count; i++)
                {
                    vertices[_holeVertices[i]] = _holeTransform.position + _offSets[i];
                }
                break;
        }
        _mesh.vertices = vertices;
        _meshFilter.mesh = _mesh;
        _meshCollider.sharedMesh = _mesh;
    }
    
    
    // Update is called once per frame
    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButton(0) && _selfControl)
        {
            MoveHoleGameObject();
            ManipulateHole("follow");
            
        }
#else
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved && _selfControl)
        {
            MoveHoleGameObject();
            ManipulateHole("follow");
        }
#endif

    }
    
    
    private void FixedUpdate()
    {
        if (_clearBridge)
        {
            ManipulateHole("follow");
            float step = animationSpeed * Time.deltaTime;
            var currentPos = transform.position;
            currentPos = Vector3.MoveTowards(currentPos, animationTargetPoint.transform.position, step);

            if (currentPos.z >= animationTargetPoint.transform.position.z)
            {
                _clearBridge = false;
                ManipulateHole("close");
            }
            transform.position = currentPos;
        }
        if (_animateHole)
        {
            ManipulateHole("follow");
            _selfControl = false;
            float step = animationSpeed * Time.deltaTime;
            var currentPos = transform.position;
            currentPos = Vector3.MoveTowards(currentPos, new Vector3(0.0f, currentPos.y, animationTargetPoint.transform.position.z), step);
            if (currentPos.z > animationTargetPoint.transform.position.z)
            {
                currentPos.z = animationTargetPoint.transform.position.z;
                if (currentPos.x == 0.0f)
                {
                    _animateHole = false;
                    ManipulateHole("close");
                }
            }
            transform.position = currentPos;
        }

        foreach (var rb in _affectedRigidbodies)
        {
            rb.AddForce((transform.position - rb.transform.position) * (magnetForce * Time.fixedDeltaTime));
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        _affectedRigidbodies.Add(other.attachedRigidbody);
    }

    private void OnTriggerExit(Collider other)
    {
        _affectedRigidbodies.Remove(other.attachedRigidbody);
    }


    private void GameOver()
    {
        _selfControl = false;
    }
    
    private void OnDisable()
    {
        if (GameManager.Instance)
        {
            GameManager.Instance.OnGameStateStarted -= GameStateStarted;
            GameManager.Instance.OnGameStateCompleted -= GameStateCompleted;
        }
    }
}
