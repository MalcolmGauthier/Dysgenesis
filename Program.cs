/*
 Dysgenesis par Malcolm Gauthier
 Beta v0.2
 */
using SDL2;
using System.Runtime.CompilerServices;
using static SDL2.SDL;
using static SDL2.SDL_mixer;

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
        CUTSCENE_START,
        CUTSCENE_BAD_END,
        CUTSCENE_GOOD_END,
        CREDITS
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
        public static int W_HAUTEUR = 1080;
        public static int W_LARGEUR = 1920;
        public static int W_SEMI_HAUTEUR = W_HAUTEUR / 2;
        public static int W_SEMI_LARGEUR = W_LARGEUR / 2;
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

        public static Player player = new();
        public static Curseur curseur = new();
        public static BombePulsar bombe = new();
        public static List<Ennemi> enemies = new(30);
        public static List<Item> items = new(10);
        public static List<Projectile> projectiles = new(50);
        public static List<Explosion> explosions = new(10);
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
        public static bool mute_sound = false, free_items = false, cutscene_skip = true,
                           show_fps = false, monologue_skip = false, lvl_select = false,
                           fps_unlock = false, crashtest = false, fullscreen = true;
        static void Main()
        {
            if (Init() != 0)
                CrashReport(new Exception("Erreure d'initialization: " + SDL_GetError()));

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
            while (!exit)
            {
                Controlls();
                Code();
                Render();
                SDLRender();

                if (!fps_unlock)
                    while (frame_time > DateTime.Now.Ticks - temps_entre_60_images_todo_enlever / Program.G_FPS) ;
                frame_time = DateTime.Now.Ticks;

                gTimer++;

                // bug: si le jeu ne roule pas assez vite sur un ordinateur pour faire 60fps,
                // les scènes sont désynchronisées de la musique
                if (GamemodeAction() || Gamemode == Gamemode.TITLESCREEN) //todo: wtf
                    temps_entre_60_images_todo_enlever = TimeSpan.TicksPerSecond;
                else
                    temps_entre_60_images_todo_enlever = TimeSpan.TicksPerSecond * 2;
            }

            SDL_DestroyWindow(window);
            SDL_DestroyRenderer(render);
            SDL_Quit();
        }

        // initializer SDL et tout les objets
        static int Init()
        {
            if (SDL_Init(SDL_INIT_VIDEO | SDL_INIT_AUDIO) != 0)
                return 1;

            window = SDL_CreateWindow(Program.W_TITLE, SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED, Program.W_LARGEUR, Program.W_HAUTEUR,
                fullscreen ? SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP : 0 | SDL_WindowFlags.SDL_WINDOW_SHOWN | SDL_WindowFlags.SDL_WINDOW_RESIZABLE);
            render = SDL_CreateRenderer(window, -1, SDL_RendererFlags.SDL_RENDERER_ACCELERATED);
            SDL_PollEvent(out e);

            if (SDL_SetRenderDrawBlendMode(render, SDL_BlendMode.SDL_BLENDMODE_BLEND) != 0)
                return 2;

            SDL_SetRenderDrawColor(render, couleure_fond_ecran.r, couleure_fond_ecran.g, couleure_fond_ecran.b, couleure_fond_ecran.a);
            SDL_RenderPresent(render);

            Etoiles.Spawn(new SDL_Rect() { x = 0, y = 0, w = Program.W_LARGEUR, h = Program.W_HAUTEUR });
            frame_time = DateTime.Now.Ticks;

            if (Son.InitSDLMixer() != 0)
                return 3;

            //SaveLoad.Load();
            SDL_ShowCursor((int)SDL_bool.SDL_FALSE);
            return 0;
        }

        // mettre à jour les valeures pour touches pesées
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
                            Son.StopMusic();
                            niveau = 0;
                            player.Init();
                            
                            Gamemode = Gamemode.CUTSCENE_START;
                            break;

                        case Curseur.OptionsCurseur.CONTINUER:
                            Son.StopMusic();

                            // on place le joueur au niveau avant celui qui l'a tué, mais on
                            // ment au jeu en dissant que tout les ennemis sont morts pour pouvoir
                            // rejouer l'animation qui dit le niveau
                            niveau = nv_continue - 1;
                            ens_killed = Level_Data.lvl_list[niveau].Length;
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
                        Son.StopMusic();
                        Gamemode = Gamemode.CREDITS;
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

            // si joueur mort, fait rien d'autre
            if (player.Exist())
                return;

            if (TouchePesee(Touches.K))
                VagueElectrique.EssayerCreation();

            // évite div/0, mais ne devrait jamais être frappé
            if (niveau == -1)
                niveau = 0;

            // les ennemis apparaîssent plus vite dépandant du niveau
            int timer_enemy_spawn = 400 / (niveau + 1);
            if (gTimer % timer_enemy_spawn == timer_enemy_spawn - 1 &&
                ens_needed > 0 && !player.Mort())
            {
                int verif = enemies.Count;

                if (Gamemode == Gamemode.GAMEPLAY)
                    new Ennemi(Level_Data.lvl_list[niveau][ens_needed - 1], StatusEnnemi.INITIALIZATION);
                else
                    new Ennemi(Level_Data.arcade_ens[ens_needed - 1], StatusEnnemi.INITIALIZATION);

                // vérifie si un ennemi a bien été créé avent de décrementer le nb d'ennemis à créér
                if (enemies.Count > verif)
                    ens_needed--;
            }

            // vérifie si niveau complété
            if (enemies.Count == 0)
            {
                if ((Gamemode == Gamemode.GAMEPLAY && ens_killed >= Level_Data.lvl_list[niveau].Length) ||
                    (Gamemode == Gamemode.ARCADE && ens_killed >= Level_Data.arcade_ens.Length))
                {
                    Level_Data.ChangerNiveau();
                }
            }

            ExecuterLogique(projectiles.Cast<Sprite>().ToList());
            ExecuterLogique(enemies.Cast<Sprite>().ToList());
            ExecuterLogique(items.Cast<Sprite>().ToList());
            ExecuterLogique(explosions.Cast<Sprite>().ToList());

            player.HP = Math.Clamp(player.HP, 0, Player.JOUEUR_MAX_HP);
            player.vagues = Math.Clamp(player.vagues, 0, Player.JOUEUR_MAX_VAGUES);
        }

        // dessiner tout à l'objet SDL_Renderer
        static void Render()
        {
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
                    bombe.Exist();//todo: bouger à code
                }

                foreach (Ennemi e in enemies)
                    e.RenderObject();

                foreach (Item i in items)
                    i.RenderObject();

                foreach (Projectile p in projectiles)
                    p.RenderObject();

                player.RenderObject();

                if (bombe.HP_bombe <= 0)
                    return;

                VagueElectrique.Afficher();

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
                        new Vector2(Program.W_SEMI_LARGEUR - 300, Program.W_SEMI_HAUTEUR + 400),
                        2
                    );
                }
            }
            else if (Gamemode == Gamemode.TITLESCREEN)
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
                    new Vector2(Text.CENTRE, Program.W_HAUTEUR - 30), 2);

                if (curseur.curseur_max_selection >= 2)
                    Text.DisplayText("continuer: niveau " + nv_continue,
                    new Vector2(Program.W_SEMI_LARGEUR - 114, Program.W_SEMI_HAUTEUR + 125), 2);
                if (curseur.curseur_max_selection >= 3)
                    Text.DisplayText("arcade",
                    new Vector2(Program.W_SEMI_LARGEUR - 114, Program.W_SEMI_HAUTEUR + 175), 2);

                curseur.RenderObject();
            }
            else
            {
                switch (Gamemode)
                {
                    case Gamemode.CUTSCENE_INTRO:
                        Cutscene.Cut_0();
                        break;
                    case Gamemode.CUTSCENE_START:
                        Cutscene.Cut_1();
                        break;
                    case Gamemode.CUTSCENE_GOOD_END:
                        Cutscene.Cut_2();
                        break;
                    case Gamemode.CUTSCENE_BAD_END:
                        Cutscene.Cut_3();
                        break;
                    case Gamemode.CREDITS:
                        Cutscene.Cut_4();
                        break;
                }
            }            
        }

        // portion render qui devrait toujours rouler, de quoi que ce soit
        public static void SDLRender()
        {
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

            Son.ChangerVolume();

            SDL_SetRenderDrawColor(render, couleure_fond_ecran.r, couleure_fond_ecran.g, couleure_fond_ecran.b, 255);
            SDL_RenderPresent(render);
            SDL_RenderClear(render);
        }

        public static bool GamemodeAction()
        {
            return Gamemode == Gamemode.GAMEPLAY || Gamemode == Gamemode.ARCADE;
        }
        public static bool TouchePesee(Touches touche)
        {
            return (touches_peses & (int)touche) != 0;
        }
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
                    sprites.RemoveAt(i);
                    i--;
                }
            }
        }

        // écran à montrer si le jeu plante. ce code-ci ne devrait jamais donner d'erreure.
        public static void CrashReport(Exception e)
        {
            try
            {
                Son.StopMusic();
                SDL_SetRenderDrawColor(render, 0, 0, 0, 255);
                SDL_RenderClear(render);
                Text.DisplayText("erreure fatale!\n\n"
                    + e.Message + "\n"
                    + e.StackTrace + "\n" +
                    "\n\ntapez sur escape pour quitter l'application.", new Vector2(10, 10), 1
                );
                SDL_SetRenderDrawColor(render, 0, 0, 0, 255);
                SDL_RenderPresent(render);
                while (!exit)
                {
                    SDL_PollEvent(out Program.e);
                    if (Program.e.type == SDL_EventType.SDL_KEYDOWN)
                        if (Program.e.key.keysym.sym == SDL_Keycode.SDLK_ESCAPE)
                            exit = true;
                }
            }
            finally
            {
                SDL_DestroyRenderer(render);
                SDL_DestroyWindow(window);
                SDL_Quit();
                Environment.Exit(-1);
            }
        }
    }
}