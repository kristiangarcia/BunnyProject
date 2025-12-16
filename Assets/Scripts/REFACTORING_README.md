# Refactorización del Proyecto Bunny

## Resumen de Cambios

Este proyecto ha sido refactorizado para mejorar la organización, escalabilidad y mantenibilidad del código.

## Nueva Estructura de Carpetas

```
Assets/Scripts/
├── Core/                    # Clases centrales y constantes
│   └── GameConstants.cs     # Constantes globales del juego
├── Interfaces/              # Interfaces reutilizables
│   ├── IDamageable.cs       # Entidades que pueden recibir daño
│   └── IRespawnable.cs      # Entidades que pueden reaparecer
├── Managers/                # Gestores singleton del juego
│   ├── GameManager.cs       # Gestor principal (antes SceneManager)
│   ├── AudioManager.cs      # Gestor de audio mejorado
│   └── RespawnManager.cs    # Gestor de reapariciones (antes RespawnPool)
├── Player/                  # Componentes del jugador
│   ├── PlayerMovement.cs    # Movimiento y salto
│   ├── PlayerHealth.cs      # Salud y muerte
│   ├── PlayerPowerup.cs     # Sistema de powerups
│   ├── PlayerCombat.cs      # Combate con enemigos
│   ├── PlayerCollector.cs   # Recolección de objetos
│   └── PlayerInputHandler.cs # Entradas especiales
├── Enemies/                 # Enemigos
│   └── Enemy.cs             # Enemigo mejorado (antes Enemigo)
├── UI/                      # Componentes de interfaz
│   ├── HUDController.cs     # Controlador del HUD
│   └── MainMenu.cs          # Menú principal (antes MenuInicial)
├── Effects/                 # Efectos visuales
│   └── ParallaxEffect.cs    # Efecto parallax (antes EfectoParallax)
```

## Cambios Principales

### 1. Uso de Namespaces
Todo el código ahora usa namespaces para mejor organización:
- `BunnyGame.Core` - Constantes y clases centrales
- `BunnyGame.Interfaces` - Interfaces
- `BunnyGame.Managers` - Gestores
- `BunnyGame.Player` - Componentes del jugador
- `BunnyGame.Enemies` - Enemigos
- `BunnyGame.UI` - Interfaz de usuario
- `BunnyGame.Effects` - Efectos

### 2. Cambios de Nombres de Clases

| Clase Antigua | Clase Nueva | Ubicación |
|--------------|-------------|-----------|
| `SceneManager` | `GameManager` | `Managers/GameManager.cs` |
| `RespawnPool` | `RespawnManager` | `Managers/RespawnManager.cs` |
| `Enemigo` | `Enemy` | `Enemies/Enemy.cs` |
| `MiConejo` | Dividido en 6 componentes | `Player/` |
| `EfectoParallax` | `ParallaxEffect` | `Effects/ParallaxEffect.cs` |
| `MenuInicial` | `MainMenu` | `UI/MainMenu.cs` |

### 3. División del Jugador (MiConejo)

El monolítico `MiConejo.cs` ahora está dividido en componentes especializados:

1. **PlayerMovement** - Maneja movimiento, salto, entrada de controles
2. **PlayerHealth** - Maneja salud, muerte, respawn
3. **PlayerPowerup** - Maneja el sistema de powerups
4. **PlayerCombat** - Maneja colisiones y combate con enemigos
5. **PlayerCollector** - Maneja recolección de estrellas y powerups
6. **PlayerInputHandler** - Maneja entradas especiales (menú, etc.)

### 4. Patrón Singleton Mejorado

Todos los gestores usan un patrón Singleton consistente:
```csharp
public static GameManager Instance { get; private set; }
```

### 5. Uso de Constantes

Todos los strings mágicos y números están ahora en `GameConstants.cs`:
```csharp
// Antes
if (other.gameObject.CompareTag("Enemigo"))
audioM.PlaySFX(3);

// Ahora
if (other.gameObject.CompareTag(GameConstants.TAG_ENEMY))
AudioManager.Instance.PlaySFX(GameConstants.SFX_ENEMY_KILL);
```

### 6. Interfaces para Comportamientos Comunes

- `IDamageable` - Para entidades que pueden morir
- `IRespawnable` - Para entidades que pueden reaparecer

## Migración Necesaria

