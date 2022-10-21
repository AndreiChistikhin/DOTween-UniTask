using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovementUI : MonoBehaviour
{
    [SerializeField] private Button _forwardButton;
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _rightButotn;
    [SerializeField] private Button _leftButton;

    private List<Button> _buttons;
    private Dictionary<Button, Vector2?> _movementDictionary;

    public event Action<Vector2?> MovementButtonPressed;

    private async UniTaskVoid Start()
    {
        _buttons = new List<Button> {_forwardButton, _backButton, _rightButotn, _leftButton};

        _movementDictionary = new Dictionary<Button, Vector2?>
        {
            {_forwardButton, Vector2.up},
            {_backButton, Vector2.down},
            {_rightButotn, Vector2.right},
            {_leftButton, Vector2.left}
        };

        foreach (var button in _buttons)
        {
            button.onClick.AddListener(() =>
            {
                if (_movementDictionary[button] == null)
                    return;
                button.image.DOComplete();
                button.image.DOColor(Color.red, 0.2f).SetLoops(2, LoopType.Yoyo).WithCancellation(this.GetCancellationTokenOnDestroy());

                MovementButtonPressed?.Invoke(_movementDictionary[button]);
            });
        }
    }

    private void OnDestroy()
    {
        foreach (var button in _buttons)
        {
            button.image.DOKill();
        }
    }
}