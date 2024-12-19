using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenAdd : MonoBehaviour
{
    public MeshRenderer myRenderer;
    public List<Texture2D> addTextures;
    // Start is called before the first frame update
    void Start()
    {
        int textureIndex = Random.Range(0,addTextures.Count);
        myRenderer.material.SetTexture("_AddTexture",addTextures[textureIndex]);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
