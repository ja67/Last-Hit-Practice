using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


namespace LastHitPractice
{
    public class GameController : MonoBehaviour
    {
        [HideInInspector] public Camera cam;
        [HideInInspector] public SoundManager SoundManager;
        public GameObject infantry;
        public static GameController instance;

        [HideInInspector] public float timeLeft;
        [HideInInspector] public int lastHitCount;

        public Player player;
        [HideInInspector] public List<GameObject> friendlyObjectList;
        [HideInInspector] public List<GameObject> enemyObjectList;
        [HideInInspector] public I18n i18n;


        private const int numOfSpawnCreep = 3;

        public Text CurrentLanguageText;
        public Button PrevLanguageButton;
        public Button NextLanguageButton;

        public Slider MusicControllerSlider;
        public Button MusicMuteButton;
        public Slider SoundControllerSlider;
        public Button SoundMuteButton;

        public Sprite MuteSprite;
        public Sprite UnmuteSprite;

        public Text GameOverText;
        public Text HitText;
        public Text TimerText;
        public Text LanguageText;
        public Text MusicControllerText;
        public Text SoundControllerText;
        public Text HelpButtonText;
        public Text RestartButtonText;
        public Text QuitButtonText;


        // Use this for initialization

        private void Awake()
        {
            //Check if there is already an instance of SoundManager
            if (instance == null)
                //if not, set it to this.
                instance = this;
            //If instance already exists:
            else if (instance != this)
                //Destroy this, this enforces our singleton pattern so there can only be one instance of GameController.
                Destroy(gameObject);

            //Set GameController to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
            DontDestroyOnLoad(gameObject);
        }
        void Start()
        {
            if (cam == null)
            {
                cam = Camera.main;
            }
            friendlyObjectList = new List<GameObject>();
            enemyObjectList = new List<GameObject>();
            i18n = I18n.Instance;
            InitLanguageController();
            InitMusicSoundController();
            StartCoroutine(Spawn());
            UpdateText();
            CurrentLanguageText.text = i18n.__(I18n.CurrentLocale);
        }

        private void InitMusicSoundController()
        {
            MusicControllerSlider.value = 1f;
            SoundControllerSlider.value = 1f;
            MusicControllerSlider.onValueChanged.AddListener(delegate { MusicSoundSliderHandler(SoundManager.Controller.Music, MusicControllerSlider.value); });
            SoundControllerSlider.onValueChanged.AddListener(delegate { MusicSoundSliderHandler(SoundManager.Controller.Sound,SoundControllerSlider.value); });
            MusicMuteButton.onClick.AddListener(delegate { MusicSoundMuteButtonHandler(SoundManager.Controller.Music); });
            SoundMuteButton.onClick.AddListener(delegate { MusicSoundMuteButtonHandler(SoundManager.Controller.Sound); });
        }

        private void InitLanguageController()
        {
            PrevLanguageButton.onClick.AddListener(delegate { SwitchLanguageButtonHandler(false); });
            NextLanguageButton.onClick.AddListener(delegate { SwitchLanguageButtonHandler(true); });
        }

