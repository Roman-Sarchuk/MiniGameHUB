using TMPro;
using UnityEngine;

public class Stopwatch : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TMP_Text _stopwatch;
    private float elapsedTime = 0f;
    private bool isRunning = false;

    void Awake()
    {
        UpdateStopwatch();
    }

    void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateStopwatch();
        }
    }

    public void Play()
    {
        isRunning = true;
    }

    public void Stop()
    {
        isRunning = false;
    }

    private void UpdateStopwatch()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        if (minutes < 100)
            _stopwatch.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        else
            _stopwatch.text = "∞";
    }

    public void Refresh()
    {
        isRunning = false;
        elapsedTime = 0f;
        UpdateStopwatch();
    }
}
