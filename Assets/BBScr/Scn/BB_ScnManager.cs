using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScnManager : MonoBehaviour
{
    /** PRIVATE STATICS **/

    static string[] SCN_MANAGER_REQUIREDOBJS = {
        "ScnPrior",
        "ScnPrior/ScnPriorCamera",
        "ScnPrior/ScnPriorUI",
        "ScnPrior/ScnPriorUI/ScnPriorUIBoundary",
        "ScnPrior/ScnPriorUI/ScnPriorUIEvents",
        "ScnMain",
    };
    
    const string SCN_MANAGER_INSTANCENAME = "ScnMain";
    const bool Debugm = true;

    Vector2 cameraPosition;
    Vector2 cameraShake;

    int cameraShakeLevel;

    static GameObject getScnObj_(string scnObj)
    {
        string[] hierarchy = scnObj.Split('/');
        
        GameObject found = null;

        string total = "/";
        foreach (string entry in hierarchy)
        {
            total += entry + "/";
            found = GameObject.Find(total);
            Debug.Assert(found != null/*?*/, "ScnObj " + total + " doesnt exist. Is this a BBScn?");
        }

        //Debug.Log("got obj '" + total + "' Return.");
        return found;
    }
    static void checkScnObj_(string scnObj)
    {
        GameObject dummy = getScnObj_(scnObj);
    }
    static void init_()
    {
        // Check if the required GameObjects are there.
        foreach(string obj in SCN_MANAGER_REQUIREDOBJS)
        {
            checkScnObj_(obj);
        }
    }

    /** PUBLIC STATICS **/

    /// <summary>
    /// Returns the Scene Manager Instance.
    /// </summary>
    /// <returns>Scene Manager Instance</returns>
    public static ScnManager Instance()
    {
        return getScnObj_(SCN_MANAGER_INSTANCENAME).GetComponent<ScnManager>();
    }

    /// <summary>
    /// Retrieves a GameObject.
    /// </summary>
    /// <param name="obj">GameObject Name.</param>
    /// <returns>Found GameObject</returns>
    public static GameObject GetObject(string obj)
    {
        return getScnObj_(obj);
    }

    /// <summary>
    /// Returns the Scene Camera.
    /// </summary>
    /// <returns>Scene Camera</returns>
    public static Camera GetCamera()
    {
        return GetObject("ScnPrior/ScnPriorCamera").GetComponent<Camera>();
    }

    public void SetCameraPosition(Vector2 vector)
    {
        cameraPosition = vector;
    }

    public void SetCameraPosition(float x, float y)
    {
        SetCameraPosition(new Vector2(x, y));
    }

    public void SetCameraShakeLevel(int level)
    {
        cameraShakeLevel = level;
    }

    System.Random rngGen;

    /** MONOBEHAVIOUR OVERRIDES **/
    private void Start()
    {
        Debug.Assert(gameObject.name == SCN_MANAGER_INSTANCENAME, "Must call it from " + SCN_MANAGER_INSTANCENAME);
        init_();
        cameraPosition = GetCamera().transform.position;
        cameraShake = new Vector2(0, 0);
        cameraShakeLevel = 5;

        rngGen = new System.Random();
    }

    private void Update()
    {
        if (cameraShakeLevel != 0)
        {
            cameraShake.x = rngGen.Next(0, (cameraShakeLevel / 2));
            cameraShake.y = rngGen.Next(0, (cameraShakeLevel / 2));
        }
        GetCamera().transform.position = cameraPosition + cameraShake;
    }
}
