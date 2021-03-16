using System;
using System.Collections.Generic;
using UnityEngine;

namespace SengokuWarrior
{
    [Serializable]
    public class CharacterInfoStorage
    {
        public List<Save.SavedCharacter> SavedCharacters = new List<Save.SavedCharacter>();
        public static void Load()
        {

        }


        public Save.SavedCharacter GetData(int id)
        {
              Save.SavedCharacter chara = SavedCharacters.Find(item =>item.id == id);
              return chara;  
        }

        public void RegisterData(Character chara)
        {
            int foundIndex = SavedCharacters.FindIndex(item => item.id == chara.id);

            if (foundIndex != -1)
            {
                SavedCharacters[foundIndex] = new Save.SavedCharacter(chara);
            }
            else
            {
                SavedCharacters.Add(new Save.SavedCharacter(chara));
            }
        }


    }
}
