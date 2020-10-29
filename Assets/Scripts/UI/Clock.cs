/*
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 3, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street - Fifth Floor, Boston, MA 02110-1301, USA.
 */

using TMPro;
using UnityEngine;

public class Clock : MonoBehaviour
{
    const int REALTIME_DAY_SECONDS = 60 * 60 * 24;

    public float slowFactor = 2f;

    TMP_Text clockText;
    float clockDelta;
    float clockMax;
    float clockSecond, clockMinute, clockHour;
    float currentTime = 0f;
    float sunTicks;

    void Start()
    {
        clockText = GetComponent<TMP_Text>();
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
