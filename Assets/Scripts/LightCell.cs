using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public class LightCell : MonoBehaviour
{
    // Components
    public Image CellImage {  get; private set; }
    // Parameters
    public bool isActive = false;
    private int _xIndex;
    private int _yIndex;

    private void Awake()
    {
        CellImage = GetComponent<Image>();
    }

    public void SetCoord(int x, int y)
    {
        _xIndex = x;
        _yIndex = y;
    }

    public void SendCoord()
    {
        LightsOut.Instance.HandleClick(_xIndex, _yIndex);
    }
}
