using System;
using System.Numerics;
using static SDL2.SDL;

namespace Dysgenesis
{
    public enum TypeEnnemi
    {
        OCTAHEDRON,
        DIAMANT,
        TOURNANT,
        ENERGIE,
        CROISSANT,
        DUPLIQUEUR,
        PATRA,
        OCTAHEDRON_DUR,
        DIAMANT_DUR,
        TOURNANT_DUR,
        ENERGIE_DUR,
        CROISSANT_DUR,
        DUPLIQUEUR_DUR,
        PATRA_DUR,
        BOSS,
        PATRA_MINION,
        PATRA_MINION_DUR
    };
    public enum StatusEnnemi
    {
        VIDE = 0,
        INITIALIZATION,
        NORMAL,
        MORT,

        DUPLIQUEUR_0_RESTANT = 60,
        DUPLIQUEUR_1_RESTANT,
        DUPLIQUEUR_2_RESTANT,

        PATRA_0_RESTANT = 70,
        PATRA_1_RESTANT,
        PATRA_2_RESTANT,
        PATRA_3_RESTANT,
        PATRA_4_RESTANT,
        PATRA_5_RESTANT,
        PATRA_6_RESTANT,
        PATRA_7_RESTANT,
        PATRA_8_RESTANT,

        BOSS_INIT = 150,
        BOSS_INIT_2,
        BOSS_INIT_3,
        BOSS_NORMAL,
        BOSS_MORT,
        BOSS_MORT_2,
        BOSS_MORT_3,
    }
    public class Ennemi : Sprite
    {
        public const int NB_TYPES_ENNEMIS = 17;
        const int BOSS_MAX_HP = 150;
        const int DISTANCE_DE_BORD_EVITER_INIT = 200;
        const float VITESSE_MOYENNE_ENNEMI = 0.4f;
        const float VITESSE_PROFONDEUR_MOYENNE_ENNEMI = 0.9995f;
        const float ENNEMI_FRICTION = 0.8f;

        //todo: créer classe data ennemi, ou classe ennemi parentale
        readonly float[] liste_vitesse_ennemi =
        {
            0.125f, 0.5f, 0.5f, 0.125f, 1.5f, 0.5f, 2,
            1, 2, 2, 1, 2, 2, 2,
            4, 1, 1
        };
        readonly float[] liste_vitesse_profondeur_ennemi =
        {
            VITESSE_PROFONDEUR_MOYENNE_ENNEMI, VITESSE_PROFONDEUR_MOYENNE_ENNEMI, VITESSE_PROFONDEUR_MOYENNE_ENNEMI * VITESSE_PROFONDEUR_MOYENNE_ENNEMI,
            VITESSE_PROFONDEUR_MOYENNE_ENNEMI / 10 + 0.9f, VITESSE_PROFONDEUR_MOYENNE_ENNEMI, VITESSE_PROFONDEUR_MOYENNE_ENNEMI / 10 + 0.9f,
            1, VITESSE_PROFONDEUR_MOYENNE_ENNEMI, VITESSE_PROFONDEUR_MOYENNE_ENNEMI, VITESSE_PROFONDEUR_MOYENNE_ENNEMI * VITESSE_PROFONDEUR_MOYENNE_ENNEMI,
            VITESSE_PROFONDEUR_MOYENNE_ENNEMI, VITESSE_PROFONDEUR_MOYENNE_ENNEMI, VITESSE_PROFONDEUR_MOYENNE_ENNEMI / 100 + 0.99f,
            VITESSE_PROFONDEUR_MOYENNE_ENNEMI / 1000 + 0.999f, 1, VITESSE_PROFONDEUR_MOYENNE_ENNEMI * VITESSE_PROFONDEUR_MOYENNE_ENNEMI,
            VITESSE_PROFONDEUR_MOYENNE_ENNEMI * VITESSE_PROFONDEUR_MOYENNE_ENNEMI
        };
        readonly float[] liste_vitesse_tir_ennemi =
        {
            0, 2, 2, 0, 4, 2, 3,
            1, 1, 1, 2, 3, 1, 4,
            0.5f, 0, 0
        };
        readonly int[] liste_hp_ennemi =
        {
            1, 3, 3, 10, 10, 7, 10,
            3, 5, 7, 15, 12, 10, 15,
            BOSS_MAX_HP, 1, 1
        };
        readonly int[] liste_largeur_ennemi =
        {
            30, 30, 50, 50, 60, 40, 100,
            30, 30, 50, 50, 60, 40, 100,
            50, 30, 30
        };
        readonly uint[] liste_couleure_ennemi =
        {
            0xFFFF00FF, 0xFF7F00FF, 0x00FF00FF, 0x00FFFFFF, 0x0000FFFF, 0x7F007FFF, 0xFF00FFFF,
            0x7F7F00FF, 0x7F4000FF, 0x007F00FF, 0x007F7FFF, 0x00007FFF, 0x400080FF, 0x7F007FFF,
            0xFF0000FF, 0xFF00FFFF, 0x7F007FFF
        };
        readonly int[][] liste_indexs_tir_ennemi =
        {
            new int[] { 18, -1},
            new int[] {61, 45},
            new int[] {0, 8},
            new int[] {-1, -1},
            new int[] {2, 21},
            new int[] {3, -1},
            new int[] {5, -1},
            new int[] {18, -1},
            new int[] {61, 45},
            new int[] {0, 8},
            new int[] {-1, -1},
            new int[] {2, 21},
            new int[] {3, -1},
            new int[] {5, -1},
            new int[] {1, 16},
            new int[] {-1, -1},
            new int[] {-1, -1}
        };

