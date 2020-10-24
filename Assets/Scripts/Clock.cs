using UnityEngine;
using UnityEngine.UI;

public class Clock : MonoBehaviour
{
    const int REALTIME_DAY_SECONDS = 60 * 60 * 24;

    public float slowFactor = 2f;

    Text clockText;
    float clockDelta;
    float clockMax;
    float clockSecond, clockMinute, clockHour;
    float currentTime = 0f;
    float sunTicks;

    void Start()
    {
        clockText = GetComponent<Text>();
        clockDelta = Time.deltaTime / slowFactor;
        sunTicks = 360f / clockDelta;
        clockSecond = sunTicks / REALTIME_DAY_SECONDS;
        clockMinute = clockSecond * 60f;

        // Not sure why it needs double minus one, but without it, the hour turns after 30 seconds
        // This also helps make sure the hour turns when minutes roll back to 00
        clockHour = clockMinute * 119f; 
        clockMax = clockSecond * sunTicks;
    }

    void Update()
    {
        currentTime += clockSecond;
        if (currentTime >= clockMax) currentTime -= clockMax;

        //int seconds = Mathf.RoundToInt(currentTime / clockSecond) % 60;
        int minutes = Mathf.RoundToInt(currentTime / clockMinute) % 60;
        int hours = Mathf.RoundToInt(currentTime / clockHour);

        clockText.text = $"{hours:D2}:{minutes:D2}";
    }
}
