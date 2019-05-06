﻿using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public UnityEvent onEat;
    public UnityEvent onDie;
    public List<Transform> tails;
    [Range(0.5f, 1.0f)] public float bonesDistance;
    public GameObject bonePrefab;
    [Range(0.0f, 0.2f)] public float speed;

    private Transform _transform;
    private Vector3 _defaultTailPosition;
    private readonly string[] _dangerObjects = { "Wall", "Tail" };

    private void Start()
    {
        _transform = GetComponent<Transform>();
        _defaultTailPosition = new Vector3(10.0f, _transform.position.y, 0.0f);
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
        var newPosition = _transform.position + _transform.forward * speed;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            onEat?.Invoke();
            Destroy(other.gameObject);
            AddTail();
        }
        else if (_dangerObjects.Contains(other.gameObject.tag))
        {
            onDie?.Invoke();
            SceneManager.LoadScene("MainScene");
        }
    }
}