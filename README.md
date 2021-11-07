# RISC-V Simulator

This is a basic implementation of the RV32I spec, written in F#.It has been made for course 02155 Computer Architecture and Engineering at Denmarks Technical University.

## Usage

From the root of the repository, build the simulator with

```bash
dotnet build
```

After building, you can run the simulator with

```bash
dotnet run "tests/task1/addlarge.bin"
```

The simulator takes a single argument specifying the binary file from which to load instructions.

Running the simulator will load the instructions from the provided file and execute them; Writing the program counter, Instruction type, and content of the registers after execution, to stdout for convenience. Finally, the resulting state of the registers is written to a file in the directory of the provided program file with the form `<program-name>_.res`. This can then be compared with `<program-name>.res` to determine if the execution was successful.

## Testing

For added convenience, the repository contains a basic bash script, `run_tests.sh`, that will build the simulator and then run through all the programs provided for tasks 1, 2, and 3, and test each respective result file. Run it with

```bash
./run_tests.sh
```

## Debugging in Visual Studio Code

I've provided the configurations I use for debugging the simulator in VSCode. It's not sure they will work as is on all platforms or machines, See [here](https://code.visualstudio.com/Docs/editor/debugging#_launch-configurations) for more info on debugging in VSCode.

With this configuration, I find myself having a very smooth workflow when developing the simulator; It consists of me running the tests, taking up the first test case that fails. I will change the argument in `launch.json` to point to the failing test case, and begin implementing what I need to successfully execute the instructions. Afterwards, I run the tests again to check if I made any breaking changes, addressing them if it's the case. 

---

**`.vscode/launch.json`**

```json
{
    // Use IntelliSense to learn about possible attributes.
    // Hover to view descriptions of existing attributes.
    // For more information, visit: https://go.microsoft.com/fwlink/?linkid=830387
    "version": "0.2.0",
    "configurations": [
        {
            "name": ".NET Core Launch (console)",
            "type": "coreclr",
            "request": "launch",
            "preLaunchTask": "build",
            "program": "${workspaceFolder}/bin/Debug/net5.0/riscv-simulator.dll",
            "args": [
                "tests/task1/addneg.bin",
            ],
            "cwd": "${workspaceFolder}",
            "stopAtEntry": false,
            "console": "internalConsole"
        }
    ]
}
```

---

**`.vscode/tasks.json`**

```json
{
    // See https://go.microsoft.com/fwlink/?LinkId=733558
    // for the documentation about the tasks.json format
    "version": "2.0.0",
    "tasks": [
        {
            "label": "build",
            "command": "dotnet",
            "type": "shell",
            "args": [
                "build",
                // Ask dotnet build to generate full paths for file names.
                "/property:GenerateFullPaths=true",
                // Do not generate summary otherwise it leads to duplicate errors in Problems panel
                "/consoleloggerparameters:NoSummary"
            ],
            "group": "build",
            "presentation": {
                "reveal": "silent"
            },
            "problemMatcher": "$msCompile"
        }
    ]
}
```