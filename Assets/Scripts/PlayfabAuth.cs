using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine.UI;
using TMPro;

namespace GetCloser.Network
{
    public class PlayfabAuth : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField login_User;
        [SerializeField]
        private TMP_InputField login_Pass;

        [SerializeField]
        private TMP_InputField Register_User;
        [SerializeField]
        private TMP_InputField Register_Pass;
        [SerializeField]
        private TMP_InputField Register_Email;

        [SerializeField]
        private Transform successfulRegistry;

        [SerializeField]
        private Transform RegistrationObjs;

        private string PlayerIdCache = "";

        

        public void PlayfabLogin()
        {
            Debug.Log("Playfab authenticating using custom ID");
            //LoginWithCustomIDRequest request = new LoginWithCustomIDRequest();
            //request.CreateAccount = true;
            //request.CustomId = 1.ToString();
            //PlayFabClientAPI.LoginWithCustomID(request, RequestToken, OnError);
            LoginWithPlayFabRequest request = new LoginWithPlayFabRequest();
            request.Username = login_User.text;
            request.Password = login_Pass.text;

            PlayFabClientAPI.LoginWithPlayFab(request, RequestToken, OnError);
            PhotonNetwork.LocalPlayer.NickName = login_User.text;
        }

        public void PlayfabRegister()
        {
            Debug.Log("Playfab authenticating using custom ID");
            //LoginWithCustomIDRequest request = new LoginWithCustomIDRequest();
            //request.CreateAccount = true;
            //request.CustomId = 1.ToString();
            //PlayFabClientAPI.LoginWithCustomID(request, RequestToken, OnError);
            RegisterPlayFabUserRequest request = new RegisterPlayFabUserRequest();
            request.Username = Register_User.text;
            request.Password = Register_Pass.text;
            request.Email = Register_Email.text;

            PlayFabClientAPI.RegisterPlayFabUser(request,result=> { Debug.Log("Account Made"); ManageLoginPanels(); } , OnError);
            
        }

        private void ManageLoginPanels()
        {
            successfulRegistry.gameObject.SetActive(true);
            RegistrationObjs.gameObject.SetActive(false);
        }


        void RequestToken(LoginResult result)
        {
            Debug.Log("PlayFab authenticated. Requesting photon token...");
            PlayerIdCache = result.PlayFabId;
            GetPhotonAuthenticationTokenRequest request = new GetPhotonAuthenticationTokenRequest();
            request.PhotonApplicationId = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime; 

            PlayFabClientAPI.GetPhotonAuthenticationToken(request,AuthWithPhoton,OnError);
        }

        void AuthWithPhoton(GetPhotonAuthenticationTokenResult result)
        {
            Debug.Log("Photon token acquired: " + result.PhotonCustomAuthenticationToken + "  Authentication complete.");
            var customAuth = new AuthenticationValues { AuthType = CustomAuthenticationType.Custom };

            customAuth.AddAuthParameter("username", PlayerIdCache);
            customAuth.AddAuthParameter("token", result.PhotonCustomAuthenticationToken);
            PhotonNetwork.AuthValues = customAuth;
            PhotonManager.ConnectToPhoton();
        }

        void OnError(PlayFabError error)
        {
            Debug.LogError($"[ERROR]| {error.GenerateErrorReport()}");
        }

        public void Quit()
        {
            Application.Quit();
        }

    }
}
