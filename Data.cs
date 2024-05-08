using System.Text;
using static SDL2.SDL;

namespace Dysgenesis
{
    public static class Data
    {
        public const string W_TITLE = "Dysgenesis";
        public static short W_HAUTEUR = 1080;
        public static short W_LARGEUR = 1920;
        public static short W_SEMI_HAUTEUR = 540;
        public static short W_SEMI_LARGEUR = 960;

        public const short G_FPS = 60;
        public const byte G_MAX_DEPTH = 50;

        public const byte P_WIDTH = 50;
        public const float P_PERMA_PITCH = 0.3f;
        public const float P_PROJ_SPEED = 0.9f;

        public const short S_DENSITY = 100;
        public const byte S_SPAWN_RADIUS = 100;
        public const float S_SPEED = 1.02f;

        public const byte E_AVG_SPEED = 8;
        public const float E_AVG_Z_SPEED = 0.125f;
        public const float E_FRICTION = 0.9f;
        public const byte E_15_MAX_HP = 150;

        public static readonly Vector3[] MODELE_P =
        {
            new(-50,0,0), // rouge
            new(-46,-2,-50), // orange
            new(-46,0,-2), // vomi
            new(-46,-4,-2), // -vomi
            new(-46,0,-2), // vomi
            new(-10,5,-5), // vert pâle
            new(-10,-5,-5), // -vert pâle
            new(-10,5,-5), // vert pâle
            new(-5,5,-15), // rose
            new(-5,0,-15), // -rose
            new(-5,5,-15), // rose
            new(5,5,-15), // rose
            new(5,0,-15), // -rose
            new(5,5,-15), // rose
            new(10,5,-5), // vert pâle
            new(46,0,-2), // vomi
            new(46,-2,-50), // orange
            new(50,0,0), // rouge
            new(50,-5,0), // rouge 2
            new(13,-10,10), // bleu
            new(15,-9,20), // mauve
            new(4,10,10), // vert foncé
            new(13,-10,10), // bleu
            new(15,-9,20), // mauve
            new(0,-15,20), // bleh
            new(-15,-9,20), // mauve
            new(-4,10,10), // vert foncé
            new(-13,-10,10), // bleu
            new(-15,-9,20), // mauve
            new(-13,-10,10), // bleu
            new(-50,-5,0), // rouge 2
            new(-50,0,0), // rouge
            new(-50,-5,0), // rouge 2
            new(-46,-2,-50), // orange
            new(-46,-4,-2), // -vomi
            new(-10,-5,-5), // -vert pâle
            new(-5,0,-15), // -rose
            new(5,0,-15), // -rose
            new(10,-5,-5), // -vert pâle
            new(46,-4,-2), // -vomi
            new(46,-2,-50), // orange
            new(50,-5,0), // rouge 2
            new(50,0,0), // rouge
            new(10,5,10), // -bleu
            new(4,10,10), // vert foncé
            new(2,10,10), // l'autre
            new(1,15,10), // bleu pâle
            new(-1,15,10), // bleu pâle
            new(1,15,10), // bleu pâle
            new(0,15,25), // jaune
            new(0,-15,20), // bleh
            new(0,15,25), // jaune
            new(-1,15,10), // bleu pâle
            new(-2,10,10), // l'autre
            new(-4,10,10), // vert foncé
            new(-10,5,10), // -bleu
            new(-50,0,0) // rouge
        };
        public static readonly int[] MODELE_P_SAUTS =
        {
            -1
        };

        public static sbyte[,] MODELE_A = new sbyte[7, 2]
        {
            { -25, -10},
            { 0, 30},
            { 25, -10},
            { 0, 0},
            { 0, 30},
            { 0, 0},
            { -25, -10}
        };

