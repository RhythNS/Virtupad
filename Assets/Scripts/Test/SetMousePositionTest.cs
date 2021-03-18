using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMousePositionTest : MonoBehaviour
{
    private uDesktopDuplication.Texture texture;

    // Start is called before the first frame update
    void Start()
    {
        texture = GetComponent<uDesktopDuplication.Texture>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
            texture.monitor.SetMousePosition(200, 200);
    }
}
