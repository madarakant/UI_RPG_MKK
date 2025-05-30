using UnityEngine;
using UnityEngine.UI;

public class RestartButton : MonoBehaviour
{
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(Restart);
    }

    private void Restart()
    {
        GameManager.Instance.RestartGame();
    }
}