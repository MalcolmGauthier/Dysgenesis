using SDL2;
using static SDL2.SDL;
using static System.Math;

namespace Dysgenesis
{
    public enum ProprietaireProjectile
    {
        JOUEUR,
        ENNEMI
    }

    public class Projectile : Sprite
    {
        const float VITESSE_PROJECTILE = Data.P_PROJ_SPEED;

        public Vector3 destination;
        public ProprietaireProjectile proprietaire;
        public byte ID;//todo: enlever ID
        public bool laser;

        public Projectile(Vector3 position, Vector3 destination, ProprietaireProjectile proprietaire, byte ID)
        {
            this.proprietaire = proprietaire;
            this.position = position;
            this.destination = destination;
            this.ID = ID;
            laser = false;

            if (proprietaire == ProprietaireProjectile.JOUEUR)
            {
                if (Program.GamemodeAction())
                    Son.JouerEffet(ListeAudioEffets.TIR);

                if (Program.player.powerup == TypeItem.LASER)
                    laser = true;
            }

            Program.projectiles.Add(this);
        }

        public override bool Exist()
        {
            if (ProjectileToucheJoueur() > 0) return true;

            if (Abs(position.z - destination.z) < 1)
            {
                Program.projectiles.Remove(this);
                return true;
            }
            else if (position.z < destination.z)
                position.z++;
            else
                position.z--;

            return false;
        }

        public void FindTarget()
        {
            int closest = 99;
            int closest_distance = 9999;

            for (int i = 0; i < Program.enemies.Count; i++)
            {
                if (Program.enemies[i].position.z > 0)
                {
                    int distance = Background.Distance(
                        Program.player.position.x,
                        Program.player.position.y,
                        Program.enemies[i].position.x,
                        Program.enemies[i].position.y
                    );

                    if (distance < closest_distance)
                    {
                        closest = i;
                        closest_distance = distance;
                    }
                }
            }
            if (closest != 99)
            {
                destination.x = Program.enemies[closest].position.x;
                destination.y = Program.enemies[closest].position.y;
            }
        }

        public float[] PositionsSurEcran(float depth)
        {
            Vector3 pos = position;
            Vector3 dest = destination;

            if (proprietaire == ProprietaireProjectile.ENNEMI)
            {
                // "it just works" - Todd Howard
                (pos, dest) = (dest, pos);
                pos.z = depth;
            }

            float dist_x_de_destination = pos.x - dest.x;
            float dist_y_de_destination = pos.y - dest.y;
            float facteur_profondeur_1 = MathF.Pow(VITESSE_PROJECTILE, depth);
            float facteur_profondeur_2 = MathF.Pow(VITESSE_PROJECTILE, depth + 1);


            return new float[4]{
                dist_x_de_destination * facteur_profondeur_1 + dest.x,
                dist_y_de_destination * facteur_profondeur_1 + dest.y,
                dist_x_de_destination * facteur_profondeur_2 + dest.x,
                dist_y_de_destination * facteur_profondeur_2 + dest.y
            };
        }
        public float[] PositionsSurEcran()
        {
            return PositionsSurEcran(position.z);
        }

        // Code de collision Projectile/Joueur.
        // Retourne >0 si projectile détruit, <=0 sinon
        public int ProjectileToucheJoueur()
        {
            if (proprietaire == ProprietaireProjectile.JOUEUR)
            {
                return -1;
            }

            if (position.z > 0)
            {
                return -2;
            }

            // pour si plusieurs projectiles touchent le joueur sur une image
            if (Program.player.Mort())
            {
                Program.projectiles.Remove(this);
                return 2;
            }

            float[] positions_projectile = PositionsSurEcran();

            // si le projectile manque le joueur
            // 0.75f = marge de manoeuvre, le projectile doit clairment frapper le joueur
            if (Background.Distance(positions_projectile[0], positions_projectile[1], Program.player.position.x, Program.player.position.y) > 0.75f * Data.P_WIDTH)
            {
                return 0;
            }

            // joueur frappé
            Program.player.HP -= 1;
            new Explosion(Program.player.position);
            Program.projectiles.Remove(this);

            if (!Program.player.Mort())
            {
                return 1;
            }

            // joueur mort
            Son.JouerEffet(ListeAudioEffets.EXPLOSION_JOUEUR);
            Son.StopMusic();
            Program.player.timer = 0;

            // code pour option continuer sur menu
            if (Program.curseur.curseur_max_selection < 2)
                Program.curseur.curseur_max_selection = 2;
            if (Program.Gamemode == Gamemode.GAMEPLAY)
                Program.nv_continue = Program.niveau;

            return 3;
        }

