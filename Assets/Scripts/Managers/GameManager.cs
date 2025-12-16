using UnityEngine;
using BunnyGame.Core;

namespace BunnyGame.Managers
{
    /// <summary>
    /// Gestor principal del juego que maneja el estado del juego y colectables
    /// Renombrado de SceneManager para evitar conflictos con UnityEngine.SceneManagement.SceneManager
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        private int estrellas = 0;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void ObtenerEstrella()
        {
            estrellas++;
            Debug.Log($"<color=#FFFF00>Estrellas obtenidas: {estrellas}</color>");
        }

        public void ResetearEstrellas()
        {
            estrellas = 0;
            Debug.Log("<color=#FFAA00>Estrellas reseteadas a 0</color>");
        }

        public int Estrellas => estrellas;

        /// <summary>
        /// Calcula la puntuación final basada en estrellas y tiempo restante
        /// </summary>
        public int CalcularPuntuacion(int estrellas, float tiempoRestante)
        {
            int puntosPorEstrellas = estrellas * GameConstants.STARS_SCORE_MULTIPLIER;
            int puntosPorTiempo = Mathf.FloorToInt(tiempoRestante * GameConstants.TIME_SCORE_MULTIPLIER);
            return puntosPorEstrellas + puntosPorTiempo;
        }
    }
}
