#include "stdafx.h"

void configureVirtualization(int state)
{
	HANDLE processToken;
	if(!OpenProcessToken(GetCurrentProcess(), MAXIMUM_ALLOWED, &processToken))
	{
		_tprintf(L"Could not get process virtualization information. Error : %d", GetLastError());
		return;
	}

	if(SetTokenInformation(processToken, TokenVirtualizationEnabled, &state, sizeof(state)))
	{
		_tprintf(L"\r\nVirtualization %s.\r\n", state == 1 ? L"ENABLED" : L"DISABLED");
	}	

	CloseHandle(processToken);
}

void showVirtualizationStatus()
{
	HANDLE processToken;
	if(!OpenProcessToken(GetCurrentProcess(), MAXIMUM_ALLOWED, &processToken))
	{
		_tprintf(L"Could not get process virtualization information. Error : %d\r\n", GetLastError());
		return;
	}

	DWORD length;
	GetTokenInformation(processToken, TokenVirtualizationEnabled, NULL, 0, &length);
	
	DWORD lastError = GetLastError();
	if ( lastError != ERROR_INSUFFICIENT_BUFFER)
	{
		_tprintf(L"Error getting info. Error : %d", lastError);
		goto cleanup;
	}

	int state;
	DWORD x;
	if(GetTokenInformation(processToken, TokenVirtualizationEnabled, &state, length, &x))
	{
		_tprintf(L"Virtualization is %s\r\n", state == 1 ? L"ON" : L"OFF");
	}

cleanup:
	CloseHandle(processToken);
}

int _tmain(int argc, _TCHAR* argv[])
{
	if (argc < 2) 
	{
		_tprintf(L"Usage: %s [-d|-e] <file> [contents]\r\n", PathFindFileName(argv[0]));
		return -2;
	}

	int fileParamIndex = 1;
	bool configFlag = false;
	if (argc >= 2 && argv[fileParamIndex][0] == L'-')
	{
		configFlag = true;
		configureVirtualization(argv[fileParamIndex][1] == L'd' ? 0 : 1);
		fileParamIndex++;
	}

	showVirtualizationStatus();

	if (argc == 3 && configFlag || argc == 2) 
	{
		HANDLE file = CreateFile(argv[fileParamIndex], FILE_READ_ACCESS, 0, NULL, OPEN_EXISTING, 0, NULL);
		if (file == INVALID_HANDLE_VALUE)
		{
			_tprintf(L"Could not open file (error %d) : %s\r\n", GetLastError(), argv[fileParamIndex]);
			return -1;
		}

		CHAR buffer[4096];
		DWORD read;
		while (ReadFile(file, buffer, sizeof(buffer)/sizeof(buffer[0]), &read, NULL))
		{
			if (read == 0)
			{
				break;
			}
			buffer[read] = 0;
			printf(buffer);
		}

		CloseHandle(file);
	}
	else
	{
		HANDLE file = CreateFile(argv[fileParamIndex], FILE_WRITE_ACCESS, 0, NULL, CREATE_ALWAYS, 0, NULL);
		if (file == INVALID_HANDLE_VALUE)
		{
			_tprintf(L"Could not create file (error %d) : %s", GetLastError(), argv[fileParamIndex]);
			return -1;
		}

		char buffer[4096];
		size_t convertedLen;
		wcstombs_s(&convertedLen, buffer, argv[fileParamIndex + 1], sizeof(buffer)/sizeof(buffer[0]));
		DWORD writen;
		if (WriteFile(file, buffer, strlen(buffer), &writen, NULL))
		{
			_tprintf(L"%d bytes writen to %s", writen, argv[fileParamIndex]);
		}
		else
		{
			_tprintf(L"Error %d while writing to to %s", GetLastError(), argv[fileParamIndex]);
		}

		CloseHandle(file);
	}

	return 0;
}

