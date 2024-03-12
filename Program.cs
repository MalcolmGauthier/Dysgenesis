/*
 Dysgenesis par Malcolm Gauthier
 Beta v0.1: ~6397 lignes de code
 */
using SDL2;
using System.Data;
using System.Linq.Expressions;
using System.Text;
using static SDL2.SDL;
using static SDL2.SDL_mixer;

namespace Dysgenesis
{
    public struct Vector2
    {
        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        public float x;
        public float y;
    }
    public struct Vector3
    {
        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public float x;
        public float y;
        public float z;
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
    public static class Program
    {
        static readonly SDL_Rect BARRE_HP = new SDL_Rect() { x = 125, y = 15, w = 10, h = 20 };
        static readonly SDL_Rect BARRE_VAGUE = new SDL_Rect() { x = 125, y = 40, w = 100, h = 20 };
        static readonly int[] code_arcade = { 0, (int)Touches.A, (int)Touches.R, (int)Touches.C, (int)Touches.A, (int)Touches.D, (int)Touches.E };
        const int touches_arcade = (int)Touches.A | (int)Touches.R | (int)Touches.C | (int)Touches.A | (int)Touches.D | (int)Touches.E;

        static IntPtr window;
        public static IntPtr render;
        static SDL_Event e;
        public static SDL_Color couleure_fond_ecran = new SDL_Color() { r = 0, g = 0, b = 0, a = 255 };

        public static Player player = new Player();
        public static Curseur curseur = new Curseur();
        public static List<Ennemi> enemies = new List<Ennemi>(30);
        public static List<Item> items = new List<Item>(10);
        public static List<Projectile> projectiles = new List<Projectile>(50);
        public static List<Explosion> explosions = new List<Explosion>(10);
        public static Random RNG = new Random();

        public static Gamemode gamemode = Gamemode.CUTSCENE_INTRO;
        public static ushort level;
        public static ushort nv_continue = 1;
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

        // DEBUG VARS
        static byte debug_count = 0, debug_count_display = 0;
        static long debug_time = DateTime.Now.Ticks;
        public static bool mute_sound = false, free_items = false, cutscene_skip = false,
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

            while (!exit)
            {
                Controlls();
                Code();
                Render();
                if (!fps_unlock)
                    while (frame_time > DateTime.Now.Ticks - 10000000 / Data.G_FPS) { }
                frame_time = DateTime.Now.Ticks;
                gTimer++;

                if (GamemodeAction() || gamemode == Gamemode.TITLESCREEN) //todo: wtf
                    Data.G_FPS = 60;
                else
                    Data.G_FPS = 30;
            }

