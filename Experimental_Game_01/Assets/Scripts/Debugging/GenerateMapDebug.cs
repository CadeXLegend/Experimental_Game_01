using Generation;
using UnityEngine;

public class GenerateMapDebug : MonoBehaviour
{
    [SerializeField] private string keycode;
    public string Keycode { get => keycode; }
    private KeyCode kc;
    [SerializeField] private MapGenerator generator;
    [SerializeField] private GridContainer container;
    // Start is called before the first frame update
    void Start()
    {
        kc = (KeyCode)System.Enum.Parse(typeof(KeyCode), keycode);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(kc))
        {
            container.ClearGrid();
            generator.Generate();
        }
    }
}
