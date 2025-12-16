using UnityEngine;
using BunnyGame.Core;
using BunnyGame.Managers;

namespace BunnyGame.Player
{
    /// <summary>
    /// Maneja la recolección de objetos y powerups
    /// </summary>
    public class PlayerCollector : MonoBehaviour
    {
        private PlayerPowerup playerPowerup;

        void Awake()
        {
            playerPowerup = GetComponent<PlayerPowerup>();
        }

        void OnTriggerEnter2D(Collider2D colisionador)
        {
            if (colisionador.gameObject.CompareTag(GameConstants.TAG_COLLECTIBLE))
            {
                RecolectarEstrella(colisionador.gameObject);
            }
            else if (colisionador.gameObject.CompareTag(GameConstants.TAG_POWERUP))
            {
                RecolectarPowerup(colisionador.gameObject);
            }
        }

        private void RecolectarEstrella(GameObject estrella)
        {
            estrella.SetActive(false);

            // Agregar estrella al GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ObtenerEstrella();
            }

            // Reproducir sonido
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(GameConstants.SFX_COLLECT_STAR);
            }

            Debug.Log("<color=#FF00FF>Estrella obtenida!</color>");
        }

        private void RecolectarPowerup(GameObject powerup)
        {
            powerup.SetActive(false);

            // Registrar para respawn
            if (RespawnManager.Instance != null)
            {
                RespawnManager.Instance.RegistrarParaRespawn(powerup);
            }

            // Activar powerup
            if (playerPowerup != null)
            {
                playerPowerup.ActivarPowerup();
            }

            // Reproducir sonido
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(GameConstants.SFX_POWERUP);
            }
        }
    }
}
