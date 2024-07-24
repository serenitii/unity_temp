using System.Collections;
using DG.Tweening;
using Jois;
using UnityEngine;

namespace PhotoAr
{
    public class MainMenuScene : MonoBehaviour
    {
        IEnumerator Start()
        {
            if (APP.States.HasFirstTimeIntro == false)
            {
                if (PlayerPrefs.HasKey(Consts.PrefKey_TermsOfUseDone) == false)
                {
                    UiManager.Current.OpenWindow((int)PageViews.TermsOfUse);
                    yield break;
                }

                APP.States.HasFirstTimeIntro = true;

                UiManager.Current.OpenWindow((int) PageViews.LogoIntro);

                DOVirtual.DelayedCall(1.8F, () =>
                {
                    // 
                    UiManager.Current.OpenWindow((int) PageViews.MainMenu);
                });
            }
            else
            {
                UiManager.Current.OpenWindow((int) PageViews.MainMenu);
            }

            yield return null;
            // // 카메라 권한 허용을 처음부터 물어보기 위함
            // GameObject.Find(Consts.ArSessionOriginPath).SetActive(false);
            // GameObject.Find(Consts.ArSessionPath).SetActive(false);
        } 
    }
}