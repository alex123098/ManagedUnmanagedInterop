#pragma once
#include <string>

class MemoryMappedFile
{
public:
	explicit MemoryMappedFile(std::wstring sFileName, size_t nFileSize);
	~MemoryMappedFile();

	MemoryMappedFile(const MemoryMappedFile& that) = delete;
	MemoryMappedFile& operator=(const MemoryMappedFile&) = delete;

	void Write(unsigned offset, void* data, size_t length);
	void Read(unsigned offset, void* buffer, size_t bufferLength);

private:
	HANDLE m_hFile;
	void* m_pMMFView;
};

