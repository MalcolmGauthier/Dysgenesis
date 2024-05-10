﻿using System.Collections.ObjectModel;
using System.Diagnostics;
using static SDL2.SDL;

namespace Dysgenesis
{
    public class Player : Sprite
    {
        public const float JOUEUR_MAX_VAGUES = 3.0f;
        public const int JOUEUR_VITESSE_TIR = 25;
        public const int JOUEUR_MAX_HP = 150;
        const float VAGUE_ELECTRIQUE_REGENERATION = 1.0f / (30f * Data.G_FPS); // ~1/1800, 1 par 30 secondes
        const int JOUEUR_VITESSE = 1;
        const int JOUEUR_LARGEUR = 50;
        const int JOUEUR_HAUTEUR = 20;

        public Vector2 velocity;
        public TypeItem powerup = TypeItem.NONE;

        public float shockwaves = 0;
        public int HP = 100;
        public int fire_rate = JOUEUR_VITESSE_TIR;
        int fire_timer = 0;

        public SDL_Rect HP_BAR = new SDL_Rect { x = 10, y = 15, w = 10, h = 20 };
        public SDL_Rect SHOCK_BAR = new SDL_Rect { x = 10, y = 40, w = 100, h = 20 };

        public Player()
        {
            indexs_de_tir = new int[] { 1, 16 };
            modele = Data.MODELE_P;
            indexs_lignes_sauter = Data.MODELE_P_SAUTS;
            Init();
        }
        public override bool Exist()
        {
            if (Program.TouchePesee(Touches.R))//DEBUG
            {
                HP = 0;
                timer = 0;
            }

            if (Mort())
            {
                AnimationMort();
                return true;
            }

            Move();

            if (Proj_Collision() != 0)
                return true;

            if (Program.bombe.HP_bombe > 0)
                TirJoueur();

            shockwaves += VAGUE_ELECTRIQUE_REGENERATION;

            return false;
        }

        // retourne le joueur à son état de départ
        public void Init()
        {
            HP = 100;
            powerup = TypeItem.NONE;
            shockwaves = 3;
            // ces deux lignes sont fait pourque le joueur commence le jeu en volant très vite du bas de l'écran, comme si il arrivait
            position = new Vector3(Data.W_SEMI_LARGEUR, Data.W_HAUTEUR, 0);
            velocity = new Vector2(0, -30);
            afficher = true;
            pitch = 0;
            roll = 0;
            couleure = new SDL_Color() { r = 255, g = 255, b = 255, a = 255 };
            taille = 1;
        }

        // vérifie si le joueur veut tirer et peut tirer, et tire au besoin.
        // retourne 1 si tir créé, 0 sinon. 
        int TirJoueur()
        {
            const Touches TOUCHE_POUR_TIRER = Touches.J;
            // liste de vecteurs pour décaler les cibles des projectiles du joueur
            ReadOnlyCollection<Vector2> DECALS_SPREAD = new(new Vector2[]
            {
                new(10, -200),
                new(-10, -200),
                new(-15, -240),
                new(15, -240),
                new(10, -190),
                new(-10, -190)
            });

            fire_timer++;

            if (fire_timer < fire_rate)
                return 0;

            if (!Program.TouchePesee(TOUCHE_POUR_TIRER))
                return 0;

            fire_timer = 0;

            List<Projectile> indexs = new List<Projectile>();

            // nb de projectiles à tirer
            byte iterations = 2;
            if (powerup == TypeItem.SPREAD)
                iterations = 6;

            Debug.Assert(iterations <= DECALS_SPREAD.Count);

            for (int i = 0; i < iterations; i++)
            {
                // la moitié des tirs se font du point de tir 1, l'autre moitié du point de tir 2
                float[] line = RenderLineData(indexs_de_tir[i % indexs_de_tir.Length]);
                indexs.Add(new Projectile(
                    new Vector3(line[0], line[1], position.z),
                    // destination du projectile
                    new Vector3(
                        position.x + DECALS_SPREAD[i].x,
                        position.y + DECALS_SPREAD[i].y,
                        Data.G_MAX_DEPTH
                    ),
                    ProprietaireProjectile.JOUEUR,
                    (byte)(i % 2)
                ));
            }

            if (powerup == TypeItem.HOMING)
            {
                indexs[0].FindTarget();
                indexs[1].FindTarget();
            }

            return 1;
        }

        // retourne >0 si mort, 0 si vivant
        int Proj_Collision()
        {
            if (Mort())
                return 2;

            // pas besoin de boucle for car on n'a pas besoin de détruire les projectiles lors d'une collisions.
            // les projectiles qui font collision vont se faire détruire en frappant z=0 le prochain frame tout de même.
            foreach (Projectile p in Program.projectiles)
            {
                if (p.proprietaire == ProprietaireProjectile.JOUEUR)
                    continue;

                if (p.position.z >= 1)
                    continue;

                float[] depths = p.PositionsSurEcran();
                if (Background.Distance(depths[0], depths[1], position.x, position.y) > 30)
                    continue;

                HP--;
                new Explosion(p.destination);

                if (!Mort())
                    continue;

                // code si joueur est mort d'un projectile ennemi
                timer = 0;
                Son.StopMusic();

                // code pour option continuer au menu
                if (Program.curseur.curseur_max_selection < 2)
                    Program.curseur.curseur_max_selection = 2;
                if (Program.Gamemode == Gamemode.GAMEPLAY)
                    Program.nv_continue = Program.niveau;

                return 1;
            }

            return 0;
        }

