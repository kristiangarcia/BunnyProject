using TMPro;
using UnityEngine;
using BunnyGame.Managers;
using BunnyGame.Player;
using BunnyGame.Core;
using BunnyGame.Effects;
using System.Collections;

namespace BunnyGame.UI
{
    /// <summary>
    /// Controlador del HUD que muestra estrellas y tiempo restante
    /// </summary>
    public class HUDController : MonoBehaviour
    {
        [Header("Referencias UI")]
        [SerializeField] private TextMeshProUGUI estrellasText;
        [SerializeField] private TextMeshProUGUI tiempoRestanteText;

        [Header("Referencias del Jugador")]
        [SerializeField] private PlayerHealth playerHealth;

        [Header("Configuración del Tiempo")]
        [SerializeField] private float tiempoInicial = 60f;
        [SerializeField][Range(0f, 1f)] private float volumenMusicaCuentaRegresiva = 0.3f;

        private float tiempoActual;
        private bool tiempoActivo = true;
        private bool tiempoTerminado = false;
        private int ultimoSegundoSonido = -1;
        private bool volumenReducido = false;

        #region Unity Callbacks

        void Start()
        {
            InicializarHUD();
        }

        void Update()
        {
            ActualizarEstrellas();
            ActualizarTiempo();
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Obtiene el tiempo restante (mayor = mas rapido completado = mas puntos)
        /// </summary>
        public float GetTiempoRestante()
        {
            return tiempoActual;
        }

        #endregion

        #region Inicialización

        private void InicializarHUD()
        {
            // Inicializar estrellas
            if (estrellasText != null)
                estrellasText.text = "0";

            // Inicializar tiempo
            tiempoActual = tiempoInicial;
            ActualizarTextoTiempo();
        }

        #endregion

        #region Actualización de UI

        private void ActualizarEstrellas()
        {
            if (GameManager.Instance != null && estrellasText != null)
            {
                estrellasText.text = GameManager.Instance.Estrellas.ToString();
            }
        }

        private void ActualizarTiempo()
        {
            if (!tiempoActivo) return;

            if (tiempoActual <= 0)
            {
                // Si el tiempo llegó a 0 y no se ha procesado aún
                if (!tiempoTerminado)
                {
                    tiempoTerminado = true;
                    // Ya no llamar a TiempoAgotado aquí, se maneja en ReproducirSonidoCuentaRegresiva
                }
                return;
            }

            tiempoActual -= Time.deltaTime;
            if (tiempoActual < 0)
                tiempoActual = 0;

            // Reproducir sonido de cuenta regresiva cuando quedan 5 segundos o menos
            ReproducirSonidoCuentaRegresiva();

            ActualizarTextoTiempo();
        }

        private void ReproducirSonidoCuentaRegresiva()
        {
            // Detectar en qué segundo estamos (5.9→5, 4.5→4, 1.1→1, 0.5→0)
            int segundoActual = Mathf.FloorToInt(tiempoActual);

            // Reducir volumen de música cuando empiece la cuenta regresiva (específicamente en el segundo 5)
            if (!volumenReducido && segundoActual == 5)
            {
                volumenReducido = true;
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.CambiarVolumenMusica(volumenMusicaCuentaRegresiva);
                }

                // Activar wobble de cámara cuando empieza la cuenta regresiva
                if (CameraShakeController.Instance != null)
                {
                    CameraShakeController.Instance.ActivarWobble();
                }
            }

            // Reproducir sonido si estamos en los segundos 5, 4, 3, 2 o 1
            // y es diferente al último segundo que sonó
            if (segundoActual >= 1 && segundoActual <= 5 && segundoActual != ultimoSegundoSonido)
            {
                ultimoSegundoSonido = segundoActual;

                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySFX(GameConstants.SFX_COUNTDOWN);
                }
            }
            // Detectar cuando llega al segundo 0 (tiempoActual entre 0.01 y 0.99)
            else if (tiempoActual > 0 && segundoActual == 0 && ultimoSegundoSonido != 0)
            {
                ultimoSegundoSonido = 0;
                tiempoTerminado = true;

                // Parar música y reproducir sonido final
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PararMusica();
                    AudioManager.Instance.PlaySFX(GameConstants.SFX_TIME_UP);
                }

                // Pausar el juego e iniciar corrutina para despausar y matar
                StartCoroutine(EsperarYMatarJugador());
            }
        }

        private IEnumerator EsperarYMatarJugador()
        {
            Debug.Log("<color=#FF0000>¡Tiempo agotado! Pausando juego...</color>");

            // Pausar el juego DESPUÉS de reproducir el sonido
            Time.timeScale = 0f;

            // Esperar 1 segundo en tiempo real (ignora timeScale)
            yield return new WaitForSecondsRealtime(1f);

            Debug.Log("<color=#FFAA00>Despausando y matando jugador...</color>");

            // Despausar el juego antes de matar al jugador
            Time.timeScale = 1f;

            // Matar al jugador (comportamiento original)
            if (playerHealth != null)
            {
                playerHealth.Die();
            }
            else
            {
                Debug.LogWarning("No se asignó PlayerHealth en HUDController!");
            }
        }

        private void ActualizarTextoTiempo()
        {
            if (tiempoRestanteText == null) return;

            int minutos = Mathf.FloorToInt(tiempoActual / 60f);
            int segundos = Mathf.FloorToInt(tiempoActual % 60f);
            tiempoRestanteText.text = string.Format("{0:00}:{1:00}", minutos, segundos);
        }

        #endregion

        #region Control de Tiempo

        public void ReiniciarTiempo()
        {
            tiempoActual = tiempoInicial;
            tiempoActivo = true;
            tiempoTerminado = false;
            ultimoSegundoSonido = -1;
            volumenReducido = false;

            // Asegurar que el juego esté despausado
            Time.timeScale = 1f;

            // Detener la corrutina de muerte si está activa
            StopAllCoroutines();

            // Restaurar volumen normal de la música
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.RestaurarVolumenMusica();
            }

            // Desactivar shake de cámara al reiniciar
            if (CameraShakeController.Instance != null)
            {
                CameraShakeController.Instance.DesactivarShake();
            }

            ActualizarTextoTiempo();
            Debug.Log("<color=#00FFFF>Tiempo reiniciado!</color>");
        }

        public void PausarTiempo()
        {
            tiempoActivo = false;
        }

        public void ReanudarTiempo()
        {
            tiempoActivo = true;
        }

        public void AgregarTiempo(float segundos)
        {
            tiempoActual += segundos;
            ActualizarTextoTiempo();
        }

        #endregion

        #region Properties

        public float TiempoActual => tiempoActual;
        public bool TiempoTerminado => tiempoActual <= 0;

        #endregion
    }
}
