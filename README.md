This simple solution contains example of interoperability between two processes:
Managed C# Console application (Supervisor project)
Unmanaged C++ Console application (UnmanagedFunc project)

Processes interact via memory mapped file. Unmanaged C++ process is started within C# process at its startup.
All interaction behaviour is implemented via synchronization events with auto reset.

Solution doesn't depend upon any components except .net framework 4.5