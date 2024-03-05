using System;
using System.IO;


namespace fp2p1
{
    class Program
    {
        const bool DEBUG = true;

        struct Coor
        { // coordenadas en el tablero 
            public int x, y;
        }

        struct Pilar
        { // pilar en el tablero
            public Coor coor; // posición en el tablero
            public int val;
        } // valor

        struct Rect
        {  // rectangulo determinado por dos esquinas
            public Coor lt, rb;
        }  // left-top, right-bottom

        struct Tablero
        { // tamaño, pilares, rectángulos marcados
            public int fils, cols; // num fils y cols del tablero   
            public Pilar[] pils;  // array de pilares
            public Rect[] rects;  // array de rectángulos
            public int numRects;
        } // num de rectángulos definidos = prim pos libre en rect

        static void Main()
        {
            int nivelActual = 1; // Nivel inicial
            bool juegoTerminado = false;

            while (!juegoTerminado)
            {
                string nombreArchivo = "001.txt"; // Nombre de archivo
                Console.Clear();
                Console.WriteLine($"Nivel {nivelActual}");
                Console.WriteLine("Presiona Enter para comenzar...");

                Console.ReadLine(); 

                // Juega el nivel y actualiza el estado del juego
                juegoTerminado = JuegaNivel(nombreArchivo);

                if (juegoTerminado)
                {
                    nivelActual++;
                }
                else
                {
                    Console.WriteLine("¿Quieres intentar el nivel de nuevo? (S para sí, cualquier otra tecla para salir)");
                    char retryInput = Console.ReadKey().KeyChar;
                    if (char.ToUpper(retryInput) != 'S')
                    {
                        break; // Sale del bucle si el jugador decide no volver a intentar el nivel.
                    }
                }
            }

            Console.WriteLine("Fin del juego. ¡Hasta luego!");
        }


        static void LeeNivel(string file, ref Tablero tab)
        {
            StreamReader sr = new StreamReader(file);
            
                tab.fils = int.Parse(sr.ReadLine());
                tab.cols = int.Parse(sr.ReadLine());

                tab.pils = new Pilar[tab.fils * tab.cols];
                tab.rects = new Rect[tab.fils * tab.cols];

                int pilarIndex = 0;
                int rowIndex = 0;
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    string[] nums = line.Split(' ');

                    for (int colIndex = 0; colIndex < tab.cols; colIndex++)
                    {
                        if (nums[colIndex] != "-")
                        {
                            tab.pils[pilarIndex] = new Pilar
                            {
                                coor = new Coor { x = colIndex, y = rowIndex },
                                val = int.Parse(nums[colIndex])
                            };
                            pilarIndex++;
                        }
                    }

                    rowIndex++;
                }
            
        }

        static Rect NormalizaRect(Coor c1, Coor c2)
        {
            //Comprueba si las coor son iguales 
            if (c1.x == c2.x && c1.y == c2.y)
            {
                c2.x++;
            }
            // Calcula las esquinas normalizadas lt y rb
            Coor lt = new Coor { x = Math.Min(c1.x, c2.x), y = Math.Min(c1.y, c2.y) };
            Coor rb = new Coor { x = Math.Max(c1.x, c2.x) + 1, y = Math.Max(c1.y, c2.y) + 1 };

            
            return new Rect { lt = lt, rb = rb };
        }

