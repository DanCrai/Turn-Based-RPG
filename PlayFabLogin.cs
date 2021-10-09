using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayFabLogin : MonoBehaviour
{
    private string userEmail;
    private string userPassword;
    private string username;
    [SerializeField]
    GameObject logInPanel;
    [SerializeField]
    GameObject registerPanel;
    [SerializeField]
    GameObject wrongCredentials;
    [SerializeField]
    GameObject registeredSuccess;
    [SerializeField]
    GameObject registrationFailPanel;
    public void Start()
    {
        //Note: Setting title Id here can be skipped if you have set the value in Editor Extensions already.
        if (string.IsNullOrEmpty(PlayFabSettings.TitleId))
        {
            PlayFabSettings.TitleId = "BBCC2"; // Please change this value to your own titleId from PlayFab Game Manager
        }
        //var request = new LoginWithCustomIDRequest { CustomId = "GettingStartedGuide", CreateAccount = true };
        //PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);

        /*if (PlayerPrefs.HasKey("EMAIL") && PlayerPrefs.HasKey("PASSWORD"))
        {
            userEmail = PlayerPrefs.GetString("EMAIL");
            userPassword = PlayerPrefs.GetString("PASSWORD");
            var request = new LoginWithEmailAddressRequest { Email = userEmail, Password = userPassword };
            PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
        }*/
    }

    private void OnLoginSuccess(LoginResult result)
    {
        PlayerPrefs.SetString("EMAIL", userEmail);
        PlayerPrefs.SetString("PASSWORD", userPassword);
        logInPanel.SetActive(false);
        GeneralManager.LoadNewScene();
    }

    private void OnLoginFailure(PlayFabError error)
    {
        wrongCredentials.SetActive(true);
        registeredSuccess.SetActive(false);
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        registrationFailPanel.SetActive(false);
        registerPanel.SetActive(false);
        logInPanel.SetActive(true);
        registeredSuccess.SetActive(true);
        logInPanel.transform.GetChild(1).GetComponent<InputField>().text = userEmail;
        logInPanel.transform.GetChild(2).GetComponent<InputField>().text = userPassword;
        Debug.Log("register success");
        PlayerPrefs.SetString("EMAIL", userEmail);
        PlayerPrefs.SetString("PASSWORD", userPassword);
    }

    private void OnRegisterFailure(PlayFabError error)
    {
        registrationFailPanel.SetActive(true);
        string errorMessage = error.GenerateErrorReport();
        errorMessage = errorMessage.Substring(errorMessage.IndexOf('\n') + 1);
        registrationFailPanel.GetComponentInChildren<Text>().text = errorMessage;
    }

    public void GetUserEmail(string emailIn)
    {
        userEmail = emailIn;
    }
    public void GetUserPassword(string passwordIn)
    {
        userPassword = passwordIn;
    }
    public void GetUsername(string usernameIn)
    {
        username = usernameIn;
    }
    public void OnClickLogin()
    {
        var request = new LoginWithEmailAddressRequest { Email = userEmail, Password = userPassword };
        Debug.Log(userEmail);
        Debug.Log(userPassword);
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }
    public void OnClickRegister()
    {
        registeredSuccess.SetActive(false);
        wrongCredentials.SetActive(false);
        registrationFailPanel.SetActive(false);
        var registerRequest = new RegisterPlayFabUserRequest { Email = userEmail, Password = userPassword, Username = username };
        PlayFabClientAPI.RegisterPlayFabUser(registerRequest, OnRegisterSuccess, OnRegisterFailure);
    }
    public void ActivateRegister()
    {
        wrongCredentials.SetActive(false);
        logInPanel.SetActive(false);
        registerPanel.SetActive(true);
    }

    static public void SetStats()
    {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate{ StatisticName = "score", Value = GeneralManager.GetScore()}
            }
        },
        result => { Debug.Log("User statistics updated"); },
        error => { Debug.LogError(error.GenerateErrorReport()); });
    }

    static public void GetStats()
    {
        PlayFabClientAPI.GetPlayerStatistics(new GetPlayerStatisticsRequest(),
            OnGetStatistics,
            error => Debug.LogError("Statics couldn't be loaded!"));
    }
    static void OnGetStatistics(GetPlayerStatisticsResult getPlayerStatisticsResult)
    {
        Debug.Log(getPlayerStatisticsResult.Statistics[0].Value);
        GeneralManager.SetHighScore(getPlayerStatisticsResult.Statistics[0].Value);
    }
}
