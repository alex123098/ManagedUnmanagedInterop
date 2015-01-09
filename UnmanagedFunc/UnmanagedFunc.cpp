// UnmanagedFunc.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <memory>

#include "SimpleSettingsProvider.h"
#include "CommandProcessor.h"
#include "MemoryMappedFile.h"
#include <iostream>

using namespace std;

int _tmain()
{
	cout << "Creaing settings provider...";
	auto settings = new SimpleSettingsProvider();
	cout << "DONE" << endl;
	wstring exitEventName = settings->Get("exitEventName");
	wstring writeEventName = settings->Get("writeEventName");
	wstring mmfName = settings->Get("MMFName");
	HANDLE events[2];

	cout << "Setting up events...";
	events[0] = CreateEvent(nullptr, TRUE, FALSE, exitEventName.c_str());
	events[1] = CreateEvent(nullptr, FALSE, FALSE, writeEventName.c_str());
	cout << "DONE" << endl;

	cout << "Creating memory mapped file...";
	auto mmf = new MemoryMappedFile(mmfName, 4096);
	cout << "DONE" << endl;
	
	cout << "Creating command processor...";
	CommandProcessor command_processor(settings);
	cout << "DONE" << endl;

	cout << "Message loop started" << endl;
	while (1) {
		bool exit = WaitForMultipleObjects(2, events, FALSE, INFINITE) == 0;
		if (exit) {
			break;
		}

		command_processor.ProcessNextCommand(mmf);
	}
	
	cout << "Finalizing..." << endl;

	delete mmf;
	for (int i = 0; i < 2; i++) {
		CloseHandle(events[i]);
	}
	delete settings;
	return 0;
}