        static void Render(Tablero tab, Coor act, Coor ori)
        {
            Console.Clear();

            // Dibuja el tablero vacio 
            for (int i = 0; i < tab.fils + 1; i++)
            {
                for (int j = 0; j < tab.cols + 1; j++)
                {
                    Console.Write('+');
                    if (j < tab.cols)
                    {
                       Console.Write("   ");
                    }
                }
                Console.WriteLine();

                if (i < tab.fils)
                {
                    for (int j = 0; j < tab.cols; j++)
                    {
                        Console.Write("    ");
                    }
                    Console.WriteLine();
                }
            }

            // Dibuja los pilares
            foreach (Pilar pilar in tab.pils)
            {
                Console.SetCursorPosition(pilar.coor.x * 4 + 2, pilar.coor.y * 2 + 1);
                Console.Write(pilar.val);
            }

            // Dibuja los rectángulos ya marcados
            foreach (Rect rect in tab.rects.Take(tab.numRects))
            {
                RenderRect(rect);
            }

            // Dibuja el rectángulo en curso en verde
            if (ori.x != -1 && act.x != -1)
            {
                Coor lt = new Coor { x = Math.Min(ori.x, act.x), y = Math.Min(ori.y, act.y) };
                Coor rb = new Coor { x = Math.Max(ori.x, act.x), y = Math.Max(ori.y, act.y) };
                Rect currentRect = new Rect { lt = lt, rb = rb };

                Console.ForegroundColor = ConsoleColor.Green;
                RenderRect(currentRect);
                Console.ResetColor();
            }



            // Información de DEBUG 
            if (DEBUG)
            {
                Console.SetCursorPosition(0, 20);
                Console.WriteLine("\nDEBUG INFO:");
                Console.WriteLine($"Cursor: ({act.x}, {act.y})");
                Console.WriteLine($"Esquina origen: ({ori.x}, {ori.y})");
                Console.WriteLine($"Número de rectángulos: {tab.numRects}");
            }


            Console.SetCursorPosition(act.x * 4 + 2, act.y * 2 + 1);
        }

        static void RenderRect(Rect r)
        {
            // Dibuja las conexiones horizontales "---"
            for (int i = r.lt.x * 4 + 1; i < r.rb.x * 4 + 1; i += 4)
            {
                Console.SetCursorPosition(i, r.lt.y * 2);
                Console.Write("---");
                Console.SetCursorPosition(i, r.rb.y * 2);
                Console.Write("---");
            }

            // Dibuja las conexiones verticales "|"
            for (int i = r.lt.y * 2 + 1; i <= r.rb.y * 2 - 1; i += 2)
            {
                Console.SetCursorPosition(r.lt.x * 4, i);
                Console.Write("|");
                Console.SetCursorPosition(r.rb.x * 4, i);
                Console.Write("|");
            }
        }

        static void ProcesaInput(char ch, Tablero tab, ref Coor act, ref Coor ori)
        {
            switch (ch)
            {
                case 'l':
                    if (act.x > 0) act.x --;
                    break;
                case 'r':
                    if (act.x < tab.cols - 1) act.x++;
                    break;
                case 'u':
                    if (act.y > 0) act.y--;
                    break;
                case 'd':
                    if (act.y < tab.fils - 1) act.y++;
                    break;
                case 'c':
                    // Marcar la primera esquina del rectángulo en curso
                    if (ori.x == -1)
                    {
                        // Comienza la selección de un nuevo rectángulo.
                        ori = new Coor { x = act.x, y = act.y };
                    }
                    else
                    {
                        // Termina la selección del rectángulo y lo inserta en el tablero.
                        Coor c1 = new Coor { x = ori.x, y = ori.y };
                        Coor c2 = new Coor { x = act.x, y = act.y };

                        // Si se intenta marcar la primera esquina sobre otro ya existente, elimina el segundo.
                        EliminaRect(ref tab, c1);

                        // Inserta el nuevo rectángulo en el tablero.
                        InsertaRect(ref tab, c1, c2);
                        ori = new Coor { x = -1, y = -1 }; // Reinicia la selección
                    }
                    break;
                case 'q':
                    // Salir del programa falta condicion para salir
                    break;
            }
        }

