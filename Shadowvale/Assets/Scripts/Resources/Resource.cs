using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : Interaction
{


    public enum Type : int
    {
        wood,
        stone,
        food
    }
    public enum Size : int
    {
        small,
        medium,
        large
    }

    public Type type;
    public Size size = Size.small;
    public int val = 10;
    protected Animator anim;
    public SpriteRenderer rend;
    AudioSource audio;
    public GameObject destroyedPrefab;
    Color startColour;
    public Color corruptColour;
    private void Start()
    {
        anim = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
        startColour = rend.color;
    }

    public void ChangeColour(float val)
    {
        rend.color = Color.Lerp(startColour, corruptColour, val);
    }

    public bool Gather(Inventory inv)
    {
        StartCoroutine(HitRoutine());

        // Adds resource to the resource type's corresponding inventory 
        inv.resources[(int)type]++;

        val--;
        audio.Play();
        if (val <= 0)
        {
            Pathfinding.UpdateNodeGrid();
            Remove();
            return false;
        }
        return true;
    }

    IEnumerator HitRoutine()
    {
        anim.SetBool("Hit", true);
        yield return new WaitForSeconds(0.1f);
        anim.SetBool("Hit", false);
        if (val <= 0)
        {
            if (destroyedPrefab != null)
            {
                GameObject destroyed = Instantiate(destroyedPrefab, transform.position, Quaternion.identity);
                Grid.GetTile(new Vector2Int((int)transform.position.x, (int)transform.position.y)).structure = destroyed.GetComponent<Stump>();
            }
            Destroy(gameObject);
        }
    }

    void Remove()
    {
        if (type == Type.wood)
        {
            Resources.trees.Remove(this);
        }
        else if (type == Type.wood)
        {
            Resources.stones.Remove(this);
        }
        Resources.allResources.Remove(this);
        Grid.GetTile(new Vector2Int((int)transform.position.x, (int)transform.position.y)).structure = null;
        Pathfinding.UpdateNodeGrid();
    }

    public void Save(ResourceData data)
    {
        data.type = (int)type;
        data.val = val;
        data.size = (int)size;
        data.pos = new Vector2Int((int)transform.position.x, (int)transform.position.y);
    }

    public void Load(ResourceData data)
    {
        Tile tile = Grid.tiles[data.pos.x, data.pos.y];
        tile.structure = this;
        if (tile.corruptionVal > 0)
        {
            ChangeColour(tile.corruptionVal / 100);
        }
        if (type == Resource.Type.wood)
        {
            Resources.trees.Add(Grid.tiles[data.pos.x, data.pos.y].structure);
        }
        else if (type == Resource.Type.stone)
        {
            Resources.stones.Add(Grid.tiles[data.pos.x, data.pos.y].structure);
        }

        Resources.allResources.Add(Grid.tiles[data.pos.x, data.pos.y].structure);
    }
}
