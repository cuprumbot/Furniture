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
        // Wait a bit to ensure screenshot is saved
        yield return new WaitForSeconds(1f);

        if (File.Exists(path))
        {
            statusText.text = "Screenshot saved! Sharing...";
            ShareScreenshot(path);
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

        statusText.text = "Sharing...";
    }
}
