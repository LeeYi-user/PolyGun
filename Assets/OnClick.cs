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
    private bool join;

    // Start is called before the first frame update
    void Start()
    {
        forScript = GameObject.Find("For Script");
        button.onClick.AddListener(OnClickHandler);
    }

    // Update is called once per frame
    void Update()
    {
        if (inputField.text != "")
        {
            join = true;
            button.GetComponentInChildren<TMP_Text>().text = "JOIN";
        }
        else
        {
            join = false;
            button.GetComponentInChildren<TMP_Text>().text = "HOST";
        }
    }

    void OnClickHandler()
    {
        canvas.enabled = false;

        if (join)
        {
            forScript.GetComponent<UnityRelay>().JoinRelay(inputField.text);
        }
        else
        {
            forScript.GetComponent<UnityRelay>().CreateRelay();
        }
    }
}
