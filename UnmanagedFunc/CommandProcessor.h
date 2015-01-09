#pragma once
#include "stdafx.h"

class MemoryMappedFile;
class ISettingsProvider;

class CommandProcessor
{
public:
	explicit CommandProcessor(ISettingsProvider* settingsProvider);
	~CommandProcessor();
	
	CommandProcessor(const CommandProcessor&) = delete;
	CommandProcessor& operator=(const CommandProcessor&) = delete;

	bool ProcessNextCommand(MemoryMappedFile* mmf);

private:
	HANDLE m_hReadEvent;
	int m_nResultOffset;
	int m_mCommandOffset;
};

