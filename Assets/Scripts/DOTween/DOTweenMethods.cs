using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

//http://dotween.demigiant.com/documentation.php
public class DOTweenMethods : MonoBehaviour
{
    [SerializeField] private ExampleType _example;
    [SerializeField] private Transform[] _objectsToMove;

    private Dictionary<ExampleType, Action> _examplesDictionary;

    private async void Start()
    {
        _examplesDictionary = new Dictionary<ExampleType, Action>
        {
            {ExampleType.MoveAndRotate, DoMove},
            {ExampleType.Sequence, Sequence},
            {ExampleType.Async,AsyncMethod},
            {ExampleType.SimultaneousAsync, SimultaneousAsync},
            {ExampleType.Jump, Jump},
            {ExampleType.Shake,Shake},
            {ExampleType.Punch,Punch},
            {ExampleType.DoVirtual, DoVirtual},
            {ExampleType.SpeedBase,SpeedBase}
        };
        await Task.Delay(1000);
        _examplesDictionary[_example].Invoke();
    }

    //ease-https://easings.net/
    //ease.flash - мерцание
    //DoLocalMove Snapping - привязка к int 
    //Rotatemode.Fast - Наименьший путь, Rotatemode.FastBeyond360 - выйдет за 360
    private void DoMove()
    {
        _objectsToMove[0].DOLocalMove(Vector3.zero, 1).SetEase(Ease.Linear).SetLoops(2, LoopType.Yoyo);
        _objectsToMove[0].DORotate(new Vector3(0, 360, 0), 1, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);
    }

    //AppendCallback - добавляем колбек в sequence
    //ApendInterval - добавляем ожидание в sequence
    //Insert вклиниваем tween в необходимое время sequence
    //Join - добавляем tween к последнему tween или callback в sequence
    //Prepend - впихиваем tween в самое начало
    private void Sequence()
    {
        const int duration = 2;
        Sequence sequence = DOTween.Sequence();

        foreach (var objectToMove in _objectsToMove)
        {
            sequence.Append(objectToMove.DOMoveX(0, duration));
        }

        sequence.OnComplete(() => { sequence.Rewind(); });
    }
    
    private async void AsyncMethod()
    {
        const int duration = 2;
        foreach (var objectToMove in _objectsToMove)
        {
            await objectToMove.DOMoveX(0, duration).AsyncWaitForCompletion();
        }
    }
    
    private async void SimultaneousAsync()
    {
        const int duration = 2;
        List<Task> tasks = new List<Task>();

        foreach (var objectToMove in _objectsToMove)
        {
            tasks.Add(objectToMove.DOMoveX(0,duration).AsyncWaitForCompletion() );
        }

        await Task.WhenAll(tasks);
        Debug.Log("HI");
    }

    private void Jump()
    {
        const int jumpPower = 5;
        const int numberOfJumps = 1;
        const int jumpDuration = 2;

        _objectsToMove[0].DOJump(Vector3.zero, jumpPower, numberOfJumps, jumpDuration);
    }

    //При  удалении объекта ВСЕГДА делаем kill на твине
    private void Shake()
    {
        const int duration = 2;
        const float strength = 2;
        _objectsToMove[0].DOShakePosition(duration, strength).OnComplete(()=>_objectsToMove[0].DOKill());
    }

    private void Punch()
    {
        const int duration = 2;

        _objectsToMove[0].DOPunchPosition(Vector3.left*3, 1,0,0).SetDelay(duration*0.5f).SetEase(Ease.OutExpo);
    }

    private void DoVirtual()
    {
        const float startingValue = 0;
        const float endValue = 2;
        const int duration = 2;

        DOVirtual.Float(startingValue, endValue, duration, t =>
        {
            Debug.Log(t);
        }).OnComplete(()=>DOVirtual.DelayedCall(1,SayHi));
    }

    private void SpeedBase()
    {
        const int speed = 1;
        
        _objectsToMove[0].DOMoveX(0, speed).SetSpeedBased(true);
    }

    //SetAs - задаем одинаковые параметры tween или TweenOarams
    //SetAutoKill - убиваем твин, как он доиграет
    //SetInverted - играет с конца в начало
    //SetUpdate - устанавливаем тип update(fixed,late,normal)
    //SetDelay - ставим задержку
    //SetSpeedBased - вместо времени твина ставим скорость
    //Остальные set не так полезны
    private void TweenerSettings()
    {
        Tween tween=_objectsToMove[0].DOMoveX(0, 1);
        tween.timeScale = 0.5f;
        _objectsToMove[0].DOMoveX(0, 1).SetAs(tween).SetAutoKill(true).SetDelay(1).SetSpeedBased(true);
    }

    //OnRewind - при вызове rewind или когда играет назад и дошел до начальной позиции
    //OnStart - 1 раз срабатывает при старте твина, в отличие от OnPlay
    //OnStepComplete - срабатывает при каждом loop
    private void ChainedCallBacks()
    {
        transform.DOMoveX(0, 2).OnRewind(SayHi).OnStepComplete(SayHi).OnStart(SayHi);
    }

    //1)Можно совершать действия со всеми твинами или с твинами объекта через статику
    //2)Pause() у твина
    //3)transform.DoPause();
    //Flip - меняет направлениетвина
    //GoTo - идет к определенному времени твина
    //Rewind - перематывает назад
    //Toggle - включает/выключает паузу
    private void ControlTween()
    {
        DOTween.PauseAll();
        DOTween.Kill(transform);
        transform.DORewind();
    }

    private void SayHi()
    {
        Debug.Log("hi");
    }

    //убиваем все твины при деактивации объекта
    private void OnDisable()
    {
        transform.DOKill();
    }

    private enum ExampleType
    {
        MoveAndRotate,
        Sequence,
        Async,
        SimultaneousAsync,
        Jump,
        Shake,
        Punch,
        DoVirtual,
        SpeedBase
    }
}