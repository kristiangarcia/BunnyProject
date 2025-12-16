using UnityEngine;
using UnityEngine.Audio;

namespace BunnyGame.UI
{
    /// <summary>
    /// Gestor de configuraciones del juego
    /// Singleton que persiste entre escenas y guarda/carga configuraciones
    /// </summary>
    public class SettingsManager : MonoBehaviour
    {
        [Header("Audio Mixer")]
        [SerializeField] private AudioMixer audioMixer;
        private static SettingsManager instance;
        public static SettingsManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("SettingsManager");
                    instance = go.AddComponent<SettingsManager>();
                    DontDestroyOnLoad(go);
                    instance.CargarConfiguracion();
                }
                return instance;
            }
        }

        [Header("Configuración de Audio")]
        private float volumenMaestro = 1f;
        private float volumenMusica = 0.8f;
        private float volumenEfectos = 1f;

        [Header("Configuración de Video")]
        private int resolucionAncho = 1920;
        private int resolucionAlto = 1080;
        private bool pantallaCompleta = true;
        private int nivelCalidad = 2;

        // Propiedades de Audio
        public float VolumenMaestro
        {
            get => volumenMaestro;
            set
            {
                volumenMaestro = Mathf.Clamp01(value);
                PlayerPrefs.SetFloat("VolumenMaestro", volumenMaestro);
            }
        }

        public float VolumenMusica
        {
            get => volumenMusica;
            set
            {
                volumenMusica = Mathf.Clamp01(value);
                PlayerPrefs.SetFloat("VolumenMusica", volumenMusica);
            }
        }

        public float VolumenEfectos
        {
            get => volumenEfectos;
            set
            {
                volumenEfectos = Mathf.Clamp01(value);
                PlayerPrefs.SetFloat("VolumenEfectos", volumenEfectos);
            }
        }

        // Propiedades de Video
        public int ResolucionAncho
        {
            get => resolucionAncho;
            set
            {
                resolucionAncho = value;
                PlayerPrefs.SetInt("ResolucionAncho", resolucionAncho);
            }
        }

        public int ResolucionAlto
        {
            get => resolucionAlto;
            set
            {
                resolucionAlto = value;
                PlayerPrefs.SetInt("ResolucionAlto", resolucionAlto);
            }
        }

        public bool PantallaCompleta
        {
            get => pantallaCompleta;
            set
            {
                pantallaCompleta = value;
                PlayerPrefs.SetInt("PantallaCompleta", pantallaCompleta ? 1 : 0);
            }
        }

        public int NivelCalidad
        {
            get => nivelCalidad;
            set
            {
                nivelCalidad = value;
                PlayerPrefs.SetInt("NivelCalidad", nivelCalidad);
            }
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                CargarConfiguracion();
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            // Aplicar configuración en Start para asegurar que todo está inicializado
            AplicarConfiguracion();
        }

        public void CargarConfiguracion()
        {
            // Cargar audio
            volumenMaestro = PlayerPrefs.GetFloat("VolumenMaestro", 1f);
            volumenMusica = PlayerPrefs.GetFloat("VolumenMusica", 0.8f);
            volumenEfectos = PlayerPrefs.GetFloat("VolumenEfectos", 1f);

            // Cargar video
            resolucionAncho = PlayerPrefs.GetInt("ResolucionAncho", Screen.currentResolution.width);
            resolucionAlto = PlayerPrefs.GetInt("ResolucionAlto", Screen.currentResolution.height);
            pantallaCompleta = PlayerPrefs.GetInt("PantallaCompleta", 1) == 1;
            nivelCalidad = PlayerPrefs.GetInt("NivelCalidad", QualitySettings.GetQualityLevel());
        }

        public void AplicarConfiguracion()
        {
            // Aplicar audio
            if (audioMixer != null)
            {
                float dbMaestro = volumenMaestro > 0 ? Mathf.Log10(volumenMaestro) * 20 : -80f;
                float dbMusica = volumenMusica > 0 ? Mathf.Log10(volumenMusica) * 20 : -80f;
                float dbEfectos = volumenEfectos > 0 ? Mathf.Log10(volumenEfectos) * 20 : -80f;

                audioMixer.SetFloat("Master", dbMaestro);
                audioMixer.SetFloat("Music", dbMusica);
                audioMixer.SetFloat("SFX", dbEfectos);
            }

            // Aplicar video
            Screen.SetResolution(resolucionAncho, resolucionAlto, pantallaCompleta);
            QualitySettings.SetQualityLevel(nivelCalidad);
        }

        public void GuardarConfiguracion()
        {
            PlayerPrefs.Save();
            Debug.Log("Configuración guardada");
        }

        public void RestaurarPorDefecto()
        {
            volumenMaestro = 1f;
            volumenMusica = 0.8f;
            volumenEfectos = 1f;
            resolucionAncho = Screen.currentResolution.width;
            resolucionAlto = Screen.currentResolution.height;
            pantallaCompleta = true;
            nivelCalidad = 2;

            PlayerPrefs.SetFloat("VolumenMaestro", volumenMaestro);
            PlayerPrefs.SetFloat("VolumenMusica", volumenMusica);
            PlayerPrefs.SetFloat("VolumenEfectos", volumenEfectos);
            PlayerPrefs.SetInt("ResolucionAncho", resolucionAncho);
            PlayerPrefs.SetInt("ResolucionAlto", resolucionAlto);
            PlayerPrefs.SetInt("PantallaCompleta", 1);
            PlayerPrefs.SetInt("NivelCalidad", nivelCalidad);

            AplicarConfiguracion();
            GuardarConfiguracion();

            Debug.Log("Configuración restaurada a valores por defecto");
        }
    }
}
