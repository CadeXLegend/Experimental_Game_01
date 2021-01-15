using UnityEngine;

public class DisableChildOnKeyPress : MonoBehaviour
{
    [SerializeField] private string keycode;
    private KeyCode kc;
    private GameObject child;
    // Start is called before the first frame update
    void Start()
    {
        child = transform.GetChild(0).gameObject;
        kc = (KeyCode)System.Enum.Parse(typeof(KeyCode), keycode);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(kc))
            child.SetActive(!child.activeInHierarchy);
    }
}
