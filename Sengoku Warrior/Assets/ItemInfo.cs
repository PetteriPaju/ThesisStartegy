using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SengokuWarrior
{

    public class ItemInfo : MonoBehaviour
    {

        public Text NameField;
        public Text ValueField;
        public Image SpriteField;
        public Text DecriptionField;
        public Image coinSprite;
        public VerticalLayoutGroup Content;

        public ItemInfoStyle[] styles;

        public Image[] GearSlotImages;
        public GameObject GearHelper;



        // Use this for initialization
        void Start()
        {
            if (GearHelper)
            GearHelper.SetActive(false);
            Clear();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Clear()
        {
            DestroyChildren();
            if (GearHelper) GearHelper.SetActive(false);
            NameField.text = "";
            ValueField.text = "";
            SpriteField.enabled = false;
            coinSprite.enabled = false;
            DecriptionField.text = "";
        }

        public GameObject CreateText (ItemInfoStyle style, GameObject parent, string text)
        {
            GameObject label = new GameObject();
            Text labeltext = label.AddComponent<Text>();
            SetStyle(style, labeltext);
            labeltext.text = text;
            label.transform.SetParent(parent.transform);
            label.transform.localScale = new Vector3(1, 1, 1);
            label.transform.position = Vector3.zero;

            return label;


        }

        public HorizontalLayoutGroup BuildLine(GameObject container, float height)
        {
            GameObject newLine = new GameObject("WeaponLine");
            newLine.transform.SetParent(container.transform);
            HorizontalLayoutGroup g = newLine.AddComponent<HorizontalLayoutGroup>();
            newLine.transform.localScale = new Vector3(1, 1, 1);
            newLine.GetComponent<RectTransform>().right = container.GetComponent<RectTransform>().right;
            newLine.GetComponent<RectTransform>().sizeDelta = new Vector2(newLine.GetComponent<RectTransform>().sizeDelta.x*2, height);

            return g;
        }
        public VerticalLayoutGroup BuildColumn(GameObject container, float height)
        {
            GameObject newLine = new GameObject("WeaponLine");
            newLine.transform.SetParent(container.transform);
            VerticalLayoutGroup g = newLine.AddComponent<VerticalLayoutGroup>();
            newLine.transform.localScale = new Vector3(1, 1, 1);
            newLine.GetComponent<RectTransform>().right = container.GetComponent<RectTransform>().right;
            newLine.GetComponent<RectTransform>().sizeDelta = new Vector2(newLine.GetComponent<RectTransform>().sizeDelta.x, height);

            return g;
        }
        public void BuildWeaponUI(GameObject container, WeaponAttribute attr)
        {

            HorizontalLayoutGroup g = BuildLine(container, 20);
            CreateText(GetStyle("WeaponStatLabel"), g.gameObject, "Type");
            CreateText(GetStyle("WeaponStatLabelRight"), g.gameObject, attr.WeaponType.ToString());
            g = BuildLine(container,20);
            CreateText(GetStyle("WeaponStatLabel"), g.gameObject, "Range");
            CreateText(GetStyle("WeaponStatLabelRight"), g.gameObject, attr.range.ToString());
            Canvas.ForceUpdateCanvases();
        }

        public void BuildStatAttributeUI(GameObject container,StatsAttribute attr)
        {
            HorizontalLayoutGroup g;
            for (int i =0; i < attr.enabled.Length; i++)
            {
                if (attr.enabled[i])
                {
                    g = BuildLine(container, 20);
                    CreateText(GetStyle("WeaponStatLabel"), g.gameObject, ((Stats.StatType)i).ToString());
                    string plus = "+";
                    if (attr.stats[i] < 0)
                    {
                        plus = "-";
                    }
                    CreateText(GetStyle("WeaponStatLabelRight"), g.gameObject, plus + attr.stats[i].ToString());

                }
            }    
            Canvas.ForceUpdateCanvases();
        }

        public void BuildArmorSlotAttributeUI(GameObject container, GearSlotAttribute attr)
        {
            if (GearHelper) GearHelper.SetActive(true);
            for (int i =0; i<attr.PossibleSlots.Length; i++)
            {
                if (!attr.PossibleSlots[i])
                {
                    GearSlotImages[i].color = new Color(GearSlotImages[i].color.r, GearSlotImages[i].color.g, GearSlotImages[i].color.b, 0.33f);
                }
                else
                {
                    GearSlotImages[i].color = new Color(GearSlotImages[i].color.r, GearSlotImages[i].color.g, GearSlotImages[i].color.b, 1f);
                }

                Outline outline = GearSlotImages[i].GetComponent<Outline>();

                if (outline)
                {
                    outline.enabled = false;
                    /*
                    if (!attr.UnequipLots[i])
                    {
                        outline.enabled = false;

                    }
                    else
                    {
                        outline.enabled = true;
                    }
                    */
                }

            }
        }
        public void BuildRecoveryItemUI(GameObject container, Use_RecoverAttribute attr)
        {

            HorizontalLayoutGroup g = BuildLine(container, 20);
            CreateText(GetStyle("HeaderLabel"), g.gameObject, "On Use");

            for (int i = 0; i < attr.Recover.Length; i++)
            {
                if (attr.enabled[i])
                {
                    g = BuildLine(container, 15);
                    CreateText(GetStyle("HeaderLabel"), g.gameObject, i == 0 ? "HP" : "MP");
                    string plus = "+";
                    if (attr.Recover[i] < 0)
                    {
                        plus = "-";
                    }
                    CreateText(GetStyle("HeaderLabel"), g.gameObject, plus+attr.Recover[i].ToString());

                }
            }

          }

        public void SetStyle(ItemInfoStyle st,Text text)
        {
            text.font = st.font;
            text.fontSize = st.fontSize;
            text.fontStyle = st.fontStyle;
            text.color = st.color;
            text.alignment = st.anchor;
        }
        public void SetStyle(string name, Text text)
        {
            SetStyle(GetStyle(name), text);
        }
        public ItemInfoStyle GetStyle(string name)
        {
            for (int i = 0; i < styles.Length; i++)
            {
                if (styles[i].Name == name) return styles[i];
            }

            Debug.LogWarning("Style with name: " + name + ", was not found!");
            return new ItemInfoStyle();
        }

        public void BuildUI(NewItem item)
        {
            DestroyChildren();
            if (item == null) return;
            if (GearHelper) GearHelper.SetActive(false);
            NameField.text = item.GameName;
            ValueField.text = item.Value.ToString();
            coinSprite.enabled = true;
            SpriteField.sprite = item.Sprite;
            SpriteField.enabled = true;
            DecriptionField.text = item.Description;
            HorizontalLayoutGroup g = BuildLine(Content.gameObject, 5);

            if (item.GetAttribute<WeaponAttribute>())
            {
                BuildWeaponUI(Content.gameObject, item.GetAttribute<WeaponAttribute>());
                g = BuildLine(Content.gameObject, 5);
            }
            if (item.GetAttribute<StatsAttribute>())
            {
                BuildStatAttributeUI(Content.gameObject, item.GetAttribute<StatsAttribute>());
            }
            if (item.GetAttribute<GearSlotAttribute>())
            {
                BuildArmorSlotAttributeUI(Content.gameObject, item.GetAttribute<GearSlotAttribute>());
            }
            if (item.GetAttribute<UseAttribute>())
            {
                UseAttribute atr = item.GetAttribute<UseAttribute>();
                if (atr.GetType() == typeof(Use_RecoverAttribute))
                {
                    BuildRecoveryItemUI(Content.gameObject, atr as Use_RecoverAttribute);
                }


            }
        }

        private void DestroyChildren()
        {
            foreach (Transform t in Content.transform)
            {
                GameObject.Destroy(t.gameObject);
            }
        }

        [System.Serializable]
        public class ItemInfoStyle
        {
            public string Name = "UI Style";
            public Font font;
            public int fontSize = 15;
            public FontStyle fontStyle = FontStyle.Normal;
            public Color color = Color.white;
            public TextAnchor anchor = TextAnchor.UpperLeft;

        }


    }

}
