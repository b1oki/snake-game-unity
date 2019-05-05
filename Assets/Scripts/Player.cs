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
    private float _globalHeadAngle;

    private void Start()
    {
        _transform = GetComponent<Transform>();
        _globalHeadAngle = 0;
    }

    private void Update()
    {
        RotateSnake();
        MoveSnake();
    }

    private void RotateSnake()
    {
        var rotation = Input.GetAxis("Horizontal") * 4.0f;
        _transform.Rotate(0, rotation, 0);
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