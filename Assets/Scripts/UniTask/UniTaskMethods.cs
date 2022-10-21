using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.LowLevel;
using UnityEngine.Networking;
using UnityEngine.UI;

//https://github.com/Cysharp/UniTask
//https://cysharp.github.io/UniTask/api/Cysharp.Threading.Tasks.UnityAsyncExtensions.html
//Преимущества над тасками:
//UniTask структура, библиотека намного меньше алочит память
//Делает все AsyncObject(UnityWebRequest...) и Coroutines awaitable
//Нет тредов, можно запускать на Webgl, юзает PlayerLoop
//Проще говоря, как написано в доке:
//Task is too heavy and not matched to Unity threading (single-thread).
//UniTask does not use threads and SynchronizationContext/ExecutionContext because Unity's asynchronous object is automaticaly dispatched by Unity's engine layer.
//It achieves faster and lower allocation, and is completely integrated with Unity.
//You can await AsyncOperation, ResourceRequest, AssetBundleRequest, AssetBundleCreateRequest, UnityWebRequestAsyncOperation, AsyncGPUReadbackRequest,
//IEnumerator and others when using Cysharp.Threading.Tasks;.
public class UniTaskMethods : MonoBehaviour
{
    [SerializeField] private ExampleType _example;
    [SerializeField] private Button _button;

    private Dictionary<ExampleType, Func<UniTaskVoid>> _examplesDictionary;

    private async UniTaskVoid Start()
    {
        _examplesDictionary = new Dictionary<ExampleType,Func<UniTaskVoid>>
        {
            {ExampleType.TextReturn, TextReturn},
            {ExampleType.ToUniTask, ToUniTask},
            {ExampleType.Delays, Delays},
            {ExampleType.SendRequest, SendRequest},
            {ExampleType.CancellationUnityToken,CancellationUnityToken},
            {ExampleType.TimeOutAndLink, TimeOutAndLink},
            {ExampleType.ShowProgress, ShowProgress},
            {ExampleType.PlayerLoops, PlayerLoops},
            {ExampleType.UniTaskAction,UniTaskAction},
            {ExampleType.TripleClick,TripleClick},
            {ExampleType.TripleCLickForEachAsync,TripleClickForEachAsync},
            {ExampleType.Wait3Sec,Wait3SecondsAfterPress}
        };
        await UniTask.Delay(1000);
        _examplesDictionary[_example].Invoke();
    }

    private async UniTaskVoid TextReturn()
    {
        string text = await TextReturnAsync();
        Debug.Log(text);
    }

    private async UniTask<string> TextReturnAsync()
    {
        await UniTask.Delay(5000);
        return "HI";
    }

    private async UniTaskVoid ToUniTask()
    {
        await Resources.LoadAsync<TextAsset>("HI").ToUniTask(Progress.Create<float>(x => Debug.Log(x)));
    }

    private async UniTaskVoid Delays()
    {
        CancellationTokenSource tokenSource = new CancellationTokenSource();
        await UniTask.Delay(TimeSpan.FromSeconds(1), DelayType.UnscaledDeltaTime, PlayerLoopTiming.FixedUpdate,
            tokenSource.Token);
        Debug.Log("UnitaskDelay - 1 sec");
        await UniTask.Delay(1000, ignoreTimeScale: true, cancellationToken: tokenSource.Token);
        Debug.Log("UniTask.Delay - different overload");
        await UniTask.DelayFrame(100);
        Debug.Log("Delay100Frames");
        await UniTask.Yield(PlayerLoopTiming.EarlyUpdate);
        Debug.Log("YieldEarlyUpdate");
        await UniTask.NextFrame();
        Debug.Log("NextFrame");
        await UniTask.WaitForEndOfFrame(this); 
        Debug.Log("WaitForEndOfFrame");
        await UniTask.WaitForFixedUpdate();
        Debug.Log("WaitForFixedUpdate");
        await UniTask.WaitUntil(() => gameObject.activeSelf);
        Debug.Log("WaitUntil");
        UniTask.Post(UniTask.Action(async () =>
        {
            Debug.Log("Post started");
            await UniTask.Delay(5000);
            Debug.Log("Post 5 sec");
        }));
        UniTask.Create(async () =>
        {
            Debug.Log("CreateStarted");
            await UniTask.Delay(5000);
            Debug.Log("Create 5 sec");
        });
        //UniTask.sw
        // await UniTask.WaitUntilValueChanged(this, x => x.isActiveAndEnabled);
        //await UniTask.WhenAny() UniTask.WhenAll() UniTask.WaitWhile()
    }

    private async UniTaskVoid SendRequest()
    {
        GetTextAsync(UnityWebRequest.Get("http://google.com")).Forget();

        GetTextAsync(UnityWebRequest.Get("http://bing.com")).Forget();

        GetTextAsync(UnityWebRequest.Get("http://yahoo.com")).Forget();
    }
    
