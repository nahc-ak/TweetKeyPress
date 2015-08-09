using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TweetKeyPress
{
    partial class Program
    {
        // Program.csのProgramクラスを分割
        // フックに関する処理はこっちのファイルに書く（長いので）

        private const int WH_MOUSE_LL = 14;
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x100;
        private const int WM_KEYUP = 0x101;
        private const int XBUTTON1 = 0x1;
        private const int XBUTTON2 = 0x2;

        private static IntPtr _hookID_mouse = IntPtr.Zero;
        private static IntPtr _hookID_keyboard = IntPtr.Zero;
        private static LowLevelMouseProc _proc_mouse = HookCallback_mouse;
        private static LowLevelKeyboardProc _proc_keyboard = HookCallback_keyboard;

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        private static IntPtr SetHook(LowLevelMouseProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_MOUSE_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }
        
        // マウスが押されたとき、対応するボタンのカウントを増やす処理
        private static IntPtr HookCallback_mouse(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && MouseMessages.WM_LBUTTONDOWN == (MouseMessages)wParam)
            {
                // 左ボタン
                myDataTable.CountUp("Left_MouseButton");
            }
            if (nCode >= 0 && MouseMessages.WM_MBUTTONDOWN == (MouseMessages)wParam)
            {
                // 中央のボタン
                myDataTable.CountUp("Middle_MouseButton");
            }
            if (nCode >= 0 && MouseMessages.WM_RBUTTONDOWN == (MouseMessages)wParam)
            {
                // 右ボタン
                myDataTable.CountUp("Right_MouseButton");
            }
            if (nCode >= 0 && MouseMessages.WM_XBUTTONDOWN == (MouseMessages)wParam)
            {
                // X1、X2ボタン
                MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                //Console.WriteLine(hookStruct.pt.x + ", " + hookStruct.pt.y);
                switch (hookStruct.mouseData >> 16)
                {
                    case XBUTTON1:
                        myDataTable.CountUp("X1_MouseButton");
                        break;
                    case XBUTTON2:
                        myDataTable.CountUp("X2_MouseButton");
                        break;
                }
            }
            return CallNextHookEx(_hookID_mouse, nCode, wParam, lParam);
        }

        // キーボードのキーが押された・離された時の処理
        private static IntPtr HookCallback_keyboard(int nCode, IntPtr wParam, IntPtr lParam)
        {
            // キーが離されたとき
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYUP)
            {
                // ほとんどのキーは、押して離すとキーが離されたことを示すのでWM_KEYUPでカウントアップする。
                int vkCode = Marshal.ReadInt32(lParam);
                myDataTable.CountUp(((VKeys)vkCode).ToString());
            }
            // キーが押されたとき
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                // WM_KEYDOWNしか出さない？キー
                // 英数、カタカナひらがな（KATAKANAとHIRAGANAで交互）、半角全角（SBCとDBCで交互）
                // ToDo カタカナひらがなと半角全角のカウント方法を考える
                if (((VKeys)vkCode).ToString() == "VK_DBE_ALPHANUMERIC")
                {
                    myDataTable.CountUp(((VKeys)vkCode).ToString());
                }
            }
            return CallNextHookEx(_hookID_keyboard, nCode, wParam, lParam);
        }

        // Twitterで呟く時のキーの名前     VKey,日本語 の対
        public static Dictionary<string, string> TweetVKeys = new Dictionary<string, string>()
        {
            {"Date", "日付"},
            {"Left_MouseButton", "マウスの左ボタン"}, // Left mouse button
            {"Right_MouseButton", "マウスの右ボタン"}, // Right mouse button
            {"Break", "Breakキー"}, // Control-break processing // Ctrl+Pause
            {"Middle_MouseButton", "マウスの中央ボタン"}, // Middle mouse button (three-button mouse)
            {"X1_MouseButton", "マウスのX1ボタン"}, // X1 mouse button
            {"X2_MouseButton", "マウスのX2ボタン"}, // X2 mouse button
            {"Undefined_1", "Undefined_1"}, // Undefined
            {"Backspace", "Backspaceキー"}, // BACKSPACE key
            {"Tab", "Tabキー"}, // TAB key
            {"Reserved_1", "Reserved_1"}, // Reserved
            {"Reserved_2", "Reserved_2"}, // Reserved
            {"Clear", "Clearキー（テンキーのNumLockを解除した状態の「５」キー）"}, // CLEAR key
            {"Enter", "Enterキー"}, // ENTER key
            {"Undefined_2", "Undefined_2"}, // Undefined
            {"Undefined_3", "Undefined_3"}, // Undefined
            {"Shift", "Shiftキー"}, // SHIFT key
            {"Ctrl", "Ctrlキー"}, // CTRL key
            {"Alt", "Altキー"}, // ALT key
            {"Pause", "Pauseキー"}, // PAUSE key
            {"CapsLock", "CapsLockキー"}, // CAPS LOCK key // Shift+英数キー
            {"IME_Kana", "IME_Kana"}, // IME Kana mode
            {"Undefined_4", "Undefined_4"}, // Undefined
            {"IME_Junja", "IME_Junja"}, // IME Junja mode
            {"IME_final", "IME_final"}, // IME final mode
            {"IME_Hanja", "IME_Hanja"}, // IME Hanja mode
            {"IME_Kanji", "IME_Kanji"}, // IME Kanji mode
            {"Undefined_5", "Undefined_5"}, // Undefined
            {"Esc", "Escキー"}, // ESC key
            {"IME_Convert", "変換キー"}, // IME convert
            {"IME_NonConvert", "無変換キー"}, // IME nonconvert
            {"IME_Accept", "IME_Accept"}, // IME accept
            {"IME_ModeChange", "IME_ModeChange"}, // IME mode change request
            {"Space", "スペースキー"}, // SPACEBAR
            {"PageUp", "PageUpキー"}, // PAGE UP key
            {"PageDown", "PageDownキー"}, // PAGE DOWN key
            {"End", "Endキー"}, // END key
            {"Home", "Homeキー"}, // HOME key
            {"Left", "「←」キー"}, // LEFT ARROW key
            {"Up", "「↑」キー"}, // UP ARROW key
            {"Right", "「→」キー"}, // RIGHT ARROW key
            {"Down", "「↓」キー"}, // DOWN ARROW key
            {"Select", "Selectキー"}, // SELECT key
            {"Print", "Printキー"}, // PRINT key
            {"Execute", "Executeキー"}, // EXECUTE key
            {"PrintScreen", "PrintScreenキー"}, // PRINT SCREEN key
            {"Insert", "Insertキー"}, // INS key
            {"Delete", "Deleteキー"}, // DEL key
            {"Help", "Helpキー"}, // HELP key
            {"Key_0", "数字の0キー"}, // 0 Key_
            {"Key_1", "数字の1キー"}, // 1 Key_
            {"Key_2", "数字の2キー"}, // 2 Key_
            {"Key_3", "数字の3キー"}, // 3 Key_
            {"Key_4", "数字の4キー"}, // 4 Key_
            {"Key_5", "数字の5キー"}, // 5 Key_
            {"Key_6", "数字の6キー"}, // 6 Key_
            {"Key_7", "数字の7キー"}, // 7 Key_
            {"Key_8", "数字の8キー"}, // 8 Key_
            {"Key_9", "数字の9キー"}, // 9 Key_
            {"Undefined_6", "Undefined_6"}, // Undefined
            {"Undefined_7", "Undefined_7"}, // Undefined
            {"Undefined_8", "Undefined_8"}, // Undefined
            {"Undefined_9", "Undefined_9"}, // Undefined
            {"Undefined_10", "Undefined_10"}, // Undefined
            {"Undefined_11", "Undefined_11"}, // Undefined
            {"Undefined_12", "Undefined_12"}, // Undefined
            {"Key_A", "Aキー"}, // A key
            {"Key_B", "Bキー"}, // B key
            {"Key_C", "Cキー"}, // C key
            {"Key_D", "Dキー"}, // D key
            {"Key_E", "Eキー"}, // E key
            {"Key_F", "Fキー"}, // F key
            {"Key_G", "Gキー"}, // G key
            {"Key_H", "Hキー"}, // H key
            {"Key_I", "Iキー"}, // I key
            {"Key_J", "Jキー"}, // J key
            {"Key_K", "Kキー"}, // K key
            {"Key_L", "Lキー"}, // L key
            {"Key_M", "Mキー"}, // M key
            {"Key_N", "Nキー"}, // N key
            {"Key_O", "Oキー"}, // O key
            {"Key_P", "Pキー"}, // P key
            {"Key_Q", "Qキー"}, // Q key
            {"Key_R", "Rキー"}, // R key
            {"Key_S", "Sキー"}, // S key
            {"Key_T", "Tキー"}, // T key
            {"Key_U", "Uキー"}, // U key
            {"Key_V", "Vキー"}, // V key
            {"Key_W", "Wキー"}, // W key
            {"Key_X", "Xキー"}, // X key
            {"Key_Y", "Yキー"}, // Y key
            {"Key_Z", "Zキー"}, // Z key
            {"Left_Windows", "左のWindowsキー"}, // Left Windows key (Natural keyboard)
            {"Right_Windows", "右のWindowsキー"}, // Right Windows key (Natural keyboard)
            {"Applications", "アプリケーションキー"}, // Applications key (Natural keyboard)
            {"Reserved_3", "Reserved_3"}, // Reserved
            {"Sleep", "キーボードのスリープキー"}, // Computer Sleep key
            {"Numpad_0", "テンキーの0キー"}, // Numeric keypad 0 key
            {"Numpad_1", "テンキーの1キー"}, // Numeric keypad 1 key
            {"Numpad_2", "テンキーの2キー"}, // Numeric keypad 2 key
            {"Numpad_3", "テンキーの3キー"}, // Numeric keypad 3 key
            {"Numpad_4", "テンキーの4キー"}, // Numeric keypad 4 key
            {"Numpad_5", "テンキーの5キー"}, // Numeric keypad 5 key
            {"Numpad_6", "テンキーの6キー"}, // Numeric keypad 6 key
            {"Numpad_7", "テンキーの7キー"}, // Numeric keypad 7 key
            {"Numpad_8", "テンキーの8キー"}, // Numeric keypad 8 key
            {"Numpad_9", "テンキーの9キー"}, // Numeric keypad 9 key
            {"Multiply", "テンキーの*キー"}, // Multiply key
            {"Add", "テンキーの+キー"}, // Add key // プラス
            {"Separator", "テンキーのカンマキー"}, // Separator key // カンマ
            {"Subtract", "テンキーのマイナスキー"}, // Subtract key // マイナス
            {"Decimal", "テンキーの.キー"}, // Decimal key // 小数点
            {"Divide", "テンキーの/キー"}, // Divide key // 割る
            {"F1", "F1キー"}, // F1 key
            {"F2", "F2キー"}, // F2 key
            {"F3", "F3キー"}, // F3 key
            {"F4", "F4キー"}, // F4 key
            {"F5", "F5キー"}, // F5 key
            {"F6", "F6キー"}, // F6 key
            {"F7", "F7キー"}, // F7 key
            {"F8", "F8キー"}, // F8 key
            {"F9", "F9キー"}, // F9 key
            {"F10", "F10キー"}, // F10 key
            {"F11", "F11キー"}, // F11 key
            {"F12", "F12キー"}, // F12 key
            {"F13", "F13キー"}, // F13 key
            {"F14", "F14キー"}, // F14 key
            {"F15", "F15キー"}, // F15 key
            {"F16", "F16キー"}, // F16 key
            {"F17", "F17キー"}, // F17 key
            {"F18", "F18キー"}, // F18 key
            {"F19", "F19キー"}, // F19 key
            {"F20", "F21キー"}, // F20 key
            {"F21", "F22キー"}, // F21 key
            {"F22", "F23キー"}, // F22 key
            {"F23", "F24キー"}, // F23 key
            {"F24", "F25キー"}, // F24 key
            {"Unassigned_1", "Unassigned_1"}, // Unassigned
            {"Unassigned_2", "Unassigned_2"}, // Unassigned
            {"Unassigned_3", "Unassigned_3"}, // Unassigned
            {"Unassigned_4", "Unassigned_4"}, // Unassigned
            {"Unassigned_5", "Unassigned_5"}, // Unassigned
            {"Unassigned_6", "Unassigned_6"}, // Unassigned
            {"Unassigned_7", "Unassigned_7"}, // Unassigned
            {"Unassigned_8", "Unassigned_8"}, // Unassigned
            {"NumLock", "NumLockキー"}, // NUM LOCK key
            {"ScrollLock", "ScrollLockキー"}, // SCROLL LOCK key
            {"OEM_specific_1", "OEM_specific_1"}, // OEM specific
            {"OEM_specific_2", "OEM_specific_2"}, // OEM specific
            {"OEM_specific_3", "OEM_specific_3"}, // OEM specific
            {"OEM_specific_4", "OEM_specific_4"}, // OEM specific
            {"OEM_specific_5", "OEM_specific_5"}, // OEM specific
            {"Unassigned_9", "Unassigned_9"}, // Unassigned
            {"Unassigned_10", "Unassigned_10"}, // Unassigned
            {"Unassigned_11", "Unassigned_11"}, // Unassigned
            {"Unassigned_12", "Unassigned_12"}, // Unassigned
            {"Unassigned_13", "Unassigned_13"}, // Unassigned
            {"Unassigned_14", "Unassigned_14"}, // Unassigned
            {"Unassigned_15", "Unassigned_15"}, // Unassigned
            {"Unassigned_16", "Unassigned_16"}, // Unassigned
            {"Unassigned_17", "Unassigned_17"}, // Unassigned
            {"Left_Shift", "左のShiftキー"}, // Left SHIFT key
            {"Right_Shift", "右のShiftキー"}, // Right SHIFT key
            {"Left_Control", "左のCtrlキー"}, // Left CONTROL key
            {"Right_Control", "右のCtrlキー"}, // Right CONTROL key
            {"Left_Alt", "左のAltキー"}, // Left MENU key
            {"Right_Alt", "右のAltキー"}, // Right MENU key
            {"Browser_Back", "キーボードのブラウザバックボタン"}, // Browser Back key
            {"Browser_Forward", "キーボードのブラウザ進むボタン"}, // Browser Forward key
            {"Browser_Refresh", "キーボードのブラウザ更新ボタン"}, // Browser Refresh key
            {"Browser_Stop", "キーボードのブラウザ中止ボタン"}, // Browser Stop key
            {"Browser_Search", "キーボードのブラウザ検索ボタン"}, // Browser Search key
            {"Brouser_Favorites", "キーボードのブラウザお気に入りボタン"}, // Browser Favorites key
            {"Browser_Start", "キーボードのブラウザボタン"}, // Browser Start and Home key
            {"Volume_Mute", "キーボードの音量ミュートキー"}, // Volume Mute key
            {"Volume_Down", "キーボードの音量ダウン"}, // Volume Down key
            {"Volume_Up", "キーボードの音量アップ"}, // Volume Up key
            {"Media_Next_Track", "キーボードの次のトラックキー"}, // Next Track key
            {"Media_Previous_Track", "キーボードの前のトラックキー"}, // Previous Track key
            {"Media_Stop", "キーボードの再生ストップキー"}, // Stop Media key
            {"Media_Play_Pause", "キーボードの再生（一時停止）キー"}, // Play/Pause Media key
            {"Launch_Mail", "キーボードのメールキー"}, // Start Mail key
            {"Launch_Media_Select", "キーボードのメディアセレクトキー"}, // Select Media key
            {"Launch_App1", "キーボードのアプリ起動１キー"}, // Start Application 1 key
            {"Launch_App2", "キーボードのアプリ起動２キー"}, // Start Application 2 key
            {"Reserved_4", "Reserved_4"}, // Reserved
            {"Reserved_5", "Reserved_5"}, // Reserved
            {"Semicolon_Colon", "コロンのキー"}, // Used for miscellaneous characters; it can vary by keyboard."For the US standard keyboard, the ';:' key" // : OEM_1
            {"Plus", "プラスのキー"}, // "For any country/region, the '+' key"
            {"Comma", "カンマのキー"}, // "For any country/region, the ',' key"
            {"Minus", "マイナスのキー"}, // "For any country/region, the '-' key"
            {"Period", "ピリオドのキー"}, // "For any country/region, the '.' key"
            {"Slash_Questionmark", "？のキー"}, // Used for miscellaneous characters; it can vary by keyboard."For the US standard keyboard, the '/?' key" // / OEM2
            {"Graveaccent_Atsign", "＠のキー"}, // "For the US standard keyboard, the '`~' key" // `@ OEM_3
            {"Reserved_6", "Reserved_6"}, // Reserved
            {"Reserved_7", "Reserved_7"}, // Reserved
            {"Reserved_8", "Reserved_8"}, // Reserved
            {"Reserved_9", "Reserved_9"}, // Reserved
            {"Reserved_10", "Reserved_10"}, // Reserved
            {"Reserved_11", "Reserved_11"}, // Reserved
            {"Reserved_12", "Reserved_12"}, // Reserved
            {"Reserved_13", "Reserved_13"}, // Reserved
            {"Reserved_14", "Reserved_14"}, // Reserved
            {"Reserved_15", "Reserved_15"}, // Reserved
            {"Reserved_16", "Reserved_16"}, // Reserved
            {"Reserved_17", "Reserved_17"}, // Reserved
            {"Reserved_18", "Reserved_18"}, // Reserved
            {"Reserved_19", "Reserved_19"}, // Reserved
            {"Reserved_20", "Reserved_20"}, // Reserved
            {"Reserved_21", "Reserved_21"}, // Reserved
            {"Reserved_22", "Reserved_22"}, // Reserved
            {"Reserved_23", "Reserved_23"}, // Reserved
            {"Reserved_24", "Reserved_24"}, // Reserved
            {"Reserved_25", "Reserved_25"}, // Reserved
            {"Reserved_26", "Reserved_26"}, // Reserved
            {"Reserved_27", "Reserved_27"}, // Reserved
            {"Reserved_28", "Reserved_28"}, // Reserved
            {"Unassigned_18", "Unassigned_18"}, // Unassigned
            {"Unassigned_19", "Unassigned_19"}, // Unassigned
            {"Unassigned_20", "Unassigned_20"}, // Unassigned
            {"Open_Bracket", "「のキー"}, // Used for miscellaneous characters; it can vary by keyboard."For the US standard keyboard, the '[{' key" // [ OEM_4
            {"Backslash_Verticalbar", "￥/｜のキー"}, // Used for miscellaneous characters; it can vary by keyboard."For the US standard keyboard, the '\|' key" // \ OEM_5
            {"Closed_Bracket", "」のキー"}, // Used for miscellaneous characters; it can vary by keyboard."For the US standard keyboard, the ']}' key" // ] OEM_6
            {"Caret", "＾のキー"}, // Used for miscellaneous characters; it can vary by keyboard."For the US standard keyboard, the 'single-quote/double-quote' key" // ^ OEM_7
            {"OEM_8", "OEM_8"}, // Used for miscellaneous characters; it can vary by keyboard.
            {"Reserved_29", "Reserved_29"}, // Reserved
            {"OEM_specific_6", "OEM_specific_6"}, // OEM specific
            {"Backslash_Underscore", "＿のキー"}, // Either the angle bracket key or the backslash key on the RT 102-key keyboard // \(backslash) OEM_102
            {"OEM_specific_7", "OEM_specific_7"}, // OEM specific
            {"OEM_specific_8", "OEM_specific_8"}, // OEM specific
            {"IME_Process", "IME_Process"}, // IME PROCESS key
            {"OEM_specific_9", "OEM_specific_9"}, // OEM specific
            {"Packet", "Packet"}, // "Used to pass Unicode characters as if they were keystrokes. The VK_PACKET key is the low word of a 32-bit Virtual Key value used for non-keyboard input methods. For more information, see Remark in?KEYBDINPUT,SendInput,?WM_KEYDOWN, and?WM_KEYUP"
            {"Unassigned_21", "Unassigned_21"}, // Unassigned
            {"OEM_specific_10", "OEM_specific_10"}, // OEM specific
            {"OEM_specific_11", "OEM_specific_11"}, // OEM specific
            {"OEM_specific_12", "OEM_specific_12"}, // OEM specific
            {"OEM_specific_13", "OEM_specific_13"}, // OEM specific
            {"OEM_specific_14", "OEM_specific_14"}, // OEM specific
            {"OEM_specific_15", "OEM_specific_15"}, // OEM specific
            {"OEM_specific_16", "OEM_specific_16"}, // OEM specific
            {"VK_DBE_ALPHANUMERIC", "英数キー"}, // OEM specific // Changes the mode to alphanumeric // keydown 英数
            {"VK_DBE_KATAKANA", "カタカナひらがな（カタカナ）"}, // OEM specific // Changes the mode to katakana // keydown カタカナひらがな
            {"VK_DBE_HIRAGANA", "カタカナひらがな（ひらがな）"}, // OEM specific // Changes the mode to hiragana // keydown カタカナひらがな
            {"VK_DBE_SBCSCHAR", "全角/半角キー（半角）"}, // OEM specific // Changes the mode to single-byte characters // keydown 全角/半角
            {"VK_DBE_DBCSCHAR", "全角/半角キー（全角）"}, // OEM specific // Changes the mode to double-byte characters // keydown 全角/半角
            {"VK_DBE_ROMAN", "VK_DBE_ROMAN"}, // OEM specific // Changes the mode to Roman characters // ローマ？
            {"Attn", "Attn"}, // Attn key
            {"CrSel", "CrSel"}, // CrSel key
            {"ExSel", "ExSel"}, // ExSel key
            {"EraseEOF", "EraseEOF"}, // Erase EOF key
            {"Play", "Play"}, // Play key
            {"Zoom", "Zoom"}, // Zoom key
            {"NoName", "NoName"}, // Reserved
            {"PA1", "PA1"}, // PA1 key
            {"OEM_Clear", "OEM_Clear"} // Clear key
        };
    }

    public enum VKeys
    {
        Left_MouseButton = 0x01, // Left mouse button
        Right_MouseButton = 0x02, // Right mouse button
        Break = 0x03, // Control-break processing // Ctrl+Pause
        Middle_MouseButton = 0x04, // Middle mouse button (three-button mouse)
        X1_MouseButton = 0x05, // X1 mouse button
        X2_MouseButton = 0x06, // X2 mouse button
        Undefined_1 = 0x07, // Undefined
        Backspace = 0x08, // BACKSPACE key
        Tab = 0x09, // TAB key
        Reserved_1 = 0x0A, // Reserved
        Reserved_2 = 0x0B, // Reserved
        Clear = 0x0C, // CLEAR key
        Enter = 0x0D, // ENTER key
        Undefined_2 = 0x0E, // Undefined
        Undefined_3 = 0x0F, // Undefined
        Shift = 0x10, // SHIFT key
        Ctrl = 0x11, // CTRL key
        Alt = 0x12, // ALT key
        Pause = 0x13, // PAUSE key
        CapsLock = 0x14, // CAPS LOCK key // Shift+英数キー
        IME_Kana = 0x15, // IME Kana mode
        // VK_HANGUEL = 0x15, // IME Hanguel mode (maintained for compatibility; use?VK_HANGUL)
        // VK_HANGUL = 0x15, // IME Hangul mode
        Undefined_4 = 0x16, // Undefined
        IME_Junja = 0x17, // IME Junja mode
        IME_final = 0x18, // IME final mode
        IME_Hanja = 0x19, // IME Hanja mode
        IME_Kanji = 0x19, // IME Kanji mode
        Undefined_5 = 0x1A, // Undefined
        Esc = 0x1B, // ESC key
        IME_Convert = 0x1C, // IME convert
        IME_NonConvert = 0x1D, // IME nonconvert
        IME_Accept = 0x1E, // IME accept
        IME_ModeChange = 0x1F, // IME mode change request
        Space = 0x20, // SPACEBAR
        PageUp = 0x21, // PAGE UP key
        PageDown = 0x22, // PAGE DOWN key
        End = 0x23, // END key
        Home = 0x24, // HOME key
        Left = 0x25, // LEFT ARROW key
        Up = 0x26, // UP ARROW key
        Right = 0x27, // RIGHT ARROW key
        Down = 0x28, // DOWN ARROW key
        Select = 0x29, // SELECT key
        Print = 0x2A, // PRINT key
        Execute = 0x2B, // EXECUTE key
        PrintScreen = 0x2C, // PRINT SCREEN key
        Insert = 0x2D, // INS key
        Delete = 0x2E, // DEL key
        Help = 0x2F, // HELP key
        Key_0 = 0x30, // 0 Key_
        Key_1 = 0x31, // 1 Key_
        Key_2 = 0x32, // 2 Key_
        Key_3 = 0x33, // 3 Key_
        Key_4 = 0x34, // 4 Key_
        Key_5 = 0x35, // 5 Key_
        Key_6 = 0x36, // 6 Key_
        Key_7 = 0x37, // 7 Key_
        Key_8 = 0x38, // 8 Key_
        Key_9 = 0x39, // 9 Key_
        Undefined_6 = 0x3A, // Undefined
        Undefined_7 = 0x3B, // Undefined
        Undefined_8 = 0x3C, // Undefined
        Undefined_9 = 0x3D, // Undefined
        Undefined_10 = 0x3E, // Undefined
        Undefined_11 = 0x3F, // Undefined
        Undefined_12 = 0x40, // Undefined
        Key_A = 0x41, // A key
        Key_B = 0x42, // B key
        Key_C = 0x43, // C key
        Key_D = 0x44, // D key
        Key_E = 0x45, // E key
        Key_F = 0x46, // F key
        Key_G = 0x47, // G key
        Key_H = 0x48, // H key
        Key_I = 0x49, // I key
        Key_J = 0x4A, // J key
        Key_K = 0x4B, // K key
        Key_L = 0x4C, // L key
        Key_M = 0x4D, // M key
        Key_N = 0x4E, // N key
        Key_O = 0x4F, // O key
        Key_P = 0x50, // P key
        Key_Q = 0x51, // Q key
        Key_R = 0x52, // R key
        Key_S = 0x53, // S key
        Key_T = 0x54, // T key
        Key_U = 0x55, // U key
        Key_V = 0x56, // V key
        Key_W = 0x57, // W key
        Key_X = 0x58, // X key
        Key_Y = 0x59, // Y key
        Key_Z = 0x5A, // Z key
        Left_Windows = 0x5B, // Left Windows key (Natural keyboard)
        Right_Windows = 0x5C, // Right Windows key (Natural keyboard)
        Applications = 0x5D, // Applications key (Natural keyboard)
        Reserved_3 = 0x5E, // Reserved
        Sleep = 0x5F, // Computer Sleep key
        Numpad_0 = 0x60, // Numeric keypad 0 key
        Numpad_1 = 0x61, // Numeric keypad 1 key
        Numpad_2 = 0x62, // Numeric keypad 2 key
        Numpad_3 = 0x63, // Numeric keypad 3 key
        Numpad_4 = 0x64, // Numeric keypad 4 key
        Numpad_5 = 0x65, // Numeric keypad 5 key
        Numpad_6 = 0x66, // Numeric keypad 6 key
        Numpad_7 = 0x67, // Numeric keypad 7 key
        Numpad_8 = 0x68, // Numeric keypad 8 key
        Numpad_9 = 0x69, // Numeric keypad 9 key
        Multiply = 0x6A, // Multiply key
        Add = 0x6B, // Add key // プラス
        Separator = 0x6C, // Separator key // カンマ
        Subtract = 0x6D, // Subtract key // マイナス
        Decimal = 0x6E, // Decimal key // 小数点
        Divide = 0x6F, // Divide key // 割る
        F1 = 0x70, // F1 key
        F2 = 0x71, // F2 key
        F3 = 0x72, // F3 key
        F4 = 0x73, // F4 key
        F5 = 0x74, // F5 key
        F6 = 0x75, // F6 key
        F7 = 0x76, // F7 key
        F8 = 0x77, // F8 key
        F9 = 0x78, // F9 key
        F10 = 0x79, // F10 key
        F11 = 0x7A, // F11 key
        F12 = 0x7B, // F12 key
        F13 = 0x7C, // F13 key
        F14 = 0x7D, // F14 key
        F15 = 0x7E, // F15 key
        F16 = 0x7F, // F16 key
        F17 = 0x80, // F17 key
        F18 = 0x81, // F18 key
        F19 = 0x82, // F19 key
        F20 = 0x83, // F20 key
        F21 = 0x84, // F21 key
        F22 = 0x85, // F22 key
        F23 = 0x86, // F23 key
        F24 = 0x87, // F24 key
        Unassigned_1 = 0x88, // Unassigned
        Unassigned_2 = 0x89, // Unassigned
        Unassigned_3 = 0x8A, // Unassigned
        Unassigned_4 = 0x8B, // Unassigned
        Unassigned_5 = 0x8C, // Unassigned
        Unassigned_6 = 0x8D, // Unassigned
        Unassigned_7 = 0x8E, // Unassigned
        Unassigned_8 = 0x8F, // Unassigned
        NumLock = 0x90, // NUM LOCK key
        ScrollLock = 0x91, // SCROLL LOCK key
        OEM_specific_1 = 0x92, // OEM specific
        OEM_specific_2 = 0x93, // OEM specific
        OEM_specific_3 = 0x94, // OEM specific
        OEM_specific_4 = 0x95, // OEM specific
        OEM_specific_5 = 0x96, // OEM specific
        Unassigned_9 = 0x97, // Unassigned
        Unassigned_10 = 0x98, // Unassigned
        Unassigned_11 = 0x99, // Unassigned
        Unassigned_12 = 0x9A, // Unassigned
        Unassigned_13 = 0x9B, // Unassigned
        Unassigned_14 = 0x9C, // Unassigned
        Unassigned_15 = 0x9D, // Unassigned
        Unassigned_16 = 0x9E, // Unassigned
        Unassigned_17 = 0x9F, // Unassigned
        Left_Shift = 0xA0, // Left SHIFT key
        Right_Shift = 0xA1, // Right SHIFT key
        Left_Control = 0xA2, // Left CONTROL key
        Right_Control = 0xA3, // Right CONTROL key
        Left_Alt = 0xA4, // Left MENU key
        Right_Alt = 0xA5, // Right MENU key
        Browser_Back = 0xA6, // Browser Back key
        Browser_Forward = 0xA7, // Browser Forward key
        Browser_Refresh = 0xA8, // Browser Refresh key
        Browser_Stop = 0xA9, // Browser Stop key
        Browser_Search = 0xAA, // Browser Search key
        Brouser_Favorites = 0xAB, // Browser Favorites key
        Browser_Start = 0xAC, // Browser Start and Home key
        Volume_Mute = 0xAD, // Volume Mute key
        Volume_Down = 0xAE, // Volume Down key
        Volume_Up = 0xAF, // Volume Up key
        Media_Next_Track = 0xB0, // Next Track key
        Media_Previous_Track = 0xB1, // Previous Track key
        Media_Stop = 0xB2, // Stop Media key
        Media_Play_Pause = 0xB3, // Play/Pause Media key
        Launch_Mail = 0xB4, // Start Mail key
        Launch_Media_Select = 0xB5, // Select Media key
        Launch_App1 = 0xB6, // Start Application 1 key
        Launch_App2 = 0xB7, // Start Application 2 key
        Reserved_4 = 0xB8, // Reserved
        Reserved_5 = 0xB9, // Reserved
        Semicolon_Colon = 0xBA, // Used for miscellaneous characters; it can vary by keyboard."For the US standard keyboard, the ';:' key" // : OEM_1
        Plus = 0xBB, // "For any country/region, the '+' key"
        Comma = 0xBC, // "For any country/region, the ',' key"
        Minus = 0xBD, // "For any country/region, the '-' key"
        Period = 0xBE, // "For any country/region, the '.' key"
        Slash_Questionmark = 0xBF, // Used for miscellaneous characters; it can vary by keyboard."For the US standard keyboard, the '/?' key" // / OEM2
        Graveaccent_Atsign = 0xC0, // "For the US standard keyboard, the '`~' key" // `@ OEM_3
        Reserved_6 = 0xC1, // Reserved
        Reserved_7 = 0xC2, // Reserved
        Reserved_8 = 0xC3, // Reserved
        Reserved_9 = 0xC4, // Reserved
        Reserved_10 = 0xC5, // Reserved
        Reserved_11 = 0xC6, // Reserved
        Reserved_12 = 0xC7, // Reserved
        Reserved_13 = 0xC8, // Reserved
        Reserved_14 = 0xC9, // Reserved
        Reserved_15 = 0xCA, // Reserved
        Reserved_16 = 0xCB, // Reserved
        Reserved_17 = 0xCC, // Reserved
        Reserved_18 = 0xCD, // Reserved
        Reserved_19 = 0xCE, // Reserved
        Reserved_20 = 0xCF, // Reserved
        Reserved_21 = 0xD0, // Reserved
        Reserved_22 = 0xD1, // Reserved
        Reserved_23 = 0xD2, // Reserved
        Reserved_24 = 0xD3, // Reserved
        Reserved_25 = 0xD4, // Reserved
        Reserved_26 = 0xD5, // Reserved
        Reserved_27 = 0xD6, // Reserved
        Reserved_28 = 0xD7, // Reserved
        Unassigned_18 = 0xD8, // Unassigned
        Unassigned_19 = 0xD9, // Unassigned
        Unassigned_20 = 0xDA, // Unassigned
        Open_Bracket = 0xDB, // Used for miscellaneous characters; it can vary by keyboard."For the US standard keyboard, the '[{' key" // [ OEM_4
        Backslash_Verticalbar = 0xDC, // Used for miscellaneous characters; it can vary by keyboard."For the US standard keyboard, the '\|' key" // \ OEM_5
        Closed_Bracket = 0xDD, // Used for miscellaneous characters; it can vary by keyboard."For the US standard keyboard, the ']}' key" // ] OEM_6
        Caret = 0xDE, // Used for miscellaneous characters; it can vary by keyboard."For the US standard keyboard, the 'single-quote/double-quote' key" // ^ OEM_7
        OEM_8 = 0xDF, // Used for miscellaneous characters; it can vary by keyboard.
        Reserved_29 = 0xE0, // Reserved
        OEM_specific_6 = 0xE1, // OEM specific
        Backslash_Underscore = 0xE2, // Either the angle bracket key or the backslash key on the RT 102-key keyboard // \(backslash) OEM_102
        OEM_specific_7 = 0xE3, // OEM specific
        OEM_specific_8 = 0xE4, // OEM specific
        IME_Process = 0xE5, // IME PROCESS key
        OEM_specific_9 = 0xE6, // OEM specific
        Packet = 0xE7, // "Used to pass Unicode characters as if they were keystrokes. The VK_PACKET key is the low word of a 32-bit Virtual Key value used for non-keyboard input methods. For more information, see Remark in?KEYBDINPUT,SendInput,?WM_KEYDOWN, and?WM_KEYUP"
        Unassigned_21 = 0xE8, // Unassigned
        OEM_specific_10 = 0xE9, // OEM specific
        OEM_specific_11 = 0xEA, // OEM specific
        OEM_specific_12 = 0xEB, // OEM specific
        OEM_specific_13 = 0xEC, // OEM specific
        OEM_specific_14 = 0xED, // OEM specific
        OEM_specific_15 = 0xEE, // OEM specific
        OEM_specific_16 = 0xEF, // OEM specific
        // OEM_specific = 0xF0, // OEM specific
        // OEM_specific = 0xF1, // OEM specific
        // OEM_specific = 0xF2, // OEM specific
        // OEM_specific = 0xF3, // OEM specific
        // OEM_specific = 0xF4, // OEM specific
        // OEM_specific = 0xF5, // OEM specific

        VK_DBE_ALPHANUMERIC = 0xF0, // OEM specific // Changes the mode to alphanumeric //keydown 英数
        VK_DBE_KATAKANA = 0xF1, // OEM specific // Changes the mode to katakana //keydown カタカナひらがな
        VK_DBE_HIRAGANA = 0xF2, // OEM specific // Changes the mode to hiragana //keydown カタカナひらがな
        VK_DBE_SBCSCHAR = 0xF3, // OEM specific // Changes the mode to single-byte characters //keydown 全角/半角
        VK_DBE_DBCSCHAR = 0xF4, // OEM specific // Changes the mode to double-byte characters //keydown 全角/半角
        VK_DBE_ROMAN = 0xF5, // OEM specific // Changes the mode to Roman characters //ローマ？

        // #define VK_DBE_NOROMAN 0x0f6 // Changes the mode to non-Roman characters
        // #define VK_DBE_ENTERWORDREGISTERMODE 0x0f7 // Activates the word registration dialog box
        // #define VK_DBE_ENTERIMECONFIGMODE 0x0f8 // Activates a dialog box for setting up an IME environment
        // #define VK_DBE_FLUSHSTRING 0x0f9 // Deletes the undetermined string without determining it
        // #define VK_DBE_CODEINPUT 0x0fa // Changes the mode to code input
        // #define VK_DBE_NOCODEINPUT 0x0fb // Changes the mode to non-code input
        // #define VK_DBE_DETERMINESTRING 0x0fc
        // #define VK_DBE_ENTERDLGCONVERSIONMODE 0x0fd

        Attn = 0xF6, // Attn key
        CrSel = 0xF7, // CrSel key
        ExSel = 0xF8, // ExSel key
        EraseEOF = 0xF9, // Erase EOF key
        Play = 0xFA, // Play key
        Zoom = 0xFB, // Zoom key
        NoName = 0xFC, // Reserved
        PA1 = 0xFD, // PA1 key
        OEM_Clear = 0xFE // Clear key
    }

    public enum MouseMessages
    {
        WM_LBUTTONDOWN = 0x0201,
        WM_LBUTTONUP = 0x0202,
        WM_MBUTTONDOWN = 0x0207,
        WM_MBUTTONUP = 0x0208,
        WM_MOUSEMOVE = 0x0200,
        WM_MOUSEWHEEL = 0x020A,
        WM_RBUTTONDOWN = 0x0204,
        WM_RBUTTONUP = 0x0205,
        WM_XBUTTONDOWN = 0x020B,
        WM_XBUTTONUP = 0x020C
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MSLLHOOKSTRUCT
    {
        public POINT pt;
        public uint mouseData;
        public uint flags;
        public uint time;
        public IntPtr dwExtraInfo;
    }
}
