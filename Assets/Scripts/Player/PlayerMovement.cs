using UnityEngine;
using UnityEngine.InputSystem;
using BunnyGame.Core;
using BunnyGame.Managers;

namespace BunnyGame.Player
{
    /// <summary>
    /// Maneja el movimiento y salto del jugador
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Animator))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Configuración de Movimiento")]
        [SerializeField] private float velocidadBase = 1.2f;
        [SerializeField] private float fuerzaSalto = 3.2f;
        [SerializeField] private float distanciaRaycast = 0.2f;
        [SerializeField, Range(0f, 1f)] private float controlEnAire = 0.2f; // Control reducido en el aire

        [Header("Jump Feel")]
        [SerializeField] private float jumpBufferTime = 0.15f; // Ventana de tiempo para guardar input de salto

        private Rigidbody2D rb;
        private SpriteRenderer spriteRenderer;
        private Animator animator;
        private Vector2 inputMovimiento;
        private bool agachado = false;
        private float velocidadMovimiento = 0f;
        private float multiplicadorActual = 1f;

        // Jump Buffer: temporizador que cuenta hacia atrás desde jumpBufferTime hasta 0
        private float jumpBufferCounter = 0f;

        // Referencias a otros componentes del jugador
        private PlayerHealth playerHealth;

        #region Unity Callbacks

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            playerHealth = GetComponent<PlayerHealth>();
        }

        void Update()
        {
            ActualizarJumpBuffer();
            ActualizarAnimaciones();
        }

        void FixedUpdate()
        {
            if (playerHealth != null && playerHealth.IsDead) return;

            ActualizarMovimientoFisico();
            ProcesarSalto();
        }

        void OnDrawGizmos()
        {
            // Dibujar raycast para debug
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.down * distanciaRaycast);
        }

        #endregion

        #region Input Handlers

        public void OnMover(InputAction.CallbackContext contexto)
        {
            if (playerHealth != null && playerHealth.IsDead) return;

            inputMovimiento = contexto.ReadValue<Vector2>();

            if (contexto.performed)
            {
                ProcesarMovimientoHorizontal();
                ProcesarAgacharse();
            }
            else if (contexto.canceled)
            {
                velocidadMovimiento = 0f;
                agachado = false;
            }
        }

        public void OnSaltar(InputAction.CallbackContext contexto)
        {
            if (playerHealth != null && playerHealth.IsDead) return;

            if (contexto.performed)
            {
                // Activar temporizador de jump buffer (iniciar cuenta atrás desde 0.15s)
                // El salto se ejecutará en FixedUpdate si aterrizamos antes de que expire
                jumpBufferCounter = jumpBufferTime;
            }
        }

        #endregion

        #region Movimiento

        private void ProcesarMovimientoHorizontal()
        {
            if (inputMovimiento.x < 0)
            {
                if (rb.linearVelocity.x >= 0)
                    spriteRenderer.flipX = true;
            }
            else if (inputMovimiento.x > 0)
            {
                if (rb.linearVelocity.x <= 0)
                    spriteRenderer.flipX = false;
            }

            velocidadMovimiento = inputMovimiento.x * velocidadBase * multiplicadorActual;
        }

        private void ProcesarAgacharse()
        {
            agachado = inputMovimiento.y < -0.5f && EstaEnSuelo();
        }

        private void ActualizarMovimientoFisico()
        {
            if (velocidadMovimiento != 0 && !agachado)
            {
                // Velocidad a la que queremos llegar según el input del jugador
                float velocidadObjetivo = velocidadMovimiento;
                float nuevaVelocidadX;

                if (EstaEnSuelo())
                {
                    // EN EL SUELO: Control instantánico y total
                    // La velocidad cambia inmediatamente a la velocidad deseada
                    nuevaVelocidadX = velocidadObjetivo;
                }
                else
                {
                    // EN EL AIRE: Control reducido mediante interpolación
                    //
                    // Mathf.Lerp hace una interpolación lineal (mezcla gradual) entre dos valores:
                    // - Valor A: velocidad actual (rb.linearVelocity.x)
                    // - Valor B: velocidad deseada (velocidadObjetivo)
                    // - Factor t: controlEnAire (0 a 1)
                    //
                    // Si controlEnAire = 1.0 → cambio instantáneo (igual que en el suelo)
                    // Si controlEnAire = 0.5 → cambia 50% hacia la velocidad deseada cada frame
                    // Si controlEnAire = 0.0 → no cambia nada (sin control en el aire)
                    //
                    // Esto crea el efecto de "inercia" en el aire, haciendo que sea más difícil
                    // cambiar de dirección mientras saltas, dando peso y realismo al personaje
                    nuevaVelocidadX = Mathf.Lerp(rb.linearVelocity.x, velocidadObjetivo, controlEnAire);
                }

                // Aplicar la nueva velocidad horizontal manteniendo la velocidad vertical (salto/caída)
                rb.linearVelocity = new Vector2(nuevaVelocidadX, rb.linearVelocity.y);
            }
            else if (EstaEnSuelo())
            {
                // Cuando no hay input de movimiento y estamos en el suelo, detener deslizamiento
                // Mantener velocidad vertical para no afectar saltos/caídas
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            }
        }

        private void ProcesarSalto()
        {
            // Ejecutar salto si:
            // 1. El temporizador aún no ha expirado (jumpBufferCounter > 0)
            // 2. Estamos en el suelo
            // Esto permite presionar saltar un poco antes de aterrizar
            if (jumpBufferCounter > 0f && EstaEnSuelo())
            {
                float fuerzaFinal = fuerzaSalto * multiplicadorActual;
                rb.AddForce(Vector2.up * fuerzaFinal, ForceMode2D.Impulse);

                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySFX(GameConstants.SFX_JUMP);
                }

                // Resetear temporizador para evitar saltos múltiples
                jumpBufferCounter = 0f;
            }
        }

        #endregion

        #region Jump Buffer

        private void ActualizarJumpBuffer()
        {
            // Decrementar el temporizador cada frame (cuenta atrás hacia 0)
            // Ejemplo: 0.15 -> 0.13 -> 0.11 -> ... -> 0
            // Si llega a 0 antes de aterrizar, el input de saltar expira
            if (jumpBufferCounter > 0f)
            {
                jumpBufferCounter -= Time.deltaTime;
            }
        }

        #endregion

        #region Ground Check

        public bool EstaEnSuelo()
        {
            RaycastHit2D hit = Physics2D.Raycast(
                transform.position,
                Vector2.down,
                distanciaRaycast,
                LayerMask.GetMask(GameConstants.LAYER_GROUND)
            );
            return hit.collider != null;
        }

        #endregion

        #region Animaciones

        private void ActualizarAnimaciones()
        {
            if (animator == null) return;

            animator.SetFloat("Speed", Mathf.Abs(velocidadMovimiento));
            animator.SetBool("isGrounded", EstaEnSuelo());
            animator.SetFloat("verticalVelocity", rb.linearVelocity.y);
            animator.SetBool("isDucking", agachado);
        }

        #endregion

        #region Powerup

        public void AplicarMultiplicador(float multiplicador)
        {
            multiplicadorActual = multiplicador;

            // Recalcular velocidad si está en movimiento
            if (inputMovimiento.x != 0)
            {
                velocidadMovimiento = inputMovimiento.x * velocidadBase * multiplicadorActual;
            }

            // Actualizar velocidad de animación
            if (animator != null)
            {
                animator.speed = multiplicador;
            }
        }

        public void ResetearMultiplicador()
        {
            AplicarMultiplicador(1f);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Resetea el input al respawnear (solo si no hay input activo)
        /// </summary>
        public void ResetearInputSiNoPresionado()
        {
            velocidadMovimiento = 0f;
            agachado = false;
            jumpBufferCounter = 0f;
        }

        #endregion

        #region Properties

        public float VelocidadVertical => rb != null ? rb.linearVelocity.y : 0f;
        public Vector2 InputMovimiento => inputMovimiento;
        public bool EstaAgachado => agachado;

        #endregion
    }
}
