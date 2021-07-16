using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconSprites : MonoBehaviour
{
    public Sprite[] resourceIcons = new Sprite[Resources.NUM], followerIcons;

    private void Start()
    {
        Icons.sprites = this;
    }
}
public static class Icons
{
    public static IconSprites sprites;

    public static Sprite Resource(Resource.Type type)
    {
        return sprites.resourceIcons[(int)type];
    }

    public static Sprite Follower(Follower.Type type)
    {
        return sprites.followerIcons[(int)type];
    }
}

