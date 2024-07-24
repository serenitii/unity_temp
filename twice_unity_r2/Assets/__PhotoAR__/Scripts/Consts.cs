namespace PhotoAr
{
    public enum PageViews
    {
        Unknown,
        MainMenu,
        Settings,
        ArShop,
        MyCollection,
        LogoIntro,
        ArShopPackageInfo,
        TermsOfUse,
        WarningsOfAR,
        DividedDownload,
        MyCollectionDetail,
        PopupBasic,
        PopupSerialNumberInput,
        PopupBasicYesNo,
        ArShopIPad,
        BeyondLive
        
        // PopupInvalidSerialNumber,
        // PopupTooManySerialNumber
    }
    
    public enum ArViews
    {
        None,
        ArPage,
        FilterPage
    }
    
    public static class Consts
    {
        public const string ArSessionOriginPath = "/AR Session Origin";
        public const string ArSessionPath = "/AR Session";
        public const string SquaredArImageSuffix = "_sq";
        public const string SquaredArImageSuffixPoint = "_sq.";
        public const string LowVideoSuffix = "_low";
        public const string VideoExt = ".mp4";

        public const int SCENE_MAIN_MENU = 0;
        public const int SCENE_AR = 1;
        
        public const int MAXARPackages = 5; // review_only_01 포함
        
        public const string PrefKey_AppDataVersion = "AppDataVersion";
        public const string PrefKey_EnabledArPackages = "EnabledPackages";
        public const string PrefKey_CardDownloads = "CardDownloads"; // 개별 다운로드(카드 단위)
        public const string PrefKey_LowQuality = "LowQuality";
        public const string PrefKey_SerialNumberCardMap = "SerialNumberCardMap";
        public const string PrefKey_TermsOfUseDone = "TermsOfUseDone";
        
        public const string CheckSerialNumber = "일련번호를 확인해 주세요";
        public const string TooManySerialNumber = "일련번호를 등록초과입니다\n고객센터에 문의 바랍니다";
        public const string AlreadyUsing = "입력하신 번호는 이미 다른 카드에 사용되었습니다";
        public const string ConfirmDownload = "<b>{0}</b> 을 다운로드하려는 것이 맞습니까?";

        public const string MagicSN = "X112233"; //; //"asdf1298";
        
        // iLLUU
        // public const string dataVersionUrl = "https://photoar1.s3.ap-northeast-2.amazonaws.com/ar_version.json";
        // public const string dataUrl = "https://photoar1.s3.ap-northeast-2.amazonaws.com/ar_data.json";
        
        // TWICE 
        // public const string appHeaderDataUrl = "https://twice-ar.s3.ap-northeast-2.amazonaws.com/ar_app_data/ar_header.json";
        // public const string appDataUrl = "https://twice-ar.s3.ap-northeast-2.amazonaws.com/ar_app_data/ar_data.json";
        public const string AppDataFileName = "ar_data.json";

        public const int LOADING_LOAD_PACKAGE_IMAGES = 999001;
        
        public const string ReviewSamplePackage = "twice_02";

        public const int FirstArContentIndex = 1;
    }
}