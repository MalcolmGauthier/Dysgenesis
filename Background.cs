using static SDL2.SDL;
using static SDL2.SDL_mixer;
using static System.MathF;
#pragma warning disable CA1806 // c# me demande de regarder le résultat de SDL_RenderDrawLine lolololololol

namespace Dysgenesis
{
    public enum ListeAudioEffets
    {
        PRESENTE,
        NIVEAU,
        TIR,
        EXPLOSION_ENNEMI,
        EXPLOSION_JOUEUR,
        POWERUP,
        VAGUE,
        DOTV_ENTREE,
    };
    public enum ListeAudioMusique
    {
        ATW,
        CRTP,
        DYSGENESIS,
        TBOT,
        DOTV,
        EUGENESIS,
        DCQBPM
    }
    public enum CutsceneIndex
    {
        STARTUP,
        INTRO,
        GOOD_END,
        BAD_END,
        CREDITS
    }

    // fonction générales. avant était une classe pour encapsuler tout dans ce fichier
    // todo: se débarrasser de cette classe en mettant les fonctions dans program ou qqc d'autre
    public static class Background
    {
        // Dessine un polygone avec X côtés pour créér une approximation de cercle.
        public static void DessinerCercle(Vector2 position, int taille, byte cotes)
        {
            float ang;
            float next_ang;

            for (int i = 0; i < cotes; i++)
            {
                // Tau = 2*Pi
                ang = (i * Tau) / cotes;
                next_ang = ((i + 1) * Tau) / cotes;

                SDL_RenderDrawLineF(Program.render,
                    position.x + taille * Sin(ang),
                    position.y + taille * Cos(ang),
                    position.x + taille * Sin(next_ang),
                    position.y + taille * Cos(next_ang)
                );
            }
        }

        // = +sqrt(w(a²)+h(b²))
        // todo: déplacer dans vector2
        public static int Distance(float x1, float y1, float x2, float y2, float mult_x = 1, float mult_y = 1)
        {
            return (int)Sqrt(mult_x * Pow(Abs(x1 - x2), 2) + mult_y * Pow(Abs(y1 - y2), 2));
        }
        public static int Distance(float x1, float y1, float x2, float y2)
        {
            return Distance(x1, y1, x2, y2, 1, 1);
        }
    }

    // classe statique qui gère la bombe pulsar, ou n'importe quoi qui à a faire avec
    // la bombe pulsar
    // TODO: rendre un sprite
    public class BombePulsar : Sprite
    {
        const byte QUANTITE_RAYONS_BOMBE_PULSAR = 50;
        public const int BOMBE_PULSAR_MAX_HP = 50;
        public static readonly SDL_Color COULEUR_BOMBE = new SDL_Color() { r = 150, g = 255, b = 255, a = 255 };
        readonly static float[] hyperbole_bleue_bombe_pulsar_data = new float[72] {
            -0.35f, -0.95f, -0.4f, -1.55f,
            -0.4f, -1.55f, -0.6f, -2.15f,
            -0.6f, -2.15f, -0.9f, -2.35f,
            0.2f, -1.0f, 0.25f, -1.55f,
            0.25f, -1.55f, 0.35f, -2.25f,
            0.35f, -2.25f, 0.6f, -2.45f,
            -0.15f, -1.15f, -0.15f, -1.8f,
            0.0f, -1.75f, 0.2f, -2.25f,
            -0.25f, -2.25f, -0.4f, -2.45f,
            -0.3f, 0.95f, -0.45f, 1.5f,
            -0.45f, 1.5f, -0.65f, 1.95f,
            -0.65f, 1.95f, -1.0f, 2.2f,
            0.25f, 0.95f, 0.3f, 1.5f,
            0.3f, 1.5f, 0.5f, 2.0f,
            0.5f, 2.0f, 1.0f, 2.3f,
            0.0f, 1.2f, 0.05f, 1.8f,
            -0.1f, 1.55f, -0.15f, 1.95f,
            -0.4f, 1.75f, -0.25f, 1.3f
        };

        public short HP_bombe = BOMBE_PULSAR_MAX_HP;

        // Dessine la forme de la bombe à pulsar, qui est un cercle avec des lignes qui sortent au hasard
        // de son centre. est utilisé par la bombe, des ennemis, et autres
        public static void DessinerBombePulsar(Vector2 position, byte rayon, SDL_Color couleure, bool hyperbole_bleue, Vector2[]? lignes_prefaites)
        {
            // dessine les lignes bleues en haut et en bas de la bombe qui ont l'air d'une hyperboloide
            if (hyperbole_bleue)
            {
                SDL_SetRenderDrawColor(Program.render, 0, 0, 255, 255);

                for (int i = 0; i < hyperbole_bleue_bombe_pulsar_data.Length; i += 4)
                {
                    SDL_RenderDrawLine(Program.render,
                        (int)(hyperbole_bleue_bombe_pulsar_data[i + 0] * rayon + position.x),
                        (int)(hyperbole_bleue_bombe_pulsar_data[i + 1] * rayon + position.y),
                        (int)(hyperbole_bleue_bombe_pulsar_data[i + 2] * rayon + position.x),
                        (int)(hyperbole_bleue_bombe_pulsar_data[i + 3] * rayon + position.y)
                    );
                }
            }

            SDL_SetRenderDrawColor(Program.render, couleure.r, couleure.g, couleure.b, couleure.a);
            Background.DessinerCercle(position, rayon, 50);

            // dans une des scènes, la bombe doit ralentir puis s'éteindre, et
            // c'est la seule fois que cette section est utilisée
            if (lignes_prefaites != null)
            {
                for (int i = 0; i < QUANTITE_RAYONS_BOMBE_PULSAR; i++)
                {
                    SDL_RenderDrawLine(Program.render,
                        (int)lignes_prefaites[i].x,
                        (int)lignes_prefaites[i].y,
                        (int)position.x,
                        (int)position.y
                    );
                }

                return;
            }

            // dessine les lignes à l'intérieur du cercle au hasard
            float angle;
            for (int i = 0; i < QUANTITE_RAYONS_BOMBE_PULSAR; i++)
            {
                angle = Program.RNG.NextSingle() * PI;
                SDL_RenderDrawLineF(Program.render,
                    Program.RNG.Next(-rayon, rayon) * Cos(angle) + position.x,
                    Program.RNG.Next(-rayon, rayon) * Sin(angle) + position.y,
                    position.x,
                    position.y
                );
            }
        }
        public static void DessinerBombePulsar(Vector2 position, byte rayon, SDL_Color couleure, bool hyperbole_bleue)
        {
            DessinerBombePulsar(position, rayon, couleure, hyperbole_bleue, null);
        }
        public static void DessinerBombePulsar(Vector2 position, byte rayon, bool hyperbole_bleue)
        {
            DessinerBombePulsar(position, rayon, COULEUR_BOMBE, hyperbole_bleue, null);
        }

        // animation d'explosion de la bombe
        // retourne 1 si animation en cours
        public int AnimationExplosion()
        {
            if (HP_bombe > 0)
                return 0;

            timer++;

            if (timer == 1)
            {
                Mix_HaltMusic();
                Son.JouerEffet(ListeAudioEffets.EXPLOSION_JOUEUR);

                // assure que le joueur est incapable de mourrir durant l'animation d'explosion
                Program.projectiles.Clear();
                if (Program.player.HP <= 0)
                    Program.player.HP = 1;
            }
            else if (timer < 200)
            {
                // fade vers blanc
                byte opacite = (byte)Math.Clamp(timer * 4, byte.MinValue, byte.MaxValue);

                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, opacite);
                SDL_RenderFillRect(Program.render, IntPtr.Zero);
            }
            else
            {
                timer = 0;
                Program.Gamemode = Gamemode.CUTSCENE_GOOD_END;
                HP_bombe = BOMBE_PULSAR_MAX_HP;
            }

            return 1;
        }

        public void VerifCollision()
        {
            // ce code roule seulement si niveau 20 et monologue fini
            if (Program.enemies[0].statut != StatusEnnemi.BOSS_NORMAL)
                return;

            for (int i = 0; i < Program.projectiles.Count; i++)
            {
                if (Program.projectiles[i].position.z < Data.G_MAX_DEPTH - 1)
                    continue;

                float[] positions = Program.projectiles[i].PositionsSurEcran();
                if (Background.Distance(positions[2], positions[3], Data.W_SEMI_LARGEUR, Data.W_SEMI_HAUTEUR / 2) < 20)
                {
                    HP_bombe--;
                    new Explosion(new Vector3(positions[2], positions[3], Data.G_MAX_DEPTH / 4));
                    // TODO: séparer code render de code logique
                    // rend la bombe rouge pour 1 image quand elle est frappée
                    DessinerBombePulsar(
                        new Vector2(Data.W_SEMI_LARGEUR, Data.W_SEMI_HAUTEUR / 2),
                        20,
                        new SDL_Color() { r = 255, g = 0, b = 0, a = 255 },
                        false
                    );
                }
            }
        }

