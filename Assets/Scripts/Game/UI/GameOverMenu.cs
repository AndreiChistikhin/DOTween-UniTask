using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField] private PlayerHP _playerHP;
    [SerializeField] private Button _playAgainButton;
    [SerializeField] private Button _quitButton;
    [SerializeField] private CanvasGroup _menuCanvasGroup;

    private const float MenuShowTime=1;

    private void Start()
    {
        _playAgainButton.enabled = false;
        _quitButton.enabled = false;
        _menuCanvasGroup.alpha = 0;
    }

    private void OnEnable()
    {
        _playerHP.PlayerDied += ShowMenu;
        _playAgainButton.onClick.AddListener(PlayAgain);
        _quitButton.onClick.AddListener(QuitGame);
    }

    private void OnDisable()
    {
        _playerHP.PlayerDied -= ShowMenu;
        _playAgainButton.onClick.RemoveListener(PlayAgain);
        _quitButton.onClick.RemoveListener(QuitGame);
    }

    private void ShowMenu()
    {
        gameObject.SetActive(true);
        _menuCanvasGroup.DOFade(1, MenuShowTime).OnComplete(() =>
        {
            _playAgainButton.enabled = true;
            _quitButton.enabled = true;
        }).WithCancellation(this.GetCancellationTokenOnDestroy());
    }

    private void PlayAgain()
    {
        SceneManager.LoadScene(sceneBuildIndex: 0);
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        Debug.Log("Quit");
#else 
        Application.Quit();
#endif
    }
}
