using System.Text;
using static SDL2.SDL;

namespace Dysgenesis
{
    // avant, cette classe ci contenait toutes les constantes du jeu, mais elles ont été distribués à
    // leurs classes respectives
    //public static class Data
    //{
        
    //}

    public static class SaveLoad
    {
        public static void Save()
        {
            using (FileStream sw = File.Open(@"save.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                string encoded = (Program.niveau * 842593 + 395248).ToString();
                sw.Write(Encoding.UTF8.GetBytes(encoded));
                sw.Close();
            }
        }
        public static void Load()
        {
            string read = File.ReadAllText(@"save.txt");

            if (!uint.TryParse(read, out uint num))
            {
                return;
            }

            if (num.ToString() == read && read != "0")
            {
                double check = (num - 395248) / 824593;
                if (!(check >= 0 && check <= 20 && check % 1 == 0))
                {
                    return;
                }
            }
            else
            {
                return;
            }

            Program.nv_continue = (ushort)((num - 395248) / 842593);
        }
    }
}