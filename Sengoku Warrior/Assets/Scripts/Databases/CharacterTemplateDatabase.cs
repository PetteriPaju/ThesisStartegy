using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SengokuWarrior
{
    [CreateAssetMenu(menuName = "Tactics/Items/Characte rDatabase", fileName = "CharacterDatabase")]
    public class CharacterTemplateDatabase : ScriptableObject
    {
        public List<Character> commonCharacters = new List<Character>();
        public List<Character> uniqueCharacters = new List<Character>();

        public void CreateCharacter(bool unique)
        {
            List<Character> lst = unique ? uniqueCharacters : commonCharacters;

            lst.Add(new Character());
        }

        public Character GetCharacter(Character chara)
        {
            Character newChara=null;
            int indexA = commonCharacters.IndexOf(chara);
            if (indexA != -1)
            {
                newChara = commonCharacters[indexA].Clone(false);
            }
            else
            {
                int indexB = uniqueCharacters.IndexOf(chara);

                if (indexB != -1)
                {
                    newChara = uniqueCharacters[indexB].Clone(true);
                }
            }

            return newChara;
        }

        public Character GetCharacterWithID(int id)
        {
            Character newChara = null;
            int indexA = commonCharacters.FindIndex(item => item.id == id);
            if (indexA != -1)
            {
                newChara = commonCharacters[indexA].Clone(false);
            }
            else
            {
                int indexB = uniqueCharacters.FindIndex(item => item.id == id);

                if (indexB != -1)
                {
                    newChara = uniqueCharacters[indexB].Clone(true);
                }
            }

            return newChara;
        }

    }
}
