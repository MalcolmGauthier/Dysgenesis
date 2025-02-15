using System;

namespace Dysgenesis
{
    // contient le data pour les niveaux et gère les transitions de niveau en mode normal et arcade
    public static class DataNiveau
    {
        // liste de tous les ennemis que chaque niveau contient
        public static TypeEnnemi[][] liste_niveaux =
        {
            new TypeEnnemi[] { }, // niveau 0
            new TypeEnnemi[] { TypeEnnemi.OCTAHEDRON, TypeEnnemi.OCTAHEDRON, TypeEnnemi.OCTAHEDRON, TypeEnnemi.OCTAHEDRON, TypeEnnemi.OCTAHEDRON}, // niveau 1
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

        public static TypeEnnemi[] liste_ennemis_arcade = Array.Empty<TypeEnnemi>();

        static int timer;

        // Générer la liste d'ennemis pour le prochain niveau d'arcade. Créé avec du hasard.
        // retourne la longeure de la liste crée
        static int GenererListeArcade()
        {
            const int VARIABLITE_ENNEMI_CHOISI = 2;
            const float RAPIDITE_DE_DIFFICULTE = 8.0f; // +grand = moins rapide, ne pas mettre négatif!

            if (Program.niveau < 0)
                return 0;

            // nb d'ennemis à tuer pour le prochain niveau
            // formule magique
            liste_ennemis_arcade = new TypeEnnemi[(int)MathF.Sqrt(Program.niveau * 10) + 2]; // overflow à niveau 214748365

            int next_entry;
            // https://www.desmos.com/calculator/c7nlh1k17t formule moins magique
            int formule = (int)(ennemis_valides_arcade.Length * Program.niveau / (Program.niveau + RAPIDITE_DE_DIFFICULTE));
            for (int i = 0; i < liste_ennemis_arcade.Length; i++)
            {
                next_entry = formule + Program.RNG.Next(-VARIABLITE_ENNEMI_CHOISI, VARIABLITE_ENNEMI_CHOISI + 1);

                next_entry = Math.Clamp(next_entry, 0, ennemis_valides_arcade.Length);

                liste_ennemis_arcade[i] = (TypeEnnemi)next_entry;
            }

            return liste_ennemis_arcade.Length;
        }

        // animation pour changement de niveau + mise à jours des nb d'ennemis pour le prochain
        public static void ChangerNiveau()
        {
            const int TEMPS_AVANT_TEXTE = (int)(3.33f * Program.G_FPS);
            const int TEMPS_TEXTE_SUR_ECRAN = (int)(2.0f * Program.G_FPS) + TEMPS_AVANT_TEXTE;
            const int TEMPS_APRES_TEXTE_PARTI = (int)(0.5f  + Program.G_FPS) + TEMPS_TEXTE_SUR_ECRAN;

            // en mode arcade, l'animation commence dès que le dernier ennemi est tué.
            if (Program.Gamemode == Gamemode.ARCADE && timer < TEMPS_AVANT_TEXTE)
                timer = TEMPS_AVANT_TEXTE;

            if (timer == TEMPS_AVANT_TEXTE)
                Son.JouerEffet(ListeAudioEffets.NIVEAU);

            if (timer > TEMPS_AVANT_TEXTE && timer < TEMPS_TEXTE_SUR_ECRAN)
            {
                Text.DisplayText("niveau " + (Program.niveau + 1), new Vector2(Text.CENTRE, Text.CENTRE), 5);
            }

            if (timer >= TEMPS_APRES_TEXTE_PARTI)
            {
                Program.niveau++;
                timer = 0;

                if (Program.Gamemode == Gamemode.GAMEPLAY)
                {
                    Program.ennemis_a_creer = liste_niveaux[Program.niveau].Length;
                }
                else
                {
                    Program.ennemis_a_creer = GenererListeArcade();
                }

                Program.ennemis_tues = 0;
            }

            timer++;
        }
    }
}