using static SDL2.SDL;

namespace Dysgenesis
{
    internal class Octahedron : Ennemi
    {
        static readonly Vector3[] MODELE_E1 =
        {
            new( 0, 0, -20 ),
            new( -16, 0, 10 ),
            new( 16, 0, 10 ),
            new( 0, 0, -20 ),
            new( 0, -30, 0 ),
            new( -16, 0, 10 ),
            new( 0, -30, 0 ),
            new( 16, 0, 10 ),
            new( 0, 30, 0 ),
            new( 0, 0, -20 ),
            new( -16, 0, 10 ),
            new( 0, 30, 0 ),
            new( -5, 5, 0 ),
            new( 0, 2, 0 ),
            new( 5, 5, 0 ),
            new( 0, 8, 0 ),
            new( -5, 5, 0 ),
            new( 0, 5, 0 ),
            new( 0, 5, 0 )
        };
        static readonly int[] MODELE_E1_SAUTS = { 12, 17, -1 };
        static readonly int[] MODELE_E1_POINTS_TIR = { 18 };

        public Octahedron(bool dur)
            : base(MODELE_E1, MODELE_E1_SAUTS, Program.RGBtoSDLColor(0xFFFF00), 1,
            VITESSE_MOYENNE_ENNEMI / 8, VITESSE_MOYENNE_Z_ENNEMI, 30, 0, MODELE_E1_POINTS_TIR)
        {
            this.version_dur = dur;

            if (version_dur)
            {
                couleure = Program.RGBtoSDLColor(0x808000);
                vitesse = VITESSE_MOYENNE_ENNEMI * 0.8f;
                vitesse_tir = Program.G_FPS / VITESSE_MOYENNE_TIR_ENNEMI;
                HP = 3;
            }
        }

        public override void AnimationModele()
        {
            float speed = version_dur ? 50f : 20f;

            float angle = timer / speed;

            for (int i = 0; i < modele_en_marche.Length; i++)
            {
                if (i == 0 || i == 3 || i == 9)
                {
                    modele_en_marche[i].x = MathF.Cos(angle) * 20;
                    modele_en_marche[i].z = MathF.Sin(angle) * 20;
                }
                if (i == 1 || i == 5 || i == 10)
                {
                    modele_en_marche[i].x = MathF.Cos(angle + 2) * 20;
                    modele_en_marche[i].z = MathF.Sin(angle + 2) * 20;
                }
                if (i == 2 || i == 7)
                {
                    modele_en_marche[i].x = MathF.Cos(angle + 4) * 20;
                    modele_en_marche[i].z = MathF.Sin(angle + 4) * 20;
                }
            }
        }
    }

    internal class Diamant : Ennemi
    {
        static readonly Vector3[] MODELE_E2 =
        {
            new( -30, 0, 0 ),
            new( -15, 0, 26 ),
            new( 0, 50, 0 ),
            new( -30, 0, 0 ),
            new( -15, 0, -26 ),
            new( 0, 50, 0 ),
            new( 15, 0, -26 ),
            new( -15, 0, -26 ),
            new( 15, 0, -26 ),
            new( 30, 0, 0 ),
            new( 0, 50, 0 ),
            new( 15, 0, 26 ),
            new( 30, 0, 0 ),
            new( 15, 0, 26 ),
            new( -15, 0, 26 ),
            new( -12, -10, 21 ),
            new( -25, -10, 0 ),
            new( -30, 0, 0 ),
            new( -25, -10, 0 ),
            new( -12, -10, -21 ),
            new( -15, 0, -26 ),
            new( -12, -10, -21 ),
            new( 12, -10, -21 ),
            new( 15, 0, -26 ),
            new( 12, -10, -21 ),
            new( 25, -10, 0 ),
            new( 30, 0, 0 ),
            new( 25, -10, 0 ),
            new( 12, -10, 21 ),
            new( 15, 0, 26 ),
            new( 12, -10, 21 ),
            new( -12, -10, 21 ),
            new( -12, -10, 3 ),
            new( -12, -10, -3 ),
            new( -17, -10, 0 ),
            new( -20, -18, 0 ),
            new( -15, -18, 3 ),
            new( -12, -10, 3 ),
            new( -15, -18, 3 ),
            new( -15, -18, -3 ),
            new( -12, -10, -3 ),
            new( -15, -18, -3 ),
            new( -20, -18, 0 ),
            new( -5, -23, 0 ),
            new( -15, -18, -3 ),
            new( -5, -23, 0 ),
            new( -15, -18, 3 ),
            new( -12, -10, 3 ),
            new( -12, -10, 21 ),
            new( 12, -10, 21 ),
            new( 12, -10, 3 ),
            new( 12, -10, -3 ),
            new( 17, -10, 0 ),
            new( 20, -18, 0 ),
            new( 15, -18, 3 ),
            new( 12, -10, 3 ),
            new( 15, -18, 3 ),
            new( 15, -18, -3 ),
            new( 12, -10, -3 ),
            new( 15, -18, -3 ),
            new( 20, -18, 0 ),
            new( 5, -23, 0 ),
            new( 15, -18, -3 ),
            new( 5, -23, 0 ),
            new( 15, -18, 3 ),
            new( 12, -10, 3 ),
            new( 12, -10, 21 )
        };
        static readonly int[] MODELE_E2_SAUTS = { 48, 50, -1 };
        static readonly int[] MODELE_E2_POINTS_TIR = { 61, 45 };

