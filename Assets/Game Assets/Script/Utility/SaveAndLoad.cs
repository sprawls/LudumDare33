using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveAndLoad {

    public static List<GameSave> savedGames = new List<GameSave>();

    public static void Save() {

        if (GameSave.current == null) Load(); //create an empty file if none
        SaveCurrentInSavedGames(); //Add Save at correct spot in saved games

        //Debug.Log("Saving at Destination : " + Application.persistentDataPath); //Debug Save Path
        
        //Sava data on disk
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create (Application.persistentDataPath + "/entro.py"); //you can call it anything you want
        bf.Serialize(file, SaveAndLoad.savedGames);
        file.Close();
       
    }

    static void SaveCurrentInSavedGames() {
        if (savedGames.Count > GameSave.current.index) {
            savedGames[GameSave.current.index] = GameSave.current;
        } else {
            GameSave.current.index = savedGames.Count;
            SaveAndLoad.savedGames.Add(GameSave.current);
        }
    }
     
    public static void Load() {

        Debug.Log("Loading at Destination : " + Application.persistentDataPath); //Debug Save Path

        if(File.Exists(Application.persistentDataPath + "/entro.py")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/entro.py", FileMode.Open);
            SaveAndLoad.savedGames = (List<GameSave>)bf.Deserialize(file);
            GameSave.current = savedGames[0];
            file.Close();
        } else {
            //Create a Fresh save file
            savedGames = new List<GameSave>();
            savedGames.Add(new GameSave(true));
            GameSave.current = savedGames[0];
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/entro.py"); //you can call it anything you want
            bf.Serialize(file, SaveAndLoad.savedGames);
            file.Close();
        }
    }

    public static void DeleteSaves() {
        if (File.Exists(Application.persistentDataPath + "/entro.py")) {
            File.Delete(Application.persistentDataPath + "/entro.py");
        }
        savedGames = new List<GameSave>();
        savedGames.Add(new GameSave(true));
        GameSave.current = savedGames[0];
        LevelManager.Instance.UpdatePersistentData(GameSave.current); //Update game with empty
    }




}
