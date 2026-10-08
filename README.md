# Desarroladores-vs-Zombies:
Trabajo de Base de Datos y Algoritmos y Estructura de Datos sobre el proyecto interdisciplinario

# Nuestro Proyecto:

#🧟‍♂️ Informe de Proyecto: Zombie Escape

# 1. Introducción
Zombie Escape no es solo un juego de supervivencia, es un ecosistema de software completo. Este proyecto nace como una integración práctica entre el desarrollo de videojuegos 2D y la gestión eficiente de datos. Todo lo que ocurre en pantalla —desde el movimiento y la aparición de enemigos hasta el cálculo de daño— está respaldado por estructuras de datos optimizadas en memoria y una base de datos relacional que registra las métricas de cada partida en tiempo real.

Para lograr un flujo de trabajo profesional, el desarrollo se gestionó bajo la metodología ágil Scrum y se utilizó Git para el control de versiones, asegurando un trabajo colaborativo ordenado.

# 2. ¿De qué trata el juego? (Mecánicas y Gameplay)
El objetivo es simple pero desafiante: sobrevivir el mayor tiempo posible a hordas de zombies.

El Personaje: El jugador se mueve por un entorno 2D hostil. Debe gestionar tácticamente su stamina (energía) para correr y administrar recursos limitados en el mapa, como munición, barricadas temporales y kits de cura.

Los Enemigos: El juego cuenta con un sistema de spawn procedural que genera distintos tipos de zombies con comportamientos únicos: el caminante clásico, el corredor rápido y el tanque (alta resistencia).

Combate y Métricas: Cuenta con un sistema de combate cuerpo a cuerpo y a distancia. Cada encuentro, daño recibido o acción estratégica se detecta por proximidad y se registra automáticamente de fondo.

# 3. Objetivos del Proyecto
Desarrollo integral: Conectar la lógica y renderizado de un motor gráfico con un backend de persistencia de datos.

Optimización algorítmica: Utilizar las estructuras de datos adecuadas para que el juego corra de manera fluida, justificando la complejidad espacial y temporal de cada algoritmo.

Persistencia y Análisis: Guardar el historial de las partidas, daño y encuentros en una base de datos normalizada (en 3FN) de al menos 8 tablas, permitiendo generar reportes analíticos de la habilidad del jugador.

# 4. Código y Tecnología (Stack Técnico)
Para que el juego funcione, el motor (frontend) y los datos (backend) se comunican constantemente. Esta es la arquitectura del código:

Lenguaje y Motor Gráfico: Está desarrollado en C# utilizando el framework MonoGame para el renderizado de gráficos, físicas de colisión y lectura de controles.

Estructuras de Datos y Algoritmos:

Listas enlazadas y estructuras dinámicas para manejar el spawn y despawn constante de los enemigos en pantalla.

Pilas y Colas para gestionar el inventario de recursos, las oleadas y el orden de los eventos.

Arrays 1D/2D para el mapeo del escenario (zonas seguras y colisiones).

Algoritmos de búsqueda y ordenamiento, sumado a funciones recursivas, para optimizar el cálculo de rutas de los zombies o armar los rankings de jugadores.

Base de Datos y Persistencia: La conexión se hace vía código ejecutando operaciones CRUD y consultas complejas (JOINs, GROUP BY, funciones agregadas). Además de la DB, hay manejo de archivos binarios/texto locales para logs y backups.
