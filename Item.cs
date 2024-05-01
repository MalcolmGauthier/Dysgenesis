using static SDL2.SDL;

namespace Dysgenesis
{
    public enum TypeItem
    {
        NONE,
        X2_SHOT,
        X3_SHOT,
        HOMING,
        SPREAD,
        LASER,
        HP,
        WAVE
    };

    public class Item : Sprite
    {
        const int TIMER_DISPARITION_ITEM = 120;
        const int HP_BONUS = 10;
        const float FACTEUR_VITESSE_ITEM = 0.98f;
        readonly SDL_Color[] couleures_items =
        {
            new SDL_Color(){ r=255, g=127, b=0, a=255 }, // orange, x2
            new SDL_Color(){ r=255, g=255, b=0, a=255 }, // jaune, x3
            new SDL_Color(){ r=64, g=255, b=64, a=255 }, // vert pâle, homing
            new SDL_Color(){ r=0, g=0, b=255, a=255 }, // bleu, spread
            new SDL_Color(){ r=127, g=0, b=255, a=255 }, // mauve, laser
            new SDL_Color(){ r=255, g=0, b=0, a=255 }, // rouge, hp
            new SDL_Color(){ r=0, g=255, b=255, a=255 } // bleu pâle, vague
        };

        public TypeItem type;
        public Item(Ennemi parent)
        {
            float roll = Program.RNG.Next(100);

            byte facteur = 30;
            if (Program.gamemode != Gamemode.GAMEPLAY)
                facteur = 40;

            roll -= facteur * MathF.Pow(0.8f, Program.level);

            if (Program.free_items)
                roll = roll * 0.4f + 60;

            if (roll < 60)
                return;

            type = TypeItem.NONE;
            pitch = 1.0f;
            afficher = true;
            position = parent.position;

            if (roll < 70)
                type = TypeItem.HP;
            else if (roll < 80)
                type = TypeItem.WAVE;
            else if (roll < 85)
                type = TypeItem.X2_SHOT;
            else if (roll < 90)
                type = TypeItem.HOMING;
            else if (roll < 94)
                type = TypeItem.SPREAD;
            else if (roll < 97)
                type = TypeItem.LASER;
            else
                type = TypeItem.X3_SHOT;

            if (Program.free_items)
                type = (TypeItem)Program.RNG.Next(1, 8);

            int index_data = (int)type - 1;
            couleure = couleures_items[index_data];
            modele = Data.modeles_items[index_data];
            indexs_lignes_sauter = Data.lignes_a_sauter_items[index_data];

            Program.items.Add(this);
        }
        public override bool Exist()
        {
            if (Move() != 0) return true;
            if (CheckPlayerCollision() != 0) return true;

            return false;
        }
        int Move()
        {
            if (position.z > 0)
            {
                position.z *= FACTEUR_VITESSE_ITEM;

                if (position.z < 1f)
                    position.z = 0;
            }
            else
            {
                if (timer > TIMER_DISPARITION_ITEM)
                {
                    Program.items.Remove(this);
                    return 1;
                }

                timer++;
            }

            return 0;
        }
        int CheckPlayerCollision()
        {
            if (position.z > 0)
                return 0;

            if (Background.Distance(Program.player.position.x, Program.player.position.y, position.x, position.y) < 50)
            {
                switch (type)
                {
                    case TypeItem.NONE:
                        break;
                    case TypeItem.HP:
                        Program.player.HP += HP_BONUS;
                        break;
                    case TypeItem.WAVE:
                        Program.player.shockwaves += 1.0f;
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

            return 0;
        }
        public override void RenderObject()
        {
            if (type == TypeItem.WAVE)
            {
                SDL_SetRenderDrawColor(Program.render, couleure.r, couleure.g, couleure.b, couleure.a);

                for (int i = 0; i < 3; i++)
                    Background.DessinerCercle(new Vector2() { x = position.x, y = position.y }, (byte)((30 - i * 4) * MathF.Pow(0.95f, position.z)), 50);

                return;
            }

            base.RenderObject();

            if (type == TypeItem.HOMING) //TODO: ajouter point à modèle
                SDL_RenderDrawPoint(Program.render, (int)position.x, (int)position.y);
        }
    }
}