        // animation quand joueur est mort. utilise timer.
        void AnimationMort()
        {
            const float JOUEUR_MORT_VITESSE_X = -0.6f;
            const float JOUEUR_MORT_VITESSE_Y = 0.6f;
            const float JOUEUR_MORT_VITESSE_ROLL = 1.0f / (1 * Data.G_FPS);
            const float JOUEUR_MORT_FACTEUR_GRANDEUR = 1.0f / Data.G_FPS;
            const int QUANTITE_LIGNES_MODELE_MORT = 3;

            const int IMAGES_AVANT_TEXTE = 2 * Data.G_FPS;
            const int IMAGES_AVANT_RETOUR_MENU = 15 * Data.G_FPS;

            if (timer == 1)
            {
                Son.JouerEffet(ListeAudioEffets.EXPLOSION_JOUEUR);
                Program.bouger_etoiles = false;

                // pourque le joueur aie l'air plus "détruit", seulement un tiers des lignes sont visibles
                List<int> temp = new();
                for (int i = 1; i < modele.Length; i++)
                {
                    if (i % QUANTITE_LIGNES_MODELE_MORT != 0)
                        temp.Add(i);
                }
                temp.Add(-1);
                indexs_lignes_sauter = temp.ToArray();
            }

            // inutile de bouger le joueuru si il est invisible
            if (timer <= byte.MaxValue)
            {
                couleure.a = (byte)(255 - timer);
                position.x += JOUEUR_MORT_VITESSE_X;
                position.y += JOUEUR_MORT_VITESSE_Y;
                roll += JOUEUR_MORT_VITESSE_ROLL;
                taille = 1 + timer * JOUEUR_MORT_FACTEUR_GRANDEUR;
            }

            if (timer > IMAGES_AVANT_TEXTE)
            {
                Text.DisplayText("game over",
                    new Vector2(Text.CENTRE, Text.CENTRE), 5, Text.BLANC, timer - 120, Text.NO_SCROLL);

                if (Program.Gamemode == Gamemode.ARCADE)
                {
                    Text.DisplayText("score: " + Program.niveau,
                        new Vector2(Text.CENTRE, Data.W_SEMI_HAUTEUR + 30), 2, Text.BLANC, timer - 120, Text.NO_SCROLL);
                }
            }

            if (timer > IMAGES_AVANT_RETOUR_MENU)
            {
                afficher = false;
                timer = 0;
                // défaire le truc qu'on a fait dans le if(timer==1)
                indexs_lignes_sauter = Data.MODELE_P_SAUTS;
                Son.JouerMusique(ListeAudioMusique.DYSGENESIS, true);
                Program.bouger_etoiles = true;
                Program.enemies.Clear();
                Program.Gamemode = Gamemode.TITLESCREEN;
                return;
            }

            timer++;
        }

        // bouge le joueur
        void Move()
        {
            const float JOUEUR_FRICTION = 0.9f;
            //const float JOUEUR_MAX_PITCH = 0.5f;
            //const float JOUEUR_MAX_ROLL = 0.1f;
            const float JOUEUR_PITCH_ACCELERATION = 0.05f;
            const float JOUEUR_ROLL_ACCELERATION = 0.05f;
            const float JOUEUR_PITCH_FRICTION = 0.95f;
            const float JOUEUR_ROLL_FRICTION = 0.95f;
            const float JOUEUR_PITCH_CONSTANT = 0.3f;

            timer++;

            if (Program.TouchePesee(Touches.W))
            {
                velocity.y -= JOUEUR_VITESSE;
                pitch += JOUEUR_PITCH_ACCELERATION;
            }
            if (Program.TouchePesee(Touches.A))
            {
                velocity.x -= JOUEUR_VITESSE;
                roll -= JOUEUR_ROLL_ACCELERATION;
            }
            if (Program.TouchePesee(Touches.S))
            {
                velocity.y += JOUEUR_VITESSE;
                pitch -= JOUEUR_PITCH_ACCELERATION;
            }
            if (Program.TouchePesee(Touches.D))
            {
                velocity.x += JOUEUR_VITESSE;
                roll += JOUEUR_ROLL_ACCELERATION;
            }

            velocity.x *= JOUEUR_FRICTION;
            velocity.y *= JOUEUR_FRICTION;
            pitch = (pitch - JOUEUR_PITCH_CONSTANT) * JOUEUR_PITCH_FRICTION + JOUEUR_PITCH_CONSTANT;
            roll *= JOUEUR_ROLL_FRICTION;

            position.x += velocity.x;
            position.y += velocity.y;

            if (position.x - JOUEUR_LARGEUR < 0)
                position.x = JOUEUR_LARGEUR;

            if (position.x + JOUEUR_LARGEUR > Data.W_LARGEUR)
                position.x = Data.W_LARGEUR - JOUEUR_LARGEUR;

            if (position.y - JOUEUR_HAUTEUR < 0)
                position.y = JOUEUR_HAUTEUR;

            if (position.y + JOUEUR_HAUTEUR > Data.W_HAUTEUR)
                position.y = Data.W_HAUTEUR - JOUEUR_HAUTEUR;
        }

        // fonction mystère
        public bool Mort()
        {
            return HP <= 0;
        }
    }
}