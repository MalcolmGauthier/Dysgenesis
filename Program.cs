/*
 Dysgenesis par Malcolm Gauthier
 Beta v0.3
 */
using static SDL2.SDL;
#pragma warning disable CS8618 // J'AI CRÉÉ LES OBJETS DANS LE FUCKING CONSTRUCTEUR, ARRÊTE DE CHIÂLER

namespace Dysgenesis
{
    public struct Vector2
    {
        public float x;
        public float y;

        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        // = +sqrt(w(a²)+h(b²))
        public static int Distance(float x1, float y1, float x2, float y2, float mult_x = 1, float mult_y = 1)
        {
            return (int)MathF.Sqrt(mult_x * MathF.Pow(MathF.Abs(x1 - x2), 2) + mult_y * MathF.Pow(MathF.Abs(y1 - y2), 2));
        }
        public static int Distance(float x1, float y1, float x2, float y2)
        {
            return Distance(x1, y1, x2, y2, 1, 1);
        }

        public static implicit operator Vector2(Vector3 vector) => new Vector2(vector.x, vector.y);
    }
    public struct Vector3
    {
        public float x;
        public float y;
        public float z;

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static implicit operator Vector3(Vector2 vector) => new Vector3(vector.x, vector.y, 0);
    }
    public enum Gamemode
    {
        TITLESCREEN,
        GAMEPLAY,
        ARCADE,
        CUTSCENE_INTRO,
        CUTSCENE_NOUVELLE_PARTIE,
        CUTSCENE_MAUVAISE_FIN,
        CUTSCENE_BONNE_FIN,
        CUTSCENE_GENERIQUE
    }
    public enum Touches
    {
        W = 1,
        A = 2,
        S = 4,
        D = 8,
        J = 16,
        K = 32,
        R = 64,
        C = 128,
        E = 256,
        MINUS = 1024,
        PLUS = 2048,
        M = 4096,
    }

    // Classe Main. contient les variables importantes
    public static class Program
    {
        const string W_TITLE = "Dysgenesis";
        public static int W_HAUTEUR { get; private set; } = 1080;
        public static int W_LARGEUR { get; private set; } = 1920;
        public static int W_SEMI_HAUTEUR { get; private set; } = W_HAUTEUR / 2;
        public static int W_SEMI_LARGEUR { get; private set; } = W_LARGEUR / 2;
        public const int G_FPS = 60;
        public const int G_MAX_DEPTH = 50;

        static readonly SDL_Rect BARRE_HP = new SDL_Rect() { x = 125, y = 15, w = 10, h = 20 };
        static readonly SDL_Rect BARRE_VAGUE = new SDL_Rect() { x = 125, y = 40, w = 100, h = 20 };
        static readonly int[] CODE_ARCADE = { 0, (int)Touches.A, (int)Touches.R, (int)Touches.C, (int)Touches.A, (int)Touches.D, (int)Touches.E };
        const int TOUCHES_VALIDES_ARCADE = (int)Touches.A | (int)Touches.R | (int)Touches.C | (int)Touches.A | (int)Touches.D | (int)Touches.E;

        static IntPtr window;
        public static IntPtr render;
        static SDL_Event e;
        public static SDL_Color couleure_fond_ecran = new SDL_Color() { r = 0, g = 0, b = 0, a = 255 };

        public static Player player;
        public static Curseur curseur;
        public static List<Ennemi> enemies;
        public static List<Item> items;
        public static List<Projectile> projectiles;
        public static List<Explosion> explosions;
        public static Random RNG = new();

        public static Gamemode Gamemode
        {
            get => _gamemode;
            set
            {
                gTimer = 0;
                _gamemode = value;
            }
        }
        static Gamemode _gamemode = Gamemode.CUTSCENE_INTRO;

        public static int niveau;
        public static int nv_continue = 1;
        public static int gTimer = 0;
        public static int ens_killed = 0;
        public static int ens_needed = 0;
        public static bool bouger_etoiles = true;