    async UniTaskVoid GetTextAsync(UnityWebRequest req)
    {
        var op = await req.SendWebRequest()
            .WithCancellation(this.GetCancellationTokenOnDestroy()); //withCancellation - метод расширения UniTask
        Debug.Log(op.downloadHandler.text);
    }

    private async UniTaskVoid CancellationUnityToken()
    {
        var timeOutToken = new CancellationTokenSource();
        timeOutToken.CancelAfterSlim(TimeSpan.FromSeconds(5));
        StartCancellation(timeOutToken).Forget();
        Debug.Log("SuppressCancellation");
        bool x=await UniTask.Delay(10000, cancellationToken:  timeOutToken.Token).SuppressCancellationThrow();
        Debug.Log(x);
        //рекомендуется передавать токен в параметрах, Suppress... делает булик вместе экепшна при кансле и метод не прерывается
    }

    private async UniTaskVoid StartCancellation(CancellationTokenSource timeOutToken)
    {
        await UniTask.WaitUntilCanceled(timeOutToken.Token);
        timeOutToken.Cancel();
    }

    private async UniTaskVoid TimeOutAndLink()
    {
        var cts = new CancellationTokenSource();
        var timeOutToken = new CancellationTokenSource();
        timeOutToken.CancelAfterSlim(TimeSpan.FromSeconds(5));
        await UnityWebRequest.Get("http://yahoo.com").SendWebRequest().WithCancellation(timeOutToken.Token);
        Debug.Log("TimeoutCancellation");
        var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, timeOutToken.Token);
        await UnityWebRequest.Get("http://yahoo.com").SendWebRequest().WithCancellation(linkedTokenSource.Token);
        Debug.Log("LinkedCancellation");
        TimeoutController timeoutController = new TimeoutController();
        await UnityWebRequest.Get("http://yahoo.com").SendWebRequest()
            .WithCancellation(timeoutController.Timeout(TimeSpan.FromSeconds(5)));
        timeoutController.Reset();
        Debug.Log("TimeOutController");
    }

    private async UniTaskVoid ShowProgress()
    {
        var progress = Progress.Create<float>(x => Debug.Log(x));

        var request = await UnityWebRequest.Get("http://yahoo.com")
            .SendWebRequest()
            .ToUniTask(progress: progress);
    }

    private async UniTaskVoid PlayerLoops()
    {
        //ECS
        //var playerLoop = ScriptBehaviourUpdateOrder.CurrentPlayerLoop;
        //PlayerLoopHelper.Initialize(ref playerLoop);
        //Check if is ready
        Debug.Log("UniTaskPlayerLoop ready? " + PlayerLoopHelper.IsInjectedUniTaskPlayerLoop());
        PlayerLoopHelper.DumpCurrentPlayerLoop();
        var loop = PlayerLoop.GetCurrentPlayerLoop();
        PlayerLoopHelper.Initialize(ref loop, InjectPlayerLoopTimings.Minimum);
    }

    private async UniTaskVoid UniTaskAction()
    {
        Action actEvent = null;

        actEvent += UniTask.Action(async () =>
        {
            await UniTask.Yield();
            Debug.Log("UniTask.Action");
        });
        actEvent.Invoke();
    }

    async UniTaskVoid TripleClick()
    {
        await _button.OnClickAsync();
        await _button.OnClickAsync();
        await _button.OnClickAsync();
        Debug.Log("Three times clicked");
    }
    
    async UniTaskVoid TripleClickForEachAsync()
    {
        await _button.OnClickAsAsyncEnumerable().Take(3).ForEachAsync(_ => { Debug.Log("Every clicked"); });
        Debug.Log("Three times clicked, complete.");
    }

    private async UniTaskVoid Wait3SecondsAfterPress()
    {
        _button.OnClickAsAsyncEnumerable().Subscribe(async x =>
        {
            await UniTask.Delay(TimeSpan.FromSeconds(3));
            Debug.Log("Subscribe, no wait queue");
        });
        await _button.OnClickAsAsyncEnumerable().Queue().ForEachAwaitAsync(async x =>
        {
            await UniTask.Delay(TimeSpan.FromSeconds(3));
            Debug.Log("ForEachAwait, 3 seconds Queue");
        });
    }
    
    public async UniTask<int> FooAsync()
    {
        await UniTask.Yield();
        throw new OperationCanceledException(); //cancel behaviour in an async UniTask method, но лучше использовать SuppressCancellationThrow,
        //т к вызов OperationCanceledException() весьма тяжелый 
    }

    //Unitask в одном треде - pPlayerLoop, перейти на другие треды - UniTask.RunOnThreadPool; UniTask.SwitchToThreadPool 
    //P.S. Unitask Tracker - топ

    private enum ExampleType
    {
        TextReturn,
        ToUniTask,
        Delays,
        SendRequest,
        CancellationUnityToken,
        TimeOutAndLink,
        ShowProgress,
        PlayerLoops,
        UniTaskAction,
        TripleClick,
        TripleCLickForEachAsync,
        Wait3Sec
    }
}