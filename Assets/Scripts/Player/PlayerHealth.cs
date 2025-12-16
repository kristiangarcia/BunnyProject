using UnityEngine;
using BunnyGame.Interfaces;
using BunnyGame.Managers;
using BunnyGame.UI;
using BunnyGame.Core;
using BunnyGame.Effects;

namespace BunnyGame.Player
{
    /// <summary>
    /// Maneja la salud, muerte y respawn del jugador
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    public class PlayerHealth : MonoBehaviour, IDamageable, IRespawnable
    {
        [Header("Configuración de Respawn")]
        [SerializeField] private float tiempoRespawn = 0.5f;

        [Header("Referencias")]
        [SerializeField] private HUDController hudController;

        private Rigidbody2D rb;
        private Animator animator;
        private Vector3 posicionInicial;
        private bool estaMuerto = false;

        // Referencias a otros componentes del jugador
        private PlayerMovement playerMovement;
        private PlayerPowerup playerPowerup;
        private PlayerCombat playerCombat;

        #region Unity Callbacks

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();

            // Obtener referencias a otros componentes
            playerMovement = GetComponent<PlayerMovement>();
            playerPowerup = GetComponent<PlayerPowerup>();
            playerCombat = GetComponent<PlayerCombat>();
        }

        void Start()
        {
            posicionInicial = transform.position;
        }

        void Update()
        {
            ActualizarAnimacion();
        }

        #endregion

        #region IDamageable Implementation

        public void Die()
        {
            if (estaMuerto) return;

            estaMuerto = true;

            // Desactivar powerup PRIMERO si está activo
            if (playerPowerup != null && playerPowerup.PowerupActivo)
            {
                playerPowerup.DesactivarPowerup();
            }

            // Reproducir sonido y parar música
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(GameConstants.SFX_PLAYER_DEATH);
                AudioManager.Instance.PararMusica();
            }

            // Activar shake de cámara al morir (DESPUÉS de desactivar powerup)
            if (CameraShakeController.Instance != null)
            {
                CameraShakeController.Instance.ActivarShake();
            }

            // Activar animación de muerte
            if (animator != null)
            {
                animator.SetTrigger("Death");
            }

            // Pausar el tiempo del HUD
            if (hudController != null)
            {
                hudController.PausarTiempo();
            }

            // Detener movimiento
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;

            Debug.Log("<color=#FF0000>MUERTE!</color>");

            // Reiniciar después del tiempo configurado
            Invoke(nameof(Respawn), tiempoRespawn);
        }

        public bool IsDead => estaMuerto;

        #endregion

        #region IRespawnable Implementation

        public void Respawn()
        {
            estaMuerto = false;
            transform.position = posicionInicial;
            transform.localScale = Vector3.one;
            rb.bodyType = RigidbodyType2D.Dynamic;

            // Resetear estrellas
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ResetearEstrellas();
            }

            // Reiniciar el tiempo del HUD
            if (hudController != null)
            {
                hudController.ReiniciarTiempo();
            }

            // Respawnear todos los objetos
            if (RespawnManager.Instance != null)
            {
                RespawnManager.Instance.RespawnearTodo();
            }

            // Resetear movimiento (solo si no está presionado actualmente)
            if (playerMovement != null)
            {
                playerMovement.ResetearInputSiNoPresionado();
            }

            // Reiniciar música
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.IniciarMusica();
            }

            Debug.Log("<color=#00FFFF>Conejo respawneado!</color>");
        }

        public Vector3 SpawnPosition => posicionInicial;

        #endregion

        #region Animaciones

        private void ActualizarAnimacion()
        {
            if (animator != null)
            {
                animator.SetBool("isDead", estaMuerto);
            }
        }

        #endregion
    }
}
