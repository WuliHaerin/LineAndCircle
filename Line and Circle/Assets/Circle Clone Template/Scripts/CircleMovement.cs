using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using StarkSDKSpace;
using TTSDK.UNBridgeLib.LitJson;
using TTSDK;
using UnityEngine.SceneManagement;
public class CircleMovement : MonoBehaviour {
    
    //Challenge Generator
    public ChallengeGenerator myGenerator;
    //End Panel to show score
    public GameObject EndPanel;
    //Menu Info
    public Text startBest;
    public Text endBest;
    public Text currentScore;
    public Text sessionScore;
    //Jump force
    public float jumpForce = 5.0f;
    //Game Has started?
    bool started = false;
    Vector3 defaultPosition = Vector3.zero;
    Rigidbody2D myRigidbody;
    Animator myAnim;
    bool isOver = false;
    int score = 0;
    //Sound Variables
    AudioSource myAudioPlayer;
    public AudioClip jump;
    public AudioClip crash;
    private StarkAdManager starkAdManager;
    public string clickid;

    // Use this for initialization
    void Start () {

        defaultPosition = transform.position;
        myRigidbody = transform.GetComponent<Rigidbody2D>();
        myAnim = transform.GetComponent<Animator>();
        if (!myRigidbody) {
            Debug.LogError("No Rigidbody Found Please Assign one on " + gameObject.name.ToString() + " Object");
        }
        if (!myAnim)
        {
            Debug.LogError("No Animator Found Please Assign one on " + gameObject.name.ToString() + " Object");
        }

        if (startBest)
        {
            startBest.text = PlayerPrefs.GetInt("Best", 0).ToString();
        }
        else {
            Debug.LogWarning("Varibles not assigned");
        }

        if (currentScore)
        {
            currentScore.text = score.ToString();
        }
        else {
            Debug.LogWarning("Varibles not assigned");
        }

        myAudioPlayer = transform.GetComponent<AudioSource>();

        if (!myAudioPlayer) {
            Debug.LogWarning("No Audio Source found");
        }
        if(MenuController.isContinue==true)
        {
            score = PlayerPrefs.GetInt("Score");
            currentScore.text = score.ToString();
            MenuController.isContinue = false;
        }

    }
	
	// Update is called once per frame
	void Update () {
        //Are we started
        if (!started) {
            //Sway when we are not started
            transform.position = new Vector3(defaultPosition.x, defaultPosition.y + Mathf.Sin(Time.time*3.0f) * 0.2f, defaultPosition.z);
        }
        //Tap and jump control and play sound
        if (Input.GetButtonDown("Fire1") && !isOver && started)
        {
            if (!myRigidbody) {
                return;
            }
            score++;
            myRigidbody.gravityScale = 1.0f;
            myRigidbody.velocity = new Vector2(0.0f,jumpForce);

            if (currentScore)
            {
                currentScore.text = score.ToString();
            }
            else {
                Debug.LogWarning("Varibles not assigned");
            }
            if (myAudioPlayer && jump)
            {
                myAudioPlayer.PlayOneShot(jump);
            }
            else {
                Debug.LogWarning("Please assign all the variables");
            }

        }
	}

    void Over()
    {
        //Game Over play death animation sound and show end panel

        Debug.Log("Over");
        myGenerator.enabled = false;
        myRigidbody.gravityScale = 0.0f;
        myRigidbody.velocity = Vector3.zero;
        if (myAnim) {
            myAnim.enabled = true;
        }
        isOver = true;
        PlayerPrefs.SetInt("Score", score);
        if (PlayerPrefs.GetInt("Best", 0) < score) {
            PlayerPrefs.SetInt("Best", score);
        }

        if (endBest)
        {
            endBest.text = PlayerPrefs.GetInt("Best", 0).ToString();
        }
        else {
            Debug.LogWarning("Varibles not assigned");
        }

        if (sessionScore) {
            sessionScore.text = score.ToString();
        }
        else {
            Debug.LogWarning("Varibles not assigned");
        }

        if (myAudioPlayer && crash)
        {
            myAudioPlayer.PlayOneShot(crash);
        }
        else {
            Debug.LogWarning("Please assign all the variables");
        }

        Invoke("EnableEndPanel", 1.0f);

    }
    void EnableEndPanel() {
        //Enable end panel

        if (currentScore)
        {
            currentScore.enabled = false;
        }
        else {
            Debug.LogWarning("Varibles not assigned");
        }

        if (EndPanel)
        {
            EndPanel.SetActive(true);
            ShowInterstitialAd("1lcaf5895d5l1293dc",
                () => {
                    Debug.LogError("--插屏广告完成--");

                },
                (it, str) => {
                    Debug.LogError("Error->" + str);
                });
        }
        else {
            Debug.LogError("Please Assign all the variables");
        }
    }
    public void StartTheGame() {
        //Start the game

        if (!started)
        {
            started = true;
            if (myGenerator)
                myGenerator.enabled = true;
            else
                Debug.LogError("Please Assign all the variables");
        }
        if (!myRigidbody)
        {
            return;
        }
        if (currentScore)
        {
            currentScore.enabled = true;
        }
        else {
            Debug.LogWarning("Varibles not assigned");
        }

        if (myAudioPlayer && jump)
        {
            myAudioPlayer.PlayOneShot(jump);
        }
        else {
            Debug.LogWarning("Please assign all the variables");
        }

        myRigidbody.gravityScale = 1.0f;
        myRigidbody.velocity = new Vector2(0.0f, jumpForce);
    }


    /// <summary>
    /// 播放插屏广告
    /// </summary>
    /// <param name="adId"></param>
    /// <param name="errorCallBack"></param>
    /// <param name="closeCallBack"></param>
    public void ShowInterstitialAd(string adId, System.Action closeCallBack, System.Action<int, string> errorCallBack)
    {
        starkAdManager = StarkSDK.API.GetStarkAdManager();
        if (starkAdManager != null)
        {
            var mInterstitialAd = starkAdManager.CreateInterstitialAd(adId, errorCallBack, closeCallBack);
            mInterstitialAd.Load();
            mInterstitialAd.Show();
        }
    }

    public void Restert()
    {
        SceneManager.LoadScene("Main");
    }
}
