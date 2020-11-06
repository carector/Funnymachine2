using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEngine.UI;

public class ImageRankerTestScript : MonoBehaviour
{
    public int battleSize = 250;

    public List<Image> h_images;
    public Text remainingBattles;
    public string imagesPath = @"C:\Users\Basel\Documents\PhotoTagger\ImageResources";
    public List<string> storedImagePaths = new List<string>();
    public List<Texture2D> finalRankings = new List<Texture2D>();

    Dictionary<string, int> rankings = new Dictionary<string, int>();

    // How many battles an image has won
    int leftImageStreak = 0;
    int rightImageStreak = 0;

    string leftImagePath = "";
    string rightImagePath = "";

    private void Start()
    {
        // Store all images in list on runtime start
        storedImagePaths.AddRange(Directory.GetFiles(imagesPath, "*.png", SearchOption.AllDirectories));
        storedImagePaths.AddRange(Directory.GetFiles(imagesPath, "*.jpg", SearchOption.AllDirectories));
        storedImagePaths.AddRange(Directory.GetFiles(imagesPath, "*.jpeg", SearchOption.AllDirectories));

        h_images[2].enabled = false;
        LoadRandomImageLeft();
        LoadRandomImageRight();
        leftImageStreak = 0;
        rightImageStreak = 0;
    }

    public void LoadRandomImageLeft()
    {
        if (storedImagePaths.Count > 0 && battleSize > 0)
        {
            h_images[2].enabled = false;

            if (leftImagePath != "")
            {
                print("Pee");
                rankings.Add(leftImagePath, leftImageStreak);
            }

            rightImageStreak++;
            leftImageStreak = 0;
            LoadRandomImage(0);
            storedImagePaths.Remove(leftImagePath);
            battleSize--;
            remainingBattles.text = battleSize.ToString();
        }
        else
        {
            rankings.Add(rightImagePath, rightImageStreak);
            h_images[1].enabled = false;
            h_images[0].enabled = false;
        }
    }

    public void LoadRandomImageRight()
    {
        if (storedImagePaths.Count > 0 && battleSize > 0)
        {
            h_images[2].enabled = false;

            if (rightImagePath != "")
            {
                print("Pee");
                rankings.Add(rightImagePath, rightImageStreak);
            }

            leftImageStreak++;
            LoadRandomImage(1);
            storedImagePaths.Remove(rightImagePath);
            battleSize--;
            remainingBattles.text = battleSize.ToString();
        }
        else
        {
            rankings.Add(leftImagePath, leftImageStreak);
            h_images[1].enabled = false;
            h_images[0].enabled = false;
        }
    }

    public void CheckHighest()
    {
        h_images[2].enabled = true;
        string path = GetHighestRanking();
        Image im = h_images[2];
        Texture2D tex = new WWW("file:///" + path).texture;

        im.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        im.rectTransform.sizeDelta = new Vector2(im.sprite.rect.width / 10, im.sprite.rect.height / 10);
        im.GetComponent<AspectRatioFitter>().aspectRatio = (float)tex.width / tex.height;
    }

    public void LoadRandomImage(int uiImageIndex)
    {
        Image im = h_images[uiImageIndex];

        // Grab path of random image
        string randPath = GetRandomImagePath();

        // Store image path so we can remove it later
        if (uiImageIndex == 0)
            leftImagePath = randPath;
        else
            rightImagePath = randPath;

        Texture2D tex = new WWW("file:///" + randPath).texture;

        im.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        im.rectTransform.sizeDelta = new Vector2(im.sprite.rect.width / 10, im.sprite.rect.height / 10);
        im.GetComponent<AspectRatioFitter>().aspectRatio = (float)tex.width / tex.height;
    }

    public string GetRandomImagePath()
    {
        return storedImagePaths[Random.Range(0, storedImagePaths.Count)];
    }

    public string GetHighestRanking()
    {
        string storedPath = "";
        int storedValue = 0;
        int compared = 0;

        foreach (KeyValuePair<string, int> k in rankings)
        {
            if (k.Value > storedValue)
            {
                storedPath = k.Key;
                storedValue = k.Value;
                print(k.Key + " had value " + k.Value);
            }
            compared++;
        }
        print(compared + " images compared");

        return storedPath;
    }
}