using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BB_SmartBool
{
    bool boolean;
    bool alreadySet;

    public BB_SmartBool()
    {
        boolean = false;
        alreadySet = false;
    }

    public BB_SmartBool(bool value)
    {
        boolean = value;
        alreadySet = false;
    }

    public bool Is()
    {
        return boolean;
    }

    // Update is called once per frame
    public void Set(bool value)
    {
        if (!alreadySet && value != boolean)
        {
            boolean = value;
            alreadySet = true;
        }
    }

    public void ForceSet(bool flag)
    {
        boolean = flag;
        alreadySet = true;
    }

    public void Reset()
    {
        alreadySet = false;
    }

    public static implicit operator bool(BB_SmartBool obj)
    {
        return obj.Is();
    }

    public static bool operator ==(bool @bool, BB_SmartBool obj)
    {
        return Equals(obj.Is(), @bool);
    }
    public static bool operator !=(bool @bool, BB_SmartBool obj)
    {
        return !Equals(obj.Is(), @bool);
    }
    public static bool operator ==(BB_SmartBool obj, bool @bool)
    {
        return Equals(obj.Is(), @bool);
    }
    public static bool operator !=(BB_SmartBool obj, bool @bool)
    {
        return !Equals(obj.Is(), @bool);
    }
}
