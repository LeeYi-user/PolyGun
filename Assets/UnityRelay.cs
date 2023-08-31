using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UnityRelay : MonoBehaviour
{
    public GameObject player;
    private bool playing;

    // Start is called before the first frame update
    private async void Start()
    {
        await UnityServices.InitializeAsync();

        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        catch (AuthenticationException e)
        {
            Debug.Log(e);
        }

        SceneManager.LoadScene("MenuScene");
    }

    // Update is called once per frame
    private void Update()
    {
        if (player && !playing)
        {
            playing = true;
        }

        if (!player && playing)
        {
            playing = false;
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene("MenuScene");
        }
    }

    public async void CreateRelay()
    {
        try
        {
            SceneManager.LoadScene("SampleScene");

            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(4);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartHost();

            ShowCode(joinCode);
        }
        catch (RelayServiceException e)
        {
            SceneManager.LoadScene("MenuScene");
            Debug.Log(e);
        }
    }

    public async void JoinRelay(string joinCode)
    {
        try
        {
            SceneManager.LoadScene("SampleScene");

            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartClient();

            ShowCode(joinCode);
        }
        catch (RelayServiceException e)
        {
            SceneManager.LoadScene("MenuScene");
            Debug.Log(e);
        }
    }

    void ShowCode(string joinCode)
    {
        GameObject.Find("Code").GetComponent<TMP_Text>().text = joinCode;
    }
}
