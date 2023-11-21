using static SDL2.SDL;
using static Dysgenesis.Data;
using static Dysgenesis.Program;
using static Dysgenesis.Background;
namespace Dysgenesis
{
    public class Player
    {
        public float x = 960, y = 810, vx = 0, vy = 0, pitch = 0, roll = 0, shockwaves = 0, scale = 1;
        public sbyte[,] model = MODELE_P;
        public short HP = 150, death_timer = 0;
        public byte powerup = 0, fade = 255;
        public bool dead = false;
        public SDL_Rect HP_BAR = new SDL_Rect { x = 10, y = 15, w = 10, h = 20 };
        public SDL_Rect SHOCK_BAR = new SDL_Rect { x = 10, y = 40, w = 100, h = 20 };
        public void Exist()
        {
            if (HP > 0)
                Move();
            WallCheck();
            Proj_Collision();
            if (BombePulsar.HP_bombe > 0)
                Projectile.TirJoueur();
            if (!player.dead)
                player.shockwaves += 0.000556f; // ~1/1800, 1 par 30 secondes @ 60 fps
        }
        //public Player() // constructeur rendu inutile
        //{
            #region vieux modèle de joueur
            //player_model[0].x_offset = -50;  player_model[0].y_offset = 0;    player_model[0].depth = 0; // rouge
            //player_model[1].x_offset = -46;  player_model[1].y_offset = -2;   player_model[1].depth = -50; // orange
            //player_model[2].x_offset = -46;  player_model[2].y_offset = 0;    player_model[2].depth = -2; // vomi
            //player_model[3].x_offset = -46;  player_model[3].y_offset = -4;   player_model[3].depth = -2; // -vomi
            //player_model[4].x_offset = -46;  player_model[4].y_offset = 0;    player_model[4].depth = -2; // vomi
            //player_model[5].x_offset = -10;  player_model[5].y_offset = 5;    player_model[5].depth = -5; // vert pâle
            //player_model[6].x_offset = -10;  player_model[6].y_offset = -5;   player_model[6].depth = -5; // -vert pâle
            //player_model[7].x_offset = -10;  player_model[7].y_offset = 5;    player_model[7].depth = -5; // vert pâle
            //player_model[8].x_offset = -5;   player_model[8].y_offset = 5;    player_model[8].depth = -15; // rose
            //player_model[9].x_offset = -5;   player_model[9].y_offset = 0;    player_model[9].depth = -15; // -rose
            //player_model[10].x_offset = -5;  player_model[10].y_offset = 5;   player_model[10].depth = -15; // rose
            //player_model[11].x_offset = 5;   player_model[11].y_offset = 5;   player_model[11].depth = -15; // rose
            //player_model[12].x_offset = 5;   player_model[12].y_offset = 0;   player_model[12].depth = -15; // -rose
            //player_model[13].x_offset = 5;   player_model[13].y_offset = 5;   player_model[13].depth = -15; // rose
            //player_model[14].x_offset = 10;  player_model[14].y_offset = 5;   player_model[14].depth = -5; // vert pâle
            //player_model[15].x_offset = 46;  player_model[15].y_offset = 0;   player_model[15].depth = -2; // vomi
            //player_model[16].x_offset = 46;  player_model[16].y_offset = -2;  player_model[16].depth = -50; // orange
            //player_model[17].x_offset = 50;  player_model[17].y_offset = 0;   player_model[17].depth = -0; // rouge
            //player_model[18].x_offset = 50;  player_model[18].y_offset = -5;  player_model[18].depth = -0; // rouge 2
            //player_model[19].x_offset = 13;  player_model[19].y_offset = -10; player_model[19].depth = 10; // bleu
            //player_model[20].x_offset = 15;  player_model[20].y_offset = -9;  player_model[20].depth = 20; // mauve
            //player_model[21].x_offset = 4;   player_model[21].y_offset = 10;  player_model[21].depth = 10; // vert foncé
            //player_model[22].x_offset = 13;  player_model[22].y_offset = -10; player_model[22].depth = 10; // bleu
            //player_model[23].x_offset = 15;  player_model[23].y_offset = -9;  player_model[23].depth = 20; // mauve
            //player_model[24].x_offset = 0;   player_model[24].y_offset = -15; player_model[24].depth = 20; // bleh
            //player_model[25].x_offset = -15; player_model[25].y_offset = -9;  player_model[25].depth = 20; // mauve
            //player_model[26].x_offset = -4;  player_model[26].y_offset = 10;  player_model[26].depth = 10; // vert foncé
            //player_model[27].x_offset = -13; player_model[27].y_offset = -10; player_model[27].depth = 10; // bleu
            //player_model[28].x_offset = -15; player_model[28].y_offset = -9;  player_model[28].depth = 20; // mauve
            //player_model[29].x_offset = -13; player_model[29].y_offset = -10; player_model[29].depth = 10; // bleu
            //player_model[30].x_offset = -50; player_model[30].y_offset = -5;  player_model[30].depth = 0; // rouge 2
            //player_model[31].x_offset = -50; player_model[31].y_offset = 0;   player_model[31].depth = 0; // rouge
            //player_model[32].x_offset = -50; player_model[32].y_offset = -5;  player_model[32].depth = 0; // rouge 2
            //player_model[33].x_offset = -46; player_model[33].y_offset = -2;  player_model[33].depth = -50; // orange
            //player_model[34].x_offset = -46; player_model[34].y_offset = -4;  player_model[34].depth = -2; // -vomi
            //player_model[35].x_offset = -10; player_model[35].y_offset = -5;  player_model[35].depth = -5; // -vert pâle
            //player_model[36].x_offset = -5;  player_model[36].y_offset = 0;   player_model[36].depth = -15; // -rose
            //player_model[37].x_offset = 5;   player_model[37].y_offset = 0;   player_model[37].depth = -15; // -rose
            //player_model[38].x_offset = 10;  player_model[38].y_offset = -5;  player_model[38].depth = -5; // -vert pâle
            //player_model[39].x_offset = 46;  player_model[39].y_offset = -5;  player_model[39].depth = -2; // -vomi
            //player_model[40].x_offset = 46;  player_model[40].y_offset = -2;  player_model[40].depth = -50; // orange
            //player_model[41].x_offset = 50;  player_model[41].y_offset = -5;  player_model[41].depth = 0; // rouge 2
            //player_model[42].x_offset = 50;  player_model[42].y_offset = 0;   player_model[42].depth = 0; // rouge
            //player_model[43].x_offset = 10;  player_model[43].y_offset = 5;   player_model[43].depth = 10; // -bleu
            //player_model[44].x_offset = 4;   player_model[44].y_offset = 10;  player_model[44].depth = 10; // vert foncé
            //player_model[45].x_offset = 2;   player_model[45].y_offset = 10;  player_model[45].depth = 10;
            //player_model[46].x_offset = 1;   player_model[46].y_offset = 15;  player_model[46].depth = 10; // bleu pâle
            //player_model[47].x_offset = -1;  player_model[47].y_offset = 15;  player_model[47].depth = 10; // bleu pâle
            //player_model[48].x_offset = 1;   player_model[48].y_offset = 15;  player_model[48].depth = 10; // bleu pâle
            //player_model[49].x_offset = 0;   player_model[49].y_offset = 15;  player_model[49].depth = 25; // jaune
            //player_model[50].x_offset = 0;   player_model[50].y_offset = -15; player_model[50].depth = 20; // bleh
            //player_model[51].x_offset = 0;   player_model[51].y_offset = 15;  player_model[51].depth = 25; // jaune
            //player_model[52].x_offset = -1;  player_model[52].y_offset = 15;  player_model[52].depth = 10; // bleu pâle
            //player_model[53].x_offset = -2;  player_model[53].y_offset = 10;  player_model[53].depth = 10;
            //player_model[54].x_offset = -4;  player_model[54].y_offset = 10;  player_model[54].depth = 10; // vert foncé
            //player_model[55].x_offset = -10; player_model[55].y_offset = 5;   player_model[55].depth = 10; // -bleu
            //player_model[56].x_offset = -50; player_model[56].y_offset = 0;   player_model[56].depth = 0; // rouge
            #endregion
        //}
        public void Render()
        {
            try
            {
                SDL_SetRenderDrawColor(render, 255, 255, 255, fade);
                if (HP > 0)
                    for (int i = 0; i < player.model.GetLength(0) - 1; i++)
                    {
                        #region LOLOLOLOL plus besoin rip bozo
                        //short x_offset = (short)(scale * model[i, 0]);
                        //short next_x_offset = (short)(scale * model[i + 1, 0]);
                        //short y_offset = (short)(scale * model[i, 1]);
                        //short next_y_offset = (short)(scale * model[i + 1, 1]);
                        //Background.NouveauDrawLine(render, (int)(Rotate(x_offset, y_offset, x, y, roll)[0]),
                        //    (int)(Rotate(x_offset, y_offset, x, y, roll)[1] + model[i, 2] * (pitch + PERMA_PITCH)),
                        //    (int)(Rotate(next_x_offset, next_y_offset, x, y, roll)[0]),
                        //    (int)(Rotate(next_x_offset, next_y_offset, x, y, roll)[1] + model[i + 1, 2] * (pitch + PERMA_PITCH)));
                        #endregion
                        int[] pos = RenderCalc(i);
                        if (Program.gamemode >= 4 || Program.gamemode == 1)
                            Background.NouveauDrawLine(render, pos[0], pos[1], pos[2], pos[3]);
                        else
                            SDL_RenderDrawLine(render, pos[0], pos[1], pos[2], pos[3]);
                    }
                else
                {
                    if (!dead)
                    {
                        dead = true;
                        death_timer = 0;
                        Son.PlaySFX(Son.SFX_list.p_boom);
                        Son.StopMusic();
                        if (gamemode == 2)
                            nv_continue = level;
                    }
                    death_timer++;
                    if (death_timer < 256)
                        for (int i = 0; i < player.model.GetLength(0) - 1; i++)
                        {
                            if (i % 4 == 0)
                                SDL_SetRenderDrawColor(render, 255, 255, 255, (byte)(255 - death_timer));
                            else
                                SDL_SetRenderDrawColor(render, 0, 0, 0, 255);
                            scale = death_timer / 60f + 1;
                            x -= 1 / 50f;
                            y += 1 / 50f;
                            roll += 1 / 4096f;
                            int[] pos = RenderCalc(i);
                            if (Program.gamemode >= 4 || Program.gamemode == 1)
                                Background.NouveauDrawLine(render, pos[0], pos[1], pos[2], pos[3]);
                            else
                                SDL_RenderDrawLine(render, pos[0], pos[1], pos[2], pos[3]);
                        }
                    if (death_timer > 120)
                    {
                        Text.DisplayText("game over", short.MinValue, short.MinValue, 5, alpha: (short)(death_timer - 120));
                        if (gamemode == 3)
                            Text.DisplayText("score: " + level, short.MinValue, (short)(W_SEMI_HAUTEUR + 30), 2, alpha: (short)(death_timer - 120));
                    }
                    if (death_timer > 500)
                    {
                        dead = false;
                        death_timer = 0;
                        gamemode = 0;
                        x = -1;
                        y = -1;
                        if (gamemode == 2)
                            SaveLoad.Save();
                        else
                            Level_Data.arcade_ens = new byte[0];
                        for (int i = 0; i < enemies.Length; i++)
                        {
                            enemies[i] = null;
                        }
                        for (int i = 0; i < Projectile.pos.GetLength(0); i++)
                        {
                            Projectile.pos[i, 0] = -1;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                CrashReport(ex);
            }
        }
        void Proj_Collision()
        {
            for (int i = 0; i < Projectile.pos.GetLength(0); i++)
            {
                if (Projectile.pos[i, 0] != -1 && !player.dead)
                {
                    int[] depths = Projectile.CalcDepths(i);
                    if (Distance(depths[0], depths[1], x, y) < 30 && Projectile.pos[i, 2] == 0)
                    {
                        HP--;
                        Explosion.Call((short)Projectile.pos[i, 3], (short)Projectile.pos[i, 4], 0);
                        Son.PlaySFX(Son.SFX_list.e_boom);
                    }
                }
            }
        }
        void Move()
        {
            //if (Keys.l_click)
            //    HP = 0; // debug
            if (Keys.w)
            {
                player.vy -= P_SPEED;
                player.pitch += 0.05f;
            }
            if (Keys.a)
            {
                player.vx -= P_SPEED;
                player.roll -= 0.05f;
            }
            if (Keys.s)
            {
                player.vy += P_SPEED;
                player.pitch -= 0.05f;
            }
            if (Keys.d)
            {
                player.vx += P_SPEED;
                player.roll += 0.05f;
            }
            player.vx *= P_FRICTION;
            player.vy *= P_FRICTION;
            player.pitch *= P_PITCH_FRICTION;
            player.roll *= P_ROLL_FRICTION;
            if (player.roll > P_MAX_ROLL)
                player.roll = P_MAX_ROLL;
            if (player.roll < -P_MAX_ROLL)
                player.roll = -P_MAX_ROLL;
            if (player.pitch > P_MAX_PITCH)
                player.pitch = P_MAX_PITCH;
            if (player.pitch < -P_MAX_PITCH)
                player.pitch = -P_MAX_PITCH;
            player.x += player.vx;
            player.y += player.vy;
        }
        // ne faisait pas assez, 28 références vers 6, gros changement.
        //public short[] Rotate(short x_offset, short y_offset, float pos_x, float pos_y, float roll)
        //{
        //    short[] real_xy = new short[2];
        //    real_xy[0] = (short)(Math.Cos(roll) * (-x_offset) - Math.Sin(roll) * (-y_offset) + pos_x);
        //    real_xy[1] = (short)(Math.Sin(roll) * (-x_offset) + Math.Cos(roll) * (-y_offset) + pos_y);
        //    return real_xy;
        //}
        public int[] RenderCalc(int index)
        {
            double sinroll = Math.Sin(roll);
            double cosroll = Math.Cos(roll);
            float pitchconst = pitch + P_PERMA_PITCH;
            return new int[4] {
                (int)(scale * (cosroll * -model[index, 0] - sinroll * -model[index, 1]) + x),
                (int)(scale * (sinroll * -model[index, 0] + cosroll * -model[index, 1]) + y + model[index, 2] * pitchconst),
                (int)(scale * (cosroll * -model[index + 1, 0] - sinroll * -model[index + 1, 1]) + x),
                (int)(scale * (sinroll * -model[index + 1, 0] + cosroll * -model[index + 1, 1]) + y + model[index + 1, 2] * pitchconst)
            };
        }
        void WallCheck()
        {
            if (player.dead)
                return;
            byte semi_x = P_WIDTH / 2;
            byte semi_y = P_HEIGHT / 2;
            if (player.x - semi_x < 0)
                player.x = semi_x;
            if (player.x + semi_x > W_LARGEUR)
                player.x = W_LARGEUR - semi_x;
            if (player.y - semi_y < 0)
                player.y = semi_y;
            if (player.y + semi_y > W_HAUTEUR)
                player.y = W_HAUTEUR - semi_y;
        }
    }
}