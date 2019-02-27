using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

namespace LastHitPractice
{
    public class StartScreenController : MonoBehaviour
    {

        [HideInInspector] public Camera cam;
        [HideInInspector] public I18n i18n;
        public static StartScreenController instance;

        public Text TitleText;
        public Text StartButtonText;

        // Start is called before the first frame update
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
        }
        void Start()
        {
            if (cam == null)
            {
                cam = Camera.main;
            }
            i18n = I18n.Instance;

            foreach (Text text in new[]{ TitleText, StartButtonText})
            {
                text.text = i18n.__(Regex.Replace(text.name, "( |Text$)", string.Empty));
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void StartGame()
        {
            SceneManager.LoadScene("Main");
        }
    }
}
