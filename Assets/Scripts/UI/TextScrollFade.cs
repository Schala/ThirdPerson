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

public class TextScrollFade : MonoBehaviour
{
    public float maxLifetime = 1f;
    public float speed = 1f;
    TMP_Text text;
    float lifetime = 0f;

    private void Awake() => text = GetComponentInChildren<TMP_Text>();

	void Update()
    {
        if (lifetime < maxLifetime)
        {
            transform.position += Vector3.up * speed * Time.deltaTime;
            text.color = Color.Lerp(text.color, Color.clear, speed * Time.deltaTime);
        }
        else Destroy(gameObject);

        lifetime += Time.deltaTime;
    }
}
