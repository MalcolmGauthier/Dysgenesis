using static SDL2.SDL;
#pragma warning disable CA1806

namespace Dysgenesis
{
    public enum ProprietaireProjectile
    {
        JOUEUR,
        ENNEMI
    }

    // classe pour les projectiles, du joueur et des ennemis
    public class Projectile : Sprite
    {
        const float VITESSE_PROJECTILE = 0.95f;

        // variable statique pour s'assurer que seulement un effet sonnor de tir s'éxecute par image.
        // quand t'as l'item qui te fait tirer 6 fois, ca commence à être vraiment bruyant...
        public static bool son_cree = false;

        public Vector3 destination;
        public ProprietaireProjectile proprietaire;
        public bool laser;

        // utilisé pour le calcul des positions de tirs ennemis sur l'écran
        float z_init;
        //todo: enlever ID, il dit si c'est le point de tir 1 ou 2 qui l'a tiré
        byte ID;

        public Projectile(Vector3 position, Vector3 destination, ProprietaireProjectile proprietaire, byte ID)
        {
            this.proprietaire = proprietaire;
            this.position = position;
            this.destination = destination;
            this.ID = ID;
            laser = false;
            z_init = this.position.z;

            if (proprietaire == ProprietaireProjectile.JOUEUR)
            {
                if (Program.GamemodeAction() && !son_cree)
                {
                    Son.JouerEffet(ListeAudioEffets.TIR);
                    son_cree = true;
                }

                if (Program.player.powerup == TypeItem.LASER)
                    laser = true;
            }

            Program.projectiles.Add(this);
        }

        // logique des projectiles
        public override bool Exist()
        {
            if (ProjectileToucheJoueur() > 0)
                return true;

            // si le projectile est arrivé à sa destination
            if (MathF.Abs(position.z - destination.z) < 1.0f)
            {
                Program.projectiles.Remove(this);
                return true;
            }

            if (position.z < destination.z)
            {
                position.z++;
            }
            else
            {
                position.z--;
            }

            return false;
        }

        // fonction pour quand le joueur a l'item qui tir vers l'ennemi le plus proche
        // modifie la destination à l'ennemi le plus proche, mais ne le change pas si aucun ennemi est trouvé
        public void TrouverCible()
        {
            if (Program.enemies.Count == 0)
                return;

            int plus_petite_distance = int.MaxValue;

            for (int i = 0; i < Program.enemies.Count; i++)
            {
                // on ne tire pas aux ennemis à z=0
                if (Program.enemies[i].position.z <= 0)
                    continue;

                int distance = Vector2.Distance(
                    Program.player.position.x,
                    Program.player.position.y,
                    Program.enemies[i].position.x,
                    Program.enemies[i].position.y
                );

                if (distance < plus_petite_distance)
                {
                    destination.x = Program.enemies[i].position.x;
                    destination.y = Program.enemies[i].position.y;
                    plus_petite_distance = distance;
                }
            }
        }

