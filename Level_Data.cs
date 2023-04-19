using static Dysgenesis.Background;
using static Dysgenesis.Background.Text;
using static Dysgenesis.Program;

namespace Dysgenesis
{
    public static class Level_Data
    {
        public static byte[][] lvl_list = new byte[21][];
        public static byte[] arcade_ens = new byte[0];
        static bool locked;
        static int timer;
        public static void Init()
        { // doite à gauche!!!
            lvl_list[0] = new byte[0];
            lvl_list[1] = new byte[5] { 1, 1, 1, 1, 1 };
            lvl_list[2] = new byte[8] { 2, 2, 2, 1, 1, 2, 2, 1 };
            lvl_list[3] = new byte[10] { 3, 1, 2, 2, 1, 3, 3, 3, 1, 3 };
            lvl_list[4] = new byte[10] { 3, 3, 2, 2, 2, 2, 3, 3, 3, 2 };
            lvl_list[5] = new byte[10] { 3, 2, 4, 4, 3, 3, 1, 8, 4, 3 };
            lvl_list[6] = new byte[10] { 4, 8, 8, 4, 9, 3, 2, 8, 9, 8 };
            lvl_list[7] = new byte[10] { 9, 10, 8, 8, 5, 5, 4, 10, 9, 5 };
            lvl_list[8] = new byte[10] { 9, 11, 6, 5, 9, 8, 6, 5, 10, 6 };
            lvl_list[9] = new byte[10] { 11, 6, 8, 10, 7, 11, 10, 11, 7, 9 };
            lvl_list[10] = new byte[10] { 9, 7, 10, 10, 11, 10, 9, 8, 8, 9 };
            lvl_list[11] = new byte[10] { 10, 11, 12, 11, 7, 9, 12, 11, 10, 8 };
            lvl_list[12] = new byte[10] { 12, 10, 10, 9, 9, 10, 11, 11, 10, 12 };
            lvl_list[13] = new byte[10] { 12, 10, 11, 9, 13, 11, 12, 12, 10, 9 };
            lvl_list[14] = new byte[10] { 13, 12, 11, 13, 10, 10, 13, 12, 13, 10 };
            lvl_list[15] = new byte[10] { 12, 12, 13, 13, 11, 11, 14, 11, 12, 10 };
            lvl_list[16] = new byte[10] { 11, 11, 12, 12, 14, 13, 13, 11, 12, 12 };
            lvl_list[17] = new byte[8] { 12, 13, 13, 14, 13, 13, 14, 12 };
            lvl_list[18] = new byte[9] { 13, 14, 14, 12, 12, 13, 13, 12, 14 };
            lvl_list[19] = new byte[10] { 13, 14, 13, 13, 12, 12, 14, 11, 12, 14 };
            lvl_list[20] = new byte[1] { 15 };
        }
        public static void Generate_list()
        {
            byte[] result = new byte[(ushort)Math.Sqrt(level * 10) + 2]; // overflow @ beaucoup lol
            for (int i = 0; i < result.Length; i++)
            {
                // beaucoup de RNG() et de blabla
                int enemy_rng = RNG.Next(100);
                if (level > 1 && i < 3 && enemy_rng < 50)
                {
                    result[i] = 2;
                    continue;
                }
                enemy_rng = RNG.Next(100);
                if (level > 3 && i < 6 && enemy_rng < 40)
                {
                    result[i] = 3;
                    continue;
                }
                enemy_rng = RNG.Next(100);
                if (level > 4 && i < 8 && enemy_rng < 40)
                {
                    result[i] = 4;
                    continue;
                }
                enemy_rng = RNG.Next(100);
                if (level > 6 && i < 10 && enemy_rng < 30)
                {
                    result[i] = 5;
                    continue;
                }
                enemy_rng = RNG.Next(100);
                if (level > 8 && i < 12 && enemy_rng < 30)
                {
                    result[i] = 6;
                    continue;
                }
                enemy_rng = RNG.Next(100);
                if (level > 10 && i < 13 && enemy_rng < 30)
                {
                    result[i] = 7;
                    continue;
                }
                enemy_rng = RNG.Next(100);
                if (level > 8 && i < 15 && enemy_rng < 50)
                {
                    result[i] = 8;
                    continue;
                }
                enemy_rng = RNG.Next(100);
                if (level > 8 && i < 18 && enemy_rng < 40)
                {
                    result[i] = 9;
                    continue;
                }
                enemy_rng = RNG.Next(100);
                if (level > 10 && i < 20 && enemy_rng < 40)
                {
                    result[i] = 10;
                    continue;
                }
                enemy_rng = RNG.Next(100);
                if (level > 12 && i < 25 && enemy_rng < 40)
                {
                    result[i] = 11;
                    continue;
                }
                enemy_rng = RNG.Next(100);
                if (level > 15 && i < 30 && enemy_rng < 30)
                {
                    result[i] = 12;
                    continue;
                }
                enemy_rng = RNG.Next(100);
                if (level > 17 && i < 40 && enemy_rng < 30)
                {
                    result[i] = 13;
                    continue;
                }
                if (level >= 20)
                    result[i] = 14;
                else
                    result[i] = 1;
            }
            arcade_ens = result;
        }
        public static void Level_Change()
        {
            if (!locked)
            {
                locked = true;
                timer = 0;
                if (gamemode == 3)
                    timer = 199;
            }
            timer++;
            if (timer == 201 && (level != 20 || gamemode == 3))
                Son.PlaySFX(Son.SFX_list.level);
            if (timer > 200 && timer < 320 && (level != 20 || gamemode == 3))
            {
                DisplayText($"niveau {level + 1}", short.MinValue, short.MinValue, 5);
            }
            if (timer >= 350)
            {
                level++;
                if (gamemode == 2)
                    ens_needed = (byte)lvl_list[level].Length;
                if (gamemode == 3)
                {
                    Generate_list();
                    ens_needed = (byte)arcade_ens.Length;
                }
                ens_killed = 0;
                locked = false;
                if (level == 20)
                    Son.StopMusic();
                if (level == 21 && gamemode == 2)
                    gamemode = 6;
            }
        }
    }
}