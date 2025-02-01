using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class LightsOut : MonoBehaviour
{
    // Range: [(_fieldHight / _maxBlockSize), (_fieldHight / _
    [Header("Components")]
    [SerializeField] private GameObject _fieldBox;
    [SerializeField] private Counter _counter;
    [SerializeField] private Stopwatch _stopwatch;
    [SerializeField] private GameObject _victoryBox;
    [SerializeField] private GameObject _cellPrefab;
    [Header("Input")]
    // Range: [(_fieldWidth / _maxBlockSize), (_fieldWidth / _minBlockSize)]
    [SerializeField] private int _col = 5;
    // Range: [(_fieldHight / _maxBlockSize), (_fieldHight / _minBlockSize)]
    [SerializeField] private int _row = 5;
    [SerializeField] private int _minOnCellCount = 1;
    [SerializeField] private int _maxOnCellCount = 10;
    [SerializeField] private bool _isAutoCalcOnCell = true; // _minOnCellCount = (_col * _row * 1 / 5) , _maxOnCellCount = (_col * _row * 4 / 5)
    [SerializeField] private Color _offColor = new Color(0.231f, 0.231f, 0.231f, 1f);
    [SerializeField] private Color _onColor = new Color(0.207f, 0.69f, 0.325f, 1f);
    [Header("Limitation")]
    [SerializeField] private int _fieldWidth = 1000;
    [SerializeField] private int _fieldHight = 1000;
    [SerializeField] private int _minBlockSize = 50;
    [SerializeField] private int _maxBlockSize = 200;
    // ***** ***** *****
    public static LightsOut Instance { get; private set; }
    private LightCell[,] _field;

    private void Start()
    {
        // singleton initialization
        if (Instance == null)
            Instance = this;
        else
            Debug.LogError($"ERR[{gameObject.name}]: LightsOut must be a singleton");

        // verification
        if (!(_fieldWidth / _maxBlockSize <= _col && _col <= _fieldWidth / _minBlockSize))
            Debug.LogError($"ERR[{gameObject.name}]: _col must be in range [{_fieldWidth / _maxBlockSize}, {_fieldWidth / _minBlockSize}]");

        if (!(_fieldHight / _maxBlockSize <= _row && _row <= _fieldHight / _minBlockSize))
            Debug.LogError($"ERR[{gameObject.name}]: _row must be in range [{_fieldHight / _maxBlockSize}, {_fieldHight / _minBlockSize}]");
        // verification
        if (!_isAutoCalcOnCell) 
        {
            if (_minOnCellCount < 1)
                Debug.LogError($"ERR[{gameObject.name}]: _minOnCellCount must be >= 1");
            else if (_maxOnCellCount >= _col * _row)
                Debug.LogError($"ERR[{gameObject.name}]: _minOnCellCount must be < {_col * _row}");
            else if (_minOnCellCount >= _maxOnCellCount)
                Debug.LogError($"ERR[{gameObject.name}]: _minOnCellCount must be < _maxOnCellCount");
        }
        else   // calc OnCellCount
        {
            _minOnCellCount = _col * _row / 5;
            _maxOnCellCount = _col * _row * 4 / 5;
        }
    }

    private void Awake()
    {
        // configure field
        _fieldBox.GetComponent<RectTransform>().sizeDelta = new Vector2(_fieldWidth, _fieldHight);
        _fieldBox.GetComponent<GridLayoutGroup>().cellSize = new Vector2(_fieldWidth / _col, _fieldHight / _row);
        _victoryBox.GetComponent<RectTransform>().sizeDelta = new Vector2(_fieldWidth, _fieldHight);

        // init field
        Restart();
    }
    // ***** ***** *****

    private void CreateField()
    {
        // clear field
        foreach (Transform child in _fieldBox.transform)
        {
            Destroy(child.gameObject);
        }

        _field = new LightCell[_row, _col];

        // create cells
        GameObject obj;
        int i, j;
        for (i = 0; i < _row; i++)
        {
            for (j = 0; j < _col; j++)
            {
                obj = Instantiate(_cellPrefab, Vector3.zero, Quaternion.identity, transform);
                _field[i, j] = obj.GetComponent<LightCell>();

                obj.transform.SetParent(_fieldBox.transform, false);

                _field[i, j].SetCoord(i, j);
                _field[i, j].CellImage.color = _offColor;
            }
        }

        // rand turn on some cells
        int k = _isAutoCalcOnCell ? Random.Range(_col * _row / 5, _col * _row * 4 / 5) : Random.Range(_minOnCellCount, _maxOnCellCount);
        while (k > 0)
        {
            i = Random.Range(0, _row);
            j = Random.Range(0, _col);

            if (!_field[i, j].isActive)
            {
                ToggleCell(i, j);

                k--;
            }
        }
    }

    public void HandleClick(int x, int y)
    {
        _counter.Increment();

        ToggleCell(x, y);
        if (x > 0) ToggleCell(x - 1, y);
        if (y > 0) ToggleCell(x, y - 1);
        if (x < _col - 1) ToggleCell(x + 1, y);
        if (y < _row - 1) ToggleCell(x, y + 1);

        CheckWin();
    }

    private void ToggleCell(int x, int y)
    {
        _field[x, y].isActive = !_field[x, y].isActive;

        _field[x, y].CellImage.color = _field[x, y].isActive ? _onColor : _offColor;
    }

    private void CheckWin()
    {
        bool state = _field[0, 0].isActive;
        for (int i = 0; i < _row; i++)
        {
            for (int j = 0; j < _col; j++)
            {
                if (_field[i, j].isActive != state)
                    return;
            }
        }

        // victory processing
        _fieldBox.SetActive(false);
        _victoryBox.SetActive(true);

        _stopwatch.Stop();
    }

    public void Restart()
    {
        _counter.Refresh();
        _stopwatch.Refresh();
        _stopwatch.Play();

        CreateField();

        _fieldBox.SetActive(true);
        _victoryBox.SetActive(false);
    }
}
