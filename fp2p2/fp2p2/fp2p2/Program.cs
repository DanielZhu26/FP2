using Coordenadas;
using System;
using System.IO;
namespace fp2p2
{
    class Tablero
    {
        // contenido de las casillas
        enum Casilla { Libre, Muro, Comida, Vitamina, MuroCelda };
        // matriz de casillas (tablero)
        Casilla[,] cas;
        // representacion de los personajes (pacman y fantasmas)
        struct Personaje
        {
            public Coor pos, dir, // posicion y direccion actual
            ini; // posicion inicial (para fantasmas)
        }
        // vector de personajes, 0 es pacman y el resto fantasmas
        Personaje[] pers;
        // colores para los personajes
        ConsoleColor[] colors = {ConsoleColor.DarkYellow, ConsoleColor.Red,
        ConsoleColor.Magenta, ConsoleColor.Cyan, ConsoleColor.DarkBlue };
        const int lapCarcelFantasmas = 3000; // retardo para quitar el muro a los fantasmas
        int lapFantasmas; // tiempo restante para quitar el muro
        int numComida; // numero de casillas restantes con comida o vitamina
        Random rnd; // generador de aleatorios
                    // flag para mensajes de depuracion en consola
        private bool DEBUG = true;


        Tablero(string file)
        {
            try
            {
                // Inicializar el generador de números aleatorios
                if (DEBUG)
                    rnd = new Random(100); // Semilla fija para depuración
                else
                    rnd = new Random(); // Semilla aleatoria

                StreamReader reader = new StreamReader(file);
                {
                    // Leer la primera línea para determinar el número de columnas
                    string firstLine = reader.ReadLine();
                    int cols = firstLine.Split(' ').Length;

                    // Contar el número de filas contando las líneas restantes en el archivo
                    int rows = 1;
                    while (!reader.EndOfStream)
                    {
                        reader.ReadLine();
                        rows++;
                    }

                    // Reiniciar el StreamReader para volver al inicio del archivo
                    reader.BaseStream.Seek(0, SeekOrigin.Begin);

                    // Inicializar el tablero con el tamaño obtenido
                    cas = new Casilla[rows, cols];
                    pers = new Personaje[5]; // 1 pacman y 4 fantasmas

                    // Leer el contenido del archivo y cargar el tablero
                    for (int i = 0; i < rows; i++)
                    {
                        string[] tokens = reader.ReadLine().Split(' ');
                        for (int j = 0; j < cols; j++)
                        {
                            int value = int.Parse(tokens[j]);
                            switch (value)
                            {
                                case 0:
                                    cas[i, j] = Casilla.Libre;
                                    break;
                                case 1:
                                    cas[i, j] = Casilla.Muro;
                                    break;
                                case 2:
                                    cas[i, j] = Casilla.Comida;
                                    break;
                                case 3:
                                    cas[i, j] = Casilla.Vitamina;
                                    break;
                                case 4:
                                    cas[i, j] = Casilla.MuroCelda;
                                    break;
                                case 5:
                                case 6:
                                case 7:
                                case 8:
                                    cas[i, j] = Casilla.Libre;

                                    // Crear un nuevo personaje para el fantasma y establecer su posición inicial
                                    pers[value - 5].pos = new Coor(i, j);
                                    pers[value - 5].dir = new Coor(1, 0); 
                                    pers[value - 5].ini = new Coor(i, j);
                                    break;
                                case 9:
                                    cas[i, j] = Casilla.Libre;
                                    // Establecer la posición inicial del Pacman
                                    pers[0].pos = new Coor(i, j);
                                    pers[0].dir = new Coor(0, 1); // Dirección inicial
                                    pers[0].ini = new Coor(i, j);
                                    break;

                            }
                        }
                    }
                }
                lapFantasmas = lapCarcelFantasmas;
              
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("El archivo de nivel no existe.");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al cargar el nivel: " + ex.Message);
                throw;
            }
        }


        void Render()
        {

        }


    }

}
