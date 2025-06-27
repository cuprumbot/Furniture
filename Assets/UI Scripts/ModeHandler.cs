using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Mode
{
    None,
    Place,
    Move,
    Delete,
    Rotate,
    Resize
}

public class ModeHandler : MonoBehaviour
{
    public static Mode mode = Mode.None;

    public static void setSelectMode ()
    {
        // This is intentional
        // After choosing a model, we are ready to place it
        // Also, Mode.Select doesn't even exist
        mode = Mode.Place;
    }

    public static void setPlaceMode ()
    {
        mode = Mode.Place;
    }

    public static void setMoveMode ()
    {
        mode = Mode.Move;
    }

    public static void setDeleteMode ()
    {
        mode = Mode.Delete;
    }

    public static void setRotateMode ()
    {
        mode = Mode.Rotate;
    }

    public static void setResizeMode ()
    {
        mode = Mode.Resize;
    }

    public static void setNoneMode ()
    {
        mode = Mode.None;
    }
}