        public Diamant(bool dur)
            : base(MODELE_E2, MODELE_E2_SAUTS, Program.RGBtoSDLColor(0xFF7F00), 3,
            VITESSE_MOYENNE_ENNEMI / 2, VITESSE_MOYENNE_Z_ENNEMI, 30, VITESSE_MOYENNE_TIR_ENNEMI * 2, MODELE_E2_POINTS_TIR)
        {
            this.version_dur = dur;

            if (dur)
            {
                vitesse = VITESSE_MOYENNE_ENNEMI;
                HP = 5;
                couleure = Program.RGBtoSDLColor(0x804000);
            }
        }
    }

    internal class Tournant : Ennemi
    {
        static readonly Vector3[] MODELE_E3 =
        {
            new( -50, 0, 0 ),
            new( -50, -10, 40 ),
            new( -45, 0, 0 ),
            new( -50, 0, 0 ),
            new( -20, -10, -10 ),
            new( -20, 10, -10 ),
            new( 20, 10, -10 ),
            new( 20, 10, 10 ),
            new( 50, 0, 0 ),
            new( 50, -10, 40 ),
            new( 45, 0, 0 ),
            new( 50, 0, 0 ),
            new( 20, 10, -10 ),
            new( 20, -10, -10 ),
            new( -20, -10, -10 ),
            new( -20, -10, 10 ),
            new( 20, -10, 10 ),
            new( 20, -10, -10 ),
            new( 50, 0, 0 ),
            new( 20, -10, 10 ),
            new( 20, 10, 10 ),
            new( -20, 10, 10 ),
            new( -50, 0, 0 ),
            new( -20, -10, 10 ),
            new( -20, 10, 10 ),
            new( -20, 10, -10 ),
            new( -50, 0, 0 )
        };
        static readonly int[] MODELE_E3_SAUTS = { -1 };
        static readonly int[] MODELE_E3_POINTS_TIR = { 1, 9 };

        public Tournant(bool dur)
            : base(MODELE_E3, MODELE_E3_SAUTS, Program.RGBtoSDLColor(0x00FF00), 3,
            VITESSE_MOYENNE_ENNEMI / 2, VITESSE_MOYENNE_Z_ENNEMI, 50, VITESSE_MOYENNE_TIR_ENNEMI * 2, MODELE_E3_POINTS_TIR)
        {
            this.version_dur = dur;

            if (dur)
            {
                vitesse = VITESSE_MOYENNE_ENNEMI;
                HP = 7;
                couleure = Program.RGBtoSDLColor(0x008000);
            }
        }

