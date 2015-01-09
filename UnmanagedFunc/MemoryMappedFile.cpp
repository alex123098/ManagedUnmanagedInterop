#include "stdafx.h"
#include "MemoryMappedFile.h"

MemoryMappedFile::MemoryMappedFile(std::wstring sFileName, size_t nFileSize)
{
	m_hFile = CreateFileMapping(INVALID_HANDLE_VALUE, nullptr, PAGE_READWRITE, 0, nFileSize, sFileName.c_str());
	if (!m_hFile) {
		throw std::system_error(GetLastError(), std::iostream_category(), std::string("Failed to create the memory mapped file"));
	}
	m_pMMFView = MapViewOfFile(m_hFile, FILE_MAP_ALL_ACCESS, 0, 0, 0);
	if (m_pMMFView == nullptr) {
		CloseHandle(m_hFile);
		throw std::system_error(GetLastError(), std::iostream_category(), std::string("Failed to map file view"));
	}
}

MemoryMappedFile::~MemoryMappedFile()
{
	UnmapViewOfFile(m_pMMFView);
	CloseHandle(m_hFile);
}

void MemoryMappedFile::Write(unsigned offset, void* data, size_t length)
{
	CopyMemory(reinterpret_cast<char*>(m_pMMFView) + offset, data, length);
}

void MemoryMappedFile::Read(unsigned offset, void* buffer, size_t bufferLength)
{
	CopyMemory(buffer, reinterpret_cast<char*>(m_pMMFView)+offset, bufferLength);
}