        static long frame_time;
        static bool exit = false;
        static bool arcade_unlock = false;
        static byte arcade_steps = 0;
        static int touches_peses = 0;
        static int timer_generique = 0;
        static long temps_entre_60_images_todo_enlever = TimeSpan.TicksPerSecond;

        // DEBUG VARS
        static byte debug_fps_count = 0, debug_fps_count_display = 0;
        static long debug_fps_time = DateTime.Now.Ticks;
        // en temps normal, toutes ces variables sont à faux sauf fullscreen
        public static bool mute_sound = false, free_items = false, partial_cutscene_skip = false,
                           show_fps = false, monologue_skip = true, lvl_select = false,
                           fps_unlock = false, crashtest = false, fullscreen = false,
                           taille_ecran_dynamique = false, true_cutscene_skip = false;

        // point d'entrée du code
        static void Main()
        {
            if (Init() != 0)
            {
                CrashReport(new Exception("Erreure d'initialization: " + SDL_GetError()));
            }

            if (crashtest)
            {
                CrashReport(new Exception("crash test. disable crashtest debug flag to play the game."));
            }

            // main loop:
            // étape 1: mettre à jour les valeures pour quelles touches sont pesées
            // étape 2: éxecuter la logique pour faire avancer le jeu
            // étape 3: dessiner à l'objet render tout les objets actifs
            // étape 4: afficher l'objet render à l'écran
            // étape 5: attendre pour le reste du 60e de seconde au besoin
            // étape 7: incrémenter le global timer
            try
            {
                while (!exit)
                {
                    Controlls();
                    Code();
                    Render();
                    SDLRender();

                    if (!fps_unlock)
                        while (frame_time > DateTime.Now.Ticks - temps_entre_60_images_todo_enlever / G_FPS) ;
                    frame_time = DateTime.Now.Ticks;

                    gTimer++;

                    // bug: si le jeu ne roule pas assez vite sur un ordinateur pour faire 60fps,
                    // les scènes sont désynchronisées de la musique
                    if (GamemodeAction() || Gamemode == Gamemode.TITLESCREEN) //TODO: wtf
                        temps_entre_60_images_todo_enlever = TimeSpan.TicksPerSecond;
                    else
                        temps_entre_60_images_todo_enlever = TimeSpan.TicksPerSecond * 2;
                }
            }
            catch (Exception e)
            {
                CrashReport(e);
            }

            SDL_DestroyWindow(window);
            SDL_DestroyRenderer(render);
            SDL_Quit();
        }

