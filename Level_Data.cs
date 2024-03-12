using System;

namespace Dysgenesis
{
    public static class Level_Data
    {
        public static TypeEnnemi[][] lvl_list =
        {
            new TypeEnnemi[] { }, // niveau 0
            new TypeEnnemi[] { TypeEnnemi.OCTAHEDRON, TypeEnnemi.OCTAHEDRON, TypeEnnemi.OCTAHEDRON, TypeEnnemi.OCTAHEDRON, TypeEnnemi.OCTAHEDRON }, // niveau 1
            new TypeEnnemi[] { TypeEnnemi.DIAMANT, TypeEnnemi.DIAMANT, TypeEnnemi.DIAMANT, TypeEnnemi.OCTAHEDRON, TypeEnnemi.OCTAHEDRON, TypeEnnemi.DIAMANT, TypeEnnemi.DIAMANT, TypeEnnemi.OCTAHEDRON }, // niveau 2
            new TypeEnnemi[] { TypeEnnemi.TOURNANT, TypeEnnemi.OCTAHEDRON, TypeEnnemi.DIAMANT, TypeEnnemi.DIAMANT, TypeEnnemi.OCTAHEDRON, TypeEnnemi.TOURNANT, TypeEnnemi.TOURNANT, TypeEnnemi.TOURNANT, TypeEnnemi.OCTAHEDRON, TypeEnnemi.TOURNANT }, // niveau 3
            new TypeEnnemi[] { TypeEnnemi.TOURNANT, TypeEnnemi.TOURNANT, TypeEnnemi.DIAMANT, TypeEnnemi.DIAMANT, TypeEnnemi.DIAMANT, TypeEnnemi.DIAMANT, TypeEnnemi.TOURNANT, TypeEnnemi.TOURNANT, TypeEnnemi.TOURNANT, TypeEnnemi.DIAMANT }, // niveau 4
            new TypeEnnemi[] { TypeEnnemi.TOURNANT, TypeEnnemi.DIAMANT, TypeEnnemi.ENERGIE, TypeEnnemi.ENERGIE, TypeEnnemi.TOURNANT, TypeEnnemi.TOURNANT, TypeEnnemi.OCTAHEDRON, TypeEnnemi.OCTAHEDRON_DUR, TypeEnnemi.ENERGIE, TypeEnnemi.TOURNANT }, // niveau 5
            new TypeEnnemi[] { TypeEnnemi.ENERGIE, TypeEnnemi.OCTAHEDRON_DUR, TypeEnnemi.OCTAHEDRON_DUR, TypeEnnemi.ENERGIE, TypeEnnemi.DIAMANT_DUR, TypeEnnemi.TOURNANT, TypeEnnemi.DIAMANT, TypeEnnemi.OCTAHEDRON_DUR, TypeEnnemi.DIAMANT_DUR, TypeEnnemi.OCTAHEDRON_DUR }, // niveau 6
            new TypeEnnemi[] { TypeEnnemi.DIAMANT_DUR, TypeEnnemi.TOURNANT_DUR, TypeEnnemi.OCTAHEDRON_DUR, TypeEnnemi.OCTAHEDRON_DUR, TypeEnnemi.CROISSANT, TypeEnnemi.CROISSANT, TypeEnnemi.ENERGIE, TypeEnnemi.TOURNANT_DUR, TypeEnnemi.DIAMANT_DUR, TypeEnnemi.CROISSANT }, // niveau 7
            new TypeEnnemi[] { TypeEnnemi.DIAMANT_DUR, TypeEnnemi.ENERGIE_DUR, TypeEnnemi.DUPLIQUEUR, TypeEnnemi.CROISSANT, TypeEnnemi.DIAMANT_DUR, TypeEnnemi.OCTAHEDRON_DUR, TypeEnnemi.DUPLIQUEUR, TypeEnnemi.CROISSANT, TypeEnnemi.TOURNANT_DUR, TypeEnnemi.DUPLIQUEUR }, // niveau 8
            new TypeEnnemi[] { TypeEnnemi.ENERGIE_DUR, TypeEnnemi.DUPLIQUEUR, TypeEnnemi.OCTAHEDRON_DUR, TypeEnnemi.TOURNANT_DUR, TypeEnnemi.PATRA, TypeEnnemi.ENERGIE_DUR, TypeEnnemi.TOURNANT_DUR, TypeEnnemi.ENERGIE_DUR, TypeEnnemi.PATRA, TypeEnnemi.DIAMANT_DUR }, // niveau 9
            new TypeEnnemi[] { TypeEnnemi.DIAMANT_DUR, TypeEnnemi.PATRA, TypeEnnemi.TOURNANT_DUR, TypeEnnemi.TOURNANT_DUR, TypeEnnemi.ENERGIE_DUR, TypeEnnemi.TOURNANT_DUR, TypeEnnemi.DIAMANT_DUR, TypeEnnemi.OCTAHEDRON_DUR, TypeEnnemi.OCTAHEDRON_DUR, TypeEnnemi.DIAMANT_DUR }, // niveau 10
            new TypeEnnemi[] { TypeEnnemi.TOURNANT_DUR, TypeEnnemi.ENERGIE_DUR, TypeEnnemi.CROISSANT_DUR, TypeEnnemi.ENERGIE_DUR, TypeEnnemi.PATRA, TypeEnnemi.DIAMANT_DUR, TypeEnnemi.CROISSANT_DUR, TypeEnnemi.ENERGIE_DUR, TypeEnnemi.TOURNANT_DUR, TypeEnnemi.OCTAHEDRON_DUR }, // niveau 11
            new TypeEnnemi[] { TypeEnnemi.CROISSANT_DUR, TypeEnnemi.TOURNANT_DUR, TypeEnnemi.TOURNANT_DUR, TypeEnnemi.DIAMANT_DUR, TypeEnnemi.DIAMANT_DUR, TypeEnnemi.TOURNANT_DUR, TypeEnnemi.ENERGIE_DUR, TypeEnnemi.ENERGIE_DUR, TypeEnnemi.TOURNANT_DUR, TypeEnnemi.CROISSANT_DUR }, // niveau 12
            new TypeEnnemi[] { TypeEnnemi.CROISSANT_DUR, TypeEnnemi.TOURNANT_DUR, TypeEnnemi.ENERGIE_DUR, TypeEnnemi.DIAMANT_DUR, TypeEnnemi.DUPLIQUEUR_DUR, TypeEnnemi.ENERGIE_DUR, TypeEnnemi.CROISSANT_DUR, TypeEnnemi.CROISSANT_DUR, TypeEnnemi.TOURNANT_DUR, TypeEnnemi.DIAMANT_DUR }, // niveau 13
            new TypeEnnemi[] { TypeEnnemi.DUPLIQUEUR_DUR, TypeEnnemi.CROISSANT_DUR, TypeEnnemi.ENERGIE_DUR, TypeEnnemi.DUPLIQUEUR_DUR, TypeEnnemi.TOURNANT_DUR, TypeEnnemi.TOURNANT_DUR, TypeEnnemi.DUPLIQUEUR_DUR, TypeEnnemi.CROISSANT_DUR, TypeEnnemi.DUPLIQUEUR_DUR, TypeEnnemi.TOURNANT_DUR }, // niveau 14
            new TypeEnnemi[] { TypeEnnemi.CROISSANT_DUR, TypeEnnemi.CROISSANT_DUR, TypeEnnemi.DUPLIQUEUR_DUR, TypeEnnemi.DUPLIQUEUR_DUR, TypeEnnemi.ENERGIE_DUR, TypeEnnemi.ENERGIE_DUR, TypeEnnemi.PATRA_DUR, TypeEnnemi.ENERGIE_DUR, TypeEnnemi.CROISSANT_DUR, TypeEnnemi.TOURNANT_DUR }, // niveau 15
            new TypeEnnemi[] { TypeEnnemi.ENERGIE_DUR, TypeEnnemi.ENERGIE_DUR, TypeEnnemi.CROISSANT_DUR, TypeEnnemi.CROISSANT_DUR, TypeEnnemi.PATRA_DUR, TypeEnnemi.DUPLIQUEUR_DUR, TypeEnnemi.DUPLIQUEUR_DUR, TypeEnnemi.ENERGIE_DUR, TypeEnnemi.CROISSANT_DUR, TypeEnnemi.CROISSANT_DUR }, // niveau 16
            new TypeEnnemi[] { TypeEnnemi.CROISSANT_DUR, TypeEnnemi.DUPLIQUEUR_DUR, TypeEnnemi.DUPLIQUEUR_DUR, TypeEnnemi.PATRA_DUR, TypeEnnemi.DUPLIQUEUR_DUR, TypeEnnemi.DUPLIQUEUR_DUR, TypeEnnemi.PATRA_DUR, TypeEnnemi.CROISSANT_DUR }, // niveau 17
            new TypeEnnemi[] { TypeEnnemi.DUPLIQUEUR_DUR, TypeEnnemi.PATRA_DUR, TypeEnnemi.PATRA_DUR, TypeEnnemi.CROISSANT_DUR, TypeEnnemi.CROISSANT_DUR, TypeEnnemi.DUPLIQUEUR_DUR, TypeEnnemi.DUPLIQUEUR_DUR, TypeEnnemi.CROISSANT_DUR, TypeEnnemi.PATRA_DUR }, // niveau 18
            new TypeEnnemi[] { TypeEnnemi.DUPLIQUEUR_DUR, TypeEnnemi.PATRA_DUR, TypeEnnemi.DUPLIQUEUR_DUR, TypeEnnemi.DUPLIQUEUR_DUR, TypeEnnemi.CROISSANT_DUR, TypeEnnemi.CROISSANT_DUR, TypeEnnemi.PATRA_DUR, TypeEnnemi.ENERGIE_DUR, TypeEnnemi.CROISSANT_DUR, TypeEnnemi.PATRA_DUR }, // niveau 19
            new TypeEnnemi[] { TypeEnnemi.BOSS } // niveau 20
        };
        static readonly TypeEnnemi[] ennemis_valides_arcade =
        {
            // doivent être mis en ordre de plus probable à moins probable
            TypeEnnemi.OCTAHEDRON, TypeEnnemi.DIAMANT, TypeEnnemi.TOURNANT, TypeEnnemi.ENERGIE, TypeEnnemi.CROISSANT, TypeEnnemi.DUPLIQUEUR, TypeEnnemi.PATRA,
            TypeEnnemi.OCTAHEDRON_DUR, TypeEnnemi.DIAMANT_DUR, TypeEnnemi.TOURNANT_DUR, TypeEnnemi.ENERGIE_DUR, TypeEnnemi.CROISSANT_DUR, TypeEnnemi.DUPLIQUEUR_DUR, TypeEnnemi.PATRA_DUR
        };
        public static TypeEnnemi[] arcade_ens = new TypeEnnemi[0];
        static int timer;
        static void GenererListeArcade()
        {
            arcade_ens = new TypeEnnemi[(int)MathF.Sqrt(Program.level * 10) + 2];

            for (int i = 0; i < arcade_ens.Length; i++)
            {
                int enemy_rng = Program.RNG.Next(100);
                // j = ID type ennemi
                for (int j = 0; j < ennemis_valides_arcade.Length; j++)
                {
                    // todo: expliquer wtf c'est quoi ça
                    if (Program.level > (j * j) / 10f && i < (j * j) / 5 && enemy_rng < -1.5f * j + 50)
                    {
                        arcade_ens[i] = ennemis_valides_arcade[j];
                        break;
                    }
                }
            }
        }
        public static void Level_Change()
        {
            if (Program.gamemode == Gamemode.ARCADE && timer < 200)
                timer = 200;

            if (timer == 200)
                Son.JouerEffet(ListeAudio.NIVEAU);

            if (timer > 200 && timer < 320)
            {
                Text.DisplayText("niveau "+(Program.level + 1), new Vector2( Text.CENTRE, Text.CENTRE ), 5);
            }

            if (timer >= 350)
            {
                Program.level++;
                timer = 0;

                if (Program.gamemode == Gamemode.GAMEPLAY)
                {
                    Program.ens_needed = lvl_list[Program.level].Length;
                }
                else
                {
                    GenererListeArcade();
                    Program.ens_needed = arcade_ens.Length;
                }
                Program.ens_killed = 0;
            }

            timer++;
        }
    }
}