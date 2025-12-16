using UnityEngine;
using BunnyGame.Interfaces;
using BunnyGame.Managers;
using BunnyGame.Player;

namespace BunnyGame.Enemies
{
    /// <summary>
    /// Controlador de enemigo con movimiento patrullado y animaciones de muerte
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Collider2D))]
    public class Enemy : MonoBehaviour, IDamageable, IRespawnable
    {
        [Header("Movimiento")]
        [SerializeField] private float velocidad = 3f;
        [SerializeField] private Vector2 desplazamiento;

        [Header("Persecución")]
        [SerializeField] private bool puedePerseguir = false;
        [SerializeField] private Transform jugador;
        [SerializeField] private float rangoDeteccion = 5f;
        [SerializeField] private float velocidadPersecucion = 4f;

        private SpriteRenderer spriteRenderer;
        private Animator animator;
        private Collider2D enemyCollider;
        private PlayerMovement playerMovement;
        private Vector3 posicionInicial;
        private Vector3 posicionFinal;
        private bool moviendoAFinal = true;
        private bool estaMuerto = false;
        private bool persiguiendo = false;

        #region Unity Callbacks

        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            enemyCollider = GetComponent<Collider2D>();

            // Obtener referencia al PlayerMovement del jugador
            if (jugador != null)
            {
                playerMovement = jugador.GetComponent<PlayerMovement>();
            }
        }

        void Start()
        {
            InicializarPosiciones();
        }

        void OnEnable()
        {
            // Resetear estado al reactivarse
            estaMuerto = false;

            // Reactivar collider
            if (enemyCollider != null)
            {
                enemyCollider.enabled = true;
            }
        }

        void Update()
        {
            if (!estaMuerto)
            {
                ActualizarMovimiento();
            }
        }

        #endregion

        #region Inicialización

        private void InicializarPosiciones()
        {
            posicionInicial = transform.position;
            posicionFinal = posicionInicial + new Vector3(desplazamiento.x, desplazamiento.y, 0);
        }

        #endregion

        #region Movimiento

        private void ActualizarMovimiento()
        {
            // Verificar si debe perseguir al jugador
            if (puedePerseguir && DebePererseguirJugador())
            {
                PerseguirJugador();
            }
            else
            {
                PatrullarNormalmente();
            }
        }

        private bool DebePererseguirJugador()
        {
            if (jugador == null) return false;

            // No perseguir si el jugador está agachado
            if (playerMovement != null && playerMovement.EstaAgachado)
            {
                return false;
            }

            // Verificar si el jugador está en rango de detección
            float distancia = Vector3.Distance(transform.position, jugador.position);
            return distancia <= rangoDeteccion;
        }

        private void PerseguirJugador()
        {
            persiguiendo = true;

            // Moverse hacia el jugador
            transform.position = Vector3.MoveTowards(
                transform.position,
                jugador.position,
                velocidadPersecucion * Time.deltaTime
            );

            // Flip según dirección del jugador
            ActualizarFlip(jugador.position);
        }

        private void PatrullarNormalmente()
        {
            persiguiendo = false;

            Vector3 posicionDestino = moviendoAFinal ? posicionFinal : posicionInicial;

            // Movimiento suave
            transform.position = Vector3.MoveTowards(transform.position, posicionDestino, velocidad * Time.deltaTime);

            // Flip según dirección
            ActualizarFlip(posicionDestino);

            // Cambiar dirección al llegar al destino
            if (Vector3.Distance(transform.position, posicionDestino) < 0.01f)
            {
                moviendoAFinal = !moviendoAFinal;
            }
        }

        private void ActualizarFlip(Vector3 posicionDestino)
        {
            if (posicionDestino.x > transform.position.x)
                spriteRenderer.flipX = true; // Mira a la derecha
            else if (posicionDestino.x < transform.position.x)
                spriteRenderer.flipX = false; // Mira a la izquierda
        }

        #endregion

        #region IDamageable Implementation

        public void Die()
        {
            if (estaMuerto) return;

            estaMuerto = true;

            // Desactivar collider para que la explosión no mate al jugador
            if (enemyCollider != null)
            {
                enemyCollider.enabled = false;
            }

            // Reproducir animación de muerte
            if (animator != null)
            {
                animator.SetTrigger("Death");
            }

            Debug.Log($"<color=#FF6600>{gameObject.name} ha muerto - reproduciendo animación</color>");
        }

        public bool IsDead => estaMuerto;

        #endregion

        #region IRespawnable Implementation

        public void Respawn()
        {
            transform.position = posicionInicial;
            gameObject.SetActive(true);
            // OnEnable() se encarga de resetear estaMuerto y reactivar el collider
        }

        public Vector3 SpawnPosition => posicionInicial;

        #endregion

        #region Animation Events

        /// <summary>
        /// Este método es llamado por un AnimationEvent al final de la animación de muerte
        /// </summary>
        public void OnAnimacionMuerteCompletada()
        {
            Debug.Log($"<color=#00FFFF>OnAnimacionMuerteCompletada llamado para {gameObject.name}</color>");

            // Desactivar el objeto
            gameObject.SetActive(false);

            // Notificar al RespawnManager
            if (RespawnManager.Instance != null)
            {
                RespawnManager.Instance.RegistrarParaRespawn(gameObject);
            }
            else
            {
                Debug.LogWarning("RespawnManager.Instance es null!");
            }
        }

        #endregion
    }
}
