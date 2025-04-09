using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;



public static class LocalDataManager
{
    #region RankingData
    
    #if UNITY_EDITOR
    static readonly string path_dataRoot = Path.Combine(Application.dataPath, "99_SavedData");
    #else 
    static readonly string path_dataRoot = Path.Combine(Application.persistentDataPath, "99_SavedData");
    #endif
    static readonly string path_rankingData = Path.Combine(path_dataRoot, "0_Ranking.txt");

    //==========================
    // 저장된 캐시를 불러와 T타입의 데이터로 변환한다.
    //=========================
    public static T LoadJson<T>(string path)
    {
        T ret = default;

        CheckDataPath(path);
        string data = File.ReadAllText(path);  
        try
        {
            if (!string.IsNullOrEmpty(data))
            {
                ret = JsonUtility.FromJson<T>(data);
                Debug.LogWarning($" 길이:{data.Length }, 내용:{data}");
            }
            else
            {
                Debug.LogError($"{path}에 데이터가 없음;; ");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"JSON 파일 로드 중 오류 발생: {e.Message}\n{e.StackTrace}");
        }
        return ret;
    }

    //===========================
    // content를 json string으로 변환하여 path에 저장한다. 
    //==========================
    public static void SaveJson(string path, string json)
    {

        CheckDataPath(path);
        try
        {
            File.WriteAllText(path, json);

            // Debug.Log($" 파일 저장 완료 - {path}");
            Debug.LogWarning($"{json}");
        }
        catch (Exception e)
        {
            Debug.LogError($"파일 저장 중 오류 발생: {e.Message}\n{e.StackTrace}");
        }

    }


    public static void CheckDataPath(string path)
    { 
        // 디렉토리 경로를 추출
        string directory = Path.GetDirectoryName(path);

        // 디렉토리가 없으면 생성
        if (string.IsNullOrEmpty(directory)==false && Directory.Exists(directory)==false)
        {
            Directory.CreateDirectory(directory);
        }
        
        
        //
        if (File.Exists(path)== false)
        {
            //Debug.LogError(" 파일 찾기 오류 : " + path + " 파일이 없습니다!!!");

            File.Create(path).Dispose();
        }
        else
        {
            //Debug.LogWarning(" 파일 찾기 : " + path + " 파일을 찾았습니다. ");
        }
                  
    }

    //=====================================================================================
    public static RankingData LoadRankingData()
    {
        RankingData rankingData =LoadJson<RankingData>(path_rankingData); 
        if(rankingData ==null)
        {
            rankingData = new();
        }
        rankingData.CheckIntegrity();


        return rankingData;
    }
    
    
    public static void SaveClearedGamePlayInfo(CurrGamePlayInfo currGamePlayInfo)
    {
        if( currGamePlayInfo.cleared == false)
        {
            return;
        }
        
        //
        RankingData currRankingData = LoadRankingData();
        Difficulty currDifficulty = currGamePlayInfo.currDifficulty;



        if (currDifficulty == Difficulty.Easy)
        {
            currRankingData.list_easy.Add(currGamePlayInfo);
            currRankingData.list_easy = currRankingData.list_easy
                .OrderByDescending(x => x.towerHp)
                .ThenByDescending(x => x.killCount)
                .ThenByDescending(x =>x.clearDate)    // 최후 정렬 순서 - 클리어 날짜
                .Take(5)                   // 상위 5개만
                .ToList();
        
        }
        else if (currDifficulty == Difficulty.Normal)
        {
            currRankingData.list_normal.Add(currGamePlayInfo);
            currRankingData.list_normal = currRankingData.list_normal
                .OrderByDescending(x => x.towerHp)
                .ThenByDescending(x => x.killCount)
                .ThenByDescending(x =>x.clearDate)    // 최후 정렬 순서 - 클리어 날짜
                .Take(5)                   // 상위 5개만
                .ToList();
        }
        else if( currDifficulty ==Difficulty.Hard )
        {
            currRankingData.list_hard.Add(currGamePlayInfo);
            currRankingData.list_hard = currRankingData.list_hard
                .OrderByDescending(x => x.towerHp)
                .ThenByDescending(x => x.killCount)
                .ThenByDescending(x =>x.clearDate)    // 최후 정렬 순서 - 클리어 날짜
                .Take(5)                   // 상위 5개만
                .ToList();
        }



        SaveRankingData(currRankingData);
    }

    public static void SaveRankingData(RankingData rankingData)
    {
        
        
        string json = JsonUtility.ToJson(rankingData, true); // true는 들여쓰기(Pretty Print)
        SaveJson(path_rankingData, json);
    }

    #endregion



    //========================================================================================
    #region PlayerPrefs

    static readonly float defualtVolume  = 8f;
    static readonly float defualtSense  = 1f;
    static readonly string fieldName_master = "master"; 
    static readonly string fieldName_bgm = "bgm";
    static readonly string fieldName_sfx = "sfx";
    static readonly string fieldName_mouseSense = "mouseSense";
    

    // Get
    public static float GetMaster()
    {
        float ret = PlayerPrefs.GetFloat(fieldName_master, defualtVolume);

        return ret;
    }
    public static float GetBgm()
    {
        float ret = PlayerPrefs.GetFloat(fieldName_bgm, defualtVolume);

        return ret;
    }

    public static float GetSfx()
    {
        float ret = PlayerPrefs.GetFloat(fieldName_sfx, defualtVolume);

        return ret;
    }

    public static float GetMouseSense()
    {
        float ret = PlayerPrefs.GetFloat(fieldName_mouseSense, 1f);

        return ret;
    }
    
    public static void SetMaster(float value)
    {
        PlayerPrefs.SetFloat(fieldName_master, value);
        PlayerPrefs.Save();
    }

    public static void SetBgm(float value)
    {
        PlayerPrefs.SetFloat(fieldName_bgm, value);
        PlayerPrefs.Save();
    }

    public static void SetSfx(float value)
    {
        PlayerPrefs.SetFloat(fieldName_sfx, value);
        PlayerPrefs.Save();
    }

    public static void SetSense(float value)
    {
        PlayerPrefs.SetFloat(fieldName_mouseSense, value);
        PlayerPrefs.Save();
    }


    #endregion
}