        // initializer SDL et tout les objets
        static int Init()
        {
            // le DLL de SDLSayers que j'utilise est en 64 bit, et donc le programme plante ici si on essaye de l'executer en 32 bit.
            // apart cela, ce code est fait pour fonctionner facilement en 32 bit
            // dans C/C++, ce problème n'existe pas, car le code SDL est compilé avec le projet.
            if (SDL_Init(SDL_INIT_VIDEO | SDL_INIT_AUDIO) != 0)
                return 1;

            //TODO: threading
            /*//static void VerifierFenetreBougee()
            //{
            //    while (true)
            //        while (SDL_PollEvent(out e) == 1)
            //        {
            //            if (e.type == SDL_EventType.SDL_WINDOWEVENT)
            //                if (e.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_MOVED)
            //                {
            //                    Son.ArreterMusique();
            //                }
            //        }
            //}
            //ThreadStart ts = VerifierFenetreBougee;
            //Thread t = new Thread(ts);
            //t.Start();*/

            // ce jeu à été très hardcodé à 1920x1080 quand je l'ai fait, mais j'ai pu quaisiment tout enlever des limites.
            // malheureusement, il y a toujours des problèmes avec les scènes, alors c'est mieux de pas les montrer à des écrans trop petits
            SDL_Rect taille_ecran;
            SDL_GetDisplayBounds(0, out taille_ecran);
            taille_ecran = new SDL_Rect() { w = 1280, h = 960 };//DEBUG
            // si écran trop grand, limite-le
            if (taille_ecran.w > 1920 || taille_ecran.h > 1080)
            {
                fullscreen = false;
            }
            // si l'écran est trop petit, ne montre pas les scènes
            else if (taille_ecran.w < 1920 || taille_ecran.h < 1080)
            {
                partial_cutscene_skip = true;
                W_LARGEUR = taille_ecran.w;
                W_HAUTEUR = taille_ecran.h;
                W_SEMI_LARGEUR = W_LARGEUR / 2;
                W_SEMI_HAUTEUR = W_HAUTEUR / 2;
            }

            window = SDL_CreateWindow(W_TITLE, SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED, W_LARGEUR, W_HAUTEUR,
                (fullscreen ? SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP : 0) |
                SDL_WindowFlags.SDL_WINDOW_SHOWN | 
                (taille_ecran_dynamique ? SDL_WindowFlags.SDL_WINDOW_RESIZABLE : 0)
            );
            render = SDL_CreateRenderer(window, -1, SDL_RendererFlags.SDL_RENDERER_ACCELERATED);

            // active la transparence
            if (SDL_SetRenderDrawBlendMode(render, SDL_BlendMode.SDL_BLENDMODE_BLEND) != 0)
                return 2;

            SDL_ShowCursor((int)SDL_bool.SDL_FALSE);

            SDL_SetRenderDrawColor(render, couleure_fond_ecran.r, couleure_fond_ecran.g, couleure_fond_ecran.b, couleure_fond_ecran.a);
            SDL_RenderPresent(render);

            if (Son.InitSDLMixer() != 0)
                return 3;

            Etoiles.Spawn(new SDL_Rect() { x = 0, y = 0, w = W_LARGEUR, h = W_HAUTEUR }, Etoiles.DENSITE_ETOILES);
            frame_time = DateTime.Now.Ticks;

            player = new Player();
            curseur = new Curseur();
            enemies = new List<Ennemi>(30);
            items = new List<Item>(10);
            projectiles = new List<Projectile>(50);
            explosions = new List<Explosion>(10);

            return 0;
        }