        public int damage_radius;
        public int HP;

        public float fire_cooldown;
        public float speed;
        public float z_speed;

        public TypeEnnemi type;
        public StatusEnnemi statut = StatusEnnemi.VIDE;
        Vector2 velocite;
        Vector2 target;
        Vector3[] modele_en_marche;

        public Ennemi(TypeEnnemi type, StatusEnnemi statut, Ennemi? parent = null)
        {
            Program.enemies.Add(this);

            this.type = type;
            this.statut = statut;
            velocite = new Vector2();
            target = new Vector2(Program.player.position.x, Program.player.position.y);

            //todo: voir en haut défénitions
            damage_radius = liste_largeur_ennemi[(int)type];
            HP = liste_hp_ennemi[(int)type];
            speed = liste_vitesse_ennemi[(int)type] * VITESSE_MOYENNE_ENNEMI;
            z_speed = liste_vitesse_profondeur_ennemi[(int)type];
            indexs_de_tir = liste_indexs_tir_ennemi[(int)type];
            fire_cooldown = liste_vitesse_tir_ennemi[(int)type];
            modele = Data.modeles_ennemis[(int)type];
            indexs_lignes_sauter = Data.lignes_a_sauter_ennemis[(int)type];
            uint couleure = liste_couleure_ennemi[(int)type];

            this.couleure = new SDL_Color()
            {
                r = (byte)((couleure & 0xFF000000) >> 24),
                g = (byte)((couleure & 0x00FF0000) >> 16),
                b = (byte)((couleure & 0x0000FF00) >> 8),
                a = (byte)(couleure & 0x000000FF)
            };
            modele_en_marche = modele.ToArray();

            afficher = true;
            taille = 1;

            if (parent != null)
            {
                position = parent.position;
            }
            else
            {
                position = new Vector3(
                    Program.RNG.Next(DISTANCE_DE_BORD_EVITER_INIT, Data.W_LARGEUR - DISTANCE_DE_BORD_EVITER_INIT),
                    Program.RNG.Next(DISTANCE_DE_BORD_EVITER_INIT, Data.W_HAUTEUR - DISTANCE_DE_BORD_EVITER_INIT),
                    Data.G_DEPTH_LAYERS
                );
            }

            if (type == TypeEnnemi.BOSS)
            {
                this.statut = StatusEnnemi.BOSS_INIT;
            }
            else if (type == TypeEnnemi.PATRA || type == TypeEnnemi.PATRA_DUR)
            {
                this.statut = StatusEnnemi.PATRA_8_RESTANT;
            }
            else if ((type == TypeEnnemi.DUPLIQUEUR || type == TypeEnnemi.DUPLIQUEUR_DUR) &&
                !(statut >= StatusEnnemi.DUPLIQUEUR_0_RESTANT && statut <= StatusEnnemi.DUPLIQUEUR_2_RESTANT))
            {
                this.statut = StatusEnnemi.DUPLIQUEUR_2_RESTANT;
            }
            else if (statut == StatusEnnemi.INITIALIZATION)
            {
                this.statut = StatusEnnemi.NORMAL;
            }
        }

