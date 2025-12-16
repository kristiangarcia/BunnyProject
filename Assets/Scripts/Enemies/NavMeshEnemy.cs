using UnityEngine;
using UnityEngine.AI;
using BunnyGame.Player;
using BunnyGame.Interfaces;
using BunnyGame.Managers;

namespace BunnyGame.Enemies
{
    /// <summary>
    /// Controlador de enemigo navmesh que sigue al jugador
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Collider2D))]
    public class NavMeshEnemy : MonoBehaviour, IDamageable, IRespawnable
    {
        [SerializeField] private Transform target;
        [SerializeField] private float maxDistanciaBusqueda = 0.5f; // Radio para verificar si está en NavMesh
        [SerializeField] private float rangoAgacharse = 1f; // Rango donde el agacharse tiene efecto
        [SerializeField] private float tiempoEsperaAntesDevolverse = 1.5f; // Tiempo quieto antes de volver
        [SerializeField] private float velocidadRetorno = 0.4f; // Velocidad al volver (más lento que persiguiendo)

        private NavMeshAgent agent;
        private SpriteRenderer spriteRenderer;
        private PlayerMovement playerMovement;
        private Animator animator;
        private Collider2D enemyCollider;
        private Vector3 posicionInicial;
        private bool estaMuerto = false;
        private float tiempoAgachadoAcumulado = 0f;
        private float velocidadOriginal;
        private float tiempoUltimoFlip = 0f;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            enemyCollider = GetComponent<Collider2D>();

            // Guardar velocidad original del NavMeshAgent
            if (agent != null)
            {
                velocidadOriginal = agent.speed;
            }

            // Obtener referencia al PlayerMovement del target
            if (target != null)
            {
                playerMovement = target.GetComponent<PlayerMovement>();
            }
        }

        private void Start()
        {
            posicionInicial = transform.position;
        }

        private void OnEnable()
        {
            // Resetear estado al reactivarse
            estaMuerto = false;
            tiempoAgachadoAcumulado = 0f;

            // Reiniciar NavMeshAgent al reactivarse
            if (agent != null)
            {
                agent.updateRotation = false;
                agent.updateUpAxis = false;
                agent.isStopped = false;
                agent.speed = velocidadOriginal;
                agent.ResetPath();
            }

            // Reactivar collider
            if (enemyCollider != null)
            {
                enemyCollider.enabled = true;
            }
        }

        private void Update()
        {
            if (estaMuerto || target == null) return;

            // Verificar si la posición del target está EN el NavMesh
            if (!NavMesh.SamplePosition(target.position, out NavMeshHit hit, maxDistanciaBusqueda, NavMesh.AllAreas))
            {
                // Jugador fuera del NavMesh - detenerse
                agent.ResetPath();
                tiempoAgachadoAcumulado = 0f;
                tiempoUltimoFlip = 0f;
                return;
            }

            float distanciaAlJugador = Vector3.Distance(transform.position, target.position);
            bool jugadorAgachado = playerMovement != null && playerMovement.EstaAgachado;

            // Comprobar si el jugador se agacha DENTRO del rango
            if (jugadorAgachado && distanciaAlJugador <= rangoAgacharse)
            {
                // Incrementar timer
                tiempoAgachadoAcumulado += Time.deltaTime;

                if (tiempoAgachadoAcumulado < tiempoEsperaAntesDevolverse)
                {
                    // Quedarse quieto y mirar a los lados confundido
                    agent.ResetPath();
                    agent.isStopped = true;
                    MirarALadosConfundido();
                }
                else
                {
                    // Volver a posición inicial lentamente
                    agent.isStopped = false;
                    VolverAPosicionInicialLento();
                }
            }
            else
            {
                // Jugador visible o fuera del rango de agacharse - perseguir normal
                tiempoAgachadoAcumulado = 0f;
                tiempoUltimoFlip = 0f;
                agent.isStopped = false;
                agent.speed = velocidadOriginal;
                agent.SetDestination(hit.position);
                ActualizarDireccionSprite(target.position);
            }
        }

        private void MirarALadosConfundido()
        {
            tiempoUltimoFlip += Time.deltaTime;

            // Cada 0.5 segundos, cambiar la dirección (flipear)
            if (tiempoUltimoFlip >= 0.5f)
            {
                spriteRenderer.flipX = !spriteRenderer.flipX;
                tiempoUltimoFlip = 0f;
            }
        }

        private void VolverAPosicionInicialLento()
        {
            // Si ya está cerca de su posición inicial, detenerse
            if (Vector3.Distance(transform.position, posicionInicial) < 0.5f)
            {
                agent.ResetPath();
                return;
            }

            // Cambiar a velocidad lenta
            agent.speed = velocidadRetorno;

            // Moverse hacia la posición inicial
            agent.SetDestination(posicionInicial);
            ActualizarDireccionSprite(posicionInicial);
        }

        private void ActualizarDireccionSprite(Vector3 posicionDestino)
        {
            // Flipear sprite según la dirección hacia el destino
            if (posicionDestino.x > transform.position.x)
            {
                spriteRenderer.flipX = true; // Mira a la derecha
            }
            else if (posicionDestino.x < transform.position.x)
            {
                spriteRenderer.flipX = false; // Mira a la izquierda
            }
        }

        #region IDamageable Implementation

        public void Die()
        {
            if (estaMuerto) return;

            estaMuerto = true;

            // Detener el NavMeshAgent
            if (agent != null)
            {
                agent.isStopped = true;
                agent.ResetPath();
            }

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
            // OnEnable() se encarga de resetear estaMuerto, reactivar collider y NavMeshAgent
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