        // mettre à jour les valeures pour touches pesées
        //TODO: threading pour détection de mouvement de fenêtre
        static void Controlls()
        {
            while (SDL_PollEvent(out e) == 1)
            {
                switch (e.type)
                {
                    case SDL_EventType.SDL_QUIT:
                        exit = true;
                        break;

                    case SDL_EventType.SDL_KEYDOWN:
                        switch (e.key.keysym.sym)
                        {
                            case SDL_Keycode.SDLK_ESCAPE:
                                exit = true;
                                break;

                            case SDL_Keycode.SDLK_w:
                                touches_peses |= (int)Touches.W;
                                break;

                            case SDL_Keycode.SDLK_a:
                                touches_peses |= (int)Touches.A;
                                break;

                            case SDL_Keycode.SDLK_s:
                                touches_peses |= (int)Touches.S;
                                break;

                            case SDL_Keycode.SDLK_d:
                                touches_peses |= (int)Touches.D;
                                break;

                            case SDL_Keycode.SDLK_j:
                                touches_peses |= (int)Touches.J;
                                break;

                            case SDL_Keycode.SDLK_k:
                                touches_peses |= (int)Touches.K;
                                break;

                            case SDL_Keycode.SDLK_r:
                                touches_peses |= (int)Touches.R;
                                break;

                            case SDL_Keycode.SDLK_c:
                                touches_peses |= (int)Touches.C;
                                break;

                            case SDL_Keycode.SDLK_e:
                                touches_peses |= (int)Touches.E;
                                break;

                            case SDL_Keycode.SDLK_MINUS:
                                touches_peses |= (int)Touches.MINUS;
                                break;

                            case SDL_Keycode.SDLK_EQUALS:
                                touches_peses |= (int)Touches.PLUS;
                                break;

                            case SDL_Keycode.SDLK_m:
                                touches_peses |= (int)Touches.M;
                                break;
                        }
                        break;

                    case SDL_EventType.SDL_KEYUP:
                        switch (e.key.keysym.sym)
                        {
                            case SDL_Keycode.SDLK_ESCAPE:
                                exit = true;
                                break;

                            case SDL_Keycode.SDLK_w:
                                touches_peses &= ~(int)Touches.W;
                                break;

                            case SDL_Keycode.SDLK_a:
                                touches_peses &= ~(int)Touches.A;
                                break;

                            case SDL_Keycode.SDLK_s:
                                touches_peses &= ~(int)Touches.S;
                                break;

                            case SDL_Keycode.SDLK_d:
                                touches_peses &= ~(int)Touches.D;
                                break;

                            case SDL_Keycode.SDLK_j:
                                touches_peses &= ~(int)Touches.J;
                                break;

                            case SDL_Keycode.SDLK_k:
                                touches_peses &= ~(int)Touches.K;
                                break;

                            case SDL_Keycode.SDLK_r:
                                touches_peses &= ~(int)Touches.R;
                                break;

                            case SDL_Keycode.SDLK_c:
                                touches_peses &= ~(int)Touches.C;
                                break;

                            case SDL_Keycode.SDLK_e:
                                touches_peses &= ~(int)Touches.E;
                                break;

                            case SDL_Keycode.SDLK_MINUS:
                                touches_peses &= ~(int)Touches.MINUS;
                                break;

                            case SDL_Keycode.SDLK_EQUALS:
                                touches_peses &= ~(int)Touches.PLUS;
                                break;

                            case SDL_Keycode.SDLK_m:
                                touches_peses &= ~(int)Touches.M;
                                break;
                        }
                        break;
                }
            }
        }