        // retourne la position du projectile (début et fin de la ligne)
        // sur l'écran du joueur. ceci est important pour le rendering
        // la variable profondeur devrait toujours être position.z, et est utile
        // pour les lasers et ennemis
        public float[] PositionsSurEcran(float profondeur)
        {
            Vector3 pos = position;
            Vector3 dest = destination;

            // la formule exponentielle utilisée pour les projectiles assume que le projectile traverse
            // dans la direction +Z. Mais pour les ennemis, le projectile traverse dans la direction -Z.
            // Alors si on essaye de dessiner le projectile sans ce bout de code, la ligne commencera grande
            // quand tiré par l'ennemi, et raptissera en approchant le joueur. Pour l'effet 3D, on veut l'inverse.
            // Pour réparer cela, on ment à la formule mathématique en inversant la position de départ (rappel que
            // le x et y variable position est utilisée par le projectile comme position de départ, elles ne changent
            // jamais) avec la destination. pas besoin de remettre les z à les bonnes valeures, car elles ne sont
            // pas utilisés dans cette fonction, et on utilise des copies locales.
            // 
            // Mais cette technique créé un autre problème qu'on doit régler! maintenant, à cause que en avant
            // est vers l'ennemi pour le calcul de position, le projectile ne touche plus l'ennemi, car les positions
            // des projectiles sont maintenant aux asymptotes de la fonction exponentielle. G_MAX_DEPTH n'est pas infini,
            // alors quand le projectile apparaît, il n'est pas du tout proche de l'ennemi. et ce phénomène devient de
            // plus en plus pire à force que l'ennemi se raproche de l'écran. J'ai à moitié réglé ce problème en
            // ajoutant du code dans le constructeur qui trouve la différence entre la position sur l'écran du
            // projectile avec où il devrait être, et modifie la position et la cible pour la déplacer avec l'ennemi.
            // le seul problème maintenant c'est que la cible n'est pas sur le joueur, et donc des projectiles tirés
            // de loin manquent.
            //
            // Donc finalement, j'ai juste demandé à chatGPT de me donner une nouvelle formule pour les projectiles,
            // et celle ci fonctionne. Pour ceci il fallait que je créé une nouvelle variable pour stoquer la coordonée z
            // originale du projectile, car elle en a besoin de se rappeller pour cette formule-ci.
            // elle ne fonctionne pas pour les projectiles du joueur, mais je n'ai pas envie
            // de régler cette stupide formule jusqu'à ce qu'il en aie une seule.
            // 
            // TLDR: "it just works" - Todd Howard
            if (proprietaire == ProprietaireProjectile.ENNEMI)
            {
                (pos, dest) = (dest, pos);

                float dist_x_de_dest = pos.x - dest.x;
                float dist_y_de_dest = pos.y - dest.y;
                float fact_profondeur_1 = MathF.Pow((z_init - profondeur) / (z_init - pos.z), 3);
                float fact_profondeur_2 = MathF.Pow((z_init - (profondeur + 1)) / (z_init - pos.z), 3);

                return new float[4]
                {
                    dist_x_de_dest * fact_profondeur_1 + dest.x,
                    dist_y_de_dest * fact_profondeur_1 + dest.y,
                    dist_x_de_dest * fact_profondeur_2 + dest.x,
                    dist_y_de_dest * fact_profondeur_2 + dest.y
                };
            }

            // techniquement ce code est meilleur, mais le vieux code marche mieux avec l'item HOMING

            //profondeur = Program.G_MAX_DEPTH - profondeur;

            //float dist_x_de_dest2 = pos.x - dest.x;
            //float dist_y_de_dest2 = pos.y - dest.y;
            //float fact_profondeur_12 = MathF.Pow((z_init - profondeur) / (z_init - dest.z), 3);
            //float fact_profondeur_22 = MathF.Pow((z_init - (profondeur + 1)) / (z_init - dest.z), 3);

            //return new float[4]
            //{
            //        dist_x_de_dest2 * fact_profondeur_12 + dest.x,
            //        dist_y_de_dest2 * fact_profondeur_12 + dest.y,
            //        dist_x_de_dest2 * fact_profondeur_22 + dest.x,
            //        dist_y_de_dest2 * fact_profondeur_22 + dest.y
            //};

            float dist_x_de_destination = pos.x - dest.x;
            float dist_y_de_destination = pos.y - dest.y;
            float facteur_profondeur_1 = MathF.Pow(VITESSE_PROJECTILE, profondeur);
            float facteur_profondeur_2 = MathF.Pow(VITESSE_PROJECTILE, profondeur + 1);

            return new float[4]
            {
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
        int ProjectileToucheJoueur()
        {
            const float MARGE_DE_MANOEUVRE = 0.75f;

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
            if (Vector2.Distance(
                positions_projectile[0],
                positions_projectile[1],
                Program.player.position.x,
                Program.player.position.y) > MARGE_DE_MANOEUVRE * Player.JOUEUR_LARGEUR)
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
            Son.ArreterMusique();
            Program.player.timer = 0;

            // code pour option continuer sur menu
            if (Program.curseur.curseur_max_selection < 2)
                Program.curseur.curseur_max_selection = 2;
            if (Program.Gamemode == Gamemode.GAMEPLAY)
                Program.nv_continue = Program.niveau;

            return 3;
        }

        // dessine le projectile à l'écran
        public override void RenderObjet()
        {
            if (Program.player.Mort())
                return;

            if (proprietaire == ProprietaireProjectile.ENNEMI)
            {
                SDL_SetRenderDrawColor(Program.render, 255, 0, 0, 255);
            }
            else
            {
                switch (Program.player.powerup)
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
            }

            float[] positions;

            if (laser)
            {
                // à chaque image, attacher le bout du laser au points de tir du joueur
                // TODO: ceci est la seule utilisation de ID, trouver comment l'enlever
                positions = Program.player.RenderDataLigne(Program.player.indexs_de_tir[ID % Program.player.indexs_de_tir.Length]);
                position.x = positions[0];
                position.y = positions[1];

                for (byte i = 0; i < Program.G_MAX_DEPTH; i++)
                {
                    const int LARGEUR_MAX_LASER = 5;

                    // cette formule ne fonctionne seulement pour la direction +Z, qui est celle du joueur
                    int largeur_laser = (int)(((Program.G_MAX_DEPTH - i) / (float)Program.G_MAX_DEPTH) * LARGEUR_MAX_LASER);

                    positions = PositionsSurEcran(i);
                    SDL_RenderDrawLineF(Program.render,
                        positions[0] + Program.RNG.Next(-largeur_laser, largeur_laser),
                        positions[1] + Program.RNG.Next(-largeur_laser, largeur_laser),
                        positions[2] + Program.RNG.Next(-largeur_laser, largeur_laser),
                        positions[3] + Program.RNG.Next(-largeur_laser, largeur_laser)
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

    // classe pour les vagues électriques que le joueur peux utiliser si il y a
    // des ennemis à coté de lui
    public static class VagueElectrique
    {
        const int LARGEUR_MAX_VAGUE_ELECTRIQUE = 80;
        const int LARGEUR_MIN_VAGUE_ELECTRIQUE = 50;
        const int PRECISION_VAGUE_ELECTRIQUE = 50;
        const int TEMPS_AVANT_VAGUE_FAIT_DEGATS = Program.G_FPS / 6;
        const int DURATION_VAGUE_ELECTRIQUE = 1 * Program.G_FPS;

        static float rayon = 0;
        static bool actif = false;
        static uint timer;

        // affiche une nouvelle vague electrique si possible
        public static void EssayerCreation()
        {
            if (actif || Program.player.Mort() || Program.player.vagues < 1.0f)
                return;

            Program.player.vagues -= 1.0f;
            timer = 0;
            rayon = 0;
            actif = true;
            Son.JouerEffet(ListeAudioEffets.VAGUE);
        }

        // logique pour la vague électrique
        public static void Exist()
        {
            if (!actif)
                return;

            rayon = (1 - MathF.Pow(0.90f, timer)) * LARGEUR_MAX_VAGUE_ELECTRIQUE + LARGEUR_MIN_VAGUE_ELECTRIQUE;

            timer++;
            if (timer > DURATION_VAGUE_ELECTRIQUE)
                actif = false;

            // la vague est seulement capable de faire des dégats après 10 images, et ensuite une fois au trois images
            if (timer < TEMPS_AVANT_VAGUE_FAIT_DEGATS || timer % 3 != 0)
                return;

            for (int i = 0; i < Program.enemies.Count; i++)
            {
                if (Program.enemies[i].position.z != 0)
                    continue;

                if (Vector2.Distance(
                    Program.enemies[i].position.x,
                    Program.enemies[i].position.y,
                    Program.player.position.x,
                    Program.player.position.y) <= rayon)
                {
                    Program.enemies[i].HP--;
                }

                // si l'ennemi meurt
                if (Program.enemies[i].HP <= 0)
                {
                    Program.ennemis_tues++;
                    Program.enemies[i].afficher = false;
                    Program.enemies.RemoveAt(i);
                }
            }
        }

        // dessine la vague électrique à l'écran si besoin
        public static void Render()
        {
            if (!actif)
                return;

            SDL_SetRenderDrawColor(Program.render, 0, 255, 255, 255);
            float angle = MathF.PI / (PRECISION_VAGUE_ELECTRIQUE / 2.0f);

            const int NOMBRE_DE_CERCLES = 3;
            for (int i = 0; i < NOMBRE_DE_CERCLES; i++)
            {
                float rand;
                float angle_pos;
                Vector2 new_pos;
                Vector2 prev_pos = new Vector2(
                    Program.player.position.x,
                    Program.player.position.y + rayon
                );

                for (float j = 0; j < PRECISION_VAGUE_ELECTRIQUE; j++)
                {
                    rand = Program.RNG.Next(-20, 20) + rayon;
                    angle_pos = (j + 1) * angle;

                    new_pos = new Vector2(
                        Program.player.position.x + rand * MathF.Sin(angle_pos),
                        Program.player.position.y + rand * MathF.Cos(angle_pos)
                    );

                    // pourque le cercle se connecte à la fin
                    if (j == PRECISION_VAGUE_ELECTRIQUE - 1)
                        new_pos = new Vector2(Program.player.position.x, Program.player.position.y + rayon);

                    SDL_RenderDrawLineF(Program.render,
                        prev_pos.x,
                        prev_pos.y,
                        new_pos.x,
                        new_pos.y
                    );

                    // en se rapellant de la dernière position, le cercle est continu et ne saute pas à chaque ligne.
                    prev_pos = new Vector2(
                        new_pos.x,
                        new_pos.y
                    );
                }
            }
        }
    }
}