        // Update is called once physics timestamp
        private void FixedUpdate()
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft < 0)
            {
                timeLeft = 0;
            }
            UpdateDynamicText();
            foreach (GameObject friend in friendlyObjectList)
            {
                Dictionary<GameObject, int> friendAggroMap = friend.GetComponent<Infantry>().AggroMap;
                foreach (GameObject enemy in enemyObjectList)
                {
                    Dictionary<GameObject, int> enemyAggroMap = enemy.GetComponent<Infantry>().AggroMap;
                    if (!friendAggroMap.ContainsKey(enemy))
                    {
                        int initAggro = Mathf.RoundToInt(Mathf.Abs((enemy.gameObject.transform.position - friend.gameObject.transform.position).magnitude));
                        friendAggroMap[enemy] = initAggro;
                        enemyAggroMap[friend] = initAggro;
                    }
                }
            }
            foreach (GameObject friend in friendlyObjectList)
            {
                Infantry friendInfantry = friend.GetComponent<Infantry>();
                friendInfantry.targetEnemy = friendInfantry.AggroMap.FirstOrDefault(x => x.Value == friendInfantry.AggroMap.Values.Max()).Key;
            }
            foreach (GameObject enemy in enemyObjectList)
            {
                Infantry enemyInfantry = enemy.GetComponent<Infantry>();
                enemyInfantry.targetEnemy = enemyInfantry.AggroMap.FirstOrDefault(x => x.Value == enemyInfantry.AggroMap.Values.Max()).Key;
            }
        }

        private void UpdateText()
        {
            Text[] textArray = {
                GameOverText, HitText, LanguageText,
                MusicControllerText, SoundControllerText,
                HelpButtonText, RestartButtonText, QuitButtonText
            };
            foreach(Text text in textArray)
            {
                text.text = i18n.__(Regex.Replace(text.name, "( |Text$)", string.Empty));
            }
            TimerText.text = i18n.__("Timer") + Mathf.RoundToInt(timeLeft);
        }

        private void UpdateDynamicText()
        {
                TimerText.text = i18n.__("Timer") + Mathf.RoundToInt(timeLeft);
        }


        Infantry SpawnSingleCreep(Vector3 spawnPosition, int teamId)
        {
            GameObject newInfantryObj = Instantiate(infantry, spawnPosition, Quaternion.identity);
            Infantry newInfantry = newInfantryObj.GetComponent<Infantry>() as Infantry;
            newInfantry.TeamId = teamId;
            return newInfantry;

        }

        // Update is called once per frame
        void Update()
        {

            if (Input.GetMouseButtonDown(1))
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
                if (hit.collider != null)
                {
                    player.targetEnemy = hit.transform.gameObject;
                }
                else
                {
                    player.targetEnemy = null;
                    player.targetPosition = mousePos;
                }
            }
        }

        IEnumerator Spawn()
        {
            for (int i = 0; i < numOfSpawnCreep; i++)
            {
                //Spawn Friendly Creep
                Vector2 pos = new Vector2(Infantry.xLimit, Random.Range(-Infantry.yLimit, Infantry.yLimit));
                friendlyObjectList.Add(SpawnSingleCreep(pos, 1).gameObject);
                //Spawn Enemy Creep
                pos = new Vector2(-Infantry.xLimit, Random.Range(-Infantry.yLimit, Infantry.yLimit));
                enemyObjectList.Add(SpawnSingleCreep(pos, 2).gameObject);
            }
            yield return new WaitForSeconds(30);
        }

        public void MusicSoundSliderHandler(SoundManager.Controller controller, float value)
        {
            if(controller == SoundManager.Controller.Music)
            {
                SoundManager.musicSource.volume = value; 
            }
            else if(controller == SoundManager.Controller.Sound)
            {
                SoundManager.soundSource.volume = value; 
            }
        }

        public void MusicSoundMuteButtonHandler(SoundManager.Controller controller)
        {
            AudioSource source = null;
            Slider volumeSlider = null;
            Button muteButton = null;
            if(controller == SoundManager.Controller.Music)
            {
                source = SoundManager.musicSource;
                volumeSlider = MusicControllerSlider;
                muteButton = MusicMuteButton;
            }
            else if(controller == SoundManager.Controller.Sound)
            {
                source = SoundManager.soundSource; 
                volumeSlider = SoundControllerSlider;
                muteButton = SoundMuteButton;
            }
            source.mute = !source.mute;
            volumeSlider.interactable = !source.mute;
            if (source.mute)
            {
                muteButton.image.sprite = MuteSprite;
            }
            else
            {
                muteButton.image.sprite = UnmuteSprite;
            }
        }

        public void SwitchLanguageButtonHandler(bool isNext)
        {
            i18n.NextLanguage(isNext);
            CurrentLanguageText.text = i18n.__(I18n.CurrentLocale);
            UpdateText();
        }
    }
}
