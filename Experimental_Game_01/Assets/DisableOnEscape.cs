using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnEscape : MonoBehaviour
{
    GameObject child;
    // Start is called before the first frame update
    void Start()
    {
        child = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            child.SetActive(!child.activeInHierarchy);
    }
}
