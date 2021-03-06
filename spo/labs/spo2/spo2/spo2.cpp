#include "stdafx.h"
#include "header.h"

#define ARRAY_LENGTH 10000
#define ARRAY_SIZE	 ARRAY_LENGTH * sizeof(int)

#define BINARY_FILENAME L"..\\array.bin"
#define INPUT_FILENAME  L"..\\array.txt"
#define OUTPUT_FILENAME L"..\\sorted.txt"


int* loadArray(const wchar_t* filename)
{
	int* arr = new int[ARRAY_LENGTH];
	ifstream input(filename, ios_base::in);
	for (int i = 0; i < ARRAY_LENGTH; i++) input >> arr[i];
	input.close();
	return arr;
}

void binaryToText(const wchar_t* binaryFilename, const wchar_t* textFilename)
{
	HANDLE hFile = CreateFile(binaryFilename, GENERIC_READ | GENERIC_WRITE, 0, NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);

	int* arr = new int[ARRAY_LENGTH];
	ReadFile(hFile, (char*) arr, ARRAY_SIZE, NULL, NULL);
	CloseHandle(hFile);

	ofstream output(textFilename, ios_base::out);
	for (int i = 0; i < ARRAY_LENGTH; i++) output << arr[i] << endl;
	output.close();
}

void serializeArray(const int* arr, const wchar_t* filename)
{
	HANDLE hFile = CreateFile(filename, GENERIC_READ | GENERIC_WRITE, 0, NULL, OPEN_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);
	WriteFile(hFile, (char*) arr, ARRAY_SIZE, NULL, NULL);
	CloseHandle(hFile);
}

//Сортировка Шелла (по возрастанию)
void sort(int* arr, int n)
{
    for (int gap = n/2; gap > 0; gap /= 2)
    {
        for (int i = gap; i < n; i += 1)
        {
            int j, temp = arr[i];
 
            for (j = i; j >= gap && arr[j - gap] > temp; j -= gap)
                arr[j] = arr[j - gap];
             
            arr[j] = temp;
        }
    }
}

time_t elapsedMcs(time_point<high_resolution_clock> startTime)
{
	auto duration = duration_cast<microseconds>(high_resolution_clock::now() - startTime);
    return duration.count();
}

time_t sortHeap(const int* arr)
{
	auto startTime = high_resolution_clock::now();
	//===============================

	HANDLE hHeap = GetProcessHeap();
	int* data = (int*) HeapAlloc(hHeap, 0, ARRAY_SIZE);

	memcpy(data, arr, ARRAY_SIZE);
	sort(data, ARRAY_LENGTH);

	serializeArray(data, BINARY_FILENAME);

	//===============================
	HeapFree(hHeap, 0, data);
	return elapsedMcs(startTime);
}

time_t sortBasePtr(const int* arr)
{
	serializeArray(arr, BINARY_FILENAME);

	auto startTime = high_resolution_clock::now();
	//===============================

	HANDLE hFile = CreateFile(BINARY_FILENAME, GENERIC_READ | GENERIC_WRITE, 0, nullptr, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, nullptr);

	DWORD dwFileSize = GetFileSize(hFile, nullptr);

	HANDLE hMapping = CreateFileMapping(hFile, nullptr, PAGE_READWRITE, 0, ARRAY_SIZE, nullptr);

	int* mapData = (int*) MapViewOfFile(hMapping, FILE_MAP_READ | FILE_MAP_WRITE, 0, 0, dwFileSize);
	int* data = new int[ARRAY_LENGTH];

	for (int i = 0; i <= dwFileSize / sizeof(int); i++) data[i] = *(mapData + i);

	sort(data, dwFileSize / sizeof(int));

	for (int i = 0; i <= dwFileSize / sizeof(int); ++i) mapData[i] = *(data + i);
	
	//===============================
	UnmapViewOfFile(mapData);
	CloseHandle(hMapping);
	CloseHandle(hFile);

    return elapsedMcs(startTime);
}

time_t sortMapping(const int* arr)
{
	serializeArray(arr, BINARY_FILENAME);

	auto startTime = high_resolution_clock::now();
	//===============================	

	HANDLE hFile = CreateFile(BINARY_FILENAME, GENERIC_READ | GENERIC_WRITE, FILE_SHARE_READ | FILE_SHARE_WRITE, 
		NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);

	DWORD dwFileSize = GetFileSize(hFile, NULL);

	HANDLE hMapping = CreateFileMapping(hFile, NULL, PAGE_READWRITE, 0, ARRAY_SIZE, NULL);

	int* data = (int*) MapViewOfFile(hMapping, FILE_MAP_READ | FILE_MAP_WRITE, 0, 0, dwFileSize);

	sort(data, dwFileSize / sizeof(int));

	//===============================
	UnmapViewOfFile(data);
	CloseHandle(hFile);
	CloseHandle(hMapping);

	return elapsedMcs(startTime);
}

int main()
{
    cout << "+--------+----------------+----------------+----------------+\n"
         << "| LENGTH |      HEAP      |  BASE POINTER  |  FILE MAPPING  |\n"
         << "+--------+----------------+----------------+----------------+\n";

	const int* arr = loadArray(INPUT_FILENAME);	

	time_t time1 = sortHeap(arr);
	DeleteFile(BINARY_FILENAME);

	time_t time2 = sortBasePtr(arr);
	DeleteFile(BINARY_FILENAME);

	time_t time3 = sortMapping(arr);
	binaryToText(BINARY_FILENAME, OUTPUT_FILENAME);
	DeleteFile(BINARY_FILENAME);

	delete(arr);

	printf("|%8u|%12lli mcs|%12lli mcs|%12lli mcs|\n", ARRAY_LENGTH, time1, time2, time3);

    cout << "+--------+----------------+----------------+----------------+\n\n";

	system("pause");
    return EXIT_SUCCESS;
}