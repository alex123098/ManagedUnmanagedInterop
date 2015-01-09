#pragma once

class ISettingsProvider
{
public:
	virtual ~ISettingsProvider() { }

	virtual std::wstring Get(std::string key) = 0;

	ISettingsProvider(const ISettingsProvider&) = delete;
	ISettingsProvider & operator=(const ISettingsProvider&) = delete;
	ISettingsProvider() = default;
};
