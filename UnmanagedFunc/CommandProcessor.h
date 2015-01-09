#pragma once
#include "stdafx.h"
#include <memory>

class MemoryMappedFile;
class ISettingsProvider;

class CommandProcessor
{
public:
	HANDLE m_hReadEvent;
	explicit CommandProcessor(ISettingsProvider* settingsProvider);
	bool ProcessNextCommand(MemoryMappedFile* mmf);
	~CommandProcessor();

private:
	int m_nResultOffset;
	int m_mCommandOffset;
};