        public override void AnimationModele()
        {
            float speed = version_dur ? 50f : 20f;

            float angle = timer / speed;
            
            for (int i = 0; i < modele_en_marche.GetLength(0); i++)
            {
                switch (i)
                {
                    case 16:
                    case 19:
                        modele_en_marche[i].y = MathF.Cos(angle) * 10;
                        modele_en_marche[i].z = MathF.Sin(angle) * 10;
                        break;
                    case 13:
                    case 17:
                        modele_en_marche[i].y = MathF.Cos(angle + 1.5f) * 10;
                        modele_en_marche[i].z = MathF.Sin(angle + 1.5f) * 10;
                        break;
                    case 6:
                    case 12:
                        modele_en_marche[i].y = MathF.Cos(angle + 3) * 10;
                        modele_en_marche[i].z = MathF.Sin(angle + 3) * 10;
                        break;
                    case 7:
                    case 20:
                        modele_en_marche[i].y = MathF.Cos(angle + 4.5f) * 10;
                        modele_en_marche[i].z = MathF.Sin(angle + 4.5f) * 10;
                        break;
                    case 15:
                    case 23:
                        modele_en_marche[i].y = MathF.Cos(-angle) * 10;
                        modele_en_marche[i].z = MathF.Sin(-angle) * 10;
                        break;
                    case 4:
                    case 14:
                        modele_en_marche[i].y = MathF.Cos(-angle + 1.5f) * 10;
                        modele_en_marche[i].z = MathF.Sin(-angle + 1.5f) * 10;
                        break;
                    case 5:
                    case 25:
                        modele_en_marche[i].y = MathF.Cos(-angle + 3) * 10;
                        modele_en_marche[i].z = MathF.Sin(-angle + 3) * 10;
                        break;
                    case 21:
                    case 24:
                        modele_en_marche[i].y = MathF.Cos(-angle + 4.5f) * 10;
                        modele_en_marche[i].z = MathF.Sin(-angle + 4.5f) * 10;
                        break;
                }
            }
        }
    }

    internal class Energie : Ennemi
    {
        static readonly Vector3[] MODELE_E4 = Array.Empty<Vector3>();
        static readonly int[] MODELE_E4_SAUTS = Array.Empty<int>();
        static readonly int[] MODELE_E4_POINTS_TIR = { -1 };

        public Energie(bool dur)
            : base(MODELE_E4, MODELE_E4_SAUTS, Program.RGBtoSDLColor(0x00FFFF),
            10, 0, VITESSE_MOYENNE_Z_ENNEMI / 1.5f, 50, 0, MODELE_E4_POINTS_TIR)
        {
            this.version_dur = dur;

            if (dur)
            {
                vitesse = VITESSE_MOYENNE_ENNEMI / 2;
                vitesse_z = VITESSE_MOYENNE_Z_ENNEMI * 1.5f;
                vitesse_tir = Program.G_FPS / VITESSE_MOYENNE_TIR_ENNEMI;
                HP = 15;
                couleure = Program.RGBtoSDLColor(0x008080);
            }
        }

        // les boules d'enegries sont les seuls ennemis sans modèles. ils utilisent le graphique de la bombe à pulsar.
        public override void RenderObjet()
        {
            BombePulsar.DessinerBombePulsar(
                new Vector2(position.x, position.y),
                (byte)(40 * MathF.Pow(0.95f, position.z)),
                couleure,
                false
            );
        }
    }