        public override bool Exist()
        {
            if (Program.player.Mort())
                return false;

            timer++;

            if (type == TypeEnnemi.BOSS && statut != StatusEnnemi.BOSS_NORMAL)
            {
                Enemy15();
                return false;
            }

            UpdateModele();
            Move();

            for (int i = 0; i < Program.projectiles.Count; i++)
            {
                if (ProjCollision(Program.projectiles[i]) != 0) return true;
            }

            if (PlayerCollision() != 0) return true;

            Tirer();

            return false;
        }

        public bool Mort()
        {
            return HP <= 0;
        }

        public void UpdateModele()
        {
            float speed = 0;
            SDL_SetRenderDrawColor(Program.render, couleure.r, couleure.g, couleure.b, couleure.a);

            switch (type)
            {
                case TypeEnnemi.OCTAHEDRON_DUR:
                    speed = 50.0f;
                    goto case TypeEnnemi.OCTAHEDRON;
                case TypeEnnemi.OCTAHEDRON:
                    if (speed == 0)
                        speed = 20.0f;

                    for (int i = 0; i < modele_en_marche.Length; i++)
                    {
                        if (i == 0 || i == 3 || i == 9)
                        {
                            modele_en_marche[i].x = (sbyte)(MathF.Cos(timer / speed) * 20);
                            modele_en_marche[i].z = (sbyte)(MathF.Sin(timer / speed) * 20);
                        }
                        if (i == 1 || i == 5 || i == 10)
                        {
                            modele_en_marche[i].x = (sbyte)(MathF.Cos(timer / speed + 2) * 20);
                            modele_en_marche[i].z = (sbyte)(MathF.Sin(timer / speed + 2) * 20);
                        }
                        if (i == 2 || i == 7)
                        {
                            modele_en_marche[i].x = (sbyte)(MathF.Cos(timer / speed + 4) * 20);
                            modele_en_marche[i].z = (sbyte)(MathF.Sin(timer / speed + 4) * 20);
                        }
                    }
                    break;

                case TypeEnnemi.TOURNANT_DUR:
                    speed = 50.0f;
                    goto case TypeEnnemi.TOURNANT;
                case TypeEnnemi.TOURNANT:
                    if (speed == 0)
                        speed = 20.0f;

                    for (int i = 0; i < modele_en_marche.GetLength(0); i++)
                    {
                        switch (i)
                        {
                            case 16:
                            case 19:
                                modele_en_marche[i].y = MathF.Cos(timer / speed) * 10;
                                modele_en_marche[i].z = MathF.Sin(timer / speed) * 10;
                                break;
                            case 13:
                            case 17:
                                modele_en_marche[i].y = MathF.Cos(timer / speed + 1.5f) * 10;
                                modele_en_marche[i].z = MathF.Sin(timer / speed + 1.5f) * 10;
                                break;
                            case 6:
                            case 12:
                                modele_en_marche[i].y = MathF.Cos(timer / speed + 3) * 10;
                                modele_en_marche[i].z = MathF.Sin(timer / speed + 3) * 10;
                                break;
                            case 7:
                            case 20:
                                modele_en_marche[i].y = MathF.Cos(timer / speed + 4.5f) * 10;
                                modele_en_marche[i].z = MathF.Sin(timer / speed + 4.5f) * 10;
                                break;
                            case 15:
                            case 23:
                                modele_en_marche[i].y = MathF.Cos(timer / -speed) * 10;
                                modele_en_marche[i].z = MathF.Sin(timer / -speed) * 10;
                                break;
                            case 4:
                            case 14:
                                modele_en_marche[i].y = MathF.Cos(timer / -speed + 1.5f) * 10;
                                modele_en_marche[i].z = MathF.Sin(timer / -speed + 1.5f) * 10;
                                break;
                            case 5:
                            case 25:
                                modele_en_marche[i].y = MathF.Cos(timer / -speed + 3) * 10;
                                modele_en_marche[i].z = MathF.Sin(timer / -speed + 3) * 10;
                                break;
                            case 21:
                            case 24:
                                modele_en_marche[i].y = MathF.Cos(timer / -speed + 4.5f) * 10;
                                modele_en_marche[i].z = MathF.Sin(timer / -speed + 4.5f) * 10;
                                break;
                        }
                    }
                    break;

                case TypeEnnemi.ENERGIE:
                case TypeEnnemi.ENERGIE_DUR:
                    BombePulsar.DessinerBombePulsar(
                        new Vector2(position.x, position.y),
                        (byte)(40 * MathF.Pow(0.95f, position.z)),
                        couleure,
                        false
                    );
                    break;

                case TypeEnnemi.CROISSANT:
                    speed = 20.0f;
                    goto case TypeEnnemi.CROISSANT_DUR;
                case TypeEnnemi.CROISSANT_DUR:
                    if (speed == 0)
                        speed = 50.0f;

                    for (int i = 0; i < modele_en_marche.Length; i++)
                    {
                        if (i == 49 || i == 56)
                        {
                            modele_en_marche[i].x = MathF.Cos(timer / speed) * 20;
                            modele_en_marche[i].z = MathF.Sin(timer / speed) * 20;
                        }
                        if (i == 53 || i == 50)
                        {
                            modele_en_marche[i].x = MathF.Cos(timer / speed + 1.5f) * 20;
                            modele_en_marche[i].z = MathF.Sin(timer / speed + 1.5f) * 20;
                        }
                        if (i == 52 || i == 54)
                        {
                            modele_en_marche[i].x = MathF.Cos(timer / speed + 3) * 20;
                            modele_en_marche[i].z = MathF.Sin(timer / speed + 3) * 20;
                        }
                        if (i == 55 || i == 57)
                        {
                            modele_en_marche[i].x = MathF.Cos(timer / speed + 4.5f) * 20;
                            modele_en_marche[i].z = MathF.Sin(timer / speed + 4.5f) * 20;
                        }
                    }
                    break;

                case TypeEnnemi.PATRA:
                case TypeEnnemi.PATRA_DUR:

                    float ang14 = (timer / 20f) % (float)(2 * MathF.PI);
                    float profondeure = 20 * MathF.Pow(0.95f, position.z);
                    float profondeure_prochain = (2 * MathF.PI) / 3;
                    short centerX2, centerY2;
                    for (int i = 0; i < (int)statut - (int)StatusEnnemi.PATRA_0_RESTANT; i++)
                    {
                        centerX2 = (short)(MathF.Cos(ang14) * 80 * MathF.Pow(0.95f, position.z) + position.x);
                        centerY2 = (short)(MathF.Sin(ang14) * 80 * MathF.Pow(0.95f, position.z) + position.y);
                        float subAng = (timer / -10f) % (float)(2 * MathF.PI);
                        for (int j = 0; j < 3; j++)
                        {
                            SDL_RenderDrawLine(Program.render,
                                (int)(centerX2 + MathF.Cos(subAng) * profondeure),
                                (int)(centerY2 + MathF.Sin(subAng) * profondeure),
                                (int)(centerX2 + MathF.Cos(subAng + profondeure_prochain) * profondeure),
                                (int)(centerY2 + MathF.Sin(subAng + profondeure_prochain) * profondeure)
                            );
                            subAng += (float)(2 * MathF.PI) / 3;
                        }
                        ang14 += (float)(2 * MathF.PI) / 8;
                    }
                    break;
            }
        }

