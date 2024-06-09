using static SDL2.SDL;

namespace Dysgenesis
{
    // fonction générale pour pas mal tout ce qui a une position et qui se fait dessiner à l'écran
    public abstract class Sprite
    {
        public const int POSITION_TIR_NON_EXISTANTE = -1;

        public Vector3 position;
        public Vector3[] modele = Array.Empty<Vector3>();
        public SDL_Color couleure = new SDL_Color() { r = 255, g = 255, b = 255, a = 255 };
        public float taille = 1.0f;
        public float pitch = 0.0f;
        public float roll = 0.0f;
        public int[] indexs_lignes_sauter = Array.Empty<int>();
        public int[] indexs_de_tir = new int[2]; // indexes dans le modèles pour de quelle lignes partent les projectiles
        public int timer = 0;
        public bool afficher = true;

        // retourne la position d'une ligne sur le modèle donné.
        public float[] PositionLigneModele(int line_index, Vector3[] modele)
        {
            if (line_index >= modele.Length || line_index < 0)
                return new float[4];

            float sinroll = MathF.Sin(roll);
            float cosroll = MathF.Cos(roll);

            float grandeure_ligne = taille * MathF.Pow(0.95f, position.z);

            // pour éviter erreure index OOB
            if (line_index == modele.Length - 1)
            {
                return new float[2]
                {
                    grandeure_ligne * (cosroll * -modele[line_index].x - sinroll * -modele[line_index].y) + position.x,
                    grandeure_ligne * (sinroll * -modele[line_index].x + cosroll * -modele[line_index].y) + position.y + modele[line_index].z * pitch,
                };
            }

            // rendering quaisiment 3D. avec cette manière, le modèle peut bouger avec n'importe quel roll,
            // mais aucun yaw, et pitch est trop distorté hors de -1 à 1.
            // le x et y du modèle doivent être inversés woops mais c'est mieux que avoir à aller dans leurs modèles et manuellement tout changer.
            return new float[4]
            {
                grandeure_ligne * (cosroll * -modele[line_index    ].x - sinroll * -modele[line_index]    .y) + position.x,
                grandeure_ligne * (sinroll * -modele[line_index    ].x + cosroll * -modele[line_index]    .y) + position.y + modele[line_index    ].z * pitch,
                grandeure_ligne * (cosroll * -modele[line_index + 1].x - sinroll * -modele[line_index + 1].y) + position.x,
                grandeure_ligne * (sinroll * -modele[line_index + 1].x + cosroll * -modele[line_index + 1].y) + position.y + modele[line_index + 1].z * pitch
            };
        }
        public float[] RenderDataLigne(int line_index)
        {
            return PositionLigneModele(line_index, modele);
        }

        // dessine un modèle à l'écran à l'aide de la liste d'indexs à sauter
        public virtual void RenderObject(Vector3[] modele)
        {
            if (!afficher || modele == null)
                return;

            float[] positions_ligne;
            byte index_sauts = 0;

            SDL_SetRenderDrawColor(Program.render, couleure.r, couleure.g, couleure.b, couleure.a);

            for (int i = 0; i < modele.Length - 1; i++)
            {
                // pour ettre efficace et stable, la liste d'indexs de sauts doit être en ordre et terminer avec un nombre extra.
                // personellement, j'utilise toujours -1.
                if (i == indexs_lignes_sauter[index_sauts] - 1)
                {
                    index_sauts++;
                    continue;
                }

                positions_ligne = PositionLigneModele(i, modele);

                SDL_RenderDrawLineF(Program.render, positions_ligne[0], positions_ligne[1], positions_ligne[2], positions_ligne[3]);
            }
        }
        public virtual void RenderObject()
        {
            RenderObject(modele);
        }

        // Cette fonction roule la logique pour le sprite. Il n'est pas applé automatiquement,
        // pourque l'ordre puisse être choisit manuellement.
        // Normalement, il retourne vrai si le sprite a été détruit pendant l'éxecution du code, faux sinon.
        public abstract bool Exist();
    }
}