using static SDL2.SDL;

namespace Dysgenesis
{
    // les types d'item que les ennemis peuvent laisser tomber. est aussi utilisé pour identifier quel item le joueur possède.
    public enum TypeItem
    {
        NONE,
        X2_SHOT,
        X3_SHOT,
        HOMING,
        SPREAD,
        LASER,
        HP,
        VAGUE
    };

    // Classe pour les items parfois laissés par les ennemis
    // contient le code pour créér l'item, le bouger, le dessiner et quand un joueur le tocuhe
    public class Item : Sprite
    {
        const int TIMER_DISPARITION_ITEM = 2 * Program.G_FPS;
        const int HP_BONUS = 10;
        const float FACTEUR_VITESSE_ITEM = 0.98f;
        const float POS_Z_COLLISION_JOUEUR = 2.0f;
        readonly int NB_TYPES_ITEM = Enum.GetNames(typeof(TypeItem)).Length;

        // modèles items
        static readonly Vector3[] MODELE_I1 =
        {
            new(-5, -10, 0 ),
            new(-15, -10, 0),
            new(-20, -5, 0),
            new(-5, -5, 0),
            new(-5, -20, 0),
            new(0, -25, 0),
            new(10, -25, 0),
            new(5, -20, 0),
            new(-5, -20, 0),
            new(5, -20, 0),
            new(5, -5, 0),
            new(20, -5, 0),
            new(25, -10, 0),
            new(10, -10, 0),
            new(10, -25, 0),
            new(10, -10, 0),
            new( 25, -10, 0),
            new( 25, 0, 0),
            new( 20, 5, 0),
            new( 20, -5, 0),
            new(20, 5, 0),
            new(5, 5, 0),
            new(5, 20, 0),
            new(10, 15, 0),
            new(10, 5, 0),
            new(10, 15, 0),
            new(5, 20, 0),
            new(-5, 20, 0),
            new(-5, 5, 0),
            new(-20, 5, 0),
            new(-20, -5, 0)
        };
        static readonly int[] MODELE_I1_SAUTS = { -1 };
        static readonly Vector3[] MODELE_I3 =
        {
            new(-19, 22,0),
            new(-70, 0,0),
            new(-19, -22 ,0),
            new(-19, 22 ,0),
            new(-30, 12 ,0),
            new(-30, -12 ,0),
            new(-12, -30 ,0),
            new(12, -30 ,0),
            new(19, -22 ,0),
            new(70, 0 ,0),
            new(19, 22 ,0),
            new(70, 0 ,0),
            new(19, -22 ,0),
            new(30, -12 ,0),
            new(30, 12 ,0),
            new(12, 30 ,0),
            new(-12, 30 ,0),
            new(-19, 22 ,0),
            new(-14, -6 ,0),
            new(-8, 6 ,0),
            new(-8, -6 ,0),
            new(-14, 6 ,0),
            new(-4, -12 ,0),
            new(10, -12 ,0),
            new(10, 0 ,0),
            new(-4, 0 ,0),
            new(-4, 12 ,0),
            new(10, 12 ,0)
        };
        static readonly int[] MODELE_I3_SAUTS = { 3, 18, 20, 22, -1 };
        static readonly Vector3[] MODELE_I4 =
        {
            new(19, 22 ,0),
            new(70, 0 ,0),
            new(19, -22 ,0),
            new(19, 22 ,0),
            new(30, 12 ,0),
            new(30, -12 ,0),
            new(12, -30 ,0),
            new(-12, -30 ,0),
            new(-19, -22 ,0),
            new(-70, 0 ,0),
            new(-19, 22 ,0),
            new(-70, 0 ,0),
            new(-19, -22 ,0),
            new(-30, -12 ,0),
            new(-30, 12 ,0),
            new(-12, 30 ,0),
            new(12, 30 ,0),
            new(19, 22 ,0),
            new(14, -6 ,0),
            new(8, 6 ,0),
            new(8, -6 ,0),
            new(14, 6 ,0),
            new(4, -12 ,0),
            new(-10, -12 ,0),
            new(-10, 0 ,0),
            new(4, 0 ,0),
            new(-10, 0 ,0),
            new(-10, 12 ,0),
            new(4, 12 ,0)
        };
        static readonly int[] MODELE_I4_SAUTS = { 3, 18, 20, 22, -1 };
        static readonly Vector3[] MODELE_I5 =
        {
            new(-19, 22 ,0),
            new(-70, 0 ,0),
            new(-19, -22 ,0),
            new(-70, 0 ,0),
            new(-19, 22 ,0),
            new(-30, 12 ,0),
            new(-30, -12 ,0),
            new(-12, -30 ,0),
            new(12, -30 ,0),
            new(19, -22 ,0),
            new(70, 0 ,0),
            new(19, 22 ,0),
            new(70, 0 ,0),
            new(19, -22 ,0),
            new(30, -12 ,0),
            new(30, 12 ,0),
            new(12, 30 ,0),
            new(-12, 30 ,0),
            new(-19, 22 ,0),
            new(-16, 0 ,0),
            new(-4, 0 ,0),
            new(0, -4 ,0),
            new(0, -16 ,0),
            new(16, 0 ,0),
            new(4, 0 ,0),
            new(0, 4 ,0),
            new(0, 16 ,0),
            new(0, 10 ,0),
            new(-10, 0 ,0),
            new(0, -10 ,0),
            new(10, 0 ,0),
            new(0, 10  ,0),
            new(0, 0, 0),
            new(0, 0, 0)
        };
        static readonly int[] MODELE_I5_SAUTS = { 19, 21, 23, 25, 32, -1 };
        static readonly Vector3[] MODELE_I6 =
        {
            new(-19, -22 ,0),
            new(-70, 0 ,0),
            new(-19, 22 ,0),
            new(-70, 0 ,0),
            new(-19, -22 ,0),
            new(-30, -12 ,0),
            new(-30, 12 ,0),
            new(-12, 30 ,0),
            new(12, 30 ,0),
            new(19, 22 ,0),
            new(70, 0 ,0),
            new(19, -22 ,0),
            new(70, 0 ,0),
            new(19, 22 ,0),
            new(30, 12 ,0),
            new(30, -12 ,0),
            new(12, -30 ,0),
            new(-12, -30 ,0),
            new(-19, -22 ,0),
            new(-16, 10 ,0),
            new(-10, -10 ,0),
            new(-4, 10 ,0),
            new(-10, 10 ,0),
            new(-10, -10 ,0),
            new(4, 10 ,0),
            new(10, -10 ,0),
            new(16, 10 ,0),
            new(10, 10 ,0),
            new(10, -10 , 0)
        };
        static readonly int[] MODELE_I6_SAUTS = { 19, 22, 24, 27, -1 };
        static readonly Vector3[] MODELE_I7 =
        {
            new(-19, 22 ,0),
            new(-70, 0 ,0),
            new(-19, -22 ,0),
            new(-70, 0 ,0),
            new(-19, 22 ,0),
            new(-30, 12 ,0),
            new(-30, -12 ,0),
            new(-12, -30 ,0),
            new(12, -30 ,0),
            new(19, -22 ,0),
            new(70, 0 ,0),
            new(19, 22 ,0),
            new(70, 0 ,0),
            new(19, -22 ,0),
            new(30, -12 ,0),
            new(30, 12 ,0),
            new(12, 30 ,0),
            new(-12, 30 ,0),
            new(-19, 22 ,0),
            new(-6, 20 ,0),
            new(6, 12 ,0),
            new(-6, 4 ,0),
            new(6, -4 ,0),
            new(-6, -12 ,0),
            new(6, -20 , 0)
        };
        static readonly int[] MODELE_I7_SAUTS = { 19, -1 };

