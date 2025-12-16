using UnityEngine;
using BunnyGame.UI;
using BunnyGame.Managers;

namespace BunnyGame.Core
{
    /// <summary>
    /// Detecta cuando el jugador llega al objetivo del nivel
    /// </summary>
    public class LevelGoal : MonoBehaviour
    {
        [Header("Configuración")]
        [SerializeField] private bool completarAlTocar = true;

        private bool nivelCompletado = false;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (nivelCompletado) return;

            if (collision.CompareTag(GameConstants.TAG_PLAYER))
            {
                CompletarNivel();
            }
        }

        private void CompletarNivel()
        {
            if (nivelCompletado) return;

            nivelCompletado = true;

            Debug.Log("<color=#00FF00>¡NIVEL COMPLETADO!</color>");

            // Obtener tiempo restante desde HUDController (mas tiempo restante = mas rapido = mas puntos)
            HUDController hudController = FindObjectOfType<HUDController>();
            float tiempoRestante = 0f;

            if (hudController != null)
            {
                tiempoRestante = hudController.GetTiempoRestante();
            }

            // Obtener estrellas del GameManager
            int stars = GameManager.Instance != null ? GameManager.Instance.Estrellas : 0;

            // Mostrar Game Over Panel
            GameOverPanel gameOverPanel = FindObjectOfType<GameOverPanel>(true);
            if (gameOverPanel != null)
            {
                gameOverPanel.ShowGameOver(stars, tiempoRestante, false); // timeExpired = false
            }
            else
            {
                Debug.LogError("GameOverPanel no encontrado en la escena");
            }
        }
    }
}
