// using System;
// using System.Threading.Tasks;

using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Jois
{
    public class AuthData
    {
        public string AccessToken;
        public string Uid;
        public string UserName;
        public string PictureUrl;
        public Texture ProfileTexture;
        public bool IsAutoLogIn;

        public bool IsLoggedIn => string.IsNullOrEmpty(Uid) == false || string.IsNullOrEmpty(UserName) == false;

        public AuthData()
        {
            Uid = string.Empty;
            UserName = string.Empty;
            PictureUrl = string.Empty;
        }
    }

    public class AuthManager : MonoBehaviour
    {
        [SerializeField] private AuthFacebook _facebook;

        private string _authType;

        public const string AUTH_FACEBOOK = "FACEBOOK";
        private const string PREFS_AUTH_TYPE = "AUTH_TYPE";

        private AuthFacebook _currentAuth;


        public void SetAuthType(string authType)
        {
        }

        public async UniTask<AuthData> AutoLogInAsync()
        {
            if (PlayerPrefs.HasKey(PREFS_AUTH_TYPE))
            {
                var authType = PlayerPrefs.GetString(PREFS_AUTH_TYPE);
                if (authType == AUTH_FACEBOOK)
                {
                    _currentAuth = _facebook;
                    return await _currentAuth.AuthLogInAsync();
                }
            }

            return new AuthData
            {
                IsAutoLogIn = true
            };
        }

        public async UniTask<AuthData> LogInAsync()
        {
            _currentAuth = _facebook;
            _currentAuth.gameObject.SetActive(true);

            UniTask.NextFrame();

            //await UniTask.Delay(TimeSpan.FromSeconds(0.1F));

            var authResult = await _currentAuth.LogInAsync();

            return authResult;
        }
    }
}