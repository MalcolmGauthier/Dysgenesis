/*
 Dysgenesis par Malcolm Gauthier
 Beta v0.1: ~6397 lignes de code
 */
using static SDL2.SDL;
using static Dysgenesis.Data;
using static Dysgenesis.Background;
using NAudio.Wave;
using System.Linq.Expressions;

namespace Dysgenesis
{
    class Keys
    {
        public static bool w, a, s, d, l_click, j, k, r, c, e, plus, moins;
    }
    public static class Program
    {
        public static IntPtr window;
        public static IntPtr render;
        public static SDL_Event e;
        public static Player player = new Player();
        public static Enemy[] enemies = new Enemy[30];
        public static Item[] items = new Item[10];
        public static uint gTimer = 0;
        public static bool gTimer_lock = false, exit = false, arcade_unlock = false;
        public static long frame_time;
        public static byte ens_killed = 0, ens_needed = 0, gamemode = 4, arcade_steps = 0, gFade = 0, volume = 8, v_timer = 0;
        public static ushort level = 1, nv_continue = 1;
        public static Random RNG = new Random();
        public static WaveOutEvent bg_music = new WaveOutEvent();
        public static WaveOutEvent cut_music = new WaveOutEvent();
        public static WaveOutEvent[] sfx = new WaveOutEvent[8];

