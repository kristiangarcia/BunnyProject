using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BunnyGame.Core;

namespace BunnyGame.Managers
{
    /// <summary>
    /// Gestor de reaparición de objetos con patrón Singleton
    /// Maneja enemigos, powerups y colectables
    /// </summary>
    public class RespawnManager : MonoBehaviour
    {
        public static RespawnManager Instance { get; private set; }

        [Header("Tiempos de Respawn (en segundos)")]
        [SerializeField] private float tiempoRespawnEnemigo = 10f;
        [SerializeField] private float tiempoRespawnPowerup = 10f;
        [SerializeField] private float tiempoRespawnObjeto = 10f;

        private Dictionary<GameObject, Vector3> posicionesOriginales = new Dictionary<GameObject, Vector3>();
        private Dictionary<GameObject, Coroutine> respawnCoroutines = new Dictionary<GameObject, Coroutine>();

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

        void Start()
        {
            InicializarPosicionesOriginales();
        }

        private void InicializarPosicionesOriginales()
        {
            GuardarPosicionesIniciales(GameConstants.TAG_ENEMY);
            GuardarPosicionesIniciales(GameConstants.TAG_POWERUP);
            GuardarPosicionesIniciales(GameConstants.TAG_COLLECTIBLE);
        }

        private void GuardarPosicionesIniciales(string tag)
        {
            GameObject[] objetos = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject obj in objetos)
            {
                if (!posicionesOriginales.ContainsKey(obj))
                {
                    posicionesOriginales[obj] = obj.transform.position;
                }
            }
        }

        public void RegistrarParaRespawn(GameObject objeto)
        {
            if (objeto == null)
            {
                Debug.LogWarning("Intentando registrar un objeto nulo para respawn");
                return;
            }

            // Guardar posición si no está guardada
            if (!posicionesOriginales.ContainsKey(objeto))
            {
                posicionesOriginales[objeto] = objeto.transform.position;
            }

            // Cancelar corrutina anterior si existe
            if (respawnCoroutines.ContainsKey(objeto) && respawnCoroutines[objeto] != null)
            {
                StopCoroutine(respawnCoroutines[objeto]);
            }

            // Determinar el tiempo de respawn según el tag
            float tiempoRespawn = ObtenerTiempoRespawn(objeto.tag);

            // Iniciar corrutina de respawn y guardarla
            Coroutine nuevaCorrutina = StartCoroutine(RespawnDespuesDeTiempo(objeto, tiempoRespawn));
            respawnCoroutines[objeto] = nuevaCorrutina;

            Debug.Log($"<color=#FFAA00>{objeto.name} ({objeto.tag}) será respawneado en {tiempoRespawn} segundos</color>");
        }

        private float ObtenerTiempoRespawn(string tag)
        {
            return tag switch
            {
                GameConstants.TAG_ENEMY => tiempoRespawnEnemigo,
                GameConstants.TAG_POWERUP => tiempoRespawnPowerup,
                GameConstants.TAG_COLLECTIBLE => tiempoRespawnObjeto,
                _ => 10f // Tiempo por defecto
            };
        }

        private IEnumerator RespawnDespuesDeTiempo(GameObject objeto, float tiempo)
        {
            yield return new WaitForSeconds(tiempo);

            if (objeto != null)
            {
                RespawnearObjeto(objeto);

                // Limpiar referencia de la corrutina
                if (respawnCoroutines.ContainsKey(objeto))
                {
                    respawnCoroutines.Remove(objeto);
                }
            }
        }

        private void RespawnearObjeto(GameObject objeto)
        {
            if (objeto == null) return;

            // Restaurar posición original
            if (posicionesOriginales.ContainsKey(objeto))
            {
                objeto.transform.position = posicionesOriginales[objeto];
            }

            // Reactivar el objeto
            objeto.SetActive(true);

            Debug.Log($"<color=#00FF00>{objeto.name} ha respawneado!</color>");
        }

        /// <summary>
        /// Respawnea todos los objetos inmediatamente (usado cuando muere el jugador)
        /// </summary>
        public void RespawnearTodo()
        {
            // Detener todas las corrutinas de respawn en curso
            StopAllCoroutines();
            respawnCoroutines.Clear();

            // Respawnear todos los objetos registrados
            foreach (var kvp in posicionesOriginales)
            {
                GameObject objeto = kvp.Key;
                if (objeto != null)
                {
                    RespawnearObjeto(objeto);
                }
            }

            Debug.Log("<color=#FFFF00>Todos los objetos han sido respawneados!</color>");
        }

        #region Properties

        public int CantidadObjetosRegistrados => posicionesOriginales.Count;

        #endregion
    }
}
