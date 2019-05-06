using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public UnityEvent onEat;
    public UnityEvent onDie;
    public Text livesText;
    public List<Transform> tails;
    [Range(0.5f, 1.0f)] public float bonesDistance;
    public GameObject bonePrefab;
    public GameObject foodPrefab;
    [Range(0.0f, 0.2f)] public float speed;

    private Transform _transform;
    private Vector3 _defaultTailPosition;
    private int _livesLeft = 3;
    private readonly string[] _dangerObjects = { "Wall", "Tail" };
    private readonly float _worldLimit = 5.0f;

    private void Start()
    {
        _transform = GetComponent<Transform>();
        _defaultTailPosition = new Vector3(10.0f, _transform.position.y, 0.0f);
        SetLivesText();
        AddFood();
    }

    private void Update()
    {
        RotateSnake();
        MoveSnake();
    }

    private void RotateSnake()
    {
        var rotation = Input.GetAxis("Horizontal") * 6.0f;
        _transform.Rotate(0, rotation, 0);
    }

    private void MoveSnake()
    {
        var newPosition = _transform.position + (_transform.forward * speed);
        MoveTails();
        _transform.position = newPosition;
    }

    private void MoveTails()
    {
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
    }

    private void AddTail()
    {
        var bone = Instantiate(bonePrefab, _defaultTailPosition, bonePrefab.transform.rotation);
        tails.Add(bone.transform);
    }

    private void AddFood()
    {
        var foodPosition = new Vector3(Random.Range(-_worldLimit, _worldLimit), foodPrefab.transform.position.y, Random.Range(-_worldLimit, _worldLimit));
        Instantiate(foodPrefab, foodPosition, foodPrefab.transform.rotation);
    }

    private void NextGameTry()
    {
        _livesLeft -= 1;
        SetLivesText();
        if (_livesLeft < 1)
        {
            SceneManager.LoadScene("MainScene");
        }
    }

    private void SetLivesText()
    {
        livesText.text = $"{_livesLeft} LIVES LEFT";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            onEat?.Invoke();
            Destroy(other.gameObject);
            AddTail();
            AddFood();
        }
        else if (_dangerObjects.Contains(other.gameObject.tag))
        {
            onDie?.Invoke();
            NextGameTry();
        }
    }
}