        // logique du jeu
        static void Code()
        {
            // ajuster les valeures pour la taille de la fenêtre
            if (taille_ecran_dynamique && !fullscreen)
            {
                // on ne peut pas mettre W_LARGEUR et W_HAUTUER directement, car une propriété ne peut pas être une référence
                int w, h;
                SDL_GetWindowSize(window, out w, out h);
                W_LARGEUR = w;
                W_HAUTEUR = h;
                W_SEMI_HAUTEUR = W_HAUTEUR / 2;
                W_SEMI_LARGEUR = W_LARGEUR / 2;
                //Text.DisplayText(W_LARGEUR + ", " + W_HAUTEUR, new(500, 500), 2);
            }

            Son.ChangerVolume();

            // la logique pour les scènes est fait dans le code pour les scènes
            if (GamemodeCutscene())
                return;

            if (bouger_etoiles)
                Etoiles.Move();

            if (Gamemode == Gamemode.TITLESCREEN)
            {
                if (!arcade_unlock)
                {
                    // vérifie si la prochaine touche requise pour débloquer arcade est pesée, et pas la dernière
                    if (!TouchePesee((Touches)CODE_ARCADE[arcade_steps]) && TouchePesee((Touches)CODE_ARCADE[arcade_steps + 1]))
                    {
                        arcade_steps++;
                        Son.JouerEffet(ListeAudioEffets.EXPLOSION_ENNEMI);
                        if (arcade_steps >= CODE_ARCADE.Length - 1)
                        {
                            arcade_unlock = true;
                            curseur.curseur_max_selection = 3;
                            // récompense extra
                            lvl_select = true;
                        }
                    }
                    // vérifie si une touche autre que la prochaine requise ou la dernière est pesée
                    else if (TouchePesee((Touches)TOUCHES_VALIDES_ARCADE - CODE_ARCADE[arcade_steps] - CODE_ARCADE[arcade_steps + 1]))
                    {
                        arcade_steps = 0;
                    }
                }

                // debug level select
                if (lvl_select && gTimer % 10 == 0)
                {
                    if (TouchePesee(Touches.A) && nv_continue > 1)
                        nv_continue--;
                    else if (TouchePesee(Touches.D) && nv_continue < 20)
                        nv_continue++;
                }

                // retourne vrai si option sélectionnée
                if (curseur.Exist())
                {
                    switch (curseur.selection)
                    {
                        case Curseur.OptionsCurseur.NOUVELLE_PARTIE:
                            Son.ArreterMusique();
                            niveau = 0;
                            player.Init();
                            
                            Gamemode = Gamemode.CUTSCENE_NOUVELLE_PARTIE;
                            break;

                        case Curseur.OptionsCurseur.CONTINUER:
                            Son.ArreterMusique();

                            // on place le joueur au niveau avant celui qui l'a tué, mais on
                            // ment au jeu en dissant que tout les ennemis sont morts pour pouvoir
                            // rejouer l'animation qui dit le niveau
                            niveau = nv_continue - 1;
                            ens_killed = DataNiveau.lvl_list[niveau].Length;
                            ens_needed = 0;

                            player.Init();
                            player.afficher = true;

                            // conséquences pour avoir sélectionné l'option
                            player.HP = 50;
                            player.vagues = 0;

                            Gamemode = Gamemode.GAMEPLAY;
                            break;

                        case Curseur.OptionsCurseur.ARCADE:
                            niveau = 0;
                            player.Init();
                            player.afficher = true;
                            Son.JouerMusique(ListeAudioMusique.DCQBPM, true);
                            Gamemode = Gamemode.ARCADE;
                            break;

                        default:
                            break;
                    }
                }

                // la scène du générique peut être activée en appuyant sur M pendant 2 secondes au menu
                if (TouchePesee(Touches.M))
                {
                    timer_generique++;

                    if (timer_generique >= 120)
                    {
                        Son.ArreterMusique();
                        Gamemode = Gamemode.CUTSCENE_GENERIQUE;
                        return;
                    }
                }
                else
                {
                    timer_generique = 0;
                }
            }

            // le reste du code est pour gamemodes GAMEPLAY et ARCADE
            if (!GamemodeAction())
                return;

            // on éxecute les ennemis avant le joueur pourque leurs modèles peuvent tourner même si le joueur est mort
            ExecuterLogique(enemies.Cast<Sprite>().ToList());

            if (VerifBoss())
            {
                BombePulsar.Exist();
            }

            // si joueur mort, fait rien d'autre
            if (player.Exist())
                return;

            if (TouchePesee(Touches.K))
                VagueElectrique.EssayerCreation();

            VagueElectrique.Exist();

            // évite un div/0, mais ne devrait jamais être frappé
            if (niveau == -1)
                niveau = 0;

            // les ennemis apparaîssent plus vite dépandant du niveau
            int timer_enemy_spawn = 400 / (niveau + 1);
            if (gTimer % timer_enemy_spawn == timer_enemy_spawn - 1 &&
                ens_needed > 0 && !player.Mort())
            {
                int verif = enemies.Count;

                if (Gamemode == Gamemode.GAMEPLAY)
                    new Ennemi(DataNiveau.lvl_list[niveau][ens_needed - 1], StatusEnnemi.INITIALIZATION);
                else
                    new Ennemi(DataNiveau.arcade_ens[ens_needed - 1], StatusEnnemi.INITIALIZATION);

                // vérifie si un ennemi a bien été créé avent de décrementer le nb d'ennemis à créér
                if (enemies.Count > verif)
                    ens_needed--;
            }

            // vérifie si niveau complété
            if (enemies.Count == 0)
            {
                if ((Gamemode == Gamemode.GAMEPLAY && ens_killed >= DataNiveau.lvl_list[niveau].Length) ||
                    (Gamemode == Gamemode.ARCADE && ens_killed >= DataNiveau.arcade_ens.Length))
                {
                    DataNiveau.ChangerNiveau();
                }
            }

            ExecuterLogique(projectiles.Cast<Sprite>().ToList());
            Projectile.son_cree = false;
            ExecuterLogique(items.Cast<Sprite>().ToList());
            ExecuterLogique(explosions.Cast<Sprite>().ToList());

            player.HP = Math.Clamp(player.HP, 0, Player.JOUEUR_MAX_HP);
            player.vagues = Math.Clamp(player.vagues, 0, Player.JOUEUR_MAX_VAGUES);
        }

