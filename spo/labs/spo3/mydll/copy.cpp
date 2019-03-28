#include "stdafx.h"
#include <stdio.h>
#include <Windows.h>
#include <chrono>
#include <cmath>
#include <iostream>
#include <fstream>
using namespace std;
using namespace std::chrono;

#pragma comment( lib, "libucrt.lib" )

#define FILENAME_SIZE           16
#define INITIAL_FILESIZE_KB     256
#define BUFFER_SIZE				1024 * INITIAL_FILESIZE_KB

typedef int (*function)(wchar_t*, wchar_t*);

int copy_c(wchar_t* originalName, wchar_t* copyName)
{
	char buffer[BUFFER_SIZE];

	FILE* oldFile = _wfopen(originalName, L"rb");
    if (oldFile == NULL)
	{
		_wperror(originalName);
		return EXIT_FAILURE;
	}

	FILE* newFile = _wfopen(copyName, L"wb");
	if (newFile == NULL)
	{
		_wperror(copyName);
		return EXIT_FAILURE;
	}

	size_t inBytes, outBytes;
	while ((inBytes = fread(buffer, 1, BUFFER_SIZE, oldFile)) > 0)
	{
		outBytes = fwrite(buffer, 1, inBytes, newFile);

		if (outBytes != inBytes)
		{
			perror("Failed to copy file via C functions.");
			return EXIT_FAILURE;
		}
	}

	fclose(oldFile);
	fclose(newFile);

    return EXIT_SUCCESS;
}

int copy_windows(wchar_t* originalName, wchar_t* copyName)
{
	CHAR buffer[BUFFER_SIZE];

    HANDLE hIn = CreateFile(originalName, GENERIC_READ, 0, NULL, OPEN_EXISTING, 0, NULL);
    if (hIn == INVALID_HANDLE_VALUE)
	{
		cout << "Failed to open input file via WinAPI: error code " << GetLastError();
		return EXIT_FAILURE;
	}

    HANDLE hOut = CreateFile(copyName, GENERIC_WRITE, 0, NULL, OPEN_ALWAYS, 0, NULL);
    if (hOut == INVALID_HANDLE_VALUE) 
    {
		cout << "Failed to open output file via WinAPI: error code " << GetLastError();
		return EXIT_FAILURE;
	}

	DWORD nIn, nOut;
    while (ReadFile(hIn, buffer, BUFFER_SIZE, &nIn, NULL) && nIn > 0)
	{
		WriteFile(hOut, buffer, nIn, &nOut, NULL);
		if (nIn != nOut)
		{
			cout << "Failed to copy file via WinAPI: error code " << GetLastError();
			return EXIT_FAILURE;
		}
	}

	CloseHandle(hIn);
	CloseHandle(hOut);

	return EXIT_SUCCESS;
}

int copy_copyfile(wchar_t* originalName, wchar_t* copyName)
{
    int success = CopyFile(originalName, copyName, FALSE);
    if (!success) cout << "Failed to copy file via CopyFile: error code " << GetLastError();
	return success ? EXIT_SUCCESS : EXIT_FAILURE;
}

time_t execTime(function func, wchar_t* originalName, wchar_t* copyName)
{
    auto startTime = high_resolution_clock::now();

    int result = func(originalName, copyName);
    if (result == EXIT_FAILURE) return -1;

	auto endTime = high_resolution_clock::now();

    auto duration = duration_cast<microseconds>(endTime - startTime);
    return duration.count();
}

void DllCopy()
{
    const wchar_t* originalName = L"original";
	const wchar_t* copyName = L"copy";

    FILE* original;

    cout << "+--------+----------------+----------------+----------------+" << endl <<
            "|  SIZE  |       C        |     WINDOWS    |    COPYFILE    |" << endl <<
            "+--------+----------------+----------------+----------------+" << endl;

    size_t size_kb = INITIAL_FILESIZE_KB, oldSize_kb = 0;
    time_t time1, time2, time3;

    char buffer[1024 * INITIAL_FILESIZE_KB];

    for (int i = 0; i < 10; i++)
    {
        for (int k = 0; k < (size_kb - oldSize_kb) / INITIAL_FILESIZE_KB; k++)
        {
            original = _wfopen(originalName, L"ab");
            fwrite(buffer, 1, 1024 * INITIAL_FILESIZE_KB, original);
            fclose(original);
        }

        size_t displaySize = size_kb;
        bool megabytes = displaySize >= 1024;
        if (megabytes) displaySize = displaySize >> 10;
        time1 = execTime(&copy_c, (wchar_t*)originalName, (wchar_t*)copyName);
        time2 = execTime(&copy_windows, (wchar_t*)originalName, (wchar_t*)copyName);
        time3 = execTime(&copy_copyfile, (wchar_t*)originalName, (wchar_t*)copyName);
        printf("|%5d %cB", displaySize, megabytes ? 'M' : 'K');
		printf("|%12i mcs", time1);
		printf("|%12i mcs", time2);
		printf("|%12i mcs|\n", time3);

        oldSize_kb = size_kb; 
        size_kb = size_kb << 1;
    }

    _wremove(originalName);
    _wremove(copyName);

    cout << "+--------+----------------+----------------+----------------+" << endl;
}