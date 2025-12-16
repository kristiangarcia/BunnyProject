using UnityEngine;
using BunnyGame.Interfaces;
using BunnyGame.Core;
using BunnyGame.Managers;
using BunnyGame.Enemies;

namespace BunnyGame.Player
{
    /// <summary>
    /// Maneja el combate y colisiones del jugador con enemigos
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerCombat : MonoBehaviour
    {
        [Header("Configuración de Combate")]
        [SerializeField] private float fuerzaReboteEnemigo = 4f;

        private Rigidbody2D rb;
        private PlayerHealth playerHealth;
        private PlayerMovement playerMovement;

        #region Unity Callbacks

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            playerHealth = GetComponent<PlayerHealth>();
            playerMovement = GetComponent<PlayerMovement>();
        }

        void OnCollisionEnter2D(Collision2D colision)
        {
            ManejarColision(colision);
        }

        void OnCollisionExit2D(Collision2D colision)
        {
            Debug.Log($"<color=#FF0000>Colisión de salida con: {colision.gameObject.name}</color>");
        }

        #endregion

        #region Colisiones

        private void ManejarColision(Collision2D colision)
        {
            Debug.Log($"<color=#00FF00>Colisión iniciada con: {colision.gameObject.name}</color>");

            if (playerHealth != null && playerHealth.IsDead) return;

            if (colision.gameObject.CompareTag(GameConstants.TAG_ENEMY))
            {
                ManejarColisionEnemigo(colision);
            }
            else if (colision.gameObject.CompareTag(GameConstants.TAG_TRAP))
            {
                playerHealth?.Die();
            }
        }

        private void ManejarColisionEnemigo(Collision2D colision)
        {
            bool cayendoDesdeArriba = DeterminarSiCaeDesdeArriba(colision);

            if (cayendoDesdeArriba)
            {
                MatarEnemigo(colision.gameObject);
                AplicarRebote();
            }
            else
            {
                // Colisión lateral - el jugador muere
                playerHealth?.Die();
            }
        }

        private bool DeterminarSiCaeDesdeArriba(Collision2D colision)
        {
            // Obtener información del contacto
            ContactPoint2D contacto = colision.GetContact(0);
            Vector2 normal = contacto.normal;

            // La normal apunta desde el enemigo hacia el jugador
            // Si normal.y > 0.5 significa que el contacto es desde arriba (más de 60° vertical)
            // Esto es más fiable que la velocidad porque funciona incluso si Unity ya detuvo el movimiento
            return normal.y > 0.5f;
        }

        private void MatarEnemigo(GameObject enemigo)
        {
            // Obtener cualquier componente que implemente IDamageable
            IDamageable damageable = enemigo.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.Die();

                // Reproducir sonido
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySFX(GameConstants.SFX_ENEMY_KILL);
                }

                Debug.Log("<color=#FFFF00>Enemigo eliminado!</color>");
            }
            else
            {
                Debug.LogError($"El GameObject {enemigo.name} con tag '{GameConstants.TAG_ENEMY}' no tiene un componente IDamageable!");
            }
        }

        private void AplicarRebote()
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            rb.AddForce(Vector2.up * fuerzaReboteEnemigo, ForceMode2D.Impulse);
        }

        #endregion
    }
}
