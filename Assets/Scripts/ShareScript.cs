using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;


// import windows dll to talk to WinAPI so when share button is pressed
// Share screen opens with screenshot of score. 
public class WindowsShareUI : MonoBehaviour
{
    [DllImport(
        "Commute_with_Windows",
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Unicode
    )]
    private static extern int ShowWindowsShare(
        [MarshalAs(UnmanagedType.LPWStr)] string filePath,
        [MarshalAs(UnmanagedType.LPWStr)] string title
    );

    private string temporaryFilePath;

    // Connect this to your Share button's OnClick event.
    public void OpenWindowsShareDialog()
    {
#if UNITY_STANDALONE_WIN

        StartCoroutine(CaptureAndShareRoutine());

#else

        Debug.LogWarning(
            "Windows Share is only available in a Windows standalone build."
        );

#endif
    }


    private IEnumerator CaptureAndShareRoutine()
    {
        // Wait for the frame to finish rendering.
        yield return new WaitForEndOfFrame();

        // Location where Unity will temporarily save it.
        temporaryFilePath = Path.Combine(
            Path.GetTempPath(),
            "flappybird-score.png"
        );

        // Delete an old screenshot if one exists.
        if (File.Exists(temporaryFilePath))
        {
            File.Delete(temporaryFilePath);
        }

        Debug.Log(
            "Capturing screenshot to: " +
            temporaryFilePath
        );

        // Take the screenshot.
        ScreenCapture.CaptureScreenshot(
            temporaryFilePath
        );

        // Wait until the screenshot exists.
        while (!File.Exists(temporaryFilePath))
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Give the file system a moment to finish writing.
        yield return new WaitForSeconds(0.2f);

        // Call C++ Windows DLL.
        int result = ShowWindowsShare(
            temporaryFilePath,
            "My Flappy Bird High Score!"
        );

        Debug.Log("DLL returned: " + result);

        if (result != 0)
        {
            Debug.LogError(
                "Windows Share DLL returned error: " +
                result
            );
        }
        else
        {
            Debug.Log(
                "Windows Share request sent successfully."
            );
        }
    }
}