    internal class Croissant : Ennemi
    {
        static readonly Vector3[] MODELE_E5 =
        {
            new( -15, 45, 10),
            new( -10, 30, 10),
            new( -25, 30, 50),
            new( -15, 45, 10 ),
            new( -40, 30, 10 ),
            new( -25, 30, 50 ),
            new( -25, 20, 10 ),
            new( -10, 30, 10 ),
            new( -25, 20, 10 ),
            new( -40, 30, 10 ),
            new( -50, 0, 10 ),
            new( -40, -30, 10 ),
            new( -15, -50, 10 ),
            new( 15, -50, 10 ),
            new( 40, -30, 10 ),
            new( 50, 0, 10 ),
            new( 40, 25, 10 ),
            new( 15, 45, 10 ),
            new( 25, 30, 50 ),
            new( 40, 25, 10 ),
            new( 25, 20, 10 ),
            new( 25, 30, 50 ),
            new( 10, 30, 10 ),
            new( 15, 45, 10 ),
            new( 10, 30, 10 ),
            new( 25, 20, 10 ),
            new( 35, 0, 10 ),
            new( 25, -20, 10 ),
            new( 10, -30, 10 ),
            new( 0, -10, 0 ),
            new( -10, -30, 10 ),
            new( -25, -20, 10 ),
            new( -35, 0, 10 ),
            new( -25, 20, 10 ),
            new( -25, 20, -10 ),
            new( -10, 30, -10 ),
            new( -10, 30, 10 ),
            new( -10, 30, -10 ),
            new( -15, 45, -10 ),
            new( -15, 45, 10 ),
            new( -15, 45, -10 ),
            new( -40, 30, -10 ),
            new( -40, 30, 10 ),
            new( -40, 30, -10 ),
            new( -25, 20, -10 ),
            new( -35, 0, -10 ),
            new( -25, -20, -10 ),
            new( -10, -30, -10 ),
            new( 0, -10, 0 ),
            new( -15, 10, -15 ),
            new( 15, 10, -15 ),
            new( 0, -10, 0 ),
            new( 15, 10, 15 ),
            new( 15, 10, -15 ),
            new( 15, 10, 15 ),
            new( -15, 10, 15 ),
            new( -15, 10, -15 ),
            new( -15, 10, 15 ),
            new( 0, -10, 0 ),
            new( 10, -30, -10 ),
            new( 25, -20, -10 ),
            new( 35, 0, -10 ),
            new( 25, 20, -10 ),
            new( 40, 30, -10 ),
            new( 25, 20, -10 ),
            new( 25, 20, 10 ),
            new( 25, 20, -10 ),
            new( 10, 30, -10 ),
            new( 10, 30, 10 ),
            new( 10, 30, -10 ),
            new( 15, 45, -10 ),
            new( 15, 45, 10 ),
            new( 15, 45, -10 ),
            new( 40, 30, -10 ),
            new( 40, 30, 10 ),
            new( 40, 30, -10 ),
            new( 50, 0, -10 ),
            new( 40, -30, -10 ),
            new( 15, -50, -10 ),
            new( -15, -50, -10 ),
            new( -40, -30, -10 ),
            new( -50, 0, -10 ),
            new( -40, 30, -10 )
        };
        static readonly int[] MODELE_E5_SAUTS = { -1 };
        static readonly int[] MODELE_E5_POINTS_TIR = { 2, 21 };

        public Croissant(bool dur)
            : base(MODELE_E5, MODELE_E5_SAUTS, Program.RGBtoSDLColor(0x0000FF), 10,
            VITESSE_MOYENNE_ENNEMI * 0.75f, VITESSE_MOYENNE_Z_ENNEMI, 60, VITESSE_MOYENNE_TIR_ENNEMI * 4, MODELE_E5_POINTS_TIR)
        {
            this.version_dur = dur;

            if (dur)
            {
                vitesse = VITESSE_MOYENNE_ENNEMI;
                vitesse_tir = Program.G_FPS / (VITESSE_MOYENNE_TIR_ENNEMI * 3);
                HP = 12;
                couleure = Program.RGBtoSDLColor(0x000080);
            }
        }

        public override void AnimationModele()
        {
            float speed = version_dur ? 50f : 20f;

            float angle = timer / speed;

            for (int i = 0; i < modele_en_marche.Length; i++)
            {
                if (i == 49 || i == 56)
                {
                    modele_en_marche[i].x = MathF.Cos(angle) * 20;
                    modele_en_marche[i].z = MathF.Sin(angle) * 20;
                }
                if (i == 53 || i == 50)
                {
                    modele_en_marche[i].x = MathF.Cos(angle + 1.5f) * 20;
                    modele_en_marche[i].z = MathF.Sin(angle + 1.5f) * 20;
                }
                if (i == 52 || i == 54)
                {
                    modele_en_marche[i].x = MathF.Cos(angle + 3) * 20;
                    modele_en_marche[i].z = MathF.Sin(angle + 3) * 20;
                }
                if (i == 55 || i == 57)
                {
                    modele_en_marche[i].x = MathF.Cos(angle + 4.5f) * 20;
                    modele_en_marche[i].z = MathF.Sin(angle + 4.5f) * 20;
                }
            }
        }
    }

