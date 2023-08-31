using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class OnClick : MonoBehaviour
{
    public Button button;
    public TMP_InputField inputField;
    public GameObject forScript;

    private bool join;

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
        SceneManager.LoadScene("SampleScene");

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
