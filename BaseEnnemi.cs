using static SDL2.SDL;

namespace Dysgenesis
{
    // enum pour tout les types d'ennemis dans le jeu.
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

    // enum pour tout les etats possible pour un ennemi dans le jeu.
    // certains ennemis sont spéciaux et gardent leur data extra dans leur statut
    public enum StatusEnnemi
    {
        // statuts par défaut
        VIDE = 0,
        INITIALIZATION,
        NORMAL,
        MORT,

        // statuts pour DUPLIQUEUR et DUPLIQUEUR_DUR, leur nb. de duplications restantes
        DUPLIQUEUR_0_RESTANT = 60,
        DUPLIQUEUR_1_RESTANT,
        DUPLIQUEUR_2_RESTANT,

        // statuts pour PATRA et PATRA_DUR, leur nb. de minions restant autour d'eux
        PATRA_0_RESTANT = 70,
        PATRA_1_RESTANT,
        PATRA_2_RESTANT,
        PATRA_3_RESTANT,
        PATRA_4_RESTANT,
        PATRA_5_RESTANT,
        PATRA_6_RESTANT,
        PATRA_7_RESTANT,
        PATRA_8_RESTANT,

        // statuts pour le BOSS. Ceci est pour les scènes avant et après son arrivée.
        BOSS_INIT = 150,
        BOSS_INIT_2,
        BOSS_INIT_3,
        BOSS_NORMAL,
        BOSS_MORT,
        BOSS_MORT_2,
        BOSS_MORT_3,
    }

    // classe ennemi
    public abstract class Ennemi : Sprite
    {
        const int DISTANCE_DE_BORD_EVITER_INIT = 200;
        protected const float VITESSE_MOYENNE_ENNEMI = 0.4f;
        protected const float VITESSE_MOYENNE_Z_ENNEMI = 2.5f;
        protected const float VITESSE_MOYENNE_TIR_ENNEMI = 0.2f;
        const float ENNEMI_FRICTION = 0.8f;

        public int largeur;
        public int HP;

        protected float vitesse_tir;
        protected float vitesse;
        protected float vitesse_z;

        public bool version_dur { get; protected set; }

        public StatusEnnemi status = StatusEnnemi.VIDE;
        protected Vector2 velocite;
        protected Vector2 cible;
        // on a besoin d'un deuxième tableau pour le modèle pour pouvoir envoyer à la
        // méthode render, car le tableau modele est copié poar valeure, et on veut
        // que chaque ennemi aie un modèle qui bouge différent des autres
        protected Vector3[] modele_en_marche = Array.Empty<Vector3>();

        public Ennemi(Vector3[] modele, int[] modele_sauts, SDL_Color couleure, int HP, float vitesse, float vitesse_z, int largeur_hitbox, float intervale_tir, int[] indexs_de_tir)
        {
            status = StatusEnnemi.INITIALIZATION;
            velocite = new Vector2();
            cible = Program.player.position;

            this.largeur = largeur_hitbox;
            this.HP = HP;
            this.vitesse = vitesse;
            this.vitesse_z = vitesse_z;
            this.indexs_de_tir = indexs_de_tir;
            this.vitesse_tir = Program.G_FPS / intervale_tir;//TODO: enlever gfps
            this.couleure = couleure;
            this.modele = modele;
            this.indexs_lignes_sauter = modele_sauts;

            // copie le tableau par valeur au lieu de par reference
            modele_en_marche = modele.ToArray();

            afficher = true;
            taille = 1;

            // les constructeurs enfants peuvent mettre une valeure non au hasard si ils veulent.
            position = new Vector3(
                Program.RNG.Next(DISTANCE_DE_BORD_EVITER_INIT, Program.W_LARGEUR - DISTANCE_DE_BORD_EVITER_INIT),
                Program.RNG.Next(DISTANCE_DE_BORD_EVITER_INIT, Program.W_HAUTEUR - DISTANCE_DE_BORD_EVITER_INIT),
                Program.G_MAX_DEPTH
            );

            // l'ennemi peut être initializé avec un statut autre que init, et c'est important de le garder
            if (status == StatusEnnemi.INITIALIZATION)
            {
                this.status = StatusEnnemi.NORMAL;
            }

            Program.enemies.Add(this);
        }

