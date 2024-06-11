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
    public class Ennemi : Sprite
    {
        // À MODIFIER SI ENNEMIS AJOUTÉS!!! Enum.GetNames n'est pas const, alors il ne peut pas être utilisé
        const int NB_TYPES_ENNEMIS = 17;
        const int BOSS_MAX_HP = 150;
        const int DISTANCE_DE_BORD_EVITER_INIT = 200;
        const float VITESSE_MOYENNE_ENNEMI = 0.4f;
        const float VITESSE_MOYENNE_Z_ENNEMI = 2.5f;
        const float VITESSE_MOYENNE_TIR_ENNEMI = 0.2f;
        const float ENNEMI_FRICTION = 0.8f;

        // Data pour ennemi, est seulement utilisé pour le dictionnaire dessous
        private struct EnnemiData
        {
            public float vitesse;
            public float vitesse_z;
            public float vitesse_tir;
            public int hp_max;
            public int largeur;
            public SDL_Color couleure;
            public int[] indexs_tir;
            public Vector3[] modele;
            public int[] modele_sauts;
        }

        // modèles ennemis
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
        static readonly Vector3[] MODELE_E7_1 =
        {
            new( 0, 10, 0 ),
            new( -8, -6, 0 ),
            new( 8, -6, 0 ),
            new( 0, 10, 0 )
        };
        static readonly int[] MODELE_E7_1_SAUTS = { -1 };
        //public static readonly Vector3[][] modeles_ennemis =
        //{
        //    MODELE_E1, MODELE_E2, MODELE_E3, new Vector3[]{ }, MODELE_E5,
        //    MODELE_E6, MODELE_E7, MODELE_E1, MODELE_E2, MODELE_E3,
        //    new Vector3[]{ }, MODELE_E5, MODELE_E6, MODELE_E7, Player.MODELE_P,
        //    MODELE_E7_1, MODELE_E7_1
        //};
        //public static readonly int[][] lignes_a_sauter_ennemis =
        //{
        //    MODELE_E1_SAUTS, MODELE_E2_SAUTS, MODELE_E3_SAUTS, new int[1]{-1}, MODELE_E5_SAUTS,
        //    MODELE_E6_SAUTS, MODELE_E7_SAUTS, MODELE_E1_SAUTS, MODELE_E2_SAUTS, MODELE_E3_SAUTS,
        //    new int[1]{-1}, MODELE_E5_SAUTS, MODELE_E6_SAUTS, MODELE_E7_SAUTS, Player.MODELE_P_SAUTS,
        //    MODELE_E7_1_SAUTS, MODELE_E7_1_SAUTS
        //};

        // le data pour tout les ennemis dans le jeu. Les ennemis sont trops simmilaires
        // pour que ca vaut la peine de créér des classes individuelles pour chaque.
        static readonly Dictionary<TypeEnnemi, EnnemiData> DataEnnemi = new()
        {
            { TypeEnnemi.OCTAHEDRON, new EnnemiData()
            {
                vitesse = VITESSE_MOYENNE_ENNEMI / 8,
                vitesse_z = VITESSE_MOYENNE_Z_ENNEMI,
                vitesse_tir = 0,
                hp_max = 1,
                largeur = 30,
                couleure = Program.RGBAtoSDLColor(0xFFFF00FF),
                indexs_tir = new int[] {18},
                modele = MODELE_E1,
                modele_sauts = MODELE_E1_SAUTS
            }},

            { TypeEnnemi.DIAMANT, new EnnemiData()
            {
                vitesse = VITESSE_MOYENNE_ENNEMI / 2,
                vitesse_z = VITESSE_MOYENNE_Z_ENNEMI,
                vitesse_tir = 2 * VITESSE_MOYENNE_TIR_ENNEMI,
                hp_max = 3,
                largeur = 30,
                couleure = Program.RGBAtoSDLColor(0xFF7F00FF),
                indexs_tir = new int[] {61, 45},
                modele = MODELE_E2,
                modele_sauts = MODELE_E2_SAUTS
            }},

            { TypeEnnemi.TOURNANT, new EnnemiData()
            {
                vitesse = VITESSE_MOYENNE_ENNEMI / 2,
                vitesse_z = VITESSE_MOYENNE_Z_ENNEMI,
                vitesse_tir = 2 * VITESSE_MOYENNE_TIR_ENNEMI,
                hp_max = 3,
                largeur = 50,
                couleure = Program.RGBAtoSDLColor(0x00FF00FF),
                indexs_tir = new int[] {0, 8},
                modele = MODELE_E3,
                modele_sauts = MODELE_E3_SAUTS
            }},

            { TypeEnnemi.ENERGIE, new EnnemiData()
            {
                vitesse = 0,
                vitesse_z = VITESSE_MOYENNE_Z_ENNEMI / 1.5f,
                vitesse_tir = 0,
                hp_max = 10,
                largeur = 50,
                couleure = Program.RGBAtoSDLColor(0x00FFFFFF),
                indexs_tir = Array.Empty<int>(),
                modele = Array.Empty<Vector3>(),
                modele_sauts = new int[]{-1}
            }},

            { TypeEnnemi.CROISSANT, new EnnemiData()
            {
                vitesse = VITESSE_MOYENNE_ENNEMI * 0.75f,
                vitesse_z = VITESSE_MOYENNE_Z_ENNEMI,
                vitesse_tir = 4 * VITESSE_MOYENNE_TIR_ENNEMI,
                hp_max = 10,
                largeur = 60,
                couleure = Program.RGBAtoSDLColor(0x0000FFFF),
                indexs_tir = new int[] {2, 21},
                modele = MODELE_E5,
                modele_sauts = MODELE_E5_SAUTS
            }},

            { TypeEnnemi.DUPLIQUEUR, new EnnemiData()
            {
                vitesse = VITESSE_MOYENNE_ENNEMI / 2,
                vitesse_z = VITESSE_MOYENNE_Z_ENNEMI / 2,
                vitesse_tir = 2 * VITESSE_MOYENNE_TIR_ENNEMI,
                hp_max = 7,
                largeur = 40,
                couleure = Program.RGBAtoSDLColor(0x7F007FFF),
                indexs_tir = new int[] {3},
                modele = MODELE_E6,
                modele_sauts = MODELE_E6_SAUTS
            }},

            { TypeEnnemi.PATRA, new EnnemiData()
            {
                vitesse = VITESSE_MOYENNE_ENNEMI * 1,
                vitesse_z = 0,
                vitesse_tir = 3 * VITESSE_MOYENNE_TIR_ENNEMI,
                hp_max = 10,
                largeur = 100,
                couleure = Program.RGBAtoSDLColor(0xFF00FFFF),
                indexs_tir = new int[] {5},
                modele = MODELE_E7,
                modele_sauts = MODELE_E7_SAUTS
            }},

            { TypeEnnemi.OCTAHEDRON_DUR, new EnnemiData()
            {
                vitesse = VITESSE_MOYENNE_ENNEMI * 0.8f,
                vitesse_z = VITESSE_MOYENNE_Z_ENNEMI,
                vitesse_tir = 1 * VITESSE_MOYENNE_TIR_ENNEMI,
                hp_max = 3,
                largeur = 30,
                couleure = Program.RGBAtoSDLColor(0x7F7F00FF),
                indexs_tir = new int[] {18},
                modele = MODELE_E1,
                modele_sauts = MODELE_E1_SAUTS
            }},

            { TypeEnnemi.DIAMANT_DUR, new EnnemiData()
            {
                vitesse = VITESSE_MOYENNE_ENNEMI * 1.0f,
                vitesse_z = VITESSE_MOYENNE_Z_ENNEMI,
                vitesse_tir = 2 * VITESSE_MOYENNE_TIR_ENNEMI,
                hp_max = 5,
                largeur = 30,
                couleure = Program.RGBAtoSDLColor(0x7F4000FF),
                indexs_tir = new int[] {61, 45},
                modele = MODELE_E2,
                modele_sauts = MODELE_E2_SAUTS
            }},

            { TypeEnnemi.TOURNANT_DUR, new EnnemiData()
            {
                vitesse = VITESSE_MOYENNE_ENNEMI * 1.0f,
                vitesse_z = VITESSE_MOYENNE_Z_ENNEMI,
                vitesse_tir = 2 * VITESSE_MOYENNE_TIR_ENNEMI,
                hp_max = 7,
                largeur = 50,
                couleure = Program.RGBAtoSDLColor(0x007F00FF),
                indexs_tir = new int[] {0, 8},
                modele = MODELE_E3,
                modele_sauts = MODELE_E3_SAUTS
            }},

            { TypeEnnemi.ENERGIE_DUR, new EnnemiData()
            {
                vitesse = VITESSE_MOYENNE_ENNEMI / 2,
                vitesse_z = VITESSE_MOYENNE_Z_ENNEMI * 1.5f,
                vitesse_tir = 1 * VITESSE_MOYENNE_TIR_ENNEMI,
                hp_max = 15,
                largeur = 50,
                couleure = Program.RGBAtoSDLColor(0x007F7FFF),
                indexs_tir = Array.Empty<int>(),
                modele = Array.Empty<Vector3>(),
                modele_sauts = new int[]{-1}
            }},

            { TypeEnnemi.CROISSANT_DUR, new EnnemiData()
            {
                vitesse = VITESSE_MOYENNE_ENNEMI * 1.0f,
                vitesse_z = VITESSE_MOYENNE_Z_ENNEMI,
                vitesse_tir = 3 * VITESSE_MOYENNE_TIR_ENNEMI,
                hp_max = 12,
                largeur = 60,
                couleure = Program.RGBAtoSDLColor(0x00007FFF),
                indexs_tir = new int[] {2, 21},
                modele = MODELE_E5,
                modele_sauts = MODELE_E5_SAUTS
            }},

            { TypeEnnemi.DUPLIQUEUR_DUR, new EnnemiData()
            {
                vitesse = VITESSE_MOYENNE_ENNEMI * 1.25f,
                vitesse_z = VITESSE_MOYENNE_Z_ENNEMI / 4,
                vitesse_tir = 1 * VITESSE_MOYENNE_TIR_ENNEMI,
                hp_max = 10,
                largeur = 40,
                couleure = Program.RGBAtoSDLColor(0x400080FF),
                indexs_tir = new int[] {3},
                modele = MODELE_E6,
                modele_sauts = MODELE_E6_SAUTS
            }},

            { TypeEnnemi.PATRA_DUR, new EnnemiData()
            {
                vitesse = VITESSE_MOYENNE_ENNEMI,
                vitesse_z = VITESSE_MOYENNE_Z_ENNEMI / 4,
                vitesse_tir = 4 * VITESSE_MOYENNE_TIR_ENNEMI,
                hp_max = 15,
                largeur = 100,
                couleure = Program.RGBAtoSDLColor(0x7F007FFF),
                indexs_tir = new int[] {5},
                modele = MODELE_E7,
                modele_sauts = MODELE_E7_SAUTS
            }},

            { TypeEnnemi.BOSS, new EnnemiData()
            {
                vitesse = VITESSE_MOYENNE_ENNEMI * 4f,
                vitesse_z = 0,
                vitesse_tir = 3f * VITESSE_MOYENNE_TIR_ENNEMI,
                hp_max = BOSS_MAX_HP,
                largeur = 100,
                couleure = Program.RGBAtoSDLColor(0xFF0000FF),
                indexs_tir = new int[] {1, 16},
                modele = Player.MODELE_P,
                modele_sauts = Player.MODELE_P_SAUTS
            }},

            { TypeEnnemi.PATRA_MINION, new EnnemiData()
            {
                vitesse = VITESSE_MOYENNE_ENNEMI,
                vitesse_z = VITESSE_MOYENNE_Z_ENNEMI * 1.0f,
                vitesse_tir = 0,
                hp_max = 1,
                largeur = 30,
                couleure = Program.RGBAtoSDLColor(0xFF00FFFF),
                indexs_tir = Array.Empty<int>(),
                modele = MODELE_E7_1,
                modele_sauts = MODELE_E7_1_SAUTS
            }},

            { TypeEnnemi.PATRA_MINION_DUR, new EnnemiData()
            {
                vitesse = VITESSE_MOYENNE_ENNEMI,
                vitesse_z = VITESSE_MOYENNE_Z_ENNEMI * 1.5f,
                vitesse_tir = 0,
                hp_max = 1,
                largeur = 30,
                couleure = Program.RGBAtoSDLColor(0x7F007FFF),
                indexs_tir = Array.Empty<int>(),
                modele = MODELE_E7_1,
                modele_sauts = MODELE_E7_1_SAUTS
            }},
        };

        public int largeur;
        public int HP;

        float intervale_tir;
        float vitesse;
        float vitesse_z;

        public TypeEnnemi type;
        public StatusEnnemi statut = StatusEnnemi.VIDE;
        Vector2 velocite;
        Vector2 cible;
        // on a besoin d'un deuxième tableau pour le modèle pour pouvoir envoyer à la
        // méthode render, car le tableau modele est copié poar valeure, et on veut
        // que chaque ennemi aie un modèle qui bouge différent des autres
        Vector3[] modele_en_marche = Array.Empty<Vector3>();

        public Ennemi(TypeEnnemi type, StatusEnnemi statut, Ennemi? parent = null)
        {
            this.type = type;
            this.statut = statut;
            velocite = new Vector2();
            cible = new Vector2(Program.player.position.x, Program.player.position.y);

            if (!DataEnnemi.TryGetValue(type, out EnnemiData data))
                return;

            largeur = data.largeur;
            HP = data.hp_max;
            vitesse = data.vitesse;
            vitesse_z = data.vitesse_z;
            indexs_de_tir = data.indexs_tir;
            intervale_tir = Program.G_FPS / data.vitesse_tir;//TODO: enlever gfps
            couleure = data.couleure;
            modele = data.modele;
            indexs_lignes_sauter = data.modele_sauts;

            // copie le tableau par valeur au lieu de par reference
            modele_en_marche = modele.ToArray();

            afficher = true;
            taille = 1;

            // la variable parent permet de créér l'ennemi à la position d'un autre
            if (parent != null)
            {
                position = parent.position;
            }
            else
            {
                position = new Vector3(
                    Program.RNG.Next(DISTANCE_DE_BORD_EVITER_INIT, Program.W_LARGEUR - DISTANCE_DE_BORD_EVITER_INIT),
                    Program.RNG.Next(DISTANCE_DE_BORD_EVITER_INIT, Program.W_HAUTEUR - DISTANCE_DE_BORD_EVITER_INIT),
                    Program.G_MAX_DEPTH
                );
            }

            // assignation de statuts
            if (type == TypeEnnemi.BOSS)
            {
                this.statut = StatusEnnemi.BOSS_INIT;
                // ce code est normalement éxecuté dans INIT1 après 1 image, mais on veut que le boss apparaîsse au fond immédiatement
                position.z = 99;
            }
            else if (type == TypeEnnemi.PATRA || type == TypeEnnemi.PATRA_DUR)
            {
                this.statut = StatusEnnemi.PATRA_8_RESTANT;
            }
            else if (type == TypeEnnemi.DUPLIQUEUR || type == TypeEnnemi.DUPLIQUEUR_DUR)
            {
                switch (statut)
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
                        this.statut = StatusEnnemi.DUPLIQUEUR_2_RESTANT;
                        break;
                }
            }
            // pas else, car l'ennemi peut être initializé avec un statut autre que init, et c'est important de le garder
            else if (statut == StatusEnnemi.INITIALIZATION)
            {
                this.statut = StatusEnnemi.NORMAL;
            }

            Program.enemies.Add(this);
        }

        // logique ennemi. retourne vrai si mort.
        public override bool Exist()
        {
            timer++;

            if (Program.player.Mort())
            {
                // c'est cool si les ennemis bougent leurs modèlent même quand le joueur est mort,
                // mais je ne peux pas mettre ActualiserModele comme étant la premier fonction dans la logique,
                // car certains ennemis utilisent la position de l'ennemi pour leur calcul, qui va changer quand Bouger est appelé
                ActualiserModele();
                return false;
            }

            // si boss
            if (CodeBoss())
            {
                return false;
            }

            Bouger();

            ActualiserModele();

            cible = TrouverCible();

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

        // certains ennemis ont des modèles qui bougent. cette méthode est en charge de les bouger
        public void ActualiserModele()
        {
            float speed = 0;
            float angle;
            SDL_SetRenderDrawColor(Program.render, couleure.r, couleure.g, couleure.b, couleure.a);

            switch (type)
            {
                case TypeEnnemi.OCTAHEDRON_DUR:
                    speed = 50.0f;
                    goto case TypeEnnemi.OCTAHEDRON;
                case TypeEnnemi.OCTAHEDRON:
                    if (speed == 0)
                        speed = 20.0f;

                    angle = timer / speed;

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
                    break;

                case TypeEnnemi.TOURNANT_DUR:
                    speed = 50.0f;
                    goto case TypeEnnemi.TOURNANT;
                case TypeEnnemi.TOURNANT:
                    if (speed == 0)
                        speed = 20.0f;

                    angle = timer / speed;
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
                    break;

                case TypeEnnemi.CROISSANT:
                    speed = 20.0f;
                    goto case TypeEnnemi.CROISSANT_DUR;
                case TypeEnnemi.CROISSANT_DUR:
                    if (speed == 0)
                        speed = 50.0f;

                    angle = timer / speed;

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
                    break;

                // code plutôt compliqué qui dessine un nombre X de triangles qui tournent sens horraire,
                // où tous les triangles ensemble tournent sens horraire autour de l'ennemi.
                // comme on le voit dans le nom, l'ennemi est inspiré de Patra de Zelda sur NES
                case TypeEnnemi.PATRA:
                case TypeEnnemi.PATRA_DUR:

                    // angle de départ pour le cercle de triangles
                    angle = (timer / 20f) % MathF.Tau;
                    float profondeure = MathF.Pow(0.95f, position.z);
                    float angle_prochain = MathF.Tau / 3;

                    // pour chaque triangle restant sur 8
                    for (int i = 0; i < (int)statut - (int)StatusEnnemi.PATRA_0_RESTANT; i++)
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
                    break;
            }
        }

        // gère les animations du boss.
        // retourne vrai si le boss est en train de faire une animation
        bool CodeBoss()
        {
            if (type != TypeEnnemi.BOSS || statut == StatusEnnemi.BOSS_NORMAL)
            {
                return false;
            }

            switch (statut)
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
                        statut = StatusEnnemi.BOSS_MORT_2;
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
                        statut = StatusEnnemi.BOSS_MORT_3;

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
                    statut = StatusEnnemi.BOSS_INIT_2;

                    break;

                // partie #2 de l'animation d'initialization du boss: le boss vole vers l'écran jusqu'à z=20
                case StatusEnnemi.BOSS_INIT_2:

                    position.z = 100 - timer;

                    if (position.z <= 20)
                    {
                        statut = StatusEnnemi.BOSS_INIT_3;
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
                        statut = StatusEnnemi.BOSS_NORMAL;
                        BombePulsar.HP_bombe = BombePulsar.BOMBE_PULSAR_MAX_HP;
                        Son.JouerMusique(ListeAudioMusique.DOTV, true);
                    }

                    break;
            }

            return true;
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

            // code pour patra
            if (statut > StatusEnnemi.PATRA_0_RESTANT && statut <= StatusEnnemi.PATRA_8_RESTANT)
            {
                HP++;
                statut--;

                new Ennemi(type == TypeEnnemi.PATRA ? TypeEnnemi.PATRA_MINION : TypeEnnemi.PATRA_MINION_DUR, StatusEnnemi.INITIALIZATION, this);
            }

            // le reste du code est pour ceux qui viennent de mourrir
            if (HP > 0)
            {
                return 0;
            }

            if (type == TypeEnnemi.BOSS)
            {
                statut = StatusEnnemi.BOSS_MORT;
                return 1;
            }

            Program.enemies.Remove(this);

            if (type == TypeEnnemi.PATRA_MINION || type == TypeEnnemi.PATRA_MINION_DUR)
            {
                statut = StatusEnnemi.MORT;
                return 1;
            }

            // code pour dupliqueur avec des dupliquations restantes
            if (statut > StatusEnnemi.DUPLIQUEUR_0_RESTANT && statut <= StatusEnnemi.DUPLIQUEUR_2_RESTANT)
            {
                // déplace le parent pourque les enfants soient séparés physiquement
                const int DUPLIQUEUR_DISTANCE_SEPARATION_ENFANT = 30;
                StatusEnnemi nouveau_status = statut - 1;

                position.x += DUPLIQUEUR_DISTANCE_SEPARATION_ENFANT;
                new Ennemi(type, nouveau_status, this);
                position.x -= DUPLIQUEUR_DISTANCE_SEPARATION_ENFANT * 2;
                new Ennemi(type, nouveau_status, this);

                statut = StatusEnnemi.MORT;
                return 1;
            }

            // code pour les ennemis normaux qui peuvent lâcher un item
            statut = StatusEnnemi.MORT;
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

            // ces ennemis ci-dessous ne comptent pas pour le total, alors on demande un nouveau ennemi si ce n'est pas un ci-dessous qui vient de mourrir
            if (!// <---- NOT!!!
                (type == TypeEnnemi.PATRA_MINION
                || type == TypeEnnemi.PATRA_MINION_DUR
                || (statut >= StatusEnnemi.DUPLIQUEUR_0_RESTANT && statut <= StatusEnnemi.DUPLIQUEUR_1_RESTANT)
                || (statut >= StatusEnnemi.PATRA_1_RESTANT && statut <= StatusEnnemi.PATRA_8_RESTANT))
                )
            {
                Program.ennemis_a_creer++;
            }

            // tue l'ennemi
            statut = StatusEnnemi.MORT;
            Program.enemies.Remove(this);
            return 1;
        }

        // retourne une nouvelle cible de position pour l'ennemi,
        // ou le même si il n'a pas besoin de changer.
        Vector2 TrouverCible()
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

            // seulement ces ennemis sont capables de tourner leur modèle
            if (type == TypeEnnemi.DUPLIQUEUR || type == TypeEnnemi.DUPLIQUEUR_DUR || type == TypeEnnemi.BOSS)
                roll = velocite.x / 15;

            // fait que le boss (qui a le modèle du joueur) paraît normal
            if (type == TypeEnnemi.BOSS)
                pitch -= 0.5f;
        }

        // vérifie si l'ennemi peux tirer, et tire au besoin
        void Tirer()
        {
            if (intervale_tir == 0)
                return;

            if (timer % intervale_tir >= 1)
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

                Vector3 cible = Program.player.position;
                // ce code fait que la cible est non où le joueur est, mais où il VA être.
                // bien sûr, donner ceci à tous les ennemis rendrait le jeu beaucoup trop dur, alors c'est seulement pour le boss.
                if (type == TypeEnnemi.BOSS)
                {
                    cible = new Vector3(
                        Program.player.position.x + Program.player.velocite.x * position.z,
                        Program.player.position.y + Program.player.velocite.y * position.z, 
                        Program.player.position.z
                    );
                }

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
            // les ennemis énergis sont les seuls à ne pas utiliser de modèle
            if (type is (TypeEnnemi.ENERGIE or TypeEnnemi.ENERGIE_DUR))
            {
                BombePulsar.DessinerBombePulsar(
                    new Vector2(position.x, position.y),
                    (byte)(40 * MathF.Pow(0.95f, position.z)),
                    couleure,
                    false
                );

                return;
            }

            base.RenderObject(modele_en_marche);
        }
    }
}