        // modèles ennemis
        static readonly Vector3[] MODELE_E1 =
        {
            new( 0, 0, -20 ),
            new( -16, 0, 10 ),
            new( 16, 0, 10 ),
            new( 0, 0, -20 ),
            new( 0, -30, 0 ),
            new( -16, 0, 10 ),
            new( 0, -30, 0 ),
            new( 16, 0, 10 ),
            new( 0, 30, 0 ),
            new( 0, 0, -20 ),
            new( -16, 0, 10 ),
            new( 0, 30, 0 ),
            new( -5, 5, 0 ),
            new( 0, 2, 0 ),
            new( 5, 5, 0 ),
            new( 0, 8, 0 ),
            new( -5, 5, 0 ),
            new( 0, 5, 0 ),
            new( 0, 5, 0 )
        };
        static readonly int[] MODELE_E1_SAUTS = { 12, 17, -1 };
        static readonly Vector3[] MODELE_E2 =
        {
            new( -30, 0, 0 ),
            new( -15, 0, 26 ),
            new( 0, 50, 0 ),
            new( -30, 0, 0 ),
            new( -15, 0, -26 ),
            new( 0, 50, 0 ),
            new( 15, 0, -26 ),
            new( -15, 0, -26 ),
            new( 15, 0, -26 ),
            new( 30, 0, 0 ),
            new( 0, 50, 0 ),
            new( 15, 0, 26 ),
            new( 30, 0, 0 ),
            new( 15, 0, 26 ),
            new( -15, 0, 26 ),
            new( -12, -10, 21 ),
            new( -25, -10, 0 ),
            new( -30, 0, 0 ),
            new( -25, -10, 0 ),
            new( -12, -10, -21 ),
            new( -15, 0, -26 ),
            new( -12, -10, -21 ),
            new( 12, -10, -21 ),
            new( 15, 0, -26 ),
            new( 12, -10, -21 ),
            new( 25, -10, 0 ),
            new( 30, 0, 0 ),
            new( 25, -10, 0 ),
            new( 12, -10, 21 ),
            new( 15, 0, 26 ),
            new( 12, -10, 21 ),
            new( -12, -10, 21 ),
            new( -12, -10, 3 ),
            new( -12, -10, -3 ),
            new( -17, -10, 0 ),
            new( -20, -18, 0 ),
            new( -15, -18, 3 ),
            new( -12, -10, 3 ),
            new( -15, -18, 3 ),
            new( -15, -18, -3 ),
            new( -12, -10, -3 ),
            new( -15, -18, -3 ),
            new( -20, -18, 0 ),
            new( -5, -23, 0 ),
            new( -15, -18, -3 ),
            new( -5, -23, 0 ),
            new( -15, -18, 3 ),
            new( -12, -10, 3 ),
            new( -12, -10, 21 ),
            new( 12, -10, 21 ),
            new( 12, -10, 3 ),
            new( 12, -10, -3 ),
            new( 17, -10, 0 ),
            new( 20, -18, 0 ),
            new( 15, -18, 3 ),
            new( 12, -10, 3 ),
            new( 15, -18, 3 ),
            new( 15, -18, -3 ),
            new( 12, -10, -3 ),
            new( 15, -18, -3 ),
            new( 20, -18, 0 ),
            new( 5, -23, 0 ),
            new( 15, -18, -3 ),
            new( 5, -23, 0 ),
            new( 15, -18, 3 ),
            new( 12, -10, 3 ),
            new( 12, -10, 21 )
        };
        static readonly int[] MODELE_E2_SAUTS = { 48, 50, -1 };
        static readonly Vector3[] MODELE_E3 =
        {
            new( -50, 0, 0 ),
            new( -50, -10, 40 ),
            new( -45, 0, 0 ),
            new( -50, 0, 0 ),
            new( -20, -10, -10 ),
            new( -20, 10, -10 ),
            new( 20, 10, -10 ),
            new( 20, 10, 10 ),
            new( 50, 0, 0 ),
            new( 50, -10, 40 ),
            new( 45, 0, 0 ),
            new( 50, 0, 0 ),
            new( 20, 10, -10 ),
            new( 20, -10, -10 ),
            new( -20, -10, -10 ),
            new( -20, -10, 10 ),
            new( 20, -10, 10 ),
            new( 20, -10, -10 ),
            new( 50, 0, 0 ),
            new( 20, -10, 10 ),
            new( 20, 10, 10 ),
            new( -20, 10, 10 ),
            new( -50, 0, 0 ),
            new( -20, -10, 10 ),
            new( -20, 10, 10 ),
            new( -20, 10, -10 ),
            new( -50, 0, 0 )
        };
        static readonly int[] MODELE_E3_SAUTS = { -1 };
        static readonly Vector3[] MODELE_E5 =
        {
            new( -15, 45, 10),
            new( -10, 30, 10),
            new( -25, 30, 50),
            new( -15, 45, 10 ),
            new( -40, 30, 10 ),
            new( -25, 30, 50 ),
            new( -25, 20, 10 ),
            new( -10, 30, 10 ),
            new( -25, 20, 10 ),
            new( -40, 30, 10 ),
            new( -50, 0, 10 ),
            new( -40, -30, 10 ),
            new( -15, -50, 10 ),
            new( 15, -50, 10 ),
            new( 40, -30, 10 ),
            new( 50, 0, 10 ),
            new( 40, 25, 10 ),
            new( 15, 45, 10 ),
            new( 25, 30, 50 ),
            new( 40, 25, 10 ),
            new( 25, 20, 10 ),
            new( 25, 30, 50 ),
            new( 10, 30, 10 ),
            new( 15, 45, 10 ),
            new( 10, 30, 10 ),
            new( 25, 20, 10 ),
            new( 35, 0, 10 ),
            new( 25, -20, 10 ),
            new( 10, -30, 10 ),
            new( 0, -10, 0 ),
            new( -10, -30, 10 ),
            new( -25, -20, 10 ),
            new( -35, 0, 10 ),
            new( -25, 20, 10 ),
            new( -25, 20, -10 ),
            new( -10, 30, -10 ),
            new( -10, 30, 10 ),
            new( -10, 30, -10 ),
            new( -15, 45, -10 ),
            new( -15, 45, 10 ),
            new( -15, 45, -10 ),
            new( -40, 30, -10 ),
            new( -40, 30, 10 ),
            new( -40, 30, -10 ),
            new( -25, 20, -10 ),
            new( -35, 0, -10 ),
            new( -25, -20, -10 ),
            new( -10, -30, -10 ),
            new( 0, -10, 0 ),
            new( -15, 10, -15 ),
            new( 15, 10, -15 ),
            new( 0, -10, 0 ),
            new( 15, 10, 15 ),
            new( 15, 10, -15 ),
            new( 15, 10, 15 ),
            new( -15, 10, 15 ),
            new( -15, 10, -15 ),
            new( -15, 10, 15 ),
            new( 0, -10, 0 ),
            new( 10, -30, -10 ),
            new( 25, -20, -10 ),
            new( 35, 0, -10 ),
            new( 25, 20, -10 ),
            new( 40, 30, -10 ),
            new( 25, 20, -10 ),
            new( 25, 20, 10 ),
            new( 25, 20, -10 ),
            new( 10, 30, -10 ),
            new( 10, 30, 10 ),
            new( 10, 30, -10 ),
            new( 15, 45, -10 ),
            new( 15, 45, 10 ),
            new( 15, 45, -10 ),
            new( 40, 30, -10 ),
            new( 40, 30, 10 ),
            new( 40, 30, -10 ),
            new( 50, 0, -10 ),
            new( 40, -30, -10 ),
            new( 15, -50, -10 ),
            new( -15, -50, -10 ),
            new( -40, -30, -10 ),
            new( -50, 0, -10 ),
            new( -40, 30, -10 )
        };
        static readonly int[] MODELE_E5_SAUTS = { -1 };
        static readonly Vector3[] MODELE_E6 =
        {
            new( -25, 0, 0 ),
            new( 0, -10, 0 ),
            new( 25, 0, 0 ),
            new( 0, -10, -30 ),
            new( 0, -10, 0 ),
            new( 0, -10, -30 ),
            new( -25, 0, 0 )
        };
        static readonly int[] MODELE_E6_SAUTS = { -1 };
        static readonly Vector3[] MODELE_E7 =
        {
            new( 0, 20, 0),
            new( 40, 0, 0),
            new( 0, -20, 0),
            new( -40, 0, 0 ),
            new( 0, 20, 0 ),
            new( 10, 0, 0 ),
            new( 0, -20, 0 ),
            new( -10, 0, 0 ),
            new( 0, 20, 0 )
        };
        static readonly int[] MODELE_E7_SAUTS = { -1 };
        static readonly Vector3[] MODELE_E7_1 =
        {
            new( 0, 10, 0 ),
            new( -8, -6, 0 ),
            new( 8, -6, 0 ),
            new( 0, 10, 0 )
        };
        static readonly int[] MODELE_E7_1_SAUTS = { -1 };
        public static readonly Vector3[][] modeles_ennemis =
        {
            MODELE_E1, MODELE_E2, MODELE_E3, new Vector3[]{ }, MODELE_E5,
            MODELE_E6, MODELE_E7, MODELE_E1, MODELE_E2, MODELE_E3,
            new Vector3[]{ }, MODELE_E5, MODELE_E6, MODELE_E7, MODELE_P,
            MODELE_E7_1, MODELE_E7_1
        };
        public static readonly int[][] lignes_a_sauter_ennemis =
        {
            MODELE_E1_SAUTS, MODELE_E2_SAUTS, MODELE_E3_SAUTS, new int[1]{-1}, MODELE_E5_SAUTS,
            MODELE_E6_SAUTS, MODELE_E7_SAUTS, MODELE_E1_SAUTS, MODELE_E2_SAUTS, MODELE_E3_SAUTS,
            new int[1]{-1}, MODELE_E5_SAUTS, MODELE_E6_SAUTS, MODELE_E7_SAUTS, MODELE_P_SAUTS,
            MODELE_E7_1_SAUTS, MODELE_E7_1_SAUTS
        };