        void Enemy15()
        {
            switch (statut)
            {
                case StatusEnnemi.BOSS_MORT:
                    Son.StopMusic();

                    if (Background.Distance(position.x, position.y, Data.W_SEMI_LARGEUR, Data.W_SEMI_HAUTEUR) > 30)
                    {
                        if (position.x > Data.W_SEMI_LARGEUR)
                            position.x -= 3;
                        else
                            position.x += 3;
                        if (position.y > Data.W_SEMI_HAUTEUR)
                            position.y -= 3;
                        else
                            position.y += 3;
                    }
                    else
                    {
                        statut = StatusEnnemi.BOSS_MORT_2;
                        timer = 0;
                    }

                    if (Program.gTimer % 30 == 0)
                    {
                        new Explosion(new Vector3(position.x + Program.RNG.Next(-20, 20), position.y + Program.RNG.Next(-20, 20), 0));
                    }

                    break;
                case StatusEnnemi.BOSS_MORT_2:
                    position.x = Data.W_SEMI_LARGEUR + Program.RNG.Next(-2, 2);
                    position.y = Data.W_SEMI_HAUTEUR + Program.RNG.Next(-2, 2);

                    if (Program.gTimer % 30 == 0)
                    {
                        new Explosion(new Vector3(position.x + Program.RNG.Next(-20, 20), position.y + Program.RNG.Next(-20, 20), 0));
                    }

                    if (timer > 300)
                        statut = StatusEnnemi.BOSS_MORT_3;

                    break;
                case StatusEnnemi.BOSS_MORT_3:
                    if (timer == 301)
                        Son.JouerEffet(ListeAudio.EXPLOSION_JOUEUR);

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
                        Program.gamemode = Gamemode.CUTSCENE_BAD_END;
                        Program.gTimer = 0;
                        Program.enemies.Clear();
                        Program.explosions.Clear();
                    }

                    break;
                case StatusEnnemi.BOSS_INIT:
                    timer = 0;
                    Program.player.powerup = TypeItem.NONE;
                    Program.player.fire_rate = Player.JOUEUR_VITESSE_TIR;
                    position = new Vector3(Data.W_SEMI_LARGEUR, Data.W_SEMI_HAUTEUR, 100);
                    statut = StatusEnnemi.BOSS_INIT_2;
                    break;
                case StatusEnnemi.BOSS_INIT_2:
                    position.z = 100 - timer;

                    if (position.z <= 20)
                    {
                        statut = StatusEnnemi.BOSS_INIT_3;
                        timer = 0;
                        Program.player.velocity.y = -10;
                        Program.bouger_etoiles = false;
                    }

                    break;
                case StatusEnnemi.BOSS_INIT_3:
                    if (timer == 1 && Program.monologue_skip)
                        timer = 1021;

                    if (timer < 240)
                        Text.DisplayText("allo pilote. ton vaisseau n'est pas le seul.", new Vector2(300, 800), 3, Text.BLANC, Text.OPAQUE, timer / 2);
                    else if (timer >= 260 && timer < 480)
                        Text.DisplayText("un espion nous a transféré les plans.", new Vector2(300, 800), 3, Text.BLANC, Text.OPAQUE, timer / 2 - 130);
                    else if (timer >= 500 && timer < 800)
                        Text.DisplayText("la bombe à pulsar est très puissante, et\n" +
                            "encore plus fragile. je ne peux pas te laisser près d'elle.", new Vector2(300, 800), 3, Text.BLANC, Text.OPAQUE, timer / 2 - 250);
                    else if (timer >= 820 && timer < 1020)
                        Text.DisplayText("que le meilleur pilote gagne. en guarde.", new Vector2(300, 800), 3, Text.BLANC, Text.OPAQUE, timer / 2 - 410);

                    if (timer > 1020)
                    {
                        statut = StatusEnnemi.BOSS_NORMAL;
                        BombePulsar.HP_bombe = BombePulsar.BOMBE_PULSAR_MAX_HP;
                        Son.JouerMusique(ListeAudio.DOTV, true);
                    }

                    if (timer == 886)
                        Son.JouerMusique(ListeAudio.DOTV_ENTREE, false);

                    break;
            }
        }