        static char LeeInput()
        {
            char d = ' ';
            while (d == ' ')
            {
                if (Console.KeyAvailable)
                {
                    string tecla = Console.ReadKey().Key.ToString();
                    switch (tecla)
                    {
                        case "LeftArrow":
                            d = 'l';
                            break;
                        case "UpArrow":
                            d = 'u';
                            break;
                        case "RightArrow":
                            d = 'r';
                            break;
                        case "DownArrow":
                            d = 'd';
                            break;
                        case "Spacebar":
                            d = 'c';
                            break;
                        case "Escape":
                        case "Q":
                            d = 'q';
                            break;
                    }
                }
            }
            return d;
        }

        static bool Dentro(Coor c, Rect r)
        {
            // Implementa la determinación de si la coordenada c está dentro del rectángulo r.
            return (c.x > r.lt.x && c.x < r.rb.x && c.y > r.lt.y && c.y < r.rb.y);
        }

        static bool Intersect(Rect r1, Rect r2)
        {
            // Implementa la determinación de si dos rectángulos tienen intersección común.
            return !(r1.rb.x < r2.lt.x || r1.lt.x > r2.rb.x || r1.rb.y < r2.lt.y || r1.lt.y > r2.rb.y);
        }

        static void InsertaRect(ref Tablero tab, Coor c1, Coor c2)
        {
            Rect nuevoRect = NormalizaRect(c1, c2);

            // Verifica si el nuevo rectángulo se solapa con alguno de los existentes.
            bool solapamiento = false;
            for (int i = 0; i < tab.numRects; i++)
            {
                if (Intersect(nuevoRect, tab.rects[i]))
                {
                    solapamiento = true;
                    break;
                }
            }

            if (!solapamiento)
            {
                // Inserta el nuevo rectángulo en el tablero.
                tab.rects[tab.numRects] = nuevoRect;
                tab.numRects++;
            }
        }

        static bool EliminaRect(ref Tablero tab, Coor c)
        {
            bool eliminado = false;
            for (int i = 0; i < tab.numRects; i++)
            {
                if (Dentro(c, tab.rects[i]))
                {
                    // Elimina el rectángulo si contiene la coordenada c.
                    for (int j = i; j < tab.numRects - 1; j++)
                    {
                        tab.rects[j] = tab.rects[j + 1];
                    }
                    tab.numRects--;
                    eliminado = true;
                }
            }
            return eliminado;  
        }

        static int AreaRect(Rect r)
        {
            // Calcula el área del rectángulo.
            return (r.rb.x - r.lt.x + 1) * (r.rb.y - r.lt.y + 1);
        }

        static bool CheckRect(Rect r, Pilar[] p)
        {
            bool encontrado = false;
            int i = 0;
            while (i < p.Length)
            {
                if (Dentro(p[i].coor, r) && AreaRect(r) == p[i].val)
                {
                    encontrado = true;  // Se encontró un pilar válido dentro del rectángulo.
                }
                i++;
            }

            return encontrado;
        }

        static bool FinJuego(Tablero tab)
        {
            bool encontrado = false;
            int i = 0;
            while (i < tab.numRects)
            {
                if (!CheckRect(tab.rects[i], tab.pils))
                {
                    encontrado = true;  // Si encuentra un rectángulo que no cumple, retorna falso.
                }
                i++;
            }

            return encontrado;
        }

        static bool JuegaNivel(string file)
        {
            bool terminado = false;
            Tablero tab = new Tablero();
            Coor act = new Coor { x = 0, y = 0 };
            Coor ori = new Coor { x = -1, y = -1 };

            LeeNivel(file, ref tab);

            while (!terminado)
            {
                Render(tab, act, ori);
                char input = LeeInput();
                ProcesaInput(input, tab, ref act, ref ori);

                if (FinJuego(tab))
                {
                    Console.Clear();
                    Console.WriteLine("¡Nivel superado!");
                    terminado = true;
                }

                if (input == 'q')
                {
                    Console.Clear();
                    Console.WriteLine("Juego abortado.");
                    terminado = false;
                }
            }
            return terminado;
        }


    }
}