#include "stdafx.h"
#include <Windows.h>
#include <iostream>
using namespace std;

#define MAX_KEYSTROKES 255
#define CONTINUE_INPUT 0
#define STOP_INPUT	   1
#define ACTION_HOTKEY_ID 1
#define EXIT_HOTKEY_ID	 2
#define VK_K 0x4B
#define VK_Q 0x51

INPUT keystrokes[MAX_KEYSTROKES];
unsigned char keyCount = 0;

int getKeystroke()
{
	HANDLE hStdin = GetStdHandle(STD_INPUT_HANDLE); 
	INPUT_RECORD inputBuffer[MAX_KEYSTROKES];
	DWORD inputCount = 0;

	ReadConsoleInput(hStdin, inputBuffer, MAX_KEYSTROKES, &inputCount);
	for (int i = 0; i < inputCount; i++)
	{
		if (inputBuffer[i].EventType != KEY_EVENT) continue;

		auto keyEvent = inputBuffer[i].Event.KeyEvent;
		if (keyEvent.wVirtualKeyCode == VK_RETURN) return STOP_INPUT;

		INPUT input;
		auto vk = keyEvent.wVirtualScanCode;

		input.type = INPUT_KEYBOARD;
		input.ki.wVk = keyEvent.wVirtualKeyCode;
		input.ki.wScan = keyEvent.wVirtualScanCode;
		input.ki.time = 0;
		input.ki.dwExtraInfo = 0;
		input.ki.dwFlags = KEYEVENTF_UNICODE;
		if (keyEvent.bKeyDown == FALSE) input.ki.dwFlags |= KEYEVENTF_KEYUP;

		keystrokes[keyCount] = input;

		keyCount++;
		if (keyCount == MAX_KEYSTROKES) return STOP_INPUT;
	}

	return 0;
}

int main()
{
    HANDLE hMutex = CreateMutex( NULL, TRUE, L"spo_lab6_mutex" );
	if(GetLastError() == ERROR_ALREADY_EXISTS)
	{
		puts("Program is already running!");

		puts("Press any key to exit.");
		getchar();
		return EXIT_FAILURE;
	}

	puts("Enter your keystroke sequence.\n");
	while (keyCount < MAX_KEYSTROKES)
	{
		if(getKeystroke() == STOP_INPUT) break;
	}
	
	bool actionHotkey = RegisterHotKey(NULL, ACTION_HOTKEY_ID, MOD_ALT, VK_K);
	bool exitHotkey = RegisterHotKey(NULL, EXIT_HOTKEY_ID, MOD_ALT, VK_Q);
	if (!actionHotkey || !exitHotkey) 
	{
		puts("Failed to initialize hotkeys!");

		puts("Press any key to exit.");
		getchar();
		ReleaseMutex(hMutex);
		CloseHandle(hMutex);
		return EXIT_FAILURE;
	}
	
	puts("Press <Alt-K> to simulate keystrokes or <Alt-Q> to exit.");

	MSG msg = {0};
    while (GetMessage(&msg, NULL, 0, 0) != 0)
    {
        if (msg.message == WM_HOTKEY)
        {
            WORD vk = msg.lParam >> 16;
			if (vk == VK_K) 
			{
				Sleep(300); //Подождём, пока пользователь не отпустит Alt
				UINT simulatedEvents = SendInput(keyCount, keystrokes, sizeof(INPUT));
				if (simulatedEvents == keyCount) puts("Simulated keystroke sequence.");
				else puts("Failed to simulate keystroke sequence!");
			}
			else if (vk == VK_Q) 
			{
				UnregisterHotKey(NULL, ACTION_HOTKEY_ID);
				UnregisterHotKey(NULL, EXIT_HOTKEY_ID);
				ReleaseMutex(hMutex);
				CloseHandle(hMutex);
				return EXIT_SUCCESS;
			}
        }
    } 
}
