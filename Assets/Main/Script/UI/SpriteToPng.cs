using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SpriteToPng : MonoBehaviour
{
    Sprite[] m_sprites;

    // Start is called before the first frame update
    void Start()
    { 
        m_sprites = Resources.LoadAll<Sprite>("FontImage/text_01");
        Debug.Log(m_sprites.Length);
        for(int i=0; i<m_sprites.Length; i++)
        {
            var spr = m_sprites[i];
            Texture2D tex = new Texture2D((int)spr.rect.width, (int)spr.rect.height, TextureFormat.ARGB32, false);
            tex.SetPixels(spr.texture.GetPixels((int)spr.rect.x, (int)spr.rect.y, (int)spr.rect.width, (int)spr.rect.height));
            var bytes = tex.EncodeToPNG();

            //그냥 경로를 주어주면 저장이되지만, 에디터내에 저장하고싶다면 Application.dataPath를 사용하면 Assets까지의 경로를 반환해주므로 이것을 사용하면된다.
            //단말기 예를들어 핸드폰같은경우는 Application.PersistentDataPath를 사용하면된다.
            File.WriteAllBytes(Application.dataPath + "/ImageFonts/" + string.Format("imageFont_{0:00}.png", i+1), bytes);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
