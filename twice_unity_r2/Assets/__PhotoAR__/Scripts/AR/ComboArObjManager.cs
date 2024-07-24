using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PhotoAr;
using UnityEngine;

namespace PhotoAr
{
    public class ComboArObjManager : MonoBehaviour
    {
        private Dictionary<string, ArPlaceContainer> _arObjs = new Dictionary<string, ArPlaceContainer>();

        private static ComboArObjManager _current;
        public static ComboArObjManager Current => _current;

        public Dictionary<string, ArPlaceContainer> ArObjs => _arObjs;

        private ComboData _currentCombo;

        private bool _comboEnabled;

        public bool ComboEnabled => _comboEnabled;

        public Dictionary<string, int> _objectsRightSide = new Dictionary<string, int>();
        
        void Start()
        {
            _current = this;
            APP.ComboArMgr = this;
        }

        void FixedUpdate()
        {
            if (_arObjs.Count < 2)
                return;

            switch (_arObjs.Count)
            {
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                {
                    Check2Combos();
                    break;
                }
                // case 6:
                // {
                //     Check6Combos();
                //     break;
                // }
            }
            
        }

        // 로드된 팩키지에서의 카드를 추출한다 
        List<ArPlaceContainer> FindComboObjectsInLoadedPackages(Dictionary<string, ArPlaceContainer> enabledObjs)
        {
            foreach (var packageName in APP.Data.EnabledArPackages)
            {
                var package = APP.Data.GetPackage(packageName);
                // Debug.LogFormat("p({0}) c({1}) ", packageName, package.combos);
                if (package.combos == null || package.combos.Count == 0)
                    continue;

                foreach (var combo in package.combos)
                {
                    bool isFit = true;
                    
                    foreach (var item in combo.cards)
                    {
                        if (enabledObjs.ContainsKey(item) == false)
                        {
                            isFit = false;
                            break;
                        } 
                    }
                    
                    if (isFit)
                    {
                        _currentCombo = combo;
                        var objs = new List<ArPlaceContainer>();
                        foreach (var item in combo.cards)
                        {
                            objs.Add(enabledObjs[item]);
                        }
                        _comboVideoFile = combo.file;
                        return objs; 
                    }
                }
            }

            return null;     
        }
        
        // 같은 팩키지에서의 카드를 추출한다 (Not Use) 
        List<ArPlaceContainer> FindComboObjectsInSamePackages(Dictionary<string, ArPlaceContainer> enabledObjs)
        {
            foreach (var packageName in APP.Data.EnabledArPackages)
            {
                var package = APP.Data.GetPackage(packageName);
                if (package.combos == null || package.combos.Count == 0)
                    continue;

                foreach (var combo in package.combos)
                {
                    bool isFit = true;
                    foreach (var item in combo.cards)
                    {
                        if (enabledObjs.ContainsKey(item) == false)
                        {
                            isFit = false;
                            break;
                        } 
                    }

                    if (isFit)
                    {
                        _currentCombo = combo;
                        var objs = new List<ArPlaceContainer>(); // {_arObjs[combo.cards[0]], _arObjs[combo.cards[1]]};
                        foreach (var item in combo.cards)
                        {
                            objs.Add(enabledObjs[item]);
                        }
                        _comboVideoFile = combo.file;
                        return objs; 
                    }
                }
            }

            return null;     
        }

        private int _checkComboCounter;
        private int _checkDecoupleCounter;
        public string _debugText = "";
        // public int _enabledComboCount = 0;
        private string _comboVideoFile = "";
        // public string _enabledComboName = "";
        public List<string> _enabledTargetCardNames = new List<string>();
        
