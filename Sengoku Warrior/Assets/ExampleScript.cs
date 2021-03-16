using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleScript : MonoBehaviour {

    [Range(0,10)]
    public float attribute = 3f;
    public float noAttribute = 5;

}

public class DefaultFloatAttribute : PropertyAttribute
{
    public readonly float defaultValue;

    public DefaultFloatAttribute(float defFloat)
    {
        this.defaultValue = defFloat;
    }
}