        // code de collision entre l'ennemi et un projectile.
        // retourne 1 si l'ennemi se fait tuer par l'attaque, sinon 0
        int ProjCollision(Projectile projectile)
        {
            // si le projectile n'est pas actif ou le projectile est d'origine ennemie
            if (!afficher || projectile.proprietaire == ProprietaireProjectile.ENNEMI)
                return 0;

            // vérifie si le projectile est à la bonne profondeur pour frapper, mais on ignore la profondeur si le joueur a un laser
            if (MathF.Abs(projectile.position.z - position.z) > 2 && Program.player.powerup != TypeItem.LASER)
                return 0;

            // si c'est un laser, un ne vérifie qu'une fois au 10 frames
            if (Program.player.powerup == TypeItem.LASER && Program.gTimer % 10 != 0)
                return 0;

            // les positions sur l'écran du projectile
            float[] proj_pos = projectile.PositionsSurEcran();

            // si la distance est plus que la largeur de l'ennemi
            if (Background.Distance(position.x, position.y, proj_pos[0], proj_pos[1]) > damage_radius)
                return 0;

            // ennemi touché
            HP--;
            new Explosion(position);

            // désactive le projectile, sauf si c'est un laser
            if (Program.player.powerup != TypeItem.LASER)
                Program.projectiles.Remove(projectile);

            // code pour patra qui a toujours des minions
            if (statut > StatusEnnemi.PATRA_0_RESTANT && statut <= StatusEnnemi.PATRA_8_RESTANT)
            {
                HP++;
                statut--;

                new Ennemi(type == TypeEnnemi.PATRA ? TypeEnnemi.PATRA_MINION : TypeEnnemi.PATRA_MINION_DUR, StatusEnnemi.NORMAL, this);
            }

            // le reste du code est pour ceux qui viennent de mourrir
            if (!Mort())
            {
                return 0;
            }

            // code pour minion patron
            if (type == TypeEnnemi.PATRA_MINION || type == TypeEnnemi.PATRA_MINION_DUR)
            {
                statut = StatusEnnemi.MORT;
                Program.enemies.Remove(this);
                return 1;
            }

            // code pour boss
            if (type == TypeEnnemi.BOSS)
            {
                statut = StatusEnnemi.BOSS_MORT;
                return 1;
            }

            // code pour dupliqueur avec des dupliquations restantes
            if (statut > StatusEnnemi.DUPLIQUEUR_0_RESTANT && statut <= StatusEnnemi.DUPLIQUEUR_2_RESTANT)
            {
                position.x += 30;
                new Ennemi(type, statut - 1, this);
                position.x -= 60;
                new Ennemi(type, statut - 1, this);

                statut = StatusEnnemi.MORT;
                Program.enemies.Remove(this);
                return 1;
            }

            // code pour les ennemis normaux qui peuvent lâcher un item
            Program.ens_killed++;
            new Item(this);
            statut = StatusEnnemi.MORT;
            Program.enemies.Remove(this);
            return 1;
        }

