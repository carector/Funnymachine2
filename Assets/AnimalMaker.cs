using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class AnimalMaker : MonoBehaviour
{
    public Button butt;
    public Text leftText;
    public Text rightText;
    public Text fullText;
    public Image centerImage;
    public AudioClip snareRoll;
    public List<AudioClip> storedSongs;

    string prefixPath;
    string suffixPath;
    string musicPath;
    string imagePath;
    bool calculatingStrings = false;

    AudioSource audio;

    const string glyphs = "!@#$%&?";
    List<string> storedImagePaths = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        prefixPath = Application.dataPath + "/Sources/prefixes.txt";
        suffixPath = Application.dataPath + "/Sources/suffixes.txt";
        imagePath = Application.dataPath + "/Sources/Images";

        audio = GetComponent<AudioSource>();

        // Store all images in list on runtime start
        storedImagePaths.AddRange(Directory.GetFiles(imagePath, "*.png", SearchOption.AllDirectories));
        storedImagePaths.AddRange(Directory.GetFiles(imagePath, "*.jpg", SearchOption.AllDirectories));
        storedImagePaths.AddRange(Directory.GetFiles(imagePath, "*.jpeg", SearchOption.AllDirectories));

        // Do the same with audio
        //storedSongPaths.AddRange(Directory.GetFiles(musicPath, "*.mp3", SearchOption.AllDirectories));
        //storedSongPaths.AddRange(Directory.GetFiles(musicPath, "*.wav", SearchOption.AllDirectories));
        //storedSongPaths.AddRange(Directory.GetFiles(musicPath, "*.ogg", SearchOption.AllDirectories));
    }

    public void Begin()
    {
        StopAllCoroutines();
        StartCoroutine(BeginCreation());
        butt.interactable = false;
    }

    IEnumerator BeginCreation()
    {
        fullText.text = "";
        centerImage.color = Color.clear;
        audio.Stop();

        leftText.rectTransform.anchoredPosition = new Vector2(0, -241);
        rightText.rectTransform.anchoredPosition = new Vector2(0, -241);

        calculatingStrings = true;
        StartCoroutine(SetRandomText(leftText));
        StartCoroutine(SetRandomText(rightText));

        while ((rightText.rectTransform.anchoredPosition.x - 200) < 5)
        {
            leftText.rectTransform.anchoredPosition = Vector2.Lerp(leftText.rectTransform.anchoredPosition, new Vector2(-350, -241), 0.05f);
            rightText.rectTransform.anchoredPosition = Vector2.Lerp(rightText.rectTransform.anchoredPosition, new Vector2(350, -241), 0.05f);
            yield return new WaitForEndOfFrame();
        }
        audio.PlayOneShot(snareRoll);
        yield return new WaitForSeconds(1.5f);
        calculatingStrings = false;

        leftText.text = GetRandomName(prefixPath);
        rightText.text = GetRandomName(suffixPath);
        yield return new WaitForSeconds(1);

        fullText.text = leftText.text + " " + rightText.text;
        leftText.text = "";
        rightText.text = "";
        LoadRandomImage();
        centerImage.color = Color.white;
        LoadRandomSong();
        butt.interactable = true;
    }

    IEnumerator SetRandomText(Text txt)
    {
        txt.text = GetRandomString();
        yield return new WaitForFixedUpdate();

        if (calculatingStrings)
            StartCoroutine(SetRandomText(txt));
    }

    string GetRandomString()
    {
        string r = "";
        for (int i = 0; i < 10; i++)
            r += glyphs[Random.Range(0, glyphs.Length)];
        return r;
    }

    void LoadRandomImage()
    {
        // Grab path of random image
        string randPath = GetRandomImagePath();

        Texture2D tex = new WWW("file:///" + randPath).texture;

        centerImage.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        //centerImage.rectTransform.sizeDelta = new Vector2(im.sprite.rect.width / 10, im.sprite.rect.height / 10);
        //centerImage.GetComponent<AspectRatioFitter>().aspectRatio = (float)tex.width / tex.height;
    }

    void LoadRandomSong()
    {
        // Grab path of random song
        AudioClip song = GetRandomSong();
        audio.clip = song;
        audio.Play();
    }

    string GetRandomName(string path)
    {
        string[] lines = System.IO.File.ReadAllLines(path);
        return lines[Random.Range(0, lines.Length)];
    }

    public AudioClip GetRandomSong()
    {
        return storedSongs[Random.Range(0, storedSongs.Count)];
    }
    public string GetRandomImagePath()
    {
        return storedImagePaths[Random.Range(0, storedImagePaths.Count)];
    }
}
