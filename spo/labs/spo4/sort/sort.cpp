#include <stdio.h>
#include <Windows.h>
#include <string>

using namespace std;

#define ARRAY_LENGTH 10000
#define BUFFER_SIZE  8

#define LT -1
#define GT 1
#define EQ 0

typedef enum {ASC, DESC} order_t;

int cmp(int a, int b)
{
	if (a > b) return GT;
	if (a < b) return LT;
	return EQ;
}

void sort(int* arr, size_t n, order_t order)
{
	int gap, i, j, temp;
	int ord = (order == ASC ? GT : LT);

    for (int gap = n/2; gap > 0; gap /= 2)
    {
        for (int i = gap; i < n; i += 1)
        {
            temp = arr[i];
 
			j = i;
			while (j >= gap && cmp(arr[j - gap], temp) == ord)
			{
				arr[j] = arr[j - gap];
				j -= gap;
			}
             
            arr[j] = temp;
        }
    }
}

int bisect(int* arr, size_t n, order_t order, int x)
{
	int leftCmp = (order == ASC ? LT : GT);

	int first = 0, last = n;
	while (first < last)
	{
		int i = (first + last) / 2;

		if (x == arr[i]) return i;
		else if (cmp(x, arr[i]) == leftCmp) last = i;
		else first = i + 1;
	}

	return -1;
}

int wmain(int argc, wchar_t* argv[])
{
	order_t order = (wstring)argv[1] == L"asc" ? ASC : DESC;
	int x = stoi( (wstring)argv[2] );

	int arr[ARRAY_LENGTH];
	char buffer[BUFFER_SIZE];

	FILE* file = fopen("..\\array.txt", "r");
	for (int i = 0; i < ARRAY_LENGTH; i++)
	{
		if (fgets(buffer, BUFFER_SIZE, file) == nullptr) break;
		arr[i] = atoi(buffer);
	}
	fclose(file);

	sort(arr, ARRAY_LENGTH, order);
	int m = bisect(arr, ARRAY_LENGTH, order, x);

	printf("Done. Found %i in position %i", x, m);
	return EXIT_SUCCESS;
}