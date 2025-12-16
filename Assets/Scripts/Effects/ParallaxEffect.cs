using UnityEngine;

namespace BunnyGame.Effects
{
    /// <summary>
    /// Efecto parallax para fondos que siguen la cámara con un multiplicador
    /// Solo aplica el efecto en el eje X (horizontal) para evitar problemas al saltar
    /// </summary>
    public class ParallaxEffect : MonoBehaviour
    {
        [SerializeField][Range(0f, 1f)] private float efectoParallax = 0.5f;

        private Transform camara;
        private Vector3 ultimaPosicionCamara;

        void Start()
        {
            InicializarCamara();
        }

        void LateUpdate()
        {
            if (camara == null) return;

            ActualizarPosicionParallax();
        }

        private void InicializarCamara()
        {
            if (Camera.main != null)
            {
                camara = Camera.main.transform;
                ultimaPosicionCamara = camara.position;
            }
            else
            {
                Debug.LogWarning("No se encontró cámara principal para el efecto parallax");
            }
        }

        private void ActualizarPosicionParallax()
        {
            Vector3 movimientoFondo = camara.position - ultimaPosicionCamara;
            // Eje X: aplica el efecto parallax con el multiplicador
            // Eje Y: sigue la cámara 1:1 (sin multiplicador) para evitar ver bordes al saltar
            transform.position += new Vector3(movimientoFondo.x * efectoParallax, movimientoFondo.y, 0);
            ultimaPosicionCamara = camara.position;
        }
    }
}
