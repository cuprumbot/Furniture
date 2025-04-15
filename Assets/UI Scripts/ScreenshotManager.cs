using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScreenshotManager : MonoBehaviour
{
    public TextMeshProUGUI statusText;

    public void OnClickTakeScreenshot()
    {
        TakeScreenshot();
    }

    public void TakeScreenshot()
    {
        string filename = $"ss_{System.DateTime.Now:yyyyMMdd_HHmmss}.png";
        string path = System.IO.Path.Combine(Application.persistentDataPath, filename);

        ScreenCapture.CaptureScreenshot(filename);
        StartCoroutine(ConfirmScreenshotSaved(path));
    }

    private IEnumerator ConfirmScreenshotSaved(string path)
    {
        yield return new WaitForSeconds(1f);

        if (System.IO.File.Exists(path))
        {
            statusText.text = "Screenshot saved! " + path;
            ShareScreenshot(path);
        }
        else
        {
            statusText.text = "Failed to save screenshot.";
        }
    }

    private void ShareScreenshot(string imagePath)
    {
#if UNITY_ANDROID
    statusText.text = "Starting share...";

    AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
    AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");

    intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
    intentObject.Call<AndroidJavaObject>("setType", "image/*");

    intentObject.Call<AndroidJavaObject>("addFlags", 1); // FLAG_GRANT_READ_URI_PERMISSION

    AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
    AndroidJavaClass fileClass = new AndroidJavaClass("java.io.File");

    AndroidJavaObject fileObject = new AndroidJavaObject("java.io.File", imagePath);
    AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("fromFile", fileObject);

    intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);

    AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

    AndroidJavaObject chooser = intentClass.CallStatic<AndroidJavaObject>(
        "createChooser", intentObject, "Share Screenshot");

    currentActivity.Call("startActivity", chooser);
    statusText.text = "Finished share!";
#endif
    }
}