    internal class Dupliqueur : Ennemi
    {
        static readonly Vector3[] MODELE_E6 =
        {
            new( -25, 0, 0 ),
            new( 0, -10, 0 ),
            new( 25, 0, 0 ),
            new( 0, -10, -30 ),
            new( 0, -10, 0 ),
            new( 0, -10, -30 ),
            new( -25, 0, 0 )
        };
        static readonly int[] MODELE_E6_SAUTS = { -1 };
        static readonly int[] MODELE_E6_POINTS_TIR = { 3 };

        public Dupliqueur(bool dur, Dupliqueur? parent)
            : base(MODELE_E6, MODELE_E6_SAUTS, Program.RGBtoSDLColor(0x800080), 7,
            VITESSE_MOYENNE_ENNEMI / 2, VITESSE_MOYENNE_Z_ENNEMI / 2, 40, VITESSE_MOYENNE_TIR_ENNEMI * 2, MODELE_E6_POINTS_TIR)
        {
            this.version_dur = dur;

            if (dur)
            {
                vitesse = VITESSE_MOYENNE_ENNEMI * 1.25f;
                vitesse_z = VITESSE_MOYENNE_Z_ENNEMI / 4;
                vitesse_tir = Program.G_FPS / VITESSE_MOYENNE_TIR_ENNEMI;
                HP = 10;
                couleure = Program.RGBtoSDLColor(0x400080);
            }

            if (parent != null)
            {
                position = parent.position;
                status = parent.status - 1;
            }

            switch (status)
            {
                case StatusEnnemi.DUPLIQUEUR_0_RESTANT:
                    HP /= 4;
                    taille *= 0.5f;
                    break;

                case StatusEnnemi.DUPLIQUEUR_1_RESTANT:
                    HP /= 2;
                    taille *= 0.75f;
                    break;

                // les dupliqueurs sont initializés avec StatusEnnemi.INITIALIZATION
                default:
                    this.status = StatusEnnemi.DUPLIQUEUR_2_RESTANT;
                    break;
            }
        }

        protected override bool ActionsEnnemi()
        {
            roll = velocite.x / 15;
            return false;
        }

        protected override void QuandTouché(bool collision_joueur)
        {
            if (collision_joueur && status >= StatusEnnemi.DUPLIQUEUR_0_RESTANT && status <= StatusEnnemi.DUPLIQUEUR_1_RESTANT)
            {
                Program.ennemis_a_creer--;
            }
        }

        public override bool QuandMort()
        {
            // code pour dupliqueur avec des dupliquations restantes
            if (status > StatusEnnemi.DUPLIQUEUR_0_RESTANT && status <= StatusEnnemi.DUPLIQUEUR_2_RESTANT)
            {
                // déplace le parent pourque les enfants soient séparés physiquement
                const int DUPLIQUEUR_DISTANCE_SEPARATION_ENFANT = 30;

                position.x += DUPLIQUEUR_DISTANCE_SEPARATION_ENFANT;
                new Dupliqueur(version_dur, this);
                position.x -= DUPLIQUEUR_DISTANCE_SEPARATION_ENFANT * 2;
                new Dupliqueur(version_dur, this);

                status = StatusEnnemi.MORT;
                return true;
            }

            return false;
        }
    }

    internal class Patra : Ennemi
    {
        static readonly Vector3[] MODELE_E7 =
        {
            new( 0, 20, 0),
            new( 40, 0, 0),
            new( 0, -20, 0),
            new( -40, 0, 0 ),
            new( 0, 20, 0 ),
            new( 10, 0, 0 ),
            new( 0, -20, 0 ),
            new( -10, 0, 0 ),
            new( 0, 20, 0 )
        };
        static readonly int[] MODELE_E7_SAUTS = { -1 };
        static readonly int[] MODELE_E7_POINTS_TIR = { 5 };

