using static Dysgenesis.Data;
using static Dysgenesis.Program;
using static SDL2.SDL;
using static Dysgenesis.Background.Text;
using static Dysgenesis.Background;
namespace Dysgenesis
{
    public class Enemy
    {
        public byte type = 0;
        public byte special = 3;
        public byte damage_radius = 30;
        public byte alpha = 255;

        public short HP = 2;
        public short depth = G_DEPTH_LAYERS;
        public short shoot_index = -1;
        public short shoot_index_2 = -1;

        public float pitch = 0.3f;
        public float roll;
        public float fire_cooldown;
        public float speed;
        public float z_speed;
        public float vx = 0;
        public float vy = 0;
        public float x = W_SEMI_LARGEUR;
        public float y = W_SEMI_HAUTEUR;
        public float scale = 1f;

        public uint lTimer = 0;
        public uint color = 0xFFFFFFFF;

        public float[] target = new float[2] { player.x, player.y };
        public sbyte[,] model = new sbyte[0, 0];
        short[] skipped_line_indexes = new short[0];
        public Enemy(byte enemy_type, byte new_special = 3, short spx = -1, short spy = -1, short spz = -1)
        {
            try
            {
                type = enemy_type;
                switch (type)
                {
                    case 1:
                        speed = 0.2f;
                        z_speed = E_AVG_Z_SPEED;
                        fire_cooldown = 0;
                        HP = 1;
                        color = 0xFFFF00;
                        shoot_index = 18;
                        damage_radius = 30;
                        skipped_line_indexes = new short[2] { 12, 17 };
                        model = MODELE_E1;
                        break;
                    case 2:
                        speed = E_AVG_SPEED / 2;
                        z_speed = E_AVG_Z_SPEED;
                        fire_cooldown = 2;
                        HP = 3;
                        color = 0xFF7F00;
                        shoot_index = 61;
                        shoot_index_2 = 45;
                        damage_radius = 30;
                        skipped_line_indexes = new short[2] { 48, 50 };
                        model = MODELE_E2;
                        break;
                    case 3:
                        speed = E_AVG_SPEED / 2;
                        z_speed = E_AVG_Z_SPEED * 2;
                        fire_cooldown = 2;
                        HP = 3;
                        color = 0x00FF00;
                        shoot_index = 0;
                        shoot_index_2 = 8;
                        damage_radius = 50;
                        skipped_line_indexes = new short[0];
                        model = MODELE_E3;
                        break;
                    case 4:
                        speed = 0.2f;
                        z_speed = E_AVG_Z_SPEED / 2;
                        fire_cooldown = 0;
                        damage_radius = 50;
                        HP = 10;
                        color = 0x00FFFF;
                        model = new sbyte[0, 0];
                        break;
                    case 5:
                        speed = E_AVG_SPEED * 1.5f;
                        z_speed = E_AVG_Z_SPEED;
                        fire_cooldown = 4;
                        HP = 10;
                        color = 0x0000FF;
                        shoot_index = 2;
                        shoot_index_2 = 21;
                        damage_radius = 60;
                        model = MODELE_E5;
                        break;
                    case 6:
                        speed = E_AVG_SPEED / 2;
                        z_speed = E_AVG_Z_SPEED / 2;
                        fire_cooldown = 2;
                        HP = (short)(4 / Math.Pow(2, new_special - 3));
                        color = 0x7F007F;
                        special = new_special;
                        shoot_index = 3;
                        damage_radius = 40;
                        model = MODELE_E6;
                        break;
                    case 7:
                        speed = E_AVG_SPEED * 2;
                        z_speed = 0;
                        fire_cooldown = 3;
                        HP = 10;
                        color = 0xFF00FF;
                        skipped_line_indexes = new short[0] { };
                        shoot_index = 5;
                        damage_radius = 100;
                        depth = 49;
                        special = 8;
                        model = MODELE_E7;
                        break;
                    case 8:
                        speed = E_AVG_SPEED;
                        z_speed = E_AVG_Z_SPEED;
                        fire_cooldown = 1;
                        HP = 3;
                        color = 0x7F7F00;
                        shoot_index = 18;
                        damage_radius = 30;
                        skipped_line_indexes = new short[2] { 12, 17 };
                        model = MODELE_E1;
                        break;
                    case 9:
                        speed = E_AVG_SPEED * 2;
                        z_speed = E_AVG_Z_SPEED;
                        fire_cooldown = 1;
                        HP = 5;
                        color = 0x7F4000;
                        shoot_index = 61;
                        shoot_index_2 = 63;
                        damage_radius = 30;
                        skipped_line_indexes = new short[2] { 48, 50 };
                        model = MODELE_E2;
                        break;
                    case 10:
                        speed = E_AVG_SPEED * 2;
                        z_speed = E_AVG_Z_SPEED * 2;
                        fire_cooldown = 1;
                        HP = 7;
                        color = 0x007F00;
                        shoot_index = 0;
                        shoot_index_2 = 8;
                        damage_radius = 50;
                        skipped_line_indexes = new short[0];
                        model = MODELE_E3;
                        break;
                    case 11:
                        speed = E_AVG_SPEED;
                        z_speed = E_AVG_Z_SPEED;
                        fire_cooldown = 2;
                        damage_radius = 50;
                        HP = 15;
                        color = 0x007F7F;
                        model = new sbyte[0, 0];
                        break;
                    case 12:
                        speed = E_AVG_SPEED * 2;
                        z_speed = E_AVG_Z_SPEED;
                        fire_cooldown = 3;
                        HP = 12;
                        color = 0x00007F;
                        shoot_index = 2;
                        shoot_index_2 = 21;
                        damage_radius = 60;
                        model = MODELE_E5;
                        break;
                    case 13:
                        speed = E_AVG_SPEED * 2;
                        z_speed = E_AVG_Z_SPEED / 4;
                        fire_cooldown = 1;
                        HP = (short)(8 / Math.Pow(2, new_special - 3));
                        color = 0x400080;
                        special = new_special;
                        shoot_index = 3;
                        damage_radius = 50;
                        model = MODELE_E6;
                        break;
                    case 14:
                        speed = E_AVG_SPEED * 2;
                        z_speed = E_AVG_Z_SPEED / 8;
                        fire_cooldown = 4;
                        HP = 15;
                        color = 0x7F007F;
                        skipped_line_indexes = new short[0] { };
                        shoot_index = 5;
                        damage_radius = 100;
                        depth = 50;
                        special = 8;
                        model = MODELE_E7;
                        break;
                    case 15:
                        speed = E_AVG_SPEED * 4;
                        z_speed = 0;
                        fire_cooldown = 0.5f;
                        HP = E_15_MAX_HP;
                        color = 0xFF0000;
                        depth = 7;
                        special = 110;
                        shoot_index = 1;
                        shoot_index_2 = 16;
                        damage_radius = 50;
                        model = new sbyte[player.model.GetLength(0), player.model.GetLength(1)];
                        for (int i = 0; i < player.model.GetLength(0); i++)
                        {
                            model[i, 0] = player.model[i, 0];
                            model[i, 1] = (sbyte)-player.model[i, 1];
                            model[i, 2] = (sbyte)-player.model[i, 2];
                        }
                        break;
                    case 16:
                        speed = E_AVG_SPEED;
                        z_speed = E_AVG_Z_SPEED * 1.5f;
                        fire_cooldown = 99;
                        special = 0;
                        depth = 20;
                        HP = 1;
                        color = 0xFF00FF;
                        model = MODELE_E7_1;
                        break;
                    case 17:
                        speed = E_AVG_SPEED;
                        z_speed = E_AVG_Z_SPEED * 1.5f;
                        fire_cooldown = 99;
                        special = 0;
                        depth = 20;
                        HP = 1;
                        color = 0x7F007F;
                        model = MODELE_E7_1;
                        break;
                }
                if (new_special != 3)
                {
                    x = spx;
                    y = spy;
                    depth = spz;
                }
                else
                {
                    x = RNG.Next(200, W_LARGEUR - 200);
                    y = RNG.Next(200, W_HAUTEUR - 300);
                }
            }
            catch (Exception ex)
            {
                CrashReport(ex);
            }
        }
        public void Exist(ref Enemy e)
        {
            try
            {
                lTimer++;
                if (type == 15 && special != 99)
                {
                    Enemy15(ref e);
                    return;
                }
                UpdateModele();
                Move();
                ProjCollision(ref e);
                if (e != null)
                    PlayerCollision(ref e);
                if (e != null && BombePulsar.HP_bombe > 0)
                    Tirer();
            }
            catch (Exception ex)
            {
                CrashReport(ex);
            }
        }
        public void UpdateModele()
        {
            try
            {
                byte size;
                double speed;
                switch (type)
                {
                    case 1:
                        speed = 50.0;
                        goto Octohedron;
                    case 8:
                        speed = 20.0;
                    Octohedron:
                        for (int i = 0; i < model.GetLength(0); i++)
                        {
                            if (i == 0 || i == 3 || i == 9)
                            {
                                model[i, 0] = (sbyte)(Math.Cos(gTimer / speed) * 20);
                                model[i, 2] = (sbyte)(Math.Sin(gTimer / speed) * 20);
                            }
                            if (i == 1 || i == 5 || i == 10)
                            {
                                model[i, 0] = (sbyte)(Math.Cos(gTimer / speed + 2) * 20);
                                model[i, 2] = (sbyte)(Math.Sin(gTimer / speed + 2) * 20);
                            }
                            if (i == 2 || i == 7)
                            {
                                model[i, 0] = (sbyte)(Math.Cos(gTimer / speed + 4) * 20);
                                model[i, 2] = (sbyte)(Math.Sin(gTimer / speed + 4) * 20);
                            }
                        }
                        break;
                    case 3:
                        speed = 50.0;
                        goto blocVert;
                    case 10:
                        speed = 20.0;
                    blocVert:
                        for (int i = 0; i < model.GetLength(0); i++)
                        {
                            switch (i)
                            {
                                case 16:
                                case 19:
                                    model[i, 1] = (sbyte)(Math.Cos(gTimer / speed) * 10);
                                    model[i, 2] = (sbyte)(Math.Sin(gTimer / speed) * 10);
                                    break;
                                case 13:
                                case 17:
                                    model[i, 1] = (sbyte)(Math.Cos(gTimer / speed + 1.5) * 10);
                                    model[i, 2] = (sbyte)(Math.Sin(gTimer / speed + 1.5) * 10);
                                    break;
                                case 6:
                                case 12:
                                    model[i, 1] = (sbyte)(Math.Cos(gTimer / speed + 3) * 10);
                                    model[i, 2] = (sbyte)(Math.Sin(gTimer / speed + 3) * 10);
                                    break;
                                case 7:
                                case 20:
                                    model[i, 1] = (sbyte)(Math.Cos(gTimer / speed + 4.5) * 10);
                                    model[i, 2] = (sbyte)(Math.Sin(gTimer / speed + 4.5) * 10);
                                    break;
                                case 15:
                                case 23:
                                    model[i, 1] = (sbyte)(Math.Cos(gTimer / -speed) * 10);
                                    model[i, 2] = (sbyte)(Math.Sin(gTimer / -speed) * 10);
                                    break;
                                case 4:
                                case 14:
                                    model[i, 1] = (sbyte)(Math.Cos(gTimer / -speed + 1.5) * 10);
                                    model[i, 2] = (sbyte)(Math.Sin(gTimer / -speed + 1.5) * 10);
                                    break;
                                case 5:
                                case 25:
                                    model[i, 1] = (sbyte)(Math.Cos(gTimer / -speed + 3) * 10);
                                    model[i, 2] = (sbyte)(Math.Sin(gTimer / -speed + 3) * 10);
                                    break;
                                case 21:
                                case 24:
                                    model[i, 1] = (sbyte)(Math.Cos(gTimer / -speed + 4.5) * 10);
                                    model[i, 2] = (sbyte)(Math.Sin(gTimer / -speed + 4.5) * 10);
                                    break;
                            }
                        }
                        break;
                    case 4:
                        SDL_SetRenderDrawColor(render, 0, 255, 255, 255);
                        goto NRGball;
                    case 11:
                        SDL_SetRenderDrawColor(render, 0, 127, 127, 255);
                    NRGball:
                        size = (byte)(40 * Math.Pow(0.95, depth));
                        DessinerCercle((short)x, (short)y, size, 50);
                        for (int i = 0; i < 50; i++)
                        {
                            float ang = RNG.NextSingle() * (float)Math.PI;
                            SDL_RenderDrawLine(render, (int)(RNG.Next(-size, size) * Math.Cos(ang) + x), (int)(RNG.Next(-size, size) * Math.Sin(ang) + y), (int)x, (int)y);
                        }
                        break;
                    case 5:
                        speed = 50.0;
                        goto Split;
                    case 12:
                        speed = 20.0;
                    Split:
                        for (int i = 0; i < model.GetLength(0); i++)
                        {
                            if (i == 49 || i == 56)
                            {
                                model[i, 0] = (sbyte)(Math.Cos(gTimer / speed) * 20);
                                model[i, 2] = (sbyte)(Math.Sin(gTimer / speed) * 20);
                            }
                            if (i == 53 || i == 50)
                            {
                                model[i, 0] = (sbyte)(Math.Cos(gTimer / speed + 1.5) * 20);
                                model[i, 2] = (sbyte)(Math.Sin(gTimer / speed + 1.5) * 20);
                            }
                            if (i == 52 || i == 54)
                            {
                                model[i, 0] = (sbyte)(Math.Cos(gTimer / speed + 3) * 20);
                                model[i, 2] = (sbyte)(Math.Sin(gTimer / speed + 3) * 20);
                            }
                            if (i == 55 || i == 57)
                            {
                                model[i, 0] = (sbyte)(Math.Cos(gTimer / speed + 4.5) * 20);
                                model[i, 2] = (sbyte)(Math.Sin(gTimer / speed + 4.5) * 20);
                            }
                        }
                        break;
                    //case 6:
                    //case 13:
                    //    if (special == 4 || special == 5)
                    //    {
                    //        special -= 3;
                    //        for (int i = 0; i < model.GetLength(0); i++)
                    //        {
                    //            model[i, 0] /= (sbyte)Math.Pow(2, special);
                    //            model[i, 1] /= (sbyte)Math.Pow(2, special);
                    //            model[i, 2] /= (sbyte)Math.Pow(2, special);
                    //        }
                    //    }
                    //    break;
                    case 7:
                        SDL_SetRenderDrawColor(render, 255, 0, 255, 255);
                        goto Patra;
                    case 14:
                        SDL_SetRenderDrawColor(render, 127, 0, 127, 255);
                    Patra:
                        float ang14 = (gTimer / 20f) % (float)(2 * Math.PI);
                        short centerX2, centerY2;
                        for (int i = 0; i < special; i++)
                        {
                            centerX2 = (short)(Math.Cos(ang14) * 80 * Math.Pow(0.95, depth) + x);
                            centerY2 = (short)(Math.Sin(ang14) * 80 * Math.Pow(0.95, depth) + y);
                            float subAng = (gTimer / -10f) % (float)(2 * Math.PI);
                            for (int j = 0; j < 3; j++)
                            {
                                SDL_RenderDrawLine(render,
                                    (int)(centerX2 + Math.Cos(subAng) * 20 * Math.Pow(0.95, depth)),
                                    (int)(centerY2 + Math.Sin(subAng) * 20 * Math.Pow(0.95, depth)),
                                    (int)(centerX2 + Math.Cos(subAng + (float)(2 * Math.PI) / 3) * 20 * Math.Pow(0.95, depth)),
                                    (int)(centerY2 + Math.Sin(subAng + (float)(2 * Math.PI) / 3) * 20 * Math.Pow(0.95, depth)));
                                subAng += (float)(2 * Math.PI) / 3;
                            }
                            ang14 += (float)(2 * Math.PI) / 8;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                CrashReport(ex);
            }
        }
        void Enemy15(ref Enemy e)
        {
            try
            {
                switch (special)
                {
                    case 100:
                        Son.StopMusic();
                        if (Distance(x, y, W_SEMI_LARGEUR, W_SEMI_HAUTEUR) > 30)
                        {
                            if (x > W_SEMI_LARGEUR)
                                x -= 3;
                            else
                                x += 3;
                            if (y > W_SEMI_HAUTEUR)
                                y -= 3;
                            else
                                y += 3;
                        }
                        else
                        {
                            special = 101;
                            lTimer = 0;
                        }
                        if (gTimer % 30 == 0)
                        {
                            Explosion.Call((short)(x + RNG.Next(-20, 20)), (short)(y + RNG.Next(-20, 20)), 0);
                            Son.PlaySFX(Son.SFX_list.e_boom);
                        }
                        break;
                    case 101:
                        x = W_SEMI_LARGEUR + RNG.Next(-2, 2);
                        y = W_SEMI_HAUTEUR + RNG.Next(-2, 2);
                        if (gTimer % 30 == 0)
                        {
                            Explosion.Call((short)(x + RNG.Next(-20, 20)), (short)(y + RNG.Next(-20, 20)), 0);
                            Son.PlaySFX(Son.SFX_list.e_boom);
                        }
                        if (lTimer > 300)
                            special = 102;
                        break;
                    case 102:
                        if (lTimer == 301)
                            Son.PlaySFX(Son.SFX_list.p_boom);
                        roll += 0.05f;
                        scale += 0.005f;
                        if (gTimer % 30 == 0)
                        {
                            Explosion.Call((short)(x + RNG.Next(-20, 20)), (short)(y + RNG.Next(-20, 20)), 0);
                        }
                        gFade = (byte)(lTimer - 300);
                        if (lTimer > 550)
                        {
                            gamemode = 6;
                            e = null;
                        }
                        break;
                    case 110:
                        lTimer = 0;
                        player.powerup = 0;
                        x = W_SEMI_LARGEUR;
                        y = W_SEMI_HAUTEUR;
                        special = 111;
                        depth = 100;
                        break;
                    case 111:
                        depth = (short)(100 - lTimer);
                        if (depth == 20)
                        {
                            special = 112;
                            lTimer = 0;
                            player.vy = -10;
                        }
                        break;
                    case 112:
                        if (lTimer == 1 && monologue_skip)
                            lTimer = 1021;
                        if (lTimer < 240)
                            DisplayText("allo pilote. ton vaisseau n'est pas le seul.", 300, 800, 3, scroll: (int)lTimer / 2);
                        else if (lTimer >= 260 && lTimer < 480)
                            DisplayText("un espion nous a transféré les plans.", 300, 800, 3, scroll: (int)lTimer / 2 - 130);
                        else if (lTimer >= 500 && lTimer < 800)
                            DisplayText("la bombe à pulsar est très puissante, et\n" +
                                "encore plus fragile. je ne peux pas te laisser près d'elle.", 300, 800, 3, scroll: (int)lTimer / 2 - 250);
                        else if (lTimer >= 820 && lTimer < 1020)
                            DisplayText("que le meilleur pilote gagne. en guarde.", 300, 800, 3, scroll: (int)lTimer / 2 - 410);
                        if (lTimer > 1020)
                        {
                            special = 99;
                            BombePulsar.HP_bombe = BP_MAX_HP;
                        }
                        if (lTimer == 884)
                            Son.PlaySFX(Son.SFX_list.ton);
                        break;
                }
            }
            catch (Exception ex)
            {
                CrashReport(ex);
            }
        }
        void ProjCollision(ref Enemy e)
        {
            try
            {
                for (int i = 0; i < Projectile.pos.GetLength(0); i++)
                {
                    if (Projectile.pos[i, 0] != -1 && e != null && Projectile.owner[i] == 0)
                    {
                        int[] proj_pos = Projectile.CalcDepths(i);
                        if (Distance(x, y, proj_pos[0], proj_pos[1]) < damage_radius && (depth == Projectile.pos[i, 2] || player.powerup == 7))
                        {
                            if (player.powerup == 7 && gTimer % 10 != 0)
                                continue;
                            HP--;
                            Explosion.Call((short)proj_pos[0], (short)proj_pos[1], (byte)depth);
                            if (type == 7 || type == 14)
                            {
                                if (special != 0)
                                {
                                    for (int j = 0; j < enemies.Length; j++)
                                    {
                                        if (enemies[j] == null)
                                        {
                                            enemies[j] = new Enemy(type == 7 ? (byte)16 : (byte)17, 0, (short)proj_pos[0], (short)proj_pos[1], (byte)depth);
                                            break;
                                        }
                                    }
                                    special--;
                                    HP++;
                                }
                                if (z_speed == 0 && special == 0) z_speed = E_AVG_Z_SPEED * 0.5f;
                            }
                            if (HP <= 0)
                            {
                                if (type == 6 || type == 13)
                                {
                                    if (special == 3 || special == 4)
                                        special -= 3;
                                    if (special == 0 || special == 1)
                                    {
                                        for (int j = 0; j < enemies.Length; j++)
                                        {
                                            if (enemies[j] == null)
                                            {
                                                enemies[j] = new Enemy(type, (byte)(special + 4), (short)(x + RNG.Next(-10, 10)), (short)(y + RNG.Next(-10, 10)), depth);
                                                break;
                                            }
                                        }
                                        for (int j = 0; j < enemies.Length; j++)
                                        {
                                            if (enemies[j] == null)
                                            {
                                                enemies[j] = new Enemy(type, (byte)(special + 4), (short)(x + RNG.Next(-10, 10)), (short)(y + RNG.Next(-10, 10)), depth);
                                                break;
                                            }
                                        }
                                    }
                                }
                                if (!((type == 6 && special != 0) || (type == 13 && special != 0) ||
                                    (type == 7 && special != 0) || (type == 14 && special != 0) || type == 16 || type == 17 || type == 15))
                                {
                                    ens_killed++;
                                    for (int j = 0; j < items.Length; j++)
                                    {
                                        if (items[j] == null)
                                        {
                                            items[j] = new Item();
                                            items[j].Spawn(ref items[j], x, y, (byte)depth);
                                            break;
                                        }
                                    }
                                }
                                if (type != 15)
                                    e = null;
                                else
                                    special = 100;
                                if (player.powerup != 7)
                                    Projectile.pos[i, 0] = -1;
                            }
                            if (player.powerup != 7)
                                Projectile.pos[i, 2] = G_DEPTH_LAYERS;
                            Son.PlaySFX(Son.SFX_list.e_boom);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CrashReport(ex);
            }
        }
        void PlayerCollision(ref Enemy e)
        {
            try
            {
                if (e.depth == 0)
                {
                    if (e.x > player.x - 50 && x < player.x + 50 && y > player.y - 30 && y < player.y + 30)
                    {
                        Explosion.Call((short)x, (short)y, 0);
                        Son.PlaySFX(Son.SFX_list.e_boom);
                        player.HP -= 3;
                        if ((special == 3 || special == 0 || type == 14) && type != 16 && type != 17) ens_needed++;
                        e = null;
                    }
                }
                if (player.dead && e != null)
                {
                    speed = 0;
                    z_speed = 0;
                }
                if (e != null)
                    if (HP <= 0 && type != 15)
                    {
                        if (type == 6 || type == 13)
                        {
                            if (special == 3)
                                special -= 3;
                            if (special == 0 || special == 1)
                            {
                                for (int j = 0; j < enemies.Length; j++)
                                {
                                    if (enemies[j] == null)
                                    {
                                        enemies[j] = new Enemy(type, (byte)(special + 4), (short)(x + RNG.Next(-10, 10)), (short)(y + RNG.Next(-10, 10)), depth);
                                        break;
                                    }
                                }
                                for (int j = 0; j < enemies.Length; j++)
                                {
                                    if (enemies[j] == null)
                                    {
                                        enemies[j] = new Enemy(type, (byte)(special + 4), (short)(x + RNG.Next(-10, 10)), (short)(y + RNG.Next(-10, 10)), depth);
                                        break;
                                    }
                                }
                            }
                        }
                        ens_killed++;
                        if ((type == 6 && special != 0) || (type == 13 && special != 0) ||
                            (type == 7 && special != 0) || (type == 14 && special != 0) ||
                            type == 16 || type == 17)
                            ens_killed--;
                        e = null;
                    }
            }
            catch (Exception ex)
            {
                CrashReport(ex);
            }
        }
        float[] UpdateTarget()
        {
            try
            {
                if (depth == 0)
                    return new float[2] { player.x, player.y };
                short dist_enemy_target = Distance(target[0], target[1], x, y);
                if (dist_enemy_target < 30)
                {
                    short dist_player_target = Distance(target[0], target[1], player.x, player.y);
                    if (dist_player_target < 205 * (depth / (float)G_DEPTH_LAYERS))
                    {
                        float num1, num2;
                        do
                        {
                            num1 = RNG.Next(100, W_LARGEUR - 100);
                            num2 = RNG.Next(100, W_HAUTEUR - 400);
                        }
                        while (Distance(num1, num2, player.x, player.y) < 800 * (W_LARGEUR / 1920.0));
                        return new float[2] { num1, num2 };
                    }
                    if (dist_player_target < 800)
                    {
                        return new float[2] { player.x, player.y - 200 * (depth / (float)G_DEPTH_LAYERS) };
                    }
                    else
                    {
                        float num1, num2;
                        do
                        {
                            num1 = RNG.Next(100, W_LARGEUR - 100);
                            num2 = RNG.Next(100, W_HAUTEUR - 400);
                        }
                        while (Distance(num1, num2, player.x, player.y) > 800 * (W_LARGEUR / 1920.0));
                        return new float[2] { num1, num2 };
                    }
                }
                return target;
            }
            catch (Exception ex)
            {
                CrashReport(ex);
                return Array.Empty<float>();
            }
        }
        void Move()
        {
            try
            {
                if (z_speed != 0 && depth != 0)
                {
                    if ((gTimer / 30f) % z_speed == 0) depth--;
                }
                if (speed != 0)
                {
                    target = UpdateTarget();
                    //SDL_SetRenderDrawColor(render, 255, 0, 0, 255); // debug: mettre point rouge sur target
                    //SDL_RenderDrawPoint(render, (int)e.target[0], (int)e.target[1]);
                    float true_speed = (speed + (depth == 0 ? 20 : 0)) / 60;
                    if (x < target[0])
                        vx += true_speed;
                    if (x > target[0])
                        vx -= true_speed;
                    if (y < target[1])
                        vy += true_speed;
                    if (y > target[1])
                        vy -= true_speed;
                    vx *= E_FRICTION;
                    vy *= E_FRICTION;
                    x += vx;
                    y += vy;
                }
                if (depth == 0)
                    speed *= 1.01f;
                pitch = (y - 540) / 540;
                if (type == 6 || type == 13 || type == 15)
                    roll = vx / 15;
                if (type == 15)
                    pitch += 0.5f;
            }
            catch (Exception ex)
            {
                CrashReport(ex);
            }
        }
        void Tirer()
        {
            try
            {
                if (shoot_index == -1 || ((type == 6 || type == 13) && special != 3))
                    return;
                if (gTimer % (fire_cooldown * G_FPS) == 0 && depth != 0)
                {
                    Projectile.TirEnemy(this, true);
                    if (shoot_index_2 != -1)
                        Projectile.TirEnemy(this, false);
                }
            }
            catch (Exception ex)
            {
                CrashReport(ex);
            }
        }
        public void RenderEnemy()
        {
            try
            {
                if (model == null)
                    return;
                if (type == 15 && gamemode != 7)
                    alpha = (byte)(255 - BombePulsar.lTimer * 1.3f);
                SDL_SetRenderDrawColor(render, (byte)((color & 0xFF0000) >> 16), (byte)((color & 0x00FF00) >> 8), (byte)(color & 0x0000FF), alpha);
                for (int i = 0; i < model.GetLength(0) - 1; i++)
                {
                    foreach (short j in skipped_line_indexes)
                    {
                        if (i + 1 == j) i++;
                    }
                    if (type == 6 || type == 13 || type == 15)
                    {
                        short[] real_xy = Rotate(model[i, 0], model[i, 1]);
                        short[] next_real_xy = Rotate(model[i + 1, 0], model[i + 1, 1]);
                        SDL_RenderDrawLine(render,
                            (int)(real_xy[0] * Math.Pow(0.95, depth) * scale + x),
                            (int)((-real_xy[1] + (model[i, 2] * pitch)) * Math.Pow(0.95, depth) * scale + y),
                            (int)(next_real_xy[0] * Math.Pow(0.95, depth) * scale + x),
                            (int)((-next_real_xy[1] + (model[i + 1, 2] * pitch)) * Math.Pow(0.95, depth) * scale + y));
                    }
                    else
                    {
                        SDL_RenderDrawLine(render,
                            (int)(model[i, 0] * Math.Pow(0.95, depth) + x),
                            (int)((model[i, 1] + (model[i, 2] * pitch)) * Math.Pow(0.95, depth) + y),
                            (int)(model[i + 1, 0] * Math.Pow(0.95, depth) + x),
                            (int)((model[i + 1, 1] + (model[i + 1, 2] * pitch)) * Math.Pow(0.95, depth) + y));
                    }
                }
                SDL_SetRenderDrawColor(render, 0, 0, 0, 255);
            }
            catch (Exception ex)
            {
                CrashReport(ex);
            }
        }
        public short[] Rotate(short x_offset, short y_offset)
        {
            short[] real_xy = new short[2];
            real_xy[0] = (short)(Math.Cos(-roll) * (-x_offset) - Math.Sin(-roll) * (-y_offset));
            real_xy[1] = (short)(Math.Sin(-roll) * (-x_offset) + Math.Cos(-roll) * (-y_offset));
            return real_xy;
        }
    }
}