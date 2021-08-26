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
    protected SpriteRenderer rend;
    AudioSource audio;
    private void Start()
    {
        anim = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();
        audio = GetComponent<AudioSource>();
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
        Grid.tiles[data.pos.x, data.pos.y].structure = this;

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