        // appelle le bon constructeur pour créér un ennemi.
        public static void CreerEnnemi(TypeEnnemi type)
        {
            switch (type)
            {
                case TypeEnnemi.OCTAHEDRON:
                    new Octahedron(false);
                    break;
                case TypeEnnemi.DIAMANT:
                    new Diamant(false);
                    break;
                case TypeEnnemi.TOURNANT:
                    new Tournant(false);
                    break;
                case TypeEnnemi.ENERGIE:
                    new Energie(false);
                    break;
                case TypeEnnemi.CROISSANT:
                    new Croissant(false);
                    break;
                case TypeEnnemi.DUPLIQUEUR:
                    new Dupliqueur(false, null);
                    break;
                case TypeEnnemi.PATRA:
                    new Patra(false);
                    break;
                case TypeEnnemi.OCTAHEDRON_DUR:
                    new Octahedron(true);
                    break;
                case TypeEnnemi.DIAMANT_DUR:
                    new Diamant(true);
                    break;
                case TypeEnnemi.TOURNANT_DUR:
                    new Tournant(true);
                    break;
                case TypeEnnemi.ENERGIE_DUR:
                    new Energie(true);
                    break;
                case TypeEnnemi.CROISSANT_DUR:
                    new Croissant(true);
                    break;
                case TypeEnnemi.DUPLIQUEUR_DUR:
                    new Dupliqueur(true, null);
                    break;
                case TypeEnnemi.PATRA_DUR:
                    new Patra(true);
                    break;
                case TypeEnnemi.BOSS:
                    new Boss();
                    break;
                case TypeEnnemi.PATRA_MINION:
                    break;
                case TypeEnnemi.PATRA_MINION_DUR:
                    break;
            }
        }

        // logique ennemi. retourne vrai si mort.
        public override bool Exist()
        {
            timer++;

            if (Program.player.Mort())
            {
                // c'est cool si les ennemis bougent leurs modèlent même quand le joueur est mort,
                // mais je ne peux pas mettre AnimationModele comme étant la premier fonction dans la logique,
                // car certains ennemis utilisent la position de l'ennemi pour leur calcul, qui va changer quand Bouger est appelé
                AnimationModele();
                return false;
            }

            if (ActionsEnnemi())
            {
                return false;
            }

            Bouger();

            AnimationModele();

            cible = TrouverCibleMouvement();

            for (int i = 0; i < Program.projectiles.Count; i++)
            {
                if (ProjCollision(Program.projectiles[i]) != 0)
                    return true;
            }

            if (CollisionJoueur(Program.player) != 0)
            {
                return true;
            }

            Tirer();

            return false;
        }
        protected virtual bool ActionsEnnemi() { return false; }

        // certains ennemis ont des modèles qui bougent. cette méthode est en charge de les bouger
        public virtual void AnimationModele()
        {
            return;
        }

        // code de collision entre l'ennemi et un projectile.
        // retourne 1 si l'ennemi se fait tuer par l'attaque, sinon 0
        int ProjCollision(Projectile projectile)
        {
            if (!afficher || projectile.proprietaire == ProprietaireProjectile.ENNEMI)
                return 0;

            // vérifie si le projectile est à la bonne profondeur pour frapper, mais on ignore la profondeur si le joueur a un laser
            if (MathF.Abs(projectile.position.z - position.z) > 2 && !projectile.laser)
                return 0;

            // si laser, ne vérifie qu'une fois au 10 images
            if (projectile.laser && Program.gTimer % 10 != 0)
                return 0;

            float[] proj_pos = projectile.PositionsSurEcran();

            if (Vector2.Distance(position.x, position.y, proj_pos[0], proj_pos[1]) > largeur)
                return 0;

            // ennemi touché
            HP--;
            new Explosion(position);

            // désactive le projectile, sauf si laser
            if (!projectile.laser)
                Program.projectiles.Remove(projectile);

            QuandTouché(false);

            // le reste du code est pour ceux qui viennent de mourrir
            if (HP > 0)
            {
                return 0;
            }

            Program.enemies.Remove(this);

            if (QuandMort())
            {
                return 1;
            }

            // code pour les ennemis normaux qui peuvent lâcher un item
            status = StatusEnnemi.MORT;
            Program.ennemis_tues++;
            new Item(this);
            return 1;
        }

