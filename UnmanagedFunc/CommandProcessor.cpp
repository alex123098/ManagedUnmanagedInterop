#include "stdafx.h"

#include <Synchapi.h>
#include <string>
#include <iostream>

#include "common.h"
#include "MemoryMappedFile.h"
#include "SimpleSettingsProvider.h"
#include "CommandProcessor.h"

using namespace std;

CommandProcessor::CommandProcessor(ISettingsProvider* settingsProvider)
{
	m_hReadEvent = CreateEvent(nullptr, TRUE, FALSE, settingsProvider->Get("readEventName").c_str());
	wstring resultOffset = settingsProvider->Get("resultOffset");
	m_nResultOffset = stoi(resultOffset);
	wstring commandOffset = settingsProvider->Get("commandOffset");
	m_mCommandOffset = stoi(commandOffset);
}

bool CommandProcessor::ProcessNextCommand(MemoryMappedFile* mmf)
{
	byte cmd;
	mmf->Read(m_mCommandOffset, &cmd, sizeof(char));
	LogicCommand command = static_cast<LogicCommand>(cmd);
	
	cout << "Received command: " << LogicCommandsStr[cmd] << endl;

	bool result = cmd != END;
	mmf->Write(m_nResultOffset, &result, sizeof(bool));
	SetEvent(m_hReadEvent);

	return result;
}

CommandProcessor::~CommandProcessor()
{
	CloseHandle(m_hReadEvent);
}
