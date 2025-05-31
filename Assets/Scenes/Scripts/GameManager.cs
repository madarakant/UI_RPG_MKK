using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Canvas))]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Player Settings")]
    [SerializeField] private Vector2 playerScreenPosition = new Vector2(-300, 0);
    [SerializeField] private Sprite playerSprite;

    [Header("Enemy Prefabs")] 
    [SerializeField] private GameObject kaitoPrefab;
    [SerializeField] private GameObject rinPrefab;
    [SerializeField] private GameObject lenPrefab;
    [SerializeField] private Vector2 enemyScreenPosition = new Vector2(300, 0);

    [Header("UI References")]
    [SerializeField] private TMP_Text playerStatsText;
    [SerializeField] private TMP_Text enemyStatsText;
    [SerializeField] private TMP_Text battleLogText;
    [SerializeField] private Button attackButton;
    [SerializeField] private Button shieldButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private GameObject gameOverPanel;

    private Player player;
    private Enemy currentEnemy;
    private bool gameActive = true;
    private Canvas mainCanvas;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            mainCanvas = GetComponent<Canvas>();
            InitializeGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeGame()
    {
        if (InitializeUI())
        {
            CreatePlayer();
            SpawnNewEnemy();
        }
    }

    private bool InitializeUI()
    {
        if (attackButton == null || shieldButton == null || restartButton == null)
        {
            Debug.LogError("Missing UI buttons!");
            return false;
        }

        attackButton.onClick.AddListener(OnAttackButtonClicked);
        shieldButton.onClick.AddListener(OnShieldButtonClicked);
        restartButton.onClick.AddListener(RestartGame);
        
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        
        if (restartButton != null)
            restartButton.gameObject.SetActive(false);

        return true;
    }

    private void CreatePlayer()
    {
        GameObject playerObj = new GameObject("Player");
        playerObj.transform.SetParent(mainCanvas.transform, false);
        
        Image playerImage = playerObj.AddComponent<Image>();
        playerImage.sprite = playerSprite;
        playerImage.preserveAspect = true;
        
        RectTransform rt = playerObj.GetComponent<RectTransform>();
        rt.anchoredPosition = playerScreenPosition;
        rt.sizeDelta = new Vector2(150, 150);

        player = playerObj.AddComponent<Player>();
        
        if (playerStatsText != null && battleLogText != null)
        {
            player.SetTextComponents(playerStatsText, battleLogText);
            player.OnDeathEvent.AddListener(HandlePlayerDeath);
        }
        else
        {
            Debug.LogError("Player UI text components not assigned!");
        }
    }

    private void SpawnNewEnemy()
    {
        if (currentEnemy != null)
        {
            Destroy(currentEnemy.gameObject);
        }

        GameObject enemyPrefab = GetRandomEnemyPrefab();
        if (enemyPrefab == null) // Fixed typo: enemyPrefab instead of enemyPrefrab
        {
            Debug.LogError("No enemy prefabs assigned in GameManager!");
            CreateFallbackEnemy();
            return;
        }

        GameObject enemyObj = Instantiate(enemyPrefab, mainCanvas.transform);
        enemyObj.name = enemyPrefab.name + "_Instance";

        VerifyEnemyComponents(enemyObj);

        currentEnemy = enemyObj.GetComponent<Enemy>();
        if (currentEnemy == null)
        {
            Debug.LogError("Enemy component still missing after verification!");
            Destroy(enemyObj);
            CreateFallbackEnemy();
            return;
        }

        RectTransform rt = enemyObj.GetComponent<RectTransform>();
        rt.anchoredPosition = enemyScreenPosition;
        rt.sizeDelta = new Vector2(150, 150);

       
    }

    private void VerifyEnemyComponents(GameObject enemyObj)
    {
        if (!enemyObj.TryGetComponent(out Image image))
        {
            image = enemyObj.AddComponent<Image>();
            image.preserveAspect = true;
            Debug.LogWarning("Added missing Image component to enemy");
        }

        if (!enemyObj.TryGetComponent(out Enemy _))
        {
            if (enemyObj.name.Contains("Kaito"))
                enemyObj.AddComponent<Kaito>();
            else if (enemyObj.name.Contains("Rin"))
                enemyObj.AddComponent<Rin>();
            else if (enemyObj.name.Contains("Len"))
                enemyObj.AddComponent<Len>();
            else
                enemyObj.AddComponent<Enemy>();
        }

        if (!enemyObj.TryGetComponent(out Weapon _))
        {
            if (enemyObj.name.Contains("Rin"))
                enemyObj.AddComponent<Axe>();
            else
                enemyObj.AddComponent<Sword>();
        }
    }

    private void CreateFallbackEnemy()
    {
        GameObject fallback = new GameObject("FallbackEnemy", 
            typeof(RectTransform), 
            typeof(Image), 
            typeof(Enemy), 
            typeof(Sword));

        Image img = fallback.GetComponent<Image>();
        img.color = Color.magenta;
        img.rectTransform.sizeDelta = new Vector2(150, 150);

        RectTransform rt = fallback.GetComponent<RectTransform>();
        rt.anchoredPosition = enemyScreenPosition;

        currentEnemy = fallback.GetComponent<Enemy>();
        
        // Use the Enemy's public method to set name if available
        if (currentEnemy is Enemy enemy)
        {
            enemy.SetCharacterName("Fallback");
        }
        
        
        Debug.LogWarning("Created fallback enemy due to prefab issues");
    }

    

    private void HandleEnemyDeath()
    {
        AppendToBattleLog("\nYou defeated the enemy!");
        SpawnNewEnemy();
    }

    private GameObject GetRandomEnemyPrefab()
    {
        int enemyType = Random.Range(0, 3);
        switch (enemyType)
        {
            case 0: return kaitoPrefab;
            case 1: return rinPrefab;
            case 2: return lenPrefab;
            default: return null;
        }
    }

    private void OnAttackButtonClicked()
    {
        if (!gameActive || player == null || currentEnemy == null) return;

        player.Attack(currentEnemy);
        currentEnemy.UpdateStatsDisplay();

        if (currentEnemy.IsDead())
        {
            AppendToBattleLog("\nYou defeated the enemy!");
            SpawnNewEnemy();
            return;
        }

        currentEnemy.Attack(player);
        player.UpdateStatsDisplay();
    }

    private void OnShieldButtonClicked()
    {
        if (!gameActive || player == null) return;
        player.ToggleShield();
    }

    private void HandlePlayerDeath()
    {
        gameActive = false;
        AppendToBattleLog("\nYou have been defeated! Game Over!");
        
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
        
        if (restartButton != null)
            restartButton.gameObject.SetActive(true);
        
        player?.SetVisible(false);
    }

    public void RestartGame()
    {
        StartCoroutine(RestartGameRoutine());
    }

    private IEnumerator RestartGameRoutine()
    {
        if (currentEnemy != null) 
            Destroy(currentEnemy.gameObject);
        
        if (player != null) 
            Destroy(player.gameObject);

        if (battleLogText != null)
            battleLogText.text = "Game restarted!\nReady for battle!";

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (restartButton != null)
            restartButton.gameObject.SetActive(false);

        yield return null;

        gameActive = true;
        CreatePlayer();
        SpawnNewEnemy();
    }

    private void AppendToBattleLog(string message)
    {
        if (battleLogText != null)
        {
            battleLogText.text += message;
            
            var scrollRect = battleLogText.GetComponentInParent<ScrollRect>();
            if (scrollRect != null)
            {
                Canvas.ForceUpdateCanvases();
                scrollRect.verticalNormalizedPosition = 0;
            }
        }
    }
}