        string GetAnchorCardName(List<ArPlaceContainer> list)
        {
            switch (list.Count)
            {
                case 3:
                    return list[1].CardName;
                case 4:
                case 5:
                case 6:
                    return list[3].CardName;
                default:
                    return list[0].CardName;
            }
        }
        void Check2Combos() // int checkCount)
        {
            var vList = FindComboObjectsInLoadedPackages(_arObjs);
            if (vList == null)
            {
                _comboEnabled = false;
                _checkComboCounter = 0;
                return;
            }

            if (_comboEnabled && _enabledTargetCardNames.Count != vList.Count)
            {
                _comboEnabled = false;
                _checkComboCounter = 0;
                foreach (var item in vList)
                {
                    item.OnComboLost();
                } 
            }

            float minDistX = vList[0].CardSize.x * 1.06F;
            float minDistY = vList[0].CardSize.y * 1.06F;
            int checkCount = vList.Count;
            
            // Debug.LogFormat("Check2Combos count({0}) ({1})", vList.Count, string.Join(", ", vList));
            var distX1 = Vector3.Distance(vList[0].transform.position, vList[1].transform.position);
            var distX2 = 0F;
            var distY1 = 0F;
            var dist_3_6 = 0F;
            if (checkCount != 2)
             distX2 = Vector3.Distance(vList[1].transform.position, vList[2].transform.position);
            
            var unified = checkCount == 2 ? distX1 < minDistX : distX1 < minDistX && distX2 < minDistX;
            if (unified && checkCount >= 4)
            {
                distY1 =Vector3.Distance(vList[0].transform.position, vList[3].transform.position);
                dist_3_6 = Vector3.Distance(vList[2].transform.position, vList[5].transform.position);
                unified = distY1 < minDistY && dist_3_6 < minDistY;
            } 
            
            _debugText = string.Format(">> ({4}) dist1( {0} ) dist2( {1} ) minDistX({2}) minDistY({3}) 2nd({5})",
                distX1, distX2, minDistX, minDistY, _comboEnabled, string.Join(",", _objectsRightSide.Keys));
            if (!_comboEnabled)
            {
                // 결합조건
                _checkComboCounter = unified ? ++_checkComboCounter : 0;
                if (_objectsRightSide.Keys.Count == 0)
                    unified = false;

                if (unified && _checkComboCounter > 5)
                {
                    _comboEnabled = true;
                    _checkDecoupleCounter = 0;
                    
                    Debug.LogFormat("- combo is enabling distance({0}) minDistance({1}) ({2}) ", 
                        distX1, minDistX, _comboVideoFile);

                    _enabledTargetCardNames = new List<string>();
                    foreach (var item in vList)
                    {
                        _enabledTargetCardNames.Add(item.CardName);
                    }

                    var anchorCardName = vList.Count == 3 ? vList[1].CardName : vList[0].CardName; // GetAnchorCardName(vList); // checkCount == 2 ? vList[0].CardName : vList[1].CardName;
                    Debug.LogFormat("- OnComboFound ({0}) [{1}] ", anchorCardName, string.Join(", ", _enabledTargetCardNames));
                    foreach (var item in vList)
                    {
                        item.OnComboFound(anchorCardName, _enabledTargetCardNames, _comboVideoFile, checkCount); 
                    }
                }
            }
            else
            {
                // 분해조건

                if (!unified) // && ++_checkDecoupleCounter > 5) // maxDistance)
                {
                    _comboEnabled = false;
                    _checkComboCounter = 0;
                    foreach (var item in vList)
                    {
                        item.OnComboLost();
                    }
                    Debug.LogFormat("combo is disabled");
                }
                else
                    _checkDecoupleCounter = 0;
            }
        }

        public void AddArObj(ArPlaceContainer obj)
        {
            Debug.LogFormat(">> AddArObj ( {0} ) count( {1} ) ", obj.CardName, _arObjs.Count + 1);
            _arObjs.Add(obj.CardName, obj);
        }

        public void RemoveArObj(ArPlaceContainer obj)
        {
            Debug.LogFormat(">> RemoveArObj ( {0} ) count({1}) ", obj.CardName, _arObjs.Count - 1);
            _arObjs.Remove(obj.CardName);
            if (_enabledTargetCardNames.Contains(obj.CardName))
            {
                _comboEnabled = false;
                _checkComboCounter = 0; 
            }
        }
    }
}