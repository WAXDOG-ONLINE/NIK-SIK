using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Painting : MonoBehaviour
{
    public MeshRenderer myRenderer;
    public List<Texture2D> paintingTextures;
    // Start is called before the first frame update
    void Start()
    {
        int textureIndex = Random.Range(0,paintingTextures.Count);
        myRenderer.material.mainTexture = paintingTextures[textureIndex];
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
