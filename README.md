# carina-bot
[![GitHub issues](https://img.shields.io/github/issues/EduFdezSoy/carina-bot.svg)](https://github.com/EduFdezSoy/carina-bot/issues)
[![GitHub forks](https://img.shields.io/github/forks/EduFdezSoy/carina-bot.svg)](https://github.com/EduFdezSoy/carina-bot/network)
[![GitHub stars](https://img.shields.io/github/stars/EduFdezSoy/carina-bot.svg)](https://github.com/EduFdezSoy/carina-bot/stargazers)
[![Build Status](https://travis-ci.org/EduFdezSoy/carina-bot.svg?branch=master)](https://travis-ci.org/EduFdezSoy/carina-bot)
[![GitHub license](https://img.shields.io/github/license/EduFdezSoy/carina-bot.svg)](https://github.com/EduFdezSoy/carina-bot/blob/master/LICENSE)

Bot cruzado Telegram - Minecraft  

## To - Do  
:sparkles: Chat cruzado Telegram - Minecraft (see rcon).  
:sparkles: Creador de markers para overviewer ([carina.edufdezsoy.es](carina.edufdezsoy.es)).  
:sparkles: Capacidad de realizar copias de seguridad del server en la nube, en caso de falta de espacio se borra la antigua.  
:star2: Estado del servidor (uso de cpu, ram, disco y uptime) e informe de estado del render de overviewer (sacar del json que genera para la web).  
:star2: Ejecutar comandos con privilegios en el servidor (se necesita tabla sql con privilegios).  
:star2: Listado de tareas pendientes en el servidor (Leer, Editar y guardar log de tareas).  
:star: Mostrar log del servidor (ultimas lineas del archivo del log).  
:star: Organizador de quedadas grupales dentro del server y posterior aviso. (en x fecha y hora).   
:star: Usuarios online (en minecraft).  
:star: Gestión del pago del servidor, con aviso 5 dias antes (y repetir cada día) y elimininación del usuario a los 10 días despues del aviso (No podrá entrar al server).


## SQL
* Tabla usuarios (uuid minecraft <--> uuid telegram + usuarios legibles de ambos).
* Tabla de privilegios (id interno de usuario + poder).
* Registro de comandos enviados y respuestas (log).
* Tabla de markers en el mapa.
