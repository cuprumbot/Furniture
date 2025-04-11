using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeHandler : MonoBehaviour
{
    public static bool placeMode = false;
    public static bool moveMode = false;
    public static bool deleteMode = false; 

    public static void setPlaceMode ()
    {
        placeMode = true;
        moveMode = false;
        deleteMode = false;
    }

    public static void setMoveMode ()
    {
        moveMode = true;
        placeMode = false;
        deleteMode = false;
    }

    public static void setDeleteMode ()
    {
        deleteMode = true;
        placeMode = false;
        moveMode = false;
    }

    public static void setNoMode ()
    {
        placeMode = false;
        moveMode = false;
        deleteMode = false;
    }
}