        // dessiner tout à l'objet SDL_Renderer
        // important de n'éxecuter AUCUNE LOGIQUE ici.
        static void Render()
        {
            if (GamemodeCutscene())
            {
                switch (Gamemode)
                {
                    case Gamemode.CUTSCENE_INTRO:
                        Cutscene.Cut_0();
                        break;
                    case Gamemode.CUTSCENE_NOUVELLE_PARTIE:
                        Cutscene.Cut_1();
                        break;
                    case Gamemode.CUTSCENE_BONNE_FIN:
                        Cutscene.Cut_2();
                        break;
                    case Gamemode.CUTSCENE_MAUVAISE_FIN:
                        Cutscene.Cut_3();
                        break;
                    case Gamemode.CUTSCENE_GENERIQUE:
                        Cutscene.Cut_4();
                        break;
                }

                return;
            }

            if (GamemodeAction())
            {
                Etoiles.Render(Etoiles.DENSITE_ETOILES);

                if (VerifBoss())
                {
                    BombePulsar.DessinerBombePulsar(
                        new Vector2(Program.W_SEMI_LARGEUR, Program.W_SEMI_HAUTEUR / 2),
                        (byte)(25 - enemies[0].position.z / 4),
                        true
                    );
                }

                foreach (Ennemi e in enemies)
                    e.RenderObject();

                foreach (Item i in items)
                    i.RenderObject();

                foreach (Projectile p in projectiles)
                    p.RenderObject();

                foreach (Explosion e in explosions)
                    e.RenderObject();

                player.RenderObject();

                if (BombePulsar.HP_bombe <= 0)
                    return;

                VagueElectrique.Render();

                SDL_Rect barre_hud = BARRE_HP;
                for (int i = 0; i < player.HP; i++)
                {
                    if (i <= 20)
                        SDL_SetRenderDrawColor(render, 255, 0, 0, 255);
                    else if (i <= 50)
                        SDL_SetRenderDrawColor(render, 255, 255, 0, 255);
                    else if (i <= 100)
                        SDL_SetRenderDrawColor(render, 64, 255, 64, 255);
                    else
                        SDL_SetRenderDrawColor(render, 0, 0, 255, 255);
                    SDL_RenderFillRect(render, ref barre_hud);
                    barre_hud.x += barre_hud.w + 1;
                }

                barre_hud = BARRE_VAGUE;
                int vagues_entiers = (int)MathF.Floor(player.vagues);
                SDL_SetRenderDrawColor(render, 0, 255, 255, 255);
                for (int i = vagues_entiers; i > 0; i--)
                {
                    SDL_RenderFillRect(render, ref barre_hud);
                    barre_hud.x += barre_hud.w + 5;
                }

                float vagues_reste = player.vagues % 1.0f;
                barre_hud.w = (int)(MathF.Round(vagues_reste, 2) * 100);
                SDL_RenderFillRect(render, ref barre_hud);

                Text.DisplayText("    hp:\nvagues:", new Vector2(10, 15), 2);

                if (VerifBoss())
                {
                    Text.DisplayText("    hp:\nennemi", new Vector2(10, 85), 2);
                    barre_hud = BARRE_HP;
                    barre_hud.y += 70;
                    for (int i = 0; i < enemies[0].HP; i++)
                    {
                        if (i <= 20)
                            SDL_SetRenderDrawColor(render, 255, 0, 0, 255);
                        else if (i <= 50)
                            SDL_SetRenderDrawColor(render, 255, 255, 0, 255);
                        else
                            SDL_SetRenderDrawColor(render, 64, 255, 64, 255);

                        SDL_RenderFillRect(render, ref barre_hud);
                        barre_hud.x += barre_hud.w + 1;
                    }
                }

                // est affiché pour niveau 0 et 1
                if (Gamemode == Gamemode.GAMEPLAY && niveau < 2)
                {
                    Text.DisplayText(
                        "controles:\n" +
                        "wasd pour bouger\n" +
                        "j pour tirer\n" +
                        "k pour activer une vague électrique",
                        new Vector2(Program.W_SEMI_LARGEUR - 300, Program.W_SEMI_HAUTEUR + 100),
                        2
                    );
                }

                return;
            }

            if (Gamemode == Gamemode.TITLESCREEN)
            {
                Etoiles.Render(Etoiles.DENSITE_ETOILES);

                Text.DisplayText("dysgenesis",
                    new Vector2(Text.CENTRE, Text.CENTRE), 5);

                Text.DisplayText("nouvelle partie",
                    new Vector2(Program.W_SEMI_LARGEUR - 114, Program.W_SEMI_HAUTEUR + 75), 2);

                Text.DisplayText("controles menu: w et s pour bouger le curseur, " +
                    "j pour sélectionner\n\ncontroles globaux: esc. pour quitter, " +
                    "+/- pour monter ou baisser le volume",
                    new Vector2(10, Program.W_HAUTEUR - 40), 1);

                Text.DisplayText("v 0.3 (beta)",
                    new Vector2(Program.W_LARGEUR - 200, Program.W_HAUTEUR - 30), 2);

                if (curseur.curseur_max_selection >= 2)
                    Text.DisplayText("continuer: niveau " + nv_continue,
                    new Vector2(Program.W_SEMI_LARGEUR - 114, Program.W_SEMI_HAUTEUR + 125), 2);

                if (curseur.curseur_max_selection >= 3)
                    Text.DisplayText("arcade",
                    new Vector2(Program.W_SEMI_LARGEUR - 114, Program.W_SEMI_HAUTEUR + 175), 2);

                curseur.RenderObject();
            }
        }

