using TMPro;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class Counter : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TMP_Text _counter;
    private int _currentValue = 0;

    void Awake()
    {
        UpdateCounter();
    }

    public void Increment()
    {
        _currentValue++;
        UpdateCounter();
    }

    void UpdateCounter()
    {
        if (_currentValue < 1000)
            _counter.text = _currentValue.ToString("D3"); // Відображення у форматі 000
        else
            _counter.text = "∞";
    }

    public void Refresh()
    {
        _currentValue = 0;
        UpdateCounter();
    }
}