        // code pour détecter si un ennemi touche le joueur.
        // retourne 1 si touché, 0 sinon.
        int CollisionJoueur(Player player)
        {
            const int DOMMAGES_COLLISION_JOUEUR = 3;

            if (position.z != 0)
                return 0;

            if (Vector2.Distance(position.x, position.y, player.position.x, player.position.y) > Player.JOUEUR_LARGEUR)
                return 0;

            // touché
            new Explosion(position);
            player.HP -= DOMMAGES_COLLISION_JOUEUR;
            // l'animation de mort du joueur doit avoir le timer du joueur mis à 0
            if (player.Mort())
                player.timer = 0;

            // certains ennemis ne veulent pas augmenter le compte du nr d'ennemis qu'il reste à créér.
            // Si ils réduissent Program.ennemis_a_creer de 1 dans QuandTouché(true), ils veulent défaire la ligne d'après.
            QuandTouché(true);
            Program.ennemis_a_creer++;

            // tue l'ennemi
            status = StatusEnnemi.MORT;
            Program.enemies.Remove(this);
            return 1;
        }

        protected virtual void QuandTouché(bool collision_joueur) { return; }
        // appelé quand l'ennemi est tué avec des projectiles ou vague electrique. false = mort normale, true = interrompre le processus de mort.
        public virtual bool QuandMort() { return false; }

        // retourne une nouvelle cible de position pour l'ennemi,
        // ou le même si il n'a pas besoin de changer.
        Vector2 TrouverCibleMouvement()
        {
            const int BORD_EVITER = 100;
            const int BAS_EVITER = 400;
            // si la taille de l'écran ou qqc d'autre cause une boucle infini, on veut éviter cela
            const int LIMITE_BOUCLE = 500;
            int anti_boucle_infini = 0;

            // l'ennemi va directement au joueur quand il se rend à z=0
            if (position.z == 0)
            {
                return Program.player.position;
            }

            int dist_ennemi_cible = Vector2.Distance(cible.x, cible.y, position.x, position.y);

            // si l'ennemi n'est pas à sa cible, ne la change pas
            if (dist_ennemi_cible > 30)
            {
                return cible;
            }

            // si la résolution de la fenêtre est trop petite, abandonne. le joueur est déjà en misère.
            if (Program.W_LARGEUR <= BORD_EVITER * 2 || Program.W_HAUTEUR <= BORD_EVITER + BAS_EVITER)
                return new Vector2(Program.RNG.Next(Program.W_LARGEUR), Program.RNG.Next(Program.W_HAUTEUR));

            int dist_player_ennemi = Vector2.Distance(position.x, position.y, Program.player.position.x, Program.player.position.y);
            float nouveauX, nouveauY;

            // si l'ennemi est plutôt proche du joueur, nouvelle cible = loin du joueur
            if (dist_player_ennemi < 200)
            {
                do
                {
                    // ici et plus loin, on veut éviter le bas de l'écran, car le joueur ne peut pas tirer là
                    nouveauX = Program.RNG.Next(100, Program.W_LARGEUR - 100);
                    nouveauY = Program.RNG.Next(100, Program.W_HAUTEUR - 400);

                    if (++anti_boucle_infini > LIMITE_BOUCLE)
                        return new Vector2(Program.RNG.Next(Program.W_LARGEUR), Program.RNG.Next(Program.W_HAUTEUR));
                }
                while (Vector2.Distance(nouveauX, nouveauY, Program.player.position.x, Program.player.position.y) < 800);

                return new Vector2(nouveauX, nouveauY);
            }

            // si l'ennemi est pas trop proche du joueur, retourne la position du joueur
            if (dist_player_ennemi < 800)
            {
                float Yverif = Math.Clamp(Program.player.position.y, 0, Program.W_HAUTEUR - 400);
                return new Vector2(Program.player.position.x, Yverif);
            }

            // si l'ennemi est loin du joueur, nouvelle cible = plutôt proche du joueur
            do
            {
                nouveauX = Program.RNG.Next(100, Program.W_LARGEUR - 100);
                nouveauY = Program.RNG.Next(100, Program.W_HAUTEUR - 400);

                if (++anti_boucle_infini > LIMITE_BOUCLE)
                    return new Vector2(Program.RNG.Next(Program.W_LARGEUR), Program.RNG.Next(Program.W_HAUTEUR));

            }
            while (Vector2.Distance(nouveauX, nouveauY, Program.player.position.x, Program.player.position.y) > 800);

            return new Vector2(nouveauX, nouveauY);
        }
        protected virtual Vector3 TrouverCibleTir() => Program.player.position;

