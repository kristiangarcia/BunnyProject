using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using BunnyGame.Core;
using BunnyGame.Managers;

namespace BunnyGame.UI
{
    /// <summary>
    /// Panel que se muestra cuando termina la partida (por tiempo agotado o nivel completado)
    /// Permite al jugador ingresar su nombre y enviar su puntuación al leaderboard
    /// </summary>
    public class GameOverPanel : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshProUGUI tituloTexto;
        [SerializeField] private TextMeshProUGUI estrellasTexto;
        [SerializeField] private TextMeshProUGUI tiempoTexto;
        [SerializeField] private TextMeshProUGUI puntuacionTexto;
        [SerializeField] private TMP_InputField nombreInputField;
        [SerializeField] private Button enviarScoreButton;
        [SerializeField] private Button reintentarButton;
        [SerializeField] private Button verRankingsButton;
        [SerializeField] private Button menuPrincipalButton;
        [SerializeField] private TextMeshProUGUI mensajeTexto;

        private int currentStars;
        private float currentTimeRemaining;
        private int currentScore;
        private bool scoreSent = false;

        private void Start()
        {
            // Ocultar panel al inicio
            if (panel != null)
                panel.SetActive(false);

            // Configurar botones
            if (enviarScoreButton != null)
                enviarScoreButton.onClick.AddListener(OnEnviarScore);

            if (reintentarButton != null)
                reintentarButton.onClick.AddListener(OnReintentar);

            if (verRankingsButton != null)
                verRankingsButton.onClick.AddListener(OnVerRankings);

            if (menuPrincipalButton != null)
                menuPrincipalButton.onClick.AddListener(OnMenuPrincipal);

            // Cargar nombre guardado
            if (nombreInputField != null)
            {
                string savedName = LeaderboardManager.Instance.GetSavedPlayerName();
                if (!string.IsNullOrEmpty(savedName))
                    nombreInputField.text = savedName;
            }

            // Suscribirse a eventos del LeaderboardManager
            LeaderboardManager.Instance.OnSubmitSuccess += OnSubmitSuccess;
            LeaderboardManager.Instance.OnSubmitError += OnSubmitError;
        }

        private void OnDestroy()
        {
            if (LeaderboardManager.Instance != null)
            {
                LeaderboardManager.Instance.OnSubmitSuccess -= OnSubmitSuccess;
                LeaderboardManager.Instance.OnSubmitError -= OnSubmitError;
            }
        }

        /// <summary>
        /// Muestra el panel de Game Over con la información de la partida
        /// </summary>
        /// <param name="stars">Estrellas recolectadas</param>
        /// <param name="timeRemaining">Tiempo transcurrido (NO restante)</param>
        /// <param name="timeExpired">Si terminó por tiempo agotado</param>
        public void ShowGameOver(int stars, float timeRemaining, bool timeExpired)
        {
            currentStars = stars;
            currentTimeRemaining = timeRemaining;
            currentScore = LeaderboardManager.Instance.CalculateScore(stars, timeRemaining);
            scoreSent = false;

            // Mostrar panel
            if (panel != null)
                panel.SetActive(true);

            // Pausar el juego
            Time.timeScale = 0f;

            // Actualizar titulo
            if (tituloTexto != null)
            {
                tituloTexto.text = timeExpired ? "TIEMPO AGOTADO!" : "NIVEL COMPLETADO!";
            }

            // Mostrar estadisticas
            if (estrellasTexto != null)
                estrellasTexto.text = $"Estrellas: {stars}";

            if (tiempoTexto != null)
            {
                int minutes = Mathf.FloorToInt(timeRemaining / 60);
                int seconds = Mathf.FloorToInt(timeRemaining % 60);
                tiempoTexto.text = $"Tiempo: {minutes:00}:{seconds:00}";
            }

            if (puntuacionTexto != null)
                puntuacionTexto.text = $"Puntuacion: {currentScore}";

            // Limpiar mensaje
            if (mensajeTexto != null)
                mensajeTexto.text = "";

            // Habilitar botón de enviar
            if (enviarScoreButton != null)
                enviarScoreButton.interactable = true;

            Debug.Log($"Game Over - Stars: {stars}, Time: {timeRemaining}s, Score: {currentScore}");
        }

        private void OnEnviarScore()
        {
            if (scoreSent)
            {
                MostrarMensaje("Score ya enviado", Color.yellow);
                return;
            }

            if (nombreInputField == null || string.IsNullOrWhiteSpace(nombreInputField.text))
            {
                MostrarMensaje("Por favor ingresa tu nombre", Color.red);
                return;
            }

            string playerName = nombreInputField.text.Trim();

            // Validar longitud
            if (playerName.Length < 3 || playerName.Length > 15)
            {
                MostrarMensaje("Nombre debe tener 3-15 caracteres", Color.red);
                return;
            }

            // Deshabilitar botón mientras se envía
            if (enviarScoreButton != null)
                enviarScoreButton.interactable = false;

            MostrarMensaje("Enviando score...", Color.white);

            // Enviar score
            LeaderboardManager.Instance.SubmitScore(playerName, currentStars, currentTimeRemaining);
        }

        private void OnSubmitSuccess()
        {
            scoreSent = true;
            MostrarMensaje("Score enviado exitosamente!", Color.green);

            if (enviarScoreButton != null)
                enviarScoreButton.interactable = false;
        }

        private void OnSubmitError(string error)
        {
            MostrarMensaje($"Error: {error}", Color.red);

            if (enviarScoreButton != null)
                enviarScoreButton.interactable = true;
        }

        private void MostrarMensaje(string mensaje, Color color)
        {
            if (mensajeTexto != null)
            {
                mensajeTexto.text = mensaje;
                mensajeTexto.color = color;
            }

            Debug.Log($"GameOverPanel: {mensaje}");
        }

        private void OnReintentar()
        {
            // Reanudar tiempo
            Time.timeScale = 1f;

            // Recargar escena actual
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void OnVerRankings()
        {
            // Reanudar tiempo
            Time.timeScale = 1f;

            // Ocultar este panel
            if (panel != null)
                panel.SetActive(false);

            // Buscar y mostrar LeaderboardPanel (incluir inactivos)
            LeaderboardPanel leaderboardPanel = FindObjectOfType<LeaderboardPanel>(true);
            if (leaderboardPanel != null)
            {
                leaderboardPanel.ShowLeaderboard();
            }
            else
            {
                Debug.LogWarning("LeaderboardPanel no encontrado en la escena");
            }
        }

        private void OnMenuPrincipal()
        {
            // Reanudar tiempo
            Time.timeScale = 1f;

            // Cargar escena de menú principal
            SceneManager.LoadScene(GameConstants.SCENE_MAIN_MENU);
        }
    }
}
