using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using System.Collections.Generic;
using System.Linq;

namespace BunnyGame.UI
{
    /// <summary>
    /// Controlador del menú de opciones
    /// Gestiona configuraciones de audio y video
    /// </summary>
    public class OptionsMenu : MonoBehaviour
    {
        [Header("Audio Mixer")]
        [SerializeField] private AudioMixer audioMixer;

        [Header("Controles de Audio")]
        [SerializeField] private Slider volumenMaestroSlider;
        [SerializeField] private Slider volumenMusicaSlider;
        [SerializeField] private Slider volumenEfectosSlider;
        [SerializeField] private TextMeshProUGUI volumenMaestroTexto;
        [SerializeField] private TextMeshProUGUI volumenMusicaTexto;
        [SerializeField] private TextMeshProUGUI volumenEfectosTexto;

        [Header("Controles de Video")]
        [SerializeField] private TMP_Dropdown resolucionDropdown;
        [SerializeField] private Toggle pantallaCompletaToggle;
        [SerializeField] private TMP_Dropdown calidadDropdown;

        private Resolution[] resoluciones;
        private SettingsManager settingsManager;

        private void Start()
        {
            settingsManager = SettingsManager.Instance;

            ConfigurarControlesAudio();
            ConfigurarControlesVideo();
            CargarConfiguracion();
        }

        private void ConfigurarControlesAudio()
        {
            // Configurar sliders de volumen
            if (volumenMaestroSlider != null)
            {
                volumenMaestroSlider.minValue = 0f;
                volumenMaestroSlider.maxValue = 1f;
                volumenMaestroSlider.onValueChanged.AddListener(CambiarVolumenMaestro);
            }

            if (volumenMusicaSlider != null)
            {
                volumenMusicaSlider.minValue = 0f;
                volumenMusicaSlider.maxValue = 1f;
                volumenMusicaSlider.onValueChanged.AddListener(CambiarVolumenMusica);
            }

            if (volumenEfectosSlider != null)
            {
                volumenEfectosSlider.minValue = 0f;
                volumenEfectosSlider.maxValue = 1f;
                volumenEfectosSlider.onValueChanged.AddListener(CambiarVolumenEfectos);
            }
        }

        private void ConfigurarControlesVideo()
        {
            // Configurar resoluciones
            if (resolucionDropdown != null)
            {
                resolucionDropdown.ClearOptions();

                resoluciones = Screen.resolutions
                    .Where(r => r.refreshRateRatio.value >= 60) // Filtrar resoluciones con al menos 60Hz
                    .Distinct()
                    .ToArray();

                List<string> opciones = new List<string>();

                for (int i = 0; i < resoluciones.Length; i++)
                {
                    string opcion = resoluciones[i].width + " x " + resoluciones[i].height;
                    opciones.Add(opcion);
                }

                resolucionDropdown.AddOptions(opciones);

                // Seleccionar la resolución actual
                Resolution resolucionActual = Screen.currentResolution;
                for (int i = 0; i < resoluciones.Length; i++)
                {
                    if (resoluciones[i].width == resolucionActual.width &&
                        resoluciones[i].height == resolucionActual.height)
                    {
                        resolucionDropdown.value = i;
                        break;
                    }
                }

                resolucionDropdown.onValueChanged.AddListener(CambiarResolucion);
            }

            // Configurar pantalla completa
            if (pantallaCompletaToggle != null)
            {
                pantallaCompletaToggle.isOn = Screen.fullScreen;
                pantallaCompletaToggle.onValueChanged.AddListener(CambiarPantallaCompleta);
            }

            // Configurar calidad
            if (calidadDropdown != null)
            {
                calidadDropdown.ClearOptions();
                calidadDropdown.AddOptions(QualitySettings.names.ToList());
                calidadDropdown.value = QualitySettings.GetQualityLevel();
                calidadDropdown.onValueChanged.AddListener(CambiarCalidad);
            }
        }

