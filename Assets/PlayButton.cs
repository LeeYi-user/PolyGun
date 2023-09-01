using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayButton : MonoBehaviour
{
    public Canvas canvas;
    public TMP_InputField inputField;

    private GameObject forScript;

    // Start is called before the first frame update
    void Start()
    {
        forScript = GameObject.Find("For Script");
    }

    // Update is called once per frame
    void Update()
    {
        if (inputField.text == "")
        {
            gameObject.GetComponentInChildren<TMP_Text>().text = "HOST";
        }
        else
        {
            gameObject.GetComponentInChildren<TMP_Text>().text = "JOIN";
        }
    }

    public void OnClick()
    {
        canvas.enabled = false;

        if (gameObject.GetComponentInChildren<TMP_Text>().text == "HOST")
        {
            forScript.GetComponent<UnityRelay>().CreateRelay();
        }
        else
        {
            forScript.GetComponent<UnityRelay>().JoinRelay(inputField.text);
        }
    }
}
