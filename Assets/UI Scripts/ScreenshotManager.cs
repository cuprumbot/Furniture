using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

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
        string path = Path.Combine(Application.persistentDataPath, filename);

        ScreenCapture.CaptureScreenshot(filename);
        StartCoroutine(ConfirmScreenshotSaved(path));
    }

    private IEnumerator ConfirmScreenshotSaved(string path)
    {
        yield return new WaitForSeconds(1f); // wait for the file to save

        if (System.IO.File.Exists(path))
        {
            statusText.text = "Screenshot saved.";

            SaveToGallery(path); // step 2: make visible in Android Gallery
            ShareScreenshot(path); // step 3: share using NativeShare
        }
        else
        {
            statusText.text = "Failed to save screenshot.";
        }
    }

    private void ShareScreenshot(string imagePath)
    {
        // In case you need to redo this:
        // Windows > Package Manager
        // Add from URL:
        // https://github.com/yasirkula/UnityNativeShare.git

        new NativeShare()
            .AddFile(imagePath)
            .SetSubject("Screenshot")
            .SetText("Furniture Screenshot")
            .SetTitle("Share Screenshot")
            .Share();

        //statusText.text = "Sharing...";
    }

    private void SaveToGallery(string path)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

                using (AndroidJavaClass pluginClass = new AndroidJavaClass("edu.galileo.innovacion.furniture.SaveImageToGallery"))
                {
                    pluginClass.CallStatic("Save", activity, path);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error saving to gallery: " + e.Message);
            statusText.text = "Error saving to gallery: " + e.Message;
        }
#endif
    }
}