        public Patra(bool dur)
            : base(MODELE_E7, MODELE_E7_SAUTS, Program.RGBtoSDLColor(0xFF00FF), 10,
            VITESSE_MOYENNE_ENNEMI, 0, 100, VITESSE_MOYENNE_TIR_ENNEMI * 3, MODELE_E7_POINTS_TIR)
        {
            this.version_dur = dur;

            if (dur)
            {
                vitesse_z = VITESSE_MOYENNE_Z_ENNEMI / 4;
                vitesse_tir = Program.G_FPS / (VITESSE_MOYENNE_TIR_ENNEMI * 4);
                HP = 15;
                couleure = Program.RGBtoSDLColor(0x800080);
            }

            status = StatusEnnemi.PATRA_8_RESTANT;
        }

        // code plutôt compliqué qui dessine un nombre X de triangles qui tournent sens horraire,
        // où tous les triangles ensemble tournent sens horraire autour de l'ennemi.
        // comme on le voit dans le nom, l'ennemi est inspiré de Patra de Zelda sur NES
        public override void AnimationModele()
        {
            SDL_SetRenderDrawColor(Program.render, couleure.r, couleure.g, couleure.b, couleure.a);

            // angle de départ pour le cercle de triangles
            float angle = (timer / 20f) % MathF.Tau;
            float profondeure = MathF.Pow(0.95f, position.z);
            float angle_prochain = MathF.Tau / 3;

            // pour chaque triangle restant
            for (int i = 0; i < status - StatusEnnemi.PATRA_0_RESTANT; i++)
            {
                // angle de départ pour les triangles
                float subAng = (timer / -10f) % MathF.Tau;
                // centre du triangle i
                float centerX2 = MathF.Cos(angle) * 80 * profondeure + position.x;
                float centerY2 = MathF.Sin(angle) * 80 * profondeure + position.y;
                // pour chaque côté du triangle
                for (int j = 0; j < 3; j++)
                {
                    SDL_RenderDrawLineF(Program.render,
                        centerX2 + MathF.Cos(subAng) * profondeure * 20,
                        centerY2 + MathF.Sin(subAng) * profondeure * 20,
                        centerX2 + MathF.Cos(subAng + angle_prochain) * profondeure * 20,
                        centerY2 + MathF.Sin(subAng + angle_prochain) * profondeure * 20
                    );
                    subAng += MathF.Tau / 3.0f;
                }
                angle += MathF.Tau / 8.0f;
            }
        }

        protected override void QuandTouché(bool collision_joueur)
        {
            if (collision_joueur)
            {
                if (status >= StatusEnnemi.PATRA_1_RESTANT && status <= StatusEnnemi.PATRA_8_RESTANT)
                    Program.ennemis_a_creer--;
                return;
            }

            // code pour patra
            if (status > StatusEnnemi.PATRA_0_RESTANT && status <= StatusEnnemi.PATRA_8_RESTANT)
            {
                HP++;
                status--;

                new MinionPatra(this);
            }
        }
    }

    internal class MinionPatra : Ennemi
    {
        static readonly Vector3[] MODELE_E7_1 =
        {
            new( 0, 10, 0 ),
            new( -8, -6, 0 ),
            new( 8, -6, 0 ),
            new( 0, 10, 0 )
        };
        static readonly int[] MODELE_E7_1_SAUTS = { -1 };
        static readonly int[] MODELE_E7_1_POINTS_TIR = Array.Empty<int>();

        public MinionPatra(Patra? parent)
            : base(MODELE_E7_1, MODELE_E7_1_SAUTS, Program.RGBtoSDLColor(0xFF00FF), 1,
            VITESSE_MOYENNE_ENNEMI, VITESSE_MOYENNE_Z_ENNEMI, 30, 0, MODELE_E7_1_POINTS_TIR)
        {
            if (parent != null)
            {
                position = parent.position;
                parent.status--;
                this.version_dur = parent.version_dur;
            }

            if (version_dur)
            {
                vitesse_z = VITESSE_MOYENNE_Z_ENNEMI * 1.5f;
                couleure = Program.RGBtoSDLColor(0x800080);
            }
        }

