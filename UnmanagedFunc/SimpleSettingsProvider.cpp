#include "stdafx.h"
#include "SimpleSettingsProvider.h"

using namespace std;

SimpleSettingsProvider::SimpleSettingsProvider()
{
	m_simpleSettingsMap.insert(make_pair(string("exitEventName"), wstring(L"LogicExitResetEvent")));
	m_simpleSettingsMap.insert(make_pair(string("writeEventName"), wstring(L"WriteResetEvent")));
	m_simpleSettingsMap.insert(make_pair(string("readEventName"), wstring(L"ReadResetEvent")));
	m_simpleSettingsMap.insert(make_pair(string("commandOffset"), wstring(L"0")));
	m_simpleSettingsMap.insert(make_pair(string("resultOffset"), wstring(L"100")));
	m_simpleSettingsMap.insert(make_pair(string("MMFName"), wstring(L"DemoExchangeMMF")));
}


SimpleSettingsProvider::~SimpleSettingsProvider()
{
}

std::wstring SimpleSettingsProvider::Get(std::string key)
{
	return m_simpleSettingsMap.at(key);
}