        // bouge l'ennemi accordément
        void Bouger()
        {
            const float LIM_MIN_Z_ENNEMI = 1.0f;
            const float ACCELERATION_ENNEMI_Z0 = 1.01f;
            const float MAX_PITCH = 0.25f;

            // mouvement avant/arrière
            if (vitesse_z != 0 && position.z != 0)
            {
                position.z -= vitesse_z / Program.G_FPS;

                if (position.z < LIM_MIN_Z_ENNEMI)
                    position.z = 0;
            }

            // mouvement haut/bas/gauche/droite
            if (vitesse != 0)
            {
                if (position.x < cible.x)
                    velocite.x += vitesse;
                else if (position.x > cible.x)
                    velocite.x -= vitesse;

                if (position.y < cible.y)
                    velocite.y += vitesse;
                else if (position.y > cible.y)
                    velocite.y -= vitesse;

                velocite.x *= ENNEMI_FRICTION;
                velocite.y *= ENNEMI_FRICTION;
                position.x += velocite.x;
                position.y += velocite.y;
            }

            // quand les ennemis sont à la profondeur du joueur, ils accélèrent exponentiellement pour
            // rapidement et assurément frapper le joueur
            if (position.z == 0)
                vitesse *= ACCELERATION_ENNEMI_Z0;

            // le pitch de l'ennemi dépend de sa position verticale sur l'écran, et peux aller de +0.25 à -0.25
            pitch = ((position.y - Program.W_SEMI_HAUTEUR) / Program.W_SEMI_HAUTEUR) * MAX_PITCH;
        }

        // vérifie si l'ennemi peux tirer, et tire au besoin
        void Tirer()
        {
            if (vitesse_tir == 0)
                return;

            if (timer % vitesse_tir >= 1)
                return;

            // les ennemis ne peuvent pas tirer dans le dernier quart de leur approche, car sinon
            // les projectiles arrivent au joueur trop vite et sont aussi détachés de leur cible.
            if (position.z < Program.G_MAX_DEPTH / 4)
                return;

            float[] data_ligne;
            for (byte i = 0; i < indexs_de_tir.Length; i++)
            {
                // indexs_de_tir = id de ligne du modèle auquel le bout est le point de tir
                data_ligne = RenderDataLigne(indexs_de_tir[i]);

                // si l'index de tir est invalide ou RenderDataLigne retourne une erreur, la position par défaut est le centre de l'ennemi.
                // sans cette vérification, la position retournée est de (0,0)
                if (data_ligne[0] == 0 && data_ligne[1] == 0)
                {
                    data_ligne[0] = position.x;
                    data_ligne[1] = position.y;
                }

                Vector3 cible = TrouverCibleTir();

                new Projectile(
                    new Vector3(data_ligne[0], data_ligne[1], position.z),
                    cible,
                    ProprietaireProjectile.ENNEMI,
                    i
                );
            }
        }

        // render avec le modèle capable de bouger au lieu de celui qui est statique
        public override void RenderObjet()
        {
            base.RenderObject(modele_en_marche);
        }
    }
}