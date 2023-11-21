using static SDL2.SDL;
using static Dysgenesis.Program;
using static Dysgenesis.Data;
using static System.Math;
using static Dysgenesis.Background;

namespace Dysgenesis
{
    public static class Projectile
    {
        public static float[,] pos = new float[60, 6];
        public static byte[] owner = new byte[pos.GetLength(0)];
        public static byte fire_rate = P_SHOOTING_COOLDOWN;
        public static uint cooldown;
        static Projectile()
        {
            for (int i = 0; i < pos.GetLength(0); i++)
            {
                pos[i, 0] = -1;//x
                pos[i, 1] = -1;//y
                pos[i, 2] = 0;//z
                pos[i, 3] = -1;//tx
                pos[i, 4] = -1;//ty
                pos[i, 5] = 0;//tz
            }
        }
        public static void Exist()
        {
            for (int i = 0; i < pos.GetLength(0); i++)
            {
                if (pos[i, 0] == -1)
                    continue;
                if (pos[i, 2] > pos[i, 5])
                    pos[i, 2]--;
                if (pos[i, 2] < pos[i, 5])
                    pos[i, 2]++;
                if (pos[i, 2] == pos[i, 5])
                    pos[i, 0] = -1;
            }
        }
        public static void TirJoueur()
        {
            if (player.powerup == 3 || player.powerup == 6)
                fire_rate = P_SHOOTING_COOLDOWN / 2;
            else if (player.powerup == 4 || player.powerup == 5)
                fire_rate = P_SHOOTING_COOLDOWN / 3;
            else if (player.powerup == 7)
                fire_rate = P_SHOOTING_COOLDOWN * 2;
            else
                fire_rate = P_SHOOTING_COOLDOWN;
            if (Keys.j && gTimer - cooldown > fire_rate)
            {
                cooldown = gTimer;
                int proj1 = -1, proj2 = -1, proj3 = -1, proj4 = -1, proj5 = -1, proj6 = -1;
                for (byte i = 0; i < pos.GetLength(0); i++)
                {
                    if (pos[i, 0] == -1)
                    {
                        if (proj2 != -1)
                        {
                            proj1 = i;
                            break;
                        }
                        if (proj3 != -1 || player.powerup != 6)
                        {
                            proj2 = i;
                            continue;
                        }
                        if (proj4 != -1)
                        {
                            proj3 = i;
                            continue;
                        }
                        if (proj5 != -1)
                        {
                            proj4 = i;
                            continue;
                        }
                        if (proj6 != -1)
                        {
                            proj5 = i;
                            continue;
                        }
                        else proj6 = i;
                    }
                }
                if (proj1 != -1 && proj2 != -1)
                {
                    int[] shoot_point1 = player.RenderCalc(1);
                    int[] shoot_point2 = player.RenderCalc(16);

                    pos[proj1, 0] = shoot_point1[0];
                    pos[proj1, 1] = shoot_point1[1];
                    pos[proj1, 2] = 1;
                    pos[proj1, 3] = pos[proj1, 0] - 45;
                    pos[proj1, 4] = pos[proj1, 1] - 200;
                    pos[proj1, 5] = G_DEPTH_LAYERS;
                    owner[proj1] = 0;

                    pos[proj2, 0] = shoot_point2[0];
                    pos[proj2, 1] = shoot_point2[1];
                    pos[proj2, 2] = 1;
                    pos[proj2, 3] = pos[proj2, 0] + 45;
                    pos[proj2, 4] = pos[proj2, 1] - 200;
                    pos[proj2, 5] = G_DEPTH_LAYERS;
                    owner[proj2] = 0;

                    if (player.powerup == 5)
                        FindTarget(proj1, proj2);
                    if (player.powerup == 6)
                    {
                        pos[proj3, 0] = shoot_point1[0];
                        pos[proj3, 1] = shoot_point1[1];
                        pos[proj3, 2] = 1;
                        pos[proj3, 3] = pos[proj3, 0];
                        pos[proj3, 4] = pos[proj3, 1] - 250;
                        pos[proj3, 5] = G_DEPTH_LAYERS;
                        owner[proj3] = 0;

                        pos[proj4, 0] = shoot_point1[0];
                        pos[proj4, 1] = shoot_point1[1];
                        pos[proj4, 2] = 1;
                        pos[proj4, 3] = pos[proj4, 0] + 20;
                        pos[proj4, 4] = pos[proj4, 1] - 200;
                        pos[proj4, 5] = G_DEPTH_LAYERS;
                        owner[proj4] = 0;

                        pos[proj5, 0] = shoot_point2[0];
                        pos[proj5, 1] = shoot_point2[1];
                        pos[proj5, 2] = 1;
                        pos[proj5, 3] = pos[proj5, 0];
                        pos[proj5, 4] = pos[proj5, 1] - 250;
                        pos[proj5, 5] = G_DEPTH_LAYERS;
                        owner[proj5] = 0;

                        pos[proj6, 0] = shoot_point2[0];
                        pos[proj6, 1] = shoot_point2[1];
                        pos[proj6, 2] = 1;
                        pos[proj6, 3] = pos[proj6, 0] - 20;
                        pos[proj6, 4] = pos[proj6, 1] - 200;
                        pos[proj6, 5] = G_DEPTH_LAYERS;
                        owner[proj6] = 0;
                    }
                    Son.PlaySFX(Son.SFX_list.shoot);
                }
            }
        }
        public static void TirEnemy(Enemy e, bool first)
        {
            try
            {
                byte proj = 255;
                for (byte i = 0; i < pos.GetLength(0); i++)
                {
                    if (pos[i, 0] == -1)
                    {
                        proj = i;
                        break;
                    }
                }
                if (proj != 255)
                {
                    if (first)
                    {
                        pos[proj, 0] = (float)(e.model[e.shoot_index,0] * Pow(0.95, e.depth) + e.x);
                        pos[proj, 1] = (float)(e.model[e.shoot_index,1] * Pow(0.95, e.depth) + e.y);
                    }
                    else
                    {
                        pos[proj, 0] = (float)(e.model[e.shoot_index_2,0] * Pow(0.95, e.depth) + e.x);
                        pos[proj, 1] = (float)(e.model[e.shoot_index_2,1] * Pow(0.95, e.depth) + e.y);
                    }
                    pos[proj, 2] = e.depth;
                    pos[proj, 3] = player.x;
                    pos[proj, 4] = player.y;
                    pos[proj, 5] = -1;
                    owner[proj] = 1;
                }
            }
            catch (Exception ex)
            {
                CrashReport(ex);
            }
        }
        static void FindTarget(int proj1, int proj2)
        {
            short closest = 99;
            short closest_distance = 9999;
            for (short i = 0; i < enemies.Length; i++)
            {
                if (enemies[i] != null)
                {
                    short distance = Distance(player.x, player.y, enemies[i].x, enemies[i].y);
                    if (distance < closest_distance)
                    {
                        closest = i;
                        closest_distance = distance;
                    }
                }
            }
            if (closest != 99)
            {
                pos[proj1, 3] = enemies[closest].x;
                pos[proj1, 4] = enemies[closest].y;
                pos[proj2, 3] = enemies[closest].x;
                pos[proj2, 4] = enemies[closest].y;
            }
        }
        public static int[] CalcDepths(int i, byte powerup7 = 0)
        {
            float[] cur_pos;
            float[] target_pos;
            if (owner[i] == 0)
            {
                cur_pos = new float[3] { pos[i, 0], pos[i, 1], pos[i, 2] };
                target_pos = new float[2] { pos[i, 3], pos[i, 4] };
            }
            else
            {
                cur_pos = new float[3] { pos[i, 3], pos[i, 4], pos[i, 2] };
                target_pos = new float[2] { pos[i, 0], pos[i, 1] };
            }
            byte depth = powerup7 == 0 ? (byte)cur_pos[2] : powerup7;
            return new int[4] {
                (int)((cur_pos[0] - target_pos[0]) * Pow(P_PROJ_SPEED, depth) + target_pos[0]),
                (int)((cur_pos[1] - target_pos[1]) * Pow(P_PROJ_SPEED, depth) + target_pos[1]),
                (int)((cur_pos[0] - target_pos[0]) * Pow(P_PROJ_SPEED, depth + 1) + target_pos[0]),
                (int)((cur_pos[1] - target_pos[1]) * Pow(P_PROJ_SPEED, depth + 1) + target_pos[1]),
            };
        }
        public static void Render()
        {
            for (int i = 0; i < pos.GetLength(0); i++)
            {
                if (pos[i, 0] == -1)
                    continue;
                if (owner[i] == 1)
                    SDL_SetRenderDrawColor(render, 255, 0, 0, 255);
                else
                    switch (player.powerup)
                    {
                        case 0:
                        case 1:
                        case 2:
                            SDL_SetRenderDrawColor(render, 255, 0, 0, 255);
                            break;
                        case 3:
                            SDL_SetRenderDrawColor(render, 255, 127, 0, 255);
                            break;
                        case 4:
                            SDL_SetRenderDrawColor(render, 255, 255, 0, 255);
                            break;
                        case 5:
                            SDL_SetRenderDrawColor(render, 64, 255, 64, 255);
                            break;
                        case 6:
                            SDL_SetRenderDrawColor(render, 0, 0, 255, 255);
                            break;
                        case 7:
                            SDL_SetRenderDrawColor(render, 127, 0, 255, 255);
                            break;
                    }
                if (!player.dead)
                    if (player.powerup != 7 || owner[i] == 1)
                    {
                        if (pos[i, 0] != -1)
                        {
                            int[] pos = CalcDepths(i);
                            if (Program.gamemode >= 4 || Program.gamemode == 1)
                                Background.NouveauDrawLine(render, pos[0], pos[1], pos[2], pos[3]);
                            else
                                SDL_RenderDrawLine(render, pos[0], pos[1], pos[2], pos[3]);
                        }
                    }
                    else
                    {
                        int[] attach_point;
                        if (i % 2 == 0)
                            attach_point = player.RenderCalc(1);
                        else
                            attach_point = player.RenderCalc(16);
                        pos[i, 0] = attach_point[0];
                        pos[i, 1] = attach_point[1];
                        for (byte j = 0; j < G_DEPTH_LAYERS; j++)
                        {
                            int[] pos = CalcDepths(i, j);
                            SDL_RenderDrawLine(render, pos[0] + RNG.Next(-5, 5), pos[1] + RNG.Next(-5, 5), pos[2] + RNG.Next(-5, 5), pos[3] + RNG.Next(-5, 5));
                        }
                    }
            }
        }
    }
    public static class Shockwave
    {
        static short x = 0, y = 0;
        static float r = 0, grow = 0;
        static bool shown = false;
        static uint cooldown = 0;
        public static void Spawn()
        {
            if (!shown && cooldown > 60 && player.shockwaves >= 1 && !player.dead)
            {
                cooldown = 0;
                player.shockwaves -= 1f;
                x = (short)player.x;
                y = (short)player.y;
                r = 0;
                grow = 160;
                shown = true;
                Son.PlaySFX(Son.SFX_list.wave);
            }
        }
        public static void Display()
        {
            cooldown++;
            if (shown)
            {
                SDL_SetRenderDrawColor(render, 0, 255, 255, 255);
                Dessiner(r);
                Dessiner(r);
                Dessiner(r);
                grow /= 1.2f;
                x = (short)player.x;
                y = (short)player.y;
                r = 150 - grow;
                if (r >= 149)
                    shown = false;
                if (cooldown >= 10)
                    for (int i = 0; i < enemies.Length; i++)
                    {
                        if (enemies[i] == null)
                            continue;
                        if (enemies[i].depth != 0)
                            continue;
                        if (Distance(enemies[i].x, enemies[i].y, player.x, player.y) <= 150 && gTimer % 3 == 0)
                            enemies[i].HP--;
                    }
            }
        }
        static void Dessiner(float size)
        {
            sbyte randint1 = 0;
            for (int i = 0; i < 50; i++)
            {
                sbyte randint2 = randint1;
                randint1 = (sbyte)RNG.Next(-20, 20);
                Background.NouveauDrawLine(render,
                    (int)(x + (size + randint1) * Sin(i * PI / 25)),
                    (int)(y + (size + randint1) * Cos(i * PI / 25)),
                    (int)(x + (size + randint2) * Sin((i + 1) * PI / 25)),
                    (int)(y + (size + randint2) * Cos((i + 1) * PI / 25)));
            }
        }
    }
    // vieux, j'ai fusioné tir enemi et tir joueur car ils avaient 80% du même code
    //public class Projectile
    //{
    //    public static class PlayerProjs
    //    {
    //        public static float[,] pos = new float[30, 5];
    //        public static byte fire_rate = P_SHOOTING_COOLDOWN;
    //        public static uint cooldown;
    //        public static void Init()
    //        {
    //            for (int i = 0; i < pos.GetLength(0); i++)
    //            {
    //                pos[i, 0] = -1;//x
    //                pos[i, 1] = -1;//y
    //                pos[i, 2] = 0;//z
    //                pos[i, 3] = -1;//tx
    //                pos[i, 4] = -1;//ty
    //            }
    //        }
    //        public static void Exist()
    //        {
    //            Shoot();
    //            Move();
    //        }
    //        static void Move()
    //        {
    //            for (int i = 0; i < pos.GetLength(0); i++)
    //            {
    //                if (pos[i,0] != -1)
    //                {
    //                    pos[i, 2]++;
    //                }
    //                if (pos[i, 2] >= G_DEPTH_LAYERS)
    //                {
    //                    pos[i, 0] = -1;
    //                    pos[i, 2] = 0;
    //                }
    //            }
    //        }
    //        static void Shoot()
    //        {
    //            if (player.powerup == 3)
    //                fire_rate = P_SHOOTING_COOLDOWN / 2;
    //            else if (player.powerup == 4)
    //                fire_rate = P_SHOOTING_COOLDOWN / 3;
    //            else if (player.powerup == 7)
    //                fire_rate = P_SHOOTING_COOLDOWN * 2;
    //            else
    //                fire_rate = P_SHOOTING_COOLDOWN;
    //            if (Keys.j && gTimer - cooldown > fire_rate)
    //            {
    //                cooldown = gTimer;
    //                int proj1 = -1, proj2 = -1, proj3 = -1, proj4 = -1, proj5 = -1, proj6 = -1;
    //                for (byte i = 0; i < pos.GetLength(0); i++)
    //                {
    //                    if (pos[i,0] == -1)
    //                    {
    //                        if (proj2 != -1)
    //                        {
    //                            proj1 = i;
    //                            break;
    //                        }
    //                        if (proj3 != -1 || player.powerup != 6)
    //                        {
    //                            proj2 = i;
    //                            continue;
    //                        }
    //                        if (player.powerup != 6) continue;
    //                        if (proj4 != -1)
    //                        {
    //                            proj3 = i;
    //                            continue;
    //                        }
    //                        if (proj5 != -1) 
    //                        {
    //                            proj4 = i;
    //                            continue;
    //                        }
    //                        if (proj6 != -1)
    //                        {
    //                            proj5 = i;
    //                            continue;
    //                        }
    //                        else proj6 = i;
    //                    }
    //                }
    //                if (proj1 != -1 && proj2 != -1)
    //                {
    //                    int[] shoot_point1 = player.RenderCalc(1);
    //                    int[] shoot_point2 = player.RenderCalc(16);

