using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class PlayerInfo : MonoBehaviour
{
    public int role = 0;
    public string username = "Krusher98";

    public InputField nameField;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void UpdateName()
    {
        username = nameField.text;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            role = 1;
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            role = 2;
        }
    }

    public void NextScene()
    {
        SceneManager.LoadScene(1);
    }
}
