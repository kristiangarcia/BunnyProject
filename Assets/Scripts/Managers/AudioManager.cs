using UnityEngine;
using UnityEngine.Audio;

namespace BunnyGame.Managers
{
    /// <summary>
    /// Gestor de audio centralizado con patrón Singleton
    /// Maneja música de fondo y efectos de sonido
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("Audio Clips")]
        [SerializeField] private AudioClip backgroundMusic;
        [SerializeField] private AudioClip[] sfxClips;

        [Header("Mixer Groups")]
        [SerializeField] private AudioMixerGroup musicGroup;
        [SerializeField] private AudioMixerGroup sfxGroup;

        private const float DEFAULT_PITCH = 1.0f;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        void Start()
        {
            InicializarAudio();
        }

        private void InicializarAudio()
        {
            // Asignar mixer groups
            if (musicSource != null && musicGroup != null)
                musicSource.outputAudioMixerGroup = musicGroup;

            if (sfxSource != null && sfxGroup != null)
                sfxSource.outputAudioMixerGroup = sfxGroup;

            // Reproducir música de fondo
            if (musicSource != null && backgroundMusic != null)
            {
                musicSource.clip = backgroundMusic;
                musicSource.loop = true;
                musicSource.Play();
            }
        }

        #region SFX Methods

        public void PlaySFX(int index)
        {
            if (sfxSource == null)
            {
                Debug.LogWarning("SFX Source no está asignado en AudioManager");
                return;
            }

            if (index >= 0 && index < sfxClips.Length && sfxClips[index] != null)
            {
                sfxSource.PlayOneShot(sfxClips[index]);
            }
            else
            {
                Debug.LogWarning($"Índice de SFX {index} fuera de rango o clip nulo");
            }
        }

        #endregion

        #region Music Control Methods

        public void PararMusica()
        {
            if (musicSource != null)
                musicSource.Stop();
        }

        public void IniciarMusica()
        {
            if (musicSource != null && !musicSource.isPlaying)
                musicSource.Play();
        }

        public void PausarMusica()
        {
            if (musicSource != null)
                musicSource.Pause();
        }

        public void ReanudarMusica()
        {
            if (musicSource != null)
                musicSource.UnPause();
        }

        public void CambiarPitchMusica(float pitch)
        {
            if (musicSource != null)
                musicSource.pitch = pitch;
        }

        public void RestaurarPitchMusica()
        {
            if (musicSource != null)
                musicSource.pitch = DEFAULT_PITCH;
        }

        public void CambiarVolumenMusica(float volumen)
        {
            if (musicSource != null)
                musicSource.volume = Mathf.Clamp01(volumen);
        }

        public void RestaurarVolumenMusica()
        {
            if (musicSource != null)
                musicSource.volume = 1f;
        }

        #endregion

        #region Properties

        public bool MusicaReproduciendose => musicSource != null && musicSource.isPlaying;

        public float PitchActual => musicSource != null ? musicSource.pitch : DEFAULT_PITCH;

        #endregion
    }
}
