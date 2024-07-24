using UnityEngine;
using UnityEngine.UI;


namespace Jois
{
    public class FpsText : MonoBehaviour
    {
        [SerializeField] Text fpsText;
        [SerializeField] float updateInterval = 0.5F;

        float accum = 0; // FPS accumulated over the interval
        int frames = 0; // Frames drawn over the interval
        float timeleft = 0f; // Left time for current interval

        void Update()
        {
            timeleft -= Time.deltaTime;
            accum += Time.timeScale / Time.deltaTime;
            ++frames;

            // Interval ended - update GUI text and start new interval
            if (timeleft <= 0.0)
            {
                // display two fractional digits (f2 format)
                float fps = accum / frames;
                string format = System.String.Format("{0:F1}", fps);

                if (fpsText != null)
                {
                    fpsText.text = format;

                    if (fps < 30)
                        fpsText.color = Color.yellow;
                    else if (fps < 10)
                        fpsText.color = Color.red;
                    else
                        fpsText.color = Color.green;
                }

                //  DebugConsole.Log(format,level);
                timeleft = updateInterval;
                accum = 0.0F;
                frames = 0;
            }
        }
    }
}