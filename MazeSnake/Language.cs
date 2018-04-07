using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MazeSnake
{
    public static class LanguageTranslator
    {
        public static Dictionary<string, string> SpanishDictionary = new Dictionary<string, string>()
        {
            #region Spanish Dictionary
            { "start", "Comienzar" },
            { "credits", "Créditos" },
            { "quit", "Dejar" },
            { "play", "Jugar" },
            { "achievements", "Logros" },
            { "stats", "Estadísticas" },
            { "shop", "Tienda" },
            { "snakes", "Serpientes" }, 
            { "log out", "Cerrar Sesión" },
            { "back", "Regresa" },

            { "inventory", "Inventario" },

            { "speed", "Velocidad" },
            { "teleport", "Viaje" },
            { "electric", "Eléctrico" },
            { "jackpot", "Premio mayor" },
            { "forcefield", "Escudo" },
            { "frozen", "Congelado" },

            { "mazes completed: ", "Laberintos terminados: " },
            { "powerups collected: ", "Energía acumulada: " },
            { "times teleported: ", "Tiempos transportados: " },
            { "seconds with speed: ", "Segundos con velocidad: " },
            { "coins collected: ", "Monedas recolectadas: " },
            { "walls broken: ", "Paredes fracturado: " },
            { "achievements completed: ", "Logros terminados: " },
            { "time frozen: ", "Tiempo congelado: " },
            { "enemies avoided: ", "Enemigos evitados: " },

            { "create new user", "Crear nuevo usuario" },
            { "create user", "Crear usario" },
            { "cancel", "Cancelar" },
            { "okay", "Continúa" },
            { "do not show again", "No mostrar de nuevo" },

            { "please sign in to play", "Inicia sesión para jugar" },
            { "playing as", "Jugando como" },

            { "resume", "Continuar" },
            { "main menu", "Menú principal" },
            { "save & quit game", "Salva y deja el juego" },

            { "please select a gamemode.", "Por favor, seleccione un modo de juego." },
            { "classic", "Original" },
            { "freeplay", "Juego seguro" },
            { "star mode", "Modo estrella" },

            { "showing statistics for", "Mostrando estadísticas de" },

            { "save", "Entrega" },
            { "volume", "Volumen" },
            { "show mouse cursor while playing", "Mostrar cursor mientras se reproduce" },
            { "show user's coins while playing", "Mostrar las monedas del usuario mientras se juega" },
            { "show user's xp and level while playing", "Mostrar la experiencia y el nivel del usuario durante la reproducción" },

            { "please enter a username.", "Por favor, ingrese un nombre de usuario." },
            { "your username must include at least one non-whitespace character.", 
                "Su nombre de usuario debe incluir al menos un carácter que no sea de\nespacio en blanco." },
            { "already exists. please choose a different\nusername.", "ya existe. Por favor escoja un nombre de\nusuario diferente." },

            { "who are you?", "¿Quién eres tú?" },

            { "select", "Seleccionar" },

            { "electric powerup", "Eléctrico energía" },
            { "speed powerup", "Velocidad energía" },
            { "teleport powerup", "Transporte energía" },
            { "forcefield powerup", "Energía de escudo" },
            { "frozen powerup", "Energía congelado" },
            { "finish", "Acabado" },

            { "mazesnake version", "Versión de MazeSnake" },

            { "red snake", "Serpiente de rojo" },
            { "orange snake", "Serpiente de naranja" },
            { "yellow snake", "Serpiente de amarillo" },
            { "blue snake", "Serpiente de azul" },
            { "purple snake", "Serpiente de purpura" },
            { "camo snake", "Serpiente camuflaje" },
            { "albino snake", "Serpiente albino" },
            { "sea snake", "Serpiente de el mar" },
            { "rainbow snake", "Serpiente de arco iris" },
            { "zebra snake", "Serpiente cebra" },
            { "posh snake", "Serpiente elegante" },
            { "bumble snake", "Serpiente abeja" },
            { "robosnake", "Serpiente robot" },
            { "dull snake", "Serpiente aburrido" },
            { "polka dot snake", "Serpiente lunares" },
            { "old snake", "Vieja serpiente" },
            { "young snake", "Serpiente joven" },
            { "pixel snake", "Pixel serpiente" },
            { "builder snake", "Serpiente constructor" },
            { "homecoming snake", "Serpiente de graduación" },
            { "cosmic snake", "Serpiente cósmica" },
            { "ghost snake", "Serpiente fantasma" },
            { "freedom snake", "Serpiente de la libertad" },
            { "snowsnake", "Serpiente de nieve" },
            //{ "cornucopia", "cornucopia" }, Cornucopia doesn't need to be translated

            { "level", "Nivel" },
            { "buy", "Comprar" },

            { "the shortcut ctrl+w exits the game. are you sure you want to do this?", "El atajo Ctrl + W sale del juego. ¿Seguro que quieres hacer esto?" },
            { "you leveled up to level", "¡Usted niveló hasta el nivel" },
            { "level 5 changes:\n*mazes are now harder", "Cambios de nivel 5:\n*Los laberintos son ahora más difíciles" },
            { "level 15 changes:\n*mazes are now timed\n*time decreses every time you level up", "Cambios de nivel 15:\n*Los laberintos ahora están cronometrados" + 
                "\n*El tiempo decrementa cada vez que subes de nivel" },
            { "time's up!", "¡El tiempo ha terminado!" },

            { "electric eel", "Anguila electrica" },
            { "wizard", "Mago" },

            { "you have completed the \nachievement", "Has completado el logro\n" },
            { "username", "Nombre" },

            { "view", "Ver" },

            { "are you sure you want to delete", "¿Estas seguro de que desea eliminar" },

            { "powerups", "Energías"},
            { "xp points", "XP puntos" },
            { "purchase xp", "Compra XP" },
            { "cost", "Costo" },
            #endregion
        };
        public static Dictionary<string, string> FrenchDictionary = new Dictionary<string, string>()
        {
        };
        public static Dictionary<string, string> GermanDictionary = new Dictionary<string, string>()
        {
        };
        public static Dictionary<string, string> ItalianDictionary = new Dictionary<string, string>()
        {
        };
        public static Dictionary<string, string> PolishDictionary = new Dictionary<string, string>()
        {
        };

        public static string Translate(string text)
        {
            return LanguageTranslator.TranslateInto(GameInfo.Language, text);
        }
        public static string TranslateInto(Language lang, string text)
        {
            string returnVal = "";
            switch (lang)
            {
                case Language.Spanish:
                    if (SpanishDictionary.ContainsKey(text.ToLower()))
                    {
                        returnVal = SpanishDictionary[text.ToLower()];
                    }
                    else
                    {
                        returnVal = text;
                    }
                    break;
                //case Language.French:
                //    if (FrenchDictionary.ContainsKey(text.ToLower()))
                //    {
                //        returnVal = FrenchDictionary[text.ToLower()];
                //    }
                //    else
                //    {
                //        returnVal = text;
                //    }
                //    break;
                //case Language.German:
                //    if (GermanDictionary.ContainsKey(text.ToLower()))
                //    {
                //        returnVal = GermanDictionary[text.ToLower()];
                //    }
                //    else
                //    {
                //        returnVal = text;
                //    }
                //    break;
                //case Language.Italian:
                //    if (ItalianDictionary.ContainsKey(text.ToLower()))
                //    {
                //        returnVal = ItalianDictionary[text.ToLower()];
                //    }
                //    else
                //    {
                //        returnVal = text;
                //    }
                //    break;
                //case Language.Polish:
                //    if (PolishDictionary.ContainsKey(text.ToLower()))
                //    {
                //        returnVal = PolishDictionary[text.ToLower()];
                //    }
                //    else
                //    {
                //        returnVal = text;
                //    }
                //    break;
                case Language.English:
                    returnVal = text;
                    break;
                //case Language.Backwards:
                //    text = text.ToLower();

                //    // First, make sure the text isn't already translated into backwards English
                //    if (SpanishDictionary.ContainsKey(text))
                //    {
                //        // The text needs to be translated
                //        char[] charArray = text.ToCharArray();
                //        List<char> characters = new List<char>();
                //        characters = charArray.Reverse().ToList();
                //        returnVal = "";
                //        string addChar = "";
                //        for (int i = 0; i <= characters.Count - 1; i++)
                //        {
                //            if (i == 0)
                //            {
                //                addChar = characters[i].ToString().ToUpper();
                //            }
                //            else
                //            {
                //                addChar = characters[i].ToString();
                //            }
                //            returnVal += addChar;
                //        }
                //    }
                //    else
                //    {
                //        // The text is likely already backwards
                //        returnVal = text;
                //    }
                //    break;
            }
            return returnVal;
        }

        public static string GetName(Language lang)
        {
            string returnVal = "";
            switch (lang)
            {
                case Language.English:
                    returnVal = "English";
                    break;
                case Language.Spanish:
                    returnVal = "Español";
                    break;
                //case Language.Backwards:
                //    returnVal = "Sdrawkcab";
                //    break;
            }
            return returnVal;
        }
    }
    public enum Language
    {
        English = 0, // English (US)
        Spanish = 1, // Español
        //Backwards = 2, // Sdrawkcab
        //French = 3, // Français
        //German = 4, // Deutsche
        //Italian = 5, // Italiano
        //Polish = 6, // Polski
    }
}
