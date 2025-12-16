using UnityEngine;
using BunnyGame.Managers;
using BunnyGame.Core;
using BunnyGame.Effects;

namespace BunnyGame.Player
{
    /// <summary>
    /// Maneja el sistema de powerup del jugador
    /// </summary>
    public class PlayerPowerup : MonoBehaviour
    {
        [Header("Configuración de Powerup")]
        [SerializeField] private float duracionPowerup = 3f;
        [SerializeField] private float multiplicadorPowerup = 1.4f;
        [SerializeField] private float pitchPowerup = 1.5f;

        private bool powerupActivo = false;
        private PlayerMovement playerMovement;

        #region Unity Callbacks

        void Awake()
        {
            playerMovement = GetComponent<PlayerMovement>();
        }

        #endregion

        #region Powerup Control

        public void ActivarPowerup()
        {
            // Si ya hay un powerup activo, cancelar el invoke anterior
            if (powerupActivo)
            {
                CancelInvoke(nameof(DesactivarPowerup));
            }

            powerupActivo = true;

            // Aplicar efectos del powerup
            AplicarEfectosPowerup();

            // Desactivar después del tiempo configurado
            Invoke(nameof(DesactivarPowerup), duracionPowerup);
        }

        public void DesactivarPowerup()
        {
            // Protección: solo desactivar si realmente está activo
            if (!powerupActivo) return;

            powerupActivo = false;

            // Cancelar cualquier Invoke pendiente
            CancelInvoke(nameof(DesactivarPowerup));

            // Restaurar valores normales
            RemoverEfectosPowerup();
        }

        private void AplicarEfectosPowerup()
        {
            // Aumentar multiplicador de movimiento
            if (playerMovement != null)
            {
                playerMovement.AplicarMultiplicador(multiplicadorPowerup);
            }

            // Cambiar pitch de música
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.CambiarPitchMusica(pitchPowerup);
            }

            // Activar wobble de cámara
            if (CameraShakeController.Instance != null)
            {
                CameraShakeController.Instance.ActivarWobble();
            }
        }

        private void RemoverEfectosPowerup()
        {
            // Resetear multiplicador de movimiento
            if (playerMovement != null)
            {
                playerMovement.ResetearMultiplicador();
            }

            // Restaurar pitch de música
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.RestaurarPitchMusica();
            }

            // Desactivar shake de cámara
            if (CameraShakeController.Instance != null)
            {
                CameraShakeController.Instance.DesactivarShake();
            }
        }

        #endregion

        #region Properties

        public bool PowerupActivo => powerupActivo;

        #endregion
    }
}
