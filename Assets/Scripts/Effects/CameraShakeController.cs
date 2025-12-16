using UnityEngine;
using Unity.Cinemachine;

namespace BunnyGame.Effects
{
    /// <summary>
    /// Controla el shake de la cámara activándolo/desactivándolo
    /// </summary>
    public class CameraShakeController : MonoBehaviour
    {
        [Header("Noise Profiles")]
        [SerializeField] private NoiseSettings wobbleProfile;
        [SerializeField] private NoiseSettings shakeProfile;

        // Componente de Cinemachine que controla el ruido/shake
        private CinemachineBasicMultiChannelPerlin perlinNoise;

        // Singleton para acceso fácil desde otros scripts
        public static CameraShakeController Instance { get; private set; }

        #region Unity Callbacks

        void Awake()
        {
            // Configurar singleton
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            // Obtener el componente de ruido Perlin de la cámara virtual
            CinemachineCamera virtualCamera = GetComponent<CinemachineCamera>();

            if (virtualCamera != null)
            {
                perlinNoise = virtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Noise) as CinemachineBasicMultiChannelPerlin;

                if (perlinNoise == null)
                {
                    Debug.LogError("No se encontró CinemachineBasicMultiChannelPerlin en la cámara. Añádelo manualmente.");
                }
            }
            else
            {
                Debug.LogError("CameraShakeController debe estar en el mismo GameObject que CinemachineCamera!");
            }
        }

        #endregion

        #region Shake Control

        /// <summary>
        /// Activa el wobble de la cámara (para powerups y countdown)
        /// </summary>
        /// <param name="amplitude">Intensidad del wobble (amplitud). Por defecto 0.05</param>
        /// <param name="frequency">Velocidad del wobble (frecuencia). Por defecto 0.8</param>
        public void ActivarWobble(float amplitude = 0.05f, float frequency = 0.8f)
        {
            if (perlinNoise != null)
            {
                if (wobbleProfile != null)
                {
                    perlinNoise.NoiseProfile = wobbleProfile;
                }
                perlinNoise.AmplitudeGain = amplitude;
                perlinNoise.FrequencyGain = frequency;
            }
        }

        /// <summary>
        /// Activa el shake de la cámara (para cuando el jugador muere)
        /// </summary>
        /// <param name="amplitude">Intensidad del shake (amplitud). Por defecto 1</param>
        /// <param name="frequency">Velocidad del shake (frecuencia). Por defecto 1</param>
        public void ActivarShake(float amplitude = 0.5f, float frequency = 1f)
        {
            if (perlinNoise != null)
            {
                if (shakeProfile != null)
                {
                    perlinNoise.NoiseProfile = shakeProfile;
                }
                perlinNoise.AmplitudeGain = amplitude;
                perlinNoise.FrequencyGain = frequency;
            }
        }

        /// <summary>
        /// Desactiva el shake de la cámara
        /// </summary>
        public void DesactivarShake()
        {
            if (perlinNoise != null)
            {
                perlinNoise.AmplitudeGain = 0f;
                perlinNoise.FrequencyGain = 0f;
            }
        }

        #endregion
    }
}
