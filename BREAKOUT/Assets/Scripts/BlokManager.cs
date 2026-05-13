using System.Collections.Generic;
using UnityEngine;

public class BlokManager : MonoBehaviour
{
    public GameObject blokPrefab;
    public int rows = 5;
    public int columns = 8;
    public float spacing = 0.1f;

    public List<Blok> blokken = new List<Blok>();

    void Start()
    {
        MaakBlokken();
    }

    public void MaakBlokken()
    {
        blokken.Clear();

        float blokWidth = blokPrefab.GetComponent<SpriteRenderer>().bounds.size.x;
        float blokHeight = blokPrefab.GetComponent<SpriteRenderer>().bounds.size.y;

        float totalWidth = columns * (blokWidth + spacing) - spacing;
        float totalHeight = rows * (blokHeight + spacing) - spacing;

        Vector2 startPos = new Vector2(
            -totalWidth / 2f + blokWidth / 2f,
            Camera.main.orthographicSize - blokHeight
        );

        Color[] kleuren = {
        new Color(1f, 0.5f, 0f),
        Color.yellow,
        Color.red,
        Color.hotPink
    };

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                float x = startPos.x + c * (blokWidth + spacing);
                float y = startPos.y - r * (blokHeight + spacing);

                GameObject nieuwBlok = Instantiate(blokPrefab, new Vector2(x, y), Quaternion.identity);

                Blok blok = nieuwBlok.GetComponent<Blok>();
                blok.kleur = kleuren[r % kleuren.Length];

                blokken.Add(blok);
            }
        }
    }



    public void BlokVerwijderd(Blok blok)
    {
        blokken.Remove(blok);
        CheckWin();
    }

    public void CheckWin()
    {
        if (blokken.Count == 0)
        {
            Debug.Log("Je hebt gewonnen!");
        }
    }
}