        private void CargarConfiguracion()
        {
            if (settingsManager == null) return;

            // Cargar volúmenes
            if (volumenMaestroSlider != null)
                volumenMaestroSlider.value = settingsManager.VolumenMaestro;

            if (volumenMusicaSlider != null)
                volumenMusicaSlider.value = settingsManager.VolumenMusica;

            if (volumenEfectosSlider != null)
                volumenEfectosSlider.value = settingsManager.VolumenEfectos;

            // Actualizar textos
            ActualizarTextoVolumen();
        }

        public void CambiarVolumenMaestro(float volumen)
        {
            if (settingsManager != null)
                settingsManager.VolumenMaestro = volumen;

            if (audioMixer != null)
            {
                // Convertir de rango 0-1 a decibelios (-80 a 0)
                float db = volumen > 0 ? Mathf.Log10(volumen) * 20 : -80f;
                audioMixer.SetFloat("Master", db);
            }

            ActualizarTextoVolumen();
        }

        public void CambiarVolumenMusica(float volumen)
        {
            if (settingsManager != null)
                settingsManager.VolumenMusica = volumen;

            if (audioMixer != null)
            {
                float db = volumen > 0 ? Mathf.Log10(volumen) * 20 : -80f;
                audioMixer.SetFloat("Music", db);
            }

            ActualizarTextoVolumen();
        }

        public void CambiarVolumenEfectos(float volumen)
        {
            if (settingsManager != null)
                settingsManager.VolumenEfectos = volumen;

            if (audioMixer != null)
            {
                float db = volumen > 0 ? Mathf.Log10(volumen) * 20 : -80f;
                audioMixer.SetFloat("SFX", db);
            }

            ActualizarTextoVolumen();
        }

        private void ActualizarTextoVolumen()
        {
            if (volumenMaestroTexto != null && volumenMaestroSlider != null)
                volumenMaestroTexto.text = Mathf.RoundToInt(volumenMaestroSlider.value * 100) + "%";

            if (volumenMusicaTexto != null && volumenMusicaSlider != null)
                volumenMusicaTexto.text = Mathf.RoundToInt(volumenMusicaSlider.value * 100) + "%";

            if (volumenEfectosTexto != null && volumenEfectosSlider != null)
                volumenEfectosTexto.text = Mathf.RoundToInt(volumenEfectosSlider.value * 100) + "%";
        }

        public void CambiarResolucion(int indiceResolucion)
        {
            if (resoluciones == null || indiceResolucion >= resoluciones.Length) return;

            Resolution resolucion = resoluciones[indiceResolucion];
            Screen.SetResolution(resolucion.width, resolucion.height, Screen.fullScreen);

            if (settingsManager != null)
            {
                settingsManager.ResolucionAncho = resolucion.width;
                settingsManager.ResolucionAlto = resolucion.height;
            }
        }

        public void CambiarPantallaCompleta(bool pantallaCompleta)
        {
            Screen.fullScreen = pantallaCompleta;

            if (settingsManager != null)
                settingsManager.PantallaCompleta = pantallaCompleta;
        }

        public void CambiarCalidad(int indiceCalidad)
        {
            QualitySettings.SetQualityLevel(indiceCalidad);
            Debug.Log($"Calidad cambiada a: {QualitySettings.names[indiceCalidad]} (Nivel {indiceCalidad})");

            if (settingsManager != null)
                settingsManager.NivelCalidad = indiceCalidad;
        }

        public void AplicarCambios()
        {
            if (settingsManager != null)
                settingsManager.GuardarConfiguracion();

            Debug.Log("Configuración guardada");
        }

        public void RestaurarPorDefecto()
        {
            if (settingsManager != null)
            {
                settingsManager.RestaurarPorDefecto();
                CargarConfiguracion();
            }

            // Restaurar controles de video
            if (pantallaCompletaToggle != null)
                pantallaCompletaToggle.isOn = true;

            if (calidadDropdown != null)
                calidadDropdown.value = QualitySettings.GetQualityLevel();

            Debug.Log("Configuración restaurada a valores por defecto");
        }
    }
}
