using System.Collections.Generic;
using SpeechRecognitionAPI;

namespace LC_VEGA
{
    public class LanguageHelper
    {
        // Voice commands
        internal static readonly Dictionary<Languages, Dictionary<string, string>> Commands = new()
        {
            [Languages.English] = new()
            {
                ["startListening"] = "activate",
                ["stopListening"]  = "deactivate",
                ["moonInfo"]  = "info about",
                // ["bestiaryEntries"]  = "read",
                ["creatureInfo"]  = "info about",
                ["activateScanner"]  = "activate scanner/activate advanced scanner/turn on scanner/turn on advanced scanner/scan/enable scanner/enable advanced scanner",
                ["deactivateScanner"]  = "disable scanner/disable advanced scanner/turn off scanner/turn off advanced scanner/disable scan",
                ["openDoor"]  = "open secure door/open door/open the door/open the secure door",
                ["closeDoor"]  = "close secure door/close door/close the door/close the secure door",
                ["openAllDoors"]  = "open all secure doors/open all doors",
                ["closeAllDoors"]  = "close all secure doors/close all doors",
                ["disableTurret"]  = "disable turret/disable the turret",
                ["disableAllTurrets"]  = "disable all turrets",
                ["disableMine"]  = "disable the mine/disable mine/disable the landmine/disable landmine",
                ["disableAllMines"]  = "disable all mines/disable all landmines",
                ["disableTrap"]  = "disable the trap/disable trap/disable the spike trap/disable spike trap",
                ["disableAllTraps"]  = "disable all traps/disable all spike traps",
                ["teleporter"]  = "teleport/activate teleporter/tp/activate tp",
                ["mapSwitch"]  = "switch to me/switch radar/switch radar to me/focus/focus on me",
                ["crewStatus"]  = "crew status/team status/crew info/team info/crew report/team report",
                ["shipCrew"]  = "crew in ship/people in ship/get crew in ship/get people in ship/how many people are in the ship/is anyone in the ship/is anybody in the ship",
                ["scrap"]  = "scrap left/items left/scan for scrap/scan for items",
                ["radarBooster"]  = "ping",
                ["radarFlash"]  = "flash",
                ["signalTranslator"]  = "transmit/send",
                ["time"]  = "what's the current time of day/current time of day/time of day/current time/time/what time is it",
                ["openShip"]  = "open ship doors/open the ship's doors/open hangar doors",
                ["closeShip"]  = "close ship doors/close the ship's doors/close hangar doors",
                ["lightsOn"]  = "lights on/turn the lights on",
                ["lightsOff"]  = "lights out/lights off/turn the lights off",
                ["magnetOn"]  = "activate magnet/enable magnet/turn magnet on",
                ["magnetOff"]  = "deactivate magnet/disable magnet/turn magnet off",
                ["weatherInfo"]  = "info about",
                ["stopTalking"]  = "shut up/stop/stop talking",
                ["gratitude"]  = "thank you/thanks",
            },
            [Languages.Spanish] = new()
            {
                ["startListening"] = "activate",
                ["stopListening"]  = "desactivate",
                ["moonInfo"]  = "info sobre/informacion sobre",
                // ["bestiaryEntries"]  = "lee la entrada llamada/leeme la entrada llamada/lee el registro llamado/leeme el registro llamado",
                ["creatureInfo"]  = "info sobre/informacion sobre",
                ["activateScanner"]  = "activa el escaner/activa el escaner avanzado/enciende el escaner/enciende el escaner avanzado/escanea",
                ["deactivateScanner"]  = "desactiva el escaner/desactiva el escaner avanzado/apaga el escaner/apaga el escaner avanzado",
                ["openDoor"]  = "abre la puerta",
                ["closeDoor"]  = "cierra la puerta",
                ["openAllDoors"]  = "abre todas las puertas",
                ["closeAllDoors"]  = "cierra todas las puertas",
                ["disableTurret"]  = "desactiva la torreta/apaga la torreta",
                ["disableAllTurrets"]  = "desactiva todas las torretas/apaga todas las torretas",
                ["disableMine"]  = "desactiva la mina/apaga la mina",
                ["disableAllMines"]  = "desactiva todas las minas/apaga todas las minas",
                ["disableTrap"]  = "desactiva la trampa/apaga la trampa",
                ["disableAllTraps"]  = "desactiva todas las trampas/apaga todas las trampas",
                ["teleporter"]  = "teletransportame/activa el teletransportador/tp/activa el tp",
                ["mapSwitch"]  = "centrate en mi/cambia el radar",
                ["crewStatus"]  = "estado del equipo/estado actual",
                ["shipCrew"]  = "quien esta en la nave/hay alguien en la nave",
                ["scrap"]  = "cuantos objetos quedan/cuanta chatarra queda/haz un recuento",
                ["radarBooster"]  = "ping",
                ["radarFlash"]  = "flash",
                ["signalTranslator"]  = "transmite/envia/escribe",
                ["time"]  = "que hora es",
                ["openShip"]  = "abre las puertas de la nave/abre la nave",
                ["closeShip"]  = "cierra las puertas de la nave/cierra la nave",
                ["lightsOn"]  = "enciende la luz/enciende las luces",
                ["lightsOff"]  = "apaga la luz/apaga las luces",
                ["magnetOn"]  = "enciende el iman/activa el iman",
                ["magnetOff"]  = "apaga el iman/desactiva el iman",
                ["weatherInfo"]  = "info sobre/informacion sobre",
                ["stopTalking"]  = "callate/para de hablar/deja de hablar",
                ["gratitude"]  = "gracias",
            },
        };
        
        // Names of weather phenomena
        internal static readonly Dictionary<Languages, string[]> Weathers = new()
        {
            [Languages.English] = ["Foggy","Rainy","Stormy","Flooded","Eclipsed"],
            [Languages.Spanish] = ["la niebla", "la lluvia", "las tormentas", "las inundaciones", "los eclipses"]
        };
        
        // Signal transmitter messages
        internal static readonly Dictionary<Languages, string> Messages = new()
        {
            [Languages.English] = "YES, NO, OKAY, HELP, THANKS, ITEMS, MAIN, FIRE, GIANT, GIANTS, DOG, DOGS, WORM, WORMS, BABOONS, HAWKS, DANGER, GIRL, GHOST, BRACKEN, BUTLER, BUTLERS, BUG, BUGS, YIPPEE, SNARE, FLEA, COIL, JESTER, SLIME, THUMPER, MIMIC, MIMICS, MASKED, SPIDER, SNAKES, OLD BIRD, LOST, INSIDE, TRAPPED, LEAVE, GOLD, APPARATUS",
            [Languages.Spanish] = "SI, NO, OKAY, AYUDA, GRACIAS, OBJETOS, ENTRADA, SALIDA, GIGANTE, GIGANTES, PERRO, PERROS, GUSANO, GUSANOS, BABUINO, BABUINOS, PELIGRO, NIÑA, FANTASMA, BRACKEN, ASESINO, BICHO, BICHOS, YIPPEE, PULGA, COIL, JESTER, SLIME, THUMPER, CLON, CLONES, MASCARA, ARAÑA, SERPIENTE, SERPIENTES, ROBOT, DENTRO, ATRAPADO, VETE, ORO, GENERADOR"
        };
    }
}