        // modèles items
        static readonly Vector3[] MODELE_I1 =
        {
            new(-5, -10, 0 ),
            new(-15, -10, 0),
            new(-20, -5, 0),
            new(-5, -5, 0),
            new(-5, -20, 0),
            new(0, -25, 0),
            new(10, -25, 0),
            new(5, -20, 0),
            new(-5, -20, 0),
            new(5, -20, 0),
            new(5, -5, 0),
            new(20, -5, 0),
            new(25, -10, 0),
            new(10, -10, 0),
            new(10, -25, 0),
            new(10, -10, 0),
            new( 25, -10, 0),
            new( 25, 0, 0),
            new( 20, 5, 0),
            new( 20, -5, 0),
            new(20, 5, 0),
            new(5, 5, 0),
            new(5, 20, 0),
            new(10, 15, 0),
            new(10, 5, 0),
            new(10, 15, 0),
            new(5, 20, 0),
            new(-5, 20, 0),
            new(-5, 5, 0),
            new(-20, 5, 0),
            new(-20, -5, 0)
        };
        static readonly int[] MODELE_I1_SAUTS = { -1 };
        static readonly Vector3[] MODELE_I3 =
        {
            new(-19, 22,0),
            new(-70, 0,0),
            new(-19, -22 ,0),
            new(-19, 22 ,0),
            new(-30, 12 ,0),
            new(-30, -12 ,0),
            new(-12, -30 ,0),
            new(12, -30 ,0),
            new(19, -22 ,0),
            new(70, 0 ,0),
            new(19, 22 ,0),
            new(70, 0 ,0),
            new(19, -22 ,0),
            new(30, -12 ,0),
            new(30, 12 ,0),
            new(12, 30 ,0),
            new(-12, 30 ,0),
            new(-19, 22 ,0),
            new(-14, -6 ,0),
            new(-8, 6 ,0),
            new(-8, -6 ,0),
            new(-14, 6 ,0),
            new(-4, -12 ,0),
            new(10, -12 ,0),
            new(10, 0 ,0),
            new(-4, 0 ,0),
            new(-4, 12 ,0),
            new(10, 12 ,0)
        };
        static readonly int[] MODELE_I3_SAUTS = { 3, 18, 20, 22, -1 };
        static readonly Vector3[] MODELE_I4 =
        {
            new(19, 22 ,0),
            new(70, 0 ,0),
            new(19, -22 ,0),
            new(19, 22 ,0),
            new(30, 12 ,0),
            new(30, -12 ,0),
            new(12, -30 ,0),
            new(-12, -30 ,0),
            new(-19, -22 ,0),
            new(-70, 0 ,0),
            new(-19, 22 ,0),
            new(-70, 0 ,0),
            new(-19, -22 ,0),
            new(-30, -12 ,0),
            new(-30, 12 ,0),
            new(-12, 30 ,0),
            new(12, 30 ,0),
            new(19, 22 ,0),
            new(14, -6 ,0),
            new(8, 6 ,0),
            new(8, -6 ,0),
            new(14, 6 ,0),
            new(4, -12 ,0),
            new(-10, -12 ,0),
            new(-10, 0 ,0),
            new(4, 0 ,0),
            new(-10, 0 ,0),
            new(-10, 12 ,0),
            new(4, 12 ,0)
        };
        static readonly int[] MODELE_I4_SAUTS = { 3, 18, 20, 22, -1 };
        static readonly Vector3[] MODELE_I5 =
        {
            new(-19, 22 ,0),
            new(-70, 0 ,0),
            new(-19, -22 ,0),
            new(-70, 0 ,0),
            new(-19, 22 ,0),
            new(-30, 12 ,0),
            new(-30, -12 ,0),
            new(-12, -30 ,0),
            new(12, -30 ,0),
            new(19, -22 ,0),
            new(70, 0 ,0),
            new(19, 22 ,0),
            new(70, 0 ,0),
            new(19, -22 ,0),
            new(30, -12 ,0),
            new(30, 12 ,0),
            new(12, 30 ,0),
            new(-12, 30 ,0),
            new(-19, 22 ,0),
            new(-16, 0 ,0),
            new(-4, 0 ,0),
            new(0, -4 ,0),
            new(0, -16 ,0),
            new(16, 0 ,0),
            new(4, 0 ,0),
            new(0, 4 ,0),
            new(0, 16 ,0),
            new(0, 10 ,0),
            new(-10, 0 ,0),
            new(0, -10 ,0),
            new(10, 0 ,0),
            new(0, 10  ,0),
            new(0, 0, 0),
            new(0, 0, 0)
        };
        static readonly int[] MODELE_I5_SAUTS = { 19, 21, 23, 25, 32, -1 };
        static readonly Vector3[] MODELE_I6 =
        {
            new(-19, -22 ,0),
            new(-70, 0 ,0),
            new(-19, 22 ,0),
            new(-70, 0 ,0),
            new(-19, -22 ,0),
            new(-30, -12 ,0),
            new(-30, 12 ,0),
            new(-12, 30 ,0),
            new(12, 30 ,0),
            new(19, 22 ,0),
            new(70, 0 ,0),
            new(19, -22 ,0),
            new(70, 0 ,0),
            new(19, 22 ,0),
            new(30, 12 ,0),
            new(30, -12 ,0),
            new(12, -30 ,0),
            new(-12, -30 ,0),
            new(-19, -22 ,0),
            new(-16, 10 ,0),
            new(-10, -10 ,0),
            new(-4, 10 ,0),
            new(-10, 10 ,0),
            new(-10, -10 ,0),
            new(4, 10 ,0),
            new(10, -10 ,0),
            new(16, 10 ,0),
            new(10, 10 ,0),
            new(10, -10 , 0)
        };
        static readonly int[] MODELE_I6_SAUTS = { 19, 22, 24, 27, -1 };
        static readonly Vector3[] MODELE_I7 =
        {
            new(-19, 22 ,0),
            new(-70, 0 ,0),
            new(-19, -22 ,0),
            new(-70, 0 ,0),
            new(-19, 22 ,0),
            new(-30, 12 ,0),
            new(-30, -12 ,0),
            new(-12, -30 ,0),
            new(12, -30 ,0),
            new(19, -22 ,0),
            new(70, 0 ,0),
            new(19, 22 ,0),
            new(70, 0 ,0),
            new(19, -22 ,0),
            new(30, -12 ,0),
            new(30, 12 ,0),
            new(12, 30 ,0),
            new(-12, 30 ,0),
            new(-19, 22 ,0),
            new(-6, 20 ,0),
            new(6, 12 ,0),
            new(-6, 4 ,0),
            new(6, -4 ,0),
            new(-6, -12 ,0),
            new(6, -20 , 0)
        };
        static readonly int[] MODELE_I7_SAUTS = { 19, -1 };
        public static readonly Vector3[][] modeles_items =
        {
            MODELE_I3, MODELE_I4, MODELE_I5, MODELE_I6, MODELE_I7, MODELE_I1, new Vector3[0]
        };
        public static readonly int[][] lignes_a_sauter_items =
        {
            MODELE_I3_SAUTS, MODELE_I4_SAUTS, MODELE_I5_SAUTS,
            MODELE_I6_SAUTS, MODELE_I7_SAUTS, MODELE_I1_SAUTS, new int[0]
        };
    }

    public static class SaveLoad
    {
        public static void Save()
        {
            using (FileStream sw = File.Open(@"save.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                string encoded = (Program.niveau * 842593 + 395248).ToString();
                sw.Write(Encoding.UTF8.GetBytes(encoded));
                sw.Close();
            }
        }
        public static void Load()
        {
            string read = File.ReadAllText(@"save.txt");

            if (!uint.TryParse(read, out uint num))
            {
                return;
            }

            if (num.ToString() == read && read != "0")
            {
                double check = (num - 395248) / 824593;
                if (!(check >= 0 && check <= 20 && check % 1 == 0))
                {
                    return;
                }
            }
            else
            {
                return;
            }

            Program.nv_continue = (ushort)((num - 395248) / 842593);
        }
    }
}