        // code pour détecter si un ennemi touche le joueur. retourne 1 si touché, 0 sinon.
        int PlayerCollision()
        {
            // si ennemi n'est pas à la profondeure du joueur ou l'ennemi est trop loin du joueur, quitte
            if (position.z != 0)
                return 0;

            if (Background.Distance(position.x, position.y, Program.player.position.x, Program.player.position.y) > Data.P_WIDTH)
                return 0;

            // touché
            new Explosion(position);
            Program.player.HP -= 3;

            byte ajout = 1;
            // ces ennemis ci-dessous ne comptent pas pour le total, alors on demande un nouveau ennemi si ce n'est pas un ci-dessous qui vient de mourrir
            if (type == TypeEnnemi.PATRA_MINION
                || type == TypeEnnemi.PATRA_MINION_DUR
                || (statut >= StatusEnnemi.DUPLIQUEUR_0_RESTANT && statut <= StatusEnnemi.DUPLIQUEUR_1_RESTANT)
                || (statut >= StatusEnnemi.PATRA_1_RESTANT && statut <= StatusEnnemi.PATRA_8_RESTANT))
            {
                ajout = 0;
            }
            Program.ens_needed += ajout;

            // tue l'ennemi
            statut = StatusEnnemi.MORT;
            Program.enemies.Remove(this);
            return 1;
        }

