using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class ImageLoaderTestScript : MonoBehaviour
{
    public List<Image> h_images;
    public List<Texture2D> images = new List<Texture2D>();
    public string imagesPath = @"C:\Users\Basel\Documents\PhotoTagger\ImageResources";

    

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadImages()
    {
        images.Clear();
        StartCoroutine(LoadImagesCoroutine());
    }

    public IEnumerator LoadImagesCoroutine()
    {
        print("Hoo haw");
        yield return StartCoroutine("LoadAll", Directory.GetFiles(imagesPath, "*.png", SearchOption.AllDirectories));

        int i = 0;
        foreach (Image im in h_images)
        {
            im.sprite = Sprite.Create(images[i], new Rect(0, 0, images[i].width, images[i].height), new Vector2(0.5f, 0.5f));
            im.rectTransform.sizeDelta = new Vector2(im.sprite.rect.width / 10, im.sprite.rect.height / 10);
            i++;
        }
    }

    public IEnumerator LoadAll(string[] filePaths)
    {
        foreach (string filePath in filePaths)
        {
            WWW load = new WWW("file:///" + filePath);
            yield return load;
            if (!string.IsNullOrEmpty(load.error))
            {
                Debug.LogWarning(filePath + " error");
            }
            else
            {
                images.Add(load.texture);
            }
        }
    }
}
