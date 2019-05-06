using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public UnityEvent onEat;
    public UnityEvent onDie;
    public List<Transform> tails;
    [Range(0.5f, 1)] public float bonesDistance;
    public GameObject bonePrefab;
    [Range(0, 0.5f)] public float speed;

    private Transform _transform;
    private readonly string[] _dangerObjects = {"Wall", "Tail"};

    private void Start()
    {
        _transform = GetComponent<Transform>();
    }

    private void Update()
    {
        RotateSnake();
        MoveSnake();
    }

    private void RotateSnake()
    {
        float currentAngle = _transform.eulerAngles.y;
        var rotation = currentAngle;
        var vertical = Input.GetAxis("Vertical");
        var horizontal = Input.GetAxis("Horizontal");
        if (horizontal > 0) {
            rotation = 90;
        } else if (horizontal < 0) {
            rotation = 270;
        } else if (vertical > 0) {
            rotation = 0;
        } else if (vertical < 0) {
            rotation = 180;
        }
        if (rotation != currentAngle) {
            _transform.rotation = Quaternion.AngleAxis(rotation, Vector3.up);
        }
    }

    private void MoveSnake()
    {
        var newPosition = _transform.position + _transform.forward * speed;
        var sqrDistance = bonesDistance * bonesDistance;
        var previousBonePosition = _transform.position;
        foreach (var bone in tails)
        {
            if ((bone.position - previousBonePosition).sqrMagnitude < sqrDistance)
            {
                break;
            }

            var prePreviousBonePosition = bone.position;
            bone.position = previousBonePosition;
            previousBonePosition = prePreviousBonePosition;
        }

        _transform.position = newPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            onEat?.Invoke();
            Destroy(other.gameObject);
            var bone = Instantiate(bonePrefab);
            tails.Add(bone.transform);
        }
        else if (_dangerObjects.Contains(other.gameObject.tag))
        {
            onDie?.Invoke();
            SceneManager.LoadScene("MainScene");
        }
    }
}