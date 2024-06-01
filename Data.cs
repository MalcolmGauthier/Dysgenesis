using System.Text;
using static SDL2.SDL;

namespace Dysgenesis
{
    // avant, cette classe ci contenait toutes les constantes du jeu, mais elles ont été distribués à
    // leurs classes respectives
    //public static class Data
    //{
        
    //}

    // avant tu sauvegardait automatiquement le niveau que tu avait comme option continuer, mais j'ai enlevé cette fonction car elle ne fonctionnait pas
    public static class SaveLoad
    {
        const int NB_MAGIQUE_1 = 395248;
        const int NB_MAGIQUE_2 = 842598;
        const string FICHIER_SAUVEGARDE = @"save.txt";

        public static void Save()
        {
            using (FileStream sw = File.Open(FICHIER_SAUVEGARDE, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                string encoded = (Program.niveau * NB_MAGIQUE_2 + NB_MAGIQUE_1).ToString();
                sw.Write(Encoding.UTF8.GetBytes(encoded));
            }
        }

        public static void Load()
        {
            string read = File.ReadAllText(FICHIER_SAUVEGARDE);

            if (!uint.TryParse(read, out uint num))
            {
                return;
            }

            float check;

            check = (num - NB_MAGIQUE_1) / NB_MAGIQUE_2;

            // vérifie que check est un entier qui donne un nb de niveau réaliste
            if (check < 0 || check > 20 || check % 1 > 0)
            {
                return;
            }

            Program.nv_continue = (int)check;
        }
    }
}