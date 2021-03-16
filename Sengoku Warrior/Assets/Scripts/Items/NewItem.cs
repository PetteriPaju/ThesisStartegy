using UnityEngine;
using System.Collections.Generic;
namespace SengokuWarrior
{
    public class NewItem : ScriptableObject 
{
    public string ItemName = "New Item";

    [SerializeField]
    public List<ItemAttribute> attributes = new List<ItemAttribute>();
    public int id = -1;
    private Dictionary<System.Type, ItemAttribute> d_attributes = new Dictionary<System.Type, ItemAttribute>();

    public void GenereateNewId()
        {
            id = System.Guid.NewGuid().GetHashCode();
        }

        public void Add(ItemAttribute attribute)
    {
        if (D_attributes.ContainsKey(attribute.GetType()))
        {
        GameObject.DestroyImmediate(attribute, true);
        Debug.LogWarning("This Item already has an attribute:" + attribute.GetType());
       }
       else
        {
          attributes.Add(attribute);
          d_attributes[attribute.GetType()] = attribute;
          Sort();
        }
           

        }
    
    private void Sort()
    {
     attributes.Sort((a, b) => (a.Priority().CompareTo(b.Priority())));
    }

       public void Destroy (ItemAttribute attribute)
       {
        attributes.Remove(attribute);
        d_attributes[attribute.GetType()] = null;
        GameObject.DestroyImmediate(attribute, true);
            Sort();
        }

    public TAttribute GetAttribute<TAttribute>() where TAttribute : ItemAttribute
    {

        foreach (ItemAttribute attr in attributes)
            {
                if (attr is TAttribute)
                {
                    return (TAttribute)attr;
                }
            }
        /*
        ItemAttribute attribute;

        if (D_attributes.TryGetValue(typeof(TAttribute), out attribute))
        {
            return (TAttribute)attribute;
        }
        */
        return null;
    }


    private Dictionary<System.Type, ItemAttribute> D_attributes
    {
        get
        {
            if (d_attributes.Count != attributes.Count)
            {
                RefreshDictionary();
            }
            return d_attributes;
        }
    }

    public void RefreshDictionary()
    {
        d_attributes.Clear();
        for (int i = 0; i < attributes.Count; i++)
        {
            d_attributes.Add(attributes[i].GetType(), attributes[i]);
        }
    }

        public float Value
        {
            get
            {
                CommonAttributes atr = GetAttribute<CommonAttributes>();
                if (atr)
                {
                    return atr.value;
                }
                else
                    return -1;
            }
        }

        public string GameName
        {
            get
            {
                CommonAttributes atr = GetAttribute<CommonAttributes>();
                if (atr)
                {
                    return atr._name;
                }
                else
                    return "#null";
            }
        }
        public string Description
        {
            get
            {
                CommonAttributes atr = GetAttribute<CommonAttributes>();
                if (atr)
                {
                    return atr.Description;
                }
                else
                    return "#null";
            }
        }
        public Sprite Sprite
        {
            get
            {
                CommonAttributes atr = GetAttribute<CommonAttributes>();
                if (atr)
                {
                    return atr.GetSprite();
                }
                else
                    return new Sprite();
            }
        }
    }
    }

