using System.Collections.Generic;
using System.Linq;
using Jois;
using UnityEngine;
using UnityEngine.UI;

namespace PhotoAr
{
    public class MyCollectionDetailPage : BaseWindow
    {
        [SerializeField] private Button _exitButton;
        [SerializeField] private List<MyCollectionCardItem> _items;
        [SerializeField] private RectTransform _scrollContent;
        
        private void Start()
        {
            InitUIs();
        }

        void InitUIs()
        {
            _exitButton.onClick.AddListener(() =>
            {
                //
                UiManager.Current.PopWindow();
            });
        }


        public override void OnUIViewShown(object param)
        {
            // 팩키지 네임으로 다운로드 받은 카드 동영상이 있는지 체크해서 보여줌 
            string packageName = param as string;

            LoadTextures(packageName);

            UpdateUI(packageName);
        }

        void LoadTextures(string packageName)
        {
            // 스크롤 초기화 
            _scrollContent.anchoredPosition = new Vector3(_scrollContent.anchoredPosition.x, 0, 0);
            
            var package = APP.Data.GetPackage(packageName);

            List<string> cardNames = new List<string>(16);
            var cardTextures = APP.Data.CardTextures;
            foreach (var card in package.cards)
            {
                if (APP.Data.ExistsCardVideoInLocal(package, card.name))
                {
                    if (cardTextures.ContainsKey(card.name) == false)
                    {
                        APP.Data.LoadCardTexture(packageName, card.name);
                    }
                }
            }
        }

        void UpdateUI(string packageName)
        {
            var package = APP.Data.GetPackage(packageName);

            List<string> cardNames = new List<string>(16);
            var cardTextures = APP.Data.CardTextures;
            int index = 0;
            foreach (var card in package.cards)
            {
                if (APP.Data.ExistsCardVideoInLocal(package, card.name))
                {
                    _items[index].SetData(package.videoShape, card.name);
                    ++index;
                }
            }

            for (int i = index; i < _items.Count; ++i)
                _items[i].gameObject.SetActive(false);
        }
    }
}