        // portion render qui devrait toujours rouler, de quoi que ce soit
        public static void SDLRender()
        {
            // debug
            if (show_fps)
            {
                if (debug_fps_time < DateTime.Now.Ticks - TimeSpan.TicksPerSecond) // fps
                {
                    debug_fps_count++;
                    debug_fps_count_display = debug_fps_count;
                    debug_fps_count = 0;
                    debug_fps_time = DateTime.Now.Ticks;
                }
                else
                {
                    debug_fps_count++;
                }

                SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
                Text.DisplayText(debug_fps_count_display.ToString(), new Vector2(1828, 52), 3);
            }

            // le son est dans cette fonction et non l'autre cars elle doit toujours être éxecutée, mais aussi la dernière chose dessinée
            Son.RenderVolume();

            SDL_SetRenderDrawColor(render, couleure_fond_ecran.r, couleure_fond_ecran.g, couleure_fond_ecran.b, byte.MaxValue);
            SDL_RenderPresent(render);
            SDL_RenderClear(render);
        }

        // retourne vrai si Gamemode est un mode d'action (pas menu ou scène), et l'autre où mode scène
        public static bool GamemodeAction()
        {
            return Gamemode == Gamemode.GAMEPLAY || Gamemode == Gamemode.ARCADE;
        }
        public static bool GamemodeCutscene()
        {
            return
                Gamemode == Gamemode.CUTSCENE_INTRO ||
                Gamemode == Gamemode.CUTSCENE_NOUVELLE_PARTIE ||
                Gamemode == Gamemode.CUTSCENE_BONNE_FIN ||
                Gamemode == Gamemode.CUTSCENE_MAUVAISE_FIN ||
                Gamemode == Gamemode.CUTSCENE_GENERIQUE
            ;
        }