        public override void RenderObject()
        {
            if (proprietaire == ProprietaireProjectile.ENNEMI)
            {
                SDL_SetRenderDrawColor(Program.render, 255, 0, 0, 255);
            }
            else switch (Program.player.powerup)
            {
                case TypeItem.X2_SHOT:
                    SDL_SetRenderDrawColor(Program.render, 255, 127, 0, 255);
                    break;
                case TypeItem.X3_SHOT:
                    SDL_SetRenderDrawColor(Program.render, 255, 255, 0, 255);
                    break;
                case TypeItem.HOMING:
                    SDL_SetRenderDrawColor(Program.render, 64, 255, 64, 255);
                    break;
                case TypeItem.SPREAD:
                    SDL_SetRenderDrawColor(Program.render, 0, 0, 255, 255);
                    break;
                case TypeItem.LASER:
                    SDL_SetRenderDrawColor(Program.render, 127, 0, 255, 255);
                    break;
                default:
                    SDL_SetRenderDrawColor(Program.render, 255, 0, 0, 255);
                    break;
            }

            if (Program.player.Mort())
                return;

            float[] positions;

            if (laser && proprietaire == ProprietaireProjectile.JOUEUR)
            {
                // à chaque image, attacher le bout du laser au points de tir du joueur
                // TODO: ceci est la seule utilisation de ID, trouver comment l'enlever
                positions = Program.player.RenderLineData(Program.player.indexs_de_tir[ID % Program.player.indexs_de_tir.Length]);
                position.x = positions[0];
                position.y = positions[1];

                for (byte i = 0; i < Data.G_MAX_DEPTH; i++)
                {
                    positions = PositionsSurEcran(i);
                    SDL_RenderDrawLineF(Program.render,
                        positions[0] + Program.RNG.Next(-5, 5),
                        positions[1] + Program.RNG.Next(-5, 5),
                        positions[2] + Program.RNG.Next(-5, 5),
                        positions[3] + Program.RNG.Next(-5, 5)
                    );
                }

                return;
            }

            positions = PositionsSurEcran();
            SDL_RenderDrawLineF(Program.render,
                positions[0],
                positions[1],
                positions[2],
                positions[3]
            );
        }
    }
    public static class Shockwave
    {
        const int LARGEUR_MAX_VAGUE_ELECTRIQUE = 150;
        const int LARGEUR_MIN_VAGUE_ELECTRIQUE = 150;
        const int PRECISION_VAGUE_ELECTRIQUE = 50;

        static float rayon = 0;
        static float grow = 0;
        static bool shown = false;
        static uint cooldown;

        // affiche une nouvelle vague electrique si possible
        public static void AttemptSpawn()
        {
            if (shown || Program.player.Mort() || Program.player.shockwaves < 1.0f)
                return;

            cooldown = 0;
            Program.player.shockwaves -= 1.0f;
            rayon = 0;
            grow = LARGEUR_MAX_VAGUE_ELECTRIQUE + LARGEUR_MIN_VAGUE_ELECTRIQUE;
            shown = true;
            Son.JouerEffet(ListeAudioEffets.VAGUE);
        }
        
        public static void Display()
        {
            if (!shown)
                return;

            cooldown++;

            SDL_SetRenderDrawColor(Program.render, 0, 255, 255, 255);
            float angle = MathF.PI / (PRECISION_VAGUE_ELECTRIQUE / 2.0f);

            for (int nb_de_cercles = 0; nb_de_cercles < 3; nb_de_cercles++)
            {
                float rand1, rand2;
                for (float i = 0; i < PRECISION_VAGUE_ELECTRIQUE; i++)
                {
                    rand1 = Program.RNG.Next(-20, 20) + rayon;
                    rand2 = Program.RNG.Next(-20, 20) + rayon;
                    float angle_pos1 = i * angle;
                    float angle_pos2 = (i + 1) * angle;
                    SDL_RenderDrawLineF(Program.render,
                        Program.player.position.x + rand1 * MathF.Sin(angle_pos1),
                        Program.player.position.y + rand1 * MathF.Cos(angle_pos1),
                        Program.player.position.x + rand2 * MathF.Sin(angle_pos2),
                        Program.player.position.y + rand2 * MathF.Cos(angle_pos2)
                    );
                }
            }

            grow /= 1.2f;
            rayon = LARGEUR_MAX_VAGUE_ELECTRIQUE - grow;
            if (rayon >= LARGEUR_MAX_VAGUE_ELECTRIQUE - 1)
                shown = false;

            if (cooldown < 10 || cooldown % 3 != 0)
                return;

            for (int i = 0; i < Program.enemies.Count; i++)
            {
                if (Program.enemies[i].position.z != 0)
                    continue;

                if (Background.Distance(Program.enemies[i].position.x, Program.enemies[i].position.y, Program.player.position.x, Program.player.position.y) <= LARGEUR_MAX_VAGUE_ELECTRIQUE)
                    Program.enemies[i].HP--;

                if (Program.enemies[i].HP <= 0)
                {
                    Program.ens_killed++;
                    Program.enemies[i].afficher = false;
                }
            }
        }
    }
}