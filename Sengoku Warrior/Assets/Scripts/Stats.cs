using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SengokuWarrior
{
    [System.Serializable]
    public class Stats
    {

        public enum GearSlots
        {
            Helmet,
            Plate,
            Legs,
            LeftArm,
            RightArm,
            Ring
        }

        public enum WeaponType
        {
            Sword,
            Staff,
            Bow
        }
        public enum StatType
        {
            HP,
            MP,
            Attack,
            Defence,
            Speed
        }

        protected string characterName = "Character";
        [SerializeField]
        protected int level;
        [SerializeField]
        protected Color overlayColor = Color.white;
        [SerializeField]
        protected Gear gear = new Gear();

        [SerializeField]
        protected float baseHp = 100;
        protected float calculatedHp;
        protected float currentHp = 100;
        [SerializeField]
        protected float baseMp = 100;
        protected float calculatedMp;
        protected float currentMp = 100;
        [SerializeField]
        protected float baseDef = 25;
        protected float calculatedDef;
        [SerializeField]
        protected float baseAttack = 25;
        protected float calculatedAttack;

        [SerializeField]
        protected float baseSpeed = 25;
        protected float calculatedSpeed;

        [NonSerialized]
        public bool isGuarding = false;
        [NonSerialized]
        public int movementPoints = 5;

        private float guardModifire = 5;

        public List<int> CanEquipWeapons = new List<int>();

        public int attackRange
        {
            get
            {
                int range = 1;
                if (Gear.items[(int)GearSlots.RightArm] != null)
                {
                    WeaponAttribute attr = Gear.items[(int)GearSlots.RightArm].GetAttribute<WeaponAttribute>();
                    if (attr != null) range = attr.range;

                        else if (Gear.items[(int)GearSlots.LeftArm] != null)
                        {
                        attr = Gear.items[(int)GearSlots.LeftArm].GetAttribute<WeaponAttribute>();
                        if (attr != null) range = attr.range;
                    }
                }
                else if (Gear.items[(int)GearSlots.LeftArm] != null)
                {
                    WeaponAttribute attr = Gear.items[(int)GearSlots.LeftArm].GetAttribute<WeaponAttribute>();
                    if (attr != null) range = attr.range;
                }

                return range;
            }
        }


        public int TurnsMade = 0;
        [NonSerialized]
        private Character parent;

        public Stats() { }
        public Stats(string test) { this.characterName = test; }
        public Stats(string charaname, int level, float hp, float mp, float attack, float def, float speed, Gear gear, List<int> equipables, Character parent)
        {
            this.characterName = charaname;
            this.level = level;
            this.baseHp = hp;
            this.baseMp = mp;
            this.baseDef = def;
            this.baseAttack = attack;
            this.baseSpeed = speed;
            this.Gear = gear;
            this.CanEquipWeapons = equipables;
            this.parent = parent;

            CalculateStats();
        }

        public void SetCurrentHP(float hp)
        {
            
            this.currentHp = Mathf.Min(hp, calculatedHp);
            this.currentHp = Mathf.Max(0, hp);
        }
        public void SetCurrentMP(float mp)
        {
            this.currentMp = Mathf.Min(mp, calculatedMp);
            this.currentMp = Mathf.Max(0, mp);
        }

        public void ResetHPandMP()
        {
            currentHp =calculatedHp;
            currentMp =calculatedMp;
        }
        public void AddHealth(float damage)
        {
            //Heal if Alive
            if (this.currentHp > 0)
            {
                this.currentHp += damage;

                this.currentHp = Mathf.Min(this.currentHp, calculatedHp);
            }
        }

        public void DepleteMP(float amount)
        {
            this.currentMp -= amount;

            this.currentMp = Mathf.Max(0, this.currentMp);
            this.currentMp = Mathf.Min(this.currentMp, calculatedMp);

        }

        float BonusFromGear(Stats.StatType _t)
        {
            float bonus = 0;

            foreach (NewItem itm in Gear.items)
            {
                if (itm)
                {
                    StatsAttribute attr = itm.GetAttribute<StatsAttribute>();
                    if (attr)
                    {
                        if (attr.enabled[(int)_t])
                        {
                            bonus += attr.stats[(int)_t];
                        }
                    }
                }
            }

            return bonus;
        }


        public void CalculateStats()
        {
            //Add all armour values to defence
            calculatedDef = baseDef + BonusFromGear(StatType.Defence);
            //Increase attack value based on weapon
            calculatedAttack = baseAttack + BonusFromGear(StatType.Attack);

            //Since we for sake of this project we never change HP or MP we use base values
            calculatedHp = baseHp + BonusFromGear(StatType.HP);
            calculatedMp = baseMp + BonusFromGear(StatType.MP);

            calculatedSpeed = baseSpeed + BonusFromGear(StatType.Speed);
        }
        public void ResetHealth()
        {
            currentHp = calculatedHp;
            currentMp = calculatedMp;
        }

        public float DamageTo(Stats target)
        {
            float damage = this.calculatedAttack;
            damage -= target.calculatedDef;

            if (target.isGuarding) damage -= guardModifire;

            damage = Mathf.Max(1, damage);
            return damage;
        }

        public float HPLeft(int damage)
        {
            float hp = this.CurrentHP;
            hp -= damage;

            return Mathf.Max(0, hp);
        }
        public float HPAfterAttack(Stats target)
        {
            float hp = target.CurrentHP;
            float damage = DamageTo(target);

            hp -= damage;

            return Mathf.Max(0, hp);

        }

        public string CharacterName
        {
            get
            {
                return characterName;
            }
            set
            {
                characterName = value;
            }
        }

        public int Level
        {
            get
            {
                return level;
            }
        }

        public float CurrentHP
        {
            get
            {
                return currentHp;
            }
        }

        public float CalculatedHP
        {
            get
            {
                return calculatedHp;
            }
        }
        public float CurrentMP
        {
            get
            {
                return currentMp;
            }
        }
        public float CalculatedMP
        {
            get
            {
                return calculatedMp;
            }
        }

        public float CalculatedDef
        {
            get
            {
                return calculatedDef;
            }
        }
        public float CalculatedAtt
        {
            get
            {
                return calculatedAttack;
            }
        }
        public float CalculatedSpeed
        {
            get
            {
                return calculatedSpeed;
            }
        }


        public Color OverlayColor
        {
            get
            {
                return overlayColor;
            }

            set
            {
                overlayColor = value;
            }
        }

        public float BaseDef
        {
            get
            {
                return baseDef;
            }

            set
            {
                baseDef = value;
            }
        }

        public float BaseAttack
        {
            get
            {
                return baseAttack;
            }

            set
            {
                baseAttack = value;
            }
        }
        public float BaseHP
        {
            get
            {
                return baseHp;
            }
            set
            {
                baseHp = value;
            }
        }
        public float BaseMP
        {
            get
            {
                return baseMp;
            }
            set
            {
                baseMp = value;
            }
        }
        public float BaseSpeed
        {
            get
            {
                return baseSpeed;
            }
            set
            {
                baseSpeed = value;
            }

        }

        public Gear Gear
        {
            get
            {
                return gear;
            }
            set
            {
                gear = value;
            }

        }
    }


}