    //                    pos[proj1, 0] = shoot_point1[0];
    //                    pos[proj1, 1] = shoot_point1[1];
    //                    pos[proj1, 3] = pos[proj1, 0] - 45;
    //                    pos[proj1, 4] = pos[proj1, 1] - 200;

    //                    pos[proj2, 0] = shoot_point2[0];
    //                    pos[proj2, 1] = shoot_point2[1];
    //                    pos[proj2, 3] = pos[proj2, 0] + 45;
    //                    pos[proj2, 4] = pos[proj2, 1] - 200;

    //                    if (player.powerup == 5) FindTarget(proj1, proj2);
    //                    if (player.powerup == 6)
    //                    {
    //                        pos[proj3, 0] = shoot_point1[0];
    //                        pos[proj3, 1] = shoot_point1[1];
    //                        pos[proj3, 3] = pos[proj3, 0];
    //                        pos[proj3, 4] = pos[proj3, 1] - 250;

    //                        pos[proj4, 0] = shoot_point1[0];
    //                        pos[proj4, 1] = shoot_point1[1];
    //                        pos[proj4, 3] = pos[proj4, 0] + 20;
    //                        pos[proj4, 4] = pos[proj4, 1] - 200;

    //                        pos[proj5, 0] = shoot_point2[0];
    //                        pos[proj5, 1] = shoot_point2[1];
    //                        pos[proj5, 3] = pos[proj5, 0];
    //                        pos[proj5, 4] = pos[proj5, 1] - 250;

