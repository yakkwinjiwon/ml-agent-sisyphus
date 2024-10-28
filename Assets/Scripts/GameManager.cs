using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    [SerializeField]
    private TextMeshProUGUI total;

    [SerializeField]
    private TextMeshProUGUI success;

    public int TotalCount => _totalCount;
    
    private int _totalCount = 0;
    private int _successCount = 0;

    private void Awake() {
        if (Instance == null)
        {
            Instance = this;
        }    
    }

    public void IncreaseTotalCount()
    {
        _totalCount++;
        total.SetText(_totalCount.ToString());
    }

    public void IncreaseSuccessCount()
    {
        _successCount++;
        success.SetText(_successCount.ToString());
    }
    
}