        // data pour dictionnaire ci-dessous
        private struct ItemData
        {
            public SDL_Color couleure;
            public Vector3[] modele;
            public int[] modele_sauts;
        }

        // dictionnaire pour le data de chaque item
        static readonly Dictionary<TypeItem, ItemData> DataItem = new()
        {
            { TypeItem.HP, new ItemData()
            {
                couleure = Program.RGBAtoSDLColor(0xFF0000FF),
                modele = MODELE_I1,
                modele_sauts = MODELE_I1_SAUTS,
            }},

            { TypeItem.VAGUE, new ItemData()
            {
                couleure = Program.RGBAtoSDLColor(0x00FFFFFF),
                modele = Array.Empty<Vector3>(),
                modele_sauts = Array.Empty<int>(),
            }},

            { TypeItem.X2_SHOT, new ItemData()
            {
                couleure = Program.RGBAtoSDLColor(0xFF8000FF),
                modele = MODELE_I3,
                modele_sauts = MODELE_I3_SAUTS,
            }},

            { TypeItem.X3_SHOT, new ItemData()
            {
                couleure = Program.RGBAtoSDLColor(0xFFFF00FF),
                modele = MODELE_I4,
                modele_sauts = MODELE_I4_SAUTS,
            }},

            { TypeItem.HOMING, new ItemData()
            {
                couleure = Program.RGBAtoSDLColor(0x40FF40FF),
                modele = MODELE_I5,
                modele_sauts = MODELE_I5_SAUTS,
            }},

            { TypeItem.SPREAD, new ItemData()
            {
                couleure = Program.RGBAtoSDLColor(0x0000FFFF),
                modele = MODELE_I6,
                modele_sauts = MODELE_I6_SAUTS,
            }},

            { TypeItem.LASER, new ItemData()
            {
                couleure = Program.RGBAtoSDLColor(0x8000FFFF),
                modele = MODELE_I7,
                modele_sauts = MODELE_I7_SAUTS,
            }},
        };

        // le type de l'item.
        public TypeItem type;

