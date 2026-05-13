using UnityEngine;

public class Blok : MonoBehaviour
{
    public Color kleur;
    public int punten = 10;

    private SpriteRenderer sr;
    private BlokManager manager;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = kleur;
        manager = FindAnyObjectByType<BlokManager>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject)
        {
            Verwijder();
        }
    }

    public void Verwijder()
    {
        manager.BlokVerwijderd(this);
        Destroy(gameObject);
    }
}
