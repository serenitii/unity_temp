using System;
using System.Collections.Generic;
using UnityEngine;

namespace PhotoAr
{
    [Serializable]
    public class AppHeaderData
    {
        public int version; // 버전
        public string server; // 서버 주소
        public string content; // ar_data.json URL 주소
        
        public bool updateAppInStore; // 휴대폰앱 강제 업데이트
        // public bool updateData; // 설정 파일 강제 업데이트
    }

    [Serializable]
    public class AppSettingsData
    {
        public string webViewUrl;
    }

    public enum VideoShapeType
    {
        Rectangle,
        Circle,
        Triangle
    }
    
    [Serializable]
    public class VideoShape
    {
        public int shape; // 0: rectangle, 1: circle
        public int shapeOptions;
        public float scale;
        public float offsetX;
        public float offsetY;
        public float width;
        public float height;

        public VideoShape()
        {
        }

        public VideoShape(VideoShape rhs)
        {
            this.shape = rhs.shape;
            this.shapeOptions = rhs.shapeOptions;
            this.scale = rhs.scale;
            this.offsetX = rhs.offsetX;
            this.offsetY = rhs.offsetY;
            this.width = rhs.width;
            this.height = rhs.height;
        }

        public override string ToString()
        {
            return string.Format("shape({0}, {1}) scale({2}) offset({3}, {4}) size({5}, {6}) ",
                shape, shapeOptions, scale, offsetX, offsetY, width, height);
        }
    }

    [Serializable]
    public class CardData
    {
        public string packageName; // only on code
        public string name;
        public VideoShape videoShape;

        // combo
        public string playerName;
        public bool comboEnabled; // only on code
    }

    [Serializable]
    public class ComboData
    {
        public string file; // ???
        public List<string> cards;
    }

    [Serializable]
    public class PackageData
    {
        // public int id;
        public string name;
        public int version;
        public string updatedAt; // CMS 를 통해 업데이트한 날짜

        public bool reviewOnly;
        public string title;
        public string artist;
        public string exploreDesc;
        public string packageImage;
        public String arImageExtension;
        public string cardImageExtension;
        public bool hasLowVideos;
        public bool mute;
        public VideoShape videoShape;
        public List<CardData> cards;

        // 일련번호
        public int serialNumberMode; // 0: None, 1: Package, 2: Cards

        // 조합
        public List<ComboData> combos;
    }

    [Serializable]
    public class AppData
    {
        public AppSettingsData appSettings;
        public string storageUrl;
        public List<string> explorePackages;
        public List<string> shopPackages;
        public List<PackageData> packages;
    }

    #region Serial Number

    public enum SerialNumberMode
    {
        None,
        Package,
        Card
    }

    [Serializable]
    public class ReqCheckSerialNumberUsableDto
    {
        public string sn; // serial number
        public int pid; // package id
    }

    [Serializable]
    public class ResCheckSerialNumberUsableDto
    {
        public int usedCount;
        public string serialNumber;
        public int packageId;
        public string updateAt;
        public int serialNumberCode; // 0:usable, 1:존재하지 않음, 2: 하루 용량 초과, 3: 전체 용량 초과
    }

    [Serializable]
    public class ReqUseSerialNumberDto
    {
        public string packageName;
        public string cardName;
        public int packageId;
        public string serialNumber;
    }

    [Serializable]
    public class ResUseSerialNumberDto
    {
        public int usedCount;
        public bool hasUsed;
    }

    #endregion

    [Serializable]
    public class ReqLog
    {
        public string uid;
        public string action;
        public string param;
    }

    [Serializable]
    public class ResLog
    {
        public string msg;
    }

    // - 첫 실행 - 1st-exec
    // - json 버전업 u-js
    //    - 팩키지 다운로드 pk-dl
    //    - 팩키지 삭제 pk-del
    public enum LogActions
    {
        FirstExec
    }

    public static class LogActionNames
    {
        public const string FirstExec = "1st";
        public const string AppExec = "run";
        public const string UpdateJson = "up-js";
        public const string PackageDownload = "pk-dl";
        public const string PackageDelete = "pk-del";
        public const string SongDownload = "song-dl";
        public const string SongDelete = "song-del";
        public const string PlayAR = "play-ar";
    }
}