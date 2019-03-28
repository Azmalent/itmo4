#include <stdio.h>
#include <Windows.h>
#include <chrono>
#include <cmath>
#include <iostream>
#include <fstream>
#include <string>
using namespace std;
using namespace std::chrono;

#define COPYMETHOD_C            0
#define COPYMETHOD_WINDOWS      1
#define COPYMETHOD_COPYFILE     2
#define COPYMETHOD_BENCHMARK    3
#define COPYMETHOD_INVALID      -1

#define FILENAME_SIZE           16
#define INITIAL_FILESIZE_KB     1 << 7     //128 KB

int copy_c(char* originalName, char* copyName)
{
	char buffer[BUFSIZ];

	FILE* oldFile = fopen(originalName, "rb");
    if (oldFile == NULL)
	{
		perror(originalName);
		return EXIT_FAILURE;
	}

	FILE* newFile = fopen(copyName, "wb");
	if (newFile == NULL)
	{
		perror(copyName);
		return EXIT_FAILURE;
	}

	size_t inBytes, outBytes;
	while ((inBytes = fread(buffer, 1, BUFSIZ, oldFile)) > 0)
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

int copy_windows(char* originalName, char* copyName)
{
	CHAR buffer[BUFSIZ];

    HANDLE hIn = CreateFile(originalName, GENERIC_READ, 0, NULL, OPEN_EXISTING, 0, NULL);
    if (hIn == INVALID_HANDLE_VALUE)
	{
		cout << "Failed to open input file: error code " << GetLastError();
		return EXIT_FAILURE;
	}

    HANDLE hOut = CreateFile(copyName, GENERIC_WRITE, 0, NULL, OPEN_ALWAYS, 0, NULL);
    if (hOut == INVALID_HANDLE_VALUE) 
    {
		cout << "Failed to open output file: error code " << GetLastError();
		return EXIT_FAILURE;
	}

	DWORD nIn, nOut;
    while (ReadFile(hIn, buffer, BUFSIZ, &nIn, NULL) && nIn > 0)
	{
		WriteFile(hOut, buffer, nIn, &nOut, NULL);
		if (nIn != nOut)
		{
			cout << "Failed to copy file: error code " << GetLastError();
			return EXIT_FAILURE;
		}
	}

	CloseHandle(hIn);
	CloseHandle(hOut);

    return EXIT_SUCCESS;
}

int copy_copyfile(char* originalName, char* copyName)
{
    int success = CopyFile(originalName, copyName, FALSE);
    if (!success) cout << "Failed to copy file: error code " << GetLastError();
	return success ? EXIT_SUCCESS : EXIT_FAILURE;
}

void calculateAveragePoints(string filename)
{
	ifstream input(filename, ios_base::in);

	int count = 0;
	float sum = 0;

    int semester, points;
    string discipline, teacher;
	while (input >> semester >> discipline >> points >> teacher)
	{
		sum += points;
		count++;
	}

	input.close();

    float avg = sum / count;
    if (!isnan(avg)) cout << "Average points: " << avg << endl;
	else cout << "Incorrect file format; unable to parse points." << endl;
}

int getCopyMethod(string argument)
{
    if (argument == "c") return COPYMETHOD_C;
    else if (argument == "windows") return COPYMETHOD_WINDOWS;
    else if (argument == "copyfile") return COPYMETHOD_COPYFILE;
    else if (argument == "benchmark") return COPYMETHOD_BENCHMARK;
    return COPYMETHOD_INVALID;
}

time_t execTime(int (*function)(char*, char*), char* originalName, char* copyName)
{
    auto startTime = steady_clock::now();
    int result = function(originalName, copyName);
    if (result == EXIT_FAILURE) return -1;
    
    auto duration = duration_cast<microseconds>(steady_clock::now() - startTime);
    return duration.count();
}

int benchmark()
{
    char originalName[FILENAME_SIZE], copyName[FILENAME_SIZE];
    tmpnam_s(originalName, FILENAME_SIZE);
    tmpnam_s(copyName, FILENAME_SIZE);

    FILE* original;

    cout << "+--------+----------------+----------------+----------------+" << endl <<
            "|  SIZE  |       C        |     WINDOWS    |    COPYFILE    |" << endl <<
            "+--------+----------------+----------------+----------------+" << endl;

    size_t size_kb = INITIAL_FILESIZE_KB, oldSize_kb = 0;
    time_t time1, time2, time3;

    char buffer[1024];
    for (int i = 0; i < 1024; i++) buffer[i] = '1';

    for (int i = 0; i < 10; i++)
    {
        for (int k = 0; k < size_kb - oldSize_kb; k++)
        {
            original = fopen(originalName + 1, "ab");
            fwrite(buffer, 1, 1024, original);
            fclose(original);
        }

        size_t displaySize = size_kb;
        bool megabytes = displaySize >= 1024;
        if (megabytes) displaySize = displaySize >> 10;
        time1 = execTime(&copy_c, originalName + 1, copyName + 1);
        time2 = execTime(&copy_windows, originalName + 1, copyName + 1);
        time3 = execTime(&copy_copyfile, originalName + 1, copyName + 1);
        printf("|%5i %cB|%12i mcs|%12i mcs|%12i mcs|\n", displaySize, megabytes ? 'M' : 'K', time1, time2, time3);

        oldSize_kb = size_kb; 
        size_kb = size_kb << 1;
    }

    remove(originalName + 1);
    remove(copyName + 1);

    cout << "+--------+----------------+----------------+----------------+" << endl;
    return EXIT_SUCCESS;
}

bool validateArgs(int argc, char* argv[])
{
    if (argc != 2 && argc != 4 && argc != 5) return false;
    if (argc == 5 && (string)argv[4] != "--avg") return false;

    int copyMethod = getCopyMethod(argv[1]);
    if (copyMethod == COPYMETHOD_INVALID) return false;
    if (argc == 2 == !(copyMethod == COPYMETHOD_BENCHMARK)) return false;

    return true;
}

int main(int argc, char* argv[])
{
    bool argsAreValid = validateArgs(argc, argv); 
    if (!argsAreValid) 
    {
        cout << "Usage:" << endl << "lab1 {c|windows|copyfile} <OLD_FILENAME> <NEW_FILENAME> [--avg]" << endl << endl;
        cout << "For benchmark:" << endl << "lab1 benchmark" << endl;
        return EXIT_FAILURE;
    }

    int copyExitcode;
    switch (getCopyMethod(argv[1]))
    {
        case COPYMETHOD_C:
            copyExitcode = copy_c(argv[2], argv[3]);
            if(copyExitcode == EXIT_FAILURE) return EXIT_FAILURE;
            break;

        case COPYMETHOD_WINDOWS:
            copyExitcode = copy_windows(argv[2], argv[3]);
            if(copyExitcode == EXIT_FAILURE) return EXIT_FAILURE;
            break;

        case COPYMETHOD_COPYFILE:
            copyExitcode = copy_copyfile(argv[2], argv[3]);
            if(copyExitcode == EXIT_FAILURE) return EXIT_FAILURE;
            break;

        case COPYMETHOD_BENCHMARK: 
            return benchmark();
    }

    cout << "File was succesfully copied." << endl;
    if (argc == 7) calculateAveragePoints(argv[2]); 

    return copyExitcode;
}