using static SDL2.SDL;
using static Dysgenesis.Program;
using static Dysgenesis.Data;
using static System.Math;
using static Dysgenesis.Background.Son;
using NAudio.Wave;

namespace Dysgenesis
{
    public static class Background
    {
        public static void DessinerCercle(int x, int y, byte size, byte sides)
        {
            for (int i = 0; i < sides; i++)
            {
                if (Program.gamemode >= 4 || Program.gamemode == 1)
                    Background.NouveauDrawLine(render,
                        (int)(x + size * Sin(i * PI / (sides / 2))),
                        (int)(y + size * Cos(i * PI / (sides / 2))),
                        (int)(x + size * Sin((i + 1) * PI / (sides / 2))),
                        (int)(y + size * Cos((i + 1) * PI / (sides / 2))));
                else
                    SDL_RenderDrawLine(render,
                        (int)(x + size * Sin(i * PI / (sides / 2))),
                        (int)(y + size * Cos(i * PI / (sides / 2))),
                        (int)(x + size * Sin((i + 1) * PI / (sides / 2))),
                        (int)(y + size * Cos((i + 1) * PI / (sides / 2))));
            }
        }
        public static short Distance(float x1, float y1, float x2, float y2, float mult_x = 1, float mult_y = 1)
        { // c = +sqrt(w(a²)+h(b²))
            return (short)Sqrt(mult_x * Pow(Abs(x1 - x2), 2) + mult_y * Pow(Abs(y1 - y2), 2));
        }
        public static class BombePulsar
        {
            public static short HP_bombe = BP_MAX_HP;
            public static ushort lTimer = 0;
            public static void DessinerBombePulsar(short x, short y, byte size)
            {
                SDL_SetRenderDrawColor(render, 200, 255, 255, 255);
                DessinerCercle(x, y, size, 50);
                for (int i = 0; i < 50; i++)
                {
                    float ang = RNG.NextSingle() * (float)PI;
                    if (Program.gamemode >= 4 || Program.gamemode == 1)
                        Background.NouveauDrawLine(render, (int)(RNG.Next(-size, size) * Cos(ang) + x), (int)(RNG.Next(-size, size) * Sin(ang) + y), x, y);
                    else
                        SDL_RenderDrawLine(render, (int)(RNG.Next(-size, size) * Cos(ang) + x), (int)(RNG.Next(-size, size) * Sin(ang) + y), x, y);
                }
            }
            public static void Niveau20()
            {
                if (gamemode == 2 && level == 20 && enemies[0] != null)
                {
                    if (enemies[0].depth == 20)
                    {
                        SDL_SetRenderDrawColor(render, 0, 0, 255, 255);
                        SDL_RenderDrawLine(render, 953, 251, 952, 239);
                        SDL_RenderDrawLine(render, 952, 239, 948, 227);
                        SDL_RenderDrawLine(render, 948, 227, 942, 223);
                        SDL_RenderDrawLine(render, 964, 250, 965, 239);
                        SDL_RenderDrawLine(render, 965, 239, 967, 225);
                        SDL_RenderDrawLine(render, 967, 225, 972, 221);
                        SDL_RenderDrawLine(render, 957, 247, 957, 234);
                        SDL_RenderDrawLine(render, 960, 235, 964, 225);
                        SDL_RenderDrawLine(render, 955, 225, 952, 221);
                        SDL_RenderDrawLine(render, 954, 289, 951, 300);
                        SDL_RenderDrawLine(render, 951, 300, 947, 309);
                        SDL_RenderDrawLine(render, 947, 309, 940, 314);
                        SDL_RenderDrawLine(render, 965, 289, 966, 300);
                        SDL_RenderDrawLine(render, 966, 300, 970, 310);
                        SDL_RenderDrawLine(render, 970, 310, 980, 316);
                        SDL_RenderDrawLine(render, 960, 294, 961, 306);
                        SDL_RenderDrawLine(render, 958, 301, 957, 309);
                        SDL_RenderDrawLine(render, 952, 305, 955, 296);
                    }// -960, -270
                    DessinerBombePulsar(W_SEMI_LARGEUR, (short)(W_SEMI_HAUTEUR / 2), (byte)(25 - enemies[0].depth / 4));
                    VerifCollision();
                }
            }
            static void VerifCollision()
            {
                if (enemies[0].special != 99)
                    return;
                if (HP_bombe == 0)
                {
                    lTimer++;
                    if (lTimer == 2)
                    {
                        StopMusic();
                        PlaySFX(SFX_list.p_boom);
                    }
                    if (lTimer < 64)
                    {
                        Cutscene.rect.x = 0;
                        Cutscene.rect.y = 0;
                        Cutscene.rect.w = W_LARGEUR;
                        Cutscene.rect.h = W_HAUTEUR;
                        SDL_SetRenderDrawColor(render, 255, 255, 255, (byte)(lTimer * 4));
                        SDL_RenderFillRect(render, ref Cutscene.rect);
                    }
                    if (lTimer >= 64 && lTimer <= 200)
                    {
                        Cutscene.rect.x = 0;
                        Cutscene.rect.y = 0;
                        Cutscene.rect.w = W_LARGEUR;
                        Cutscene.rect.h = W_HAUTEUR;
                        SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
                        SDL_RenderFillRect(render, ref Cutscene.rect);
                    }
                    if (lTimer >= 200)
                    {
                        lTimer = 0;
                        gamemode = 5;
                    }
                    return;
                }

                for (int i = 0; i < Projectile.pos.GetLength(0); i++)
                {
                    if (Projectile.pos[i, 0] == -1 || Projectile.pos[i, 2] != G_DEPTH_LAYERS - 1)
                        continue;
                    int[] positions = Projectile.CalcDepths(i);
                    if (Distance(positions[2], positions[3], W_SEMI_LARGEUR, W_SEMI_HAUTEUR / 2) < 20)
                    {
                        HP_bombe--;
                        Explosion.Call((short)positions[2], (short)positions[3], G_DEPTH_LAYERS / 4);
                        SDL_SetRenderDrawColor(render, 255, 0, 0, 255);
                        DessinerCercle(W_SEMI_LARGEUR, (short)(W_SEMI_HAUTEUR / 2), (byte)(25 - enemies[0].depth / 4), 50);
                        for (int j = 0; j < 50; j++)
                        {
                            float ang = RNG.NextSingle() * (float)PI;
                            SDL_RenderDrawLine(render, (int)(RNG.Next(-(byte)(25 - enemies[0].depth / 4), (byte)(25 - enemies[0].depth / 4)) * Cos(ang) + W_SEMI_LARGEUR),
                                (int)(RNG.Next(-(byte)(25 - enemies[0].depth / 4), (byte)(25 - enemies[0].depth / 4)) * Sin(ang) + W_SEMI_HAUTEUR / 2), W_SEMI_LARGEUR, W_SEMI_HAUTEUR / 2);
                        }
                        if (HP_bombe == 0)
                            return;
                    }
                }
            }
        }
        public static class Etoiles
        {
            public static float[,] star_positions = new float[S_DENSITY, 2];
            public static void Init()
            {
                for (int i = 0; i < S_DENSITY; i++)
                {
                    star_positions[i, 0] = RNG.Next(0, W_LARGEUR);
                    star_positions[i, 1] = RNG.Next(0, W_HAUTEUR);
                }
            }
            public static void Exist()
            {
                if (player.death_timer == 0)
                    Move();
                Spawn();
            }
            public static void Move()
            {
                for (int i = 0; i < S_DENSITY; i++)
                {
                    star_positions[i, 0] = (star_positions[i, 0] - W_LARGEUR / 2) * S_SPEED + W_LARGEUR / 2;
                    star_positions[i, 1] = (star_positions[i, 1] - W_HAUTEUR / 2) * S_SPEED + W_HAUTEUR / 2;
                }
            }
            public static void Spawn()
            {
                for (int i = 0; i < S_DENSITY; i++)
                {
                    if (star_positions[i,0] >= W_LARGEUR || star_positions[i,0] <= 0 || star_positions[i,1] >= W_HAUTEUR || star_positions[i,1] <= 0)
                    {
                        try_again:
                        star_positions[i, 0] = RNG.Next(W_SEMI_LARGEUR - S_SPAWN_RADIUS, W_SEMI_LARGEUR + S_SPAWN_RADIUS);
                        star_positions[i, 1] = RNG.Next(W_SEMI_HAUTEUR - S_SPAWN_RADIUS, W_SEMI_HAUTEUR + S_SPAWN_RADIUS);
                        if (star_positions[i, 0] == W_SEMI_LARGEUR && star_positions[i, 1] == W_SEMI_HAUTEUR)
                            goto try_again;
                    }
                }
            }
        }
        public static class Text
        {
            // documentation texte
            // 
            //   text: le texte qui sera affiché à l'écran
            //         charactères supportés: a-z, 0-9, +, -, é, è, ê, à,  , ., ,, ', :, \, /, ", (, ), \n
            //         lettres sont majuscule seuelement, mais le texte qui rentre dans la fonction doit être minuscule, les majuscules seront automatiquement
            //         convertis en minuscules avant d'êtres déssinés.
            //         \n fonctionne et est la seule facon de passer à une prochaine ligne dans le même appel de texte, et quand la ligne est sauté, il revient
            //         au x de départ.
            //   x, y: position haut gauche du premier charactère affiché.
            //         mettre short.MinValue (ou -32768) va centrer le texte au millieu de l'écran.
            //   size: nombre entier qui donne le multiple de la largeure et hauteure.
            //         la largeur d'un charactère sera de 5 * size, et la hauteur de 10 * size.
            //  color: couleure RGB du texte, où blanc est la valeure par défaut
            //         R, G et B attachés ensemble en un chiffre, où les bits 23 à 16 sont pour le rouge, 15 à 8 pour le vert et 7 à 0 pour le bleu.
            //  alpha: transparence du texte, 100% opaque par défaut. Ceci sera la valeure a du RGBA de sdl, et va automatiquement
            //         arrondir à une valeure de byte si dépassement.
            // scroll: le nb. de charactères que seront affichés à l'écran, peu importe la longeure du texte.
            //         int.MaxValue par défaut, ce qui le met automatiquement à la longeure du texte, car si la valeure entrée est plus grande que la longeure du texte,
            //         scroll sera ajusté pour ne pas dépasser la longeure du texte. si scroll est négatif, aucun texte n,est affiché.
            //
            // toutes ces valeures peuvent êtres complètements différentes d'une image à l'autre, mais marchent mieux avec un changement lent.
            public static short extra_y = 0;
            public static int ret_length = 0, text_tick = 0;
            private static void DisplayChar(char charac, short x, short y, byte size, short i, byte r, byte g, byte b, byte a)
            {
                SDL_SetRenderDrawColor(render, r, g, b, a);
                y += extra_y;
                x -= (short)ret_length;
                if (x + size * 5 > 1920)
                {
                    extra_y += (short)(13 * size);
                    ret_length = (int)((i + 1) * 8 * size);
                }
                switch (charac)
                {
                    case 'a':
                        Background.NouveauDrawLine(render, x, y + 10 * size, x, y);
                        Background.NouveauDrawLine(render, x, y, x + 5 * size, y);
                        Background.NouveauDrawLine(render, x + 5 * size, y, x + 5 * size, y + 10 * size);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 5 * size, x, y + 5 * size);
                        break;
                    case 'b':
                        Background.NouveauDrawLine(render, x, y + 10 * size, x, y);
                        Background.NouveauDrawLine(render, x, y, x + 5 * size, y);
                        Background.NouveauDrawLine(render, x + 5 * size, y, x + 5 * size, y + 3 * size);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 3 * size, x, y + 5 * size);
                        Background.NouveauDrawLine(render, x, y + 5 * size, x + 5 * size, y + 7 * size);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 7 * size, x + 5 * size, y + 10 * size);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 10 * size, x, y + 10 * size);
                        break;
                    case 'c':
                        Background.NouveauDrawLine(render, x, y + 10 * size, x, y);
                        Background.NouveauDrawLine(render, x, y, x + 5 * size, y);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 10 * size, x, y + 10 * size);
                        break;
                    case 'd':
                        Background.NouveauDrawLine(render, x, y + 10 * size, x, y);
                        Background.NouveauDrawLine(render, x, y, x + 2 * size, y);
                        Background.NouveauDrawLine(render, x + 2 * size, y + 10 * size, x, y + 10 * size);
                        Background.NouveauDrawLine(render, x + 2 * size, y + 10 * size, x + 5 * size, y + 5 * size);
                        Background.NouveauDrawLine(render, x + 2 * size, y, x + 5 * size, y + 5 * size);
                        break;
                    case 'e':
                        Background.NouveauDrawLine(render, x, y + 10 * size, x, y);
                        Background.NouveauDrawLine(render, x, y, x + 5 * size, y);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 10 * size, x, y + 10 * size);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 5 * size, x, y + 5 * size);
                        break;
                    case 'f':
                        Background.NouveauDrawLine(render, x, y + 10 * size, x, y);
                        Background.NouveauDrawLine(render, x, y, x + 5 * size, y);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 5 * size, x, y + 5 * size);
                        break;
                    case 'g':
                        Background.NouveauDrawLine(render, x, y + 10 * size, x, y);
                        Background.NouveauDrawLine(render, x, y, x + 5 * size, y);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 10 * size, x, y + 10 * size);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 10 * size, x + 5 * size, y + 5 * size);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 5 * size, x + 3 * size, y + 5 * size);
                        break;
                    case 'h':
                        Background.NouveauDrawLine(render, x, y + 10 * size, x, y);
                        Background.NouveauDrawLine(render, x + 5 * size, y, x + 5 * size, y + 10 * size);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 5 * size, x, y + 5 * size);
                        break;
                    case 'i':
                        Background.NouveauDrawLine(render, x, y, x + 5 * size, y);
                        Background.NouveauDrawLine(render, (int)(x + 2.5f * size), y, (int)(x + 2.5f * size), y + 10 * size);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 10 * size, x, y + 10 * size);
                        break;
                    case 'j':
                        Background.NouveauDrawLine(render, x + 5 * size, y, x + 5 * size, y + 10 * size);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 10 * size, x, y + 10 * size);
                        Background.NouveauDrawLine(render, x, y + 10 * size, x, y + 7 * size);
                        break;
                    case 'k':
                        Background.NouveauDrawLine(render, x, y + 10 * size, x, y);
                        Background.NouveauDrawLine(render, x, y + 5 * size, x + 5 * size, y);
                        Background.NouveauDrawLine(render, x, y + 5 * size, x + 5 * size, y + 10 * size);
                        break;
                    case 'l':
                        Background.NouveauDrawLine(render, x, y + 10 * size, x, y);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 10 * size, x, y + 10 * size);
                        break;
                    case 'm':
                        Background.NouveauDrawLine(render, x, y + 10 * size, x, y);
                        Background.NouveauDrawLine(render, x + 5 * size, y, x + 5 * size, y + 10 * size);
                        Background.NouveauDrawLine(render, x, y, (int)(x + 2.5f * size), y + 5 * size);
                        Background.NouveauDrawLine(render, x + 5 * size, y, (int)(x + 2.5f * size), y + 5 * size);
                        break;
                    case 'n':
                        Background.NouveauDrawLine(render, x, y + 10 * size, x, y);
                        Background.NouveauDrawLine(render, x + 5 * size, y, x + 5 * size, y + 10 * size);
                        Background.NouveauDrawLine(render, x, y, x + 5 * size, y + 10 * size);
                        break;
                    case 'o':
                        Background.NouveauDrawLine(render, x, y + 10 * size, x, y);
                        Background.NouveauDrawLine(render, x + 5 * size, y, x + 5 * size, y + 10 * size);
                        Background.NouveauDrawLine(render, x, y, x + 5 * size, y);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 10 * size, x, y + 10 * size);
                        break;
                    case 'p':
                        Background.NouveauDrawLine(render, x, y + 10 * size, x, y);
                        Background.NouveauDrawLine(render, x, y, x + 5 * size, y);
                        Background.NouveauDrawLine(render, x + 5 * size, y, x + 5 * size, y + 5 * size);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 5 * size, x, y + 5 * size);
                        break;
                    case 'q':
                        Background.NouveauDrawLine(render, x, y + 10 * size, x, y);
                        Background.NouveauDrawLine(render, x + 4 * size, y, x + 4 * size, y + 10 * size);
                        Background.NouveauDrawLine(render, x, y, x + 4 * size, y);
                        Background.NouveauDrawLine(render, x + 4 * size, y + 10 * size, x, y + 10 * size);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 10 * size, x + 3 * size, y + 5 * size);
                        break;
                    case 'r':
                        Background.NouveauDrawLine(render, x, y + 10 * size, x, y);
                        Background.NouveauDrawLine(render, x, y, x + 5 * size, y);
                        Background.NouveauDrawLine(render, x + 5 * size, y, x + 5 * size, y + 5 * size);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 5 * size, x, y + 5 * size);
                        Background.NouveauDrawLine(render, x + 2 * size, y + 5 * size, x + 5 * size, y + 10 * size);
                        break;
                    case 's':
                        Background.NouveauDrawLine(render, x, y, x + 5 * size, y);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 10 * size, x, y + 10 * size);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 5 * size, x, y + 5 * size);
                        Background.NouveauDrawLine(render, x, y, x, y + 5 * size);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 10 * size, x + 5 * size, y + 5 * size);
                        break;
                    case 't':
                        Background.NouveauDrawLine(render, (int)(x + 2.5f * size), y, (int)(x + 2.5f * size), y + 10 * size);
                        Background.NouveauDrawLine(render, x, y, x + 5 * size, y);
                        break;
                    case 'u':
                        Background.NouveauDrawLine(render, x, y + 10 * size, x, y);
                        Background.NouveauDrawLine(render, x + 5 * size, y, x + 5 * size, y + 10 * size);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 10 * size, x, y + 10 * size);
                        break;
                    case 'v':
                        Background.NouveauDrawLine(render, x, y, (int)(x + 2.5f * size), y + 10 * size);
                        Background.NouveauDrawLine(render, x + 5 * size, y, (int)(x + 2.5f * size), y + 10 * size);
                        break;
                    case 'w':
                        Background.NouveauDrawLine(render, x, y + 10 * size, x, y);
                        Background.NouveauDrawLine(render, x + 5 * size, y, x + 5 * size, y + 10 * size);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 10 * size, x, y + 10 * size);
                        Background.NouveauDrawLine(render, (int)(x + 2.5f * size), y + 10 * size, (int)(x + 2.5f * size), y + 4 * size);
                        break;
                    case 'x':
                        Background.NouveauDrawLine(render, x, y, x + 5 * size, y + 10 * size);
                        Background.NouveauDrawLine(render, x + 5 * size, y, x, y + 10 * size);
                        break;
                    case 'y':
                        Background.NouveauDrawLine(render, x + 5 * size, y, x, y + 10 * size);
                        Background.NouveauDrawLine(render, x, y, (int)(x + 2.5f * size), y + 5 * size);
                        break;
                    case 'z':
                        Background.NouveauDrawLine(render, x, y, x + 5 * size, y);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 10 * size, x, y + 10 * size);
                        Background.NouveauDrawLine(render, x + 5 * size, y, x, y + 10 * size);
                        break;
                    case '0':
                        Background.NouveauDrawLine(render, x + 5 * size, y, x, y + 10 * size);
                        Background.NouveauDrawLine(render, x, y + 10 * size, x, y);
                        Background.NouveauDrawLine(render, x + 5 * size, y, x + 5 * size, y + 10 * size);
                        Background.NouveauDrawLine(render, x, y, x + 5 * size, y);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 10 * size, x, y + 10 * size);
                        break;
                    case '1':
                        Background.NouveauDrawLine(render, x, y + 3 * size, (int)(x + 2.5f * size), y);
                        Background.NouveauDrawLine(render, (int)(x + 2.5f * size), y, (int)(x + 2.5f * size), y + 10 * size);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 10 * size, x, y + 10 * size);
                        break;
                    case '2':
                        Background.NouveauDrawLine(render, x, y + 3 * size, x + 1 * size, y);
                        Background.NouveauDrawLine(render, x + 1 * size, y, x + 4 * size, y);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 3 * size, x + 4 * size, y);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 3 * size, x, y + 10 * size);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 10 * size, x, y + 10 * size);
                        break;
                    case '3':
                        Background.NouveauDrawLine(render, x + 5 * size, y + 10 * size, x + 5 * size, y);
                        Background.NouveauDrawLine(render, x, y, x + 5 * size, y);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 10 * size, x, y + 10 * size);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 5 * size, x, y + 5 * size);
                        break;
                    case '4':
                        Background.NouveauDrawLine(render, x, y + 5 * size, x, y);
                        Background.NouveauDrawLine(render, x + 5 * size, y, x + 5 * size, y + 10 * size);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 5 * size, x, y + 5 * size);
                        break;
                    case '5':
                        Background.NouveauDrawLine(render, x + 5 * size, y, x, y);
                        Background.NouveauDrawLine(render, x, y + 5 * size, x, y);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 5 * size, x, y + 5 * size);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 5 * size, x + 5 * size, y + 8 * size);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 8 * size, x + 4 * size, y + 10 * size);
                        Background.NouveauDrawLine(render, x + 4 * size, y + 10 * size, x + 1 * size, y + 10 * size);
                        Background.NouveauDrawLine(render, x + 1 * size, y + 10 * size, x, y + 8 * size);
                        break;
                    case '6':
                        Background.NouveauDrawLine(render, x, y + 10 * size, x, y);
                        Background.NouveauDrawLine(render, x, y, x + 5 * size, y);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 10 * size, x, y + 10 * size);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 10 * size, x + 5 * size, y + 5 * size);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 5 * size, x, y + 5 * size);
                        break;
                    case '7':
                        Background.NouveauDrawLine(render, x + 5 * size, y, x, y);
                        Background.NouveauDrawLine(render, x + 5 * size, y, x, y + 10 * size);
                        break;
                    case '8':
                        Background.NouveauDrawLine(render, x + 5 * size, y + 5 * size, x, y + 5 * size);
                        Background.NouveauDrawLine(render, x, y + 10 * size, x, y);
                        Background.NouveauDrawLine(render, x + 5 * size, y, x + 5 * size, y + 10 * size);
                        Background.NouveauDrawLine(render, x, y, x + 5 * size, y);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 10 * size, x, y + 10 * size);
                        break;
                    case '9':
                        Background.NouveauDrawLine(render, x + 5 * size, y + 10 * size, x + 5 * size, y);
                        Background.NouveauDrawLine(render, x, y, x + 5 * size, y);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 10 * size, x, y + 10 * size);
                        Background.NouveauDrawLine(render, x, y, x, y + 5 * size);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 5 * size, x, y + 5 * size);
                        break;
                    case ' ':
                        break;
                    case '.':
                        Background.NouveauDrawDot(render, x, y + 10 * size);
                        break;
                    case ':':
                        Background.NouveauDrawDot(render, x, y + 3 * size);
                        Background.NouveauDrawDot(render, x, y + 7 * size);
                        break;
                    case '\n':
                        extra_y += (short)(13 * size);
                        ret_length = (int)((i + 1) * 8 * size);
                        break;
                    case ',':
                        Background.NouveauDrawLine(render, x + 2 * size, y + 8 * size, x, y + 10 * size);
                        break;
                    case '\'':
                        Background.NouveauDrawLine(render, x + 2 * size, y + 2 * size, x, y);
                        break;
                    case 'é':
                        Background.NouveauDrawLine(render, x, y + 10 * size, x, y);
                        Background.NouveauDrawLine(render, x, y, x + 5 * size, y);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 10 * size, x, y + 10 * size);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 5 * size, x, y + 5 * size);
                        Background.NouveauDrawLine(render, x + 1 * size, y - 2, x + 4 * size, y - 4);
                        break;
                    case 'è':
                        Background.NouveauDrawLine(render, x, y + 10 * size, x, y);
                        Background.NouveauDrawLine(render, x, y, x + 5 * size, y);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 10 * size, x, y + 10 * size);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 5 * size, x, y + 5 * size);
                        Background.NouveauDrawLine(render, x + 1 * size, y - 4, x + 4 * size, y - 2);
                        break;
                    case 'à':
                        Background.NouveauDrawLine(render, x + 1 * size, y - 2, x + 4 * size, y);
                        Background.NouveauDrawLine(render, x, y + 10 * size, x, y);
                        Background.NouveauDrawLine(render, x, y, x + 5 * size, y);
                        Background.NouveauDrawLine(render, x + 5 * size, y, x + 5 * size, y + 10 * size);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 5 * size, x, y + 5 * size);
                        break;
                    case '"':
                        Background.NouveauDrawLine(render, x + 2 * size, y, x + 2 * size, y + 3 * size);
                        Background.NouveauDrawLine(render, x + 4 * size, y, x + 4 * size, y + 3 * size);
                        break;
                    case '-':
                        Background.NouveauDrawLine(render, x + 1 * size, y + 5 * size, x + 4 * size, y + 5 * size);
                        break;
                    case 'ê':
                        Background.NouveauDrawLine(render, x, y + 10 * size, x, y);
                        Background.NouveauDrawLine(render, x, y, x + 5 * size, y);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 10 * size, x, y + 10 * size);
                        Background.NouveauDrawLine(render, x + 5 * size, y + 5 * size, x, y + 5 * size);
                        Background.NouveauDrawLine(render, x + 1 * size, y - 2, (int)(x + 2.5f * size), y - 4);
                        Background.NouveauDrawLine(render, x + 4 * size, y - 2, (int)(x + 2.5f * size), y - 4);
                        break;
                    case '/':
                        Background.NouveauDrawLine(render, x, y + 10 * size, x + 5 * size, y);
                        break;
                    case '\\':
                        Background.NouveauDrawLine(render, x + 5 * size, y + 10 * size, x, y);
                        break;
                    case '(':
                        Background.NouveauDrawLine(render, x + 4 * size, y, x + 2 * size, y + 3 * size);
                        Background.NouveauDrawLine(render, x + 2 * size, y + 3 * size, x + 2 * size, y + 7 * size);
                        Background.NouveauDrawLine(render, x + 4 * size, y + 10 * size, x + 2 * size, y + 7 * size);
                        break;
                    case ')':
                        Background.NouveauDrawLine(render, x + 1 * size, y, x + 3 * size, y + 3 * size);
                        Background.NouveauDrawLine(render, x + 3 * size, y + 3 * size, x + 3 * size, y + 7 * size);
                        Background.NouveauDrawLine(render, x + 1 * size, y + 10 * size, x + 3 * size, y + 7 * size);
                        break;
                    case '+':
                        Background.NouveauDrawLine(render, x + 0 * size, y + 5 * size, x + 4 * size, y + 5 * size);
                        Background.NouveauDrawLine(render, x + (int)(2.5f * size), y + 3 * size, x + (int)(2.5f * size), y + 7 * size);
                        break;
                        //case '':
                        //    break;
                }
            }
            public static void DisplayText(string text, short x, short y, byte size, int color = 0xFFFFFF, short alpha = 255, int scroll = int.MaxValue)
            {
                extra_y = 0;
                ret_length = 0;

                if (scroll > text.Length)
                    scroll = text.Length;
                if (scroll < -1)
                    return;

                if (alpha < 0)
                    alpha = 0;
                if (alpha > 255)
                    alpha = 255;

                if (x == short.MinValue)
                    x = (short)(960 - (8 * size * text.Length - 1) / 2);
                if (y == short.MinValue)
                    y = (short)(540 - (10 * size) / 2);

                text = text.ToLower();
                for (short i = 0; i < scroll; i++)
                {
                    DisplayChar(text[i], (short)(x + i * 8 * size), y, size, i, (byte)((color & 0xFF0000) >> 16), (byte)((color & 0x00FF00) >> 8), (byte)(color & 0x0000FF), (byte)alpha);
                }
            }
        }
        // idée scrapée, je voulais mettre des planètes au fond de l'écran qui semblaient tourner, mais c'était trop compliqué pour si peu.
        //public static class Planet
        //{
        //    static void CreatePattern()
        //    {
                
        //    }

        //    public short[] RotatePoint(short width, short height, sbyte angle, short input_x, short input_y)
        //    {
        //        float angle_rad = angle * (float)(PI / 180);
        //        float delta = (float)Atan2(-width * Sin(angle_rad), height * Cos(angle_rad));
        //        short[] result = new short[2];
        //        result[0] = (short)(width * Cos(input_x + delta) * Cos(angle_rad) - height * Sin(input_x + delta) * Sin(angle_rad));
        //        result[1] = (short)(width * Cos(input_y + delta) * Sin(angle_rad) - height * Sin(input_y + delta) * Cos(angle_rad));
        //        return result;
        //    }
        //    public bool PointWithinPlanet(short point_x, short point_y, short planet_x, short planet_y)
        //    {
        //        return (Pow(point_x - planet_x, 2) + Pow(point_y - planet_y, 2) <= 1);
        //    }
        //    public short[] IntersectionLigneCercle(short x1, short y1, short x2, short y2, short x_cercle, short y_cercle, short r_cercle)
        //    {
        //        float a = (float)(Pow(x2 - x1, 2) + Pow(y2 - y1, 2));
        //        float b = 2 * (x2 - x1) * (x1 - x_cercle) + 2 * (y2 - y1) * (y1 - y_cercle);
        //        float c = (float)(Pow(x1 - x_cercle, 2) + Pow(y1 - y_cercle, 2) - Pow(r_cercle, 2));
        //        float r = (float)(-b - Sqrt(Pow(b, 2) - 4 * a * c) / 2 * a);
        //        short[] result = new short[2];
        //        result[0] = (short)((x2 > x1 ? (x1 + (x2 - x1)) : (x2 + (x1 - x2))) * r);
        //        result[1] = (short)((y2 > y1 ? (y1 + (y2 - y1)) : (y2 + (y1 - y2))) * r);
        //        return result;
        //    }
        //}
        public static class Cutscene
        {
            public static SDL_Rect rect = new SDL_Rect();
            static short[,] stars = new short[50, 2];
            static short[,] stars_glx = new short[300, 2];
            static short[,] neutron_slowdown = new short[50, 2];
            static sbyte[,] f_model = MODELE_A;
            static float[,] f_model_pos = new float[7, 3] {{0,0,0},{0,0,0},{0,0,0},{0,0,0},{0,0,0},{0,0,0},{0,0,0}};
            static byte lTimer = 10;
            static short temp = 0;
            // rendu inutile à cause de nouvelle propriété de la fonction pour montrer du texte
            //public static void Fade()
            //{
            //    if (gTimer == 0)
            //    {
            //        black.x = 0; black.y = 0; black.w = 1920; black.h = 1080;
            //    }
            //    SDL_SetRenderDrawColor(render, 0, 0, 0, (byte)gTimer);
            //    SDL_RenderFillRect(render, ref black);
            //}
            public static void Cut_0()
            {
                if (gTimer == 1 && cutscene_skip)
                    gTimer = 451;
                if (gTimer == 75)
                {
                    PlaySong(Music_list.presents);
                }

                if (gTimer >= 75 && gTimer < 150)
                {
                    Text.DisplayText("malcolm gauthier", short.MinValue, short.MinValue, 2);
                    Text.DisplayText("\n présente", short.MinValue, short.MinValue, 2);

                }
                else if (gTimer >= 150 && gTimer <= 225)
                {
                    Text.DisplayText("malcolm gauthier", short.MinValue, short.MinValue, 2, alpha: (short)((225 - gTimer) * 3.4f));
                    Text.DisplayText("\n présente", short.MinValue, short.MinValue, 2, alpha: (short)((225 - gTimer) * 3.4f));
                }

                if (gTimer > 225)
                {
                    gTimer_lock = false;
                    gamemode = 0;
                    PlaySong(Music_list.title);
                }
            }
            public static void Cut_1()
            {
                if (gTimer == 1 && cutscene_skip)
                    gTimer = 2101;

                if (gTimer == 60)
                {
                    PlaySong(Music_list.crtp);
                }

                if (gTimer >= 60 && gTimer <= 300)
                {
                    Text.DisplayText("des centaines d'années après les premiers voyages hors de la terre, l'espace \n" +
                                     "est devenue zone de guerre et de colonisation. des douzaines de factions \n" +
                                     "existent à travers la galaxie.", 20, 700, 3, scroll: (ushort)(gTimer - 60));

                    SDL_SetRenderDrawColor(render, 255, 127, 0, 255);
                    DessinerCercle(960, 440, 200, 50);
                    SDL_SetRenderDrawColor(render, 255, 0, 127, 255);
                    DessinerCercle(260, 390, 100, 50);
                    SDL_SetRenderDrawColor(render, 255, 127, 127, 255);
                    DessinerCercle(1660, 390, 100, 50);

                    #region planète 1
                    SDL_SetRenderDrawColor(render, 127, 255, 127, 255);
                    Background.NouveauDrawLine(render, 818, 298, 930, 336);
                    Background.NouveauDrawLine(render, 930, 336, 971, 355);
                    Background.NouveauDrawLine(render, 971, 355, 910, 373);
                    Background.NouveauDrawLine(render, 910, 373, 860, 400);
                    Background.NouveauDrawLine(render, 860, 400, 893, 412);
                    Background.NouveauDrawLine(render, 893, 412, 906, 438);
                    Background.NouveauDrawLine(render, 906, 438, 861, 453);
                    Background.NouveauDrawLine(render, 861, 453, 766, 492);
                    Background.NouveauDrawLine(render, 1160, 440, 1066, 425);
                    Background.NouveauDrawLine(render, 1066, 425, 1000, 455);
                    Background.NouveauDrawLine(render, 1000, 455, 1002, 490);
                    Background.NouveauDrawLine(render, 1002, 490, 1036, 497);
                    Background.NouveauDrawLine(render, 1036, 497, 1048, 515);
                    Background.NouveauDrawLine(render, 1048, 515, 989, 545);
                    Background.NouveauDrawLine(render, 989, 545, 1050, 583);
                    Background.NouveauDrawLine(render, 1050, 583, 1101, 581);
                    #endregion

                    #region planète 2
                    SDL_SetRenderDrawColor(render, 0, 0, 255, 255);
                    Background.NouveauDrawLine(render, 189, 319, 218, 334);
                    Background.NouveauDrawLine(render, 218, 334, 250, 344);
                    Background.NouveauDrawLine(render, 250, 344, 258, 365);
                    Background.NouveauDrawLine(render, 258, 365, 237, 395);
                    Background.NouveauDrawLine(render, 237, 395, 219, 425);
                    Background.NouveauDrawLine(render, 219, 425, 227, 454);
                    Background.NouveauDrawLine(render, 227, 454, 234, 486);
                    Background.NouveauDrawLine(render, 307, 302, 302, 330);
                    Background.NouveauDrawLine(render, 302, 330, 318, 339);
                    Background.NouveauDrawLine(render, 318, 339, 342, 333);
                    Background.NouveauDrawLine(render, 360, 390, 354, 406);
                    Background.NouveauDrawLine(render, 354, 406, 340, 411);
                    Background.NouveauDrawLine(render, 340, 411, 322, 395);
                    Background.NouveauDrawLine(render, 322, 395, 294, 400);
                    Background.NouveauDrawLine(render, 294, 400, 272, 426);
                    Background.NouveauDrawLine(render, 272, 426, 304, 450);
                    Background.NouveauDrawLine(render, 304, 450, 330, 460);
                    #endregion

                    #region planète 3
                    SDL_SetRenderDrawColor(render, 255, 0, 0, 255);
                    Background.NouveauDrawLine(render, 1657, 489, 1605, 443);
                    Background.NouveauDrawLine(render, 1605, 443, 1579, 393);
                    Background.NouveauDrawLine(render, 1579, 393, 1583, 350);
                    Background.NouveauDrawLine(render, 1583, 350, 1599, 310);
                    Background.NouveauDrawLine(render, 1700, 481, 1674, 457);
                    Background.NouveauDrawLine(render, 1674, 457, 1648, 422);
                    Background.NouveauDrawLine(render, 1648, 422, 1633, 361);
                    Background.NouveauDrawLine(render, 1633, 361, 1637, 321);
                    Background.NouveauDrawLine(render, 1637, 321, 1647, 290);
                    Background.NouveauDrawLine(render, 1740, 449, 1713, 419);
                    Background.NouveauDrawLine(render, 1713, 419, 1689, 371);
                    Background.NouveauDrawLine(render, 1689, 371, 1686, 331);
                    Background.NouveauDrawLine(render, 1686, 331, 1702, 299);
                    Background.NouveauDrawLine(render, 1759, 385, 1744, 371);
                    Background.NouveauDrawLine(render, 1744, 371, 1735, 349);
                    Background.NouveauDrawLine(render, 1735, 349, 1738, 328);
                    #endregion

                    #region drapeaux
                    SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
                    Background.NouveauDrawLine(render, 950, 100, 957, 240);
                    Background.NouveauDrawLine(render, 1644, 178, 1653, 290);
                    Background.NouveauDrawLine(render, 252, 174, 258, 290);

                    SDL_SetRenderDrawColor(render, 255, 0, 0, 255);
                    Background.NouveauDrawLine(render, 252, 174, 333, 176);
                    Background.NouveauDrawLine(render, 333, 176, 333, 225);
                    Background.NouveauDrawLine(render, 333, 225, 255, 229);

                    SDL_SetRenderDrawColor(render, 0, 0, 255, 255);
                    Background.NouveauDrawLine(render, 950, 100, 1081, 92);
                    Background.NouveauDrawLine(render, 1081, 92, 1082, 158);
                    Background.NouveauDrawLine(render, 1082, 158, 954, 163);

                    SDL_SetRenderDrawColor(render, 0, 255, 0, 255);
                    Background.NouveauDrawLine(render, 1644, 178, 1747, 172);
                    Background.NouveauDrawLine(render, 1747, 172, 1750, 230);
                    Background.NouveauDrawLine(render, 1750, 230, 1649, 235);
                    #endregion

                    if (gTimer == 60)
                    {
                        for (int i = 0; i < 50; i++)
                        {
                            int x = 151, y = 499;
                            while ((x > 150 && x < 375 && y < 500 && y > 280) ||
                                (x > 750 && x < 1175 && y < 650 && y > 230) ||
                                (x > 1550 && x < 1775 && y < 500 && y > 280))
                            {
                                x = RNG.Next(25, 1880);
                                y = RNG.Next(25, 680);
                            }
                            stars[i, 0] = (short)x;
                            stars[i, 1] = (short)y;

                        }
                    }
                    SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
                    for (int i = 0; i < 50; i++)
                    {
                        Background.NouveauDrawDot(render, stars[i, 0], stars[i, 1]);
                    }

                    if (gTimer % RNG.Next(10, 15) == 0)
                    {
                        short try_x = (short)RNG.Next(160, 1760), try_y = (short)RNG.Next(240, 640);
                        while (Distance(try_x, try_y, 960, 440) > 200 && Distance(try_x, try_y, 260, 390) > 100 && 
                            Distance(try_x, try_y, 1660, 390) > 100)
                        {
                            try_x = (short)RNG.Next(160, 1760);
                            try_y = (short)RNG.Next(240, 640);
                        }
                        Explosion.Call(try_x, try_y, (byte)RNG.Next(G_DEPTH_LAYERS / 8, G_DEPTH_LAYERS / 4));
                    }
                    Explosion.Draw();
                } // planètes
                else if (gTimer > 300 && gTimer <= 540)
                {
                    Text.DisplayText("ayant servi plus d'une décennie pour l'armée de ta planète, tu es reconnu \n" +
                                     "par le dirigeant militaire de ta faction comme un des meilleurs pilotes \n" +
                                     "de la région galactique locale.", 20, 700, 3, scroll: (ushort)(gTimer - 300));

                    #region toi
                    Background.NouveauDrawLine(render, 1282, 680, 1261, 417);
                    Background.NouveauDrawLine(render, 1261, 417, 1340, 366);
                    Background.NouveauDrawLine(render, 1340, 366, 1373, 400);
                    Background.NouveauDrawLine(render, 1373, 400, 1453, 400);
                    Background.NouveauDrawLine(render, 1453, 400, 1492, 368);
                    Background.NouveauDrawLine(render, 1492, 368, 1545, 412);
                    Background.NouveauDrawLine(render, 1545, 412, 1511, 680);

                    Background.NouveauDrawLine(render, 1261, 417, 1229, 637);
                    Background.NouveauDrawLine(render, 1545, 412, 1624, 337);
                    Background.NouveauDrawLine(render, 1624, 337, 1417, 260);
                    DessinerCercle(1416, 314, 77, 24);

                    SDL_SetRenderDrawColor(render, 255, 0, 127, 255);
                    Background.NouveauDrawLine(render, 1462, 434, 1503, 434);
                    Background.NouveauDrawLine(render, 1503, 434, 1484, 483);
                    Background.NouveauDrawLine(render, 1484, 483, 1462, 434);

                    SDL_SetRenderDrawColor(render, 255, 255, 0, 255);
                    Background.NouveauDrawLine(render, 1484, 483, 1468, 503);
                    Background.NouveauDrawLine(render, 1468, 503, 1484, 522);
                    Background.NouveauDrawLine(render, 1484, 522, 1499, 502);
                    Background.NouveauDrawLine(render, 1499, 502, 1484, 483);
                    #endregion

                    #region drapeau
                    SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
                    Background.NouveauDrawLine(render, 148, 680, 118, 57);
                    Background.NouveauDrawLine(render, 118, 57, 172, 49);
                    Background.NouveauDrawLine(render, 172, 49, 203, 680);

                    SDL_SetRenderDrawColor(render, 255, 255, 0, 255);
                    Background.NouveauDrawLine(render, 175, 115, 255, 61);
                    Background.NouveauDrawLine(render, 255, 61, 442, 55);
                    Background.NouveauDrawLine(render, 442, 55, 534, 88);
                    Background.NouveauDrawLine(render, 534, 88, 693, 82);
                    Background.NouveauDrawLine(render, 693, 82, 766, 40);
                    Background.NouveauDrawLine(render, 766, 40, 804, 443);
                    Background.NouveauDrawLine(render, 804, 443, 746, 478);
                    Background.NouveauDrawLine(render, 746, 478, 580, 489);
                    Background.NouveauDrawLine(render, 580, 489, 481, 443);
                    Background.NouveauDrawLine(render, 481, 443, 311, 452);
                    Background.NouveauDrawLine(render, 311, 452, 194, 500);

                    SDL_SetRenderDrawColor(render, 255, 127, 0, 255);
                    DessinerCercle(479, 232, 91, 24);
                    DessinerCercle(479, 232, 65, 24);

                    SDL_SetRenderDrawColor(render, 0, 255, 0, 255);
                    Background.NouveauDrawLine(render, 563, 195, 672, 204);
                    Background.NouveauDrawLine(render, 672, 204, 778, 172);
                    Background.NouveauDrawLine(render, 570, 243, 677, 251);
                    Background.NouveauDrawLine(render, 677, 251, 782, 214);
                    Background.NouveauDrawLine(render, 395, 197, 276, 190);
                    Background.NouveauDrawLine(render, 276, 190, 180, 219);
                    Background.NouveauDrawLine(render, 390, 250, 277, 242);
                    Background.NouveauDrawLine(render, 277, 242, 183, 282);
                    #endregion

                    #region étoile
                    SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
                    Background.NouveauDrawLine(render, 472, 167, 443, 286);
                    Background.NouveauDrawLine(render, 443, 286, 534, 197);
                    Background.NouveauDrawLine(render, 534, 197, 420, 206);
                    Background.NouveauDrawLine(render, 420, 206, 524, 279);
                    Background.NouveauDrawLine(render, 524, 279, 472, 167);
                    #endregion
                } // o7
                else if (gTimer > 540 && gTimer <= 780)
                {
                    Text.DisplayText("un jour, une lettre arrive à ta porte. elle porte l'emblême officielle du pays,\n" +
                                     "donc c'est probablement très important.", 20, 700, 3, scroll: (ushort)(gTimer - 540));

                    #region lettre
                    Background.NouveauDrawLine(render, 717, 607, 600, 400);
                    Background.NouveauDrawLine(render, 600, 400, 958, 198);
                    Background.NouveauDrawLine(render, 958, 198, 1062, 411);
                    Background.NouveauDrawLine(render, 1062, 411, 717, 607);
                    Background.NouveauDrawLine(render, 600, 400, 792, 385);
                    Background.NouveauDrawLine(render, 856, 353, 958, 198);
                    #endregion

                    #region emblême cercle
                    SDL_SetRenderDrawColor(render, 255, 127, 0, 255);
                    DessinerCercle(832, 384, 30, 24);
                    DessinerCercle(832, 384, 39, 24);
                    #endregion

                    #region étoile
                    SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
                    Background.NouveauDrawLine(render, 859, 396, 815, 358);
                    Background.NouveauDrawLine(render, 815, 358, 831, 414);
                    Background.NouveauDrawLine(render, 831, 414, 849, 359);
                    Background.NouveauDrawLine(render, 849, 359, 802, 389);
                    Background.NouveauDrawLine(render, 802, 389, 859, 396);
                    #endregion
                } // lettre fermée
                else if (gTimer > 780 && gTimer <= 1020)
                {
                    Text.DisplayText("\"bonjour. \n" +
                                     "la coalition des planètes locales vous a choisi pour mener une mission \n" +
                                     "seule qui sauvera notre peuple des mains ennemies, ou bien même la mort.\"", 20, 700, 3, scroll: (ushort)(gTimer - 780));

                    #region lettre
                    Background.NouveauDrawLine(render, 717, 607, 600, 400);
                    Background.NouveauDrawLine(render, 958, 198, 1062, 411);
                    Background.NouveauDrawLine(render, 1062, 411, 717, 607);
                    Background.NouveauDrawLine(render, 600, 400, 832, 383);
                    Background.NouveauDrawLine(render, 832, 383, 958, 198);
                    Background.NouveauDrawLine(render, 600, 400, 648, 316);
                    Background.NouveauDrawLine(render, 958, 198, 868, 193);
                    #endregion

                    #region papier
                    SDL_SetRenderDrawColor(render, 255, 150, 25, 255);
                    Background.NouveauDrawLine(render, 695, 393, 577, 199);
                    Background.NouveauDrawLine(render, 577, 199, 802, 79);
                    Background.NouveauDrawLine(render, 802, 79, 911, 267);
                    #endregion

                    #region blabla
                    SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
                    Background.NouveauDrawLine(render, 611, 212, 669, 180);
                    Background.NouveauDrawLine(render, 636, 252, 817, 153);
                    Background.NouveauDrawLine(render, 644, 273, 718, 231);
                    Background.NouveauDrawLine(render, 739, 218, 821, 173);
                    Background.NouveauDrawLine(render, 659, 292, 684, 276);
                    Background.NouveauDrawLine(render, 702, 265, 799, 212);
                    Background.NouveauDrawLine(render, 818, 201, 839, 188);
                    Background.NouveauDrawLine(render, 664, 312, 797, 228);
                    Background.NouveauDrawLine(render, 820, 221, 850, 200);
                    Background.NouveauDrawLine(render, 682, 337, 766, 280);
                    Background.NouveauDrawLine(render, 798, 263, 860, 225);
                    Background.NouveauDrawLine(render, 698, 358, 869, 251);
                    Background.NouveauDrawLine(render, 843, 317, 893, 282);
                    #endregion
                } // lettre ouverte
                else if (gTimer > 1020 && gTimer <= 1260)
                {
                    Text.DisplayText("\"on a récemment crée l'un des meilleurs vaisseaux de la galaxie, mais \n" +
                                     "à cause du nombre de ressources requis pour le construire, on en n'a \n" +
                                     "qu'un seul.\"", 20, 700, 3, scroll: (ushort)(gTimer - 1020));

                    #region modèles joueur
                    player.x = 400;
                    player.y = 400;
                    player.pitch = 4;
                    player.roll = 0;
                    player.scale = 6;
                    player.Render();

                    player.x = 1400;
                    player.y = 200;
                    player.pitch = 0;
                    player.roll = 0.25f * (float)Sin(gTimer / 30f);
                    player.scale = 5;
                    player.Render();
                    #endregion

                    #region lignes
                    Background.NouveauDrawLine(render, 293, 261, 347, 173);
                    Background.NouveauDrawLine(render, 347, 173, 588, 173);
                    Background.NouveauDrawLine(render, 462, 549, 521, 628);
                    Background.NouveauDrawLine(render, 521, 628, 744, 631);
                    Background.NouveauDrawLine(render, 1191, 76, 1164, 99);
                    Background.NouveauDrawLine(render, 1164, 99, 1166, 258);
                    Background.NouveauDrawLine(render, 1195, 284, 1166, 258);
                    Background.NouveauDrawLine(render, 1165, 176, 981, 171);
                    Background.NouveauDrawLine(render, 1474, 168, 1544, 96);
                    Background.NouveauDrawLine(render, 1544, 96, 1736, 96);
                    #endregion

                    #region texte et blabla rouge
                    Text.DisplayText("x-57", 880, 330, 10);
                    SDL_SetRenderDrawColor(render, 255, 0, 0, 255);
                    Background.NouveauDrawLine(render, 394, 142, 556, 136);
                    Background.NouveauDrawLine(render, 565, 591, 708, 582);
                    Background.NouveauDrawLine(render, 1016, 129, 1136, 137);
                    Background.NouveauDrawLine(render, 1589, 64, 1709, 67);
                    #endregion

                    #region plus de blabla
                    Background.NouveauDrawLine(render, 874, 472, 1237, 465);
                    Background.NouveauDrawLine(render, 1306, 456, 1680, 458);
                    Background.NouveauDrawLine(render, 1700, 500, 1565, 508);
                    Background.NouveauDrawLine(render, 1521, 512, 1028, 523);
                    Background.NouveauDrawLine(render, 979, 527, 867, 522);
                    Background.NouveauDrawLine(render, 874, 571, 1149, 569);
                    Background.NouveauDrawLine(render, 1206, 559, 1331, 566);
                    Background.NouveauDrawLine(render, 1402, 558, 1685, 553);
                    Background.NouveauDrawLine(render, 1687, 626, 1264, 626);
                    Background.NouveauDrawLine(render, 1210, 632, 1111, 631);
                    Background.NouveauDrawLine(render, 1042, 634, 879, 638);
                    #endregion

                } // vaisseau
                else if (gTimer > 1260 && gTimer <= 1500)
                {
                    Text.DisplayText("\"votre mission est d'aller détruire la bombe à pulsar dans la région \n" +
                                     "d'espace de l'ennemi, et de s'assurer qu'elle est neutralisée, ou sous \n" +
                                     "notre controle.\"", 20, 700, 3, scroll: (ushort)(gTimer - 1260));

                    BombePulsar.DessinerBombePulsar(960, 330, 180);

                    if (gTimer == 1261)
                    {
                        for (int i = 0; i < 50; i++)
                        {
                            int x = 960, y = 330;
                            while (x > 780 && y > 150 && x < 1140 && y < 510)
                            {
                                x = RNG.Next(25, 1880);
                                y = RNG.Next(25, 680);
                            }
                            stars[i, 0] = (short)x;
                            stars[i, 1] = (short)y;
                        }
                    }
                    SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
                    for (int i = 0; i < 50; i++)
                    {
                        Background.NouveauDrawDot(render, stars[i, 0], stars[i, 1]);
                    }

                    #region bleu autour de pôles
                    SDL_SetRenderDrawColor(render, 0, 0, 255, 255);
                    Background.NouveauDrawLine(render, 833, 201, 770, 127);
                    Background.NouveauDrawLine(render, 770, 127, 698, 120);
                    Background.NouveauDrawLine(render, 698, 120, 640, 135);
                    Background.NouveauDrawLine(render, 854, 184, 800, 100);
                    Background.NouveauDrawLine(render, 800, 100, 818, 36);
                    Background.NouveauDrawLine(render, 818, 36, 860, 21);
                    Background.NouveauDrawLine(render, 1055, 482, 1109, 549);
                    Background.NouveauDrawLine(render, 1109, 549, 1106, 606);
                    Background.NouveauDrawLine(render, 1106, 606, 1082, 643);
                    Background.NouveauDrawLine(render, 1078, 465, 1136, 526);
                    Background.NouveauDrawLine(render, 1136, 526, 1196, 531);
                    Background.NouveauDrawLine(render, 1196, 531, 1230, 515);
                    #endregion

                    #region plus de bleu autour des pôles
                    Background.NouveauDrawLine(render, 1069, 479, 1097, 518);
                    Background.NouveauDrawLine(render, 1090, 494, 1137, 542);
                    Background.NouveauDrawLine(render, 1111, 533, 1134, 567);
                    Background.NouveauDrawLine(render, 1120, 564, 1123, 617);
                    Background.NouveauDrawLine(render, 1139, 583, 1144, 619);
                    Background.NouveauDrawLine(render, 1148, 559, 1202, 596);
                    Background.NouveauDrawLine(render, 1169, 547, 1219, 553);
                    Background.NouveauDrawLine(render, 829, 181, 795, 139);
                    Background.NouveauDrawLine(render, 829, 158, 780, 85);
                    Background.NouveauDrawLine(render, 782, 123, 738, 96);
                    Background.NouveauDrawLine(render, 723, 101, 676, 86);
                    Background.NouveauDrawLine(render, 770, 97, 753, 57);
                    Background.NouveauDrawLine(render, 792, 79, 802, 28);
                    #endregion

                } // bombe à pulsar
                else if (gTimer > 1500 && gTimer <= 1740)
                {
                    Text.DisplayText("\"les coordonées de la bombe se trouvent programmés dans votre vaisseau, \n" +
                                     "qui vous attend au garage 05. \n" +
                                     "on compte sur vous, n'échouez pas.\" \n" +
                                     "- le dirigeant militaire", 20, 700, 3, scroll: (ushort)(gTimer - 1500));

                    #region lettre
                    Background.NouveauDrawLine(render, 717, 607, 600, 400);
                    Background.NouveauDrawLine(render, 958, 198, 1062, 411);
                    Background.NouveauDrawLine(render, 1062, 411, 717, 607);
                    Background.NouveauDrawLine(render, 600, 400, 832, 383);
                    Background.NouveauDrawLine(render, 832, 383, 958, 198);
                    Background.NouveauDrawLine(render, 600, 400, 648, 316);
                    Background.NouveauDrawLine(render, 958, 198, 868, 193);
                    #endregion

                    #region papier
                    SDL_SetRenderDrawColor(render, 255, 150, 25, 255);
                    Background.NouveauDrawLine(render, 695, 393, 577, 199);
                    Background.NouveauDrawLine(render, 577, 199, 802, 79);
                    Background.NouveauDrawLine(render, 802, 79, 911, 267);
                    #endregion

                    #region blabla
                    SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
                    Background.NouveauDrawLine(render, 611, 212, 669, 180);
                    Background.NouveauDrawLine(render, 636, 252, 817, 153);
                    Background.NouveauDrawLine(render, 644, 273, 718, 231);
                    Background.NouveauDrawLine(render, 739, 218, 821, 173);
                    Background.NouveauDrawLine(render, 659, 292, 684, 276);
                    Background.NouveauDrawLine(render, 702, 265, 799, 212);
                    Background.NouveauDrawLine(render, 818, 201, 839, 188);
                    Background.NouveauDrawLine(render, 664, 312, 797, 228);
                    Background.NouveauDrawLine(render, 820, 221, 850, 200);
                    Background.NouveauDrawLine(render, 682, 337, 766, 280);
                    Background.NouveauDrawLine(render, 798, 263, 860, 225);
                    Background.NouveauDrawLine(render, 698, 358, 869, 251);
                    Background.NouveauDrawLine(render, 843, 317, 893, 282);
                    #endregion

                } // lettre ouverte 2
                else if (gTimer > 1740 && gTimer <= 1980)
                {
                    Text.DisplayText("05", 100, 200, 15, 0x00FF00);
                    SDL_SetRenderDrawColor(render, 255, 255, 255, 255);

                    #region hangar
                    Background.NouveauDrawLine(render, 20, 455, 1900, 455);
                    Background.NouveauDrawLine(render, 500, 455, 500, 111);
                    Background.NouveauDrawLine(render, 500, 111, 1500, 111);
                    Background.NouveauDrawLine(render, 1500, 111, 1500, 455);
                    #endregion

                    if (gTimer < 1800)
                    {
                        Background.NouveauDrawLine(render, 1000, 111, 1000, 455);
                    }
                    if (gTimer >= 1800 && gTimer < 1860)
                    {
                        Background.NouveauDrawLine(render, 1000 - (int)(gTimer - 1800) * 5, 111, 1000 - (int)(gTimer - 1800) * 5, 455);
                        Background.NouveauDrawLine(render, 1000 + (int)(gTimer - 1800) * 5, 111, 1000 + (int)(gTimer - 1800) * 5, 455);
                    }
                    if (gTimer >= 1860)
                    {
                        Background.NouveauDrawLine(render, 700, 111, 700, 455);
                        Background.NouveauDrawLine(render, 1300, 111, 1300, 455);
                    }

                    if (gTimer == 1741)
                    {
                        for (int i = 0; i < 30; i++)
                        {
                            stars[i, 0] = (short)RNG.Next(720, 1280);
                            stars[i, 1] = (short)RNG.Next(120, 450);
                        }
                    }
                    if (gTimer >= 1800) for (int i = 0; i < 30; i++)
                    {
                        if (Abs(stars[i, 0] - 1000.0) < (gTimer - 1800) * 5)
                                Background.NouveauDrawDot(render, stars[i, 0], stars[i, 1]);
                    }

                    if (gTimer < 1830)
                    {
                        player.x = 1000;
                        player.y = 550;
                        player.pitch = 0;
                        player.roll = 0;
                        player.scale = 2;
                        player.Render();
                    }
                    if (gTimer >= 1830 && gTimer <= 1860)
                    {
                        player.x = 1000;
                        player.y = 250 * (float)Pow(0.9, gTimer-1830) + 300;
                        player.pitch = -((gTimer - 1830) / 90f);
                        player.roll = 0;
                        player.scale = 2 * (float)Pow(0.9, gTimer - 1830);
                        player.Render();
                    }

                    if (gTimer < 1815) Background.NouveauDrawLine(render, 972, 566, 952, 586);
                    if (gTimer < 1810) Background.NouveauDrawDot(render, (int)(gTimer - 1740 + 952 - 70), 585);
                    if (gTimer >= 1810 && gTimer <= 1815) Background.NouveauDrawDot(render, (int)((gTimer - 1810) * 2 + 972 - 20), (int)((1810 - gTimer) * 2 + 603 - 20));
                } // départ

                if (gTimer > 30 && gTimer < 2000)
                {
                    SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
                    Background.NouveauDrawLine(render, 20, 20, 1900, 20);
                    Background.NouveauDrawLine(render, 1900, 20, 1900, 680);
                    Background.NouveauDrawLine(render, 1900, 680, 20, 680);
                    Background.NouveauDrawLine(render, 20, 20, 20, 680);
                }

                if (gTimer > 2100)
                {
                    player.x = W_LARGEUR / 2;
                    player.y = W_HAUTEUR - 30;
                    player.roll = 0;
                    player.vy = -30;
                    player.scale = 1;
                    gTimer_lock = false;
                    gamemode = 2;
                    player.HP = 100;
                    player.shockwaves = 3;
                    level = 0;
                    ens_needed = (byte)Level_Data.lvl_list[level].Length;
                    ens_killed = 0;
                }
            }
            public static void Cut_2()
            {
                if (gTimer == 2 && cutscene_skip)
                    gTimer = 1830;

                if (gTimer == 60)
                {
                    PlaySong(Music_list.atw);
                    gTimer = 120;
                    player.dead = false;
                    //gTimer = 1740;//
                }

                if (gTimer >= 120 && gTimer < 300)
                {
                    #region étoiles
                    if (gTimer == 120)
                    {
                        for (int i = 0; i < stars.GetLength(0); i++)
                        {
                            int x = 1500, y = 400;
                            while ((x > 1375 && y > 128 && x < 1676 && y < 402) || (x > 137 && y > 433 && x < 379 && y < 622))
                            {
                                x = RNG.Next(25, 1880);
                                y = RNG.Next(25, 680);
                            }
                            stars[i, 0] = (short)x;
                            stars[i, 1] = (short)y;
                        }
                    }
                    SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
                    for (int i = 0; i < stars.GetLength(0); i++)
                    {
                        Background.NouveauDrawDot(render, stars[i, 0], stars[i, 1]);
                    }
                    #endregion

                    #region player
                    player.x = 277;
                    player.y = 519;
                    player.pitch = 0.7f;
                    player.roll = 0.8f;
                    player.scale = 2;
                    player.Render();
                    #endregion

                    #region enemy 15
                    player.scale = 1f;
                    player.x = 771;
                    player.y = 325;
                    player.roll = 0.5f;
                    double sinroll = Sin(player.roll);
                    double cosroll = Cos(player.roll);
                    float pitchconst = player.pitch + P_PERMA_PITCH;
                    SDL_SetRenderDrawColor(render, 255, 0, 0, 255);
                    for (int i = 0; i < player.model.GetLength(0) - 1; i++)
                    {
                        int[] pos = new int[4] {
                            (int)(player.scale * (cosroll * -player.model[i, 0] - sinroll * -player.model[i, 1]) + player.x),
                            (int)(player.scale * (sinroll * -player.model[i, 0] + cosroll * -player.model[i, 1]) + player.y - player.model[i, 2] * pitchconst),
                            (int)(player.scale * (cosroll * -player.model[i + 1, 0] - sinroll * -player.model[i + 1, 1]) + player.x),
                            (int)(player.scale * (sinroll * -player.model[i + 1, 0] + cosroll * -player.model[i + 1, 1]) + player.y - player.model[i + 1, 2] * pitchconst)
                        };
                        Background.NouveauDrawLine(render, pos[0], pos[1], pos[2], pos[3]);
                    }
                    #endregion

                    #region pulsar bomb
                    BombePulsar.DessinerBombePulsar((short)(1522 + RNG.Next(-5, 5)), (short)(264 + RNG.Next(-5, 5)), 133);

                    SDL_SetRenderDrawColor(render, 0, 0, 255, 255);
                    Background.NouveauDrawLine(render, 1463, 144, 1445, 97);
                    Background.NouveauDrawLine(render, 1445, 97, 1422, 68);
                    Background.NouveauDrawLine(render, 1422, 68, 1373, 46);
                    Background.NouveauDrawLine(render, 1525, 131, 1522, 86);
                    Background.NouveauDrawLine(render, 1522, 86, 1554, 48);
                    Background.NouveauDrawLine(render, 1554, 48, 1584, 35);
                    Background.NouveauDrawLine(render, 1501, 36, 1499, 82);
                    Background.NouveauDrawLine(render, 1464, 63, 1484, 112);
                    Background.NouveauDrawLine(render, 1500, 396, 1491, 454);
                    Background.NouveauDrawLine(render, 1491, 454, 1470, 493);
                    Background.NouveauDrawLine(render, 1470, 493, 1450, 534);
                    Background.NouveauDrawLine(render, 1549, 395, 1550, 450);
                    Background.NouveauDrawLine(render, 1550, 450, 1574, 486);
                    Background.NouveauDrawLine(render, 1574, 486, 1612, 513);
                    Background.NouveauDrawLine(render, 1514, 434, 1506, 485);
                    Background.NouveauDrawLine(render, 1539, 475, 1560, 520);
                    #endregion

                    #region boom
                    if (gTimer > 200)
                    {
                        lTimer = (byte)(gTimer - 200);
                        SDL_SetRenderDrawColor(render, 255, 255, 255, (byte)(154 + lTimer));
                        rect.x = 1522 - lTimer * 30;
                        if (rect.x < 20)
                            rect.x = 20;
                        rect.y = 264 + lTimer * 10;
                        if (rect.y > 680)
                            rect.y = 680;
                        rect.w = 2 * (1522 - rect.x);
                        if (rect.x + rect.w > 1900)
                            rect.w = 1900 - rect.x;
                        rect.h = -2 * (rect.y - 264); // https://i.imgur.com/upLlqTg.png BITCH, CA MARCHE PARFAITEMENT
                        if (rect.y + rect.h < 20)
                            rect.h = 20 - rect.y;
                        NouveauDrawBox(render, ref rect);
                        rect.x = 1522 - lTimer * 20;
                        if (rect.x < 20)
                            rect.x = 20;
                        rect.y = 264 + lTimer * 20;
                        if (rect.y > 680)
                            rect.y = 680;
                        rect.w = 2 * (1522 - rect.x);
                        if (rect.x + rect.w > 1900)
                            rect.w = 1900 - rect.x;
                        rect.h = -2 * (rect.y - 264);
                        if (rect.y + rect.h < 20)
                            rect.h = 20 - rect.y;
                        NouveauDrawBox(render, ref rect);
                        rect.x = 1522 - lTimer * 10;
                        if (rect.x < 20)
                            rect.x = 20;
                        rect.y = 264 + lTimer * 30;
                        if (rect.y > 680)
                            rect.y = 680;
                        rect.w = 2 * (1522 - rect.x);
                        if (rect.x + rect.w > 1900)
                            rect.w = 1900 - rect.x;
                        rect.h = -2 * (rect.y - 264);
                        if (rect.y + rect.h < 20)
                            rect.h = 20 - rect.y;
                        NouveauDrawBox(render, ref rect);
                    }
                    #endregion
                } // boom - fini
                else if (gTimer >= 300 && gTimer < 660)
                {
                    #region étoiles
                    if (gTimer == 300)
                    {
                        for (int i = 0; i < stars.GetLength(0); i++)
                        {
                            int x = RNG.Next(25, 1880), y = RNG.Next(25, 680);
                            while (x > 442 && y > 76 && x < 1447 && y < 601)
                            {
                                x = RNG.Next(25, 1880);
                                y = RNG.Next(25, 680);
                            }
                            stars[i, 0] = (short)x;
                            stars[i, 1] = (short)y;
                        }
                        for (int i = 0; i < stars_glx.GetLength(0); i++)
                        {
                            int x = RNG.Next(442, 1447), y = RNG.Next(76, 601);
                            while (Distance(x, y, 948, 338, 0.3f) > 270 || Distance(x, y, 954, 276, 0.6f) < 80)
                            {
                                x = RNG.Next(25, 1880);
                                y = RNG.Next(25, 680);
                            }
                            stars_glx[i, 0] = (short)x;
                            stars_glx[i, 1] = (short)y;
                        }
                    }
                    SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
                    for (int i = 0; i < stars.GetLength(0); i++)
                    {
                        Background.NouveauDrawDot(render, stars[i, 0], stars[i, 1]);
                    }
                    for (int i = 0; i < stars_glx.GetLength(0); i++)
                    {
                        if (gTimer > 480)
                        {
                            if (Distance(stars_glx[i, 0], stars_glx[i, 1], 716, 437, 0.5f) < 100)
                                SDL_SetRenderDrawColor(render, 0, 0, 0, 255);
                            else
                                SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
                        }
                        Background.NouveauDrawDot(render, stars_glx[i, 0], stars_glx[i, 1]);
                    }
                    #endregion

                    #region galaxie
                    SDL_SetRenderDrawColor(render, 255, 255, 255, 60);
                    Background.NouveauDrawLine(render, 525, 362, 477, 195);
                    Background.NouveauDrawLine(render, 477, 195, 547, 223);
                    Background.NouveauDrawLine(render, 547, 223, 686, 91);
                    Background.NouveauDrawLine(render, 686, 91, 697, 141);
                    Background.NouveauDrawLine(render, 697, 141, 950, 79);
                    Background.NouveauDrawLine(render, 950, 79, 933, 126);
                    Background.NouveauDrawLine(render, 933, 126, 1230, 92);
                    Background.NouveauDrawLine(render, 1230, 92, 1191, 126);
                    Background.NouveauDrawLine(render, 1191, 126, 1404, 179);
                    Background.NouveauDrawLine(render, 1404, 179, 1344, 186);
                    Background.NouveauDrawLine(render, 1344, 186, 1434, 323);
                    Background.NouveauDrawLine(render, 1434, 323, 1370, 312);
                    Background.NouveauDrawLine(render, 1370, 312, 1404, 441);
                    Background.NouveauDrawLine(render, 1404, 441, 1354, 427);
                    Background.NouveauDrawLine(render, 1354, 427, 1285, 571);
                    Background.NouveauDrawLine(render, 1285, 571, 1285, 499);
                    Background.NouveauDrawLine(render, 1285, 499, 1017, 600);
                    Background.NouveauDrawLine(render, 1017, 600, 1032, 538);
                    Background.NouveauDrawLine(render, 1032, 538, 700, 600);
                    Background.NouveauDrawLine(render, 700, 600, 734, 553);
                    Background.NouveauDrawLine(render, 734, 553, 500, 500);
                    Background.NouveauDrawLine(render, 500, 500, 600, 500);
                    Background.NouveauDrawLine(render, 600, 500, 451, 377);
                    Background.NouveauDrawLine(render, 451, 377, 525, 362);

                    SDL_SetRenderDrawColor(render, 127, 0, 127, 255);
                    Background.NouveauDrawLine(render, 850, 350, 833, 306);
                    Background.NouveauDrawLine(render, 833, 306, 842, 271);
                    Background.NouveauDrawLine(render, 842, 271, 878, 221);
                    Background.NouveauDrawLine(render, 878, 221, 952, 193);
                    Background.NouveauDrawLine(render, 952, 193, 1027, 216);
                    Background.NouveauDrawLine(render, 1027, 216, 1064, 260);
                    Background.NouveauDrawLine(render, 1064, 260, 1076, 299);
                    Background.NouveauDrawLine(render, 1076, 299, 1070, 340);

                    Background.NouveauDrawLine(render, 828, 315, 798, 324);
                    Background.NouveauDrawLine(render, 798, 324, 800, 350);
                    Background.NouveauDrawLine(render, 800, 350, 831, 359);
                    Background.NouveauDrawLine(render, 831, 359, 1082, 347);
                    Background.NouveauDrawLine(render, 1082, 347, 1114, 333);
                    Background.NouveauDrawLine(render, 1114, 333, 1112, 309);
                    Background.NouveauDrawLine(render, 1112, 309, 1081, 304);
                    #endregion

                    #region explosions
                    if (gTimer < 480)
                    {
                        if (gTimer % RNG.Next(8, 12) == 0)
                            Explosion.Call((short)RNG.Next(500, 1400), (short)RNG.Next(100, 550), G_DEPTH_LAYERS / 2);
                    }
                    Explosion.Draw();
                    #endregion

                    #region boom
                    if (gTimer > 450 && gTimer <= 500)
                    {
                        lTimer = (byte)(2 * gTimer - 900);
                        int abs_lTimer = -2 * Abs(lTimer - 50) + 100; // y=-2|x-(r/2)|+r, ou environ /\
                        SDL_SetRenderDrawColor(render, 255, 255, 255, (byte)(154 + lTimer));
                        rect.x = 716 - abs_lTimer * 2;
                        rect.y = 437 + abs_lTimer / 4;
                        rect.w = 2 * (716 - rect.x);
                        rect.h = -2 * (rect.y - 437);
                        NouveauDrawBox(render, ref rect);
                        rect.x = 716 - abs_lTimer;
                        rect.y = 437 + abs_lTimer / 2;
                        rect.w = 2 * (716 - rect.x);
                        rect.h = -2 * (rect.y - 437);
                        NouveauDrawBox(render, ref rect);
                        rect.x = 716 - abs_lTimer / 2;
                        rect.y = 437 + abs_lTimer;
                        rect.w = 2 * (716 - rect.x);
                        rect.h = -2 * (rect.y - 437);
                        NouveauDrawBox(render, ref rect);
                    }
                    #endregion
                } // boom galactique (2) - fini
                else if (gTimer >= 660 && gTimer < 840)
                {
                    #region étoiles
                    if (gTimer == 660)
                    {
                        for (int i = 0; i < stars.GetLength(0); i++)
                        {
                            int x = RNG.Next(25, 1880), y = RNG.Next(25, 680);
                            while (Distance(x, y, W_SEMI_LARGEUR, W_SEMI_HAUTEUR / 2, 0.2f) < 200)
                            {
                                x = RNG.Next(25, 1880);
                                y = RNG.Next(25, 680);
                            }
                            stars[i, 0] = (short)x;
                            stars[i, 1] = (short)y;
                        }
                    }
                    SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
                    for (int i = 0; i < stars.GetLength(0); i++)
                    {
                        Background.NouveauDrawDot(render, stars[i, 0], stars[i, 1]);
                    }
                    #endregion

                    #region vaisseaux
                    SDL_SetRenderDrawColor(render, 255, 0, 0, 255);
                    Background.NouveauDrawDot(render, (int)-gTimer + 1900, W_SEMI_HAUTEUR / 2);
                    SDL_SetRenderDrawColor(render, 0, 255, 0, 255);
                    Background.NouveauDrawDot(render, W_SEMI_LARGEUR, (int)gTimer / -2 + 730);
                    SDL_SetRenderDrawColor(render, 255, 255, 0, 255);
                    Background.NouveauDrawDot(render, (int)gTimer - 1, W_SEMI_HAUTEUR / 2);
                    SDL_SetRenderDrawColor(render, 0, 0, 255, 255);
                    Background.NouveauDrawDot(render, W_SEMI_LARGEUR, (int)gTimer / 2 - 200);
                    #endregion

                } // ranconte - fini
                else if (gTimer >= 840 && gTimer < 1020)
                {
                    Text.DisplayText("le grand vide laissé par l'explosion a démontré la vérité de cette guerre.", 20, 700, 3, scroll: (int)gTimer/2 - 420);

                    #region étoiles
                    if (gTimer == 840)
                    {
                        for (int i = 0; i < stars.GetLength(0); i++)
                        {
                            stars[i, 0] = (short)RNG.Next(25, 1880);
                            stars[i, 1] = (short)RNG.Next(25, 180);
                        }
                    }
                    SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
                    for (int i = 0; i < stars.GetLength(0); i++)
                    {
                        Background.NouveauDrawDot(render, stars[i, 0], stars[i, 1]);
                    }
                    #endregion

                    #region vaisseaux
                    sbyte[,] model = MODELE_E6;
                    short x, y;
                    sbyte depth = -15;
                    float pitch = -1f;

                    SDL_SetRenderDrawColor(render, 255, 0, 0, 255);
                    x = (short)(-gTimer/10 + 1200);
                    y = (short)(W_SEMI_HAUTEUR - 100);
                    Background.NouveauDrawLine(render, x, y, x + 100, y - 65);
                    Background.NouveauDrawLine(render, x + 100, y - 65, x + 90, y);
                    Background.NouveauDrawLine(render, x + 90, y, x + 139, y - 63);
                    Background.NouveauDrawLine(render, x + 139, y - 63, x, y);
                    Background.NouveauDrawLine(render, x, y, x + 90, y);

                    SDL_SetRenderDrawColor(render, 0, 255, 0, 255);
                    x = W_SEMI_LARGEUR;
                    y = (short)(W_SEMI_HAUTEUR / 2 + ((int)gTimer - 880) / 10);
                    for (int i = 0; i < model.GetLength(0) - 1; i++)
                    {
                        Background.NouveauDrawLine(render,
                            (int)(model[i, 0] * Pow(0.95, depth) + x),
                            (int)((model[i, 1] + (model[i, 2] * pitch)) * Pow(0.95, depth) + y),
                            (int)(model[i + 1, 0] * Pow(0.95, depth) + x),
                            (int)((model[i + 1, 1] + (model[i + 1, 2] * pitch)) * Pow(0.95, depth) + y));
                    }

                    SDL_SetRenderDrawColor(render, 255, 255, 0, 255);
                    x = (short)(gTimer/10 + 700);
                    y = (short)(W_SEMI_HAUTEUR - 100);
                    Background.NouveauDrawLine(render, x, y, x - 100, y - 65);
                    Background.NouveauDrawLine(render, x - 100, y - 65, x - 90, y);
                    Background.NouveauDrawLine(render, x - 90, y, x - 139, y - 63);
                    Background.NouveauDrawLine(render, x - 139, y - 63, x, y);
                    Background.NouveauDrawLine(render, x, y, x - 90, y);

                    SDL_SetRenderDrawColor(render, 0, 0, 255, 255);
                    x = W_SEMI_LARGEUR;
                    y = (short)(W_SEMI_HAUTEUR - (gTimer - 800) / 10);
                    for (int i = 0; i < model.GetLength(0) - 1; i++)
                    {
                        Background.NouveauDrawLine(render,
                            (int)(model[i, 0] * Pow(0.95, depth) + x),
                            (int)((model[i, 1] + (model[i, 2] * -pitch)) * Pow(0.95, depth) + y),
                            (int)(model[i + 1, 0] * Pow(0.95, depth) + x),
                            (int)((model[i + 1, 1] + (model[i + 1, 2] * -pitch)) * Pow(0.95, depth) + y));
                    }
                    #endregion

                } // ranconte proche - fini
                else if (gTimer >= 1020 && gTimer < 1200)
                {
                    Text.DisplayText("l'abscence de vrai gagnants.", 20, 700, 3, scroll: (int)(gTimer / 2 - 510));

                    #region étoiles
                    if (gTimer == 1020)
                    {
                        for (int i = 0; i < stars.GetLength(0); i++)
                        {
                            int x = RNG.Next(25, 1880), y = RNG.Next(25, 680);
                            while (Distance(x, y, W_SEMI_LARGEUR, W_SEMI_HAUTEUR / 2 + 100, 0.2f) < 200)
                            {
                                x = RNG.Next(25, 1880);
                                y = RNG.Next(25, 680);
                            }
                            stars[i, 0] = (short)x;
                            stars[i, 1] = (short)y;
                        }
                    }
                    SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
                    for (int i = 0; i < stars.GetLength(0); i++)
                    {
                        Background.NouveauDrawDot(render, stars[i, 0], stars[i, 1]);
                    }
                    #endregion

                    #region vaisseaux
                    SDL_SetRenderDrawColor(render, 255, 0, 0, 255);
                    Background.NouveauDrawDot(render, 960, 340);
                    SDL_SetRenderDrawColor(render, 0, 255, 0, 255);
                    Background.NouveauDrawDot(render, 950, 345);
                    SDL_SetRenderDrawColor(render, 255, 255, 0, 255);
                    Background.NouveauDrawDot(render, 940, 340);
                    SDL_SetRenderDrawColor(render, 0, 0, 255, 255);
                    Background.NouveauDrawDot(render, 950, 335);
                    #endregion
                } // ranconte loin - fini
                else if (gTimer >= 1200 && gTimer < 1380)
                {
                    Text.DisplayText("les factions qui ont survécu ont vite cherché la paix entre eux.\n" +
                                     "des milliards sont morts, victimes de cette guerre. des milliards de trop.", 20, 700, 3, scroll: (int)(gTimer - 1200));

                    #region promesse
                    SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
                    Background.NouveauDrawLine(render, 600, 680, 600, 500);
                    Background.NouveauDrawLine(render, 600, 500, 1350, 500);
                    Background.NouveauDrawLine(render, 1350, 500, 1350, 680);

                    DessinerCercle(730, 189, 57, 50);
                    Background.NouveauDrawLine(render, 648, 500, 606, 238);
                    Background.NouveauDrawLine(render, 606, 238, 836, 255);
                    Background.NouveauDrawLine(render, 836, 255, 806, 500);
                    Background.NouveauDrawLine(render, 606, 238, 593, 470);

                    DessinerCercle(1203, 186, 57, 50);
                    Background.NouveauDrawLine(render, 1126, 500, 1096, 255);
                    Background.NouveauDrawLine(render, 1096, 255, 1304, 235);
                    Background.NouveauDrawLine(render, 1304, 235, 1278, 500);
                    Background.NouveauDrawLine(render, 1304, 235, 1336, 435);

                    Background.NouveauDrawLine(render, 836, 255, 972, 297 + (gTimer % 30 < 15 ? 0 : 50));
                    Background.NouveauDrawLine(render, 1096, 255, 972, 297 + (gTimer % 30 < 15 ? 0 : 50));
                    #endregion

                    #region drapeaux
                    short[] positions = new short[4] { 100, 300, 1500, 1700};
                    foreach (short i in positions)
                    {
                        SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
                        Background.NouveauDrawLine(render, i, 680, i, 509);
                        Background.NouveauDrawLine(render, i, 100, i, 365);

                        switch (i)
                        {
                            case 100:
                                SDL_SetRenderDrawColor(render, 255, 0, 0, 255);
                                break;
                            case 300:
                                SDL_SetRenderDrawColor(render, 255, 255, 0, 255);
                                break;
                            case 1500:
                                SDL_SetRenderDrawColor(render, 0, 255, 0, 255);
                                break;
                            case 1700:
                                SDL_SetRenderDrawColor(render, 0, 0, 255, 255);
                                break;
                        }
                        Background.NouveauDrawLine(render, i, 100, 87 + i, 272);
                        Background.NouveauDrawLine(render, 87 + i, 272, 100 + i, 400);
                        Background.NouveauDrawLine(render, 100 + i, 400, 76 + i, 449);
                        Background.NouveauDrawLine(render, 76 + i, 449, 69 + i, 519);
                        Background.NouveauDrawLine(render, 69 + i, 519, 90 + i, 557);
                        Background.NouveauDrawLine(render, 90 + i, 557, 32 + i, 574);
                        Background.NouveauDrawLine(render, 32 + i, 574, -28 + i, 449);
                        Background.NouveauDrawLine(render, -28 + i, 449, -13 + i, 389);
                        Background.NouveauDrawLine(render, -13 + i, 389, 14 + i, 338);
                        Background.NouveauDrawLine(render, 14 + i, 338, 15 + i, 305);
                        Background.NouveauDrawLine(render, 15 + i, 305, i, 300);
                    }
                    #endregion
                }// paix - fini
                else if (gTimer >= 1380 && gTimer < 1560)
                {
                    #region traité
                    SDL_SetRenderDrawColor(render, 255, 150, 25, 255);
                    Background.NouveauDrawLine(render, 700, 600, 800, 100);
                    Background.NouveauDrawLine(render, 800, 100, 1200, 100);
                    Background.NouveauDrawLine(render, 1200, 100, 1300, 600);
                    Background.NouveauDrawLine(render, 1300, 600, 700, 600);

                    Text.DisplayText("traité de paix", 825, 125, 3, 0x7f7f7f);

                    SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
                    Background.NouveauDrawLine(render, 816, 238, 979, 236);
                    Background.NouveauDrawLine(render, 1028, 238, 1180, 234);
                    Background.NouveauDrawLine(render, 809, 283, 849, 280);
                    Background.NouveauDrawLine(render, 905, 286, 1177, 283);
                    Background.NouveauDrawLine(render, 797, 325, 910, 324);
                    Background.NouveauDrawLine(render, 945, 325, 1011, 325);
                    Background.NouveauDrawLine(render, 1040, 324, 1183, 326);
                    Background.NouveauDrawLine(render, 782, 382, 1085, 382);
                    Background.NouveauDrawLine(render, 1128, 380, 1199, 382);
                    Background.NouveauDrawLine(render, 779, 434, 899, 434);
                    Background.NouveauDrawLine(render, 776, 483, 988, 482);
                    Background.NouveauDrawLine(render, 783, 583, 1098, 582);
                    Background.NouveauDrawLine(render, 775, 523, 744, 572);
                    Background.NouveauDrawLine(render, 744, 520, 774, 572);
                    #endregion

                    #region main + signature
                    short x = 2000, y = 0;
                    if (gTimer > 1410 && gTimer <= 1430)
                    {
                        x = 800;
                        y = (short)(750 - (gTimer - 1410) * 9);
                    }
                    else if (gTimer > 1430 && gTimer <= 1500)
                    {
                        x = (short)(800 + (gTimer - 1430) * 3.57f);
                        if (temp == 0)
                        {
                            lTimer = 127;
                            temp = 561;
                            for (int i = 0; i < stars.GetLength(0); i++)
                            {
                                stars[i, 0] = -1;
                                stars[i, 1] = -1;
                            }
                        }
                        if (Abs(temp - (lTimer + 434)) < 6)
                        {
                            lTimer = (byte)RNG.Next(107, 147);
                            for (int i = 0; i < stars.GetLength(0); i++)
                            {
                                if (stars[i, 0] == -1)
                                {
                                    stars[i, 0] = x;
                                    stars[i, 1] = temp;
                                    break;
                                }
                            }
                        }
                        else if (temp > lTimer + 434)
                            temp -= 5;
                        else if (temp < lTimer + 434)
                            temp += 5;
                        y = temp;
                    }
                    else if (gTimer > 1500 && gTimer <= 1520)
                    {
                        x = 1050;
                        y = (short)(561 + (gTimer - 1500) * 9);
                    }
                    Background.NouveauDrawLine(render, x, y, x + 13, y - 27);
                    Background.NouveauDrawLine(render, x + 13, y - 27, x + 46, y - 70);
                    Background.NouveauDrawLine(render, x + 46, y - 70, x + 53, y - 63);
                    Background.NouveauDrawLine(render, x + 53, y - 63, x + 18, y - 19);
                    Background.NouveauDrawLine(render, x + 18, y - 19, x, y);
                    Background.NouveauDrawLine(render, x + 43, y - 61, x + 202, y + 178);

                    if (gTimer > 1430)
                    {
                        for (int i = 1; i < stars.GetLength(0); i++)
                        {
                            if (stars[i, 0] == -1 || stars[1, 0] == -1)
                                break;
                            Background.NouveauDrawLine(render, stars[i - 1, 0], stars[i - 1, 1], stars[i, 0], stars[i, 1]);
                        }
                    }

                    if (gTimer == 1380)
                    {
                        rect.x = 0;
                        rect.y = 681;
                        rect.h = 400;
                        rect.w = 2000;
                    }
                    SDL_SetRenderDrawColor(render, 0, 0, 0, 255);
                    SDL_RenderFillRect(render, ref rect);
                    #endregion

                    Text.DisplayText("les factions qui ont survécu ont vite charché la paix entre eux.\n" +
                                     "des milliards sont morts, victimes de cette guerre. des milliards de trop.", 20, 700, 3);
                } // signature - fini
                else if (gTimer >= 1560 && gTimer < 1740)
                {
                    Text.DisplayText("les factions galactiques prospèrent maintenant tous économiquement\n" +
                                     "avec leurs liens d'amitié entre eux.", 20, 700, 3, scroll: (int)(gTimer - 1560));

                    #region planètes
                    SDL_SetRenderDrawColor(render, 255, 127, 0, 255);
                    DessinerCercle(960, 440, 200, 50);
                    SDL_SetRenderDrawColor(render, 255, 0, 127, 255);
                    DessinerCercle(260, 390, 100, 50);
                    SDL_SetRenderDrawColor(render, 255, 127, 127, 255);
                    DessinerCercle(1660, 390, 100, 50);

                    SDL_SetRenderDrawColor(render, 127, 255, 127, 255);
                    Background.NouveauDrawLine(render, 818, 298, 930, 336);
                    Background.NouveauDrawLine(render, 930, 336, 971, 355);
                    Background.NouveauDrawLine(render, 971, 355, 910, 373);
                    Background.NouveauDrawLine(render, 910, 373, 860, 400);
                    Background.NouveauDrawLine(render, 860, 400, 893, 412);
                    Background.NouveauDrawLine(render, 893, 412, 906, 438);
                    Background.NouveauDrawLine(render, 906, 438, 861, 453);
                    Background.NouveauDrawLine(render, 861, 453, 766, 492);
                    Background.NouveauDrawLine(render, 1160, 440, 1066, 425);
                    Background.NouveauDrawLine(render, 1066, 425, 1000, 455);
                    Background.NouveauDrawLine(render, 1000, 455, 1002, 490);
                    Background.NouveauDrawLine(render, 1002, 490, 1036, 497);
                    Background.NouveauDrawLine(render, 1036, 497, 1048, 515);
                    Background.NouveauDrawLine(render, 1048, 515, 989, 545);
                    Background.NouveauDrawLine(render, 989, 545, 1050, 583);
                    Background.NouveauDrawLine(render, 1050, 583, 1101, 581);

                    SDL_SetRenderDrawColor(render, 0, 0, 255, 255);
                    Background.NouveauDrawLine(render, 189, 319, 218, 334);
                    Background.NouveauDrawLine(render, 218, 334, 250, 344);
                    Background.NouveauDrawLine(render, 250, 344, 258, 365);
                    Background.NouveauDrawLine(render, 258, 365, 237, 395);
                    Background.NouveauDrawLine(render, 237, 395, 219, 425);
                    Background.NouveauDrawLine(render, 219, 425, 227, 454);
                    Background.NouveauDrawLine(render, 227, 454, 234, 486);
                    Background.NouveauDrawLine(render, 307, 302, 302, 330);
                    Background.NouveauDrawLine(render, 302, 330, 318, 339);
                    Background.NouveauDrawLine(render, 318, 339, 342, 333);
                    Background.NouveauDrawLine(render, 360, 390, 354, 406);
                    Background.NouveauDrawLine(render, 354, 406, 340, 411);
                    Background.NouveauDrawLine(render, 340, 411, 322, 395);
                    Background.NouveauDrawLine(render, 322, 395, 294, 400);
                    Background.NouveauDrawLine(render, 294, 400, 272, 426);
                    Background.NouveauDrawLine(render, 272, 426, 304, 450);
                    Background.NouveauDrawLine(render, 304, 450, 330, 460);

                    SDL_SetRenderDrawColor(render, 255, 0, 0, 255);
                    Background.NouveauDrawLine(render, 1657, 489, 1605, 443);
                    Background.NouveauDrawLine(render, 1605, 443, 1579, 393);
                    Background.NouveauDrawLine(render, 1579, 393, 1583, 350);
                    Background.NouveauDrawLine(render, 1583, 350, 1599, 310);
                    Background.NouveauDrawLine(render, 1700, 481, 1674, 457);
                    Background.NouveauDrawLine(render, 1674, 457, 1648, 422);
                    Background.NouveauDrawLine(render, 1648, 422, 1633, 361);
                    Background.NouveauDrawLine(render, 1633, 361, 1637, 321);
                    Background.NouveauDrawLine(render, 1637, 321, 1647, 290);
                    Background.NouveauDrawLine(render, 1740, 449, 1713, 419);
                    Background.NouveauDrawLine(render, 1713, 419, 1689, 371);
                    Background.NouveauDrawLine(render, 1689, 371, 1686, 331);
                    Background.NouveauDrawLine(render, 1686, 331, 1702, 299);
                    Background.NouveauDrawLine(render, 1759, 385, 1744, 371);
                    Background.NouveauDrawLine(render, 1744, 371, 1735, 349);
                    Background.NouveauDrawLine(render, 1735, 349, 1738, 328);
                    #endregion

                    #region drapeaux
                    SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
                    Background.NouveauDrawLine(render, 950, 100, 957, 240);
                    Background.NouveauDrawLine(render, 1644, 178, 1653, 290);
                    Background.NouveauDrawLine(render, 252, 174, 258, 290);

                    SDL_SetRenderDrawColor(render, 255, 0, 0, 255);
                    Background.NouveauDrawLine(render, 252, 174, 333, 176);
                    Background.NouveauDrawLine(render, 333, 176, 333, 225);
                    Background.NouveauDrawLine(render, 333, 225, 255, 229);

                    SDL_SetRenderDrawColor(render, 0, 0, 255, 255);
                    Background.NouveauDrawLine(render, 950, 100, 1081, 92);
                    Background.NouveauDrawLine(render, 1081, 92, 1082, 158);
                    Background.NouveauDrawLine(render, 1082, 158, 954, 163);

                    SDL_SetRenderDrawColor(render, 0, 255, 0, 255);
                    Background.NouveauDrawLine(render, 1644, 178, 1747, 172);
                    Background.NouveauDrawLine(render, 1747, 172, 1750, 230);
                    Background.NouveauDrawLine(render, 1750, 230, 1649, 235);
                    #endregion

                    #region étoiles
                    if (gTimer == 1560)
                    {
                        for (int i = 0; i < 50; i++)
                        {
                            int x = 151, y = 499;
                            while ((x > 150 && x < 375 && y < 500 && y > 280) ||
                                (x > 750 && x < 1175 && y < 650 && y > 230) ||
                                (x > 1550 && x < 1775 && y < 500 && y > 280))
                            {
                                x = RNG.Next(25, 1880);
                                y = RNG.Next(25, 680);
                            }
                            stars[i, 0] = (short)x;
                            stars[i, 1] = (short)y;

                        }
                    }
                    SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
                    for (int i = 0; i < 50; i++)
                    {
                        Background.NouveauDrawDot(render, stars[i, 0], stars[i, 1]);
                    }
                    #endregion

                    #region vols
                    SDL_SetRenderDrawColor(render, 255, 255, 0, 255);
                    short lz = (short)(gTimer - 1560);
                    Background.NouveauDrawDot(render, 304 + lz, 360);
                    Background.NouveauDrawDot(render, 631 - lz, 454);
                    Background.NouveauDrawDot(render, 872 + lz, 354);
                    Background.NouveauDrawDot(render, 1097 + lz, 479);
                    Background.NouveauDrawDot(render, 1300 - lz, 372);
                    Background.NouveauDrawDot(render, 1600 - lz, 418);
                    #endregion
                } // vols entre planètes - fini
                else if (gTimer >= 1740 && gTimer < 1920)
                {
                    Text.DisplayText("et même si le trou laissé par l'explosion laissera une empreinte\n" +
                                     "pour quelque temps...", 20, 700, 3, scroll: (int)(gTimer - 1740));
                    
                    #region étoiles
                    if (gTimer == 1740)
                    {
                        for (int i = 0; i < stars.GetLength(0); i++)
                        {
                            int x = RNG.Next(25, 1880), y = RNG.Next(25, 680);
                            while (Distance(x, y, W_SEMI_LARGEUR, W_SEMI_HAUTEUR / 2 + 100, 0.2f) < 200)
                            {
                                x = RNG.Next(25, 1880);
                                y = RNG.Next(25, 680);
                            }
                            stars[i, 0] = (short)x;
                            stars[i, 1] = (short)y;
                        }
                    }
                    SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
                    for (int i = 0; i < stars.GetLength(0); i++)
                    {
                        Background.NouveauDrawDot(render, stars[i, 0], stars[i, 1]);
                    }
                    #endregion

                    #region vols
                    SDL_SetRenderDrawColor(render, 255, 255, 0, 255);
                    short lz = (short)(gTimer - 1740);
                    Background.NouveauDrawDot(render, 152 + lz, 199);
                    Background.NouveauDrawDot(render, 556 - lz, 104 + lz);
                    Background.NouveauDrawDot(render, 257, 505 - lz);
                    Background.NouveauDrawDot(render, 1176 + lz, 643);
                    Background.NouveauDrawDot(render, 1690, 433 - lz);
                    Background.NouveauDrawDot(render, 1608 + lz, 214 + lz);
                    Background.NouveauDrawDot(render, 1429 + lz, 125);
                    Background.NouveauDrawDot(render, 626 - lz, 643);
                    Background.NouveauDrawDot(render, 1284 - lz, 176);
                    #endregion
                } // vols entre étoiles - fini
                else if (gTimer >= 1920 && gTimer < 2310)
                {
                    Text.DisplayText("même les plus grandes cicatrices se guérissent éventuellement.", 20, 700, 3, scroll:(int)(gTimer/2 - 960));

                    #region étoiles
                    if (gTimer == 1920)
                    {
                        for (int i = 0; i < stars.GetLength(0); i++)
                        {
                            int x = RNG.Next(25, 1880), y = RNG.Next(25, 680);
                            while (x > 442 && y > 76 && x < 1447 && y < 601)
                            {
                                x = RNG.Next(25, 1880);
                                y = RNG.Next(25, 680);
                            }
                            stars[i, 0] = (short)x;
                            stars[i, 1] = (short)y;
                        }
                        for (int i = 0; i < stars_glx.GetLength(0); i++)
                        {
                            if (Distance(stars_glx[i, 0], stars_glx[i, 1], 716, 437, 0.5f) < 100)
                            {
                                stars_glx[i, 0] = -1;
                                stars_glx[i, 1] = -1;
                            }
                        }
                    }
                    SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
                    for (int i = 0; i < stars.GetLength(0); i++)
                    {
                        Background.NouveauDrawDot(render, stars[i, 0], stars[i, 1]);
                    }
                    for (int i = 0; i < stars_glx.GetLength(0); i++)
                    {
                        Background.NouveauDrawDot(render, stars_glx[i, 0], stars_glx[i, 1]);
                    }
                    if (gTimer > 2070 && gTimer % 10 == 0)
                    {
                        for (int i = 0; i < stars_glx.GetLength(0); i++)
                        {
                            if (stars_glx[i, 0] == -1)
                            {
                                int x = RNG.Next(516, 916), y = RNG.Next(337, 537);
                                while (Distance(x, y, 716, 437, 0.5f) > 100)
                                {
                                    x = RNG.Next(516, 916);
                                    y = RNG.Next(337, 537);
                                }
                                stars_glx[i, 0] = (short)x;
                                stars_glx[i, 1] = (short)y;
                                break;
                            }
                        }
                    }
                    #endregion

                    #region galaxie
                    SDL_SetRenderDrawColor(render, 255, 255, 255, 60);
                    Background.NouveauDrawLine(render, 525, 362, 477, 195);
                    Background.NouveauDrawLine(render, 477, 195, 547, 223);
                    Background.NouveauDrawLine(render, 547, 223, 686, 91);
                    Background.NouveauDrawLine(render, 686, 91, 697, 141);
                    Background.NouveauDrawLine(render, 697, 141, 950, 79);
                    Background.NouveauDrawLine(render, 950, 79, 933, 126);
                    Background.NouveauDrawLine(render, 933, 126, 1230, 92);
                    Background.NouveauDrawLine(render, 1230, 92, 1191, 126);
                    Background.NouveauDrawLine(render, 1191, 126, 1404, 179);
                    Background.NouveauDrawLine(render, 1404, 179, 1344, 186);
                    Background.NouveauDrawLine(render, 1344, 186, 1434, 323);
                    Background.NouveauDrawLine(render, 1434, 323, 1370, 312);
                    Background.NouveauDrawLine(render, 1370, 312, 1404, 441);
                    Background.NouveauDrawLine(render, 1404, 441, 1354, 427);
                    Background.NouveauDrawLine(render, 1354, 427, 1285, 571);
                    Background.NouveauDrawLine(render, 1285, 571, 1285, 499);
                    Background.NouveauDrawLine(render, 1285, 499, 1017, 600);
                    Background.NouveauDrawLine(render, 1017, 600, 1032, 538);
                    Background.NouveauDrawLine(render, 1032, 538, 700, 600);
                    Background.NouveauDrawLine(render, 700, 600, 734, 553);
                    Background.NouveauDrawLine(render, 734, 553, 500, 500);
                    Background.NouveauDrawLine(render, 500, 500, 600, 500);
                    Background.NouveauDrawLine(render, 600, 500, 451, 377);
                    Background.NouveauDrawLine(render, 451, 377, 525, 362);

                    SDL_SetRenderDrawColor(render, 127, 0, 127, 255);
                    Background.NouveauDrawLine(render, 850, 350, 833, 306);
                    Background.NouveauDrawLine(render, 833, 306, 842, 271);
                    Background.NouveauDrawLine(render, 842, 271, 878, 221);
                    Background.NouveauDrawLine(render, 878, 221, 952, 193);
                    Background.NouveauDrawLine(render, 952, 193, 1027, 216);
                    Background.NouveauDrawLine(render, 1027, 216, 1064, 260);
                    Background.NouveauDrawLine(render, 1064, 260, 1076, 299);
                    Background.NouveauDrawLine(render, 1076, 299, 1070, 340);

                    Background.NouveauDrawLine(render, 828, 315, 798, 324);
                    Background.NouveauDrawLine(render, 798, 324, 800, 350);
                    Background.NouveauDrawLine(render, 800, 350, 831, 359);
                    Background.NouveauDrawLine(render, 831, 359, 1082, 347);
                    Background.NouveauDrawLine(render, 1082, 347, 1114, 333);
                    Background.NouveauDrawLine(render, 1114, 333, 1112, 309);
                    Background.NouveauDrawLine(render, 1112, 309, 1081, 304);
                    #endregion
                } // étoiles qui reviennent - fini

                if (gTimer > 30 && gTimer < 2340)
                {
                    SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
                    Background.NouveauDrawLine(render, 20, 20, 1900, 20);
                    Background.NouveauDrawLine(render, 1900, 20, 1900, 680);
                    Background.NouveauDrawLine(render, 1900, 680, 20, 680);
                    Background.NouveauDrawLine(render, 20, 20, 20, 680);
                }

                if (gTimer > 2400)
                {
                    gTimer_lock = false;
                    gamemode = 7;
                    player.scale = 1;
                    player.x = W_LARGEUR / 2;
                    player.y = W_HAUTEUR - 30;
                    player.roll = 0;
                    player.vy = -30;
                    gFade = 0;
                    enemies[0] = null;
                }
            }
            public static void Cut_3()
            {
                if (gTimer == 2 && cutscene_skip)
                    gTimer = 1830;

                if (gTimer == 60)
                {
                    PlaySong(Music_list.tbot);
                    player.dead = false;
                    //gTimer = 780;//
                }

                if (gTimer >= 60 && gTimer < 240)
                {
                    #region stars
                    if (gTimer == 60)
                    {
                        for (int i = 0; i < stars.GetLength(0); i++)
                        {
                            int x = 1500, y = 400;
                            while ((x > 1375 && y > 128 && x < 1676 && y < 402) || (x > 137 && y > 433 && x < 379 && y < 622))
                            {
                                x = RNG.Next(25, 1880);
                                y = RNG.Next(25, 680);
                            }
                            stars[i, 0] = (short)x;
                            stars[i, 1] = (short)y;
                        }
                    }
                    SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
                    for (int i = 0; i < stars.GetLength(0); i++)
                    {
                        Background.NouveauDrawDot(render, stars[i, 0], stars[i, 1]);
                    }
                    #endregion

                    #region player
                    player.x = 277;
                    player.y = 519;
                    player.pitch = 0.7f;
                    player.roll = 0.8f;
                    player.scale = 2;
                    player.Render();
                    #endregion

                    #region enemy 15
                    short death_timer = (short)(gTimer - 60);
                    if (death_timer > 255)
                        death_timer = 255;
                    player.scale = death_timer / 60f + 0.5f;
                    player.x = 771 - death_timer;
                    player.y = 325 + death_timer;
                    player.roll = death_timer / 250;
                    double sinroll = Sin(player.roll);
                    double cosroll = Cos(player.roll);
                    float pitchconst = player.pitch + P_PERMA_PITCH;
                    for (int i = 0; i < player.model.GetLength(0) - 1; i++)
                    {
                        if (i % 4 == 0)
                            SDL_SetRenderDrawColor(render, 255, 0, 0, (byte)(255 - death_timer - 60));
                        else
                            SDL_SetRenderDrawColor(render, 0, 0, 0, 255);
                        int[] pos = new int[4] {
                            (int)(player.scale * (cosroll * -player.model[i, 0] - sinroll * -player.model[i, 1]) + player.x),
                            (int)(player.scale * (sinroll * -player.model[i, 0] + cosroll * -player.model[i, 1]) + player.y - player.model[i, 2] * pitchconst),
                            (int)(player.scale * (cosroll * -player.model[i + 1, 0] - sinroll * -player.model[i + 1, 1]) + player.x),
                            (int)(player.scale * (sinroll * -player.model[i + 1, 0] + cosroll * -player.model[i + 1, 1]) + player.y - player.model[i + 1, 2] * pitchconst)
                        };
                        Background.NouveauDrawLine(render, pos[0], pos[1], pos[2], pos[3]);
                    }
                    #endregion

                    #region pulsar bomb
                    BombePulsar.DessinerBombePulsar(1522, 264, 133);

                    SDL_SetRenderDrawColor(render, 0, 0, 255, 255);
                    Background.NouveauDrawLine(render, 1463, 144, 1445, 97);
                    Background.NouveauDrawLine(render, 1445, 97, 1422, 68);
                    Background.NouveauDrawLine(render, 1422, 68, 1373, 46);
                    Background.NouveauDrawLine(render, 1525, 131, 1522, 86);
                    Background.NouveauDrawLine(render, 1522, 86, 1554, 48);
                    Background.NouveauDrawLine(render, 1554, 48, 1584, 35);
                    Background.NouveauDrawLine(render, 1501, 36, 1499, 82);
                    Background.NouveauDrawLine(render, 1464, 63, 1484, 112);
                    Background.NouveauDrawLine(render, 1500, 396, 1491, 454);
                    Background.NouveauDrawLine(render, 1491, 454, 1470, 493);
                    Background.NouveauDrawLine(render, 1470, 493, 1450, 534);
                    Background.NouveauDrawLine(render, 1549, 395, 1550, 450);
                    Background.NouveauDrawLine(render, 1550, 450, 1574, 486);
                    Background.NouveauDrawLine(render, 1574, 486, 1612, 513);
                    Background.NouveauDrawLine(render, 1514, 434, 1506, 485);
                    Background.NouveauDrawLine(render, 1539, 475, 1560, 520);
                    #endregion
                } // e15 tué - fini
                else if (gTimer >= 240 && gTimer < 421)
                {
                    #region alpha
                    short alpha = (short)(255 - (gTimer - 330) * 4);
                    if (alpha < 0)
                        alpha = 0;
                    #endregion

                    #region stars
                    if (gTimer == 240)
                    {
                        for (int i = 0; i < stars.GetLength(0); i++)
                        {
                            int x = 708, y = 108;
                            while (x > 707 && y > 107 && x < 1195 && y < 574)
                            {
                                x = RNG.Next(25, 1880);
                                y = RNG.Next(25, 680);
                            }
                            stars[i, 0] = (short)x;
                            stars[i, 1] = (short)y;
                        }
                    }
                    SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
                    for (int i = 0; i < stars.GetLength(0); i++)
                    {
                        Background.NouveauDrawDot(render, stars[i, 0], stars[i, 1]);
                    }
                    #endregion

                    #region pulsar bomb
                    if (gTimer < 300)
                        BombePulsar.DessinerBombePulsar(956, 336, 224);
                    else if (gTimer >= 300 && gTimer < 330)
                    {
                        SDL_SetRenderDrawColor(render, 200, 255, 255, 255);
                        DessinerCercle(956, 336, 224, 50);
                        if (gTimer % Ceiling((gTimer - 299) / 10f) == 0)
                            for (int i = 0; i < 50; i++)
                            {
                                float ang = RNG.NextSingle() * (float)PI;
                                neutron_slowdown[i, 0] = (short)(RNG.Next(-224, 224) * Cos(ang) + 956);
                                neutron_slowdown[i, 1] = (short)(RNG.Next(-224, 224) * Sin(ang) + 336);
                            }
                        for (int i = 0; i < 50; i++)
                            Background.NouveauDrawLine(render, neutron_slowdown[i, 0], neutron_slowdown[i, 1], 956, 336);
                    }
                    else if (gTimer >= 330)
                    {
                        DessinerCercle(956, 336, 224, 50);
                        SDL_SetRenderDrawColor(render, 200, 255, 255, (byte)alpha);
                        for (int i = 0; i < 50; i++)
                            Background.NouveauDrawLine(render, neutron_slowdown[i, 0], neutron_slowdown[i, 1], 956, 336);
                    }
                    #endregion

                    #region bleu
                    if (gTimer < 330)
                        SDL_SetRenderDrawColor(render, 0, 0, 255, 255);
                    else
                        SDL_SetRenderDrawColor(render, 0, 0, 255, (byte)alpha);

                    Background.NouveauDrawLine(render, 834, 148, 811, 86);
                    Background.NouveauDrawLine(render, 811, 86, 760, 50);
                    Background.NouveauDrawLine(render, 760, 50, 693, 21);
                    Background.NouveauDrawLine(render, 935, 113, 930, 61);
                    Background.NouveauDrawLine(render, 930, 61, 940, 21);
                    Background.NouveauDrawLine(render, 867, 112, 849, 64);
                    Background.NouveauDrawLine(render, 849, 64, 831, 37);
                    Background.NouveauDrawLine(render, 894, 57, 895, 21);
                    Background.NouveauDrawLine(render, 948, 560, 949, 614);
                    Background.NouveauDrawLine(render, 949, 614, 931, 679);
                    Background.NouveauDrawLine(render, 1041, 543, 1066, 591);
                    Background.NouveauDrawLine(render, 1066, 591, 1134, 632);
                    Background.NouveauDrawLine(render, 1134, 632, 1207, 654);
                    Background.NouveauDrawLine(render, 1032, 578, 1053, 614);
                    Background.NouveauDrawLine(render, 1053, 614, 1090, 648);
                    Background.NouveauDrawLine(render, 1000, 600, 1001, 640);
                    Background.NouveauDrawLine(render, 1001, 640, 989, 679);
                    #endregion
                } // bombe désactivée - fini
                else if (gTimer >= 421 && gTimer < 600)
                {
                    #region stars
                    if (gTimer == 421)
                    {
                        int x, y;
                        for (int i = 0; i < stars.GetLength(0); i++)
                        {
                            x = RNG.Next(25, 1880);
                            y = RNG.Next(25, 680);
                            stars[i, 0] = (short)x;
                            stars[i, 1] = (short)y;
                        }
                    }
                    SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
                    for (int i = 0; i < stars.GetLength(0); i++)
                    {
                        Background.NouveauDrawDot(render, stars[i, 0], stars[i, 1]);
                    }
                    #endregion

                    #region portal
                    SDL_SetRenderDrawColor(render, 0, 255, 255, 255);
                    for (int i = 0; i < 50; i++)
                    {
                        float ang = RNG.NextSingle() * (float)PI;
                        Background.NouveauDrawLine(render, (int)(RNG.Next(-150, 150) * Cos(ang) + 960), (int)(RNG.Next(-80, 80) * Sin(ang) + 130), 960, 130);
                    }
                    #endregion

                    #region amis
                    if (gTimer == 421)
                    {
                        for (byte i = 0; i < f_model_pos.GetLength(0); i++)
                        {
                            f_model_pos[i, 0] = 960 + (-3 + i);
                            f_model_pos[i, 1] = 130;
                            f_model_pos[i, 2] = 0;
                        }
                    }

                    for (byte i = 0; i < f_model_pos.GetLength(0); i++)
                    {
                        f_model_pos[i, 0] = (f_model_pos[i, 0] - 960) * 1.03f + 960;
                        if (i % 2 == 0)
                            f_model_pos[i, 1] *= 1.005f;
                        else
                            f_model_pos[i, 1] *= 1.008f;
                        f_model_pos[i, 2] += 0.01f;
                    }

                    SDL_SetRenderDrawColor(render, 255, 255, 0, 255);
                    for (byte i = 0; i < f_model_pos.GetLength(0); i++)
                    {
                        for (byte j = 0; j < f_model.GetLength(0) - 1; j++)
                        {
                            Background.NouveauDrawLine(render,
                                (int)(f_model[j, 0] * f_model_pos[i, 2] + f_model_pos[i, 0]),
                                (int)(f_model[j, 1] * f_model_pos[i, 2] + f_model_pos[i, 1]),
                                (int)(f_model[j+1, 0] * f_model_pos[i, 2] + f_model_pos[i, 0]),
                                (int)(f_model[j+1, 1] * f_model_pos[i, 2] + f_model_pos[i, 1]));
                        }
                    }
                    #endregion
                } // les amis viennent - dini
                else if (gTimer >= 600 && gTimer < 780)
                {
                    #region stars
                    if (gTimer == 600)
                    {
                        for (int i = 0; i < stars.GetLength(0); i++)
                        {
                            int x = 848, y = 63;
                            while ((x > 847 && y > 62 && x < 1250 && y < 448) || y > 448)
                            {
                                x = RNG.Next(25, 1880);
                                y = RNG.Next(25, 680);
                            }
                            stars[i, 0] = (short)x;
                            stars[i, 1] = (short)y;
                        }
                    }
                    SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
                    for (int i = 0; i < stars.GetLength(0); i++)
                    {
                        Background.NouveauDrawDot(render, stars[i, 0], stars[i, 1]);
                    }
                    #endregion

                    #region planète
                    SDL_SetRenderDrawColor(render, 255, 0, 0, 255);
                    Background.NouveauDrawLine(render, 21, 598, 200, 500);
                    Background.NouveauDrawLine(render, 200, 500, 598, 448);
                    Background.NouveauDrawLine(render, 598, 448, 1300, 448);
                    Background.NouveauDrawLine(render, 1300, 448, 1700, 500);
                    Background.NouveauDrawLine(render, 1700, 500, 1899, 600);
                    #endregion

                    #region vieu drapeau
                    SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
                    Background.NouveauDrawLine(render, 849, 448, 849, 62);

                    if (gTimer < 700)
                    {
                        short move = (short)((gTimer - 630) * 8);
                        if (move < 0)
                            move = 0;
                        if (move > 450)
                            move = 450;
                        SDL_SetRenderDrawColor(render, 255, 0, 0, 255);
                        Background.NouveauDrawLine(render, 850, 62 + move, 1247, 60 + move);
                        Background.NouveauDrawLine(render, 1247, 60 + move, 1247, 264 + move);
                        Background.NouveauDrawLine(render, 1247, 264 + move, 850, 273 + move);
                        Background.NouveauDrawLine(render, 850, 273 + move, 850, 62 + move);

                        SDL_SetRenderDrawColor(render, 0, 0, 255, 255);
                        Background.NouveauDrawLine(render, 848, 232 + move, 1191, 61 + move);
                        Background.NouveauDrawLine(render, 1246, 90 + move, 886, 272 + move);

                        SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
                        Background.NouveauDrawLine(render, 1177, 166 + move, 1200, 250 + move);
                        Background.NouveauDrawLine(render, 1200, 250 + move, 1136, 200 + move);
                        Background.NouveauDrawLine(render, 1136, 200 + move, 1215, 200 + move);
                        Background.NouveauDrawLine(render, 1215, 200 + move, 1150, 250 + move);
                        Background.NouveauDrawLine(render, 1150, 250 + move, 1177, 166 + move);
                    }
                    #endregion

                    #region nouveau drapeau
                    if (gTimer > 715)
                    {
                        short move = (short)((gTimer - 715) * 8);
                        if (move < 0)
                            move = 0;
                        if (move > 380)
                            move = 380;
                        SDL_SetRenderDrawColor(render, 255, 255, 0, 255);
                        Background.NouveauDrawLine(render, 850, 449 - move, 1251, 450 - move);
                        Background.NouveauDrawLine(render, 1251, 450 - move, 1256, 680 - move);
                        Background.NouveauDrawLine(render, 1256, 680 - move, 850, 680 - move);
                        Background.NouveauDrawLine(render, 850, 680 - move, 850, 449 - move);

                        SDL_SetRenderDrawColor(render, 255, 127, 0, 255);
                        DessinerCercle(1050, (short)(560 - move), 84, 50);
                        DessinerCercle(1050, (short)(560 - move), 63, 50);

                        SDL_SetRenderDrawColor(render, 0, 255, 0, 255);
                        Background.NouveauDrawLine(render, 850, 518 - move, 978, 518 - move);
                        Background.NouveauDrawLine(render, 976, 599 - move, 848, 599 - move);
                        Background.NouveauDrawLine(render, 1124, 521 - move, 1253, 519 - move);
                        Background.NouveauDrawLine(render, 1124, 600 - move, 1254, 600 - move);

                        SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
                        Background.NouveauDrawLine(render, 1050, 498 - move, 1090, 609 - move);
                        Background.NouveauDrawLine(render, 1090, 609 - move, 992, 536 - move);
                        Background.NouveauDrawLine(render, 992, 536 - move, 1106, 533 - move);
                        Background.NouveauDrawLine(render, 1106, 533 - move, 1017, 614 - move);
                        Background.NouveauDrawLine(render, 1017, 614 - move, 1050, 498 - move);
                    }
                    #endregion

                    if (gTimer == 600)
                        rect = new SDL_Rect() { x = 600, y = 449, w = 700, h = 500 };
                    SDL_SetRenderDrawColor(render, 0, 0, 0, 255);
                    SDL_RenderFillRect(render, ref rect);
                } // drapeau - fini
                else if (gTimer >= 780 && gTimer < 960)
                {
                    #region toi
                    SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
                    Background.NouveauDrawLine(render, 248, 680, 156, 309);
                    Background.NouveauDrawLine(render, 156, 309, 303, 243);
                    Background.NouveauDrawLine(render, 303, 243, 400, 300);
                    Background.NouveauDrawLine(render, 400, 300, 499, 249);
                    Background.NouveauDrawLine(render, 499, 249, 630, 310);
                    Background.NouveauDrawLine(render, 630, 310, 539, 680);

                    DessinerCercle(400, 200, 78, 50);

                    Background.NouveauDrawLine(render, 156, 309, 159, 649);
                    Background.NouveauDrawLine(render, 630, 310, 685, 215);
                    Background.NouveauDrawLine(render, 685, 215, 385, 140);

                    SDL_SetRenderDrawColor(render, 255, 255, 0, 255);
                    Background.NouveauDrawLine(render, 480, 380, 460, 400);
                    Background.NouveauDrawLine(render, 460, 400, 480, 420);
                    Background.NouveauDrawLine(render, 480, 420, 500, 400);
                    Background.NouveauDrawLine(render, 500, 400, 480, 380);

                    Background.NouveauDrawLine(render, 540, 380, 520, 400);
                    Background.NouveauDrawLine(render, 520, 400, 540, 420);
                    Background.NouveauDrawLine(render, 540, 420, 560, 400);
                    Background.NouveauDrawLine(render, 560, 400, 540, 380);

                    SDL_SetRenderDrawColor(render, 127, 0, 127, 255);
                    Background.NouveauDrawLine(render, 480, 380, 460, 320);
                    Background.NouveauDrawLine(render, 460, 320, 500, 320);
                    Background.NouveauDrawLine(render, 500, 320, 480, 380);

                    SDL_SetRenderDrawColor(render, 127, 255, 127, 255);
                    Background.NouveauDrawLine(render, 540, 380, 520, 320);
                    Background.NouveauDrawLine(render, 520, 320, 560, 320);
                    Background.NouveauDrawLine(render, 560, 320, 540, 380);
                    #endregion

                    #region chef
                    SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
                    Background.NouveauDrawLine(render, 865, 680, 800, 300);
                    Background.NouveauDrawLine(render, 800, 300, 879, 260);
                    Background.NouveauDrawLine(render, 879, 260, 959, 289);
                    Background.NouveauDrawLine(render, 959, 289, 1048, 254);
                    Background.NouveauDrawLine(render, 1048, 254, 1118, 295);
                    Background.NouveauDrawLine(render, 1118, 295, 1057, 680);

                    DessinerCercle(959, 205, 76, 50);

                    Background.NouveauDrawLine(render, 893, 166, 1031, 164);
                    Background.NouveauDrawLine(render, 1031, 164, 1018, 127);
                    Background.NouveauDrawLine(render, 1018, 127, 886, 114);
                    Background.NouveauDrawLine(render, 886, 114, 876, 132);
                    Background.NouveauDrawLine(render, 876, 132, 893, 166);

                    SDL_SetRenderDrawColor(render, 255, 255, 0, 255);
                    Background.NouveauDrawLine(render, 892, 133, 904, 123);
                    Background.NouveauDrawLine(render, 904, 123, 918, 133);
                    Background.NouveauDrawLine(render, 918, 133, 906, 145);
                    Background.NouveauDrawLine(render, 906, 145, 892, 133);

                    Background.NouveauDrawLine(render, 1020, 360, 1000, 380);
                    Background.NouveauDrawLine(render, 1000, 380, 1020, 400);
                    Background.NouveauDrawLine(render, 1020, 400, 1040, 380);
                    Background.NouveauDrawLine(render, 1040, 380, 1020, 360);

                    Background.NouveauDrawLine(render, 1080, 360, 1060, 380);
                    Background.NouveauDrawLine(render, 1060, 380, 1080, 400);
                    Background.NouveauDrawLine(render, 1080, 400, 1100, 380);
                    Background.NouveauDrawLine(render, 1100, 380, 1080, 360);

                    Background.NouveauDrawLine(render, 1000, 400, 1020, 420);
                    Background.NouveauDrawLine(render, 1020, 420, 1040, 400);
                    Background.NouveauDrawLine(render, 1040, 400, 1030, 390);
                    Background.NouveauDrawLine(render, 1010, 390, 1000, 400);

                    Background.NouveauDrawLine(render, 1070, 390, 1060, 400);
                    Background.NouveauDrawLine(render, 1060, 400, 1080, 420);
                    Background.NouveauDrawLine(render, 1080, 420, 1100, 400);
                    Background.NouveauDrawLine(render, 1100, 400, 1090, 390);

                    SDL_SetRenderDrawColor(render, 127, 0, 127, 255);
                    Background.NouveauDrawLine(render, 1000, 300, 1040, 300);
                    Background.NouveauDrawLine(render, 1040, 300, 1020, 360);
                    Background.NouveauDrawLine(render, 1020, 360, 1000, 300);

                    SDL_SetRenderDrawColor(render, 255, 0, 0, 255);
                    Background.NouveauDrawLine(render, 1060, 300, 1100, 300);
                    Background.NouveauDrawLine(render, 1100, 300, 1080, 360);
                    Background.NouveauDrawLine(render, 1080, 360, 1060, 300);

                    SDL_SetRenderDrawColor(render, 127, 127, 255, 255);
                    Background.NouveauDrawLine(render, 1000, 340, 1013, 340);
                    Background.NouveauDrawLine(render, 1027, 340, 1040, 340);
                    Background.NouveauDrawLine(render, 1040, 340, 1030, 370);
                    Background.NouveauDrawLine(render, 1011, 369, 1000, 340);

                    SDL_SetRenderDrawColor(render, 255, 127, 0, 255);
                    Background.NouveauDrawLine(render, 1060, 340, 1073, 340);
                    Background.NouveauDrawLine(render, 1087, 340, 1100, 340);
                    Background.NouveauDrawLine(render, 1100, 340, 1091, 371);
                    Background.NouveauDrawLine(render, 1071, 369, 1060, 340);

                    SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
                    Background.NouveauDrawLine(render, 800, 300, 553, 328);
                    Background.NouveauDrawLine(render, 531, 340, 1118, 295);
                    #endregion

                    #region drapeaux
                    for (short i = 0; i < 401; i += 200)
                    {
                        SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
                        Background.NouveauDrawLine(render, 1300 + i, 680, 1300 + i, 494);
                        Background.NouveauDrawLine(render, 1300 + i, 100, 1300 + i, 365);

                        SDL_SetRenderDrawColor(render, 255, 255, 0, 255);
                        Background.NouveauDrawLine(render, 1300 + i, 100, 1387 + i, 272);
                        Background.NouveauDrawLine(render, 1387 + i, 272, 1400 + i, 400);
                        Background.NouveauDrawLine(render, 1400 + i, 400, 1376 + i, 449);
                        Background.NouveauDrawLine(render, 1376 + i, 449, 1369 + i, 519);
                        Background.NouveauDrawLine(render, 1369 + i, 519, 1390 + i, 557);
                        Background.NouveauDrawLine(render, 1390 + i, 557, 1332 + i, 574);
                        Background.NouveauDrawLine(render, 1332 + i, 574, 1282 + i, 449);
                        Background.NouveauDrawLine(render, 1282 + i, 449, 1287 + i, 389);
                        Background.NouveauDrawLine(render, 1287 + i, 389, 1314 + i, 338);
                        Background.NouveauDrawLine(render, 1314 + i, 338, 1315 + i, 305);
                        Background.NouveauDrawLine(render, 1315 + i, 305, 1300 + i, 300);

                        SDL_SetRenderDrawColor(render, 255, 127, 0, 255);
                        DessinerCercle((short)(1346 + i), 375, 32, 50);

                        SDL_SetRenderDrawColor(render, 0, 255, 0, 255);
                        Background.NouveauDrawLine(render, 1368 + i, 352, 1371 + i, 309);
                        Background.NouveauDrawLine(render, 1371 + i, 309, 1346 + i, 253);
                        Background.NouveauDrawLine(render, 1346 + i, 253, 1300 + i, 170);
                        Background.NouveauDrawLine(render, 1339 + i, 343, 1340 + i, 308);
                        Background.NouveauDrawLine(render, 1340 + i, 308, 1321 + i, 268);
                        Background.NouveauDrawLine(render, 1321 + i, 268, 1300 + i, 249);
                        Background.NouveauDrawLine(render, 1325 + i, 400, 1311 + i, 437);
                        Background.NouveauDrawLine(render, 1311 + i, 437, 1318 + i, 488);
                        Background.NouveauDrawLine(render, 1318 + i, 488, 1351 + i, 568);
                        Background.NouveauDrawLine(render, 1375 + i, 561, 1350 + i, 500);
                        Background.NouveauDrawLine(render, 1350 + i, 500, 1339 + i, 441);
                        Background.NouveauDrawLine(render, 1339 + i, 441, 1351 + i, 407);
                    }
                    #endregion
                } // honneure - fini
                else if (gTimer >= 960 && gTimer < 1140)
                {
                    #region toi + chaise
                    SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
                    Background.NouveauDrawLine(render, 1034, 680, 1030, 521);
                    Background.NouveauDrawLine(render, 1030, 521, 1085, 470);
                    Background.NouveauDrawLine(render, 1085, 470, 1623, 474);
                    Background.NouveauDrawLine(render, 1623, 474, 1684, 523);
                    Background.NouveauDrawLine(render, 1684, 523, 1682, 680);

                    Background.NouveauDrawLine(render, 1512, 473, 1647, 110);
                    Background.NouveauDrawLine(render, 1647, 110, 1715, 80);
                    Background.NouveauDrawLine(render, 1715, 80, 1785, 117);
                    Background.NouveauDrawLine(render, 1785, 117, 1811, 165);
                    Background.NouveauDrawLine(render, 1811, 165, 1684, 523);

                    if (gTimer < 1050)
                    {
                        Background.NouveauDrawLine(render, 1500, 200, 1449, 203);
                        Background.NouveauDrawLine(render, 1449, 203, 1415, 472);
                        Background.NouveauDrawLine(render, 1513, 253, 1444, 505);
                        Background.NouveauDrawLine(render, 1444, 505, 1249, 508);
                        DessinerCercle(1555, 157, 70, 50);
                    }
                    else
                    {
                        Background.NouveauDrawLine(render, 1246, 471, 1199, 277);
                        Background.NouveauDrawLine(render, 1199, 277, 1400, 250);
                        Background.NouveauDrawLine(render, 1400, 250, 1448, 472);
                        Background.NouveauDrawLine(render, 1199, 277, 1180, 494);
                        Background.NouveauDrawLine(render, 1180, 494, 1078, 539);
                        Background.NouveauDrawLine(render, 1400, 250, 1482, 323);
                        Background.NouveauDrawLine(render, 1482, 323, 1494, 471);
                        DessinerCercle(1266, 220, 70, 50);
                    }
                    #endregion

                    #region fenêtre
                    SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
                    Background.NouveauDrawLine(render, 800, 100, 1050, 100);
                    Background.NouveauDrawLine(render, 1050, 100, 1050, 400);
                    Background.NouveauDrawLine(render, 1050, 400, 800, 400);
                    Background.NouveauDrawLine(render, 800, 400, 800, 100);
                    Background.NouveauDrawLine(render, 800, 250, 1050, 250);
                    Background.NouveauDrawLine(render, 925, 100, 925, 400);

                    if (gTimer == 960)
                    {
                        for (int i = 0; i < stars.GetLength(0) / 2; i++)
                        {
                            stars[i, 0] = (short)RNG.Next(800, 1050);
                            stars[i, 1] = (short)RNG.Next(100, 400);
                            neutron_slowdown[i, 0] = stars[i, 0];
                            neutron_slowdown[i, 1] = stars[i, 1];
                        }
                    }
                    SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
                    for (int i = 0; i < stars.GetLength(0) / 2; i++)
                    {
                        Background.NouveauDrawDot(render, stars[i, 0], stars[i, 1]);
                    }
                    #endregion

                    #region table + médailles
                    SDL_SetRenderDrawColor(render, 120, 50, 0, 255);
                    Background.NouveauDrawLine(render, 20, 200, 700, 200);
                    Background.NouveauDrawLine(render, 700, 200, 700, 250);
                    Background.NouveauDrawLine(render, 700, 250, 20, 250);

                    SDL_SetRenderDrawColor(render, 255, 127, 0, 255);
                    Background.NouveauDrawLine(render, 99, 219, 150, 219);
                    Background.NouveauDrawLine(render, 150, 219, 127, 300);
                    Background.NouveauDrawLine(render, 127, 300, 99, 219);

                    SDL_SetRenderDrawColor(render, 127, 255, 127, 255);
                    Background.NouveauDrawLine(render, 252, 221, 300, 221);
                    Background.NouveauDrawLine(render, 300, 221, 276, 301);
                    Background.NouveauDrawLine(render, 276, 301, 252, 221);

                    SDL_SetRenderDrawColor(render, 127, 0, 127, 255);
                    Background.NouveauDrawLine(render, 400, 223, 450, 223);
                    Background.NouveauDrawLine(render, 450, 223, 427, 301);
                    Background.NouveauDrawLine(render, 427, 301, 400, 223);

                    SDL_SetRenderDrawColor(render, 255, 255, 0, 255);
                    Background.NouveauDrawLine(render, 127, 300, 101, 323);
                    Background.NouveauDrawLine(render, 101, 323, 127, 348);
                    Background.NouveauDrawLine(render, 127, 348, 154, 324);
                    Background.NouveauDrawLine(render, 154, 324, 127, 300);

                    Background.NouveauDrawLine(render, 276, 301, 250, 325);
                    Background.NouveauDrawLine(render, 250, 325, 277, 349);
                    Background.NouveauDrawLine(render, 277, 349, 304, 325);
                    Background.NouveauDrawLine(render, 304, 325, 276, 301);

                    Background.NouveauDrawLine(render, 427, 301, 401, 324);
                    Background.NouveauDrawLine(render, 401, 324, 427, 349);
                    Background.NouveauDrawLine(render, 427, 349, 454, 325);
                    Background.NouveauDrawLine(render, 454, 325, 427, 301);
                    #endregion

                    #region trophés
                    for (short i = 0; i < 301; i += 150)
                    {
                        Background.NouveauDrawLine(render, 150 + i, 200, 166 + i, 171);
                        Background.NouveauDrawLine(render, 166 + i, 171, 221 + i, 172);
                        Background.NouveauDrawLine(render, 221 + i, 172, 238 + i, 200);
                        Background.NouveauDrawLine(render, 188 + i, 171, 188 + i, 154);
                        Background.NouveauDrawLine(render, 201 + i, 171, 201 + i, 154);
                        Background.NouveauDrawLine(render, 173 + i, 71, 161 + i, 124);
                        Background.NouveauDrawLine(render, 161 + i, 124, 176 + i, 154);
                        Background.NouveauDrawLine(render, 176 + i, 154, 215 + i, 154);
                        Background.NouveauDrawLine(render, 215 + i, 154, 227 + i, 128);
                        Background.NouveauDrawLine(render, 227 + i, 128, 218 + i, 71);
                        Background.NouveauDrawLine(render, 218 + i, 71, 173 + i, 71);
                        Background.NouveauDrawLine(render, 167 + i, 98, 151 + i, 97);
                        Background.NouveauDrawLine(render, 151 + i, 97, 144 + i, 119);
                        Background.NouveauDrawLine(render, 144 + i, 119, 166 + i, 133);
                        Background.NouveauDrawLine(render, 223 + i, 100, 237 + i, 97);
                        Background.NouveauDrawLine(render, 237 + i, 97, 245 + i, 122);
                        Background.NouveauDrawLine(render, 245 + i, 122, 224 + i, 136);
                    }
                    #endregion
                } // à la maison - fini
                else if (gTimer >= 1140 && gTimer < 1320)
                {
                    Explosion.Draw();

                    #region bordure
                    SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
                    Background.NouveauDrawLine(render, 750, 650, 750, 50);
                    Background.NouveauDrawLine(render, 750, 50, 1200, 50);
                    Background.NouveauDrawLine(render, 1200, 50, 1200, 650);
                    Background.NouveauDrawLine(render, 1200, 650, 750, 650);

                    Background.NouveauDrawLine(render, 975, 50, 975, 650);
                    Background.NouveauDrawLine(render, 750, 350, 1200, 350);

                    SDL_SetRenderDrawColor(render, 0, 0, 0, 255);
                    rect.x = 700; rect.y = 30; rect.w = 50; rect.h = 650;
                    SDL_RenderFillRect(render, ref rect);
                    rect.x = 1201; rect.y = 30; rect.w = 50; rect.h = 650;
                    SDL_RenderFillRect(render, ref rect);
                    rect.x = 700; rect.y = 30; rect.w = 500; rect.h = 20;
                    SDL_RenderFillRect(render, ref rect);
                    rect.x = 700; rect.y = 651; rect.w = 500; rect.h = 20;
                    SDL_RenderFillRect(render, ref rect);
                    #endregion

                    #region le reste
                    if (gTimer == 1140)
                    {
                        int x, y;
                        for (int i = 0; i < stars.GetLength(0); i++)
                        {
                            x = RNG.Next(750, 1200);
                            y = RNG.Next(50, 650);
                            stars[i, 0] = (short)x;
                            stars[i, 1] = (short)y;
                        }
                    }
                    SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
                    for (int i = 0; i < stars.GetLength(0); i++)
                    {
                        Background.NouveauDrawDot(render, stars[i, 0], stars[i, 1]);
                    }

                    if (lTimer == 0)
                    {
                        lTimer = (byte)RNG.Next(10, 30);
                        byte ripbozo;
                        encore:
                        ripbozo = (byte)RNG.Next(0, stars.GetLength(0));
                        if (stars[ripbozo, 0] == -1)
                            goto encore;
                        Explosion.Call(stars[ripbozo, 0], stars[ripbozo, 1], (byte)RNG.Next(5, 20));
                        if (gTimer % 3 == 0)
                        {
                            stars[ripbozo, 0] = -1;
                            stars[ripbozo, 1] = -1;
                        }
                    }
                    else
                        lTimer--;
                    #endregion
                } // big trouble in little window - fini
                else if (gTimer >= 1320 && gTimer < 1800)
                {
                    #region toi + chaise
                    SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
                    Background.NouveauDrawLine(render, 1034, 680, 1030, 521);
                    Background.NouveauDrawLine(render, 1030, 521, 1085, 470);
                    Background.NouveauDrawLine(render, 1085, 470, 1623, 474);
                    Background.NouveauDrawLine(render, 1623, 474, 1684, 523);
                    Background.NouveauDrawLine(render, 1684, 523, 1682, 680);

                    Background.NouveauDrawLine(render, 1512, 473, 1647, 110);
                    Background.NouveauDrawLine(render, 1647, 110, 1715, 80);
                    Background.NouveauDrawLine(render, 1715, 80, 1785, 117);
                    Background.NouveauDrawLine(render, 1785, 117, 1811, 165);
                    Background.NouveauDrawLine(render, 1811, 165, 1684, 523);

                    Background.NouveauDrawLine(render, 1318, 225, 1408, 244);
                    Background.NouveauDrawLine(render, 1408, 244, 1490, 321);
                    Background.NouveauDrawLine(render, 1490, 321, 1527, 433);
                    Background.NouveauDrawLine(render, 1272, 317, 1313, 375);
                    Background.NouveauDrawLine(render, 1313, 375, 1342, 473);
                    Background.NouveauDrawLine(render, 1390, 323, 1296, 531);
                    Background.NouveauDrawLine(render, 1296, 531, 1270, 266);
                    Background.NouveauDrawLine(render, 1290, 342, 1228, 527);
                    Background.NouveauDrawLine(render, 1228, 527, 1212, 256);
                    DessinerCercle(1256, 251, 67, 50);
                    #endregion

                    #region fenêtre
                    if (gTimer == 1320)
                        Explosion.Clear();

                    Explosion.Draw();

                    SDL_SetRenderDrawColor(render, 0, 0, 0, 255);
                    rect.x = 750; rect.y = 30; rect.w = 50; rect.h = 350;
                    SDL_RenderFillRect(render, ref rect);
                    rect.x = 1051; rect.y = 30; rect.w = 50; rect.h = 350;
                    SDL_RenderFillRect(render, ref rect);
                    rect.x = 750; rect.y = 80; rect.w = 300; rect.h = 20;
                    SDL_RenderFillRect(render, ref rect);
                    rect.x = 750; rect.y = 401; rect.w = 300; rect.h = 20;
                    SDL_RenderFillRect(render, ref rect);

                    SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
                    Background.NouveauDrawLine(render, 800, 100, 1050, 100);
                    Background.NouveauDrawLine(render, 1050, 100, 1050, 400);
                    Background.NouveauDrawLine(render, 1050, 400, 800, 400);
                    Background.NouveauDrawLine(render, 800, 400, 800, 100);

                    Background.NouveauDrawLine(render, 800, 250, 1050, 250);
                    Background.NouveauDrawLine(render, 925, 100, 925, 400);

                    if (gTimer == 1320)
                    {
                        for (byte i = 0; i < stars.GetLength(0) / 2; i++)
                        {
                            stars[i, 0] = neutron_slowdown[i, 0];
                            stars[i, 1] = neutron_slowdown[i, 1];
                        }
                    }
                    SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
                    for (int i = 0; i < stars.GetLength(0) / 2; i++)
                    {
                        Background.NouveauDrawDot(render, stars[i, 0], stars[i, 1]);
                    }

                    if (lTimer == 0 && gTimer < 1800)
                    {
                        lTimer = (byte)RNG.Next(30, 50);
                        byte ripbozo;
                        encore2:
                        ripbozo = (byte)RNG.Next(0, stars.GetLength(0) / 2);
                        if (stars[ripbozo, 0] == -1)
                            goto encore2;
                        Explosion.Call(stars[ripbozo, 0], stars[ripbozo, 1], (byte)RNG.Next(10, 18));
                        if (gTimer % 3 == 0)
                        {
                            stars[ripbozo, 0] = -1;
                            stars[ripbozo, 1] = -1;
                        }
                    }
                    else
                        lTimer--;
                    #endregion

                    #region table + médailles
                    SDL_SetRenderDrawColor(render, 120, 50, 0, 255);
                    Background.NouveauDrawLine(render, 20, 200, 700, 200);
                    Background.NouveauDrawLine(render, 700, 200, 700, 250);
                    Background.NouveauDrawLine(render, 700, 250, 20, 250);

                    SDL_SetRenderDrawColor(render, 255, 127, 0, 255);
                    Background.NouveauDrawLine(render, 99, 219, 150, 219);
                    Background.NouveauDrawLine(render, 150, 219, 127, 300);
                    Background.NouveauDrawLine(render, 127, 300, 99, 219);

                    SDL_SetRenderDrawColor(render, 127, 255, 127, 255);
                    Background.NouveauDrawLine(render, 252, 221, 300, 221);
                    Background.NouveauDrawLine(render, 300, 221, 276, 301);
                    Background.NouveauDrawLine(render, 276, 301, 252, 221);

                    SDL_SetRenderDrawColor(render, 127, 0, 127, 255);
                    Background.NouveauDrawLine(render, 400, 223, 450, 223);
                    Background.NouveauDrawLine(render, 450, 223, 427, 301);
                    Background.NouveauDrawLine(render, 427, 301, 400, 223);

                    SDL_SetRenderDrawColor(render, 255, 255, 0, 255);
                    Background.NouveauDrawLine(render, 127, 300, 101, 323);
                    Background.NouveauDrawLine(render, 101, 323, 127, 348);
                    Background.NouveauDrawLine(render, 127, 348, 154, 324);
                    Background.NouveauDrawLine(render, 154, 324, 127, 300);

                    Background.NouveauDrawLine(render, 276, 301, 250, 325);
                    Background.NouveauDrawLine(render, 250, 325, 277, 349);
                    Background.NouveauDrawLine(render, 277, 349, 304, 325);
                    Background.NouveauDrawLine(render, 304, 325, 276, 301);

                    Background.NouveauDrawLine(render, 427, 301, 401, 324);
                    Background.NouveauDrawLine(render, 401, 324, 427, 349);
                    Background.NouveauDrawLine(render, 427, 349, 454, 325);
                    Background.NouveauDrawLine(render, 454, 325, 427, 301);
                    #endregion

                    #region trophés
                    for (short i = 0; i < 301; i += 150)
                    {
                        Background.NouveauDrawLine(render, 150 + i, 200, 166 + i, 171);
                        Background.NouveauDrawLine(render, 166 + i, 171, 221 + i, 172);
                        Background.NouveauDrawLine(render, 221 + i, 172, 238 + i, 200);
                        Background.NouveauDrawLine(render, 188 + i, 171, 188 + i, 154);
                        Background.NouveauDrawLine(render, 201 + i, 171, 201 + i, 154);
                        Background.NouveauDrawLine(render, 173 + i, 71, 161 + i, 124);
                        Background.NouveauDrawLine(render, 161 + i, 124, 176 + i, 154);
                        Background.NouveauDrawLine(render, 176 + i, 154, 215 + i, 154);
                        Background.NouveauDrawLine(render, 215 + i, 154, 227 + i, 128);
                        Background.NouveauDrawLine(render, 227 + i, 128, 218 + i, 71);
                        Background.NouveauDrawLine(render, 218 + i, 71, 173 + i, 71);
                        Background.NouveauDrawLine(render, 167 + i, 98, 151 + i, 97);
                        Background.NouveauDrawLine(render, 151 + i, 97, 144 + i, 119);
                        Background.NouveauDrawLine(render, 144 + i, 119, 166 + i, 133);
                        Background.NouveauDrawLine(render, 223 + i, 100, 237 + i, 97);
                        Background.NouveauDrawLine(render, 237 + i, 97, 245 + i, 122);
                        Background.NouveauDrawLine(render, 245 + i, 122, 224 + i, 136);
                    }
                    #endregion

                    #region citation
                    if (gTimer > 1520)
                        Text.DisplayText("\"c'est un jeu bizarre, la guerre.", 20, 700, 3, scroll: (ushort)(gTimer - 1520));
                    if (gTimer > 1580)
                        Text.DisplayText(" la seule facon de gagner est de ne pas jouer.\"", 20, 739, 3, scroll: (ushort)(gTimer - 1580));
                    if (gTimer > 1670)
                        Text.DisplayText("- w.o.p.r.", 20, 778, 3, scroll: (ushort)(gTimer - 1670));
                    #endregion
                } // la guerre c mauvais - fini

                if (gTimer > 30 && gTimer < 1830)
                {
                    SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
                    Background.NouveauDrawLine(render, 20, 20, 1900, 20);
                    Background.NouveauDrawLine(render, 1900, 20, 1900, 680);
                    Background.NouveauDrawLine(render, 1900, 680, 20, 680);
                    Background.NouveauDrawLine(render, 20, 20, 20, 680);
                }

                if (gTimer > 1860)
                {
                    gTimer_lock = false;
                    gamemode = 7;
                    player.scale = 1;
                    player.x = W_LARGEUR / 2;
                    player.y = W_HAUTEUR - 30;
                    player.roll = 0;
                    player.vy = -30;
                    gFade = 0;
                    enemies[0] = null;
                }
            }
            public static void Cut_4()
            {
                if (gTimer == 2 && cutscene_skip)
                    gTimer = 2601;

                if (gTimer == 60)
                {
                    PlaySong(Music_list.eugenesis);
                    player.HP = 1;
                    player.dead = false;
                    //gTimer = 2200;//
                }
                // beats: 60, 300, 540, 780, 1020, 1260, 1500, 1740, 1980, 2240
                //gTimer += 2;//

                #region texte
                if (gTimer >= 60 && gTimer < 500)
                {
                    byte alpha = 255;
                    if (gTimer >= 60 && gTimer < 145)
                        alpha = (byte)((gTimer - 60) * 3);
                    short extra_y = 0;
                    if (gTimer >= 300)
                        extra_y = (short)(gTimer - 300);
                    Text.DisplayText("dysgenesis", short.MinValue, (short)(540 - extra_y * 3), 4, alpha: alpha, scroll: (int)((gTimer - 60) / 5));
                }
                if (gTimer >= 300 && gTimer < 700)
                    Text.DisplayText("conception:\nmalcolm gauthier", 100, (short)(W_HAUTEUR - (gTimer - 300) * 3), 4, scroll: ((int)gTimer - 330) / 5);
                if (gTimer >= 540 && gTimer < 940)
                    Text.DisplayText("         modèles:\nmalcolm gauthier", 1300, (short)(W_HAUTEUR - (gTimer - 540) * 3), 4, scroll: ((int)gTimer - 560) / 5);
                if (gTimer >= 780 && gTimer < 1180)
                    Text.DisplayText("programmation:\nmalcolm gauthier", 100, (short)(W_HAUTEUR - (gTimer - 780) * 3), 4, scroll: ((int)gTimer - 800) / 5);
                if (gTimer >= 1020 && gTimer < 1420)
                    Text.DisplayText(" effets sonnores:\nmalcolm gauthier", 1300, (short)(W_HAUTEUR - (gTimer - 1020) * 3), 4, scroll: ((int)gTimer - 1040) / 5);
                if (gTimer >= 1260 && gTimer < 1660)
                    Text.DisplayText("musique", short.MinValue, (short)(W_HAUTEUR - (gTimer - 1260) * 3), 4, scroll: ((int)gTimer - 1280) / 5);
                if (gTimer >= 1300 && gTimer < 1700)
                {
                    Text.DisplayText("\"dance of the violins\"", short.MinValue, (short)(W_HAUTEUR - (gTimer - 1300) * 3), 3, scroll: ((int)gTimer - 1320) / 5);
                    Text.DisplayText("jesse valentine (f-777)", short.MinValue, (short)(W_HAUTEUR - (gTimer - 1320) * 3), 3, scroll: ((int)gTimer - 1320) / 5);
                }
                if (gTimer >= 1400 && gTimer < 1800)
                {
                    Text.DisplayText("\"240 bits per mile\"", short.MinValue, (short)(W_HAUTEUR - (gTimer - 1400) * 3), 3, scroll: ((int)gTimer - 1420) / 5);
                    Text.DisplayText("leon riskin", short.MinValue, (short)(W_HAUTEUR - (gTimer - 1420) * 3), 3, scroll: ((int)gTimer - 1420) / 5);
                }
                if (gTimer >= 1500 && gTimer < 1900)
                {
                    Text.DisplayText("\"dysgenesis\"         \"eugenesis\"", short.MinValue, (short)(W_HAUTEUR - (gTimer - 1500) * 3), 3, scroll: ((int)gTimer - 1520) / 3);
                    Text.DisplayText("malcolm gauthier", short.MinValue, (short)(W_HAUTEUR - (gTimer - 1520) * 3), 3, scroll: ((int)gTimer - 1520) / 5);
                }
                if (gTimer >= 1600 && gTimer < 2000)
                {
                    Text.DisplayText("autres musiques", short.MinValue, (short)(W_HAUTEUR - (gTimer - 1600) * 3), 3, scroll: ((int)gTimer - 1620) / 5);
                    Text.DisplayText("malcolm gauthier, mélodies non-originales", short.MinValue, (short)(W_HAUTEUR - (gTimer - 1620) * 3), 3, scroll: ((int)gTimer - 1620) / 3);
                }
                if (gTimer >= 1740 && gTimer < 2140)
                    Text.DisplayText("mélodies utilisées", 100, (short)(W_HAUTEUR - (gTimer - 1740) * 3), 4, scroll: ((int)gTimer - 1740) / 5);
                if (gTimer >= 1780 && gTimer < 2180)
                {
                    Text.DisplayText("\"can't remove the pain\"", 400, (short)(W_HAUTEUR - (gTimer - 1780) * 3), 3, scroll: ((int)gTimer - 1800) / 5);
                    Text.DisplayText("todd porter et herman miller", 400, (short)(W_HAUTEUR - (gTimer - 1800) * 3), 3, scroll: ((int)gTimer - 1800) / 5);
                }
                if (gTimer >= 1880 && gTimer < 2280)
                {
                    Text.DisplayText("\"pesenka\"", 400, (short)(W_HAUTEUR - (gTimer - 1880) * 3), 3, scroll: ((int)gTimer - 1900) / 5);
                    Text.DisplayText("Sergey Zhukov et Aleksey Potekhin", 400, (short)(W_HAUTEUR - (gTimer - 1900) * 3), 3, scroll: ((int)gTimer - 1900) / 5);
                }
                if (gTimer >= 1980 && gTimer < 2380)
                {
                    Text.DisplayText("\"the beginning of time\"", 400, (short)(W_HAUTEUR - (gTimer - 1980) * 3), 3, scroll: ((int)gTimer - 2000) / 5);
                    Text.DisplayText("nathan ingalls (dj-nate)", 400, (short)(W_HAUTEUR - (gTimer - 2000) * 3), 3, scroll: ((int)gTimer - 2000) / 5);
                }
                if (gTimer >= 2240)
                {
                    byte alpha = 0;
                    if (gTimer < 2350)
                        alpha = (byte)((gTimer - 2250) * 2.5f);
                    else
                        alpha = 255;
                    Text.DisplayText("fin", short.MinValue, short.MinValue, 5, alpha: alpha);
                    if (gTimer > 2350)
                        Text.DisplayText("tapez \"arcade\" au menu du jeu pour accéder au mode arcade!", 20, 1050, 2, alpha: (short)((gTimer - 2350) * 10));
                }
                #endregion

                #region ennemis & joueur
                if (gTimer >= 400 && gTimer < 500)
                {
                    player.scale = 10 * (float)Pow(0.95, gTimer - 400);
                    player.x = (gTimer - 400) * 4 + 1000;
                    player.y = (float)Pow((int)gTimer - 450, 2) * -0.1f + 600;
                    player.pitch = (gTimer - 400) / -333f;
                    if (gTimer >= 460 && gTimer < 480)
                        player.roll = (gTimer - 440) * (float)(PI / 10) + 0.3f;
                    else
                        player.roll = (500 - gTimer) * (float)(PI / 300);
                    if (gTimer >= 490)
                        player.fade = (byte)(255 - (gTimer - 490) * 25);
                    player.Render();
                }
                else if (gTimer >= 600 && gTimer < 900)
                {
                    if (gTimer == 600)
                    {
                        enemies[0] = new Enemy(5);
                        enemies[1] = new Enemy(1);
                        enemies[2] = new Enemy(2);
                        for (byte i = 0; i < 3; i++)
                        {
                            enemies[i].depth = -5;
                        }
                    }
                    for (byte i = 0; i < 3; i++)
                    {
                        enemies[i].x = (600 - (int)gTimer) * 8.2f + 2000 + i * 200;
                        enemies[i].y = (float)Sin((gTimer - 600) / -10f + i) * 50 + W_SEMI_HAUTEUR;
                    }
                    if (gTimer == 899)
                    {
                        for (byte i = 0; i < 3; i++)
                        {
                            enemies[i] = null;
                        }
                    }
                }
                else if (gTimer >= 1000 && gTimer < 1200)
                {
                    for (byte i = 0; i < 2; i++)
                    {
                        if (gTimer == 1000)
                        {
                            enemies[i] = new Enemy(7);
                            enemies[i].depth = -7;
                        }
                        if (gTimer > 1150)
                        {
                            if (i == 0)
                                enemies[i].x = 300 - (float)Pow((gTimer - 1150), 1.9);
                            else
                                enemies[i].x = 1000 + (float)Pow((gTimer - 1150), 1.9);
                            enemies[i].y = 800 - (float)Pow(gTimer - 1150, 2);
                        }
                        else
                        {
                            enemies[i].x = 300 + i * 700;
                            enemies[i].y = (gTimer - 1000) * -3 + 1250;
                        }
                        if (gTimer == 1199)
                            enemies[i] = null;
                    }
                }
                else if (gTimer >= 1300 && gTimer < 1610)
                {
                    for (byte i = 0; i < 2; i++)
                    {
                        if (gTimer == 1300)
                        {
                            enemies[i] = new Enemy(4);
                            enemies[i].depth = -5;
                        }
                        enemies[i].x = 200 + i * 1500 + (gTimer - 1300);
                        enemies[i].y = (gTimer - 1300) * -4 + 1150 + i * 200;
                        if (gTimer == 1609)
                            enemies[i] = null;
                    }
                }
                else if (gTimer >= 1620 && gTimer < 1830)
                {
                    if (gTimer == 1620)
                    {
                        enemies[0] = new Enemy(3);
                        enemies[0].depth = 10;
                    }

                    enemies[0].x = 1000;
                    enemies[0].y = (gTimer - 1620) * -3 + 1150;

                    if (gTimer == 1829)
                    {
                        Explosion.Call((short)enemies[0].x, (short)enemies[0].y, 5);
                        enemies[0] = null;
                    }
                }
                else if (gTimer >= 1900 && gTimer < 2000)
                {
                    if (gTimer == 1900)
                    {
                        enemies[0] = new Enemy(6);
                        enemies[0].depth = -5;
                        enemies[0].pitch = -0.6f;
                    }
                    enemies[0].roll = 0.5f * (float)Sin(gTimer / 6f);
                    enemies[0].y = (gTimer - 1900) * -3f + 1100;
                    enemies[0].x = (gTimer - 1900) * -8 + 1950;
                    if (gTimer == 1999)
                    {
                        enemies[1] = new Enemy(6);
                        enemies[1].pitch = -0.6f;
                        enemies[0].depth = 5;
                        enemies[1].depth = 5;
                    }
                }
                else if (gTimer > 2240)
                {
                    player.scale = 1;
                    player.roll = (float)Sin(gTimer / 8f) / 5f;
                    player.pitch = 0.3f;
                    player.x = W_SEMI_LARGEUR;
                    player.y = W_SEMI_HAUTEUR + 100;
                    byte alpha = 0;
                    if (gTimer < 2350)
                        alpha = (byte)((gTimer - 2250) * 2.5f);
                    else
                        alpha = 255;
                    player.fade = alpha;
                    player.Render();
                }

                if (gTimer >= 1600 && gTimer < 1750)
                {
                    if (gTimer == 1600)
                        enemies[2] = new Enemy(15);
                    enemies[2].depth = (short)(40 - Pow((gTimer - 1600) / 33f, 3));
                    enemies[2].pitch = (gTimer - 1600) / 200f;
                    enemies[2].roll = (1600 - (int)gTimer) * (float)(PI / 600);
                    if (gTimer < 1650)
                        enemies[2].x = 1500 + (gTimer - 1600) * 10;
                    else
                        enemies[2].x = -600 + (gTimer - 1600) * 10;
                    enemies[2].y = 200 * (float)Sin((gTimer - 1600) / 50f) + W_SEMI_HAUTEUR;
                    enemies[2].alpha = gTimer < 1620 ? (byte)((gTimer - 1600) * 12f) : (byte)255;
                    if (gTimer == 1749)
                        enemies[2] = null;
                }
                else if (gTimer >= 1770 && gTimer < 1850)
                {
                    if (gTimer == 1770)
                    {
                        player.scale = 2;
                        player.pitch = 0.3f;
                        player.roll = -0.7f;
                        player.fade = 255;
                        player.y = 750;
                    }
                    player.x = 2050 - (gTimer - 1770) * 30;
                    player.Render();

                    if (gTimer == 1800)
                    {
                        int[] shoot_point1 = player.RenderCalc(1);
                        int[] shoot_point2 = player.RenderCalc(16);
                        Projectile.pos[0, 0] = shoot_point1[0];
                        Projectile.pos[0, 1] = shoot_point1[1];
                        Projectile.pos[0, 2] = 1;
                        Projectile.pos[0, 3] = Projectile.pos[0, 0] - 200;
                        Projectile.pos[0, 4] = Projectile.pos[0, 1] - 120;
                        Projectile.pos[0, 5] = 10;
                        Projectile.pos[1, 0] = shoot_point2[0];
                        Projectile.pos[1, 1] = shoot_point2[1];
                        Projectile.pos[1, 2] = 1;
                        Projectile.pos[1, 3] = Projectile.pos[0, 0] - 200;
                        Projectile.pos[1, 4] = Projectile.pos[0, 1] - 120;
                        Projectile.pos[1, 5] = 10;
                    }
                }
                else if (gTimer >= 1999 && gTimer < 2200)
                {
                    for (byte i = 0; i < 2; i++)
                    {
                        enemies[i].roll = 0.5f * (float)Sin(gTimer / 6f);
                        enemies[i].x = (gTimer - 1900) * -8 + 1950;
                    }
                    enemies[0].y = (gTimer - 1900) * -3f + 1100 + (float)Sqrt(gTimer - 1999) * 18;
                    enemies[1].y = (gTimer - 1900) * -3f + 1100 - (float)Sqrt(gTimer - 1999) * 18;
                    if (gTimer == 2199)
                    {
                        enemies[0] = null;
                        enemies[1] = null;
                    }
                }

                for (byte i = 0; i < 3; i++)
                {
                    if (enemies[i] != null)
                    {
                        enemies[i].RenderEnemy();
                        enemies[i].UpdateModele();
                    }
                }
                Explosion.Draw();
                if (gTimer >= 1800 && gTimer < 1830)
                {
                    SDL_SetRenderDrawColor(render, 255, 0, 0, 255);
                    int[] pos = Projectile.CalcDepths(0);
                    Background.NouveauDrawLine(render, pos[0], pos[1], pos[2], pos[3]);
                    pos = Projectile.CalcDepths(1);
                    Background.NouveauDrawLine(render, pos[0], pos[1], pos[2], pos[3]);
                    Projectile.pos[0, 2]++;
                    Projectile.pos[1, 2]++;
                }
                #endregion

                #region gfade
                if (gTimer > 2550)
                    gFade = (byte)((gTimer - 2550) * 5);
                if (gFade != 0)
                {
                    rect.x = 0; rect.y = 0; rect.w = W_LARGEUR; rect.h = W_HAUTEUR;
                    SDL_SetRenderDrawColor(render, 0, 0, 0, gFade);
                    SDL_RenderFillRect(render, ref rect);
                }
                #endregion

                #region générique
                // générique

                // fait par:
                // Malcolm Gauthier

                // modèles:
                // Malcolm Gauthier

                // programmation:
                // Malcolm Gauthier

                // effets sonores:
                // Malcolm Gauthier


                // musique:

                // "Dance of the Violins"
                // Jesse Valentine (F-777)

                // "240 Bits Per Mile"
                // Leon Riskin

                // "Dysgenesis"
                // Malcolm Gauthier

                // "Eugenesis"
                // Malcolm Gauthier

                // Autres
                // Malcolm Gauthier, mélodies non-originales


                // mélodies utilisées:

                // "Can't remove the pain"
                // Todd Porter
                // Herman Miller

                // "Pesenka"
                // Sergey Zhukov
                // Aleksey Potekhin

                // "The beginning of Time"
                // Nathan Ingalls (DJ-Nate)
                #endregion

                if (gTimer > 2600)
                {
                    gTimer_lock = false;
                    gamemode = 0;
                    player.scale = 1;
                    player.x = -1;
                    player.y = W_HAUTEUR - 30;
                    player.roll = 0;
                    player.vy = -30;
                    player.fade = 255;
                    gFade = 0;
                    BombePulsar.HP_bombe = BP_MAX_HP;
                }
            }
        }
        public static class Curseur
        {
            public static byte selected = 1;
            public static sbyte returned_sel = -1;
            public static byte max_selection = 1;
            public static short x0, y0;
            public static byte gap;
            public static void SetVars(byte new_max_selection, short new_x0, short new_y0, byte new_gap)
            {
                x0 = new_x0;
                y0 = new_y0;
                gap = new_gap;
                max_selection = new_max_selection;
                returned_sel = -1;
            }
            public static void Exist()
            {
                Move();
                Render();
            }
            public static void Move()
            {
                if (gTimer > 15)
                {
                    if (Keys.w || Keys.s || Keys.j) gTimer = 0;
                    if (Keys.w && selected != 1) selected--;
                    if (Keys.s && selected < max_selection) selected++;
                    if (Keys.j) returned_sel = (sbyte)selected;
                }
                if (!Keys.w && !Keys.s && gTimer < 50) gTimer = 50;
            }
            public static void Render()
            {
                SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
                NouveauDrawLine(render, x0 - 15, y0 - 15 + gap * selected, x0 + 15, y0 + gap * selected);
                NouveauDrawLine(render, x0 + 15, y0 + gap * selected, x0 - 15, y0 + 15 + gap * selected);
                NouveauDrawLine(render, x0 - 15, y0 + 15 + gap * selected, x0 - 12, y0 + gap * selected);
                NouveauDrawLine(render, x0 - 12, y0 + gap * selected, x0 - 15, y0 - 15 + gap * selected);
            }
        }
        public static class Explosion
        {
            static short[,] pos = new short[20,3];
            public static void Init()
            {
                for (int i = 0; i < pos.GetLength(0); i++)
                    pos[i, 0] = -1;
            }
            public static void Call(short x, short y, byte z)
            {
                for (int i = 0; i < pos.GetLength(0); i++)
                {
                    if (pos[i, 0] == -1)
                    {
                        pos[i, 0] = x;
                        pos[i, 1] = y;
                        pos[i, 2] = z;
                        break;
                    }
                }
            }
            public static void Draw()
            {
                for (int i = 0; i < pos.GetLength(0); i++)
                {
                    if (pos[i, 0] != -1)
                    {
                        byte r = (byte)(100f / (pos[i, 2] + 1) + 5);
                        for (int j = pos[i, 2]; j < 50; j++)
                        {
                            float ang = RNG.NextSingle() * (float)PI;
                            SDL_SetRenderDrawColor(render, (byte)(RNG.Next(0, 128) + 127), (byte)RNG.Next(0, 127), 0, 255);
                            if (gamemode >= 4 || gamemode == 1)
                                NouveauDrawLine(render, (int)(RNG.Next(-r, r) * Cos(ang) + pos[i, 0]), (int)(RNG.Next(-r, r) * Sin(ang) + pos[i, 1]),
                                    (int)(RNG.Next(-r, r) * Cos(ang) + pos[i, 0]), (int)(RNG.Next(-r, r) * Sin(ang) + pos[i, 1]));
                            else
                                SDL_RenderDrawLine(render, (int)(RNG.Next(-r, r) * Cos(ang) + pos[i, 0]), (int)(RNG.Next(-r, r) * Sin(ang) + pos[i, 1]), 
                                    (int)(RNG.Next(-r, r) * Cos(ang) + pos[i, 0]), (int)(RNG.Next(-r, r) * Sin(ang) + pos[i, 1]));
                        }

                        pos[i, 2]++;

                        if (pos[i, 2] >= 20)
                            pos[i, 0] = -1;
                    }
                }
            }
            public static void Clear()
            {
                for (byte i = 0; i < pos.GetLength(0); i++)
                {
                    pos[i, 0] = -1;
                }
            }
        }
        public static class Son
        {
            static AudioFileReader crtp;
            static AudioFileReader presents;
            static AudioFileReader title;
            static AudioFileReader atw;
            static AudioFileReader level;
            static AudioFileReader shoot;
            static AudioFileReader e_boom;
            static AudioFileReader powerup;
            static AudioFileReader p_boom;
            static AudioFileReader wave;
            static AudioFileReader tbot;
            static AudioFileReader dotv;
            static AudioFileReader ton;
            static AudioFileReader eugensesis;
            static AudioFileReader dcq_pbm;
            static AudioFileReader stone;
            public static void LoadMusic()
            {
                try
                {
                    crtp =       new AudioFileReader(@"audio\cant remove the pain.wav");
                    presents =   new AudioFileReader(@"audio\presents.wav");
                    title =      new AudioFileReader(@"audio\titlescreen.wav");
                    atw =        new AudioFileReader(@"audio\around the world.wav");
                    level =      new AudioFileReader(@"audio\sfx1.wav");
                    shoot =      new AudioFileReader(@"audio\laserShoot.wav");
                    e_boom =     new AudioFileReader(@"audio\explosion_enemy.wav");
                    powerup =    new AudioFileReader(@"audio\powerUp.wav");
                    p_boom =     new AudioFileReader(@"audio\explosion.wav");
                    wave =       new AudioFileReader(@"audio\synth.wav");
                    tbot =       new AudioFileReader(@"audio\The beginning of Time.wav");
                    dotv =       new AudioFileReader(@"audio\Dance of the Violins.wav");
                    ton =        new AudioFileReader(@"audio\tone.wav");
                    eugensesis = new AudioFileReader(@"audio\eugenesis.wav");
                    dcq_pbm =    new AudioFileReader(@"audio\240 Bits Per Mile.wav");
                    stone =      new AudioFileReader(@"audio\Stone Cold.wav");
                }
                catch (Exception ex)
                {
                    CrashReport(ex);
                }
            }
            public static void LoadSFX()
            {
                try
                {
                    sfx[1].Init(level);
                    sfx[2].Init(shoot);
                    sfx[3].Init(e_boom);
                    sfx[4].Init(powerup);
                    sfx[5].Init(p_boom);
                    sfx[6].Init(wave);
                    sfx[7].Init(ton);
                }
                catch (Exception ex)
                {
                    CrashReport(ex);
                }
            }
            public enum SFX_list
            {
                level = 1,
                shoot,
                e_boom,
                powerup,
                p_boom,
                wave,
                ton
            }
            public enum Music_list
            {
                presents = 1,
                title,
                crtp,
                atw,
                tbot,
                dotv,
                eugenesis,
                dcq_pbm,
                stone
            }
            public static void StopMusic()
            {
                try
                {
                    bg_music.Stop();
                    cut_music.Stop();
                }
                catch (Exception)
                {
                    throw; // up
                }
            }
            public static void PlaySong(Music_list track)
            {
                if (mute_music)
                    return;
                switch (track)
                {
                    case Music_list.presents:
                        presents.Position = 0;
                        cut_music.Init(presents);
                        cut_music.Play();
                        break;
                    case Music_list.title:
                        title.Position = 0;
                        bg_music.Init(title);
                        bg_music.Play();
                        break;
                    case Music_list.crtp:
                        crtp.Position = 0;
                        cut_music.Init(crtp);
                        cut_music.Play();
                        break;
                    case Music_list.atw:
                        atw.Position = 0;
                        cut_music.Init(atw);
                        cut_music.Play();
                        break;
                    case Music_list.tbot:
                        tbot.Position = 0;
                        cut_music.Init(tbot);
                        cut_music.Play();
                        break;
                    case Music_list.dotv:
                        dotv.Position = 0;
                        bg_music.Init(dotv);
                        bg_music.Play();
                        break;
                    case Music_list.eugenesis:
                        eugensesis.Position = 0;
                        cut_music.Init(eugensesis);
                        cut_music.Play();
                        break;
                    case Music_list.dcq_pbm:
                        dcq_pbm.Position = 0;
                        bg_music.Init(dcq_pbm);
                        bg_music.Play();
                        break;
                    case Music_list.stone:
                        stone.Position = 0;
                        bg_music.Init(stone);
                        bg_music.Play();
                        break;
                }
            }
            public static void PlaySFX(SFX_list track)
            {
                switch (track)
                {
                    case SFX_list.level:
                        level.Position = 0;
                        sfx[1].Play();
                        break;
                    case SFX_list.shoot:
                        shoot.Position = 0;
                        sfx[2].Play();
                        break;
                    case SFX_list.e_boom:
                        e_boom.Position = 0;
                        sfx[3].Play();
                        break;
                    case SFX_list.powerup:
                        powerup.Position = 0;
                        sfx[4].Play();
                        break;
                    case SFX_list.p_boom:
                        p_boom.Position = 0;
                        sfx[5].Play();
                        break;
                    case SFX_list.wave:
                        wave.Position = 0;
                        sfx[6].Play();
                        break;
                    case SFX_list.ton:
                        ton.Position = 0;
                        sfx[7].Play();
                        break;
                }
            }
            public static void LoopBGSong(Music_list track)
            {
                if (bg_music.PlaybackState == PlaybackState.Stopped && !mute_music)
                {
                    PlaySong(track);
                    return;
                }
            }
            public static void BaisserVolume()
            {
                if (volume > 0)
                {
                    volume--;
                    bg_music.Volume = (volume / 16f);
                    cut_music.Volume = (volume / 16f);
                    sfx[1].Volume = (volume / 16f);
                    sfx[2].Volume = (volume / 16f);
                    sfx[3].Volume = (volume / 16f);
                    sfx[4].Volume = (volume / 16f);
                    sfx[5].Volume = (volume / 16f);
                    sfx[6].Volume = (volume / 16f);
                    sfx[7].Volume = (volume / 16f);
                }
            }
            public static void AugmenterVolume()
            {
                if (volume < 16)
                {
                    volume++;
                    bg_music.Volume = (volume / 16f);
                    cut_music.Volume = (volume / 16f);
                    sfx[1].Volume = (volume / 16f);
                    sfx[2].Volume = (volume / 16f);
                    sfx[3].Volume = (volume / 16f);
                    sfx[4].Volume = (volume / 16f);
                    sfx[5].Volume = (volume / 16f);
                    sfx[6].Volume = (volume / 16f);
                    sfx[7].Volume = (volume / 16f);
                }
            }
        }
        public static int NouveauDrawLine(IntPtr render, int x1, int y1, int x2, int y2)
        {
            double facteur_largeur = W_LARGEUR / 1920.0;
            double facteur_hauteur = W_HAUTEUR / 1080.0;
            return SDL_RenderDrawLine(render, (int)(x1 * facteur_largeur), (int)(y1 * facteur_hauteur), (int)(x2 * facteur_largeur), (int)(y2 * facteur_hauteur));
        }
        public static int NouveauDrawDot(IntPtr render, int x, int y)
        {
            double facteur_largeur = W_LARGEUR / 1920.0;
            double facteur_hauteur = W_HAUTEUR / 1080.0;
            return SDL_RenderDrawPoint(render, (int)(x * facteur_largeur), (int)(y * facteur_hauteur));
        }
        public static int NouveauDrawBox(IntPtr render, ref SDL_Rect rect)
        {
            double facteur_largeur = W_LARGEUR / 1920.0;
            double facteur_hauteur = W_HAUTEUR / 1080.0;
            SDL_Rect unref_rect = rect;
            unref_rect.x = (int)(rect.x * facteur_largeur);
            unref_rect.w = (int)(rect.w * facteur_largeur);
            unref_rect.y = (int)(rect.y * facteur_hauteur);
            unref_rect.h = (int)(rect.h * facteur_hauteur);
            return SDL_RenderFillRect(render, ref unref_rect);
        }
    }
}