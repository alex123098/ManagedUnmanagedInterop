#pragma once

typedef enum
{
	START = 1,
	PROCESS = 2,
	END = 3
} LogicCommand;

static const char* LogicCommandsStr[] = {
	"INVALID",
	"START",
	"PROCESS",
	"END"
};
