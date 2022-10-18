using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerMovementUI _playerMovementUI;

    private Queue<Action> _movementCommandsQueue;
    private bool _firstMovement = true;
    private UniTask _movement;

    private void Start()
    {
        _movementCommandsQueue = new Queue<Action>();
    }

    private void OnEnable()
    {
        _playerMovementUI.MovementButtonPressed += Move;
    }

    private void OnDisable()
    {
        _playerMovementUI.MovementButtonPressed -= Move;
    }

    private void Move(Vector2? moveDirection)
    {
        _movementCommandsQueue.Enqueue(UniTask.Action(async () =>
        {
            _firstMovement = false;
            await transform
                .DOLocalMove(transform.localPosition + new Vector3(moveDirection.Value.x, moveDirection.Value.y, 0) * 300, 0.8f)
                .OnComplete(StartMove).SetEase(Ease.Linear).AsyncWaitForCompletion().AsUniTask().AttachExternalCancellation(this.GetCancellationTokenOnDestroy());
        }));
        if (_firstMovement)
            StartMove();
    }

    private void StartMove()
    {
        if (_movementCommandsQueue.Count == 0)
        {
            _firstMovement = true;
            return;
        }
        _movementCommandsQueue.Dequeue().Invoke();
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}