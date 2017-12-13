# carina-bot
[![GitHub issues](https://img.shields.io/github/issues/EduFdezSoy/carina-bot.svg)](https://github.com/EduFdezSoy/carina-bot/issues)
[![GitHub forks](https://img.shields.io/github/forks/EduFdezSoy/carina-bot.svg)](https://github.com/EduFdezSoy/carina-bot/network)
[![GitHub stars](https://img.shields.io/github/stars/EduFdezSoy/carina-bot.svg)](https://github.com/EduFdezSoy/carina-bot/stargazers)
[![Build Status](https://travis-ci.org/EduFdezSoy/carina-bot.svg?branch=master)](https://travis-ci.org/EduFdezSoy/carina-bot)
[![GitHub license](https://img.shields.io/github/license/EduFdezSoy/carina-bot.svg)](https://github.com/EduFdezSoy/carina-bot/blob/master/LICENSE)

Bot cruzado Telegram - Minecraft  

## To - Do  
* Chat cruzado Telegram - Minecraft (see rcon)
* Creador de markers para overviewer ([carina.edufdezsoy.es](carina.edufdezsoy.es))  
* Usuarios online (en minecraft)
* Estado del servidor (uso de cpu, ram, disco y uptime)
* Estado del render de overviewer (se puede sacar del json que genera para la web)
* Ejecutar comandos con privilegios en el servidor (se necesita tabla sql con privilegios)
* Mostrar log del servidor (ultimas lineas del archivo del log)
* Listado de tareas pendientes en el servidor para no entrar y estar perdido.
* Organizador de quedadas grupales dentro del server y posterior aviso. (en x fecha y hora).


## SQL
* Tabla usuarios (uuid minecraft <--> uuid telegram + usuarios legibles de ambos)
* Tabla de privilegios (id interno de usuario + poder)
* Registro de comandos enviados y respuestas (log)
* Tabla de markers en el mapa
