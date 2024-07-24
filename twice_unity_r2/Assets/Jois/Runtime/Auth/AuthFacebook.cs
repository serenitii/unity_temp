using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

#if APP_USES_FACEBOOK
using Facebook.MiniJSON;
using Facebook.Unity;

#endif


namespace Jois
{
#if !APP_USES_FACEBOOK
    public class AuthFacebook : MonoBehaviour
    {
        public async UniTask<AuthData> AuthLogInAsync()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1F));
            return null;
        }

        public async UniTask<AuthData> LogInAsync()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1F));
            return null;
        }
    }
    
#else
    public class FbJsonMyInfo
    {
        public string name;
        public string id;
    }

    public class AuthFacebook : MonoBehaviour
    {
        private bool _asyncFlagInit;
        private bool _asyncFlagLogIn;
        private int _logInQueryCount;

        private AuthData _authData;

        private IResult _logInResult;

        const string QUERY_ME = "/me";
        const string QUERY_MY_PROFILE = "/me/picture?width=128&height=128";
        const string QUERY_MY_PROFILE_URL = "/me/picture?redirect=false";
        const string QUERY_FRIENDS = "/me?fields=friends.limit(1000).fields(id).field(name)";

        private const int LOGIN_QUERY_COUNT = 2;

        static readonly List<string> PERMISSIONS = new List<string>
        {
            "public_profile"
            //"public_profile", "email", "user_friends"
        };

        public async UniTask<AuthData> AuthLogInAsync()
        {
            Debug.LogFormat("[Facebook] AutoLogIn");

            if (FB.IsInitialized == false)
            {
                _asyncFlagInit = false;
                FB.Init(OnInitComplete, OnHideUnity);
                await UniTask.WaitUntil(() => _asyncFlagInit);
            } 
            
            _authData = new AuthData();
            _authData.IsAutoLogIn = true;
            
            Debug.Assert(FB.IsInitialized);

            if (FB.IsLoggedIn)
            {
                _authData.AccessToken = AccessToken.CurrentAccessToken.TokenString;
                _authData.Uid = AccessToken.CurrentAccessToken.UserId;

                Debug.LogFormat("Already Logged In - Uid({0}) AccessToken({1}) ", _authData.Uid, _authData.AccessToken);
            }

            return _authData;
        }
        
        public async UniTask<AuthData> LogInAsync()
        {
            Debug.LogFormat("[Facebook] LogIn");

            if (FB.IsInitialized == false)
            {
                _asyncFlagInit = false;
                FB.Init(OnInitComplete, OnHideUnity);
                await UniTask.WaitUntil(() => _asyncFlagInit);
            }

            _authData = new AuthData();

            Debug.Assert(FB.IsInitialized);

            if (FB.IsLoggedIn)
            {
                _authData.AccessToken = AccessToken.CurrentAccessToken.TokenString;
                _authData.Uid = AccessToken.CurrentAccessToken.UserId;

                Debug.LogFormat("Already Logged In - Uid({0}) AccessToken({1}) ", _authData.Uid, _authData.AccessToken);
            }
            else
            {
                _asyncFlagLogIn = false;
                _logInQueryCount = 0;

                FB.LogInWithReadPermissions(PERMISSIONS, OnLogInFinish);

                await UniTask.WaitUntil(() => _asyncFlagLogIn && _logInQueryCount == LOGIN_QUERY_COUNT);

                if (FB.IsLoggedIn)
                {
                    _authData.AccessToken = AccessToken.CurrentAccessToken.TokenString;
                    _authData.Uid = AccessToken.CurrentAccessToken.UserId;

                    Debug.LogFormat("Successfully Logged In: Uid({0}) UserName({1}) PictureUrl({2}) ",
                        _authData.Uid, _authData.UserName, _authData.PictureUrl);
                }
                else
                {
                    Debug.LogFormat("Failed to log in: Uid({0}) UserName({1}) PictureUrl({2}) ",
                        _authData.Uid, _authData.UserName, _authData.PictureUrl);
                }
            }

            return _authData;
        }

        // public void AutoLogIn()
        // {
        // }

        public void LogOut()
        {
            Debug.LogFormat("[Facebook] LogOut ");
            FB.LogOut();
            _authData = null;
        }

        void OnInitComplete()
        {
            Debug.LogFormat("AuthFacebook.OnInitComplete IsLoggedIn({0}), IsInitialized({1}), AccessToken({2}) \n",
                FB.IsLoggedIn, FB.IsInitialized,
                AccessToken.CurrentAccessToken == null ? string.Empty : AccessToken.CurrentAccessToken.ToString());

            _asyncFlagInit = true;

            // switch (_reservedTask)
            // {
            //     case AuthTask.None:
            //         break;
            //     case AuthTask.AutoLogIn:
            //         AutoLogInImpl(_autoLogInCallback);
            //         _autoLogInCallback = null;
            //         break;
            //     case AuthTask.LogIn:
            //         LogInImpl();
            //         break;
            //     default:
            //         throw new ArgumentOutOfRangeException();
            // }
        }

        void OnHideUnity(bool isGameShown)
        {
//            this.Status = "Success - Check log for details";
//            this.LastResponse = string.Format("Success Response: OnHideUnity Called {0}\n", isGameShown);
//            LogView.AddLog("Is game shown: " + isGameShown);
            Debug.LogFormat("isGameShown({0}) \n", isGameShown);
        }

        void OnLogInFinish(IResult result)
        {
            if (result == null)
            {
                Debug.LogError("Null Response \n");
                _asyncFlagLogIn = true;
                return;
            }

            // Some platforms return the empty string instead of null.
            if (!string.IsNullOrEmpty(result.Error))
            {
                //this.Status = "Error - Check log for details";
                //this.LastResponse = "Error Response:\n" + result.Error;
                Debug.LogErrorFormat("FB LogIn Error-{0} \n", result.Error);
            }
            else if (result.Cancelled)
            {
                //this.Status = "Cancelled - Check log for details";
                //this.LastResponse = "Cancelled Response:\n" + result.RawResult;

                Debug.LogFormat("FB Cancelled Response-{0} \n", result.RawResult);
            }
            else if (!string.IsNullOrEmpty(result.RawResult))
            {
                //this.Status = "Success - Check log for details";
                //this.LastResponse = "Success Response:\n" + result.RawResult;

                Debug.LogFormat("FB Success Response-{0} \n", result.RawResult);

                // _logInQueryCount = 0;
                // _completedQueryCount = 3;


                FB.API(QUERY_ME, HttpMethod.GET, MyInfoCallback);
                FB.API(QUERY_MY_PROFILE_URL, HttpMethod.GET, OnGetProfileUrl);
                //FB.API(QUERY_MY_PROFILE, HttpMethod.GET, OnGetProfileTexture);
                //FB.API(QUERY_FRIENDS, HttpMethod.GET, OnGetFriendsData);
            }
            else
            {
                //this.LastResponse = "Empty Response\n";
                Debug.Log("FB Empty Response \n");
            }

            _asyncFlagLogIn = true;
        }


        // 내 정보
        void MyInfoCallback(IResult result)
        {
            bool isSuccess = string.IsNullOrEmpty(result.Error) && !string.IsNullOrEmpty(result.RawResult);
            if (isSuccess)
            {
                Debug.LogFormat("MyInfoCallback-{0}\n", result.RawResult);

                var myInfo = JsonUtility.FromJson<FbJsonMyInfo>(result.RawResult);
                _authData.Uid = myInfo.id;
                _authData.UserName = myInfo.name;
            }
            else
            {
                Debug.Log("failed. HandleResultMyInfo");

                if (!string.IsNullOrEmpty(result.Error))
                {
                    Debug.Log("Failed. " + result.Error);
                }
                else if (result.Cancelled)
                {
                    Debug.Log("Cancelled. " + result.RawResult);
                }
                else if (!string.IsNullOrEmpty(result.RawResult)) // Sucess
                {
                }
                else
                {
                    Debug.Log("Failed(Empty). ");
                }
            }

            ++_logInQueryCount;
            //HandleLogInStep(isSuccess);
        }

        void OnGetProfileUrl(IGraphResult result)
        {
            Debug.LogFormat("OnGetProfileUrl RawResult: {0}\n", result.RawResult);
            bool isSuccess = string.IsNullOrEmpty(result.Error);

            if (result.ResultDictionary.TryGetValue("data", out var data))
            {
                var dict = data as Dictionary<string, object>;
                _authData.PictureUrl = dict["url"] as string;
            }

            ++_logInQueryCount;
            //HandleLogInStep(isSuccess);
        }
        
        void OnGetProfileTexture(IGraphResult result)
        {
            bool isSuccess = string.IsNullOrEmpty(result.Error) && result.Texture != null;
            if (isSuccess)
            {
                Debug.LogFormat("FB OnGetProfileTexture raw Result: {0} \n", result.RawResult);
                _authData.ProfileTexture = result.Texture;
            }

            ++_logInQueryCount;
            //HandleLogInStep(isSuccess);
        }
    }
#endif
}