        // DEBUG VARS
        public static byte debug_count = 0, debug_count_display = 0;
        public static long debug_time = DateTime.Now.Ticks;
        public static bool mute_music = false, free_items = false, cutscene_skip = false, 
                           show_fps = false, monologue_skip = false, lvl_select = false;
        static void Main()
        {
            Init();
            while (!exit)
            {
                Controlls();
                Code();
                Render();
                while (frame_time > DateTime.Now.Ticks - (gamemode == 2 || gamemode == 3 || gamemode == 0 ? 10000000 : 20000000) / G_FPS) { }
                frame_time = DateTime.Now.Ticks;
                gTimer++;
            }
            SDL_DestroyWindow(window);
            SDL_DestroyRenderer(render);
            SDL_Quit();
        }
        static void Init()
        {
            try
            {
                window = SDL_CreateWindow(W_TITLE, SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED, W_LARGEUR, W_HAUTEUR,
                 SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP | SDL_WindowFlags.SDL_WINDOW_SHOWN);
                render = SDL_CreateRenderer(window, -1, SDL_RendererFlags.SDL_RENDERER_ACCELERATED);
                TailleEcran();
                SDL_Init(SDL_INIT_VIDEO);
                SDL_PollEvent(out e);
                SDL_SetRenderDrawBlendMode(render, SDL_BlendMode.SDL_BLENDMODE_BLEND);
                SDL_SetRenderDrawColor(render, 0, 0, 0, 255);
                SDL_RenderPresent(render);
                Etoiles.Init();
                Level_Data.Init();
                Explosion.Init();
                SDL_ShowCursor(SDL_DISABLE);
                frame_time = DateTime.Now.Ticks;
                for (int i = 0; i < sfx.Length; i++)
                {
                    sfx[i] = new WaveOutEvent();
                }
                Son.LoadMusic();
                Son.LoadSFX();
                volume = 4;
                bg_music.Volume = 0.25f;
                cut_music.Volume = 0.25f;
                for (byte i = 0; i < sfx.Length; i++)
                {
                    sfx[i].Volume = 0.25f;
                }
                try
                {
                    SaveLoad.Load();
                }
                catch (Exception _)
                {
                    return;
                }
                if (nv_continue != 1)
                    player.x = -1;
            }
            catch (Exception ex)
            {
                CrashReport(ex);
            }
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
                                Keys.w = true;
                                break;
                            case SDL_Keycode.SDLK_a:
                                Keys.a = true;
                                break;
                            case SDL_Keycode.SDLK_s:
                                Keys.s = true;
                                break;
                            case SDL_Keycode.SDLK_d:
                                Keys.d = true;
                                break;
                            case SDL_Keycode.SDLK_j:
                                Keys.j = true;
                                break;
                            case SDL_Keycode.SDLK_k:
                                Keys.k = true;
                                break;
                            case SDL_Keycode.SDLK_r:
                                Keys.r = true;
                                break;
                            case SDL_Keycode.SDLK_c:
                                Keys.c = true;
                                break;
                            case SDL_Keycode.SDLK_e:
                                Keys.e = true;
                                break;
                            case SDL_Keycode.SDLK_EQUALS:
                                Keys.plus = true;
                                break;
                            case SDL_Keycode.SDLK_MINUS:
                                Keys.moins = true;
                                break;
                        }
                        break;
                    case SDL_EventType.SDL_KEYUP:
                        switch (e.key.keysym.sym)
                        {
                            case SDL_Keycode.SDLK_w:
                                Keys.w = false;
                                break;
                            case SDL_Keycode.SDLK_a:
                                Keys.a = false;
                                break;
                            case SDL_Keycode.SDLK_s:
                                Keys.s = false;
                                break;
                            case SDL_Keycode.SDLK_d:
                                Keys.d = false;
                                break;
                            case SDL_Keycode.SDLK_j:
                                Keys.j = false;
                                break;
                            case SDL_Keycode.SDLK_k:
                                Keys.k = false;
                                break;
                            case SDL_Keycode.SDLK_r:
                                Keys.r = false;
                                break;
                            case SDL_Keycode.SDLK_c:
                                Keys.c = false;
                                break;
                            case SDL_Keycode.SDLK_e:
                                Keys.e = false;
                                break;
                            case SDL_Keycode.SDLK_EQUALS:
                                Keys.plus = false;
                                break;
                            case SDL_Keycode.SDLK_MINUS:
                                Keys.moins = false;
                                break;
                        }
                        break;
                    case SDL_EventType.SDL_MOUSEBUTTONDOWN:
                        if (e.button.button == SDL_BUTTON_LEFT)
                        {
                            Keys.l_click = true;
                        }
                        break;
                    case SDL_EventType.SDL_MOUSEBUTTONUP:
                        if (e.button.button == SDL_BUTTON_LEFT)
                        {
                            Keys.l_click = false;
                        }
                        break;
                }
            }
        }
        static void Code()
        {
            try
            {
                if (gamemode == 2 && level == 20 && enemies[0] != null)
                {
                    if (enemies[0].depth != 20)
                        Etoiles.Exist();
                }
                else
                    Etoiles.Exist();

                if (Keys.a && lvl_select && gTimer % 10 == 0 && nv_continue > 1)
                    nv_continue--;
                if (Keys.d && lvl_select && gTimer % 10 == 0 && nv_continue < 20)
                    nv_continue++;

                if (gamemode != 2 && gamemode != 3)
                    return;

                player.Exist();

                if (player.HP > 0)
                {
                    Projectile.Exist();
                }

                if (gTimer % (100 + 100 / (level + 1)) == (99 + 100 / (level + 1)) && ens_needed > 0 && !player.dead)
                {
                    for (int i = 0; i < enemies.Length; i++)
                    {
                        if (enemies[i] == null)
                        {
                            if (gamemode == 2)
                                enemies[i] = new Enemy(Level_Data.lvl_list[level][ens_needed - 1]);
                            else
                                enemies[i] = new Enemy(Level_Data.arcade_ens[ens_needed - 1]);
                            ens_needed--;
                            break;
                        }
                    }
                }

                if (gamemode == 2)
                    if (ens_killed == Level_Data.lvl_list[level].Length && gamemode == 2 && IsEmpty(enemies))
                        Level_Data.Level_Change();

                if (gamemode == 2)
                {
                    if (level == 20 && enemies[0] != null && !player.dead)
                    {
                        if (enemies[0].special == 99 && enemies[0].HP > 0 && BombePulsar.HP_bombe > 0)
                            Son.LoopBGSong(Son.Music_list.dotv);
                    }
                    //else if (level != 20 && !player.dead)     // j'ai longtemps essayé de chercher une musique de gameplay régulier, mais
                    //    Son.LoopBGSong(Son.Music_list.stone); // rien de ce que j'ai trouvé me satisfait. j'ai faillit mettre ceci
                }                                               // (stone cold par leon riskin) comme musique, mais je l'ai enlevé au dernier
                else                                            // moment. J'ai décidé que je préfère le silence.
                {
                    if (!player.dead)
                        Son.LoopBGSong(Son.Music_list.dcq_pbm);
                }

                if (gamemode == 3)
                {
                    if (ens_killed == Level_Data.arcade_ens.Length)
                        Level_Data.Level_Change();
                }

                for (int i = 0; i < enemies.Length; i++)
                {
                    if (enemies[i] != null)
                        enemies[i].Exist(ref enemies[i]);
                }

                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i] != null)
                        items[i].Exist(ref items[i]);
                }
            }
            catch (Exception ex)
            {
                CrashReport(ex);
            }
        }
        static void Render()
        {
            switch (gamemode)
            {
                case 0:
                    Render_Menu();
                    break;
                case 1:
                    Render_Cutscene(1);
                    break;
                case 2:
                case 3:
                    Render_Gameplay();
                    break;
                case 4:
                    Render_Cutscene(0);
                    break;
                case 5:
                    Render_Cutscene(2);
                    break;
                case 6:
                    Render_Cutscene(3);
                    break;
                case 7:
                    Render_Cutscene(4);
                    break;
            }

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
                Text.DisplayText(debug_count_display.ToString(), 1858, 22, 3);
            }
            //Text.DisplayText(gTimer + "", short.MinValue, 40, 2); // afficher gTimer à l'écran

            // volume
            if (v_timer != 0)
                v_timer--;
            if (Keys.plus || Keys.moins || v_timer != 0)
            {
                SDL_Rect vol = new SDL_Rect() { x = 1560, y = 10, w = 350, h = 100 };
                if (Keys.plus && v_timer < 25)
                {
                    Son.AugmenterVolume();
                    v_timer = 30;
                }
                if (Keys.moins && v_timer < 25)
                {
                    Son.BaisserVolume();
                    v_timer = 30;
                }
                SDL_SetRenderDrawColor(render, 0, 0, 0, 255);
                SDL_RenderFillRect(render, ref vol);
                SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
                SDL_RenderDrawLine(render, vol.x, vol.y, vol.x + vol.w, vol.y);
                SDL_RenderDrawLine(render, vol.x + vol.w, vol.y, vol.x + vol.w, vol.y + vol.h);
                SDL_RenderDrawLine(render, vol.x + vol.w, vol.y + vol.h, vol.x, vol.y + vol.h);
                SDL_RenderDrawLine(render, vol.x, vol.y + vol.h, vol.x, vol.y);
                Text.DisplayText("volume: " + volume, 1600, 40, 3);
            }

            SDL_SetRenderDrawColor(render, 0, 0, 0, 255);
            SDL_RenderPresent(render);
            SDL_RenderClear(render);
        }
        static void Render_Menu()
        {
            if (!arcade_unlock)
            {
                switch (arcade_steps)
                {
                    case 0:
                        if (Keys.a)
                            arcade_steps++;
                        break;
                    case 1:
                        if (Keys.r && !Keys.a)
                        {
                            arcade_steps++;
                            Son.PlaySFX(Son.SFX_list.e_boom);
                        }
                        if (Keys.d || Keys.c || Keys.w || Keys.s || Keys.j || Keys.k || Keys.e)
                            arcade_steps = 0;
                        break;
                    case 2:
                        if (Keys.c && !Keys.r)
                        {
                            arcade_steps++;
                            Son.PlaySFX(Son.SFX_list.e_boom);
                        }
                        if (Keys.d || Keys.a || Keys.w || Keys.s || Keys.j || Keys.k || Keys.e)
                            arcade_steps = 0;
                        break;
                    case 3:
                        if (Keys.a && !Keys.c)
                        {
                            arcade_steps++;
                            Son.PlaySFX(Son.SFX_list.e_boom);
                        }
                        if (Keys.d || Keys.r || Keys.w || Keys.s || Keys.j || Keys.k || Keys.e)
                            arcade_steps = 0;
                        break;
                    case 4:
                        if (Keys.d && !Keys.a)
                        {
                            arcade_steps++;
                            Son.PlaySFX(Son.SFX_list.e_boom);
                        }
                        if (Keys.r || Keys.c || Keys.w || Keys.s || Keys.j || Keys.k || Keys.e)
                            arcade_steps = 0;
                        break;
                    case 5:
                        if (Keys.e && !Keys.d)
                        {
                            arcade_unlock = true;
                            Son.PlaySFX(Son.SFX_list.e_boom);
                        }
                        if (Keys.r || Keys.c || Keys.w || Keys.s || Keys.j || Keys.k || Keys.a)
                            arcade_steps = 0;
                        break;
                }
            }

            SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
            for (int i = 0; i < S_DENSITY; i++)
            {
                SDL_RenderDrawPoint(render, (int)Etoiles.star_positions[i, 0], (int)Etoiles.star_positions[i, 1]);
            }
            byte options = 1;
            Text.DisplayText("dysgenesis", short.MinValue, short.MinValue, 5);
            Text.DisplayText("nouvelle partie", (short)(W_SEMI_LARGEUR - 114), (short)(W_SEMI_HAUTEUR + 75), 2);
            if (player.x == -1 || arcade_unlock)
            {
                Text.DisplayText("continuer: niveau " + nv_continue, (short)(W_SEMI_LARGEUR - 114), (short)(W_SEMI_HAUTEUR + 125), 2);
                options++;
            }
            if (arcade_unlock)
            {
                Text.DisplayText("arcade", (short)(W_SEMI_LARGEUR - 114), (short)(W_SEMI_HAUTEUR + 175), 2);
                options++;
            }
            Text.DisplayText("controles menu: w et s pour bouger le curseur, j pour sélectionner\n\n" +
                             "controles globaux: esc. pour quitter, +/- pour monter ou baisser le volume", 10, (short)(W_HAUTEUR - 40), 1);
            Text.DisplayText("v 0.1 (beta)", short.MinValue, (short)(W_HAUTEUR - 30), 2);
            Curseur.SetVars(options, (short)(W_SEMI_LARGEUR - 150), (short)(W_SEMI_HAUTEUR + 35), 50);
            Curseur.Exist();
            Son.LoopBGSong(Son.Music_list.title);
            if (Curseur.returned_sel != -1)
            {
                Son.StopMusic();
                gamemode = (byte)Curseur.returned_sel;
                if (gamemode == 1 || gamemode == 3)
                {
                    level = 0;
                    player.HP = 100;
                }
                if (gamemode == 2)
                {
                    level = (ushort)(nv_continue - 1);
                    player.HP = 49;
                    player.shockwaves = 0;
                    player.x = W_SEMI_LARGEUR;
                    player.y = W_HAUTEUR - 30;
                    player.roll = 0;
                    player.vx = 0;
                    player.vy = -30;
                    player.scale = 1;
                    ens_needed = 0;
                    ens_killed = (byte)Level_Data.lvl_list[level].Length;
                    Projectile.cooldown = gTimer;
                }
                if (gamemode == 3)
                {
                    player.HP = 100;
                    player.shockwaves = 3;
                    player.powerup = 0;
                    ens_killed = 0;
                    ens_needed = 0;
                    player.x = W_SEMI_LARGEUR;
                    player.y = W_HAUTEUR - 30;
                    player.roll = 0;
                    player.vx = 0;
                    player.vy = -30;
                    player.scale = 1;
                    Projectile.cooldown = gTimer;
                }
            }
        }
        static void Render_Cutscene(byte cutscene_num)
        {
            try
            {
                if (!gTimer_lock)
                {
                    gTimer_lock = true;
                    gTimer = 0;
                }
                switch (cutscene_num)
                {
                    case 0:
                        Cutscene.Cut_0();
                        break;
                    case 1:
                        Cutscene.Cut_1();
                        break;
                    case 2:
                        Cutscene.Cut_2();
                        break;
                    case 3:
                        Cutscene.Cut_3();
                        break;
                    case 4:
                        Cutscene.Cut_4();
                        break;
                }
            }
            catch (Exception ex)
            {
                CrashReport(ex);
            }
        }
        static void Render_Gameplay()
        {
            SDL_SetRenderDrawColor(render, 255, 255, 255, 255);
            for (int i = 0; i < S_DENSITY; i++)
            {
                SDL_RenderDrawPoint(render, (int)Etoiles.star_positions[i, 0], (int)Etoiles.star_positions[i, 1]);
            }

            if (gamemode == 2 && level == 20 && enemies[0] != null)
            {
                BombePulsar.Niveau20();
            }

            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i] != null)
                    enemies[i].RenderEnemy();
            }

            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] != null)
                    items[i].Render();
            }

            if (BombePulsar.HP_bombe > 0)
                Projectile.Render();

            player.Render();

            if (BombePulsar.HP_bombe <= 0)
                return;

            if (Keys.k)
                Shockwave.Spawn();
            Shockwave.Display();

            Explosion.Draw();

            if (player.HP > 150)
                player.HP = 150;

            Text.DisplayText("    hp:", 10, 15, 2);
            player.HP_BAR.y = 15;
            for (int i = player.HP; i > 0; i--)
            {
                if (i < 20)
                    SDL_SetRenderDrawColor(render, 255, 0, 0, 255);
                else if (i < 50)
                    SDL_SetRenderDrawColor(render, 255, 255, 0, 255);
                else if (i < 101)
                    SDL_SetRenderDrawColor(render, 0, 255, 0, 255);
                else
                    SDL_SetRenderDrawColor(render, 0, 0, 255, 255);
                player.HP_BAR.x = i * 11 + 114;
                SDL_RenderFillRect(render, ref player.HP_BAR);
            }

            Text.DisplayText("vagues:", 10, 40, 2);
            SDL_SetRenderDrawColor(render, 0, 255, 255, 255);
            if (player.shockwaves >= 3)
                player.shockwaves = 3;
            for (int i = (int)Math.Floor(player.shockwaves); i > 0; i--)
            {
                player.SHOCK_BAR.x = i * 105 + 20;
                SDL_RenderFillRect(render, ref player.SHOCK_BAR);
            }
            for (int i = 0; i < (int)(Math.Round(player.shockwaves % 1, 2) * 100); i++)
            {
                SDL_RenderDrawLine(render, (int)Math.Floor(player.shockwaves) * 105 + 125 + i, 40, (int)Math.Floor(player.shockwaves) * 105 + 125 + i, 59);
            }

            if (gamemode == 2 && level == 20 && enemies[0] != null)
            {
                Text.DisplayText("    hp:\nennemi", 10, 85, 2);
                player.HP_BAR.y = 85;
                for (int i = enemies[0].HP; i > 0; i--)
                {
                    if (i < 20)
                        SDL_SetRenderDrawColor(render, 255, 0, 0, 255);
                    else if (i < 50)
                        SDL_SetRenderDrawColor(render, 255, 255, 0, 255);
                    else
                        SDL_SetRenderDrawColor(render, 0, 255, 0, 255);
                    player.HP_BAR.x = i * 11 + 114;
                    SDL_RenderFillRect(render, ref player.HP_BAR);
                }
            }

            if (gamemode == 2 && level == 1 && ens_killed < Level_Data.lvl_list[1].Length)
            {
                Text.DisplayText("controles du vaisseau:\n" +
                                 "    wasd pour bouger\n" +
                                 "    j pour tirer\n" +
                                 "    k pour envoyer une vague électrique", 600, 800, 2);
            }

            if (gFade != 0)
            {
                Cutscene.rect.x = 0; Cutscene.rect.y = 0; Cutscene.rect.w = W_LARGEUR; Cutscene.rect.h = W_HAUTEUR;
                SDL_SetRenderDrawColor(render, 0, 0, 0, gFade);
                SDL_RenderFillRect(render, ref Cutscene.rect);
            }
        }
        public static bool IsEmpty(Enemy[] enemies)
        {
            foreach (Enemy enemy in enemies)
            {
                if (enemy != null)
                    return false;
            }
            return true;
        }
        public static void CrashReport(Exception e)
        {
            Son.StopMusic();
            SDL_SetRenderDrawColor(render, 0, 0, 0, 255);
            SDL_RenderClear(render);
            Text.DisplayText("erreure fatale!\n\n" 
                + e.Message + "\n"
                + e.StackTrace + "\n"
                + e.TargetSite.ToString() +
                "\n\ntapez sur escape pour quitter l'application.", 10, 10, 1);
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