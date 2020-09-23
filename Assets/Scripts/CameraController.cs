using UnityEngine;

public class CameraController : MonoBehaviour
{
    
    private Vector3 _initialPosition;
    [SerializeField] private GameObject targetGameObject = default;
    [SerializeField] private float animationSpeed = default;
    [SerializeField] private float reverseAnimationSpeed = default;
    
    
    private bool _moveTarget;
    private bool _moveBack;
    
    // Start is called before the first frame update
    private void Start()
    {
        _initialPosition = gameObject.transform.position;
        GameManager.Instance.OnGameStateStarted += GameStateStarted;
        GameManager.Instance.OnGameStateCompleted += GameStateCompleted;
    }
    private void GameStateStarted(string state)
    {
        switch (state)
        {
            case "Ground1":
                break;
            case "Bridge":
                _moveTarget = true;
                break;
            case "Ground2":
                break;
        }
    }


    private void GameStateCompleted(string state)
    {
        switch (state)
        {
            case "Ground1":
                break;
            case "Bridge":
                break;
            case "Ground2":
                _moveBack = true;
                break;
        }
    }
    
    
    private void Update()
    {
        if (_moveTarget)
        {
            float step = animationSpeed * Time.deltaTime;
            var currentPos = transform.position;
            currentPos = Vector3.MoveTowards(currentPos, targetGameObject.transform.localPosition, step);
            
            if (currentPos.z >= targetGameObject.transform.localPosition.z)
            {
                _moveTarget = false;
                GameManager.Instance.GameStateCompleted("Bridge");
                GameManager.Instance.GameStateStarted("Ground2");
            }
            transform.position = currentPos;
        }

        if (_moveBack)
        {
            float step = reverseAnimationSpeed * Time.deltaTime;
            var currentPos = transform.position;
            currentPos = Vector3.MoveTowards(currentPos, _initialPosition, step);
            if (currentPos.z <= _initialPosition.z)
            {
                _moveBack = false;
            }
            transform.position = currentPos;
        }
    }
}