        Vector2 UpdateTarget()
        {
            if (position.z == 0)
                return new Vector2(Program.player.position.x, Program.player.position.y);

            short dist_enemy_target = Background.Distance(target.x, target.y, position.x, position.y);

            if (dist_enemy_target > 30)
            {
                return target;
            }

            short dist_player_target = Background.Distance(target.x, target.y, Program.player.position.x, Program.player.position.y);

            if (dist_player_target < 205 * (position.z / Data.G_DEPTH_LAYERS))
            {
                float num1 = Program.RNG.Next(100, Data.W_LARGEUR - 100);
                float num2 = Program.RNG.Next(100, Data.W_HAUTEUR - 400);

                while (Background.Distance(num1, num2, Program.player.position.x, Program.player.position.y) < 800)
                {
                    num1 = Program.RNG.Next(100, Data.W_LARGEUR - 100);
                    num2 = Program.RNG.Next(100, Data.W_HAUTEUR - 400);
                }

                return new Vector2(num1, num2);
            }

            if (dist_player_target < 800)
            {
                return new Vector2(Program.player.position.x, Program.player.position.y - 300 * (position.z / Data.G_DEPTH_LAYERS));
            }
            else
            {
                float num1 = Program.RNG.Next(100, Data.W_LARGEUR - 100);
                float num2 = Program.RNG.Next(100, Data.W_HAUTEUR - 400);

                while (Background.Distance(num1, num2, Program.player.position.x, Program.player.position.y) > 800)
                {
                    num1 = Program.RNG.Next(100, Data.W_LARGEUR - 100);
                    num2 = Program.RNG.Next(100, Data.W_HAUTEUR - 400);
                }

                return new Vector2(num1, num2);
            }
        }

        void Move()
        {
            if (z_speed != 0 && position.z != 0)
            {
                position.z *= z_speed;

                if (position.z < 1.0f)
                    position.z = 0;
            }

            if (speed != 0)
            {
                target = UpdateTarget();

                if (position.x < target.x)
                    velocite.x += speed;
                else if (position.x > target.x)
                    velocite.x -= speed;

                if (position.y < target.y)
                    velocite.y += speed;
                else if (position.y > target.y)
                    velocite.y -= speed;

                velocite.x *= ENNEMI_FRICTION;
                velocite.y *= ENNEMI_FRICTION;
                position.x += velocite.x;
                position.y += velocite.y;
            }

            if (position.z == 0)
                speed *= 1.01f;

            pitch = (position.y - Data.W_SEMI_HAUTEUR) / Data.W_SEMI_HAUTEUR * 0.25f;

            if (type == TypeEnnemi.DUPLIQUEUR || type == TypeEnnemi.DUPLIQUEUR_DUR || type == TypeEnnemi.BOSS)
                roll = velocite.x / 15;

            if (type == TypeEnnemi.BOSS)
                pitch -= 0.5f;
        }

        void Tirer()
        {
            if (fire_cooldown == 0)
                return;

            if (timer % fire_cooldown != 0)
                return;

            float[] line_data;
            if (timer % (int)(fire_cooldown * Data.G_FPS) == 0)
            {
                for (byte i = 0; i < indexs_de_tir.Length; i++)
                {
                    if (indexs_de_tir[i] == -1)
                        break;

                    line_data = RenderLineData(indexs_de_tir[i]);
                    new Projectile(
                        new Vector3(line_data[0], line_data[1], position.z),
                        Program.player.position,
                        ProprietaireProjectile.ENNEMI,
                        i
                    );
                }
            }
        }

        public override void RenderObject()
        {
            base.RenderObject(modele_en_marche);
        }
    }
}