        protected override void QuandTouché(bool collision_joueur)
        {
            if (collision_joueur)
            {
                Program.ennemis_a_creer--;
            }
        }

        public override bool QuandMort()
        {
            return true;
        }
    }

    internal class Boss : Ennemi
    {
        const int BOSS_MAX_HP = 150;

        public Boss()
            : base(Player.MODELE_P, Player.MODELE_P_SAUTS, Program.RGBtoSDLColor(0xFF0000), BOSS_MAX_HP,
            VITESSE_MOYENNE_ENNEMI * 4, 0, 100, VITESSE_MOYENNE_TIR_ENNEMI * 3, Program.player.indexs_de_tir)
        {
            status = StatusEnnemi.BOSS_INIT;
            // ce code est normalement éxecuté dans INIT1 après 1 image, mais on veut que le boss apparaîsse au fond immédiatement
            position.z = 99;
        }

        // gère les animations du boss.
        // retourne vrai si le boss est en train de faire une animation
        bool CodeBoss()
        {
            if (status == StatusEnnemi.BOSS_NORMAL)
            {
                return false;
            }

            switch (status)
            {
                // partie #1 de l'animation de mort: le boss bouge vers le centre de l'écran avec des explosions
                case StatusEnnemi.BOSS_MORT:

                    if (Son.MusiqueJoue())
                        Son.ArreterMusique();

                    if (Vector2.Distance(position.x, position.y, Program.W_SEMI_LARGEUR, Program.W_SEMI_HAUTEUR) > 30)
                    {
                        if (position.x > Program.W_SEMI_LARGEUR)
                            position.x -= vitesse;
                        else
                            position.x += vitesse;
                        if (position.y > Program.W_SEMI_HAUTEUR)
                            position.y -= vitesse;
                        else
                            position.y += vitesse;
                    }
                    else
                    {
                        status = StatusEnnemi.BOSS_MORT_2;
                        timer = 0;
                    }

                    if (Program.gTimer % 30 == 0)
                    {
                        new Explosion(new Vector3(position.x + Program.RNG.Next(-20, 20), position.y + Program.RNG.Next(-20, 20), 0));
                    }

                    break;

                // partie #2 de l'animation de mort: le boss vibre au centre de l'écran avec des explosions pour 5 secondes
                case StatusEnnemi.BOSS_MORT_2:

                    position.x = Program.W_SEMI_LARGEUR + Program.RNG.Next(-2, 2);
                    position.y = Program.W_SEMI_HAUTEUR + Program.RNG.Next(-2, 2);

                    if (Program.gTimer % 30 == 0)
                    {
                        new Explosion(new Vector3(position.x + Program.RNG.Next(-20, 20), position.y + Program.RNG.Next(-20, 20), 0));
                    }

                    if (timer > 300)
                        status = StatusEnnemi.BOSS_MORT_3;

                    break;

                // partie #3 de l'animation de mort: le boss roule vers la caméra et l'écran fond vers blanc
                case StatusEnnemi.BOSS_MORT_3:

                    if (timer == 302)
                        Son.JouerEffet(ListeAudioEffets.EXPLOSION_JOUEUR);

                    roll += 0.05f;
                    taille += 0.005f;

                    if (Program.gTimer % 30 == 0)
                    {
                        new Explosion(new Vector3(position.x + Program.RNG.Next(-20, 20), position.y + Program.RNG.Next(-20, 20), 0));
                    }

                    if (timer < byte.MaxValue)
                    {
                        Program.couleure_fond_ecran.a = (byte)timer;
                        couleure.a = (byte)(255 - timer);
                    }

                    if (timer > 550)
                    {
                        Program.Gamemode = Gamemode.CUTSCENE_MAUVAISE_FIN;

                        Program.enemies.Clear();
                        Program.explosions.Clear();
                    }

                    break;

                // partie #1 de l'animation d'initialization du boss: le boss apparaît et enlève l'item du joueur
                case StatusEnnemi.BOSS_INIT:

                    timer = 0;
                    Program.player.powerup = TypeItem.NONE;
                    Program.player.fire_rate = Player.JOUEUR_VITESSE_TIR;
                    position = new Vector3(Program.W_SEMI_LARGEUR, Program.W_SEMI_HAUTEUR, 100);
                    status = StatusEnnemi.BOSS_INIT_2;

                    break;

                // partie #2 de l'animation d'initialization du boss: le boss vole vers l'écran jusqu'à z=20
                case StatusEnnemi.BOSS_INIT_2:

                    position.z = 100 - timer;

                    if (position.z <= 20)
                    {
                        status = StatusEnnemi.BOSS_INIT_3;
                        timer = 0;
                        Program.player.velocite.y = -10;
                        Program.bouger_etoiles = false;
                    }

                    break;

                // partie #3 de l'animation d'initialization du boss: monologue.
                // bla bla bla utilize le debug monologue_skip pour sauter cette étape
                case StatusEnnemi.BOSS_INIT_3:

                    if (Program.monologue_skip)
                        timer = 1021;

                    if (timer < 240)
                    {
                        Text.DisplayText("allo pilote. ton vaisseau n'est pas le seul.",
                            new Vector2(Text.CENTRE, Program.W_SEMI_HAUTEUR + 250), 2, scroll: timer / 2);
                    }
                    else if (timer >= 260 && timer < 480)
                    {
                        Text.DisplayText("un espion nous a transféré les plans.",
                            new Vector2(Text.CENTRE, Program.W_SEMI_HAUTEUR + 250), 2, scroll: timer / 2 - 130);
                    }
                    else if (timer >= 500 && timer < 800)
                    {
                        Text.DisplayText("la bombe à pulsar est très puissante, et",
                            new Vector2(Text.CENTRE, Program.W_SEMI_HAUTEUR + 250), 2, scroll: timer / 2 - 250);
                        Text.DisplayText("encore plus fragile. je ne peux pas te laisser près d'elle.",
                            new Vector2(Text.CENTRE, Program.W_SEMI_HAUTEUR + 288), 2, scroll: timer / 2 - 290);
                    }
                    else if (timer >= 820 && timer < 1020)
                    {
                        Text.DisplayText("que le meilleur pilote gagne. en guarde.",
                            new Vector2(Text.CENTRE, Program.W_SEMI_HAUTEUR + 250), 2, scroll: timer / 2 - 410);
                    }

                    // effet sonnore qui fait que la musique ne commence pas d'un coup
                    if (timer == 886)
                        Son.JouerEffet(ListeAudioEffets.DOTV_ENTREE);

                    if (timer > 1020)
                    {
                        status = StatusEnnemi.BOSS_NORMAL;
                        BombePulsar.HP_bombe = BombePulsar.BOMBE_PULSAR_MAX_HP;
                        Son.JouerMusique(ListeAudioMusique.DOTV, true);
                    }

                    break;
            }

            return true;
        }

        protected override bool ActionsEnnemi()
        {
            if (status == StatusEnnemi.BOSS_NORMAL)
            {
                roll = velocite.x / 15;
                // fait que le boss (qui a le modèle du joueur) paraît normal
                pitch -= 0.5f;
            }

            return CodeBoss();
        }

        // ce code fait que la cible est non où le joueur est, mais où il VA être.
        // bien sûr, donner ceci à tous les ennemis rendrait le jeu beaucoup trop dur, alors c'est seulement pour le boss.
        protected override Vector3 TrouverCibleTir()
        {
            return new Vector3(
                Program.player.position.x + Program.player.velocite.x * position.z,
                Program.player.position.y + Program.player.velocite.y * position.z,
                Program.player.position.z
            );
        }

        public override bool QuandMort()
        {
            status = StatusEnnemi.BOSS_MORT;
            // le programme va supprimmer l'ennemi car il est en train de mourrir, mais ceci est le boss, et on en a besoin pour la scène d'apres.
            // donc on ajoute une deuxième copie dans la liste d'ennemie, et la première des deux sera supprimée.
            Program.enemies.Add(this);
            return true;
        }
    }
}