            SDL_DestroyWindow(window);
            SDL_DestroyRenderer(render);
            SDL_Quit();
        }
        static int Init()
        {
            if (SDL_Init(SDL_INIT_VIDEO | SDL_INIT_AUDIO) != 0) return 1;
            window = SDL_CreateWindow(Data.W_TITLE, SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED, Data.W_LARGEUR, Data.W_HAUTEUR,
                fullscreen ? SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP : 0 | SDL_WindowFlags.SDL_WINDOW_SHOWN | SDL_WindowFlags.SDL_WINDOW_RESIZABLE);
            render = SDL_CreateRenderer(window, -1, SDL_RendererFlags.SDL_RENDERER_ACCELERATED);
            SDL_PollEvent(out e);
            if (SDL_SetRenderDrawBlendMode(render, SDL_BlendMode.SDL_BLENDMODE_BLEND) != 0) return 2;
            SDL_SetRenderDrawColor(render, couleure_fond_ecran.r, couleure_fond_ecran.g, couleure_fond_ecran.b, couleure_fond_ecran.a);
            SDL_RenderPresent(render);
            Etoiles.Spawn(new SDL_Rect() { x = 0, y = 0, w = Data.W_LARGEUR, h = Data.W_HAUTEUR });
            frame_time = DateTime.Now.Ticks;
            if (Son.InitSDLMixer() != 0) return 3;
            SaveLoad.Load();
            SDL_ShowCursor(0);
            return 0;
        }
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
        static void Code()
        {
            if (bouger_etoiles)
                Etoiles.Move();

            if (gamemode == Gamemode.TITLESCREEN)
            {
                if (TouchePesee(Touches.M))
                    timer_generique++;
                else
                    timer_generique = 0;

                if (timer_generique >= 120)
                {
                    gTimer = 0;
                    Son.StopMusic();
                    gamemode = Gamemode.CREDITS;
                    return;
                }
            }

            if (!GamemodeAction())
                return;

            player.Exist();

            for (int i = 0; i < projectiles.Count; i++)
            {
                if (projectiles[i].Exist())
                    i--;
            }

            if (gTimer % (400 / (level + 1)) == (399 / (level + 1)) - 1 &&
                ens_needed > 0 &&
                !player.Mort())
            {
                int verif = enemies.Count;

                if (gamemode == Gamemode.GAMEPLAY)
                    new Ennemi(Level_Data.lvl_list[level][ens_needed - 1], StatusEnnemi.INITIALIZATION);
                else
                    new Ennemi(Level_Data.arcade_ens[ens_needed - 1], StatusEnnemi.INITIALIZATION);
                if (enemies.Count > verif)
                    ens_needed--;
            }

            if (enemies.Count == 0)
            {
                if ((gamemode == Gamemode.GAMEPLAY && ens_killed >= Level_Data.lvl_list[level].Length) ||
                    (gamemode == Gamemode.ARCADE && ens_killed >= Level_Data.arcade_ens.Length))
                {
                    Level_Data.Level_Change();
                }
            }

            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].Exist())
                    i--;
            }

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Exist())
                    i--;
            }
        }
        static void Render()
        {
            if (GamemodeAction())
            {
                Etoiles.Render(Data.S_DENSITY);

                if (VerifBoss())
                {
                    BombePulsar.DessinerBombePulsar(
                        new Vector2(Data.W_SEMI_LARGEUR, Data.W_SEMI_HAUTEUR / 2),
                        (byte)(25 - enemies[0].position.z / 4),
                        BombePulsar.COULEUR_BOMBE,
                        true
                    );
                    BombePulsar.VerifCollision();
                }

                foreach (Ennemi e in enemies)
                    e.RenderObject();

                foreach (Item i in items)
                    i.RenderObject();

                foreach (Projectile p in projectiles)
                    p.RenderObject();

                player.RenderObject();

                if (BombePulsar.HP_bombe <= 0)//todo: enlver goto, peux pas faire return car ça ne render pas
                    goto render;

                if (TouchePesee(Touches.K))
                    Shockwave.Spawn();

                Shockwave.Display();

                for (int i = 0; i < explosions.Count; i++)
                {
                    if (explosions[i].Exist())
                        i--;
                }

                if (player.HP > Player.JOUEUR_MAX_HP)
                    player.HP = Player.JOUEUR_MAX_HP;

                if (player.shockwaves > Player.JOUEUR_MAX_VAGUES)
                    player.shockwaves = Player.JOUEUR_MAX_VAGUES;

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
                int vagues_entiers = (int)MathF.Floor(player.shockwaves);
                float vagues_reste = player.shockwaves % 1.0f;
                SDL_SetRenderDrawColor(render, 0, 255, 255, 255);
                for (int i = vagues_entiers; i > 0; i--)
                {
                    SDL_RenderFillRect(render, ref barre_hud);
                    barre_hud.x += barre_hud.w + 5;
                }
                for (int i = (int)((vagues_reste - (vagues_reste % 0.01f)) * 100); i > 0; i--)
                {
                    int posx = vagues_entiers * 105 + BARRE_VAGUE.x + i;
                    SDL_RenderDrawLine(render, posx, 40, posx, 59);
                }

                Text.DisplayText("    hp:\nvagues:", new Vector2(10, 15), 2);

                if (VerifBoss())
                {//todo: fonction dessiner hp/barres
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
            }
            else if (gamemode == Gamemode.TITLESCREEN)
            {
                if (!arcade_unlock)
                {
                    if (!TouchePesee((Touches)code_arcade[arcade_steps]) && TouchePesee((Touches)code_arcade[arcade_steps + 1]))
                    {
                        arcade_steps++;
                        Son.JouerEffet(ListeAudio.EXPLOSION_ENNEMI);
                        if (arcade_steps >= code_arcade.Length - 1)
                        {
                            arcade_unlock = true;
                            curseur.curseur_max_selection = 3;
                        }
                    }
                    else if (TouchePesee((Touches)touches_arcade - code_arcade[arcade_steps] - code_arcade[arcade_steps + 1]))
                    {
                        arcade_steps = 0;
                    }
                }

                Etoiles.Render(Data.S_DENSITY);

                Text.DisplayText("dysgenesis",
                    new Vector2(Text.CENTRE, Text.CENTRE), 5);
                Text.DisplayText("nouvelle partie",
                    new Vector2(Data.W_SEMI_LARGEUR - 114, Data.W_SEMI_HAUTEUR + 75), 2);
                Text.DisplayText("controles menu: w et s pour bouger le curseur, " +
                    "j pour sélectionner\n\ncontroles globaux: esc. pour quitter, " +
                    "+/- pour monter ou baisser le volume",
                    new Vector2(10, Data.W_HAUTEUR - 40), 1);
                Text.DisplayText("v 0.2 (beta)",
                    new Vector2(Text.CENTRE, Data.W_HAUTEUR - 30), 2);

                if (curseur.curseur_max_selection >= 2)
                    Text.DisplayText("continuer: niveau " + nv_continue,
                    new Vector2(Data.W_SEMI_LARGEUR - 114, Data.W_SEMI_HAUTEUR + 125), 2);
                if (curseur.curseur_max_selection >= 3)
                    Text.DisplayText("arcade",
                    new Vector2(Data.W_SEMI_LARGEUR - 114, Data.W_SEMI_HAUTEUR + 175), 2);

                if (lvl_select && gTimer % 10 == 0)
                {
                    if (TouchePesee(Touches.A) && nv_continue > 1)
                        nv_continue--;
                    else if (TouchePesee(Touches.D) && nv_continue < 20)
                        nv_continue++;
                }

                switch (curseur.Selection())
                {
                    case 0:
                        Son.StopMusic();
                        level = 0;
                        player.Init();
                        gTimer = 0;
                        gamemode = Gamemode.CUTSCENE_START;
                        break;

                    case 1:
                        Son.StopMusic();
                        level = (byte)(nv_continue - 1);
                        ens_killed = Level_Data.lvl_list[level].Length;
                        ens_needed = 0;
                        player.Init();
                        player.HP = 50;
                        player.shockwaves = 0;
                        player.afficher = true;
                        gamemode = Gamemode.GAMEPLAY;
                        break;

                    case 2:
                        level = 0;
                        player.Init();
                        player.afficher = true;
                        Son.JouerMusique(ListeAudio.DCQBPM, true);
                        gamemode = Gamemode.ARCADE;
                        break;

                    default:
                        break;
                }
            }
            else
            {
                switch (gamemode)
                {
                    case Gamemode.CUTSCENE_INTRO:
                        Cutscene.Cut_0();
                        break;
                    case Gamemode.CUTSCENE_START:
                        Cutscene.Cut_1();
                        break;
                    case Gamemode.CUTSCENE_BAD_END:
                        Cutscene.Cut_3();
                        break;
                    case Gamemode.CUTSCENE_GOOD_END:
                        Cutscene.Cut_2();
                        break;
                    case Gamemode.CREDITS:
                        Cutscene.Cut_4();
                        break;
                }
            }

        render:

            if (show_fps)
            {
                if (debug_time < DateTime.Now.Ticks - 10000000) // fps
                {
                    debug_count++;
                    debug_count_display = debug_count;
                    debug_count = 0;
                    debug_time = DateTime.Now.Ticks;
                }
                else
                    debug_count++;
                SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
                Text.DisplayText(debug_count_display.ToString(), new Vector2(1828, 52), 3);
            }
            //Text.DisplayText(gTimer + "", short.MinValue, 40, 2); // afficher gTimer à l'écran

            Son.ChangerVolume();

            SDL_SetRenderDrawColor(render, couleure_fond_ecran.r, couleure_fond_ecran.g, couleure_fond_ecran.b, 255);
            SDL_RenderPresent(render);
            SDL_RenderClear(render);
        }
        public static bool GamemodeAction()
        {
            return gamemode == Gamemode.GAMEPLAY || gamemode == Gamemode.ARCADE;
        }
        public static bool TouchePesee(Touches touche)
        {
            return (touches_peses & (int)touche) != 0;
        }
        public static bool VerifBoss()
        {
            if (enemies.Count != 1)
                return false;

            return enemies[0].type == TypeEnnemi.BOSS;
        }
        public static void CrashReport(Exception e)
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
            SDL_DestroyRenderer(render);
            SDL_DestroyWindow(window);
            SDL_Quit();
        }
    }
}