        // retourne vrai si la touche spécifié est enfoncé pendant cette image
        public static bool TouchePesee(Touches touche)
        {
            return (touches_peses & (int)touche) != 0;
        }

        // retourne vrai si le boss existe et est le seul ennemi
        static bool VerifBoss()
        {
            if (enemies.Count != 1)
                return false;

            return enemies[0].type == TypeEnnemi.BOSS;
        }

        // éxecute .Exist dans une liste de sprites, en tenant compte de ceux qui se font enlever
        static void ExecuterLogique(List<Sprite> sprites)
        {
            for (int i = 0; i < sprites.Count; i++)
            {
                if (sprites[i].Exist())
                {
                    // !!! ceci ne supprime que l'objet dans la liste locale sprites!
                    // l'objet devrait être enlevé de sa liste respective avant que son .Exist() retourne faux.
                    sprites.RemoveAt(i);
                    i--;
                }
            }
        }

        // Dessine un polygone avec X côtés pour créér une approximation de cercle.
        public static void DessinerCercle(Vector2 position, int taille, byte cotes)
        {
            float ang;
            float next_ang;

            for (int i = 0; i < cotes; i++)
            {
                // Tau = 2*Pi
                ang = (i * MathF.Tau) / cotes;
                next_ang = ((i + 1) * MathF.Tau) / cotes;

                SDL_RenderDrawLineF(Program.render,
                    position.x + taille * MathF.Sin(ang),
                    position.y + taille * MathF.Cos(ang),
                    position.x + taille * MathF.Sin(next_ang),
                    position.y + taille * MathF.Cos(next_ang)
                );
            }
        }

        // convertit une valeure couleure hex en SDLColor
        public static SDL_Color RGBAtoSDLColor(uint RGBA)
        {
            return new SDL_Color()
            {
                r = (byte)((RGBA >> 24) & 0xFF),
                g = (byte)((RGBA >> 16) & 0xFF),
                b = (byte)((RGBA >> 8) & 0xFF),
                a = (byte)((RGBA >> 0) & 0xFF),
            };
        }

        // écran à montrer si le jeu plante. ce code-ci ne devrait jamais donner d'erreure
        // si SDL est initializé, mais il y a un try finally quand même.
        public static void CrashReport(Exception e)
        {
            try
            {
                Son.ArreterMusique();
                // effacer l'écran avec du noir
                SDL_SetRenderDrawColor(render, 0, 0, 0, 255);
                SDL_RenderClear(render);
                Text.DisplayText("erreure fatale!\n\n"
                    + e.Message + "\n"
                    + e.StackTrace + "\n" +
                    "\n\ntapez sur échapp. pour quitter l'application.", new Vector2(10, 10), 1
                );
                // le texte vas mettre le RenderDrawColor à blanc, alors on le remets à noir pour présenter l'écran
                SDL_SetRenderDrawColor(render, 0, 0, 0, 255);
                SDL_RenderPresent(render);
                // ne fait absolument rien sauf vérifier quand le joueur pèse la touche échapper
                //TODO: ajouter limite à boucle
                while (!exit)
                {
                    SDL_PollEvent(out Program.e);

                    if (Program.e.type == SDL_EventType.SDL_QUIT)
                        exit = true;

                    if (Program.e.type == SDL_EventType.SDL_KEYDOWN)
                        if (Program.e.key.keysym.sym == SDL_Keycode.SDLK_ESCAPE)
                            exit = true;
                }
            }
            finally
            {
                // détruire objets SDL + quitter programme avec code erreure
                SDL_DestroyRenderer(render);
                SDL_DestroyWindow(window);
                SDL_Quit();
                Environment.Exit(-1);
            }
        }
    }
}