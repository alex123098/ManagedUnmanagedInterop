#pragma once

#include <map>

#include "ISettingsProvider.h"

class SimpleSettingsProvider :
	public ISettingsProvider
{
public:
	SimpleSettingsProvider();
	~SimpleSettingsProvider();

	std::wstring Get(std::string key) override;

private:
	std::map<std::string, std::wstring> m_simpleSettingsMap;
};

