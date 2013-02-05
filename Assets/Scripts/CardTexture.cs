using UnityEngine;
using System.Collections;

public class CardTexture : MonoBehaviour {

    public void Activate(string activatedTexture)
    {
        renderer.material.mainTexture = (Texture2D)Resources.Load(activatedTexture);
    }

    public void Deactivate(string normalTexture)
    {
        renderer.material.mainTexture = (Texture2D)Resources.Load(normalTexture);
    } 	
}
