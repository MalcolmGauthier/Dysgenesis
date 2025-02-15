﻿using static SDL2.SDL;
using static SDL2.SDL_mixer;
#pragma warning disable CA1806

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

    // classe statique qui gère la bombe pulsar, ou n'importe quoi qui à a faire avec
    // la bombe pulsar
    public static class BombePulsar
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

        public static short HP_bombe = BOMBE_PULSAR_MAX_HP;
        static bool bombe_frapee = false;
        static int timer = 0;

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
            Program.DessinerCercle(position, rayon, 50);

            // si la bombe est frapée par un projectile, on la dessine avec du rouge pour une image
            if (bombe_frapee)
            {
                SDL_SetRenderDrawColor(Program.render, 255, 0, 0, 255);
                bombe_frapee = false;
            }

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
                angle = Program.RNG.NextSingle() * MathF.PI;

                SDL_RenderDrawLineF(Program.render,
                    Program.RNG.Next(-rayon, rayon) * MathF.Cos(angle) + position.x,
                    Program.RNG.Next(-rayon, rayon) * MathF.Sin(angle) + position.y,
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
        public static int AnimationExplosion()
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
                Program.Gamemode = Gamemode.CUTSCENE_BONNE_FIN;
                HP_bombe = BOMBE_PULSAR_MAX_HP;
            }

            return 1;
        }

        // vérifie la collision avec les projectiles du joueur
        public static void VerifCollision()
        {
            // ce code roule seulement si niveau 20 et monologue fini
            if (Program.enemies[0].status != StatusEnnemi.BOSS_NORMAL)
                return;

            for (int i = 0; i < Program.projectiles.Count; i++)
            {
                if (Program.projectiles[i].position.z < Program.G_MAX_DEPTH - 1)
                    continue;

                float[] positions = Program.projectiles[i].PositionsSurEcran();
                if (Vector2.Distance(positions[2], positions[3], Program.W_SEMI_LARGEUR, Program.W_SEMI_HAUTEUR / 2) < 20)
                {
                    HP_bombe--;
                    new Explosion(new Vector3(positions[2], positions[3], Program.G_MAX_DEPTH / 4));
                    bombe_frapee = true;
                }
            }
        }

        // logique
        public static bool Exist()
        {
            if (AnimationExplosion() == 0)
                VerifCollision();

            return false;
        }
    }

    // classe statique qui gère les étoiles dans le fond d'écran.
    // les fonctions peuvent êtres appelées avec des spécifications
    public static class Etoiles
    {
        public const int DENSITE_ETOILES = 100;
        const int RAYON_DEBUT_ETOILES = 100;
        const float VITESSE_ETOILES = 1.02f;

        public static List<Vector2> positions_etoiles = new();

        // remplit positions_etoiles avec des positions dans bounds
        public static void CreerEtoiles(SDL_Rect bounds, int limite)
        {
            positions_etoiles = new List<Vector2>(limite);

            for (int i = 0; i < limite; i++)
            {
                positions_etoiles.Add(new Vector2(
                    Program.RNG.Next(bounds.x, bounds.x + bounds.w),
                    Program.RNG.Next(bounds.y, bounds.y + bounds.h)
                ));
            }
        }

        // bouge les étoiles, pourqu'ils vont vers le bord de l'écran
        public static void Bouger()
        {
            for (int i = 0; i < positions_etoiles.Count; i++)
            {
                positions_etoiles[i] = new Vector2(
                    (positions_etoiles[i].x - Program.W_SEMI_LARGEUR) * VITESSE_ETOILES + Program.W_SEMI_LARGEUR,
                    (positions_etoiles[i].y - Program.W_SEMI_HAUTEUR) * VITESSE_ETOILES + Program.W_SEMI_HAUTEUR
                );

                // si l'étoile est hors de l'écran
                if (
                    positions_etoiles[i].x > Program.W_LARGEUR ||
                    positions_etoiles[i].y > Program.W_HAUTEUR ||
                    positions_etoiles[i].x < 0 ||
                    positions_etoiles[i].y < 0
                    )
                {
                    positions_etoiles[i] = new Vector2(
                        Program.RNG.Next(Program.W_SEMI_LARGEUR - RAYON_DEBUT_ETOILES, Program.W_SEMI_LARGEUR + RAYON_DEBUT_ETOILES),
                        Program.RNG.Next(Program.W_SEMI_HAUTEUR - RAYON_DEBUT_ETOILES, Program.W_SEMI_HAUTEUR + RAYON_DEBUT_ETOILES)
                    );

                    // une étoile qui se trouve exactement au centre de l'écran ne bougera pas, alors on doit le tasser un peu
                    if (positions_etoiles[i].x == Program.W_SEMI_LARGEUR && positions_etoiles[i].y == Program.W_SEMI_HAUTEUR)
                        positions_etoiles[i] = new Vector2(positions_etoiles[i].x + 1, positions_etoiles[i].y);
                }
            }
        }

        // dessine les étoiles à l'écran.
        // limite permet de dessiner moins d'étoiles, mais c'est seulement fait pendant les scènes
        public static void Render(int limite)
        {
            limite = Math.Clamp(limite, 0, positions_etoiles.Count);

            SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);

            for (int i = 0; i < limite; i++)
                SDL_RenderDrawPointF(Program.render, positions_etoiles[i].x, positions_etoiles[i].y);
        }
    }

    // classe statique qui aide à convertir des string en texte écrit dans le jeu.
    // cette classe est très utile pour faire du débug dans n'importe quel projet SDL,
    // et donc cette classe est écrite d'une façon où c'est très facile de la modifier
    // pour qu'elle fonctionne en C et C++.
    public static class Text
    {
        // documentation texte
        // 
        //   text: le texte qui sera affiché à l'écran
        //         charactères supportés: a-z, 0-9, +, -, é, è, ê, à,  , ., ,, ', :, \, /, ", (, ), \n
        //         lettres sont majuscule seuelement, mais le texte qui rentre dans la fonction doit être minuscule, les majuscules seront automatiquement
        //         convertis en minuscules avant d'êtres déssinés.
        //         \n fonctionne et est la seule facon de passer à une prochaine ligne dans le même appel de texte, et quand la ligne est sautée, il revient
        //         au x de départ.
        //
        //   x, y: position haut gauche du premier charactère affiché.
        //         mettre Text.CENTRE (ou -2147483648) pour centrer le texte au millieu de l'écran.
        //
        //   size: nombre qui donne le multiple de la largeure et hauteure.
        //         la largeur d'un charactère sera de 5 * size px, et la hauteur de 10 * size px.
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

        public const float LARGEUR_DEFAUT = 5.0f;
        public const float HAUTEUR_DEFAUT = 10.0f;
        public const float ESPACE_DEFAUT = 3.0f;

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

        // rappel que cette classe est spécifiquement faite pour être facile à copier dans un fichier C/C++. Détails plus haut.
        private static int GetListEntry(char c)
        {
            if (c >= 'a' && c <= 'z')
                return char_draw_info_starting_indexes[c - 'a'];
            else if (c >= '0' && c <= '9')
                return char_draw_info_starting_indexes[c - '0' + 26];
            else
            {
                switch (c)
                {
                    case '.':
                        return char_draw_info_starting_indexes[36];
                    case ':':
                        return char_draw_info_starting_indexes[37];
                    case ',':
                        return char_draw_info_starting_indexes[38];
                    case '\'':
                        return char_draw_info_starting_indexes[39];
                    case 'é':
                        return char_draw_info_starting_indexes[40];
                    case 'è':
                        return char_draw_info_starting_indexes[41];
                    case 'ê':
                        return char_draw_info_starting_indexes[42];
                    case 'à':
                        return char_draw_info_starting_indexes[43];
                    case '"':
                        return char_draw_info_starting_indexes[44];
                    case '-':
                        return char_draw_info_starting_indexes[45];
                    case '/':
                        return char_draw_info_starting_indexes[46];
                    case '\\':
                        return char_draw_info_starting_indexes[47];
                    case '(':
                        return char_draw_info_starting_indexes[48];
                    case ')':
                        return char_draw_info_starting_indexes[49];
                    case '+':
                        return char_draw_info_starting_indexes[50];
                }
            }

            return 0;
        }
        public static void DisplayText(string text, Vector2 position, float size,
            int color = BLANC, int alpha = OPAQUE, int scroll = NO_SCROLL)
        {
            if (scroll <= 0)
                return;

            if (alpha <= 0)
                return;

            string working_text = text.ToLower();

            short extra_y = 0;
            int return_length = 0;

            scroll = Math.Clamp(scroll, 0, text.Length);
            if (scroll > text.Length)
                scroll = text.Length;

            alpha = Math.Clamp(alpha, 0, byte.MaxValue);

            if (position.x == CENTRE)
                position.x = Program.W_SEMI_LARGEUR - ((LARGEUR_DEFAUT + ESPACE_DEFAUT) * size * text.Length - 1) / 2;

            if (position.y == CENTRE)
                position.y = Program.W_SEMI_HAUTEUR - (HAUTEUR_DEFAUT * size) / 2;

            SDL_SetRenderDrawColor(Program.render, (byte)((color >> 16) & 0xFF), (byte)((color >> 8) & 0xFF), (byte)(color & 0xFF), (byte)alpha);

            float x;
            float y;
            int current_info_index;
            for (int i = 0; i < scroll; i++)
            {
                y = position.y + extra_y;
                x = position.x + i * (LARGEUR_DEFAUT + ESPACE_DEFAUT) * size - return_length;

                if (x + size * LARGEUR_DEFAUT * 4 > Program.W_LARGEUR)
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

                    SDL_RenderDrawLineF(Program.render,
                        char_draw_info[current_info_index] * size + x,
                        char_draw_info[current_info_index + 1] * size + y,
                        char_draw_info[current_info_index + 2] * size + x,
                        char_draw_info[current_info_index + 3] * size + y
                    );

                    current_info_index += 4;
                }
            }

        }
    }

    // facilement la pire classe dans le jeu. le code est terrible et illisible. je n'ai pas envie de le documenter et encore moins le réparer.
    // tout est hard-codé, c'est terrible. désolé en avence.
    public static class Cutscene
    {
        static Ennemi[] ens = new Ennemi[2];
        static Vector2[] neutron_slowdown = new Vector2[50];
        static SDL_Rect rect = new SDL_Rect();
        static short[,] stars = new short[50, 2];
        static short[,] stars_glx = new short[300, 2];
        static float[,] f_model_pos = new float[7, 3];
        static sbyte[,] f_model = new sbyte[7, 2]
        {
            { -25, -10},
            { 0, 30},
            { 25, -10},
            { 0, 0},
            { 0, 30},
            { 0, 0},
            { -25, -10}
        };
        static short temp = 0;
        static byte lTimer = 10;
        static byte gFade = 0;

        public static void Cut_0() // intro
        {
            if (Program.cutscene_skip_complet)
                Program.gTimer = 451;

            if (Program.gTimer == 75)
            {
                Son.JouerEffet(ListeAudioEffets.PRESENTE);
            }

            if (Program.gTimer >= 75 && Program.gTimer < 150)
            {
                Text.DisplayText("malcolm gauthier", (Text.CENTRE, Text.CENTRE), 2);
                Text.DisplayText("\n présente", (Text.CENTRE, Text.CENTRE), 2);

            }
            else if (Program.gTimer >= 150 && Program.gTimer <= 225)
            {
                Text.DisplayText("malcolm gauthier", (Text.CENTRE, Text.CENTRE), 2, alpha: (short)((225 - Program.gTimer) * 3.4f));
                Text.DisplayText("\n présente", (Text.CENTRE, Text.CENTRE), 2, alpha: (short)((225 - Program.gTimer) * 3.4f));
            }

            if (Program.gTimer > 225)
            {
                Program.Gamemode = Gamemode.TITLESCREEN;
                Son.JouerMusique(ListeAudioMusique.DYSGENESIS, true);
            }
        }
        public static void Cut_1() // new game
        {
            if (Program.cutscene_skip_partiel || Program.cutscene_skip_complet)
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
                                 "existent à travers la galaxie.", (20, 700), 3, scroll: (ushort)(Program.gTimer - 60));

                SDL_SetRenderDrawColor(Program.render, 255, 127, 0, 255);
                DessinerCercle((960, 440), 200, 50);
                SDL_SetRenderDrawColor(Program.render, 255, 0, 127, 255);
                DessinerCercle((260, 390), 100, 50);
                SDL_SetRenderDrawColor(Program.render, 255, 127, 127, 255);
                DessinerCercle((1660, 390), 100, 50);

                #region planète 1
                SDL_SetRenderDrawColor(Program.render, 127, 255, 127, 255);
                NouveauSDLRenderDrawLine(Program.render, 818, 298, 930, 336);
                NouveauSDLRenderDrawLine(Program.render, 930, 336, 971, 355);
                NouveauSDLRenderDrawLine(Program.render, 971, 355, 910, 373);
                NouveauSDLRenderDrawLine(Program.render, 910, 373, 860, 400);
                NouveauSDLRenderDrawLine(Program.render, 860, 400, 893, 412);
                NouveauSDLRenderDrawLine(Program.render, 893, 412, 906, 438);
                NouveauSDLRenderDrawLine(Program.render, 906, 438, 861, 453);
                NouveauSDLRenderDrawLine(Program.render, 861, 453, 766, 492);
                NouveauSDLRenderDrawLine(Program.render, 1160, 440, 1066, 425);
                NouveauSDLRenderDrawLine(Program.render, 1066, 425, 1000, 455);
                NouveauSDLRenderDrawLine(Program.render, 1000, 455, 1002, 490);
                NouveauSDLRenderDrawLine(Program.render, 1002, 490, 1036, 497);
                NouveauSDLRenderDrawLine(Program.render, 1036, 497, 1048, 515);
                NouveauSDLRenderDrawLine(Program.render, 1048, 515, 989, 545);
                NouveauSDLRenderDrawLine(Program.render, 989, 545, 1050, 583);
                NouveauSDLRenderDrawLine(Program.render, 1050, 583, 1101, 581);
                #endregion

                #region planète 2
                SDL_SetRenderDrawColor(Program.render, 0, 0, 255, 255);
                NouveauSDLRenderDrawLine(Program.render, 189, 319, 218, 334);
                NouveauSDLRenderDrawLine(Program.render, 218, 334, 250, 344);
                NouveauSDLRenderDrawLine(Program.render, 250, 344, 258, 365);
                NouveauSDLRenderDrawLine(Program.render, 258, 365, 237, 395);
                NouveauSDLRenderDrawLine(Program.render, 237, 395, 219, 425);
                NouveauSDLRenderDrawLine(Program.render, 219, 425, 227, 454);
                NouveauSDLRenderDrawLine(Program.render, 227, 454, 234, 486);
                NouveauSDLRenderDrawLine(Program.render, 307, 302, 302, 330);
                NouveauSDLRenderDrawLine(Program.render, 302, 330, 318, 339);
                NouveauSDLRenderDrawLine(Program.render, 318, 339, 342, 333);
                NouveauSDLRenderDrawLine(Program.render, 360, 390, 354, 406);
                NouveauSDLRenderDrawLine(Program.render, 354, 406, 340, 411);
                NouveauSDLRenderDrawLine(Program.render, 340, 411, 322, 395);
                NouveauSDLRenderDrawLine(Program.render, 322, 395, 294, 400);
                NouveauSDLRenderDrawLine(Program.render, 294, 400, 272, 426);
                NouveauSDLRenderDrawLine(Program.render, 272, 426, 304, 450);
                NouveauSDLRenderDrawLine(Program.render, 304, 450, 330, 460);
                #endregion

                #region planète 3
                SDL_SetRenderDrawColor(Program.render, 255, 0, 0, 255);
                NouveauSDLRenderDrawLine(Program.render, 1657, 489, 1605, 443);
                NouveauSDLRenderDrawLine(Program.render, 1605, 443, 1579, 393);
                NouveauSDLRenderDrawLine(Program.render, 1579, 393, 1583, 350);
                NouveauSDLRenderDrawLine(Program.render, 1583, 350, 1599, 310);
                NouveauSDLRenderDrawLine(Program.render, 1700, 481, 1674, 457);
                NouveauSDLRenderDrawLine(Program.render, 1674, 457, 1648, 422);
                NouveauSDLRenderDrawLine(Program.render, 1648, 422, 1633, 361);
                NouveauSDLRenderDrawLine(Program.render, 1633, 361, 1637, 321);
                NouveauSDLRenderDrawLine(Program.render, 1637, 321, 1647, 290);
                NouveauSDLRenderDrawLine(Program.render, 1740, 449, 1713, 419);
                NouveauSDLRenderDrawLine(Program.render, 1713, 419, 1689, 371);
                NouveauSDLRenderDrawLine(Program.render, 1689, 371, 1686, 331);
                NouveauSDLRenderDrawLine(Program.render, 1686, 331, 1702, 299);
                NouveauSDLRenderDrawLine(Program.render, 1759, 385, 1744, 371);
                NouveauSDLRenderDrawLine(Program.render, 1744, 371, 1735, 349);
                NouveauSDLRenderDrawLine(Program.render, 1735, 349, 1738, 328);
                #endregion

                #region drapeaux
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                NouveauSDLRenderDrawLine(Program.render, 950, 100, 957, 240);
                NouveauSDLRenderDrawLine(Program.render, 1644, 178, 1653, 290);
                NouveauSDLRenderDrawLine(Program.render, 252, 174, 258, 290);

                SDL_SetRenderDrawColor(Program.render, 255, 0, 0, 255);
                NouveauSDLRenderDrawLine(Program.render, 252, 174, 333, 176);
                NouveauSDLRenderDrawLine(Program.render, 333, 176, 333, 225);
                NouveauSDLRenderDrawLine(Program.render, 333, 225, 255, 229);

                SDL_SetRenderDrawColor(Program.render, 0, 0, 255, 255);
                NouveauSDLRenderDrawLine(Program.render, 950, 100, 1081, 92);
                NouveauSDLRenderDrawLine(Program.render, 1081, 92, 1082, 158);
                NouveauSDLRenderDrawLine(Program.render, 1082, 158, 954, 163);

                SDL_SetRenderDrawColor(Program.render, 0, 255, 0, 255);
                NouveauSDLRenderDrawLine(Program.render, 1644, 178, 1747, 172);
                NouveauSDLRenderDrawLine(Program.render, 1747, 172, 1750, 230);
                NouveauSDLRenderDrawLine(Program.render, 1750, 230, 1649, 235);
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
                    NouveauSDLRenderDrawPoint(Program.render, stars[i, 0], stars[i, 1]);
                }

                if (Program.gTimer % Program.RNG.Next(10, 15) == 0)
                {
                    short try_x = (short)Program.RNG.Next(160, 1760), try_y = (short)Program.RNG.Next(240, 640);
                    while (Vector2.Distance(try_x, try_y, 960, 440) > 200 && Vector2.Distance(try_x, try_y, 260, 390) > 100 &&
                        Vector2.Distance(try_x, try_y, 1660, 390) > 100)
                    {
                        try_x = (short)Program.RNG.Next(160, 1760);
                        try_y = (short)Program.RNG.Next(240, 640);
                    }
                    new Explosion(new Vector3(try_x, try_y, (byte)Program.RNG.Next(Program.G_MAX_DEPTH / 8, Program.G_MAX_DEPTH / 4)));
                }
                for (int i = 0; i < Program.explosions.Count; i++)
                {
                    Program.explosions[i].RenderObjet();
                    if (Program.explosions[i].Exist())
                        i--;
                }
            } // planètes
            else if (Program.gTimer > 300 && Program.gTimer <= 540)
            {
                Text.DisplayText("ayant servi plus d'une décennie pour l'armée de ta planète, tu es reconnu \n" +
                                 "par le dirigeant militaire de ta faction comme un des meilleurs pilotes \n" +
                                 "de la région galactique locale.", (20, 700), 3, scroll: (ushort)(Program.gTimer - 300));

                #region toi
                NouveauSDLRenderDrawLine(Program.render, 1282, 680, 1261, 417);
                NouveauSDLRenderDrawLine(Program.render, 1261, 417, 1340, 366);
                NouveauSDLRenderDrawLine(Program.render, 1340, 366, 1373, 400);
                NouveauSDLRenderDrawLine(Program.render, 1373, 400, 1453, 400);
                NouveauSDLRenderDrawLine(Program.render, 1453, 400, 1492, 368);
                NouveauSDLRenderDrawLine(Program.render, 1492, 368, 1545, 412);
                NouveauSDLRenderDrawLine(Program.render, 1545, 412, 1511, 680);

                NouveauSDLRenderDrawLine(Program.render, 1261, 417, 1229, 637);
                NouveauSDLRenderDrawLine(Program.render, 1545, 412, 1624, 337);
                NouveauSDLRenderDrawLine(Program.render, 1624, 337, 1417, 260);
                DessinerCercle((1416, 314), 77, 24);

                SDL_SetRenderDrawColor(Program.render, 255, 0, 127, 255);
                NouveauSDLRenderDrawLine(Program.render, 1462, 434, 1503, 434);
                NouveauSDLRenderDrawLine(Program.render, 1503, 434, 1484, 483);
                NouveauSDLRenderDrawLine(Program.render, 1484, 483, 1462, 434);

                SDL_SetRenderDrawColor(Program.render, 255, 255, 0, 255);
                NouveauSDLRenderDrawLine(Program.render, 1484, 483, 1468, 503);
                NouveauSDLRenderDrawLine(Program.render, 1468, 503, 1484, 522);
                NouveauSDLRenderDrawLine(Program.render, 1484, 522, 1499, 502);
                NouveauSDLRenderDrawLine(Program.render, 1499, 502, 1484, 483);
                #endregion

                #region drapeau
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                NouveauSDLRenderDrawLine(Program.render, 148, 680, 118, 57);
                NouveauSDLRenderDrawLine(Program.render, 118, 57, 172, 49);
                NouveauSDLRenderDrawLine(Program.render, 172, 49, 203, 680);

                SDL_SetRenderDrawColor(Program.render, 255, 255, 0, 255);
                NouveauSDLRenderDrawLine(Program.render, 175, 115, 255, 61);
                NouveauSDLRenderDrawLine(Program.render, 255, 61, 442, 55);
                NouveauSDLRenderDrawLine(Program.render, 442, 55, 534, 88);
                NouveauSDLRenderDrawLine(Program.render, 534, 88, 693, 82);
                NouveauSDLRenderDrawLine(Program.render, 693, 82, 766, 40);
                NouveauSDLRenderDrawLine(Program.render, 766, 40, 804, 443);
                NouveauSDLRenderDrawLine(Program.render, 804, 443, 746, 478);
                NouveauSDLRenderDrawLine(Program.render, 746, 478, 580, 489);
                NouveauSDLRenderDrawLine(Program.render, 580, 489, 481, 443);
                NouveauSDLRenderDrawLine(Program.render, 481, 443, 311, 452);
                NouveauSDLRenderDrawLine(Program.render, 311, 452, 194, 500);

                SDL_SetRenderDrawColor(Program.render, 255, 127, 0, 255);
                DessinerCercle((479, 232), 91, 24);
                DessinerCercle((479, 232), 65, 24);

                SDL_SetRenderDrawColor(Program.render, 0, 255, 0, 255);
                NouveauSDLRenderDrawLine(Program.render, 563, 195, 672, 204);
                NouveauSDLRenderDrawLine(Program.render, 672, 204, 778, 172);
                NouveauSDLRenderDrawLine(Program.render, 570, 243, 677, 251);
                NouveauSDLRenderDrawLine(Program.render, 677, 251, 782, 214);
                NouveauSDLRenderDrawLine(Program.render, 395, 197, 276, 190);
                NouveauSDLRenderDrawLine(Program.render, 276, 190, 180, 219);
                NouveauSDLRenderDrawLine(Program.render, 390, 250, 277, 242);
                NouveauSDLRenderDrawLine(Program.render, 277, 242, 183, 282);
                #endregion

                #region étoile
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                NouveauSDLRenderDrawLine(Program.render, 472, 167, 443, 286);
                NouveauSDLRenderDrawLine(Program.render, 443, 286, 534, 197);
                NouveauSDLRenderDrawLine(Program.render, 534, 197, 420, 206);
                NouveauSDLRenderDrawLine(Program.render, 420, 206, 524, 279);
                NouveauSDLRenderDrawLine(Program.render, 524, 279, 472, 167);
                #endregion
            } // o7
            else if (Program.gTimer > 540 && Program.gTimer <= 780)
            {
                Text.DisplayText("un jour, une lettre arrive à ta porte.\n" +
                                 "elle porte l'emblême officielle du pays, donc c'est probablement très \n" +
                                 "important.",
                                 (20, 700), 3, scroll: (ushort)(Program.gTimer - 540));

                #region lettre
                NouveauSDLRenderDrawLine(Program.render, 717, 607, 600, 400);
                NouveauSDLRenderDrawLine(Program.render, 600, 400, 958, 198);
                NouveauSDLRenderDrawLine(Program.render, 958, 198, 1062, 411);
                NouveauSDLRenderDrawLine(Program.render, 1062, 411, 717, 607);
                NouveauSDLRenderDrawLine(Program.render, 600, 400, 792, 385);
                NouveauSDLRenderDrawLine(Program.render, 856, 353, 958, 198);
                #endregion

                #region emblême cercle
                SDL_SetRenderDrawColor(Program.render, 255, 127, 0, 255);
                DessinerCercle((832, 384), 30, 24);
                DessinerCercle((832, 384), 39, 24);
                #endregion

                #region étoile
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                NouveauSDLRenderDrawLine(Program.render, 859, 396, 815, 358);
                NouveauSDLRenderDrawLine(Program.render, 815, 358, 831, 414);
                NouveauSDLRenderDrawLine(Program.render, 831, 414, 849, 359);
                NouveauSDLRenderDrawLine(Program.render, 849, 359, 802, 389);
                NouveauSDLRenderDrawLine(Program.render, 802, 389, 859, 396);
                #endregion
            } // lettre fermée
            else if (Program.gTimer > 780 && Program.gTimer <= 1020)
            {
                Text.DisplayText("\"bonjour. \n" +
                                 "la coalition des planètes locales vous a choisi pour mener une mission \n" +
                                 "seule qui sauvera notre peuple des mains ennemies, ou bien même la mort.\"", (20, 700), 3, scroll: (ushort)(Program.gTimer - 780));

                #region lettre
                NouveauSDLRenderDrawLine(Program.render, 717, 607, 600, 400);
                NouveauSDLRenderDrawLine(Program.render, 958, 198, 1062, 411);
                NouveauSDLRenderDrawLine(Program.render, 1062, 411, 717, 607);
                NouveauSDLRenderDrawLine(Program.render, 600, 400, 832, 383);
                NouveauSDLRenderDrawLine(Program.render, 832, 383, 958, 198);
                NouveauSDLRenderDrawLine(Program.render, 600, 400, 648, 316);
                NouveauSDLRenderDrawLine(Program.render, 958, 198, 868, 193);
                #endregion

                #region papier
                SDL_SetRenderDrawColor(Program.render, 255, 150, 25, 255);
                NouveauSDLRenderDrawLine(Program.render, 695, 393, 577, 199);
                NouveauSDLRenderDrawLine(Program.render, 577, 199, 802, 79);
                NouveauSDLRenderDrawLine(Program.render, 802, 79, 911, 267);
                #endregion

                #region blabla
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                NouveauSDLRenderDrawLine(Program.render, 611, 212, 669, 180);
                NouveauSDLRenderDrawLine(Program.render, 636, 252, 817, 153);
                NouveauSDLRenderDrawLine(Program.render, 644, 273, 718, 231);
                NouveauSDLRenderDrawLine(Program.render, 739, 218, 821, 173);
                NouveauSDLRenderDrawLine(Program.render, 659, 292, 684, 276);
                NouveauSDLRenderDrawLine(Program.render, 702, 265, 799, 212);
                NouveauSDLRenderDrawLine(Program.render, 818, 201, 839, 188);
                NouveauSDLRenderDrawLine(Program.render, 664, 312, 797, 228);
                NouveauSDLRenderDrawLine(Program.render, 820, 221, 850, 200);
                NouveauSDLRenderDrawLine(Program.render, 682, 337, 766, 280);
                NouveauSDLRenderDrawLine(Program.render, 798, 263, 860, 225);
                NouveauSDLRenderDrawLine(Program.render, 698, 358, 869, 251);
                NouveauSDLRenderDrawLine(Program.render, 843, 317, 893, 282);
                #endregion
            } // lettre ouverte
            else if (Program.gTimer > 1020 && Program.gTimer <= 1260)
            {
                Text.DisplayText("\"on a récemment crée l'un des meilleurs vaisseaux de la galaxie, mais \n" +
                                 "à cause du nombre de ressources requis pour le construire, on en n'a \n" +
                                 "qu'un seul.\"", (20, 700), 3, scroll: (ushort)(Program.gTimer - 1020));

                #region modèles joueur
                Program.player.position = new Vector3(400, 400, 0);
                Program.player.pitch = 4;
                Program.player.roll = 0;
                Program.player.taille = 6;
                Program.player.RenderObjet();

                Program.player.position = new Vector3(1400, 200, 0);
                Program.player.pitch = 0;
                Program.player.roll = 0.25f * MathF.Sin(Program.gTimer / 30f);
                Program.player.taille = 5;
                Program.player.RenderObjet();
                #endregion

                #region lignes
                NouveauSDLRenderDrawLine(Program.render, 293, 261, 347, 173);
                NouveauSDLRenderDrawLine(Program.render, 347, 173, 588, 173);
                NouveauSDLRenderDrawLine(Program.render, 462, 549, 521, 628);
                NouveauSDLRenderDrawLine(Program.render, 521, 628, 744, 631);
                NouveauSDLRenderDrawLine(Program.render, 1191, 76, 1164, 99);
                NouveauSDLRenderDrawLine(Program.render, 1164, 99, 1166, 258);
                NouveauSDLRenderDrawLine(Program.render, 1195, 284, 1166, 258);
                NouveauSDLRenderDrawLine(Program.render, 1165, 176, 981, 171);
                NouveauSDLRenderDrawLine(Program.render, 1474, 168, 1544, 96);
                NouveauSDLRenderDrawLine(Program.render, 1544, 96, 1736, 96);
                #endregion

                #region texte et blabla rouge
                Text.DisplayText("x-57", (880, 330), 10);
                SDL_SetRenderDrawColor(Program.render, 255, 0, 0, 255);
                NouveauSDLRenderDrawLine(Program.render, 394, 142, 556, 136);
                NouveauSDLRenderDrawLine(Program.render, 565, 591, 708, 582);
                NouveauSDLRenderDrawLine(Program.render, 1016, 129, 1136, 137);
                NouveauSDLRenderDrawLine(Program.render, 1589, 64, 1709, 67);
                #endregion

                #region plus de blabla
                NouveauSDLRenderDrawLine(Program.render, 874, 472, 1237, 465);
                NouveauSDLRenderDrawLine(Program.render, 1306, 456, 1680, 458);
                NouveauSDLRenderDrawLine(Program.render, 1700, 500, 1565, 508);
                NouveauSDLRenderDrawLine(Program.render, 1521, 512, 1028, 523);
                NouveauSDLRenderDrawLine(Program.render, 979, 527, 867, 522);
                NouveauSDLRenderDrawLine(Program.render, 874, 571, 1149, 569);
                NouveauSDLRenderDrawLine(Program.render, 1206, 559, 1331, 566);
                NouveauSDLRenderDrawLine(Program.render, 1402, 558, 1685, 553);
                NouveauSDLRenderDrawLine(Program.render, 1687, 626, 1264, 626);
                NouveauSDLRenderDrawLine(Program.render, 1210, 632, 1111, 631);
                NouveauSDLRenderDrawLine(Program.render, 1042, 634, 879, 638);
                #endregion

            } // vaisseau
            else if (Program.gTimer > 1260 && Program.gTimer <= 1500)
            {
                Text.DisplayText("\"votre mission est d'aller détruire la bombe à pulsar dans la région \n" +
                                    "d'espace de l'ennemi, et de s'assurer qu'elle est neutralisée, ou sous \n" +
                                    "notre controle.\"", (20, 700), 3, scroll: (ushort)(Program.gTimer - 1260));

                BombePulsar.DessinerBombePulsar((960, 330), 180, false);

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
                    NouveauSDLRenderDrawPoint(Program.render, stars[i, 0], stars[i, 1]);
                }

                #region bleu autour de pôles
                SDL_SetRenderDrawColor(Program.render, 0, 0, 255, 255);
                NouveauSDLRenderDrawLine(Program.render, 833, 201, 770, 127);
                NouveauSDLRenderDrawLine(Program.render, 770, 127, 698, 120);
                NouveauSDLRenderDrawLine(Program.render, 698, 120, 640, 135);
                NouveauSDLRenderDrawLine(Program.render, 854, 184, 800, 100);
                NouveauSDLRenderDrawLine(Program.render, 800, 100, 818, 36);
                NouveauSDLRenderDrawLine(Program.render, 818, 36, 860, 21);
                NouveauSDLRenderDrawLine(Program.render, 1055, 482, 1109, 549);
                NouveauSDLRenderDrawLine(Program.render, 1109, 549, 1106, 606);
                NouveauSDLRenderDrawLine(Program.render, 1106, 606, 1082, 643);
                NouveauSDLRenderDrawLine(Program.render, 1078, 465, 1136, 526);
                NouveauSDLRenderDrawLine(Program.render, 1136, 526, 1196, 531);
                NouveauSDLRenderDrawLine(Program.render, 1196, 531, 1230, 515);
                #endregion

                #region plus de bleu autour des pôles
                NouveauSDLRenderDrawLine(Program.render, 1069, 479, 1097, 518);
                NouveauSDLRenderDrawLine(Program.render, 1090, 494, 1137, 542);
                NouveauSDLRenderDrawLine(Program.render, 1111, 533, 1134, 567);
                NouveauSDLRenderDrawLine(Program.render, 1120, 564, 1123, 617);
                NouveauSDLRenderDrawLine(Program.render, 1139, 583, 1144, 619);
                NouveauSDLRenderDrawLine(Program.render, 1148, 559, 1202, 596);
                NouveauSDLRenderDrawLine(Program.render, 1169, 547, 1219, 553);
                NouveauSDLRenderDrawLine(Program.render, 829, 181, 795, 139);
                NouveauSDLRenderDrawLine(Program.render, 829, 158, 780, 85);
                NouveauSDLRenderDrawLine(Program.render, 782, 123, 738, 96);
                NouveauSDLRenderDrawLine(Program.render, 723, 101, 676, 86);
                NouveauSDLRenderDrawLine(Program.render, 770, 97, 753, 57);
                NouveauSDLRenderDrawLine(Program.render, 792, 79, 802, 28);
                #endregion

            } // bombe à pulsar
            else if (Program.gTimer > 1500 && Program.gTimer <= 1740)
            {
                Text.DisplayText("\"les coordonées de la bombe se trouvent programmés dans votre vaisseau, \n" +
                                 "qui vous attend au garage 05. \n" +
                                 "on compte sur vous, n'échouez pas.\" \n" +
                                 "- le dirigeant militaire", (20, 700), 3, scroll: (ushort)(Program.gTimer - 1500));

                #region lettre
                NouveauSDLRenderDrawLine(Program.render, 717, 607, 600, 400);
                NouveauSDLRenderDrawLine(Program.render, 958, 198, 1062, 411);
                NouveauSDLRenderDrawLine(Program.render, 1062, 411, 717, 607);
                NouveauSDLRenderDrawLine(Program.render, 600, 400, 832, 383);
                NouveauSDLRenderDrawLine(Program.render, 832, 383, 958, 198);
                NouveauSDLRenderDrawLine(Program.render, 600, 400, 648, 316);
                NouveauSDLRenderDrawLine(Program.render, 958, 198, 868, 193);
                #endregion

                #region papier
                SDL_SetRenderDrawColor(Program.render, 255, 150, 25, 255);
                NouveauSDLRenderDrawLine(Program.render, 695, 393, 577, 199);
                NouveauSDLRenderDrawLine(Program.render, 577, 199, 802, 79);
                NouveauSDLRenderDrawLine(Program.render, 802, 79, 911, 267);
                #endregion

                #region blabla
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                NouveauSDLRenderDrawLine(Program.render, 611, 212, 669, 180);
                NouveauSDLRenderDrawLine(Program.render, 636, 252, 817, 153);
                NouveauSDLRenderDrawLine(Program.render, 644, 273, 718, 231);
                NouveauSDLRenderDrawLine(Program.render, 739, 218, 821, 173);
                NouveauSDLRenderDrawLine(Program.render, 659, 292, 684, 276);
                NouveauSDLRenderDrawLine(Program.render, 702, 265, 799, 212);
                NouveauSDLRenderDrawLine(Program.render, 818, 201, 839, 188);
                NouveauSDLRenderDrawLine(Program.render, 664, 312, 797, 228);
                NouveauSDLRenderDrawLine(Program.render, 820, 221, 850, 200);
                NouveauSDLRenderDrawLine(Program.render, 682, 337, 766, 280);
                NouveauSDLRenderDrawLine(Program.render, 798, 263, 860, 225);
                NouveauSDLRenderDrawLine(Program.render, 698, 358, 869, 251);
                NouveauSDLRenderDrawLine(Program.render, 843, 317, 893, 282);
                #endregion

            } // lettre ouverte 2
            else if (Program.gTimer > 1740 && Program.gTimer <= 1980)
            {
                Text.DisplayText("05", (100, 200), 15, 0x00FF00);
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);

                #region hangar
                NouveauSDLRenderDrawLine(Program.render, 20, 455, 1900, 455);
                NouveauSDLRenderDrawLine(Program.render, 500, 455, 500, 111);
                NouveauSDLRenderDrawLine(Program.render, 500, 111, 1500, 111);
                NouveauSDLRenderDrawLine(Program.render, 1500, 111, 1500, 455);
                #endregion

                if (Program.gTimer < 1800)
                {
                    NouveauSDLRenderDrawLine(Program.render, 1000, 111, 1000, 455);
                }
                if (Program.gTimer >= 1800 && Program.gTimer < 1860)
                {
                    NouveauSDLRenderDrawLine(Program.render, 1000 - (int)(Program.gTimer - 1800) * 5, 111, 1000 - (int)(Program.gTimer - 1800) * 5, 455);
                    NouveauSDLRenderDrawLine(Program.render, 1000 + (int)(Program.gTimer - 1800) * 5, 111, 1000 + (int)(Program.gTimer - 1800) * 5, 455);
                }
                if (Program.gTimer >= 1860)
                {
                    NouveauSDLRenderDrawLine(Program.render, 700, 111, 700, 455);
                    NouveauSDLRenderDrawLine(Program.render, 1300, 111, 1300, 455);
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
                        if (MathF.Abs(stars[i, 0] - 1000.0f) < (Program.gTimer - 1800) * 5)
                            NouveauSDLRenderDrawPoint(Program.render, stars[i, 0], stars[i, 1]);
                    }

                if (Program.gTimer < 1830)
                {
                    Program.player.position = new Vector3(1000, 550, 0);
                    Program.player.pitch = 0;
                    Program.player.roll = 0;
                    Program.player.taille = 2;
                    Program.player.RenderObjet();
                }
                if (Program.gTimer >= 1830 && Program.gTimer <= 1860)
                {
                    Program.player.position = new Vector3(
                        1000,
                        250 * MathF.Pow(0.9f, Program.gTimer - 1830) + 300,
                        0
                    );
                    Program.player.taille = 2 * MathF.Pow(0.9f, Program.gTimer - 1830);
                    Program.player.RenderObjet();
                }

                if (Program.gTimer < 1815)
                    NouveauSDLRenderDrawLine(Program.render, 972, 566, 952, 586);

                if (Program.gTimer < 1810)
                    NouveauSDLRenderDrawPoint(Program.render, (int)(Program.gTimer - 1740 + 952 - 70), 585);

                if (Program.gTimer >= 1810 && Program.gTimer <= 1815)
                    NouveauSDLRenderDrawPoint(Program.render, (int)((Program.gTimer - 1810) * 2 + 972 - 20), (int)((1810 - Program.gTimer) * 2 + 603 - 20));

            } // départ

            if (Program.gTimer > 30 && Program.gTimer < 2000)
            {
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                NouveauSDLRenderDrawLine(Program.render, 20, 20, 1900, 20);
                NouveauSDLRenderDrawLine(Program.render, 1900, 20, 1900, 680);
                NouveauSDLRenderDrawLine(Program.render, 1900, 680, 20, 680);
                NouveauSDLRenderDrawLine(Program.render, 20, 20, 20, 680);
            }

            if (Program.gTimer > 2100)
            {
                Program.player.Init();
                Program.Gamemode = Gamemode.GAMEPLAY;
                Program.niveau = 0;
                Program.ennemis_a_creer = (byte)DataNiveau.liste_niveaux[Program.niveau].Length;
                Program.ennemis_tues = 0;
                Program.explosions.Clear();
            }
        }
        public static void Cut_2() // good end
        {
            if (Program.cutscene_skip_partiel || Program.cutscene_skip_complet)
                Program.gTimer = 2401;

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
                    NouveauSDLRenderDrawPoint(Program.render, stars[i, 0], stars[i, 1]);
                }
                #endregion

                #region player
                Program.player.position = new Vector3(277, 519, 0);
                Program.player.pitch = 0.7f;
                Program.player.roll = 0.8f;
                Program.player.taille = 2;
                Program.player.RenderObjet();
                #endregion

                #region enemy 15
                Program.player.taille = 1f;
                Program.player.position = new Vector3(771, 325, 0);
                Program.player.roll = 0.5f;
                double sinroll = MathF.Sin(Program.player.roll);
                double cosroll = MathF.Cos(Program.player.roll);
                float pitchconst = Program.player.pitch + 0.3f;//permapitch du joueur
                SDL_SetRenderDrawColor(Program.render, 255, 0, 0, 255);
                for (int i = 0; i < Program.player.modele.Length - 1; i++)
                {
                    int[] pos = new int[4] {
                        (int)(Program.player.taille * (cosroll * -Program.player.modele[i].x - sinroll * -Program.player.modele[i].y) + Program.player.position.x),
                        (int)(Program.player.taille * (sinroll * -Program.player.modele[i].x + cosroll * -Program.player.modele[i].y) + Program.player.position.y - Program.player.modele[i].z * pitchconst),
                        (int)(Program.player.taille * (cosroll * -Program.player.modele[i + 1].x - sinroll * -Program.player.modele[i + 1].y) + Program.player.position.x),
                        (int)(Program.player.taille * (sinroll * -Program.player.modele[i + 1].x + cosroll * -Program.player.modele[i + 1].y) + Program.player.position.y - Program.player.modele[i + 1].z * pitchconst)
                    };
                    NouveauSDLRenderDrawLine(Program.render, pos[0], pos[1], pos[2], pos[3]);
                }
                #endregion

                #region bombe pulsar
                BombePulsar.DessinerBombePulsar((1522 + Program.RNG.Next(-5, 5), 264 + Program.RNG.Next(-5, 5)),
                    133, false);

                SDL_SetRenderDrawColor(Program.render, 0, 0, 255, 255);
                NouveauSDLRenderDrawLine(Program.render, 1463, 144, 1445, 97);
                NouveauSDLRenderDrawLine(Program.render, 1445, 97, 1422, 68);
                NouveauSDLRenderDrawLine(Program.render, 1422, 68, 1373, 46);
                NouveauSDLRenderDrawLine(Program.render, 1525, 131, 1522, 86);
                NouveauSDLRenderDrawLine(Program.render, 1522, 86, 1554, 48);
                NouveauSDLRenderDrawLine(Program.render, 1554, 48, 1584, 35);
                NouveauSDLRenderDrawLine(Program.render, 1501, 36, 1499, 82);
                NouveauSDLRenderDrawLine(Program.render, 1464, 63, 1484, 112);
                NouveauSDLRenderDrawLine(Program.render, 1500, 396, 1491, 454);
                NouveauSDLRenderDrawLine(Program.render, 1491, 454, 1470, 493);
                NouveauSDLRenderDrawLine(Program.render, 1470, 493, 1450, 534);
                NouveauSDLRenderDrawLine(Program.render, 1549, 395, 1550, 450);
                NouveauSDLRenderDrawLine(Program.render, 1550, 450, 1574, 486);
                NouveauSDLRenderDrawLine(Program.render, 1574, 486, 1612, 513);
                NouveauSDLRenderDrawLine(Program.render, 1514, 434, 1506, 485);
                NouveauSDLRenderDrawLine(Program.render, 1539, 475, 1560, 520);
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
                        while (Vector2.Distance(x, y, 948, 338, 0.3f) > 270 || Vector2.Distance(x, y, 954, 276, 0.6f) < 80)
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
                    NouveauSDLRenderDrawPoint(Program.render, stars[i, 0], stars[i, 1]);
                }
                for (int i = 0; i < stars_glx.GetLength(0); i++)
                {
                    if (Program.gTimer > 480)
                    {
                        if (Vector2.Distance(stars_glx[i, 0], stars_glx[i, 1], 716, 437, 0.5f) < 100)
                            SDL_SetRenderDrawColor(Program.render, 0, 0, 0, 255);
                        else
                            SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                    }
                    NouveauSDLRenderDrawPoint(Program.render, stars_glx[i, 0], stars_glx[i, 1]);
                }
                #endregion

                #region galaxie
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 60);
                NouveauSDLRenderDrawLine(Program.render, 525, 362, 477, 195);
                NouveauSDLRenderDrawLine(Program.render, 477, 195, 547, 223);
                NouveauSDLRenderDrawLine(Program.render, 547, 223, 686, 91);
                NouveauSDLRenderDrawLine(Program.render, 686, 91, 697, 141);
                NouveauSDLRenderDrawLine(Program.render, 697, 141, 950, 79);
                NouveauSDLRenderDrawLine(Program.render, 950, 79, 933, 126);
                NouveauSDLRenderDrawLine(Program.render, 933, 126, 1230, 92);
                NouveauSDLRenderDrawLine(Program.render, 1230, 92, 1191, 126);
                NouveauSDLRenderDrawLine(Program.render, 1191, 126, 1404, 179);
                NouveauSDLRenderDrawLine(Program.render, 1404, 179, 1344, 186);
                NouveauSDLRenderDrawLine(Program.render, 1344, 186, 1434, 323);
                NouveauSDLRenderDrawLine(Program.render, 1434, 323, 1370, 312);
                NouveauSDLRenderDrawLine(Program.render, 1370, 312, 1404, 441);
                NouveauSDLRenderDrawLine(Program.render, 1404, 441, 1354, 427);
                NouveauSDLRenderDrawLine(Program.render, 1354, 427, 1285, 571);
                NouveauSDLRenderDrawLine(Program.render, 1285, 571, 1285, 499);
                NouveauSDLRenderDrawLine(Program.render, 1285, 499, 1017, 600);
                NouveauSDLRenderDrawLine(Program.render, 1017, 600, 1032, 538);
                NouveauSDLRenderDrawLine(Program.render, 1032, 538, 700, 600);
                NouveauSDLRenderDrawLine(Program.render, 700, 600, 734, 553);
                NouveauSDLRenderDrawLine(Program.render, 734, 553, 500, 500);
                NouveauSDLRenderDrawLine(Program.render, 500, 500, 600, 500);
                NouveauSDLRenderDrawLine(Program.render, 600, 500, 451, 377);
                NouveauSDLRenderDrawLine(Program.render, 451, 377, 525, 362);

                SDL_SetRenderDrawColor(Program.render, 127, 0, 127, 255);
                NouveauSDLRenderDrawLine(Program.render, 850, 350, 833, 306);
                NouveauSDLRenderDrawLine(Program.render, 833, 306, 842, 271);
                NouveauSDLRenderDrawLine(Program.render, 842, 271, 878, 221);
                NouveauSDLRenderDrawLine(Program.render, 878, 221, 952, 193);
                NouveauSDLRenderDrawLine(Program.render, 952, 193, 1027, 216);
                NouveauSDLRenderDrawLine(Program.render, 1027, 216, 1064, 260);
                NouveauSDLRenderDrawLine(Program.render, 1064, 260, 1076, 299);
                NouveauSDLRenderDrawLine(Program.render, 1076, 299, 1070, 340);

                NouveauSDLRenderDrawLine(Program.render, 828, 315, 798, 324);
                NouveauSDLRenderDrawLine(Program.render, 798, 324, 800, 350);
                NouveauSDLRenderDrawLine(Program.render, 800, 350, 831, 359);
                NouveauSDLRenderDrawLine(Program.render, 831, 359, 1082, 347);
                NouveauSDLRenderDrawLine(Program.render, 1082, 347, 1114, 333);
                NouveauSDLRenderDrawLine(Program.render, 1114, 333, 1112, 309);
                NouveauSDLRenderDrawLine(Program.render, 1112, 309, 1081, 304);
                #endregion

                #region explosions
                if (Program.gTimer < 480)
                {
                    if (Program.gTimer % Program.RNG.Next(8, 12) == 0)
                        new Explosion(new Vector3(Program.RNG.Next(500, 1400), Program.RNG.Next(100, 550), Program.G_MAX_DEPTH / 2));
                }
                for (int i = 0; i < Program.explosions.Count; i++)
                {
		    Program.explosions[i].RenderObjet();
                    if (Program.explosions[i].Exist())
                        i--;
                }
                #endregion

                #region boom
                if (Program.gTimer > 450 && Program.gTimer <= 500)
                {
                    lTimer = (byte)(2 * Program.gTimer - 900);
                    int abs_lTimer = -2 * (int)MathF.Abs(lTimer - 50) + 100; // y=-2|x-(r/2)|+r, ou environ /\
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
                        while (Vector2.Distance(x, y, Program.W_SEMI_LARGEUR, Program.W_SEMI_HAUTEUR / 2, 0.2f) < 200)
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
                    NouveauSDLRenderDrawPoint(Program.render, stars[i, 0], stars[i, 1]);
                }
                #endregion

                #region vaisseaux
                SDL_SetRenderDrawColor(Program.render, 255, 0, 0, 255);
                NouveauSDLRenderDrawPoint(Program.render, -Program.gTimer + 1900, Program.W_SEMI_HAUTEUR / 2);
                SDL_SetRenderDrawColor(Program.render, 0, 255, 0, 255);
                NouveauSDLRenderDrawPoint(Program.render, Program.W_SEMI_LARGEUR, Program.gTimer / -2 + 730);
                SDL_SetRenderDrawColor(Program.render, 255, 255, 0, 255);
                NouveauSDLRenderDrawPoint(Program.render, Program.gTimer - 1, Program.W_SEMI_HAUTEUR / 2);
                SDL_SetRenderDrawColor(Program.render, 0, 0, 255, 255);
                NouveauSDLRenderDrawPoint(Program.render, Program.W_SEMI_LARGEUR, Program.gTimer / 2 - 200);
                #endregion

            } // ranconte - fini
            else if (Program.gTimer >= 840 && Program.gTimer < 1020)
            {
                Text.DisplayText("le grand vide laissé par l'explosion a démontré la vérité de cette guerre.",
                    (20, 700), 3, scroll: (int)Program.gTimer / 2 - 420);

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
                    NouveauSDLRenderDrawPoint(Program.render, stars[i, 0], stars[i, 1]);
                }
                #endregion

                #region vaisseaux
                Vector3[] model =
                {
                    new( -25, 0, 0 ),
                    new( 0, -10, 0 ),
                    new( 25, 0, 0 ),
                    new( 0, -10, -30 ),
                    new( 0, -10, 0 ),
                    new( 0, -10, -30 ),
                    new( -25, 0, 0 )
                };
                int x, y;
                sbyte depth = -15;
                float pitch = -1f;

                SDL_SetRenderDrawColor(Program.render, 255, 0, 0, 255);
                x = (short)(-Program.gTimer / 10 + 1200);
                y = (short)(Program.W_SEMI_HAUTEUR - 100);
                NouveauSDLRenderDrawLine(Program.render, x, y, x + 100, y - 65);
                NouveauSDLRenderDrawLine(Program.render, x + 100, y - 65, x + 90, y);
                NouveauSDLRenderDrawLine(Program.render, x + 90, y, x + 139, y - 63);
                NouveauSDLRenderDrawLine(Program.render, x + 139, y - 63, x, y);
                NouveauSDLRenderDrawLine(Program.render, x, y, x + 90, y);

                SDL_SetRenderDrawColor(Program.render, 0, 255, 0, 255);
                x = Program.W_SEMI_LARGEUR;
                y = (short)(Program.W_SEMI_HAUTEUR / 2 + ((int)Program.gTimer - 880) / 10);
                for (int i = 0; i < model.GetLength(0) - 1; i++)
                {
                    NouveauSDLRenderDrawLine(Program.render,
                        (int)(model[i].x * MathF.Pow(0.95f, depth) + x),
                        (int)((model[i].y + (model[i].z * pitch)) * MathF.Pow(0.95f, depth) + y),
                        (int)(model[i + 1].x * MathF.Pow(0.95f, depth) + x),
                        (int)((model[i + 1].y + (model[i + 1].z * pitch)) * MathF.Pow(0.95f, depth) + y));
                }

                SDL_SetRenderDrawColor(Program.render, 255, 255, 0, 255);
                x = (short)(Program.gTimer / 10 + 700);
                y = (short)(Program.W_SEMI_HAUTEUR - 100);
                NouveauSDLRenderDrawLine(Program.render, x, y, x - 100, y - 65);
                NouveauSDLRenderDrawLine(Program.render, x - 100, y - 65, x - 90, y);
                NouveauSDLRenderDrawLine(Program.render, x - 90, y, x - 139, y - 63);
                NouveauSDLRenderDrawLine(Program.render, x - 139, y - 63, x, y);
                NouveauSDLRenderDrawLine(Program.render, x, y, x - 90, y);

                SDL_SetRenderDrawColor(Program.render, 0, 0, 255, 255);
                x = Program.W_SEMI_LARGEUR;
                y = (short)(Program.W_SEMI_HAUTEUR - (Program.gTimer - 800) / 10);
                for (int i = 0; i < model.GetLength(0) - 1; i++)
                {
                    NouveauSDLRenderDrawLine(Program.render,
                        (int)(model[i].x * MathF.Pow(0.95f, depth) + x),
                        (int)((model[i].y + (model[i].z * -pitch)) * MathF.Pow(0.95f, depth) + y),
                        (int)(model[i + 1].x * MathF.Pow(0.95f, depth) + x),
                        (int)((model[i + 1].y + (model[i + 1].z * -pitch)) * MathF.Pow(0.95f, depth) + y));
                }
                #endregion

            } // ranconte proche - fini
            else if (Program.gTimer >= 1020 && Program.gTimer < 1200)
            {
                Text.DisplayText("l'abscence de vrai gagnants.", (20, 700), 3, scroll: (int)(Program.gTimer / 2 - 510));

                #region étoiles
                if (Program.gTimer == 1020)
                {
                    for (int i = 0; i < stars.GetLength(0); i++)
                    {
                        int x = Program.RNG.Next(25, 1880), y = Program.RNG.Next(25, 680);
                        while (Vector2.Distance(x, y, Program.W_SEMI_LARGEUR, Program.W_SEMI_HAUTEUR / 2 + 100, 0.2f) < 200)
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
                    NouveauSDLRenderDrawPoint(Program.render, stars[i, 0], stars[i, 1]);
                }
                #endregion

                #region vaisseaux
                SDL_SetRenderDrawColor(Program.render, 255, 0, 0, 255);
                NouveauSDLRenderDrawPoint(Program.render, 960, 340);
                SDL_SetRenderDrawColor(Program.render, 0, 255, 0, 255);
                NouveauSDLRenderDrawPoint(Program.render, 950, 345);
                SDL_SetRenderDrawColor(Program.render, 255, 255, 0, 255);
                NouveauSDLRenderDrawPoint(Program.render, 940, 340);
                SDL_SetRenderDrawColor(Program.render, 0, 0, 255, 255);
                NouveauSDLRenderDrawPoint(Program.render, 950, 335);
                #endregion
            } // ranconte loin - fini
            else if (Program.gTimer >= 1200 && Program.gTimer < 1380)
            {
                Text.DisplayText("les factions qui ont survécu ont vite cherché la paix entre eux.\n" +
                                 "des milliards sont morts, victimes de cette guerre. des milliards de trop.",
                                 (20, 700), 3, scroll: (int)(Program.gTimer - 1200));

                #region promesse
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                NouveauSDLRenderDrawLine(Program.render, 600, 680, 600, 500);
                NouveauSDLRenderDrawLine(Program.render, 600, 500, 1350, 500);
                NouveauSDLRenderDrawLine(Program.render, 1350, 500, 1350, 680);

                DessinerCercle((730, 189), 57, 50);
                NouveauSDLRenderDrawLine(Program.render, 648, 500, 606, 238);
                NouveauSDLRenderDrawLine(Program.render, 606, 238, 836, 255);
                NouveauSDLRenderDrawLine(Program.render, 836, 255, 806, 500);
                NouveauSDLRenderDrawLine(Program.render, 606, 238, 593, 470);

                DessinerCercle((1203, 186), 57, 50);
                NouveauSDLRenderDrawLine(Program.render, 1126, 500, 1096, 255);
                NouveauSDLRenderDrawLine(Program.render, 1096, 255, 1304, 235);
                NouveauSDLRenderDrawLine(Program.render, 1304, 235, 1278, 500);
                NouveauSDLRenderDrawLine(Program.render, 1304, 235, 1336, 435);

                NouveauSDLRenderDrawLine(Program.render, 836, 255, 972, 297 + (Program.gTimer % 30 < 15 ? 0 : 50));
                NouveauSDLRenderDrawLine(Program.render, 1096, 255, 972, 297 + (Program.gTimer % 30 < 15 ? 0 : 50));
                #endregion

                #region drapeaux
                short[] positions = new short[4] { 100, 300, 1500, 1700 };
                foreach (short i in positions)
                {
                    SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                    NouveauSDLRenderDrawLine(Program.render, i, 680, i, 509);
                    NouveauSDLRenderDrawLine(Program.render, i, 100, i, 365);

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
                    NouveauSDLRenderDrawLine(Program.render, i, 100, 87 + i, 272);
                    NouveauSDLRenderDrawLine(Program.render, 87 + i, 272, 100 + i, 400);
                    NouveauSDLRenderDrawLine(Program.render, 100 + i, 400, 76 + i, 449);
                    NouveauSDLRenderDrawLine(Program.render, 76 + i, 449, 69 + i, 519);
                    NouveauSDLRenderDrawLine(Program.render, 69 + i, 519, 90 + i, 557);
                    NouveauSDLRenderDrawLine(Program.render, 90 + i, 557, 32 + i, 574);
                    NouveauSDLRenderDrawLine(Program.render, 32 + i, 574, -28 + i, 449);
                    NouveauSDLRenderDrawLine(Program.render, -28 + i, 449, -13 + i, 389);
                    NouveauSDLRenderDrawLine(Program.render, -13 + i, 389, 14 + i, 338);
                    NouveauSDLRenderDrawLine(Program.render, 14 + i, 338, 15 + i, 305);
                    NouveauSDLRenderDrawLine(Program.render, 15 + i, 305, i, 300);
                }
                #endregion
            }// paix - fini
            else if (Program.gTimer >= 1380 && Program.gTimer < 1560)
            {
                #region traité
                SDL_SetRenderDrawColor(Program.render, 255, 150, 25, 255);
                NouveauSDLRenderDrawLine(Program.render, 700, 600, 800, 100);
                NouveauSDLRenderDrawLine(Program.render, 800, 100, 1200, 100);
                NouveauSDLRenderDrawLine(Program.render, 1200, 100, 1300, 600);
                NouveauSDLRenderDrawLine(Program.render, 1300, 600, 700, 600);

                Text.DisplayText("traité de paix", (825, 125), 3, 0x7f7f7f);

                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                NouveauSDLRenderDrawLine(Program.render, 816, 238, 979, 236);
                NouveauSDLRenderDrawLine(Program.render, 1028, 238, 1180, 234);
                NouveauSDLRenderDrawLine(Program.render, 809, 283, 849, 280);
                NouveauSDLRenderDrawLine(Program.render, 905, 286, 1177, 283);
                NouveauSDLRenderDrawLine(Program.render, 797, 325, 910, 324);
                NouveauSDLRenderDrawLine(Program.render, 945, 325, 1011, 325);
                NouveauSDLRenderDrawLine(Program.render, 1040, 324, 1183, 326);
                NouveauSDLRenderDrawLine(Program.render, 782, 382, 1085, 382);
                NouveauSDLRenderDrawLine(Program.render, 1128, 380, 1199, 382);
                NouveauSDLRenderDrawLine(Program.render, 779, 434, 899, 434);
                NouveauSDLRenderDrawLine(Program.render, 776, 483, 988, 482);
                NouveauSDLRenderDrawLine(Program.render, 783, 583, 1098, 582);
                NouveauSDLRenderDrawLine(Program.render, 775, 523, 744, 572);
                NouveauSDLRenderDrawLine(Program.render, 744, 520, 774, 572);
                #endregion

                #region main + signature
                int x = 2000, y = 0;
                if (Program.gTimer > 1410 && Program.gTimer <= 1430)
                {
                    x = 800;
                    y = 750 - (Program.gTimer - 1410) * 9;
                }
                else if (Program.gTimer > 1430 && Program.gTimer <= 1500)
                {
                    x = (int)(800 + (Program.gTimer - 1430) * 3.57f);
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
                    if (MathF.Abs(temp - (lTimer + 434)) < 6)
                    {
                        lTimer = (byte)Program.RNG.Next(107, 147);
                        for (int i = 0; i < stars.GetLength(0); i++)
                        {
                            if (stars[i, 0] == -1)
                            {
                                stars[i, 0] = (short)x;
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
                    y = 561 + (Program.gTimer - 1500) * 9;
                }
                NouveauSDLRenderDrawLine(Program.render, x, y, x + 13, y - 27);
                NouveauSDLRenderDrawLine(Program.render, x + 13, y - 27, x + 46, y - 70);
                NouveauSDLRenderDrawLine(Program.render, x + 46, y - 70, x + 53, y - 63);
                NouveauSDLRenderDrawLine(Program.render, x + 53, y - 63, x + 18, y - 19);
                NouveauSDLRenderDrawLine(Program.render, x + 18, y - 19, x, y);
                NouveauSDLRenderDrawLine(Program.render, x + 43, y - 61, x + 202, y + 178);

                if (Program.gTimer > 1430)
                {
                    for (int i = 1; i < stars.GetLength(0); i++)
                    {
                        if (stars[i, 0] == -1 || stars[1, 0] == -1)
                            break;
                        NouveauSDLRenderDrawLine(Program.render, stars[i - 1, 0], stars[i - 1, 1], stars[i, 0], stars[i, 1]);
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
                                 "des milliards sont morts, victimes de cette guerre. des milliards de trop.", (20, 700), 3);
            } // signature - fini
            else if (Program.gTimer >= 1560 && Program.gTimer < 1740)
            {
                Text.DisplayText("les factions galactiques prospèrent maintenant tous économiquement\n" +
                                 "avec leurs liens d'amitié entre eux.", (20, 700), 3, scroll: (int)(Program.gTimer - 1560));

                #region planètes
                SDL_SetRenderDrawColor(Program.render, 255, 127, 0, 255);
                DessinerCercle((960, 440), 200, 50);
                SDL_SetRenderDrawColor(Program.render, 255, 0, 127, 255);
                DessinerCercle((260, 390), 100, 50);
                SDL_SetRenderDrawColor(Program.render, 255, 127, 127, 255);
                DessinerCercle((1660, 390), 100, 50);

                SDL_SetRenderDrawColor(Program.render, 127, 255, 127, 255);
                NouveauSDLRenderDrawLine(Program.render, 818, 298, 930, 336);
                NouveauSDLRenderDrawLine(Program.render, 930, 336, 971, 355);
                NouveauSDLRenderDrawLine(Program.render, 971, 355, 910, 373);
                NouveauSDLRenderDrawLine(Program.render, 910, 373, 860, 400);
                NouveauSDLRenderDrawLine(Program.render, 860, 400, 893, 412);
                NouveauSDLRenderDrawLine(Program.render, 893, 412, 906, 438);
                NouveauSDLRenderDrawLine(Program.render, 906, 438, 861, 453);
                NouveauSDLRenderDrawLine(Program.render, 861, 453, 766, 492);
                NouveauSDLRenderDrawLine(Program.render, 1160, 440, 1066, 425);
                NouveauSDLRenderDrawLine(Program.render, 1066, 425, 1000, 455);
                NouveauSDLRenderDrawLine(Program.render, 1000, 455, 1002, 490);
                NouveauSDLRenderDrawLine(Program.render, 1002, 490, 1036, 497);
                NouveauSDLRenderDrawLine(Program.render, 1036, 497, 1048, 515);
                NouveauSDLRenderDrawLine(Program.render, 1048, 515, 989, 545);
                NouveauSDLRenderDrawLine(Program.render, 989, 545, 1050, 583);
                NouveauSDLRenderDrawLine(Program.render, 1050, 583, 1101, 581);

                SDL_SetRenderDrawColor(Program.render, 0, 0, 255, 255);
                NouveauSDLRenderDrawLine(Program.render, 189, 319, 218, 334);
                NouveauSDLRenderDrawLine(Program.render, 218, 334, 250, 344);
                NouveauSDLRenderDrawLine(Program.render, 250, 344, 258, 365);
                NouveauSDLRenderDrawLine(Program.render, 258, 365, 237, 395);
                NouveauSDLRenderDrawLine(Program.render, 237, 395, 219, 425);
                NouveauSDLRenderDrawLine(Program.render, 219, 425, 227, 454);
                NouveauSDLRenderDrawLine(Program.render, 227, 454, 234, 486);
                NouveauSDLRenderDrawLine(Program.render, 307, 302, 302, 330);
                NouveauSDLRenderDrawLine(Program.render, 302, 330, 318, 339);
                NouveauSDLRenderDrawLine(Program.render, 318, 339, 342, 333);
                NouveauSDLRenderDrawLine(Program.render, 360, 390, 354, 406);
                NouveauSDLRenderDrawLine(Program.render, 354, 406, 340, 411);
                NouveauSDLRenderDrawLine(Program.render, 340, 411, 322, 395);
                NouveauSDLRenderDrawLine(Program.render, 322, 395, 294, 400);
                NouveauSDLRenderDrawLine(Program.render, 294, 400, 272, 426);
                NouveauSDLRenderDrawLine(Program.render, 272, 426, 304, 450);
                NouveauSDLRenderDrawLine(Program.render, 304, 450, 330, 460);

                SDL_SetRenderDrawColor(Program.render, 255, 0, 0, 255);
                NouveauSDLRenderDrawLine(Program.render, 1657, 489, 1605, 443);
                NouveauSDLRenderDrawLine(Program.render, 1605, 443, 1579, 393);
                NouveauSDLRenderDrawLine(Program.render, 1579, 393, 1583, 350);
                NouveauSDLRenderDrawLine(Program.render, 1583, 350, 1599, 310);
                NouveauSDLRenderDrawLine(Program.render, 1700, 481, 1674, 457);
                NouveauSDLRenderDrawLine(Program.render, 1674, 457, 1648, 422);
                NouveauSDLRenderDrawLine(Program.render, 1648, 422, 1633, 361);
                NouveauSDLRenderDrawLine(Program.render, 1633, 361, 1637, 321);
                NouveauSDLRenderDrawLine(Program.render, 1637, 321, 1647, 290);
                NouveauSDLRenderDrawLine(Program.render, 1740, 449, 1713, 419);
                NouveauSDLRenderDrawLine(Program.render, 1713, 419, 1689, 371);
                NouveauSDLRenderDrawLine(Program.render, 1689, 371, 1686, 331);
                NouveauSDLRenderDrawLine(Program.render, 1686, 331, 1702, 299);
                NouveauSDLRenderDrawLine(Program.render, 1759, 385, 1744, 371);
                NouveauSDLRenderDrawLine(Program.render, 1744, 371, 1735, 349);
                NouveauSDLRenderDrawLine(Program.render, 1735, 349, 1738, 328);
                #endregion

                #region drapeaux
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                NouveauSDLRenderDrawLine(Program.render, 950, 100, 957, 240);
                NouveauSDLRenderDrawLine(Program.render, 1644, 178, 1653, 290);
                NouveauSDLRenderDrawLine(Program.render, 252, 174, 258, 290);

                SDL_SetRenderDrawColor(Program.render, 255, 0, 0, 255);
                NouveauSDLRenderDrawLine(Program.render, 252, 174, 333, 176);
                NouveauSDLRenderDrawLine(Program.render, 333, 176, 333, 225);
                NouveauSDLRenderDrawLine(Program.render, 333, 225, 255, 229);

                SDL_SetRenderDrawColor(Program.render, 0, 0, 255, 255);
                NouveauSDLRenderDrawLine(Program.render, 950, 100, 1081, 92);
                NouveauSDLRenderDrawLine(Program.render, 1081, 92, 1082, 158);
                NouveauSDLRenderDrawLine(Program.render, 1082, 158, 954, 163);

                SDL_SetRenderDrawColor(Program.render, 0, 255, 0, 255);
                NouveauSDLRenderDrawLine(Program.render, 1644, 178, 1747, 172);
                NouveauSDLRenderDrawLine(Program.render, 1747, 172, 1750, 230);
                NouveauSDLRenderDrawLine(Program.render, 1750, 230, 1649, 235);
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
                    NouveauSDLRenderDrawPoint(Program.render, stars[i, 0], stars[i, 1]);
                }
                #endregion

                #region vols
                SDL_SetRenderDrawColor(Program.render, 255, 255, 0, 255);
                short lz = (short)(Program.gTimer - 1560);
                NouveauSDLRenderDrawPoint(Program.render, 304 + lz, 360);
                NouveauSDLRenderDrawPoint(Program.render, 631 - lz, 454);
                NouveauSDLRenderDrawPoint(Program.render, 872 + lz, 354);
                NouveauSDLRenderDrawPoint(Program.render, 1097 + lz, 479);
                NouveauSDLRenderDrawPoint(Program.render, 1300 - lz, 372);
                NouveauSDLRenderDrawPoint(Program.render, 1600 - lz, 418);
                #endregion
            } // vols entre planètes - fini
            else if (Program.gTimer >= 1740 && Program.gTimer < 1920)
            {
                Text.DisplayText("et même si le trou laissé par l'explosion laissera une empreinte\n" +
                                    "pour quelque temps...", (20, 700), 3, scroll: (int)(Program.gTimer - 1740));

                #region étoiles
                if (Program.gTimer == 1740)
                {
                    for (int i = 0; i < stars.GetLength(0); i++)
                    {
                        int x = Program.RNG.Next(25, 1880), y = Program.RNG.Next(25, 680);
                        while (Vector2.Distance(x, y, Program.W_SEMI_LARGEUR, Program.W_SEMI_HAUTEUR / 2 + 100, 0.2f) < 200)
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
                    NouveauSDLRenderDrawPoint(Program.render, stars[i, 0], stars[i, 1]);
                }
                #endregion

                #region vols
                SDL_SetRenderDrawColor(Program.render, 255, 255, 0, 255);
                short lz = (short)(Program.gTimer - 1740);
                NouveauSDLRenderDrawPoint(Program.render, 152 + lz, 199);
                NouveauSDLRenderDrawPoint(Program.render, 556 - lz, 104 + lz);
                NouveauSDLRenderDrawPoint(Program.render, 257, 505 - lz);
                NouveauSDLRenderDrawPoint(Program.render, 1176 + lz, 643);
                NouveauSDLRenderDrawPoint(Program.render, 1690, 433 - lz);
                NouveauSDLRenderDrawPoint(Program.render, 1608 + lz, 214 + lz);
                NouveauSDLRenderDrawPoint(Program.render, 1429 + lz, 125);
                NouveauSDLRenderDrawPoint(Program.render, 626 - lz, 643);
                NouveauSDLRenderDrawPoint(Program.render, 1284 - lz, 176);
                #endregion
            } // vols entre étoiles - fini
            else if (Program.gTimer >= 1920 && Program.gTimer < 2310)
            {
                Text.DisplayText("même les plus grandes cicatrices se guérissent éventuellement.", (20, 700), 3, scroll: (int)(Program.gTimer / 2 - 960));

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
                        if (Vector2.Distance(stars_glx[i, 0], stars_glx[i, 1], 716, 437, 0.5f) < 100)
                        {
                            stars_glx[i, 0] = -1;
                            stars_glx[i, 1] = -1;
                        }
                    }
                }
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                for (int i = 0; i < stars.GetLength(0); i++)
                {
                    NouveauSDLRenderDrawPoint(Program.render, stars[i, 0], stars[i, 1]);
                }
                for (int i = 0; i < stars_glx.GetLength(0); i++)
                {
                    NouveauSDLRenderDrawPoint(Program.render, stars_glx[i, 0], stars_glx[i, 1]);
                }
                if (Program.gTimer > 2070 && Program.gTimer % 10 == 0)
                {
                    for (int i = 0; i < stars_glx.GetLength(0); i++)
                    {
                        if (stars_glx[i, 0] == -1)
                        {
                            int x = Program.RNG.Next(516, 916), y = Program.RNG.Next(337, 537);
                            while (Vector2.Distance(x, y, 716, 437, 0.5f) > 100)
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
                NouveauSDLRenderDrawLine(Program.render, 525, 362, 477, 195);
                NouveauSDLRenderDrawLine(Program.render, 477, 195, 547, 223);
                NouveauSDLRenderDrawLine(Program.render, 547, 223, 686, 91);
                NouveauSDLRenderDrawLine(Program.render, 686, 91, 697, 141);
                NouveauSDLRenderDrawLine(Program.render, 697, 141, 950, 79);
                NouveauSDLRenderDrawLine(Program.render, 950, 79, 933, 126);
                NouveauSDLRenderDrawLine(Program.render, 933, 126, 1230, 92);
                NouveauSDLRenderDrawLine(Program.render, 1230, 92, 1191, 126);
                NouveauSDLRenderDrawLine(Program.render, 1191, 126, 1404, 179);
                NouveauSDLRenderDrawLine(Program.render, 1404, 179, 1344, 186);
                NouveauSDLRenderDrawLine(Program.render, 1344, 186, 1434, 323);
                NouveauSDLRenderDrawLine(Program.render, 1434, 323, 1370, 312);
                NouveauSDLRenderDrawLine(Program.render, 1370, 312, 1404, 441);
                NouveauSDLRenderDrawLine(Program.render, 1404, 441, 1354, 427);
                NouveauSDLRenderDrawLine(Program.render, 1354, 427, 1285, 571);
                NouveauSDLRenderDrawLine(Program.render, 1285, 571, 1285, 499);
                NouveauSDLRenderDrawLine(Program.render, 1285, 499, 1017, 600);
                NouveauSDLRenderDrawLine(Program.render, 1017, 600, 1032, 538);
                NouveauSDLRenderDrawLine(Program.render, 1032, 538, 700, 600);
                NouveauSDLRenderDrawLine(Program.render, 700, 600, 734, 553);
                NouveauSDLRenderDrawLine(Program.render, 734, 553, 500, 500);
                NouveauSDLRenderDrawLine(Program.render, 500, 500, 600, 500);
                NouveauSDLRenderDrawLine(Program.render, 600, 500, 451, 377);
                NouveauSDLRenderDrawLine(Program.render, 451, 377, 525, 362);

                SDL_SetRenderDrawColor(Program.render, 127, 0, 127, 255);
                NouveauSDLRenderDrawLine(Program.render, 850, 350, 833, 306);
                NouveauSDLRenderDrawLine(Program.render, 833, 306, 842, 271);
                NouveauSDLRenderDrawLine(Program.render, 842, 271, 878, 221);
                NouveauSDLRenderDrawLine(Program.render, 878, 221, 952, 193);
                NouveauSDLRenderDrawLine(Program.render, 952, 193, 1027, 216);
                NouveauSDLRenderDrawLine(Program.render, 1027, 216, 1064, 260);
                NouveauSDLRenderDrawLine(Program.render, 1064, 260, 1076, 299);
                NouveauSDLRenderDrawLine(Program.render, 1076, 299, 1070, 340);

                NouveauSDLRenderDrawLine(Program.render, 828, 315, 798, 324);
                NouveauSDLRenderDrawLine(Program.render, 798, 324, 800, 350);
                NouveauSDLRenderDrawLine(Program.render, 800, 350, 831, 359);
                NouveauSDLRenderDrawLine(Program.render, 831, 359, 1082, 347);
                NouveauSDLRenderDrawLine(Program.render, 1082, 347, 1114, 333);
                NouveauSDLRenderDrawLine(Program.render, 1114, 333, 1112, 309);
                NouveauSDLRenderDrawLine(Program.render, 1112, 309, 1081, 304);
                #endregion
            } // étoiles qui reviennent - fini

            if (Program.gTimer > 30 && Program.gTimer < 2340)
            {
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                NouveauSDLRenderDrawLine(Program.render, 20, 20, 1900, 20);
                NouveauSDLRenderDrawLine(Program.render, 1900, 20, 1900, 680);
                NouveauSDLRenderDrawLine(Program.render, 1900, 680, 20, 680);
                NouveauSDLRenderDrawLine(Program.render, 20, 20, 20, 680);
            }

            if (Program.gTimer > 2400)
            {
                Program.Gamemode = Gamemode.CUTSCENE_GENERIQUE;
                Program.player.Init();
                //Program.gFade = 0;
                Program.enemies.Clear();
            }
        }
        public static void Cut_3() // bad end
        {
            if (Program.cutscene_skip_partiel || Program.cutscene_skip_complet)
                Program.gTimer = 1861;

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
                    NouveauSDLRenderDrawPoint(Program.render, stars[i, 0], stars[i, 1]);
                }
                #endregion

                #region Program.player
                Program.player.position = new Vector3(277, 519, 0);
                Program.player.pitch = 0.7f;
                Program.player.roll = 0.8f;
                Program.player.taille = 2;
                Program.player.RenderObjet();
                #endregion

                #region enemy 15
                short death_timer = (short)(Program.gTimer - 60);
                if (death_timer > 255)
                    death_timer = 255;
                Program.player.taille = death_timer / 60f + 0.5f;
                Program.player.position = new Vector3(771 - death_timer, 325 + death_timer, 0);
                Program.player.roll = death_timer / 250;
                double sinroll = MathF.Sin(Program.player.roll);
                double cosroll = MathF.Cos(Program.player.roll);
                float pitchconst = Program.player.pitch + 0.3f;//joueur permapitch
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
                    NouveauSDLRenderDrawLine(Program.render, pos[0], pos[1], pos[2], pos[3]);
                }
                #endregion

                #region pulsar bomb
                BombePulsar.DessinerBombePulsar((1522, 264), 133, false);

                SDL_SetRenderDrawColor(Program.render, 0, 0, 255, 255);
                NouveauSDLRenderDrawLine(Program.render, 1463, 144, 1445, 97);
                NouveauSDLRenderDrawLine(Program.render, 1445, 97, 1422, 68);
                NouveauSDLRenderDrawLine(Program.render, 1422, 68, 1373, 46);
                NouveauSDLRenderDrawLine(Program.render, 1525, 131, 1522, 86);
                NouveauSDLRenderDrawLine(Program.render, 1522, 86, 1554, 48);
                NouveauSDLRenderDrawLine(Program.render, 1554, 48, 1584, 35);
                NouveauSDLRenderDrawLine(Program.render, 1501, 36, 1499, 82);
                NouveauSDLRenderDrawLine(Program.render, 1464, 63, 1484, 112);
                NouveauSDLRenderDrawLine(Program.render, 1500, 396, 1491, 454);
                NouveauSDLRenderDrawLine(Program.render, 1491, 454, 1470, 493);
                NouveauSDLRenderDrawLine(Program.render, 1470, 493, 1450, 534);
                NouveauSDLRenderDrawLine(Program.render, 1549, 395, 1550, 450);
                NouveauSDLRenderDrawLine(Program.render, 1550, 450, 1574, 486);
                NouveauSDLRenderDrawLine(Program.render, 1574, 486, 1612, 513);
                NouveauSDLRenderDrawLine(Program.render, 1514, 434, 1506, 485);
                NouveauSDLRenderDrawLine(Program.render, 1539, 475, 1560, 520);
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
                    NouveauSDLRenderDrawPoint(Program.render, stars[i, 0], stars[i, 1]);
                }
                #endregion

                #region pulsar bomb
                if (Program.gTimer < 300)
                    BombePulsar.DessinerBombePulsar((956, 336), 224, false);
                else if (Program.gTimer >= 300 && Program.gTimer < 330)
                {
                    SDL_SetRenderDrawColor(Program.render, 200, 255, 255, 255);
                    if (Program.gTimer % MathF.Ceiling((Program.gTimer - 299) / 10f) == 0)
                        for (int i = 0; i < neutron_slowdown.Length; i++)
                        {
                            float ang = Program.RNG.NextSingle() * MathF.PI;
                            neutron_slowdown[i].x = (short)(Program.RNG.Next(-224, 224) * MathF.Cos(ang) + 956);
                            neutron_slowdown[i].y = (short)(Program.RNG.Next(-224, 224) * MathF.Sin(ang) + 336);
                        }

                    BombePulsar.DessinerBombePulsar((956, 336), 224, BombePulsar.COULEUR_BOMBE, false, neutron_slowdown);
                }
                else if (Program.gTimer >= 330)
                {
                    SDL_SetRenderDrawColor(Program.render, BombePulsar.COULEUR_BOMBE.r, BombePulsar.COULEUR_BOMBE.g, BombePulsar.COULEUR_BOMBE.b, (byte)alpha);
                    DessinerCercle((956, 336), 224, 50);
                    for (int i = 0; i < 50; i++)
                        NouveauSDLRenderDrawLine(Program.render, (int)neutron_slowdown[i].x, (int)neutron_slowdown[i].y, 956, 336);
                }
                #endregion

                #region bleu
                if (Program.gTimer < 330)
                    SDL_SetRenderDrawColor(Program.render, 0, 0, 255, 255);
                else
                    SDL_SetRenderDrawColor(Program.render, 0, 0, 255, (byte)alpha);

                NouveauSDLRenderDrawLine(Program.render, 834, 148, 811, 86);
                NouveauSDLRenderDrawLine(Program.render, 811, 86, 760, 50);
                NouveauSDLRenderDrawLine(Program.render, 760, 50, 693, 21);
                NouveauSDLRenderDrawLine(Program.render, 935, 113, 930, 61);
                NouveauSDLRenderDrawLine(Program.render, 930, 61, 940, 21);
                NouveauSDLRenderDrawLine(Program.render, 867, 112, 849, 64);
                NouveauSDLRenderDrawLine(Program.render, 849, 64, 831, 37);
                NouveauSDLRenderDrawLine(Program.render, 894, 57, 895, 21);
                NouveauSDLRenderDrawLine(Program.render, 948, 560, 949, 614);
                NouveauSDLRenderDrawLine(Program.render, 949, 614, 931, 679);
                NouveauSDLRenderDrawLine(Program.render, 1041, 543, 1066, 591);
                NouveauSDLRenderDrawLine(Program.render, 1066, 591, 1134, 632);
                NouveauSDLRenderDrawLine(Program.render, 1134, 632, 1207, 654);
                NouveauSDLRenderDrawLine(Program.render, 1032, 578, 1053, 614);
                NouveauSDLRenderDrawLine(Program.render, 1053, 614, 1090, 648);
                NouveauSDLRenderDrawLine(Program.render, 1000, 600, 1001, 640);
                NouveauSDLRenderDrawLine(Program.render, 1001, 640, 989, 679);
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
                    NouveauSDLRenderDrawPoint(Program.render, stars[i, 0], stars[i, 1]);
                }
                #endregion

                #region portal
                SDL_SetRenderDrawColor(Program.render, 0, 255, 255, 255);
                for (int i = 0; i < 50; i++)
                {
                    float ang = Program.RNG.NextSingle() * MathF.PI;
                    NouveauSDLRenderDrawLine(Program.render, (int)(Program.RNG.Next(-150, 150) * MathF.Cos(ang) + 960), (int)(Program.RNG.Next(-80, 80) * MathF.Sin(ang) + 130), 960, 130);
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
                        NouveauSDLRenderDrawLine(Program.render,
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
                    NouveauSDLRenderDrawPoint(Program.render, stars[i, 0], stars[i, 1]);
                }
                #endregion

                #region planète
                SDL_SetRenderDrawColor(Program.render, 255, 0, 0, 255);
                NouveauSDLRenderDrawLine(Program.render, 21, 598, 200, 500);
                NouveauSDLRenderDrawLine(Program.render, 200, 500, 598, 448);
                NouveauSDLRenderDrawLine(Program.render, 598, 448, 1300, 448);
                NouveauSDLRenderDrawLine(Program.render, 1300, 448, 1700, 500);
                NouveauSDLRenderDrawLine(Program.render, 1700, 500, 1899, 600);
                #endregion

                #region vieu drapeau
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                NouveauSDLRenderDrawLine(Program.render, 849, 448, 849, 62);

                if (Program.gTimer < 700)
                {
                    short move = (short)((Program.gTimer - 630) * 8);
                    if (move < 0)
                        move = 0;
                    if (move > 450)
                        move = 450;
                    SDL_SetRenderDrawColor(Program.render, 255, 0, 0, 255);
                    NouveauSDLRenderDrawLine(Program.render, 850, 62 + move, 1247, 60 + move);
                    NouveauSDLRenderDrawLine(Program.render, 1247, 60 + move, 1247, 264 + move);
                    NouveauSDLRenderDrawLine(Program.render, 1247, 264 + move, 850, 273 + move);
                    NouveauSDLRenderDrawLine(Program.render, 850, 273 + move, 850, 62 + move);

                    SDL_SetRenderDrawColor(Program.render, 0, 0, 255, 255);
                    NouveauSDLRenderDrawLine(Program.render, 848, 232 + move, 1191, 61 + move);
                    NouveauSDLRenderDrawLine(Program.render, 1246, 90 + move, 886, 272 + move);

                    SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                    NouveauSDLRenderDrawLine(Program.render, 1177, 166 + move, 1200, 250 + move);
                    NouveauSDLRenderDrawLine(Program.render, 1200, 250 + move, 1136, 200 + move);
                    NouveauSDLRenderDrawLine(Program.render, 1136, 200 + move, 1215, 200 + move);
                    NouveauSDLRenderDrawLine(Program.render, 1215, 200 + move, 1150, 250 + move);
                    NouveauSDLRenderDrawLine(Program.render, 1150, 250 + move, 1177, 166 + move);
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
                    NouveauSDLRenderDrawLine(Program.render, 850, 449 - move, 1251, 450 - move);
                    NouveauSDLRenderDrawLine(Program.render, 1251, 450 - move, 1256, 680 - move);
                    NouveauSDLRenderDrawLine(Program.render, 1256, 680 - move, 850, 680 - move);
                    NouveauSDLRenderDrawLine(Program.render, 850, 680 - move, 850, 449 - move);

                    SDL_SetRenderDrawColor(Program.render, 255, 127, 0, 255);
                    DessinerCercle((1050, 560 - move), 84, 50);
                    DessinerCercle((1050, 560 - move), 63, 50);

                    SDL_SetRenderDrawColor(Program.render, 0, 255, 0, 255);
                    NouveauSDLRenderDrawLine(Program.render, 850, 518 - move, 978, 518 - move);
                    NouveauSDLRenderDrawLine(Program.render, 976, 599 - move, 848, 599 - move);
                    NouveauSDLRenderDrawLine(Program.render, 1124, 521 - move, 1253, 519 - move);
                    NouveauSDLRenderDrawLine(Program.render, 1124, 600 - move, 1254, 600 - move);

                    SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                    NouveauSDLRenderDrawLine(Program.render, 1050, 498 - move, 1090, 609 - move);
                    NouveauSDLRenderDrawLine(Program.render, 1090, 609 - move, 992, 536 - move);
                    NouveauSDLRenderDrawLine(Program.render, 992, 536 - move, 1106, 533 - move);
                    NouveauSDLRenderDrawLine(Program.render, 1106, 533 - move, 1017, 614 - move);
                    NouveauSDLRenderDrawLine(Program.render, 1017, 614 - move, 1050, 498 - move);
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
                NouveauSDLRenderDrawLine(Program.render, 248, 680, 156, 309);
                NouveauSDLRenderDrawLine(Program.render, 156, 309, 303, 243);
                NouveauSDLRenderDrawLine(Program.render, 303, 243, 400, 300);
                NouveauSDLRenderDrawLine(Program.render, 400, 300, 499, 249);
                NouveauSDLRenderDrawLine(Program.render, 499, 249, 630, 310);
                NouveauSDLRenderDrawLine(Program.render, 630, 310, 539, 680);

                DessinerCercle((400, 200), 78, 50);

                NouveauSDLRenderDrawLine(Program.render, 156, 309, 159, 649);
                NouveauSDLRenderDrawLine(Program.render, 630, 310, 685, 215);
                NouveauSDLRenderDrawLine(Program.render, 685, 215, 385, 140);

                SDL_SetRenderDrawColor(Program.render, 255, 255, 0, 255);
                NouveauSDLRenderDrawLine(Program.render, 480, 380, 460, 400);
                NouveauSDLRenderDrawLine(Program.render, 460, 400, 480, 420);
                NouveauSDLRenderDrawLine(Program.render, 480, 420, 500, 400);
                NouveauSDLRenderDrawLine(Program.render, 500, 400, 480, 380);

                NouveauSDLRenderDrawLine(Program.render, 540, 380, 520, 400);
                NouveauSDLRenderDrawLine(Program.render, 520, 400, 540, 420);
                NouveauSDLRenderDrawLine(Program.render, 540, 420, 560, 400);
                NouveauSDLRenderDrawLine(Program.render, 560, 400, 540, 380);

                SDL_SetRenderDrawColor(Program.render, 127, 0, 127, 255);
                NouveauSDLRenderDrawLine(Program.render, 480, 380, 460, 320);
                NouveauSDLRenderDrawLine(Program.render, 460, 320, 500, 320);
                NouveauSDLRenderDrawLine(Program.render, 500, 320, 480, 380);

                SDL_SetRenderDrawColor(Program.render, 127, 255, 127, 255);
                NouveauSDLRenderDrawLine(Program.render, 540, 380, 520, 320);
                NouveauSDLRenderDrawLine(Program.render, 520, 320, 560, 320);
                NouveauSDLRenderDrawLine(Program.render, 560, 320, 540, 380);
                #endregion

                #region chef
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                NouveauSDLRenderDrawLine(Program.render, 865, 680, 800, 300);
                NouveauSDLRenderDrawLine(Program.render, 800, 300, 879, 260);
                NouveauSDLRenderDrawLine(Program.render, 879, 260, 959, 289);
                NouveauSDLRenderDrawLine(Program.render, 959, 289, 1048, 254);
                NouveauSDLRenderDrawLine(Program.render, 1048, 254, 1118, 295);
                NouveauSDLRenderDrawLine(Program.render, 1118, 295, 1057, 680);

                DessinerCercle((959, 205), 76, 50);

                NouveauSDLRenderDrawLine(Program.render, 893, 166, 1031, 164);
                NouveauSDLRenderDrawLine(Program.render, 1031, 164, 1018, 127);
                NouveauSDLRenderDrawLine(Program.render, 1018, 127, 886, 114);
                NouveauSDLRenderDrawLine(Program.render, 886, 114, 876, 132);
                NouveauSDLRenderDrawLine(Program.render, 876, 132, 893, 166);

                SDL_SetRenderDrawColor(Program.render, 255, 255, 0, 255);
                NouveauSDLRenderDrawLine(Program.render, 892, 133, 904, 123);
                NouveauSDLRenderDrawLine(Program.render, 904, 123, 918, 133);
                NouveauSDLRenderDrawLine(Program.render, 918, 133, 906, 145);
                NouveauSDLRenderDrawLine(Program.render, 906, 145, 892, 133);

                NouveauSDLRenderDrawLine(Program.render, 1020, 360, 1000, 380);
                NouveauSDLRenderDrawLine(Program.render, 1000, 380, 1020, 400);
                NouveauSDLRenderDrawLine(Program.render, 1020, 400, 1040, 380);
                NouveauSDLRenderDrawLine(Program.render, 1040, 380, 1020, 360);

                NouveauSDLRenderDrawLine(Program.render, 1080, 360, 1060, 380);
                NouveauSDLRenderDrawLine(Program.render, 1060, 380, 1080, 400);
                NouveauSDLRenderDrawLine(Program.render, 1080, 400, 1100, 380);
                NouveauSDLRenderDrawLine(Program.render, 1100, 380, 1080, 360);

                NouveauSDLRenderDrawLine(Program.render, 1000, 400, 1020, 420);
                NouveauSDLRenderDrawLine(Program.render, 1020, 420, 1040, 400);
                NouveauSDLRenderDrawLine(Program.render, 1040, 400, 1030, 390);
                NouveauSDLRenderDrawLine(Program.render, 1010, 390, 1000, 400);

                NouveauSDLRenderDrawLine(Program.render, 1070, 390, 1060, 400);
                NouveauSDLRenderDrawLine(Program.render, 1060, 400, 1080, 420);
                NouveauSDLRenderDrawLine(Program.render, 1080, 420, 1100, 400);
                NouveauSDLRenderDrawLine(Program.render, 1100, 400, 1090, 390);

                SDL_SetRenderDrawColor(Program.render, 127, 0, 127, 255);
                NouveauSDLRenderDrawLine(Program.render, 1000, 300, 1040, 300);
                NouveauSDLRenderDrawLine(Program.render, 1040, 300, 1020, 360);
                NouveauSDLRenderDrawLine(Program.render, 1020, 360, 1000, 300);

                SDL_SetRenderDrawColor(Program.render, 255, 0, 0, 255);
                NouveauSDLRenderDrawLine(Program.render, 1060, 300, 1100, 300);
                NouveauSDLRenderDrawLine(Program.render, 1100, 300, 1080, 360);
                NouveauSDLRenderDrawLine(Program.render, 1080, 360, 1060, 300);

                SDL_SetRenderDrawColor(Program.render, 127, 127, 255, 255);
                NouveauSDLRenderDrawLine(Program.render, 1000, 340, 1013, 340);
                NouveauSDLRenderDrawLine(Program.render, 1027, 340, 1040, 340);
                NouveauSDLRenderDrawLine(Program.render, 1040, 340, 1030, 370);
                NouveauSDLRenderDrawLine(Program.render, 1011, 369, 1000, 340);

                SDL_SetRenderDrawColor(Program.render, 255, 127, 0, 255);
                NouveauSDLRenderDrawLine(Program.render, 1060, 340, 1073, 340);
                NouveauSDLRenderDrawLine(Program.render, 1087, 340, 1100, 340);
                NouveauSDLRenderDrawLine(Program.render, 1100, 340, 1091, 371);
                NouveauSDLRenderDrawLine(Program.render, 1071, 369, 1060, 340);

                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                NouveauSDLRenderDrawLine(Program.render, 800, 300, 553, 328);
                NouveauSDLRenderDrawLine(Program.render, 531, 340, 1118, 295);
                #endregion

                #region drapeaux
                for (short i = 0; i < 401; i += 200)
                {
                    SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                    NouveauSDLRenderDrawLine(Program.render, 1300 + i, 680, 1300 + i, 494);
                    NouveauSDLRenderDrawLine(Program.render, 1300 + i, 100, 1300 + i, 365);

                    SDL_SetRenderDrawColor(Program.render, 255, 255, 0, 255);
                    NouveauSDLRenderDrawLine(Program.render, 1300 + i, 100, 1387 + i, 272);
                    NouveauSDLRenderDrawLine(Program.render, 1387 + i, 272, 1400 + i, 400);
                    NouveauSDLRenderDrawLine(Program.render, 1400 + i, 400, 1376 + i, 449);
                    NouveauSDLRenderDrawLine(Program.render, 1376 + i, 449, 1369 + i, 519);
                    NouveauSDLRenderDrawLine(Program.render, 1369 + i, 519, 1390 + i, 557);
                    NouveauSDLRenderDrawLine(Program.render, 1390 + i, 557, 1332 + i, 574);
                    NouveauSDLRenderDrawLine(Program.render, 1332 + i, 574, 1282 + i, 449);
                    NouveauSDLRenderDrawLine(Program.render, 1282 + i, 449, 1287 + i, 389);
                    NouveauSDLRenderDrawLine(Program.render, 1287 + i, 389, 1314 + i, 338);
                    NouveauSDLRenderDrawLine(Program.render, 1314 + i, 338, 1315 + i, 305);
                    NouveauSDLRenderDrawLine(Program.render, 1315 + i, 305, 1300 + i, 300);

                    SDL_SetRenderDrawColor(Program.render, 255, 127, 0, 255);
                    DessinerCercle((1346 + i, 375), 32, 50);

                    SDL_SetRenderDrawColor(Program.render, 0, 255, 0, 255);
                    NouveauSDLRenderDrawLine(Program.render, 1368 + i, 352, 1371 + i, 309);
                    NouveauSDLRenderDrawLine(Program.render, 1371 + i, 309, 1346 + i, 253);
                    NouveauSDLRenderDrawLine(Program.render, 1346 + i, 253, 1300 + i, 170);
                    NouveauSDLRenderDrawLine(Program.render, 1339 + i, 343, 1340 + i, 308);
                    NouveauSDLRenderDrawLine(Program.render, 1340 + i, 308, 1321 + i, 268);
                    NouveauSDLRenderDrawLine(Program.render, 1321 + i, 268, 1300 + i, 249);
                    NouveauSDLRenderDrawLine(Program.render, 1325 + i, 400, 1311 + i, 437);
                    NouveauSDLRenderDrawLine(Program.render, 1311 + i, 437, 1318 + i, 488);
                    NouveauSDLRenderDrawLine(Program.render, 1318 + i, 488, 1351 + i, 568);
                    NouveauSDLRenderDrawLine(Program.render, 1375 + i, 561, 1350 + i, 500);
                    NouveauSDLRenderDrawLine(Program.render, 1350 + i, 500, 1339 + i, 441);
                    NouveauSDLRenderDrawLine(Program.render, 1339 + i, 441, 1351 + i, 407);
                }
                #endregion
            } // honneure - fini
            else if (Program.gTimer >= 960 && Program.gTimer < 1140)
            {
                #region toi + chaise
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                NouveauSDLRenderDrawLine(Program.render, 1034, 680, 1030, 521);
                NouveauSDLRenderDrawLine(Program.render, 1030, 521, 1085, 470);
                NouveauSDLRenderDrawLine(Program.render, 1085, 470, 1623, 474);
                NouveauSDLRenderDrawLine(Program.render, 1623, 474, 1684, 523);
                NouveauSDLRenderDrawLine(Program.render, 1684, 523, 1682, 680);

                NouveauSDLRenderDrawLine(Program.render, 1512, 473, 1647, 110);
                NouveauSDLRenderDrawLine(Program.render, 1647, 110, 1715, 80);
                NouveauSDLRenderDrawLine(Program.render, 1715, 80, 1785, 117);
                NouveauSDLRenderDrawLine(Program.render, 1785, 117, 1811, 165);
                NouveauSDLRenderDrawLine(Program.render, 1811, 165, 1684, 523);

                if (Program.gTimer < 1050)
                {
                    NouveauSDLRenderDrawLine(Program.render, 1500, 200, 1449, 203);
                    NouveauSDLRenderDrawLine(Program.render, 1449, 203, 1415, 472);
                    NouveauSDLRenderDrawLine(Program.render, 1513, 253, 1444, 505);
                    NouveauSDLRenderDrawLine(Program.render, 1444, 505, 1249, 508);
                    DessinerCercle((1555, 157), 70, 50);
                }
                else
                {
                    NouveauSDLRenderDrawLine(Program.render, 1246, 471, 1199, 277);
                    NouveauSDLRenderDrawLine(Program.render, 1199, 277, 1400, 250);
                    NouveauSDLRenderDrawLine(Program.render, 1400, 250, 1448, 472);
                    NouveauSDLRenderDrawLine(Program.render, 1199, 277, 1180, 494);
                    NouveauSDLRenderDrawLine(Program.render, 1180, 494, 1078, 539);
                    NouveauSDLRenderDrawLine(Program.render, 1400, 250, 1482, 323);
                    NouveauSDLRenderDrawLine(Program.render, 1482, 323, 1494, 471);
                    DessinerCercle((1266, 220), 70, 50);
                }
                #endregion

                #region fenêtre
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                NouveauSDLRenderDrawLine(Program.render, 800, 100, 1050, 100);
                NouveauSDLRenderDrawLine(Program.render, 1050, 100, 1050, 400);
                NouveauSDLRenderDrawLine(Program.render, 1050, 400, 800, 400);
                NouveauSDLRenderDrawLine(Program.render, 800, 400, 800, 100);
                NouveauSDLRenderDrawLine(Program.render, 800, 250, 1050, 250);
                NouveauSDLRenderDrawLine(Program.render, 925, 100, 925, 400);

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
                    NouveauSDLRenderDrawPoint(Program.render, stars[i, 0], stars[i, 1]);
                }
                #endregion

                #region table + médailles
                SDL_SetRenderDrawColor(Program.render, 120, 50, 0, 255);
                NouveauSDLRenderDrawLine(Program.render, 20, 200, 700, 200);
                NouveauSDLRenderDrawLine(Program.render, 700, 200, 700, 250);
                NouveauSDLRenderDrawLine(Program.render, 700, 250, 20, 250);

                SDL_SetRenderDrawColor(Program.render, 255, 127, 0, 255);
                NouveauSDLRenderDrawLine(Program.render, 99, 219, 150, 219);
                NouveauSDLRenderDrawLine(Program.render, 150, 219, 127, 300);
                NouveauSDLRenderDrawLine(Program.render, 127, 300, 99, 219);

                SDL_SetRenderDrawColor(Program.render, 127, 255, 127, 255);
                NouveauSDLRenderDrawLine(Program.render, 252, 221, 300, 221);
                NouveauSDLRenderDrawLine(Program.render, 300, 221, 276, 301);
                NouveauSDLRenderDrawLine(Program.render, 276, 301, 252, 221);

                SDL_SetRenderDrawColor(Program.render, 127, 0, 127, 255);
                NouveauSDLRenderDrawLine(Program.render, 400, 223, 450, 223);
                NouveauSDLRenderDrawLine(Program.render, 450, 223, 427, 301);
                NouveauSDLRenderDrawLine(Program.render, 427, 301, 400, 223);

                SDL_SetRenderDrawColor(Program.render, 255, 255, 0, 255);
                NouveauSDLRenderDrawLine(Program.render, 127, 300, 101, 323);
                NouveauSDLRenderDrawLine(Program.render, 101, 323, 127, 348);
                NouveauSDLRenderDrawLine(Program.render, 127, 348, 154, 324);
                NouveauSDLRenderDrawLine(Program.render, 154, 324, 127, 300);

                NouveauSDLRenderDrawLine(Program.render, 276, 301, 250, 325);
                NouveauSDLRenderDrawLine(Program.render, 250, 325, 277, 349);
                NouveauSDLRenderDrawLine(Program.render, 277, 349, 304, 325);
                NouveauSDLRenderDrawLine(Program.render, 304, 325, 276, 301);

                NouveauSDLRenderDrawLine(Program.render, 427, 301, 401, 324);
                NouveauSDLRenderDrawLine(Program.render, 401, 324, 427, 349);
                NouveauSDLRenderDrawLine(Program.render, 427, 349, 454, 325);
                NouveauSDLRenderDrawLine(Program.render, 454, 325, 427, 301);
                #endregion

                #region trophés
                for (short i = 0; i < 301; i += 150)
                {
                    NouveauSDLRenderDrawLine(Program.render, 150 + i, 200, 166 + i, 171);
                    NouveauSDLRenderDrawLine(Program.render, 166 + i, 171, 221 + i, 172);
                    NouveauSDLRenderDrawLine(Program.render, 221 + i, 172, 238 + i, 200);
                    NouveauSDLRenderDrawLine(Program.render, 188 + i, 171, 188 + i, 154);
                    NouveauSDLRenderDrawLine(Program.render, 201 + i, 171, 201 + i, 154);
                    NouveauSDLRenderDrawLine(Program.render, 173 + i, 71, 161 + i, 124);
                    NouveauSDLRenderDrawLine(Program.render, 161 + i, 124, 176 + i, 154);
                    NouveauSDLRenderDrawLine(Program.render, 176 + i, 154, 215 + i, 154);
                    NouveauSDLRenderDrawLine(Program.render, 215 + i, 154, 227 + i, 128);
                    NouveauSDLRenderDrawLine(Program.render, 227 + i, 128, 218 + i, 71);
                    NouveauSDLRenderDrawLine(Program.render, 218 + i, 71, 173 + i, 71);
                    NouveauSDLRenderDrawLine(Program.render, 167 + i, 98, 151 + i, 97);
                    NouveauSDLRenderDrawLine(Program.render, 151 + i, 97, 144 + i, 119);
                    NouveauSDLRenderDrawLine(Program.render, 144 + i, 119, 166 + i, 133);
                    NouveauSDLRenderDrawLine(Program.render, 223 + i, 100, 237 + i, 97);
                    NouveauSDLRenderDrawLine(Program.render, 237 + i, 97, 245 + i, 122);
                    NouveauSDLRenderDrawLine(Program.render, 245 + i, 122, 224 + i, 136);
                }
                #endregion
            } // à la maison - fini
            else if (Program.gTimer >= 1140 && Program.gTimer < 1320)
            {
                for (int i = 0; i < Program.explosions.Count; i++)
                {
                    Program.explosions[i].RenderObjet();
                    if (Program.explosions[i].Exist())
                        i--;
                }

                #region bordure
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                NouveauSDLRenderDrawLine(Program.render, 750, 650, 750, 50);
                NouveauSDLRenderDrawLine(Program.render, 750, 50, 1200, 50);
                NouveauSDLRenderDrawLine(Program.render, 1200, 50, 1200, 650);
                NouveauSDLRenderDrawLine(Program.render, 1200, 650, 750, 650);

                NouveauSDLRenderDrawLine(Program.render, 975, 50, 975, 650);
                NouveauSDLRenderDrawLine(Program.render, 750, 350, 1200, 350);

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
                    NouveauSDLRenderDrawPoint(Program.render, stars[i, 0], stars[i, 1]);
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
                NouveauSDLRenderDrawLine(Program.render, 1034, 680, 1030, 521);
                NouveauSDLRenderDrawLine(Program.render, 1030, 521, 1085, 470);
                NouveauSDLRenderDrawLine(Program.render, 1085, 470, 1623, 474);
                NouveauSDLRenderDrawLine(Program.render, 1623, 474, 1684, 523);
                NouveauSDLRenderDrawLine(Program.render, 1684, 523, 1682, 680);

                NouveauSDLRenderDrawLine(Program.render, 1512, 473, 1647, 110);
                NouveauSDLRenderDrawLine(Program.render, 1647, 110, 1715, 80);
                NouveauSDLRenderDrawLine(Program.render, 1715, 80, 1785, 117);
                NouveauSDLRenderDrawLine(Program.render, 1785, 117, 1811, 165);
                NouveauSDLRenderDrawLine(Program.render, 1811, 165, 1684, 523);

                NouveauSDLRenderDrawLine(Program.render, 1318, 225, 1408, 244);
                NouveauSDLRenderDrawLine(Program.render, 1408, 244, 1490, 321);
                NouveauSDLRenderDrawLine(Program.render, 1490, 321, 1527, 433);
                NouveauSDLRenderDrawLine(Program.render, 1272, 317, 1313, 375);
                NouveauSDLRenderDrawLine(Program.render, 1313, 375, 1342, 473);
                NouveauSDLRenderDrawLine(Program.render, 1390, 323, 1296, 531);
                NouveauSDLRenderDrawLine(Program.render, 1296, 531, 1270, 266);
                NouveauSDLRenderDrawLine(Program.render, 1290, 342, 1228, 527);
                NouveauSDLRenderDrawLine(Program.render, 1228, 527, 1212, 256);
                DessinerCercle((1256, 251), 67, 50);
                #endregion

                #region fenêtre
                if (Program.gTimer == 1320)
                    Program.explosions.Clear();

                for (int i = 0; i < Program.explosions.Count; i++)
                {
                    Program.explosions[i].RenderObjet();
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
                NouveauSDLRenderDrawLine(Program.render, 800, 100, 1050, 100);
                NouveauSDLRenderDrawLine(Program.render, 1050, 100, 1050, 400);
                NouveauSDLRenderDrawLine(Program.render, 1050, 400, 800, 400);
                NouveauSDLRenderDrawLine(Program.render, 800, 400, 800, 100);

                NouveauSDLRenderDrawLine(Program.render, 800, 250, 1050, 250);
                NouveauSDLRenderDrawLine(Program.render, 925, 100, 925, 400);

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
                    NouveauSDLRenderDrawPoint(Program.render, stars[i, 0], stars[i, 1]);
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
                NouveauSDLRenderDrawLine(Program.render, 20, 200, 700, 200);
                NouveauSDLRenderDrawLine(Program.render, 700, 200, 700, 250);
                NouveauSDLRenderDrawLine(Program.render, 700, 250, 20, 250);

                SDL_SetRenderDrawColor(Program.render, 255, 127, 0, 255);
                NouveauSDLRenderDrawLine(Program.render, 99, 219, 150, 219);
                NouveauSDLRenderDrawLine(Program.render, 150, 219, 127, 300);
                NouveauSDLRenderDrawLine(Program.render, 127, 300, 99, 219);

                SDL_SetRenderDrawColor(Program.render, 127, 255, 127, 255);
                NouveauSDLRenderDrawLine(Program.render, 252, 221, 300, 221);
                NouveauSDLRenderDrawLine(Program.render, 300, 221, 276, 301);
                NouveauSDLRenderDrawLine(Program.render, 276, 301, 252, 221);

                SDL_SetRenderDrawColor(Program.render, 127, 0, 127, 255);
                NouveauSDLRenderDrawLine(Program.render, 400, 223, 450, 223);
                NouveauSDLRenderDrawLine(Program.render, 450, 223, 427, 301);
                NouveauSDLRenderDrawLine(Program.render, 427, 301, 400, 223);

                SDL_SetRenderDrawColor(Program.render, 255, 255, 0, 255);
                NouveauSDLRenderDrawLine(Program.render, 127, 300, 101, 323);
                NouveauSDLRenderDrawLine(Program.render, 101, 323, 127, 348);
                NouveauSDLRenderDrawLine(Program.render, 127, 348, 154, 324);
                NouveauSDLRenderDrawLine(Program.render, 154, 324, 127, 300);

                NouveauSDLRenderDrawLine(Program.render, 276, 301, 250, 325);
                NouveauSDLRenderDrawLine(Program.render, 250, 325, 277, 349);
                NouveauSDLRenderDrawLine(Program.render, 277, 349, 304, 325);
                NouveauSDLRenderDrawLine(Program.render, 304, 325, 276, 301);

                NouveauSDLRenderDrawLine(Program.render, 427, 301, 401, 324);
                NouveauSDLRenderDrawLine(Program.render, 401, 324, 427, 349);
                NouveauSDLRenderDrawLine(Program.render, 427, 349, 454, 325);
                NouveauSDLRenderDrawLine(Program.render, 454, 325, 427, 301);
                #endregion

                #region trophés
                for (short i = 0; i < 301; i += 150)
                {
                    NouveauSDLRenderDrawLine(Program.render, 150 + i, 200, 166 + i, 171);
                    NouveauSDLRenderDrawLine(Program.render, 166 + i, 171, 221 + i, 172);
                    NouveauSDLRenderDrawLine(Program.render, 221 + i, 172, 238 + i, 200);
                    NouveauSDLRenderDrawLine(Program.render, 188 + i, 171, 188 + i, 154);
                    NouveauSDLRenderDrawLine(Program.render, 201 + i, 171, 201 + i, 154);
                    NouveauSDLRenderDrawLine(Program.render, 173 + i, 71, 161 + i, 124);
                    NouveauSDLRenderDrawLine(Program.render, 161 + i, 124, 176 + i, 154);
                    NouveauSDLRenderDrawLine(Program.render, 176 + i, 154, 215 + i, 154);
                    NouveauSDLRenderDrawLine(Program.render, 215 + i, 154, 227 + i, 128);
                    NouveauSDLRenderDrawLine(Program.render, 227 + i, 128, 218 + i, 71);
                    NouveauSDLRenderDrawLine(Program.render, 218 + i, 71, 173 + i, 71);
                    NouveauSDLRenderDrawLine(Program.render, 167 + i, 98, 151 + i, 97);
                    NouveauSDLRenderDrawLine(Program.render, 151 + i, 97, 144 + i, 119);
                    NouveauSDLRenderDrawLine(Program.render, 144 + i, 119, 166 + i, 133);
                    NouveauSDLRenderDrawLine(Program.render, 223 + i, 100, 237 + i, 97);
                    NouveauSDLRenderDrawLine(Program.render, 237 + i, 97, 245 + i, 122);
                    NouveauSDLRenderDrawLine(Program.render, 245 + i, 122, 224 + i, 136);
                }
                #endregion

                #region citation
                if (Program.gTimer > 1520)
                    Text.DisplayText("\"c'est un jeu bizarre, la guerre.", (20, 700), 3, scroll: (ushort)(Program.gTimer - 1520));
                if (Program.gTimer > 1580)
                    Text.DisplayText(" la seule facon de gagner est de ne pas jouer.\"", (20, 739), 3, scroll: (ushort)(Program.gTimer - 1580));
                if (Program.gTimer > 1670)
                    Text.DisplayText("- w.o.p.r.", (20, 778), 3, scroll: (ushort)(Program.gTimer - 1670));
                #endregion
            } // la guerre c mauvais - fini

            if (Program.gTimer > 30 && Program.gTimer < 1830)
            {
                SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
                NouveauSDLRenderDrawLine(Program.render, 20, 20, 1900, 20);
                NouveauSDLRenderDrawLine(Program.render, 1900, 20, 1900, 680);
                NouveauSDLRenderDrawLine(Program.render, 1900, 680, 20, 680);
                NouveauSDLRenderDrawLine(Program.render, 20, 20, 20, 680);
            }

            if (Program.gTimer > 1860)
            {
                Program.Gamemode = Gamemode.CUTSCENE_GENERIQUE;
                Program.player.Init();
                Program.enemies.Clear();
                Program.explosions.Clear();
            }
        }
        public static void Cut_4() // generique
        {
            if (Program.cutscene_skip_complet)
                Program.gTimer = 2601;

            if (Program.gTimer == 60)
            {
                Son.JouerMusique(ListeAudioMusique.EUGENESIS, false);
                Program.player.HP = 1;
                Program.player.afficher = true;
                //Program.gTimer = 1620;//
            }
            // beats: 60, 300, 540, 780, 1020, 1260, 1500, 1740, 1980, 2240

            #region texte
            if (Program.gTimer >= 60 && Program.gTimer < 500)
            {
                byte alpha = 255;
                if (Program.gTimer >= 60 && Program.gTimer < 145)
                    alpha = (byte)((Program.gTimer - 60) * 3);
                short extra_y = 0;
                if (Program.gTimer >= 300)
                    extra_y = (short)(Program.gTimer - 300);
                Text.DisplayText("dysgenesis", (Text.CENTRE, 540 - extra_y * 3), 4, alpha: alpha, scroll: ((Program.gTimer - 60) / 5));
            }
            if (Program.gTimer >= 300 && Program.gTimer < 700)
                Text.DisplayText("conception:\nmalcolm gauthier", (100, Program.W_HAUTEUR - (Program.gTimer - 300) * 3), 4, scroll: (Program.gTimer - 310) / 5);
            if (Program.gTimer >= 540 && Program.gTimer < 940)
                Text.DisplayText("         modèles:\nmalcolm gauthier", (Program.W_LARGEUR - 620, Program.W_HAUTEUR - (Program.gTimer - 540) * 3), 4, scroll: (Program.gTimer - 540) / 5);
            if (Program.gTimer >= 780 && Program.gTimer < 1180)
                Text.DisplayText("programmation:\nmalcolm gauthier", (100, Program.W_HAUTEUR - (Program.gTimer - 780) * 3), 4, scroll: (Program.gTimer - 780) / 5);
            if (Program.gTimer >= 1020 && Program.gTimer < 1420)
                Text.DisplayText(" effets sonnores:\nmalcolm gauthier", (Program.W_LARGEUR - 620, Program.W_HAUTEUR - (Program.gTimer - 1020) * 3), 4, scroll: (Program.gTimer - 1020) / 5);
            if (Program.gTimer >= 1260 && Program.gTimer < 1660)
                Text.DisplayText("musique", (Text.CENTRE, Program.W_HAUTEUR - (Program.gTimer - 1260) * 3), 4, scroll: (Program.gTimer - 1260) / 5);
            if (Program.gTimer >= 1300 && Program.gTimer < 1700)
            {
                Text.DisplayText("\"dance of the violins\"", (Text.CENTRE, Program.W_HAUTEUR - (Program.gTimer - 1300) * 3), 3, scroll: (Program.gTimer - 1300) / 5);
                Text.DisplayText("jesse valentine (f-777)", (Text.CENTRE, Program.W_HAUTEUR - (Program.gTimer - 1320) * 3), 3, scroll: (Program.gTimer - 1320) / 5);
            }
            if (Program.gTimer >= 1400 && Program.gTimer < 1800)
            {
                Text.DisplayText("\"240 bits per mile\"", (Text.CENTRE, Program.W_HAUTEUR - (Program.gTimer - 1400) * 3), 3, scroll: (Program.gTimer - 1400) / 5);
                Text.DisplayText("leon riskin", (Text.CENTRE, Program.W_HAUTEUR - (Program.gTimer - 1420) * 3), 3, scroll: (Program.gTimer - 1420) / 5);
            }
            if (Program.gTimer >= 1500 && Program.gTimer < 1900)
            {
                Text.DisplayText("\"dysgenesis\"         \"eugenesis\"", (Text.CENTRE, Program.W_HAUTEUR - (Program.gTimer - 1500) * 3), 3, scroll: (Program.gTimer - 1500) / 3);
                Text.DisplayText("malcolm gauthier", (Text.CENTRE, Program.W_HAUTEUR - (Program.gTimer - 1520) * 3), 3, scroll: (Program.gTimer - 1520) / 5);
            }
            if (Program.gTimer >= 1600 && Program.gTimer < 2000)
            {
                Text.DisplayText("autres musiques", (Text.CENTRE, Program.W_HAUTEUR - (Program.gTimer - 1600) * 3), 3, scroll: (Program.gTimer - 1600) / 5);
                Text.DisplayText("malcolm gauthier, mélodies non-originales", (Text.CENTRE, Program.W_HAUTEUR - (Program.gTimer - 1620) * 3), 3, scroll: (Program.gTimer - 1620) / 3);
            }
            if (Program.gTimer >= 1740 && Program.gTimer < 2140)
                Text.DisplayText("mélodies utilisées", (100, Program.W_HAUTEUR - (Program.gTimer - 1740) * 3), 4, scroll: (Program.gTimer - 1740) / 5);
            if (Program.gTimer >= 1780 && Program.gTimer < 2180)
            {
                Text.DisplayText("\"can't remove the pain\"", (400, Program.W_HAUTEUR - (Program.gTimer - 1780) * 3), 3, scroll: (Program.gTimer - 1780) / 5);
                Text.DisplayText("todd porter et herman miller", (400, Program.W_HAUTEUR - (Program.gTimer - 1800) * 3), 3, scroll: (Program.gTimer - 1800) / 5);
            }
            if (Program.gTimer >= 1880 && Program.gTimer < 2280)
            {
                Text.DisplayText("\"pesenka\"", (400, Program.W_HAUTEUR - (Program.gTimer - 1880) * 3), 3, scroll: (Program.gTimer - 1880) / 5);
                Text.DisplayText("Sergey Zhukov et Aleksey Potekhin", (400, Program.W_HAUTEUR - (Program.gTimer - 1900) * 3), 3, scroll: (Program.gTimer - 1900) / 5);
            }
            if (Program.gTimer >= 1980 && Program.gTimer < 2380)
            {
                Text.DisplayText("\"the beginning of time\"", (400, Program.W_HAUTEUR - (Program.gTimer - 1980) * 3), 3, scroll: (Program.gTimer - 1980) / 5);
                Text.DisplayText("nathan ingalls (dj-nate)", (400, Program.W_HAUTEUR - (Program.gTimer - 2000) * 3), 3, scroll: (Program.gTimer - 2000) / 5);
            }
            if (Program.gTimer >= 2250)
            {
                byte alpha = 0;
                if (Program.gTimer < 2350)
                    alpha = (byte)((Program.gTimer - 2250) * 2.5f);
                else
                    alpha = 255;
                Text.DisplayText("fin", (Text.CENTRE, Text.CENTRE), 5, alpha: alpha);
                if (Program.gTimer > 2350)
                    Text.DisplayText("tapez \"arcade\" au menu du jeu pour accéder au mode arcade!", (5, 5), 1, alpha: (short)((Program.gTimer - 2350) * 10));
            }
            #endregion

            #region ennemis & joueur
            if (Program.gTimer >= 400 && Program.gTimer < 500)
            {
                Program.player.taille = 10 * MathF.Pow(0.95f, Program.gTimer - 400);
                Program.player.position.x = (Program.gTimer - 400) * 4 + 1000;
                Program.player.position.y = MathF.Pow(Program.gTimer - 450, 2) * -0.1f + 600;
                Program.player.pitch = (Program.gTimer - 400) / 333f;
                if (Program.gTimer >= 460 && Program.gTimer < 480)
                    Program.player.roll = (Program.gTimer - 440) * (MathF.PI / 10) + 0.3f;
                else
                    Program.player.roll = (500 - Program.gTimer) * (MathF.PI / 300);
                Program.player.RenderObjet();
            }
            else if (Program.gTimer >= 600 && Program.gTimer < 900)
            {
                if (Program.gTimer == 600)
                {
                    new Croissant(false);
                    new Octahedron(false);
                    new Diamant(false);
                    for (byte i = 0; i < 3; i++)
                    {
                        Program.enemies[i].position.z = -5;
                        Program.enemies[i].pitch = 0.2f;
                    }
                }
                for (byte i = 0; i < 3; i++)
                {
                    Program.enemies[i].position.x = (600 - Program.gTimer) * 8.2f + 2000 + i * 200;
                    Program.enemies[i].position.y = MathF.Sin((Program.gTimer - 600) / -10f + i) * 50 + Program.W_SEMI_HAUTEUR;
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
                        ens[i] = new Patra(false);
                        ens[i].position.z = -5;
                    }
                    if (Program.gTimer > 1150)
                    {
                        if (i == 0)
                            ens[i].position.x = 300 - MathF.Pow((Program.gTimer - 1150), 1.9f);
                        else
                            ens[i].position.x = 1000 + MathF.Pow((Program.gTimer - 1150), 1.9f);
                        ens[i].position.y = 800 - MathF.Pow(Program.gTimer - 1150, 2);
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
                        ens[i] = new Energie(false);
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
                    ens[1] = new Tournant(false);
                    ens[1].position.z = 10;
                }

                ens[1].position.x = 1000;
                ens[1].position.y = (Program.gTimer - 1620) * -3 + 1150;

                if (Program.gTimer == 1829)
                {
                    Program.explosions.Clear();
                    new Explosion(new Vector3(ens[1].position.x, ens[1].position.y, 40));
                    Program.enemies.Clear();
                }
            }
            else if (Program.gTimer >= 1900 && Program.gTimer < 2000)
            {
                if (Program.gTimer == 1900)
                {
                    ens[0] = new Dupliqueur(false, null);
                    ens[0].position.z = -5;
                    ens[0].pitch = -0.6f;
                }
                ens[0].roll = 0.5f * MathF.Sin(Program.gTimer / 6f);
                ens[0].position.y = (Program.gTimer - 1900) * -3f + 1100;
                ens[0].position.x = (Program.gTimer - 1900) * -8 + 1950;
                if (Program.gTimer == 1999)
                {
                    ens[1] = new Dupliqueur(false, null);
                    ens[1].pitch = -0.6f;
                    ens[0].position.z = 5;
                    ens[1].position.z = 5;
                }
            }
            else if (Program.gTimer > 2240)
            {
                Program.player.taille = 1;
                Program.player.roll = MathF.Sin(Program.gTimer / 8f) / 5f;
                Program.player.pitch = 0.3f;
                Program.player.position.x = Program.W_SEMI_LARGEUR;
                Program.player.position.y = Program.W_SEMI_HAUTEUR + 100;
                byte alpha;
                if (Program.gTimer < 2350)
                    alpha = (byte)((Program.gTimer - 2240) * 2.33f);
                else
                    alpha = 255;
                Program.player.couleure.a = alpha;
                Program.player.RenderObjet();
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
                        ens[0] = new Boss();

                    ens[0].position.z = 40 - MathF.Pow((Program.gTimer - 1600) / 33f, 3);
                    ens[0].pitch = (Program.gTimer - 1600) / -200f;
                    ens[0].roll = (1600 - Program.gTimer) * (MathF.PI / 600);

                    if (Program.gTimer < 1650)
                        ens[0].position.x = 1500 + (Program.gTimer - 1600) * 10;
                    else
                        ens[0].position.x = -600 + (Program.gTimer - 1600) * 10;

                    ens[0].position.y = 200 * MathF.Sin((Program.gTimer - 1600) / 50f) + Program.W_SEMI_HAUTEUR;
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
                Program.player.RenderObjet();

                if (Program.gTimer == 1805)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        float[] line = Program.player.RenderDataLigne(Program.player.indexs_de_tir[i % 2]);
                        new Projectile(
                            new Vector3(line[0], line[1], Program.player.position.z),
                            new Vector3(
                                Program.player.position.x + 10 + i % 2 * 2 * -10,
                                Program.player.position.y - 350,
                                20
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
                        Program.enemies[i].roll = 0.5f * MathF.Sin(Program.gTimer / 6f);
                        Program.enemies[i].position.x = (Program.gTimer - 1900) * -8 + 1950;
                    }
                    Program.enemies[0].position.y = (Program.gTimer - 1900) * -3f + 1100 + MathF.Sqrt(Program.gTimer - 1999) * 18;
                    Program.enemies[1].position.y = (Program.gTimer - 1900) * -3f + 1100 - MathF.Sqrt(Program.gTimer - 1999) * 18;
                }
            }

            foreach (Ennemi e in Program.enemies)
            {
                e.RenderObjet();
                e.AnimationModele();
                e.timer++;
            }
            for (int i = 0; i < Program.explosions.Count; i++)
            {
                Program.explosions[i].RenderObjet();
                if (Program.explosions[i].Exist())
                    i--;
            }
            for (int i = 0; i < Program.projectiles.Count; i++)
            {
                Program.projectiles[i].RenderObjet();
                if (Program.projectiles[i].Exist())
                    i--;
            }

            #endregion

            #region gfade
            if (Program.gTimer > 2550)
                gFade = (byte)((Program.gTimer - 2550) * 5);
            if (gFade != 0)
            {
                rect.x = 0; rect.y = 0; rect.w = Program.W_LARGEUR; rect.h = Program.W_HAUTEUR;
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
                Program.bouger_etoiles = true;
                gFade = 0;
                BombePulsar.HP_bombe = BombePulsar.BOMBE_PULSAR_MAX_HP;
            }
        }

        // les scènes ont étés hardcodés pour parraître normal à 1920 x 1080,
        // alors ces fonctions me permet de facilement remplacer les vieilles
        //MISE À JOUR: les scènes qui devaient utiliser ces fonctions ne sont plus activées quand l'écran est trop petit,
        // alors le fait que ces fonctions sont utilisées au lieu des officielles ne fait plus rien, car elle vont seulement êtres
        // activées quand la fenêtre est à 1920x1080. Tu peux réactiver ces scènes pour des petits écrans en modifiant le code dans
        // Init() dans Program, mais tout qui n'est pas une ligne, un point ou un cercle n'apparaîtera pas correctement (texte, modèles, rectangles, etc.)
        const float OG_RES_HARDCODE_X = 1920.0f;
        const float OG_RES_HARDCODE_Y = 1080.0f;
        private static void NouveauSDLRenderDrawLine(IntPtr renderer, float x1, float y1, float x2, float y2)
        {
            float
                newX1 = x1 / OG_RES_HARDCODE_X * Program.W_LARGEUR,
                newX2 = x2 / OG_RES_HARDCODE_X * Program.W_LARGEUR,
                newY1 = y1 / OG_RES_HARDCODE_Y * Program.W_HAUTEUR,
                newY2 = y2 / OG_RES_HARDCODE_Y * Program.W_HAUTEUR
            ;

            SDL_RenderDrawLineF(renderer, newX1, newY1, newX2, newY2);
        }
        private static void NouveauSDLRenderDrawPoint(IntPtr renderer, int x1, int y1)
        {
            float
                newX1 = x1 / OG_RES_HARDCODE_X * Program.W_LARGEUR,
                newY1 = y1 / OG_RES_HARDCODE_Y * Program.W_HAUTEUR
            ;

            SDL_RenderDrawPointF(renderer, newX1, newY1);
        }
        private static void DessinerCercle(Vector2 position, int taille, byte cotes)
        {
            float ang;
            float next_ang;

            for (int i = 0; i < cotes; i++)
            {
                // Tau = 2*Pi
                ang = (i * MathF.Tau) / cotes;
                next_ang = ((i + 1) * MathF.Tau) / cotes;

                NouveauSDLRenderDrawLine(Program.render,
                    position.x + taille * MathF.Sin(ang),
                    position.y + taille * MathF.Cos(ang),
                    position.x + taille * MathF.Sin(next_ang),
                    position.y + taille * MathF.Cos(next_ang)
                );
            }
        }
    }

    // classe qui s'occupe du curseur sur le menu principal
    // TODO: rendre plus général
    public class Curseur : Sprite
    {
        public enum OptionsCurseur
        {
            NOUVELLE_PARTIE,
            CONTINUER,
            ARCADE,
            AUCUN,
        }

        const int CURSEUR_DAS = Program.G_FPS / 4; // Delayed Auto Shift
        readonly int CURSEUR_X_INIT;
        readonly int CURSEUR_Y_INIT;
        const int CURSEUR_ESPACE = 50;
        readonly int NB_OPTIONS = Enum.GetNames(typeof(OptionsCurseur)).Length;
        readonly Vector3[] curseur_data =
        {
            new(-15, -15, 0),
            new(15, 0, 0),
            new(-15, 15, 0),
            new(-12, 0, 0),
            new(-15, -15, 0)
        }; // modèle du curseur

        public int curseur_max_selection = 0;
        public OptionsCurseur selection = OptionsCurseur.AUCUN;
        OptionsCurseur curseur_option_selectionnee = 0;

        public Curseur()
        {
            modele = curseur_data;
            CURSEUR_X_INIT = Program.W_SEMI_LARGEUR - 150;
            CURSEUR_Y_INIT = Program.W_SEMI_HAUTEUR + 85;
        }

        // code curseur, retourne vrai si option sélectionnée
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
                if (Program.TouchePesee(Touches.S) && (int)curseur_option_selectionnee < curseur_max_selection - 1)
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
            selection = OptionsCurseur.AUCUN;
            return false;
        }

        public override void RenderObjet()
        {
            SDL_SetRenderDrawColor(Program.render, 255, 255, 255, 255);
            for (int i = 0; i < curseur_data.Length - 1; i++)
            {
                SDL_RenderDrawLineF(Program.render,
                    CURSEUR_X_INIT + curseur_data[i].x,
                    CURSEUR_Y_INIT + curseur_data[i].y + CURSEUR_ESPACE * (int)curseur_option_selectionnee,
                    CURSEUR_X_INIT + curseur_data[i + 1].x,
                    CURSEUR_Y_INIT + curseur_data[i + 1].y + CURSEUR_ESPACE * (int)curseur_option_selectionnee
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
            timer = (int)Math.Clamp(Program.G_MAX_DEPTH - this.position.z, 0f, Program.G_MAX_DEPTH - 1);

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
        public override void RenderObjet()
        {
            const int RAYON_MIN_EXPLOSION = 2;

            if (Program.player.Mort())
                return;

            // division par zéro évitée dans le constructeur
            byte rayon = (byte)(Program.G_MAX_DEPTH * 8f / (Program.G_MAX_DEPTH - timer) + RAYON_MIN_EXPLOSION);

            for (int i = 0; i < DENSITE_EXPLOSION; i++)
            {
                float angle = Program.RNG.NextSingle() * MathF.PI;
                float sin_ang = MathF.Sin(angle);
                float cos_ang = MathF.Cos(angle);

                // couleure rouge-orange-jaune au hasard
                SDL_SetRenderDrawColor(Program.render,
                    (byte)Program.RNG.Next(128, 256),
                    (byte)Program.RNG.Next(0, 128),
                    0,
                    255
                );

                // dessine des lignes au hasard dans un cercle
                SDL_RenderDrawLineF(Program.render,
                    Program.RNG.Next(-rayon, rayon) * cos_ang + position.x,
                    Program.RNG.Next(-rayon, rayon) * sin_ang + position.y,
                    Program.RNG.Next(-rayon, rayon) * cos_ang + position.x,
                    Program.RNG.Next(-rayon, rayon) * sin_ang + position.y
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
            x = Program.W_LARGEUR - 360,
            y = 10,
            w = 350,
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
            { ListeAudioMusique.DOTV, @"audio\dotv.wav" },
            { ListeAudioMusique.EUGENESIS, @"audio\eugenesis.wav" },
            { ListeAudioMusique.DCQBPM, @"audio\240 Bits Per Mile.wav" }
        };

        static IntPtr musique = IntPtr.Zero;
        static IntPtr[] effets_sonnores = new IntPtr[NB_CHAINES_SFX];

        static int timer = 0;
        static int prochain_chunk = 0;
        static byte volume = 2;
        private static int volume_SDL;
        static bool render = false;

        // initialize SDL mixer
        public static int InitSDLMixer()
        {
            // 44100 hz, format défaut, mono, chunk = 2kb
            if (Mix_OpenAudio(MIX_DEFAULT_FREQUENCY, MIX_DEFAULT_FORMAT, 1, 2048) != 0)
                return -1;

            // +1 répare un bug où que la dernière chaine alloquée ne joue pas de son.
            // je ne sais pas pourquoi, mais sans le +1, chaque 20e effet sonnore ne jouera pas
            Mix_AllocateChannels(NB_CHAINES_SFX + 1);
            volume_SDL = volume * 8;

            return 0;
        }

        // arrête la musique
        public static void ArreterMusique()
        {
            Mix_PauseMusic();
        }

        // retourne vrai si la musique joue, faux sinon
        public static bool MusiqueJoue()
        {
            return Mix_PlayingMusic() == (int)SDL_bool.SDL_TRUE;
        }

        // Change la musique pour une nouvelle dans la liste de musiques. Il ne peux seulement avoir
        // une musique qui joue à la fois.
        public static int JouerMusique(ListeAudioMusique musique_a_jouer, bool boucle)
        {
            if (Program.mute_son)
                return 0;

            // -1 boucles dit à SDL de jouer la musique infiniment
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
            if (Program.mute_son)
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
            {
                //Program.CrashReport(new Exception("effet sonnore non joué"));//DEBUG
                return -3;
            }

            Mix_Volume(ALL_CHUNKS, volume_SDL / 2);

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
            const int VOLUME_TEMPS_AFFICHAGE = Program.G_FPS / 2; // nb d'images que la boite reste après que les touches soient lâchés
            const int VOLUME_DAS = Program.G_FPS / 12; // nb d'images entre les incréments/décrements du volume

            if (Program.TouchePesee(Touches.PLUS) || Program.TouchePesee(Touches.MINUS))
            {
                if (timer < VOLUME_TEMPS_AFFICHAGE - VOLUME_DAS)
                {
                    timer = VOLUME_TEMPS_AFFICHAGE;

                    if (Program.TouchePesee(Touches.PLUS) && volume < MAX_VOLUME_GUI)
                        volume++;
                    if (Program.TouchePesee(Touches.MINUS) && volume > 0)
                        volume--;

                    // le jeu utilise 0 à MAX_VOLUME_GUI, SDL utilise 0 à 128
                    volume_SDL = (int)(MIX_MAX_VOLUME * (volume / (float)MAX_VOLUME_GUI));
                    Mix_VolumeMusic(volume_SDL);
                    Mix_Volume(ALL_CHUNKS, volume_SDL / 2);
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