        // appeler ce constructeur n'est pas pour créér un item, mais pour rouler la chance de créér un item, et le créér au besoin
        // parent est seulement utilisé pour la position, mais pourrait aussi être utilisé pour vérifier le type de l'ennemi
        public Item(Ennemi parent)
        {
            // nombre entre 0 et 100. le plus grand le nombre, le meilleur que l'item sera
            float nb_hasard = Program.RNG.Next(100);

            // les items doivent êtres plus rares en mode arcade.
            byte facteur = 30;
            if (Program.Gamemode == Gamemode.ARCADE)
                facteur = 40;

            // il est plus difficile d'avoir des items dans les premiers nivaux, et impossible d'avoir des forts
            nb_hasard -= facteur * MathF.Pow(0.8f, Program.niveau);

            // debug
            if (Program.items_gratuit)
                nb_hasard = 101;

            // si roll est moins que 80, aucun item ne vas apparaître
            if (nb_hasard < 80)
                return;

            type = TypeItem.NONE;
            //pitch = 1.0f;//TODO: nécéssaire?
            afficher = true;
            position = parent.position;

            // le plus grand que roll est, le meilleur l'item.
            // ces items sont ordonnées de façon pire au meilleur, trouvé en testant le jeu assez
            if (nb_hasard < 85)
                type = TypeItem.VAGUE;
            else if (nb_hasard < 90)
                type = TypeItem.HP;
            else if (nb_hasard < 92)
                type = TypeItem.HOMING;
            else if (nb_hasard < 94)
                type = TypeItem.X2_SHOT;
            else if (nb_hasard < 96)
                type = TypeItem.LASER;
            else if (nb_hasard < 98)
                type = TypeItem.SPREAD;
            else
                type = TypeItem.X3_SHOT;

            // le debug freeitems fait que tout les ennemis lachent un item au hasard
            if (Program.items_gratuit)
                type = (TypeItem)Program.RNG.Next(1, NB_TYPES_ITEM - 1);

            // si l'item choisi ne contient pas de data (ne devrait jamais arriver sauf si quelqu'un ajoute des nouveaux et oublie le data)
            if (!DataItem.TryGetValue(type, out ItemData data))
                return;

            couleure = data.couleure;
            modele = data.modele;
            indexs_lignes_sauter = data.modele_sauts;

            Program.items.Add(this);
        }

        // logique des items. il bougent et vérifient la collision.
        // retourne vrai si l'item est détruit
        public override bool Exist()
        {
            if (Bouger() != 0)
                return true;

            if (VerifierCollisionJoueur() != 0)
                return true;

            return false;
        }

        // retourne >0 si item détruit, 0 sinon
        // l'item bouge de manière exponentiellement lent vers l'écran, et dès que l'item peut être touché par le joueur, il disparaît après 2 secondes
        int Bouger()
        {
            position.z *= FACTEUR_VITESSE_ITEM;
            
            if (position.z > POS_Z_COLLISION_JOUEUR)
            {
                return 0;
            }

            if (timer > TIMER_DISPARITION_ITEM)
            {
                Program.items.Remove(this);
                return 1;
            }

            timer++;

            return 0;
        }

        // vérifie si le joueur touche l'item, et applique l'effet si oui.
        // retourne 1 si collision, 0 sinon
        int VerifierCollisionJoueur()
        {
            if (position.z > POS_Z_COLLISION_JOUEUR)
                return 0;

            if (Vector2.Distance(Program.player.position.x, Program.player.position.y, position.x, position.y) > Player.JOUEUR_LARGEUR)
            {
                return 0;
            }

            // collision

            switch (type)
            {
                case TypeItem.NONE:
                    break;
                case TypeItem.HP:
                    Program.player.HP += HP_BONUS;
                    break;
                case TypeItem.VAGUE:
                    Program.player.vagues += 1.0f;
                    break;
                case TypeItem.HOMING:
                case TypeItem.X2_SHOT:
                    Program.player.fire_rate = Player.JOUEUR_VITESSE_TIR / 2;
                    Program.player.powerup = type;
                    break;
                case TypeItem.X3_SHOT:
                    Program.player.fire_rate = Player.JOUEUR_VITESSE_TIR / 3;
                    Program.player.powerup = type;
                    break;
                case TypeItem.LASER:
                    Program.player.fire_rate = Player.JOUEUR_VITESSE_TIR * 2;
                    Program.player.powerup = type;
                    break;
                default:
                    Program.player.fire_rate = Player.JOUEUR_VITESSE_TIR;
                    Program.player.powerup = type;
                    break;
            }

            Program.items.Remove(this);
            Son.JouerEffet(ListeAudioEffets.POWERUP);
            return 1;
        }

        // dessine l'item à l'écran. la vague n'a pas de modèle, c'est juste 3 cercles.
        public override void RenderObjet()
        {
            if (type == TypeItem.VAGUE)
            {
                SDL_SetRenderDrawColor(Program.render, couleure.r, couleure.g, couleure.b, couleure.a);

                float profondeur = MathF.Pow(0.95f, position.z);

                // rayons = 30, 26 et 22 px
                for (int i = 0; i < 3; i++)
                    Program.DessinerCercle(position, (int)((30 - i * 4) * profondeur), 50);

                return;
            }

            base.RenderObjet();
        }
    }
}