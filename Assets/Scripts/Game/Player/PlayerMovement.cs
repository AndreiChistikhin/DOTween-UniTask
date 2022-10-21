using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerMovementUI _playerMovementUI;
    [SerializeField] private ScreenWrapper _screenWrapper;
    [SerializeField] private PlayerHP _playerHP;

    private Queue<Action> _movementCommandsQueue;
    private bool _firstMovement = true;
    private UniTask _movement;

    private const int MovementSpeed = 200;
    private const float MovingTime=0.5f;

    private void Start()
    {
        _movementCommandsQueue = new Queue<Action>();
    }

    private void OnEnable()
    {
        _playerMovementUI.MovementButtonPressed += Move;
        _screenWrapper.ScreenWasCrossed += StopMove;
        _playerHP.PlayerDied += () => _movementCommandsQueue = null;
    }

    private void OnDisable()
    {
        _playerMovementUI.MovementButtonPressed -= Move;
        _screenWrapper.ScreenWasCrossed -= StopMove;
    }

    private void Move(Vector2? moveDirection)
    {
        if (_movementCommandsQueue == null)
            return;
        _movementCommandsQueue.Enqueue(UniTask.Action(async () =>
        {
            _firstMovement = false;
            await transform
                .DOLocalMove(
                    transform.localPosition + new Vector3(moveDirection.Value.x, moveDirection.Value.y, 0) * MovementSpeed, MovingTime)
                .OnComplete(StartMove).SetEase(Ease.Linear).WithCancellation(this.GetCancellationTokenOnDestroy());
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

    private void StopMove()
    {
        _movementCommandsQueue.Clear();
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}