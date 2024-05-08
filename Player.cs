using SDL2;
using System.Numerics;
using static SDL2.SDL;

namespace Dysgenesis
{
    public class Player : Sprite
    {
        public const float JOUEUR_MAX_VAGUES = 3.0f;
        public const int JOUEUR_VITESSE_TIR = 25;
        public const int JOUEUR_MAX_HP = 150;
        const float VAGUE_ELECTRIQUE_REGENERATION = 1.0f / (30.0f * 60.0f); // ~1/1800, 1 par 30 secondes @ 60 fps
        const float JOUEUR_FRICTION = 0.9f;
        const float JOUEUR_MAX_PITCH = 0.5f;
        const float JOUEUR_MAX_ROLL = 0.1f;
        const float JOUEUR_PITCH_FRICTION = 0.95f;
        const float JOUEUR_ROLL_FRICTION = 0.95f;
        const float JOUEUR_PITCH_CONSTANT = 0.3f;
        const float JOUEUR_MORT_VITESSE_X = -0.2f;
        const float JOUEUR_MORT_VITESSE_Y = 0.2f;
        const float JOUEUR_MORT_VITESSE_ROLL = 1 / 300.0f;
        const float JOUEUR_MORT_FACTEUR_GRANDEUR = 1 / 60.0f;
        const int DECAL_HORIZONTAL_TIR = 10;
        const int DECAL_VERTICAL_TIR = -200;
        const int JOUEUR_VITESSE = 1;
        const int JOUEUR_LARGEUR = 50;
        const int JOUEUR_HAUTEUR = 20;
        readonly short[] DECALS_SPREAD =
        {
            -15, -240,
            15, -240,
            10, -190,
            -10, -190,
        };

        public Vector2 velocity;
        public TypeItem powerup = TypeItem.NONE;

        public float shockwaves = 0;
        public short HP = 100;
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
            Move();
            Proj_Collision();
            if (BombePulsar.HP_bombe > 0 && !Mort())
                TirJoueur();
            if (!Mort())
                shockwaves += VAGUE_ELECTRIQUE_REGENERATION;

            if (Program.TouchePesee(Touches.C))
                HP = 1;

            return false;
        }
        public void Init()
        {
            HP = 100;
            powerup = TypeItem.NONE;
            shockwaves = 3;
            velocity = new Vector2(0, -30);
            afficher = true;
            pitch = 0;
            roll = 0;
            couleure = new SDL_Color() { r = 255, g = 255, b = 255, a = 255 };
            taille = 1;
            position = new Vector3(Data.W_SEMI_LARGEUR, Data.W_HAUTEUR, 0);
        }
        int TirJoueur()
        {
            fire_timer++;

            if (fire_timer < fire_rate)
                return 0;

            if (!Program.TouchePesee(Touches.J))
                return 0;

            fire_timer = 0;

            List<Projectile> indexs = new List<Projectile>();
            byte iterations = 2;
            if (powerup == TypeItem.SPREAD)
                iterations = 6;

            for (int i = 0; i < iterations; i++)
            {
                float[] line = RenderLineData(indexs_de_tir[i % 2]);
                indexs.Add(new Projectile(
                    new Vector3(line[0], line[1], position.z),
                    new Vector3(
                        position.x + DECAL_HORIZONTAL_TIR + i % 2 * 2 * -DECAL_HORIZONTAL_TIR,
                        position.y + DECAL_VERTICAL_TIR,
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
            else if (powerup == TypeItem.SPREAD)
            {
                for (int i = 2; i < 6; i++)
                {
                    indexs[i].destination.x = indexs[i].position.x + DECALS_SPREAD[(i - 2) * 2];
                    indexs[i].destination.y = indexs[i].position.y + DECALS_SPREAD[(i - 2) * 2 + 1];
                }
            }

            return 1;
        }
        void Proj_Collision()
        {
            if (Mort())
                return;

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
                if (Program.curseur.curseur_max_selection < 2)
                    Program.curseur.curseur_max_selection = 2;
                if (Program.Gamemode == Gamemode.GAMEPLAY)
                    Program.nv_continue = Program.niveau;

                return;
            }
        }
        void Move()
        {
            timer++;

            if (Mort())
            {
                if (timer == 1)
                {
                    Son.JouerEffet(ListeAudioEffets.EXPLOSION_JOUEUR);
                    indexs_lignes_sauter = new int[0];
                    for (int i = 0; i < modele.Length; i++)
                    {
                        if (i % 3 != 0)
                            indexs_lignes_sauter = indexs_lignes_sauter.Append(i).ToArray();
                    }
                }

                if (timer <= byte.MaxValue)
                {
                    couleure.a = (byte)(255 - timer);
                    position.x += JOUEUR_MORT_VITESSE_X;
                    position.y += JOUEUR_MORT_VITESSE_Y;
                    roll += JOUEUR_MORT_VITESSE_ROLL;
                    taille = 1 + timer * JOUEUR_MORT_FACTEUR_GRANDEUR;
                }

                if (timer > 120)
                {
                    Text.DisplayText("game over",
                        new Vector2(Text.CENTRE, Text.CENTRE), 5, Text.BLANC, (short)(timer - 120), Text.NO_SCROLL);

                    if (Program.Gamemode == Gamemode.ARCADE)
                        Text.DisplayText("score: " + Program.niveau,
                            new Vector2(Text.CENTRE, Data.W_SEMI_HAUTEUR + 30), 2, Text.BLANC, (short)(timer - 120), Text.NO_SCROLL);
                }

                if (timer > 800)
                {
                    afficher = false;
                    indexs_lignes_sauter = Data.MODELE_P_SAUTS;
                    Son.JouerMusique(ListeAudioMusique.DYSGENESIS, true);
                    Program.bouger_etoiles = true;
                    Program.enemies.Clear();
                    Program.Gamemode = Gamemode.TITLESCREEN;
                }

                timer++;

                return;
            }

            if (Program.TouchePesee(Touches.W))
            {
                velocity.y -= JOUEUR_VITESSE;
                pitch += 0.05f;
            }
            if (Program.TouchePesee(Touches.A))
            {
                velocity.x -= JOUEUR_VITESSE;
                roll -= 0.05f;
            }
            if (Program.TouchePesee(Touches.S))
            {
                velocity.y += JOUEUR_VITESSE;
                pitch -= 0.05f;
            }
            if (Program.TouchePesee(Touches.D))
            {
                velocity.x += JOUEUR_VITESSE;
                roll += 0.05f;
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

        public bool Mort()
        {
            return HP <= 0;
        }
    }
}