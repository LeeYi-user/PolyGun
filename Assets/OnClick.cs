using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OnClick : MonoBehaviour
{
    public Canvas canvas;
    public Button button;
    public TMP_InputField inputField;

    private GameObject forScript;

    // Start is called before the first frame update
    void Start()
    {
        forScript = GameObject.Find("For Script");
        button.onClick.AddListener(OnClickHandler);
    }

    // Update is called once per frame
    void Update()
    {
        if (inputField.text == "")
        {
            button.GetComponentInChildren<TMP_Text>().text = "HOST";
        }
        else
        {
            button.GetComponentInChildren<TMP_Text>().text = "JOIN";
        }
    }

    void OnClickHandler()
    {
        canvas.enabled = false;

        if (button.GetComponentInChildren<TMP_Text>().text == "HOST")
        {
            forScript.GetComponent<UnityRelay>().CreateRelay();
        }
        else
        {
            forScript.GetComponent<UnityRelay>().JoinRelay(inputField.text);
        }
    }
}