    //                        pos[proj6, 0] = shoot_point2[0];
    //                        pos[proj6, 1] = shoot_point2[1];
    //                        pos[proj6, 3] = pos[proj6, 0] - 20;
    //                        pos[proj6, 4] = pos[proj6, 1] - 200;
    //                    }
    //                    Son.PlaySFX(Son.SFX_list.shoot);
    //                }
    //            }
    //        }
    //        static void FindTarget(int proj1, int proj2)
    //        {
    //            short closest = 99;
    //            short closest_distance = 9999;
    //            for (short i = 0; i < enemies.Length; i++)
    //            {
    //                if (enemies[i] != null)
    //                {
    //                    short distance = DistanceFrom(player.x, player.y, enemies[i].x, enemies[i].y);
    //                    if (distance < closest_distance)
    //                    {
    //                        closest = i;
    //                        closest_distance = distance;
    //                    }
    //                }
    //            }
    //            if (closest != 99)
    //            {
    //                pos[proj1, 3] = enemies[closest].x;
    //                pos[proj1, 4] = enemies[closest].y;
    //                pos[proj2, 3] = enemies[closest].x;
    //                pos[proj2, 4] = enemies[closest].y;
    //            }
    //        }
    //        // fonction inutile, merged avec Move()
    //        //void Kill_self()
    //        //{
    //        //    for (int i = 0; i < pos.GetLength(0); i++)
    //        //    {
    //        //        if (pos[i,2] >= Game.DEPTH_LAYERS)
    //        //        {
    //        //            pos[i, 0] = -1;
    //        //            pos[i, 2] = 0;
    //        //        }
    //        //    }
    //        //}
    //        #region .
    //        #endregion
    //        // vieux calc depth, terrible à appeler. bon débarras.
    //        //public int Calc_Depth(int index, byte x_or_y, bool next_depth, byte l_or_r)
    //        //{
    //        //    float? real_pos = player_proj.pos[index, x_or_y];
    //        //    int x_offset = 45;
    //        //    if (l_or_r == 1) x_offset *= -1;
    //        //    float? total_offset = x_or_y == 0 ? player_proj.pos[index, 0] + x_offset : player_proj.pos[index, 1] - 200;
    //        //    double depth_factor = Pow(Player_Data.PROJECTILE_SPEED, (double)player_proj.pos[index, 2] - (next_depth ? 1 : 0));
    //        //    return (int)((real_pos - total_offset) * depth_factor + total_offset);
    //        //}
    //        public static int[] CalcDepths(int index, byte powerup7 = 0)
    //        {
    //            float[] cur_pos = new float[3] { pos[index, 0], pos[index, 1], pos[index, 2] };
    //            float[] target_pos = new float[2] { pos[index, 3], pos[index, 4] };
    //            byte depth = powerup7 == 0 ? (byte)cur_pos[2] : powerup7;
    //            return new int[4] {
    //                (int)((cur_pos[0] - target_pos[0]) * Pow(P_PROJ_SPEED, depth) + target_pos[0]),
    //                (int)((cur_pos[1] - target_pos[1]) * Pow(P_PROJ_SPEED, depth) + target_pos[1]),
    //                (int)((cur_pos[0] - target_pos[0]) * Pow(P_PROJ_SPEED, depth + 1) + target_pos[0]),
    //                (int)((cur_pos[1] - target_pos[1]) * Pow(P_PROJ_SPEED, depth + 1) + target_pos[1]),
    //            };
    //        }
    //        public static void Render()
    //        {
    //            switch (player.powerup)
    //            {
    //                case 0:
    //                case 1:
    //                case 2:
    //                    SDL_SetRenderDrawColor(render, 255, 0, 0, 255);
    //                    break;
    //                case 3:
    //                    SDL_SetRenderDrawColor(render, 255, 127, 0, 255);
    //                    break;
    //                case 4:
    //                    SDL_SetRenderDrawColor(render, 255, 255, 0, 255);
    //                    break;
    //                case 5:
    //                    SDL_SetRenderDrawColor(render, 64, 255, 64, 255);
    //                    break;
    //                case 6:
    //                    SDL_SetRenderDrawColor(render, 0, 0, 255, 255);
    //                    break;
    //                case 7:
    //                    SDL_SetRenderDrawColor(render, 127, 0, 255, 255);
    //                    break;
    //            }
    //            if (!player.dead)
    //                if (player.powerup != 7)
    //                {
    //                    for (int i = 0; i < pos.GetLength(0); i++)
    //                    {
    //                        if (pos[i, 0] != -1)
    //                        {
    //                            int[] pos = CalcDepths(i);
    //                            Background.NouveauDrawLine(render, pos[0], pos[1], pos[2], pos[3]);
    //                        }
    //                    }
    //                }
    //                else
    //                {
    //                    for (int i = 0; i < pos.GetLength(0); i++)
    //                    {
    //                        if (pos[i, 0] == -1)
    //                            continue;
    //                        if (i % 2 == 0)
    //                        {
    //                            int[] attach_point = player.RenderCalc(1);
    //                            pos[i, 0] = attach_point[0];
    //                            pos[i, 1] = attach_point[1];
    //                        }
    //                        else
    //                        {
    //                            int[] attach_point = player.RenderCalc(16);
    //                            pos[i, 0] = attach_point[0];
    //                            pos[i, 1] = attach_point[1];
    //                        }
    //                        for (byte j = 0; j < G_DEPTH_LAYERS; j++)
    //                        {
    //                            int[] pos = CalcDepths(i, j);
    //                            Background.NouveauDrawLine(render, pos[0] + RNG.Next(-5, 5), pos[1] + RNG.Next(-5, 5), pos[2] + RNG.Next(-5, 5), pos[3] + RNG.Next(-5, 5));
    //                        }
    //                    }
    //                }
    //        }
    //    }
    //    public static class EnemyProjs
    //    {
    //        public static float[,] pos = new float[30,5];
    //        public static void Init()
    //        {
    //            for (int i = 0; i < pos.GetLength(0); i++)
    //            {
    //                pos[i, 0] = -1;
    //                pos[i, 1] = -1;
    //                pos[i, 2] = G_DEPTH_LAYERS;
    //                pos[i, 3] = -1;
    //                pos[i, 4] = -1;
    //            }
    //        }
    //        public static void Exist()
    //        {
    //            Move();
    //        }
    //        static void Move()
    //        {
    //            for (int i = 0; i < pos.GetLength(0); i++)
    //            {
    //                if (pos[i, 0] != -1)
    //                {
    //                    pos[i, 2]--;
    //                }
    //                if (pos[i, 2] <= 0)
    //                {
    //                    pos[i, 0] = -1;
    //                    pos[i, 2] = G_DEPTH_LAYERS;
    //                }
    //            }
    //        }
    //        public static void Shoot(Enemy e, bool first)
    //        {
    //            try
    //            {
    //                byte proj = 255;
    //                for (byte i = 0; i < pos.GetLength(0); i++)
    //                {
    //                    if (pos[i, 0] == -1)
    //                    {
    //                        proj = i;
    //                        break;
    //                    }
    //                }
    //                if (proj < pos.GetLength(0))
    //                {
    //                    if (first)
    //                    {
    //                        pos[proj, 0] = (float)(e.model[e.shoot_index][0] * Pow(0.95, e.depth) + e.x);
    //                        pos[proj, 1] = (float)(e.model[e.shoot_index][1] * Pow(0.95, e.depth) + e.y);
    //                        pos[proj, 3] = player.x;
    //                        pos[proj, 4] = player.y;
    //                    }
    //                    else
    //                    {
    //                        pos[proj, 0] = (float)(e.model[e.shoot2_index][0] * Pow(0.95, e.depth) + e.x);
    //                        pos[proj, 1] = (float)(e.model[e.shoot2_index][1] * Pow(0.95, e.depth) + e.y);
    //                        pos[proj, 3] = player.x;
    //                        pos[proj, 4] = player.y;
    //                    }
    //                    if (e.type == 15)
    //                        pos[proj, 2] = 20;
    //                }
    //            }
    //            catch (Exception ex)
    //            {
    //                CrashReport(ex);
    //            }
    //        }
    //        public static int[] CalcDepths(int index)
    //        {
    //            float[] self_pos = new float[3] { pos[index, 0], pos[index, 1], pos[index, 2] };
    //            float[] target_pos = new float[2] { pos[index, 3], pos[index, 4] };
    //            return new int[4] {
    //                (int)((target_pos[0] - self_pos[0]) * Pow(P_PROJ_SPEED, self_pos[2]) + self_pos[0]),
    //                (int)((target_pos[1] - self_pos[1]) * Pow(P_PROJ_SPEED, self_pos[2]) + self_pos[1]),
    //                (int)((target_pos[0] - self_pos[0]) * Pow(P_PROJ_SPEED, self_pos[2] + 1) + self_pos[0]),
    //                (int)((target_pos[1] - self_pos[1]) * Pow(P_PROJ_SPEED, self_pos[2] + 1) + self_pos[1]),
    //            };
    //        }
    //        public static void Render()
    //        {
    //            SDL_SetRenderDrawColor(render, 255, 0, 0, 255);
    //            if (!player.dead)
    //                for (int i = 0; i < pos.GetLength(0); i++)
    //                {
    //                    if (pos[i, 0] != -1)
    //                    {
    //                        int[] depths = CalcDepths(i);
    //                        Background.NouveauDrawLine(render, depths[0], depths[1], depths[2], depths[3]);
    //                    }
    //                }
    //        }
    //    }
}