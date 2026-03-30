using System.Collections.Generic;

namespace LC_VEGA
{
    public class LanguageHelper
    {
        internal static readonly string[] Languages = ["English", "Spanish"];
        
        // Voice commands
        internal static readonly Dictionary<string, Dictionary<string, string>> Commands = new()
        {
            ["English"] = new()
            {
                ["startListening"] = "VEGA activate",
                ["stopListening"]  = "VEGA deactivate",
                ["moonInfo"]  = "VEGA info about",
                ["bestiaryEntries"]  = "VEGA read",
                ["creatureInfo"]  = "VEGA info about",
                ["activateScanner"]  = "VEGA activate scanner/VEGA activate advanced scanner/VEGA turn on scanner/VEGA turn on advanced scanner/VEGA scan/VEGA enable scanner/VEGA enable advanced scanner",
                ["deactivateScanner"]  = "VEGA disable scanner/VEGA disable advanced scanner/VEGA turn off scanner/VEGA turn off advanced scanner/VEGA disable scan",
                ["openDoor"]  = "VEGA open secure door/VEGA open door/VEGA open the door/VEGA open the secure door",
                ["closeDoor"]  = "VEGA close secure door/VEGA close door/VEGA close the door/VEGA close the secure door",
                ["openAllDoors"]  = "VEGA open all secure doors/VEGA open all doors",
                ["closeAllDoors"]  = "VEGA close all secure doors/VEGA close all doors",
                ["disableTurret"]  = "VEGA disable turret/VEGA disable the turret",
                ["disableAllTurrets"]  = "VEGA disable all turrets",
                ["disableMine"]  = "VEGA disable the mine/VEGA disable mine/VEGA disable the landmine/VEGA disable landmine",
                ["disableAllMines"]  = "VEGA disable all mines/VEGA disable all landmines",
                ["disableTrap"]  = "VEGA disable the trap/VEGA disable trap/VEGA disable the spike trap/VEGA disable spike trap",
                ["disableAllTraps"]  = "VEGA disable all traps/VEGA disable all spike traps",
                ["teleporter"]  = "VEGA teleport/VEGA activate teleporter/VEGA tp/VEGA activate tp",
                ["mapSwitch"]  = "VEGA switch to me/VEGA switch radar/VEGA switch radar to me/VEGA focus/VEGA focus on me",
                ["crewStatus"]  = "VEGA crew status/VEGA team status/VEGA crew info/VEGA team info/VEGA crew report/VEGA team report",
                ["shipCrew"]  = "VEGA crew in ship/VEGA people in ship/VEGA get crew in ship/VEGA get people in ship/VEGA how many people are in the ship/VEGA is anyone in the ship/VEGA is anybody in the ship",
                ["scrap"]  = "VEGA scrap left/VEGA items left/VEGA scan for scrap/VEGA scan for items",
                ["radarBooster"]  = "VEGA ping",
                ["radarFlash"]  = "VEGA flash",
                ["signalTranslator"]  = "VEGA transmit/VEGA send",
                ["time"]  = "VEGA what's the current time of day/VEGA current time of day/VEGA time of day/VEGA current time/VEGA time/VEGA what time is it?",
                ["openShip"]  = "VEGA open ship doors/VEGA open the ship's doors/VEGA open hangar doors",
                ["closeShip"]  = "VEGA close ship doors/VEGA close the ship's doors/VEGA close hangar doors",
                ["lightsOn"]  = "VEGA lights on/VEGA turn the lights on",
                ["lightsOff"]  = "VEGA lights out/VEGA lights off/VEGA turn the lights off",
                ["magnetOn"]  = "VEGA activate magnet/VEGA enable magnet/VEGA turn magnet on",
                ["magnetOff"]  = "VEGA deactivate magnet/VEGA disable magnet/VEGA turn magnet off",
                ["weatherInfo"]  = "VEGA info about",
                ["stopTalking"]  = "VEGA shut up/VEGA stop/VEGA stop talking/Shut up VEGA/Stop VEGA/Stop talking VEGA",
                ["gratitude"]  = "VEGA thank you/VEGA thanks/Thank you VEGA/Thanks VEGA",
            },
            ["Spanish"] = new()
            {
                ["startListening"] = "VEGA activate",
                ["stopListening"]  = "VEGA desactivate",
                ["moonInfo"]  = "VEGA info sobre/VEGA informacion sobre",
                ["bestiaryEntries"]  = "VEGA lee la entrada del/VEGA leeme la entrada del/VEGA lee el registro del/VEGA leeme el registro del",
                ["creatureInfo"]  = "VEGA info sobre los/VEGA informacion sobre los",
                ["activateScanner"]  = "VEGA activa el escaner/VEGA activa el escaner avanzado/VEGA enciende el escaner/VEGA enciende el escaner avanzado",
                ["deactivateScanner"]  = "VEGA desactiva el escaner/VEGA desactiva el escaner avanzado/VEGA apaga el escaner/VEGA apaga el escaner avanzado",
                ["openDoor"]  = "VEGA abre la puerta",
                ["closeDoor"]  = "VEGA cierra la puerta",
                ["openAllDoors"]  = "VEGA abre todas las puertas",
                ["closeAllDoors"]  = "VEGA cierra todas las puertas",
                ["disableTurret"]  = "VEGA desactiva la torreta/VEGA apaga la torreta",
                ["disableAllTurrets"]  = "VEGA desactiva todas las torretas/VEGA apaga todas las torretas",
                ["disableMine"]  = "VEGA desactiva la mina/VEGA apaga la mina",
                ["disableAllMines"]  = "VEGA desactiva todas las minas/VEGA apaga todas las minas",
                ["disableTrap"]  = "VEGA desactiva la trampa/VEGA apaga la trampa",
                ["disableAllTraps"]  = "VEGA desactiva todas las trampas/VEGA apaga todas las trampas",
                ["teleporter"]  = "VEGA teletransportame/VEGA activa el teletransportador/VEGA tp/VEGA activa el tp",
                ["mapSwitch"]  = "VEGA centrate en mi/VEGA cambia el radar",
                ["crewStatus"]  = "VEGA estado del equipo/VEGA estado actual",
                ["shipCrew"]  = "VEGA quien esta en la nave/VEGA hay alguien en la nave",
                ["scrap"]  = "VEGA cuantos objetos quedan/VEGA cuanta chatarra queda/VEGA haz un recuento",
                ["radarBooster"]  = "VEGA ping",
                ["radarFlash"]  = "VEGA flash",
                ["signalTranslator"]  = "VEGA transmite/VEGA envia/VEGA escribe",
                ["time"]  = "VEGA que hora es",
                ["openShip"]  = "VEGA abre las puertas de la nave/VEGA abre la nave",
                ["closeShip"]  = "VEGA cierra las puertas de la nave/VEGA cierra la nave",
                ["lightsOn"]  = "VEGA enciende la luz/VEGA enciende las luces",
                ["lightsOff"]  = "VEGA apaga la luz/VEGA apaga las luces",
                ["magnetOn"]  = "VEGA enciende el iman/VEGA activa el iman",
                ["magnetOff"]  = "VEGA apaga el iman/VEGA desactiva el iman",
                ["weatherInfo"]  = "VEGA info sobre/VEGA informacion sobre",
                ["stopTalking"]  = "VEGA callate/VEGA para/VEGA deja de hablar/Callate VEGA/Para VEGA/Deja de hablar VEGA",
                ["gratitude"]  = "Gracias VEGA",
            },
        };
        
