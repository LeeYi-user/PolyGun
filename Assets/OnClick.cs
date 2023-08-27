using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OnClick : MonoBehaviour
{
    public Button button;
    public TMP_InputField inputField;

    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(OnClickHandler);
    }

    // Update is called once per frame
    void Update()
    {
        if (inputField.text != "")
        {
            button.GetComponentInChildren<TMP_Text>().text = "JOIN";
        }
        else
        {
            button.GetComponentInChildren<TMP_Text>().text = "HOST";
        }
    }

    void OnClickHandler()
    {
        Debug.Log(inputField.text);
    }
}
