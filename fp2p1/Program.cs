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

            // Dibuja el tablero vacío con intersecciones '+'
            for (int i = 0; i < tab.fils; i++)
            {
                for (int j = 0; j < tab.cols; j++)
                {
                    Console.Write('+');
                    if (j < tab.cols - 1)
                    {
                        Console.Write("---");
                    }
                }
                Console.WriteLine();

                if (i < tab.fils - 1)
                {
                    for (int j = 0; j < tab.cols; j++)
                    {
                        Console.Write("|   ");
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

            // Información de depuración si DEBUG es true
            if (DEBUG)
            {
                Console.SetCursorPosition(0, 20);
                Console.WriteLine("\nDEBUG INFO:");
                Console.WriteLine($"Cursor: ({act.x}, {act.y})");
                Console.WriteLine($"Esquina origen: ({ori.x}, {ori.y})");
                Console.WriteLine($"Número de rectángulos: {tab.numRects}");
            }
        }

        static void RenderRect(Rect r)
        {
            // Dibuja las conexiones horizontales "---"
            for (int i = r.lt.x * 4 + 2; i < r.rb.x * 4 + 2; i += 4)
            {
                Console.SetCursorPosition(i, r.lt.y * 2);
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


        static char leeInput()
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

    }
}