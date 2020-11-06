using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class AnimalMaker : MonoBehaviour
{
    public Button butt;
    public Text leftText;
    public Text rightText;
    public Text fullText;
    public Image centerImage;
    public AudioClip snareRoll;
    public List<AudioClip> storedSongs;
    public List<Texture2D> storedImages;
    public List<Texture2D> displayedImages = new List<Texture2D>();

    bool calculatingStrings = false;

    string[] prefixes;
    string[] suffixes;

    AudioSource audio;
    AudioClip lastAudioClip;
    string lastPrefix;
    string lastSuffix;

    const string glyphs = "asdfghjkl;peiowu[q";

    // Start is called before the first frame update
    void Start()
    {
        // Load prefixes and suffixes into string arrays
        string pre = (Resources.Load("prefixes") as TextAsset).text;
        string suf = (Resources.Load("suffixes") as TextAsset).text;

        prefixes = Regex.Split(pre, "\n|\r|\r\n");
        suffixes = Regex.Split(suf, "\n|\r|\r\n");

        audio = GetComponent<AudioSource>();

        // Store all images in list on runtime start
        //storedImagePaths.AddRange(Directory.GetFiles(imagePath, "*.png", SearchOption.AllDirectories));
        //storedImagePaths.AddRange(Directory.GetFiles(imagePath, "*.jpg", SearchOption.AllDirectories));
        //storedImagePaths.AddRange(Directory.GetFiles(imagePath, "*.jpeg", SearchOption.AllDirectories));

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
        if (displayedImages.Count >= storedImages.Count)
            displayedImages.Clear();

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

        leftText.text = GetRandomName(prefixes);
        rightText.text = GetRandomName(suffixes);

        // Double check names for blank lines and repeated names
        while (leftText.text == "" || rightText.text == "" || leftText.text == rightText.text || leftText.text == lastPrefix || rightText.text == lastSuffix)
        {
            if(leftText.text == "" || leftText.text == lastPrefix)
                leftText.text = GetRandomName(prefixes);
            if(rightText.text == "" || rightText.text == leftText.text || rightText.text == lastSuffix)
                rightText.text = GetRandomName(suffixes);

            yield return new WaitForEndOfFrame();
        }

        lastPrefix = leftText.text;
        lastSuffix = rightText.text;
        yield return new WaitForSeconds(1);

        fullText.text = leftText.text + " " + rightText.text;
        leftText.text = "";
        rightText.text = "";
        LoadRandomImage();
        centerImage.color = Color.white;
        LoadRandomSong();
        lastAudioClip = audio.clip;
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
        // Grab random image - repeat if already shown
        Texture2D tex = GetRandomImage();
        while (displayedImages.Contains(tex))
            tex = GetRandomImage();

        displayedImages.Add(tex);
        centerImage.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        //centerImage.rectTransform.sizeDelta = new Vector2(im.sprite.rect.width / 10, im.sprite.rect.height / 10);
        //centerImage.GetComponent<AspectRatioFitter>().aspectRatio = (float)tex.width / tex.height;
    }

    void LoadRandomSong()
    {
        // Grab path of random song - repeat if duplicate
        AudioClip song = GetRandomSong();
        while (song == lastAudioClip)
            song = GetRandomSong();

        audio.clip = song;
        audio.Play();
    }

    string GetRandomName(string[] names)
    {
        return names[Random.Range(0, names.Length)];
    }

    public AudioClip GetRandomSong()
    {
        return storedSongs[Random.Range(0, storedSongs.Count)];
    }
    public Texture2D GetRandomImage()
    {
        return storedImages[Random.Range(0, storedImages.Count)];
    }
}