        // Names of weather phenomena
        internal static readonly Dictionary<string, string[]> Weathers = new()
        {
            ["English"] = ["Foggy","Rainy","Stormy","Flooded","Eclipsed"],
            ["Spanish"] = ["la niebla", "la lluvia", "las tormentas", "las inundaciones", "los eclipses"]
        };
        
        // Signal transmitter messages
        internal static readonly Dictionary<string, string> Messages = new()
        {
            ["English"] = "YES, NO, OKAY, HELP, THANKS, ITEMS, MAIN, FIRE, GIANT, GIANTS, DOG, DOGS, WORM, WORMS, BABOONS, HAWKS, DANGER, GIRL, GHOST, BRACKEN, BUTLER, BUTLERS, BUG, BUGS, YIPPEE, SNARE, FLEA, COIL, JESTER, SLIME, THUMPER, MIMIC, MIMICS, MASKED, SPIDER, SNAKES, OLD BIRD, LOST, INSIDE, TRAPPED, LEAVE, GOLD, APPARATUS",
            ["Spanish"] = "SI, NO, OKAY, AYUDA, GRACIAS, OBJETOS, ENTRADA, SALIDA, GIGANTE, GIGANTES, PERRO, PERROS, GUSANO, GUSANOS, BABUINO, BABUINOS, PELIGRO, NIÑA, FANTASMA, BRACKEN, ASESINO, BICHO, BICHOS, YIPPEE, PULGA, COIL, JESTER, SLIME, THUMPER, CLON, CLONES, MASCARA, ARAÑA, SERPIENTE, SERPIENTES, ROBOT, DENTRO, ATRAPADO, VETE, ORO, GENERADOR"
        };
    }
}