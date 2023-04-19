using static Dysgenesis.Program;
using static Dysgenesis.Background;
using static SDL2.SDL;
using static Dysgenesis.Data;
using System.Linq;

namespace Dysgenesis
{
    public class Item
    {
        public float x, y;
        public byte depth, type = 0;
        sbyte[,] model = new sbyte[0,0];
        byte[] skipped_line_indexes = new byte[0];
        uint localTimer = 0, color = 0xFFFFFFFF;
        public byte TypeRoll()
        {
            if (free_items)
                return (byte)RNG.Next(1, 8);
            float roll = RNG.Next(1, 100);
            roll -= (gamemode == 2 ? 20 : 40) * (float)Math.Pow(0.8, level);
            if (roll < 60)
                return 0;
            if (roll < 70)
                return 1;
            if (roll < 80)
                return 2;
            if (roll < 85)
                return 3;
            if (roll < 90)
                return 6;
            if (roll < 94)
                return 5;
            if (roll < 97)
                return 7;
            return 4;
        }
        public void Spawn(ref Item self, float new_x, float new_y, byte new_depth)
        {
            x = new_x;
            y = new_y;
            depth = new_depth;
            type = TypeRoll();
            if (type == 0)
            {
                self = null;
                return;
            }
            switch (type)
            {
                case 1: // HP
                    color = 0xFF0000FF;
                    model = MODELE_I1;
                    break;
                case 2: // shock
                    color = 0x00FFFFFF;
                    break;
                case 3: // x2
                    color = 0xFF7F00FF;
                    skipped_line_indexes = new byte[4] { 2, 17, 19, 21 };
                    model = MODELE_I3;
                    break;
                case 4: // x3
                    color = 0xFFFF00FF;
                    skipped_line_indexes = new byte[4] { 2, 17, 19, 21 };
                    model = MODELE_I4;
                    break;
                case 5: // homing
                    color = 0x7FFF00FF;
                    skipped_line_indexes = new byte[4] { 18, 20, 22, 24 };
                    model = MODELE_I5;
                    break;
                case 6: // spread
                    color = 0x0000FFFF;
                    skipped_line_indexes = new byte[4] { 18, 21, 23, 26};
                    model = MODELE_I6;
                    break;
                case 7: // laser
                    color = 0x7F00FFFF;
                    skipped_line_indexes = new byte[1] { 18 };
                    model = MODELE_I7;
                    break;
            }
        }
        public void Exist(ref Item self)
        {
            Move(ref self);
            if (self != null)
                CheckPlayerCollision(ref self);
        }
        void Move(ref Item self)
        {
            localTimer++;
            if (depth > 0 && localTimer % 10 == 0)
            {
                depth--;
                if (depth == 0) localTimer = 0;
            }
            else
            {
                if (localTimer > 120 && depth == 0)
                {
                    self = null;
                    return;
                }
            }
        }
        void CheckPlayerCollision(ref Item self)
        {
            if (Distance(player.x, player.y, x, y) < 50 && depth == 0)
            {
                if (type == 1)
                    player.HP += 10;
                else if (type == 2)
                    player.shockwaves++;
                else player.powerup = type;
                self = null;
                Son.PlaySFX(Son.SFX_list.powerup);
            }
        }
        public void Render()
        {
            SDL_SetRenderDrawColor(render, (byte)((color & 0xFF000000) >> 24), (byte)((color & 0x00FF0000) >> 16), (byte)((color & 0x0000FF00) >> 8), (byte)(color & 0x000000FF));
            if (type != 2)
            {
                byte SLI = 0;
                for (int i = 0; i < model.GetLength(0) - 1; i++)
                {
                    if (skipped_line_indexes.Length > SLI)
                        if (i == skipped_line_indexes[SLI])
                        {
                            SLI++;
                            continue;
                        }
                    SDL_RenderDrawLine(render,
                            (int)(model[i, 0] * Math.Pow(0.95, depth) + x),
                            (int)(model[i, 1] * Math.Pow(0.95, depth) + y),
                            (int)(model[i + 1, 0] * Math.Pow(0.95, depth) + x),
                            (int)(model[i + 1, 1] * Math.Pow(0.95, depth) + y));
                }
            }
            else
            {
                DessinerCercle((short)x, (short)y, (byte)(30 * Math.Pow(0.95, depth)), 50);
                DessinerCercle((short)x, (short)y, (byte)(26 * Math.Pow(0.95, depth)), 50);
                DessinerCercle((short)x, (short)y, (byte)(22 * Math.Pow(0.95, depth)), 50);
            }
            if (type == 5)
                SDL_RenderDrawPoint(render, (int)x, (int)y);
        }
    }
}