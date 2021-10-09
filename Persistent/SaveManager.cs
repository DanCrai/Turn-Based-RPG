

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

static public class SaveManager
{

    static private SaveFile saveFile;
    static private string filePath;

    static public bool LOCK
    {
        get;
        private set;
    }


    static SaveManager()
    {
        LOCK = false;
        filePath = Application.persistentDataPath + "/saveFile1.save";

        saveFile = new SaveFile();
    }


    static public void Save()
    {
        // If this is LOCKed, don't save
        if (LOCK) return;
        Debug.Log(filePath);
        CombatStateMachine[] csms = GameObject.FindGameObjectWithTag("Party").GetComponentsInChildren<CombatStateMachine>();
        UnitSaveClass[] characters = new UnitSaveClass[4];
        for (int i = 0; i < 4; i++)
        {
            characters[i] = new UnitSaveClass();
            characters[i].unit = csms[i].GetUnit();
            characters[i].className = csms[i].GetUnit().GetClass().GetClassName();
            List<BaseAttack> attacks = csms[i].GetUnit().GetAttacks();
            List<BaseAttack> availableSpells = csms[i].GetUnit().GetClass().GetAvailableSpells();
            int[] indexes = new int[attacks.Count];
            int j = 0;
            foreach(BaseAttack at in attacks)
            {
                indexes[j++] = availableSpells.IndexOf(at);
            }
            characters[i].attacksIndexes = indexes;
        }
        saveFile.characters = characters;
        saveFile.level = GeneralManager.GetLevelCounter();
        saveFile.scenario = GeneralManager.GetScenario();

        string jsonSaveFile = JsonUtility.ToJson(saveFile, true);

        File.WriteAllText(filePath, jsonSaveFile);

    }


    static public void Load()
    {
        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);

            try
            {
                saveFile = JsonUtility.FromJson<SaveFile>(dataAsJson);
            }
            catch
            {
                Debug.LogWarning("SaveGameManager:Load() – SaveFile was malformed.\n" + dataAsJson);
                return;
            }

            LOCK = true;
            // Load the Achievements
            GeneralManager.SetLevelCounter(saveFile.level);
            GeneralManager.SetScenario(saveFile.scenario);
            Debug.Log(saveFile.characters[0].className);
            
            CombatStateMachine[] csms = GameObject.FindGameObjectWithTag("Party").GetComponentsInChildren<CombatStateMachine>();
            for (int counter = 0; counter < 4; counter++)
            {
                Object g = Resources.Load(saveFile.characters[counter].className);
                BaseClass baseClass = (((GameObject)g).GetComponent<BaseClass>());
                baseClass.SelectClass(csms[counter]);
                Unit unit = new Unit(saveFile.characters[counter].unit);
                unit.SetClassFromSave(baseClass);
                List<BaseAttack> attacks = baseClass.GetAvailableSpells();
                for (int i = 0; i < saveFile.characters[counter].attacksIndexes.Length; i++)
                {
                    unit.AddAttack(attacks[saveFile.characters[counter].attacksIndexes[i]]);
                }
                Debug.Log(unit.GetAttacks().Count);
                csms[counter].SetUnit(unit);
            }
            LOCK = false;
            GeneralManager.FinishedLoading();
        }
        /*else
        {
            LOCK = true;


            LOCK = false;
        }*/
    }


    static public void DeleteSave()
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            saveFile = new SaveFile();
            Debug.Log("SaveGameManager:DeleteSave() – Successfully deleted save file.");
        }
        else
        {
            Debug.LogWarning("SaveGameManager:DeleteSave() – Unable to find and delete save file!"
                + " This is absolutely fine if you've never saved or have just deleted the file.");
        }

        // Lock the file to prevent any saving
        LOCK = true;


        // Unlock the file
        LOCK = false;
    }


}



//[System.Serializable]
public class SaveFile
{
    public UnitSaveClass[] characters;
    public int highScore = 5000;
    public int level = 0;
    public int scenario = -1;
}

[System.Serializable]
public class UnitSaveClass
{
    public UnitSaveClass()
    {
        unit = new Unit();
        className = "";
        attacksIndexes = null;
    }
    public override string ToString()
    {
        return "Unit: + " + unit.GetUnitName() + className;
    }
    [SerializeField]
    public Unit unit;
    [SerializeField]
    public string className;
    [SerializeField]
    public int[] attacksIndexes;
}