        public override bool Exist()
        {
            if (AnimationExplosion() != 0)
                VerifCollision();

            return false;
        }
    }
    public static class Etoiles
    {
        const int DENSITE_ETOILES = 100;
        const int RAYON_DEBUTE_ETOILES = 100;
        const float VITESSE_ETOILES = 1.02f;

        public static Vector2[] star_positions = new Vector2[DENSITE_ETOILES];
        public static void Spawn(SDL_Rect bounds, short limite = DENSITE_ETOILES)
        {
            if (limite > DENSITE_ETOILES)
                limite = DENSITE_ETOILES;

            for (int i = 0; i < limite; i++)
            {
                star_positions[i].x = Program.RNG.Next(bounds.x, bounds.x + bounds.w);
                star_positions[i].y = Program.RNG.Next(bounds.y, bounds.y + bounds.h);
            }
        }
        public static void Move()
        {
            for (int i = 0; i < DENSITE_ETOILES; i++)
            {
                star_positions[i].x = (star_positions[i].x - Data.W_SEMI_LARGEUR) * VITESSE_ETOILES + Data.W_SEMI_LARGEUR;
                star_positions[i].y = (star_positions[i].y - Data.W_SEMI_HAUTEUR) * VITESSE_ETOILES + Data.W_SEMI_HAUTEUR;

                if (star_positions[i].x >= Data.W_LARGEUR || star_positions[i].x <= 0 || star_positions[i].y >= Data.W_HAUTEUR || star_positions[i].y <= 0)
                {
                    star_positions[i].x = Program.RNG.Next(Data.W_SEMI_LARGEUR - RAYON_DEBUTE_ETOILES, Data.W_SEMI_LARGEUR + RAYON_DEBUTE_ETOILES);
                    star_positions[i].y = Program.RNG.Next(Data.W_SEMI_HAUTEUR - RAYON_DEBUTE_ETOILES, Data.W_SEMI_HAUTEUR + RAYON_DEBUTE_ETOILES);

                    if (star_positions[i].x == Data.W_SEMI_LARGEUR && star_positions[i].y == Data.W_SEMI_HAUTEUR)
                        star_positions[i].x++;
                }
            }
        }
        public static void Render(short limite)
        {
            if (limite > Data.S_DENSITY)
                limite = Data.S_DENSITY;

            SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);

            for (int i = 0; i < limite; i++)
                SDL_RenderDrawPointF(Program.render, star_positions[i].x, star_positions[i].y);
        }
    }
    public static class Text
    {
        // documentation texte todo: update
        // 
        //   text: le texte qui sera affiché à l'écran
        //         charactères supportés: a-z, 0-9, +, -, é, è, ê, à,  , ., ,, ', :, \, /, ", (, ), \n
        //         lettres sont majuscule seuelement, mais le texte qui rentre dans la fonction doit être minuscule, les majuscules seront automatiquement
        //         convertis en minuscules avant d'êtres déssinés.
        //         \n fonctionne et est la seule facon de passer à une prochaine ligne dans le même appel de texte, et quand la ligne est sauté, il revient
        //         au x de départ.
        //
        //   x, y: position haut gauche du premier charactère affiché.
        //         mettre Text.CENTRE (ou -2147483648) va centrer le texte au millieu de l'écran.
        //
        //   size: nombre qui donne le multiple de la largeure et hauteure.
        //         la largeur d'un charactère sera de 5 * size, et la hauteur de 10 * size.
        //
        //  color: couleure RGB du texte, où blanc est la valeure par défaut
        //         R, G et B attachés ensemble en un chiffre, où les bits 23 à 16 sont pour le rouge, 15 à 8 pour le vert et 7 à 0 pour le bleu.
        //
        //  alpha: transparence du texte, 100% opaque par défaut. Ceci sera la valeure A du RGBA de sdl, et va automatiquement
        //         arrondir à une valeure de byte si dépassement.
        //
        // scroll: le nb. de charactères que seront affichés à l'écran, peu importe la longeure du texte.
        //         int.MaxValue par défaut. si scroll est négatif, aucun texte n'est affiché.
        //
        //
        public const int CENTRE = int.MinValue;
        public const int NO_SCROLL = int.MaxValue;
        public const int BLANC = 0xFFFFFF;
        public const int ROUGE = 0xFF0000;
        public const int VERT = 0x00FF00;
        public const int BLEU = 0x0000FF;
        public const int NOIR = 0x000000;
        public const int OPAQUE = 255;

        const float LARGEUR_DEFAUT = 5.0f;
        const float HAUTEUR_DEFAUT = 10.0f;
        const float ESPACE_DEFAUT = 3.0f;

        static readonly short[] char_draw_info_starting_indexes = {
            1, 18, 47, 60, 81, 98, 111, 132, 145, 158, 171, 184, 193, 210, 223, 240, // a - p
	        257, 278, 299, 320, 329, 342, 351, 368, 377, 386, 399, 420, 433, 454, 471, // q - 4
	        484, 513, 534, 543, 564, 585, 590, 599, 604, 609, 630, 651, 676, 697, 706, // 5 - ??
	        711, 716, 721, 734, 747
        };
        static readonly sbyte[] char_draw_info =
        {
            127, // space/unknown

	        0, 10, 0, 0, // a
	        0, 0, 5, 0,
            5, 0, 5, 10,
            5, 5, 0, 5, 127,

            0, 10, 0, 0, // b
	        0, 0, 5, 0,
            5, 0, 5, 3,
            5, 3, 0, 5,
            0, 5, 5, 7,
            5, 7, 5, 10,
            5, 10, 0, 10, 127,

            0, 10, 0, 0, // c
	        0, 0, 5, 0,
            5, 10, 0, 10, 127,

            0, 10, 0, 0, // d
	        0, 0, 2, 0,
            2, 10, 0, 10,
            2, 10, 5, 5,
            2, 0, 5, 5, 127,

            0, 10, 0, 0, // e
	        0, 0, 5, 0,
            5, 10, 0, 10,
            5, 5, 0, 5, 127,

            0, 10, 0, 0, // f
	        0, 0, 5, 0,
            5, 5, 0, 5, 127,

            0, 10, 0, 0, // g
	        0, 0, 5, 0,
            5, 10, 0, 10,
            5, 10, 5, 5,
            5, 5, 3, 5, 127,

            0, 10, 0, 0, // h
	        5, 0, 5, 10,
            5, 5, 0, 5, 127,

            0, 0, 5, 0, // i
	        2, 0, 2, 10,
            5, 10, 0, 10, 127,

            5, 0, 5, 10, // j
	        5, 10, 0, 10,
            0, 10, 0, 7, 127,

            0, 10, 0, 0, // k
	        0, 5, 5, 0,
            0, 5, 5, 10, 127,

            0, 10, 0, 0, // l
	        5, 10, 0, 10, 127,

            0, 10, 0, 0, // m
	        5, 0, 5, 10,
            0, 0, 2, 5,
            5, 0, 2, 5, 127,

            0, 10, 0, 0, // n
	        5, 0, 5, 10,
            0, 0, 5, 10, 127,

            0, 10, 0, 0, // o
	        5, 0, 5, 10,
            0, 0, 5, 0,
            5, 10, 0, 10, 127,

            0, 10, 0, 0, // p
	        0, 0, 5, 0,
            5, 0, 5, 5,
            5, 5, 0, 5, 127,

            0, 10, 0, 0, // q
	        4, 0, 4, 10,
            0, 0, 4, 0,
            4, 10, 0, 10,
            5, 10, 3, 5, 127,

            0, 10, 0, 0, // r
	        0, 0, 5, 0,
            5, 0, 5, 5,
            5, 5, 0, 5,
            2, 5, 5, 10, 127,

            0, 0, 5, 0, // s
	        5, 10, 0, 10,
            5, 5, 0, 5,
            0, 0, 0, 5,
            5, 10, 5, 5, 127,

            2, 0, 2, 10, // t
	        0, 0, 5, 0, 127,

            0, 10, 0, 0, // u
	        5, 0, 5, 10,
            5, 10, 0, 10, 127,

            0, 0, 2, 10, // v
	        5, 0, 2, 10, 127,

            0, 10, 0, 0, // w
	        5, 0, 5, 10,
            5, 10, 0, 10,
            2, 10, 2, 4, 127,

            0, 0, 5, 10, // x
	        5, 0, 0, 10, 127,

            5, 0, 0, 10, // y
	        0, 0, 2, 5, 127,

            0, 0, 5, 0, // z
	        5, 10, 0, 10,
            5, 0, 0, 10, 127,

            5, 0, 0, 10, // 0
	        0, 10, 0, 0,
            5, 0, 5, 10,
            0, 0, 5, 0,
            5, 10, 0, 10, 127,

            0, 3, 2, 0, // 1
	        2, 0, 2, 10,
            5, 10, 0, 10, 127,

            0, 3, 1, 0, // 2
	        1, 0, 4, 0,
            5, 3, 4, 0,
            5, 3, 0, 10,
            5, 10, 0, 10, 127,

            5, 10, 5, 0, // 3
	        0, 0, 5, 0,
            5, 10, 0, 10,
            5, 5, 0, 5, 127,

            0, 5, 0, 0, // 4
	        5, 0, 5, 10,
            5, 5, 0, 5, 127,

            5, 0, 0, 0, // 5
	        0, 5, 0, 0,
            5, 5, 0, 5,
            5, 5, 5, 8,
            5, 8, 4, 10,
            4, 10, 1, 10,
            1, 10, 0, 8, 127,

            0, 10, 0, 0, // 6
	        0, 0, 5, 0,
            5, 10, 0, 10,
            5, 10, 5, 5,
            5, 5, 0, 5, 127,

            5, 0, 0, 0, // 7
	        5, 0, 0, 10, 127,

            5, 5, 0, 5, // 8
	        0, 10, 0, 0,
            5, 0, 5, 10,
            0, 0, 5, 0,
            5, 10, 0, 10, 127,

            5, 10, 5, 0, // 9
	        0, 0, 5, 0,
            5, 10, 0, 10,
            0, 0, 0, 5,
            5, 5, 0, 5, 127,

            0, 10, 0, 10, 127, // .

	        0, 3, 0, 3, // :
	        0, 7, 0, 7, 127,

            2, 8, 0, 10, 127, // ,

	        2, 2, 0, 0, 127, // '

	        0, 10, 0, 0, // é
	        0, 0, 5, 0,
            5, 10, 0, 10,
            5, 5, 0, 5,
            1, -2, 4, -4, 127,

            0, 10, 0, 0, // è
	        0, 0, 5, 0,
            5, 10, 0, 10,
            5, 5, 0, 5,
            1, -4, 4, -2, 127,

            0, 10, 0, 0, // ê
	        0, 0, 5, 0,
            5, 10, 0, 10,
            5, 5, 0, 5,
            1, -2, 2, -4,
            4, -2, 2, -4, 127,

            0, 10, 0, 0, // à
	        0, 0, 5, 0,
            5, 0, 5, 10,
            5, 5, 0, 5,
            1, -2, 4, 0, 127,

            2, 0, 2, 3, // "
	        4, 0, 4, 3, 127,

            1, 5, 4, 5, 127, // -

	        0, 10, 5, 0, 127, // /

	        5, 10, 0, 0, 127, // \

	        4, 0, 2, 3, // (
	        2, 3, 2, 7,
            4, 10, 2, 7, 127,

            1, 0, 3, 3, // )
	        3, 3, 3, 7,
            1, 10, 3, 7, 127,

            0, 5, 4, 5, // +
	        2, 3, 2, 7, 127
        };

        private static int GetListEntry(char c)
        {
            if (c >= 'a' && c <= 'z')
                return char_draw_info_starting_indexes[c - 'a'];
            else if (c >= '0' && c <= '9')
                return char_draw_info_starting_indexes[c - '0' + 26];
            else if (c == '.')
                return char_draw_info_starting_indexes[36];
            else if (c == ':')
                return char_draw_info_starting_indexes[37];
            else if (c == ',')
                return char_draw_info_starting_indexes[38];
            else if (c == '\'')
                return char_draw_info_starting_indexes[39];
            else if (c == 'é')
                return char_draw_info_starting_indexes[40];
            else if (c == 'è')
                return char_draw_info_starting_indexes[41];
            else if (c == 'ê')
                return char_draw_info_starting_indexes[42];
            else if (c == 'à')
                return char_draw_info_starting_indexes[43];
            else if (c == '"')
                return char_draw_info_starting_indexes[44];
            else if (c == '-')
                return char_draw_info_starting_indexes[45];
            else if (c == '/')
                return char_draw_info_starting_indexes[46];
            else if (c == '\\')
                return char_draw_info_starting_indexes[47];
            else if (c == '(')
                return char_draw_info_starting_indexes[48];
            else if (c == ')')
                return char_draw_info_starting_indexes[49];
            else if (c == '+')
                return char_draw_info_starting_indexes[50];
            else
                return 0;
        }
        public static void DisplayText(string text, Vector2 position, float size,
            int color = BLANC, short alpha = OPAQUE, int scroll = NO_SCROLL)
        {
            if (scroll <= 0)
                return;

            if (alpha <= 0)
                return;

            string working_text = text.ToLower();

            short extra_y = 0;
            int return_length = 0;
            int text_length = text.Length;

            if (scroll > text_length)
                scroll = text_length;

            if (alpha > 255)
                alpha = 255;

            if (position.x == CENTRE)
                position.x = Data.W_SEMI_LARGEUR - ((LARGEUR_DEFAUT + ESPACE_DEFAUT) * size * text_length - 1) / 2;

            if (position.y == CENTRE)
                position.y = Data.W_SEMI_HAUTEUR - (HAUTEUR_DEFAUT * size) / 2;

            SDL_SetRenderDrawColor(Program.render, (byte)((color >> 16) & 0xFF), (byte)((color >> 8) & 0xFF), (byte)(color & 0xFF), (byte)alpha);

            float x;
            float y;
            int current_info_index;
            for (int i = 0; i < scroll; i++)
            {
                y = position.y + extra_y;
                x = position.x + i * (LARGEUR_DEFAUT + ESPACE_DEFAUT) * size - return_length;

                if (x + size * LARGEUR_DEFAUT * 4 > Data.W_LARGEUR)
                {
                    extra_y += (short)((HAUTEUR_DEFAUT + ESPACE_DEFAUT) * size);
                    return_length = (int)((i + 1) * 8 * size);
                }

                if (working_text[i] == '\0')
                    return;

                if (working_text[i] == '\n')
                {
                    extra_y += (short)((HAUTEUR_DEFAUT + ESPACE_DEFAUT) * size);
                    return_length = (int)((i + 1) * (LARGEUR_DEFAUT + ESPACE_DEFAUT) * size);
                    continue;
                }

                current_info_index = GetListEntry(working_text[i]);

                while (current_info_index < char_draw_info.Length)
                {
                    if (char_draw_info[current_info_index] == 127)
                        break;

                    SDL_RenderDrawLine(Program.render,
                        (int)(char_draw_info[current_info_index] * size + x),
                        (int)(char_draw_info[current_info_index + 1] * size + y),
                        (int)(char_draw_info[current_info_index + 2] * size + x),
                        (int)(char_draw_info[current_info_index + 3] * size + y)
                    );

                    current_info_index += 4;
                }
            }

        }
    }
    public static class Cutscene
    {
        public static SDL_Rect rect = new SDL_Rect();
        static short[,] stars = new short[50, 2];
        static short[,] stars_glx = new short[300, 2];
        static Vector2[] neutron_slowdown = new Vector2[50];
        static sbyte[,] f_model = Data.MODELE_A;
        static float[,] f_model_pos = new float[7, 3];
        static byte lTimer = 10;
        static short temp = 0;
        static byte gFade = 0;
        static Ennemi[] ens = new Ennemi[2];

        public static void Cut_0() // intro
        {
            if (Program.gTimer == 1 && Program.cutscene_skip)
                Program.gTimer = 451;
            if (Program.gTimer == 75)
            {
                Son.JouerEffet(ListeAudioEffets.PRESENTE);
            }

            if (Program.gTimer >= 75 && Program.gTimer < 150)
            {
                Text.DisplayText("malcolm gauthier", new Vector2(Text.CENTRE, Text.CENTRE), 2);
                Text.DisplayText("\n présente", new Vector2(Text.CENTRE, Text.CENTRE), 2);

            }
            else if (Program.gTimer >= 150 && Program.gTimer <= 225)
            {
                Text.DisplayText("malcolm gauthier", new Vector2(Text.CENTRE, Text.CENTRE), 2, alpha: (short)((225 - Program.gTimer) * 3.4f));
                Text.DisplayText("\n présente", new Vector2(Text.CENTRE, Text.CENTRE), 2, alpha: (short)((225 - Program.gTimer) * 3.4f));
            }

            if (Program.gTimer > 225)
            {
                Program.Gamemode = Gamemode.TITLESCREEN;
                Son.JouerMusique(ListeAudioMusique.DYSGENESIS, true);
            }
        }
        public static void Cut_1() // new game
        {
            if (Program.gTimer == 1 && Program.cutscene_skip)
                Program.gTimer = 2101;

            if (Program.gTimer == 60)
            {
                Son.JouerMusique(ListeAudioMusique.CRTP, false);
                Program.player.afficher = true;
            }

            if (Program.gTimer >= 60 && Program.gTimer <= 300)
            {
                Text.DisplayText("des centaines d'années après les premiers voyages hors de la terre, l'espace\n" +
                                 "est devenue zone de guerre et de colonisation. des douzaines de factions \n" +
                                 "existent à travers la galaxie.", new Vector2(20, 700), 3, scroll: (ushort)(Program.gTimer - 60));

                SDL_SetRenderDrawColor(Program.render, 255, 127, 0, 255);
                Background.DessinerCercle(new Vector2(960, 440), 200, 50);
                SDL_SetRenderDrawColor(Program.render, 255, 0, 127, 255);
                Background.DessinerCercle(new Vector2(260, 390), 100, 50);
                SDL_SetRenderDrawColor(Program.render, 255, 127, 127, 255);
                Background.DessinerCercle(new Vector2(1660, 390), 100, 50);

                #region planète 1
                SDL_SetRenderDrawColor(Program.render, 127, 255, 127, 255);
                SDL_RenderDrawLine(Program.render, 818, 298, 930, 336);
                SDL_RenderDrawLine(Program.render, 930, 336, 971, 355);
                SDL_RenderDrawLine(Program.render, 971, 355, 910, 373);
                SDL_RenderDrawLine(Program.render, 910, 373, 860, 400);
                SDL_RenderDrawLine(Program.render, 860, 400, 893, 412);
                SDL_RenderDrawLine(Program.render, 893, 412, 906, 438);
                SDL_RenderDrawLine(Program.render, 906, 438, 861, 453);
                SDL_RenderDrawLine(Program.render, 861, 453, 766, 492);
                SDL_RenderDrawLine(Program.render, 1160, 440, 1066, 425);
                SDL_RenderDrawLine(Program.render, 1066, 425, 1000, 455);
                SDL_RenderDrawLine(Program.render, 1000, 455, 1002, 490);
                SDL_RenderDrawLine(Program.render, 1002, 490, 1036, 497);
                SDL_RenderDrawLine(Program.render, 1036, 497, 1048, 515);
                SDL_RenderDrawLine(Program.render, 1048, 515, 989, 545);
                SDL_RenderDrawLine(Program.render, 989, 545, 1050, 583);
                SDL_RenderDrawLine(Program.render, 1050, 583, 1101, 581);
                #endregion

                #region planète 2
                SDL_SetRenderDrawColor(Program.render, 0, 0, 255, 255);
                SDL_RenderDrawLine(Program.render, 189, 319, 218, 334);
                SDL_RenderDrawLine(Program.render, 218, 334, 250, 344);
                SDL_RenderDrawLine(Program.render, 250, 344, 258, 365);
                SDL_RenderDrawLine(Program.render, 258, 365, 237, 395);
                SDL_RenderDrawLine(Program.render, 237, 395, 219, 425);
                SDL_RenderDrawLine(Program.render, 219, 425, 227, 454);
                SDL_RenderDrawLine(Program.render, 227, 454, 234, 486);
                SDL_RenderDrawLine(Program.render, 307, 302, 302, 330);
                SDL_RenderDrawLine(Program.render, 302, 330, 318, 339);
                SDL_RenderDrawLine(Program.render, 318, 339, 342, 333);
                SDL_RenderDrawLine(Program.render, 360, 390, 354, 406);
                SDL_RenderDrawLine(Program.render, 354, 406, 340, 411);
                SDL_RenderDrawLine(Program.render, 340, 411, 322, 395);
                SDL_RenderDrawLine(Program.render, 322, 395, 294, 400);
                SDL_RenderDrawLine(Program.render, 294, 400, 272, 426);
                SDL_RenderDrawLine(Program.render, 272, 426, 304, 450);
                SDL_RenderDrawLine(Program.render, 304, 450, 330, 460);
                #endregion

                #region planète 3
                SDL_SetRenderDrawColor(Program.render, 255, 0, 0, 255);
                SDL_RenderDrawLine(Program.render, 1657, 489, 1605, 443);
                SDL_RenderDrawLine(Program.render, 1605, 443, 1579, 393);
                SDL_RenderDrawLine(Program.render, 1579, 393, 1583, 350);
                SDL_RenderDrawLine(Program.render, 1583, 350, 1599, 310);
                SDL_RenderDrawLine(Program.render, 1700, 481, 1674, 457);
                SDL_RenderDrawLine(Program.render, 1674, 457, 1648, 422);
                SDL_RenderDrawLine(Program.render, 1648, 422, 1633, 361);
                SDL_RenderDrawLine(Program.render, 1633, 361, 1637, 321);
                SDL_RenderDrawLine(Program.render, 1637, 321, 1647, 290);
                SDL_RenderDrawLine(Program.render, 1740, 449, 1713, 419);
                SDL_RenderDrawLine(Program.render, 1713, 419, 1689, 371);
                SDL_RenderDrawLine(Program.render, 1689, 371, 1686, 331);
                SDL_RenderDrawLine(Program.render, 1686, 331, 1702, 299);
                SDL_RenderDrawLine(Program.render, 1759, 385, 1744, 371);
                SDL_RenderDrawLine(Program.render, 1744, 371, 1735, 349);
                SDL_RenderDrawLine(Program.render, 1735, 349, 1738, 328);
                #endregion

                #region drapeaux
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                SDL_RenderDrawLine(Program.render, 950, 100, 957, 240);
                SDL_RenderDrawLine(Program.render, 1644, 178, 1653, 290);
                SDL_RenderDrawLine(Program.render, 252, 174, 258, 290);

                SDL_SetRenderDrawColor(Program.render, 255, 0, 0, 255);
                SDL_RenderDrawLine(Program.render, 252, 174, 333, 176);
                SDL_RenderDrawLine(Program.render, 333, 176, 333, 225);
                SDL_RenderDrawLine(Program.render, 333, 225, 255, 229);

                SDL_SetRenderDrawColor(Program.render, 0, 0, 255, 255);
                SDL_RenderDrawLine(Program.render, 950, 100, 1081, 92);
                SDL_RenderDrawLine(Program.render, 1081, 92, 1082, 158);
                SDL_RenderDrawLine(Program.render, 1082, 158, 954, 163);

                SDL_SetRenderDrawColor(Program.render, 0, 255, 0, 255);
                SDL_RenderDrawLine(Program.render, 1644, 178, 1747, 172);
                SDL_RenderDrawLine(Program.render, 1747, 172, 1750, 230);
                SDL_RenderDrawLine(Program.render, 1750, 230, 1649, 235);
                #endregion

                if (Program.gTimer == 60)
                {
                    for (int i = 0; i < 50; i++)
                    {
                        int x = 151, y = 499;
                        while ((x > 150 && x < 375 && y < 500 && y > 280) ||
                            (x > 750 && x < 1175 && y < 650 && y > 230) ||
                            (x > 1550 && x < 1775 && y < 500 && y > 280))
                        {
                            x = Program.RNG.Next(25, 1880);
                            y = Program.RNG.Next(25, 680);
                        }
                        stars[i, 0] = (short)x;
                        stars[i, 1] = (short)y;

                    }
                }
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                for (int i = 0; i < 50; i++)
                {
                    SDL_RenderDrawPoint(Program.render, stars[i, 0], stars[i, 1]);
                }

                if (Program.gTimer % Program.RNG.Next(10, 15) == 0)
                {
                    short try_x = (short)Program.RNG.Next(160, 1760), try_y = (short)Program.RNG.Next(240, 640);
                    while (Background.Distance(try_x, try_y, 960, 440) > 200 && Background.Distance(try_x, try_y, 260, 390) > 100 &&
                        Background.Distance(try_x, try_y, 1660, 390) > 100)
                    {
                        try_x = (short)Program.RNG.Next(160, 1760);
                        try_y = (short)Program.RNG.Next(240, 640);
                    }
                    new Explosion(new Vector3(try_x, try_y, (byte)Program.RNG.Next(Data.G_MAX_DEPTH / 8, Data.G_MAX_DEPTH / 4)));
                }
                for (int i = 0; i < Program.explosions.Count; i++)
                {
                    if (Program.explosions[i].Exist())
                        i--;
                }
            } // planètes
            else if (Program.gTimer > 300 && Program.gTimer <= 540)
            {
                Text.DisplayText("ayant servi plus d'une décennie pour l'armée de ta planète, tu es reconnu \n" +
                                 "par le dirigeant militaire de ta faction comme un des meilleurs pilotes \n" +
                                 "de la région galactique locale.", new Vector2(20, 700), 3, scroll: (ushort)(Program.gTimer - 300));

                #region toi
                SDL_RenderDrawLine(Program.render, 1282, 680, 1261, 417);
                SDL_RenderDrawLine(Program.render, 1261, 417, 1340, 366);
                SDL_RenderDrawLine(Program.render, 1340, 366, 1373, 400);
                SDL_RenderDrawLine(Program.render, 1373, 400, 1453, 400);
                SDL_RenderDrawLine(Program.render, 1453, 400, 1492, 368);
                SDL_RenderDrawLine(Program.render, 1492, 368, 1545, 412);
                SDL_RenderDrawLine(Program.render, 1545, 412, 1511, 680);

                SDL_RenderDrawLine(Program.render, 1261, 417, 1229, 637);
                SDL_RenderDrawLine(Program.render, 1545, 412, 1624, 337);
                SDL_RenderDrawLine(Program.render, 1624, 337, 1417, 260);
                Background.DessinerCercle(new Vector2(1416, 314), 77, 24);

                SDL_SetRenderDrawColor(Program.render, 255, 0, 127, 255);
                SDL_RenderDrawLine(Program.render, 1462, 434, 1503, 434);
                SDL_RenderDrawLine(Program.render, 1503, 434, 1484, 483);
                SDL_RenderDrawLine(Program.render, 1484, 483, 1462, 434);

                SDL_SetRenderDrawColor(Program.render, 255, 255, 0, 255);
                SDL_RenderDrawLine(Program.render, 1484, 483, 1468, 503);
                SDL_RenderDrawLine(Program.render, 1468, 503, 1484, 522);
                SDL_RenderDrawLine(Program.render, 1484, 522, 1499, 502);
                SDL_RenderDrawLine(Program.render, 1499, 502, 1484, 483);
                #endregion

                #region drapeau
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                SDL_RenderDrawLine(Program.render, 148, 680, 118, 57);
                SDL_RenderDrawLine(Program.render, 118, 57, 172, 49);
                SDL_RenderDrawLine(Program.render, 172, 49, 203, 680);

                SDL_SetRenderDrawColor(Program.render, 255, 255, 0, 255);
                SDL_RenderDrawLine(Program.render, 175, 115, 255, 61);
                SDL_RenderDrawLine(Program.render, 255, 61, 442, 55);
                SDL_RenderDrawLine(Program.render, 442, 55, 534, 88);
                SDL_RenderDrawLine(Program.render, 534, 88, 693, 82);
                SDL_RenderDrawLine(Program.render, 693, 82, 766, 40);
                SDL_RenderDrawLine(Program.render, 766, 40, 804, 443);
                SDL_RenderDrawLine(Program.render, 804, 443, 746, 478);
                SDL_RenderDrawLine(Program.render, 746, 478, 580, 489);
                SDL_RenderDrawLine(Program.render, 580, 489, 481, 443);
                SDL_RenderDrawLine(Program.render, 481, 443, 311, 452);
                SDL_RenderDrawLine(Program.render, 311, 452, 194, 500);

                SDL_SetRenderDrawColor(Program.render, 255, 127, 0, 255);
                Background.DessinerCercle(new Vector2(479, 232), 91, 24);
                Background.DessinerCercle(new Vector2(479, 232), 65, 24);

                SDL_SetRenderDrawColor(Program.render, 0, 255, 0, 255);
                SDL_RenderDrawLine(Program.render, 563, 195, 672, 204);
                SDL_RenderDrawLine(Program.render, 672, 204, 778, 172);
                SDL_RenderDrawLine(Program.render, 570, 243, 677, 251);
                SDL_RenderDrawLine(Program.render, 677, 251, 782, 214);
                SDL_RenderDrawLine(Program.render, 395, 197, 276, 190);
                SDL_RenderDrawLine(Program.render, 276, 190, 180, 219);
                SDL_RenderDrawLine(Program.render, 390, 250, 277, 242);
                SDL_RenderDrawLine(Program.render, 277, 242, 183, 282);
                #endregion

                #region étoile
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                SDL_RenderDrawLine(Program.render, 472, 167, 443, 286);
                SDL_RenderDrawLine(Program.render, 443, 286, 534, 197);
                SDL_RenderDrawLine(Program.render, 534, 197, 420, 206);
                SDL_RenderDrawLine(Program.render, 420, 206, 524, 279);
                SDL_RenderDrawLine(Program.render, 524, 279, 472, 167);
                #endregion
            } // o7
            else if (Program.gTimer > 540 && Program.gTimer <= 780)
            {
                Text.DisplayText("un jour, une lettre arrive à ta porte.\n" +
                                 "elle porte l'emblême officielle du pays, donc c'est probablement très \n" +
                                 "important.",
                                 new Vector2(20, 700), 3, scroll: (ushort)(Program.gTimer - 540));

                #region lettre
                SDL_RenderDrawLine(Program.render, 717, 607, 600, 400);
                SDL_RenderDrawLine(Program.render, 600, 400, 958, 198);
                SDL_RenderDrawLine(Program.render, 958, 198, 1062, 411);
                SDL_RenderDrawLine(Program.render, 1062, 411, 717, 607);
                SDL_RenderDrawLine(Program.render, 600, 400, 792, 385);
                SDL_RenderDrawLine(Program.render, 856, 353, 958, 198);
                #endregion

                #region emblême cercle
                SDL_SetRenderDrawColor(Program.render, 255, 127, 0, 255);
                Background.DessinerCercle(new Vector2(832, 384), 30, 24);
                Background.DessinerCercle(new Vector2(832, 384), 39, 24);
                #endregion

                #region étoile
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                SDL_RenderDrawLine(Program.render, 859, 396, 815, 358);
                SDL_RenderDrawLine(Program.render, 815, 358, 831, 414);
                SDL_RenderDrawLine(Program.render, 831, 414, 849, 359);
                SDL_RenderDrawLine(Program.render, 849, 359, 802, 389);
                SDL_RenderDrawLine(Program.render, 802, 389, 859, 396);
                #endregion
            } // lettre fermée
            else if (Program.gTimer > 780 && Program.gTimer <= 1020)
            {
                Text.DisplayText("\"bonjour. \n" +
                                 "la coalition des planètes locales vous a choisi pour mener une mission \n" +
                                 "seule qui sauvera notre peuple des mains ennemies, ou bien même la mort.\"", new Vector2(20, 700), 3, scroll: (ushort)(Program.gTimer - 780));

                #region lettre
                SDL_RenderDrawLine(Program.render, 717, 607, 600, 400);
                SDL_RenderDrawLine(Program.render, 958, 198, 1062, 411);
                SDL_RenderDrawLine(Program.render, 1062, 411, 717, 607);
                SDL_RenderDrawLine(Program.render, 600, 400, 832, 383);
                SDL_RenderDrawLine(Program.render, 832, 383, 958, 198);
                SDL_RenderDrawLine(Program.render, 600, 400, 648, 316);
                SDL_RenderDrawLine(Program.render, 958, 198, 868, 193);
                #endregion

                #region papier
                SDL_SetRenderDrawColor(Program.render, 255, 150, 25, 255);
                SDL_RenderDrawLine(Program.render, 695, 393, 577, 199);
                SDL_RenderDrawLine(Program.render, 577, 199, 802, 79);
                SDL_RenderDrawLine(Program.render, 802, 79, 911, 267);
                #endregion

                #region blabla
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                SDL_RenderDrawLine(Program.render, 611, 212, 669, 180);
                SDL_RenderDrawLine(Program.render, 636, 252, 817, 153);
                SDL_RenderDrawLine(Program.render, 644, 273, 718, 231);
                SDL_RenderDrawLine(Program.render, 739, 218, 821, 173);
                SDL_RenderDrawLine(Program.render, 659, 292, 684, 276);
                SDL_RenderDrawLine(Program.render, 702, 265, 799, 212);
                SDL_RenderDrawLine(Program.render, 818, 201, 839, 188);
                SDL_RenderDrawLine(Program.render, 664, 312, 797, 228);
                SDL_RenderDrawLine(Program.render, 820, 221, 850, 200);
                SDL_RenderDrawLine(Program.render, 682, 337, 766, 280);
                SDL_RenderDrawLine(Program.render, 798, 263, 860, 225);
                SDL_RenderDrawLine(Program.render, 698, 358, 869, 251);
                SDL_RenderDrawLine(Program.render, 843, 317, 893, 282);
                #endregion
            } // lettre ouverte
            else if (Program.gTimer > 1020 && Program.gTimer <= 1260)
            {
                Text.DisplayText("\"on a récemment crée l'un des meilleurs vaisseaux de la galaxie, mais \n" +
                                 "à cause du nombre de ressources requis pour le construire, on en n'a \n" +
                                 "qu'un seul.\"", new Vector2(20, 700), 3, scroll: (ushort)(Program.gTimer - 1020));

                #region modèles joueur
                Program.player.position = new Vector3(400, 400, 0);
                Program.player.pitch = 4;
                Program.player.roll = 0;
                Program.player.taille = 6;
                Program.player.RenderObject();

                Program.player.position = new Vector3(1400, 200, 0);
                Program.player.pitch = 0;
                Program.player.roll = 0.25f * (float)Sin(Program.gTimer / 30f);
                Program.player.taille = 5;
                Program.player.RenderObject();
                #endregion

                #region lignes
                SDL_RenderDrawLine(Program.render, 293, 261, 347, 173);
                SDL_RenderDrawLine(Program.render, 347, 173, 588, 173);
                SDL_RenderDrawLine(Program.render, 462, 549, 521, 628);
                SDL_RenderDrawLine(Program.render, 521, 628, 744, 631);
                SDL_RenderDrawLine(Program.render, 1191, 76, 1164, 99);
                SDL_RenderDrawLine(Program.render, 1164, 99, 1166, 258);
                SDL_RenderDrawLine(Program.render, 1195, 284, 1166, 258);
                SDL_RenderDrawLine(Program.render, 1165, 176, 981, 171);
                SDL_RenderDrawLine(Program.render, 1474, 168, 1544, 96);
                SDL_RenderDrawLine(Program.render, 1544, 96, 1736, 96);
                #endregion

                #region texte et blabla rouge
                Text.DisplayText("x-57", new Vector2(880, 330), 10);
                SDL_SetRenderDrawColor(Program.render, 255, 0, 0, 255);
                SDL_RenderDrawLine(Program.render, 394, 142, 556, 136);
                SDL_RenderDrawLine(Program.render, 565, 591, 708, 582);
                SDL_RenderDrawLine(Program.render, 1016, 129, 1136, 137);
                SDL_RenderDrawLine(Program.render, 1589, 64, 1709, 67);
                #endregion

                #region plus de blabla
                SDL_RenderDrawLine(Program.render, 874, 472, 1237, 465);
                SDL_RenderDrawLine(Program.render, 1306, 456, 1680, 458);
                SDL_RenderDrawLine(Program.render, 1700, 500, 1565, 508);
                SDL_RenderDrawLine(Program.render, 1521, 512, 1028, 523);
                SDL_RenderDrawLine(Program.render, 979, 527, 867, 522);
                SDL_RenderDrawLine(Program.render, 874, 571, 1149, 569);
                SDL_RenderDrawLine(Program.render, 1206, 559, 1331, 566);
                SDL_RenderDrawLine(Program.render, 1402, 558, 1685, 553);
                SDL_RenderDrawLine(Program.render, 1687, 626, 1264, 626);
                SDL_RenderDrawLine(Program.render, 1210, 632, 1111, 631);
                SDL_RenderDrawLine(Program.render, 1042, 634, 879, 638);
                #endregion

            } // vaisseau
            else if (Program.gTimer > 1260 && Program.gTimer <= 1500)
            {
                Text.DisplayText("\"votre mission est d'aller détruire la bombe à pulsar dans la région \n" +
                                    "d'espace de l'ennemi, et de s'assurer qu'elle est neutralisée, ou sous \n" +
                                    "notre controle.\"", new Vector2(20, 700), 3, scroll: (ushort)(Program.gTimer - 1260));

                BombePulsar.DessinerBombePulsar(new Vector2(960, 330), 180, false);

                if (Program.gTimer == 1261)
                {
                    for (int i = 0; i < 50; i++)
                    {
                        int x = 960, y = 330;
                        while (x > 780 && y > 150 && x < 1140 && y < 510)
                        {
                            x = Program.RNG.Next(25, 1880);
                            y = Program.RNG.Next(25, 680);
                        }
                        stars[i, 0] = (short)x;
                        stars[i, 1] = (short)y;
                    }
                }
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                for (int i = 0; i < 50; i++)
                {
                    SDL_RenderDrawPoint(Program.render, stars[i, 0], stars[i, 1]);
                }

                #region bleu autour de pôles
                SDL_SetRenderDrawColor(Program.render, 0, 0, 255, 255);
                SDL_RenderDrawLine(Program.render, 833, 201, 770, 127);
                SDL_RenderDrawLine(Program.render, 770, 127, 698, 120);
                SDL_RenderDrawLine(Program.render, 698, 120, 640, 135);
                SDL_RenderDrawLine(Program.render, 854, 184, 800, 100);
                SDL_RenderDrawLine(Program.render, 800, 100, 818, 36);
                SDL_RenderDrawLine(Program.render, 818, 36, 860, 21);
                SDL_RenderDrawLine(Program.render, 1055, 482, 1109, 549);
                SDL_RenderDrawLine(Program.render, 1109, 549, 1106, 606);
                SDL_RenderDrawLine(Program.render, 1106, 606, 1082, 643);
                SDL_RenderDrawLine(Program.render, 1078, 465, 1136, 526);
                SDL_RenderDrawLine(Program.render, 1136, 526, 1196, 531);
                SDL_RenderDrawLine(Program.render, 1196, 531, 1230, 515);
                #endregion

                #region plus de bleu autour des pôles
                SDL_RenderDrawLine(Program.render, 1069, 479, 1097, 518);
                SDL_RenderDrawLine(Program.render, 1090, 494, 1137, 542);
                SDL_RenderDrawLine(Program.render, 1111, 533, 1134, 567);
                SDL_RenderDrawLine(Program.render, 1120, 564, 1123, 617);
                SDL_RenderDrawLine(Program.render, 1139, 583, 1144, 619);
                SDL_RenderDrawLine(Program.render, 1148, 559, 1202, 596);
                SDL_RenderDrawLine(Program.render, 1169, 547, 1219, 553);
                SDL_RenderDrawLine(Program.render, 829, 181, 795, 139);
                SDL_RenderDrawLine(Program.render, 829, 158, 780, 85);
                SDL_RenderDrawLine(Program.render, 782, 123, 738, 96);
                SDL_RenderDrawLine(Program.render, 723, 101, 676, 86);
                SDL_RenderDrawLine(Program.render, 770, 97, 753, 57);
                SDL_RenderDrawLine(Program.render, 792, 79, 802, 28);
                #endregion

            } // bombe à pulsar
            else if (Program.gTimer > 1500 && Program.gTimer <= 1740)
            {
                Text.DisplayText("\"les coordonées de la bombe se trouvent programmés dans votre vaisseau, \n" +
                                 "qui vous attend au garage 05. \n" +
                                 "on compte sur vous, n'échouez pas.\" \n" +
                                 "- le dirigeant militaire", new Vector2(20, 700), 3, scroll: (ushort)(Program.gTimer - 1500));

                #region lettre
                SDL_RenderDrawLine(Program.render, 717, 607, 600, 400);
                SDL_RenderDrawLine(Program.render, 958, 198, 1062, 411);
                SDL_RenderDrawLine(Program.render, 1062, 411, 717, 607);
                SDL_RenderDrawLine(Program.render, 600, 400, 832, 383);
                SDL_RenderDrawLine(Program.render, 832, 383, 958, 198);
                SDL_RenderDrawLine(Program.render, 600, 400, 648, 316);
                SDL_RenderDrawLine(Program.render, 958, 198, 868, 193);
                #endregion

                #region papier
                SDL_SetRenderDrawColor(Program.render, 255, 150, 25, 255);
                SDL_RenderDrawLine(Program.render, 695, 393, 577, 199);
                SDL_RenderDrawLine(Program.render, 577, 199, 802, 79);
                SDL_RenderDrawLine(Program.render, 802, 79, 911, 267);
                #endregion

                #region blabla
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                SDL_RenderDrawLine(Program.render, 611, 212, 669, 180);
                SDL_RenderDrawLine(Program.render, 636, 252, 817, 153);
                SDL_RenderDrawLine(Program.render, 644, 273, 718, 231);
                SDL_RenderDrawLine(Program.render, 739, 218, 821, 173);
                SDL_RenderDrawLine(Program.render, 659, 292, 684, 276);
                SDL_RenderDrawLine(Program.render, 702, 265, 799, 212);
                SDL_RenderDrawLine(Program.render, 818, 201, 839, 188);
                SDL_RenderDrawLine(Program.render, 664, 312, 797, 228);
                SDL_RenderDrawLine(Program.render, 820, 221, 850, 200);
                SDL_RenderDrawLine(Program.render, 682, 337, 766, 280);
                SDL_RenderDrawLine(Program.render, 798, 263, 860, 225);
                SDL_RenderDrawLine(Program.render, 698, 358, 869, 251);
                SDL_RenderDrawLine(Program.render, 843, 317, 893, 282);
                #endregion

            } // lettre ouverte 2
            else if (Program.gTimer > 1740 && Program.gTimer <= 1980)
            {
                Text.DisplayText("05", new Vector2(100, 200), 15, 0x00FF00);
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);

                #region hangar
                SDL_RenderDrawLine(Program.render, 20, 455, 1900, 455);
                SDL_RenderDrawLine(Program.render, 500, 455, 500, 111);
                SDL_RenderDrawLine(Program.render, 500, 111, 1500, 111);
                SDL_RenderDrawLine(Program.render, 1500, 111, 1500, 455);
                #endregion

                if (Program.gTimer < 1800)
                {
                    SDL_RenderDrawLine(Program.render, 1000, 111, 1000, 455);
                }
                if (Program.gTimer >= 1800 && Program.gTimer < 1860)
                {
                    SDL_RenderDrawLine(Program.render, 1000 - (int)(Program.gTimer - 1800) * 5, 111, 1000 - (int)(Program.gTimer - 1800) * 5, 455);
                    SDL_RenderDrawLine(Program.render, 1000 + (int)(Program.gTimer - 1800) * 5, 111, 1000 + (int)(Program.gTimer - 1800) * 5, 455);
                }
                if (Program.gTimer >= 1860)
                {
                    SDL_RenderDrawLine(Program.render, 700, 111, 700, 455);
                    SDL_RenderDrawLine(Program.render, 1300, 111, 1300, 455);
                }

                if (Program.gTimer == 1741)
                {
                    for (int i = 0; i < 30; i++)
                    {
                        stars[i, 0] = (short)Program.RNG.Next(720, 1280);
                        stars[i, 1] = (short)Program.RNG.Next(120, 450);
                    }
                }
                if (Program.gTimer >= 1800) for (int i = 0; i < 30; i++)
                    {
                        if (Abs(stars[i, 0] - 1000.0f) < (Program.gTimer - 1800) * 5)
                            SDL_RenderDrawPoint(Program.render, stars[i, 0], stars[i, 1]);
                    }

                if (Program.gTimer < 1830)
                {
                    Program.player.position = new Vector3(1000, 550, 0);
                    Program.player.pitch = 0;
                    Program.player.roll = 0;
                    Program.player.taille = 2;
                    Program.player.RenderObject();
                }
                if (Program.gTimer >= 1830 && Program.gTimer <= 1860)
                {
                    Program.player.position = new Vector3(
                        1000,
                        250 * (float)Pow(0.9f, Program.gTimer - 1830) + 300,
                        0
                    );
                    Program.player.taille = 2 * (float)Pow(0.9f, Program.gTimer - 1830);
                    Program.player.RenderObject();
                }

                if (Program.gTimer < 1815)
                    SDL_RenderDrawLine(Program.render, 972, 566, 952, 586);

                if (Program.gTimer < 1810)
                    SDL_RenderDrawPoint(Program.render, (int)(Program.gTimer - 1740 + 952 - 70), 585);

                if (Program.gTimer >= 1810 && Program.gTimer <= 1815)
                    SDL_RenderDrawPoint(Program.render, (int)((Program.gTimer - 1810) * 2 + 972 - 20), (int)((1810 - Program.gTimer) * 2 + 603 - 20));

            } // départ

            if (Program.gTimer > 30 && Program.gTimer < 2000)
            {
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                SDL_RenderDrawLine(Program.render, 20, 20, 1900, 20);
                SDL_RenderDrawLine(Program.render, 1900, 20, 1900, 680);
                SDL_RenderDrawLine(Program.render, 1900, 680, 20, 680);
                SDL_RenderDrawLine(Program.render, 20, 20, 20, 680);
            }

            if (Program.gTimer > 2100)
            {
                Program.player.Init();
                Program.Gamemode = Gamemode.GAMEPLAY;
                Program.niveau = 0;
                Program.ens_needed = (byte)Level_Data.lvl_list[Program.niveau].Length;
                Program.ens_killed = 0;
                Program.explosions.Clear();
            }
        }
        public static void Cut_2() // good end
        {
            if (Program.gTimer == 2 && Program.cutscene_skip)
                Program.gTimer = 1830;

            if (Program.gTimer == 60)
            {
                Son.JouerMusique(ListeAudioMusique.ATW, false);
                Program.gTimer = 120;
                //Program.player.dead = false;
                //Program.gTimer = 1740;//
            }

            if (Program.gTimer >= 120 && Program.gTimer < 300)
            {
                #region étoiles
                if (Program.gTimer == 120)
                {
                    for (int i = 0; i < stars.GetLength(0); i++)
                    {
                        int x = 1500, y = 400;
                        while ((x > 1375 && y > 128 && x < 1676 && y < 402) || (x > 137 && y > 433 && x < 379 && y < 622))
                        {
                            x = Program.RNG.Next(25, 1880);
                            y = Program.RNG.Next(25, 680);
                        }
                        stars[i, 0] = (short)x;
                        stars[i, 1] = (short)y;
                    }
                }
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                for (int i = 0; i < stars.GetLength(0); i++)
                {
                    SDL_RenderDrawPoint(Program.render, stars[i, 0], stars[i, 1]);
                }
                #endregion

                #region player
                Program.player.position = new Vector3(277, 519, 0);
                Program.player.pitch = 0.7f;
                Program.player.roll = 0.8f;
                Program.player.taille = 2;
                Program.player.RenderObject();
                #endregion

                #region enemy 15
                Program.player.taille = 1f;
                Program.player.position = new Vector3(771, 325, 0);
                Program.player.roll = 0.5f;
                double sinroll = Sin(Program.player.roll);
                double cosroll = Cos(Program.player.roll);
                float pitchconst = Program.player.pitch + Data.P_PERMA_PITCH;
                SDL_SetRenderDrawColor(Program.render, 255, 0, 0, 255);
                for (int i = 0; i < Program.player.modele.Length - 1; i++)
                {
                    int[] pos = new int[4] {
                        (int)(Program.player.taille * (cosroll * -Program.player.modele[i].x - sinroll * -Program.player.modele[i].y) + Program.player.position.x),
                        (int)(Program.player.taille * (sinroll * -Program.player.modele[i].x + cosroll * -Program.player.modele[i].y) + Program.player.position.y - Program.player.modele[i].z * pitchconst),
                        (int)(Program.player.taille * (cosroll * -Program.player.modele[i + 1].x - sinroll * -Program.player.modele[i + 1].y) + Program.player.position.x),
                        (int)(Program.player.taille * (sinroll * -Program.player.modele[i + 1].x + cosroll * -Program.player.modele[i + 1].y) + Program.player.position.y - Program.player.modele[i + 1].z * pitchconst)
                    };
                    SDL_RenderDrawLine(Program.render, pos[0], pos[1], pos[2], pos[3]);
                }
                #endregion

                #region bombe pulsar
                BombePulsar.DessinerBombePulsar(new Vector2(1522 + Program.RNG.Next(-5, 5), 264 + Program.RNG.Next(-5, 5)),
                    133, false);

                SDL_SetRenderDrawColor(Program.render, 0, 0, 255, 255);
                SDL_RenderDrawLine(Program.render, 1463, 144, 1445, 97);
                SDL_RenderDrawLine(Program.render, 1445, 97, 1422, 68);
                SDL_RenderDrawLine(Program.render, 1422, 68, 1373, 46);
                SDL_RenderDrawLine(Program.render, 1525, 131, 1522, 86);
                SDL_RenderDrawLine(Program.render, 1522, 86, 1554, 48);
                SDL_RenderDrawLine(Program.render, 1554, 48, 1584, 35);
                SDL_RenderDrawLine(Program.render, 1501, 36, 1499, 82);
                SDL_RenderDrawLine(Program.render, 1464, 63, 1484, 112);
                SDL_RenderDrawLine(Program.render, 1500, 396, 1491, 454);
                SDL_RenderDrawLine(Program.render, 1491, 454, 1470, 493);
                SDL_RenderDrawLine(Program.render, 1470, 493, 1450, 534);
                SDL_RenderDrawLine(Program.render, 1549, 395, 1550, 450);
                SDL_RenderDrawLine(Program.render, 1550, 450, 1574, 486);
                SDL_RenderDrawLine(Program.render, 1574, 486, 1612, 513);
                SDL_RenderDrawLine(Program.render, 1514, 434, 1506, 485);
                SDL_RenderDrawLine(Program.render, 1539, 475, 1560, 520);
                #endregion

                #region boom
                if (Program.gTimer > 200)
                {
                    lTimer = (byte)(Program.gTimer - 200);
                    SDL_SetRenderDrawColor(Program.render, 255, 255, 255, (byte)(154 + lTimer));
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
                    SDL_RenderFillRect(Program.render, ref rect);
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
                    SDL_RenderFillRect(Program.render, ref rect);
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
                    SDL_RenderFillRect(Program.render, ref rect);
                }
                #endregion
            } // boom - fini
            else if (Program.gTimer >= 300 && Program.gTimer < 660)
            {
                #region étoiles
                if (Program.gTimer == 300)
                {
                    for (int i = 0; i < stars.GetLength(0); i++)
                    {
                        int x = Program.RNG.Next(25, 1880), y = Program.RNG.Next(25, 680);
                        while (x > 442 && y > 76 && x < 1447 && y < 601)
                        {
                            x = Program.RNG.Next(25, 1880);
                            y = Program.RNG.Next(25, 680);
                        }
                        stars[i, 0] = (short)x;
                        stars[i, 1] = (short)y;
                    }
                    for (int i = 0; i < stars_glx.GetLength(0); i++)
                    {
                        int x = Program.RNG.Next(442, 1447), y = Program.RNG.Next(76, 601);
                        while (Background.Distance(x, y, 948, 338, 0.3f) > 270 || Background.Distance(x, y, 954, 276, 0.6f) < 80)
                        {
                            x = Program.RNG.Next(25, 1880);
                            y = Program.RNG.Next(25, 680);
                        }
                        stars_glx[i, 0] = (short)x;
                        stars_glx[i, 1] = (short)y;
                    }
                }
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                for (int i = 0; i < stars.GetLength(0); i++)
                {
                    SDL_RenderDrawPoint(Program.render, stars[i, 0], stars[i, 1]);
                }
                for (int i = 0; i < stars_glx.GetLength(0); i++)
                {
                    if (Program.gTimer > 480)
                    {
                        if (Background.Distance(stars_glx[i, 0], stars_glx[i, 1], 716, 437, 0.5f) < 100)
                            SDL_SetRenderDrawColor(Program.render, 0, 0, 0, 255);
                        else
                            SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                    }
                    SDL_RenderDrawPoint(Program.render, stars_glx[i, 0], stars_glx[i, 1]);
                }
                #endregion

                #region galaxie
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 60);
                SDL_RenderDrawLine(Program.render, 525, 362, 477, 195);
                SDL_RenderDrawLine(Program.render, 477, 195, 547, 223);
                SDL_RenderDrawLine(Program.render, 547, 223, 686, 91);
                SDL_RenderDrawLine(Program.render, 686, 91, 697, 141);
                SDL_RenderDrawLine(Program.render, 697, 141, 950, 79);
                SDL_RenderDrawLine(Program.render, 950, 79, 933, 126);
                SDL_RenderDrawLine(Program.render, 933, 126, 1230, 92);
                SDL_RenderDrawLine(Program.render, 1230, 92, 1191, 126);
                SDL_RenderDrawLine(Program.render, 1191, 126, 1404, 179);
                SDL_RenderDrawLine(Program.render, 1404, 179, 1344, 186);
                SDL_RenderDrawLine(Program.render, 1344, 186, 1434, 323);
                SDL_RenderDrawLine(Program.render, 1434, 323, 1370, 312);
                SDL_RenderDrawLine(Program.render, 1370, 312, 1404, 441);
                SDL_RenderDrawLine(Program.render, 1404, 441, 1354, 427);
                SDL_RenderDrawLine(Program.render, 1354, 427, 1285, 571);
                SDL_RenderDrawLine(Program.render, 1285, 571, 1285, 499);
                SDL_RenderDrawLine(Program.render, 1285, 499, 1017, 600);
                SDL_RenderDrawLine(Program.render, 1017, 600, 1032, 538);
                SDL_RenderDrawLine(Program.render, 1032, 538, 700, 600);
                SDL_RenderDrawLine(Program.render, 700, 600, 734, 553);
                SDL_RenderDrawLine(Program.render, 734, 553, 500, 500);
                SDL_RenderDrawLine(Program.render, 500, 500, 600, 500);
                SDL_RenderDrawLine(Program.render, 600, 500, 451, 377);
                SDL_RenderDrawLine(Program.render, 451, 377, 525, 362);

                SDL_SetRenderDrawColor(Program.render, 127, 0, 127, 255);
                SDL_RenderDrawLine(Program.render, 850, 350, 833, 306);
                SDL_RenderDrawLine(Program.render, 833, 306, 842, 271);
                SDL_RenderDrawLine(Program.render, 842, 271, 878, 221);
                SDL_RenderDrawLine(Program.render, 878, 221, 952, 193);
                SDL_RenderDrawLine(Program.render, 952, 193, 1027, 216);
                SDL_RenderDrawLine(Program.render, 1027, 216, 1064, 260);
                SDL_RenderDrawLine(Program.render, 1064, 260, 1076, 299);
                SDL_RenderDrawLine(Program.render, 1076, 299, 1070, 340);

                SDL_RenderDrawLine(Program.render, 828, 315, 798, 324);
                SDL_RenderDrawLine(Program.render, 798, 324, 800, 350);
                SDL_RenderDrawLine(Program.render, 800, 350, 831, 359);
                SDL_RenderDrawLine(Program.render, 831, 359, 1082, 347);
                SDL_RenderDrawLine(Program.render, 1082, 347, 1114, 333);
                SDL_RenderDrawLine(Program.render, 1114, 333, 1112, 309);
                SDL_RenderDrawLine(Program.render, 1112, 309, 1081, 304);
                #endregion

                #region explosions
                if (Program.gTimer < 480)
                {
                    if (Program.gTimer % Program.RNG.Next(8, 12) == 0)
                        new Explosion(new Vector3(Program.RNG.Next(500, 1400), Program.RNG.Next(100, 550), Data.G_MAX_DEPTH / 2));
                }
                for (int i = 0; i < Program.explosions.Count; i++)
                {
                    if (Program.explosions[i].Exist())
                        i--;
                }
                #endregion

                #region boom
                if (Program.gTimer > 450 && Program.gTimer <= 500)
                {
                    lTimer = (byte)(2 * Program.gTimer - 900);
                    int abs_lTimer = -2 * (int)Abs(lTimer - 50) + 100; // y=-2|x-(r/2)|+r, ou environ /\
                    SDL_SetRenderDrawColor(Program.render, 255, 255, 255, (byte)(154 + lTimer));
                    rect.x = 716 - abs_lTimer * 2;
                    rect.y = 437 + abs_lTimer / 4;
                    rect.w = 2 * (716 - rect.x);
                    rect.h = -2 * (rect.y - 437);
                    SDL_RenderFillRect(Program.render, ref rect);
                    rect.x = 716 - abs_lTimer;
                    rect.y = 437 + abs_lTimer / 2;
                    rect.w = 2 * (716 - rect.x);
                    rect.h = -2 * (rect.y - 437);
                    SDL_RenderFillRect(Program.render, ref rect);
                    rect.x = 716 - abs_lTimer / 2;
                    rect.y = 437 + abs_lTimer;
                    rect.w = 2 * (716 - rect.x);
                    rect.h = -2 * (rect.y - 437);
                    SDL_RenderFillRect(Program.render, ref rect);
                }
                #endregion
            } // boom galactique (2) - fini
            else if (Program.gTimer >= 660 && Program.gTimer < 840)
            {
                #region étoiles
                if (Program.gTimer == 660)
                {
                    for (int i = 0; i < stars.GetLength(0); i++)
                    {
                        int x = Program.RNG.Next(25, 1880), y = Program.RNG.Next(25, 680);
                        while (Background.Distance(x, y, Data.W_SEMI_LARGEUR, Data.W_SEMI_HAUTEUR / 2, 0.2f) < 200)
                        {
                            x = Program.RNG.Next(25, 1880);
                            y = Program.RNG.Next(25, 680);
                        }
                        stars[i, 0] = (short)x;
                        stars[i, 1] = (short)y;
                    }
                }
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                for (int i = 0; i < stars.GetLength(0); i++)
                {
                    SDL_RenderDrawPoint(Program.render, stars[i, 0], stars[i, 1]);
                }
                #endregion

                #region vaisseaux
                SDL_SetRenderDrawColor(Program.render, 255, 0, 0, 255);
                SDL_RenderDrawPoint(Program.render, -Program.gTimer + 1900, Data.W_SEMI_HAUTEUR / 2);
                SDL_SetRenderDrawColor(Program.render, 0, 255, 0, 255);
                SDL_RenderDrawPoint(Program.render, Data.W_SEMI_LARGEUR, Program.gTimer / -2 + 730);
                SDL_SetRenderDrawColor(Program.render, 255, 255, 0, 255);
                SDL_RenderDrawPoint(Program.render, Program.gTimer - 1, Data.W_SEMI_HAUTEUR / 2);
                SDL_SetRenderDrawColor(Program.render, 0, 0, 255, 255);
                SDL_RenderDrawPoint(Program.render, Data.W_SEMI_LARGEUR, Program.gTimer / 2 - 200);
                #endregion

            } // ranconte - fini
            else if (Program.gTimer >= 840 && Program.gTimer < 1020)
            {
                Text.DisplayText("le grand vide laissé par l'explosion a démontré la vérité de cette guerre.",
                    new Vector2(20, 700), 3, scroll: (int)Program.gTimer / 2 - 420);

                #region étoiles
                if (Program.gTimer == 840)
                {
                    for (int i = 0; i < stars.GetLength(0); i++)
                    {
                        stars[i, 0] = (short)Program.RNG.Next(25, 1880);
                        stars[i, 1] = (short)Program.RNG.Next(25, 180);
                    }
                }
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                for (int i = 0; i < stars.GetLength(0); i++)
                {
                    SDL_RenderDrawPoint(Program.render, stars[i, 0], stars[i, 1]);
                }
                #endregion

                #region vaisseaux
                Vector3[] model = Data.modeles_ennemis[(int)TypeEnnemi.DUPLIQUEUR];
                short x, y;
                sbyte depth = -15;
                float pitch = -1f;

                SDL_SetRenderDrawColor(Program.render, 255, 0, 0, 255);
                x = (short)(-Program.gTimer / 10 + 1200);
                y = (short)(Data.W_SEMI_HAUTEUR - 100);
                SDL_RenderDrawLine(Program.render, x, y, x + 100, y - 65);
                SDL_RenderDrawLine(Program.render, x + 100, y - 65, x + 90, y);
                SDL_RenderDrawLine(Program.render, x + 90, y, x + 139, y - 63);
                SDL_RenderDrawLine(Program.render, x + 139, y - 63, x, y);
                SDL_RenderDrawLine(Program.render, x, y, x + 90, y);

                SDL_SetRenderDrawColor(Program.render, 0, 255, 0, 255);
                x = Data.W_SEMI_LARGEUR;
                y = (short)(Data.W_SEMI_HAUTEUR / 2 + ((int)Program.gTimer - 880) / 10);
                for (int i = 0; i < model.GetLength(0) - 1; i++)
                {
                    SDL_RenderDrawLine(Program.render,
                        (int)(model[i].x * Pow(0.95f, depth) + x),
                        (int)((model[i].y + (model[i].z * pitch)) * Pow(0.95f, depth) + y),
                        (int)(model[i + 1].x * Pow(0.95f, depth) + x),
                        (int)((model[i + 1].y + (model[i + 1].z * pitch)) * Pow(0.95f, depth) + y));
                }

                SDL_SetRenderDrawColor(Program.render, 255, 255, 0, 255);
                x = (short)(Program.gTimer / 10 + 700);
                y = (short)(Data.W_SEMI_HAUTEUR - 100);
                SDL_RenderDrawLine(Program.render, x, y, x - 100, y - 65);
                SDL_RenderDrawLine(Program.render, x - 100, y - 65, x - 90, y);
                SDL_RenderDrawLine(Program.render, x - 90, y, x - 139, y - 63);
                SDL_RenderDrawLine(Program.render, x - 139, y - 63, x, y);
                SDL_RenderDrawLine(Program.render, x, y, x - 90, y);

                SDL_SetRenderDrawColor(Program.render, 0, 0, 255, 255);
                x = Data.W_SEMI_LARGEUR;
                y = (short)(Data.W_SEMI_HAUTEUR - (Program.gTimer - 800) / 10);
                for (int i = 0; i < model.GetLength(0) - 1; i++)
                {
                    SDL_RenderDrawLine(Program.render,
                        (int)(model[i].x * Pow(0.95f, depth) + x),
                        (int)((model[i].y + (model[i].z * -pitch)) * Pow(0.95f, depth) + y),
                        (int)(model[i + 1].x * Pow(0.95f, depth) + x),
                        (int)((model[i + 1].y + (model[i + 1].z * -pitch)) * Pow(0.95f, depth) + y));
                }
                #endregion

            } // ranconte proche - fini
            else if (Program.gTimer >= 1020 && Program.gTimer < 1200)
            {
                Text.DisplayText("l'abscence de vrai gagnants.", new Vector2(20, 700), 3, scroll: (int)(Program.gTimer / 2 - 510));

                #region étoiles
                if (Program.gTimer == 1020)
                {
                    for (int i = 0; i < stars.GetLength(0); i++)
                    {
                        int x = Program.RNG.Next(25, 1880), y = Program.RNG.Next(25, 680);
                        while (Background.Distance(x, y, Data.W_SEMI_LARGEUR, Data.W_SEMI_HAUTEUR / 2 + 100, 0.2f) < 200)
                        {
                            x = Program.RNG.Next(25, 1880);
                            y = Program.RNG.Next(25, 680);
                        }
                        stars[i, 0] = (short)x;
                        stars[i, 1] = (short)y;
                    }
                }
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                for (int i = 0; i < stars.GetLength(0); i++)
                {
                    SDL_RenderDrawPoint(Program.render, stars[i, 0], stars[i, 1]);
                }
                #endregion

                #region vaisseaux
                SDL_SetRenderDrawColor(Program.render, 255, 0, 0, 255);
                SDL_RenderDrawPoint(Program.render, 960, 340);
                SDL_SetRenderDrawColor(Program.render, 0, 255, 0, 255);
                SDL_RenderDrawPoint(Program.render, 950, 345);
                SDL_SetRenderDrawColor(Program.render, 255, 255, 0, 255);
                SDL_RenderDrawPoint(Program.render, 940, 340);
                SDL_SetRenderDrawColor(Program.render, 0, 0, 255, 255);
                SDL_RenderDrawPoint(Program.render, 950, 335);
                #endregion
            } // ranconte loin - fini
            else if (Program.gTimer >= 1200 && Program.gTimer < 1380)
            {
                Text.DisplayText("les factions qui ont survécu ont vite cherché la paix entre eux.\n" +
                                 "des milliards sont morts, victimes de cette guerre. des milliards de trop.",
                                 new Vector2(20, 700), 3, scroll: (int)(Program.gTimer - 1200));

                #region promesse
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                SDL_RenderDrawLine(Program.render, 600, 680, 600, 500);
                SDL_RenderDrawLine(Program.render, 600, 500, 1350, 500);
                SDL_RenderDrawLine(Program.render, 1350, 500, 1350, 680);

                Background.DessinerCercle(new Vector2(730, 189), 57, 50);
                SDL_RenderDrawLine(Program.render, 648, 500, 606, 238);
                SDL_RenderDrawLine(Program.render, 606, 238, 836, 255);
                SDL_RenderDrawLine(Program.render, 836, 255, 806, 500);
                SDL_RenderDrawLine(Program.render, 606, 238, 593, 470);

                Background.DessinerCercle(new Vector2(1203, 186), 57, 50);
                SDL_RenderDrawLine(Program.render, 1126, 500, 1096, 255);
                SDL_RenderDrawLine(Program.render, 1096, 255, 1304, 235);
                SDL_RenderDrawLine(Program.render, 1304, 235, 1278, 500);
                SDL_RenderDrawLine(Program.render, 1304, 235, 1336, 435);

                SDL_RenderDrawLine(Program.render, 836, 255, 972, 297 + (Program.gTimer % 30 < 15 ? 0 : 50));
                SDL_RenderDrawLine(Program.render, 1096, 255, 972, 297 + (Program.gTimer % 30 < 15 ? 0 : 50));
                #endregion

                #region drapeaux
                short[] positions = new short[4] { 100, 300, 1500, 1700 };
                foreach (short i in positions)
                {
                    SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                    SDL_RenderDrawLine(Program.render, i, 680, i, 509);
                    SDL_RenderDrawLine(Program.render, i, 100, i, 365);

                    switch (i)
                    {
                        case 100:
                            SDL_SetRenderDrawColor(Program.render, 255, 0, 0, 255);
                            break;
                        case 300:
                            SDL_SetRenderDrawColor(Program.render, 255, 255, 0, 255);
                            break;
                        case 1500:
                            SDL_SetRenderDrawColor(Program.render, 0, 255, 0, 255);
                            break;
                        case 1700:
                            SDL_SetRenderDrawColor(Program.render, 0, 0, 255, 255);
                            break;
                    }
                    SDL_RenderDrawLine(Program.render, i, 100, 87 + i, 272);
                    SDL_RenderDrawLine(Program.render, 87 + i, 272, 100 + i, 400);
                    SDL_RenderDrawLine(Program.render, 100 + i, 400, 76 + i, 449);
                    SDL_RenderDrawLine(Program.render, 76 + i, 449, 69 + i, 519);
                    SDL_RenderDrawLine(Program.render, 69 + i, 519, 90 + i, 557);
                    SDL_RenderDrawLine(Program.render, 90 + i, 557, 32 + i, 574);
                    SDL_RenderDrawLine(Program.render, 32 + i, 574, -28 + i, 449);
                    SDL_RenderDrawLine(Program.render, -28 + i, 449, -13 + i, 389);
                    SDL_RenderDrawLine(Program.render, -13 + i, 389, 14 + i, 338);
                    SDL_RenderDrawLine(Program.render, 14 + i, 338, 15 + i, 305);
                    SDL_RenderDrawLine(Program.render, 15 + i, 305, i, 300);
                }
                #endregion
            }// paix - fini
            else if (Program.gTimer >= 1380 && Program.gTimer < 1560)
            {
                #region traité
                SDL_SetRenderDrawColor(Program.render, 255, 150, 25, 255);
                SDL_RenderDrawLine(Program.render, 700, 600, 800, 100);
                SDL_RenderDrawLine(Program.render, 800, 100, 1200, 100);
                SDL_RenderDrawLine(Program.render, 1200, 100, 1300, 600);
                SDL_RenderDrawLine(Program.render, 1300, 600, 700, 600);

                Text.DisplayText("traité de paix", new Vector2(825, 125), 3, 0x7f7f7f);

                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                SDL_RenderDrawLine(Program.render, 816, 238, 979, 236);
                SDL_RenderDrawLine(Program.render, 1028, 238, 1180, 234);
                SDL_RenderDrawLine(Program.render, 809, 283, 849, 280);
                SDL_RenderDrawLine(Program.render, 905, 286, 1177, 283);
                SDL_RenderDrawLine(Program.render, 797, 325, 910, 324);
                SDL_RenderDrawLine(Program.render, 945, 325, 1011, 325);
                SDL_RenderDrawLine(Program.render, 1040, 324, 1183, 326);
                SDL_RenderDrawLine(Program.render, 782, 382, 1085, 382);
                SDL_RenderDrawLine(Program.render, 1128, 380, 1199, 382);
                SDL_RenderDrawLine(Program.render, 779, 434, 899, 434);
                SDL_RenderDrawLine(Program.render, 776, 483, 988, 482);
                SDL_RenderDrawLine(Program.render, 783, 583, 1098, 582);
                SDL_RenderDrawLine(Program.render, 775, 523, 744, 572);
                SDL_RenderDrawLine(Program.render, 744, 520, 774, 572);
                #endregion

                #region main + signature
                short x = 2000, y = 0;
                if (Program.gTimer > 1410 && Program.gTimer <= 1430)
                {
                    x = 800;
                    y = (short)(750 - (Program.gTimer - 1410) * 9);
                }
                else if (Program.gTimer > 1430 && Program.gTimer <= 1500)
                {
                    x = (short)(800 + (Program.gTimer - 1430) * 3.57f);
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
                        lTimer = (byte)Program.RNG.Next(107, 147);
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
                else if (Program.gTimer > 1500 && Program.gTimer <= 1520)
                {
                    x = 1050;
                    y = (short)(561 + (Program.gTimer - 1500) * 9);
                }
                SDL_RenderDrawLine(Program.render, x, y, x + 13, y - 27);
                SDL_RenderDrawLine(Program.render, x + 13, y - 27, x + 46, y - 70);
                SDL_RenderDrawLine(Program.render, x + 46, y - 70, x + 53, y - 63);
                SDL_RenderDrawLine(Program.render, x + 53, y - 63, x + 18, y - 19);
                SDL_RenderDrawLine(Program.render, x + 18, y - 19, x, y);
                SDL_RenderDrawLine(Program.render, x + 43, y - 61, x + 202, y + 178);

                if (Program.gTimer > 1430)
                {
                    for (int i = 1; i < stars.GetLength(0); i++)
                    {
                        if (stars[i, 0] == -1 || stars[1, 0] == -1)
                            break;
                        SDL_RenderDrawLine(Program.render, stars[i - 1, 0], stars[i - 1, 1], stars[i, 0], stars[i, 1]);
                    }
                }

                if (Program.gTimer == 1380)
                {
                    rect.x = 0;
                    rect.y = 681;
                    rect.h = 400;
                    rect.w = 2000;
                }
                SDL_SetRenderDrawColor(Program.render, 0, 0, 0, 255);
                SDL_RenderFillRect(Program.render, ref rect);
                #endregion

                Text.DisplayText("les factions qui ont survécu ont vite charché la paix entre eux.\n" +
                                 "des milliards sont morts, victimes de cette guerre. des milliards de trop.", new Vector2(20, 700), 3);
            } // signature - fini
            else if (Program.gTimer >= 1560 && Program.gTimer < 1740)
            {
                Text.DisplayText("les factions galactiques prospèrent maintenant tous économiquement\n" +
                                 "avec leurs liens d'amitié entre eux.", new Vector2(20, 700), 3, scroll: (int)(Program.gTimer - 1560));

                #region planètes
                SDL_SetRenderDrawColor(Program.render, 255, 127, 0, 255);
                Background.DessinerCercle(new Vector2(960, 440), 200, 50);
                SDL_SetRenderDrawColor(Program.render, 255, 0, 127, 255);
                Background.DessinerCercle(new Vector2(260, 390), 100, 50);
                SDL_SetRenderDrawColor(Program.render, 255, 127, 127, 255);
                Background.DessinerCercle(new Vector2(1660, 390), 100, 50);

                SDL_SetRenderDrawColor(Program.render, 127, 255, 127, 255);
                SDL_RenderDrawLine(Program.render, 818, 298, 930, 336);
                SDL_RenderDrawLine(Program.render, 930, 336, 971, 355);
                SDL_RenderDrawLine(Program.render, 971, 355, 910, 373);
                SDL_RenderDrawLine(Program.render, 910, 373, 860, 400);
                SDL_RenderDrawLine(Program.render, 860, 400, 893, 412);
                SDL_RenderDrawLine(Program.render, 893, 412, 906, 438);
                SDL_RenderDrawLine(Program.render, 906, 438, 861, 453);
                SDL_RenderDrawLine(Program.render, 861, 453, 766, 492);
                SDL_RenderDrawLine(Program.render, 1160, 440, 1066, 425);
                SDL_RenderDrawLine(Program.render, 1066, 425, 1000, 455);
                SDL_RenderDrawLine(Program.render, 1000, 455, 1002, 490);
                SDL_RenderDrawLine(Program.render, 1002, 490, 1036, 497);
                SDL_RenderDrawLine(Program.render, 1036, 497, 1048, 515);
                SDL_RenderDrawLine(Program.render, 1048, 515, 989, 545);
                SDL_RenderDrawLine(Program.render, 989, 545, 1050, 583);
                SDL_RenderDrawLine(Program.render, 1050, 583, 1101, 581);

                SDL_SetRenderDrawColor(Program.render, 0, 0, 255, 255);
                SDL_RenderDrawLine(Program.render, 189, 319, 218, 334);
                SDL_RenderDrawLine(Program.render, 218, 334, 250, 344);
                SDL_RenderDrawLine(Program.render, 250, 344, 258, 365);
                SDL_RenderDrawLine(Program.render, 258, 365, 237, 395);
                SDL_RenderDrawLine(Program.render, 237, 395, 219, 425);
                SDL_RenderDrawLine(Program.render, 219, 425, 227, 454);
                SDL_RenderDrawLine(Program.render, 227, 454, 234, 486);
                SDL_RenderDrawLine(Program.render, 307, 302, 302, 330);
                SDL_RenderDrawLine(Program.render, 302, 330, 318, 339);
                SDL_RenderDrawLine(Program.render, 318, 339, 342, 333);
                SDL_RenderDrawLine(Program.render, 360, 390, 354, 406);
                SDL_RenderDrawLine(Program.render, 354, 406, 340, 411);
                SDL_RenderDrawLine(Program.render, 340, 411, 322, 395);
                SDL_RenderDrawLine(Program.render, 322, 395, 294, 400);
                SDL_RenderDrawLine(Program.render, 294, 400, 272, 426);
                SDL_RenderDrawLine(Program.render, 272, 426, 304, 450);
                SDL_RenderDrawLine(Program.render, 304, 450, 330, 460);

                SDL_SetRenderDrawColor(Program.render, 255, 0, 0, 255);
                SDL_RenderDrawLine(Program.render, 1657, 489, 1605, 443);
                SDL_RenderDrawLine(Program.render, 1605, 443, 1579, 393);
                SDL_RenderDrawLine(Program.render, 1579, 393, 1583, 350);
                SDL_RenderDrawLine(Program.render, 1583, 350, 1599, 310);
                SDL_RenderDrawLine(Program.render, 1700, 481, 1674, 457);
                SDL_RenderDrawLine(Program.render, 1674, 457, 1648, 422);
                SDL_RenderDrawLine(Program.render, 1648, 422, 1633, 361);
                SDL_RenderDrawLine(Program.render, 1633, 361, 1637, 321);
                SDL_RenderDrawLine(Program.render, 1637, 321, 1647, 290);
                SDL_RenderDrawLine(Program.render, 1740, 449, 1713, 419);
                SDL_RenderDrawLine(Program.render, 1713, 419, 1689, 371);
                SDL_RenderDrawLine(Program.render, 1689, 371, 1686, 331);
                SDL_RenderDrawLine(Program.render, 1686, 331, 1702, 299);
                SDL_RenderDrawLine(Program.render, 1759, 385, 1744, 371);
                SDL_RenderDrawLine(Program.render, 1744, 371, 1735, 349);
                SDL_RenderDrawLine(Program.render, 1735, 349, 1738, 328);
                #endregion

                #region drapeaux
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                SDL_RenderDrawLine(Program.render, 950, 100, 957, 240);
                SDL_RenderDrawLine(Program.render, 1644, 178, 1653, 290);
                SDL_RenderDrawLine(Program.render, 252, 174, 258, 290);

                SDL_SetRenderDrawColor(Program.render, 255, 0, 0, 255);
                SDL_RenderDrawLine(Program.render, 252, 174, 333, 176);
                SDL_RenderDrawLine(Program.render, 333, 176, 333, 225);
                SDL_RenderDrawLine(Program.render, 333, 225, 255, 229);

                SDL_SetRenderDrawColor(Program.render, 0, 0, 255, 255);
                SDL_RenderDrawLine(Program.render, 950, 100, 1081, 92);
                SDL_RenderDrawLine(Program.render, 1081, 92, 1082, 158);
                SDL_RenderDrawLine(Program.render, 1082, 158, 954, 163);

                SDL_SetRenderDrawColor(Program.render, 0, 255, 0, 255);
                SDL_RenderDrawLine(Program.render, 1644, 178, 1747, 172);
                SDL_RenderDrawLine(Program.render, 1747, 172, 1750, 230);
                SDL_RenderDrawLine(Program.render, 1750, 230, 1649, 235);
                #endregion

                #region étoiles
                if (Program.gTimer == 1560)
                {
                    for (int i = 0; i < 50; i++)
                    {
                        int x = 151, y = 499;
                        while ((x > 150 && x < 375 && y < 500 && y > 280) ||
                            (x > 750 && x < 1175 && y < 650 && y > 230) ||
                            (x > 1550 && x < 1775 && y < 500 && y > 280))
                        {
                            x = Program.RNG.Next(25, 1880);
                            y = Program.RNG.Next(25, 680);
                        }
                        stars[i, 0] = (short)x;
                        stars[i, 1] = (short)y;

                    }
                }
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                for (int i = 0; i < 50; i++)
                {
                    SDL_RenderDrawPoint(Program.render, stars[i, 0], stars[i, 1]);
                }
                #endregion

                #region vols
                SDL_SetRenderDrawColor(Program.render, 255, 255, 0, 255);
                short lz = (short)(Program.gTimer - 1560);
                SDL_RenderDrawPoint(Program.render, 304 + lz, 360);
                SDL_RenderDrawPoint(Program.render, 631 - lz, 454);
                SDL_RenderDrawPoint(Program.render, 872 + lz, 354);
                SDL_RenderDrawPoint(Program.render, 1097 + lz, 479);
                SDL_RenderDrawPoint(Program.render, 1300 - lz, 372);
                SDL_RenderDrawPoint(Program.render, 1600 - lz, 418);
                #endregion
            } // vols entre planètes - fini
            else if (Program.gTimer >= 1740 && Program.gTimer < 1920)
            {
                Text.DisplayText("et même si le trou laissé par l'explosion laissera une empreinte\n" +
                                    "pour quelque temps...", new Vector2(20, 700), 3, scroll: (int)(Program.gTimer - 1740));

                #region étoiles
                if (Program.gTimer == 1740)
                {
                    for (int i = 0; i < stars.GetLength(0); i++)
                    {
                        int x = Program.RNG.Next(25, 1880), y = Program.RNG.Next(25, 680);
                        while (Background.Distance(x, y, Data.W_SEMI_LARGEUR, Data.W_SEMI_HAUTEUR / 2 + 100, 0.2f) < 200)
                        {
                            x = Program.RNG.Next(25, 1880);
                            y = Program.RNG.Next(25, 680);
                        }
                        stars[i, 0] = (short)x;
                        stars[i, 1] = (short)y;
                    }
                }
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                for (int i = 0; i < stars.GetLength(0); i++)
                {
                    SDL_RenderDrawPoint(Program.render, stars[i, 0], stars[i, 1]);
                }
                #endregion

                #region vols
                SDL_SetRenderDrawColor(Program.render, 255, 255, 0, 255);
                short lz = (short)(Program.gTimer - 1740);
                SDL_RenderDrawPoint(Program.render, 152 + lz, 199);
                SDL_RenderDrawPoint(Program.render, 556 - lz, 104 + lz);
                SDL_RenderDrawPoint(Program.render, 257, 505 - lz);
                SDL_RenderDrawPoint(Program.render, 1176 + lz, 643);
                SDL_RenderDrawPoint(Program.render, 1690, 433 - lz);
                SDL_RenderDrawPoint(Program.render, 1608 + lz, 214 + lz);
                SDL_RenderDrawPoint(Program.render, 1429 + lz, 125);
                SDL_RenderDrawPoint(Program.render, 626 - lz, 643);
                SDL_RenderDrawPoint(Program.render, 1284 - lz, 176);
                #endregion
            } // vols entre étoiles - fini
            else if (Program.gTimer >= 1920 && Program.gTimer < 2310)
            {
                Text.DisplayText("même les plus grandes cicatrices se guérissent éventuellement.", new Vector2(20, 700), 3, scroll: (int)(Program.gTimer / 2 - 960));

                #region étoiles
                if (Program.gTimer == 1920)
                {
                    for (int i = 0; i < stars.GetLength(0); i++)
                    {
                        int x = Program.RNG.Next(25, 1880), y = Program.RNG.Next(25, 680);
                        while (x > 442 && y > 76 && x < 1447 && y < 601)
                        {
                            x = Program.RNG.Next(25, 1880);
                            y = Program.RNG.Next(25, 680);
                        }
                        stars[i, 0] = (short)x;
                        stars[i, 1] = (short)y;
                    }
                    for (int i = 0; i < stars_glx.GetLength(0); i++)
                    {
                        if (Background.Distance(stars_glx[i, 0], stars_glx[i, 1], 716, 437, 0.5f) < 100)
                        {
                            stars_glx[i, 0] = -1;
                            stars_glx[i, 1] = -1;
                        }
                    }
                }
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                for (int i = 0; i < stars.GetLength(0); i++)
                {
                    SDL_RenderDrawPoint(Program.render, stars[i, 0], stars[i, 1]);
                }
                for (int i = 0; i < stars_glx.GetLength(0); i++)
                {
                    SDL_RenderDrawPoint(Program.render, stars_glx[i, 0], stars_glx[i, 1]);
                }
                if (Program.gTimer > 2070 && Program.gTimer % 10 == 0)
                {
                    for (int i = 0; i < stars_glx.GetLength(0); i++)
                    {
                        if (stars_glx[i, 0] == -1)
                        {
                            int x = Program.RNG.Next(516, 916), y = Program.RNG.Next(337, 537);
                            while (Background.Distance(x, y, 716, 437, 0.5f) > 100)
                            {
                                x = Program.RNG.Next(516, 916);
                                y = Program.RNG.Next(337, 537);
                            }
                            stars_glx[i, 0] = (short)x;
                            stars_glx[i, 1] = (short)y;
                            break;
                        }
                    }
                }
                #endregion

                #region galaxie
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 60);
                SDL_RenderDrawLine(Program.render, 525, 362, 477, 195);
                SDL_RenderDrawLine(Program.render, 477, 195, 547, 223);
                SDL_RenderDrawLine(Program.render, 547, 223, 686, 91);
                SDL_RenderDrawLine(Program.render, 686, 91, 697, 141);
                SDL_RenderDrawLine(Program.render, 697, 141, 950, 79);
                SDL_RenderDrawLine(Program.render, 950, 79, 933, 126);
                SDL_RenderDrawLine(Program.render, 933, 126, 1230, 92);
                SDL_RenderDrawLine(Program.render, 1230, 92, 1191, 126);
                SDL_RenderDrawLine(Program.render, 1191, 126, 1404, 179);
                SDL_RenderDrawLine(Program.render, 1404, 179, 1344, 186);
                SDL_RenderDrawLine(Program.render, 1344, 186, 1434, 323);
                SDL_RenderDrawLine(Program.render, 1434, 323, 1370, 312);
                SDL_RenderDrawLine(Program.render, 1370, 312, 1404, 441);
                SDL_RenderDrawLine(Program.render, 1404, 441, 1354, 427);
                SDL_RenderDrawLine(Program.render, 1354, 427, 1285, 571);
                SDL_RenderDrawLine(Program.render, 1285, 571, 1285, 499);
                SDL_RenderDrawLine(Program.render, 1285, 499, 1017, 600);
                SDL_RenderDrawLine(Program.render, 1017, 600, 1032, 538);
                SDL_RenderDrawLine(Program.render, 1032, 538, 700, 600);
                SDL_RenderDrawLine(Program.render, 700, 600, 734, 553);
                SDL_RenderDrawLine(Program.render, 734, 553, 500, 500);
                SDL_RenderDrawLine(Program.render, 500, 500, 600, 500);
                SDL_RenderDrawLine(Program.render, 600, 500, 451, 377);
                SDL_RenderDrawLine(Program.render, 451, 377, 525, 362);

                SDL_SetRenderDrawColor(Program.render, 127, 0, 127, 255);
                SDL_RenderDrawLine(Program.render, 850, 350, 833, 306);
                SDL_RenderDrawLine(Program.render, 833, 306, 842, 271);
                SDL_RenderDrawLine(Program.render, 842, 271, 878, 221);
                SDL_RenderDrawLine(Program.render, 878, 221, 952, 193);
                SDL_RenderDrawLine(Program.render, 952, 193, 1027, 216);
                SDL_RenderDrawLine(Program.render, 1027, 216, 1064, 260);
                SDL_RenderDrawLine(Program.render, 1064, 260, 1076, 299);
                SDL_RenderDrawLine(Program.render, 1076, 299, 1070, 340);

                SDL_RenderDrawLine(Program.render, 828, 315, 798, 324);
                SDL_RenderDrawLine(Program.render, 798, 324, 800, 350);
                SDL_RenderDrawLine(Program.render, 800, 350, 831, 359);
                SDL_RenderDrawLine(Program.render, 831, 359, 1082, 347);
                SDL_RenderDrawLine(Program.render, 1082, 347, 1114, 333);
                SDL_RenderDrawLine(Program.render, 1114, 333, 1112, 309);
                SDL_RenderDrawLine(Program.render, 1112, 309, 1081, 304);
                #endregion
            } // étoiles qui reviennent - fini

            if (Program.gTimer > 30 && Program.gTimer < 2340)
            {
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                SDL_RenderDrawLine(Program.render, 20, 20, 1900, 20);
                SDL_RenderDrawLine(Program.render, 1900, 20, 1900, 680);
                SDL_RenderDrawLine(Program.render, 1900, 680, 20, 680);
                SDL_RenderDrawLine(Program.render, 20, 20, 20, 680);
            }

            if (Program.gTimer > 2400)
            {
                Program.Gamemode = Gamemode.CREDITS;
                Program.player.Init();
                //Program.gFade = 0;
                Program.enemies.Clear();
            }
        }
        public static void Cut_3() // bad end
        {
            if (Program.gTimer == 2 && Program.cutscene_skip)
                Program.gTimer = 1830;

            if (Program.gTimer == 60)
            {
                Son.JouerMusique(ListeAudioMusique.TBOT, false);
                //Program.player.dead = false;
                //Program.gTimer = 780;//
            }

            if (Program.gTimer >= 60 && Program.gTimer < 240)
            {
                #region stars
                if (Program.gTimer == 60)
                {
                    for (int i = 0; i < stars.GetLength(0); i++)
                    {
                        int x = 1500, y = 400;
                        while ((x > 1375 && y > 128 && x < 1676 && y < 402) || (x > 137 && y > 433 && x < 379 && y < 622))
                        {
                            x = Program.RNG.Next(25, 1880);
                            y = Program.RNG.Next(25, 680);
                        }
                        stars[i, 0] = (short)x;
                        stars[i, 1] = (short)y;
                    }
                }
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                for (int i = 0; i < stars.GetLength(0); i++)
                {
                    SDL_RenderDrawPoint(Program.render, stars[i, 0], stars[i, 1]);
                }
                #endregion

                #region Program.player
                Program.player.position = new Vector3(277, 519, 0);
                Program.player.pitch = 0.7f;
                Program.player.roll = 0.8f;
                Program.player.taille = 2;
                Program.player.RenderObject();
                #endregion

                #region enemy 15
                short death_timer = (short)(Program.gTimer - 60);
                if (death_timer > 255)
                    death_timer = 255;
                Program.player.taille = death_timer / 60f + 0.5f;
                Program.player.position = new Vector3(771 - death_timer, 325 + death_timer, 0);
                Program.player.roll = death_timer / 250;
                double sinroll = Sin(Program.player.roll);
                double cosroll = Cos(Program.player.roll);
                float pitchconst = Program.player.pitch + Data.P_PERMA_PITCH;
                for (int i = 0; i < Program.player.modele.Length - 1; i++)
                {
                    if (i % 4 == 0)
                        SDL_SetRenderDrawColor(Program.render, 255, 0, 0, (byte)(255 - death_timer - 60));
                    else
                        SDL_SetRenderDrawColor(Program.render, 0, 0, 0, 255);
                    int[] pos = new int[4] {
                        (int)(Program.player.taille * (cosroll * -Program.player.modele[i].x - sinroll * -Program.player.modele[i].y) + Program.player.position.x),
                        (int)(Program.player.taille * (sinroll * -Program.player.modele[i].x + cosroll * -Program.player.modele[i].y) + Program.player.position.y - Program.player.modele[i].z * pitchconst),
                        (int)(Program.player.taille * (cosroll * -Program.player.modele[i + 1].x - sinroll * -Program.player.modele[i + 1].y) + Program.player.position.x),
                        (int)(Program.player.taille * (sinroll * -Program.player.modele[i + 1].x + cosroll * -Program.player.modele[i + 1].y) + Program.player.position.y - Program.player.modele[i + 1].z * pitchconst)
                    };
                    SDL_RenderDrawLine(Program.render, pos[0], pos[1], pos[2], pos[3]);
                }
                #endregion

                #region pulsar bomb
                BombePulsar.DessinerBombePulsar(new Vector2(1522, 264), 133, true);

                SDL_SetRenderDrawColor(Program.render, 0, 0, 255, 255);
                SDL_RenderDrawLine(Program.render, 1463, 144, 1445, 97);
                SDL_RenderDrawLine(Program.render, 1445, 97, 1422, 68);
                SDL_RenderDrawLine(Program.render, 1422, 68, 1373, 46);
                SDL_RenderDrawLine(Program.render, 1525, 131, 1522, 86);
                SDL_RenderDrawLine(Program.render, 1522, 86, 1554, 48);
                SDL_RenderDrawLine(Program.render, 1554, 48, 1584, 35);
                SDL_RenderDrawLine(Program.render, 1501, 36, 1499, 82);
                SDL_RenderDrawLine(Program.render, 1464, 63, 1484, 112);
                SDL_RenderDrawLine(Program.render, 1500, 396, 1491, 454);
                SDL_RenderDrawLine(Program.render, 1491, 454, 1470, 493);
                SDL_RenderDrawLine(Program.render, 1470, 493, 1450, 534);
                SDL_RenderDrawLine(Program.render, 1549, 395, 1550, 450);
                SDL_RenderDrawLine(Program.render, 1550, 450, 1574, 486);
                SDL_RenderDrawLine(Program.render, 1574, 486, 1612, 513);
                SDL_RenderDrawLine(Program.render, 1514, 434, 1506, 485);
                SDL_RenderDrawLine(Program.render, 1539, 475, 1560, 520);
                #endregion
            } // e15 tué - fini
            else if (Program.gTimer >= 240 && Program.gTimer < 421)
            {
                #region alpha
                short alpha = (short)(255 - (Program.gTimer - 330) * 4);
                if (alpha < 0)
                    alpha = 0;
                #endregion

                #region stars
                if (Program.gTimer == 240)
                {
                    for (int i = 0; i < stars.GetLength(0); i++)
                    {
                        int x = 708, y = 108;
                        while (x > 707 && y > 107 && x < 1195 && y < 574)
                        {
                            x = Program.RNG.Next(25, 1880);
                            y = Program.RNG.Next(25, 680);
                        }
                        stars[i, 0] = (short)x;
                        stars[i, 1] = (short)y;
                    }
                }
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                for (int i = 0; i < stars.GetLength(0); i++)
                {
                    SDL_RenderDrawPoint(Program.render, stars[i, 0], stars[i, 1]);
                }
                #endregion

                #region pulsar bomb
                if (Program.gTimer < 300)
                    BombePulsar.DessinerBombePulsar(new Vector2(956, 336), 224, false);
                else if (Program.gTimer >= 300 && Program.gTimer < 330)
                {
                    SDL_SetRenderDrawColor(Program.render, 200, 255, 255, 255);
                    if (Program.gTimer % Ceiling((Program.gTimer - 299) / 10f) == 0)
                        for (int i = 0; i < neutron_slowdown.Length; i++)
                        {
                            float ang = Program.RNG.NextSingle() * (float)PI;
                            neutron_slowdown[i].x = (short)(Program.RNG.Next(-224, 224) * Cos(ang) + 956);
                            neutron_slowdown[i].y = (short)(Program.RNG.Next(-224, 224) * Sin(ang) + 336);
                        }

                    BombePulsar.DessinerBombePulsar(new Vector2(956, 336), 224, BombePulsar.COULEUR_BOMBE, false, neutron_slowdown);
                }
                else if (Program.gTimer >= 330)
                {
                    Background.DessinerCercle(new Vector2(956, 336), 224, 50);
                    SDL_SetRenderDrawColor(Program.render, 200, 255, 255, (byte)alpha);
                    for (int i = 0; i < 50; i++)
                        SDL_RenderDrawLine(Program.render, (int)neutron_slowdown[i].x, (int)neutron_slowdown[i].y, 956, 336);
                }
                #endregion

                #region bleu
                if (Program.gTimer < 330)
                    SDL_SetRenderDrawColor(Program.render, 0, 0, 255, 255);
                else
                    SDL_SetRenderDrawColor(Program.render, 0, 0, 255, (byte)alpha);

                SDL_RenderDrawLine(Program.render, 834, 148, 811, 86);
                SDL_RenderDrawLine(Program.render, 811, 86, 760, 50);
                SDL_RenderDrawLine(Program.render, 760, 50, 693, 21);
                SDL_RenderDrawLine(Program.render, 935, 113, 930, 61);
                SDL_RenderDrawLine(Program.render, 930, 61, 940, 21);
                SDL_RenderDrawLine(Program.render, 867, 112, 849, 64);
                SDL_RenderDrawLine(Program.render, 849, 64, 831, 37);
                SDL_RenderDrawLine(Program.render, 894, 57, 895, 21);
                SDL_RenderDrawLine(Program.render, 948, 560, 949, 614);
                SDL_RenderDrawLine(Program.render, 949, 614, 931, 679);
                SDL_RenderDrawLine(Program.render, 1041, 543, 1066, 591);
                SDL_RenderDrawLine(Program.render, 1066, 591, 1134, 632);
                SDL_RenderDrawLine(Program.render, 1134, 632, 1207, 654);
                SDL_RenderDrawLine(Program.render, 1032, 578, 1053, 614);
                SDL_RenderDrawLine(Program.render, 1053, 614, 1090, 648);
                SDL_RenderDrawLine(Program.render, 1000, 600, 1001, 640);
                SDL_RenderDrawLine(Program.render, 1001, 640, 989, 679);
                #endregion
            } // bombe désactivée - fini
            else if (Program.gTimer >= 421 && Program.gTimer < 600)
            {
                #region stars
                if (Program.gTimer == 421)
                {
                    int x, y;
                    for (int i = 0; i < stars.GetLength(0); i++)
                    {
                        x = Program.RNG.Next(25, 1880);
                        y = Program.RNG.Next(25, 680);
                        stars[i, 0] = (short)x;
                        stars[i, 1] = (short)y;
                    }
                }
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                for (int i = 0; i < stars.GetLength(0); i++)
                {
                    SDL_RenderDrawPoint(Program.render, stars[i, 0], stars[i, 1]);
                }
                #endregion

                #region portal
                SDL_SetRenderDrawColor(Program.render, 0, 255, 255, 255);
                for (int i = 0; i < 50; i++)
                {
                    float ang = Program.RNG.NextSingle() * (float)PI;
                    SDL_RenderDrawLine(Program.render, (int)(Program.RNG.Next(-150, 150) * Cos(ang) + 960), (int)(Program.RNG.Next(-80, 80) * Sin(ang) + 130), 960, 130);
                }
                #endregion

                #region amis
                if (Program.gTimer == 421)
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

                SDL_SetRenderDrawColor(Program.render, 255, 255, 0, 255);
                for (byte i = 0; i < f_model_pos.GetLength(0); i++)
                {
                    for (byte j = 0; j < f_model.GetLength(0) - 1; j++)
                    {
                        SDL_RenderDrawLine(Program.render,
                            (int)(f_model[j, 0] * f_model_pos[i, 2] + f_model_pos[i, 0]),
                            (int)(f_model[j, 1] * f_model_pos[i, 2] + f_model_pos[i, 1]),
                            (int)(f_model[j + 1, 0] * f_model_pos[i, 2] + f_model_pos[i, 0]),
                            (int)(f_model[j + 1, 1] * f_model_pos[i, 2] + f_model_pos[i, 1]));
                    }
                }
                #endregion
            } // les amis viennent - dini
            else if (Program.gTimer >= 600 && Program.gTimer < 780)
            {
                #region stars
                if (Program.gTimer == 600)
                {
                    for (int i = 0; i < stars.GetLength(0); i++)
                    {
                        int x = 848, y = 63;
                        while ((x > 847 && y > 62 && x < 1250 && y < 448) || y > 448)
                        {
                            x = Program.RNG.Next(25, 1880);
                            y = Program.RNG.Next(25, 680);
                        }
                        stars[i, 0] = (short)x;
                        stars[i, 1] = (short)y;
                    }
                }
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                for (int i = 0; i < stars.GetLength(0); i++)
                {
                    SDL_RenderDrawPoint(Program.render, stars[i, 0], stars[i, 1]);
                }
                #endregion

                #region planète
                SDL_SetRenderDrawColor(Program.render, 255, 0, 0, 255);
                SDL_RenderDrawLine(Program.render, 21, 598, 200, 500);
                SDL_RenderDrawLine(Program.render, 200, 500, 598, 448);
                SDL_RenderDrawLine(Program.render, 598, 448, 1300, 448);
                SDL_RenderDrawLine(Program.render, 1300, 448, 1700, 500);
                SDL_RenderDrawLine(Program.render, 1700, 500, 1899, 600);
                #endregion

                #region vieu drapeau
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                SDL_RenderDrawLine(Program.render, 849, 448, 849, 62);

                if (Program.gTimer < 700)
                {
                    short move = (short)((Program.gTimer - 630) * 8);
                    if (move < 0)
                        move = 0;
                    if (move > 450)
                        move = 450;
                    SDL_SetRenderDrawColor(Program.render, 255, 0, 0, 255);
                    SDL_RenderDrawLine(Program.render, 850, 62 + move, 1247, 60 + move);
                    SDL_RenderDrawLine(Program.render, 1247, 60 + move, 1247, 264 + move);
                    SDL_RenderDrawLine(Program.render, 1247, 264 + move, 850, 273 + move);
                    SDL_RenderDrawLine(Program.render, 850, 273 + move, 850, 62 + move);

                    SDL_SetRenderDrawColor(Program.render, 0, 0, 255, 255);
                    SDL_RenderDrawLine(Program.render, 848, 232 + move, 1191, 61 + move);
                    SDL_RenderDrawLine(Program.render, 1246, 90 + move, 886, 272 + move);

                    SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                    SDL_RenderDrawLine(Program.render, 1177, 166 + move, 1200, 250 + move);
                    SDL_RenderDrawLine(Program.render, 1200, 250 + move, 1136, 200 + move);
                    SDL_RenderDrawLine(Program.render, 1136, 200 + move, 1215, 200 + move);
                    SDL_RenderDrawLine(Program.render, 1215, 200 + move, 1150, 250 + move);
                    SDL_RenderDrawLine(Program.render, 1150, 250 + move, 1177, 166 + move);
                }
                #endregion

                #region nouveau drapeau
                if (Program.gTimer > 715)
                {
                    short move = (short)((Program.gTimer - 715) * 8);
                    if (move < 0)
                        move = 0;
                    if (move > 380)
                        move = 380;
                    SDL_SetRenderDrawColor(Program.render, 255, 255, 0, 255);
                    SDL_RenderDrawLine(Program.render, 850, 449 - move, 1251, 450 - move);
                    SDL_RenderDrawLine(Program.render, 1251, 450 - move, 1256, 680 - move);
                    SDL_RenderDrawLine(Program.render, 1256, 680 - move, 850, 680 - move);
                    SDL_RenderDrawLine(Program.render, 850, 680 - move, 850, 449 - move);

                    SDL_SetRenderDrawColor(Program.render, 255, 127, 0, 255);
                    Background.DessinerCercle(new Vector2(1050, 560 - move), 84, 50);
                    Background.DessinerCercle(new Vector2(1050, 560 - move), 63, 50);

                    SDL_SetRenderDrawColor(Program.render, 0, 255, 0, 255);
                    SDL_RenderDrawLine(Program.render, 850, 518 - move, 978, 518 - move);
                    SDL_RenderDrawLine(Program.render, 976, 599 - move, 848, 599 - move);
                    SDL_RenderDrawLine(Program.render, 1124, 521 - move, 1253, 519 - move);
                    SDL_RenderDrawLine(Program.render, 1124, 600 - move, 1254, 600 - move);

                    SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                    SDL_RenderDrawLine(Program.render, 1050, 498 - move, 1090, 609 - move);
                    SDL_RenderDrawLine(Program.render, 1090, 609 - move, 992, 536 - move);
                    SDL_RenderDrawLine(Program.render, 992, 536 - move, 1106, 533 - move);
                    SDL_RenderDrawLine(Program.render, 1106, 533 - move, 1017, 614 - move);
                    SDL_RenderDrawLine(Program.render, 1017, 614 - move, 1050, 498 - move);
                }
                #endregion

                if (Program.gTimer == 600)
                    rect = new SDL_Rect() { x = 600, y = 449, w = 700, h = 500 };
                SDL_SetRenderDrawColor(Program.render, 0, 0, 0, 255);
                SDL_RenderFillRect(Program.render, ref rect);
            } // drapeau - fini
            else if (Program.gTimer >= 780 && Program.gTimer < 960)
            {
                #region toi
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                SDL_RenderDrawLine(Program.render, 248, 680, 156, 309);
                SDL_RenderDrawLine(Program.render, 156, 309, 303, 243);
                SDL_RenderDrawLine(Program.render, 303, 243, 400, 300);
                SDL_RenderDrawLine(Program.render, 400, 300, 499, 249);
                SDL_RenderDrawLine(Program.render, 499, 249, 630, 310);
                SDL_RenderDrawLine(Program.render, 630, 310, 539, 680);

                Background.DessinerCercle(new Vector2(400, 200), 78, 50);

                SDL_RenderDrawLine(Program.render, 156, 309, 159, 649);
                SDL_RenderDrawLine(Program.render, 630, 310, 685, 215);
                SDL_RenderDrawLine(Program.render, 685, 215, 385, 140);

                SDL_SetRenderDrawColor(Program.render, 255, 255, 0, 255);
                SDL_RenderDrawLine(Program.render, 480, 380, 460, 400);
                SDL_RenderDrawLine(Program.render, 460, 400, 480, 420);
                SDL_RenderDrawLine(Program.render, 480, 420, 500, 400);
                SDL_RenderDrawLine(Program.render, 500, 400, 480, 380);

                SDL_RenderDrawLine(Program.render, 540, 380, 520, 400);
                SDL_RenderDrawLine(Program.render, 520, 400, 540, 420);
                SDL_RenderDrawLine(Program.render, 540, 420, 560, 400);
                SDL_RenderDrawLine(Program.render, 560, 400, 540, 380);

                SDL_SetRenderDrawColor(Program.render, 127, 0, 127, 255);
                SDL_RenderDrawLine(Program.render, 480, 380, 460, 320);
                SDL_RenderDrawLine(Program.render, 460, 320, 500, 320);
                SDL_RenderDrawLine(Program.render, 500, 320, 480, 380);

                SDL_SetRenderDrawColor(Program.render, 127, 255, 127, 255);
                SDL_RenderDrawLine(Program.render, 540, 380, 520, 320);
                SDL_RenderDrawLine(Program.render, 520, 320, 560, 320);
                SDL_RenderDrawLine(Program.render, 560, 320, 540, 380);
                #endregion

                #region chef
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                SDL_RenderDrawLine(Program.render, 865, 680, 800, 300);
                SDL_RenderDrawLine(Program.render, 800, 300, 879, 260);
                SDL_RenderDrawLine(Program.render, 879, 260, 959, 289);
                SDL_RenderDrawLine(Program.render, 959, 289, 1048, 254);
                SDL_RenderDrawLine(Program.render, 1048, 254, 1118, 295);
                SDL_RenderDrawLine(Program.render, 1118, 295, 1057, 680);

                Background.DessinerCercle(new Vector2(959, 205), 76, 50);

                SDL_RenderDrawLine(Program.render, 893, 166, 1031, 164);
                SDL_RenderDrawLine(Program.render, 1031, 164, 1018, 127);
                SDL_RenderDrawLine(Program.render, 1018, 127, 886, 114);
                SDL_RenderDrawLine(Program.render, 886, 114, 876, 132);
                SDL_RenderDrawLine(Program.render, 876, 132, 893, 166);

                SDL_SetRenderDrawColor(Program.render, 255, 255, 0, 255);
                SDL_RenderDrawLine(Program.render, 892, 133, 904, 123);
                SDL_RenderDrawLine(Program.render, 904, 123, 918, 133);
                SDL_RenderDrawLine(Program.render, 918, 133, 906, 145);
                SDL_RenderDrawLine(Program.render, 906, 145, 892, 133);

                SDL_RenderDrawLine(Program.render, 1020, 360, 1000, 380);
                SDL_RenderDrawLine(Program.render, 1000, 380, 1020, 400);
                SDL_RenderDrawLine(Program.render, 1020, 400, 1040, 380);
                SDL_RenderDrawLine(Program.render, 1040, 380, 1020, 360);

                SDL_RenderDrawLine(Program.render, 1080, 360, 1060, 380);
                SDL_RenderDrawLine(Program.render, 1060, 380, 1080, 400);
                SDL_RenderDrawLine(Program.render, 1080, 400, 1100, 380);
                SDL_RenderDrawLine(Program.render, 1100, 380, 1080, 360);

                SDL_RenderDrawLine(Program.render, 1000, 400, 1020, 420);
                SDL_RenderDrawLine(Program.render, 1020, 420, 1040, 400);
                SDL_RenderDrawLine(Program.render, 1040, 400, 1030, 390);
                SDL_RenderDrawLine(Program.render, 1010, 390, 1000, 400);

                SDL_RenderDrawLine(Program.render, 1070, 390, 1060, 400);
                SDL_RenderDrawLine(Program.render, 1060, 400, 1080, 420);
                SDL_RenderDrawLine(Program.render, 1080, 420, 1100, 400);
                SDL_RenderDrawLine(Program.render, 1100, 400, 1090, 390);

                SDL_SetRenderDrawColor(Program.render, 127, 0, 127, 255);
                SDL_RenderDrawLine(Program.render, 1000, 300, 1040, 300);
                SDL_RenderDrawLine(Program.render, 1040, 300, 1020, 360);
                SDL_RenderDrawLine(Program.render, 1020, 360, 1000, 300);

                SDL_SetRenderDrawColor(Program.render, 255, 0, 0, 255);
                SDL_RenderDrawLine(Program.render, 1060, 300, 1100, 300);
                SDL_RenderDrawLine(Program.render, 1100, 300, 1080, 360);
                SDL_RenderDrawLine(Program.render, 1080, 360, 1060, 300);

                SDL_SetRenderDrawColor(Program.render, 127, 127, 255, 255);
                SDL_RenderDrawLine(Program.render, 1000, 340, 1013, 340);
                SDL_RenderDrawLine(Program.render, 1027, 340, 1040, 340);
                SDL_RenderDrawLine(Program.render, 1040, 340, 1030, 370);
                SDL_RenderDrawLine(Program.render, 1011, 369, 1000, 340);

                SDL_SetRenderDrawColor(Program.render, 255, 127, 0, 255);
                SDL_RenderDrawLine(Program.render, 1060, 340, 1073, 340);
                SDL_RenderDrawLine(Program.render, 1087, 340, 1100, 340);
                SDL_RenderDrawLine(Program.render, 1100, 340, 1091, 371);
                SDL_RenderDrawLine(Program.render, 1071, 369, 1060, 340);

                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                SDL_RenderDrawLine(Program.render, 800, 300, 553, 328);
                SDL_RenderDrawLine(Program.render, 531, 340, 1118, 295);
                #endregion

                #region drapeaux
                for (short i = 0; i < 401; i += 200)
                {
                    SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                    SDL_RenderDrawLine(Program.render, 1300 + i, 680, 1300 + i, 494);
                    SDL_RenderDrawLine(Program.render, 1300 + i, 100, 1300 + i, 365);

                    SDL_SetRenderDrawColor(Program.render, 255, 255, 0, 255);
                    SDL_RenderDrawLine(Program.render, 1300 + i, 100, 1387 + i, 272);
                    SDL_RenderDrawLine(Program.render, 1387 + i, 272, 1400 + i, 400);
                    SDL_RenderDrawLine(Program.render, 1400 + i, 400, 1376 + i, 449);
                    SDL_RenderDrawLine(Program.render, 1376 + i, 449, 1369 + i, 519);
                    SDL_RenderDrawLine(Program.render, 1369 + i, 519, 1390 + i, 557);
                    SDL_RenderDrawLine(Program.render, 1390 + i, 557, 1332 + i, 574);
                    SDL_RenderDrawLine(Program.render, 1332 + i, 574, 1282 + i, 449);
                    SDL_RenderDrawLine(Program.render, 1282 + i, 449, 1287 + i, 389);
                    SDL_RenderDrawLine(Program.render, 1287 + i, 389, 1314 + i, 338);
                    SDL_RenderDrawLine(Program.render, 1314 + i, 338, 1315 + i, 305);
                    SDL_RenderDrawLine(Program.render, 1315 + i, 305, 1300 + i, 300);

                    SDL_SetRenderDrawColor(Program.render, 255, 127, 0, 255);
                    Background.DessinerCercle(new Vector2(1346 + i, 375), 32, 50);

                    SDL_SetRenderDrawColor(Program.render, 0, 255, 0, 255);
                    SDL_RenderDrawLine(Program.render, 1368 + i, 352, 1371 + i, 309);
                    SDL_RenderDrawLine(Program.render, 1371 + i, 309, 1346 + i, 253);
                    SDL_RenderDrawLine(Program.render, 1346 + i, 253, 1300 + i, 170);
                    SDL_RenderDrawLine(Program.render, 1339 + i, 343, 1340 + i, 308);
                    SDL_RenderDrawLine(Program.render, 1340 + i, 308, 1321 + i, 268);
                    SDL_RenderDrawLine(Program.render, 1321 + i, 268, 1300 + i, 249);
                    SDL_RenderDrawLine(Program.render, 1325 + i, 400, 1311 + i, 437);
                    SDL_RenderDrawLine(Program.render, 1311 + i, 437, 1318 + i, 488);
                    SDL_RenderDrawLine(Program.render, 1318 + i, 488, 1351 + i, 568);
                    SDL_RenderDrawLine(Program.render, 1375 + i, 561, 1350 + i, 500);
                    SDL_RenderDrawLine(Program.render, 1350 + i, 500, 1339 + i, 441);
                    SDL_RenderDrawLine(Program.render, 1339 + i, 441, 1351 + i, 407);
                }
                #endregion
            } // honneure - fini
            else if (Program.gTimer >= 960 && Program.gTimer < 1140)
            {
                #region toi + chaise
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                SDL_RenderDrawLine(Program.render, 1034, 680, 1030, 521);
                SDL_RenderDrawLine(Program.render, 1030, 521, 1085, 470);
                SDL_RenderDrawLine(Program.render, 1085, 470, 1623, 474);
                SDL_RenderDrawLine(Program.render, 1623, 474, 1684, 523);
                SDL_RenderDrawLine(Program.render, 1684, 523, 1682, 680);

                SDL_RenderDrawLine(Program.render, 1512, 473, 1647, 110);
                SDL_RenderDrawLine(Program.render, 1647, 110, 1715, 80);
                SDL_RenderDrawLine(Program.render, 1715, 80, 1785, 117);
                SDL_RenderDrawLine(Program.render, 1785, 117, 1811, 165);
                SDL_RenderDrawLine(Program.render, 1811, 165, 1684, 523);

                if (Program.gTimer < 1050)
                {
                    SDL_RenderDrawLine(Program.render, 1500, 200, 1449, 203);
                    SDL_RenderDrawLine(Program.render, 1449, 203, 1415, 472);
                    SDL_RenderDrawLine(Program.render, 1513, 253, 1444, 505);
                    SDL_RenderDrawLine(Program.render, 1444, 505, 1249, 508);
                    Background.DessinerCercle(new Vector2(1555, 157), 70, 50);
                }
                else
                {
                    SDL_RenderDrawLine(Program.render, 1246, 471, 1199, 277);
                    SDL_RenderDrawLine(Program.render, 1199, 277, 1400, 250);
                    SDL_RenderDrawLine(Program.render, 1400, 250, 1448, 472);
                    SDL_RenderDrawLine(Program.render, 1199, 277, 1180, 494);
                    SDL_RenderDrawLine(Program.render, 1180, 494, 1078, 539);
                    SDL_RenderDrawLine(Program.render, 1400, 250, 1482, 323);
                    SDL_RenderDrawLine(Program.render, 1482, 323, 1494, 471);
                    Background.DessinerCercle(new Vector2(1266, 220), 70, 50);
                }
                #endregion

                #region fenêtre
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                SDL_RenderDrawLine(Program.render, 800, 100, 1050, 100);
                SDL_RenderDrawLine(Program.render, 1050, 100, 1050, 400);
                SDL_RenderDrawLine(Program.render, 1050, 400, 800, 400);
                SDL_RenderDrawLine(Program.render, 800, 400, 800, 100);
                SDL_RenderDrawLine(Program.render, 800, 250, 1050, 250);
                SDL_RenderDrawLine(Program.render, 925, 100, 925, 400);

                if (Program.gTimer == 960)
                {
                    for (int i = 0; i < stars.GetLength(0) / 2; i++)
                    {
                        stars[i, 0] = (short)Program.RNG.Next(800, 1050);
                        stars[i, 1] = (short)Program.RNG.Next(100, 400);
                        neutron_slowdown[i].x = stars[i, 0];
                        neutron_slowdown[i].y = stars[i, 1];
                    }
                }
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                for (int i = 0; i < stars.GetLength(0) / 2; i++)
                {
                    SDL_RenderDrawPoint(Program.render, stars[i, 0], stars[i, 1]);
                }
                #endregion

                #region table + médailles
                SDL_SetRenderDrawColor(Program.render, 120, 50, 0, 255);
                SDL_RenderDrawLine(Program.render, 20, 200, 700, 200);
                SDL_RenderDrawLine(Program.render, 700, 200, 700, 250);
                SDL_RenderDrawLine(Program.render, 700, 250, 20, 250);

                SDL_SetRenderDrawColor(Program.render, 255, 127, 0, 255);
                SDL_RenderDrawLine(Program.render, 99, 219, 150, 219);
                SDL_RenderDrawLine(Program.render, 150, 219, 127, 300);
                SDL_RenderDrawLine(Program.render, 127, 300, 99, 219);

                SDL_SetRenderDrawColor(Program.render, 127, 255, 127, 255);
                SDL_RenderDrawLine(Program.render, 252, 221, 300, 221);
                SDL_RenderDrawLine(Program.render, 300, 221, 276, 301);
                SDL_RenderDrawLine(Program.render, 276, 301, 252, 221);

                SDL_SetRenderDrawColor(Program.render, 127, 0, 127, 255);
                SDL_RenderDrawLine(Program.render, 400, 223, 450, 223);
                SDL_RenderDrawLine(Program.render, 450, 223, 427, 301);
                SDL_RenderDrawLine(Program.render, 427, 301, 400, 223);

                SDL_SetRenderDrawColor(Program.render, 255, 255, 0, 255);
                SDL_RenderDrawLine(Program.render, 127, 300, 101, 323);
                SDL_RenderDrawLine(Program.render, 101, 323, 127, 348);
                SDL_RenderDrawLine(Program.render, 127, 348, 154, 324);
                SDL_RenderDrawLine(Program.render, 154, 324, 127, 300);

                SDL_RenderDrawLine(Program.render, 276, 301, 250, 325);
                SDL_RenderDrawLine(Program.render, 250, 325, 277, 349);
                SDL_RenderDrawLine(Program.render, 277, 349, 304, 325);
                SDL_RenderDrawLine(Program.render, 304, 325, 276, 301);

                SDL_RenderDrawLine(Program.render, 427, 301, 401, 324);
                SDL_RenderDrawLine(Program.render, 401, 324, 427, 349);
                SDL_RenderDrawLine(Program.render, 427, 349, 454, 325);
                SDL_RenderDrawLine(Program.render, 454, 325, 427, 301);
                #endregion

                #region trophés
                for (short i = 0; i < 301; i += 150)
                {
                    SDL_RenderDrawLine(Program.render, 150 + i, 200, 166 + i, 171);
                    SDL_RenderDrawLine(Program.render, 166 + i, 171, 221 + i, 172);
                    SDL_RenderDrawLine(Program.render, 221 + i, 172, 238 + i, 200);
                    SDL_RenderDrawLine(Program.render, 188 + i, 171, 188 + i, 154);
                    SDL_RenderDrawLine(Program.render, 201 + i, 171, 201 + i, 154);
                    SDL_RenderDrawLine(Program.render, 173 + i, 71, 161 + i, 124);
                    SDL_RenderDrawLine(Program.render, 161 + i, 124, 176 + i, 154);
                    SDL_RenderDrawLine(Program.render, 176 + i, 154, 215 + i, 154);
                    SDL_RenderDrawLine(Program.render, 215 + i, 154, 227 + i, 128);
                    SDL_RenderDrawLine(Program.render, 227 + i, 128, 218 + i, 71);
                    SDL_RenderDrawLine(Program.render, 218 + i, 71, 173 + i, 71);
                    SDL_RenderDrawLine(Program.render, 167 + i, 98, 151 + i, 97);
                    SDL_RenderDrawLine(Program.render, 151 + i, 97, 144 + i, 119);
                    SDL_RenderDrawLine(Program.render, 144 + i, 119, 166 + i, 133);
                    SDL_RenderDrawLine(Program.render, 223 + i, 100, 237 + i, 97);
                    SDL_RenderDrawLine(Program.render, 237 + i, 97, 245 + i, 122);
                    SDL_RenderDrawLine(Program.render, 245 + i, 122, 224 + i, 136);
                }
                #endregion
            } // à la maison - fini
            else if (Program.gTimer >= 1140 && Program.gTimer < 1320)
            {
                for (int i = 0; i < Program.explosions.Count; i++)
                {
                    if (Program.explosions[i].Exist())
                        i--;
                }

                #region bordure
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                SDL_RenderDrawLine(Program.render, 750, 650, 750, 50);
                SDL_RenderDrawLine(Program.render, 750, 50, 1200, 50);
                SDL_RenderDrawLine(Program.render, 1200, 50, 1200, 650);
                SDL_RenderDrawLine(Program.render, 1200, 650, 750, 650);

                SDL_RenderDrawLine(Program.render, 975, 50, 975, 650);
                SDL_RenderDrawLine(Program.render, 750, 350, 1200, 350);

                SDL_SetRenderDrawColor(Program.render, 0, 0, 0, 255);
                rect.x = 700; rect.y = 30; rect.w = 50; rect.h = 650;
                SDL_RenderFillRect(Program.render, ref rect);
                rect.x = 1201; rect.y = 30; rect.w = 50; rect.h = 650;
                SDL_RenderFillRect(Program.render, ref rect);
                rect.x = 700; rect.y = 30; rect.w = 500; rect.h = 20;
                SDL_RenderFillRect(Program.render, ref rect);
                rect.x = 700; rect.y = 651; rect.w = 500; rect.h = 20;
                SDL_RenderFillRect(Program.render, ref rect);
                #endregion

                #region le reste
                if (Program.gTimer == 1140)
                {
                    int x, y;
                    for (int i = 0; i < stars.GetLength(0); i++)
                    {
                        x = Program.RNG.Next(750, 1200);
                        y = Program.RNG.Next(50, 650);
                        stars[i, 0] = (short)x;
                        stars[i, 1] = (short)y;
                    }
                }
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                for (int i = 0; i < stars.GetLength(0); i++)
                {
                    SDL_RenderDrawPoint(Program.render, stars[i, 0], stars[i, 1]);
                }

                if (lTimer == 0)
                {
                    lTimer = (byte)Program.RNG.Next(10, 30);
                    byte ripbozo;
                encore:
                    ripbozo = (byte)Program.RNG.Next(0, stars.GetLength(0));
                    if (stars[ripbozo, 0] == -1)
                        goto encore;
                    new Explosion(new Vector3(stars[ripbozo, 0], stars[ripbozo, 1], (byte)Program.RNG.Next(5, 20)));
                    if (Program.gTimer % 3 == 0)
                    {
                        stars[ripbozo, 0] = -1;
                        stars[ripbozo, 1] = -1;
                    }
                }
                else
                    lTimer--;
                #endregion
            } // big trouble in little window - fini
            else if (Program.gTimer >= 1320 && Program.gTimer < 1800)
            {
                #region toi + chaise
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                SDL_RenderDrawLine(Program.render, 1034, 680, 1030, 521);
                SDL_RenderDrawLine(Program.render, 1030, 521, 1085, 470);
                SDL_RenderDrawLine(Program.render, 1085, 470, 1623, 474);
                SDL_RenderDrawLine(Program.render, 1623, 474, 1684, 523);
                SDL_RenderDrawLine(Program.render, 1684, 523, 1682, 680);

                SDL_RenderDrawLine(Program.render, 1512, 473, 1647, 110);
                SDL_RenderDrawLine(Program.render, 1647, 110, 1715, 80);
                SDL_RenderDrawLine(Program.render, 1715, 80, 1785, 117);
                SDL_RenderDrawLine(Program.render, 1785, 117, 1811, 165);
                SDL_RenderDrawLine(Program.render, 1811, 165, 1684, 523);

                SDL_RenderDrawLine(Program.render, 1318, 225, 1408, 244);
                SDL_RenderDrawLine(Program.render, 1408, 244, 1490, 321);
                SDL_RenderDrawLine(Program.render, 1490, 321, 1527, 433);
                SDL_RenderDrawLine(Program.render, 1272, 317, 1313, 375);
                SDL_RenderDrawLine(Program.render, 1313, 375, 1342, 473);
                SDL_RenderDrawLine(Program.render, 1390, 323, 1296, 531);
                SDL_RenderDrawLine(Program.render, 1296, 531, 1270, 266);
                SDL_RenderDrawLine(Program.render, 1290, 342, 1228, 527);
                SDL_RenderDrawLine(Program.render, 1228, 527, 1212, 256);
                Background.DessinerCercle(new Vector2(1256, 251), 67, 50);
                #endregion

                #region fenêtre
                if (Program.gTimer == 1320)
                    Program.explosions.Clear();

                for (int i = 0; i < Program.explosions.Count; i++)
                {
                    if (Program.explosions[i].Exist())
                        i--;
                }

                SDL_SetRenderDrawColor(Program.render, 0, 0, 0, 255);
                rect.x = 750; rect.y = 30; rect.w = 50; rect.h = 350;
                SDL_RenderFillRect(Program.render, ref rect);
                rect.x = 1051; rect.y = 30; rect.w = 50; rect.h = 350;
                SDL_RenderFillRect(Program.render, ref rect);
                rect.x = 750; rect.y = 80; rect.w = 300; rect.h = 20;
                SDL_RenderFillRect(Program.render, ref rect);
                rect.x = 750; rect.y = 401; rect.w = 300; rect.h = 20;
                SDL_RenderFillRect(Program.render, ref rect);

                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                SDL_RenderDrawLine(Program.render, 800, 100, 1050, 100);
                SDL_RenderDrawLine(Program.render, 1050, 100, 1050, 400);
                SDL_RenderDrawLine(Program.render, 1050, 400, 800, 400);
                SDL_RenderDrawLine(Program.render, 800, 400, 800, 100);

                SDL_RenderDrawLine(Program.render, 800, 250, 1050, 250);
                SDL_RenderDrawLine(Program.render, 925, 100, 925, 400);

                if (Program.gTimer == 1320)
                {
                    for (byte i = 0; i < stars.GetLength(0) / 2; i++)
                    {
                        stars[i, 0] = (short)neutron_slowdown[i].x;
                        stars[i, 1] = (short)neutron_slowdown[i].y;
                    }
                }
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                for (int i = 0; i < stars.GetLength(0) / 2; i++)
                {
                    SDL_RenderDrawPoint(Program.render, stars[i, 0], stars[i, 1]);
                }

                if (lTimer == 0 && Program.gTimer < 1800)
                {
                    lTimer = (byte)Program.RNG.Next(30, 50);
                    byte ripbozo;
                encore2:
                    ripbozo = (byte)Program.RNG.Next(0, stars.GetLength(0) / 2);
                    if (stars[ripbozo, 0] == -1)
                        goto encore2;
                    new Explosion(new Vector3(stars[ripbozo, 0], stars[ripbozo, 1], (byte)Program.RNG.Next(10, 18)));
                    if (Program.gTimer % 3 == 0)
                    {
                        stars[ripbozo, 0] = -1;
                        stars[ripbozo, 1] = -1;
                    }
                }
                else
                    lTimer--;
                #endregion

                #region table + médailles
                SDL_SetRenderDrawColor(Program.render, 120, 50, 0, 255);
                SDL_RenderDrawLine(Program.render, 20, 200, 700, 200);
                SDL_RenderDrawLine(Program.render, 700, 200, 700, 250);
                SDL_RenderDrawLine(Program.render, 700, 250, 20, 250);

                SDL_SetRenderDrawColor(Program.render, 255, 127, 0, 255);
                SDL_RenderDrawLine(Program.render, 99, 219, 150, 219);
                SDL_RenderDrawLine(Program.render, 150, 219, 127, 300);
                SDL_RenderDrawLine(Program.render, 127, 300, 99, 219);

                SDL_SetRenderDrawColor(Program.render, 127, 255, 127, 255);
                SDL_RenderDrawLine(Program.render, 252, 221, 300, 221);
                SDL_RenderDrawLine(Program.render, 300, 221, 276, 301);
                SDL_RenderDrawLine(Program.render, 276, 301, 252, 221);

                SDL_SetRenderDrawColor(Program.render, 127, 0, 127, 255);
                SDL_RenderDrawLine(Program.render, 400, 223, 450, 223);
                SDL_RenderDrawLine(Program.render, 450, 223, 427, 301);
                SDL_RenderDrawLine(Program.render, 427, 301, 400, 223);

                SDL_SetRenderDrawColor(Program.render, 255, 255, 0, 255);
                SDL_RenderDrawLine(Program.render, 127, 300, 101, 323);
                SDL_RenderDrawLine(Program.render, 101, 323, 127, 348);
                SDL_RenderDrawLine(Program.render, 127, 348, 154, 324);
                SDL_RenderDrawLine(Program.render, 154, 324, 127, 300);

                SDL_RenderDrawLine(Program.render, 276, 301, 250, 325);
                SDL_RenderDrawLine(Program.render, 250, 325, 277, 349);
                SDL_RenderDrawLine(Program.render, 277, 349, 304, 325);
                SDL_RenderDrawLine(Program.render, 304, 325, 276, 301);

                SDL_RenderDrawLine(Program.render, 427, 301, 401, 324);
                SDL_RenderDrawLine(Program.render, 401, 324, 427, 349);
                SDL_RenderDrawLine(Program.render, 427, 349, 454, 325);
                SDL_RenderDrawLine(Program.render, 454, 325, 427, 301);
                #endregion

                #region trophés
                for (short i = 0; i < 301; i += 150)
                {
                    SDL_RenderDrawLine(Program.render, 150 + i, 200, 166 + i, 171);
                    SDL_RenderDrawLine(Program.render, 166 + i, 171, 221 + i, 172);
                    SDL_RenderDrawLine(Program.render, 221 + i, 172, 238 + i, 200);
                    SDL_RenderDrawLine(Program.render, 188 + i, 171, 188 + i, 154);
                    SDL_RenderDrawLine(Program.render, 201 + i, 171, 201 + i, 154);
                    SDL_RenderDrawLine(Program.render, 173 + i, 71, 161 + i, 124);
                    SDL_RenderDrawLine(Program.render, 161 + i, 124, 176 + i, 154);
                    SDL_RenderDrawLine(Program.render, 176 + i, 154, 215 + i, 154);
                    SDL_RenderDrawLine(Program.render, 215 + i, 154, 227 + i, 128);
                    SDL_RenderDrawLine(Program.render, 227 + i, 128, 218 + i, 71);
                    SDL_RenderDrawLine(Program.render, 218 + i, 71, 173 + i, 71);
                    SDL_RenderDrawLine(Program.render, 167 + i, 98, 151 + i, 97);
                    SDL_RenderDrawLine(Program.render, 151 + i, 97, 144 + i, 119);
                    SDL_RenderDrawLine(Program.render, 144 + i, 119, 166 + i, 133);
                    SDL_RenderDrawLine(Program.render, 223 + i, 100, 237 + i, 97);
                    SDL_RenderDrawLine(Program.render, 237 + i, 97, 245 + i, 122);
                    SDL_RenderDrawLine(Program.render, 245 + i, 122, 224 + i, 136);
                }
                #endregion

                #region citation
                if (Program.gTimer > 1520)
                    Text.DisplayText("\"c'est un jeu bizarre, la guerre.", new Vector2(20, 700), 3, scroll: (ushort)(Program.gTimer - 1520));
                if (Program.gTimer > 1580)
                    Text.DisplayText(" la seule facon de gagner est de ne pas jouer.\"", new Vector2(20, 739), 3, scroll: (ushort)(Program.gTimer - 1580));
                if (Program.gTimer > 1670)
                    Text.DisplayText("- w.o.p.r.", new Vector2(20, 778), 3, scroll: (ushort)(Program.gTimer - 1670));
                #endregion
            } // la guerre c mauvais - fini

            if (Program.gTimer > 30 && Program.gTimer < 1830)
            {
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                SDL_RenderDrawLine(Program.render, 20, 20, 1900, 20);
                SDL_RenderDrawLine(Program.render, 1900, 20, 1900, 680);
                SDL_RenderDrawLine(Program.render, 1900, 680, 20, 680);
                SDL_RenderDrawLine(Program.render, 20, 20, 20, 680);
            }

            if (Program.gTimer > 1860)
            {
                Program.Gamemode = Gamemode.CREDITS;
                Program.player.Init();
                Program.enemies.Clear();
                Program.explosions.Clear();
            }
        }
        public static void Cut_4() // generique
        {
            if (Program.gTimer == 2 && Program.cutscene_skip)
                Program.gTimer = 2601;

            if (Program.gTimer == 60)
            {
                Son.JouerMusique(ListeAudioMusique.EUGENESIS, false);
                Program.player.HP = 1;
                Program.player.afficher = true;
                //Program.gTimer = 1620;//
            }
            // beats: 60, 300, 540, 780, 1020, 1260, 1500, 1740, 1980, 2240
            //Program.gTimer += 2;//

            #region texte
            if (Program.gTimer >= 60 && Program.gTimer < 500)
            {
                byte alpha = 255;
                if (Program.gTimer >= 60 && Program.gTimer < 145)
                    alpha = (byte)((Program.gTimer - 60) * 3);
                short extra_y = 0;
                if (Program.gTimer >= 300)
                    extra_y = (short)(Program.gTimer - 300);
                Text.DisplayText("dysgenesis", new Vector2(Text.CENTRE, 540 - extra_y * 3), 4, alpha: alpha, scroll: (int)((Program.gTimer - 60) / 5));
            }
            if (Program.gTimer >= 300 && Program.gTimer < 700)
                Text.DisplayText("conception:\nmalcolm gauthier", new Vector2(100, Data.W_HAUTEUR - (Program.gTimer - 300) * 3), 4, scroll: ((int)Program.gTimer - 330) / 5);
            if (Program.gTimer >= 540 && Program.gTimer < 940)
                Text.DisplayText("         modèles:\nmalcolm gauthier", new Vector2(1300, Data.W_HAUTEUR - (Program.gTimer - 540) * 3), 4, scroll: ((int)Program.gTimer - 560) / 5);
            if (Program.gTimer >= 780 && Program.gTimer < 1180)
                Text.DisplayText("programmation:\nmalcolm gauthier", new Vector2(100, Data.W_HAUTEUR - (Program.gTimer - 780) * 3), 4, scroll: ((int)Program.gTimer - 800) / 5);
            if (Program.gTimer >= 1020 && Program.gTimer < 1420)
                Text.DisplayText(" effets sonnores:\nmalcolm gauthier", new Vector2(1300, Data.W_HAUTEUR - (Program.gTimer - 1020) * 3), 4, scroll: ((int)Program.gTimer - 1040) / 5);
            if (Program.gTimer >= 1260 && Program.gTimer < 1660)
                Text.DisplayText("musique", new Vector2(Text.CENTRE, Data.W_HAUTEUR - (Program.gTimer - 1260) * 3), 4, scroll: ((int)Program.gTimer - 1280) / 5);
            if (Program.gTimer >= 1300 && Program.gTimer < 1700)
            {
                Text.DisplayText("\"dance of the violins\"", new Vector2(Text.CENTRE, Data.W_HAUTEUR - (Program.gTimer - 1300) * 3), 3, scroll: ((int)Program.gTimer - 1320) / 5);
                Text.DisplayText("jesse valentine (f-777)", new Vector2(Text.CENTRE, Data.W_HAUTEUR - (Program.gTimer - 1320) * 3), 3, scroll: ((int)Program.gTimer - 1320) / 5);
            }
            if (Program.gTimer >= 1400 && Program.gTimer < 1800)
            {
                Text.DisplayText("\"240 bits per mile\"", new Vector2(Text.CENTRE, Data.W_HAUTEUR - (Program.gTimer - 1400) * 3), 3, scroll: ((int)Program.gTimer - 1420) / 5);
                Text.DisplayText("leon riskin", new Vector2(Text.CENTRE, Data.W_HAUTEUR - (Program.gTimer - 1420) * 3), 3, scroll: ((int)Program.gTimer - 1420) / 5);
            }
            if (Program.gTimer >= 1500 && Program.gTimer < 1900)
            {
                Text.DisplayText("\"dysgenesis\"         \"eugenesis\"", new Vector2(Text.CENTRE, Data.W_HAUTEUR - (Program.gTimer - 1500) * 3), 3, scroll: ((int)Program.gTimer - 1520) / 3);
                Text.DisplayText("malcolm gauthier", new Vector2(Text.CENTRE, Data.W_HAUTEUR - (Program.gTimer - 1520) * 3), 3, scroll: ((int)Program.gTimer - 1520) / 5);
            }
            if (Program.gTimer >= 1600 && Program.gTimer < 2000)
            {
                Text.DisplayText("autres musiques", new Vector2(Text.CENTRE, Data.W_HAUTEUR - (Program.gTimer - 1600) * 3), 3, scroll: ((int)Program.gTimer - 1620) / 5);
                Text.DisplayText("malcolm gauthier, mélodies non-originales", new Vector2(Text.CENTRE, Data.W_HAUTEUR - (Program.gTimer - 1620) * 3), 3, scroll: ((int)Program.gTimer - 1620) / 3);
            }
            if (Program.gTimer >= 1740 && Program.gTimer < 2140)
                Text.DisplayText("mélodies utilisées", new Vector2(100, Data.W_HAUTEUR - (Program.gTimer - 1740) * 3), 4, scroll: ((int)Program.gTimer - 1740) / 5);
            if (Program.gTimer >= 1780 && Program.gTimer < 2180)
            {
                Text.DisplayText("\"can't remove the pain\"", new Vector2(400, Data.W_HAUTEUR - (Program.gTimer - 1780) * 3), 3, scroll: ((int)Program.gTimer - 1800) / 5);
                Text.DisplayText("todd porter et herman miller", new Vector2(400, Data.W_HAUTEUR - (Program.gTimer - 1800) * 3), 3, scroll: ((int)Program.gTimer - 1800) / 5);
            }
            if (Program.gTimer >= 1880 && Program.gTimer < 2280)
            {
                Text.DisplayText("\"pesenka\"", new Vector2(400, Data.W_HAUTEUR - (Program.gTimer - 1880) * 3), 3, scroll: ((int)Program.gTimer - 1900) / 5);
                Text.DisplayText("Sergey Zhukov et Aleksey Potekhin", new Vector2(400, Data.W_HAUTEUR - (Program.gTimer - 1900) * 3), 3, scroll: ((int)Program.gTimer - 1900) / 5);
            }
            if (Program.gTimer >= 1980 && Program.gTimer < 2380)
            {
                Text.DisplayText("\"the beginning of time\"", new Vector2(400, Data.W_HAUTEUR - (Program.gTimer - 1980) * 3), 3, scroll: ((int)Program.gTimer - 2000) / 5);
                Text.DisplayText("nathan ingalls (dj-nate)", new Vector2(400, Data.W_HAUTEUR - (Program.gTimer - 2000) * 3), 3, scroll: ((int)Program.gTimer - 2000) / 5);
            }
            if (Program.gTimer >= 2250)
            {
                byte alpha = 0;
                if (Program.gTimer < 2350)
                    alpha = (byte)((Program.gTimer - 2250) * 2.5f);
                else
                    alpha = 255;
                Text.DisplayText("fin", new Vector2(Text.CENTRE, Text.CENTRE), 5, alpha: alpha);
                if (Program.gTimer > 2350)
                    Text.DisplayText("tapez \"arcade\" au menu du jeu pour accéder au mode arcade!", new Vector2(20, 1050), 2, alpha: (short)((Program.gTimer - 2350) * 10));
            }
            #endregion

            #region ennemis & joueur
            if (Program.gTimer >= 400 && Program.gTimer < 500)
            {
                Program.player.taille = 10 * (float)Pow(0.95f, Program.gTimer - 400);
                Program.player.position.x = (Program.gTimer - 400) * 4 + 1000;
                Program.player.position.y = (float)Pow(Program.gTimer - 450, 2) * -0.1f + 600;
                Program.player.pitch = (Program.gTimer - 400) / 333f;
                if (Program.gTimer >= 460 && Program.gTimer < 480)
                    Program.player.roll = (Program.gTimer - 440) * (float)(PI / 10) + 0.3f;
                else
                    Program.player.roll = (500 - Program.gTimer) * (float)(PI / 300);
                Program.player.RenderObject();
            }
            else if (Program.gTimer >= 600 && Program.gTimer < 900)
            {
                if (Program.gTimer == 600)
                {
                    new Ennemi(TypeEnnemi.CROISSANT, StatusEnnemi.INITIALIZATION);
                    new Ennemi(TypeEnnemi.OCTAHEDRON, StatusEnnemi.INITIALIZATION);
                    new Ennemi(TypeEnnemi.DIAMANT, StatusEnnemi.INITIALIZATION);
                    for (byte i = 0; i < 3; i++)
                    {
                        Program.enemies[i].position.z = -5;
                        Program.enemies[i].pitch = 0.2f;
                    }
                }
                for (byte i = 0; i < 3; i++)
                {
                    Program.enemies[i].position.x = (600 - Program.gTimer) * 8.2f + 2000 + i * 200;
                    Program.enemies[i].position.y = (float)Sin((Program.gTimer - 600) / -10f + i) * 50 + Data.W_SEMI_HAUTEUR;
                }
                if (Program.gTimer == 899)
                {
                    Program.enemies.Clear();
                }
            }
            else if (Program.gTimer >= 1000 && Program.gTimer < 1200)
            {
                for (byte i = 0; i < 2; i++)
                {
                    if (Program.gTimer == 1199)
                    {
                        Program.enemies.Clear();
                        break;
                    }
                    if (Program.gTimer == 1000)
                    {
                        ens[i] = new Ennemi(TypeEnnemi.PATRA, StatusEnnemi.PATRA_8_RESTANT);
                        ens[i].position.z = -5;
                    }
                    if (Program.gTimer > 1150)
                    {
                        if (i == 0)
                            ens[i].position.x = 300 - (float)Pow((Program.gTimer - 1150), 1.9f);
                        else
                            ens[i].position.x = 1000 + (float)Pow((Program.gTimer - 1150), 1.9f);
                        ens[i].position.y = 800 - (float)Pow(Program.gTimer - 1150, 2);
                    }
                    else
                    {
                        ens[i].position.x = 300 + i * 700;
                        ens[i].position.y = (Program.gTimer - 1000) * -3 + 1250;
                    }
                }

                if (Program.gTimer == 1199)
                {
                    Program.enemies.Remove(ens[0]);
                    Program.enemies.Remove(ens[1]);
                }
            }
            else if (Program.gTimer >= 1300 && Program.gTimer < 1600)
            {
                for (byte i = 0; i < 2; i++)
                {
                    if (Program.gTimer == 1599)
                    {
                        Program.enemies.Remove(ens[i]);
                        break;
                    }
                    if (Program.gTimer == 1300)
                    {
                        ens[i] = new Ennemi(TypeEnnemi.ENERGIE, StatusEnnemi.INITIALIZATION);
                        ens[i].position.z = -5;
                    }
                    ens[i].position.x = 200 + i * 1500 + (Program.gTimer - 1300);
                    ens[i].position.y = (Program.gTimer - 1300) * -4 + 1150 + i * 200;
                }
            }
            else if (Program.gTimer >= 1620 && Program.gTimer < 1830)
            {
                if (Program.gTimer == 1620)
                {
                    ens[1] = new Ennemi(TypeEnnemi.TOURNANT, StatusEnnemi.INITIALIZATION);
                    ens[1].position.z = 10;
                }

                ens[1].position.x = 1000;
                ens[1].position.y = (Program.gTimer - 1620) * -3 + 1150;

                if (Program.gTimer == 1829)
                {
                    new Explosion(new Vector3(ens[1].position.x, ens[1].position.y, 40));
                    Program.enemies.Clear();
                }
            }
            else if (Program.gTimer >= 1900 && Program.gTimer < 2000)
            {
                if (Program.gTimer == 1900)
                {
                    ens[0] = new Ennemi(TypeEnnemi.DUPLIQUEUR, StatusEnnemi.INITIALIZATION);
                    ens[0].position.z = -5;
                    ens[0].pitch = -0.6f;
                }
                ens[0].roll = 0.5f * (float)Sin(Program.gTimer / 6f);
                ens[0].position.y = (Program.gTimer - 1900) * -3f + 1100;
                ens[0].position.x = (Program.gTimer - 1900) * -8 + 1950;
                if (Program.gTimer == 1999)
                {
                    ens[1] = new Ennemi(TypeEnnemi.DUPLIQUEUR, StatusEnnemi.INITIALIZATION);
                    ens[1].pitch = -0.6f;
                    ens[0].position.z = 5;
                    ens[1].position.z = 5;
                }
            }
            else if (Program.gTimer > 2240)
            {
                Program.player.taille = 1;
                Program.player.roll = (float)Sin(Program.gTimer / 8f) / 5f;
                Program.player.pitch = 0.3f;
                Program.player.position.x = Data.W_SEMI_LARGEUR;
                Program.player.position.y = Data.W_SEMI_HAUTEUR + 100;
                byte alpha;
                if (Program.gTimer < 2350)
                    alpha = (byte)((Program.gTimer - 2240) * 2.33f);
                else
                    alpha = 255;
                Program.player.couleure.a = alpha;
                Program.player.RenderObject();
            }

            if (Program.gTimer >= 1600 && Program.gTimer < 1750)
            {
                if (Program.gTimer == 1749)
                {
                    Program.enemies.Remove(ens[0]);
                }
                else
                {
                    if (Program.gTimer == 1600)
                        ens[0] = new Ennemi(TypeEnnemi.BOSS, StatusEnnemi.INITIALIZATION);

                    ens[0].position.z = 40 - Pow((Program.gTimer - 1600) / 33f, 3);
                    ens[0].pitch = (Program.gTimer - 1600) / -200f;
                    ens[0].roll = (1600 - (int)Program.gTimer) * (float)(PI / 600);

                    if (Program.gTimer < 1650)
                        ens[0].position.x = 1500 + (Program.gTimer - 1600) * 10;
                    else
                        ens[0].position.x = -600 + (Program.gTimer - 1600) * 10;

                    ens[0].position.y = 200 * (float)Sin((Program.gTimer - 1600) / 50f) + Data.W_SEMI_HAUTEUR;
                    ens[0].couleure.a = Program.gTimer < 1620 ? (byte)((Program.gTimer - 1600) * 12f) : (byte)255;
                }
            }
            else if (Program.gTimer >= 1770 && Program.gTimer < 1850)
            {
                if (Program.gTimer == 1770)
                {
                    Program.player.taille = 2;
                    Program.player.pitch = 0.3f;
                    Program.player.roll = -0.7f;
                    Program.player.position.y = 750;
                }
                Program.player.position.x = 2050 - (Program.gTimer - 1770) * 30;
                Program.player.RenderObject();

                if (Program.gTimer == 1805)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        float[] line = Program.player.RenderLineData(Program.player.indexs_de_tir[i % 2]);
                        new Projectile(
                            new Vector3(line[0], line[1], Program.player.position.z),
                            new Vector3(
                                Program.player.position.x + 10 + i % 2 * 2 * -10,
                                Program.player.position.y - 200,
                                Data.G_MAX_DEPTH
                            ),
                            ProprietaireProjectile.JOUEUR,
                            (byte)(i % 2)
                        );
                    }
                }
            }
            else if (Program.gTimer >= 1999 && Program.gTimer < 2200)
            {
                if (Program.gTimer == 2199)
                {
                    Program.enemies.Clear();
                }
                else
                {
                    for (byte i = 0; i < 2; i++)
                    {
                        Program.enemies[i].roll = 0.5f * (float)Sin(Program.gTimer / 6f);
                        Program.enemies[i].position.x = (Program.gTimer - 1900) * -8 + 1950;
                    }
                    Program.enemies[0].position.y = (Program.gTimer - 1900) * -3f + 1100 + (float)Sqrt(Program.gTimer - 1999) * 18;
                    Program.enemies[1].position.y = (Program.gTimer - 1900) * -3f + 1100 - (float)Sqrt(Program.gTimer - 1999) * 18;
                }
            }

            foreach (Ennemi e in Program.enemies)
            {
                e.RenderObject();
                e.UpdateModele();
                e.timer++;
            }
            for (int i = 0; i < Program.explosions.Count; i++)
            {
                if (Program.explosions[i].Exist())
                    i--;
            }
            for (int i = 0; i < Program.projectiles.Count; i++)
            {
                Program.projectiles[i].RenderObject();
                if (Program.projectiles[i].Exist())
                    i--;
            }

            #endregion

            #region gfade
            if (Program.gTimer > 2550)
                gFade = (byte)((Program.gTimer - 2550) * 5);
            if (gFade != 0)
            {
                rect.x = 0; rect.y = 0; rect.w = Data.W_LARGEUR; rect.h = Data.W_HAUTEUR;
                SDL_SetRenderDrawColor(Program.render, 0, 0, 0, gFade);
                SDL_RenderFillRect(Program.render, ref rect);
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

            if (Program.gTimer > 2600)
            {
                Program.Gamemode = Gamemode.TITLESCREEN;
                Son.JouerMusique(ListeAudioMusique.DYSGENESIS, true);
                Program.player.Init();
                Program.player.afficher = true;
                gFade = 0;
                Program.bombe.HP_bombe = BombePulsar.BOMBE_PULSAR_MAX_HP;
            }
        }
    }

    // classe qui s'occupe du curseur sur le menu principal
    // TODO: rendre plus général
    public class Curseur : Sprite
    {
        const int CURSEUR_DAS = Data.G_FPS / 4;
        const int CURSEUR_X_INIT = 810;
        const int CURSEUR_Y_INIT = 625;
        const int CURSEUR_ESPACE = 50;
        readonly Vector3[] curseur_data =
        {
            new(-15, -15, 0),
            new(15, 0, 0),
            new(-15, 15, 0),
            new(-12, 0, 0),
            new(-15, -15, 0)
        }; // modèle du curseur

        public int curseur_max_selection = 0;
        public int selection = -1;
        byte curseur_option_selectionnee = 0;

        public Curseur()
        {
            modele = curseur_data;
        }

        public override bool Exist()
        {
            if (timer > CURSEUR_DAS)
            {
                if (Program.TouchePesee(Touches.J))
                {
                    timer = 0;
                    selection = curseur_option_selectionnee;
                    return true;
                }

                if (Program.TouchePesee(Touches.W) && curseur_option_selectionnee > 0)
                {
                    timer = 0;
                    curseur_option_selectionnee--;
                }
                if (Program.TouchePesee(Touches.S) && curseur_option_selectionnee < curseur_max_selection - 1)
                {
                    timer = 0;
                    curseur_option_selectionnee++;
                }
            }
            else
            {
                if (!Program.TouchePesee(Touches.W) && !Program.TouchePesee(Touches.S))
                {
                    timer = CURSEUR_DAS;
                }
            }

            timer++;
            selection = -1;
            return false;
        }

        public override void RenderObject()
        {
            SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
            for (int i = 0; i < curseur_data.Length - 1; i ++)
            {
                SDL_RenderDrawLineF(Program.render,
                    CURSEUR_X_INIT + curseur_data[i].x,
                    CURSEUR_Y_INIT + curseur_data[i].y + CURSEUR_ESPACE * curseur_option_selectionnee,
                    CURSEUR_X_INIT + curseur_data[i + 1].x,
                    CURSEUR_Y_INIT + curseur_data[i + 1].y + CURSEUR_ESPACE * curseur_option_selectionnee
                );
            }
        }
    }

    // Classe pour les explosions
    public class Explosion : Sprite
    {
        const int DENSITE_EXPLOSION = 50; // nb de lignes dessiné par l'explosion

        // la position Z est utilisée pour déterminer la taille de l'explosion
        public Explosion(Vector3 position)
        {
            this.position = position;

            // évite une division par zéro dans le code rendering
            Math.Clamp(this.position.z, 0f, Data.G_MAX_DEPTH - 1);

            // les explosions utilisés dans les scènes ne doivent pas faire de bruit
            if (Program.GamemodeAction())
                Son.JouerEffet(ListeAudioEffets.EXPLOSION_ENNEMI);

            Program.explosions.Add(this);
        }

        // Continue la vie de l'objet explosion
        // retourne vrai si l'explosion est terminée, et n'existe plus
        public override bool Exist()
        {
            if (timer <= 0)
            {
                Program.explosions.Remove(this);
                return true;
            }

            timer--;
            return false;
        }

        // dessine l'explosion à l'écran
        public override void RenderObject()
        {
            // division par zéro évitée dans le constructeur
            byte rayon = (byte)(100.0f / (Data.G_MAX_DEPTH - position.z) + 5);

            for (int i = 0; i < DENSITE_EXPLOSION; i++)
            {
                float angle = Program.RNG.NextSingle() * PI;
                float sin_ang = Sin(angle);
                float cos_ang = Cos(angle);

                // couleure rouge-orange-jaune au hasard
                SDL_SetRenderDrawColor(Program.render,
                    (byte)Program.RNG.Next(128, 256),
                    (byte)Program.RNG.Next(0, 128),
                    0,
                    255
                );

                // dessine des lignes au hasard dans un cercle
                SDL_RenderDrawLine(Program.render,
                    (int)(Program.RNG.Next(-rayon, rayon) * cos_ang + position.x),
                    (int)(Program.RNG.Next(-rayon, rayon) * sin_ang + position.y),
                    (int)(Program.RNG.Next(-rayon, rayon) * cos_ang + position.x),
                    (int)(Program.RNG.Next(-rayon, rayon) * sin_ang + position.y)
                );
            }
        }
    }

    // Classe qui s'occupe de la musique, les effets sonnores, et le volume
    public static class Son
    {
        const int NB_CHAINES_SFX = 20;
        const int ALL_CHUNKS = -1;
        static SDL_Rect boite_volume = new SDL_Rect()
        {
            x = Data.W_LARGEUR - 360,
            y = 10,
            w = Data.W_HAUTEUR - 730,
            h = 100
        }; // ne peut pas être readonly, RenderRect n'aime pas ca

        static readonly Dictionary<ListeAudioEffets, string> chemins_pour_effets = new()
        {
            { ListeAudioEffets.PRESENTE, @"audio\presents.wav" },
            { ListeAudioEffets.NIVEAU, @"audio\sfx1.wav" },
            { ListeAudioEffets.TIR, @"audio\laserShoot.wav" },
            { ListeAudioEffets.EXPLOSION_ENNEMI, @"audio\explosion_enemy.wav" },
            { ListeAudioEffets.EXPLOSION_JOUEUR, @"audio\explosion.wav" },
            { ListeAudioEffets.POWERUP, @"audio\powerup.wav" },
            { ListeAudioEffets.VAGUE, @"audio\synth.wav" },
            { ListeAudioEffets.DOTV_ENTREE, @"audio\tone.wav" },
        };
        static readonly Dictionary<ListeAudioMusique, string> chemins_pour_musique = new()
        {
            { ListeAudioMusique.CRTP, @"audio\cant remove the pain.wav" },
            { ListeAudioMusique.DYSGENESIS, @"audio\titlescreen.wav" },
            { ListeAudioMusique.ATW, @"audio\around the world.wav" },
            { ListeAudioMusique.TBOT, @"audio\the beginning of Time.wav" },
            { ListeAudioMusique.DOTV, @"audio\Dance of the Violins.wav" },
            { ListeAudioMusique.EUGENESIS, @"audio\eugenesis.wav" },
            { ListeAudioMusique.DCQBPM, @"audio\240 Bits Per Mile.wav" }
        };

        static IntPtr musique = IntPtr.Zero;
        static IntPtr[] effets_sonnores = new IntPtr[NB_CHAINES_SFX];

        static int timer = 0;
        static int prochain_chunk = 0;
        static byte volume = 8;
        private static int volume_SDL = 64;
        static bool render = false;

        // initialize SDL mixer
        public static int InitSDLMixer()
        {
            // 44100 hz, format défaut, mono, chunk = 2kb
            if (Mix_OpenAudio(MIX_DEFAULT_FREQUENCY, MIX_DEFAULT_FORMAT, 1, 2048) != 0)
                return -1;

            Mix_AllocateChannels(NB_CHAINES_SFX);

            return 0;
        }

        // arrête la musique
        public static void StopMusic()
        {
            Mix_PauseMusic();
        }

        // Change la musique pour une nouvelle dans la liste de musiques. Il ne peux seulement avoir
        // une musique qui joue à la fois.
        public static int JouerMusique(ListeAudioMusique musique_a_jouer, bool boucle)
        {
            if (Program.mute_sound)
                return 0;

            // -1 boucles dit à SDL de jouer la musique à le plus possible
            int loupes = boucle ? -1 : 1;

            chemins_pour_musique.TryGetValue(musique_a_jouer, out string? chemin);

            if (chemin == null)
                return -1;

            Mix_FreeMusic(musique);
            musique = Mix_LoadMUS(chemin);

            if (musique == IntPtr.Zero)
                return -2;
            if (Mix_PlayMusic(musique, loupes) != 0)
                return -3;

            Mix_VolumeMusic(volume_SDL);

            return 0;
        }

        // Joue un effet sonnore de la liste d'effets sonnores dans le prochain "chunk" de la boucle
        public static int JouerEffet(ListeAudioEffets effet_a_jouer)
        {
            if (Program.mute_sound)
                return 0;

            chemins_pour_effets.TryGetValue(effet_a_jouer, out string? chemin);

            // si objet effet_a_jouer n'a pas de chemin corrrespondant
            if (chemin == null)
                return -1;

            Mix_FreeChunk(effets_sonnores[prochain_chunk]);
            effets_sonnores[prochain_chunk] = Mix_LoadWAV(chemin);

            // si chemin ou fichier invalide
            if (effets_sonnores[prochain_chunk] == IntPtr.Zero)
                return -2;

            if (Mix_PlayChannel(prochain_chunk + 1, effets_sonnores[prochain_chunk], 0) < 0)
                return -3;

            Mix_Volume(ALL_CHUNKS, volume_SDL);

            // cycle à travers la liste de "chunks" donnés. Ceci est la meilleure façon d'avoir ce
            // système sans trous de mémoire
            prochain_chunk++;
            prochain_chunk %= NB_CHAINES_SFX;

            return 0;
        }

        // vérifie les touches du joueur pour changer le volume, et puis indique si il faut afficher
        // la boite de volume
        public static void ChangerVolume()
        {
            const int MAX_VOLUME_GUI = 16;
            const int VOLUME_TEMPS_AFFICHAGE = Data.G_FPS / 2; // nb d'images que la boite reste après que les touches soient lâchés
            const int VOLUME_DAS = Data.G_FPS / 12; // nb d'images entre les incréments/décrements du volume

            if (Program.TouchePesee(Touches.PLUS) || Program.TouchePesee(Touches.MINUS))
            {
                if (timer < VOLUME_TEMPS_AFFICHAGE - VOLUME_DAS)
                {
                    timer = VOLUME_TEMPS_AFFICHAGE;

                    if (Program.TouchePesee(Touches.PLUS) && volume < MAX_VOLUME_GUI)
                        volume++;
                    if (Program.TouchePesee(Touches.MINUS) && volume > 0)
                        volume--;

                    if (musique == IntPtr.Zero)
                        return;

                    // le jeu utilise 0 à MAX_VOLUME_GUI, SDL utilise 0 à 128
                    volume_SDL = (int)(MIX_MAX_VOLUME * (volume / (float)MAX_VOLUME_GUI));
                    Mix_VolumeMusic(volume_SDL);
                    Mix_Volume(ALL_CHUNKS, volume_SDL);
                }
            }

            render = false;
            if (timer > 0)
            {
                timer--;
                render = true;
            }

            return;
        }

        // affiche la boite volume à l'écran au besoin
        public static void RenderVolume()
        {
            if (!render)
                return;

            SDL_SetRenderDrawColor(Program.render, 0, 0, 0, 255);
            SDL_RenderFillRect(Program.render, ref boite_volume);
            SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
            SDL_RenderDrawRect(Program.render, ref boite_volume);

            Text.DisplayText("volume: " + volume, new Vector2(boite_volume.x + 40, boite_volume.y + 30), 3);
        }
    }
}