**IMPORTANTE:** Todo el código antiguo ha sido eliminado. Debes actualizar tu proyecto:

1. **En Unity Editor:**
   - **Reemplaza el componente `MiConejo`** por los 6 nuevos componentes del jugador:
     - `PlayerMovement`
     - `PlayerHealth`
     - `PlayerPowerup`
     - `PlayerCombat`
     - `PlayerCollector`
     - `PlayerInputHandler`

   - **Actualiza las referencias:**
     - Busca objetos con `SceneManager` → usa `GameManager`
     - El `AudioManager` en la escena ya no necesita GameObject.Find, usa el singleton
     - Asigna el `HUDController` en `PlayerHealth` (SerializeField en Inspector)

2. **Cambios en código (si tienes scripts adicionales):**
   ```csharp
   // Antes
   sceneManager.obtenerEstrella();
   audioM.PlaySFX(3);

   // Ahora
   GameManager.Instance.ObtenerEstrella();
   AudioManager.Instance.PlaySFX(GameConstants.SFX_ENEMY_KILL);
   ```

## Mejoras Implementadas

### ✅ Código más limpio y organizado
- Separación de responsabilidades (Single Responsibility Principle)
- Componentes más pequeños y manejables
- Mejor legibilidad

### ✅ Escalabilidad
- Fácil agregar nuevos enemigos implementando `IDamageable`
- Fácil agregar nuevos objetos respawneables
- Estructura de carpetas clara para nuevas features

### ✅ Mantenibilidad
- Constantes centralizadas
- Menos código duplicado
- Interfaces reutilizables
- Mejor documentación con comentarios XML

### ✅ Corrección de Errores
- ✅ Conflicto de nombres con `UnityEngine.SceneManagement.SceneManager` resuelto
- ✅ Advertencias de null checks simplificadas
- ✅ Uso de `RequireComponent` para dependencias
- ✅ Métodos vacíos eliminados
- ✅ Mejor manejo de referencias nulas

### ✅ Mejores Prácticas
- Uso de namespaces
- Patrón Singleton consistente
- Regiones para organizar código
- Properties en lugar de campos públicos
- Uso de constantes en lugar de magic strings

## Pasos Necesarios en Unity

1. **Actualizar GameObject del Jugador:**
   - Selecciona el GameObject del jugador en la jerarquía
   - Elimina el componente `MiConejo` (si existe)
   - Agrega los 6 nuevos componentes:
     - `PlayerMovement` (maneja movimiento y salto)
     - `PlayerHealth` (maneja vida y muerte)
     - `PlayerPowerup` (maneja powerups)
     - `PlayerCombat` (maneja combate)
     - `PlayerCollector` (maneja recolección)
     - `PlayerInputHandler` (maneja inputs especiales)
   - Asigna la referencia al `HUDController` en `PlayerHealth`

2. **Actualizar Input System:**
   - Conecta las acciones del Input System a los nuevos métodos:
     - `OnMover` → `PlayerMovement.OnMover`
     - `OnSaltar` → `PlayerMovement.OnSaltar`
     - `OnAbrirMenuInicial` → `PlayerInputHandler.OnAbrirMenuInicial`

3. **Actualizar Managers:**
   - Asegúrate de tener un GameObject con `GameManager` (antes SceneManager)
   - Asegúrate de tener un GameObject con `RespawnManager`
   - El `AudioManager` debe estar en un GameObject llamado "Música"

4. **Actualizar Enemigos:**
   - Reemplaza el componente `Enemigo` por `Enemy` en todos los enemigos

5. **Actualizar HUD:**
   - El `HUDController` ahora usa `GameManager.Instance` automáticamente

## Notas Importantes

- ✅ Todo el código antiguo ha sido eliminado
- ✅ No hay wrappers de compatibilidad - migración completa requerida
- ✅ Todos los componentes tienen documentación XML
- ✅ El código está en español para mantener consistencia
- ✅ Uso de namespaces para mejor organización
- ✅ Patrón Singleton en todos los managers

## Soporte

Si encuentras algún problema durante la migración:
1. Revisa la consola de Unity para errores
2. Verifica que las referencias en el Inspector estén asignadas correctamente
3. Asegúrate de haber agregado todos los componentes del jugador
4. Verifica que los Input Actions estén conectados a los nuevos métodos
